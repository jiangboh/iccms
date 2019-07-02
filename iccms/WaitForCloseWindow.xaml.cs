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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms
{
    /// <summary>
    /// WaitForCloseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WaitForCloseWindow : Window
    {
        public WaitForCloseWindow()
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

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //请求获取设备列表
            try
            {
                //主窗口显示
                if (msg == Parameters.WM_TipsWinCloseMessage)
                {
                    //Dispatcher.Invoke(() => { this.Close(); });
                    System.Environment.Exit(System.Environment.ExitCode);
                }
            }
            catch
            {

            }

            return hwnd;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pgbBar.DataContext = MainWindow.ClosingControlPara;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
