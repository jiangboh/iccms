using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace iccms.NavigatePages
{
    /// <summary>
    /// DeviceInfoSettingModel.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceInfoSettingModel : Page
    {
        //参数
        private class SelfDeviceParameter
        {
            private string domainFullNamePath;
            private string station;
            private string deviceName;
            private string mode;
            private bool netWorkMode;
            private string iP;
            private string port;
            private string netMask;
            private string sN;
            private string deviceNameFlag;
            private int deviceFlagModeIndex;

            public string DomainFullNamePath
            {
                get
                {
                    return domainFullNamePath;
                }

                set
                {
                    domainFullNamePath = value;
                }
            }

            public string Station
            {
                get
                {
                    return station;
                }

                set
                {
                    station = value;
                }
            }

            public string DeviceName
            {
                get
                {
                    return deviceName;
                }

                set
                {
                    deviceName = value;
                }
            }

            public string Mode
            {
                get
                {
                    return mode;
                }

                set
                {
                    mode = value;
                }
            }

            public bool NetWorkMode
            {
                get
                {
                    return netWorkMode;
                }

                set
                {
                    netWorkMode = value;
                }
            }

            public string IP
            {
                get
                {
                    return iP;
                }

                set
                {
                    iP = value;
                }
            }

            public string Port
            {
                get
                {
                    return port;
                }

                set
                {
                    port = value;
                }
            }

            public string NetMask
            {
                get
                {
                    return netMask;
                }

                set
                {
                    netMask = value;
                }
            }

            public string SN
            {
                get
                {
                    return sN;
                }

                set
                {
                    sN = value;
                }
            }

            public string DeviceNameFlag
            {
                get
                {
                    return deviceNameFlag;
                }

                set
                {
                    deviceNameFlag = value;
                }
            }

            public int DeviceFlagModeIndex
            {
                get
                {
                    return deviceFlagModeIndex;
                }

                set
                {
                    deviceFlagModeIndex = value;
                }
            }
        }

        private SelfDeviceParameter selfParam = new SelfDeviceParameter();
        public DeviceInfoSettingModel()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    this.DataContext = new Language_EN.Device_managerWindow();
                }
                else
                {
                    this.DataContext = new Language_CN.Device_managerWindow();
                }

                //数据上下文
                txtDomainName.DataContext = JsonInterFace.LteDeviceParameter;
                txtSite.DataContext = JsonInterFace.LteDeviceParameter;
                txtDeviceName.DataContext = JsonInterFace.LteDeviceParameter;
                cbxMode.DataContext = JsonInterFace.LteDeviceParameter;
                rbFixedIP.IsChecked = JsonInterFace.LteDeviceParameter.StaticIPMode;
                txtIPAddr.DataContext = JsonInterFace.LteDeviceParameter;
                txtPort.DataContext = JsonInterFace.LteDeviceParameter;
                rbDynamicIP.IsChecked = JsonInterFace.LteDeviceParameter.DynamicIPMode;
                txtSN.DataContext = JsonInterFace.LteDeviceParameter;
                txtNetMask.DataContext = JsonInterFace.LteDeviceParameter;
                cbxDeviceDistinguish.DataContext = JsonInterFace.LteDeviceParameter;
                txtDomainName.IsEnabled = false;
                txtSite.IsEnabled = false;
                //修改前缓存
                selfParam.DomainFullNamePath = JsonInterFace.LteDeviceParameter.DomainFullPathName;
                selfParam.Station = JsonInterFace.LteDeviceParameter.Station;
                selfParam.DeviceName = JsonInterFace.LteDeviceParameter.DeviceName;
                selfParam.Mode = JsonInterFace.LteDeviceParameter.DeviceMode;
                selfParam.IP = JsonInterFace.LteDeviceParameter.IpAddr;
                selfParam.Port = JsonInterFace.LteDeviceParameter.Port;
                selfParam.NetMask = JsonInterFace.LteDeviceParameter.NetMask;
                selfParam.SN = JsonInterFace.LteDeviceParameter.SN;
                selfParam.DeviceNameFlag = JsonInterFace.LteDeviceParameter.DomainFullPathName;

                if (JsonInterFace.LteDeviceParameter.Operation == DeviceTreeOperation.DeviceAdd)
                {
                    btnDelete.IsEnabled = false;
                    btnUpdate.IsEnabled = false;
                }
                else if (JsonInterFace.LteDeviceParameter.Operation == DeviceTreeOperation.DeviceDelete)
                {
                    txtDeviceName.IsEnabled = false;
                    btnAdd.IsEnabled = false;
                    btnUpdate.IsEnabled = false;
                }
                else if (JsonInterFace.LteDeviceParameter.Operation == DeviceTreeOperation.DeviceUpdate)
                {
                    btnAdd.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbxMode.SelectedIndex < 0)
                {
                    cbxMode.Focus();
                    cbxMode.IsDropDownOpen = true;
                    MessageBox.Show("请选择该设备的制式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定添加设备[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName, JsonInterFace.LteDeviceParameter.DeviceMode));
                    }
                    else
                    {
                        MessageBox.Show("网络与有服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbxMode.SelectedIndex < 0)
                {
                    MessageBox.Show("请选择制式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show("确定删除设备[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        NetWorkClient.ControllerServer.Send(JsonInterFace.DeleteDeviceNameRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, JsonInterFace.LteDeviceParameter.DeviceName));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                if (!selfParam.Mode.Equals(JsonInterFace.LteDeviceParameter.DeviceMode))
                {
                    Params.Add("mode", cbxMode.Text.Trim());
                }

                if (!selfParam.SN.Trim().Equals(JsonInterFace.LteDeviceParameter.SN))
                {
                    Params.Add("sn", txtSN.Text.Trim());
                }

                if (!selfParam.IP.Trim().Equals(JsonInterFace.LteDeviceParameter.IpAddr))
                {
                    Params.Add("ipAddr", txtIPAddr.Text.Trim());
                }

                if (!selfParam.Port.Trim().Equals(JsonInterFace.LteDeviceParameter.Port))
                {
                    Params.Add("port", txtPort.Text.Trim());
                }

                if (!selfParam.NetMask.Equals(JsonInterFace.LteDeviceParameter.NetMask))
                {
                    Params.Add("netmask", JsonInterFace.LteDeviceParameter.NetMask);
                }

                if (!selfParam.DeviceName.Trim().Equals(JsonInterFace.LteDeviceParameter.DeviceName))
                {
                    Params.Add("name", txtDeviceName.Text.Trim());
                }

                Params.Add("carrier", "0");

                if (Params.Count <= 0)
                {
                    MessageBox.Show("内容未改变！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    return;
                }

                if (MessageBox.Show("确定更新设备信息[" + JsonInterFace.LteDeviceParameter.DeviceName + "]?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    if (NetWorkClient.ControllerServer.Connected)
                    {
                        Parameters.ConfigType = "Manul";
                        NetWorkClient.ControllerServer.Send(JsonInterFace.AddDeviceNameUpdateParametersRequest(JsonInterFace.LteDeviceParameter.DomainFullPathName, selfParam.DeviceName, Params));
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void rbFixedIP_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)((sender as RadioButton).IsChecked))
            {
                txtIPAddr.Text = selfParam.IP;
                txtPort.Text = selfParam.Port;
                txtNetMask.Text = selfParam.NetMask;
            }
        }

        private void rbDynamicIP_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)((sender as RadioButton).IsChecked))
            {
                txtIPAddr.Text = string.Empty;
                txtPort.Text = string.Empty;
                txtNetMask.Text = string.Empty;
            }
        }
    }
}
