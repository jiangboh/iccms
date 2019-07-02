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
    /// UnknownDevieReNameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnknownDevieReNameWindow : Window
    {
        public UnknownDevieReNameWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UnKnownDeviceListsControlWindow.UnknownDeviceReName.UnknownNewName == "")
                {
                    MessageBox.Show("请输入或选择新设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (!UnKnownDeviceListsControlWindow.UnknownDeviceReName.NameOverride)
                    {
                        for (int i = 0; i < SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList.Count; i++)
                        {
                            if (SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList[i].SelfName
                                == UnKnownDeviceListsControlWindow.UnknownDeviceReName.UnknownNewName)
                            {
                                MessageBox.Show("重名不允许覆盖时，请输入盖站点下不相同的设备名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
            DialogResult = true;
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtSourceName.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            chkOverrideTips.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            txtNewName.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            rdbSourceDeviceList.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            rdbCustomDeviceName.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            cbbNewName.DataContext = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName;
            cbbNewName.ItemsSource = SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.StationDeviceNameList;

            //初始默认值
            SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.UnknownNewName = string.Empty;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (btnCansel.IsFocused)
                {
                    this.DragMove();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnCansel.Focus();
        }

        private void cbbNewName_DropDownOpened(object sender, EventArgs e)
        {
            if (cbbNewName.Items.Count <= 0)
            {
                MessageBox.Show("当前列表为空，若选用原有设备名作为新的设备名，请先选择站点！\n 否则可以直接输入新的设备名！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void rdbSourceDeviceList_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.SelectedCustomEnable = Visibility.Collapsed;
            SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.SelectedListEnable = Visibility.Visible;
        }

        private void rdbCustomDeviceName_Click(object sender, RoutedEventArgs e)
        {
            SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.SelectedCustomEnable = Visibility.Visible;
            SubWindow.UnKnownDeviceListsControlWindow.UnknownDeviceReName.SelectedListEnable = Visibility.Collapsed;
        }
    }
}
