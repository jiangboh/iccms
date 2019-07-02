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
    public partial class ScannerInfoWindow : Window
    {
        private List<LTE_ScannerInformation> scannerInformationList = new List<LTE_ScannerInformation>();
        private static Thread ScannerInfoThread = null;
        private static ObservableCollection<LTE_ScannerInformation> ScannerInfoTable = new ObservableCollection<LTE_ScannerInformation>();
        private static ObservableCollection<LTE_ScannerInformation> ScannerNeCellInfoTable = new ObservableCollection<LTE_ScannerInformation>();

        public ScannerInfoWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (ScannerInfoThread == null)
            {
                ScannerInfoThread = new Thread(new ThreadStart(ScannerInformations));
                ScannerInfoThread.Start();
            }
        }
        private void ScannerInformations()
        {
            while (true)
            {
                try
                {
                    lock (JsonInterFace.ScannerInformation.Scannernock)
                    {
                        if (JsonInterFace.ScannerInformation.ScannerInforTable.Rows.Count > 0)
                        {
                            bool isRepeat = false;
                            DataRow dr = JsonInterFace.ScannerInformation.ScannerInforTable.Rows[0];
                            Dispatcher.Invoke(() =>
                            {
                                for (int i = 0; i < ScannerInfoTable.Count; i++)
                                {
                                    if (dr["CellID"].ToString() == ScannerInfoTable[i].CellID)
                                    {
                                        isRepeat = true;
                                        break;
                                    }
                                }
                                if (!isRepeat)
                                {
                                    ScannerInfoTable.Add(new LTE_ScannerInformation()
                                    {
                                        CellID = dr["CellID"].ToString(),
                                        Frequency = dr["Frequency"].ToString(),
                                        PLMN1 = dr["PLMN"].ToString(),
                                        ScramblingCode = dr["ScramblingCode"].ToString(),
                                        TAC_LAC = dr["TAC_LAC"].ToString(),
                                        Intensity = dr["Intensity"].ToString(),
                                        CellNeighInfo = dr["CellNeighInfo"].ToString(),
                                        CellNeighLevel = dr["CellNeighLevel"].ToString(),
                                        Level = dr["Level"].ToString()
                                    });
                                }

                            });
                            JsonInterFace.ScannerInformation.ScannerInforTable.Rows.RemoveAt(0);
                        }
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

                txtCommunityID.DataContext = new Language_EN.ScannerInfos();
                txtPLMN.DataContext = new Language_EN.ScannerInfos();
                txtFrePoint.DataContext = new Language_EN.ScannerInfos();
                txtScrambler.DataContext = new Language_EN.ScannerInfos();
                txtTLAC.DataContext = new Language_EN.ScannerInfos();
                txtSignalStrength.DataContext = new Language_EN.ScannerInfos();
                txtCellNeighInfo.DataContext = new Language_EN.ScannerInfos();
                txtPriority.DataContext = new Language_EN.ScannerInfos();
                txtCellNeighPriority.DataContext = new Language_EN.ScannerInfos();
            }
            else
            {
                this.DataContext = new Language_CN.ScannerInfos();
                btnClose.DataContext = new Language_CN.ScannerInfos();

                txtCommunityID.DataContext = new Language_CN.ScannerInfos();
                txtPLMN.DataContext = new Language_CN.ScannerInfos();
                txtFrePoint.DataContext = new Language_CN.ScannerInfos();
                txtScrambler.DataContext = new Language_CN.ScannerInfos();
                txtTLAC.DataContext = new Language_CN.ScannerInfos();
                txtSignalStrength.DataContext = new Language_CN.ScannerInfos();
                txtCellNeighInfo.DataContext = new Language_CN.ScannerInfos();
                txtPriority.DataContext = new Language_CN.ScannerInfos();
                txtCellNeighPriority.DataContext = new Language_CN.ScannerInfos();
            }
            dgScannerInfo.ItemsSource = null;
            dgScannerInfo.Items.Clear();
            dgScannerInfo.Items.Refresh();
            ScannerInfoTable.Clear();
            dgScannerInfo.ItemsSource = ScannerInfoTable;
            ScannerNeCellInfoTable.Clear();
            dgScannerNCellInfo.ItemsSource = ScannerNeCellInfoTable;

        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mmRefresh_Click(object sender, RoutedEventArgs e)
        {
            dgScannerInfo.Items.Refresh();
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

        private void dgScannerInfo_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {           
            LTE_ScannerInformation scannerNeCell = (LTE_ScannerInformation)dgScannerInfo.SelectedItem;
            if (scannerNeCell != null) 
            {
                ScannerNeCellInfoTable.Clear();
                string[] _cellNeighInfos = scannerNeCell.CellNeighInfo.ToString().Split(new char[] { '\n' });
                string[] _cellNeighLevels = scannerNeCell.CellNeighLevel.ToString().Split(new char[] { '\n' });
                for (int i = 0; i < _cellNeighInfos.Length; i++)
                {
                    ScannerNeCellInfoTable.Add(new LTE_ScannerInformation()
                    {
                        CellNeighInfo = _cellNeighInfos[i].ToString(),
                        CellNeighLevel = _cellNeighLevels[i].ToString()
                    });
                }
            }       
        }
    }
}
