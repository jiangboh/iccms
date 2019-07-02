using DataInterface;
using iccms.Arrows;
using iccms.NavigatePages;
using NetController;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Visifire.Charts;

namespace iccms.SubWindow
{
    #region 曲线图表类
    public class StatisticalAnalysisClass : INotifyPropertyChanged
    {
        //窗口大小
        private double _chartWindowHeight = 490;
        private double _chartWindowWidth = 858;

        //曲线图区
        private Canvas _selfChart = new Canvas();
        //曲线图锁
        private object _ChartLock = null;

        //区域
        private double _chartArea_Height = 490;
        private double _chartArea_Width = 858;

        //刻度零点
        private double _xOrig = 60;
        private double _yOrig = 50;

        //背景网格零点
        private double _xBgOrig = 0;
        private double _yBgOrig = 0;

        //背景色
        private string _chartBackGround = "#FF0C2C37";

        //刻度线
        public List<Line> GridLine = null;
        public List<Line> LongScaleLine = null;
        private string _gridLineColor = "Green";
        private string _longGridLineColor = "Green";
        private bool _gridEnable;
        private double _gridLineSize = 1;
        private double _longGridLineSize = 1;
        private double _xShortScaleLineSpace = 6;
        private double _yShortScaleLineSpace = 10;
        private double _xLongScaleLineSpace = 10;
        private double _yLongScaleLineSpace = 10;
        private double _gridLineOpacity = 1;
        private double _longGridLineOpacity = 1;
        private double _x_AxialScaleLineLen = 10;
        private double _y_AxialScaleLineLen = 10;
        private double _x_LongAxialScaleLineLen = 10;
        private double _y_LongAxialScaleLineLen = 10;

        //刻度值
        public List<Label> XAxialScaleValueList = null;
        public List<Label> YAxialScaleValueList = null;
        private double _xAxialScaleValueFontSize = 12;
        private double _xAxialScaleValueOpacity = 1;
        private string _xAxialScaleValueColor = "White";
        private double _yAxialScaleValueFontSize = 12;
        private double _yAxialScaleValueOpacity = 1;
        private string _yAxialScaleValueColor = "White";

        //背景网格线
        public List<Line> BackGroundGridLine = null;
        private string _backGroundGridLineColor = "#FF0B1F0B";
        private bool _backGroundGridEnable;
        private double _backGroundGridLineSize = 1;
        private double _backGroundGridLineSpace = 10;
        private double _backGroundGridLineOpacity = 1;

        //边线
        private Rectangle _outLine = null;
        private string _outLineColor = "Red";
        private double _outLineSize = 1;
        private double _outLineXStart = 0D;
        private double _outLineYStart = 0D;

        //坐标系
        public Line X_Quadrant = null;
        public Line Y_Quadrant = null;
        private string _x_QuadrantColor = "#FF1A8D16";
        private string _y_QuadrantColor = "#FF1A8D16";
        private double _X_QuadrantLineSize = 1;
        private double _Y_QuadrantLineSize = 1;
        private double _x_QuadrantAxialLineLength = 0;
        private double _y_QuadrantAxialLineLength = 0;
        //X,Y坐标起点
        private double _x_QuadrantAxialStart = 0;
        private double _y_QuadrantAxialStart = 0;
        //X,Y轴刻度数
        private int x_QuadrantScaleValueCount = 12;
        private int y_QuadrantScaleValueCount = 128;

        //箭头坐标系
        private Arrows.ArrowLine x_QuadrantArrow = null;
        private Arrows.ArrowLine y_QuadrantArrow = null;
        private string _x_QuadrantArrowColor = "Green";
        private string _y_QuadrantArrowColor = "Green";
        private double _X_QuadrantArrowLineSize = 1;
        private double _Y_QuadrantArrowLineSize = 1;
        private double _x_QuadrantArrowAxialLineLength = 0;
        private double _y_QuadrantArrowAxialLineLength = 0;
        private double _x_QuadrantArrowAxialLineLengthSetting = 10;
        private double _y_QuadrantArrowAxialLineLengthSetting = 10;
        //X,Y坐标起点
        private double _x_QuadrantArrowAxialStart = 0;
        private double _y_QuadrantArrowAxialStart = 0;
        //X,Y轴刻度数
        private int x_QuadrantArrowScaleValueCount = 12;
        private int y_QuadrantArrowScaleValueCount = 120;

        //结点
        private Ellipse _node = null;
        private string _nodeColor = "Red";
        private double _nodeSize = 5;
        private double _node_X = 0;
        private double _node_Y = 0;

        //时间轴
        private double _second = 0;

        //十六种不同颜色
        List<string> BlackListColors = null;

        //X轴单位
        private string _xUnit = "(H)";

        //是否去重
        private bool rmDupFlag = false;

        //线端点带圆点的类
        public class ChartDotLineParameterClass
        {
            private Line _aLine;
            private Ellipse _aDot;
            private DateTime _aDTime;

            public Line ALine
            {
                get
                {
                    return _aLine;
                }

                set
                {
                    _aLine = value;
                }
            }

            public Ellipse ADot
            {
                get
                {
                    return _aDot;
                }

                set
                {
                    _aDot = value;
                }
            }

            public DateTime ADTime
            {
                get
                {
                    return _aDTime;
                }

                set
                {
                    _aDTime = value;
                }
            }
        }

        //不同黑名单字典
        public Dictionary<string, List<ChartDotLineParameterClass>> MeasureReportLineList = null;

        //IMSI信息
        public DataTable MeasReportAxialChartBlackList = null;

