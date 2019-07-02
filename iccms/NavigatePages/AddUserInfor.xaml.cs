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
using System.Data;
using NetController;
using DataInterface;
using System.Threading;

namespace iccms.NavigatePages
{

    /// <summary>
    /// AddUserInfor.xaml 的交互逻辑
    /// </summary>
    public partial class AddUserInfor : Window
    {
        public bool bolresult = false;//返回确定结果
        private object AddUserNameLanguageClass = null;
        string fullNameList;
        List<string> tmpRoleManageList = new List<string>();
        public AddUserInfor()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            bolresult = false;
        }
        public AddUserInfor(string fullName)
        {
            InitializeComponent();
            fullNameList = fullName;
            //用户组权限判断
            if (JsonInterFace.LoginUserInfo.Count > 0)
            {
                tmpRoleManageList.Clear();
                foreach (LoginedInfo loginedInfo in JsonInterFace.LoginUserInfo)
                {
                    if (loginedInfo.WorkGroup != null)
                    {
                        for (int i = JsonInterFace.RoleManageList.Count - 1; i >= 0; i--)
                        {
                            if(JsonInterFace.RoleManageList[i].Name.Equals("RoleEng") || JsonInterFace.RoleManageList[i].Name.Equals("RoleSA"))
                            {
                                break;
                            }
                            if (JsonInterFace.RoleManageList[i].Name.Equals(loginedInfo.WorkGroup))
                            {
                                tmpRoleManageList.Add(JsonInterFace.RoleManageList[i].Name);
                                break;
                            }
                            tmpRoleManageList.Add(JsonInterFace.RoleManageList[i].Name);
                        }
                    }
                }
            }
            if (tmpRoleManageList.Count > 0)
            {
                tmpRoleManageList.Reverse();
            }
            bolresult = false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                AddUserNameLanguageClass = new Language_CN.AddUserInfor();
                this.DataContext = (Language_CN.AddUserInfor)AddUserNameLanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                AddUserNameLanguageClass = new Language_EN.AddUserInfor();
                this.DataContext = (Language_EN.AddUserInfor)AddUserNameLanguageClass;
            }
            cbAddUserGroup.ItemsSource = tmpRoleManageList;
            string userName = new DesEncrypt().UnEncrypt(Parameters.ReadIniFile("Login", "UserName", "", Parameters.INIFile), new DefineCode().Code());
            txtAddBuilder.IsEnabled = false;
            txtAddBuilder.Text = userName;
        }

        private void btnAddCancel_Click(object sender, RoutedEventArgs e)
        {
            bolresult = false;
            this.Close();
        }

        private void btnAddOK_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("确定要增加用户吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                try
                {
                    if ((!txtAddUserName.Text.Trim().Equals("")) || (!cbAddUserGroup.Text.Trim().Equals("")) || (!txtAddUserPassword.Password.Trim().Equals("")))
                    {
                        JsonInterFace.AddUserManageList.Name = txtAddUserName.Text.Trim();
                        JsonInterFace.AddUserManageList.RoleType = cbAddUserGroup.Text.Trim();
                        JsonInterFace.AddUserManageList.Password = txtAddUserPassword.Password.Trim();
                        JsonInterFace.AddUserManageList.Des = txtAddOtherName.Text.Trim();
                        JsonInterFace.AddUserManageList.BuildName = txtAddBuilder.Text.Trim();
                        JsonInterFace.AddUserManageList.DomainIdSet = fullNameList;


                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "AddUser";
                            bolresult = true;
                            //添加用户
                            //Parameters.SendMessage(Parameters.UserManagerWinHandle, Parameters.WM_UserManagerResponse, 0, 0);
                        }
                        else
                        {
                            Parameters.PrintfLogsExtended("向服务器请求增加用户:", "Connected: Failed!");
                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("用户名、用户组、密码都不能为空");
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("添加用户", ex.Message, ex.StackTrace);
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnAddCancel.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnAddCancel.Focus();
        }
    }
}
