using System.ComponentModel;
using System.Windows;

namespace GUI
{
    /// <summary>
    ///     Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        #region members

        private readonly MainWindow _mainWin;

        #endregion

        #region constructors

        public Export(MainWindow mainWin)
        {
            InitializeComponent();
            _mainWin = mainWin;
        }

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _mainWin.ExportWindow = null;
        }

        #region methods

        #endregion
    }
}