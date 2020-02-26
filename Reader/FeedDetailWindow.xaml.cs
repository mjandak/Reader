using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Reader
{
    /// <summary>
    /// Interaction logic for FeedDetailWindow.xaml
    /// </summary>
    public partial class FeedDetailWindow
    {
        public FeedDetailWindow()
        {
            InitializeComponent();
        }

        private void Grid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
        }

        private void tbxName_Error(object sender, ValidationErrorEventArgs e)
        {

        }
    }
}
