using System.Diagnostics;
using System.Windows;
using ProcessManager.ViewModel;

namespace ProcessManager.View
{
    /// <summary>
    /// Interaction logic for ModuleView.xaml
    /// </summary>
    public partial class ModuleView : Window
    {
        internal ModuleView(Process process)
        {
            InitializeComponent();
            DataContext = new ModelsViewModel(process.Modules);
        }
    }
}
