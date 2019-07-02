using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace iccms.SubWindow
{
    /// <summary>
    /// UnknownDeviceStationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnknownDeviceStationWindow : Window
    {
        private static IList<CheckBoxTreeModel> DeviceStartionListData = new List<CheckBoxTreeModel>();
        public string ID = string.Empty;
        public string ToStationFullPath = string.Empty;

        public UnknownDeviceStationWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadDeviceListTreeView();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnReturn.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        //显示设备列表
        private void LoadDeviceListTreeView()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    BindCheckBoxTreeView devicetreeview = new BindCheckBoxTreeView();
                    CheckBoxTreeModel treeModel = new CheckBoxTreeModel();

                    //过滤只显示站点
                    DataTable StationsTab = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Clone();
                    StationsTab.Rows.Clear();
                    DataRow[] StationsRow = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Select(string.Format("NodeType<>'{0}'", NodeType.LeafNode));
                    for (int i = 0; i < StationsRow.Length; i++)
                    {
                        DataRow Dr = StationsTab.NewRow();
                        Dr.BeginEdit();
                        int DrColums = StationsTab.Columns.Count;
                        for (int j = 0; j < DrColums; j++)
                        {
                            Dr[j] = StationsRow[i][j];
                        }
                        StationsTab.Rows.Add(Dr);
                        Dr.EndEdit();
                    }

                    devicetreeview.Dt = StationsTab;
                    devicetreeview.DeviceTreeViewBind(ref treeModel);
                    DeviceStartionListData.Clear();
                    DeviceStartionListData.Add(treeModel);
                    DeviceListTreeView.ItemsSource = DeviceStartionListData;

                    if (StationsTab.Rows.Count > 0)
                    {
                        CheckDeviceListIsChecked();
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended("未知设备选项显示设备表失败", Ex.Message, Ex.StackTrace);
                }
            }));
        }

        //检测已选中的设备
        private void CheckDeviceListIsChecked()
        {
            string Flag = string.Empty;
            bool readyMark = false;
            DeviceNamteItemToCheck(DeviceStartionListData, ToStationFullPath, ID, ref readyMark);

            if (readyMark)
            {
                this.Tag = ToStationFullPath;
                Flag = "[已标示]";
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "上次站点已设置为[" + ToStationFullPath + "]", "标示上次已选站点", "站点" + Flag);
            }
            else
            {
                Flag = "[无操作]";
                string ToStationData = (((ToStationFullPath == "") || (ToStationFullPath == null)) || (ToStationFullPath == Parameters.ToStationDefault)) ? ("空...") : ToStationFullPath;
                JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "上次站点已设置为[" + ToStationData + "]", "标示上次已选站点", "站点标示" + Flag);
            }
        }

        //取消已选中的设备
        private void CheckDeviceListIsToUnChecked()
        {
            DeviceNamteItemToUnCheck(DeviceStartionListData, ToStationFullPath, ID);
        }

        //递归重新显示已选中项
        private bool DeviceNamteItemToCheck(IList<CheckBoxTreeModel> DeviceStartionListChild, string DeviceFullNamePath, string ID, ref bool readyMark)
        {
            if (DeviceFullNamePath != null && DeviceFullNamePath != "")
            {
                foreach (CheckBoxTreeModel child in DeviceStartionListChild)
                {
                    if (child.Children.Count > 0 && child.IsStation.Equals("0"))
                    {
                        DeviceNamteItemToCheck(child.Children, DeviceFullNamePath, ID, ref readyMark);
                    }
                    else
                    {
                        if (DeviceFullNamePath == child.FullName)
                        {
                            child.IsChecked = true;
                            child.Tag = ID;
                            readyMark = true;
                            break;
                        }
                        else
                        {
                            child.IsChecked = false;
                            child.Tag = null;
                            readyMark = false;
                        }
                    }
                }
            }

            return readyMark;
        }

        //递归取消显示已选中项
        private bool DeviceNamteItemToUnCheck(IList<CheckBoxTreeModel> DeviceStartionListChild, string DeviceFullNamePath, string ID)
        {
            bool Flag = false;
            if (DeviceFullNamePath != null && DeviceFullNamePath != "")
            {
                foreach (CheckBoxTreeModel child in DeviceStartionListChild)
                {
                    if (child.Children.Count > 0 && child.IsStation.Equals("0"))
                    {
                        DeviceNamteItemToUnCheck(child.Children, DeviceFullNamePath, ID);
                    }
                    else
                    {
                        if (DeviceFullNamePath == child.FullName)
                        {
                            child.IsChecked = false;
                            child.Tag = null;
                            Flag = true;
                            break;
                        }
                    }
                }
            }

            return Flag;
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// 选译站点
        /// </summary>
        static object DeviceTreeViewItem = new object();
        private void chkTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            string SelfName = string.Empty;
            string FullName = string.Empty;
            string SelfNodeType = string.Empty;
            string IsStation = string.Empty;
            string SelfID = string.Empty;
            bool isChecked = false;

            try
            {
                //只允许选择站点实现效果
                if (((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsStation == "0"
                    && ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked
                    && !((CheckBoxTreeModel)(sender as CheckBox).DataContext).SelfNodeType.Equals(NodeType.StructureNode))
                {
                    MessageBox.Show("该选项不是站点，请选择站点!", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked = false;
                    (sender as CheckBox).IsChecked = false;
                    return;
                }

                //已选测反选，否则选择
                CheckDeviceListIsToUnChecked();

                if ((DeviceTreeViewItem as CheckBox) == null)
                {
                    DeviceTreeViewItem = sender;
                }
                else
                {
                    if (sender != DeviceTreeViewItem)
                    {
                        //取消上一次选择 DeviceNamteItemToUnCheck
                        ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsChecked = false;
                        (DeviceTreeViewItem as CheckBox).IsChecked = false;

                        //取消选择的项从列表删除
                        SelfName = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).Name;
                        FullName = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).FullName;
                        SelfNodeType = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).SelfNodeType;
                        SelfID = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).Id;
                        IsStation = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsStation;
                        isChecked = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsChecked;
                        StationUnSelected(SelfName, FullName, SelfNodeType, IsStation, SelfID, isChecked);

                        //生效最后一次选择
                        ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked = true;
                        (sender as CheckBox).IsChecked = true;
                    }

                    DeviceTreeViewItem = sender;
                }

                SelfName = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).Name;
                FullName = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).FullName;
                SelfNodeType = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).SelfNodeType;
                SelfID = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).Id;
                IsStation = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsStation;
                isChecked = ((CheckBoxTreeModel)(DeviceTreeViewItem as CheckBox).DataContext).IsChecked;

                //选择
                if (IsStation == "1" && isChecked)
                {
                    //将选中的站点全名更新到对应的未知设备选项
                    for (int i = 0; i < UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count; i++)
                    {
                        if (UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].ID == ID)
                        {
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].ToStation = FullName;
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].StationStatuIcon = new StatusIcon().OK;
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].StationID = SelfID;
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "已为未知设备[" + UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].DeviceName + "]指派站点：--> [" + FullName + "]", "站点指派", "成功");
                            break;
                        }
                    }

                    if (this.Tag != null)
                    {
                        if (this.Tag.ToString() == "AllSameStation")
                        {
                            this.Tag = SelfID + "|" + FullName;
                        }
                    }

                    StationSelected(SelfName, FullName, SelfNodeType, IsStation, SelfID, isChecked);

                    //获取该站点下的所有设备名
                    SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList.Clear();
                    DataRow[] SelfStationID = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Select(string.Format("PathName='{0}'", FullName));
                    string ChildParentID = SelfStationID[0]["SelfID"].ToString();
                    DataRow[] SelfDeviceNameList = JsonInterFace.BindTreeViewClass.DeviceTreeTable.Select(string.Format("ParentID='{0}'", ChildParentID));
                    for (int i = 0; i < SelfDeviceNameList.Length; i++)
                    {
                        UnknownDeviceReNameClass.SourceDeviceNameOfSelectedStationClass SelfStationDeviceNameList = new UnknownDeviceReNameClass.SourceDeviceNameOfSelectedStationClass();
                        SelfStationDeviceNameList.FullName = SelfDeviceNameList[i]["PathName"].ToString();
                        SelfStationDeviceNameList.SelfName = SelfDeviceNameList[i]["SelfName"].ToString();
                        SelfStationDeviceNameList.SelfIcon = SelfDeviceNameList[i]["NodeIcon"].ToString();
                        SelfStationDeviceNameList.SelfID = SelfDeviceNameList[i]["SelfID"].ToString();
                        SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList.Add(SelfStationDeviceNameList);
                    }
                }
                //取消选择
                else if (IsStation == "1" && !(isChecked))
                {
                    //将取消选择的站点全名更新到对应的未知设备选项
                    for (int i = 0; i < UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists.Count; i++)
                    {
                        if (UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].ID == ID)
                        {
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].ToStation = string.Empty;
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].StationStatuIcon = new StatusIcon().None;
                            UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].StationID = string.Empty;
                            JsonInterFace.SystemLogsInfo.Input(DateTime.Now.ToString(), "已为未知设备[" + UnKnownDeviceListsControlWindow.ShowUnKnownDeviceLists[i].DeviceName + "]取消站点[" + FullName + "]的指派", "取消站点指派", "成功");
                            break;
                        }
                    }

                    //取消选择站点
                    this.Tag = string.Empty;

                    StationUnSelected(SelfName, FullName, SelfNodeType, IsStation, SelfID, isChecked);

                    //取消选择,清空已获取该站点下所有设备名称(重命名对话框)
                    SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList.Clear();
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("未知设备设置站点", Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 已选择到的站点
        /// </summary>
        /// <param name="SelfName"></param>
        /// <param name="FullName"></param>
        /// <param name="SelfNodeType"></param>
        /// <param name="IsStation"></param>
        /// <param name="SelfID"></param>
        /// <param name="isChecked"></param>
        private void StationSelected(
                                        string SelfName,
                                        string FullName,
                                        string SelfNodeType,
                                        string IsStation,
                                        string SelfID,
                                        bool isChecked
        )
        {
            int flag = 1;
            Dispatcher.Invoke(() =>
            {
                if (UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Count > 0)
                {
                    for (int i = 0; i < UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Count; i++)
                    {
                        if (ID == UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].ID)
                        {
                            for (int j = 0; j < UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath.Count; j++)
                            {
                                string _id = UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath[j].Split(new char[] { ':' })[0];
                                string _fullname = UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath[j].Split(new char[] { ':' })[1];
                                if (_id == ID && _fullname == FullName)
                                {
                                    flag = 0;
                                    break;
                                }
                                else
                                {
                                    flag = 1;
                                }
                            }

                            if (flag == 1)
                            {
                                UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath.Add(ID + ":" + FullName);
                            }

                            //站点ID
                            UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].ParentID = SelfID;
                        }
                        else
                        {
                            flag = 2;
                        }
                    }

                    if (flag == 2)
                    {
                        UnKnownDeviceNameStationListClass Item = new UnKnownDeviceNameStationListClass();
                        Item.ID = ID;
                        Item.DeviceName = SelfName;
                        Item.ParentID = SelfID;
                        Item.DomainFullNamePath.Add(ID + ":" + FullName);
                        UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Add(Item);
                    }

                }
                else
                {
                    UnKnownDeviceNameStationListClass Item = new UnKnownDeviceNameStationListClass();
                    Item.ID = ID;
                    Item.DeviceName = SelfName;
                    Item.ParentID = SelfID;
                    Item.DomainFullNamePath.Add(ID + ":" + FullName);
                    UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Add(Item);
                }
            });
        }

        /// <summary>
        /// 取消已选择到的站点
        /// </summary>
        /// <param name="SelfName"></param>
        /// <param name="FullName"></param>
        /// <param name="SelfNodeType"></param>
        /// <param name="IsStation"></param>
        /// <param name="SelfID"></param>
        /// <param name="isChecked"></param>
        private void StationUnSelected(
                                        string SelfName,
                                        string FullName,
                                        string SelfNodeType,
                                        string IsStation,
                                        string SelfID,
                                        bool isChecked
        )
        {
            bool Flag = false;
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.Count; i++)
                {
                    for (int j = 0; j < UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath.Count; j++)
                    {
                        string _id = UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath[j].Split(new char[] { ':' })[0];
                        string _fullname = UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath[j].Split(new char[] { ':' })[1];
                        if (_id == ID && _fullname == FullName)
                        {
                            UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath.RemoveAt(j);
                            if (UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList[i].DomainFullNamePath.Count <= 0)
                            {
                                UnKnownDeviceListsControlWindow.UnKnownDeviceNameStationList.RemoveAt(i);
                                Flag = true;
                            }
                            else
                            {
                                Flag = false;
                            }
                            break;
                        }
                    }

                    if (Flag)
                    {
                        break;
                    }
                }
            });
        }
    }
}
