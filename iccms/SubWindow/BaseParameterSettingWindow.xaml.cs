using DataInterface;
using ParameterControl;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace iccms.SubWindow
{
    /// <summary>
    /// 基本参数信息
    /// </summary>
    public class BaaseParameterCloass : INotifyPropertyChanged
    {
        //[Language]
        private string _abbreviationCode = "0";
        //[Scanner]
        private string _refresh = "10";
        private string _tatol = "200";
        private string _soundFile = @"x:\xxx\xxx\x.wav";
        private string _soundDelay = "10";
        private string _playCount = "1";
        private string _soundEnable = "0";
        private string _speeckContent = "";
        private bool _playerMode;
        private string _whiteListBackGround;
        private string _blackListBackGround;
        private string _otherListBackGround;
        private bool _whiteListMode;
        private bool _blackListMode;
        private bool _otherListMode;
        //[Device]
        private string _operators = "5";
        private string _sMSBrowseYes = "false";
        private string _sMSBrowseNo = "true";
        //[Heart]
        private string _time = "60";
        //[Logs]
        private string _logStatus = "1";
        private string _total = "1000";

        public string AbbreviationCode
        {
            get
            {
                return _abbreviationCode;
            }

            set
            {
                _abbreviationCode = value;
                NotifyPropertyChanged("AbbreviationCode");
            }
        }

        public string Refresh
        {
            get
            {
                return _refresh;
            }

            set
            {
                _refresh = value;
                NotifyPropertyChanged("Refresh");
            }
        }

        public string Tatol
        {
            get
            {
                return _tatol;
            }

            set
            {
                _tatol = value;
                NotifyPropertyChanged("Tatol");
            }
        }

        public string SoundFile
        {
            get
            {
                return _soundFile;
            }

            set
            {
                _soundFile = value;
                NotifyPropertyChanged("SoundFile");
            }
        }

        public string SoundDelay
        {
            get
            {
                return _soundDelay;
            }

            set
            {
                _soundDelay = value;
                NotifyPropertyChanged("SoundDelay");
            }
        }

        public string Operators
        {
            get
            {
                return _operators;
            }

            set
            {
                _operators = value;
                NotifyPropertyChanged("Operators");
            }
        }

        public string SMSBrowseYes
        {
            get
            {
                return _sMSBrowseYes;
            }

            set
            {
                _sMSBrowseYes = value;
                NotifyPropertyChanged("SMSBrowseYes");
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
                NotifyPropertyChanged("Time");
            }
        }

        public string LogStatus
        {
            get
            {
                return _logStatus;
            }

            set
            {
                _logStatus = value;
                NotifyPropertyChanged("LogStatus");
            }
        }

        public string Total
        {
            get
            {
                return _total;
            }

            set
            {
                _total = value;
                NotifyPropertyChanged("Total");
            }
        }

        public string SMSBrowseNo
        {
            get
            {
                return _sMSBrowseNo;
            }

            set
            {
                _sMSBrowseNo = value;
                NotifyPropertyChanged("SMSBrowseNo");
            }
        }

        public string PlayCount
        {
            get
            {
                return _playCount;
            }

            set
            {
                _playCount = value;
                NotifyPropertyChanged("PlayCount");
            }
        }

        public string SoundEnable
        {
            get
            {
                return _soundEnable;
            }

            set
            {
                _soundEnable = value;
                NotifyPropertyChanged("SoundEnable");
            }
        }

        public string SpeeckContent
        {
            get
            {
                return _speeckContent;
            }

            set
            {
                _speeckContent = value;
                NotifyPropertyChanged("SpeeckContent");
            }
        }

        public bool PlayerMode
        {
            get
            {
                return _playerMode;
            }

            set
            {
                _playerMode = value;
                NotifyPropertyChanged("PlayerMode");
            }
        }

        public string WhiteListBackGround
        {
            get
            {
                return _whiteListBackGround;
            }

            set
            {
                _whiteListBackGround = value;
                NotifyPropertyChanged("WhiteListBackGround");
            }
        }

        public string BlackListBackGround
        {
            get
            {
                return _blackListBackGround;
            }

            set
            {
                _blackListBackGround = value;
                NotifyPropertyChanged("BlackListBackGround");
            }
        }

        public string OtherListBackGround
        {
            get
            {
                return _otherListBackGround;
            }

            set
            {
                _otherListBackGround = value;
                NotifyPropertyChanged("OtherListBackGround");
            }
        }

        public bool WhiteListMode
        {
            get
            {
                return _whiteListMode;
            }

            set
            {
                _whiteListMode = value;
                NotifyPropertyChanged("WhiteListMode");
            }
        }

        public bool BlackListMode
        {
            get
            {
                return _blackListMode;
            }

            set
            {
                _blackListMode = value;
                NotifyPropertyChanged("BlackListMode");
            }
        }

        public bool OtherListMode
        {
            get
            {
                return _otherListMode;
            }

            set
            {
                _otherListMode = value;
                NotifyPropertyChanged("OtherListMode");
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
    }

    /// <summary>
    /// Log下载FTP参数设置
    /// </summary>
    public class LogDownLoadFTPParameterClass : INotifyPropertyChanged
    {
        private string _iD;
        private string _code;
        private string _root;
        private int _port;
        private bool _rootWithSN;
        private int _upDateTimeOut;
        private string _upDateFileSourceDir;

        public string ID
        {
            get
            {
                return _iD;
            }

            set
            {
                _iD = value;
                NotifyPropertyChanged("ID");
            }
        }

        public string Code
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
                NotifyPropertyChanged("Code");
            }
        }

        public string Root
        {
            get
            {
                return _root;
            }

            set
            {
                _root = value;
                NotifyPropertyChanged("Root");
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
                NotifyPropertyChanged("Port");
            }
        }

        public bool RootWithSN
        {
            get
            {
                return _rootWithSN;
            }

            set
            {
                _rootWithSN = value;
                NotifyPropertyChanged("RootWithSN");
            }
        }

        public int UpDateTimeOut
        {
            get
            {
                return _upDateTimeOut;
            }

            set
            {
                _upDateTimeOut = value;
                NotifyPropertyChanged("UpDateDelay");
            }
        }

        public string UpDateFileSourceDir
        {
            get
            {
                return _upDateFileSourceDir;
            }

            set
            {
                _upDateFileSourceDir = value;
                NotifyPropertyChanged("UpDateFileSourceDir");
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
    }

    public class DefaultWindowModelBackUp
    {
        public bool? chkDefaultWinDefaultChecked = null;
        public bool? chkDeviceListWinDefaultChecked = null;
        public bool? chkScannerWinDefaultChecked = null;
        public bool? chkBlackListWinDefaultChecked = null;
        public bool? chkSystemLogsWinDefaultChecked = null;
    }

    /// <summary>
    /// BaseParameterSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BaseParameterSettingWindow : Window
    {
        private static BaaseParameterCloass BaaseParameter = new BaaseParameterCloass();
        private static LogDownLoadFTPParameterClass LogDownLoadFTPParameter = new LogDownLoadFTPParameterClass();
        public static ObservableCollection<UserTypesParameterClass> UserTypesParameterList = new ObservableCollection<UserTypesParameterClass>();
        private DefaultWindowModelBackUp DefaultWindowModelPara = new DefaultWindowModelBackUp();

        public BaseParameterSettingWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbbLanguage.DataContext = BaaseParameter;
            txtScannerReportUpdate.DataContext = BaaseParameter;
            txtScannerReportTotal.DataContext = BaaseParameter;
            txtSacnnerWarningSoundFile.DataContext = BaaseParameter;
            txtSacnnerWarningTime.DataContext = BaaseParameter;
            txtPLMNLength.DataContext = BaaseParameter;
            chkBrowseEnable.DataContext = BaaseParameter;
            chkBrowseDisEnable.DataContext = BaaseParameter;
            txtHeartTime.DataContext = BaaseParameter;
            txtSaveWays.DataContext = BaaseParameter;
            txtShowTotal.DataContext = BaaseParameter;
            txtSacnnerWarningSoundPlayCount.DataContext = BaaseParameter;
            cbbSacnnerWarningSoundPlayEnable.DataContext = BaaseParameter;
            chkSacnnerWarningSoundPlayMedia.DataContext = BaaseParameter;
            //颜色设置
            lblWhiteListColor.DataContext = BaaseParameter;
            lblBlackListColor.DataContext = BaaseParameter;
            lblOtherListColor.DataContext = BaaseParameter;
            //播报控制
            chkSacnnerWarningSoundWhiteList.DataContext = BaaseParameter;
            chkSacnnerWarningSoundBlackList.DataContext = BaaseParameter;
            chkSacnnerWarningSoundOtherList.DataContext = BaaseParameter;

            //日志下载基本参数
            txtUserName.DataContext = LogDownLoadFTPParameter;
            txtPassWord.DataContext = LogDownLoadFTPParameter;
            txtFTPRoot.DataContext = LogDownLoadFTPParameter;
            txtFTPPort.DataContext = LogDownLoadFTPParameter;
            chkFtpRootWithSN.DataContext = LogDownLoadFTPParameter;

            //窗口显示控制
            WinControlBox.DataContext = Parameters.MainWinControlParameter;
            DefaultWindowModelPara.chkBlackListWinDefaultChecked = Parameters.MainWinControlParameter.BlackListWin;
            DefaultWindowModelPara.chkDefaultWinDefaultChecked = Parameters.MainWinControlParameter.AllWin;
            DefaultWindowModelPara.chkDeviceListWinDefaultChecked = Parameters.MainWinControlParameter.DeviceListWin;
            DefaultWindowModelPara.chkScannerWinDefaultChecked = Parameters.MainWinControlParameter.ScannerWin;
            DefaultWindowModelPara.chkSystemLogsWinDefaultChecked = Parameters.MainWinControlParameter.SystemLogsWin;

            try
            {
                if (Parameters.LanguageType == "CN")
                {
                    BaaseParameter.AbbreviationCode = "0";
                }
                else if (Parameters.LanguageType == "EN")
                {
                    BaaseParameter.AbbreviationCode = "1";
                }

                BaaseParameter.Refresh = Parameters.ScannerDataControlParameter.RefreshTime.ToString();
                BaaseParameter.Tatol = Parameters.ScannerDataControlParameter.Tatol.ToString();
                BaaseParameter.SoundFile = Parameters.ScannerDataControlParameter.SoundFile;
                BaaseParameter.SoundDelay = Parameters.ScannerDataControlParameter.SoundDelay;
                BaaseParameter.PlayCount = Parameters.ScannerDataControlParameter.PlayCount.ToString();
                BaaseParameter.SoundEnable = Parameters.ScannerDataControlParameter.SoundEnable;
                BaaseParameter.SpeeckContent = Parameters.ScannerDataControlParameter.SpeeckContent;
                BaaseParameter.PlayerMode = Parameters.ScannerDataControlParameter.PlayerMode;

                BaaseParameter.WhiteListBackGround = Parameters.ScannerDataControlParameter.WhiteListBackGround;
                BaaseParameter.BlackListBackGround = Parameters.ScannerDataControlParameter.BlackListBackGround;
                BaaseParameter.OtherListBackGround = Parameters.ScannerDataControlParameter.OtherListBackGround;

                BaaseParameter.WhiteListMode = Parameters.ScannerDataControlParameter.WhiteListMode;
                BaaseParameter.BlackListMode = Parameters.ScannerDataControlParameter.BlackListMode;
                BaaseParameter.OtherListMode = Parameters.ScannerDataControlParameter.OtherListMode;

                if (BaaseParameter.PlayerMode)
                {
                    chkSacnnerWarningSoundPlayMedia.IsChecked = true;
                    chkSacnnerWarningSoundPlayContent.IsChecked = false;
                }
                else
                {
                    chkSacnnerWarningSoundPlayMedia.IsChecked = false;
                    chkSacnnerWarningSoundPlayContent.IsChecked = true;
                }
                BaaseParameter.Operators = Parameters.PLMN_Lengh.ToString();
                if (Parameters.SMSBrowse)
                {
                    BaaseParameter.SMSBrowseYes = "true";
                    BaaseParameter.SMSBrowseNo = "false";
                }
                else
                {
                    BaaseParameter.SMSBrowseYes = "false";
                    BaaseParameter.SMSBrowseNo = "true";
                }
                BaaseParameter.Time = Parameters.HeartTime.ToString();
                BaaseParameter.LogStatus = Parameters.LogStatus.ToString();
                BaaseParameter.Total = Parameters.SysLogsTotal.ToString();

                //FTP参数
                LogDownLoadFTPParameter.ID = Parameters.DownLoadLogUser;
                LogDownLoadFTPParameter.Code = Parameters.DownLoadLogPass;
                txtPassWord.Password = LogDownLoadFTPParameter.Code;
                LogDownLoadFTPParameter.Root = Parameters.DownLoadLogRoot;
                LogDownLoadFTPParameter.Port = Parameters.DownLoadLogPort;
                LogDownLoadFTPParameter.RootWithSN = Parameters.DownLoadLogLastDirWithSN;

                //系统升级参数
                txtUpdateDelay.DataContext = LogDownLoadFTPParameter;
                txtSourceFilePath.DataContext = LogDownLoadFTPParameter;
                LogDownLoadFTPParameter.UpDateTimeOut = Parameters.UpDonwLoadTimeOutValue;
                LogDownLoadFTPParameter.UpDateFileSourceDir = Parameters.UpDateFileSourceDir;
            }
            catch (Exception ex)
            {
                MessageBox.Show("载入基本配置参数失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            WindowsClose();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //检查媒体文件是否匹配
                if (txtSacnnerWarningSoundFile.Text != "" && txtSacnnerWarningSoundFile.Text != null)
                {
                    if (!File.Exists(txtSacnnerWarningSoundFile.Text))
                    {
                        MessageBox.Show("输入的媒体文件不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string[] FileType = Parameters.CheckTrueFileName(txtSacnnerWarningSoundFile.Text);
                    if (FileType[3].ToString() == ".txt" && !BaaseParameter.PlayerMode)
                    {
                        txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                        BaaseParameter.SoundFile = txtSacnnerWarningSoundFile.Text;
                    }
                    else if (BaaseParameter.PlayerMode
                        && (FileType[3].ToString() == ".wav"
                        || FileType[3].ToString() == ".mp3"
                        || FileType[3].ToString() == ".avi"
                        || FileType[3].ToString() == ".mp4"
                        || FileType[3].ToString() == ".rmvb"
                        || FileType[3].ToString() == ".mpg"
                        || FileType[3].ToString() == ".mpeg"
                        || FileType[3].ToString() == ".3gp"))
                    {
                        txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                        BaaseParameter.SoundFile = txtSacnnerWarningSoundFile.Text;
                    }
                    else
                    {
                        txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        MessageBox.Show("媒体播放模式与打开的文件格式不匹配！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    BaaseParameter.SoundFile = "";
                }

                //背景改变则更新界面
                Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ChangeScannerDataRowsBackGroundColor, 0, 0);

                //媒体参数改变重新检测播放
                if (Parameters.ScannerDataControlParameter.WhiteListMode != BaaseParameter.WhiteListMode
                    || Parameters.ScannerDataControlParameter.BlackListMode != BaaseParameter.BlackListMode
                    || Parameters.ScannerDataControlParameter.OtherListMode != BaaseParameter.OtherListMode
                    || Parameters.ScannerDataControlParameter.SoundFile != BaaseParameter.SoundFile
                    || Parameters.ScannerDataControlParameter.SoundEnable != BaaseParameter.SoundEnable
                    || Parameters.ScannerDataControlParameter.SoundDelay != BaaseParameter.SoundDelay
                    || Parameters.ScannerDataControlParameter.SpeeckContent != BaaseParameter.SpeeckContent
                    || Parameters.ScannerDataControlParameter.PlayerMode != BaaseParameter.PlayerMode)
                {
                    Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_RestartPlayerThreadRequest, 0, 0);
                }

                //更新
                if (BaaseParameter.AbbreviationCode == "0")
                {
                    Parameters.LanguageType = "CN";
                }
                else if (BaaseParameter.AbbreviationCode == "1")
                {
                    Parameters.LanguageType = "EN";
                }

                Parameters.ScannerDataControlParameter.RefreshTime = Convert.ToInt32(BaaseParameter.Refresh);

                //实时显示数据总数
                if (Parameters.ScannerDataControlParameter.Tatol != Convert.ToInt32(BaaseParameter.Tatol))
                {
                    RealDataAlignMent(Convert.ToInt32(BaaseParameter.Tatol));
                }

                Parameters.ScannerDataControlParameter.SoundFile = BaaseParameter.SoundFile;
                Parameters.ScannerDataControlParameter.SoundDelay = BaaseParameter.SoundDelay;
                Parameters.ScannerDataControlParameter.PlayCount = Convert.ToInt32(BaaseParameter.PlayCount);
                Parameters.ScannerDataControlParameter.SoundEnable = BaaseParameter.SoundEnable;
                Parameters.ScannerDataControlParameter.SpeeckContent = BaaseParameter.SpeeckContent;
                Parameters.ScannerDataControlParameter.PlayerMode = BaaseParameter.PlayerMode;

                Parameters.ScannerDataControlParameter.WhiteListMode = BaaseParameter.WhiteListMode;
                Parameters.ScannerDataControlParameter.BlackListMode = BaaseParameter.BlackListMode;
                Parameters.ScannerDataControlParameter.OtherListMode = BaaseParameter.OtherListMode;

                Parameters.ScannerDataControlParameter.WhiteListBackGround = BaaseParameter.WhiteListBackGround;
                Parameters.ScannerDataControlParameter.BlackListBackGround = BaaseParameter.BlackListBackGround;
                Parameters.ScannerDataControlParameter.OtherListBackGround = BaaseParameter.OtherListBackGround;

                Parameters.PLMN_Lengh = Convert.ToInt32(BaaseParameter.Operators);

                if (BaaseParameter.SMSBrowseYes == "true")
                {
                    Parameters.SMSBrowse = true;
                }
                else
                {
                    Parameters.SMSBrowse = false;
                }

                Parameters.HeartTime = Convert.ToInt32(BaaseParameter.Time);
                Parameters.LogStatus = Convert.ToByte(BaaseParameter.LogStatus);
                Parameters.SysLogsTotal = Convert.ToInt32(BaaseParameter.Total);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("参数更新失败", ex.Message, ex.StackTrace);
                MessageBox.Show("参数更新失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                //保存INI
                Parameters.WriteIniFile("Language", "AbbreviationCode", Parameters.LanguageType, Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "Refresh", Parameters.ScannerDataControlParameter.RefreshTime.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "Tatol", Parameters.ScannerDataControlParameter.Tatol.ToString(), Parameters.INIFile);

                Parameters.WriteIniFile("Scanner", "SoundFile", Parameters.ScannerDataControlParameter.SoundFile, Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "SoundDelay", Parameters.ScannerDataControlParameter.SoundDelay, Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "SoundEnable", Parameters.ScannerDataControlParameter.SoundEnable, Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "SpeeckContent", Parameters.ScannerDataControlParameter.SpeeckContent, Parameters.INIFile);

                Parameters.WriteIniFile("Scanner", "WBG", Parameters.ScannerDataControlParameter.WhiteListBackGround.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "BBG", Parameters.ScannerDataControlParameter.BlackListBackGround.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "OBG", Parameters.ScannerDataControlParameter.OtherListBackGround.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "WPM", Parameters.ScannerDataControlParameter.WhiteListMode.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "BPM", Parameters.ScannerDataControlParameter.BlackListMode.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Scanner", "OPM", Parameters.ScannerDataControlParameter.OtherListMode.ToString(), Parameters.INIFile);

                Parameters.WriteIniFile("Device", "Operator", Parameters.PLMN_Lengh.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Device", "SMSBrowse", Parameters.SMSBrowse.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Heart", "Time", Parameters.HeartTime.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Logs", "LogStatus", Parameters.LogStatus.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Logs", "Total", Parameters.SysLogsTotal.ToString(), Parameters.INIFile);

                MessageBox.Show("参数保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                //状态机重置
                lock (JsonInterFace.FSM.StatusLock)
                {
                    JsonInterFace.FSM.Reset(true, true, true);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("参数配置失败", ex.Message, ex.StackTrace);
                MessageBox.Show("参数保存失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //实时显示数据数量对齐
        private void RealDataAlignMent(int Total)
        {
            try
            {
                lock (JsonInterFace.ScannerData.Mutex_DbHelper)
                {
                    Parameters.ScannerDataControlParameter.Tatol = Total;

                    if (Parameters.ScannerDataControlParameter.Tatol < NavigatePages.UEInfoWindow.SelfScannerReportInfo.Count)
                    {
                        NavigatePages.UEInfoWindow.ScannerReportRealTimeThread.Suspend();
                        ObservableCollection<ScannerDataClass> TmpData = NavigatePages.UEInfoWindow.SelfScannerReportInfo;

                        //实时显示
                        for (int i = 0; i < (NavigatePages.UEInfoWindow.SelfScannerReportInfo.Count - Parameters.ScannerDataControlParameter.Tatol - 1); i++)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                TmpData.RemoveAt(TmpData.Count - 1);
                            });
                        }

                        Dispatcher.Invoke(() =>
                        {
                            NavigatePages.UEInfoWindow.SelfScannerReportInfo = TmpData;
                            NavigatePages.UEInfoWindow.ScannerReportRealTimeThread.Resume();
                        });
                    }

                    if (Parameters.ScannerDataControlParameter.Tatol < JsonInterFace.ScannerData.ScannerDataTable.Rows.Count)
                    {
                        //实时缓存
                        for (int i = JsonInterFace.ScannerData.ScannerDataTable.Rows.Count - 1; i >= Parameters.ScannerDataControlParameter.Tatol; i--)
                        {
                            JsonInterFace.ScannerData.ScannerDataTable.Rows.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Parameters.PrintfLogsExtended("实时显示数据数量对齐失败", e.Message, e.StackTrace);
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                WindowsClose();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!(bool)chkDefaultWin.IsChecked
                && !(bool)chkDeviceListWin.IsChecked
                && !(bool)chkScannerWin.IsChecked
                && !(bool)chkBlackListWin.IsChecked
                && !(bool)chkSystemLogsWin.IsChecked)
            {
                Parameters.MainWinControlParameter.BlackListWin = (bool)DefaultWindowModelPara.chkBlackListWinDefaultChecked;
                Parameters.MainWinControlParameter.AllWin = (bool)DefaultWindowModelPara.chkDefaultWinDefaultChecked;
                Parameters.MainWinControlParameter.DeviceListWin = (bool)DefaultWindowModelPara.chkDeviceListWinDefaultChecked;
                Parameters.MainWinControlParameter.ScannerWin = (bool)DefaultWindowModelPara.chkScannerWinDefaultChecked;
                Parameters.MainWinControlParameter.SystemLogsWin = (bool)DefaultWindowModelPara.chkSystemLogsWinDefaultChecked;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnExit.IsFocused
                    || btnFTPParaSettingExit.IsFocused
                    || btnWinControlCancel.IsFocused
                   )
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnExit.Focus();
            btnFTPParaSettingExit.Focus();
            btnWinControlCancel.Focus();

        }

        private void chkBrowseEnable_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                chkBrowseDisEnable.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    chkBrowseEnable.IsChecked = true;
                }
            }
        }

        private void chkBrowseDisEnable_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                chkBrowseEnable.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    chkBrowseDisEnable.IsChecked = true;
                }
            }
        }

        private void btnSacnnerWarningSoundFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog OpenMediaFile = new System.Windows.Forms.OpenFileDialog();
            OpenMediaFile.CheckFileExists = true;
            OpenMediaFile.CheckPathExists = true;
            OpenMediaFile.Filter = "(媒体文件: *.wav,*.mp3,*.avi,*.mp4,*.rmvb,*.mpg,*.mpeg,*.3gp)|*.wav;*.mp3;*.avi;*.mp4;*.rmvb;*.mpg;*.mpeg;*.3gp|(文本文件: *.txt)|*.txt";

            if (OpenMediaFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSacnnerWarningSoundFile.Text = OpenMediaFile.FileName;
                string[] FileType = Parameters.CheckTrueFileName(OpenMediaFile.FileName);
                if (FileType[3].ToString() == ".txt" && !BaaseParameter.PlayerMode)
                {
                    txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));

                    if (Parameters.CheckFileEncodingType.FileGetTypeByName(OpenMediaFile.FileName).BodyName.ToLower() == "utf-8")
                    {
                        txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                        BaaseParameter.SpeeckContent = System.Text.Encoding.UTF8.GetString(File.ReadAllBytes(OpenMediaFile.FileName));
                    }
                    else
                    {
                        txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                        MessageBox.Show("打开的文件非UTF-8编码格式！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    BaaseParameter.SoundFile = OpenMediaFile.FileName;
                }
                else if (BaaseParameter.PlayerMode
                    && (FileType[3].ToString() == ".wav"
                    || FileType[3].ToString() == ".mp3"
                    || FileType[3].ToString() == ".avi"
                    || FileType[3].ToString() == ".mp4"
                    || FileType[3].ToString() == ".rmvb"
                    || FileType[3].ToString() == ".mpg"
                    || FileType[3].ToString() == ".mpeg"
                    || FileType[3].ToString() == ".3gp"))
                {
                    txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
                    BaaseParameter.SoundFile = OpenMediaFile.FileName;
                }
                else
                {
                    txtSacnnerWarningSoundFile.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Red"));
                    MessageBox.Show("媒体播放模式与打开的文件格式不匹配！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void chkSacnnerWarningSoundPlayMedia_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                chkSacnnerWarningSoundPlayContent.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    chkSacnnerWarningSoundPlayMedia.IsChecked = true;
                }
            }
        }

        private void chkSacnnerWarningSoundPlayContent_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as CheckBox).IsChecked)
            {
                chkSacnnerWarningSoundPlayMedia.IsChecked = false;
            }
            else
            {
                if (!(bool)(sender as CheckBox).IsChecked)
                {
                    chkSacnnerWarningSoundPlayContent.IsChecked = true;
                }
            }
        }

        private void lblWhiteListColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Color bgColor = new Color();
            if (Parameters.GettingColor(ref bgColor))
            {
                BaaseParameter.WhiteListBackGround = bgColor.ToString();
            }
        }

        private void lblBlackListColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Color bgColor = new Color();
            if (Parameters.GettingColor(ref bgColor))
            {
                BaaseParameter.BlackListBackGround = bgColor.ToString();
            }
        }

        private void lblOtherListColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Color bgColor = new Color();
            if (Parameters.GettingColor(ref bgColor))
            {
                BaaseParameter.OtherListBackGround = bgColor.ToString();
            }
        }

        private void chkSacnnerWarningSoundWhiteList_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.WhiteListMode = (bool)chkSacnnerWarningSoundWhiteList.IsChecked;
        }

        private void chkSacnnerWarningSoundBlackList_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.BlackListMode = (bool)chkSacnnerWarningSoundBlackList.IsChecked;
        }

        private void chkSacnnerWarningSoundOtherList_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.OtherListMode = (bool)chkSacnnerWarningSoundOtherList.IsChecked;
        }

        private void miDefaultWhiteListBackGround_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.WhiteListBackGround = "Transparent";
        }

        private void miDefaultBlackListBackGround_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.BlackListBackGround = "Transparent";
        }

        private void miDefaultOtherListBackGround_Click(object sender, RoutedEventArgs e)
        {
            BaaseParameter.OtherListBackGround = "Transparent";
        }

        private void lblCustomUserTypeSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SubWindow.CustomUserTypeWindow CustomUserTypeWin = new CustomUserTypeWindow();
            CustomUserTypeWin.ShowDialog();
        }

        private void btnFTPParaSettingSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtFTPPort.Text == "" || txtFTPPort.Text == null || !Parameters.ISDigital(txtFTPPort.Text))
                {
                    MessageBox.Show("FTP端口格式不正确,请输入[1~65535]之间的数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (Convert.ToInt32(txtFTPPort.Text) > 65535 || Convert.ToInt32(txtFTPPort.Text) < 1)
                {
                    MessageBox.Show("FTP端口格式不正确,请输入[1~65535]之间的数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (LogDownLoadFTPParameter.Port < 1 || LogDownLoadFTPParameter.Port > 65535)
                {
                    MessageBox.Show("FTP通讯端口格式不正确，请输入[1-65534]之间的数字！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Parameters.DownLoadLogUser = LogDownLoadFTPParameter.ID;
                LogDownLoadFTPParameter.Code = txtPassWord.Password;
                Parameters.DownLoadLogPass = LogDownLoadFTPParameter.Code;
                Parameters.DownLoadLogRoot = LogDownLoadFTPParameter.Root;
                Parameters.DownLoadLogPort = LogDownLoadFTPParameter.Port;
                Parameters.DownLoadLogLastDirWithSN = LogDownLoadFTPParameter.RootWithSN;
                Parameters.UpDonwLoadTimeOutValue = LogDownLoadFTPParameter.UpDateTimeOut;
                Parameters.UpDateFileSourceDir = LogDownLoadFTPParameter.UpDateFileSourceDir;

                Parameters.WriteIniFile("RemoteLogDownLoad", "ID", new DesEncrypt().Encrypt(Parameters.DownLoadLogUser, new DefineCode().Code()), Parameters.INIFile);
                Parameters.WriteIniFile("RemoteLogDownLoad", "Code", new DesEncrypt().Encrypt(Parameters.DownLoadLogPass, new DefineCode().Code()), Parameters.INIFile);
                Parameters.WriteIniFile("RemoteLogDownLoad", "Root", new DesEncrypt().Encrypt(Parameters.DownLoadLogRoot, new DefineCode().Code()), Parameters.INIFile);
                Parameters.WriteIniFile("RemoteLogDownLoad", "Port", new DesEncrypt().Encrypt(Parameters.DownLoadLogPort.ToString(), new DefineCode().Code()), Parameters.INIFile);
                Parameters.WriteIniFile("RemoteLogDownLoad", "LogRootExtend", Parameters.DownLoadLogLastDirWithSN.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Update", "TimeOut", Parameters.UpDonwLoadTimeOutValue.ToString(), Parameters.INIFile);
                Parameters.WriteIniFile("Update", "PackageSource", Parameters.UpDateFileSourceDir, Parameters.INIFile);
                MessageBox.Show("参数设置成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("参数设置失败！", Ex.Message, Ex.StackTrace);
                MessageBox.Show("参数设置失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnFTPParaSettingExit_Click(object sender, RoutedEventArgs e)
        {
            WindowsClose();
        }

        private void btnWinControlEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(bool)chkDefaultWin.IsChecked
                    && !(bool)chkDeviceListWin.IsChecked
                    && !(bool)chkScannerWin.IsChecked
                    && !(bool)chkBlackListWin.IsChecked
                    && !(bool)chkSystemLogsWin.IsChecked)
                {
                    if (MessageBox.Show("未配置配窗口显示模示时，将采用默认配置？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        Parameters.MainWinControlParameter.BlackListWin = (bool)DefaultWindowModelPara.chkBlackListWinDefaultChecked;
                        Parameters.MainWinControlParameter.AllWin = (bool)DefaultWindowModelPara.chkDefaultWinDefaultChecked;
                        Parameters.MainWinControlParameter.DeviceListWin = (bool)DefaultWindowModelPara.chkDeviceListWinDefaultChecked;
                        Parameters.MainWinControlParameter.ScannerWin = (bool)DefaultWindowModelPara.chkScannerWinDefaultChecked;
                        Parameters.MainWinControlParameter.SystemLogsWin = (bool)DefaultWindowModelPara.chkSystemLogsWinDefaultChecked;
                        Parameters.MainWinControlParameter.SettingStatu();
                    }
                    else
                    {
                        Parameters.MainWinControlParameter.BlackListWin = (bool)DefaultWindowModelPara.chkBlackListWinDefaultChecked;
                        Parameters.MainWinControlParameter.AllWin = (bool)DefaultWindowModelPara.chkDefaultWinDefaultChecked;
                        Parameters.MainWinControlParameter.DeviceListWin = (bool)DefaultWindowModelPara.chkDeviceListWinDefaultChecked;
                        Parameters.MainWinControlParameter.ScannerWin = (bool)DefaultWindowModelPara.chkScannerWinDefaultChecked;
                        Parameters.MainWinControlParameter.SystemLogsWin = (bool)DefaultWindowModelPara.chkSystemLogsWinDefaultChecked;
                        return;
                    }
                }
                else
                {
                    DefaultWindowModelPara.chkBlackListWinDefaultChecked = Parameters.MainWinControlParameter.BlackListWin;
                    DefaultWindowModelPara.chkDefaultWinDefaultChecked = Parameters.MainWinControlParameter.AllWin;
                    DefaultWindowModelPara.chkDeviceListWinDefaultChecked = Parameters.MainWinControlParameter.DeviceListWin;
                    DefaultWindowModelPara.chkScannerWinDefaultChecked = Parameters.MainWinControlParameter.ScannerWin;
                    DefaultWindowModelPara.chkSystemLogsWinDefaultChecked = Parameters.MainWinControlParameter.SystemLogsWin;
                    Parameters.MainWinControlParameter.SettingStatu();
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        private void btnWinControlCancel_Click(object sender, RoutedEventArgs e)
        {
            WindowsClose();
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void WindowsClose()
        {
            this.Close();
        }

        private void chkDefaultWin_Click(object sender, RoutedEventArgs e)
        {
            Parameters.MainWinControlParameter.DeviceListWin = false;
            Parameters.MainWinControlParameter.ScannerWin = false;
            Parameters.MainWinControlParameter.BlackListWin = false;
            Parameters.MainWinControlParameter.SystemLogsWin = false;
        }

        private void ChangeUserControlDefault()
        {
            Parameters.MainWinControlParameter.AllWin = false;
        }

        private void chkDeviceListWin_Click(object sender, RoutedEventArgs e)
        {
            ChangeUserControlDefault();
        }

        private void chkScannerWin_Click(object sender, RoutedEventArgs e)
        {
            ChangeUserControlDefault();
        }

        private void chkBlackListWin_Click(object sender, RoutedEventArgs e)
        {
            ChangeUserControlDefault();
        }

        private void chkSystemLogsWin_Click(object sender, RoutedEventArgs e)
        {
            ChangeUserControlDefault();
        }
    }
}
