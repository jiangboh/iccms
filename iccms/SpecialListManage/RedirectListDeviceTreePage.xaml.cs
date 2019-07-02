using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace iccms.SpecialListManage
{
    /// <summary>
    /// BWhiteListDeviceTreePage.xaml 的交互逻辑
    /// </summary>
    public partial class RedirectListDeviceTreePage : Page
    {
        private IList<CheckBoxTreeModel> UsrdomainData = new List<CheckBoxTreeModel>();

        public RedirectListDeviceTreePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadDeviceListTreeView();
        }

        private void loadDeviceListTreeView()
        {
            BindCheckBoxTreeView devicetreeview = new BindCheckBoxTreeView();
            CheckBoxTreeModel treeModel = new CheckBoxTreeModel();
            devicetreeview.Dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
            devicetreeview.DeviceTreeViewBind(ref treeModel);
            UsrdomainData.Clear();
            UsrdomainData.Add(treeModel);
            tvSpecialListDeviceTree.ItemsSource = UsrdomainData;
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

                bool NodeChecked = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked;
                string Model = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Mode;
                string[] _fullName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).FullName.Split(new char[] { '.' });
                string deviceName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).Name;

                if (Model == null || Model == "")
                {
                    JsonInterFace.ReDirection.FullName = "";
                    MessageBox.Show("请选择[设备]！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //取消错误选择项
                    (sender as CheckBox).IsChecked = false;
                    return;
                }
                else
                {
                    if (NodeChecked)
                    {
                        string fullName = string.Empty;
                        for (int i = 0; i < _fullName.Length - 1; i++)
                        {
                            if (fullName == null || fullName == "")
                            {
                                fullName = _fullName[i];
                            }
                            else
                            {
                                fullName += "." + _fullName[i];
                            }
                        }
                        JsonInterFace.ReDirection.FullName = fullName;
                        JsonInterFace.ReDirection.Name = deviceName;
                        JsonInterFace.ReDirection.UserType = "3";   //3表示获取所有
                    }
                    else
                    {
                        JsonInterFace.ReDirection.FullName = "";
                        JsonInterFace.ReDirection.Name = "";
                    }
                }

                //自动查询开始
                if (JsonInterFace.ReDirection.FullName != null && JsonInterFace.ReDirection.FullName != "")
                {
                    Parameters.SendMessage(Parameters.SpeciallistWinHandle, Parameters.WM_BWListQueryRequest, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
    }
}
