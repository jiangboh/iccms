using ParameterControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms
{
    /// <summary>
    /// WelComeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WelComeWindow : Window
    {
        public WelComeWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        /// <summary>
        /// 响应Window消息
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //请求获取设备列表
            try
            {
                //获取所有设备响应
                if (msg == Parameters.WM_WelcomeWindowMessage)
                {
                    Dispatcher.Invoke(() => { 
                        SelfClose();
                    });
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("响应Windows消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        //初始化窗口样式
        private void WelcomeWinSetting()
        {
            if (File.Exists(@"Lib/SysImg/Welcome.png"))
            {
                this.Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(@"Lib/SysImg/Welcome.png", UriKind.RelativeOrAbsolute)),
                };
            }
            else
            {
                this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF033965"));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WelcomeWinSetting();
            Parameters.WelcomeWindowHandle = new WindowInteropHelper(this).Handle;
        }

        private void SelfClose()
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Windows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            for(double i=1; i<0.1; i-=0.1)
            {
                this.Opacity = i;
                Thread.Sleep(300);
            }
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_MainWindowShowMessage, 0, 0);
        }
    }
}
