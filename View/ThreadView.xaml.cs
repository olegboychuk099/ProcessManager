using System.Diagnostics;
using System.Windows;
using ProcessManager.ViewModel;

namespace ProcessManager.View
{
    /// <summary>
    /// Interaction logic for ThreadView.xaml
    /// </summary>
    public partial class ThreadView : Window
    {
        internal ThreadView(Process process)
        {
            InitializeComponent();
            DataContext = new ThreadViewModel(process.Threads);
        }
    }
}
