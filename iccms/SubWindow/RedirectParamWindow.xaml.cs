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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iccms.SubWindow
{
    /// <summary>
    /// RedirectParamWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RedirectParamWindow : Window
    {
        private object RedirectParamLanguageClass = null;
        public RedirectParamWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                RedirectParamLanguageClass = new Language_CN.RedirectSetClass();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                RedirectParamLanguageClass = new Language_EN.RedirectSetClass();
            }
        }

        private void AddRedirectParam_Loaded(object sender, RoutedEventArgs e)
        {
            //中/英文初始化
            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = new Language_EN.RedirectSetClass();
            }
            else
            {
                this.DataContext = new Language_CN.RedirectSetClass();
            }
            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("重定向设置"))
                    {
                        btnOK.IsEnabled = false;
                    }
                    else
                    {
                        btnOK.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["重定向设置"]));
                    }
                }
                else
                {
                    if (int.Parse(RoleTypeClass.RoleType) > 3)//操作员权限
                    {
                        btnOK.IsEnabled = false;
                    }
                }
            }
            #endregion
            txtParentFullPathName.Text = JsonInterFace.ReDirection.ParentFullPathName;
            txtSelfName.Text = JsonInterFace.ReDirection.Name;
            txtOptimization.Text = JsonInterFace.ReDirection.Frequency;
            txtAdditionalFreq.Text = JsonInterFace.ReDirection.AddtionFrequency;
            if (JsonInterFace.ReDirection.RejectMethod == null)
            {
                cmbRejectMethod.SelectedIndex = -1;
            }
            else
            {
                if (JsonInterFace.ReDirection.RejectMethod.Equals("永久拒绝"))
                {
                    cmbRejectMethod.SelectedIndex = 0;
                }
                else if (JsonInterFace.ReDirection.RejectMethod.Equals("拒绝一次"))
                {
                    cmbRejectMethod.SelectedIndex = 1;
                }
                else if (JsonInterFace.ReDirection.RejectMethod.Equals("不拒绝"))
                {
                    cmbRejectMethod.SelectedIndex = 2;
                }
                else
                {
                    cmbRejectMethod.SelectedIndex = -1;
                }
            }
            if (JsonInterFace.ReDirection.UserType != null)
            {
                if (JsonInterFace.ReDirection.UserType.Equals("0") || JsonInterFace.ReDirection.UserType.Equals("白名单"))
                {
                    WhiteName.IsChecked = true;
                }
                else if (JsonInterFace.ReDirection.UserType.Equals("1") || JsonInterFace.ReDirection.UserType.Equals("黑名单"))
                {
                    BlackName.IsChecked = true;
                }
                else
                {
                    OtherName.IsChecked = true;
                }
            }
            if (JsonInterFace.ReDirection.Priority != null)
            {
                if (JsonInterFace.ReDirection.Priority.Equals("2"))
                {
                    rbGeranRedirect.IsChecked = true;
                }
                else if (JsonInterFace.ReDirection.Priority.Equals("3"))
                {
                    rbUtranRedirect.IsChecked = true;
                }
                else if (JsonInterFace.ReDirection.Priority.Equals("4"))
                {
                    rbEutranRedirect.IsChecked = true;
                }
                else
                {
                    rbOtherRedirect.IsChecked = true;
                }
            }

        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            string GeranRedirect = "0";
            string arfcn = String.Empty;
            string UtranRedirect = "0";
            string uarfcn = String.Empty;
            string EutranRedirect = "0";
            string earfcn = String.Empty;
            string parentFullPathName = txtParentFullPathName.Text;
            string name = txtSelfName.Text;
            string category = string.Empty;
            string priority = string.Empty;
            string RejectMethod = string.Empty;
            string additionalFreq = txtAdditionalFreq.Text;
            //用户类型
            if ((bool)WhiteName.IsChecked)
            {
                category = "0";
                Parameters.RedirectionParam.Category = WhiteName.Content.ToString();
            }
            else if ((bool)BlackName.IsChecked)
            {
                category = "1";
                Parameters.RedirectionParam.Category = BlackName.Content.ToString();
            }
            else
            {
                category = "2";
                Parameters.RedirectionParam.Category = OtherName.Content.ToString();
            }
            //优选
            if ((bool)rbGeranRedirect.IsChecked)
            {
                priority = "2";
                GeranRedirect = "1";
                arfcn = txtOptimization.Text.Trim();
                Parameters.RedirectionParam.Optimization = "2G";
            }
            else if ((bool)rbUtranRedirect.IsChecked)
            {
                priority = "3";
                UtranRedirect = "1";
                uarfcn = txtOptimization.Text.Trim();
                Parameters.RedirectionParam.Optimization = "3G";
            }
            else if ((bool)rbEutranRedirect.IsChecked)
            {
                priority = "4";
                EutranRedirect = "1";
                earfcn = txtOptimization.Text.Trim();
                Parameters.RedirectionParam.Optimization = "4G";
            }
            else
            {
                priority = "0";
                Parameters.RedirectionParam.Optimization = "无";
            }
            //制式
            if (cmbRejectMethod.SelectedIndex == 0)
            {
                RejectMethod = "1";
            }
            else if (cmbRejectMethod.SelectedIndex == 1)
            {
                RejectMethod = "2";
            }
            else if (cmbRejectMethod.SelectedIndex == 2)
            {
                RejectMethod = "255";
            }
            else
            {
                RejectMethod = System.Convert.ToInt32(cmbRejectMethod.Text, 10).ToString();
            }

            Parameters.RedirectionParam.Priority = priority;
            Parameters.RedirectionParam.RejectMethod = cmbRejectMethod.Text;
            Parameters.RedirectionParam.Freq = txtOptimization.Text;
            Parameters.RedirectionParam.AdditionalFreq = txtAdditionalFreq.Text;


            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_redirection_Request(parentFullPathName, name, category, priority,
                                                                                          GeranRedirect, arfcn, UtranRedirect, uarfcn, EutranRedirect, earfcn,
                                                                                          RejectMethod, additionalFreq));
                Thread.Sleep(1000);
                this.Close();
            }
            else
            {
                Parameters.PrintfLogsExtended("向服务器请求设置重定向:", "Connected: Failed!");
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddRedirectParam_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void AddRedirectParam_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        private void AddRedirectParam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            //用户类型
            if ((bool)WhiteName.IsChecked)
            {
                JsonInterFace.ReDirection.UserType = "0";
            }
            else if ((bool)BlackName.IsChecked)
            {
                JsonInterFace.ReDirection.UserType = "1";
            }
            else
            {
                JsonInterFace.ReDirection.UserType = "2";
            }
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.Get_redirection_Request(txtParentFullPathName.Text.Trim(), txtSelfName.Text.Trim(), JsonInterFace.ReDirection.UserType));
                Thread.Sleep(1000);
                this.Close();
            }
            else
            {
                Parameters.PrintfLogsExtended("向服务器请求重定向:", "Connected: Failed!");
            }
        }
    }
}
