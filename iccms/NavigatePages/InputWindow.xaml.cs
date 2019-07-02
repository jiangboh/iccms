using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class TreeViewNodeInfo : INotifyPropertyChanged
    {
        private DeviceTreeOperation operation;
        private string nodeName;
        private string desInfo;
        private string fullNodeName;
        private bool isStation;
        private IList<CheckBoxTreeModel> children = null;

        public TreeViewNodeInfo()
        {
            if (Children == null)
            {
                Children = new List<CheckBoxTreeModel>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public string NodeName
        {
            get
            {
                return nodeName;
            }

            set
            {
                nodeName = value;
                NotifyPropertyChanged("NodeName");
            }
        }

        public string FullNodeName
        {
            get
            {
                return fullNodeName;
            }

            set
            {
                fullNodeName = value;
                NotifyPropertyChanged("FullNodeName");
            }
        }

        public DeviceTreeOperation Operation
        {
            get
            {
                return operation;
            }

            set
            {
                operation = value;
                NotifyPropertyChanged("Operation");
            }
        }

        public bool IsStation
        {
            get
            {
                return isStation;
            }

            set
            {
                isStation = value;
                NotifyPropertyChanged("IsStation");
            }
        }

        public string DesInfo
        {
            get
            {
                return desInfo;
            }

            set
            {
                desInfo = value;
                NotifyPropertyChanged("DesInfo");
            }
        }

        public IList<CheckBoxTreeModel> Children
        {
            get
            {
                return children;
            }

            set
            {
                children = value;
                NotifyPropertyChanged("Children");
            }
        }
    }

    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        private TreeViewNodeInfo treeViewNodeInfoClass = new TreeViewNodeInfo();
        private Regex regexDomain = new Regex(@"[\.!/\?\\\/\@#%+=\^&*~<>\,()\[\]\{\}]");
        private object LanguageClass = null;

        public InputWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (Parameters.LanguageType.Equals("EN"))
            {
                LanguageClass = new Language_EN.DeviceTreeOperationDialog();
                chkStation.Content = new Language_EN.DeviceTreeOperationDialog().NodeTypeCaption;
            }
            else
            {
                LanguageClass = new Language_CN.DeviceTreeOperationDialog();
                chkStation.Content = new Language_CN.DeviceTreeOperationDialog().NodeTypeCaption;
            }
        }

        public TreeViewNodeInfo TreeViewNodeInfoClass
        {
            get
            {
                return treeViewNodeInfoClass;
            }

            set
            {
                treeViewNodeInfoClass = value;
            }
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewNodeInfoClass.IsStation = (bool)chkStation.IsChecked;

                if (TreeViewNodeInfoClass.NodeName.Trim().Equals(""))
                {
                    MessageBox.Show("请输入域名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (!NetWorkClient.ControllerServer.Connected)
                    {
                        MessageBox.Show("网络已断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        if (regexDomain.Match(TreeViewNodeInfoClass.NodeName).Success)
                        {
                            JsonInterFace.ShowMessage("输入的域名称格式非法,域名只能为[128位以下划线开头的字母或中文或英文组成的名称]！", 16);
                            return;
                        }
                        //添加域名
                        if (TreeViewNodeInfoClass.Operation == DeviceTreeOperation.DomainAdd)
                        {
                            Parameters.DomainActionInfoClass.SelfID += 1;
                            Parameters.DomainActionInfoClass.SelfName = TreeViewNodeInfoClass.NodeName;
                            Parameters.DomainActionInfoClass.NodeContent = TreeViewNodeInfoClass.DesInfo;
                            Parameters.DomainActionInfoClass.IsStation = (Convert.ToInt32((bool)chkStation.IsChecked)).ToString();
                            NetWorkClient.ControllerServer.Send(JsonInterFace.AddDomainNodeName(TreeViewNodeInfoClass.FullNodeName, TreeViewNodeInfoClass.NodeName, Convert.ToInt32(TreeViewNodeInfoClass.IsStation), txtDesContent.Text));

                            //添加经纬度
                            if (TreeViewNodeInfoClass.IsStation && txtLngContent.Text != "" & txtLatContent.Text != "") 
                            {
                                Regex re = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
                                if (!re.IsMatch(txtLngContent.Text) || !re.IsMatch(txtLatContent.Text))
                                {
                                    MessageBox.Show("经纬度输入格式有误", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_station_location_Request(TreeViewNodeInfoClass.FullNodeName, TreeViewNodeInfoClass.NodeName, JsonInterFace.StationMap.Lng, JsonInterFace.StationMap.Lat));
                            }
                        }
                        //重命名域名
                        else if (TreeViewNodeInfoClass.Operation == DeviceTreeOperation.DomainReName)
                        {
                            string oldDomainName = TreeViewNodeInfoClass.FullNodeName;
                            string newDomainName = string.Empty;
                            string[] tmpDomainName = oldDomainName.Split(new char[] { '.' });
                            if (!Parameters.DomainActionInfoClass.SelfID.Equals(1))
                            {
                                for (int i = 0; i < tmpDomainName.Length - 1; i++)
                                {
                                    if (!newDomainName.Trim().Equals(""))
                                    {
                                        newDomainName += "." + tmpDomainName[i];
                                    }
                                    else
                                    {
                                        newDomainName += tmpDomainName[i];
                                    }
                                }

                                newDomainName = newDomainName + "." + TreeViewNodeInfoClass.NodeName;
                                Parameters.DomainActionInfoClass.OldFullDomainName = oldDomainName;
                                Parameters.DomainActionInfoClass.SelfName = txtInputContent.Text;
                                Parameters.DomainActionInfoClass.NodeContent = txtInputContent.Text;
                                Parameters.DomainActionInfoClass.AliasName = txtDesContent.Text;
                                Parameters.DomainActionInfoClass.PathName = newDomainName;
                                Parameters.DomainActionInfoClass.NewFullDomainName = newDomainName;
                                Parameters.DomainActionInfoClass.IsStation = (Convert.ToInt32((bool)chkStation.IsChecked)).ToString();

                                if (NetWorkClient.ControllerServer.Connected)
                                {
                                    //判断域名是否修改，如果没有修改就更新备注或者是否站点（需增加接口支持）
                                    NetWorkClient.ControllerServer.Send(JsonInterFace.ReNameDomainNodeName(oldDomainName, newDomainName, Convert.ToInt32(Parameters.DomainActionInfoClass.IsStation), Parameters.DomainActionInfoClass.AliasName));
                                }
                                else
                                {
                                    JsonInterFace.ShowMessage("网络与服务器断开！", (int)WindowMessageType.Warnning);
                                }
                            }
                            else
                            {
                                JsonInterFace.ShowMessage("该主域名称不能修改！", (int)WindowMessageType.Warnning);
                            }
                            //添加经纬度
                            if (TreeViewNodeInfoClass.IsStation && txtLngContent.Text != "" & txtLatContent.Text != "")
                            {
                                string ParentName = string.Empty;
                                Regex re = new Regex(@"^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$");
                                for (int i = 0; i < TreeViewNodeInfoClass.FullNodeName.Split(new char[] { '.' }).Length - 1; i++)
                                {
                                    if (i == 0)
                                        ParentName += TreeViewNodeInfoClass.FullNodeName.Split(new char[] { '.' })[i];
                                    else
                                        ParentName += "." + TreeViewNodeInfoClass.FullNodeName.Split(new char[] { '.' })[i];
                                }
                                if (!re.IsMatch(txtLngContent.Text) || !re.IsMatch(txtLatContent.Text))
                                {
                                    MessageBox.Show("经纬度输入格式有误", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                                NetWorkClient.ControllerServer.Send(JsonInterFace.Set_station_location_Request(ParentName, TreeViewNodeInfoClass.NodeName, JsonInterFace.StationMap.Lng, JsonInterFace.StationMap.Lat));
                            }
                        }
                        else if (TreeViewNodeInfoClass.Operation == DeviceTreeOperation.DeviceAdd)
                        {
                            Parameters.DeviceActionInfoClass.SelfID = Parameters.DomainActionInfoClass.SelfID.ToString();
                            Parameters.DeviceActionInfoClass.ParentID = Parameters.DomainActionInfoClass.ParentID.ToString();

                            Parameters.DeviceActionInfoClass.DeviceName = txtInputContent.Text.Trim();
                            Parameters.DeviceActionInfoClass.DomainFullName = TreeViewNodeInfoClass.FullNodeName;
                            NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameRequest(Parameters.DeviceActionInfoClass.DomainFullName, Parameters.DeviceActionInfoClass.DeviceName, Parameters.DeviceActionInfoClass.Mode));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = (Language_EN.DeviceTreeOperationDialog)LanguageClass;
            }
            else
            {
                this.DataContext = (Language_CN.DeviceTreeOperationDialog)LanguageClass;
            }

            this.Left = Parameters.UserMousePosition.X + 30;
            this.Top = Parameters.UserMousePosition.Y + 30;

            txtInputContent.DataContext = TreeViewNodeInfoClass;
            txtDesContent.DataContext = TreeViewNodeInfoClass;
            chkStation.DataContext = TreeViewNodeInfoClass;
            txtLngContent.Text = "";
            txtLatContent.Text = "";
            txtLngContent.DataContext = JsonInterFace.StationMap;
            txtLatContent.DataContext = JsonInterFace.StationMap;

            if (!TreeViewNodeInfoClass.IsStation)
            {
                if (TreeViewNodeInfoClass.Children.Count > 0)
                {
                    if (TreeViewNodeInfoClass.Children[0].SelfNodeType.Equals(NodeType.StructureNode.ToString()))
                    {
                        chkStation.IsEnabled = false;
                    }
                }
                txtLngContent.IsEnabled = false;
                txtLatContent.IsEnabled = false;
            }
            else
            {
                chkStation.IsEnabled = false;
                txtLngContent.IsEnabled = true;
                txtLatContent.IsEnabled = true;
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!txtInputContent.IsFocused && !txtDesContent.IsFocused && !chkStation.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        /// <summary>
        /// 按ESC退出
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

        private void chkStation_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)chkStation.IsChecked)
            {
                txtLngContent.IsEnabled = true;
                txtLatContent.IsEnabled = true;
            }
            else
            {
                txtLngContent.Text = "";
                txtLatContent.Text = "";
                txtLngContent.IsEnabled = false;
                txtLatContent.IsEnabled = false;
            }
        }

        private void txtLngContent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex(@"[^0-9 .]$");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}
