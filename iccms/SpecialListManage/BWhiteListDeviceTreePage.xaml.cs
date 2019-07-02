using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace iccms.SpecialListManage
{
    /// <summary>
    /// BWhiteListDeviceTreePage.xaml 的交互逻辑
    /// </summary>
    public partial class BWhiteListDeviceTreePage : Page
    {
        public BWhiteListDeviceTreePage()
        {
            InitializeComponent();
            tvSpecialListDeviceTree.ItemsSource = NavigatePages.NameListManage._usrdomainData;
        }

        private DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
            {
                source = VisualTreeHelper.GetParent(source);
            }
            return source;
        }

        /// <summary>
        /// 选择设备内容事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        object DeviceTreeViewItem = new object();
        private void chkTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //实现单选效果
                if ((DeviceTreeViewItem as CheckBox) == null)
                {
                    DeviceTreeViewItem = sender;
                }
                else
                {
                    if (sender != DeviceTreeViewItem)
                    {
                        ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsChecked = false;
                        (DeviceTreeViewItem as CheckBox).IsChecked = false;

                        ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked = true;
                        (sender as CheckBox).IsChecked = true;
                    }
                    DeviceTreeViewItem = sender;
                }

                string SelfID = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Id;
                string Model = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Mode;
                string FullName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).FullName;
                string NodeName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Name;
                string IsStation = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsStation;
                string SelfNodeType = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).SelfNodeType;
                Boolean NodeChecked = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked;

                //已选择
                if (NodeChecked)
                {
                    if ((DeviceType.LTE_FDD == Model || DeviceType.LTE_TDD == Model || DeviceType.WCDMA == Model || DeviceType.TD_SCDMA == Model) || (SelfNodeType == NodeType.StructureNode.ToString() && IsStation == "1"))
                    {
                        JsonInterFace.BlackList.ParameterList.Clear();
                        JsonInterFace.WhiteList.ParameterList.Clear();
                        JsonInterFace.CustomList.ParameterList.Clear();
                        for (int i = 0; i < JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows.Count; i++)
                        {
                            if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][1].ToString() == (SelfID)
                                && JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][2].ToString() == (NodeName))
                            {
                                BlackListClass.blackListOrderParaClass BlackListPara = new BlackListClass.blackListOrderParaClass();
                                WhiteListClass.blackListOrderParaClass WhiteListPara = new WhiteListClass.blackListOrderParaClass();
                                CustomListClass.blackListOrderParaClass CustomListPara = new CustomListClass.blackListOrderParaClass();

                                if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.StructureNode.ToString()))
                                {
                                    BlackListPara.NodeType = "domain";
                                    WhiteListPara.NodeType = "domain";
                                    CustomListPara.NodeType = "domain";
                                    BlackListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    WhiteListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    CustomListPara.DomainFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                                    BlackListPara.DeviceFullPathName = null;
                                    WhiteListPara.DeviceFullPathName = null;
                                    CustomListPara.DeviceFullPathName = null;
                                }
                                else if (JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][8].ToString() == (NodeType.LeafNode.ToString()))
                                {
                                    BlackListPara.NodeType = "device";
                                    WhiteListPara.NodeType = "device";
                                    CustomListPara.NodeType = "device";
                                    BlackListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    WhiteListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();
                                    CustomListPara.DeviceFullPathName = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Rows[i][0].ToString();

                                    BlackListPara.DomainFullPathName = null;
                                    WhiteListPara.DomainFullPathName = null;
                                    CustomListPara.DomainFullPathName = null;
                                }

                                //黑名单参数
                                JsonInterFace.BlackList.ParameterList.Add(BlackListPara);

                                //白名单参数
                                JsonInterFace.WhiteList.ParameterList.Add(WhiteListPara);

                                //普通用户参数
                                JsonInterFace.CustomList.ParameterList.Add(CustomListPara);
                                break;
                            }
                        }

                        //重定向参数
                        string[] _DomainFullNamePath = FullName.Split(new char[] { '.' });
                        string DomainFullNamePath = string.Empty;
                        for (int k = 0; k < _DomainFullNamePath.Length - 1; k++)
                        {
                            if (DomainFullNamePath == null || DomainFullNamePath == "")
                            {
                                DomainFullNamePath = _DomainFullNamePath[k];
                            }
                            else
                            {
                                DomainFullNamePath += "." + _DomainFullNamePath[k];
                            }
                        }
                        JsonInterFace.ReDirection.FullName = DomainFullNamePath;
                        JsonInterFace.ReDirection.Name = NodeName;
                        JsonInterFace.ReDirection.UserType = "3"; //3表示获取所有
                    }
                    else
                    {
                        //重定向参数
                        string[] _DomainFullNamePath = FullName.Split(new char[] { '.' });
                        string DomainFullNamePath = string.Empty;
                        for (int k = 0; k < _DomainFullNamePath.Length - 1; k++)
                        {
                            if (DomainFullNamePath == null || DomainFullNamePath == "")
                            {
                                DomainFullNamePath = _DomainFullNamePath[k];
                            }
                            else
                            {
                                DomainFullNamePath += "." + _DomainFullNamePath[k];
                            }
                        }
                        JsonInterFace.ReDirection.FullName = DomainFullNamePath;
                        JsonInterFace.ReDirection.Name = NodeName;
                        JsonInterFace.ReDirection.UserType = "3"; //3表示获取所有

                        if (JsonInterFace.BlackList.TabControlItemName == "tiBlacklist"
                            || JsonInterFace.BlackList.TabControlItemName == "tiWhitelist"
                            || JsonInterFace.BlackList.TabControlItemName == "tiCustomList")
                        {
                            //取消错误选择项
                            (sender as CheckBox).IsChecked = false;
                            MessageBox.Show("请选择：[站点或LTE，WCDMA]3G，4G设备！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                //取消选择
                else
                {
                    JsonInterFace.BlackList.ParameterList.Clear();
                    JsonInterFace.WhiteList.ParameterList.Clear();
                    JsonInterFace.CustomList.ParameterList.Clear();

                    JsonInterFace.ReDirection.FullName = null;
                    JsonInterFace.ReDirection.Name = null;
                    JsonInterFace.ReDirection.UserType = null;
                    return;
                }

                //自动查询开始
                if (JsonInterFace.WhiteList.ParameterList.Count > 0
                || JsonInterFace.BlackList.ParameterList.Count > 0
                || JsonInterFace.CustomList.ParameterList.Count > 0
                || (JsonInterFace.ReDirection.FullName != "" && JsonInterFace.ReDirection.FullName != null))
                {
                    Parameters.SendMessage(Parameters.SpeciallistWinHandle, Parameters.WM_BWListQueryRequest, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("特殊名单管理", ex.Message, ex.StackTrace);
            }
        }
    }
}
