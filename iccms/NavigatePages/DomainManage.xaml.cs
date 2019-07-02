using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace iccms.NavigatePages
{
    /// <summary>
    /// ManageField.xaml 的交互逻辑
    /// </summary>
    public partial class DomainManage : Window
    {
        private BindTreeView bindtreeview = new BindTreeView();
        private object LanguageClass = null;
        private System.Timers.Timer GetSelfWinHandle = null;
        public DomainManage()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (GetSelfWinHandle == null)
            {
                GetSelfWinHandle = new System.Timers.Timer();
                GetSelfWinHandle.Interval = 1000;
                GetSelfWinHandle.AutoReset = false;
                GetSelfWinHandle.Elapsed += GetSelfWinHandle_Elapsed;
            }
        }

        private void GetSelfWinHandle_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                new Thread(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        while (true)
                        {
                            Parameters.DomainManageWinHandle = (IntPtr)(Parameters.FindWindow(null, this.Title));
                            if (Parameters.DomainManageWinHandle != IntPtr.Zero)
                            {
                                break;
                            }
                            else
                            {
                                Thread.Sleep(1000);
                            }
                        }
                    });
                }).Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
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
            try
            {
                //重载设备
                if (msg == Parameters.WM_DomainManageDeviceReloadEven)
                {
                    loadDeviceListTreeView();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //载入设备列表
            loadDeviceListTreeView();

            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                LanguageClass = new Language_CN.ManageField();
                this.DataContext = (Language_CN.ManageField)LanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                LanguageClass = new Language_EN.ManageField();
                this.DataContext = (Language_EN.ManageField)LanguageClass;
            }
            txtSourceDomainNodeName.DataContext = Parameters.DomainActionInfoClass;

            //获取窗口句柄
            GetSelfWinHandle.Start();
            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("域管理"))
                    {
                        btnAdd.IsEnabled = false;
                        btnDelete.IsEnabled = false;
                    }
                    else
                    {
                        btnAdd.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["域管理"]));
                        btnDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["域管理"]));
                    }
                }
            }
            #endregion
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtSourceDomainNodeName.Text.Trim() == "")
            {
                MessageBox.Show("请指定要添加域名的位置！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!txtDomainNodeName.Text.Trim().Equals(""))
            {
                //添加域结点
                if ((Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.StructureNode.ToString()) || Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.RootNode.ToString()))
                    && Parameters.DomainActionInfoClass.IsStation.Equals("0"))
                {
                    if (MessageBox.Show("确定添加域名吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        try
                        {
                            Parameters.DomainActionInfoClass.SelfName = txtDomainNodeName.Text.Trim();
                            Parameters.DomainActionInfoClass.NodeContent = txtDomainNodeName.Text.Trim();
                            Parameters.DomainActionInfoClass.IsStation = "0";
                            Parameters.DomainActionInfoClass.NodeType = NodeType.StructureNode.ToString();
                            Parameters.DomainActionInfoClass.NodeIcon = new NodeIcon().StructureCloseNodeIcon;
                            NetWorkClient.ControllerServer.Send(JsonInterFace.AddDomainNodeName(Parameters.DomainActionInfoClass.PathName, Parameters.DomainActionInfoClass.SelfName, Convert.ToInt32(Parameters.DomainActionInfoClass.IsStation), ""));
                            txtDomainNodeName.Text = "";
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }
                }
                else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.StructureNode.ToString())
                    && Parameters.DomainActionInfoClass.IsStation.Equals("1"))
                {
                    MessageBox.Show("站点中只能添加设备,要添加设备，请在设备管理中操作！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.LeafNode.ToString()))
                {
                    MessageBox.Show("不能在设备中添加域名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("请输入域名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtDomainNodeName.Text = (sender as TextBlock).Text;
        }

        private void miRefresh_Click(object sender, RoutedEventArgs e)
        {
            DeviceListTreeView.ItemsSource = null;
            DeviceListTreeView.Items.Refresh();
            loadDeviceListTreeView();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSourceDomainNodeName.Text.Trim() == "")
                {
                    MessageBox.Show("请指定要添加域名的位置！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (DeviceListTreeView.SelectedItem != null)
                {

                    string NodeName = ((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                    int SelfID = Convert.ToInt32(((CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id);
                    int ParentID = 0;
                    string FullNodeName = string.Empty;
                    bool isStation = false;
                    string TreeNodeType = string.Empty;

                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(NodeName) && int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString()).Equals(SelfID))
                        {
                            FullNodeName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                            ParentID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][3].ToString());
                            isStation = Convert.ToBoolean(int.Parse(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString()));
                            TreeNodeType = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString();
                            break;
                        }
                    }

                    if (TreeNodeType.Equals(NodeType.StructureNode.ToString()))
                    {
                        try
                        {
                            if (MessageBox.Show("确定删除[" + Parameters.DomainActionInfoClass.PathName + "]吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning).Equals(MessageBoxResult.OK))
                            {
                                Parameters.DomainActionInfoClass.PathName = FullNodeName;
                                Parameters.DomainActionInfoClass.SelfID = SelfID;
                                Parameters.DomainActionInfoClass.SelfName = NodeName;
                                Parameters.DomainActionInfoClass.ParentID = ParentID;
                                btnDelete.Tag = "Delete";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDomainNameRequest(FullNodeName));
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }
                    else if (Parameters.DomainActionInfoClass.NodeType.Equals(NodeType.RootNode.ToString()))
                    {
                        MessageBox.Show("[设备]域不允许删除！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("选择的内容不是域名！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("请选择要删除的域名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// ESC退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        //设备列表邦定
        private void loadDeviceListTreeView()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                CheckBoxTreeModel treeModel = new CheckBoxTreeModel();
                List<CheckBoxTreeModel> _usrdomainData = new List<CheckBoxTreeModel>();
                BindCheckBoxTreeView devicetreeview = new BindCheckBoxTreeView();
                DeviceListTreeView.ItemsSource = null;
                DeviceListTreeView.Items.Refresh();
                DataRow[] dr = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Copy().Select("NodeType='" + NodeType.RootNode.ToString() + "' or NodeType='" + NodeType.StructureNode.ToString() + "'");
                devicetreeview.Dt = dr[0].Table.Clone();
                foreach (DataRow _dr in dr)
                {
                    devicetreeview.Dt.ImportRow(_dr);
                }
                devicetreeview.DeviceTreeViewBind(ref treeModel);
                if (treeModel.Children.Count > 0)
                {
                    _usrdomainData.Add(treeModel);
                    DeviceListTreeView.ItemsSource = _usrdomainData;
                }
                //DeviceListTreeView.ItemsSource = JsonInterFace.UsrdomainData;

                if (btnDelete.Tag != null)
                {
                    if (btnDelete.Tag.ToString() == "Delete")
                    {
                        Parameters.DomainActionInfoClass.SelfID = -1;
                        Parameters.DomainActionInfoClass.SelfName = string.Empty;
                        Parameters.DomainActionInfoClass.NodeContent = string.Empty;
                        Parameters.DomainActionInfoClass.ParentID = -1;
                        Parameters.DomainActionInfoClass.PathName = string.Empty;
                        Parameters.DomainActionInfoClass.IsDeleted = false;
                        Parameters.DomainActionInfoClass.NodeType = string.Empty;
                        Parameters.DomainActionInfoClass.Permission = "Enable";
                        Parameters.DomainActionInfoClass.NodeIcon = string.Empty;
                        Parameters.DomainActionInfoClass.IsStation = string.Empty;
                        btnDelete.Tag = string.Empty;
                    }
                }
            }));
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

        private void DeviceListTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (DeviceListTreeView.SelectedItem != null)
                {
                    string NodeName = ((DataInterface.CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Name;
                    string SelfID = ((DataInterface.CheckBoxTreeModel)DeviceListTreeView.SelectedItem).Id;
                    for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                    {
                        if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString().Equals(NodeName) && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString().Equals(SelfID))
                        {
                            Parameters.DomainActionInfoClass.SelfID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count - 1][1].ToString()) + 1;
                            Parameters.DomainActionInfoClass.SelfName = string.Empty;
                            Parameters.DomainActionInfoClass.NodeContent = string.Empty;
                            Parameters.DomainActionInfoClass.ParentID = Convert.ToInt32(JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString());
                            Parameters.DomainActionInfoClass.PathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                            Parameters.DomainActionInfoClass.IsDeleted = false;
                            Parameters.DomainActionInfoClass.NodeType = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString();
                            Parameters.DomainActionInfoClass.Permission = "Enable";
                            Parameters.DomainActionInfoClass.NodeIcon = new NodeIcon().StructureCloseNodeIcon;
                            Parameters.DomainActionInfoClass.IsStation = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][4].ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("域管理", ex.Message, ex.StackTrace);
            }
        }
    }
}
