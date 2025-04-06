using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SimpleProfiler;



namespace SimpleProfiler
{

    public class PerfTimer : IDisposable
    {
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private readonly string _name;
        private readonly string _category;
        private readonly long _startTimestamp;
        private bool _stopped;

        public PerfTimer(string name, string category = "Func")
        {
            _name = name;
            _category = category;
            _startTimestamp = ConvertToMicroseconds(Stopwatch.GetTimestamp());  // Start the stopwatch immediately
        }

        public void Stop()
        {
            if (_stopped) return;

            _stopped = true;

            // Create the ProfileResult to be written
            var result = new ProfileResult
            {
                Name = _name,
                Category = _category,
                Start = _startTimestamp,
                End = ConvertToMicroseconds(Stopwatch.GetTimestamp()),
                ThreadID = (int)GetCurrentThreadId()
            };

            // Record the profiling data
            PerfProfiler.Instance.WriteProfile(result);
        }

        public void Dispose()
        {
            Stop();  // Ensure Stop is called when using 'using' block
        }

        // Convert timestamp ticks to microseconds
        private static long ConvertToMicroseconds(long timestamp)
        {
            return (timestamp * 1_000_000) / Stopwatch.Frequency;
        }
    }
}