        //IMSI数据获取
        public void Input(string ID, string IMSI, string RSRP)
        {
            try
            {
                DataRow dr = MeasReportAxialChartBlackList.NewRow();
                dr["ID"] = ID;
                dr["ImsiTotal"] = IMSI;
                dr["ImsiTotalRmDup"] = Convert.ToInt32(RSRP);
                MeasReportAxialChartBlackList.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //删除不存在的IMSI
        public void Remove(string IMSI)
        {
            try
            {
                for (int i = 0; i < MeasReportAxialChartBlackList.Rows.Count; i++)
                {
                    if (MeasReportAxialChartBlackList.Rows[i]["IMSI"].ToString() == IMSI)
                    {
                        MeasReportAxialChartBlackList.Rows.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("删除不存在的IMSI号", ex.Message, ex.StackTrace);
            }
        }

        //更新IMSI的信号值
        public void Update(string IMSI, string RSRP)
        {
            try
            {
                for (int i = 0; i < MeasReportAxialChartBlackList.Rows.Count; i++)
                {
                    if (MeasReportAxialChartBlackList.Rows[i]["IMSI"].ToString() == IMSI)
                    {
                        MeasReportAxialChartBlackList.Rows[i]["RSRP"] = Convert.ToDouble(RSRP);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        //删除所有IMSI
        public void DellAll()
        {
            try
            {
                MeasReportAxialChartBlackList.Clear();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        //IMSI
        private string _iMSI = string.Empty;
        public class IMSIListClass : INotifyPropertyChanged
        {
            private string _iMSI = string.Empty;

            public string IMSI
            {
                get
                {
                    return _iMSI;
                }

                set
                {
                    _iMSI = value;
                    NotifyPropertyChanged("IMSI");
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

        //时间
        private string _dTime = string.Empty;
        private Thread DTimeThread = null;

        //移动曲线控制            
        private int _moveLineControl = 0;

        public double ChartArea_Width
        {
            get
            {
                return _chartArea_Width;
            }

            set
            {
                _chartArea_Width = value;
                NotifyPropertyChanged("CharArea_Width");
            }
        }

        public double ChartArea_Height
        {
            get
            {
                return _chartArea_Height;
            }

            set
            {
                _chartArea_Height = value;
                NotifyPropertyChanged("CharArea_Height");
            }
        }

        public Ellipse Node
        {
            get
            {
                return _node;
            }

            set
            {
                _node = value;
                NotifyPropertyChanged("Node");
            }
        }

        public Rectangle OutLine
        {
            get
            {
                return _outLine;
            }

            set
            {
                _outLine = value;
                NotifyPropertyChanged("OutLine");
            }
        }

        public string ChartBackGround
        {
            get
            {
                return _chartBackGround;
            }

            set
            {
                _chartBackGround = value;
                NotifyPropertyChanged("ChartBackGround");
            }
        }

        public string GridLineColor
        {
            get
            {
                return _gridLineColor;
            }

            set
            {
                _gridLineColor = value;
                NotifyPropertyChanged("GridLineColor");
            }
        }

        public bool GridEnable
        {
            get
            {
                return _gridEnable;
            }

            set
            {
                _gridEnable = value;
                NotifyPropertyChanged("GridEnable");
            }
        }

        public string IMSI
        {
            get
            {
                return _iMSI;
            }

            set
            {
                _iMSI = value;
                NotifyPropertyChanged("IMSI");
            }
        }

        public string DTime
        {
            get
            {
                return _dTime;
            }

            set
            {
                _dTime = value;
                NotifyPropertyChanged("DTime");
            }
        }

        public double XOrig
        {
            get
            {
                return _xOrig;
            }

            set
            {
                _xOrig = value;
                NotifyPropertyChanged("XOrig");
            }
        }

        public double YOrig
        {
            get
            {
                return _yOrig;
            }

            set
            {
                _yOrig = value;
                NotifyPropertyChanged("YOrig");
            }
        }

        public double X_QuadrantLineSize
        {
            get
            {
                return _X_QuadrantLineSize;
            }

            set
            {
                _X_QuadrantLineSize = value;
                NotifyPropertyChanged("X_QuadrantLineSize");
            }
        }

        public double Y_QuadrantLineSize
        {
            get
            {
                return _Y_QuadrantLineSize;
            }

            set
            {
                _Y_QuadrantLineSize = value;
                NotifyPropertyChanged("Y_QuadrantLineSize");
            }
        }

        public double GridLineSize
        {
            get
            {
                return _gridLineSize;
            }

            set
            {
                _gridLineSize = value;
                NotifyPropertyChanged("GridLineSize");
            }
        }

        public double OutLineSize
        {
            get
            {
                return _outLineSize;
            }

            set
            {
                _outLineSize = value;
                NotifyPropertyChanged("OutLineSize");
            }
        }

        public string OutLineColor
        {
            get
            {
                return _outLineColor;
            }

            set
            {
                _outLineColor = value;
                NotifyPropertyChanged("OutLineColor");
            }
        }

        public string X_QuadrantColor
        {
            get
            {
                return _x_QuadrantColor;
            }

            set
            {
                _x_QuadrantColor = value;
                NotifyPropertyChanged("X_QuadrantColor");
            }
        }

        public string Y_QuadrantColor
        {
            get
            {
                return _y_QuadrantColor;
            }

            set
            {
                _y_QuadrantColor = value;
                NotifyPropertyChanged("Y_QuadrantColor");
            }
        }

        public string NodeColor
        {
            get
            {
                return _nodeColor;
            }

            set
            {
                _nodeColor = value;
                NotifyPropertyChanged("NodeColor");
            }
        }

        public double NodeSize
        {
            get
            {
                return _nodeSize;
            }

            set
            {
                _nodeSize = value;
                NotifyPropertyChanged("NodeSize");
            }
        }

        public double Node_X
        {
            get
            {
                return _node_X;
            }

            set
            {
                _node_X = value;
                NotifyPropertyChanged("Node_X");
            }
        }

        public double Node_Y
        {
            get
            {
                return _node_Y;
            }

            set
            {
                _node_Y = value;
                NotifyPropertyChanged("Node_Y");
            }
        }

        public double OutLineXStart
        {
            get
            {
                return _outLineXStart;
            }

            set
            {
                _outLineXStart = value;
                NotifyPropertyChanged("OutLineXStart");
            }
        }

        public double OutLineYStart
        {
            get
            {
                return _outLineYStart;
            }

            set
            {
                _outLineYStart = value;
                NotifyPropertyChanged("OutLineYStart");
            }
        }

        public ArrowLine X_QuadrantArrow
        {
            get
            {
                return x_QuadrantArrow;
            }

            set
            {
                x_QuadrantArrow = value;
                NotifyPropertyChanged("X_QuadrantArrow");
            }
        }

        public ArrowLine Y_QuadrantArrow
        {
            get
            {
                return y_QuadrantArrow;
            }

            set
            {
                y_QuadrantArrow = value;
                NotifyPropertyChanged("Y_QuadrantArrow");
            }
        }

        public string X_QuadrantArrowColor
        {
            get
            {
                return _x_QuadrantArrowColor;
            }

            set
            {
                _x_QuadrantArrowColor = value;
                NotifyPropertyChanged("X_QuadrantArrowColor");
            }
        }

        public string Y_QuadrantArrowColor
        {
            get
            {
                return _y_QuadrantArrowColor;
            }

            set
            {
                _y_QuadrantArrowColor = value;
                NotifyPropertyChanged("Y_QuadrantArrowColor");
            }
        }

        public double X_QuadrantArrowLineSize
        {
            get
            {
                return _X_QuadrantArrowLineSize;
            }

            set
            {
                _X_QuadrantArrowLineSize = value;
                NotifyPropertyChanged("X_QuadrantArrowLineSize");
            }
        }

        public double Y_QuadrantArrowLineSize
        {
            get
            {
                return _Y_QuadrantArrowLineSize;
            }

            set
            {
                _Y_QuadrantArrowLineSize = value;
                NotifyPropertyChanged("Y_QuadrantArrowLineSize");
            }
        }

        public double GridLineOpacity
        {
            get
            {
                return _gridLineOpacity;
            }

            set
            {
                _gridLineOpacity = value;
                NotifyPropertyChanged("GridLineOpacity");
            }
        }

        public double ChartWindowHeight
        {
            get
            {
                return _chartWindowHeight;
            }

            set
            {
                _chartWindowHeight = value;
                NotifyPropertyChanged("ChartWindowHeight");
            }
        }

        public double ChartWindowWidth
        {
            get
            {
                return _chartWindowWidth;
            }

            set
            {
                _chartWindowWidth = value;
                NotifyPropertyChanged("ChartWindowWidth");
            }
        }

        public string BackGroundGridLineColor
        {
            get
            {
                return _backGroundGridLineColor;
            }

            set
            {
                _backGroundGridLineColor = value;
                NotifyPropertyChanged("BackGroundGridLineColor");
            }
        }

        public bool BackGroundGridEnable
        {
            get
            {
                return _backGroundGridEnable;
            }

            set
            {
                _backGroundGridEnable = value;
                NotifyPropertyChanged("BackGroundGridEnable");
            }
        }

        public double BackGroundGridLineSize
        {
            get
            {
                return _backGroundGridLineSize;
            }

            set
            {
                _backGroundGridLineSize = value;
                NotifyPropertyChanged("BackGroundGridLineSize");
            }
        }

        public double BackGroundGridLineSpace
        {
            get
            {
                return _backGroundGridLineSpace;
            }

            set
            {
                _backGroundGridLineSpace = value;
                NotifyPropertyChanged("BackGroundGridLineSpace");
            }
        }

        public double BackGroundGridLineOpacity
        {
            get
            {
                return _backGroundGridLineOpacity;
            }

            set
            {
                _backGroundGridLineOpacity = value;
                NotifyPropertyChanged("BackGroundGridLineOpacity");
            }
        }

        public double XBgOrig
        {
            get
            {
                return _xBgOrig;
            }

            set
            {
                _xBgOrig = value;
                NotifyPropertyChanged("XBgOrig");
            }
        }

        public double YBgOrig
        {
            get
            {
                return _yBgOrig;
            }

            set
            {
                _yBgOrig = value;
                NotifyPropertyChanged("YBgOrig");
            }
        }

        public double X_AxialScaleLineLen
        {
            get
            {
                return _x_AxialScaleLineLen;
            }

            set
            {
                _x_AxialScaleLineLen = value;
                NotifyPropertyChanged("X_AxialScaleLineLen");
            }
        }

        public double Y_AxialScaleLineLen
        {
            get
            {
                return _y_AxialScaleLineLen;
            }

            set
            {
                _y_AxialScaleLineLen = value;
                NotifyPropertyChanged("Y_AxialScaleLineLen");
            }
        }

        public double X_QuadrantAxialLineLength
        {
            get
            {
                return _x_QuadrantAxialLineLength;
            }

            set
            {
                _x_QuadrantAxialLineLength = value;
                NotifyPropertyChanged("X_QuadrantAxialLineLength");
            }
        }

        public double Y_QuadrantAxialLineLength
        {
            get
            {
                return _y_QuadrantAxialLineLength;
            }

            set
            {
                _y_QuadrantAxialLineLength = value;
                NotifyPropertyChanged("Y_QuadrantAxialLineLength");
            }
        }

        public double X_QuadrantArrowAxialLineLength
        {
            get
            {
                return _x_QuadrantArrowAxialLineLength;
            }

            set
            {
                _x_QuadrantArrowAxialLineLength = value;
                NotifyPropertyChanged("X_QuadrantArrowAxialLineLength");
            }
        }

        public double Y_QuadrantArrowAxialLineLength
        {
            get
            {
                return _y_QuadrantArrowAxialLineLength;
            }

            set
            {
                _y_QuadrantArrowAxialLineLength = value;
                NotifyPropertyChanged("Y_QuadrantArrowAxialLineLength");
            }
        }

        public double X_LongAxialScaleLineLen
        {
            get
            {
                return _x_LongAxialScaleLineLen;
            }

            set
            {
                _x_LongAxialScaleLineLen = value;
                NotifyPropertyChanged("X_LongAxialScaleLineLen");
            }
        }

        public double Y_LongAxialScaleLineLen
        {
            get
            {
                return _y_LongAxialScaleLineLen;
            }

            set
            {
                _y_LongAxialScaleLineLen = value;
                NotifyPropertyChanged("Y_LongAxialScaleLineLen");
            }
        }

        public double LongGridLineOpacity
        {
            get
            {
                return _longGridLineOpacity;
            }

            set
            {
                _longGridLineOpacity = value;
                NotifyPropertyChanged("LongGridLineOpacity");
            }
        }

        public double LongGridLineSize
        {
            get
            {
                return _longGridLineSize;
            }

            set
            {
                _longGridLineSize = value;
                NotifyPropertyChanged("LongGridLineSize");
            }
        }

        public string LongGridLineColor
        {
            get
            {
                return _longGridLineColor;
            }

            set
            {
                _longGridLineColor = value;
                NotifyPropertyChanged("LongGridLineColor");
            }
        }

        public double XShortScaleLineSpace
        {
            get
            {
                return _xShortScaleLineSpace;
            }

            set
            {
                _xShortScaleLineSpace = value;
                NotifyPropertyChanged("XShortScaleLineSpace");
            }
        }

        public double YShortScaleLineSpace
        {
            get
            {
                return _yShortScaleLineSpace;
            }

            set
            {
                _yShortScaleLineSpace = value;
                NotifyPropertyChanged("YShortScaleLineSpace");
            }
        }

        public double XLongScaleLineSpace
        {
            get
            {
                return _xLongScaleLineSpace;
            }

            set
            {
                _xLongScaleLineSpace = value;
                NotifyPropertyChanged("XLongScaleLineSpace");
            }
        }

        public double YLongScaleLineSpace
        {
            get
            {
                return _yLongScaleLineSpace;
            }

            set
            {
                _yLongScaleLineSpace = value;
                NotifyPropertyChanged("YLongScaleLineSpace");
            }
        }

        public double XAxialScaleValueFontSize
        {
            get
            {
                return _xAxialScaleValueFontSize;
            }

            set
            {
                _xAxialScaleValueFontSize = value;
                NotifyPropertyChanged("XAxialScaleValueSize");
            }
        }

        public double XAxialScaleValueOpacity
        {
            get
            {
                return _xAxialScaleValueOpacity;
            }

            set
            {
                _xAxialScaleValueOpacity = value;
                NotifyPropertyChanged("XAxialScaleValueOpacity");
            }
        }

        public string XAxialScaleValueColor
        {
            get
            {
                return _xAxialScaleValueColor;
            }

            set
            {
                _xAxialScaleValueColor = value;
                NotifyPropertyChanged("XAxialScaleValueColor");
            }
        }

        public double YAxialScaleValueFontSize
        {
            get
            {
                return _yAxialScaleValueFontSize;
            }

            set
            {
                _yAxialScaleValueFontSize = value;
                NotifyPropertyChanged("YAxialScaleValueSize");
            }
        }

        public double YAxialScaleValueOpacity
        {
            get
            {
                return _yAxialScaleValueOpacity;
            }

            set
            {
                _yAxialScaleValueOpacity = value;
                NotifyPropertyChanged("YAxialScaleValueOpacity");
            }
        }

        public string YAxialScaleValueColor
        {
            get
            {
                return _yAxialScaleValueColor;
            }

            set
            {
                _yAxialScaleValueColor = value;
                NotifyPropertyChanged("YAxialScaleValueColor");
            }
        }

        public Canvas SelfChart
        {
            get
            {
                return _selfChart;
            }

            set
            {
                _selfChart = value;
                NotifyPropertyChanged("SelfChart");
            }
        }

        public double X_QuadrantAxialStart
        {
            get
            {
                return _x_QuadrantAxialStart;
            }

            set
            {
                _x_QuadrantAxialStart = value;
                NotifyPropertyChanged("X_QuadrantAxialStart");
            }
        }

        public double Y_QuadrantAxialStart
        {
            get
            {
                return _y_QuadrantAxialStart;
            }

            set
            {
                _y_QuadrantAxialStart = value;
                NotifyPropertyChanged("Y_QuadrantAxialStart");
            }
        }

        public double X_QuadrantArrowAxialStart
        {
            get
            {
                return _x_QuadrantArrowAxialStart;
            }

            set
            {
                _x_QuadrantArrowAxialStart = value;
                NotifyPropertyChanged("X_QuadrantArrowAxialStart");
            }
        }

        public double Y_QuadrantArrowAxialStart
        {
            get
            {
                return _y_QuadrantArrowAxialStart;
            }

            set
            {
                _y_QuadrantArrowAxialStart = value;
                NotifyPropertyChanged("Y_QuadrantArrowAxialStart");
            }
        }

        public double X_QuadrantArrowAxialLineLengthSetting
        {
            get
            {
                return _x_QuadrantArrowAxialLineLengthSetting;
            }

            set
            {
                _x_QuadrantArrowAxialLineLengthSetting = value;
                NotifyPropertyChanged("X_QuadrantArrowAxialLineLengthSetting");
            }
        }

        public double Y_QuadrantArrowAxialLineLengthSetting
        {
            get
            {
                return _y_QuadrantArrowAxialLineLengthSetting;
            }

            set
            {
                _y_QuadrantArrowAxialLineLengthSetting = value;
                NotifyPropertyChanged("Y_QuadrantArrowAxialLineLengthSetting");
            }
        }

        public int MoveLineControl
        {
            get
            {
                return _moveLineControl;
            }

            set
            {
                _moveLineControl = value;
                NotifyPropertyChanged("MoveLineControl");
            }
        }

        public int X_QuadrantScaleValueCount
        {
            get
            {
                return x_QuadrantScaleValueCount;
            }

            set
            {
                x_QuadrantScaleValueCount = value;
                NotifyPropertyChanged("X_QuadrantScaleValueCount");
            }
        }

        public int Y_QuadrantScaleValueCount
        {
            get
            {
                return y_QuadrantScaleValueCount;
            }

            set
            {
                y_QuadrantScaleValueCount = value;
                NotifyPropertyChanged("Y_QuadrantScaleValueCount");
            }
        }

        public int X_QuadrantArrowScaleValueCount
        {
            get
            {
                return x_QuadrantArrowScaleValueCount;
            }

            set
            {
                x_QuadrantArrowScaleValueCount = value;
                NotifyPropertyChanged("X_QuadrantArrowScaleValueCount");
            }
        }

        public int Y_QuadrantArrowScaleValueCount
        {
            get
            {
                return y_QuadrantArrowScaleValueCount;
            }

            set
            {
                y_QuadrantArrowScaleValueCount = value;
                NotifyPropertyChanged("Y_QuadrantArrowScaleValueCount");
            }
        }

        public object ChartLock
        {
            get
            {
                return _ChartLock;
            }

            set
            {
                _ChartLock = value;
                NotifyPropertyChanged("ChartLock");
            }
        }

        public double Second
        {
            get
            {
                return _second;
            }

            set
            {
                _second = value;
            }
        }

        public string XUnit
        {
            get
            {
                return _xUnit;
            }

            set
            {
                _xUnit = value;
            }
        }

        public bool RmDupFlag
        {
            get
            {
                return rmDupFlag;
            }

            set
            {
                rmDupFlag = value;
            }
        }

        //构造
        public StatisticalAnalysisClass(double width, double height)
        {
            ChartArea_Width = width;
            ChartArea_Height = height;
            if (MeasReportAxialChartBlackList == null)
            {
                MeasReportAxialChartBlackList = new DataTable("BlackList");
                InitialTab();
            }
            InitChart();
        }

        //构造
        public StatisticalAnalysisClass()
        {
            if (MeasReportAxialChartBlackList == null)
            {
                MeasReportAxialChartBlackList = new DataTable("BlackList");
                InitialTab();
            }

            InitChart();
        }

        //初始化数据表
        private void InitialTab()
        {
            DataColumn DataColumn0 = new DataColumn();
            DataColumn0.ColumnName = "ID";
            DataColumn0.DataType = System.Type.GetType("System.String");

            DataColumn DataColumn1 = new DataColumn();
            DataColumn1.ColumnName = "ImsiTotal";
            DataColumn1.DataType = System.Type.GetType("System.String");

            DataColumn DataColumn2 = new DataColumn();
            DataColumn2.ColumnName = "ImsiTotalRmDup";
            DataColumn2.DataType = System.Type.GetType("System.Int32");

            MeasReportAxialChartBlackList.Columns.Add(DataColumn0);
            MeasReportAxialChartBlackList.Columns.Add(DataColumn1);
            MeasReportAxialChartBlackList.Columns.Add(DataColumn2);
        }

        private void InitChart()
        {
            try
            {
                if (ChartLock == null)
                {
                    ChartLock = new object();
                }

                if (X_Quadrant == null)
                {
                    X_Quadrant = new Line();
                }

                if (Y_Quadrant == null)
                {
                    Y_Quadrant = new Line();
                }

                if (X_QuadrantArrow == null)
                {
                    X_QuadrantArrow = new ArrowLine();
                }

                if (Y_QuadrantArrow == null)
                {
                    Y_QuadrantArrow = new ArrowLine();
                }

                if (Node == null)
                {
                    Node = new Ellipse();
                }

                if (OutLine == null)
                {
                    OutLine = new Rectangle();
                }

                if (BlackListColors == null)
                {
                    BlackListColors = new List<string>();
                    BlackListColors.Add("Aqua");
                    BlackListColors.Add("Blue");
                    BlackListColors.Add("Red");
                    BlackListColors.Add("Yellow");
                    BlackListColors.Add("Green");
                    BlackListColors.Add("White");
                    BlackListColors.Add("Brown");
                    BlackListColors.Add("Teal");
                    BlackListColors.Add("Coral");
                    BlackListColors.Add("Crimson");
                    BlackListColors.Add("DarkRed");
                    BlackListColors.Add("DodgerBlue");
                    BlackListColors.Add("#341A1C");
                    BlackListColors.Add("#080F4C");
                    BlackListColors.Add("Orchid");
                    BlackListColors.Add("#FF7200");
                }

                if (GridLine == null)
                {
                    GridLine = new List<Line>();
                }

                if (DTimeThread == null)
                {
                    DTimeThread = new Thread(new ThreadStart(Time));
                    DTimeThread.DisableComObjectEagerCleanup();
                    DTimeThread.Start();
                }

                if (BackGroundGridLine == null)
                {
                    BackGroundGridLine = new List<Line>();
                }

                if (LongScaleLine == null)
                {
                    LongScaleLine = new List<Line>();
                }

                if (XAxialScaleValueList == null)
                {
                    XAxialScaleValueList = new List<Label>();
                }

                if (YAxialScaleValueList == null)
                {
                    YAxialScaleValueList = new List<Label>();
                }

                if (MeasureReportLineList == null)
                {
                    MeasureReportLineList = new Dictionary<string, List<ChartDotLineParameterClass>>();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        //引用绘图区
        public void SettingChartHandle(ref Canvas selfCanvas)
        {
            SelfChart = selfCanvas;
        }

        //X象限(无箭头)
        public void XQuadrant()
        {
            try
            {
                X_Quadrant.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(X_QuadrantColor));
                X_Quadrant.X1 = XOrig;
                X_Quadrant.Y1 = ChartArea_Height - YOrig;

                X_Quadrant.X2 = ChartArea_Width - XOrig;
                X_Quadrant.Y2 = ChartArea_Height - YOrig;
                X_Quadrant.StrokeThickness = X_QuadrantLineSize;

                X_QuadrantAxialLineLength = ChartArea_Width - XOrig * 2;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("X象限", ex.Message, ex.StackTrace);
            }
        }

        //X象限(带箭头)
        public void XQuadrantArrow()
        {
            try
            {
                X_QuadrantArrow.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(X_QuadrantColor));
                X_QuadrantArrow.StartPoint = new Point(XOrig, ChartArea_Height - YOrig);
                X_QuadrantArrow.EndPoint = new Point(ChartArea_Width - XOrig + X_QuadrantArrowAxialLineLengthSetting, ChartArea_Height - YOrig);
                X_QuadrantArrow.StrokeThickness = X_QuadrantArrowLineSize;
                X_QuadrantArrowAxialLineLength = ChartArea_Width - XOrig * 2;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("X象限", ex.Message, ex.StackTrace);
            }
        }

        //Y象限(带箭头)
        public void YQuadrantArrow()
        {
            try
            {
                Y_QuadrantArrow.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Y_QuadrantColor));
                Y_QuadrantArrow.StartPoint = new Point(XOrig, ChartArea_Height - YOrig);
                Y_QuadrantArrowAxialStart = ChartArea_Height - YOrig;
                Y_QuadrantArrow.EndPoint = new Point(XOrig, YOrig - Y_QuadrantArrowAxialLineLengthSetting);
                Y_QuadrantArrow.StrokeThickness = Y_QuadrantArrowLineSize;
                Y_QuadrantArrowAxialLineLength = ChartArea_Height - YOrig * 2;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Y象限", ex.Message, ex.StackTrace);
            }
        }

        //Y象限(无箭头)
        public void YQuadrant()
        {
            try
            {
                Y_Quadrant.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Y_QuadrantColor));
                Y_Quadrant.X1 = XOrig;
                Y_Quadrant.Y1 = YOrig;

                Y_Quadrant.X2 = XOrig;
                Y_Quadrant.Y2 = ChartArea_Height - YOrig;
                Y_Quadrant.StrokeThickness = Y_QuadrantLineSize;
                Y_QuadrantAxialLineLength = ChartArea_Height - YOrig * 2;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Y象限", ex.Message, ex.StackTrace);
            }
        }

        //边框线
        public void ChartOutLine()
        {
            try
            {
                OutLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(OutLineColor));
                OutLine.Width = Math.Abs(ChartArea_Width - OutLineXStart * 2);
                OutLine.Height = Math.Abs(ChartArea_Height - OutLineYStart * 2);
                OutLine.StrokeThickness = 1;
                OutLine.SetValue(Canvas.LeftProperty, OutLineXStart);
                OutLine.SetValue(Canvas.TopProperty, OutLineYStart);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表边框", ex.Message, ex.StackTrace);
            }
        }

        //结点
        public void NodePoint()
        {
            try
            {
                Node.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NodeColor));
                Node.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(NodeColor));
                Node.Width = NodeSize;
                Node.Height = NodeSize;
                Node.SetValue(Canvas.LeftProperty, Node_X);
                Node.SetValue(Canvas.TopProperty, Node_Y);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("结点", ex.Message, ex.StackTrace);
            }
        }

        //短刻度标示线
        public void ChartShortScaleLine()
        {
            try
            {
                //double YLineCount = Y_QuadrantArrowScaleValueCount;
                //YShortScaleLineSpace = (Y_QuadrantArrowAxialLineLength - 10) / YLineCount;
                GridLine.Clear();
                //for (int i = 1; i <= YLineCount; i++)
                //{
                //    Line YgridLine = new Line();
                //    YgridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridLineColor));
                //    YgridLine.X1 = XOrig;
                //    YgridLine.Y1 = ChartArea_Height - YOrig - (YShortScaleLineSpace * (i));
                //    YgridLine.Opacity = GridLineOpacity;
                //    YgridLine.X2 = XOrig - Y_AxialScaleLineLen;
                //    YgridLine.Y2 = ChartArea_Height - YOrig - (YShortScaleLineSpace * (i));
                //    YgridLine.StrokeThickness = GridLineSize;
                //    GridLine.Add(YgridLine);
                //}

                //double XLineCount = X_QuadrantArrowScaleValueCount;
                //XShortScaleLineSpace = (X_QuadrantArrowAxialLineLength - 10) / XLineCount;
                //for (int i = 1; i <= XLineCount; i++)
                //{
                //    Line XgridLine = new Line();
                //    XgridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridLineColor));
                //    XgridLine.X1 = XOrig + (XShortScaleLineSpace * (i));
                //    XgridLine.Y1 = ChartArea_Height - YOrig;
                //    XgridLine.Opacity = GridLineOpacity;
                //    XgridLine.X2 = XOrig + (XShortScaleLineSpace * (i));
                //    XgridLine.Y2 = ChartArea_Height - YOrig + X_AxialScaleLineLen;
                //    XgridLine.StrokeThickness = GridLineSize;
                //    GridLine.Add(XgridLine);
                //    ChartXAxialScaleValue(XgridLine.X2, XgridLine.Y2, (i).ToString() + "(S)", "Yellow", 12);
                //}
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("刻度线标示", ex.Message, ex.StackTrace);
            }
        }

        //长刻度标示线
        public void ChartLongScaleLine()
        {
            try
            {
                double YLongLineCount = 10;
                YShortScaleLineSpace = (Y_QuadrantArrowAxialLineLength - 10) / 100;
                LongScaleLine.Clear();
                XAxialScaleValueList.Clear();
                YAxialScaleValueList.Clear();
                for (int i = 1; i <= YLongLineCount; i++)
                {
                    Line YLgGridLine = new Line();
                    YLgGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LongGridLineColor));
                    YLgGridLine.X1 = XOrig;
                    YLgGridLine.Y1 = ChartArea_Height - YOrig - YShortScaleLineSpace * 10 * i;
                    YLgGridLine.Opacity = LongGridLineOpacity;
                    YLgGridLine.X2 = XOrig - Y_LongAxialScaleLineLen;
                    YLgGridLine.Y2 = ChartArea_Height - YOrig - YShortScaleLineSpace * 10 * i;
                    YLgGridLine.StrokeThickness = LongGridLineSize;
                    LongScaleLine.Add(YLgGridLine);
                    ChartYAxialScaleValue(YLgGridLine.X2, YLgGridLine.Y2, (i * (Y_QuadrantArrowScaleValueCount / 10)).ToString(), "Yellow", 12);
                }

                double XLongLineCount = X_QuadrantArrowScaleValueCount;
                for (int i = 1; i <= XLongLineCount; i++)
                {
                    Line XLgGridLine = new Line();
                    XLgGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(LongGridLineColor));
                    XLgGridLine.X1 = XOrig + XShortScaleLineSpace * 10 * i;
                    XLgGridLine.Y1 = ChartArea_Height - YOrig;
                    XLgGridLine.Opacity = LongGridLineOpacity;
                    XLgGridLine.X2 = XOrig + XShortScaleLineSpace * 10 * i;
                    XLgGridLine.Y2 = ChartArea_Height - YOrig + X_LongAxialScaleLineLen;
                    XLgGridLine.StrokeThickness = LongGridLineSize;
                    LongScaleLine.Add(XLgGridLine);
                    ChartXAxialScaleValue(XLgGridLine.X2, XLgGridLine.Y2, (i).ToString() + XUnit, "Yellow", 12);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("刻度线标示", ex.Message, ex.StackTrace);
            }
        }

        //X轴刻度值标示
        public void ChartXAxialScaleValue(double X, double Y, string Value, string ValueColor, double Size)
        {
            try
            {
                if (ValueColor != null)
                {
                    XAxialScaleValueColor = ValueColor;
                }
                if (Size > 0)
                {
                    XAxialScaleValueFontSize = Size;
                }

                Label XAxialScaleValue = new Label();
                XAxialScaleValue.Width = 60;
                XAxialScaleValue.Height = 25;
                XAxialScaleValue.Content = Value;
                XAxialScaleValue.Opacity = XAxialScaleValueOpacity;
                XAxialScaleValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(XAxialScaleValueColor));
                XAxialScaleValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                XAxialScaleValue.VerticalAlignment = VerticalAlignment.Center;
                XAxialScaleValue.VerticalContentAlignment = VerticalAlignment.Center;
                XAxialScaleValue.FontSize = XAxialScaleValueFontSize;
                Canvas.SetLeft(XAxialScaleValue, X - (XAxialScaleValue.Width / 2));
                Canvas.SetTop(XAxialScaleValue, Y + 2);
                XAxialScaleValueList.Add(XAxialScaleValue);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("刻度值标示", ex.Message, ex.StackTrace);
            }
        }

        //Y轴刻度值标示
        public void ChartYAxialScaleValue(double X, double Y, string Value, string ValueColor, double Size)
        {
            try
            {
                if (ValueColor != null)
                {
                    YAxialScaleValueColor = ValueColor;
                }
                if (Size > 0)
                {
                    YAxialScaleValueFontSize = Size;
                }

                Label YAxialScaleValue = new Label();
                YAxialScaleValue.Width = 60;
                YAxialScaleValue.Height = 25;
                YAxialScaleValue.Content = Value;
                YAxialScaleValue.Opacity = YAxialScaleValueOpacity;
                YAxialScaleValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(YAxialScaleValueColor));
                YAxialScaleValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                YAxialScaleValue.VerticalAlignment = VerticalAlignment.Center;
                YAxialScaleValue.VerticalContentAlignment = VerticalAlignment.Center;
                YAxialScaleValue.FontSize = YAxialScaleValueFontSize;
                YAxialScaleValue.HorizontalContentAlignment = HorizontalAlignment.Right;
                Canvas.SetLeft(YAxialScaleValue, X - YAxialScaleValue.Width - 2);
                Canvas.SetTop(YAxialScaleValue, Y - (YAxialScaleValue.Height / 2));
                YAxialScaleValueList.Add(YAxialScaleValue);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("刻度值标示", ex.Message, ex.StackTrace);
            }
        }

        //文字宽度
        private double MeasureTextWidth(double fontSize, Label Element)
        {
            FormattedText formattedText = new
                   FormattedText(Element.Content.ToString(), CultureInfo.CurrentCulture,
                   FlowDirection.LeftToRight, new Typeface(Element.FontFamily, Element.FontStyle,
                                                          Element.FontWeight, Element.FontStretch
                                                          ),
                   fontSize, Element.Foreground, null);

            return formattedText.WidthIncludingTrailingWhitespace;
        }

        //背景网格线
        public void ChartBackGroundGridLine()
        {
            try
            {
                double YBgLineCount = ChartArea_Height / BackGroundGridLineSpace;
                BackGroundGridLine.Clear();
                for (int i = 0; i < YBgLineCount - 1; i++)
                {
                    Line YBgGridLine = new Line();
                    YBgGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackGroundGridLineColor));
                    YBgGridLine.X1 = XBgOrig;
                    YBgGridLine.Y1 = ChartArea_Height - YBgOrig - (BackGroundGridLineSpace * (i + 1));
                    YBgGridLine.Opacity = BackGroundGridLineOpacity;
                    YBgGridLine.X2 = ChartArea_Width - XBgOrig;
                    YBgGridLine.Y2 = ChartArea_Height - YBgOrig - (BackGroundGridLineSpace * (i + 1));
                    YBgGridLine.StrokeThickness = BackGroundGridLineSize;
                    BackGroundGridLine.Add(YBgGridLine);
                }

                double XBgLineCount = ChartArea_Width / BackGroundGridLineSpace;
                for (int i = 0; i < XBgLineCount - 1; i++)
                {
                    Line XBgGridLine = new Line();
                    XBgGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackGroundGridLineColor));
                    XBgGridLine.X1 = XBgOrig + (BackGroundGridLineSpace * (i + 1));
                    XBgGridLine.Y1 = YBgOrig;
                    XBgGridLine.Opacity = BackGroundGridLineOpacity;
                    XBgGridLine.X2 = XBgOrig + (BackGroundGridLineSpace * (i + 1));
                    XBgGridLine.Y2 = ChartArea_Height - YBgOrig;
                    XBgGridLine.StrokeThickness = BackGroundGridLineSize;
                    BackGroundGridLine.Add(XBgGridLine);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("网格线", ex.Message, ex.StackTrace);
            }
        }

