using ParameterControl;
using System;
using System.Collections.Generic;
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

namespace iccms.NavigatePages
{
    /// <summary>
    /// StatisticalInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticalInfoWindow : Window
    {
        public StatisticalInfoWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = new Language_EN.DeviceStatistion();
                tbcBootSituation.DataContext = new Language_EN.DeviceStatistion();
                btnClose.DataContext = new Language_EN.DeviceStatistion();

                txtBlockNodeName.DataContext = new Language_EN.DeviceStatistion();
                txtBlockNoneConnect.DataContext = new Language_EN.DeviceStatistion();
                txtBlockNoneActive.DataContext = new Language_EN.DeviceStatistion();
                txtBlockActive.DataContext = new Language_EN.DeviceStatistion();
            }
            else
            {
                this.DataContext = new Language_CN.DeviceStatistion();
                tbcBootSituation.DataContext = new Language_CN.DeviceStatistion();
                btnClose.DataContext = new Language_CN.DeviceStatistion();

                txtBlockNodeName.DataContext = new Language_CN.DeviceStatistion();
                txtBlockNoneConnect.DataContext = new Language_CN.DeviceStatistion();
                txtBlockNoneActive.DataContext = new Language_CN.DeviceStatistion();
                txtBlockActive.DataContext = new Language_CN.DeviceStatistion();
            }
        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IsCheckAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnClose.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
        }
    }
}
