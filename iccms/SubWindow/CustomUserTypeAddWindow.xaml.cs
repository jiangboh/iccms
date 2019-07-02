using DataInterface;
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

namespace iccms.SubWindow
{
    /// <summary>
    /// CustomUserTypeAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomUserTypeAddWindow : Window
    {
        public CustomUserTypeAddWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lblBackGroundColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Color bgColor = new Color();
            if (Parameters.GettingColor(ref bgColor))
            {
                lblBackGroundColor.Background = new SolidColorBrush(bgColor);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                JsonInterFace.IODataHelper.CustomUserTypeSetting((bool)chkSetting.IsChecked, txtUserType.Text, lblBackGroundColor.Background.ToString(), (bool)chkAlert.IsChecked);
                MessageBox.Show("保存成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
