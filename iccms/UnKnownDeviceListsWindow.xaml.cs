using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms
{
    #region  提示器属性
    public class ElemetAttribute : INotifyPropertyChanged
    {
        private double _zero;
        private double _startColor;
        private double _secondColor;
        private double _lastColor;
        private double _elementOpacity;

        private double _elementWidth;
        private double _elementHeight;

        private double _x;
        private double _y;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public double ElementWidth
        {
            get
            {
                return _elementWidth;
            }

            set
            {
                _elementWidth = value;
                NotifyPropertyChanged("ElementWidth");
            }
        }

        public double ElementHeight
        {
            get
            {
                return _elementHeight;
            }

            set
            {
                _elementHeight = value;
                NotifyPropertyChanged("ElementHeight");
            }
        }

        public double Zero
        {
            get
            {
                return _zero;
            }

            set
            {
                _zero = value;
                NotifyPropertyChanged("Zero");
            }
        }

        public double StartColor
        {
            get
            {
                return _startColor;
            }

            set
            {
                _startColor = value;
                NotifyPropertyChanged("StartColor");
            }
        }

        public double SecondColor
        {
            get
            {
                return _secondColor;
            }

            set
            {
                _secondColor = value;
                NotifyPropertyChanged("SecondColor");
            }
        }

        public double LastColor
        {
            get
            {
                return _lastColor;
            }

            set
            {
                _lastColor = value;
                NotifyPropertyChanged("LastColor");
            }
        }

        public double ElementOpacity
        {
            get
            {
                return _elementOpacity;
            }

            set
            {
                _elementOpacity = value;
                NotifyPropertyChanged("ElementOpacity");
            }
        }

        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
                NotifyPropertyChanged("X");
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
                NotifyPropertyChanged("Y");
            }
        }
    }
    #endregion

    /// <summary>
    /// UnKnownDeviceListsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnKnownDeviceListsWindow : Window
    {
        private static Thread ChangeDraw = null;
        public static ElemetAttribute ElementAttributeParameter = new ElemetAttribute();

        public UnKnownDeviceListsWindow()
        {
            try
            {
                InitializeComponent();
                this.Owner = Application.Current.MainWindow;
                if (ChangeDraw == null)
                {
                    ChangeDraw = new Thread(new ThreadStart(ChangeDrawStyle));
                    ChangeDraw.Start();
                }
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "未知设备监听器初始化...", "初始化", "成功");
            }
            catch
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "未知设备监听器初始化...", "初始化", "失败");
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

            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("未知设备响应消息异常", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        #region 提示效果
        private void ChangeDrawStyle()
        {
            bool Enable = true;
            while (true)
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (this.Visibility != Visibility.Visible)
                    {
                        Thread.Sleep(1000);
                        Enable = false;
                    }
                    else
                    {
                        Enable = true;
                    }
                });

                if (!Enable)
                {
                    continue;
                }

                ElementAttributeParameter.ElementOpacity = 0.6;
                ElementAttributeParameter.ElementWidth = 9.375;
                ElementAttributeParameter.ElementHeight = 9.375;
                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.05;
                ElementAttributeParameter.SecondColor = 0.1;
                ElementAttributeParameter.LastColor = 0.1;
                Thread.Sleep(700);

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.1;
                ElementAttributeParameter.SecondColor = 0.25;
                ElementAttributeParameter.LastColor = 0.25;
                Thread.Sleep(500);

                ElementAttributeParameter.ElementWidth = 18.75;
                ElementAttributeParameter.ElementHeight = 18.75;

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.1;
                ElementAttributeParameter.SecondColor = 0.25;
                ElementAttributeParameter.LastColor = 0.25;
                Thread.Sleep(500);

                ElementAttributeParameter.ElementWidth += 18.75;
                ElementAttributeParameter.ElementHeight += 18.75;

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.2;
                ElementAttributeParameter.SecondColor = 0.45;
                ElementAttributeParameter.LastColor = 0.45;
                Thread.Sleep(500);

                ElementAttributeParameter.ElementWidth += 18.75;
                ElementAttributeParameter.ElementHeight += 18.75;

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.3;
                ElementAttributeParameter.SecondColor = 0.6;
                ElementAttributeParameter.LastColor = 0.46;
                Thread.Sleep(500);

                ElementAttributeParameter.ElementWidth += 18.75;
                ElementAttributeParameter.ElementHeight += 18.75;

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.4;
                ElementAttributeParameter.SecondColor = 0.75;
                ElementAttributeParameter.LastColor = 0.75;
                Thread.Sleep(500);

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 0.5;
                ElementAttributeParameter.SecondColor = 0.85;
                ElementAttributeParameter.LastColor = 0.85;
                Thread.Sleep(500);

                ElementAttributeParameter.Zero = 0;
                ElementAttributeParameter.StartColor = 1;
                ElementAttributeParameter.SecondColor = 1;
                ElementAttributeParameter.LastColor = 1;
                Thread.Sleep(500);

                for (double i = 0.1; i < 0.6; i++)
                {
                    ElementAttributeParameter.ElementOpacity -= i;
                    Thread.Sleep(300);
                }
            }
        }
        #endregion

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChangedElement.DataContext = ElementAttributeParameter;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SubWindow.UnKnownDeviceListsControlWindow UnKnownDeviceListsControlWin = new SubWindow.UnKnownDeviceListsControlWindow();
            UnKnownDeviceListsControlWin.ShowDialog();
        }

        private void Window_Close(object sender, EventArgs e)
        {
            try
            {
                if (ChangeDraw != null)
                {
                    if (ChangeDraw.ThreadState == ThreadState.Running || ChangeDraw.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        ChangeDraw.Abort();
                        ChangeDraw.Join();
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("关闭提示器", ex.Message, ex.StackTrace);
            }
        }
    }
}
