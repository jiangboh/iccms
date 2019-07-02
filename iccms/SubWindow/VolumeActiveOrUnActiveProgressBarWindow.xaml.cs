using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace iccms.SubWindow
{
    #region 进度条参数
    public class ProgressBarParameterClass : INotifyPropertyChanged
    {
        private int _finishedBarMax = 100;
        private int _finishedBarStep = 0;
        private string _tips;

        public int FinishedBarMax
        {
            get
            {
                return _finishedBarMax;
            }

            set
            {
                _finishedBarMax = value;
                NotifyPropertyChanged("FinishedBarMax");
            }
        }

        public int FinishedBarStep
        {
            get
            {
                return _finishedBarStep;
            }

            set
            {
                _finishedBarStep = value;
                NotifyPropertyChanged("FinishedBarStep");
            }
        }

        public string Tips
        {
            get
            {
                return _tips;
            }

            set
            {
                _tips = value;
                NotifyPropertyChanged("Tips");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }
    #endregion

    /// <summary>
    /// VolumeActiveOrUnActiveProgressBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VolumeActiveOrUnActiveProgressBarWindow : Window
    {
        public static ProgressBarParameterClass ProgressBarParameter = null;

        public VolumeActiveOrUnActiveProgressBarWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (ProgressBarParameter == null)
            {
                ProgressBarParameter = new ProgressBarParameterClass();
            }
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
                //查询设备
                if (msg == Parameters.WM_ProgressBarWindowClose)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Exit();
                    });
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(lblTips.Content + "操作消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.pgbVolumeActiveAPBar.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pgbVolumeActiveAPBar.Focus();
        }

        private void Window_Loaeded(object sender, RoutedEventArgs e)
        {
            lblTips.DataContext = ProgressBarParameter;
            pgbVolumeActiveAPBar.DataContext = ProgressBarParameter;

            //句柄
            WindowInteropHelper selfHandleHelper = new WindowInteropHelper(this);
            Parameters.VolumeActiveWinHandle = selfHandleHelper.Handle;
        }

        private void Exit()
        {
            lblTips.DataContext = null;
            pgbVolumeActiveAPBar.DataContext = null;
            ProgressBarParameter = null;
            System.GC.Collect();
            this.Close();
        }
    }
}
