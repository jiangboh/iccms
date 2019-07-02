using DataInterface;
using ParameterControl;
using System;
using System.Threading;
using System.Windows.Controls;

namespace iccms.NavigatePages
{
    /// <summary>
    /// Menu.xaml 的交互逻辑
    /// </summary>
    public partial class Menu : Page
    {
        private object loginLanguageClass = null;
        private Thread RoleTypeTimeThread = null;
        public Menu()
        {
            InitializeComponent();

            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                loginLanguageClass = new Language_CN.MainMenu();
            }
            else if (Parameters.LanguageType.Equals("EN"))
            {
                loginLanguageClass = new Language_EN.MainMenu();
            }
        }

        private void MainMenuPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //中/英文初始化
            if (Parameters.LanguageType.Equals("CN") || Parameters.LanguageType.Equals(""))
            {
                this.DataContext = (Language_CN.MainMenu)loginLanguageClass;
                MainMenu.DataContext = (Language_CN.MainMenu)loginLanguageClass;
                miDeviceManage.DataContext = (Language_CN.MainMenu)loginLanguageClass;
                miNameListManage.DataContext = (Language_CN.MainMenu)loginLanguageClass;
                miDataManage.DataContext = (Language_CN.MainMenu)loginLanguageClass;
            }
            else
            {
                this.DataContext = (Language_EN.MainMenu)loginLanguageClass;
                MainMenu.DataContext = (Language_EN.MainMenu)loginLanguageClass;
                miDeviceManage.DataContext = (Language_EN.MainMenu)loginLanguageClass;
                miNameListManage.DataContext = (Language_EN.MainMenu)loginLanguageClass;
                miDataManage.DataContext = (Language_EN.MainMenu)loginLanguageClass;
            }
            #region 权限 
            if (RoleTypeTimeThread == null)
            {
                RoleTypeTimeThread = new Thread(new ThreadStart(RoleType));
                RoleTypeTimeThread.Start();
            }
            #endregion
        }
        private void RoleType()
        {
            while (true)
            {
                try
                {
                    if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (RoleTypeClass.RoleType.Equals("RoleType"))
                            {
                                //设备管理,特殊名单管理,历史记录管理,用户管理,域管理,系统管理,高级设置,捕号窗口,黑名单窗口,状态窗口,系统日志,账号管理,用户组管理
                                //miDeviceManage.IsEnabled = Convert.ToBoolean(int.Parse(RoleTypeClass.RolePrivilege["设备管理"]));
                                for (int i = 0; i < MainMenu.Items.Count - 1; i++)
                                {
                                    if (i == 0)
                                    {
                                        for (int j = 0; j < ((MenuItem)MainMenu.Items[i]).Items.Count - 1; j++)
                                        {
                                            if (!RoleTypeClass.RolePrivilege.ContainsKey(((MenuItem)((MenuItem)MainMenu.Items[i]).Items[j]).Header.ToString()))
                                            {
                                                ((MenuItem)((MenuItem)MainMenu.Items[i]).Items[j]).Visibility = System.Windows.Visibility.Collapsed;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j < ((MenuItem)MainMenu.Items[i]).Items.Count; j++)
                                        {
                                            if (!RoleTypeClass.RolePrivilege.ContainsKey(((MenuItem)((MenuItem)MainMenu.Items[i]).Items[j]).Header.ToString()))
                                            {
                                                ((MenuItem)((MenuItem)MainMenu.Items[i]).Items[j]).Visibility = System.Windows.Visibility.Collapsed;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (int.Parse(RoleTypeClass.RoleType) > 1)
                                {
                                    //高级设置
                                    miAdvanceSetting.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                if (int.Parse(RoleTypeClass.RoleType) > 2)
                                {
                                    //用户管理、域管理
                                    miUserManage.Visibility = System.Windows.Visibility.Collapsed;
                                    miDeviceDomainNameManage.Visibility = System.Windows.Visibility.Collapsed;
                                }
                            }
                        });
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("获取用户组权限异常", ex.Message, ex.StackTrace);
                }
            }
        }
        private void miDeviceManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.ConfigType = DeviceType.UnknownType;
            DeviceManagerWindow DeviceManagerFrom = new DeviceManagerWindow();
            DeviceManagerFrom.ShowDialog();
        }

        private void miNameListManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                NameListManage NameListManageFrom = new NameListManage();
                NameListManageFrom.LoadDeviceListTreeView();
                #region 权限 
                if (RoleTypeClass.RoleType != null && RoleTypeClass.RoleType != "")
                {
                    if (RoleTypeClass.RoleType.Equals("RoleType"))
                    {

                    }
                    else
                    {
                        if (Parameters.ISDigital(RoleTypeClass.RoleType))
                        {
                            if (int.Parse(RoleTypeClass.RoleType) > 3)
                            {
                                //黑白名单
                                NameListManageFrom.btnBlackAdd.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnBlackEdit.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnBlackDelete.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteAdd.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteEdit.Visibility = System.Windows.Visibility.Collapsed;
                                NameListManageFrom.btnWhiteDelete.Visibility = System.Windows.Visibility.Collapsed;
                            }
                        }
                    }
                }
                #endregion
                NameListManageFrom.ShowDialog();
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }

        private void miUserManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UserManageWindow UserManageFrm = new UserManageWindow();
            UserManageFrm.ShowDialog();
        }

        private void miDataManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataManage DataManageFrm = new DataManage();
            DataManageFrm.ShowDialog();
        }

        private void miDeviceDomainNameManage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DomainManage ManageFieldFrm = new DomainManage();
            ManageFieldFrm.ShowDialog();
        }

        private void miSystemSetting_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SettingSym SettingSymFrm = new SettingSym();
            SettingSymFrm.ShowDialog();
        }

        private void miAdvanceSetting_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AdvanceSetting AdvanceSettingFrm = new AdvanceSetting();
            AdvanceSettingFrm.ShowDialog();
        }

        private void miAbout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.AboutWindow aboutWin = new SubWindow.AboutWindow();
            aboutWin.ShowDialog();
        }

        private void miSystemLogsWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.SystemLogsShowWindow SystemLogsShowWin = new SubWindow.SystemLogsShowWindow();
            SystemLogsShowWin.ShowDialog();
        }

        private void miExit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_CLOSE, 0, 0);
        }

        private void miSystemStatuViewWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.SystemStatusViewWindow SystemStatusViewWin = new SubWindow.SystemStatusViewWindow();
            SystemStatusViewWin.Topmost = true;
            SystemStatusViewWin.Show();
        }

        private void miUnknownDeviceControlWindowShow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 1;
            Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = System.Windows.Visibility.Visible;
            Parameters.UnknownDeviceWindowControlParameters.ActionType = ActionTypeList.Show.ToString();
        }

        private void miUnknownDeviceControlWindowHide_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.UnknownDeviceWindowControlParameters.DevcieTreeRowSpan = 2;
            Parameters.UnknownDeviceWindowControlParameters.UnknownDeviceTipsBlockAttribute = System.Windows.Visibility.Collapsed;
            Parameters.UnknownDeviceWindowControlParameters.ActionType = ActionTypeList.Hide.ToString();
        }
        private void miUnknownDeviceControlWindowAuto_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.UnknownDeviceWindowControlParameters.ActionType = ActionTypeList.Auto.ToString();
        }

        private void miBaseParameterSetting_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.BaseParameterSettingWindow BaseParameterSettingWin = new SubWindow.BaseParameterSettingWindow();
            BaseParameterSettingWin.Show();
        }

        /// <summary>
        /// 只显示捕号窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miScannerWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowScannerWindowControl, 0, 0);
        }

        /// <summary>
        /// 黑名单追踪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miBlackListWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowMeasurementReportWindowControl, 0, 0);
        }

        /// <summary>
        /// 系统日志状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miStatusWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowSystemLogsInfoWindowControl, 0, 0);
        }

        /// <summary>
        /// 默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miDefaultWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowDefaultWindowControl, 0, 0);
        }

        private void miShowMapViewWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.ShowDBMapWindow ShowMapViewWin = new SubWindow.ShowDBMapWindow();
            ShowMapViewWin.LoadDeviceListTreeView();
            ShowMapViewWin.ShowDialog();
        }

        private void miStatisticsInfoViewWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SubWindow.StatisticalAnalysisWindow StatisticalAnalysisViewWin = new SubWindow.StatisticalAnalysisWindow();
            StatisticalAnalysisViewWin.LoadDeviceListTreeView();
            StatisticalAnalysisViewWin.ShowDialog();
        }

        /// <summary>
        /// 通话记录窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miCallRecordsWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowCallRecordsWindowControl, 0, 0);
        }

        /// <summary>
        /// 短信记录窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miSMSRecordsWindow_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Parameters.SendMessage(Parameters.WinHandle, Parameters.WM_ShowSMSRecordsWindowControl, 0, 0);
        }
    }
}
