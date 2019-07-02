using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    public class ChartCoordinateParametersClass : INotifyPropertyChanged
    {
        private string _iMSI;
        private string _rSRP;
        private string _xStart;
        private string _yStart;
        private string _x1;
        private string _y1;
        private string _x2;
        private string _y2;
        private string _second;

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

        public string RSRP
        {
            get
            {
                return _rSRP;
            }

            set
            {
                _rSRP = value;
                NotifyPropertyChanged("RSRP");
            }
        }

        public string XStart
        {
            get
            {
                return _xStart;
            }

            set
            {
                _xStart = value;
                NotifyPropertyChanged("XStart");
            }
        }

        public string YStart
        {
            get
            {
                return _yStart;
            }

            set
            {
                _yStart = value;
                NotifyPropertyChanged("YStart");
            }
        }

        public string X1
        {
            get
            {
                return _x1;
            }

            set
            {
                _x1 = value;
                NotifyPropertyChanged("X1");
            }
        }

        public string Y1
        {
            get
            {
                return _y1;
            }

            set
            {
                _y1 = value;
                NotifyPropertyChanged("Y1");
            }
        }

        public string X2
        {
            get
            {
                return _x2;
            }

            set
            {
                _x2 = value;
                NotifyPropertyChanged("X2");
            }
        }

        public string Y2
        {
            get
            {
                return _y2;
            }

            set
            {
                _y2 = value;
                NotifyPropertyChanged("Y2");
            }
        }

        public string Second
        {
            get
            {
                return _second;
            }

            set
            {
                _second = value;
                NotifyPropertyChanged("Second");
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
    /// ChartCoordinateInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChartCoordinateInfoWindow : Window
    {
        public static ChartCoordinateParametersClass ChartCoordinateParameters = new ChartCoordinateParametersClass();

        public ChartCoordinateInfoWindow()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtIMSI.DataContext = ChartCoordinateParameters;
            txtRSRP.DataContext = ChartCoordinateParameters;
            txtXStart.DataContext = ChartCoordinateParameters;
            txtYStart.DataContext = ChartCoordinateParameters;
            txtX1.DataContext = ChartCoordinateParameters;
            txtY1.DataContext = ChartCoordinateParameters;
            txtX2.DataContext = ChartCoordinateParameters;
            txtY2.DataContext = ChartCoordinateParameters;
            txtSecond.DataContext = ChartCoordinateParameters;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnExit.Focus();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnExit.IsFocused)
                {
                    this.DragMove();
                }
            }
        }
    }
}
