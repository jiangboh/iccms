using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace iccms.SpecialListManage
{
    public class LoadStatuWinControlClass : INotifyPropertyChanged
    {
        private int _size = 0;
        private Visibility _enable = Visibility.Visible;
        private int _maximum = 100;
        private int _value = 0;
        private Visibility _listView = Visibility.Collapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(value));
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;
                NotifyPropertyChanged("Size");
            }
        }

        public Visibility Enable
        {
            get
            {
                return _enable;
            }

            set
            {
                _enable = value;
                NotifyPropertyChanged("Enable");
            }
        }

        public int Maximum
        {
            get
            {
                return _maximum;
            }

            set
            {
                _maximum = value;
                NotifyPropertyChanged("Maximum");
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }

        public Visibility ListView
        {
            get
            {
                return _listView;
            }

            set
            {
                _listView = value;
                NotifyPropertyChanged("ListView");
            }
        }
    }

    /// <summary>
    /// BWListDeviceLoadStatuControlWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BWListDeviceLoadStatuControlWindow : Page
    {
        public static LoadStatuWinControlClass LoadStatuWinControl = new LoadStatuWinControlClass();
        public BWListDeviceLoadStatuControlWindow()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PrgLoadStatuBar.DataContext = LoadStatuWinControl;
        }
    }
}
