using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProcessManager.Model;

namespace ProcessManager.Tools
{
    internal static class ProcessManager
    {
        private static readonly Thread UpdateThread;
        private static readonly Thread UpdateEntriesThread;

        internal static Dictionary<int, MyProcess> Processes { get; set; }

        static ProcessManager()
        {
            Processes = new Dictionary<int, MyProcess>();
            UpdateEntriesThread = new Thread(UpdateEntries);
            UpdateThread = new Thread(UpdateDb);
            UpdateThread.Start();
            UpdateEntriesThread.Start();
        }

        internal static void Close()
        {
            UpdateThread.Join(3000);
            UpdateEntriesThread.Join(1500);
        }

        private static async void UpdateDb()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        List<Process> processes = Process.GetProcesses().ToList();
                        IEnumerable<int> keys = Processes.Keys.ToList()
                            .Where(id => processes.All(proc => proc.Id != id));
                        foreach (int key in keys)
                        {
                            Processes.Remove(key);
                        }

                        foreach (Process proc in processes)
                        {
                            if (!Processes.ContainsKey(proc.Id))
                            {
                                try
                                {
                                    Processes[proc.Id] = new MyProcess(proc);
                                }
                                catch (InvalidOperationException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (NullReferenceException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                });
                Thread.Sleep(5000);
            }
        }

        private static async void UpdateEntries()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        foreach (int id in Processes.Keys.ToList())
                        {
                            Process pr;
                            try
                            {
                                pr = Process.GetProcessById(id);
                            }
                            catch (ArgumentException)
                            {
                                Processes.Remove(id);
                                continue;
                            }

                            Processes[id].Cpu = (int)Processes[id].CpuCounter.NextValue();
                            Processes[id].Ram = (int)(Processes[id].RamCounter.NextValue() / 1024 / 1024);
                            Processes[id].NumberOfThreads = pr.Threads.Count;
                        }
                    }
                });
                Thread.Sleep(2000);
            }
        }
    }
}
