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
    /// <summary>
    /// 黑白名单操作进度条
    /// </summary>
    public class BWOProgressBarParameterClass : INotifyPropertyChanged
    {
        private string _tipsContent = "正在提交黑名单, 请稍后....";
        private int _submitTotal = 0;
        private int _submitValue = 0;
        private int _completeTotal = 0;
        private int _completeValue = 0;
        private IntPtr _SelfHandle = IntPtr.Zero;
        private string _alertDialogCaption = "超时提示";
        private string _finished;
        private string _noResultErrorDeviceName;

        public BWOProgressBarParameterClass()
        {
        }

        private void LongTimeNotFinishRestart()
        {

        }

        public string TipsContent
        {
            get
            {
                return _tipsContent;
            }

            set
            {
                _tipsContent = value;
                NotifypropertyChanged("TipsContent");
            }
        }

        public int SubmitTotal
        {
            get
            {
                return _submitTotal;
            }

            set
            {
                _submitTotal = value;
                NotifypropertyChanged("SubmitTotal");
            }
        }

        public int SubmitValue
        {
            get
            {
                return _submitValue;
            }

            set
            {
                _submitValue = value;
                NotifypropertyChanged("SubmitValue");
            }
        }

        public int CompleteTotal
        {
            get
            {
                return _completeTotal;
            }

            set
            {
                _completeTotal = value;
                NotifypropertyChanged("CompleteTotal");
            }
        }

        public int CompleteValue
        {
            get
            {
                return _completeValue;
            }

            set
            {
                _completeValue = value;
                NotifypropertyChanged("CompleteValue");
            }
        }

        public IntPtr SelfHandle
        {
            get
            {
                return _SelfHandle;
            }

            set
            {
                _SelfHandle = value;
                NotifypropertyChanged("SelfHandle");
            }
        }

        public string AlertDialogCaption
        {
            get
            {
                return _alertDialogCaption;
            }

            set
            {
                _alertDialogCaption = value;
                NotifypropertyChanged("AlertDialogCaption");
            }
        }

        public string Finished
        {
            get
            {
                return _finished;
            }

            set
            {
                _finished = value;
                NotifypropertyChanged("Finished");
            }
        }

        public string NoResultErrorDeviceName
        {
            get
            {
                return _noResultErrorDeviceName;
            }

            set
            {
                _noResultErrorDeviceName = value;
                NotifypropertyChanged("NoResultErrorDeviceName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifypropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }

    /// <summary>
    /// ProgressBarWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        //黑白名单操作进度条
        public static BWOProgressBarParameterClass BWOProgressBarParameter = null;

        public ProgressBarWindow()
        {
            InitializeComponent();
            BWOProgressBarParameter = new BWOProgressBarParameterClass();
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
                        CloseWin();
                    });
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("特殊名单操作消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Top = SystemParameters.WorkArea.Top + 20;
            this.Left = SystemParameters.WorkArea.Width - this.Width - 50;

            lblTips.DataContext = BWOProgressBarParameter;
            pgWBListSubmitStatusBar.DataContext = BWOProgressBarParameter;
            pgWBListActioStatusBar.DataContext = BWOProgressBarParameter;

            //句柄
            WindowInteropHelper selfHandleHelper = new WindowInteropHelper(this);
            BWOProgressBarParameter.SelfHandle = selfHandleHelper.Handle;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            lblTips.DataContext = null;
            pgWBListSubmitStatusBar.DataContext = null;
            pgWBListActioStatusBar.DataContext = null;
            BWOProgressBarParameter = null;
            System.GC.Collect();
            this.Close();
        }

        //移动
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (btnCancel.IsFocused)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCancel.Focus();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void CloseWin()
        {
            try
            {
                lblTips.DataContext = null;
                pgWBListSubmitStatusBar.DataContext = null;
                pgWBListActioStatusBar.DataContext = null;
                BWOProgressBarParameter = null;
                System.GC.Collect();
                Close();
            }
            catch
            {

            }
            System.GC.Collect();
        }
    }
}
