using ParameterControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace iccms.NavigatePages
{
    /// <summary>
    /// DeviceAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddDeviceWindow : Window
    {
        private Dictionary<string, Uri> DeviceInfoWindow = new Dictionary<string, Uri>();

        public AddDeviceWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            DeviceInfoWindow.Add("DeviceInfoSettingModel", new Uri("NavigatePages/DeviceInfoSettingModel.xaml", UriKind.Relative));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = new Language_EN.Device_managerWindow();
            }
            else
            {
                this.DataContext = new Language_CN.Device_managerWindow();
            }

            FrmDeviceInfo.Navigate(DeviceInfoWindow["DeviceInfoSettingModel"]);

            //详细信息显位方位调整
            if (this.Top + this.Height > SystemParameters.WorkArea.Size.Height)
            {
                double offset = this.Height - (SystemParameters.WorkArea.Size.Height - this.Top);
                this.Top = this.Top - offset - 50;
            }

            this.Left = Parameters.UserMousePosition.X + 30;
        }


        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try
                {
                    if (btnCancel.IsFocused)
                    {
                        this.DragMove();
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCancel.Focus();
        }

        /// <summary>
        /// ESC退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
