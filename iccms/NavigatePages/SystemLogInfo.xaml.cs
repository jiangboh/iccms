using DataInterface;
using IODataControl;
using JsonLib;
using Newtonsoft.Json;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace iccms.NavigatePages
{
    /// <summary>
    /// SystemLogInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SystemLogInfo : Page
    {
        private object LanguageClass = null;

        //构造
        public SystemLogInfo()
        {
            InitializeComponent();
        }

        //保存系统日志
        private void mmSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.SelfSystemLog.Count <= 0)
            {
                MessageBox.Show("系统日志为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //按扭灰色
            (sender as MenuItem).IsEnabled = false;

            int DataCount = 0;
            Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
            svd.Filter = "(文本文件 *.txt)|*.txt";
            svd.AddExtension = true;
            svd.CheckPathExists = true;

            if (!(bool)svd.ShowDialog())
            {
                return;
            }

            ObservableCollection<SystemLogsInformation> SaveSystemLog = null;
            lock (MainWindow.LogsListsLocker)
            {
                SaveSystemLog = MainWindow.SelfSystemLog;
            }

            new Thread(() =>
            {
                try
                {
                    for (int i = 0; i < SaveSystemLog.Count; i++)
                    {
                        string DTime = SaveSystemLog[i].DTime;
                        string Object = SaveSystemLog[i].Object;
                        string Action = SaveSystemLog[i].Action;
                        string Other = SaveSystemLog[i].Other;
                        System.IO.File.AppendAllText(svd.FileName, (DTime + "  " + Object + "  " + Action + "  " + Other) + Environment.NewLine);
                        DataCount++;
                    }

                    if (DataCount != 0)
                    {
                        MessageBox.Show("导出系统日志到文件[" + svd.FileName + "]成功,共[" + DataCount.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    //按扭恢复
                    (sender as MenuItem).IsEnabled = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("导出系统日志到文件[" + svd.FileName + "]失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).Start();
        }

        //导出系统日志
        private void mmExport_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.SelfSystemLog.Count <= 0)
            {
                MessageBox.Show("系统日志为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //按扭灰色
            (sender as MenuItem).IsEnabled = false;

            Microsoft.Win32.SaveFileDialog svd = new Microsoft.Win32.SaveFileDialog();
            svd.Filter = "(Excel文件 *.xls)|*.xls|(Excel文件 *.xlsx)|*.xlsx";
            svd.AddExtension = true;
            svd.CheckPathExists = true;

            if (!(bool)svd.ShowDialog())
            {
                return;
            }

            new Thread(() => 
            {
                try
                {
                    using (ExcelHelper excelHelper = new ExcelHelper(@svd.FileName))//定义一个范围，在范围结束时处理对象
                    {
                        DataTable exportDr = JsonInterFace.SystemLogsInfo.SysDataInformation.Clone();
                        exportDr.Rows.Clear();
                        ObservableCollection<SystemLogsInformation> ExportSystemLog = null;

                        lock (MainWindow.LogsListsLocker)
                        {
                            ExportSystemLog = MainWindow.SelfSystemLog;
                        }

                        for (int i = 0; i < ExportSystemLog.Count; i++)
                        {
                            DataRow dr = exportDr.NewRow();
                            dr[0] = ExportSystemLog[i].DTime;
                            dr[1] = ExportSystemLog[i].Object;
                            dr[2] = ExportSystemLog[i].Action;
                            dr[3] = ExportSystemLog[i].Other;
                            exportDr.Rows.Add(dr);
                        }

                        int res = excelHelper.DataTableToExcelForSysLogs(exportDr, "Sheet1", true);

                        if (res != -1)
                        {
                            MessageBox.Show("导出系统日志到文件[" + svd.FileName + "]成功,共[" + res.ToString() + "]条数据！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    //按扭恢复
                    (sender as MenuItem).IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出系统日志到文件[" + svd.FileName + "]失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }).Start();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //应用语言
            try
            {
                //初始化语言
                if (Parameters.LanguageType==("CN") || Parameters.LanguageType==(""))
                {
                    LanguageClass = new Language_CN.MainStatusInformation();
                }
                else if (Parameters.LanguageType==("EN"))
                {
                    LanguageClass = new Language_EN.MainStatusInformation();
                }


                if (Parameters.LanguageType==("CN") || Parameters.LanguageType==(""))
                {
                    txtBlockDTime.DataContext = (Language_CN.MainStatusInformation)LanguageClass;
                    txtBlockObject.DataContext = (Language_CN.MainStatusInformation)LanguageClass;
                    txtBlockAction.DataContext = (Language_CN.MainStatusInformation)LanguageClass;
                    txtBlockOther.DataContext = (Language_CN.MainStatusInformation)LanguageClass;
                }
                else
                {
                    txtBlockDTime.DataContext = (Language_EN.MainStatusInformation)LanguageClass;
                    txtBlockObject.DataContext = (Language_EN.MainStatusInformation)LanguageClass;
                    txtBlockAction.DataContext = (Language_EN.MainStatusInformation)LanguageClass;
                    txtBlockOther.DataContext = (Language_EN.MainStatusInformation)LanguageClass;
                }

                //倒序
                lock (MainWindow.LogsListsLocker)
                {
                    MainWindow.SelfSystemLog = new ObservableCollection<SystemLogsInformation>(MainWindow.SelfSystemLog.OrderByDescending(item => item.DTime));
                }
                SystemLogsInfoDataGrid.ItemsSource = MainWindow.SelfSystemLog;
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void mmRefreash_Click(object sender, RoutedEventArgs e)
        {
            SystemLogsInfoDataGrid.ItemsSource = null;
            //倒序
            SystemLogsInfoDataGrid.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Descending;
            SystemLogsInfoDataGrid.ItemsSource = MainWindow.SelfSystemLog;
        }

        private void SystemLogsInfoDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SystemLogsInfoDataGrid.SelectedItem != null)
                {
                    int RowIndex = SystemLogsInfoDataGrid.SelectedIndex;
                    StringBuilder ItemEvents = new StringBuilder();

                    string LogTime = ((SystemLogsInfoDataGrid.Items[RowIndex]) as SystemLogsInformation).DTime;
                    string LogObject = ((SystemLogsInfoDataGrid.Items[RowIndex]) as SystemLogsInformation).Object;
                    string LogAction = ((SystemLogsInfoDataGrid.Items[RowIndex]) as SystemLogsInformation).Action;
                    string LogOther = ((SystemLogsInfoDataGrid.Items[RowIndex]) as SystemLogsInformation).Other;
                    ItemEvents.AppendLine("[日期]：" + Environment.NewLine + LogTime);
                    ItemEvents.AppendLine("[详细信息]：" + Environment.NewLine + LogObject);
                    ItemEvents.AppendLine("[操作类型]：" + Environment.NewLine + LogAction);
                    ItemEvents.AppendLine("[其它]：" + Environment.NewLine + LogOther);
                    MessageBox.Show(ItemEvents.ToString(), "事件详细信息");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "事件详细信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void mmLogsClear_Click(object sender, RoutedEventArgs e)
        {
            lock (MainWindow.LogsListsLocker)
            {
                MainWindow.SelfSystemLog.Clear();
            }
        }

        private void gdBlockDTime_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockDTime.Source.ToString(), ref ImgBlockDTime);
        }

        private void gdBlockObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockObject.Source.ToString(), ref ImgBlockObject);
        }

        private void gdBlockAction_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockAction.Source.ToString(), ref ImgBlockAction);
        }

        private void gdBlockOther_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColumnOderChanged(ImgBlockOther.Source.ToString(), ref ImgBlockOther);
        }

        //点击排序图标标示
        private void ColumnOderChanged(string ImgSource, ref Image SelfIcon)
        {
            if (ImgSource == new ColumnOderImage().ASC)
            {
                SelfIcon.Source = new BitmapImage(new Uri(new ColumnOderImage().DESC, UriKind.RelativeOrAbsolute));
            }
            else
            {
                SelfIcon.Source = new BitmapImage(new Uri(new ColumnOderImage().ASC, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
