using DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms.SubWindow
{
    /// <summary>
    /// RedirectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RedirectWindow : Window
    {
        public RedirectWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void chkBlack_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                if (chkWhite.IsChecked == true)
                {
                    JsonInterFace.ReDirection.UserType = "0";
                }
                else if (chkBlack.IsChecked == true)
                {
                    JsonInterFace.ReDirection.UserType = "1";
                }
                else
                {
                    JsonInterFace.ReDirection.UserType = "2";
                }
                this.Close();
            }
        }

        private void RedirectParam_Loaded(object sender, RoutedEventArgs e)
        {
            chkBlack.IsChecked = false;
            chkWhite.IsChecked = false;
            chkOther.IsChecked = false;
        }
    }
}
