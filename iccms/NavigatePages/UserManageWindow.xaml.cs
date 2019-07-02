using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace iccms.NavigatePages
{
    public class UserGroupParamInfo
    {
        private string userName;
        private string userGroup;
        private string userPassword;
        private string otherName;
        private string builder;
        public string UserName { get { return userName; } set { userName = value; } }
        public string UserGroup { get { return userGroup; } set { userGroup = value; } }
        public string UserPassword { get { return userPassword; } set { userPassword = value; } }
        public string OtherName { get { return otherName; } set { otherName = value; } }
        public string Builder { get { return builder; } set { builder = value; } }
    }
    public class SetRoleClass
    {
        public static bool SetRolePrivilege = false;
        public void SetRole(bool isChecked)
        {
            if(isChecked)
            {
                SetRolePrivilege = true;
            }
            else
            {
                SetRolePrivilege = false;
            }
        }
    }
    public class TreeModel : INotifyPropertyChanged
    {
        #region 私有变量
        /// <summary>
        /// Id值
        /// </summary>
        private string _id;
        /// <summary>
        /// 父ID
        /// </summary>
        private string _parentID;
        /// <summary>
        /// 全名
        /// </summary>
        private string _fullName;
        /// <summary>
        /// 显示的名称
        /// </summary>
        private string _name;
        /// <summary>
        /// 是否为站点
        /// </summary>
        private string _isStation;
        /// <summary>
        /// 对应设备
        /// </summary>
        private string _nodeType;
        /// <summary>
        /// 图标路径
        /// </summary>
        private string _icon;
        /// <summary>
        /// 选中状态
        /// </summary>
        private bool _isChecked;
        /// <summary>
        /// 读状态
        /// </summary>
        private bool _isRead;
        /// <summary>
        /// 写状态
        /// </summary>
        private bool _isWrite;
        /// <summary>
        /// 折叠状态
        /// </summary>
        private bool _isExpanded;
        /// <summary>
        /// 子项
        /// </summary>
        private IList<TreeModel> _children;
        /// <summary>
        /// 结点属性
        /// </summary>
        private string _selfNodeType;
        /// <summary>
        /// 父项
        /// </summary>
        private TreeModel _parent;
        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public TreeModel()
        {
            Children = new List<TreeModel>();
            _fullName = "";
            _isChecked = false;
            IsExpanded = false;
            _icon = "";
        }

        /// <summary>
        /// 键值
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 父ID值
        /// </summary>
        public string ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }


        /// <summary>
        /// 显示的字符
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 显示是否为站点
        /// </summary>
        public string IsStation
        {
            get { return _isStation; }
            set { _isStation = value; }
        }
        /// <summary>
        /// 显示是否为站点
        /// </summary>
        public string NodeType
        {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// 指针悬停时的显示说明
        /// </summary>
        public string ToolTip
        {
            get
            {
                return String.Format("{0}-{1}", Id, FullName);
            }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    NotifyPropertyChanged("IsChecked");
                    //如果选中
                    if (_isChecked)
                    {
                        IsRead = true;
                        //如果存在子类且为手动状态则子类也应该选中）
                        if (Children != null && SetRoleClass.SetRolePrivilege) 
                        {
                            //如果取消选中子项也应该取消选中
                            foreach (TreeModel child in Children) 
                            {
                                child.IsChecked = true;
                            }
                        }
                    }
                    //如果不选中
                    else
                    {
                        IsRead = false;
                        IsWrite = false;
                        if (Children != null)
                        {
                            //如果取消选中子项也应该取消选中
                            foreach (TreeModel child in Children)
                            {
                                child.IsChecked = false;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 是否可读
        /// </summary>
        public bool IsRead
        {
            get
            {
                return _isRead;
            }
            set
            {
                //被选中
                //if (_isChecked)
                {
                    if (value != _isRead)
                    {
                        _isRead = value;
                        NotifyPropertyChanged("IsRead");
                        if (!_isRead)
                        {
                            IsWrite = false;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 是否可写
        /// </summary>
        public bool IsWrite
        {
            get
            {
                return _isWrite;
            }
            set
            {
                if (value != _isWrite)
                {
                    _isWrite = value;
                    NotifyPropertyChanged("IsWrite");
                    //如果选中
                    if (_isWrite)
                    {
                        //如果存在子类，则子类也应该选中
                        if (Children != null)
                        {
                            //如果取消选中子项也应该取消选中
                            foreach (TreeModel child in Children)
                            {
                                if (child.IsChecked)
                                {
                                    child.IsWrite = true;
                                }
                            }
                        }
                    }
                    //如果不选中
                    else
                    {
                        _isWrite = false;
                        if (Children != null)
                        {
                            //如果取消选中子项也应该取消选中
                            foreach (TreeModel child in Children)
                            {
                                child.IsWrite = false;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    //折叠状态改变
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        /// <summary>
        /// 父项
        /// </summary>
        public TreeModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        public string SelfNodeType
        {
            get
            {
                return _selfNodeType;
            }

            set
            {
                _selfNodeType = value;
            }
        }
        /// <summary>
        /// 子项
        /// </summary>
        public IList<TreeModel> Children
        {
            get { return _children; }
            set { _children = value; }
        }
        
        /// <summary>
        /// 设置所有子项的选中状态
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetChildrenChecked(bool isChecked)
        {
            if (Children != null)
            {
                foreach (TreeModel child in Children)
                {
                    child.IsChecked = IsChecked;
                    child.SetChildrenChecked(IsChecked);
                }
            }
        }

        /// <summary>
        /// 设置所有子项展开状态
        /// </summary>
        /// <param name="isExpanded"></param>
        public void SetChildrenExpanded(bool isExpanded)
        {
            if (Children != null)
            {
                foreach (TreeModel child in Children)
                {
                    child.IsExpanded = isExpanded;
                    child.SetChildrenExpanded(isExpanded);
                }
            }
        }

        /// <summary>
        /// 属性改变事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

    /// <summary>
    /// UserManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserManageWindow : Window
    {
        private object UserManageLanguageClass = null;
        private UserGroupParamInfo usergroupParam = new UserGroupParamInfo();
        private IList<TreeModel> _itemsSourceData = new List<TreeModel>();
        private IList<TreeModel> _usrdomainData = new List<TreeModel>();
        BindTreeView bindtreeview = new BindTreeView();
        private static Thread ReLoadDeviceListsThread = null;
        private static ObservableCollection<UsergroupManage> UsergroupManageTable = new ObservableCollection<UsergroupManage>();
        private static Thread LoadGroupListsThread = null;
        public  SetRoleClass SetRole = new SetRoleClass();
        public static bool bol = false;
        public class BindTreeView
        {
            private DataTable dt;
            public BindTreeView()
            {

            }
            public DataTable Dt
            {
                get
                {
                    return dt;
                }

                set
                {
                    dt = value;
                }
            }
            public void TreeViewBind(TreeModel tr)
            {
                //tr.Items.Clear();//先清理一下节点
                tr.Children.Clear();

                if (Dt != null || Dt.Rows.Count > 0)
                {
                    //首先遍历父节点
                    //TreeModel item;
                    DataRow[] dr = Dt.Select("aliasName='-1'");
                    if (dr.Length != 0)
                    {
                        foreach (DataRow drTemp in dr)
                        {
                            tr.Id = drTemp["priId"].ToString();     //一般绑定ID
                            tr.Name = drTemp["funName"].ToString();    //名称
                            tr.FullName = drTemp["funName"].ToString();//全名
                            tr.IsStation = "";
                            tr.NodeType = "";
                            tr.IsChecked = false;
                            tr.IsExpanded = true;
                            BindNode(tr);
                        }
                    }
                }
            }
            /// <summary>
            /// 遍历所有的子节点
            /// </summary>
            /// <param name="item">父节点的item</param>
            private void BindNode(TreeModel item)
            {
                DataRow[] dr = Dt.Select("aliasName = '" + item.FullName + "'"); //查询子节点条件
                if (dr.Length > 0)
                {
                    foreach (DataRow drTemp in dr)
                    {
                        TreeModel childItem = new TreeModel();
                        childItem.Name = drTemp["funName"].ToString();
                        childItem.Id = drTemp["priId"].ToString();
                        childItem.FullName = drTemp["funName"].ToString();//全名
                        childItem.IsStation = "";
                        childItem.NodeType = "";
                        childItem.IsChecked = false;
                        childItem.IsExpanded = true;
                        childItem.Parent = item;
                        item.Children.Add(childItem);
                        BindNode(childItem);
                    }
                }
            }

            public void DeviceTreeViewBind(TreeModel tr)
            {
                //tr.Items.Clear();//先清理一下节点
                tr.Children.Clear();

                if (Dt != null || Dt.Rows.Count > 0)
                {
                    DataRow[] dr = Dt.Select("ParentID='-1'");
                    if (dr.Length != 0)
                    {
                        foreach (DataRow drTemp in dr)
                        {
                            tr.Id = drTemp["SelfID"].ToString();     //一般绑定ID
                            tr.Name = drTemp["SelfName"].ToString();    //名称
                            tr.ParentID = drTemp["ParentID"].ToString();
                            tr.FullName = drTemp["PathName"].ToString();//全名
                            tr.IsChecked = false;
                            tr.IsExpanded = true;
                            tr.Icon = new NodeIcon().RootNodeCloseIcon; //父结点图标
                            tr.IsStation = drTemp["IsStation"].ToString();//站点
                            tr.NodeType = drTemp["NodeType"].ToString();//设备
                            DeviceBindNode(tr);
                        }
                    }
                }
            }
            /// <summary>
            /// 遍历所有的子节点
            /// </summary>
            /// <param name="item">父节点的item</param>
            private void DeviceBindNode(TreeModel item)
            {
                DataRow[] dr = Dt.Select("ParentID = '" + item.Id + "'"); //查询子节点条件
                if (dr.Length > 0)
                    foreach (DataRow drTemp in dr)
                    {
                        TreeModel childItem = new TreeModel();
                        childItem.Name = drTemp["SelfName"].ToString();
                        childItem.Id = drTemp["SelfID"].ToString();
                        childItem.FullName = drTemp["PathName"].ToString();//全名
                        childItem.IsStation = drTemp["IsStation"].ToString();//站点
                        childItem.NodeType = drTemp["NodeType"].ToString();//设备
                        childItem.IsChecked = false;
                        childItem.IsExpanded = true;
                        if (drTemp["IsStation"].ToString().Equals("1"))
                        {
                            childItem.Icon = new NodeIcon().StationNodeIcon;
                        }
                        else
                        {
                            childItem.Icon = new NodeIcon().StructureOpenNodeIcon;
                        }
                        item.Children.Add(childItem);
                        DeviceBindNode(childItem);
                    }
            }
        }
        /// <summary>
        /// 控件数据
        /// </summary>
        public IList<TreeModel> ItemsSourceData
        {
            get { return _itemsSourceData; }
            set
            {
                _itemsSourceData = value;
            }
        }
        public IList<TreeModel> UsrdomainData
        {
            get { return _usrdomainData; }
            set
            {
                _usrdomainData = value;
            }
        }
        public UserGroupParamInfo UserGroupParam { get { return usergroupParam; } set { usergroupParam = value; } }
        public UserManageWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            SetRole.SetRole(false);
            try
            {
                #region 数据库刚创建，需增加权限权
                string[] temFunName = DefaultPrivilege.FunName.Split(new char[] { ',' });
                string[] temAliasName = DefaultPrivilege.AliasName.Split(new char[] { ',' });
                if (JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count <= 0)
                {
                    for (int i = 0; i < temFunName.Length; i++)
                    {
                        DataRow tmpRW = JsonInterFace.PrivilegeManageClass.PrivilegeTable.NewRow();
                        DefaultPrivilege.Des = "DefaultPrivilege";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_privilege_request(temFunName[i], temAliasName[i], "0"));
                        tmpRW[0] = (i + 1).ToString();
                        tmpRW[1] = temFunName[i];
                        tmpRW[2] = temAliasName[i];
                        tmpRW[3] = "0";
                        JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Add(tmpRW);
                    }
                }
                else
                {
                    if (temFunName.Length > JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count)
                    {
                        int priId = int.Parse(JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows[JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count - 1][0].ToString()) + 1;
                        for (int i = 0; i < temFunName.Length; i++)
                        {
                            DataRow[] dr = JsonInterFace.PrivilegeManageClass.PrivilegeTable.Select("funName='" + temFunName[i] + "'");
                            if (dr.Length == 0)
                            {
                                DataRow tmpRW = JsonInterFace.PrivilegeManageClass.PrivilegeTable.NewRow();
                                DefaultPrivilege.Des = "DefaultPrivilege";
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Add_privilege_request(temFunName[i], temAliasName[i], "0"));
                                tmpRW[0] = priId.ToString();
                                tmpRW[1] = temFunName[i];
                                tmpRW[2] = temAliasName[i];
                                tmpRW[3] = "0";
                                JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Add(tmpRW);
                                priId++;
                            }
                        }
                    }
                }
                #endregion
                #region 创建电围组
                if (NetWorkClient.ControllerServer.Connected)
                {
                    bool isExist = false;
                    for (int i = 0; i < JsonInterFace.RoleManageList.Count; i++)
                    {
                        if (JsonInterFace.RoleManageList[i].Name.Equals("电围组"))
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        Parameters.ConfigType = "";
                        JsonInterFace.RoleManageInfo.Name = "电围组";
                        JsonInterFace.RoleManageInfo.TimeStart = "1970-01-01 00:00:00";
                        JsonInterFace.RoleManageInfo.TimeEnd = "3000-01-01 00:00:00";
                        JsonInterFace.RoleManageInfo.RoleType = "Operator";
                        JsonInterFace.RoleManageInfo.Des = "";
                        JsonInterFace.RoleManageInfo.IsRead = "30,31,41,42,32,33,35,36,37,5,8,2,20,9,10,22,23,11,24,25,12,13,3,18,19,4,1";
                        JsonInterFace.RoleManageInfo.IsWrite = "1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1";
                        JsonInterFace.RoleManageInfo.AliasName = "电围组";

                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_role_Request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.RoleType, JsonInterFace.RoleManageInfo.TimeStart, JsonInterFace.RoleManageInfo.TimeEnd, ""));
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_group_privilege_request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.IsRead, JsonInterFace.RoleManageInfo.IsWrite));
                    }
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求增加电围组:", "Connected: Failed!");
                }
                #endregion

                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_roletype_request());//请求用户组类型
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求用户组类型:", "Connected: Failed!");
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_user_Request());//请求用户
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求用户:", "Connected: Failed!");
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.All_usr_domain_request());//请求用户-域
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求用户-域:", "Connected: Failed!");
                }
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("用户管理初始化", ex.Message, ex.StackTrace);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                UserManageLanguageClass = new Language_CN.UserManage();
                this.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
                txtUserName.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
                txtUserGroup.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
                txtUserPassword.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
                txtUserAlias.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
                txtUserManufacturer.DataContext = (Language_CN.UserManage)UserManageLanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                UserManageLanguageClass = new Language_EN.UserManage();
                this.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
                txtUserName.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
                txtUserGroup.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
                txtUserPassword.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
                txtUserAlias.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
                txtUserManufacturer.DataContext = (Language_EN.UserManage)UserManageLanguageClass;
            }
            //用户-用户组列表
            if(ReLoadDeviceListsThread==null)
            {
                ReLoadDeviceListsThread = new Thread(new ThreadStart(AddUserManager));
                ReLoadDeviceListsThread.Start();
            }            
            dgUserManage.ItemsSource = null;
            dgUserManage.Items.Refresh();
            dgUserManage.ItemsSource = UsergroupManageTable;

            //用户组列表
            if (LoadGroupListsThread==null)
            {
                LoadGroupListsThread = new Thread(new ThreadStart(AddRoleList));
                LoadGroupListsThread.Start();
            }
            lbRoleList.ItemsSource = JsonInterFace.RoleManageList;
            ckbSetRolePrivilege.IsChecked = false;

            //用户组权限
            LoadFuncListTreeView();

            //用户域
            LoadDeviceListTreeView();
            //FuncListTreeView.IsEnabled = false;

            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (!RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (int.Parse(RoleTypeClass.RoleType) > 1)
                    {
                        //用户组
                        tiUserGroup.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    if (int.Parse(RoleTypeClass.RoleType) > 2)
                    {
                        //用户
                        tiAccountNumber.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
            #endregion
        }
        private void EnumVisual(Visual myVisual, bool value)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(myVisual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                if (childVisual != null)
                {
                    if (childVisual is Button)
                    {
                        if ((childVisual as Button).Name != "" && (childVisual as Button).Name != "btnClose")
                        {
                            (childVisual as Button).IsEnabled = value;
                        }
                    }
                    EnumVisual(childVisual, value);
                }
            }
        }
        private void AddUserManager()
        {
            while (true)
            {
                try
                {
                    if (JsonInterFace.UsergroupManageClass.UsergroupTable.Rows.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (UsergroupManageTable.Count + 2 > JsonInterFace.UsergroupManageClass.UsergroupTable.Rows.Count) 
                            {
                                UsergroupManageTable.Clear();
                            }
                            for (int i = 0; i < JsonInterFace.UsergroupManageClass.UsergroupTable.Rows.Count; i++)
                            {
                                bool Flag = true;
                                DataRow dr = JsonInterFace.UsergroupManageClass.UsergroupTable.Rows[i];
                                if (dr["user_UserName"].ToString() == "engi" || dr["user_UserName"].ToString() == "root")
                                {
                                    Flag = false;
                                }
                                else
                                {
                                    for (int j = 0; j < UsergroupManageTable.Count; j++)
                                    {
                                        if (dr["user_UserName"].ToString() == UsergroupManageTable[j].UserName &&
                                           dr["user_UserGroup"].ToString() == UsergroupManageTable[j].RoleName &&
                                           dr["user_Alias"].ToString() == UsergroupManageTable[j].Des &&
                                           dr["user_Manufacturer"].ToString() == UsergroupManageTable[j].Manufacturer)
                                        {
                                            Flag = false;
                                            break;
                                        }
                                    }
                                }
                                if (Flag)
                                {
                                    UsergroupManageTable.Add(new UsergroupManage()
                                    {
                                        UserName = dr["user_UserName"].ToString(),
                                        RoleName = dr["user_UserGroup"].ToString(),
                                        Des = dr["user_Alias"].ToString(),
                                        Manufacturer = dr["user_Manufacturer"].ToString()
                                    });
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("用户列表查询", ex.Message, ex.StackTrace);
                }
                Thread.Sleep(1000);
            }
        }
        private void AddRoleList()
        {
            while(true)
            {
                try
                {
                    if (JsonInterFace.RoleManageList.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lbRoleList.Items.Refresh();
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message);
                }
                Thread.Sleep(1000);
            }
        }
        private void LoadFuncListTreeView()
        {
            TreeModel treeModel = new TreeModel();
            bindtreeview.Dt = JsonInterFace.PrivilegeManageClass.PrivilegeTable;
            bindtreeview.TreeViewBind(treeModel);
            if (treeModel.Id != null)
            {
                _itemsSourceData.Clear();
                FuncListTreeView.ItemsSource = null;
                FuncListTreeView.Items.Refresh();
                _itemsSourceData.Add(treeModel);
                FuncListTreeView.ItemsSource = _itemsSourceData;
            }
        }
        //DataRow[]转换成DataTable
        private DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone(); // 复制DataRow的表结构
            foreach (DataRow row in rows)
            {

                tmp.ImportRow(row); // 将DataRow添加到DataTable中
            }
            return tmp;
        }
        private void LoadDeviceListTreeView()
        {
            Dispatcher.Invoke(new Action(() =>
            {

                DataTable TempDevicedt = new DataTable();
                BindTreeView devicetreeview = new BindTreeView();
                TreeModel treeModel = new TreeModel();
                DeviceListTreeView.ItemsSource = null;
                DeviceListTreeView.Items.Clear();
                TempDevicedt = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Copy();
                DataRow[] dr = TempDevicedt.Select("NodeType='" + NodeType.RootNode.ToString() + "' or NodeType='" + NodeType.StructureNode.ToString() + "'");
                //devicetreeview.Dt = JsonInterFace.BindUserDomainTreeViewClass.DeviceTreeTable;
                devicetreeview.Dt = ToDataTable(dr);
                devicetreeview.DeviceTreeViewBind(treeModel);
                if (treeModel.Children.Count > 0)
                {
                    _usrdomainData.Add(treeModel);
                    DeviceListTreeView.ItemsSource = _usrdomainData;
                }
            }));
        }

        private DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
        }
        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullName = string.Empty;
                List<TreeModel> itemlist = (List<TreeModel>)GetCheckedItemsIgnoreRelation(_usrdomainData);
                if (itemlist.Count > 0)
                {
                    for (int i = 0; i < itemlist.Count - 1; i++)
                    {
                        fullName += itemlist[i].FullName + ",";
                    }
                    fullName += itemlist[itemlist.Count - 1].FullName;
                }
                AddUserInfor AddUserInforFrm = new AddUserInfor(fullName);
                AddUserInforFrm.ShowDialog();
                if (AddUserInforFrm.bolresult)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Add_user_Request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.Password, JsonInterFace.AddUserManageList.BuildName));
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Add_user_group_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.RoleType, JsonInterFace.AddUserManageList.BuildName));
                    if (JsonInterFace.AddUserManageList.DomainIdSet != null && JsonInterFace.AddUserManageList.DomainIdSet != "")
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_usr_domain_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.DomainIdSet, JsonInterFace.AddUserManageList.Des));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("添加用户", ex.Message, ex.StackTrace);
            }
        }
        private void btnUpdateUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<TreeModel> itemlist = (List<TreeModel>)GetCheckedItemsIgnoreRelation(_usrdomainData);
                if (dgUserManage.SelectedCells.Count > 0)
                {
                    JsonInterFace.AddUserManageList.Name = "";
                    JsonInterFace.AddUserManageList.RoleType = "";
                    JsonInterFace.AddUserManageList.Password = "";
                    JsonInterFace.AddUserManageList.Des = "";
                    JsonInterFace.AddUserManageList.BuildName = "";
                    JsonInterFace.AddUserManageList.DomainIdSet = "";
                    MessageBoxResult dr = MessageBox.Show("确定要更新此用户吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.OK)
                    {
                        UsergroupManage Row = (UsergroupManage)dgUserManage.SelectedCells[0].Item;
                        if (Row.UserName== JsonInterFace.LoginUserInfo[0].LoginUser)
                        {
                            MessageBox.Show("此用户正在使用，不支持更新操作", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        if (itemlist.Count > 0)
                        {
                            for (int i = 0; i < itemlist.Count - 1; i++)
                            {
                                JsonInterFace.AddUserManageList.DomainIdSet += itemlist[i].FullName + ",";
                            }
                            JsonInterFace.AddUserManageList.DomainIdSet += itemlist[itemlist.Count - 1].FullName;
                        }
                        JsonInterFace.AddUserManageList.Name = Row.UserName;
                        JsonInterFace.AddUserManageList.BuildName = Row.Manufacturer;
                        DataTable dt = JsonInterFace.UsrdomainManageClass.UsrDomainTable.Copy();
                        DataRow[] _dataRow = dt.Select("name='" + JsonInterFace.AddUserManageList.Name + "'");

                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "UpdateUser";
                            //存在就更新
                            if (_dataRow.Length > 0)
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Update_usr_domain_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.DomainIdSet, ""));
                            }
                            //不存在就添加
                            else
                            {
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Add_usr_domain_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.DomainIdSet, ""));
                            }
                        }
                        else
                        {
                            Parameters.PrintfLogsExtended("向服务器请求更新权限:", "Connected: Failed!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选择需要更新的用户", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("更新用户-域", ex.Message, ex.StackTrace);
            }
        }
        private void btnDeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (dgUserManage.SelectedCells.Count > 0)
            {
                MessageBoxResult dr = MessageBox.Show("确定要删除此用户吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    try
                    {
                        UsergroupManage Row = (UsergroupManage)dgUserManage.SelectedCells[0].Item;
                        if (Row.UserName == JsonInterFace.LoginUserInfo[0].LoginUser)
                        {
                            MessageBox.Show("此用户正在使用，不能删除", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        JsonInterFace.DelUserManageList.Name = Row.UserName;
                        JsonInterFace.DelUserManageList.RoleType = Row.RoleName;
                        //请求删除数据
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "DeleteUser";
                            //删除用户
                            Parameters.SendMessage(Parameters.UserManagerWinHandle, Parameters.WM_DelUserManagerResponse, 0, 0);
                        }
                        else
                        {
                            Parameters.PrintfLogsExtended("向服务器请求用户列表:", "Connected: Failed!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended("删除用户", ex.Message, ex.StackTrace);
                    }
                }
            }
            else
            {
                MessageBox.Show("请选择需要删除的用户", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
        private void btnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddUserGroup AddUserGroupFrm = new AddUserGroup(GetCheckedItemsIgnoreRelation(_itemsSourceData));
            AddUserGroupFrm.ShowDialog();
        }
        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lbRoleList.SelectedIndex >= 0)
                {
                    MessageBoxResult dr = MessageBox.Show("确定要删除用户组吗?" + JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString(), "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (dr == MessageBoxResult.OK)
                    {
                        if(JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString()== JsonInterFace.LoginUserInfo[0].WorkGroup)
                        {
                            MessageBox.Show("此用户组正在使用，不能删除", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        //请求删除用户组及权限
                        if (NetWorkClient.ControllerServer.Connected)
                        {
                            Parameters.ConfigType = "DeleteRole";
                            JsonInterFace.RoleManageInfo.Name = JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString();
                            //删除用户组
                            NetWorkClient.ControllerServer.Send(JsonInterFace.Del_role_Request(JsonInterFace.RoleManageInfo.Name));
                        }
                        else
                        {
                            Parameters.PrintfLogsExtended("向服务器请求删除用户组及权限:", "Connected: Failed!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请先选中需要删除的用户组", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("删除用户组", ex.Message, ex.StackTrace);
            }
        }
        private void btnUpdateGroup_Click(object sender, RoutedEventArgs e)
        {
            string tmppriIdSet = string.Empty;
            string priIdSetReadorWrite = string.Empty;
            bool bol = true;

            if (lbRoleList.SelectedIndex >= 0)
            {
                MessageBoxResult dr = MessageBox.Show("确定要更新用户组权限吗?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    try
                    {
                        if (JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString() == JsonInterFace.LoginUserInfo[0].WorkGroup)
                        {
                            MessageBox.Show("此用户组正在使用，不支持更新操作", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return;
                        }
                        if (_itemsSourceData.Count > 0)
                        {
                            List<TreeModel> itemlist = (List<TreeModel>)CheckedItemsIgnoreRelation();
                            if (itemlist.Count > 0)
                            {
                                for (int i = 0; i < itemlist.Count - 1; i++)
                                {
                                    tmppriIdSet += itemlist[i].Id + ",";
                                    priIdSetReadorWrite += Convert.ToInt32(itemlist[i].IsWrite).ToString() + ",";
                                }
                                tmppriIdSet += itemlist[itemlist.Count - 1].Id;
                                priIdSetReadorWrite += Convert.ToInt32(itemlist[itemlist.Count - 1].IsWrite).ToString();
                                JsonInterFace.RoleManageInfo.IsRead = tmppriIdSet;
                                JsonInterFace.RoleManageInfo.IsWrite = priIdSetReadorWrite;
                                JsonInterFace.RoleManageInfo.Name = JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString();
                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    Parameters.ConfigType = "UpdateRole";
                                    //如果用户权限存在，则更新用户权限
                                    foreach (GroupprivilegeManage gpm in JsonInterFace.GroupprivilegeManageList)
                                    {
                                        if (gpm.GroupName.Equals(JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString()))
                                        {
                                            NetWorkClient.ControllerServer.Send(JsonInterFace.Update_group_privilege_request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.IsRead, JsonInterFace.RoleManageInfo.IsWrite));
                                            bol = false;
                                            break;
                                        }
                                    }
                                    //如果用户权限不存在，则增加用户权限
                                    if (bol)
                                    {
                                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_group_privilege_request(JsonInterFace.RoleManageInfo.Name, JsonInterFace.RoleManageInfo.IsRead, JsonInterFace.RoleManageInfo.IsWrite));
                                    }
                                }
                                else
                                {
                                    Parameters.PrintfLogsExtended("向服务器请求更新用户组权限:", "Connected: Failed!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("请先选择用户组可访问的权限", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("未添加用户组权限，请在右边空白处右键选择添加节点", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended("更新用户组权限", ex.Message, ex.StackTrace);
                    }
                }
            }
            else
            {
                MessageBox.Show("没有选中需要更新的用户组", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void tiUserGroup_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region 权限
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("用户组设置"))
                    {
                        tiUserGroup.Visibility = System.Windows.Visibility.Collapsed;
                        tabControl.SelectedIndex = 0;
                    }
                    else
                    {
                        tiUserGroup.Visibility = System.Windows.Visibility.Visible;
                        EnumVisual(this, Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["用户组设置"])));
                    }
                }
                #endregion
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("用户组初始化", ex.Message);
            }
        }
        private void tiAccountNumber_Loaded(object sender, RoutedEventArgs e)
        {
            #region 权限
            if (RoleTypeClass.RoleType.Equals("RoleType"))
            {
                if (!RoleTypeClass.RolePrivilege.ContainsKey("账号设置"))
                {
                    tiAccountNumber.Visibility = System.Windows.Visibility.Collapsed;
                    tabControl.SelectedIndex = 1;
                }
                else
                {
                    tabControl.SelectedIndex = 0;
                    tiAccountNumber.Visibility = System.Windows.Visibility.Visible;
                    EnumVisual(this, Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["账号设置"])));
                }
            }
            #endregion
        }

        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id, IList<TreeModel> treeList)
        {
            foreach (var tree in treeList)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }
        /// <summary>
        /// 设置对应Id的项为选中状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int SetCheckedById(string id)
        {
            foreach (var tree in ItemsSourceData)
            {
                if (tree.Id.Equals(id))
                {
                    tree.IsChecked = true;
                    return 1;
                }
                if (SetCheckedById(id, tree.Children) == 1)
                {
                    return 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        /// <returns></returns>
        public IList<TreeModel> CheckedItemsIgnoreRelation()
        {

            return GetCheckedItemsIgnoreRelation(_itemsSourceData);
        }
        private IList<TreeModel> GetCheckedTreeModel(IList<TreeModel> tmptreeModel, IList<TreeModel> treeModel)
        {
            if (tmptreeModel.Count > 0)
            {
                foreach (var tree in tmptreeModel)
                {
                    if (tree.Children.Count > 0)
                    {
                        GetCheckedTreeModel(tree.Children, treeModel);
                    }
                    treeModel.Add(tree);
                }
            }
            return treeModel;
        }
        /// <summary>
        /// 私有方法，忽略层次关系的情况下，获取选中项
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IList<TreeModel> GetCheckedItemsIgnoreRelation(IList<TreeModel> list)
        {
            IList<TreeModel> treeList = new List<TreeModel>();
            IList<TreeModel> returntreeList = new List<TreeModel>();
            treeList = GetCheckedTreeModel(list, treeList);
            if (treeList.Count > 0)
            {
                foreach (var tree in treeList)
                {
                    if (tree.IsChecked)
                    {
                        returntreeList.Add(tree);
                    }
                }
            }
            return returntreeList;
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        private void menuDelItemInSelect_Click(object sender, RoutedEventArgs e)
        {
            if (FuncListTreeView.SelectedItem != null)
            {
                TreeModel tree = (TreeModel)FuncListTreeView.SelectedItem;
                if (tree.Children.Count > 0)
                {
                    //是否需要删除子节点
                }
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Del_privilege_request(tree.Name));
                    //NetWorkClient.ControllerServer.Send(JsonInterFace.All_privilege_request());//请求权限
                }
                else
                {
                    Parameters.PrintfLogsExtended("向服务器请求更新用户组权限:", "Connected: Failed!");
                }
                Thread.Sleep(1000);
                for (int i = 0; i < JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.Count; i++)
                {
                    if (JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows[i][1].ToString() == tree.Name)
                    {
                        JsonInterFace.PrivilegeManageClass.PrivilegeTable.Rows.RemoveAt(i);
                        break;
                    }
                }
                LoadFuncListTreeView();
            }
        }

        /// <summary>
        /// 全部展开菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in FuncListTreeView.ItemsSource)
            {
                if (tree.Children.Count > 0)
                {
                    tree.IsExpanded = true;
                    tree.SetChildrenExpanded(true);
                }
            }
        }

        /// <summary>
        /// 全部折叠菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in FuncListTreeView.ItemsSource)
            {
                if (tree.Children.Count > 0)
                {
                    tree.IsExpanded = false;
                    tree.SetChildrenExpanded(false);
                }
            }
        }

        /// <summary>
        /// 全部选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in FuncListTreeView.ItemsSource)
            {
                if (tree.Children.Count > 0)
                {
                    tree.IsChecked = true;
                    tree.SetChildrenChecked(true);
                }
            }
        }

        /// <summary>
        /// 全部取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (TreeModel tree in FuncListTreeView.ItemsSource)
            {
                if (tree.Children.Count > 0)
                {
                    tree.IsChecked = false;
                    tree.SetChildrenChecked(false);
                }
            }
        }

        private void FuncListTreeView_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 鼠标右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }
        /// <summary>
        /// 设置tmptreeModel中id是否选中(bol)
        /// </summary>
        /// <param name="tmptreeModel"></param>
        /// <param name="id"></param>
        /// <param name="bol"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IList<TreeModel> SetCheckedTreeModel(IList<TreeModel> tmptreeModel, string id, bool bol, string type, string isReadWrite)
        {
            List<string> itemid = new List<string>();
            List<string> readwrite = new List<string>();
            for (int i = 0; i < id.Split(new char[] { ',' }).Length; i++)
            {
                itemid.Add(id.Split(new char[] { ',' })[i]);
                if (isReadWrite.Split(new char[] { ',' }).Length > i)
                {
                    readwrite.Add(isReadWrite.Split(new char[] { ',' })[i]);
                }
                else readwrite.Add(isReadWrite.Split(new char[] { ',' })[0]);
            }
            if (tmptreeModel.Count > 0)
            {
                for (int i = 0; i < tmptreeModel.Count; i++)
                {
                    //用ID来做判断条件
                    if (type.Equals("ID"))
                    {
                        if (tmptreeModel[i].Children.Count > 0)
                        {

                            if (itemid.Contains(tmptreeModel[i].Id))
                            {
                                tmptreeModel[i].IsChecked = bol;
                                for (int j = 0; j < itemid.Count; j++)
                                {
                                    if (itemid[j] == tmptreeModel[i].Id)
                                    {
                                        if (readwrite[j].Equals("1"))
                                        {
                                            tmptreeModel[i].IsWrite = true;
                                        }
                                        else
                                        {
                                            tmptreeModel[i].IsWrite = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                tmptreeModel[i].SetChildrenChecked(false);

                            }
                            SetCheckedTreeModel(tmptreeModel[i].Children, id, bol, type, isReadWrite);
                        }
                        else if (itemid.Contains(tmptreeModel[i].Id))
                        {
                            tmptreeModel[i].IsChecked = bol;
                            for (int j = 0; j < itemid.Count; j++)
                            {
                                if (itemid[j] == tmptreeModel[i].Id)
                                {
                                    if (readwrite[j].Equals("1"))
                                    {
                                        tmptreeModel[i].IsWrite = true;
                                    }
                                    else
                                    {
                                        tmptreeModel[i].IsWrite = false;
                                    }
                                }
                            }
                        }
                    }
                    //用FullName来做判断条件
                    else
                    {
                        if (tmptreeModel[i].Children.Count > 0)
                        {

                            if (itemid.Contains(tmptreeModel[i].FullName))
                            {
                                tmptreeModel[i].IsChecked = bol;
                            }
                            else
                            {
                                tmptreeModel[i].SetChildrenChecked(false);

                            }
                            SetCheckedTreeModel(tmptreeModel[i].Children, id, bol, type, isReadWrite);
                        }
                        else if (itemid.Contains(tmptreeModel[i].FullName))
                        {
                            tmptreeModel[i].IsChecked = bol;
                        }
                    }
                }
            }
            return tmptreeModel;
        }
        private void lbRoleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string roleListItem;
            string priIdSet = string.Empty;
            string isReadWrite = string.Empty;
            IList<TreeModel> tree = new List<TreeModel>();
            SetRole.SetRole(false);
            ckbSetRolePrivilege.IsChecked = false;
            //FuncListTreeView.IsEnabled = false;
            //FuncListTreeView.IsEnabled = true;
            if (lbRoleList.SelectedItem == null)
                return;
            if (lbRoleList.SelectedIndex < 5)
                btnDeleteGroup.IsEnabled = false;
            else
                btnDeleteGroup.IsEnabled = true;
            if (lbRoleList.SelectedIndex < 2)
                btnUpdateGroup.IsEnabled = false;
            else
                btnUpdateGroup.IsEnabled = true;
            roleListItem = JsonInterFace.RoleManageList[lbRoleList.SelectedIndex].Name.ToString();
            for (int i = 0; i < _itemsSourceData.Count; i++)
            {
                _itemsSourceData[i].IsChecked = false;
            }
            if (JsonInterFace.GroupprivilegeManageList.Count > 0)
            {
                foreach (GroupprivilegeManage groupprivilegeManage in JsonInterFace.GroupprivilegeManageList)
                {
                    if (groupprivilegeManage.GroupName == roleListItem)
                    {
                        priIdSet = groupprivilegeManage.PriIdSet;
                        isReadWrite = groupprivilegeManage.Des;
                    }
                }

                FuncListTreeView.ItemsSource = SetCheckedTreeModel(_itemsSourceData, priIdSet, true, "ID", isReadWrite);
            }
        }

        private void menuAddItemInSelect_Click(object sender, RoutedEventArgs e)
        {
            TreeModel tree = (TreeModel)FuncListTreeView.SelectedItem;
            AddTreeItem AddTreeItemFrm = new AddTreeItem(tree);
            AddTreeItemFrm.ShowDialog();
            LoadFuncListTreeView();
            //if (NetWorkClient.ControllerServer.Connected)
            //{
            //    NetWorkClient.ControllerServer.Send(JsonInterFace.All_privilege_request());//请求权限
            //}
            //else
            //{
            //    Parameters.PrintfLogsExtended("向服务器请求权限:", "Connected: Failed!");
            //}
        }

        private void miRefresh_Click(object sender, RoutedEventArgs e)
        {
            //TreeModel treeModel = new TreeModel();
            //bindtreeview.Dt = JsonInterFace.PrivilegeManageClass.PrivilegeTable;
            //bindtreeview.TreeViewBind(treeModel);
            //_itemsSourceData.Add(treeModel);
            DeviceListTreeView.ItemsSource = _itemsSourceData;
        }

        private void menuAddFirstMenu_Click(object sender, RoutedEventArgs e)
        {
            TreeModel tree = new TreeModel();
            tree.Name = "所有";
            tree.Id = "-1";
            AddTreeItem AddTreeItemFrm = new AddTreeItem(tree);
            AddTreeItemFrm.ShowDialog();
            LoadFuncListTreeView();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }
        private void DgUserManage_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.AddUserManageList.DomainIdSet = "";
            if (dgUserManage.SelectedCells.Count > 0)
            {
                if (_usrdomainData.Count > 0)
                {
                    UsergroupManage usrDomain = (UsergroupManage)dgUserManage.SelectedItem;
                    string domainUserName = usrDomain.UserName;
                    foreach (TreeModel tree in _usrdomainData)
                    {
                        if (tree.Children.Count > 0)
                        {
                            tree.IsChecked = false;
                            tree.SetChildrenChecked(false);
                        }
                    }
                    if (JsonInterFace.UsrdomainManageClass.UsrDomainTable.Rows.Count > 0)
                    {
                        foreach (DataRow rw in JsonInterFace.UsrdomainManageClass.UsrDomainTable.Rows)
                        {
                            if (rw[0].ToString() == domainUserName)
                            {
                                JsonInterFace.AddUserManageList.DomainIdSet = rw[1].ToString();
                                break;
                            }
                        }
                        if (!JsonInterFace.AddUserManageList.DomainIdSet.Equals(""))
                        {
                            DeviceListTreeView.ItemsSource = SetCheckedTreeModel(_usrdomainData, JsonInterFace.AddUserManageList.DomainIdSet, true, "FullName", "");
                        }
                    }
                }
            }
        }

        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (NetWorkClient.ControllerServer.Connected)
            {
                NetWorkClient.ControllerServer.Send(JsonInterFace.All_privilege_request());
            }
            else
            {
                Parameters.PrintfLogsExtended("向服务器请求权限:", "Connected: Failed!");
            }
            Thread.Sleep(1000);
            LoadFuncListTreeView();
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
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //请求获取设备列表
            try
            {
                //添加用户名
                if(msg == Parameters.WM_UserManagerResponse)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Add_user_Request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.Password, JsonInterFace.AddUserManageList.BuildName));
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Add_user_group_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.RoleType, JsonInterFace.AddUserManageList.BuildName));
                    if (JsonInterFace.AddUserManageList.DomainIdSet != null && JsonInterFace.AddUserManageList.DomainIdSet != "")
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.Add_usr_domain_request(JsonInterFace.AddUserManageList.Name, JsonInterFace.AddUserManageList.DomainIdSet, JsonInterFace.AddUserManageList.Des));
                    }
                }
                //删除用户
                if(msg==Parameters.WM_DelUserManagerResponse)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Del_user_Request(JsonInterFace.DelUserManageList.Name));
                }
            }
            catch(Exception ex)
            {
                Parameters.PrintfLogsExtended("用户管理", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }

        private void FrmUserManager_Activated(object sender, EventArgs e)
        {
            Parameters.UserManagerWinHandle = (IntPtr)Parameters.FindWindow(null, this.Title);
        }

        private void ckbSetRolePrivilege_Click(object sender, RoutedEventArgs e)
        {
            SetRole.SetRole((bool)ckbSetRolePrivilege.IsChecked);
            //if ((bool)ckbSetRolePrivilege.IsChecked)
            //{
            //    FuncListTreeView.IsEnabled = true;
            //}
            //else
            //{
            //    FuncListTreeView.IsEnabled = false;
            //}
        }
    }
}
