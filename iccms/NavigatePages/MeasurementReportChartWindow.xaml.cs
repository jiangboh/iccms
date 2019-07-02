using iccms.Arrows;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace iccms.NavigatePages
{

    #region 曲线图表类
    public class MeasurementReportChartChartParametersClass : INotifyPropertyChanged
    {
        //窗口大小
        private double _chartWindowHeight = 200;
        private double _chartWindowWidth = 814;

        //曲线图区
        private Canvas _selfChart = new Canvas();
        //曲线图锁
        private object _ChartLock = null;

        //区域
        private double _chartArea_Width = 814;
        private double _chartArea_Height = 200;

        //刻度零点
        private double _xOrig = 60;
        private double _yOrig = 50;

        //背景网格零点
        private double _xBgOrig = 0;
        private double _yBgOrig = 0;

        //背景色
        private string _chartBackGround = "LightSky";

        //刻度线
        public List<Line> GridLine = null;
        public List<Line> LongScaleLine = null;
        private string _gridLineColor = "Green";
        private string _longGridLineColor = "Green";
        private bool _gridEnable;
        private double _gridLineSize = 1;
        private double _longGridLineSize = 1;
        private double _xShortScaleLineSpace = 10;
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
        private string _xAxialScaleValueColor = "Yellow";
        private double _yAxialScaleValueFontSize = 12;
        private double _yAxialScaleValueOpacity = 1;
        private string _yAxialScaleValueColor = "Yellow";

        //背景网格线
        public List<Line> BackGroundGridLine = null;
        private string _backGroundGridLineColor = "Green";
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
        private string _x_QuadrantColor = "Green";
        private string _y_QuadrantColor = "Green";
        private double _X_QuadrantLineSize = 1;
        private double _Y_QuadrantLineSize = 1;
        private double _x_QuadrantAxialLineLength = 0;
        private double _y_QuadrantAxialLineLength = 0;
        //X,Y坐标起点
        private double _x_QuadrantAxialStart = 0;
        private double _y_QuadrantAxialStart = 0;
        //X,Y轴刻度数
        private int x_QuadrantScaleValueCount = 60;
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
        private int x_QuadrantArrowScaleValueCount = 60;
        private int y_QuadrantArrowScaleValueCount = 128;

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
        public void Input(string IMSI, string RSRP)
        {
            try
            {
                DataRow dr = MeasReportAxialChartBlackList.NewRow();
                dr["IMSI"] = IMSI;
                dr["RSRP"] = Convert.ToInt32(RSRP);
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

        //绘制曲线
        public Thread DrawingMeasureReportAxialThread = null;

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

        //构造
        public MeasurementReportChartChartParametersClass(double width, double height)
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
        public MeasurementReportChartChartParametersClass()
        {
            if (MeasReportAxialChartBlackList == null)
            {
                MeasReportAxialChartBlackList = new DataTable("BlackList");
                InitialTab();
            }

            InitChart();

            InitDrawingMeasureReportAxialThread();
        }

        //初始化数据表
        private void InitialTab()
        {
            DataColumn DataColumn0 = new DataColumn();
            DataColumn0.ColumnName = "IMSI";
            DataColumn0.DataType = System.Type.GetType("System.String");

            DataColumn DataColumn1 = new DataColumn();
            DataColumn1.ColumnName = "RSRP";
            DataColumn1.DataType = System.Type.GetType("System.Int32");

            MeasReportAxialChartBlackList.Columns.Add(DataColumn0);
            MeasReportAxialChartBlackList.Columns.Add(DataColumn1);
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

        //初始化曲线图线程
        public void InitDrawingMeasureReportAxialThread()
        {
            if (DrawingMeasureReportAxialThread == null)
            {
                DrawingMeasureReportAxialThread = new Thread(new ThreadStart(DrawingMeasureReportAxial));
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
                double YLineCount = Y_QuadrantArrowScaleValueCount;
                YShortScaleLineSpace = (Y_QuadrantArrowAxialLineLength - 10) / YLineCount;
                GridLine.Clear();
                for (int i = 1; i <= YLineCount; i++)
                {
                    Line YgridLine = new Line();
                    YgridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridLineColor));
                    YgridLine.X1 = XOrig;
                    YgridLine.Y1 = ChartArea_Height - YOrig - (YShortScaleLineSpace * (i));
                    YgridLine.Opacity = GridLineOpacity;
                    YgridLine.X2 = XOrig - Y_AxialScaleLineLen;
                    YgridLine.Y2 = ChartArea_Height - YOrig - (YShortScaleLineSpace * (i));
                    YgridLine.StrokeThickness = GridLineSize;
                    GridLine.Add(YgridLine);
                }

                double XLineCount = X_QuadrantArrowScaleValueCount;
                XShortScaleLineSpace = (X_QuadrantArrowAxialLineLength - 10) / XLineCount;
                for (int i = 1; i <= XLineCount; i++)
                {
                    Line XgridLine = new Line();
                    XgridLine.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(GridLineColor));
                    XgridLine.X1 = XOrig + (XShortScaleLineSpace * (i));
                    XgridLine.Y1 = ChartArea_Height - YOrig;
                    XgridLine.Opacity = GridLineOpacity;
                    XgridLine.X2 = XOrig + (XShortScaleLineSpace * (i));
                    XgridLine.Y2 = ChartArea_Height - YOrig + X_AxialScaleLineLen;
                    XgridLine.StrokeThickness = GridLineSize;
                    GridLine.Add(XgridLine);
                }
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
                double YLongLineCount = Y_QuadrantArrowScaleValueCount / 10;
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
                    ChartYAxialScaleValue(YLgGridLine.X2, YLgGridLine.Y2, "-" + (i * 10).ToString(), "Yellow", 12);
                }

                double XLongLineCount = X_QuadrantArrowScaleValueCount / 10;
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
                    ChartXAxialScaleValue(XLgGridLine.X2, XLgGridLine.Y2, (i * 10).ToString() + "(S)", "Yellow", 12);
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
                XAxialScaleValue.Width = 40;
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
                YAxialScaleValue.Width = 40;
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

        //绘制曲线图
        private void DrawingMeasureReportAxial()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);

                    for (int i = 0; i < MeasReportAxialChartBlackList.Rows.Count; i++)
                    {
                        string _IMSI = string.Empty;
                        double RSRP = 0xFF;

                        if (IMSI != "全部" || IMSI != "All")
                        {
                            if (IMSI == MeasReportAxialChartBlackList.Rows[i]["IMSI"].ToString())
                            {
                                _IMSI = MeasReportAxialChartBlackList.Rows[i]["IMSI"].ToString();
                                RSRP = Convert.ToDouble(MeasReportAxialChartBlackList.Rows[i]["RSRP"].ToString());
                                if (RSRP < -128) { RSRP = -128; }
                                if (RSRP > 0) { RSRP = 0; }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            _IMSI = MeasReportAxialChartBlackList.Rows[i]["IMSI"].ToString();
                            RSRP = Convert.ToDouble(MeasReportAxialChartBlackList.Rows[i]["RSRP"].ToString());
                            if (RSRP < -128) { RSRP = -128; }
                            if (RSRP > 0) { RSRP = 0; }
                        }

                        if (IMSI == "" || IMSI == null) { continue; }

                        SelfChart.Dispatcher.Invoke(new Action(() =>
                        {
                            lock (ChartLock)
                            {
                                //超出60秒的曲线处理
                                switch (Parameters.ChartAxialModel)
                                {
                                    case 0:
                                        if (SelfChart.Children.Count >= X_QuadrantArrowScaleValueCount * 2)
                                        {
                                            if (Convert.ToDouble((SelfChart.Children[SelfChart.Children.Count - 2] as Line).Tag) >= X_QuadrantArrowScaleValueCount)
                                            {
                                                try
                                                {
                                                    SelfChart.Children.Clear();
                                                }
                                                catch (Exception ex)
                                                {
                                                    Parameters.PrintfLogsExtended("元素切换", ex.Message, ex.StackTrace);
                                                }
                                            }
                                        }
                                        break;
                                    case 1:
                                        if (SelfChart.Children.Count >= X_QuadrantArrowScaleValueCount * 2)
                                        {
                                            if (Convert.ToDouble((SelfChart.Children[SelfChart.Children.Count - 2] as Line).Tag) >= X_QuadrantArrowScaleValueCount)
                                            {
                                                switch (MoveLineControl)
                                                {
                                                    case 0:
                                                        SelfChart.Children.RemoveRange(0, 4);
                                                        MoveLineControl++;
                                                        break;
                                                    default:
                                                        SelfChart.Children.RemoveRange(0, 2);
                                                        MoveLineControl++;
                                                        break;
                                                }

                                                //元素后移
                                                for (int k = 0; k < SelfChart.Children.Count; k += 2)
                                                {
                                                    try
                                                    {
                                                        (SelfChart.Children[k] as Line).X1 -= XShortScaleLineSpace;
                                                        (SelfChart.Children[k] as Line).X2 -= XShortScaleLineSpace;
                                                        double XDot = Convert.ToDouble((SelfChart.Children[k + 1] as Ellipse).GetValue(Canvas.LeftProperty)) - XShortScaleLineSpace;
                                                        (SelfChart.Children[k + 1] as Ellipse).SetValue(Canvas.LeftProperty, XDot);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Parameters.PrintfLogsExtended("曲线后移", ex.Message, ex.StackTrace);
                                                    }
                                                }

                                                if ((SelfChart.Children[0] as Line).X1 < XOrig + (SelfChart.Children[0] as Line).Width)
                                                {
                                                    SelfChart.Children.RemoveRange(0, 2);
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }

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
                                                X2 = (SelfChart.Children[k] as Line).X2 + XShortScaleLineSpace;
                                                Y2 = Y_QuadrantArrowAxialStart - (Math.Abs(RSRP) * YShortScaleLineSpace);
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
                                            SelfLineName.ToolTip = "IMSI:" + IMSI + Environment.NewLine
                                                                   + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                                   + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                                   + "[RSPR]:" + RSRP.ToString();

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
                                            SelfEllipseName.ToolTip = "[IMSI]:" + IMSI + Environment.NewLine
                                                                      + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                                      + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                                      + "[RSPR]:" + RSRP.ToString() + Environment.NewLine
                                                                      + "XStart=" + (X2 - (NodeSize / 2f)).ToString() + ",YStart=" + (Y2 - (NodeSize / 2f)).ToString() + Environment.NewLine
                                                                      + "[Second]:" + Second;

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
                                        double Y1 = (ChartArea_Height - YOrig) + RSRP;
                                        double X2 = XOrig;
                                        double Y2 = (ChartArea_Height - YOrig) - (Math.Abs(RSRP) * YShortScaleLineSpace);
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
                                        SelfLineName.ToolTip = "[IMSI]:" + IMSI + Environment.NewLine
                                                               + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                               + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                               + "[RSPR]:" + RSRP.ToString();

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
                                        SelfEllipseName.ToolTip = "[IMSI]:" + IMSI + Environment.NewLine
                                                                  + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                                  + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                                  + "[RSPR]:" + RSRP.ToString() + Environment.NewLine
                                                                  + "XStart=" + (X2 - (NodeSize / 2f)).ToString() + ",YStart=" + (Y2 - (NodeSize / 2f)).ToString() + Environment.NewLine
                                                                  + "[Second]:" + Second;

                                        SelfChart.Children.Add(SelfLineName);
                                        SelfChart.Children.Add(SelfEllipseName);
                                    }
                                }
                                else
                                {
                                    double X1 = XOrig;
                                    double Y1 = (ChartArea_Height - YOrig) + RSRP;
                                    double X2 = XOrig;
                                    double Y2 = (ChartArea_Height - YOrig) - (Math.Abs(RSRP) * YShortScaleLineSpace);

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
                                    SelfLineName.ToolTip = "[IMSI]:" + IMSI + Environment.NewLine
                                                           + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                           + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                           + "[RSPR]:" + RSRP.ToString();

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
                                    SelfEllipseName.ToolTip = "[IMSI]:" + IMSI + Environment.NewLine
                                                              + "X1=" + X1.ToString() + ",Y1=" + Y1.ToString() + Environment.NewLine
                                                              + "X2=" + X2.ToString() + ",Y2=" + Y2.ToString() + Environment.NewLine
                                                              + "[RSPR]:" + RSRP.ToString() + Environment.NewLine
                                                              + "XStart=" + (X2 - (NodeSize / 2f)).ToString() + ",YStart=" + (Y2 - (NodeSize / 2f)).ToString() + Environment.NewLine
                                                              + "[Second]:" + Second;

                                    SelfChart.Children.Add(SelfLineName);
                                    SelfChart.Children.Add(SelfEllipseName);
                                }
                            }
                        }));
                    }

                    Second++;
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("黑名单追踪曲线", ex.Message, ex.StackTrace);
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

    /// <summary>
    /// MeasurementReportChartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MeasurementReportChartWindow : Page
    {
        public static MeasurementReportChartChartParametersClass SelfChart = SelfChart = new MeasurementReportChartChartParametersClass();
        public static System.Timers.Timer ReloadChartBaseTimer = null;
        public static System.Timers.Timer ResumeThreadTimer = null;

        public MeasurementReportChartWindow()
        {
            InitializeComponent();

            if (ReloadChartBaseTimer == null)
            {
                ReloadChartBaseTimer = new System.Timers.Timer();
                ReloadChartBaseTimer.AutoReset = false;
                ReloadChartBaseTimer.Interval = 1;
                ReloadChartBaseTimer.Elapsed += ReloadChartBaseTimer_Elapsed;
            }

            if (ResumeThreadTimer == null)
            {
                ResumeThreadTimer = new System.Timers.Timer();
                ResumeThreadTimer.AutoReset = false;
                ResumeThreadTimer.Interval = 1;
                ResumeThreadTimer.Elapsed += SuspendThreadTimer_Elapsed;
            }
        }

        //重启曲线线程
        private void SuspendThreadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.Suspended)
                {
                    SelfChart.DrawingMeasureReportAxialThread.Resume();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("重启曲线线程", ex.Message, ex.StackTrace);
            }
        }

        //窗口改变重新加载曲线图
        private void ReloadChartBaseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    ChartDrawingBase(MeasurementReportChartWin.ActualWidth, MeasurementReportChartWin.ActualHeight);
                    //曲线图跟随改变大小
                    if (SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.Running || SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.WaitSleepJoin)
                    {
                        SelfChart.DrawingMeasureReportAxialThread.Suspend();
                        SelfChart.MoveLineControl = 0;
                        SelfChart.Second = 0;
                        SelfChart.ResizeAxial(1);
                        SelfChart.DrawingMeasureReportAxialThread.Resume();
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            });
        }

        private void MeasurementReportChartWin_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ChartArea.DataContext = SelfChart;
                MeasurementReportChartWin.DataContext = SelfChart;
                SelfChart.MoveLineControl = 0;
                SelfChart.Second = 0;
                SelfChart.SettingChartHandle(ref ChartArea);
                ChartDrawingBase(MeasurementReportChartWin.ActualWidth, MeasurementReportChartWin.ActualHeight);

                //启动曲线图
                if (SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.Unstarted)
                {
                    SelfChart.DrawingMeasureReportAxialThread.Start();
                }
                else if (SelfChart.DrawingMeasureReportAxialThread.ThreadState == ThreadState.Suspended)
                {
                    SelfChart.DrawingMeasureReportAxialThread.Resume();
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("曲线图参数初始化", ex.Message, ex.StackTrace);
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
                SelfChart.ChartArea_Width = ChartWidth;
                SelfChart.ChartArea_Height = ChartHeight;

                //背景色
                SelfChart.ChartBackGround = "#FF092A19";

                //网格色
                SelfChart.GridLineColor = "Green";
                SelfChart.GridEnable = true;

                //边框
                SelfChart.OutLineColor = "DodgerBlue";
                SelfChart.ChartOutLine();
                ChartBackGround.Children.Add(SelfChart.OutLine);

                //坐标系
                SelfChart.XQuadrantArrow();
                SelfChart.YQuadrantArrow();
                SelfChart.X_QuadrantArrowColor = "Aqua";
                SelfChart.Y_QuadrantArrowColor = "Orange";
                SelfChart.X_QuadrantArrowLineSize = 3;
                SelfChart.Y_QuadrantArrowLineSize = 3;
                CoordinateGraphs.Children.Add(SelfChart.X_QuadrantArrow);
                CoordinateGraphs.Children.Add(SelfChart.Y_QuadrantArrow);

                //背景网格线
                SelfChart.BackGroundGridLineColor = "Green";
                SelfChart.BackGroundGridLineOpacity = 0.2;
                SelfChart.BackGroundGridLineSize = 1;
                SelfChart.BackGroundGridLineSpace = 20;
                SelfChart.ChartBackGroundGridLine();
                for (int i = 0; i < SelfChart.BackGroundGridLine.Count; i++)
                {
                    ChartBackGround.Children.Add(SelfChart.BackGroundGridLine[i]);
                }

                //短刻度标示线
                SelfChart.GridLineColor = "Aqua";
                SelfChart.GridLineOpacity = 1;
                SelfChart.GridLineSize = 1;
                SelfChart.X_AxialScaleLineLen = 5;
                SelfChart.Y_AxialScaleLineLen = 5;
                SelfChart.ChartShortScaleLine();
                for (int i = 0; i < SelfChart.GridLine.Count; i++)
                {
                    CoordinateGraphs.Children.Add(SelfChart.GridLine[i]);
                }

                //长刻度标示线
                SelfChart.LongGridLineColor = "Yellow";
                SelfChart.LongGridLineOpacity = 1;
                SelfChart.LongGridLineSize = 2;
                SelfChart.X_LongAxialScaleLineLen = 15;
                SelfChart.Y_LongAxialScaleLineLen = 15;
                SelfChart.ChartLongScaleLine();
                for (int i = 0; i < SelfChart.LongScaleLine.Count; i++)
                {
                    //线
                    CoordinateGraphs.Children.Add(SelfChart.LongScaleLine[i]);
                }

                //X轴长刻度值
                for (int i = 0; i < SelfChart.XAxialScaleValueList.Count; i++)
                {
                    //值
                    CoordinateGraphs.Children.Add(SelfChart.XAxialScaleValueList[i]);
                }

                //Y轴长刻度值
                for (int i = 0; i < SelfChart.YAxialScaleValueList.Count; i++)
                {
                    //值
                    CoordinateGraphs.Children.Add(SelfChart.YAxialScaleValueList[i]);
                }

                //曲线存在跟随改变大小
                SelfChart.ResizeAxial(1);
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("图表基本参数", ex.Message, ex.StackTrace);
            }
        }

        private void MeasurementReportChartWin_Unloaded(object sender, RoutedEventArgs e)
        {
            switch (Parameters.ChartAxialEnable)
            {
                case 0:
                    if (SelfChart.DrawingMeasureReportAxialThread != null)
                    {
                        try
                        {
                            SelfChart.DrawingMeasureReportAxialThread.Suspend();
                        }
                        catch (Exception ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                        finally
                        {
                            lock (SelfChart.ChartLock)
                            {
                                ChartArea.Children.Clear();
                            }
                        }
                    }
                    break;
                case 1:
                    break;
            }
        }

        private void ChartArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                SelfChart.ChartArea_Width = ChartArea.ActualWidth;
                SelfChart.ChartArea_Height = ChartArea.ActualHeight;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("改变窗口大小", ex.Message, ex.StackTrace);
            }
        }
    }
}
