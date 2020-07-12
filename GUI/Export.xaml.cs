using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace GUI
{
    /// <summary>
    /// Interaction logic for Export.xaml
    /// </summary>
    public partial class Export : Window
    {
        #region members

        private MainWindow _mainWin;

        #endregion

        #region constructors

        public Export(MainWindow mainWin)
        {
            InitializeComponent();
            _mainWin = mainWin;
        }

        #endregion

        #region methods



        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _mainWin._exportWindow = null;
        }
    }
}
