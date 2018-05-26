using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WarBender {
    class WarbendService {
        static readonly string BasePath = Path.Combine(typeof(WarbendService).Assembly.Location, "..");
        static readonly string ScriptFileName = Path.Combine(BasePath, "WarbendService.py");

        Process process;
        Task stderrTask;
        readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        readonly IProgress<string> status;
        readonly IProgress<int> progress;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        readonly Dictionary<string, Dictionary<string, JContainer>> responseCache =
            new Dictionary<string, Dictionary<string, JContainer>>();

        public ConsoleForm Console { get; }

        public WarbendService(IProgress<string> status = null, IProgress<int> progress = null) {
            this.status = status;
            this.progress = progress;

            ConsoleForm console = null;
            Task.Run(() => {
                Volatile.Write(ref console, new ConsoleForm());
                Application.Run();
            });

            while (Volatile.Read(ref console) == null) { }
            Console = console;
        }

        public void EnsureStarted() {
            if (process == null) {
                Start();
            }
        }

        IEnumerable<string> GetPythonPath() {
            yield return Path.Combine(BasePath, "warbend");
            yield return Path.Combine(BasePath, "requirements");
            yield return Path.Combine(BasePath, "modsys", "native", "Module_system 1.171");
        }

        void Start() {
            var psi = new ProcessStartInfo {
                FileName = Path.Combine(Directory.GetDirectories(BasePath, "pypy2-*").First(), "pypy.exe"),
                Arguments = ScriptFileName,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            };
            psi.EnvironmentVariables["PYTHONPATH"] = string.Join(";", GetPythonPath());

            process = new Process {
                StartInfo = psi,
                EnableRaisingEvents = true,
            };
            process.Exited += Process_Exited;
            process.Start();
            stderrTask = Task.Run(CaptureStderr);
        }

        public void Shutdown() {
            var process = this.process;
            this.process = null;
            if (process?.HasExited != false) {
                return;
            }

            process.Exited -= Process_Exited;
            try {
                process.Kill();
            } catch (Exception) {
            }
        }

        void Process_Exited(object sender, EventArgs e) {
            Shutdown();
            Console.MakeVisible(true, blocking: false);
            MessageBox.Show("WarbendService terminated unexpectedly.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        async Task CaptureStderr() {
            var stderr = process.StandardError;
            StreamWriter writer = null;
            try {
                string line;
                while ((line = await stderr.ReadLineAsync().ConfigureAwait(false)) != null) {
                    Console.WriteLine(line);
                    Trace.WriteLine(line);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "WarbendService error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                writer?.Dispose();
            }
        }

        async Task<JContainer> RequestAsync(string name, JObject args) {
            var process = this.process;
            if (process == null) {
                throw new WarbendShuttingDownException();
            }
            var stdin = process.StandardInput;
            var stdout = process.StandardOutput;

            await Task.Delay(0).ConfigureAwait(false);
            await semaphore.WaitAsync().ConfigureAwait(false);
            try {
                var request = new JObject { [name] = args ?? new JObject() };
                string json = request.ToString(Formatting.None);
                await stdin.WriteLineAsync(json).ConfigureAwait(false);
                for (; ; ) {
                    json = await stdout.ReadLineAsync().ConfigureAwait(false);
                    if (json == null) {
                        throw this.process == null ? new WarbendShuttingDownException() :
                            new WarbendServiceException("WarbendService terminated unexpectedly");
                    }

                    var response = JToken.Parse(json);
                    switch (response) {
                        case var result when result.Type == JTokenType.Null:
                            return null;
                        case JObject result when result.ContainsKey("error"):
                            throw new WarbendOperationException(result.Value<string>("error"));
                        case JContainer result:
                            return result;
                        case JValue result:
                            switch (result.Value) {
                                case string stat:
                                    status?.Report(stat);
                                    continue;
                                case long percent:
                                    progress?.Report((int)percent);
                                    continue;
                                case int percent:
                                    progress?.Report(percent);
                                    continue;
                            }
                            break;
                    }

                    throw new InvalidDataException();
                }
            } catch (WarbendShuttingDownException) {
                throw;
            } catch (WarbendOperationException) {
                throw;
            } catch (Exception) {
                Console.MakeVisible(true);
                throw;
            } finally {
                semaphore.Release();
            }
        }

        async Task<JContainer> CachedRequestAsync(string name, JObject args) {
            var path = args.Value<string>("path");
            if (!responseCache.TryGetValue(path, out var pathCache)) {
                responseCache[path] = pathCache = new Dictionary<string, JContainer>();
            }
            if (!pathCache.TryGetValue(name, out var response)) {
                pathCache[name] = response = await RequestAsync(name, args).ConfigureAwait(false);
            }
            return response;
        }

        public Task LoadAsync(string fileName, string format) =>
        RequestAsync("load", new JObject {
            ["fileName"] = fileName,
            ["format"] = format,
        });

        public Task SaveAsync(string fileName, string format) =>
            RequestAsync("save", new JObject {
                ["fileName"] = fileName,
                ["format"] = format,
            });

        public Task<JContainer> FetchAsync(string path) =>
            RequestAsync("fetch", new JObject {
                ["path"] = path,
            });

        public async Task<JContainer> UpdateAsync(string path, object selector, object value) {
            var resp = await RequestAsync("update", new JObject {
                ["path"] = path,
                ["selector"] = new JValue(selector),
                ["value"] = new JValue(value),
            }).ConfigureAwait(false);
            var affectedPaths = resp.Select(token => token.Value<string>()).ToArray();
            foreach (var p in affectedPaths) {
                responseCache.Remove(p);
            }
            return resp;
        }

        public Task<JContainer> GetArrayInfoAsync(string path) =>
            CachedRequestAsync("getArrayInfo", new JObject {
                ["path"] = path,
            });
    }

    public class WarbendServiceException : Exception {
        public WarbendServiceException() {
        }

        public WarbendServiceException(string message) : base(message) {
        }

        public WarbendServiceException(string message, System.Exception innerException) : base(message, innerException) {
        }

        protected WarbendServiceException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }

    public class WarbendShuttingDownException : WarbendServiceException {
        public WarbendShuttingDownException() {
        }

        public WarbendShuttingDownException(string message) : base(message) {
        }

        public WarbendShuttingDownException(string message, Exception innerException) : base(message, innerException) {
        }

        protected WarbendShuttingDownException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }

    public class WarbendOperationException : WarbendServiceException {
        public WarbendOperationException() {
        }

        public WarbendOperationException(string message) : base(message) {
        }

        public WarbendOperationException(string message, Exception innerException) : base(message, innerException) {
        }

        protected WarbendOperationException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
