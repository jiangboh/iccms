using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace iccms.SubWindow
{
    /// <summary>
    /// WhiteListSelfLearningSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WhiteListSelfLearningSettingWindow : Window
    {

        public class RemoteAPInfoParameterClass
        {
            string _domainFullPathName;
            string _deviceName;
            string _iP;
            string _port;
            string _innerType;
            string _sN;

            public string DomainFullPathName
            {
                get
                {
                    return _domainFullPathName;
                }

                set
                {
                    _domainFullPathName = value;
                }
            }

            public string DeviceName
            {
                get
                {
                    return _deviceName;
                }

                set
                {
                    _deviceName = value;
                }
            }

            public string IP
            {
                get
                {
                    return _iP;
                }

                set
                {
                    _iP = value;
                }
            }

            public string Port
            {
                get
                {
                    return _port;
                }

                set
                {
                    _port = value;
                }
            }

            public string InnerType
            {
                get
                {
                    return _innerType;
                }

                set
                {
                    _innerType = value;
                }
            }

            public string SN
            {
                get
                {
                    return _sN;
                }

                set
                {
                    _sN = value;
                }
            }
        }

        public RemoteAPInfoParameterClass RemoteAPInfoParameter = new RemoteAPInfoParameterClass();

        public WhiteListSelfLearningSettingWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            Regex regexInt = new Regex(@"\d");
            try
            {
                if (txtTxpower.Text.Trim() != "")
                {
                    if (regexInt.Match(txtTxpower.Text).Success)
                    {
                        if (Convert.ToInt32(txtTxpower.Text) < -128 || Convert.ToInt32(txtTxpower.Text) > 0)
                        {
                            MessageBox.Show("衰减值超出范围，范围[-128~0]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("衰减值格式非法，范围[-128~0]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入衰减值，范围[-128~0]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (txtDuration.Text.Trim() != "")
                {
                    if (regexInt.Match(txtDuration.Text).Success)
                    {
                        if (Convert.ToUInt32(txtDuration.Text) < 0 || Convert.ToUInt32(txtDuration.Text) > 65535)
                        {
                            MessageBox.Show("白名单自学习时长超出范围，范围[0-65535]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("白名单自学习时长格式非，范围[0-65535]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请输入白名单自学习时长，范围[0-65535]", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string ParentFullNamePath = string.Empty;
                string[] _ParentFullNamePath = RemoteAPInfoParameter.DomainFullPathName.Split(new char[] { '.' });
                for (int i = 0; i < _ParentFullNamePath.Length - 1; i++)
                {
                    if (ParentFullNamePath == null || ParentFullNamePath == "")
                    {
                        ParentFullNamePath = _ParentFullNamePath[i];
                    }
                    else
                    {
                        ParentFullNamePath += "." + _ParentFullNamePath[i];
                    }
                }


                // >>> 检测白名单自学习工作状态 <<<
                if (JsonInterFace.LTEDeviceDetail.Command == "已启动")
                {
                    if (JsonInterFace.WhiteListSelfLearningParameter.Command != "0")
                    {
                        MessageBox.Show("操作绝拒，白名单自学习正在工作......，请先停止！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }


                //发送
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.APWhiteListSelfLearningSettingRequest(
                                                                                                            ParentFullNamePath,
                                                                                                            RemoteAPInfoParameter.DeviceName,
                                                                                                            RemoteAPInfoParameter.IP,
                                                                                                            RemoteAPInfoParameter.Port,
                                                                                                            RemoteAPInfoParameter.InnerType,
                                                                                                            RemoteAPInfoParameter.SN
                                                                                                           ));
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
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
                if (btnCancel.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCancel.Focus();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //语言

                //数据源
                txtTxpower.DataContext = JsonInterFace.WhiteListSelfLearningParameter;
                txtDuration.DataContext = JsonInterFace.WhiteListSelfLearningParameter;
                txtClearWhiteList.DataContext = JsonInterFace.WhiteListSelfLearningParameter;
                txtCommand.DataContext = JsonInterFace.WhiteListSelfLearningParameter;
                //白名单状态
                txtWhitelListSellLearningStatus.DataContext = JsonInterFace.LTEDeviceDetail;

                //获取白名单自学习状态
                if (NetWorkClient.ControllerServer.Connected)
                {
                    string DomainFullPathName = string.Empty;
                    string[] _DomainFullPathName = RemoteAPInfoParameter.DomainFullPathName.Split(new char[] { '.' });
                    for (int i = 0; i < _DomainFullPathName.Length - 1; i++)
                    {
                        if (DomainFullPathName == "" || DomainFullPathName == null)
                        {
                            DomainFullPathName = _DomainFullPathName[i];
                        }
                        else
                        {
                            DomainFullPathName += "." + _DomainFullPathName[i];
                        }
                    }
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.GetDeviceDetailRequest(
                                                                                                DomainFullPathName,
                                                                                                RemoteAPInfoParameter.DeviceName
                                                                                            )
                                                       );
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //自学习状态参数查询
                if (NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(
                                                        JsonInterFace.APWhiteListSelfLearningParameterQuery(
                                                                                                            RemoteAPInfoParameter.DomainFullPathName,
                                                                                                            RemoteAPInfoParameter.DeviceName,
                                                                                                            RemoteAPInfoParameter.IP,
                                                                                                            RemoteAPInfoParameter.Port,
                                                                                                            RemoteAPInfoParameter.InnerType,
                                                                                                            RemoteAPInfoParameter.SN
                                                                                                           )
                                                       );
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            JsonInterFace.WhiteListSelfLearningParameter.EditorParameter.Command = "0";
            JsonInterFace.WhiteListSelfLearningParameter.EditorParameter.Txpower = "0";
            JsonInterFace.WhiteListSelfLearningParameter.EditorParameter.Duration = "0";
            JsonInterFace.WhiteListSelfLearningParameter.EditorParameter.ClearWhiteList = "0";
        }
    }
}
