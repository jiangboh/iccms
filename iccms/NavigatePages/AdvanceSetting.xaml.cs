using DataInterface;
using ParameterControl;
using System;
using System.IO;
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
using NetController;
using System.Runtime.InteropServices;

namespace iccms.NavigatePages
{
    /// <summary>
    /// AdvanceSetting.xaml 的交互逻辑
    /// </summary>
    public partial class AdvanceSetting : Window
    {
        private Dictionary<string, Uri> DeviceListWindow = new Dictionary<string, Uri>();
        private object AdvanceSetLanguageClass = null;
        public AdvanceSetting()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            DeviceListWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
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
                if (msg == Parameters.WM_BatchConfigurationOutputMessage)
                {
                    DownLoadConfigurationFile();
                }
                else if (msg == Parameters.WM_BatchConfigurationImportMessage)
                {
                    string ErrorStr = Marshal.PtrToStringUni(lParam);
                    Dispatcher.Invoke(() =>
                    {
                        txtEventsBox.AppendText(Environment.NewLine + "============ " + DateTime.Now.ToString() + " ============" + Environment.NewLine);
                        txtEventsBox.AppendText(ErrorStr);
                    });
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("响应Windows消息事件：", ex.Message, ex.StackTrace);
            }

            return hwnd;
        }

