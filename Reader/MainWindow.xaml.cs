using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWndVM();
            MsgBus<ItemOpenEvent>.Instance.MessageRecieved += feedItemOpen;
            MsgBus<FeedVM.FeedEditEvent>.Instance.MessageRecieved += feedEdit;
        }

        private void feedEdit(FeedVM.FeedEditEvent e)
        {
            var w = new FeedDetailWindow();
            w.DataContext = e.Feed;
            w.ShowDialog();
        }

        private void feedItemOpen(ItemOpenEvent e)
        {
            try
            {
                Process.Start(ConfigurationManager.AppSettings["browserpath"], e.Link);
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening in external browser.", ex);
            }
        }

        private void menuReresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuEditFeed_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menuDeleteFeed_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lblFeedTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRefreshAllFeeds_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewFeed_Click(object sender, RoutedEventArgs e)
        {
            var w = new FeedDetailWindow();
            w.DataContext = new FeedVM(new DAL.Feed());
            w.ShowDialog();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ((MainWndVM)DataContext).MultipleFeedsSelectioON = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        }
    }
}
