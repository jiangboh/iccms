using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace iccms.SubWindow
{
    /// <summary>
    /// ShowSelectScannerDataDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ShowSelectScannerDataDialog : Window
    {
        public ShowSelectScannerDataDialog()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnClose_Click(sender, e);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtIMSI.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtDTime.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtUserType.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtTMSI.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtIMEI.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtIntensity.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtOperators.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtDomainName.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtDeviceName.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
            txtDes.DataContext = NavigatePages.UEInfoWindow.SelectScannerDataInfo;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }
    }
}
