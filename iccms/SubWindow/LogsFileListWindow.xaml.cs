using DataInterface;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    public class ProgressBarParaControl : INotifyPropertyChanged
    {
        private int _maxValue = 100;
        private int _stepValue = 0;
        private Visibility _tipsWin = Visibility.Collapsed;
        private bool _isDownLoadButtonEnable = true;

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }

            set
            {
                _maxValue = value;
                NotifyPropertyChanged("MaxValue");
            }
        }

        public int StepValue
        {
            get
            {
                return _stepValue;
            }

            set
            {
                _stepValue = value;
                NotifyPropertyChanged("StepValue");
            }
        }

        public Visibility TipsWin
        {
            get
            {
                return _tipsWin;
            }

            set
            {
                _tipsWin = value;
                NotifyPropertyChanged("TipsWin");
            }
        }

        public bool IsDownLoadButtonEnable
        {
            get
            {
                return _isDownLoadButtonEnable;
            }

            set
            {
                _isDownLoadButtonEnable = value;
                NotifyPropertyChanged("IsDownLoadButtonEnable");
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

    public class LogFilesInfo : INotifyPropertyChanged
    {
        private bool _isSelected;
        private string _fileName;
        private string _fileIcon;
        private string _fileType;
        private string _fileSize;
        private string _fileDateTime;
        private int _selectedCount;
        private string _remoteDir;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }

            set
            {
                _fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        public string FileType
        {
            get
            {
                return _fileType;
            }

            set
            {
                _fileType = value;
                NotifyPropertyChanged("FileType");
            }
        }

        public string FileSize
        {
            get
            {
                return _fileSize;
            }

            set
            {
                _fileSize = value;
                NotifyPropertyChanged("FileSize");
            }
        }

        public string FileDateTime
        {
            get
            {
                return _fileDateTime;
            }

            set
            {
                _fileDateTime = value;
                NotifyPropertyChanged("FileDateTime");
            }
        }

        public string FileIcon
        {
            get
            {
                return _fileIcon;
            }

            set
            {
                _fileIcon = value;
                NotifyPropertyChanged("FileIcon");
            }
        }

        public int SelectedCount
        {
            get
            {
                return _selectedCount;
            }

            set
            {
                _selectedCount = value;
                NotifyPropertyChanged("SelectedCount");
            }
        }

        public string RemoteDir
        {
            get
            {
                return _remoteDir;
            }

            set
            {
                _remoteDir = value;
                NotifyPropertyChanged("RemoteDir");
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
    /// LogsFileListWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LogsFileListWindow : Window
    {
        public FtpHelper FTP = null;
        public string FTPRemoteDir = string.Empty;
        private static ObservableCollection<LogFilesInfo> LogFilesInfoList = new ObservableCollection<LogFilesInfo>();

        private static List<LogFilesInfo> FilesSelected = new List<LogFilesInfo>();

        private static ProgressBarParaControl DownLoadLogFilesProgressBarPara = new ProgressBarParaControl();

        private static LogFilesInfo LogFilesInfoPara = new LogFilesInfo();

        public LogsFileListWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCancel.Focus();
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

        private void SelectedAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FilesSelected.Clear();
                for (int i = 0; i < LogFilesInfoList.Count; i++)
                {
                    LogFilesInfoList[i].IsSelected = (bool)SelectedAll.IsChecked;

                    if (LogFilesInfoList[i].IsSelected)
                    {
                        FilesSelected.Add(LogFilesInfoList[i]);
                    }
                    else
                    {
                        FilesSelected.Remove(LogFilesInfoList[i]);
                    }
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("全选日志文件", Ex.Message, Ex.StackTrace);
            }

            LogFilesInfoPara.SelectedCount = FilesSelected.Count;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgLogsFileList.ItemsSource = LogFilesInfoList;
            DownLoadApLogTipsWin.DataContext = DownLoadLogFilesProgressBarPara;
            btnEnter.DataContext = DownLoadLogFilesProgressBarPara;
            txtFileSelected.DataContext = LogFilesInfoPara;
            txtRemoteDir.DataContext = LogFilesInfoPara;

            LogFilesInfoPara.RemoteDir = FTPRemoteDir;
            ListingAPLogFiles();
        }

        private void mmScannerRefresh_Click(object sender, RoutedEventArgs e)
        {
            ListingAPLogFiles();
        }

        private void ListingAPLogFiles()
        {
            try
            {
                LogFilesInfoList.Clear();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }

            new Thread(() =>
            {
                //显示FTP服务器日志文件到界面列表
                string[] FileInfo = FTP.List(null);
                for (int i = 0; i < FileInfo.Length; i++)
                {
                    try
                    {
                        List<string> TmpFileInfo = FileInfo[i].Split(new char[] { ' ' }).ToList();
                        if (!new Regex("d").Match(TmpFileInfo[0]).Success)
                        {
                            LogFilesInfo Item = new LogFilesInfo();

                            //文件名
                            Item.FileName = TmpFileInfo[TmpFileInfo.Count - 1];

                            //图标
                            FileInfo SelfFileInfo = new FileInfo(Item.FileName);
                            Item.FileIcon = Parameters.FileIcon.GetFileTypeIcon(SelfFileInfo.Extension);

                            //文件类型
                            string SelfFileType = SelfFileInfo.Extension.Substring(1, SelfFileInfo.Extension.Length - 1);
                            if (new Regex("rar").Match(SelfFileType).Success
                               || new Regex("zip").Match(SelfFileType).Success
                               || new Regex("tar").Match(SelfFileType).Success
                               || new Regex("tar.gz").Match(SelfFileType).Success
                               || new Regex("gz").Match(SelfFileType).Success
                               )
                            {
                                Item.FileType = SelfFileType.ToUpper() + " 压缩文件";
                            }
                            else if (new Regex("txt").Match(SelfFileType).Success)
                            {
                                Item.FileType = SelfFileType.ToUpper() + " 文本文件";
                            }
                            else if (new Regex("log").Match(SelfFileType).Success)
                            {
                                Item.FileType = SelfFileType.ToUpper() + " 日志文件";
                            }
                            else
                            {
                                Item.FileType = SelfFileType.ToUpper() + " 文件";
                            }

                            //修改日期
                            Item.FileDateTime = string.Format(
                                                                "{0}{1}{2}{3}{4}",
                                                                TmpFileInfo[TmpFileInfo.Count - 4],
                                                                " ",
                                                                TmpFileInfo[TmpFileInfo.Count - 3],
                                                                " ",
                                                                TmpFileInfo[TmpFileInfo.Count - 2]
                                                             );

                            //文件大小(KB)
                            Item.FileSize = (Math.Round((Convert.ToDouble(TmpFileInfo[TmpFileInfo.Count - 5]) / 1024), 2)).ToString();

                            TmpFileInfo.Clear();

                            Dispatcher.Invoke(() =>
                            {
                                LogFilesInfoList.Add(Item);
                            });
                        }
                    }
                    catch (Exception Ex)
                    {
                        Parameters.PrintfLogsExtended("显示FTP服务器日志文件列表", Ex.Message, Ex.StackTrace);
                    }
                }

                GC.Collect();
            }).Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                LogFilesInfoPara.SelectedCount = 0;
                LogFilesInfoPara.RemoteDir = string.Empty;
                FilesSelected.Clear();
                FTP.DisConnect();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended("关闭FTP连接", Ex.Message, Ex.StackTrace);
            }
        }

        private void dgLogsFileList_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgLogsFileList.CurrentItem != null)
            {
                if (dgLogsFileList.CurrentItem.ToString() != string.Format("{0}{1}{2}", "{", "NewItemPlaceholder", "}"))
                {
                    if (dgLogsFileList.CurrentColumn.SortMemberPath.ToLower() == ("IsSelected").ToLower())
                    {
                        bool value = (dgLogsFileList.CurrentItem as LogFilesInfo).IsSelected;
                        string SelfFileName = (dgLogsFileList.CurrentItem as LogFilesInfo).FileName;

                        //已选则反选
                        if (value)
                        {
                            for (int i = 0; i < LogFilesInfoList.Count; i++)
                            {
                                if (SelfFileName == LogFilesInfoList[i].FileName)
                                {
                                    LogFilesInfoList[i].IsSelected = false;
                                    break;
                                }
                            }

                            for (int i = 0; i < FilesSelected.Count; i++)
                            {
                                if (SelfFileName == FilesSelected[i].FileName)
                                {
                                    FilesSelected.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        //未选则选择
                        else
                        {
                            for (int i = 0; i < LogFilesInfoList.Count; i++)
                            {
                                if (SelfFileName == LogFilesInfoList[i].FileName)
                                {
                                    LogFilesInfoList[i].IsSelected = true;
                                    FilesSelected.Add(LogFilesInfoList[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            LogFilesInfoPara.SelectedCount = FilesSelected.Count;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            bool Flag = false;
            StringBuilder ErrMsg = new StringBuilder();
            StringBuilder DwldErrMsg = new StringBuilder();

            if (FilesSelected.Count <= 0)
            {
                MessageBox.Show("请选择要下载的AP日志文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                DownLoadLogFilesProgressBarPara.TipsWin = Visibility.Visible;
                DownLoadLogFilesProgressBarPara.IsDownLoadButtonEnable = false;
            }

            if (FTP != null)
            {
                if (FTP.Connected)
                {
                    new Thread(() =>
                    {
                        //进度完成值
                        DownLoadLogFilesProgressBarPara.MaxValue = FilesSelected.Count;
                        DownLoadLogFilesProgressBarPara.StepValue = 0;
                        string DownLoadFileLocalDir = string.Empty;

                        try
                        {
                            if (MainWindow.aDeviceSelected.Model == DeviceType.WCDMA)
                            {
                                if (!Directory.Exists(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles))
                                {
                                    Directory.CreateDirectory(JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles);
                                }
                                DownLoadFileLocalDir = JsonInterFace.WCDMADeviceSystemMaintenenceParameter.LogFiles;
                            }
                            else if (new Regex(DeviceType.LTE).Match(MainWindow.aDeviceSelected.Model).Success)
                            {
                                if (!Directory.Exists(JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles))
                                {
                                    Directory.CreateDirectory(JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles);
                                }
                                DownLoadFileLocalDir = JsonInterFace.LteDeviceSystemMaintenenceParameter.LogFiles;
                            }
                            else if (new Regex(DeviceType.TD_SCDMA).Match(MainWindow.aDeviceSelected.Model).Success)
                            {
                                if (!Directory.Exists(JsonInterFace.TDSDeviceSystemMaintenenceParameter.LogFiles))
                                {
                                    Directory.CreateDirectory(JsonInterFace.TDSDeviceSystemMaintenenceParameter.LogFiles);
                                }
                                DownLoadFileLocalDir = JsonInterFace.TDSDeviceSystemMaintenenceParameter.LogFiles;
                            }
                        }
                        catch (Exception Ex)
                        {
                            if (MainWindow.aDeviceSelected.SN != "" && MainWindow.aDeviceSelected.SN != null)
                            {
                                if (!Directory.Exists(Parameters.ApLogDir + @"\" + MainWindow.aDeviceSelected.SN))
                                {
                                    Directory.CreateDirectory(Parameters.ApLogDir + @"\" + MainWindow.aDeviceSelected.SN);
                                }
                                DownLoadFileLocalDir = Parameters.ApLogDir + @"\" + MainWindow.aDeviceSelected.SN;
                            }
                            else
                            {
                                if (!Directory.Exists(Parameters.ApLogDir + @"\Temp"))
                                {
                                    Directory.CreateDirectory(Parameters.ApLogDir + @"\Temp");
                                }
                                DownLoadFileLocalDir = Parameters.ApLogDir + @"\Temp";
                            }


                            ErrMsg.AppendLine("------------ [" + DateTime.Now.ToString() + "] ------------");
                            ErrMsg.AppendLine("错误信息：" + Ex.Message);
                            ErrMsg.AppendLine("检测到批定用于存放AP日志文件的目标目录无效，系统将AP日志文件存放到以下路径：[" + DownLoadFileLocalDir + "]");
                            ErrMsg.AppendLine("-----------------------------------------------------------");
                            IntPtr ErrMsgHandle = Marshal.StringToBSTR(ErrMsg.ToString());
                            Parameters.SendMessage(Parameters.DeviceManageWinHandle, Parameters.WM_DOWNLOAD_AP_LOGS_ERROR_MESSAGE, 0, (int)ErrMsgHandle);
                            ErrMsg.Clear();
                        }

                        for (int i = 0; i < FilesSelected.Count; i++)
                        {
                            try
                            {
                                if (Directory.Exists(DownLoadFileLocalDir))
                                {
                                    if (NetWorkClient.ControllerServer.Connected)
                                    {
                                        if (FTP.Connected)
                                        {
                                            FTP.Get(FilesSelected[i].FileName, DownLoadFileLocalDir, FilesSelected[i].FileName);
                                            Flag = false;
                                        }
                                        else
                                        {
                                            Flag = false;
                                            DwldErrMsg.AppendLine("文件[" + FilesSelected[i].FileName + "]下载失败！");
                                            DwldErrMsg.AppendLine("原因[FTP连接已断开！]");
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Flag = false;
                                        DwldErrMsg.AppendLine("文件[" + FilesSelected[i].FileName + "]下载失败！");
                                        DwldErrMsg.AppendLine("原因：网络与服务器连接已断开！]");
                                        break;
                                    }
                                }
                                else
                                {
                                    Flag = true;
                                    Parameters.PrintfLogsExtended("目标文件路径[" + DownLoadFileLocalDir + "]无效,不可用！");
                                    break;
                                }
                            }
                            catch (Exception Ex)
                            {
                                Flag = true;
                                DwldErrMsg.AppendLine("文件[" + FilesSelected[i].FileName + "]下载失败！");
                                DwldErrMsg.AppendLine("原因[" + Ex.Message + "]");
                                Parameters.PrintfLogsExtended("下载AP目志文件" + Environment.NewLine + DwldErrMsg.ToString(), Ex.Message, Ex.StackTrace);
                                break;
                            }

                            //进度显示
                            DownLoadLogFilesProgressBarPara.StepValue = i + 1;
                        }

                        DownLoadLogFilesProgressBarPara.TipsWin = Visibility.Collapsed;
                        DownLoadLogFilesProgressBarPara.IsDownLoadButtonEnable = true;
                        DownLoadLogFilesProgressBarPara.StepValue = 0;

                        if (Flag)
                        {
                            ErrMsg.AppendLine("------------ [" + DateTime.Now.ToString() + "] ------------");
                            ErrMsg.AppendLine("系统将AP日志文件存放到以下路径：[" + DownLoadFileLocalDir + "]");
                            ErrMsg.AppendLine("AP日志文件下载到[" + DownLoadFileLocalDir + "]失败：");
                            ErrMsg.AppendLine(DwldErrMsg.ToString());
                            ErrMsg.AppendLine("-----------------------------------------------------------");

                            IntPtr ErrMsgHandle = Marshal.StringToBSTR(ErrMsg.ToString());
                            Parameters.SendMessage(Parameters.DeviceManageWinHandle, Parameters.WM_DOWNLOAD_AP_LOGS_ERROR_MESSAGE, 0, (int)ErrMsgHandle);
                            MessageBox.Show("AP日志文件下载到[" + DownLoadFileLocalDir + "]失败：" + Environment.NewLine + ErrMsg.ToString(), "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            ErrMsg.AppendLine("------------ [" + DateTime.Now.ToString() + "] ------------");
                            ErrMsg.AppendLine("系统将AP日志文件存放到以下路径：[" + DownLoadFileLocalDir + "]");
                            ErrMsg.AppendLine("AP日志文件下载到[" + DownLoadFileLocalDir + "]成功！");
                            ErrMsg.AppendLine("-----------------------------------------------------------");

                            IntPtr ErrMsgHandle = Marshal.StringToBSTR(ErrMsg.ToString());
                            Parameters.SendMessage(Parameters.DeviceManageWinHandle, Parameters.WM_DOWNLOAD_AP_LOGS_ERROR_MESSAGE, 0, (int)ErrMsgHandle);
                            MessageBox.Show("AP日志文件下载到[" + DownLoadFileLocalDir + "]成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        DwldErrMsg.Clear();
                        ErrMsg.Clear();
                    }).Start();
                }
            }
        }
    }
}
