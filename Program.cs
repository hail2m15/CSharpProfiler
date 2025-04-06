using System;
using SimpleProfiler;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Start profiling session
        PerfProfiler.Instance.BeginSession("Sample Session");

        DoWork();
        DoWork2();

        // Start 3 worker threads
        Thread t1 = new Thread(() => Worker("Worker A", 200));
        Thread t2 = new Thread(() => Worker("Worker B", 300));

        t1.Start();
        t2.Start();

        // Wait for all workers to finish
        t1.Join();
        t2.Join();

        // End profiling session
        PerfProfiler.Instance.EndSession();

        Console.WriteLine("Profiling complete. Check results.json.");
    }

    static void Worker(string name, int delayMs)
    {
        using (new PerfTimer(name))
        {
            // Simulate work
            Thread.Sleep(delayMs);
        }
    }

    static void DoWork()
    {
        using (new PerfTimer("DoWork"))
        {
            Thread.Sleep(500); // Simulate work
        }
    }

    static void DoWork2()
    {
        using (new PerfTimer("DoWork2"))
        {
            Thread.Sleep(1000); // Simulate work
        }
    }
}