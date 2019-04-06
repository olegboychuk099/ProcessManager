using System.Windows.Controls;
using ProcessManager.ViewModel;

namespace ProcessManager.View
{
    /// <summary>
    /// Interaction logic for ProcessView.xaml
    /// </summary>
    public partial class ProcessView : UserControl
    {
        internal ProcessView()
        {
            InitializeComponent();
            DataContext = new ProcessViewModel();
        }


        internal void Close()
        {
            ((ProcessViewModel)DataContext).Close();
        }
    }
}
