using DataInterface;
using IODataControl;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace iccms.NavigatePages
{
    [ValueConversion(typeof(string), typeof(Brushes))]
    public class BGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string UserType = ((string)value).Trim();
            if (UserType == Parameters.UserTypes.BlackList)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Parameters.ScannerDataControlParameter.BlackListBackGround));
            }
            else if (UserType == Parameters.UserTypes.OtherList)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Parameters.ScannerDataControlParameter.OtherListBackGround));
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(Parameters.ScannerDataControlParameter.WhiteListBackGround));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    #region 捕号实时上报过滤条件
    public class ScannerDataConditionClass : INotifyPropertyChanged
    {
        private string imsi;
        private string startData;
        private string startTime;
        private string startHour;
        private string startMinute;
        private string startSecond;
        private string endDate;
        private string endTime;
        private string endHour;
        private string endMinute;
        private string endSecond;
        private string deviceName;
        private string userType;

        //构造
        public ScannerDataConditionClass()
        {
            StartHour = "00";
            StartMinute = "00";
            StartSecond = "00";

            EndHour = "23";
            EndMinute = "59";
            EndSecond = "59";
        }

        public string IMSI
        {
            get
            {
                return imsi;
            }

            set
            {
                imsi = value;
                NotifyPropertyChanged("IMSI");
            }
        }
        public string StartTime
        {
            get
            {
                return startTime;
            }

            set
            {
                startTime = value;
                NotifyPropertyChanged("StartTime");
            }
        }
        public string EndTime
        {
            get
            {
                return endTime;
            }

            set
            {
                endTime = value;
                NotifyPropertyChanged("EndTime");
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
                NotifyPropertyChanged("DeviceName");
            }
        }
        public string UserType
        {
            get
            {
                return userType;
            }

            set
            {
                userType = value;
                NotifyPropertyChanged("UserType");
            }
        }

        public string StartData
        {
            get
            {
                return startData;
            }

            set
            {
                startData = value;
                NotifyPropertyChanged("StartData");
            }
        }

        public string StartHour
        {
            get
            {
                return startHour;
            }

            set
            {
                startHour = value;
                NotifyPropertyChanged("StartHour");
            }
        }

        public string StartMinute
        {
            get
            {
                return startMinute;
            }

            set
            {
                startMinute = value;
                NotifyPropertyChanged("StartMinute");
            }
        }

        public string StartSecond
        {
            get
            {
                return startSecond;
            }

            set
            {
                startSecond = value;
                NotifyPropertyChanged("StartSecond");
            }
        }

        public string EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                endDate = value;
                NotifyPropertyChanged("EndDate");
            }
        }

        public string EndHour
        {
            get
            {
                return endHour;
            }

            set
            {
                endHour = value;
                NotifyPropertyChanged("EndHour");
            }
        }

        public string EndMinute
        {
            get
            {
                return endMinute;
            }

            set
            {
                endMinute = value;
                NotifyPropertyChanged("EndMinute");
            }
        }

        public string EndSecond
        {
            get
            {
                return endSecond;
            }

            set
            {
                endSecond = value;
                NotifyPropertyChanged("EndSecond");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
    }
    #endregion

    #region 黑名单上报条件
    public class BlackNameConditionClass : ScannerDataConditionClass
    {
        private string callerloc;

        public BlackNameConditionClass()
        {
            this.StartHour = "00";
            this.StartMinute = "00";
            this.StartSecond = "00";

            this.EndHour = "23";
            this.EndMinute = "59";
            this.EndSecond = "59";
        }

        public string Callerloc
        {
            get
            {
                return callerloc;
            }

            set
            {
                callerloc = value;
                NotifyPropertyChanged("Callerloc");
            }
        }
    }
    #endregion

    /// <summary>
    /// UEInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UEInfoWindow : Page
    {
        private object LanguageClassScannerInfoFillterSymbol = null;
        private object LanguageClassScannerInfoDataGridSymbol = null;
        private object LanguageClassBlackInfoFillterSymbol = null;
        private object LanguageClassBlackInfoDataGridSymbol = null;
        public static System.Timers.Timer ChangeDataRowBackGroundTimer = null;

        //实时上报捕号条件
        private static ScannerDataConditionClass ScannerDataConditionParameter = new ScannerDataConditionClass();
        //黑名单上报条件
        private static BlackNameConditionClass BlackNameConditionParameter = new BlackNameConditionClass();

        //界面容器
        private Dictionary<string, Uri> SubWindows = new Dictionary<string, Uri>();

        //GSM,LTE实时上报
        public static Thread ScannerReportRealTimeThread = null;
        public static ScannerDataClass SelectScannerDataInfo = new ScannerDataClass();
        public static ObservableCollection<ScannerDataClass> SelfScannerReportInfo = new ObservableCollection<ScannerDataClass>();

        //黑名单追踪
        private Thread MeasReportBlackListThread = null;
        public static ObservableCollection<MeasReportBlackListClass> SelfMeasReportBlackList = new ObservableCollection<MeasReportBlackListClass>();

        //黑名单追踪(典线图)信息获取与删除,更新
        private Thread MeasReportAxialChartBlackListGettingThread = null;
        private Thread MeasReportAxialChartBlackListRemovingThread = null;
        private static ObservableCollection<MeasReportBlackListClass> ChartIMSIDataList = new ObservableCollection<MeasReportBlackListClass>();

        //GSMV2_ZYF GSM_HJT 通话记录 短信记录实时信息
        private static Thread GSMPhoneNumberRecordInfoThread = null;
        private static Thread GSMSMSRecordInfoThread = null;
        private static ObservableCollection<GSMPhoneNumberSMSRecordInfoClass> PhoneNumberRecordList = new ObservableCollection<GSMPhoneNumberSMSRecordInfoClass>();
        private static ObservableCollection<GSMPhoneNumberSMSRecordInfoClass> SMSRecordList = new ObservableCollection<GSMPhoneNumberSMSRecordInfoClass>();

        //加载全名
        private static Thread AddFullNameThread = null;
        private static ObservableCollection<string> FullNameList = new ObservableCollection<string>();

        //构造
        public UEInfoWindow()
        {
            InitializeComponent();

            //初始化语言
            try
            {
                if (Parameters.LanguageType.Equals("EN"))
                {
                    LanguageClassScannerInfoFillterSymbol = new Language_EN.UEScannerReports.ScannerInfoSearch();
                    LanguageClassScannerInfoDataGridSymbol = new Language_EN.UEScannerReports.ScannerInfoDataGrid();
                    LanguageClassBlackInfoFillterSymbol = new Language_EN.UEBWLists.BlcakListInfoSearch();
                    LanguageClassBlackInfoDataGridSymbol = new Language_EN.UEBWLists.BlcakListInfoDataGrid();
                }
                else
                {
                    LanguageClassScannerInfoFillterSymbol = new Language_CN.UEScannerReports.ScannerInfoSearch();
                    LanguageClassScannerInfoDataGridSymbol = new Language_CN.UEScannerReports.ScannerInfoDataGrid();
                    LanguageClassBlackInfoFillterSymbol = new Language_CN.UEBWLists.BlcakListInfoSearch();
                    LanguageClassBlackInfoDataGridSymbol = new Language_CN.UEBWLists.BlcakListInfoDataGrid();
                }

                SubWindows.Add("MeasurementReportChartWindow", new Uri("NavigatePages/MeasurementReportChartWindow.xaml", UriKind.Relative));

                if (MeasReportBlackListThread == null)
                {
                    MeasReportBlackListThread = new Thread(new ThreadStart(ShowMeasReportBlackListInfo));
                    MeasReportBlackListThread.Start();
                }

                if (ScannerReportRealTimeThread == null)
                {
                    ScannerReportRealTimeThread = new Thread(new ThreadStart(ShowScannerReportInfo));
                    ScannerReportRealTimeThread.Start();
                }

                if (ChangeDataRowBackGroundTimer == null)
                {
                    ChangeDataRowBackGroundTimer = new System.Timers.Timer();
                    ChangeDataRowBackGroundTimer.AutoReset = false;
                    ChangeDataRowBackGroundTimer.Interval = 1;
                    ChangeDataRowBackGroundTimer.Elapsed += ChangeDataRowBackGroundTimer_Elapsed;
                }

                if (GSMPhoneNumberRecordInfoThread == null)
                {
                    GSMPhoneNumberRecordInfoThread = new Thread(new ThreadStart(GSMPhoneNumberRecordInfo));
                    GSMPhoneNumberRecordInfoThread.Start();
                }

                if (GSMSMSRecordInfoThread == null)
                {
                    GSMSMSRecordInfoThread = new Thread(new ThreadStart(GSMSMSRecordInfo));
                    GSMSMSRecordInfoThread.Start();
                }

                if (AddFullNameThread == null)
                {
                    AddFullNameThread = new Thread(new ThreadStart(AddFullNameList));
                    AddFullNameThread.Priority = ThreadPriority.Lowest;
                    AddFullNameThread.Start();
                }

                if (MeasReportAxialChartBlackListGettingThread == null)
                {
                    MeasReportAxialChartBlackListGettingThread = new Thread(new ThreadStart(MeasReportAxialChartBlackListGetting));
                    MeasReportAxialChartBlackListGettingThread.Start();
                }

                if (MeasReportAxialChartBlackListRemovingThread == null)
                {
                    MeasReportAxialChartBlackListRemovingThread = new Thread(new ThreadStart(MeasReportAxialChartBlackListRemoving));
                    MeasReportAxialChartBlackListRemovingThread.Start();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("UE Window InitializeComponent...", ex.Message, ex.StackTrace);
            }
        }

        //获取图表IMSI信息
        private void MeasReportAxialChartBlackListGetting()
        {
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        MeasReportBlackListClass[] BlackIMSIInfo = new MeasReportBlackListClass[SelfMeasReportBlackList.Count];
                        SelfMeasReportBlackList.CopyTo(BlackIMSIInfo, 0);

                        for (int i = 0; i < BlackIMSIInfo.Length; i++)
                        {
                            if (MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows.Count > 0)
                            {
                                bool Flag = true;
                                for (int j = 0; j < MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows.Count; j++)
                                {
                                    if (BlackIMSIInfo[i].IMSI == MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows[j]["IMSI"].ToString())
                                    {
                                        Flag = false;
                                        break;
                                    }
                                }

                                //不重复添加 True
                                if (Flag)
                                {
                                    MeasurementReportChartWindow.SelfChart.Input(BlackIMSIInfo[i].IMSI, BlackIMSIInfo[i].Intensity);
                                    MeasReportBlackListClass MeaIMSIInfo = new MeasReportBlackListClass();
                                    MeaIMSIInfo.IMSI = BlackIMSIInfo[i].IMSI;
                                    MeaIMSIInfo.Intensity = BlackIMSIInfo[i].Intensity;
                                    ChartIMSIDataList.Add(MeaIMSIInfo);
                                }
                                //重复更新 False
                                else
                                {
                                    MeasurementReportChartWindow.SelfChart.Update(BlackIMSIInfo[i].IMSI, BlackIMSIInfo[i].Intensity);
                                }
                            }
                            else
                            {
                                MeasurementReportChartWindow.SelfChart.Input(BlackIMSIInfo[i].IMSI, BlackIMSIInfo[i].Intensity);
                                MeasReportBlackListClass MeaIMSIInfo = new MeasReportBlackListClass();
                                MeaIMSIInfo.IMSI = BlackIMSIInfo[i].IMSI;
                                MeaIMSIInfo.Intensity = BlackIMSIInfo[i].Intensity;
                                ChartIMSIDataList.Add(MeaIMSIInfo);
                            }
                        }

                        Array.Clear(BlackIMSIInfo, 0, ((BlackIMSIInfo.Length - 1) >= 0 ? (BlackIMSIInfo.Length - 1) : 0));
                        BlackIMSIInfo = null;
                    });
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
                Thread.Sleep(300);
            }
        }

        //删除多余图表IMSI信息
        private void MeasReportAxialChartBlackListRemoving()
        {
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        MeasReportBlackListClass[] BlackIMSIInfo = new MeasReportBlackListClass[SelfMeasReportBlackList.Count];
                        SelfMeasReportBlackList.CopyTo(BlackIMSIInfo, 0);

                        for (int j = 0; j < MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows.Count; j++)
                        {
                            bool Flag = true;
                            for (int i = 0; i < BlackIMSIInfo.Length; i++)
                            {
                                if (BlackIMSIInfo[i].IMSI == MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows[j]["IMSI"].ToString())
                                {
                                    Flag = false;
                                    break;
                                }
                            }

                            if (Flag)
                            {
                                for (int i = 0; i < ChartIMSIDataList.Count; i++)
                                {
                                    if (ChartIMSIDataList[i].IMSI == MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows[j]["IMSI"].ToString())
                                    {
                                        ChartIMSIDataList.RemoveAt(i);
                                        break;
                                    }
                                }

                                MeasurementReportChartWindow.SelfChart.IMSI = string.Empty;
                                MeasurementReportChartWindow.SelfChart.Remove(MeasurementReportChartWindow.SelfChart.MeasReportAxialChartBlackList.Rows[j]["IMSI"].ToString());
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }

                Thread.Sleep(1000);
            }
        }

        private void ChangeDataRowBackGroundTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ChangeDataRowBackGroundStart(sender, e);
        }

        public static void ApplayDataRowBackGroundColor()
        {
            try
            {
                ChangeDataRowBackGroundTimer.Start();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("改更Scanner数据背景色失败", ex.Message, ex.StackTrace);
                MessageBox.Show("改更Scanner数据背景色失败." + ex.Message + "！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //背景颜色更改
        private void ChangeDataRowBackGroundStart(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                mmScannerDataRefresh_Click(sender, new RoutedEventArgs());
            });
        }

        /// <summary>
        /// 实时显示黑名单测量报告
        /// </summary>
        private void ShowMeasReportBlackListInfo()
        {
            string _ID = string.Empty;
            int DataCount = 0;
            DataTable BlackListDataView = JsonInterFace.MeasReportBlackList.MeasReportDataTable.Clone();

            while (true)
            {
                if (JsonInterFace.MeasReportBlackList.MeasReportDataTable.Rows.Count > 0)
                {
                    try
                    {
                        DataCount = JsonInterFace.MeasReportBlackList.MeasReportDataTable.Rows.Count;

                        //过滤
                        if ((BlackNameConditionParameter.IMSI != "" && BlackNameConditionParameter.IMSI != null)
                            || (BlackNameConditionParameter.DeviceName != "" && BlackNameConditionParameter.DeviceName != null)
                            || (BlackNameConditionParameter.StartTime != "" && BlackNameConditionParameter.StartTime != null)
                            || (BlackNameConditionParameter.EndTime != "" && BlackNameConditionParameter.EndTime != null)
                            || (BlackNameConditionParameter.Callerloc != "" && BlackNameConditionParameter.Callerloc != null))
                        {
                            string _StartTime = BlackNameConditionParameter.StartTime + " " + BlackNameConditionParameter.StartHour + ":" + BlackNameConditionParameter.StartMinute + ":" + BlackNameConditionParameter.StartSecond;
                            string _EndTime = BlackNameConditionParameter.EndTime + " " + BlackNameConditionParameter.EndHour + ":" + BlackNameConditionParameter.EndMinute + ":" + BlackNameConditionParameter.EndSecond;
                            BlackListDataView.Clear();
                            MeasureReportDataFilter(
                                                    ref BlackListDataView,
                                                    JsonInterFace.MeasReportBlackList.MeasReportDataTable,
                                                    BlackNameConditionParameter.IMSI,
                                                    BlackNameConditionParameter.DeviceName,
                                                    _StartTime,
                                                    _EndTime,
                                                    BlackNameConditionParameter.Callerloc
                                                   );
                        }
                        //不过滤
                        else
                        {
                            lock (JsonInterFace.MeasReportBlackList.Mutex_DbHelper)
                            {
                                BlackListDataView = JsonInterFace.MeasReportBlackList.MeasReportDataTable.Copy();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                    }
                }

                if (DataCount > 0)
                {
                    try
                    {
                        for (int i = 0; i < BlackListDataView.Rows.Count; i++)
                        {
                            bool Flag = true;
                            int Item = 0;

                            //添加一条
                            MeasReportBlackListClass ADataRow = new MeasReportBlackListClass()
                            {
                                //IMSI
                                IMSI = BlackListDataView.Rows[i][0].ToString(),
                                //DTime
                                DTime = BlackListDataView.Rows[i][1].ToString(),
                                //TMSI
                                TMSI = BlackListDataView.Rows[i][2].ToString(),
                                //IMEI
                                IMEI = BlackListDataView.Rows[i][3].ToString(),
                                //Intensity
                                Intensity = BlackListDataView.Rows[i][4].ToString(),
                                //Operators (运营商)
                                Operators = BlackListDataView.Rows[i][5].ToString(),
                                //DomainName(号归属地)
                                DomainName = BlackListDataView.Rows[i][6].ToString(),
                                //DeviceName
                                DeviceName = BlackListDataView.Rows[i][7].ToString()
                            };

                            Dispatcher.Invoke(() =>
                            {
                                for (int j = 0; j < SelfMeasReportBlackList.Count; j++)
                                {
                                    if (SelfMeasReportBlackList[j].IMSI == ADataRow.IMSI
                                        && SelfMeasReportBlackList[j].IMEI == ADataRow.IMEI)
                                    {
                                        Flag = false;
                                        Item = j;
                                        break;
                                    }
                                }

                                //添加
                                if (Flag)
                                {
                                    SelfMeasReportBlackList.Add(ADataRow);
                                }
                                //更新
                                else
                                {
                                    SelfMeasReportBlackList[Item].IMSI = ADataRow.IMSI;
                                    SelfMeasReportBlackList[Item].DTime = ADataRow.DTime;
                                    SelfMeasReportBlackList[Item].TMSI = ADataRow.TMSI;
                                    SelfMeasReportBlackList[Item].IMEI = ADataRow.IMEI;
                                    SelfMeasReportBlackList[Item].Intensity = ADataRow.Intensity;
                                    SelfMeasReportBlackList[Item].Operators = ADataRow.Operators;
                                    SelfMeasReportBlackList[Item].DomainName = ADataRow.DomainName;
                                    SelfMeasReportBlackList[Item].DeviceName = ADataRow.DeviceName;
                                }
                            });

                            ADataRow = null;
                        }

                        BlackListDataView.Rows.Clear();
                        DataCount = 0;
                        System.GC.Collect();

                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended("黑名单追踪上报显示", ex.Message, ex.StackTrace);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 显示实时上报信息
        /// </summary>
        private void ShowScannerReportInfo()
        {
            string _ID = string.Empty;
            int DataCount = 0;
            DataTable ScannerDataView = JsonInterFace.ScannerData.ScannerDataTable.Clone();
            while (true)
            {
                lock (JsonInterFace.ScannerData.Mutex_DbHelper)
                {
                    if (JsonInterFace.ScannerData.ScannerDataTable.Rows.Count > 0)
                    {
                        DataCount = JsonInterFace.ScannerData.ScannerDataTable.Rows.Count;

                        //过滤
                        if ((ScannerDataConditionParameter.IMSI != "" && ScannerDataConditionParameter.IMSI != null)
                            || (ScannerDataConditionParameter.DeviceName != "" && ScannerDataConditionParameter.DeviceName != null)
                            || (ScannerDataConditionParameter.StartTime != "" && ScannerDataConditionParameter.StartTime != null)
                            || (ScannerDataConditionParameter.EndTime != "" && ScannerDataConditionParameter.EndTime != null)
                            || (ScannerDataConditionParameter.UserType != "" && ScannerDataConditionParameter.UserType != null))
                        {
                            string _StartTime = ScannerDataConditionParameter.StartTime + " " + ScannerDataConditionParameter.StartHour + ":" + ScannerDataConditionParameter.StartMinute + ":" + ScannerDataConditionParameter.StartSecond;
                            string _EndTime = ScannerDataConditionParameter.EndTime + " " + ScannerDataConditionParameter.EndHour + ":" + ScannerDataConditionParameter.EndMinute + ":" + ScannerDataConditionParameter.EndSecond;
                            ScannerDataView.Clear();
                            ScannerDataFilter(
                                                ref ScannerDataView,
                                                JsonInterFace.ScannerData.ScannerDataTable,
                                                ScannerDataConditionParameter.IMSI,
                                                ScannerDataConditionParameter.DeviceName,
                                                _StartTime,
                                                _EndTime,
                                                ScannerDataConditionParameter.UserType
                                             );
                        }
                        //不过滤
                        else
                        {
                            ScannerDataView = JsonInterFace.ScannerData.ScannerDataTable.Copy();
                        }
                    }
                }

                if (DataCount > 0)
                {
                    try
                    {
                        Dispatcher.Invoke(() =>
                        {
                            int NewDataCount = ScannerDataView.Rows.Count;
                            int SourceDataCount = SelfScannerReportInfo.Count;
                            int OverrideCount = 0;

                            //新比旧多
                            if (NewDataCount > SourceDataCount)
                            {
                                OverrideCount = ScannerDataView.Rows.Count - (ScannerDataView.Rows.Count - SelfScannerReportInfo.Count);
                            }
                            //旧比新多
                            else if (SourceDataCount > NewDataCount)
                            {
                                OverrideCount = SelfScannerReportInfo.Count - ScannerDataView.Rows.Count;

                                //特殊处理先删除多余项
                                for (int i = 0; i < OverrideCount; i++)
                                {
                                    SelfScannerReportInfo.RemoveAt(i);
                                }
                            }
                            //相等
                            else if (NewDataCount == SourceDataCount)
                            {
                                OverrideCount = ScannerDataView.Rows.Count - SelfScannerReportInfo.Count;
                            }

                            for (int i = 0; i < ScannerDataView.Rows.Count; i++)
                            {
                                bool Flag = true;

                                if (SelfScannerReportInfo.Count > 0)
                                {
                                    _ID = (int.Parse(SelfScannerReportInfo[SelfScannerReportInfo.Count - 1].ID) + 1).ToString();
                                }
                                else
                                {
                                    _ID = "1";
                                }

                                //添加一条
                                ScannerDataClass ADataRow = new ScannerDataClass()
                                {
                                    //ID
                                    ID = _ID,
                                    //IMSI
                                    IMSI = ScannerDataView.Rows[i][1].ToString(),
                                    //DTime
                                    DTime = ScannerDataView.Rows[i][2].ToString(),
                                    //UserType
                                    UserType = ScannerDataView.Rows[i][3].ToString(),
                                    //TMSI
                                    TMSI = ScannerDataView.Rows[i][4].ToString(),
                                    //IMEI
                                    IMEI = ScannerDataView.Rows[i][5].ToString(),
                                    //Intensity
                                    Intensity = ScannerDataView.Rows[i][6].ToString(),
                                    //Operators (运营商)
                                    Operators = ScannerDataView.Rows[i][7].ToString(),
                                    //DomainName(号归属地)
                                    DomainName = ScannerDataView.Rows[i][8].ToString(),
                                    //DeviceName
                                    DeviceName = ScannerDataView.Rows[i][9].ToString(),
                                    //Des
                                    Des = ScannerDataView.Rows[i][10].ToString()
                                };

                                //先不检测重复项
                                /*
                                for (int j = 0; j < SelfScannerReportInfo.Count; j++)
                                {
                                    if (SelfScannerReportInfo[j].IMSI == ADataRow.IMSI
                                    && SelfScannerReportInfo[j].DTime == ADataRow.DTime
                                    && SelfScannerReportInfo[j].UserType == ADataRow.UserType
                                    && SelfScannerReportInfo[j].TMSI == ADataRow.TMSI
                                    && SelfScannerReportInfo[j].IMEI == ADataRow.IMEI
                                    && SelfScannerReportInfo[j].Intensity == ADataRow.Intensity
                                    && SelfScannerReportInfo[j].Operators == ADataRow.Operators
                                    && SelfScannerReportInfo[j].DomainName == ADataRow.DomainName
                                    && SelfScannerReportInfo[j].DeviceName == ADataRow.DeviceName)
                                    {
                                        Flag = false;
                                        break;
                                    }
                                }
                                */

                                if (Flag)
                                {
                                    //相等全复盖 或 旧比新多 经上面特殊处理后，直接覆盖
                                    if (NewDataCount == SourceDataCount || SourceDataCount > NewDataCount)
                                    {
                                        SelfScannerReportInfo[i].ID = (i + 1).ToString();
                                        SelfScannerReportInfo[i].IMSI = ADataRow.IMSI;
                                        SelfScannerReportInfo[i].DTime = ADataRow.DTime;
                                        SelfScannerReportInfo[i].UserType = ADataRow.UserType;
                                        SelfScannerReportInfo[i].TMSI = ADataRow.TMSI;
                                        SelfScannerReportInfo[i].IMEI = ADataRow.IMEI;
                                        SelfScannerReportInfo[i].Intensity = ADataRow.Intensity;
                                        SelfScannerReportInfo[i].Operators = ADataRow.Operators;
                                        SelfScannerReportInfo[i].DomainName = ADataRow.DomainName;
                                        SelfScannerReportInfo[i].DeviceName = ADataRow.DeviceName;
                                        SelfScannerReportInfo[i].Des = ADataRow.Des;
                                    }
                                    //新比旧多，复盖+添加
                                    else if (NewDataCount > SourceDataCount)
                                    {
                                        //复盖
                                        if (i < OverrideCount)
                                        {
                                            SelfScannerReportInfo[i].ID = (i + 1).ToString();
                                            SelfScannerReportInfo[i].IMSI = ADataRow.IMSI;
                                            SelfScannerReportInfo[i].DTime = ADataRow.DTime;
                                            SelfScannerReportInfo[i].UserType = ADataRow.UserType;
                                            SelfScannerReportInfo[i].TMSI = ADataRow.TMSI;
                                            SelfScannerReportInfo[i].IMEI = ADataRow.IMEI;
                                            SelfScannerReportInfo[i].Intensity = ADataRow.Intensity;
                                            SelfScannerReportInfo[i].Operators = ADataRow.Operators;
                                            SelfScannerReportInfo[i].DomainName = ADataRow.DomainName;
                                            SelfScannerReportInfo[i].DeviceName = ADataRow.DeviceName;
                                            SelfScannerReportInfo[i].Des = ADataRow.Des;
                                        }
                                        //添加
                                        else
                                        {
                                            SelfScannerReportInfo.Add(ADataRow);
                                        }
                                    }
                                    //为空时全部添加
                                    else if (OverrideCount == 0)
                                    {
                                        SelfScannerReportInfo.Add(ADataRow);
                                    }
                                }

                                ADataRow = null;
                            }

                            ScannerDataView.Rows.Clear();
                            DataCount = 0;
                            System.GC.Collect();
                        });
                    }
                    catch (Exception ex)
                    {
                        Parameters.PrintfLogsExtended("实时上报显示", ex.Message, ex.StackTrace);
                    }
                }

                //每隔指定时长刷新一次(单位：秒)(不要需改时间,时长由配置文件提供,未配或配置错误，默认10秒)
                if (Parameters.ScannerDataControlParameter.RefreshTime > 0)
                {
                    Thread.Sleep(Parameters.ScannerDataControlParameter.RefreshTime * 1000);
                }
                //默认10秒
                else
                {
                    Thread.Sleep(10 * 1000);
                }
            }
        }

        /// <summary>
        /// GSMV2/CDMA GSM 通话记录实时显示 暂定显示500条
        /// </summary>
        private void GSMPhoneNumberRecordInfo()
        {
            while (true)
            {
                Thread.Sleep(200);

                try
                {
                    if (JsonInterFace.GSMPhoneNumberSMSRecordInfo.PhoneNumberRecordDataTab.Rows.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DataTable PhoneNumberRecordDataTab = JsonInterFace.GSMPhoneNumberSMSRecordInfo.PhoneNumberRecordDataTab.Copy();

                            for (int j = 0; j < PhoneNumberRecordDataTab.Rows.Count; j++)
                            {
                                string ID = string.Empty;
                                GSMPhoneNumberSMSRecordInfoClass GSMPhoneNumberRecordInfo = new GSMPhoneNumberSMSRecordInfoClass();

                                if (PhoneNumberRecordList.Count <= 0)
                                {
                                    ID = "1";
                                }
                                else
                                {
                                    ID = (Convert.ToInt32(PhoneNumberRecordList[0].ID) + 1).ToString();
                                }
                                GSMPhoneNumberRecordInfo.ID = ID;
                                GSMPhoneNumberRecordInfo.DeviceFullPathName = PhoneNumberRecordDataTab.Rows[j]["DeviceFullPathName"].ToString();
                                GSMPhoneNumberRecordInfo.BOrmType = PhoneNumberRecordDataTab.Rows[j]["BOrmType"].ToString();
                                GSMPhoneNumberRecordInfo.BUeId = PhoneNumberRecordDataTab.Rows[j]["BUeId"].ToString();
                                GSMPhoneNumberRecordInfo.CRSRP = PhoneNumberRecordDataTab.Rows[j]["CRSRP"].ToString();
                                GSMPhoneNumberRecordInfo.BUeContentLen = PhoneNumberRecordDataTab.Rows[j]["BUeContentLen"].ToString();
                                GSMPhoneNumberRecordInfo.BUeContent = PhoneNumberRecordDataTab.Rows[j]["BUeContent"].ToString();
                                GSMPhoneNumberRecordInfo.Carrier = PhoneNumberRecordDataTab.Rows[j]["Carrier"].ToString();
                                GSMPhoneNumberRecordInfo.DataTime = PhoneNumberRecordDataTab.Rows[j]["DataTime"].ToString();

                                //显示
                                if (PhoneNumberRecordList.Count < 500)
                                {
                                    PhoneNumberRecordList.Insert(0, GSMPhoneNumberRecordInfo);
                                }
                                else
                                {
                                    PhoneNumberRecordList.RemoveAt(PhoneNumberRecordList.Count - 1);
                                    PhoneNumberRecordList.Insert(0, GSMPhoneNumberRecordInfo);
                                }

                                //清缓存
                                for (int k = 0; k < JsonInterFace.GSMPhoneNumberSMSRecordInfo.PhoneNumberRecordDataTab.Rows.Count; k++)
                                {
                                    if (JsonInterFace.GSMPhoneNumberSMSRecordInfo.PhoneNumberRecordDataTab.Rows[k]["ID"].ToString() == PhoneNumberRecordDataTab.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.GSMPhoneNumberSMSRecordInfo.PhoneNumberRecordDataTab.Rows.RemoveAt(k);
                                        break;
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("通话记录实时显示", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// GSMV2/CDMA GSM 短信记录实时显示 暂定显示500条
        /// </summary>
        private void GSMSMSRecordInfo()
        {
            while (true)
            {
                Thread.Sleep(200);

                try
                {
                    if (JsonInterFace.GSMPhoneNumberSMSRecordInfo.SMSRecordDataTab.Rows.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            DataTable SMSRecordDataTab = JsonInterFace.GSMPhoneNumberSMSRecordInfo.SMSRecordDataTab.Copy();

                            for (int j = 0; j < SMSRecordDataTab.Rows.Count; j++)
                            {
                                string ID = string.Empty;
                                GSMPhoneNumberSMSRecordInfoClass GSMSMSRecordInfo = new GSMPhoneNumberSMSRecordInfoClass();

                                if (SMSRecordList.Count <= 0)
                                {
                                    ID = "1";
                                }
                                else
                                {
                                    ID = (Convert.ToInt32(SMSRecordList[0].ID) + 1).ToString();
                                }
                                GSMSMSRecordInfo.ID = ID;
                                GSMSMSRecordInfo.DeviceFullPathName = SMSRecordDataTab.Rows[j]["DeviceFullPathName"].ToString();
                                GSMSMSRecordInfo.BOrmType = SMSRecordDataTab.Rows[j]["BOrmType"].ToString();
                                GSMSMSRecordInfo.BUeId = SMSRecordDataTab.Rows[j]["BUeId"].ToString();
                                GSMSMSRecordInfo.CRSRP = SMSRecordDataTab.Rows[j]["CRSRP"].ToString();
                                GSMSMSRecordInfo.BUeContentLen = SMSRecordDataTab.Rows[j]["BUeContentLen"].ToString();
                                GSMSMSRecordInfo.BUeContent = SMSRecordDataTab.Rows[j]["BUeContent"].ToString();
                                GSMSMSRecordInfo.DesPhoneNumber = SMSRecordDataTab.Rows[j]["DesPhoneNumber"].ToString();
                                GSMSMSRecordInfo.Carrier = SMSRecordDataTab.Rows[j]["Carrier"].ToString();
                                GSMSMSRecordInfo.DataTime = SMSRecordDataTab.Rows[j]["DataTime"].ToString();

                                //显示
                                if (SMSRecordList.Count < 500)
                                {
                                    SMSRecordList.Insert(0, GSMSMSRecordInfo);
                                }
                                else
                                {
                                    SMSRecordList.RemoveAt(SMSRecordList.Count - 1);
                                    SMSRecordList.Insert(0, GSMSMSRecordInfo);
                                }

                                //清缓存
                                for (int k = 0; k < JsonInterFace.GSMPhoneNumberSMSRecordInfo.SMSRecordDataTab.Rows.Count; k++)
                                {
                                    if (JsonInterFace.GSMPhoneNumberSMSRecordInfo.SMSRecordDataTab.Rows[k]["ID"].ToString() == SMSRecordDataTab.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.GSMPhoneNumberSMSRecordInfo.SMSRecordDataTab.Rows.RemoveAt(k);
                                        break;
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("短信记录实时显示", ex.Message, ex.StackTrace);
                }
            }
        }

        private void AddFullNameList()
        {
            while (true)
            {
                try
                {
                    if (JsonInterFace.APATTributesLists.Count > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (FullNameList.Count < 1)
                            {
                                FullNameList.Add("所有设备");
                            }
                            for (int i = 0; i < JsonInterFace.APATTributesLists.Count; i++)
                            {
                                if (JsonInterFace.APATTributesLists[i] != null)
                                {
                                    bool Flag = true;
                                    if (JsonInterFace.APATTributesLists[i].Mode.Replace("_", "-").Equals(DeviceType.CDMA) ||
                                        JsonInterFace.APATTributesLists[i].Mode.Replace("_", "-").Equals(DeviceType.GSMV2) ||
                                        JsonInterFace.APATTributesLists[i].Mode.Replace("_", "-").Equals(DeviceType.GSM))
                                    {
                                        for (int j = 0; j < FullNameList.Count; j++)
                                        {
                                            if (JsonInterFace.APATTributesLists[i].FullName == FullNameList[j])
                                            {
                                                Flag = false;
                                                break;
                                            }
                                        }
                                        if (Flag)
                                        {
                                            FullNameList.Add(JsonInterFace.APATTributesLists[i].FullName);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("获取设备全名失败", ex.Message, ex.StackTrace);
                }

                Thread.Sleep(3000);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //应用语言
            try
            {
                ScannerFillterArea.DataContext = LanguageClassScannerInfoFillterSymbol;
                txtBlockID.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockIMSI.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockDTime.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockUserType.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockTMSI.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockIMEI.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockIntensity.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockOperators.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockDistrict.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockDeviceName.DataContext = LanguageClassScannerInfoDataGridSymbol;
                txtBlockAliasName.DataContext = LanguageClassScannerInfoDataGridSymbol;

                MeasurementSearchBar.DataContext = LanguageClassBlackInfoFillterSymbol;
                MeasurementDrawingBox.DataContext = LanguageClassBlackInfoFillterSymbol;
                CallRecordsBox.DataContext = LanguageClassBlackInfoFillterSymbol;
                SMSRecordsBox.DataContext = LanguageClassBlackInfoFillterSymbol;

                TabControlBox.DataContext = Parameters.DataReportWindowsControl;
                tabBlackReportDataView.DataContext = Parameters.DataReportWindowsControl;
                tabBlackReportDrawView.DataContext = Parameters.DataReportWindowsControl;
                tabPhoneNumberRecord.DataContext = Parameters.DataReportWindowsControl;
                tabSMSRecord.DataContext = Parameters.DataReportWindowsControl;

                txtBlockBIMSI.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBDTime.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBTMSI.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBIMEI.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBIntensity.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBOperators.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBDistrict.DataContext = LanguageClassBlackInfoDataGridSymbol;
                txtBlockBDeviceName.DataContext = LanguageClassBlackInfoDataGridSymbol;

                MeasurementReportChartFram.Navigate(SubWindows["MeasurementReportChartWindow"]);

                //捕号过滤默认时间
                dpkcmdStartDate.Text = ScannerDataConditionParameter.StartTime;
                txtStartHour.Text = ScannerDataConditionParameter.StartHour;
                txtStartMinute.Text = ScannerDataConditionParameter.StartMinute;
                txtStartSecond.Text = ScannerDataConditionParameter.StartSecond;
                dpkcmdEndDate.Text = ScannerDataConditionParameter.EndTime;
                txtEndHour.Text = ScannerDataConditionParameter.EndHour;
                txtEndMinute.Text = ScannerDataConditionParameter.EndMinute;
                txtEndSecond.Text = ScannerDataConditionParameter.EndSecond;

                //黑名单追踪过滤默认时间
                dpkBLcmdStartDate.Text = BlackNameConditionParameter.StartTime;
                txtBLStartHour.Text = BlackNameConditionParameter.StartHour;
                txtBLStartMinute.Text = BlackNameConditionParameter.StartMinute;
                txtBLStartSecond.Text = BlackNameConditionParameter.StartSecond;
                dpkBLcmdEndDate.Text = BlackNameConditionParameter.EndTime;
                txtBLEndHour.Text = BlackNameConditionParameter.EndHour;
                txtBLEndMinute.Text = BlackNameConditionParameter.EndMinute;
                txtBLEndSecond.Text = BlackNameConditionParameter.EndSecond;

                ScannerInfoDataGrid.ItemsSource = SelfScannerReportInfo;
                BlackListInfoDataGrid.ItemsSource = SelfMeasReportBlackList;
                mmScannerDataStopRefresh.Tag = "0";

                //图表信息
                txtIMSIName.DataContext = MeasurementReportChartWindow.SelfChart;
                txtDateTime.DataContext = MeasurementReportChartWindow.SelfChart;
                ChartScrollViewer.DataContext = MeasurementReportChartWindow.SelfChart;
                ChartIMSIList.ItemsSource = ChartIMSIDataList;

                //通话记录
                dgPhoneRecordInfoTable.ItemsSource = PhoneNumberRecordList;
                //短信记录
                dgSMSInfoTable.ItemsSource = SMSRecordList;
                //设备名称
                cbFullName.ItemsSource = FullNameList;
                cbFullNamePhoneRecord.ItemsSource = FullNameList;
                //状态
                tabBlackReportDrawView.Tag = 0;

                //窗口控制
                ScannerWindow.DataContext = Parameters.DataReportWindowsControl;
                ScannerWindowLine.DataContext = Parameters.DataReportWindowsControl;
                FunctionWindowsArea.DataContext = Parameters.DataReportWindowsControl;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("UE界面初始化", ex.Message, ex.StackTrace);
            }
        }

        private void mmMeasReportRefresh_Click(object sender, RoutedEventArgs e)
        {
            BlackListInfoDataGrid.ItemsSource = null;
            BlackListInfoDataGrid.Items.Refresh();
            BlackListInfoDataGrid.ItemsSource = SelfMeasReportBlackList;
        }

        //捕号数据过滤查询
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ScannerDataConditionParameter.IMSI = txtIMSI.Text.Trim();
            ScannerDataConditionParameter.DeviceName = txtDeviceName.Text.Trim();
            if (cbbUserType.Text.Trim() == "全部" || cbbUserType.Text.Trim() == "All")
            {
                ScannerDataConditionParameter.UserType = "";
            }
            else
            {
                ScannerDataConditionParameter.UserType = cbbUserType.Text.Trim();
            }

            if (dpkcmdStartDate.SelectedDate != null)
            {
                ScannerDataConditionParameter.StartTime = ((DateTime)dpkcmdStartDate.SelectedDate).ToShortDateString();
            }
            else
            {
                ScannerDataConditionParameter.StartTime = "";
            }

            if (dpkcmdEndDate.SelectedDate != null)
            {
                ScannerDataConditionParameter.EndTime = ((DateTime)dpkcmdEndDate.SelectedDate).ToShortDateString();
            }
            else
            {
                ScannerDataConditionParameter.EndTime = "";
            }

            ScannerDataConditionParameter.StartHour = txtStartHour.Text;
            ScannerDataConditionParameter.StartMinute = txtStartMinute.Text;
            ScannerDataConditionParameter.StartSecond = txtStartSecond.Text;

            ScannerDataConditionParameter.EndHour = txtEndHour.Text;
            ScannerDataConditionParameter.EndMinute = txtEndMinute.Text;
            ScannerDataConditionParameter.EndSecond = txtEndSecond.Text;

            SelfScannerReportInfo.Clear();
        }

        //捕号数据过滤显示
        private void ScannerDataFilter(ref DataTable ScannerData, DataTable ScannerDataCaching, string IMSI, string DeviceName, string TimeStart, string TimeEnd, string UserType)
        {
            string FilterStr = string.Empty;
            Dictionary<string, string> ParamList = new Dictionary<string, string>();
            try
            {
                ParamList.Add("IMSI", IMSI);
                ParamList.Add("DeviceName", DeviceName);
                ParamList.Add("UserType", UserType);
                ParamList.Add("DTime", TimeStart);

                foreach (KeyValuePair<string, string> Item in ParamList)
                {
                    if (Item.Key == "IMSI")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "DeviceName")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "UserType")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "DTime")
                    {
                        if (Item.Value != "" && Item.Value != null && TimeEnd != "" && TimeEnd != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                string[] _StartDateTime = Item.Value.Split(new char[] { ' ' });
                                string[] _EndDateTime = TimeEnd.Split(new char[] { ' ' });
                                if (_StartDateTime.Length >= 2 && _EndDateTime.Length >= 2)
                                {
                                    if (Parameters.IsDate(_StartDateTime[0]) && Parameters.IsTime(_StartDateTime[1]) && Parameters.IsDate(_EndDateTime[0]) && Parameters.IsTime(_EndDateTime[1]))
                                    {
                                        FilterStr = string.Format("{0}>='{1}' AND {2}<='{3}'", Item.Key, Item.Value, Item.Key, TimeEnd);
                                    }
                                }
                            }
                            else
                            {
                                string[] _StartDateTime = Item.Value.Split(new char[] { ' ' });
                                string[] _EndDateTime = TimeEnd.Split(new char[] { ' ' });
                                if (_StartDateTime.Length >= 2 && _EndDateTime.Length >= 2)
                                {
                                    if (Parameters.IsDate(_StartDateTime[0]) && Parameters.IsTime(_StartDateTime[1]) && Parameters.IsDate(_EndDateTime[0]) && Parameters.IsTime(_EndDateTime[1]))
                                    {
                                        FilterStr = FilterStr + " AND " + string.Format("{0}>='{1}' AND {2}<='{3}'", Item.Key, Item.Value, Item.Key, TimeEnd);
                                    }
                                }
                            }
                        }
                    }
                }

                DataRow[] ScannerDataRow = ScannerDataCaching.Select(FilterStr);

                foreach (DataRow Item in ScannerDataRow)
                {
                    DataRow dr = ScannerData.NewRow();
                    dr[0] = Item[0].ToString();
                    dr[1] = Item[1].ToString();
                    dr[2] = Item[2].ToString();
                    dr[3] = Item[3].ToString();
                    dr[4] = Item[4].ToString();
                    dr[5] = Item[5].ToString();
                    dr[6] = Item[6].ToString();
                    dr[7] = Item[7].ToString();
                    dr[8] = Item[8].ToString();
                    dr[9] = Item[9].ToString();
                    dr[10] = Item[10].ToString();
                    ScannerData.Rows.Add(dr);
                }

                ParamList.Clear();
                ParamList = null;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("捕号数据过滤", ex.Message, ex.StackTrace);
            }
        }

        //黑名单追踪过滤查询
        private void btnBLSearch_Click(object sender, RoutedEventArgs e)
        {
            BlackNameConditionParameter.IMSI = txtBLIMSI.Text.Trim();
            BlackNameConditionParameter.DeviceName = txtBLDeviceName.Text.Trim();
            if (dpkBLcmdStartDate.SelectedDate != null)
            {
                BlackNameConditionParameter.StartTime = ((DateTime)dpkBLcmdStartDate.SelectedDate).ToShortDateString();
            }
            else
            {
                BlackNameConditionParameter.StartTime = "";
            }

            if (dpkBLcmdEndDate.SelectedDate != null)
            {
                BlackNameConditionParameter.EndTime = ((DateTime)dpkBLcmdEndDate.SelectedDate).ToShortDateString();
            }
            else
            {
                BlackNameConditionParameter.EndTime = "";
            }

            BlackNameConditionParameter.StartHour = txtBLStartHour.Text;
            BlackNameConditionParameter.StartMinute = txtBLStartMinute.Text;
            BlackNameConditionParameter.StartSecond = txtBLStartSecond.Text;

            BlackNameConditionParameter.EndHour = txtBLEndHour.Text;
            BlackNameConditionParameter.EndMinute = txtBLEndMinute.Text;
            BlackNameConditionParameter.EndSecond = txtBLEndSecond.Text;

            BlackNameConditionParameter.Callerloc = txtBLCallerloc.Text.Trim();
            SelfMeasReportBlackList.Clear();
        }

        //黑名单追踪过滤显示
        private void MeasureReportDataFilter(ref DataTable BlackListData, DataTable BlackListDataCaching, string IMSI, string DeviceName, string TimeStart, string TimeEnd, string District)
        {
            string FilterStr = string.Empty;
            Dictionary<string, string> ParamList = new Dictionary<string, string>();
            try
            {
                ParamList.Add("IMSI", IMSI);
                ParamList.Add("DeviceName", DeviceName);
                ParamList.Add("DomainName", District);
                ParamList.Add("DTime", TimeStart);

                foreach (KeyValuePair<string, string> Item in ParamList)
                {
                    if (Item.Key == "IMSI")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "DeviceName")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "DomainName")
                    {
                        if (Item.Value != "" && Item.Value != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                FilterStr = string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                            else
                            {
                                FilterStr = FilterStr + " AND " + string.Format("{0} like '%{1}%'", Item.Key, Item.Value);
                            }
                        }
                    }
                    else if (Item.Key == "DTime")
                    {
                        if (Item.Value != "" && Item.Value != null && TimeEnd != "" && TimeEnd != null)
                        {
                            if (FilterStr == "" || FilterStr == null)
                            {
                                string[] _StartDateTime = Item.Value.Split(new char[] { ' ' });
                                string[] _EndDateTime = TimeEnd.Split(new char[] { ' ' });
                                if (_StartDateTime.Length >= 2 && _EndDateTime.Length >= 2)
                                {
                                    if (Parameters.IsDate(_StartDateTime[0]) && Parameters.IsTime(_StartDateTime[1]) && Parameters.IsDate(_EndDateTime[0]) && Parameters.IsTime(_EndDateTime[1]))
                                    {
                                        FilterStr = string.Format("{0}>='{1}' AND {2}<='{3}'", Item.Key, Item.Value, Item.Key, TimeEnd);
                                    }
                                }
                            }
                            else
                            {
                                string[] _StartDateTime = Item.Value.Split(new char[] { ' ' });
                                string[] _EndDateTime = TimeEnd.Split(new char[] { ' ' });
                                if (_StartDateTime.Length >= 2 && _EndDateTime.Length >= 2)
                                {
                                    if (Parameters.IsDate(_StartDateTime[0]) && Parameters.IsTime(_StartDateTime[1]) && Parameters.IsDate(_EndDateTime[0]) && Parameters.IsTime(_EndDateTime[1]))
                                    {
                                        FilterStr = FilterStr + " AND " + string.Format("{0}>='{1}' AND {2}<='{3}'", Item.Key, Item.Value, Item.Key, TimeEnd);
                                    }
                                }
                            }
                        }
                    }
                }

                DataRow[] BlackListDataRow = BlackListDataCaching.Select(FilterStr);

                foreach (DataRow Item in BlackListDataRow)
                {
                    DataRow dr = BlackListData.NewRow();
                    dr.BeginEdit();
                    dr[0] = Item[0].ToString();
                    dr[1] = Item[1].ToString();
                    dr[2] = Item[2].ToString();
                    dr[3] = Item[3].ToString();
                    dr[4] = Item[4].ToString();
                    dr[5] = Item[5].ToString();
                    dr[6] = Item[6].ToString();
                    dr[7] = Item[7].ToString();
                    BlackListData.Rows.Add(dr);
                    dr.EndEdit();
                }

                ParamList.Clear();
                ParamList = null;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("黑名单追踪数据过滤", ex.Message, ex.StackTrace);
            }
        }

        private void btnSearch_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSearch.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("DodgerBlue"));
            btnSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Orange"));
        }

        private void btnSearch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSearch.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            btnSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
        }

        private void btnBLSearch_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnBLSearch.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("DodgerBlue"));
            btnBLSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Orange"));
        }

        private void btnBLSearch_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            btnSearch.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Transparent"));
            btnSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
        }

        private void ScannerInfoDataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ScannerInfoDataGrid.Items.Count > 1)
            {
                ScannerInfoDataGrid.SelectedItem = ScannerInfoDataGrid.Items[ScannerInfoDataGrid.Items.Count - 1];
                ScannerInfoDataGrid.CurrentColumn = ScannerInfoDataGrid.Columns[0];
                ScannerInfoDataGrid.ScrollIntoView(ScannerInfoDataGrid.SelectedItem, ScannerInfoDataGrid.CurrentColumn);
            }
        }

        private void mmScannerDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            ScannerInfoDataGrid.ItemsSource = null;
            ScannerInfoDataGrid.Items.Refresh();
            ScannerInfoDataGrid.ItemsSource = SelfScannerReportInfo;
        }

        private void mmScannerDataClean_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.ScannerData.ScannerDataTable.Rows.Clear();
            SelfScannerReportInfo.Clear();
            ScannerInfoDataGrid.ItemsSource = null;
            ScannerInfoDataGrid.Items.Clear();
            ScannerInfoDataGrid.Items.Refresh();
            ScannerInfoDataGrid.ItemsSource = SelfScannerReportInfo;
        }

        private void mmScannerDataCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ScannerInfoDataGrid.CurrentColumn != null)
                {
                    int ColumeIndex = ScannerInfoDataGrid.CurrentColumn.DisplayIndex;
                    int RowIndex = ScannerInfoDataGrid.SelectedIndex;
                    string Value = JsonInterFace.ScannerData.ScannerDataTable.Rows[RowIndex][ColumeIndex].ToString();
                    Clipboard.SetText(Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "复制信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void mmScannerDataExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
                svd.Filter = "(Excel文件 *.xls)|*.xls|(Excel文件 *.xlsx)|*.xlsx";
                svd.AddExtension = true;
                svd.CheckPathExists = true;

                if (!(bool)svd.ShowDialog())
                {
                    return;
                }

                using (ExcelHelper excelHelper = new ExcelHelper(@svd.FileName))//定义一个范围，在范围结束时处理对象
                {
                    DataTable exportDr = JsonInterFace.ScannerData.ScannerDataTable.Clone();
                    exportDr.Rows.Clear();

                    for (int i = 0; i < SelfScannerReportInfo.Count; i++)
                    {
                        DataRow dr = exportDr.NewRow();
                        dr[0] = SelfScannerReportInfo[i].ID;
                        dr[1] = SelfScannerReportInfo[i].IMSI;
                        dr[2] = SelfScannerReportInfo[i].DTime;
                        dr[3] = SelfScannerReportInfo[i].UserType;
                        dr[4] = SelfScannerReportInfo[i].TMSI;
                        dr[5] = SelfScannerReportInfo[i].IMEI;
                        dr[6] = SelfScannerReportInfo[i].Intensity;
                        dr[7] = SelfScannerReportInfo[i].Operators;
                        dr[8] = SelfScannerReportInfo[i].DomainName;
                        dr[9] = SelfScannerReportInfo[i].DeviceName;
                        dr[10] = SelfScannerReportInfo[i].Des;

                        exportDr.Rows.Add(dr);
                    }

                    int res = excelHelper.DataTableToExcelForScannerData(exportDr, "Sheet1", true);

                    if (res != -1)
                    {
                        MessageBox.Show("导出到Excel文件成功,共[" + res.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出到Excel文件失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ScannerInfoDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (ScannerInfoDataGrid.Items.Count > 0)
            {
                int RowIndex = ScannerInfoDataGrid.SelectedIndex;
                if (RowIndex >= 0)
                {
                    SelectScannerDataInfo.IMSI = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).IMSI;
                    SelectScannerDataInfo.DTime = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).DTime;
                    SelectScannerDataInfo.UserType = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).UserType;
                    SelectScannerDataInfo.TMSI = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).TMSI;
                    SelectScannerDataInfo.IMEI = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).IMEI;
                    SelectScannerDataInfo.Intensity = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).Intensity;
                    SelectScannerDataInfo.Operators = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).Operators;
                    SelectScannerDataInfo.DomainName = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).DomainName;
                    SelectScannerDataInfo.DeviceName = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).DeviceName;
                    SelectScannerDataInfo.Des = (ScannerInfoDataGrid.SelectedItem as ScannerDataClass).Des;
                }
            }
        }

        private void ScannerInfoDataGrid_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelfScannerReportInfo.Count > 0)
            {
                if (Parameters.FindWindow(null, "Scanner详细信息") == 0)
                {
                    SubWindow.ShowSelectScannerDataDialog ShowSelectScannerDataWin = new SubWindow.ShowSelectScannerDataDialog();
                    ShowSelectScannerDataWin.Show();
                }
            }
        }

        /// <summary>
        /// 过虑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSMSSelectData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmHistoryDataRefresh_Click(object sender, RoutedEventArgs e)
        {
            PhoneNumberRecordList.Clear();
            SMSRecordList.Clear();
        }

        private void btnPhoneRecordInfoFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void gdBlockID_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockID.Source.ToString(), ref ImgBlockID);
        }

        //点击排序图标标示
        private void ColumnOderChanged(string ImgSource, ref Image SelfIcon)
        {
            if (ImgSource == new ColumnOderImage().ASC)
            {
                SelfIcon.Source = new BitmapImage(new Uri(new ColumnOderImage().DESC, UriKind.RelativeOrAbsolute));
            }
            else
            {
                SelfIcon.Source = new BitmapImage(new Uri(new ColumnOderImage().ASC, UriKind.RelativeOrAbsolute));
            }
        }

        private void gdBlockIMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockIMSI.Source.ToString(), ref ImgBlockIMSI);
        }

        private void gdBlockDTime_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockDTime.Source.ToString(), ref ImgBlockDTime);
        }

        private void gdBlockUserType_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockUserType.Source.ToString(), ref ImgBlockUserType);
        }

        private void gdBlockTMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockTMSI.Source.ToString(), ref ImgBlockTMSI);
        }

        private void gdBlockIMEI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockIMEI.Source.ToString(), ref ImgBlockIMEI);
        }

        private void gdBlockIntensity_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockIntensity.Source.ToString(), ref ImgBlockIntensity);
        }

        private void gdBlockOperators_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockOperators.Source.ToString(), ref ImgBlockOperators);
        }

        private void gdBlockDistrict_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockDistrict.Source.ToString(), ref ImgBlockDistrict);
        }

        private void gdBlockDeviceName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockDeviceName.Source.ToString(), ref ImgBlockDeviceName);
        }

        private void gdBlockAliasName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockAliasName.Source.ToString(), ref ImgBlockAliasName);
        }

        private void gdBlockBIMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBIMSI.Source.ToString(), ref ImgBlockBIMSI);
        }

        private void gdBlockBDTime_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBDTime.Source.ToString(), ref ImgBlockBDTime);
        }

        private void gdBlockBTMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBTMSI.Source.ToString(), ref ImgBlockBTMSI);
        }

        private void gdBlockBIMEI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBIMEI.Source.ToString(), ref ImgBlockBIMEI);
        }

        private void gdBlockBIntensity_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBIntensity.Source.ToString(), ref ImgBlockBIntensity);
        }

        private void gdBlockBOperators_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBOperators.Source.ToString(), ref ImgBlockBOperators);
        }

        private void gdBlockBDistrict_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBDistrict.Source.ToString(), ref ImgBlockBDistrict);
        }

        private void gdBlockBDeviceName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockBDeviceName.Source.ToString(), ref ImgBlockBDeviceName);
        }

        private void gdGSMV2PhoneRecordIMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordIMSI.Source.ToString(), ref ImgGSMV2PhoneRecordIMSI);
        }

        private void gdGSMV2SPhoneRecordFullName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SPhoneRecordFullName.Source.ToString(), ref ImgGSMV2SPhoneRecordFullName);
        }

        private void gdGSMV2PhoneRecordCarrier_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordCarrier.Source.ToString(), ref ImgGSMV2PhoneRecordCarrier);
        }

        private void gdGSMV2PhoneRecordBUeId_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordBUeId.Source.ToString(), ref ImgGSMV2PhoneRecordBUeId);
        }

        private void gdGSMV2PhoneRecordDesPhoneNumber_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordDesPhoneNumber.Source.ToString(), ref ImgGSMV2PhoneRecordDesPhoneNumber);
        }

        private void gdGSMV2PhoneRecordBOrmType_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordBOrmType.Source.ToString(), ref ImgGSMV2PhoneRecordBOrmType);
        }

        private void gdGSMV2PhoneRecordCRSRP_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordCRSRP.Source.ToString(), ref ImgGSMV2PhoneRecordCRSRP);
        }

        private void gdGSMV2PhoneRecordDataTime_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2PhoneRecordDataTime.Source.ToString(), ref ImgGSMV2PhoneRecordDataTime);
        }

        private void gdGSMV2SMSRecordIMSI_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ColumnOderChanged(ImgGSMV2SMSRecordIMSI.Source.ToString(), ref ImgGSMV2SMSRecordIMSI);
        }

        private void gdGSMV2SMSRecordFullName_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ColumnOderChanged(ImgGSMV2SMSRecordFullName.Source.ToString(), ref ImgGSMV2SMSRecordFullName);
        }

        private void gdGSMV2SMSRecordCarrier_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordCarrier.Source.ToString(), ref ImgGSMV2SMSRecordCarrier);
        }

        private void gdGSMV2SMSRecordBUeId_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordBUeId.Source.ToString(), ref ImgGSMV2SMSRecordBUeId);
        }

        private void gdGSMV2SMSRecordMessage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordMessage.Source.ToString(), ref ImgGSMV2SMSRecordMessage);
        }

        private void gdGSMV2SMSRecordDesPhoneNumber_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordDesPhoneNumber.Source.ToString(), ref ImgGSMV2SMSRecordDesPhoneNumber);
        }

        private void gdGSMV2SMSRecordBOrmType_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordBOrmType.Source.ToString(), ref ImgGSMV2SMSRecordBOrmType);
        }

        private void gdGSMV2SMSRecordCRSRP_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordCRSRP.Source.ToString(), ref ImgGSMV2SMSRecordCRSRP);
        }

        private void gdGSMV2SMSRecordDataTime_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgGSMV2SMSRecordDataTime.Source.ToString(), ref ImgGSMV2SMSRecordDataTime);
        }

        private void ChartScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MeasurementReportChartWindow.SelfChart.ChartWindowWidth = ChartScrollViewer.ActualWidth;
            MeasurementReportChartWindow.SelfChart.ChartWindowHeight = ChartScrollViewer.ActualHeight;
            MeasurementReportChartWindow.ReloadChartBaseTimer.Start();
        }

        private void ChartIMSIList_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabControlBox.SelectedIndex.Equals(1) && Convert.ToInt32(tabBlackReportDrawView.Tag) == 0)
                {
                    if (ChartIMSIList.Items.Count > 0)
                    {
                        ChartIMSIList.SelectedIndex = 0;
                        MeasurementReportChartWindow.SelfChart.IMSI = (ChartIMSIList.SelectedItem as MeasReportBlackListClass).IMSI;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("监视黑名单追踪曲线图", ex.Message, ex.StackTrace);
            }
        }

        private void ChartIMSIList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ChartIMSIList.SelectedItem != null && ChartIMSIList.Items.Count > 0)
                {
                    if ((ChartIMSIList.SelectedItem as MeasReportBlackListClass).IMSI != MeasurementReportChartWindow.SelfChart.IMSI)
                    {
                        if (MeasurementReportChartWindow.SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.Running
                            || MeasurementReportChartWindow.SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.WaitSleepJoin)
                        {
                            MeasurementReportChartWindow.SelfChart.DrawingMeasureReportAxialThread.Suspend();
                            MeasurementReportChartWindow.SelfChart.IMSI = (ChartIMSIList.SelectedItem as MeasReportBlackListClass).IMSI;
                            MeasurementReportChartWindow.SelfChart.SelfChart.Children.Clear();
                            MeasurementReportChartWindow.SelfChart.Second = 0;
                            MeasurementReportChartWindow.SelfChart.MoveLineControl = 0;
                            MeasurementReportChartWindow.SelfChart.DrawingMeasureReportAxialThread.Resume();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("切换黑名单[IMSI]", ex.Message, ex.StackTrace);
            }
        }

        private void BlackListInfoDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (BlackListInfoDataGrid.Items.Count > 0)
            {
                int RowIndex = BlackListInfoDataGrid.SelectedIndex;
                if (RowIndex >= 0)
                {
                    MeasurementReportChartWindow.SelfChart.IMSI = (BlackListInfoDataGrid.SelectedItem as MeasReportBlackListClass).IMSI;
                }
            }
        }

        private void BlackListInfoDataGrid_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                TabControlBox.SelectedIndex = 1;
                tabBlackReportDrawView.Tag = 1;
                for (int i = 0; i < ChartIMSIList.Items.Count; i++)
                {
                    if ((ChartIMSIList.Items[i] as MeasReportBlackListClass).IMSI == MeasurementReportChartWindow.SelfChart.IMSI)
                    {
                        ChartIMSIList.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("切换到曲线图", ex.Message, ex.StackTrace);
            }
        }

        private void TabControlBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlBox.SelectedIndex != 1)
            {
                tabBlackReportDrawView.Tag = 0;
            }
        }

        private void txtIMSI_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.IMSI = txtIMSI.Text.Trim();
            SelfScannerReportInfo.Clear();
        }

        private void txtDeviceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.DeviceName = txtDeviceName.Text.Trim();
            SelfScannerReportInfo.Clear();
        }

        private void cbbUerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string UserType = (e.AddedItems[0] as ComboBoxItem).Content.ToString();
                if (UserType == "全部" || UserType == "All")
                {
                    ScannerDataConditionParameter.UserType = "";
                }
                else
                {
                    ScannerDataConditionParameter.UserType = UserType;
                }

                SelfScannerReportInfo.Clear();
            }
        }

        private void dpkcmdStartTime_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpkcmdStartDate.SelectedDate != null)
            {
                ScannerDataConditionParameter.StartTime = ((DateTime)dpkcmdStartDate.SelectedDate).ToShortDateString();
            }
            else
            {
                ScannerDataConditionParameter.StartTime = "";
            }

            SelfScannerReportInfo.Clear();
        }

        private void dpkcmdEndTime_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpkcmdEndDate.SelectedDate != null)
            {
                ScannerDataConditionParameter.EndTime = ((DateTime)dpkcmdEndDate.SelectedDate).ToShortDateString();
            }
            else
            {
                ScannerDataConditionParameter.EndTime = "";
            }

            SelfScannerReportInfo.Clear();
        }

        private void txtBLIMSI_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.IMSI = txtBLIMSI.Text.Trim();
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLDeviceName_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.DeviceName = txtBLDeviceName.Text.Trim();
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLDistrict_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.Callerloc = txtBLCallerloc.Text.Trim();
            SelfMeasReportBlackList.Clear();
        }

        private void dpkBLcmdStartTime_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpkBLcmdStartDate.SelectedDate != null)
            {
                BlackNameConditionParameter.StartTime = ((DateTime)dpkBLcmdStartDate.SelectedDate).ToShortDateString();
            }
            else
            {
                BlackNameConditionParameter.StartTime = "";
            }
            SelfMeasReportBlackList.Clear();
        }

        private void dpkBLcmdEndTime_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpkBLcmdEndDate.SelectedDate != null)
            {
                BlackNameConditionParameter.EndTime = ((DateTime)dpkBLcmdEndDate.SelectedDate).ToShortDateString();
            }
            else
            {
                BlackNameConditionParameter.EndTime = "";
            }
            SelfMeasReportBlackList.Clear();
        }

        private void txtStartHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.StartHour = txtStartHour.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtStartMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.StartMinute = txtStartMinute.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtStartSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.StartSecond = txtStartSecond.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtEndHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.EndHour = txtEndHour.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtEndMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.EndMinute = txtEndMinute.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtEndSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScannerDataConditionParameter.EndSecond = txtEndSecond.Text;
            SelfScannerReportInfo.Clear();
        }

        private void txtBLStartHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.StartHour = txtBLStartHour.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLStartMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.StartMinute = txtBLStartMinute.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLStartSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.StartSecond = txtBLStartSecond.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLEndHour_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.EndHour = txtBLEndHour.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLEndMinute_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.EndMinute = txtBLEndMinute.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void txtBLEndSecond_TextChanged(object sender, TextChangedEventArgs e)
        {
            BlackNameConditionParameter.EndSecond = txtBLEndSecond.Text;
            SelfMeasReportBlackList.Clear();
        }

        private void mmScannerDataStopRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mmScannerDataStopRefresh.Tag.ToString() == "0")
                {
                    mmScannerDataStopRefresh.Tag = "1";
                    mmScannerDataStopRefresh.Header = "实时刷新";
                    mmScannerDataStopRefresh.Icon = new Image() { Source = new BitmapImage(new Uri(@"..\Icon\ok.png", UriKind.RelativeOrAbsolute)) };

                    NavigatePages.UEInfoWindow.ScannerReportRealTimeThread.Suspend();
                }
                else if (mmScannerDataStopRefresh.Tag.ToString() == "1")
                {
                    mmScannerDataStopRefresh.Tag = "0";
                    mmScannerDataStopRefresh.Header = "停止刷新";
                    mmScannerDataStopRefresh.Icon = new Image() { Source = new BitmapImage(new Uri(@"..\Icon\agt_resume.png", UriKind.RelativeOrAbsolute)) };

                    NavigatePages.UEInfoWindow.ScannerReportRealTimeThread.Resume();
                }
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(mmScannerDataStopRefresh.Header + "失败", Ex.Message, Ex.StackTrace);
            }
        }
    }
}
