using DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// UpDownLoadFilesWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UpDownLoadFilesWindow : Window
    {
        public UpDownLoadFilesWindow()
        {
            InitializeComponent();
        }

        private void FrmUpDownLoadFileAndTiptoolBar_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = SystemParameters.PrimaryScreenWidth - (SystemParameters.PrimaryScreenWidth - (this.Width) + 30);
            this.Top = SystemParameters.PrimaryScreenHeight - (SystemParameters.PrimaryScreenHeight - (this.Height) + 20);

            UploadProgressBar.DataContext = JsonInterFace.ProgressBarInfo;

            new Thread(() =>
            {
                JsonInterFace.ProgressBarInfo.MaxValue = 100;
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        for (int i = 0; i < JsonInterFace.ProgressBarInfo.MaxValue; i++)
                        {
                            if (!JsonInterFace.ProgressBarInfo.RunProgressBar) { break; }
                            JsonInterFace.ProgressBarInfo.StepValue = i + 1;
                            Thread.Sleep(10);
                        }
                    });

                    if (!JsonInterFace.ProgressBarInfo.RunProgressBar) { break; }
                }
                this.Close();
            }).Start();
        }

        private void FrmUpDownLoadFileAndTiptoolBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
