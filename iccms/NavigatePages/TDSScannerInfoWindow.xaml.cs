using DataInterface;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

namespace iccms.NavigatePages
{
    /// <summary>
    /// ScannerInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TDSScannerInfoWindow : Window
    {
        private List<TDS_ScannerInformation> TDSscannerInformationList = new List<TDS_ScannerInformation>();
        private static Thread TDSScannerInfoThread = null;
        private static ObservableCollection<TDS_ScannerInformation> TDSScannerInfoTable = new ObservableCollection<TDS_ScannerInformation>();
        public TDSScannerInfoWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (TDSScannerInfoThread == null)
            {
                TDSScannerInfoThread = new Thread(new ThreadStart(TDSScannerInformations));
                TDSScannerInfoThread.Start();
            }
        }
        private void TDSScannerInformations()
        {
            while (true)
            {
                try
                {
                    if (JsonInterFace.TDSScannerInformation.ScannerInforTable.Rows.Count > 0)
                    {
                        bool isRepeat = false;
                        DataRow dr = JsonInterFace.TDSScannerInformation.ScannerInforTable.Rows[0];
                        Dispatcher.Invoke(() =>
                        {
                            for (int i = 0; i < TDSScannerInfoTable.Count; i++)
                            {
                                if (dr["CellID"].ToString() == TDSScannerInfoTable[i].CellID)
                                {
                                    isRepeat = true;
                                    break;
                                }
                            }
                            if (!isRepeat)
                            {
                                TDSScannerInfoTable.Add(new TDS_ScannerInformation()
                                {
                                    CellID = dr["CellID"].ToString(),
                                    UARFCN = dr["UARFCN"].ToString(),
                                    RSCP = dr["RSCP"].ToString(),
                                    RSSI = dr["RSSI"].ToString(),
                                    LAC = dr["LAC"].ToString(),
                                    CI = dr["CI"].ToString()
                                });
                            }
                        });
                        JsonInterFace.TDSScannerInformation.ScannerInforTable.Rows.RemoveAt(0);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.LanguageType.Equals("EN"))
            {
                this.DataContext = new Language_EN.ScannerInfos();
                btnClose.DataContext = new Language_EN.ScannerInfos();

                txtCellID.DataContext = new Language_EN.ScannerInfos();
                txtUARFCN.DataContext = new Language_EN.ScannerInfos();
                txtRSCP.DataContext = new Language_EN.ScannerInfos();
                txtRSSI.DataContext = new Language_EN.ScannerInfos();
                txtLAC.DataContext = new Language_EN.ScannerInfos();
                txtCI.DataContext = new Language_EN.ScannerInfos();
            }
            else
            {
                this.DataContext = new Language_CN.ScannerInfos();
                btnClose.DataContext = new Language_CN.ScannerInfos();

                txtCellID.DataContext = new Language_CN.ScannerInfos();
                txtUARFCN.DataContext = new Language_CN.ScannerInfos();
                txtRSCP.DataContext = new Language_CN.ScannerInfos();
                txtRSSI.DataContext = new Language_CN.ScannerInfos();
                txtLAC.DataContext = new Language_CN.ScannerInfos();
                txtCI.DataContext = new Language_CN.ScannerInfos();
            }
            dgTDSScannerInfo.ItemsSource = null;
            dgTDSScannerInfo.Items.Clear();
            dgTDSScannerInfo.Items.Refresh();
            TDSScannerInfoTable.Clear();
            dgTDSScannerInfo.ItemsSource = TDSScannerInfoTable;
        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmRefresh_Click(object sender, RoutedEventArgs e)
        {
            dgTDSScannerInfo.Items.Refresh();
        }

        private void mmDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void IsCheckAll_Click(object sender, RoutedEventArgs e)
        {

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
