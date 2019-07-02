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
    /// GSMCarrierChoiseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GSMCarrierChoiseWindow : Window
    {
        private string Model = string.Empty;
        public GSMCarrierChoiseWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }
        public GSMCarrierChoiseWindow(string model)
        {
            Model = model;
            InitializeComponent();
            if (Model == DeviceType.GSM)
            {
                lblCarrierChoise.Content = "GSM 载波选择";
            }
            else if (Model == DeviceType.GSMV2)
            {
                lblCarrierChoise.Content = "GSMV2 载波选择";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (Model == DeviceType.GSM)
            {
                JsonInterFace.GSMCarrierParameter.CarrierOne = (bool)rdbCarrierOne.IsChecked;
                JsonInterFace.GSMCarrierParameter.CarrierTwo = (bool)rdbCarrierTwo.IsChecked;

                if (!JsonInterFace.GSMCarrierParameter.CarrierOne && !JsonInterFace.GSMCarrierParameter.CarrierTwo)
                {
                    MessageBox.Show("请选择GSM载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else if (Model == DeviceType.GSMV2)
            {
                JsonInterFace.GSMV2CarrierParameter.CarrierOne = (bool)rdbCarrierOne.IsChecked;
                JsonInterFace.GSMV2CarrierParameter.CarrierTwo = (bool)rdbCarrierTwo.IsChecked;

                if (!JsonInterFace.GSMV2CarrierParameter.CarrierOne && !JsonInterFace.GSMV2CarrierParameter.CarrierTwo)
                {
                    MessageBox.Show("请选择GSMV2载波！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true;
        }
    }
}
