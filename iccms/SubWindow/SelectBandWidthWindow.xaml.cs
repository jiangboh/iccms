using DataInterface;
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

namespace iccms.SubWindow
{
    /// <summary>
    /// SelectBandWidthWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectBandWidthWindow : Window
    {
        public SelectBandWidthWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (cbbBandWidth.Text != null || cbbBandWidth.Text != "")
            {
                JsonInterFace.LteCellNeighParameter.BandWidth = cbbBandWidth.Text;
            }
            else
            {
                MessageBox.Show("请选择带宽值！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (JsonInterFace.LteCellNeighParameter.BandWidth != null || JsonInterFace.LteCellNeighParameter.BandWidth != "")
            {
                cbbBandWidth.Text = JsonInterFace.LteCellNeighParameter.BandWidth;
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
