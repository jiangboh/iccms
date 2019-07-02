using DataInterface;
using ParameterControl;
using System.Windows;
using System.Windows.Controls;

namespace iccms.NavigatePages
{
    /// <summary>
    /// DeviceLoadingStatusWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceLoadingStatusWindow : Page
    {
        public DeviceLoadingStatusWindow()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DeviceLoadingStatuBar.DataContext = JsonInterFace.DeviceListRequestCompleteStatus;
            if (System.IO.File.Exists(@"Lib/SysImg/Bg.gif"))
            {
                this.PictureOfGif.Image = System.Drawing.Image.FromFile(@"Lib/SysImg/Bg.gif");
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DeviceLoadingStatuBar.DataContext = null;
        }

        private void mmReLoading_Click(object sender, RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_DeviceListInfoLoad, 0, 0);
        }

        private void mmAbortLoading_Click(object sender, RoutedEventArgs e)
        {
            JsonInterFace.DeviceListRequestCompleteStatus.LoadingWindowStatu = Visibility.Collapsed;
        }
    }
}
