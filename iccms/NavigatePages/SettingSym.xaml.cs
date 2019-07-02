using DataInterface;
using NetController;
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
    /// SettingSym.xaml 的交互逻辑
    /// </summary>
    public partial class SettingSym : Window
    {
        private Dictionary<string, Uri> DeviceListWindow = new Dictionary<string, Uri>();
        private object SysLanguageClass = null;

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            IntPtr _handle;
            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }
            System.Windows.Forms.IWin32Window Members;
            #region IWin32Window Members
            IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }

        public SettingSym()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            DeviceListWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("EN"))
            {
                SysLanguageClass = new Language_EN.SettingSym();
                this.DataContext = (Language_EN.SettingSym)SysLanguageClass;
            }
            else
            {
                SysLanguageClass = new Language_CN.SettingSym();
                this.DataContext = (Language_CN.SettingSym)SysLanguageClass;
            }
            txtDefaultLocation.Text = Parameters.LogsDir;

            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("密码修改"))
                    {
                        tiChangePWD.Visibility = System.Windows.Visibility.Collapsed;
                        btnOK.IsEnabled = false;
                        tabControl.SelectedIndex = 1;
                    }
                    else
                    {
                        tabControl.SelectedIndex = 0;
                        tiChangePWD.Visibility = System.Windows.Visibility.Visible;
                        btnOK.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["密码修改"]));
                    }
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("本地设置"))
                    {
                        tiLocalSet.Visibility = System.Windows.Visibility.Collapsed;
                        btnUpdate.IsEnabled = false;
                        tabControl.SelectedIndex = 0;
                    }
                    else
                    {
                        tiLocalSet.Visibility = System.Windows.Visibility.Visible;
                        btnUpdate.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["本地设置"]));
                    }
                }
            }
            #endregion
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 移动窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnExit.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnExit.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string OriginalPd = txtOriginalPassword.Text.Trim();
            string newPd = txtNewPassword.Password.Trim();
            string reNewPd = txtReNewPassword.Password.Trim();
            string userName = new DesEncrypt().UnEncrypt(Parameters.ReadIniFile("Login", "UserName", "", Parameters.INIFile), new DefineCode().Code());
            if (newPd == reNewPd)
            {
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Modify_user_psw_Request(userName, OriginalPd, newPd));
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求修改用户密码:", "Connected: Failed!");
                }
            }
            else
            {
                MessageBox.Show("两次新密码输入不一致，请重新输入", "提示");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!txtLOGLocation.Text.Trim().Equals(""))
            {
                Parameters.WritePrivateProfileString("SystemSetting", "LogDir", txtLOGLocation.Text.Trim(), Parameters.INIFile);
            }
            MessageBox.Show("修改日志目录成功", "提示");
        }
        private void btnLogPathBrower_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Interop.HwndSource source = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            System.Windows.Forms.DialogResult result = dlg.ShowDialog(win);
            txtLOGLocation.Text = dlg.SelectedPath;
        }
    }
}