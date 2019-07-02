using ParameterControl;
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

namespace iccms.NavigatePages
{
    /// <summary>
    /// Device_manager.xaml 的交互逻辑
    /// </summary>
    public partial class Device_manager : Window
    {
        private object Device_managerLanguageClass = null;
        private Dictionary<string, Uri> AllWindow = new Dictionary<string, Uri>();
        public Device_manager()
        {
            InitializeComponent();
            AllWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
            AllWindow.Add("DeviceInfoSettingModel", new Uri("NavigatePages/DeviceInfoSettingModel.xaml", UriKind.Relative));
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                Device_managerLanguageClass = new Language_CN.Device_managerWindow();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                Device_managerLanguageClass = new Language_EN.Device_managerWindow();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //载入设备列表
            DeviceListFrm.Navigate(AllWindow["DeviceListWindow"]);
            DeviceInfoSettingFram.Navigate(AllWindow["DeviceInfoSettingModel"]);

            //中/英文初始化
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                this.DataContext = (Language_CN.Device_managerWindow)Device_managerLanguageClass;
            }
            else
            {
                this.DataContext = (Language_EN.Device_managerWindow)Device_managerLanguageClass;
            }

            Parameters.SendMessage(Parameters.DeviceTreeViewHandle, Parameters.WM_DeviceTreeViewListing, 0, 0);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void tabControlSeting_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void DeviceListFrm_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void tabControlSeting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }
    }
}
