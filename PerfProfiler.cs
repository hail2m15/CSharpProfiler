using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace SimpleProfiler
{
    public class ProfileResult
    {
        public string Name { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public int ThreadID { get; set; }
        public string Category { get; set; }
    }
    public class PerfProfiler
    {
        private static readonly Lazy<PerfProfiler> _instance = new Lazy<PerfProfiler>(() => new PerfProfiler());

        private string _sessionName = "None";
        private StreamWriter _streamWriter;
        private int _profileCount = 0;
        private readonly object _lock = new object();
        private bool _activeSession = false;

        public static PerfProfiler Instance => _instance.Value;

        private PerfProfiler() { }

        public void BeginSession(string name, string filePath = "results.json")
        {
            lock (_lock)
            {
                if (_activeSession) return;

                _sessionName = name;
                _streamWriter = new StreamWriter(filePath);
                _activeSession = true;
                _profileCount = 0;

                WriteHeader();
            }
        }

        public void EndSession()
        {
            lock (_lock)
            {
                if (!_activeSession) return;

                WriteFooter();
                _streamWriter.Dispose();
                _activeSession = false;
            }
        }

        public void WriteProfile(ProfileResult result)
        {
            lock (_lock)
            {
                if (_profileCount++ > 0)
                    _streamWriter.Write(",");

                // Replace double quotes with single quotes in name and category
                string name = result.Name.Replace("\"", "'");
                string category = result.Category.Replace("\"", "'");

                long duration = result.End - result.Start;

                // Manually format JSON like C++ version
                string json = $"{{" +
                              $"\"cat\":\"{category}\"," +
                              $"\"dur\":{duration}," +
                              $"\"name\":\"{name}\"," +
                              $"\"ph\":\"X\"," +
                              $"\"pid\":0," +
                              $"\"tid\":{result.ThreadID}," +
                              $"\"ts\":{result.Start}" +
                              $"}}";

                _streamWriter.Write(json);
                _streamWriter.Flush();
            }
        }

        private void WriteHeader()
        {
            _streamWriter.Write("{\"otherData\": {},\"traceEvents\":[");
            _streamWriter.Flush();
        }

        private void WriteFooter()
        {
            _streamWriter.Write("]}");
            _streamWriter.Flush();
        }
    }
}
