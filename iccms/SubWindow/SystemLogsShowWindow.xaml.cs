using DataInterface;
using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace iccms.SubWindow
{
    /// <summary>
    /// SystemLogsShowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SystemLogsShowWindow : Window
    {
        private class LogsStatusBarInfoClass : INotifyPropertyChanged
        {
            private int _maxCount;
            private int _StepValue;
            private Visibility _progressBarEnable = Visibility.Collapsed;
            private bool _buttonEnable = true;

            public int MaxCount
            {
                get
                {
                    return _maxCount;
                }

                set
                {
                    _maxCount = value;
                    NotifyPropertyChanged("MaxCount");
                }
            }

            public int StepValue
            {
                get
                {
                    return _StepValue;
                }

                set
                {
                    _StepValue = value;
                    NotifyPropertyChanged("StepValue");
                }
            }

            public Visibility ProgressBarEnable
            {
                get
                {
                    return _progressBarEnable;
                }

                set
                {
                    _progressBarEnable = value;
                    NotifyPropertyChanged("ProgressBarEnable");
                }
            }

            public bool ButtonEnable
            {
                get
                {
                    return _buttonEnable;
                }

                set
                {
                    _buttonEnable = value;
                    NotifyPropertyChanged("ButtonEnable");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void NotifyPropertyChanged(String value)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(value));
                }
            }
        }
        DataTable SystemLogsTab = null;
        LogsStatusBarInfoClass LogsStatusBarInfo = new LogsStatusBarInfoClass();

        string DTimeStart = string.Empty;
        string DTimeEnd = string.Empty;
        string Events = string.Empty;
        string DetailMessage = string.Empty;
        string ReasonsTracking = string.Empty;

        public SystemLogsShowWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;

            if (SystemLogsTab == null)
            {
                SystemLogsTab = new DataTable("SystemLogs");

                DataColumn DataColumn0 = new DataColumn();
                DataColumn0.DataType = System.Type.GetType("System.String");
                DataColumn0.ColumnName = "ID";

                DataColumn DataColumn1 = new DataColumn();
                DataColumn1.DataType = System.Type.GetType("System.String");
                DataColumn1.ColumnName = "DateTimes";

                DataColumn DataColumn2 = new DataColumn();
                DataColumn2.DataType = System.Type.GetType("System.String");
                DataColumn2.ColumnName = "Operations";

                DataColumn DataColumn3 = new DataColumn();
                DataColumn3.DataType = System.Type.GetType("System.String");
                DataColumn3.ColumnName = "Messages";

                DataColumn DataColumn4 = new DataColumn();
                DataColumn4.DataType = System.Type.GetType("System.String");
                DataColumn4.ColumnName = "ErrorStatus";

                SystemLogsTab.Columns.Add(DataColumn0);
                SystemLogsTab.Columns.Add(DataColumn1);
                SystemLogsTab.Columns.Add(DataColumn2);
                SystemLogsTab.Columns.Add(DataColumn3);
                SystemLogsTab.Columns.Add(DataColumn4);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnLogsQuery.DataContext = LogsStatusBarInfo;
            btnLogsClear.DataContext = LogsStatusBarInfo;
            btnLogsDelete.DataContext = LogsStatusBarInfo;
            btnClose.DataContext = LogsStatusBarInfo;
            prgStatusBar.DataContext = LogsStatusBarInfo;

            dtpDatatimeStart.SelectedDate = DateTime.Now;
            dtpDatatimeEnd.SelectedDate = DateTime.Now;

            //加载数据
            JsonInterFace.IODataHelper.LoadLogs(ref SystemLogsTab);
            dgSystemEventsLogs.ItemsSource = SystemLogsTab.DefaultView;

            #region 权限
            if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
            {
                if (RoleTypeClass.RoleType.Equals("RoleType"))
                {
                    if (!RoleTypeClass.RolePrivilege.ContainsKey("系统日志"))
                    {
                        btnLogsClear.IsEnabled = false;
                        btnLogsDelete.IsEnabled = false;
                    }
                    else
                    {
                        btnLogsClear.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统日志"]));
                        btnLogsDelete.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["系统日志"]));
                    }
                }
            }
            #endregion
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            btnClose.Focus();
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

        private void dgSystemEventsLogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dgSystemEventsLogs.CurrentColumn != null)
                {
                    int ColumeIndex = dgSystemEventsLogs.CurrentColumn.DisplayIndex;
                    if (ColumeIndex == 3)
                    {
                        StringBuilder ItemEvents = new StringBuilder();
                        ItemEvents.AppendLine("时间：" + (dgSystemEventsLogs.SelectedValue as DataRowView).Row[ColumeIndex - 2].ToString());
                        ItemEvents.AppendLine("事件：" + (dgSystemEventsLogs.SelectedValue as DataRowView).Row[ColumeIndex - 1].ToString());
                        ItemEvents.AppendLine("详细信息：" + Environment.NewLine + (dgSystemEventsLogs.SelectedValue as DataRowView).Row[ColumeIndex - 0].ToString());
                        MessageBox.Show(ItemEvents.ToString(), "事件详细信息");
                    }
                    else
                    {
                        StringBuilder ItemEvents = new StringBuilder();
                        ItemEvents.AppendLine("详细信息：" + Environment.NewLine + (dgSystemEventsLogs.SelectedValue as DataRowView).Row[ColumeIndex].ToString());
                        MessageBox.Show(ItemEvents.ToString(), "事件详细信息");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "事件详细信息", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogsQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dtpDatatimeStart.SelectedDate != null)
                {
                    DTimeStart = ((DateTime)dtpDatatimeStart.SelectedDate).ToString("yyyy-MM-dd") + " 00:00:00";
                }
                else
                {
                    DTimeStart = DateTime.Now.ToShortDateString() + " 00:00:00";
                }

                if (dtpDatatimeEnd.SelectedDate != null)
                {
                    DTimeEnd = ((DateTime)dtpDatatimeEnd.SelectedDate).ToString("yyyy-MM-dd") + " 23:59:59";
                }
                else
                {
                    DTimeEnd = DateTime.Now.ToShortDateString() + " 23:59:59";
                }

                if (dtpDatatimeStart.SelectedDate != null && dtpDatatimeEnd.SelectedDate != null)
                {
                    if (dtpDatatimeStart.SelectedDate > dtpDatatimeEnd.SelectedDate)
                    {
                        MessageBox.Show("选择开始日期大于结束日期！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (txtEvent.Text != "")
                {
                    Events = txtEvent.Text;
                }
                if (txtDetailMessage.Text != "")
                {
                    DetailMessage = txtDetailMessage.Text;
                }
                if (txtReasonsTrackingMessage.Text != "")
                {
                    ReasonsTracking = txtReasonsTrackingMessage.Text;
                }

                //查询数据
                if (dtpDatatimeEnd.SelectedDate == null && dtpDatatimeEnd.SelectedDate == null && Events == "" && DetailMessage == "" && ReasonsTracking == "")
                {
                    SystemLogsTab.Rows.Clear();
                    dgSystemEventsLogs.ItemsSource = null;

                    JsonInterFace.IODataHelper.LoadLogs(ref SystemLogsTab);

                    dgSystemEventsLogs.ItemsSource = SystemLogsTab.DefaultView;

                }
                else
                {
                    SystemLogsTab.Rows.Clear();
                    dgSystemEventsLogs.ItemsSource = null;
                    JsonInterFace.IODataHelper.LoadLogs(ref SystemLogsTab, DTimeStart, DTimeEnd, Events, DetailMessage, ReasonsTracking);
                    dgSystemEventsLogs.ItemsSource = SystemLogsTab.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("日志查询，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogsClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定清空所有系统日志？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    new Thread(() =>
                    {
                        JsonInterFace.IODataHelper.LogsClear();
                        MessageBox.Show("清空系统日志成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                        Dispatcher.Invoke(() =>
                        {
                            //查询数据
                            btnLogsQuery_Click(sender, e);
                        });
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("清空系统日志失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogsDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("确定删除列表中所有系统日志？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    new Thread(() =>
                    {
                        LogsStatusBarInfo.MaxCount = SystemLogsTab.Rows.Count - 1;
                        LogsStatusBarInfo.ProgressBarEnable = Visibility.Visible;
                        LogsStatusBarInfo.ButtonEnable = false;

                        for (int i = 0; i < SystemLogsTab.Rows.Count; i++)
                        {
                            string ID = SystemLogsTab.Rows[i]["ID"].ToString();
                            string DateTimes = SystemLogsTab.Rows[i]["DateTimes"].ToString();
                            string Operations = SystemLogsTab.Rows[i]["Operations"].ToString();
                            JsonInterFace.IODataHelper.LogsDelete(ID, DateTimes, Operations);

                            //进度
                            LogsStatusBarInfo.StepValue = i;
                        }

                        MessageBox.Show("删除系统日志成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        LogsStatusBarInfo.ProgressBarEnable = Visibility.Collapsed;
                        LogsStatusBarInfo.ButtonEnable = true;

                        Dispatcher.Invoke(() =>
                        {
                            //查询数据
                            btnLogsQuery_Click(sender, e);
                        });
                    }).Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除系统日志失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
