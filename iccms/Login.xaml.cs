using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace iccms
{
    /// <summary>
    /// 远程序服务器信息
    /// </summary>
    public class RemoteLocalHostParameters : INotifyPropertyChanged
    {
        private string _NetWorkSegmentA;
        private string _NetWorkSegmentB;
        private string _LoaclHostSegmentA;
        private string _LoaclHostSegmentB;
        private string _RemoteServerPort;

        public string NetWorkSegmentA
        {
            get
            {
                return _NetWorkSegmentA;
            }

            set
            {
                _NetWorkSegmentA = value;
                NotifyPropertyChanged("NetWorkSegmentA");
            }
        }

        public string NetWorkSegmentB
        {
            get
            {
                return _NetWorkSegmentB;
            }

            set
            {
                _NetWorkSegmentB = value;
                NotifyPropertyChanged("NetWorkSegmentB");
            }
        }

        public string LoaclHostSegmentA
        {
            get
            {
                return _LoaclHostSegmentA;
            }

            set
            {
                _LoaclHostSegmentA = value;
                NotifyPropertyChanged("LoaclHostSegmentA");
            }
        }

        public string LoaclHostSegmentB
        {
            get
            {
                return _LoaclHostSegmentB;
            }

            set
            {
                _LoaclHostSegmentB = value;
                NotifyPropertyChanged("LoaclHostSegmentB");
            }
        }

        public string RemoteServerPort
        {
            get
            {
                return _RemoteServerPort;
            }

            set
            {
                _RemoteServerPort = value;
                NotifyPropertyChanged("RemoteServerPort");
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

    public class RotateParameter : INotifyPropertyChanged
    {
        private double _Angle = 0;
        private double _CenterX = 0;
        private double _CenterY = 0;
        private double _ReverseAngle = 0;

        public double Angle
        {
            get
            {
                return _Angle;
            }

            set
            {
                _Angle = value;
                NotifyPropertyChanged("Angle");
            }
        }

        public double CenterX
        {
            get
            {
                return _CenterX;
            }

            set
            {
                _CenterX = value;
                NotifyPropertyChanged("CenterX");
            }
        }

        public double CenterY
        {
            get
            {
                return _CenterY;
            }

            set
            {
                _CenterY = value;
                NotifyPropertyChanged("CenterY");
            }
        }

        public double ReverseAngle
        {
            get
            {
                return _ReverseAngle;
            }

            set
            {
                _ReverseAngle = value;
                NotifyPropertyChanged("ReverseAngle");
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

    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        #region 进度条信息
        private class ProgressBarInfoClass : INotifyPropertyChanged
        {
            private Visibility _progressBarVisible = Visibility.Visible;
            private int _maxValue;
            private int _stepValue;

            public Visibility ProgressBarVisible
            {
                get
                {
                    return _progressBarVisible;
                }

                set
                {
                    _progressBarVisible = value;
                    NotifyPropertyChanged("ProgressBarVisible");
                }
            }

            public int MaxValue
            {
                get
                {
                    return _maxValue;
                }

                set
                {
                    _maxValue = value;
                    NotifyPropertyChanged("MaxValue");
                }
            }

            public int StepValue
            {
                get
                {
                    return _stepValue;
                }

                set
                {
                    _stepValue = value;
                    NotifyPropertyChanged("StepValue");
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

        private Parameters AppConfigurationPara = new Parameters();
        private object loginLanguageClass = null;
        private object loginWindowEventClass = null;
        private static int WaitTime = 0;
        private static bool Run = true;
        private string UserName = string.Empty;
        private string PassWord = string.Empty;
        private string ServerHost = string.Empty;
        private bool DefaultInfo = false;
        private ProgressBarInfoClass ProgressBarInfo = new ProgressBarInfoClass();

        private Thread ProgressBarStartThread = null;
        private System.Timers.Timer ActiveMainWindowTimer = null;

        private static RemoteLocalHostParameters RemoteLocalHost = null;
        private static RotateParameter GraphicRotatePara = null;
        private System.Timers.Timer RenderGraphicRoundTimer = null;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out System.Windows.Point pt);

        public Login()
        {
            InitializeComponent();
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                loginLanguageClass = new Language_CN.LoginWindow();
                loginWindowEventClass = new Language_CN.LoginWindowEventContents();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                loginLanguageClass = new Language_EN.LoginWindow();
                loginWindowEventClass = new Language_EN.LoginWindowEventContents();
            }

            //添加元素
            DrawStyle(ref LoginWinCanvas);
            DotLine(ref LoginWinCanvasDotLine);

            if (ActiveMainWindowTimer == null)
            {
                ActiveMainWindowTimer = new System.Timers.Timer();
                ActiveMainWindowTimer.Interval = 1000;
                ActiveMainWindowTimer.Elapsed += ActiveMainWindowTimer_Elapsed;
            }

            if (RemoteLocalHost == null)
            {
                RemoteLocalHost = new RemoteLocalHostParameters();
            }

            if (GraphicRotatePara == null)
            {
                GraphicRotatePara = new RotateParameter();
            }

            if (RenderGraphicRoundTimer == null)
            {
                RenderGraphicRoundTimer = new System.Timers.Timer();
                RenderGraphicRoundTimer.Interval = 30;
                RenderGraphicRoundTimer.Elapsed += RenderGraphicRoundTimer_Elapsed;
            }

            this.KeyDown += Login_KeyDown;

            if (ProgressBarStartThread == null)
            {
                ProgressBarStartThread = new Thread(new ThreadStart(ProgressBarStart));
                ProgressBarStartThread.IsBackground = true;
            }
        }

        private void RenderGraphicRoundTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GraphicRound(0.1);
        }

        /// <summary>
        /// 定时器检测登录请求状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActiveMainWindowTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (JsonInterFace.LoginUserInfo.Count > 0)
                {
                    if (JsonInterFace.LoginUserInfo[0].LoginAccess.Equals("0"))
                    {
                        if (Run)
                        {
                            LoadMain();
                        }
                        return;
                    }
                    else
                    {
                        ShowEnventInfo(JsonInterFace.LoginUserInfo[0].Information + "(" + JsonInterFace.LoginUserInfo[0].LoginAccess + ")");
                        return;
                    }
                }
                else if (WaitTime < Parameters.LoginTimeOut)
                {
                    WaitTime += 1;
                }
                else
                {
                    ShowEnventInfo(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("登录检测器", ex.Message, ex.StackTrace);
            }
        }

        private void LoadMain()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Run = false;
                    ActiveMainWindowTimer.Stop();
                    ActiveMainWindowTimer.AutoReset = false;
                    ProgressBarInfo.ProgressBarVisible = Visibility.Hidden;
                    prgWaitLogin.DataContext = null;

                    if (ProgressBarStartThread.ThreadState == ThreadState.Running || ProgressBarStartThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        ProgressBarStartThread.Abort();
                        ProgressBarStartThread.Join();
                        ProgressBarStartThread = null;
                    }
                });
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("加载主界面", ex.Message, ex.StackTrace);
                MessageBox.Show("登录失败(" + ex.Message + ")", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                ActiveMainWindow(UserName, ServerHost, PassWord, DefaultInfo);
            }
        }

        private void ShowEnventInfo(string revMessage)
        {
            try
            {
                ProgressBarInfo.StepValue = 0;
                Run = false;
                WaitTime = 0;
                JsonInterFace.LoginUserInfo.Clear();
                ActiveMainWindowTimer.Stop();

                if (ProgressBarStartThread.ThreadState == ThreadState.Running || ProgressBarStartThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    ProgressBarStartThread.Abort();
                    ProgressBarStartThread.Join();
                    ProgressBarStartThread = null;
                }

                if (revMessage == "" || revMessage == null)
                {
                    MessageBox.Show("服务器登录无响应,登录失败！", "登录", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
                else if (revMessage == "NULL")
                {
                    MessageBox.Show("服务器无法连接,登录失败,网络是否正常或服务端地址是否正确！", "登录", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
                else
                {
                    MessageBox.Show(revMessage + "!", "登录", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }

                ProgressBarInfo.ProgressBarVisible = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
            Dispatcher.Invoke(() =>
            {
                prgWaitLogin.DataContext = null;
            });
        }

        /// <summary>
        /// 按ESC键退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void CansleActionButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //中/英文初始化
                if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
                {
                    this.DataContext = (Language_CN.LoginWindow)loginLanguageClass;
                    lblUserName.DataContext = (Language_CN.LoginWindow)loginLanguageClass;
                    lblPassWord.DataContext = (Language_CN.LoginWindow)loginLanguageClass;
                    lblServer.DataContext = (Language_CN.LoginWindow)loginLanguageClass;
                    btnLoginAction.DataContext = (Language_CN.LoginWindow)loginLanguageClass;
                }
                else
                {
                    this.DataContext = (Language_EN.LoginWindow)loginLanguageClass;
                    lblUserName.DataContext = (Language_EN.LoginWindow)loginLanguageClass;
                    lblPassWord.DataContext = (Language_EN.LoginWindow)loginLanguageClass;
                    lblServer.DataContext = (Language_EN.LoginWindow)loginLanguageClass;
                    btnLoginAction.DataContext = (Language_EN.LoginWindow)loginLanguageClass;
                }

                //默认显示登录信息(是否记住用户名)
                if (Parameters.LoginDefault)
                {
                    txtUserName.Text = Parameters.LoginUserName;
                }

                txtServerHostA.DataContext = RemoteLocalHost;
                txtServerHostB.DataContext = RemoteLocalHost;
                txtServerNetWorkSegmentA.DataContext = RemoteLocalHost;
                txtServerNetWorkSegmentB.DataContext = RemoteLocalHost;
                txtServerPort.DataContext = RemoteLocalHost;
                LoginWinCanvas.DataContext = GraphicRotatePara;
                LoginWinCanvasDotLine.DataContext = GraphicRotatePara;

                //初始化服务器IP地址
                InitRemoteHostParameters();

                chkRememberLoginInfo.IsChecked = Parameters.LoginDefault;

                ProgressBarInfo.MaxValue = 100;
                ProgressBarInfo.StepValue = 0;
                prgWaitLogin.DataContext = ProgressBarInfo;

                //窗口句柄
                Parameters.WinHandle = new WindowInteropHelper(this).Handle;

                GraphicRotatePara.CenterX = LoginWinCanvas.Width / 2;
                GraphicRotatePara.CenterY = LoginWinCanvas.Height / 2;
                RenderGraphicRoundTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void InitRemoteHostParameters()
        {
            try
            {
                if (Parameters.LoginServer != null && Parameters.LoginServer != "")
                {
                    string[] IPInfo = Parameters.LoginServer.Split(new char[] { '.' });
                    RemoteLocalHost.NetWorkSegmentA = IPInfo[0];
                    RemoteLocalHost.NetWorkSegmentB = IPInfo[1];
                    RemoteLocalHost.LoaclHostSegmentA = IPInfo[2];
                    string[] PortInfo = (IPInfo[3]).Split(new char[] { ':' });
                    RemoteLocalHost.LoaclHostSegmentB = PortInfo[0];
                    RemoteLocalHost.RemoteServerPort = PortInfo[1];
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("初始化服务器地址", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoginAction_Click(object sender, RoutedEventArgs e)
        {
            LoginStart(txtServerNetWorkSegmentA, txtServerNetWorkSegmentB, txtServerHostA, txtServerHostB, txtServerPort);
        }

        private void LoginWin_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!txtUserName.IsFocused
                    && !txtPassWd.IsFocused
                    && !txtServerHostA.IsFocused
                    && !txtServerHostB.IsFocused
                    && !txtServerNetWorkSegmentA.IsFocused
                    && !txtServerNetWorkSegmentB.IsFocused
                    && !txtServerPort.IsFocused
                    )
                {
                    this.DragMove();
                }
            }
        }

        private void LoginWin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnLoginAction.Focus();
        }

        private void LoginWin_Closed(object sender, EventArgs e)
        {
            try
            {
                if (Parameters.ConfigType != "AccessLogin")
                {
                    if (NetWorkClient.ControllerServer != null)
                    {
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            NetWorkClient.ControllerServer.Close();
                        }
                    }
                }

                if (!Parameters.Logined)
                {
                    System.Environment.Exit(System.Environment.ExitCode);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("登录窗口关闭", ex.Message, ex.StackTrace);
            }
        }

        private void ActiveMainWindow(string UserName, string ServerHost, string PassWord, bool DefaultInfo)
        {
            if (Parameters.ConfigType != "AccessLogin")
            {
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        MainWindow MainWin = new MainWindow();
                        Application.Current.MainWindow = MainWin;
                        if (JsonInterFace.LoginUserInfo.Count > 0)
                        {
                            JsonInterFace.LoginUserInfo[0].LoginUser = UserName;
                            JsonInterFace.LoginUserInfo[0].LoginHost = ServerHost.Split(new char[] { ':' })[0];
                            JsonInterFace.LoginUserInfo[0].LoginPort = ServerHost.Split(new char[] { ':' })[1];
                            JsonInterFace.LoginUserInfo[0].LoginTime = DateTime.Now.ToString();
                        }
                        else
                        {
                            Parameters.PrintfLogsExtended("LoginUserInfo List Error!");
                        }
                        Parameters.Logined = true;
                        Parameters.ConfigType = "AccessLogin";

                        Parameters.WritePrivateProfileString("Login", "UserName", new DesEncrypt().Encrypt(UserName, new DefineCode().Code()), Parameters.INIFile);
                        try
                        {
                            new DesEncrypt().UnEncrypt(PassWord, new DefineCode().Code());
                            Parameters.WritePrivateProfileString("Login", "PassWord", PassWord, Parameters.INIFile);
                        }
                        catch
                        {
                            Parameters.WritePrivateProfileString("Login", "PassWord", new DesEncrypt().Encrypt(PassWord, new DefineCode().Code()), Parameters.INIFile);
                        }
                        Parameters.WritePrivateProfileString("Login", "Server", new DesEncrypt().Encrypt(ServerHost, new DefineCode().Code()), Parameters.INIFile);

                        this.Close();
                        MainWin.Show();
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended("Starting Main Window Error！", ex.Message, ex.StackTrace);
                    }
                }));
            }
        }

        private bool GetLoginAccess()
        {
            bool result = false;
            Int32 delayTime = 1000;
            Int32 waitTime = 0;

            try
            {
                //连接服务器
                string[] serverInfo = ServerHost.Split(new char[] { ':' });
                Parameters.TcpServerHost = serverInfo[0];
                Parameters.TcpServerPort = Convert.ToInt32(serverInfo[1]);
                if ((Parameters.TcpServerHost == "" || Parameters.TcpServerHost == null) && Parameters.TcpServerPort > 0)
                {
                    result = false;
                    Parameters.PrintfLogsExtended("服务器信息不正确，登录失败！");
                    MessageBox.Show("服务器信息不正确，登录失败！", "登录", MessageBoxButton.OK, MessageBoxImage.Error);
                    return result;
                }

                if (NetWorkClient.ControllerServer != null)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Close();
                    }
                }
                new NetWorkClient();
            }
            catch (Exception ex)
            {
                result = false;
                Parameters.PrintfLogsExtended("登录失败", ex.Message, ex.StackTrace);
                MessageBox.Show("登录失败，（" + ex.Message + "）", "登录", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }

            new Thread(() =>
                {
                    while (Run)
                    {
                        try
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                //发送登录请求
                                try
                                {
                                    string josnStr = JsonInterFace.LoginRequest(UserName, new DesEncrypt().UnEncrypt(PassWord, new DefineCode().Code()));
                                    NetWorkClient.ControllerServer.Send(josnStr);
                                }
                                catch
                                {
                                    string josnStr = JsonInterFace.LoginRequest(UserName, PassWord);
                                    NetWorkClient.ControllerServer.Send(josnStr);
                                }

                                while (true)
                                {
                                    if (JsonInterFace.LoginUserInfo.Count > 0)
                                    {
                                        if (JsonInterFace.LoginUserInfo[0].LoginAccess != "")
                                        {
                                            if (JsonInterFace.LoginUserInfo[0].LoginAccess == "0")
                                            {
                                                //登录成功
                                                SendUserManagerRequest();
                                                break;
                                            }
                                        }
                                    }

                                    //登录不在功，5秒登录失败
                                    if (waitTime.Equals(delayTime * Parameters.LoginTimeOut))
                                    {
                                        break;
                                    };

                                    waitTime += delayTime;

                                    Thread.Sleep(delayTime);
                                }
                                break;
                            }
                            else
                            {
                                //5秒登录失败
                                if (waitTime.Equals(delayTime * Parameters.LoginTimeOut))
                                {
                                    JsonInterFace.LoginUserInfo.Clear();
                                    LoginedInfo loginedInfo = new LoginedInfo();
                                    loginedInfo.LoginAccess = "TimeOut";
                                    loginedInfo.Information = "管理系统登录无响应，已超时！";
                                    JsonInterFace.LoginUserInfo.Add(loginedInfo);
                                    break;
                                };

                                waitTime += delayTime;
                                Thread.Sleep(delayTime);
                            }
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            Parameters.PrintfLogsExtended("智能通讯管控管理系统登录失败", ex.Message, ex.StackTrace);
                            break;
                        }
                    }
                }).Start();

            return result;
        }

        private void SendUserManagerRequest()
        {
            try
            {
                //发送用户管理请求
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_privilege_request());      //请求权限
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_role_Request());           //请求用户组
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_user_group_request());     //请求用户-用户组
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_group_privilege_request());//请求用户组权限
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求权限:", "Connected: Failed!");
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("用户管理请求失败", ex.Message, ex.StackTrace);
            }
        }

        private void ProgressBarStart()
        {
            byte delayTime = 5;
            try
            {
                ProgressBarInfo.MaxValue = Parameters.LoginTimeOut * 1000 / delayTime;
                ProgressBarInfo.ProgressBarVisible = Visibility.Visible;
                ActiveMainWindowTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }

            while (Run)
            {
                try
                {
                    for (int i = 0; i < Parameters.LoginTimeOut * 1000 / delayTime; i++)
                    {
                        if (Run && (i + 1) < ProgressBarInfo.MaxValue)
                        {
                            ProgressBarInfo.StepValue = i + 1;
                            Thread.Sleep(delayTime);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended("登录进度内部故障！", Ex.Message, Ex.StackTrace);
                    break;
                }
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //请求登录
        private void LoginStart(TextBox ServerNetWorkSegmentA, TextBox ServerNetWorkSegmentB, TextBox ServerHostA, TextBox ServerHostB, TextBox ServerPort)
        {
            try
            {
                prgWaitLogin.DataContext = ProgressBarInfo;
                ActiveMainWindowTimer.AutoReset = true;

                if (txtUserName.Text == "")
                {
                    if (Parameters.LanguageType == ("CN") || Parameters.LanguageType == (""))
                    {
                        MessageBox.Show(((Language_CN.LoginWindowEventContents)loginWindowEventClass).NullUserName, Parameters.GetTitleContent("I"), MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtUserName.Focus();
                    }
                    else if (Parameters.LanguageType == ("EN"))
                    {
                        MessageBox.Show(((Language_EN.LoginWindowEventContents)loginWindowEventClass).NullUserName, Parameters.GetTitleContent("I"), MessageBoxButton.OK, MessageBoxImage.Warning);
                        txtUserName.Focus();
                    }
                    return;
                }

                if (RemoteLocalHost.NetWorkSegmentA == "" || RemoteLocalHost.NetWorkSegmentA == null)
                {
                    MessageBox.Show("请输入服务器IP地址信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    SettingControlColorForTips(ServerNetWorkSegmentA);
                    return;
                }
                if (RemoteLocalHost.NetWorkSegmentB == "" || RemoteLocalHost.NetWorkSegmentB == null)
                {
                    MessageBox.Show("请输入服务器IP地址信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    SettingControlColorForTips(ServerNetWorkSegmentB);
                    return;
                }
                if (RemoteLocalHost.LoaclHostSegmentA == "" || RemoteLocalHost.LoaclHostSegmentA == null)
                {
                    MessageBox.Show("请输入服务器IP地址信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    SettingControlColorForTips(ServerHostA);
                    return;
                }
                if (RemoteLocalHost.LoaclHostSegmentB == "" || RemoteLocalHost.LoaclHostSegmentB == null)
                {
                    MessageBox.Show("请输入服务器IP地址信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    SettingControlColorForTips(ServerHostB);
                    return;
                }
                if (RemoteLocalHost.RemoteServerPort == "" || RemoteLocalHost.RemoteServerPort == null)
                {
                    MessageBox.Show("请输入服务器端口信息！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    SettingControlColorForTips(ServerPort);
                    return;
                }

                if (!Parameters.IsIP(
                        RemoteLocalHost.NetWorkSegmentA + "." +
                        RemoteLocalHost.NetWorkSegmentB + "." +
                        RemoteLocalHost.LoaclHostSegmentA + "." +
                        RemoteLocalHost.LoaclHostSegmentB + ":" +
                        RemoteLocalHost.RemoteServerPort
                    ))
                {
                    MessageBox.Show("服务器地址不正确,格式式为:[IP:Port]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtServerHostA.Focus();
                    return;
                }

                UserName = txtUserName.Text;
                PassWord = txtPassWd.Password;
                ServerHost = (RemoteLocalHost.NetWorkSegmentA + "." +
                             RemoteLocalHost.NetWorkSegmentB + "." +
                             RemoteLocalHost.LoaclHostSegmentA + "." +
                             RemoteLocalHost.LoaclHostSegmentB + ":" +
                             RemoteLocalHost.RemoteServerPort);

                DefaultInfo = (bool)chkRememberLoginInfo.IsChecked;

                //启动数据粘包处理线程
                JsonInterFace.StickDataPackageStart();

                //启动数据解析线程
                JsonInterFace.AnalysisDataStart();

                #region 登录与进度显示
                if (ProgressBarStartThread == null)
                {
                    Run = true;
                    ProgressBarStartThread = new Thread(new ThreadStart(ProgressBarStart));
                    ProgressBarStartThread.IsBackground = true;
                    ProgressBarStartThread.Start();

                    //登录发送请求
                    if (GetLoginAccess()) { return; }
                }
                else if (ProgressBarStartThread.ThreadState == ThreadState.Stopped)
                {
                    Run = true;
                    ProgressBarStartThread = new Thread(new ThreadStart(ProgressBarStart));
                    ProgressBarStartThread.IsBackground = true;
                    ProgressBarStartThread.Start();

                    //登录发送请求
                    if (GetLoginAccess()) { return; }
                }
                else if (ProgressBarStartThread != null && new Regex(ThreadState.Unstarted.ToString()).Match(ProgressBarStartThread.ThreadState.ToString()).Success)
                {
                    Run = true;
                    ProgressBarStartThread.Start();

                    //登录发送请求
                    if (GetLoginAccess()) { return; }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("登陆失败", ex.Message, ex.StackTrace);
                MessageBox.Show("登陆失败," + ex.Message, "提示", MessageBoxButton.OK);
            }
        }

        //控件高亮显示
        private void SettingControlColorForTips(object sender)
        {
            new Thread(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    (sender as TextBox).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF100C"));
                    Thread.Sleep(600);
                    (sender as TextBox).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7F0906"));
                }
            }).Start();
        }

        private void lblLoginAction_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoginStart(txtServerNetWorkSegmentA, txtServerNetWorkSegmentB, txtServerHostA, txtServerHostB, txtServerPort);
        }

        private void lblExit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void chkRememberLoginInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DefaultInfo = (bool)chkRememberLoginInfo.IsChecked;
                Parameters.WriteIniFileString("Login", "Default", Convert.ToInt32(DefaultInfo).ToString(), Parameters.INIFile);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                MessageBox.Show("记住用户名设置失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void btnLoginActionBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("DodgerBlue")));
        }

        private void btnExitBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("DodgerBlue")));
        }

        private void btnExitBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("Transparent")));
        }

        private void btnLoginActionBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("Transparent")));
        }

        /// <summary>
        /// 界面效果一
        /// </summary>
        /// <param name="SelfCanvas"></param>
        private void DrawStyle(ref Canvas SelfCanvas)
        {
            //外线
            Ellipse OutLine = new Ellipse();
            OutLine.Width = SelfCanvas.Width;
            OutLine.Height = SelfCanvas.Height;
            OutLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0CE9FC"));
            OutLine.StrokeThickness = 1.5;
            OutLine.Opacity = 0.4;
            DoubleCollection DCOutLine = new DoubleCollection();
            DCOutLine.Add(2);
            DCOutLine.Add(1);
            OutLine.StrokeDashArray = DCOutLine;
            Canvas.SetLeft(OutLine, 0);
            Canvas.SetTop(OutLine, 0);

            //内线
            Ellipse InnerLine = new Ellipse();
            InnerLine.Width = InputBox.Width;
            InnerLine.Height = InputBox.Height;
            InnerLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00F2CD"));
            InnerLine.StrokeThickness = 1.5;
            InnerLine.Opacity = 0.6;
            DoubleCollection DC = new DoubleCollection();
            DC.Add(2);
            DC.Add(1);
            InnerLine.StrokeDashArray = DC;
            Canvas.SetLeft(InnerLine, SelfCanvas.Width / 2 - InnerLine.Width / 2);
            Canvas.SetTop(InnerLine, SelfCanvas.Height / 2 - InnerLine.Height / 2);

            //角度线
            Ellipse ShortLine = new Ellipse();
            ShortLine.Width = InputBox.Width + 25 * 2;
            ShortLine.Height = InputBox.Height + 25 * 2;
            ShortLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("DodgerBlue"));
            ShortLine.StrokeThickness = 22;
            ShortLine.Opacity = 0.8;
            DoubleCollection DCShort = new DoubleCollection();
            DCShort.Add(0.1);
            DCShort.Add(0.5);
            ShortLine.StrokeDashArray = DCShort;
            Canvas.SetLeft(ShortLine, SelfCanvas.Width / 2 - ShortLine.Width / 2);
            Canvas.SetTop(ShortLine, SelfCanvas.Height / 2 - ShortLine.Height / 2);

            SelfCanvas.Children.Add(OutLine);
            SelfCanvas.Children.Add(InnerLine);
            SelfCanvas.Children.Add(ShortLine);
        }

        /// <summary>
        /// 界面效果二
        /// </summary>
        /// <param name="SelfCanvas"></param>
        private void DotLine(ref Canvas SelfCanvas)
        {
            double DotLineSpace = 4;
            //内环
            Ellipse InnerLine = new Ellipse();
            InnerLine.Width = InputBox.Width;
            InnerLine.Height = InputBox.Height;
            InnerLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            InnerLine.StrokeThickness = 1.5;
            InnerLine.Opacity = 0.6;
            DoubleCollection DC = new DoubleCollection();
            DC.Add(2);
            DC.Add(1);
            InnerLine.StrokeDashArray = DC;
            Canvas.SetLeft(InnerLine, SelfCanvas.Width / 2 - InnerLine.Width / 2);
            Canvas.SetTop(InnerLine, SelfCanvas.Height / 2 - InnerLine.Height / 2);

            //圆点-1
            Ellipse RoundA = new Ellipse();
            RoundA.Width = 12;
            RoundA.Height = 12;
            RoundA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            RoundA.StrokeThickness = 1;
            RoundA.Opacity = 1;
            Canvas.SetLeft(RoundA, Canvas.GetLeft(InnerLine) - RoundA.Width - DotLineSpace);
            Canvas.SetTop(RoundA, Canvas.GetTop(InnerLine) + InnerLine.Height / 2 - RoundA.Height);

            //圆点-2
            Ellipse RoundB = new Ellipse();
            RoundB.Width = 12;
            RoundB.Height = 12;
            RoundB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            RoundB.StrokeThickness = 1;
            RoundB.Opacity = 1;
            Canvas.SetBottom(RoundB, Canvas.GetTop(InnerLine) + InnerLine.Height + 3);
            Canvas.SetLeft(RoundB, Canvas.GetLeft(InnerLine) + InnerLine.Width / 2 - RoundA.Width);

            //中心线A
            Line CentLineA = new Line();
            CentLineA.X1 = SelfCanvas.Width / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace;
            CentLineA.Y1 = SelfCanvas.Height / 2;
            CentLineA.X2 = SelfCanvas.Width / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace;
            CentLineA.Y2 = SelfCanvas.Height / 2;
            CentLineA.Stroke = Brushes.Red;
            CentLineA.StrokeThickness = 1;

            //中心线B
            Line CentLineB = new Line();
            CentLineB.X1 = SelfCanvas.Width / 2;
            CentLineB.Y1 = SelfCanvas.Height / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace;
            CentLineB.X2 = SelfCanvas.Width / 2;
            CentLineB.Y2 = SelfCanvas.Height / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace;
            CentLineB.Stroke = Brushes.Red;
            CentLineB.StrokeThickness = 1;

            //弧线-1
            #region 弧线-1
            Path ARCPathA = new Path();
            PathGeometry pathGeometryA = new PathGeometry();
            //Point 结束点
            ArcSegment ARCA = new ArcSegment(
                                                new Point(
                                                           SelfCanvas.Width / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                           SelfCanvas.Height / 2
                                                         ),
                                                new Size(
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace
                                                        ),
                                                180,
                                                false,
                                                SweepDirection.Counterclockwise,
                                                true
                                           );
            PathFigure FigureA = new PathFigure();
            //起始点
            FigureA.StartPoint = new Point(SelfCanvas.Width / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace, SelfCanvas.Height / 2);
            FigureA.Segments.Add(ARCA);
            pathGeometryA.Figures.Add(FigureA);
            ARCPathA.Data = pathGeometryA;
            ARCPathA.Stroke = Brushes.Aqua;
            ARCPathA.StrokeThickness = 1;
            #endregion

            //弧线-2
            #region 弧线-2
            Path ARCPathB = new Path();
            PathGeometry pathGeometryB = new PathGeometry();
            //Point 结束点
            ArcSegment ARCB = new ArcSegment(
                                                new Point(
                                                           SelfCanvas.Width / 2,
                                                           SelfCanvas.Height / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace
                                                         ),
                                                new Size(
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace
                                                        ),
                                                90,
                                                false,
                                                SweepDirection.Counterclockwise,
                                                true
                                           );
            PathFigure FigureB = new PathFigure();
            //起始点
            FigureB.StartPoint = new Point(CentLineA.X2, CentLineA.Y2);
            FigureB.Segments.Add(ARCB);
            pathGeometryB.Figures.Add(FigureB);
            ARCPathB.Data = pathGeometryB;
            ARCPathB.Stroke = Brushes.Aqua;
            ARCPathB.StrokeThickness = 1;
            #endregion

            SelfCanvas.Children.Add(RoundA);
            SelfCanvas.Children.Add(RoundB);
            SelfCanvas.Children.Add(ARCPathA);
            SelfCanvas.Children.Add(ARCPathB);

        }

        private void DrawCenterLine(ref Canvas SelfCanvas)
        {
            double DotLineSpace = 4;
            //内环
            Ellipse InnerLine = new Ellipse();
            InnerLine.Width = InputBox.Width;
            InnerLine.Height = InputBox.Height;
            InnerLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
            InnerLine.StrokeThickness = 1.5;
            InnerLine.Opacity = 0.6;
            DoubleCollection DC = new DoubleCollection();
            DC.Add(2);
            DC.Add(1);
            InnerLine.StrokeDashArray = DC;
            Canvas.SetLeft(InnerLine, SelfCanvas.Width / 2 - InnerLine.Width / 2);
            Canvas.SetTop(InnerLine, SelfCanvas.Height / 2 - InnerLine.Height / 2);

            //圆点-1
            Ellipse RoundA = new Ellipse();
            RoundA.Width = 12;
            RoundA.Height = 12;
            RoundA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            RoundA.StrokeThickness = 1;
            RoundA.Opacity = 1;
            Canvas.SetLeft(RoundA, Canvas.GetLeft(InnerLine) - RoundA.Width - DotLineSpace);
            Canvas.SetTop(RoundA, Canvas.GetTop(InnerLine) + InnerLine.Height / 2 - RoundA.Height);

            //圆点-2
            Ellipse RoundB = new Ellipse();
            RoundB.Width = 12;
            RoundB.Height = 12;
            RoundB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Aqua"));
            RoundB.StrokeThickness = 1;
            RoundB.Opacity = 1;
            Canvas.SetBottom(RoundB, Canvas.GetTop(InnerLine) + InnerLine.Height + 3);
            Canvas.SetLeft(RoundB, Canvas.GetLeft(InnerLine) + InnerLine.Width / 2 - RoundA.Width);

            //中心线A
            Line CentLineA = new Line();
            CentLineA.X1 = SelfCanvas.Width / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace;
            CentLineA.Y1 = SelfCanvas.Height / 2;
            CentLineA.X2 = SelfCanvas.Width / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace;
            CentLineA.Y2 = SelfCanvas.Height / 2;
            CentLineA.Stroke = Brushes.Red;
            CentLineA.StrokeThickness = 1;

            //中心线B
            Line CentLineB = new Line();
            CentLineB.X1 = SelfCanvas.Width / 2;
            CentLineB.Y1 = SelfCanvas.Height / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace;
            CentLineB.X2 = SelfCanvas.Width / 2;
            CentLineB.Y2 = SelfCanvas.Height / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace;
            CentLineB.Stroke = Brushes.Red;
            CentLineB.StrokeThickness = 1;

            //弧线-1
            #region 弧线-1
            Path ARCPathA = new Path();
            PathGeometry pathGeometryA = new PathGeometry();
            //Point 结束点
            ArcSegment ARCA = new ArcSegment(
                                                new Point(
                                                           SelfCanvas.Width / 2 + InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                           SelfCanvas.Height / 2
                                                         ),
                                                new Size(
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace
                                                        ),
                                                180,
                                                false,
                                                SweepDirection.Counterclockwise,
                                                true
                                           );
            PathFigure FigureA = new PathFigure();
            //起始点
            FigureA.StartPoint = new Point(SelfCanvas.Width / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace, SelfCanvas.Height / 2);
            FigureA.Segments.Add(ARCA);
            pathGeometryA.Figures.Add(FigureA);
            ARCPathA.Data = pathGeometryA;
            ARCPathA.Stroke = Brushes.Aqua;
            ARCPathA.StrokeThickness = 1;
            #endregion

            //弧线-2
            #region 弧线-2
            Path ARCPathB = new Path();
            PathGeometry pathGeometryB = new PathGeometry();
            //Point 结束点
            ArcSegment ARCB = new ArcSegment(
                                                new Point(
                                                           SelfCanvas.Width / 2,
                                                           SelfCanvas.Height / 2 - InnerLine.Width / 2 - (RoundA.Width / 2) - DotLineSpace
                                                         ),
                                                new Size(
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace,
                                                            InnerLine.Width / 2 + (RoundA.Width / 2) + DotLineSpace
                                                        ),
                                                90,
                                                false,
                                                SweepDirection.Counterclockwise,
                                                true
                                           );
            PathFigure FigureB = new PathFigure();
            //起始点
            FigureB.StartPoint = new Point(CentLineA.X2, CentLineA.Y2);
            FigureB.Segments.Add(ARCB);
            pathGeometryB.Figures.Add(FigureB);
            ARCPathB.Data = pathGeometryB;
            ARCPathB.Stroke = Brushes.Aqua;
            ARCPathB.StrokeThickness = 1;
            #endregion

            SelfCanvas.Children.Add(RoundA);
            SelfCanvas.Children.Add(RoundB);
            SelfCanvas.Children.Add(ARCPathA);
            SelfCanvas.Children.Add(ARCPathB);
        }

        private void txtServerNetWorkSegmentA_KeyUp(object sender, KeyEventArgs e)
        {
            MoveKey(sender, e, txtServerNetWorkSegmentB, null);
        }

        //按键键功能
        private void MoveKey(object sender, KeyEventArgs e, TextBox disTextBox, TextBox backTextBox)
        {
            try
            {
                //按右方向键
                if ((e.Key == Key.Right) && (((sender as TextBox).SelectionStart >= (sender as TextBox).MaxLength) || ((sender as TextBox).SelectionStart >= (sender as TextBox).Text.Length)))
                {
                    if (disTextBox != null)
                    {
                        disTextBox.Focus();
                    }
                }
                //按左方向键
                else if ((e.Key == Key.Left) && (sender as TextBox).SelectionStart <= 0)
                {
                    if (backTextBox != null)
                    {
                        backTextBox.Focus();
                    }
                }
                //按回退删除键
                else if ((e.Key == Key.Back) && (sender as TextBox).SelectionStart <= 0)
                {
                    if ((sender as TextBox).Tag != null)
                    {
                        if (Parameters.ISDigital((sender as TextBox).Tag.ToString()))
                        {
                            if (Convert.ToInt32((sender as TextBox).Tag.ToString()) > 1)
                            {
                                (sender as TextBox).Tag = Convert.ToInt32((sender as TextBox).Tag.ToString()) + 1;
                            }
                            else
                            {
                                (sender as TextBox).Tag = string.Empty;
                                if (backTextBox != null)
                                {
                                    backTextBox.Focus();
                                    if (backTextBox.Text.Length > 0)
                                    {
                                        backTextBox.SelectionStart = backTextBox.Text.Length + 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            (sender as TextBox).Tag = "1";
                        }
                    }
                    else
                    {
                        (sender as TextBox).Tag = "1";
                    }
                }
                //按句点按扭
                else if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    if (disTextBox != null)
                    {
                        disTextBox.Focus();
                        disTextBox.SelectionStart = 0;
                    }
                }
                //按删除键
                else if (e.Key == Key.Delete)
                {
                    if ((sender as TextBox).Tag != null)
                    {
                        if (Parameters.ISDigital((sender as TextBox).Tag.ToString()))
                        {
                            if (Convert.ToInt32((sender as TextBox).Tag.ToString()) > 1)
                            {
                                (sender as TextBox).Tag = Convert.ToInt32((sender as TextBox).Tag.ToString()) + 1;
                            }
                            else
                            {
                                (sender as TextBox).Tag = string.Empty;
                                if ((sender as TextBox).Text == "" || (sender as TextBox).Text == null)
                                {
                                    if (disTextBox != null)
                                    {
                                        disTextBox.Focus();
                                        disTextBox.SelectionStart = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            (sender as TextBox).Tag = "1";
                        }
                    }
                    else
                    {
                        (sender as TextBox).Tag = "1";
                    }
                }
                else
                {
                    if ((sender as TextBox).Text != "" && (sender as TextBox).Text != null)
                    {
                        if ((sender as TextBox).Text.Length >= 3)
                        {
                            if (disTextBox != null)
                            {
                                disTextBox.Focus();
                                disTextBox.SelectionStart = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("远程地址事件处理异常", Ex.Message);
            }
        }

        private void txtServerNetWorkSegmentB_KeyUp(object sender, KeyEventArgs e)
        {
            MoveKey(sender, e, txtServerHostA, txtServerNetWorkSegmentA);
        }

        private void txtServerHostA_KeyUp(object sender, KeyEventArgs e)
        {
            MoveKey(sender, e, txtServerHostB, txtServerNetWorkSegmentB);
        }

        private void txtServerHostB_KeyUp(object sender, KeyEventArgs e)
        {
            MoveKey(sender, e, txtServerPort, txtServerHostA);
        }

        private void txtServerPort_KeyUp(object sender, KeyEventArgs e)
        {
            MoveKey(sender, e, null, txtServerHostB);
        }

        /// <summary>
        /// 鼠标左键按下全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentSelection(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TextBox).Text != "" || (sender as TextBox).Text != null)
            {
                (sender as TextBox).SelectAll();
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicRound(double Angle)
        {
            try
            {
                if (GraphicRotatePara.Angle > 360)
                {
                    GraphicRotatePara.ReverseAngle = -Angle * 2;
                    GraphicRotatePara.Angle = Angle;
                }
                else
                {
                    GraphicRotatePara.ReverseAngle -= Angle * 2;
                    GraphicRotatePara.Angle += Angle;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void LoginWin_Closing(object sender, CancelEventArgs e)
        {
            RenderGraphicRoundTimer.Stop();
        }

        private void txtServerNetWorkSegmentA_KeyDown(object sender, KeyEventArgs e)
        {
            ServerHostInputCkeck(sender, e);
        }

        private void ServerHostInputCkeck(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.D0
                    || e.Key == Key.D1
                    || e.Key == Key.D2
                    || e.Key == Key.D3
                    || e.Key == Key.D4
                    || e.Key == Key.D5
                    || e.Key == Key.D6
                    || e.Key == Key.D7
                    || e.Key == Key.D8
                    || e.Key == Key.D9
                    || e.Key == Key.NumPad0
                    || e.Key == Key.NumPad1
                    || e.Key == Key.NumPad2
                    || e.Key == Key.NumPad3
                    || e.Key == Key.NumPad4
                    || e.Key == Key.NumPad5
                    || e.Key == Key.NumPad6
                    || e.Key == Key.NumPad7
                    || e.Key == Key.NumPad8
                    || e.Key == Key.NumPad9
                    || e.Key == Key.Left
                    || e.Key == Key.Right
                    || e.Key == Key.Up
                    || e.Key == Key.Down
                    || e.Key == Key.Decimal
                    || e.Key == Key.OemPeriod
                    || e.Key == Key.Back
                   )
                {
                    if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(DateTime.Now.ToString(), "服务器IP处理内部故障，" + Ex.Message, Ex.StackTrace);
            }
        }

        private void txtServerNetWorkSegmentA_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                if ((sender as TextBox).Name == txtServerNetWorkSegmentB.Name)
                {
                    if ((sender as TextBox).SelectionStart <= 0)
                    {
                        txtServerNetWorkSegmentA.Focus();
                        txtServerNetWorkSegmentA.SelectionStart = txtServerNetWorkSegmentA.Text.Length;
                    }
                }
                else if ((sender as TextBox).Name == txtServerHostA.Name)
                {
                    if ((sender as TextBox).SelectionStart <= 0)
                    {
                        txtServerNetWorkSegmentB.Focus();
                        txtServerNetWorkSegmentB.SelectionStart = txtServerNetWorkSegmentB.Text.Length;

                        if (txtServerNetWorkSegmentB.SelectionStart <= 0)
                        {
                            txtServerNetWorkSegmentA.Focus();
                            txtServerNetWorkSegmentA.SelectionStart = txtServerNetWorkSegmentA.Text.Length;
                        }
                    }
                }
                else if ((sender as TextBox).Name == txtServerHostB.Name)
                {
                    if ((sender as TextBox).SelectionStart <= 0)
                    {
                        txtServerHostA.Focus();
                        txtServerHostA.SelectionStart = txtServerHostA.Text.Length;

                        if (txtServerHostA.SelectionStart <= 0)
                        {
                            txtServerNetWorkSegmentB.Focus();
                            txtServerNetWorkSegmentB.SelectionStart = txtServerNetWorkSegmentB.Text.Length;

                            if (txtServerNetWorkSegmentB.SelectionStart <= 0)
                            {
                                txtServerNetWorkSegmentA.Focus();
                                txtServerNetWorkSegmentA.SelectionStart = txtServerNetWorkSegmentA.Text.Length;
                            }
                        }
                    }
                }
                else if ((sender as TextBox).Name == txtServerPort.Name)
                {
                    if ((sender as TextBox).SelectionStart <= 0)
                    {
                        txtServerHostB.Focus();
                        txtServerHostB.SelectionStart = txtServerHostB.Text.Length;

                        if (txtServerHostB.SelectionStart <= 0)
                        {
                            txtServerHostA.Focus();
                            txtServerHostA.SelectionStart = txtServerHostA.Text.Length;

                            if (txtServerHostA.SelectionStart <= 0)
                            {
                                txtServerNetWorkSegmentB.Focus();
                                txtServerNetWorkSegmentB.SelectionStart = txtServerNetWorkSegmentB.Text.Length;

                                if (txtServerNetWorkSegmentB.SelectionStart <= 0)
                                {
                                    txtServerNetWorkSegmentA.Focus();
                                    txtServerNetWorkSegmentA.SelectionStart = txtServerNetWorkSegmentA.Text.Length;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoginWin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginStart(txtServerNetWorkSegmentA, txtServerNetWorkSegmentB, txtServerHostA, txtServerHostB, txtServerPort);
            }
        }
    }
}
