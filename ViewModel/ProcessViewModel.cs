using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProcessManager.Model;
using ProcessManager.Tools;
using ProcessManager.View;

namespace ProcessManager.ViewModel
{
    internal class ProcessViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MyProcess> _processes;
        private readonly Thread _updateThread;
        private MyProcess _selectedProcess;
        private RelayCommand<object> _endTaskCommand;
        private RelayCommand<object> _getModuleCommand;
        private RelayCommand<object> _getThreadCommand;
        private RelayCommand<object> _openFileLocationCommand;
        private ModuleView _moduleView;
        private ThreadView _threadView;
        private Visibility _loaderVisibility = Visibility.Hidden;
        public bool IsItemSelected => SelectedProcess != null;

        public MyProcess SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged();
                OnPropertyChanged("IsItemSelected");
            }
        }

        public ObservableCollection<MyProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        internal ProcessViewModel()
        {
            _updateThread = new Thread(UpdateUsers);
            Thread initializationThread = new Thread(InitializeProcesses);
          
            initializationThread.Start();
        }
        

        public RelayCommand<object> EndTaskCommand => _endTaskCommand ?? (_endTaskCommand = new RelayCommand<object>(EndTaskImpl));
        public RelayCommand<object> GetModuleCommand => _getModuleCommand ?? (_getModuleCommand = new RelayCommand<object>(GetModule));
        public RelayCommand<object> GetThreadCommand => _getThreadCommand ?? (_getThreadCommand = new RelayCommand<object>(GetThread));
        public RelayCommand<object> OpenFileLocationCommand => _openFileLocationCommand ?? (_openFileLocationCommand = new RelayCommand<object>(OpenFileLocationImpl));

        private void EndTaskImpl(object o)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                Process process = Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    process.Kill();
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void GetModule(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Process process = Process.GetProcessById(SelectedProcess.Id);
                        _moduleView?.Close();
                        try
                        {
                            _moduleView = new ModuleView(process);
                            _moduleView.Show();
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GetThread(object o)
        {
            try
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        Process process = Process.GetProcessById(SelectedProcess.Id);
                        _threadView?.Close();
                        try
                        {
                            _threadView = new ThreadView(process);
                            _threadView.Show();
                        }
                        catch (Win32Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void OpenFileLocationImpl(object o)
        {
            await Task.Run(() =>
            {
               // Process process = Process.GetProcessById(_selectedProcess.Id);
                Process process = Process.GetProcessById(SelectedProcess.Id);
                try
                {
                    string fullPath = process.MainModule.FileName;
                    Process.Start("explorer.exe", fullPath.Remove(fullPath.LastIndexOf('\\')));
                }
                catch (Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
        }

        private async void UpdateUsers()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        try
                        {
                            lock (Processes)
                            {
                                List<MyProcess> toRemove =
                                    new List<MyProcess>(
                                        Processes.Where(proc => !Tools.ProcessManager.Processes.ContainsKey(proc.Id)));
                                foreach (MyProcess proc in toRemove)
                                {
                                    Processes.Remove(proc);
                                }

                                List<MyProcess> toAdd =
                                    new List<MyProcess>(
                                        Tools.ProcessManager.Processes.Values.Where(proc => !Processes.Contains(proc)));
                                foreach (MyProcess proc in toAdd)
                                {
                                    Processes.Add(proc);
                                }
                            }
                        }
                        catch (NullReferenceException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (ArgumentNullException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        catch (InvalidOperationException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    });
                });
                Thread.Sleep(4000);
            }
        }

        public Visibility LoaderVisibility
        {
            get { return _loaderVisibility; }
            set
            {
                _loaderVisibility = value;
                OnPropertyChanged();
            }
        }

        private async void InitializeProcesses()
        {
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<MyProcess>(Tools.ProcessManager.Processes.Values);
            });
            _updateThread.Start();
            while (Tools.ProcessManager.Processes.Count == 0)
                Thread.Sleep(3000);

        }

        internal void Close()
        {
            _updateThread.Join(3000);
        }

        #region Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
