using DataInterface;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace iccms.SubWindow
{
    /// <summary>
    /// CDMAIMSIListInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CDMAIMSIListInputWindow : Window
    {
        public class IMSIControlInfoClass
        {
            private string iMSI;
            private string actionFlag;
            public string IMSI
            {
                get
                {
                    return iMSI;
                }
                set
                {
                    iMSI = value;
                }
            }
            public string ActionFlag
            {
                get
                {
                    return actionFlag;
                }
                set
                {
                    actionFlag = value;
                }
            }
        }

        public static IMSIControlInfoClass IMSIControlInfo = new IMSIControlInfoClass();

        public static ObservableCollection<IMSIControlInfoClass> IMSIInfoList = new ObservableCollection<IMSIControlInfoClass>();
        public CDMAIMSIListInputWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        private void btnIMSIClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IMSIInfoList.Clear();
                System.Windows.MessageBox.Show("清空IMSI导入列表成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "清空IMSI导入列表失败!", ex.Message, ex.StackTrace);
                System.Windows.MessageBox.Show("清空IMSI导入列表失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnIMSIReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FrmMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dgCDMAIMSIList.ItemsSource = IMSIInfoList;
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {

        }
    }
}
