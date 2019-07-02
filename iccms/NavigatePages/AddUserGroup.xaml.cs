using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
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
    /// AddUserGroup.xaml 的交互逻辑
    /// </summary>
    public partial class AddUserGroup : Window
    {
        private object AddUserGroupLanguageClass = null;
        private IList<TreeModel> treeSource = new List<TreeModel>();
        public AddUserGroup()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        public AddUserGroup(IList<TreeModel> treeSourceData)
        {
            InitializeComponent();
            treeSource = treeSourceData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                AddUserGroupLanguageClass = new Language_CN.AddUserGroup();
                this.DataContext = (Language_CN.AddUserGroup)AddUserGroupLanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                AddUserGroupLanguageClass = new Language_EN.AddUserGroup();
                this.DataContext = (Language_EN.AddUserGroup)AddUserGroupLanguageClass;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private string GetpriIdSet(IList<TreeModel> SourceData)
        {
            string tmppriIdSet = string.Empty;
            if (SourceData.Count > 0)
            {
                for (int i = 0; i < SourceData.Count - 1; i++)
                {
                    tmppriIdSet += SourceData[i].Id + ",";
                }
                tmppriIdSet += SourceData[SourceData.Count - 1].Id;
            }
            return tmppriIdSet;
        }
        private string GetIsWrite(IList<TreeModel> SourceData)
        {
            string tmpIsWrite = string.Empty;
            if (SourceData.Count > 0)
            {
                for (int i = 0; i < SourceData.Count - 1; i++)
                {
                    tmpIsWrite += Convert.ToInt32(SourceData[i].IsWrite).ToString() + ",";
                }
                tmpIsWrite += Convert.ToInt32(SourceData[SourceData.Count - 1].IsWrite).ToString();
            }
            return tmpIsWrite;
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string RoleType = string.Empty;
            string IsRead = string.Empty;
            string IsWrite = string.Empty;
            JsonInterFace.RoleManageInfo.Name = "";
            JsonInterFace.RoleManageInfo.RoleType = "";
            JsonInterFace.RoleManageInfo.TimeStart = "";
            JsonInterFace.RoleManageInfo.TimeEnd = "";
            JsonInterFace.RoleManageInfo.Des = "";
            JsonInterFace.RoleManageInfo.IsRead = "";
            JsonInterFace.RoleManageInfo.IsWrite = "";
            JsonInterFace.RoleManageInfo.AliasName = "";
            try
            {
                if (cbInherit.SelectedIndex == 0)
                {
                    RoleType = "Operator";
                    IsRead = GetpriIdSet(treeSource);
                    IsWrite = GetIsWrite(treeSource);
                }
                else
                {
                    if (JsonInterFace.GroupprivilegeManageList.Count > 0)
                    {
                        for (int i = 0; i < JsonInterFace.GroupprivilegeManageList.Count; i++)
                        {
                            if (cbInherit.SelectedIndex == 1)
                            {
                                RoleType = "SeniorOperator";
                                if (JsonInterFace.GroupprivilegeManageList[i].GroupName.Equals("SeniorOperator"))
                                {
                                    IsRead = JsonInterFace.GroupprivilegeManageList[i].PriIdSet;
                                    IsWrite = JsonInterFace.GroupprivilegeManageList[i].Des;
                                }
                            }
                            else if (cbInherit.SelectedIndex == 2)
                            {
                                RoleType = "Operator";
                                if (JsonInterFace.GroupprivilegeManageList[i].GroupName.Equals("Operator"))
                                {
                                    IsRead = JsonInterFace.GroupprivilegeManageList[i].PriIdSet;
                                    IsWrite = JsonInterFace.GroupprivilegeManageList[i].Des;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (cbInherit.SelectedIndex == 1)
                        {
                            RoleType = "SeniorOperator";
                            IsRead = "";
                            IsWrite = "";
                        }
                        else if (cbInherit.SelectedIndex == 2)
                        {
                            RoleType = "Operator";
                            IsRead = "";
                            IsWrite = "";
                        }
                    }

                }

                MessageBoxResult dr = MessageBox.Show("确定要增加用户组吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    //请求增加用户组和权限
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        if ((!dploreStartTime.Text.Equals("")) && (!dploreEndTime.Text.Equals("")))
                        {
                            JsonInterFace.RoleManageInfo.TimeStart = Convert.ToDateTime(dploreStartTime.Text).ToString("yyyy-MM-dd 00:00:00");
                            JsonInterFace.RoleManageInfo.TimeEnd = Convert.ToDateTime(dploreEndTime.Text).ToString("yyyy-MM-dd 00:00:00");
                        }
                        else
                        {
                            JsonInterFace.RoleManageInfo.TimeStart = "1970-01-01 00:00:00";
                            JsonInterFace.RoleManageInfo.TimeEnd = "3000-01-01 00:00:00";
                        }
                        Parameters.ConfigType = "AddRole";
                        JsonInterFace.RoleManageInfo.Name = txtUserGroupName.Text.Trim();
                        JsonInterFace.RoleManageInfo.RoleType = RoleType;
                        JsonInterFace.RoleManageInfo.Des = "";
                        JsonInterFace.RoleManageInfo.IsRead = IsRead;
                        JsonInterFace.RoleManageInfo.IsWrite = IsWrite;
                        JsonInterFace.RoleManageInfo.AliasName = txtUserGroupName.Text.Trim();
                        //添加用户组
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_role_Request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.RoleType, JsonInterFace.RoleManageInfo.TimeStart, JsonInterFace.RoleManageInfo.TimeEnd, ""));

                        if (!JsonInterFace.RoleManageInfo.IsRead.Equals(""))
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.Add_group_privilege_request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.IsRead, JsonInterFace.RoleManageInfo.IsWrite));
                        }
                        this.Close();

                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("向服务器请求增加用户及权限:", "Connected: Failed!");
                    }
                }
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("添加用户组", ex.Message, ex.StackTrace);
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
                if (btnAddGroupCancel.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnAddGroupCancel.Focus();
        }
    }
}