        //时间函数
        private void Time()
        {
            while (true)
            {
                try
                {
                    DTime = DateTime.Now.ToString();
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("时间", ex.Message, ex.StackTrace);
                }
                Thread.Sleep(1000);
            }
        }
        //绘制柱形曲线
        public void DrawingCanvas()
        {
            try
            {
                for (int i = 0; i < MeasReportAxialChartBlackList.Rows.Count; i++)
                {
                    string _IMSI = string.Empty;
                    double RSRP = 0xFF;
                    _IMSI = "";
                    if (RmDupFlag)
                        RSRP = Convert.ToDouble(MeasReportAxialChartBlackList.Rows[i][2].ToString());
                    else
                        RSRP = Convert.ToDouble(MeasReportAxialChartBlackList.Rows[i][1].ToString());
                    SelfChart.Dispatcher.Invoke(new Action(() =>
                    {
                        lock (ChartLock)
                        {
                            double X1 = XOrig + XShortScaleLineSpace * 10 * (i + 1);
                            double Y1 = (ChartArea_Height - YOrig);
                            double X2 = XOrig + XShortScaleLineSpace * 10 * (i + 1);
                            double Y2 = (ChartArea_Height - YOrig) - ((Math.Abs(RSRP) / Y_QuadrantArrowScaleValueCount) * 10 * YShortScaleLineSpace * 10);

                            //线
                            Line SelfLineName = new Line();
                            SelfLineName.Tag = Second;
                            SelfLineName.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[0]));
                            SelfLineName.StrokeThickness = 10;
                            SelfLineName.Opacity = 1;
                            SelfLineName.X1 = X1;
                            SelfLineName.Y1 = Y1;
                            SelfLineName.X2 = X2;
                            SelfLineName.Y2 = Y2;
                            SelfLineName.ToolTip = "总数:" + RSRP.ToString();
                            SelfLineName.MouseUp += SelfLineName_MouseUp;

                            //Label SelfLabelName = new Label();
                            //SelfLabelName.Width = 32;
                            //SelfLabelName.Height = 25;
                            //SelfLabelName.Content = RSRP.ToString();
                            //SelfLabelName.Opacity = 1;
                            //SelfLabelName.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[2]));
                            //SelfLabelName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                            //SelfLabelName.VerticalAlignment = VerticalAlignment.Center;
                            //SelfLabelName.VerticalContentAlignment = VerticalAlignment.Center;
                            //SelfLabelName.FontSize = 12;
                            //Canvas.SetLeft(SelfLabelName, X2 - (SelfLabelName.Width / 2));
                            //Canvas.SetTop(SelfLabelName, Y2 - 20);

                            //SelfChart.Children.Add(SelfLabelName);
                            SelfChart.Children.Add(SelfLineName);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("分时统计曲线", ex.Message, ex.StackTrace);
            }
        }

        private void SelfLineName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Line tmpline = (sender as Line);
            MessageBox.Show(tmpline.ToolTip.ToString());
        }

        //绘制曲线图
        public void DrawingMeasureReportAxial()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);

                    foreach (DataRow dr in MeasReportAxialChartBlackList.Rows)
                    {
                        string _IMSI = string.Empty;
                        double RSRP = 0xFF;
                        _IMSI = "";
                        if (RmDupFlag)
                            RSRP = Convert.ToDouble(dr["ImsiTotalRmDup"].ToString());
                        else
                            RSRP = Convert.ToDouble(dr["ImsiTotal"].ToString());

                        SelfChart.Dispatcher.Invoke(new Action(() =>
                        {
                            lock (ChartLock)
                            {
                                if (SelfChart.Children.Count > 0)
                                {
                                    bool Flag = true;
                                    for (int j = 0; j < SelfChart.Children.Count; j += 2)
                                    {
                                        if (new Regex(_IMSI).IsMatch((SelfChart.Children[j] as Line).Name))
                                        {
                                            double X1 = XOrig;
                                            double Y1 = YOrig;
                                            double X2 = XOrig;
                                            double Y2 = YOrig;
                                            Brush SelfColor = null;

                                            //得到最后元素坐标
                                            for (int k = j; k < SelfChart.Children.Count; k += 2)
                                            {
                                                //到最后时结束
                                                if (!new Regex(_IMSI).IsMatch((SelfChart.Children[k] as Line).Name))
                                                {
                                                    break;
                                                }

                                                X1 = (SelfChart.Children[k] as Line).X2;
                                                Y1 = (SelfChart.Children[k] as Line).Y2;
                                                X2 = (SelfChart.Children[k] as Line).X2 + XShortScaleLineSpace * 10;
                                                Y2 = Y_QuadrantArrowAxialStart - ((Math.Abs(RSRP) / Y_QuadrantArrowScaleValueCount) * 10 * YShortScaleLineSpace * 10);
                                                SelfColor = (SelfChart.Children[k] as Line).Stroke;
                                            }

                                            //线
                                            Line SelfLineName = new Line();
                                            SelfLineName.Name = "L" + "_" + _IMSI + "_" + Second.ToString();
                                            SelfLineName.Tag = Second;
                                            SelfLineName.Stroke = SelfColor;
                                            SelfLineName.StrokeThickness = 1;
                                            SelfLineName.Opacity = 1;
                                            SelfLineName.X1 = X1;
                                            SelfLineName.Y1 = Y1;
                                            SelfLineName.X2 = X2;
                                            SelfLineName.Y2 = Y2;
                                            SelfLineName.ToolTip = "总数:" + RSRP.ToString();

                                            //结点
                                            Ellipse SelfEllipseName = new Ellipse();
                                            SelfEllipseName.Name = "E" + "_" + _IMSI + "_" + Second.ToString();
                                            SelfEllipseName.Tag = Second;
                                            SelfEllipseName.Stroke = SelfColor;
                                            SelfEllipseName.StrokeThickness = 1;
                                            SelfEllipseName.Opacity = 1;
                                            SelfEllipseName.Fill = SelfColor;
                                            SelfEllipseName.Width = NodeSize;
                                            SelfEllipseName.Height = NodeSize;
                                            Canvas.SetLeft(SelfEllipseName, X2 - (NodeSize / 2));
                                            Canvas.SetTop(SelfEllipseName, Y2 - (NodeSize / 2));
                                            SelfEllipseName.ToolTip = "总数:" + RSRP.ToString() + Environment.NewLine;

                                            SelfEllipseName.PreviewMouseLeftButtonDown += SelfEllipseName_PreviewMouseLeftButtonDown;
                                            SelfChart.Children.Add(SelfLineName);
                                            SelfChart.Children.Add(SelfEllipseName);
                                            Flag = false;
                                            break;
                                        }
                                    }

                                    if (Flag)
                                    {
                                        double X1 = XOrig;
                                        double Y1 = (ChartArea_Height - YOrig) - RSRP;
                                        double X2 = XOrig;
                                        double Y2 = (ChartArea_Height - YOrig) - ((Math.Abs(RSRP) / Y_QuadrantArrowScaleValueCount) * 10 * YShortScaleLineSpace * 10);
                                        Brush SelfColor = null;
                                        if (SelfChart.Children.Count + 1 < BlackListColors.Count)
                                        {
                                            SelfColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[SelfChart.Children.Count + 1]));
                                        }
                                        else
                                        {
                                            SelfColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[BlackListColors.Count - 1]));
                                        }

                                        //线
                                        Line SelfLineName = new Line();
                                        SelfLineName.Name = "L" + "_" + _IMSI + "_" + Second.ToString();
                                        SelfLineName.Tag = Second;
                                        SelfLineName.Stroke = SelfColor;
                                        SelfLineName.StrokeThickness = 1;
                                        SelfLineName.Opacity = 1;
                                        SelfLineName.X1 = X1;
                                        SelfLineName.Y1 = Y1;
                                        SelfLineName.X2 = X2;
                                        SelfLineName.Y2 = Y2;
                                        SelfLineName.ToolTip = "总数:" + RSRP.ToString();

                                        //结点
                                        Ellipse SelfEllipseName = new Ellipse();
                                        SelfEllipseName.Name = "E" + "_" + _IMSI + "_" + Second.ToString();
                                        SelfEllipseName.Tag = Second;
                                        SelfEllipseName.Stroke = SelfColor;
                                        SelfEllipseName.StrokeThickness = 1;
                                        SelfEllipseName.Opacity = 1;
                                        SelfEllipseName.Fill = SelfColor;
                                        SelfEllipseName.Width = NodeSize;
                                        SelfEllipseName.Height = NodeSize;
                                        Canvas.SetLeft(SelfEllipseName, X2 - (NodeSize / 2f));
                                        Canvas.SetTop(SelfEllipseName, Y2 - (NodeSize / 2f));
                                        SelfEllipseName.ToolTip = "总数:" + RSRP.ToString();

                                        SelfChart.Children.Add(SelfLineName);
                                        SelfChart.Children.Add(SelfEllipseName);
                                    }
                                }
                                else
                                {
                                    double X1 = XOrig;
                                    double Y1 = (ChartArea_Height - YOrig);
                                    double X2 = XOrig + _xShortScaleLineSpace * 10;
                                    double Y2 = (ChartArea_Height - YOrig) - ((Math.Abs(RSRP) / Y_QuadrantArrowScaleValueCount) * 10 * YShortScaleLineSpace * 10);

                                    //线
                                    Line SelfLineName = new Line();
                                    SelfLineName.Name = "L" + "_" + _IMSI + "_" + Second.ToString();
                                    SelfLineName.Tag = Second;
                                    SelfLineName.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[0]));
                                    SelfLineName.StrokeThickness = 1;
                                    SelfLineName.Opacity = 1;
                                    SelfLineName.X1 = X1;
                                    SelfLineName.Y1 = Y1;
                                    SelfLineName.X2 = X2;
                                    SelfLineName.Y2 = Y2;
                                    SelfLineName.ToolTip = "总数:" + RSRP.ToString();

                                    //结点
                                    Ellipse SelfEllipseName = new Ellipse();
                                    SelfEllipseName.Name = "E" + "_" + _IMSI + "_" + Second.ToString();
                                    SelfEllipseName.Tag = Second;
                                    SelfEllipseName.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[0]));
                                    SelfEllipseName.StrokeThickness = 1;
                                    SelfEllipseName.Opacity = 1;
                                    SelfEllipseName.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BlackListColors[0]));
                                    SelfEllipseName.Width = NodeSize;
                                    SelfEllipseName.Height = NodeSize;
                                    Canvas.SetLeft(SelfEllipseName, X2 - (NodeSize / 2f));
                                    Canvas.SetTop(SelfEllipseName, Y2 - (NodeSize / 2f));
                                    SelfEllipseName.ToolTip = "总数:" + RSRP.ToString();

                                    SelfChart.Children.Add(SelfLineName);
                                    SelfChart.Children.Add(SelfEllipseName);
                                }
                                Second++;
                            }
                        }));
                        //清理
                        MeasReportAxialChartBlackList.Rows.RemoveAt(0);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("分时统计曲线", ex.Message, ex.StackTrace);
                }
            }
        }

        //缩放曲线
        public void ResizeAxial(int Model)
        {
            SelfChart.Dispatcher.Invoke(() =>
            {
                try
                {
                    switch (Model)
                    {
                        case 0:
                            if (SelfChart.Children.Count > 0)
                            {
                                lock (ChartLock)
                                {
                                    for (int i = 0; i < SelfChart.Children.Count; i += 2)
                                    {
                                        try
                                        {
                                            double RSRP = Convert.ToDouble(((SelfChart.Children[i] as Line).ToolTip.ToString().Split(new char[] { '\n' })[3]).Split(new char[] { ':' })[1]);
                                            double X1 = XOrig;
                                            double Y1 = (ChartArea_Height - YOrig) + RSRP;
                                            double X2 = XOrig;
                                            double Y2 = (ChartArea_Height - YOrig) - (Math.Abs(RSRP) * YShortScaleLineSpace);
                                            (SelfChart.Children[i] as Line).X1 = X1;
                                            (SelfChart.Children[i] as Line).Y1 = Y1;
                                            (SelfChart.Children[i] as Line).X2 = X2;
                                            (SelfChart.Children[i] as Line).Y2 = Y2;

                                            (SelfChart.Children[i + 1] as Ellipse).SetValue(Canvas.LeftProperty, X2);
                                            (SelfChart.Children[i + 1] as Ellipse).SetValue(Canvas.TopProperty, Y2);
                                        }
                                        catch (Exception ex)
                                        {
                                            Parameters.PrintfLogsExtended("缩放曲线", ex.Message, ex.StackTrace);
                                        }
                                    }
                                }
                            }
                            break;
                        case 1:
                            lock (ChartLock)
                            {
                                for (int i = 0; i < SelfChart.Children.Count; i += 2)
                                {
                                    try
                                    {
                                        SelfChart.Children.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        Parameters.PrintfLogsExtended("缩放曲线", ex.Message, ex.StackTrace);
                                    }
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            });
        }

        //弹框信息
        private void SelfEllipseName_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                SubWindow.ChartCoordinateInfoWindow ChartCoordinateInfoWin = new SubWindow.ChartCoordinateInfoWindow();
                string[] SelfParametersList = (sender as Ellipse).ToolTip.ToString().Split(new char[] { '\n' });
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.IMSI = SelfParametersList[0].Split(new char[] { ':' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.RSRP = SelfParametersList[3].Split(new char[] { ':' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.X1 = SelfParametersList[1].Split(new char[] { ',' })[0].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.Y1 = SelfParametersList[1].Split(new char[] { ',' })[1].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.X2 = SelfParametersList[2].Split(new char[] { ',' })[0].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.Y2 = SelfParametersList[2].Split(new char[] { ',' })[1].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.XStart = SelfParametersList[4].Split(new char[] { ',' })[0].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.YStart = SelfParametersList[4].Split(new char[] { ',' })[1].Split(new char[] { '=' })[1];
                SubWindow.ChartCoordinateInfoWindow.ChartCoordinateParameters.Second = SelfParametersList[5].Split(new char[] { ':' })[1];
                ChartCoordinateInfoWin.ShowDialog();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("曲线图元素信息", ex.Message, ex.StackTrace);
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
    #endregion

    #region 柱状图类
    public class ColumnsClass : INotifyPropertyChanged
    {
        //整体高宽
        private double _columnsHeight = 520;
        private double _columnsWidth = 920;
        //坐标原点坐标
        private double _xOrig = 60;
        private double _yOrig = 20;
        //带箭头的坐标轴
        private ArrowLine rowLines;
        private ArrowLine colLines;
        //刻度数量
        private int _xScaleCount = 10;
        private int _yScaleCount = 10;
        //坐标刻度大小
        private int _xScale;
        private int _yScale;
        //X,Y总数
        private int xScaleValueCount = 10;
        private int yScaleValueCount = 100;
        //绘制坐标刻度
        private string _outLineColor = "Red";
        public List<Line> LongScaleLine = null;
        //刻度
        public List<Label> XScaleValueList = null;
        public List<Label> YScaleValueList = null;
        private double xScaleValueFontSize = 12;
        private double xScaleValueOpacity = 1;
        private string xScaleValueColor = "Yellow";
        private double yScaleValueFontSize = 12;
        private double yScaleValueOpacity = 1;
        private string yScaleValueColor = "Yellow";
        public string xUnit = "h";
        //柱状图
        public List<Line> ColumnList = null;
        public List<Label> lblCountList = null;

        //整体高宽
        public double ColumnsHeight
        {
            get { return _columnsHeight; }
            set
            {
                _columnsHeight = value;
                NotifyPropertyChanged("ColumnsHeight");
            }
        }
        public double ColumnsWidth
        {
            get { return _columnsWidth; }
            set
            {
                _columnsWidth = value;
                NotifyPropertyChanged("ColumnsWidth");
            }
        }
        //坐标原点
        public double XOrig
        {
            get { return _xOrig; }
            set
            {
                _xOrig = value;
                NotifyPropertyChanged("XOrig");
            }
        }
        public double YOrig
        {
            get { return _yOrig; }
            set
            {
                _yOrig = value;
                NotifyPropertyChanged("YOrig");
            }
        }
        //带箭头的坐标轴
        public ArrowLine RowLines
        {
            get { return rowLines; }
            set
            {
                rowLines = value;
                NotifyPropertyChanged("RowLines");
            }
        }
        public ArrowLine ColLines
        {
            get { return colLines; }
            set
            {
                colLines = value;
                NotifyPropertyChanged("ColLines");
            }
        }
        //坐标刻度数量
        public int XScaleCount
        {
            get { return _xScaleCount; }
            set
            {
                _xScaleCount = value;
                NotifyPropertyChanged("XScaleCount");
            }
        }
        public int YScaleCount
        {
            get { return _yScaleCount; }
            set
            {
                _yScaleCount = value;
                NotifyPropertyChanged("YScaleCount");
            }
        }
        //坐标刻度大小
        public int XScale
        {
            get { return _xScale; }
            set
            {
                _xScale = value;
                NotifyPropertyChanged("XScale");
            }
        }
        public int YScale
        {
            get { return _yScale; }
            set
            {
                _yScale = value;
                NotifyPropertyChanged("YScale");
            }
        }
        public int XScaleValueCount
        {
            get { return xScaleValueCount; }
            set
            {
                xScaleValueCount = value;
                NotifyPropertyChanged("XScaleValueCount");
            }
        }
        public int YScaleValueCount
        {
            get { return yScaleValueCount; }
            set
            {
                yScaleValueCount = value;
                NotifyPropertyChanged("YScaleValueCount");
            }
        }
        public double XScaleValueFontSize
        {
            get
            {
                return xScaleValueFontSize;
            }

            set
            {
                xScaleValueFontSize = value;
                NotifyPropertyChanged("XScaleValueFontSize");
            }
        }

        public double XScaleValueOpacity
        {
            get
            {
                return xScaleValueOpacity;
            }

            set
            {
                xScaleValueOpacity = value;
                NotifyPropertyChanged("XScaleValueOpacity");
            }
        }

        public string XScaleValueColor
        {
            get
            {
                return xScaleValueColor;
            }

            set
            {
                xScaleValueColor = value;
                NotifyPropertyChanged("XScaleValueColor");
            }
        }

        public double YScaleValueFontSize
        {
            get
            {
                return yScaleValueFontSize;
            }

            set
            {
                yScaleValueFontSize = value;
                NotifyPropertyChanged("YScaleValueFontSize");
            }
        }

        public double YScaleValueOpacity
        {
            get
            {
                return yScaleValueOpacity;
            }

            set
            {
                yScaleValueOpacity = value;
                NotifyPropertyChanged("YScaleValueOpacity");
            }
        }

        public string YScaleValueColor
        {
            get
            {
                return yScaleValueColor;
            }

            set
            {
                yScaleValueColor = value;
                NotifyPropertyChanged("YScaleValueColor");
            }
        }
        public string XUnit
        {
            get { return xUnit; }
            set
            {
                xUnit = value;
                NotifyPropertyChanged("XUnit");
            }
        }
        public string OutLineColor
        {
            get { return _outLineColor; }
            set
            {
                _outLineColor = value;
                NotifyPropertyChanged("OutLineColor");
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
        public ColumnsClass()
        {
            InitChart();
        }
        private void InitChart()
        {
            try
            {
                if (LongScaleLine == null)
                {
                    LongScaleLine = new List<Line>();
                }
                if (RowLines == null)
                {
                    RowLines = new ArrowLine();
                }
                if (ColLines == null)
                {
                    ColLines = new ArrowLine();
                }
                if (ColumnList == null)
                {
                    ColumnList = new List<Line>();
                }
                if (XScaleValueList == null)
                {
                    XScaleValueList = new List<Label>();
                }
                if (YScaleValueList == null)
                {
                    YScaleValueList = new List<Label>();
                }
                if (lblCountList == null)
                {
                    lblCountList = new List<Label>();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        public void ChartOutLine()
        {
            try
            {
                ColLines.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(OutLineColor));
                ColLines.StartPoint = new Point(XOrig, ColumnsHeight - YOrig);
                ColLines.EndPoint = new Point(XOrig, YOrig);
                ColLines.StrokeThickness = 1;

                RowLines.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(OutLineColor));
                RowLines.StartPoint = new Point(XOrig, ColumnsHeight - YOrig);
                RowLines.EndPoint = new Point(ColumnsWidth - XOrig, ColumnsHeight - YOrig);
                RowLines.StrokeThickness = 1;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("坐标轴", ex.Message, ex.StackTrace);
            }
        }
        public void ChartRowLine()
        {
            try
            {
                LongScaleLine.Clear();
                XScaleValueList.Clear();
                //原点坐标
                ChartXAxialScaleValue(XOrig - 2, ColumnsHeight - YOrig, "0", "Yellow", 12);
                //X坐标
                for (int i = 0; i < 10; i++)
                {
                    Line XGridLine = new Line();
                    XGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
                    XGridLine.X1 = XScale * (i + 1) + XOrig;
                    XGridLine.Y1 = ColumnsHeight - YOrig;
                    XGridLine.X2 = XScale * (i + 1) + XOrig;
                    XGridLine.Y2 = ColumnsHeight - YOrig + 10;
                    XGridLine.Opacity = 1;
                    XGridLine.StrokeThickness = 1;
                    LongScaleLine.Add(XGridLine);
                    ChartXAxialScaleValue(XGridLine.X2, XGridLine.Y2, ((i + 1) * (XScaleValueCount / XScaleCount)).ToString() + XUnit, "Yellow", 12);
                }
                //Y坐标 
                for (int i = 0; i < 10; i++)
                {
                    Line YGridLine = new Line();
                    YGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
                    YGridLine.X1 = XOrig;
                    YGridLine.Y1 = ColumnsHeight - YScale * (i + 1) - YOrig;
                    YGridLine.X2 = XOrig - 10;
                    YGridLine.Y2 = ColumnsHeight - YScale * (i + 1) - YOrig;
                    YGridLine.Opacity = 1;
                    YGridLine.StrokeThickness = 1;
                    LongScaleLine.Add(YGridLine);
                    ChartYAxialScaleValue(YGridLine.X2, YGridLine.Y2 - 10, ((i + 1) * (YScaleValueCount / YScaleCount)).ToString(), "Yellow", 12);
                }
                //虚拟刻度
                for (int i = 0; i < 10; i++)
                {
                    Line YGridLine = new Line();
                    YGridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("blue"));
                    YGridLine.X1 = ColumnsWidth - XOrig;
                    YGridLine.Y1 = ColumnsHeight - YScale * (i + 1) - YOrig;
                    YGridLine.X2 = XOrig - 10;
                    YGridLine.Y2 = ColumnsHeight - YScale * (i + 1) - YOrig;
                    YGridLine.Opacity = 0.2;
                    YGridLine.StrokeThickness = 1;
                    LongScaleLine.Add(YGridLine);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("坐标刻度", ex.Message, ex.StackTrace);
            }
        }
        //X轴刻度值标示
        public void ChartXAxialScaleValue(double X, double Y, string Value, string ValueColor, double Size)
        {
            try
            {
                if (ValueColor != null)
                {
                    XScaleValueColor = ValueColor;
                }
                if (Size > 0)
                {
                    XScaleValueFontSize = Size;
                }

                Label XAxialScaleValue = new Label();
                XAxialScaleValue.Width = 32;
                XAxialScaleValue.Height = 25;
                XAxialScaleValue.Content = Value;
                XAxialScaleValue.Opacity = XScaleValueOpacity;
                XAxialScaleValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(XScaleValueColor));
                XAxialScaleValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                XAxialScaleValue.VerticalAlignment = VerticalAlignment.Center;
                XAxialScaleValue.VerticalContentAlignment = VerticalAlignment.Center;
                XAxialScaleValue.FontSize = XScaleValueFontSize;
                Canvas.SetLeft(XAxialScaleValue, X - (XAxialScaleValue.Width / 2));
                Canvas.SetTop(XAxialScaleValue, Y + 2);
                XScaleValueList.Add(XAxialScaleValue);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("X刻度值标示", ex.Message, ex.StackTrace);
            }
        }
        //Y轴刻度值标示
        public void ChartYAxialScaleValue(double X, double Y, string Value, string ValueColor, double Size)
        {
            try
            {
                if (ValueColor != null)
                {
                    YScaleValueColor = ValueColor;
                }
                if (Size > 0)
                {
                    YScaleValueFontSize = Size;
                }

                Label YAxialScaleValue = new Label();
                YAxialScaleValue.Width = 80;
                YAxialScaleValue.Height = 25;
                YAxialScaleValue.Content = Value;
                YAxialScaleValue.Opacity = YScaleValueOpacity;
                YAxialScaleValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(YScaleValueColor));
                YAxialScaleValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                YAxialScaleValue.VerticalAlignment = VerticalAlignment.Center;
                YAxialScaleValue.VerticalContentAlignment = VerticalAlignment.Center;
                YAxialScaleValue.FontSize = YScaleValueFontSize;
                Canvas.SetLeft(YAxialScaleValue, X - (YAxialScaleValue.Width / 2));
                Canvas.SetTop(YAxialScaleValue, Y);
                XScaleValueList.Add(YAxialScaleValue);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Y刻度值标示", ex.Message, ex.StackTrace);
            }
        }
        //显示数量
        public void ChartCountValue(double X, double Y, string Value, string ValueColor, double Size)
        {
            try
            {
                if (ValueColor != null)
                {
                    YScaleValueColor = ValueColor;
                }
                if (Size > 0)
                {
                    YScaleValueFontSize = Size;
                }

                Label YAxialScaleValue = new Label();
                YAxialScaleValue.Width = 60;
                YAxialScaleValue.Height = 25;
                YAxialScaleValue.Content = Value;
                YAxialScaleValue.Opacity = YScaleValueOpacity;
                YAxialScaleValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(YScaleValueColor));
                YAxialScaleValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                YAxialScaleValue.VerticalAlignment = VerticalAlignment.Center;
                YAxialScaleValue.VerticalContentAlignment = VerticalAlignment.Center;
                YAxialScaleValue.FontSize = YScaleValueFontSize;
                Canvas.SetLeft(YAxialScaleValue, X - (YAxialScaleValue.Width / 2));
                Canvas.SetTop(YAxialScaleValue, Y + 2);
                lblCountList.Add(YAxialScaleValue);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("总数标示", ex.Message, ex.StackTrace);
            }
        }
        //柱状图
        public void ChartColumns()
        {
            try
            {
                ColumnList.Clear();
                lblCountList.Clear();
                YScaleValueCount = 0;
                List<double> TmpCount = new List<double>();
                XScale = (int)((ColumnsWidth - (XOrig * 4)) / 10);
                YScale = (int)((ColumnsHeight - (YOrig * 4)) / 10);
                for (int i = 0; i < 10; i++)
                {
                    Random rd = new Random();
                    int intRD = rd.Next(0, (int)(99999));
                    TmpCount.Add(intRD);
                    Thread.Sleep(50);
                    if (intRD > YScaleValueCount)
                        YScaleValueCount = intRD;
                }
                for (int i = 0; i < TmpCount.Count; i++)
                {
                    Line lines = new Line();
                    lines.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Blue"));
                    lines.X1 = XScale * (i + 1) + XOrig;
                    lines.Y1 = ColumnsHeight - YOrig;
                    lines.X2 = XScale * (i + 1) + XOrig;
                    lines.Y2 = ColumnsHeight - YOrig - (((TmpCount[i] * YScaleCount) / YScaleValueCount) * YScale);
                    lines.Opacity = 1;
                    lines.StrokeThickness = 20;
                    lines.Tag = TmpCount[i];
                    ColumnList.Add(lines);
                    ChartCountValue(lines.X2 + 15, lines.Y2 - 20, TmpCount[i].ToString(), "red", 12);
                    lines.MouseLeftButtonUp += Lines_MouseLeftButtonUp;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表边框", ex.Message, ex.StackTrace);
            }
        }

        private void Lines_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Line tmpline = sender as Line;
            MessageBox.Show(tmpline.Tag.ToString());
        }
    }
    #endregion

    #region 饼状图
    public class CreateChartPieClass : INotifyPropertyChanged
    {
        private List<string> strListx = new List<string>() { "非常驻", "常驻" };
        private List<string> strListy = new List<string>() { "1", "0" };
        private double chartWidth = 300;
        private double chartHeight = 580;
        public Chart chart = new Chart();
        public List<SolidColorBrush> colorlist = new List<SolidColorBrush>() { Brushes.Green, Brushes.Red };
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public List<string> StrListx
        {
            get
            {
                return strListx;
            }

            set
            {
                strListx = value;
                NotifyPropertyChanged("StrListx");
            }
        }
        public List<string> StrListy
        {
            get
            {
                return strListy;
            }

            set
            {
                strListy = value;
                NotifyPropertyChanged("StrListy");
            }
        }
        public double ChartWidth
        {
            get
            {
                return chartWidth;
            }

            set
            {
                chartWidth = value;
                NotifyPropertyChanged("ChartWidth");
            }
        }
        public double ChartHeight
        {
            get
            {
                return chartHeight;
            }

            set
            {
                chartHeight = value;
                NotifyPropertyChanged("ChartHeight");
            }
        }

        public CreateChartPieClass()
        {
            InitChart();
        }
        private void InitChart()
        {
            if (strListx == null)
            {
                strListx = new List<string>();
            }
            if (strListy == null)
            {
                strListy = new List<string>();
            }
        }
        public void drawCircle()
        {
            try
            {
                chart.Background = Brushes.Transparent;
                //设置图标的宽度和高度
                chart.Width = ChartWidth;
                chart.Height = ChartHeight;
                //chart.Margin = new Thickness(0);
                //是否启用打印和保持图片
                chart.ToolBarEnabled = false;

                //设置图标的属性
                chart.ScrollingEnabled = false;//是否启用或禁用滚动
                chart.View3D = true;//3D效果显示

                // 创建一个新的数据线。               
                DataSeries dataSeries = new DataSeries();

                // 设置数据线的格式
                dataSeries.RenderAs = RenderAs.Pie;//柱状Stacked


                // 设置数据点              
                DataPoint dataPoint;
                for (int i = 0; i < StrListx.Count; i++)
                {
                    // 创建一个数据点的实例。                   
                    dataPoint = new DataPoint();
                    //设置颜色
                    dataPoint.Color = colorlist[i];
                    // 设置X轴点                    
                    dataPoint.AxisXLabel = StrListx[i];

                    dataPoint.LegendText = "##" + StrListx[i];
                    //设置Y轴点                   
                    dataPoint.YValue = double.Parse(StrListy[i]);
                    //添加数据点                   
                    dataSeries.DataPoints.Add(dataPoint);
                }
                chart.Series.Clear();
                // 添加数据线到数据序列。                
                chart.Series.Add(dataSeries);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("绘制饼状图", ex.Message, ex.StackTrace);
            }
        }
    }
    #endregion

    #region 圆图
    public class CreateChartEllipseClass : INotifyPropertyChanged
    {
        private List<System.Windows.Shapes.Path> pth;
        private PathGeometry pathGeome;
        private List<PathFigure> figureList;
        private int chartX = 80;
        private int chartY = 80;
        private int chartR = 80;
        private List<string> pthtColors;
        private List<Line> flagLine;
        private List<Label> flagLabel;
        private List<Rectangle> flagRectangle;
        private string message;


        private LineSegment lineSeg;

        private ArcSegment arc;// = new ArcSegment(new Point(100, 50), new Size(50, 50), 0, false, SweepDirection.Clockwise, true);
        private PathFigure figure;// = new PathFigure();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }
        public List<System.Windows.Shapes.Path> Pth
        {
            get
            {
                return pth;
            }

            set
            {
                pth = value;
                NotifyPropertyChanged("Pth");
            }
        }
        public PathGeometry PathGeome
        {
            get
            {
                return pathGeome;
            }

            set
            {
                pathGeome = value;
                NotifyPropertyChanged("PathGeome");
            }
        }
        public List<PathFigure> FigureList
        {
            get
            {
                return figureList;
            }

            set
            {
                figureList = value;
                NotifyPropertyChanged("FigureList");
            }
        }
        public LineSegment LineSeg
        {
            get
            {
                return lineSeg;
            }

            set
            {
                lineSeg = value;
                NotifyPropertyChanged("LineSeg");
            }
        }
        public ArcSegment Arc
        {
            get
            {
                return arc;
            }

            set
            {
                arc = value;
                NotifyPropertyChanged("Arc");
            }
        }
        public PathFigure Figure
        {
            get
            {
                return figure;
            }

            set
            {
                figure = value;
                NotifyPropertyChanged("Figure");
            }
        }
        public int ChartX
        {
            get
            {
                return chartX;
            }

            set
            {
                chartX = value;
                NotifyPropertyChanged("ChartX");
            }
        }
        public int ChartY
        {
            get
            {
                return chartY;
            }

            set
            {
                chartY = value;
                NotifyPropertyChanged("ChartY");
            }
        }
        public int ChartR
        {
            get
            {
                return chartR;
            }

            set
            {
                chartR = value;
                NotifyPropertyChanged("ChartR");
            }
        }
        public List<string> PthtColors
        {
            get
            {
                return pthtColors;
            }

            set
            {
                pthtColors = value;
                NotifyPropertyChanged("PthtColors");
            }
        }
        public List<Line> FlagLine
        {
            get
            {
                return flagLine;
            }

            set
            {
                flagLine = value;
                NotifyPropertyChanged("FlagLine");
            }
        }
        public List<Label> FlagLabel
        {
            get
            {
                return flagLabel;
            }

            set
            {
                flagLabel = value;
                NotifyPropertyChanged("FlagLabel");
            }
        }
        public List<Rectangle> FlagRectangle
        {
            get
            {
                return flagRectangle;
            }

            set
            {
                flagRectangle = value;
                NotifyPropertyChanged("FlagRectangle");
            }
        }
        public string Message
        {
            get
            {
                return message;
            }

            set
            {
                message = value;
                NotifyPropertyChanged("Message");
            }
        }


        public CreateChartEllipseClass()
        {
            if (Pth == null)
            {
                Pth = new List<System.Windows.Shapes.Path>();
            }
            if (pathGeome == null)
            {
                pathGeome = new PathGeometry();
            }
            if (LineSeg == null)
            {
                LineSeg = new LineSegment();
            }
            if (FigureList == null)
            {
                FigureList = new List<PathFigure>();
            }
            if (FlagRectangle == null)
            {
                FlagRectangle = new List<Rectangle>();
            }
            if (PthtColors == null)
            {
                if (PthtColors == null)
                {
                    PthtColors = new List<string>();
                    PthtColors.Add("Red");
                    PthtColors.Add("White");
                    PthtColors.Add("Aqua");
                    PthtColors.Add("Yellow");
                    PthtColors.Add("Green");
                    PthtColors.Add("Blue");
                    PthtColors.Add("Brown");
                    PthtColors.Add("Teal");
                    PthtColors.Add("Coral");
                    PthtColors.Add("Crimson");
                    PthtColors.Add("DarkRed");
                    PthtColors.Add("DodgerBlue");
                    PthtColors.Add("#341A1C");
                    PthtColors.Add("#080F4C");
                    PthtColors.Add("Orchid");
                    PthtColors.Add("#FF7200");
                }
            }
            if (FlagLine == null)
            {
                FlagLine = new List<Line>();
            }
            if (FlagLabel == null)
            {
                FlagLabel = new List<Label>();
            }
        }

        public void CreateChart(List<int> CountList)
        {
            try
            {
                int CountTotal = 0;
                Point point = new Point();
                double X = ChartR;
                double Y = 0;
                double Q = 0.00;
                for (int i = 0; i < CountList.Count; i++)
                {
                    CountTotal += CountList[i];
                }
                for (int i = 0; i < CountList.Count; i++)
                {
                    PathFigure tmpFigure = new PathFigure();
                    LineSegment tmpLineSeg = new LineSegment();
                    ArcSegment tmpArc;
                    point.X = X;
                    point.Y = Y;
                    Q += (double)CountList[i] / (double)CountTotal;

                    if (CountList[i] == CountTotal)
                    {
                        Y = ChartY + Math.Abs(Math.Cos(2 * Math.PI * (Q - 0.00001))) * ChartR * (-1);
                        X = ChartX + Math.Sin(2 * Math.PI * (Q - 0.00001)) * ChartR;
                    }
                    else if (CountList[i] == 0)
                    {
                        Y = ChartY + Math.Abs(Math.Cos(2 * Math.PI * Q)) * ChartR * (-1);
                        X = ChartX + Math.Sin(2 * Math.PI * Q) * ChartR;
                    }
                    else if (Q >= 0.25 && Q < 0.75)
                    {
                        Y = ChartY + Math.Abs(Math.Cos(2 * Math.PI * Q)) * ChartR;
                        X = ChartX + Math.Sin(2 * Math.PI * Q) * ChartR;
                    }
                    else
                    {
                        Y = ChartY + Math.Abs(Math.Cos(2 * Math.PI * Q)) * ChartR * (-1);
                        X = ChartX + Math.Sin(2 * Math.PI * Q) * ChartR;
                    }
                    if (CountList[i] == CountTotal)
                    {
                        tmpArc = new ArcSegment(new Point(X, Y), new Size(ChartR, ChartR), 0, true, SweepDirection.Clockwise, true);
                    }
                    else if (CountList[i] == 0)
                    {
                        tmpArc = new ArcSegment(new Point(X, Y), new Size(ChartR, ChartR), 0, false, SweepDirection.Clockwise, true);
                    }
                    else if ((double)CountList[i] / (double)CountTotal > 0.5)
                    {
                        tmpArc = new ArcSegment(new Point(X, Y), new Size(ChartR, ChartR), 0, true, SweepDirection.Clockwise, true);
                    }
                    else
                    {
                        tmpArc = new ArcSegment(new Point(X, Y), new Size(ChartR, ChartR), 0, false, SweepDirection.Clockwise, true);
                    }

                    tmpLineSeg.Point = new Point(ChartX, ChartX);

                    tmpFigure.StartPoint = new Point(point.X, point.Y);
                    tmpFigure.Segments.Add(tmpArc);
                    tmpFigure.Segments.Add(tmpLineSeg);
                    tmpFigure.IsClosed = true;

                    FigureList.Add(tmpFigure);
                    PathGeome.Figures.Add(tmpFigure);
                }
                for (int i = 0; i < FigureList.Count; i++)
                {
                    PathGeometry tmpPathGeometry = new PathGeometry();
                    System.Windows.Shapes.Path tmpPth = new System.Windows.Shapes.Path();
                    tmpPathGeometry.Figures.Add(FigureList[i]);
                    tmpPth.Data = tmpPathGeometry;
                    //tmpPth.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("yellow"));
                    Canvas.SetLeft(tmpPth, 80);
                    Canvas.SetTop(tmpPth, 200);
                    tmpPth.StrokeThickness = 1;
                    tmpPth.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(PthtColors[i]));
                    pth.Add(tmpPth);
                    string LabelName = string.Empty;
                    if (i > 0)
                        LabelName = "非常驻人口";
                    else
                        LabelName = "常驻人口";
                    LabelName += CountList[i].ToString() + "(占总人数的" + ((double)CountList[i] / (double)CountTotal).ToString("P") + ")";
                    Message = LabelName;
                    tmpPth.ToolTip = LabelName;
                    //CreatLineAndLabel((150 * i + 10), 400, (150 * i + 40), 400, PthtColors[i], LabelName);
                    CreatLineAndLabel(120, 30 * i + 400, PthtColors[i], LabelName);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("创建圆图比例", ex.Message, ex.StackTrace);
            }
        }
        private void CreatLineAndLabel(Double X1, Double Y1, String ColorLine, String LabelName)
        {
            try
            {
                double RectangleWidth = 40;
                double RectangleHeight = 10;
                double LineWidth = 20;
                //线
                Line gridLine1 = new Line();
                gridLine1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorLine));
                gridLine1.X1 = X1;
                gridLine1.Y1 = Y1;
                gridLine1.Opacity = 1;
                gridLine1.X2 = X1 + LineWidth;
                gridLine1.Y2 = Y1;
                gridLine1.StrokeThickness = 1;

                Line gridLine2 = new Line();
                gridLine2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorLine));
                gridLine2.X1 = X1 + LineWidth + RectangleWidth;
                gridLine2.Y1 = Y1;
                gridLine2.Opacity = 1;
                gridLine2.X2 = X1 + LineWidth * 2 + RectangleWidth;
                gridLine2.Y2 = Y1;
                gridLine2.StrokeThickness = 1;
                FlagLine.Add(gridLine1);
                FlagLine.Add(gridLine2);

                //label
                Label LabelValue = new Label();
                LabelValue.Width = LabelName.Length * 8 + 10;
                LabelValue.Height = 25;
                LabelValue.Content = LabelName;
                LabelValue.Opacity = 1;
                LabelValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
                //LabelValue.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("TransParent"));
                LabelValue.VerticalAlignment = VerticalAlignment.Center;
                LabelValue.VerticalContentAlignment = VerticalAlignment.Center;
                LabelValue.FontSize = 10;
                Canvas.SetLeft(LabelValue, X1 - (8 * LabelName.Length - RectangleWidth - 2 * LineWidth) / 2);
                Canvas.SetTop(LabelValue, Y1);
                FlagLabel.Add(LabelValue);

                //矩形
                Rectangle tmpRectangle = new Rectangle();
                tmpRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ColorLine));
                tmpRectangle.Width = RectangleWidth;
                tmpRectangle.Height = RectangleHeight;
                tmpRectangle.StrokeThickness = 1;
                tmpRectangle.SetValue(Canvas.LeftProperty, X1 + LineWidth);
                tmpRectangle.SetValue(Canvas.TopProperty, Y1 - 5);
                FlagRectangle.Add(tmpRectangle);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("创建圆图描述", ex.Message, ex.StackTrace);
            }
        }
    }
    #endregion

    /// <summary>
    /// ShowDBMapWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticalAnalysisWindow : Window
    {
        #region 柱状图
        private ColumnsClass Columns = new ColumnsClass();
        #endregion
        private StatisticalAnalysisClass StatisticalAnalysis = new StatisticalAnalysisClass();
        public static IList<CheckBoxTreeModel> UsrdomainData = new List<CheckBoxTreeModel>();
        //饼状图
        public CreateChartPieClass CreateChartPie = new CreateChartPieClass();

        public CreateChartEllipseClass CreateChartEllipse = new CreateChartEllipseClass();
        //分时统计
        private Thread StatisticalAnalysisThread = null;
        public static ObservableCollection<MeasReportBlackListClass> SelfMeasReportBlackList = new ObservableCollection<MeasReportBlackListClass>();

        //常驻人口
        private static ObservableCollection<ResidentIMSIClass> ResidentIMSIList = new ObservableCollection<ResidentIMSIClass>();

        //碰撞分析
        public static List<ConditionsClass> ConditionsList = new List<ConditionsClass>();
        private static ObservableCollection<ConditionsClass> ConditionsIMSIList = new ObservableCollection<ConditionsClass>();

        //伴随分析
        private static ObservableCollection<AccompanyClass> AccompanyIMSIList = new ObservableCollection<AccompanyClass>();

        //选中的设备列表
        private List<string> deviceFullPathNames = new List<string>();
        public StatisticalAnalysisWindow()
        {
            InitializeComponent();

            if (StatisticalAnalysisThread == null)
            {
                //StatisticalAnalysisThread = new Thread(new ThreadStart(StatisticalAnalysisFunc));
                //StatisticalAnalysisThread.Start();
            }
            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    bool tabSelectindexbol = true;
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("分时统计"))
                    {
                        //分时统计
                        tiStatisticsInfolist.Visibility = System.Windows.Visibility.Collapsed;
                        tabSelectindexbol = true;
                        tabControl.SelectedIndex = 1;
                    }
                    else
                    {
                        tabControl.SelectedIndex = 0;
                        tabSelectindexbol = false;
                    }
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("常驻人口分析"))
                    {
                        //常驻人口分析
                        tiresidentInfolist.Visibility = System.Windows.Visibility.Collapsed;
                        if (tabSelectindexbol)
                            tabControl.SelectedIndex = 2;
                    }
                    else
                    {
                        if (tabSelectindexbol)
                            tabControl.SelectedIndex = 1;
                        tabSelectindexbol = false;
                    }
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("碰撞分析"))
                    {
                        //碰撞分析
                        ticollisionInfolist.Visibility = System.Windows.Visibility.Collapsed;
                        if (tabSelectindexbol)
                            tabControl.SelectedIndex = 3;
                    }
                    else
                    {
                        if (tabSelectindexbol)
                            tabControl.SelectedIndex = 2;
                        tabSelectindexbol = false;
                    }
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("伴随分析"))
                    {
                        //伴随分析
                        tiaccompanyInfolist.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        if (tabSelectindexbol)
                            tabControl.SelectedIndex = 3;
                        tabSelectindexbol = false;
                    }
                }
            }
            #endregion

        }
        public void LoadDeviceListTreeView()
        {
            new Thread(() =>
            {
                BindCheckBoxTreeView devicetreeview = new BindCheckBoxTreeView();
                CheckBoxTreeModel treeModel = new CheckBoxTreeModel();
                devicetreeview.Dt = JsonInterFace.BindTreeViewClass.DeviceTreeTable;
                devicetreeview.DeviceTreeViewBind(ref treeModel);
                UsrdomainData.Clear();
                UsrdomainData.Add(treeModel);
            }).Start();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FrmShowDBMapWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        private void chkTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string FullName = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).FullName;
                string IsStation = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsStation;
                string SelfNodeType = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).SelfNodeType;
                Boolean NodeChecked = ((CheckBoxTreeModel)(sender as CheckBox).DataContext).IsChecked;
                if (NodeChecked)
                {
                    //设备
                    if (SelfNodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        deviceFullPathNames.Add(FullName);
                    }
                    //域
                    else
                    {
                        CheckBoxTreeModel _CheckBoxTreeModel = new CheckBoxTreeModel();
                        _CheckBoxTreeModel = (CheckBoxTreeModel)(sender as CheckBox).DataContext;
                        _CheckBoxTreeModel.IsChecked = false;
                        //站点
                        if (IsStation == "1")
                        {
                            for (int j = 0; j < _CheckBoxTreeModel.Children.Count; j++)
                            {
                                CheckBoxTreeModel tmpTreeModel = new CheckBoxTreeModel();
                                tmpTreeModel = _CheckBoxTreeModel.Children[j];
                                for (int i = 0; i < deviceFullPathNames.Count; i++)
                                {
                                    if (deviceFullPathNames[i] == tmpTreeModel.FullName)
                                    {
                                        deviceFullPathNames.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    //设备
                    if (SelfNodeType.Equals(NodeType.LeafNode.ToString()))
                    {
                        for (int i = 0; i < deviceFullPathNames.Count; i++)
                        {
                            if (deviceFullPathNames[i] == FullName)
                            {
                                deviceFullPathNames.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    //域
                    //else
                    //{
                    //    CheckBoxTreeModel _CheckBoxTreeModel = new CheckBoxTreeModel();
                    //    _CheckBoxTreeModel = (CheckBoxTreeModel)(sender as CheckBox).DataContext;
                    //    _CheckBoxTreeModel.IsChecked = false;
                    //    //站点
                    //    if (IsStation == "1")
                    //    {
                    //        for (int j = 0; j < _CheckBoxTreeModel.Children.Count; j++)
                    //        {
                    //            CheckBoxTreeModel tmpTreeModel = new CheckBoxTreeModel();
                    //            tmpTreeModel = _CheckBoxTreeModel.Children[j];
                    //            for (int i = 0; i < deviceFullPathNames.Count; i++)
                    //            {
                    //                if (deviceFullPathNames[i] == tmpTreeModel.FullName)
                    //                {
                    //                    deviceFullPathNames.RemoveAt(i);
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("设备选择", ex.Message, ex.StackTrace);
            }
        }

        private void FrmShowDBMapWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tiStatisticsInfolist.IsEnabled = true;
                tiresidentInfolist.IsEnabled = true;
                ticollisionInfolist.IsEnabled = true;
                tiaccompanyInfolist.IsEnabled = true;
                tvSpecialListDeviceTree.ItemsSource = UsrdomainData;
                ChartArea.DataContext = StatisticalAnalysis;
                StatisticalAnalysis.MoveLineControl = 0;
                StatisticalAnalysis.Second = 0;
                StatisticalAnalysis.SettingChartHandle(ref ChartArea);
                StartTime.SelectedDate = DateTime.Now.Date;
                ChartDrawingBase(858, 490);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("统计分析初始化", ex.Message, ex.StackTrace);
            }
        }
        //图表基本参数
        private void ChartDrawingBase(double ChartWidth, double ChartHeight)
        {
            try
            {
                if (ChartBackGround.Children.Count > 0)
                {
                    ChartBackGround.Children.RemoveRange(1, ChartBackGround.Children.Count - 1);
                }

                if (CoordinateGraphs.Children.Count > 0)
                {
                    CoordinateGraphs.Children.RemoveRange(1, CoordinateGraphs.Children.Count - 1);
                }

                //区域大小
                StatisticalAnalysis.ChartArea_Width = ChartWidth;
                StatisticalAnalysis.ChartArea_Height = ChartHeight;

                //背景色
                StatisticalAnalysis.ChartBackGround = "#FF092A19";

                //网格色
                StatisticalAnalysis.GridLineColor = "Green";
                StatisticalAnalysis.GridEnable = true;

                //边框
                StatisticalAnalysis.OutLineColor = "DodgerBlue";
                StatisticalAnalysis.ChartOutLine();
                ChartBackGround.Children.Add(StatisticalAnalysis.OutLine);

                //坐标系
                StatisticalAnalysis.XQuadrantArrow();
                StatisticalAnalysis.YQuadrantArrow();
                StatisticalAnalysis.X_QuadrantArrowColor = "Aqua";
                StatisticalAnalysis.Y_QuadrantArrowColor = "Orange";
                StatisticalAnalysis.X_QuadrantArrowLineSize = 3;
                StatisticalAnalysis.Y_QuadrantArrowLineSize = 3;
                CoordinateGraphs.Children.Add(StatisticalAnalysis.X_QuadrantArrow);
                CoordinateGraphs.Children.Add(StatisticalAnalysis.Y_QuadrantArrow);

                //背景网格线
                StatisticalAnalysis.BackGroundGridLineColor = "Green";
                StatisticalAnalysis.BackGroundGridLineOpacity = 0.2;
                StatisticalAnalysis.BackGroundGridLineSize = 1;
                StatisticalAnalysis.BackGroundGridLineSpace = 20;
                StatisticalAnalysis.ChartBackGroundGridLine();
                for (int i = 0; i < StatisticalAnalysis.BackGroundGridLine.Count; i++)
                {
                    ChartBackGround.Children.Add(StatisticalAnalysis.BackGroundGridLine[i]);
                }

                //短刻度标示线
                StatisticalAnalysis.GridLineColor = "Aqua";
                StatisticalAnalysis.GridLineOpacity = 1;
                StatisticalAnalysis.GridLineSize = 1;
                StatisticalAnalysis.X_AxialScaleLineLen = 5;
                StatisticalAnalysis.Y_AxialScaleLineLen = 5;
                StatisticalAnalysis.ChartShortScaleLine();
                for (int i = 0; i < StatisticalAnalysis.GridLine.Count; i++)
                {
                    CoordinateGraphs.Children.Add(StatisticalAnalysis.GridLine[i]);
                }

                //长刻度标示线
                StatisticalAnalysis.LongGridLineColor = "Yellow";
                StatisticalAnalysis.LongGridLineOpacity = 1;
                StatisticalAnalysis.LongGridLineSize = 2;
                StatisticalAnalysis.X_LongAxialScaleLineLen = 15;
                StatisticalAnalysis.Y_LongAxialScaleLineLen = 15;
                StatisticalAnalysis.ChartLongScaleLine();
                for (int i = 0; i < StatisticalAnalysis.LongScaleLine.Count; i++)
                {
                    //线
                    CoordinateGraphs.Children.Add(StatisticalAnalysis.LongScaleLine[i]);
                }

                //X轴长刻度值
                for (int i = 0; i < StatisticalAnalysis.XAxialScaleValueList.Count; i++)
                {
                    //值
                    CoordinateGraphs.Children.Add(StatisticalAnalysis.XAxialScaleValueList[i]);
                }

                //Y轴长刻度值
                for (int i = 0; i < StatisticalAnalysis.YAxialScaleValueList.Count; i++)
                {
                    //值
                    CoordinateGraphs.Children.Add(StatisticalAnalysis.YAxialScaleValueList[i]);
                }

                //曲线存在跟随改变大小
                StatisticalAnalysis.ResizeAxial(1);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表基本参数", ex.Message, ex.StackTrace);
            }
        }
        private void FrmShowDBMapWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void FrmShowDBMapWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }

        //获取图表IMSI信息
        private void StatisticalAnalysisFunc()
        {
            DataTable BListTab = null;
            //while (true)
            {
                //StatisticalAnalysis.Input("1","4", "3");
                //for (int i = 0; i < 23; i++)
                //{
                //    Random rd = new Random();
                //    string rw1 = rd.Next(0, 4).ToString();
                //    string rw2 = rd.Next(0, 4).ToString();
                //    StatisticalAnalysis.Input((i+2).ToString(),rw1, rw2);
                //    Thread.Sleep(10);
                //}
                //break;
                if (JsonInterFace.Statistical.ImsiCountDT.Rows != null)
                {
                    if (JsonInterFace.Statistical.ImsiCountDT.Rows.Count > 0)
                    {
                        try
                        {
                            lock (JsonInterFace.Statistical.Imsi_DbHelper)
                            {
                                BListTab = JsonInterFace.Statistical.ImsiCountDT.Copy();
                            }

                            for (int i = 0; i < BListTab.Rows.Count; i++)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    StatisticalAnalysis.Input(BListTab.Rows[i][0].ToString(), BListTab.Rows[i][1].ToString(), BListTab.Rows[i][2].ToString());
                                });
                                //清理
                                for (int j = 0; j < JsonInterFace.Statistical.ImsiCountDT.Rows.Count; j++)
                                {
                                    if (BListTab.Rows[i][0].ToString() == JsonInterFace.Statistical.ImsiCountDT.Rows[j]["ID"].ToString())
                                    {
                                        JsonInterFace.Statistical.ImsiCountDT.Rows.RemoveAt(j);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }
                }
                //Thread.Sleep(1000);
            }
        }

        private void FrmShowDBMapWindow_Activated(object sender, EventArgs e)
        {
            WindowInteropHelper GetWindowHandleHelper = new WindowInteropHelper(this);
            Parameters.StatisticalWinHandle = GetWindowHandleHelper.Handle;
        }

        private void FrmShowDBMapWindow_Closed(object sender, EventArgs e)
        {

        }
        private void ckbRmDupFlag_Checked(object sender, RoutedEventArgs e)
        {
            //StatisticalAnalysis.RmDupFlag = (bool)ckbRmDupFlag.IsChecked;
            //showStatistical();
        }
        private void ckbRmDupFlag_Click(object sender, RoutedEventArgs e)
        {
            StatisticalAnalysis.RmDupFlag = (bool)ckbRmDupFlag.IsChecked;
            showStatistical();
        }
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dt = (DateTime)StartTime.SelectedDate;
                string timeStart = string.Empty;
                string timeEnded = string.Empty;
                string deviceCount = string.Empty;
                string bwFlag = string.Empty;
                string operators = string.Empty;
                if (deviceFullPathNames.Count <= 0)
                {
                    MessageBox.Show("请先选择设备列表");
                    return;
                }
                if (cbAnalysisFlag.SelectedIndex == -1 || StartTime.Text == "")
                {
                    MessageBox.Show("参数有误，请重新输入");
                    return;
                }
                //清空缓存
                JsonInterFace.Statistical.ImsiCountDT.Clear();
                //tiStatisticsInfolist.IsEnabled = false;
                tiresidentInfolist.IsEnabled = false;
                ticollisionInfolist.IsEnabled = false;
                tiaccompanyInfolist.IsEnabled = false;
                lblWaitFlag.Visibility = Visibility;
                btnSelect.IsEnabled = false;
                StatisticalAnalysis.DellAll();
                StatisticalAnalysis.SelfChart.Children.Clear();
                if (cbAnalysisFlag.SelectedIndex == 0)
                {
                    StatisticalAnalysis.XShortScaleLineSpace = 6;
                    StatisticalAnalysis.X_QuadrantArrowScaleValueCount = 12;
                    StatisticalAnalysis.XUnit = "(月)";
                }
                else if (cbAnalysisFlag.SelectedIndex == 1)
                {
                    StatisticalAnalysis.XShortScaleLineSpace = 7;
                    StatisticalAnalysis.X_QuadrantArrowScaleValueCount = 10;
                    StatisticalAnalysis.XUnit = "(周)";
                }
                else if (cbAnalysisFlag.SelectedIndex == 2)
                {
                    StatisticalAnalysis.XShortScaleLineSpace = 10;
                    StatisticalAnalysis.X_QuadrantArrowScaleValueCount = 7;
                    StatisticalAnalysis.XUnit = "(日)";
                }
                else if (cbAnalysisFlag.SelectedIndex == 3)
                {
                    StatisticalAnalysis.XShortScaleLineSpace = 3;
                    StatisticalAnalysis.X_QuadrantArrowScaleValueCount = 24;
                    StatisticalAnalysis.XUnit = "(H)";
                }
                if ((bool)ckbRmDupFlag.IsChecked)
                {
                    StatisticalAnalysis.RmDupFlag = true;
                }
                else
                {
                    StatisticalAnalysis.RmDupFlag = false;
                }
                JsonInterFace.Statistical.ImsiTotalRmDupMax = 0;
                JsonInterFace.Statistical.ImsiTotalMax = 0;
                JsonInterFace.Statistical.ImsiRow = 0;
                JsonInterFace.Statistical.ImsiRowTotal = StatisticalAnalysis.X_QuadrantArrowScaleValueCount;
                timeStart = dt.ToString("yyyy-MM-dd HH:mm:ss");
                //deviceFullPathNames.Add("设备.中国.广东省.深圳市.南山区高新园.博威通.小会议室.电信捕号");
                deviceCount = deviceFullPathNames.Count.ToString();
                bwFlag = ((ComboBoxItem)cbBwFlag.Items[cbBwFlag.SelectedIndex]).Tag.ToString();
                operators = ((ComboBoxItem)cbOperator.Items[cbOperator.SelectedIndex]).Tag.ToString();
                //#region 循环请求次数
                //for (int i = 0; i < StatisticalAnalysis.X_QuadrantArrowScaleValueCount; i++)
                //{
                //    DateTime tmpdt = new DateTime();
                //    switch (StatisticalAnalysis.X_QuadrantArrowScaleValueCount)
                //    {
                //        case 24:
                //            tmpdt = dt.AddHours(i + 1);
                //            break;
                //        case 12:
                //            tmpdt = dt.AddMonths(i + 1);
                //            break;
                //        case 10:
                //            tmpdt = dt.AddDays((i + 1) * 7);
                //            break;
                //        case 7:
                //            tmpdt = dt.AddDays(i + 1);
                //            break;
                //        default:
                //            tmpdt = dt.AddHours(i + 1);
                //            break;
                //    }
                //    timeEnded = tmpdt.ToString("yyyy-MM-dd HH:mm:ss");
                //    if (NetController.NetWorkClient.ControllerServer.Connected)
                //    {
                //        NetWorkClient.ControllerServer.Send(JsonInterFace.Get_statistics_Request(timeStart, timeEnded, deviceCount, deviceFullPathNames, bwFlag, operators));
                //        timeStart = timeEnded;
                //    }
                //    else
                //    {
                //        MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    }
                //}
                //#endregion
                //提交
                new Thread(() =>
                {
                    #region 循环请求次数
                    for (int i = 0; i < StatisticalAnalysis.X_QuadrantArrowScaleValueCount; i++)
                    {
                        DateTime tmpdt = new DateTime();
                        int exitTime = 0;
                        bool exitFlag = false;//超时标志
                        switch (StatisticalAnalysis.X_QuadrantArrowScaleValueCount)
                        {
                            case 24:
                                tmpdt = dt.AddHours(i + 1);
                                break;
                            case 12:
                                tmpdt = dt.AddMonths(i + 1);
                                break;
                            case 10:
                                tmpdt = dt.AddDays((i + 1) * 7);
                                break;
                            case 7:
                                tmpdt = dt.AddDays(i + 1);
                                break;
                            default:
                                tmpdt = dt.AddHours(i + 1);
                                break;
                        }
                        timeEnded = tmpdt.ToString("yyyy-MM-dd HH:mm:ss");
                        if (NetController.NetWorkClient.ControllerServer.Connected)
                        {
                            NetWorkClient.ControllerServer.Send(JsonInterFace.Get_statistics_Request(timeStart, timeEnded, deviceCount, deviceFullPathNames, bwFlag, operators));
                            timeStart = timeEnded;
                            while (true)
                            {
                                exitTime++;
                                Thread.Sleep(1);
                                if (JsonInterFace.Statistical.ImsiRow >= (i + 1))
                                {
                                    break;
                                }
                                if (i >= (60 * 1000))  //等待一分钟后没有返回数据退出
                                {
                                    exitFlag = true;
                                    MessageBox.Show("分时统计接收数据超时(1分钟)", "提示", MessageBoxButton.OK);
                                    break;
                                }
                            }
                            if (exitFlag)
                            {
                                //清空缓存
                                JsonInterFace.Statistical.ImsiCountDT.Clear();
                                btnSelect.IsEnabled = true;
                                tiStatisticsInfolist.IsEnabled = true;
                                tiresidentInfolist.IsEnabled = true;
                                ticollisionInfolist.IsEnabled = true;
                                tiaccompanyInfolist.IsEnabled = true;
                                lblWaitFlag.Visibility = Visibility.Collapsed;
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    #endregion
                }).Start();

                ////启动线程
                //if (StatisticalAnalysis1Thread.ThreadState == ThreadState.Suspended)
                //    StatisticalAnalysis1Thread.Resume();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("分时或实时查询", ex.Message, ex.StackTrace);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {
                //分时显示
                if (msg == Parameters.WM_StatisticalResponse)
                {
                    btnSelect.IsEnabled = true;
                    tiStatisticsInfolist.IsEnabled = true;
                    tiresidentInfolist.IsEnabled = true;
                    ticollisionInfolist.IsEnabled = true;
                    tiaccompanyInfolist.IsEnabled = true;
                    lblWaitFlag.Visibility = Visibility.Collapsed;
                    StatisticalAnalysisFunc();
                    showStatistical();
                }
                //常驻人口
                else if (msg == Parameters.WM_ResidentIMSIResponse)
                {
                    tiStatisticsInfolist.IsEnabled = true;
                    tiresidentInfolist.IsEnabled = true;
                    ticollisionInfolist.IsEnabled = true;
                    tiaccompanyInfolist.IsEnabled = true;
                    lblResidentWaitFlag.Visibility = Visibility.Collapsed;
                    ResidentIMSIFunc();
                    //CircleDrawingCanvas(CreateChartPie.ChartWidth, CreateChartPie.ChartHeight);
                    CreateChartEllipseCanvas();
                }
                //碰撞
                else if (msg == Parameters.WM_ConditionsIMSIResponse)
                {
                    tiStatisticsInfolist.IsEnabled = true;
                    tiresidentInfolist.IsEnabled = true;
                    ticollisionInfolist.IsEnabled = true;
                    tiaccompanyInfolist.IsEnabled = true;
                    lblCollisionWaitFlag.Visibility = Visibility.Collapsed;
                    ConditionsIMSIFunc();
                }
                //伴随
                else if (msg == Parameters.WM_AccompanyIMSIResponse)
                {
                    tiStatisticsInfolist.IsEnabled = true;
                    tiresidentInfolist.IsEnabled = true;
                    ticollisionInfolist.IsEnabled = true;
                    tiaccompanyInfolist.IsEnabled = true;
                    lblAccompanyWaitFlag.Visibility = Visibility.Collapsed;
                    AccompanyIMSIFunc();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("Windows 消息事件：", ex.Message, ex.StackTrace);
            }
            return hwnd;
        }
        public void showStatistical()
        {
            try
            {
                if ((bool)ckbRmDupFlag.IsChecked)
                {
                    StatisticalAnalysis.Y_QuadrantArrowScaleValueCount = JsonInterFace.Statistical.ImsiTotalRmDupMax;
                }
                else
                {
                    StatisticalAnalysis.Y_QuadrantArrowScaleValueCount = JsonInterFace.Statistical.ImsiTotalMax;
                }
                if (StatisticalAnalysis.Y_QuadrantArrowScaleValueCount <= 120)
                {
                    StatisticalAnalysis.Y_QuadrantArrowScaleValueCount = StatisticalAnalysis.Y_QuadrantArrowScaleValueCount * 10;
                }
                ChartDrawingBase(858, 490);
                StatisticalAnalysis.DrawingCanvas();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("分时显示", ex.Message, ex.StackTrace);
            }
        }
        public void ResidentIMSIFunc()
        {
            for (int i = 0; i < JsonInterFace.ResidentIMSI.ResidentIMSIDT.Rows.Count; i++)
            {
                DataRow dr = JsonInterFace.ResidentIMSI.ResidentIMSIDT.Rows[i];
                Dispatcher.Invoke(() =>
                {
                    if (i == 0)
                    {
                        ResidentIMSIList.Clear();
                    }
                    ResidentIMSIList.Add(new ResidentIMSIClass()
                    {
                        ID = dr["Id"].ToString(),
                        IMSI = dr["IMSI"].ToString()
                    });
                });
            }
            JsonInterFace.ResidentIMSI.ResidentIMSIDT.Clear();
        }
        public void ConditionsIMSIFunc()
        {
            for (int i = 0; i < JsonInterFace.ConditionsIMSI.ResidentIMSIDT.Rows.Count; i++)
            {
                DataRow dr = JsonInterFace.ConditionsIMSI.ResidentIMSIDT.Rows[i];
                Dispatcher.Invoke(() =>
                {
                    if (i == 0)
                    {
                        ConditionsIMSIList.Clear();
                    }
                    ConditionsIMSIList.Add(new ConditionsClass()
                    {
                        ID = dr["Id"].ToString(),
                        IMSI = dr["IMSI"].ToString()
                    });
                });
            }
            JsonInterFace.ConditionsIMSI.ResidentIMSIDT.Clear();
        }
        public void AccompanyIMSIFunc()
        {
            try
            {
                AccompanyIMSIList.Clear();
                for (int i = 0; i < JsonInterFace.AccompanyIMSI.AccompanyIMSIDT.Rows.Count; i++)
                {
                    DataRow dr = JsonInterFace.AccompanyIMSI.AccompanyIMSIDT.Rows[i];
                    Dispatcher.Invoke(() =>
                    {
                        if (i == 0)
                        {
                            AccompanyIMSIList.Clear();
                        }
                        AccompanyIMSIList.Add(new AccompanyClass()
                        {
                            ID = dr["Id"].ToString(),
                            IMSI = dr["IMSI"].ToString()
                        });
                    });
                }
                JsonInterFace.AccompanyIMSI.AccompanyIMSIDT.Clear();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("伴随分析", ex.Message, ex.StackTrace);
            }
        }
        /// <summary>
        /// 加载饼状图
        /// </summary>
        /// <param name="ChartWidth"></param>
        /// <param name="ChartHeight"></param>
        private void CircleDrawingCanvas(double ChartWidth, double ChartHeight)
        {
            try
            {
                if (ChartCircle.Children.Count > 0)
                {
                    ChartCircle.Children.Clear();
                }
                if (JsonInterFace.ResidentIMSI.IMSITotal > 0)
                {
                    CreateChartPie.StrListy.Clear();
                    CreateChartPie.StrListy.Add((JsonInterFace.ResidentIMSI.IMSITotal - JsonInterFace.ResidentIMSI.IMSICount).ToString());
                    CreateChartPie.StrListy.Add(JsonInterFace.ResidentIMSI.IMSICount.ToString());
                    CreateChartPie.ChartHeight = ChartHeight;
                    CreateChartPie.ChartWidth = ChartWidth;
                    CreateChartPie.drawCircle();
                    ChartCircle.Children.Add(CreateChartPie.chart);
                }
                else
                {
                    MessageBox.Show("无常驻人口查询数据", "提示", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表基本参数", ex.Message, ex.StackTrace);
            }
        }

        private void CreateChartEllipseCanvas()
        {
            try
            {
                if (ChartCircleEll.Children.Count > 0)
                {
                    ChartCircleEll.Children.Clear();
                }
                if (JsonInterFace.ResidentIMSI.IMSITotal > 0)
                {
                    List<int> temd = new List<int> { JsonInterFace.ResidentIMSI.IMSICount, JsonInterFace.ResidentIMSI.IMSITotal - JsonInterFace.ResidentIMSI.IMSICount };
                    if (JsonInterFace.ResidentIMSI.IMSICount > JsonInterFace.ResidentIMSI.IMSITotal)
                    {
                        MessageBox.Show("常驻人口数据查询有误", "提示", MessageBoxButton.OK);
                        return;
                    }
                    CreateChartEllipse.CreateChart(temd);
                    for (int i = 0; i < CreateChartEllipse.Pth.Count; i++)
                    {
                        ChartCircleEll.Children.Add(CreateChartEllipse.Pth[i]);
                        ChartCircleEll.Children.Add(CreateChartEllipse.FlagLabel[i]);
                        ChartCircleEll.Children.Add(CreateChartEllipse.FlagRectangle[i]);
                    }
                    for (int i = 0; i < CreateChartEllipse.FlagLine.Count; i++)
                    {
                        ChartCircleEll.Children.Add(CreateChartEllipse.FlagLine[i]);
                    }
                }
                else
                {
                    MessageBox.Show("无常驻人口查询数据", "提示", MessageBoxButton.OK);
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表基本参数", ex.Message, ex.StackTrace);
            }
        }

        private void textbox_hour_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                switch (tb.Name)
                {
                    case "textbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textbox_minute.Background = this.Background;
                        this.textbox_second.Background = this.Background;
                        break;
                    case "textbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textbox_hour.Background = this.Background;
                        this.textbox_second.Background = this.Background;
                        break;
                    case "textbox_second":
                        tb.Background = Brushes.Gray;
                        this.textbox_hour.Background = this.Background;
                        this.textbox_minute.Background = this.Background;
                        break;
                    case "textendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textendbox_minute.Background = this.Background;
                        this.textendbox_second.Background = this.Background;
                        break;
                    case "textendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textendbox_hour.Background = this.Background;
                        this.textendbox_second.Background = this.Background;
                        break;
                    case "textendbox_second":
                        tb.Background = Brushes.Gray;
                        this.textendbox_hour.Background = this.Background;
                        this.textendbox_minute.Background = this.Background;
                        break;
                    case "textCollisionbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textCollisionbox_minute.Background = this.Background;
                        this.textCollisionbox_second.Background = this.Background;
                        break;
                    case "textCollisionbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textCollisionbox_hour.Background = this.Background;
                        this.textCollisionbox_second.Background = this.Background;
                        break;
                    case "textCollisionbox_second":
                        tb.Background = Brushes.Gray;
                        this.textCollisionbox_hour.Background = this.Background;
                        this.textCollisionbox_minute.Background = this.Background;
                        break;
                    case "textCollisionendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textCollisionendbox_minute.Background = this.Background;
                        this.textCollisionendbox_second.Background = this.Background;
                        break;
                    case "textCollisionendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textCollisionendbox_hour.Background = this.Background;
                        this.textCollisionendbox_second.Background = this.Background;
                        break;
                    case "textCollisionendbox_second":
                        tb.Background = Brushes.Gray;
                        this.textCollisionendbox_hour.Background = this.Background;
                        this.textCollisionendbox_minute.Background = this.Background;
                        break;
                    case "textAccompanybox_hour":
                        tb.Background = Brushes.Gray;
                        this.textAccompanybox_minute.Background = this.Background;
                        this.textAccompanybox_second.Background = this.Background;
                        break;
                    case "textAccompanybox_minute":
                        tb.Background = Brushes.Gray;
                        this.textAccompanybox_hour.Background = this.Background;
                        this.textAccompanybox_second.Background = this.Background;
                        break;
                    case "textAccompanybox_second":
                        tb.Background = Brushes.Gray;
                        this.textAccompanybox_hour.Background = this.Background;
                        this.textAccompanybox_minute.Background = this.Background;
                        break;
                    case "textAccompanyendbox_hour":
                        tb.Background = Brushes.Gray;
                        this.textAccompanyendbox_minute.Background = this.Background;
                        this.textAccompanyendbox_second.Background = this.Background;
                        break;
                    case "textAccompanyendbox_minute":
                        tb.Background = Brushes.Gray;
                        this.textAccompanyendbox_hour.Background = this.Background;
                        this.textAccompanyendbox_second.Background = this.Background;
                        break;
                    case "textAccompanyendbox_second":
                        tb.Background = Brushes.Gray;
                        this.textAccompanyendbox_hour.Background = this.Background;
                        this.textAccompanyendbox_minute.Background = this.Background;
                        break;

                }
            }
        }

        private void button_up_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name.Equals("button_up"))
            {
                if (this.textbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textbox_hour.Text = temp.ToString();
                }
                else if (this.textbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textbox_minute.Text = temp.ToString();
                }
                else if (this.textbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("button_upend"))
            {
                if (this.textendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textendbox_hour.Text = temp.ToString();
                }
                else if (this.textendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textendbox_minute.Text = temp.ToString();
                }
                else if (this.textendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonCollision_up"))
            {
                if (this.textCollisionbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textCollisionbox_hour.Text = temp.ToString();
                }
                else if (this.textCollisionbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textCollisionbox_minute.Text = temp.ToString();
                }
                else if (this.textCollisionbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textCollisionbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonCollision_upend"))
            {
                if (this.textCollisionendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textCollisionendbox_hour.Text = temp.ToString();
                }
                else if (this.textCollisionendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textCollisionendbox_minute.Text = temp.ToString();
                }
                else if (this.textCollisionendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textCollisionendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonAccompany_up"))
            {
                if (this.textAccompanybox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textAccompanybox_hour.Text = temp.ToString();
                }
                else if (this.textAccompanybox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textAccompanybox_minute.Text = temp.ToString();
                }
                else if (this.textAccompanybox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textAccompanybox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonAccompany_upend"))
            {
                if (this.textAccompanyendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_hour.Text);
                    temp++;
                    if (temp > 24)
                    {
                        temp = 0;
                    }
                    this.textAccompanyendbox_hour.Text = temp.ToString();
                }
                else if (this.textAccompanyendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_minute.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textAccompanyendbox_minute.Text = temp.ToString();
                }
                else if (this.textAccompanyendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_second.Text);
                    temp++;
                    if (temp > 60)
                    {
                        temp = 0;
                    }
                    this.textAccompanyendbox_second.Text = temp.ToString();
                }
            }
        }

        private void button_down_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name.Equals("button_down"))
            {
                if (this.textbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textbox_hour.Text = temp.ToString();
                }
                else if (this.textbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textbox_minute.Text = temp.ToString();
                }
                else if (this.textbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("button_downend"))
            {
                if (this.textendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textendbox_hour.Text = temp.ToString();
                }
                else if (this.textendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textendbox_minute.Text = temp.ToString();
                }
                else if (this.textendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonCollision_down"))
            {
                if (this.textCollisionbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textCollisionbox_hour.Text = temp.ToString();
                }
                else if (this.textCollisionbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textCollisionbox_minute.Text = temp.ToString();
                }
                else if (this.textCollisionbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textCollisionbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonCollision_downend"))
            {
                if (this.textCollisionendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textCollisionendbox_hour.Text = temp.ToString();
                }
                else if (this.textCollisionendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textCollisionendbox_minute.Text = temp.ToString();
                }
                else if (this.textCollisionendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textCollisionendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textCollisionendbox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonAccompany_down"))
            {
                if (this.textAccompanybox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textAccompanybox_hour.Text = temp.ToString();
                }
                else if (this.textAccompanybox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textAccompanybox_minute.Text = temp.ToString();
                }
                else if (this.textAccompanybox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanybox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textAccompanybox_second.Text = temp.ToString();
                }
            }
            else if (btn.Name.Equals("buttonAccompany_downend"))
            {
                if (this.textAccompanyendbox_hour.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_hour.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 24;
                    }
                    this.textAccompanyendbox_hour.Text = temp.ToString();
                }
                else if (this.textAccompanyendbox_minute.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_minute.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textAccompanyendbox_minute.Text = temp.ToString();
                }
                else if (this.textAccompanyendbox_second.Background == Brushes.Gray)
                {
                    int temp = System.Int32.Parse(this.textAccompanyendbox_second.Text);
                    temp--;
                    if (temp < 0)
                    {
                        temp = 60;
                    }
                    this.textAccompanyendbox_second.Text = temp.ToString();
                }
            }
        }

        private void btnSelectData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string timeStart = string.Empty;
                string timeEnded = string.Empty;
                if (deviceFullPathNames.Count != 1)
                {
                    MessageBox.Show("只支持一组设备列表");
                    return;
                }
                if (!dploreStartTime.Text.Equals(""))
                {
                    timeStart = Convert.ToDateTime(dploreStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                if (!dploreEndTime.Text.Equals(""))
                {
                    timeEnded = Convert.ToDateTime(dploreEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                }
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_resident_imsi_list_Request(timeStart, timeEnded, deviceFullPathNames[0]));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                CreateChartEllipse.FigureList.Clear();
                CreateChartEllipse.FlagLine.Clear();
                CreateChartEllipse.FlagLabel.Clear();
                CreateChartEllipse.FlagRectangle.Clear();
                CreateChartEllipse.Pth.Clear();
                ChartCircle.Children.Clear();
                ResidentIMSIList.Clear();
                tiStatisticsInfolist.IsEnabled = false;
                //tiresidentInfolist.IsEnabled = false;
                ticollisionInfolist.IsEnabled = false;
                tiaccompanyInfolist.IsEnabled = false;
                lblResidentWaitFlag.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void numtextboxchanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                if ((this.isNum(tb.Text) == false) || (tb.Text.Length > 2))
                {
                    tb.Text = "00";
                    MessageBox.Show("请输入正确的时间！", "警告！");
                    return;
                }
            }
        }
        private bool isNum(string str)
        {
            bool ret = true;
            foreach (char c in str)
            {
                if ((c < 48) || (c > 57))
                {
                    return false;
                }
            }
            return ret;
        }

        private void tiresidentInfolist_Loaded(object sender, RoutedEventArgs e)
        {
            dploreStartTime.SelectedDate = DateTime.Now.Date;
            dploreEndTime.SelectedDate = DateTime.Now.Date;
            this.textbox_hour.Text = "00";
            this.textbox_minute.Text = "00";
            this.textbox_second.Text = "00";

            this.textendbox_hour.Text = "23";
            this.textendbox_minute.Text = "59";
            this.textendbox_second.Text = "59";
            //加载数据列表
            dgResidentIMSITable.ItemsSource = ResidentIMSIList;
            ChartCircle.DataContext = CreateChartPie;
            CreateChartPie.ChartWidth = 300;
            CreateChartPie.ChartHeight = 580;

            ChartCircleEll.DataContext = CreateChartEllipse;
            //List<int> temd = new List<int> { 451,125 };
            //CreateChartEllipse.CreateChart(temd);
            //for (int i = 0; i < CreateChartEllipse.Pth.Count; i++)
            //{
            //    ChartCircleEll.Children.Add(CreateChartEllipse.Pth[i]);
            //    ChartCircleEll.Children.Add(CreateChartEllipse.FlagLabel[i]);
            //    ChartCircleEll.Children.Add(CreateChartEllipse.FlagRectangle[i]);
            //}
            //for (int i = 0; i < CreateChartEllipse.FlagLine.Count; i++)
            //{
            //    ChartCircleEll.Children.Add(CreateChartEllipse.FlagLine[i]);
            //}
        }

        private void ticollisionInfolist_Loaded(object sender, RoutedEventArgs e)
        {
            dpCollisionStartTime.SelectedDate = DateTime.Now.Date;
            dpCollisionEndTime.SelectedDate = DateTime.Now.Date;
            this.textCollisionbox_hour.Text = "00";
            this.textCollisionbox_minute.Text = "00";
            this.textCollisionbox_second.Text = "00";

            this.textCollisionendbox_hour.Text = "23";
            this.textCollisionendbox_minute.Text = "59";
            this.textCollisionendbox_second.Text = "59";
            lbConditionsList.ItemsSource = ConditionsList;
            //加载数据列表
            dgCollisionIMSITable.ItemsSource = ConditionsIMSIList;
        }

        private void btnAddCollisionData_Click(object sender, RoutedEventArgs e)
        {
            string fullname = string.Empty;
            string timeStart = string.Empty;
            string timeEnd = string.Empty;
            timeStart = Convert.ToDateTime(dpCollisionStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textCollisionbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textCollisionbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textCollisionbox_second.Text.Trim()).ToString().PadLeft(2, '0');
            timeEnd = Convert.ToDateTime(dpCollisionEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textCollisionendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textCollisionendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textCollisionendbox_second.Text.Trim()).ToString().PadLeft(2, '0');

            for (int i = 0; i < deviceFullPathNames.Count; i++)
            {
                ConditionsClass tmpConditions = new ConditionsClass();
                bool flag = false;
                //string[] nameList = deviceFullPathNames[i].Split(new char[] { '.' });
                //string _fullName = nameList[nameList.Length - 2] + "." + nameList[nameList.Length - 1];
                tmpConditions.FullName = deviceFullPathNames[i];
                tmpConditions.TimeStart = timeStart;
                tmpConditions.TimeEnd = timeEnd;
                for (int j = 0; j < ConditionsList.Count; j++)
                {
                    if (ConditionsList[j].FullName == tmpConditions.FullName &&
                        ConditionsList[j].TimeStart == tmpConditions.TimeStart &&
                        ConditionsList[j].TimeEnd == tmpConditions.TimeEnd)
                    {
                        flag = true;
                        break;
                    }

                }
                if (ConditionsList.Count >= 8)
                {
                    MessageBox.Show("查询条件不超过8组");
                    return;
                }
                else if (!flag)
                {
                    ConditionsList.Add(tmpConditions);
                    lbConditionsList.Items.Refresh();
                }
            }
        }

        private void miDellItem_Click(object sender, RoutedEventArgs e)
        {
            ConditionsClass _conditions = (ConditionsClass)(lbConditionsList.SelectedItem as ConditionsClass);
            for (int i = 0; i < ConditionsList.Count; i++)
            {
                if (ConditionsList[i] == _conditions)
                {
                    ConditionsList.RemoveAt(i);
                    break;
                }
            }
            lbConditionsList.Items.Refresh();
        }

        private void btnSelectCollision_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConditionsList.Count <= 0)
                {
                    MessageBox.Show("请先添加查询条件");
                    return;
                }
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_collision_imsi_list_Request(ConditionsList));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ConditionsIMSIList.Clear();
                tiStatisticsInfolist.IsEnabled = false;
                tiresidentInfolist.IsEnabled = false;
                //ticollisionInfolist.IsEnabled = false;
                tiaccompanyInfolist.IsEnabled = false;
                lblCollisionWaitFlag.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void tiaccompanyInfolist_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dpAccompanyStartTime.SelectedDate = DateTime.Now.Date;
                dpAccompanyEndTime.SelectedDate = DateTime.Now.Date;
                this.textAccompanybox_hour.Text = "00";
                this.textAccompanybox_minute.Text = "00";
                this.textAccompanybox_second.Text = "00";

                this.textAccompanyendbox_hour.Text = "23";
                this.textAccompanyendbox_minute.Text = "59";
                this.textAccompanyendbox_second.Text = "59";
                dgAccompanyIMSITable.ItemsSource = AccompanyIMSIList;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void btnSelectAccompany_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string imsi = string.Empty;
                string timeStart = string.Empty;
                string timeEnd = string.Empty;
                string timeWindow = string.Empty;
                imsi = txtIMSI.Text.Trim();
                timeWindow = txtTimeWindow.Text.Trim();
                timeStart = Convert.ToDateTime(dpAccompanyStartTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textAccompanybox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textAccompanybox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textAccompanybox_second.Text.Trim()).ToString().PadLeft(2, '0');
                timeEnd = Convert.ToDateTime(dpAccompanyEndTime.Text).ToString("yyyy-MM-dd") + " " + System.Int32.Parse(textAccompanyendbox_hour.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textAccompanyendbox_minute.Text.Trim()).ToString().PadLeft(2, '0') + ":" + System.Int32.Parse(textAccompanyendbox_second.Text.Trim()).ToString().PadLeft(2, '0');
                if (NetController.NetWorkClient.ControllerServer.Connected)
                {
                    NetWorkClient.ControllerServer.Send(JsonInterFace.Get_Accompany_imsi_list_Request(imsi, timeStart, timeEnd, timeWindow));
                }
                else
                {
                    MessageBox.Show("网络与服务器断开！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                tiStatisticsInfolist.IsEnabled = false;
                tiresidentInfolist.IsEnabled = false;
                ticollisionInfolist.IsEnabled = false;
                //tiaccompanyInfolist.IsEnabled = false;
                lblAccompanyWaitFlag.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("伴随分析", ex.Message, ex.StackTrace);
            }
        }

        private void miDellAllItem_Click(object sender, RoutedEventArgs e)
        {
            ConditionsList.Clear();
            lbConditionsList.Items.Refresh();
        }

        private void titext_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasArea.DataContext = Columns;
            ChartDrawingCanvas(920, 520);

        }
        private void ChartDrawingCanvas(double ChartWidth, double ChartHeight)
        {
            try
            {
                if (CanvasArea.Children.Count > 0)
                {
                    CanvasArea.Children.Clear();
                }
                Columns.ColumnsHeight = ChartHeight;
                Columns.ColumnsWidth = ChartWidth;
                Columns.OutLineColor = "red";

                Columns.ChartColumns();
                Columns.ChartOutLine();
                Columns.ChartRowLine();
                //坐标轴
                CanvasArea.Children.Add(Columns.RowLines);
                CanvasArea.Children.Add(Columns.ColLines);
                //刻度值
                for (int i = 0; i < Columns.LongScaleLine.Count; i++)
                {
                    CanvasArea.Children.Add(Columns.LongScaleLine[i]);
                }
                for (int i = 0; i < Columns.XScaleValueList.Count; i++)
                {
                    CanvasArea.Children.Add(Columns.XScaleValueList[i]);
                }
                for (int i = 0; i < Columns.YScaleValueList.Count; i++)
                {
                    CanvasArea.Children.Add(Columns.YScaleValueList[i]);
                }
                //数据显示
                for (int i = 0; i < Columns.ColumnList.Count; i++)
                {
                    CanvasArea.Children.Add(Columns.ColumnList[i]);
                }
                for (int i = 0; i < Columns.lblCountList.Count; i++)
                {
                    CanvasArea.Children.Add(Columns.lblCountList[i]);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表基本参数", ex.Message, ex.StackTrace);
            }
        }
        private void btnSelectCanvaData_Click(object sender, RoutedEventArgs e)
        {
            ChartDrawingCanvas(920, 520);
        }
        private void ExportDataToTxt(string data)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                //设置文件类型
                saveFileDialog.Filter = "csv files(*.txt)|*.txt|All files(*.*)|*.*";
                //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                saveFileDialog.AddExtension = true;
                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;
                if (data.Length > 0)
                {
                    //点了保存按钮进入  
                    if ((bool)saveFileDialog.ShowDialog())
                    {
                        //获得文件路径  
                        string localFilePath = saveFileDialog.FileName.ToString();
                        //string filname = this.openFileDialog.FileName;
                        //获取文件名，不带路径  
                        string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

                        //获取文件路径，不带文件名  
                        string FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

                        //给文件名前加上时间  
                        string newFileName = FilePath + "\\" + DateTime.Now.ToString("yyyyMMdd") + fileNameExt;

                        System.IO.File.WriteAllText(localFilePath, data);

                        //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog.OpenFile();//输出文件  

                        //fs输出带文字或图片的文件，就看需求了 
                        if (File.Exists(localFilePath))
                        {
                            MessageBox.Show("数据导出成功", "提示", MessageBoxButton.OKCancel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("数据导出", ex.Message, ex.StackTrace);
            }
        }

        private void btnExportData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                if (ResidentIMSIList.Count > 0)
                {
                    data = "序号\tIMSI" + "\r\n";
                    for (int i = 0; i < ResidentIMSIList.Count; i++)
                    {
                        data += (i + 1).ToString() + "\t\t" + ResidentIMSIList[i].IMSI + "\r\n";
                    }
                    ExportDataToTxt(data);
                }
                else
                {
                    MessageBox.Show("常驻人口数据为空", "提示");
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("常驻人口数据导出", ex.Message, ex.StackTrace);
            }
        }

        private void mmCollisionData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                if (ConditionsIMSIList.Count > 0)
                {
                    data = "序号\tIMSI" + "\r\n";
                    for (int i = 0; i < ConditionsIMSIList.Count; i++)
                    {
                        data += (i + 1).ToString() + "\t\t" + ConditionsIMSIList[i].IMSI + "\r\n";
                    }
                    ExportDataToTxt(data);
                }
                else
                {
                    MessageBox.Show("碰撞分析数据为空", "提示");
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("碰撞分析数据导出", ex.Message, ex.StackTrace);
            }
        }

        private void mmAccompanyData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                if (AccompanyIMSIList.Count > 0)
                {
                    data = "序号\tIMSI" + "\r\n";
                    for (int i = 0; i < AccompanyIMSIList.Count; i++)
                    {
                        data += (i + 1).ToString() + "\t\t" + AccompanyIMSIList[i].IMSI + "\r\n";
                    }
                    ExportDataToTxt(data);
                }
                else
                {
                    MessageBox.Show("伴随分析数据为空", "提示");
                    return;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("伴随分析数据导出", ex.Message, ex.StackTrace);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string data = string.Empty;
                if (StatisticalAnalysis.MeasReportAxialChartBlackList.Rows.Count > 0)
                {
                    data = "序号\t总数量\t去重总数" + "\r\n";
                    for (int i = 0; i < StatisticalAnalysis.MeasReportAxialChartBlackList.Rows.Count; i++)
                    {
                        data += StatisticalAnalysis.MeasReportAxialChartBlackList.Rows[i][0].ToString() + "\t\t" + StatisticalAnalysis.MeasReportAxialChartBlackList.Rows[i][1].ToString() + "\t\t" + StatisticalAnalysis.MeasReportAxialChartBlackList.Rows[i][2].ToString() + "\r\n";
                    }
                    ExportDataToTxt(data);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("分时统计数据导出", ex.Message, ex.StackTrace);
            }
        }
    }
}