        private void DownLoadConfigurationFile()
        {
            try
            {
                FtpHelper ftp = new FtpHelper();
                ftp.RemoteHost = JsonInterFace.BatchConfigurationOutputParameter.FtpServerIp;
                ftp.RemotePort = int.Parse(JsonInterFace.BatchConfigurationOutputParameter.FtpPort);
                ftp.RemoteUser = JsonInterFace.BatchConfigurationOutputParameter.FtpUsrName;
                ftp.RemotePath = JsonInterFace.BatchConfigurationOutputParameter.FtpRootDir;
                ftp.RemotePass = JsonInterFace.BatchConfigurationOutputParameter.FtpPwd;
                ftp.Connect();
                if (ftp.Connected)
                {
                    ftp.Get(
                            JsonInterFace.BatchConfigurationOutputParameter.FileName,
                            new FileInfo(JsonInterFace.BatchConfigurationOutputParameter.LocalDir).DirectoryName,
                            JsonInterFace.BatchConfigurationOutputParameter.FileName
                           );
                }
                else
                {
                    MessageBox.Show("建立连接不成功，批量配置导出失败！");
                    return;
                }

                MessageBox.Show("批量配置导出到文件[" + JsonInterFace.BatchConfigurationOutputParameter.LocalDir + "]成功！"
                                , "提示"
                                , MessageBoxButton.OK
                                , MessageBoxImage.Information
                               );
            }
            catch (Exception ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "批量配置导入导出" + ex.Message, "配置批量导出", "失败");
            }
        }

        /// <summary>
        /// Ftp上传文件
        /// </summary>
        /// <param name="FileName">完整路径文件名</param>
        /// <returns></returns>
        private bool UpLoadConfigurationFile(string FileName)
        {
            try
            {
                FtpHelper ftp = new FtpHelper();
                ftp.RemoteHost = Parameters.ServerBaseParameter.StrFtpIpAddr;
                ftp.RemotePort = int.Parse(Parameters.ServerBaseParameter.StrFtpPort);
                ftp.RemoteUser = Parameters.ServerBaseParameter.StrFtpUserId;
                ftp.RemotePath = Parameters.ServerBaseParameter.StrFtpUpdateDir;
                ftp.RemotePass = Parameters.ServerBaseParameter.StrFtpUserPsw;
                ftp.Connect();
                if (ftp.Connected)
                {
                    ftp.Put(
                            new FileInfo(FileName).DirectoryName,
                            new FileInfo(FileName).Name
                           );
                }
                else
                {
                    MessageBox.Show("建立连接不成功，批量配置导入失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "批量配置导入导出" + ex.Message, "配置批量导入", "失败");
            }
            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                AdvanceSetLanguageClass = new Language_CN.AdvanceSet();
                this.DataContext = (Language_CN.AdvanceSet)AdvanceSetLanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                AdvanceSetLanguageClass = new Language_EN.AdvanceSet();
                this.DataContext = (Language_EN.AdvanceSet)AdvanceSetLanguageClass;
            }

            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("后台设置"))
                    {
                        tiBackgroundSet.Visibility = System.Windows.Visibility.Collapsed;
                        btnSave.IsEnabled = false;
                        tabControl.SelectedIndex = 1;
                    }
                    else
                    {
                        tabControl.SelectedIndex = 0;
                        tiBackgroundSet.Visibility = System.Windows.Visibility.Visible;
                        btnSave.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["后台设置"]));
                    }
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("配置导入导出"))
                    {
                        tiImportExportSet.Visibility = System.Windows.Visibility.Collapsed;
                        btnExport.IsEnabled = false;
                        btnImport.IsEnabled = false;
                        tabControl.SelectedIndex = 0;
                    }
                    else
                    {
                        tiImportExportSet.Visibility = System.Windows.Visibility.Visible;
                        btnExport.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["配置导入导出"]));
                        btnImport.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["配置导入导出"]));
                    }
                }
            }
            #endregion

            txtDBIPAddr.DataContext = Parameters.ServerBaseParameter;
            txtLogsType.DataContext = Parameters.ServerBaseParameter;
            txtDataAlignMode.DataContext = Parameters.ServerBaseParameter;
            txtLogsFileSize.DataContext = Parameters.ServerBaseParameter;
            txtFTPServerIPAddr.DataContext = Parameters.ServerBaseParameter;
            txtFTPServerPort.DataContext = Parameters.ServerBaseParameter;
            txtFTPServerUser.DataContext = Parameters.ServerBaseParameter;
            txtFTPServerPass.DataContext = Parameters.ServerBaseParameter;
            txtFTPServerPath.DataContext = Parameters.ServerBaseParameter;
            txtCDMAPort.DataContext = Parameters.ServerBaseParameter;
            txtGSMV2Port.DataContext = Parameters.ServerBaseParameter;
            txtGSMPort.DataContext = Parameters.ServerBaseParameter;
            txtLTEPort.DataContext = Parameters.ServerBaseParameter;
            txtWCDMAPort.DataContext = Parameters.ServerBaseParameter;
            txtClientPort.DataContext = Parameters.ServerBaseParameter;
            txtAPPort.DataContext = Parameters.ServerBaseParameter;
            txtAndroidPort.DataContext = Parameters.ServerBaseParameter;
            txtTDSPort.DataContext = Parameters.ServerBaseParameter;

            //句柄
            Parameters.AdvanceSettingWinHandle = new WindowInteropHelper(this).Handle;

            //载入基本参数
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.ServerBaseParameterRequest());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定导出批量配置参数？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
                return;
            }

            if (txtExportFile.Text == "")
            {
                Microsoft.Win32.SaveFileDialog SaveFileDialog = new Microsoft.Win32.SaveFileDialog();
                SaveFileDialog.Filter = "Text File(*.txt)|*.txt|All files(*.*)|*.*";
                SaveFileDialog.AddExtension = true;
                SaveFileDialog.RestoreDirectory = true;
                if ((bool)SaveFileDialog.ShowDialog())
                {
                    try
                    {
                        txtExportFile.Text = SaveFileDialog.FileName;
                        if (SaveFileDialog.FileName != null && SaveFileDialog.FileName != "")
                        {
                            JsonInterFace.BatchConfigurationOutputParameter.LocalDir = SaveFileDialog.FileName;
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.BatchConfigurationInfoRequest(new FileInfo(JsonInterFace.BatchConfigurationOutputParameter.LocalDir).Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                try
                {
                    JsonInterFace.BatchConfigurationOutputParameter.LocalDir = txtExportFile.Text;
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.BatchConfigurationInfoRequest(new FileInfo(JsonInterFace.BatchConfigurationOutputParameter.LocalDir).Name));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定导入批量配置参数？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
                return;
            }

            Microsoft.Win32.OpenFileDialog importFileDialog = new Microsoft.Win32.OpenFileDialog();
            importFileDialog.Filter = "Text File(*.txt)|*.txt|All files(*.*)|*.*";
            importFileDialog.AddExtension = true;
            importFileDialog.RestoreDirectory = true;
            if (txtImportFile.Text.Trim() == "")
            {
                if ((bool)importFileDialog.ShowDialog())
                {
                    try
                    {
                        string localFilePath = importFileDialog.FileName.ToString();
                        txtImportFile.Text = localFilePath;

                        if (UpLoadConfigurationFile(localFilePath))
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.BatchConfigurationImportRequest(new FileInfo(localFilePath).Name));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                try
                {
                    string localFilePath = txtImportFile.Text;
                    if (File.Exists(localFilePath))
                    {
                        if (UpLoadConfigurationFile(localFilePath))
                        {
                            if (NetWorkClient.ControllerServer.Connected)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.BatchConfigurationImportRequest(new FileInfo(localFilePath).Name));
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("文件[" + localFilePath + "]不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定保存服务器基本参数？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                //检测数据正确性
                if (!Parameters.IsIP((object)Parameters.ServerBaseParameter.StrDbIpAddr)) { JsonInterFace.ShowMessage("数据库IP地址非法！", 16); return; }
                if (!Parameters.IsIP((object)Parameters.ServerBaseParameter.StrFtpIpAddr)) { JsonInterFace.ShowMessage("FTP服务器IP地址非法！", 16); return; }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrFtpPort))
                {
                    JsonInterFace.ShowMessage("FTP服务器端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrFtpPort) < 1 || int.Parse(Parameters.ServerBaseParameter.StrFtpPort) > 65535)
                    {
                        JsonInterFace.ShowMessage("FTP服务器端口非法！", 16);
                        return;
                    }
                }
                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortCDMA_ZYF))
                {
                    JsonInterFace.ShowMessage("CDMA监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortCDMA_ZYF) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortCDMA_ZYF) > 65535)
                    {
                        JsonInterFace.ShowMessage("CDMA监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortGSM_ZYF))
                {
                    JsonInterFace.ShowMessage("GSMV2监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortGSM_ZYF) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortGSM_ZYF) > 65535)
                    {
                        JsonInterFace.ShowMessage("GSMV2监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortGSM_HJT))
                {
                    JsonInterFace.ShowMessage("GSM监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortGSM_HJT) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortGSM_HJT) > 65535)
                    {
                        JsonInterFace.ShowMessage("GSM监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortLTE))
                {
                    JsonInterFace.ShowMessage("LTE监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortLTE) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortLTE) > 65535)
                    {
                        JsonInterFace.ShowMessage("LTE监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortTDS))
                {
                    JsonInterFace.ShowMessage("TDS监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortTDS) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortTDS) > 65535)
                    {
                        JsonInterFace.ShowMessage("TDS监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortWCDMA))
                {
                    JsonInterFace.ShowMessage("WCDMA监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortWCDMA) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortWCDMA) > 65535)
                    {
                        JsonInterFace.ShowMessage("WCDMA监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortAppWindows))
                {
                    JsonInterFace.ShowMessage("Client监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortAppWindows) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortAppWindows) > 65535)
                    {
                        JsonInterFace.ShowMessage("Client监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortAppLinux))
                {
                    JsonInterFace.ShowMessage("AP监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortAppLinux) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortAppLinux) > 65535)
                    {
                        JsonInterFace.ShowMessage("AP监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.StrStartPortAppAndroid))
                {
                    JsonInterFace.ShowMessage("Android监听端口非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.StrStartPortAppAndroid) < 1 || int.Parse(Parameters.ServerBaseParameter.StrStartPortAppAndroid) > 65535)
                    {
                        JsonInterFace.ShowMessage("Android监听端口非法！", 16);
                        return;
                    }
                }

                if (!Parameters.ISDigital(Parameters.ServerBaseParameter.LogMaxSize))
                {
                    JsonInterFace.ShowMessage("Log文件大小值非法！", 16);
                    return;
                }
                else
                {
                    if (int.Parse(Parameters.ServerBaseParameter.LogMaxSize) < 1 || int.Parse(Parameters.ServerBaseParameter.LogMaxSize) > 65535)
                    {
                        JsonInterFace.ShowMessage("Log文件大小值非法！", 16);
                        return;
                    }
                }

                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("strDbIpAddr", Parameters.ServerBaseParameter.StrDbIpAddr);
                Params.Add("logOutputLevel", Parameters.ServerBaseParameter.LogOutputLevel);
                Params.Add("strFtpIpAddr", Parameters.ServerBaseParameter.StrFtpIpAddr);
                Params.Add("strFtpUserId", Parameters.ServerBaseParameter.StrFtpUserId);
                Params.Add("strFtpUserPsw", Parameters.ServerBaseParameter.StrFtpUserPsw);
                Params.Add("strFtpPort", Parameters.ServerBaseParameter.StrFtpPort);
                Params.Add("strFtpUpdateDir", Parameters.ServerBaseParameter.StrFtpUpdateDir);
                Params.Add("strStartPortCDMA_ZYF", Parameters.ServerBaseParameter.StrStartPortCDMA_ZYF);
                Params.Add("strStartPortGSM_ZYF", Parameters.ServerBaseParameter.StrStartPortGSM_ZYF);
                Params.Add("strStartPortGSM_HJT", Parameters.ServerBaseParameter.StrStartPortGSM_HJT);
                Params.Add("strStartPortLTE", Parameters.ServerBaseParameter.StrStartPortLTE);
                Params.Add("strStartPortTDS", Parameters.ServerBaseParameter.StrStartPortTDS);
                Params.Add("strStartPortWCDMA", Parameters.ServerBaseParameter.StrStartPortWCDMA);
                Params.Add("strStartPortAppWindows", Parameters.ServerBaseParameter.StrStartPortAppWindows);
                Params.Add("strStartPortAppLinux", Parameters.ServerBaseParameter.StrStartPortAppLinux);
                Params.Add("strStartPortAppAndroid", Parameters.ServerBaseParameter.StrStartPortAppAndroid);
                Params.Add("dataAlignMode", Parameters.ServerBaseParameter.DataAlignMode);
                Params.Add("logMaxSize", Parameters.ServerBaseParameter.LogMaxSize);

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.ServerBaseParameterSaveRequest(Params));
                }
                else
                {
                    MessageBox.Show("网络与服务器已断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
