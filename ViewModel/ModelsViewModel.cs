using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProcessManager.ViewModel
{

    internal class ModelsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProcessModule> _modules;

        public ObservableCollection<ProcessModule> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }
        

        internal ModelsViewModel(ProcessModuleCollection modules)
        {
            Modules = new ObservableCollection<ProcessModule>(modules.Cast<ProcessModule>());
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
