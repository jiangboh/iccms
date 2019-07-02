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
    /// ManageField.xaml 的交互逻辑
    /// </summary>
    public partial class ManageField : Window
    {
        private Dictionary<string, Uri> DeviceListWindow = new Dictionary<string, Uri>();
        private object LanguageClass = null;
        public ManageField()
        {
            InitializeComponent();
            DeviceListWindow.Add("DeviceListWindow", new Uri("NavigatePages/DeviceListWindow.xaml", UriKind.Relative));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //载入设备列表
            DeviceListFrm.Navigate(DeviceListWindow["DeviceListWindow"]);
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                LanguageClass = new Language_CN.ManageField();
                this.DataContext = (Language_CN.ManageField)LanguageClass;
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                LanguageClass = new Language_EN.ManageField();
                this.DataContext = (Language_EN.ManageField)LanguageClass;
            }
        }
    }
}
