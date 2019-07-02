using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// UpdateUserInfo.xaml 的交互逻辑
    /// </summary>
    public partial class UpdateUserInfo : Window
    {
        private object LanguageClass = null;
        private UserGroupParamInfo updateUserParam = new UserGroupParamInfo();
        string PriIdSet;
        public UpdateUserInfo()
        {
            InitializeComponent();
        }
        public UpdateUserInfo(UserGroupParamInfo userGroupParam,string priId)
        {
            InitializeComponent();
            updateUserParam = userGroupParam;
            PriIdSet = priId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                LanguageClass = new Language_CN.UpdateUserInfo();
                this.DataContext = (Language_CN.UpdateUserInfo)LanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                LanguageClass = new Language_EN.UserManage();
                this.DataContext = (Language_EN.UserManage)LanguageClass;
            }
            if (updateUserParam != null)
            {
                txtUpdateUserName.Text = updateUserParam.UserName;
            }
        }

        private void btnAddCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddOK_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("确定要修改用户访问权限吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                string userName;
                string newPwd;
                string OldPwd;
                if ((!txtUpdateUserName.Text.Trim().Equals("")) || (!NewpasswordBox.Password.ToString().Equals("")) || (!OldpasswordBox.Password.ToString().Equals("")))
                {
                    userName = txtUpdateUserName.Text.Trim();
                    newPwd = NewpasswordBox.Password.ToString();
                    OldPwd = OldpasswordBox.Password.ToString();
                    //请求修改用户密码
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Modify_user_psw_Request(userName, OldPwd, newPwd));
                        if (!PriIdSet.Equals(""))
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.Update_usr_domain_request(userName,PriIdSet,""));
                        }
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("向服务器请求修改用户密码:", "Connected: Failed!");
                    }
                    if (!PriIdSet.Equals(""))
                    {
                        foreach(DataRow rw in JsonInterFace.UsrdomainManageClass.UsrDomainTable.Rows)
                        {
                            if (rw[0].ToString().Equals(userName))
                            {
                                rw[1] = PriIdSet;
                                break;
                            }
                        }
                    }
                    Thread.Sleep(1000);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名、新密码、旧密码都不能为空");
                }
            }
        }
    }
}
