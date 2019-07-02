using DataInterface;
using ParameterControl;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace iccms.SubWindow
{
    /// <summary>
    /// DeviceDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDetailWindow : Window
    {
        public string OperationType = string.Empty;
        public DeviceDetailWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void FrmDeviceDetail_Loaded(object sender, RoutedEventArgs e)
        {
            //详细信息显位方位调整
            if (this.Top + this.Height > SystemParameters.WorkArea.Size.Height)
            {
                double offset = this.Height - (SystemParameters.WorkArea.Size.Height - this.Top);
                this.Top = this.Top - offset - 50;
            }

            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = (new Language_EN.LTEDeviceDetail());
            }
            else
            {
                this.DataContext = (new Language_CN.LTEDeviceDetail());
            }

            SetDataContent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetDataContent()
        {
            if (new Regex(DeviceType.LTE).Match(OperationType).Success)
            {
                tabDeviceDetailControl.SelectedIndex = 0;
                txtDeviceName.DataContext = JsonInterFace.LTEDeviceDetail;
                txtPLMN.DataContext = JsonInterFace.LTEDeviceDetail;
                txtfreqPCI.DataContext = JsonInterFace.LTEDeviceDetail;
                txtcellStatus.DataContext = JsonInterFace.LTEDeviceDetail;
                txtscannerStatus.DataContext = JsonInterFace.LTEDeviceDetail;
                txtGPSStatus.DataContext = JsonInterFace.LTEDeviceDetail;
                txtGPSDetail.DataContext = JsonInterFace.LTEDeviceDetail;
                txtSyncStatus.DataContext = JsonInterFace.LTEDeviceDetail;
                txtSyncSource.DataContext = JsonInterFace.LTEDeviceDetail;
                txtAPReadySt.DataContext = JsonInterFace.LTEDeviceDetail;
                txtLicenseStatus.DataContext = JsonInterFace.LTEDeviceDetail;
                txtVersion.DataContext = JsonInterFace.LTEDeviceDetail;
                txtWhiteListSellLearningStatus.DataContext = JsonInterFace.LTEDeviceDetail;
            }
            else if (DeviceType.GSM == OperationType)
            {
                tabDeviceDetailControl.SelectedIndex = 1;
                txtGSMDeviceName.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;

                txtGSMCarrierOnePLMN.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;
                txtGSMCarrierOneParaMsPwr.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;
                txtGSMCarrierOneSmsType.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;
                txtGSMCarrierOnerfEnable.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;
                txtGSMCarrierOnerfFreq.DataContext = JsonInterFace.GSMSelfCarrierOneDetailInfo;

                txtGSMCarrierTwoPLMN.DataContext = JsonInterFace.GSMSelfCarrierTwoDetailInfo;
                txtGSMCarrierTwoParaMsPwr.DataContext = JsonInterFace.GSMSelfCarrierTwoDetailInfo;
                txtGSMCarrierTwoSmsType.DataContext = JsonInterFace.GSMSelfCarrierTwoDetailInfo;
                txtGSMCarrierTworfEnable.DataContext = JsonInterFace.GSMSelfCarrierTwoDetailInfo;
                txtGSMCarrierTworfFreq.DataContext = JsonInterFace.GSMSelfCarrierTwoDetailInfo;
            }
            else if (DeviceType.WCDMA == OperationType)
            {
                tabDeviceDetailControl.SelectedIndex = 2;
                txtWCDMADeviceName.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAPLMN.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAfreqPCI.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAcellStatus.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAscannerStatus.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMASyncStatus.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAGPSStatus.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAGPSDetail.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMALicenseStatus.DataContext = JsonInterFace.WCDMADeviceDetail;
                txtWCDMAVersion.DataContext = JsonInterFace.WCDMADeviceDetail;
            }
            else if (DeviceType.CDMA == OperationType)
            {
                tabDeviceDetailControl.SelectedIndex = 3;
                txtCDMADeviceName.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMAPLMN.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMAfreqPCI.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMAcellStatus.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMAscannerStatus.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMALicenseStatus.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMASyncStatus.DataContext = JsonInterFace.CDMADeviceDetail;
                txtCDMAVersion.DataContext = JsonInterFace.CDMADeviceDetail;
            }
            else if (DeviceType.GSMV2 == OperationType)
            {
                tabDeviceDetailControl.SelectedIndex = 4;
                txtGSMV2DeviceName.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2PLMN.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2freqPCI.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2cellStatus.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2scannerStatus.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2LicenseStatus.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;
                txtGSMV2Version.DataContext = JsonInterFace.GSMV2SelfCarrierOneDetailInfo;

                txtGSMV2PLMN2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
                txtGSMV2freqPCI2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
                txtGSMV2cellStatus2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
                txtGSMV2scannerStatus2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
                txtGSMV2LicenseStatus2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
                txtGSMV2Version2.DataContext = JsonInterFace.GSMV2SelfCarrierTwoDetailInfo;
            }
            else if (DeviceType.TD_SCDMA == OperationType)
            {
                tabDeviceDetailControl.SelectedIndex = 5;
                txtTDSDeviceName.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSPLMN.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSfreqPCI.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDScellStatus.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSscannerStatus.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSSyncStatus.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSGPSStatus.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSGPSDetail.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSLicenseStatus.DataContext = JsonInterFace.TDSDeviceDetail;
                txtTDSVersion.DataContext = JsonInterFace.TDSDeviceDetail;
            }
        }

        private void FrmDeviceDetail_Closed(object sender, EventArgs e)
        {
            this.DataContext = null;
            txtDeviceName.DataContext = null;
            txtPLMN.DataContext = null;
            txtfreqPCI.DataContext = null;
            txtcellStatus.DataContext = null;
            txtscannerStatus.DataContext = null;
            txtGPSStatus.DataContext = null;
            txtGPSDetail.DataContext = null;
            txtSyncStatus.DataContext = null;
            txtSyncSource.DataContext = null;
            txtLicenseStatus.DataContext = null;
            txtVersion.DataContext = null;
        }

        private void FrmDeviceDetail_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void FrmDeviceDetail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        /// <summary>
        /// 按ESC键退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmDeviceDetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }
    }
}
