using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;

namespace ProcessManager.Model
{
    internal class MyProcess
    {
        #region Properties

        internal PerformanceCounter RamCounter { get; }

        internal PerformanceCounter CpuCounter { get; }

        private string _name;
        private int _id;
        private bool _isActive;
        private int _cpu;
        private int _ram;
        private int _numberOfThreads;
        private int _numberOfModules;
        private string _userName;
        private string _path;
        private DateTime _startTime;
        private string _startDate;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public int Cpu
        {
            get
            {
                return _cpu;
            }
            set
            {
                _cpu = value;
            }
        }

        public int Ram
        {
            get
            {
                return _ram;
            }
            set
            {
                _ram = value;
            }
        }

        public int NumberOfThreads
        {
            get
            {
                return _numberOfThreads;
            }
            set
            {
                _numberOfThreads = value;
            }
        }

        public int NumberOfModules
        {
            get
            {
                return _numberOfModules;

            }
            private set
            {
                _numberOfModules = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }

        public string StartDate
        {
            get
            {
                return _startDate;
            }
            private set
            {
                _startDate = value;
            }
        }
        #endregion

        internal MyProcess(Process process)
        {
            RamCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName);
            CpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);
            _name = process.ProcessName;
            _id = process.Id;
            _isActive = process.Responding;
            _cpu = (int)CpuCounter.NextValue();
            _ram = (int)(RamCounter.NextValue() / 1024 / 1024);
            _numberOfThreads = process.Threads.Count;
            _userName = GetProcessOwner(process.Id);
            try
            {
                _path = process.MainModule.FileName;
            }
            catch (Win32Exception e)
            {
                _path = e.Message;
            }
            try
            {
                StartTime = process.StartTime;
                StartDate = StartTime.ToLongDateString();
            }
            catch (Exception)
            {
                StartTime = new DateTime(0404, 04, 04);
                StartDate = "Access denied";
            }
        }

        private static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = { "", "" };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "No owner";
        }
    }
}
