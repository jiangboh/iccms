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
    /// AddTreeItem.xaml 的交互逻辑
    /// </summary>
    public partial class AddTreeItem : Window
    {
        private TreeModel treeModel = new TreeModel();
        public AddTreeItem()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }
        public AddTreeItem(TreeModel tree)
        {
            InitializeComponent();
            treeModel = tree;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtParent.Text = treeModel.Name;
        }
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            int EndID = 0;
            String ParentId;
            string tmpTreeItemStr = string.Empty;
            ParentId = treeModel.Name;
            tmpTreeItemStr = txtParent.Text.Trim() + "-" + txtChrild.Text.Trim();
            if (NetWorkClient.ControllerServer.Connected)
            {
                DefaultPrivilege.Des = "";
                NetWorkClient.ControllerServer.Send(JsonInterFace.Add_privilege_request(txtChrild.Text.Trim(), ParentId, DefaultPrivilege.Des));//请求权限
            }
            else
            {
                Parameters.PrintfLogsExtended("向服务器请求添加权限:", "Connected: Failed!");
            }
            Thread.Sleep(1000);
            if (JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count > 0)
            {
                EndID = Convert.ToInt32(JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows[JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count - 1][0].ToString());
            }
            DataRow rw = JsonInterFace.PrivilegeManageClass.PrivilegeTable.NewRow();
            rw[0] = (EndID + 1).ToString();
            rw[1] = txtChrild.Text.Trim();
            rw[2] = ParentId;
            rw[3] = ParentId + "-" + txtChrild.Text.Trim();
            JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Add(rw);
            this.Close();
        }
        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
                if (btnNo.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnNo.Focus();
        }
    }
}
