using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProcessManager.ViewModel
{
    internal class ThreadViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProcessThread> _threads;
        
        public ObservableCollection<ProcessThread> Threads
        {
            get => _threads;
            private set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        internal ThreadViewModel(ProcessThreadCollection threads)
        {
            Threads = new ObservableCollection<ProcessThread>(threads.Cast<ProcessThread>());
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
