using DataInterface;
using IODataControl;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// CustomUserTypeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomUserTypeWindow : Window
    {
        private UserTypesParameterClass SelectCustomUserType = new UserTypesParameterClass();
        private DataTable CustomUserTypeTab = null;
        public CustomUserTypeWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (CustomUserTypeTab == null)
            {
                CustomUserTypeTab = new DataTable("CustomUserType");
                DataColumn DataColumn0 = new DataColumn();
                DataColumn0.ColumnName = "ID";
                DataColumn0.DataType = System.Type.GetType("System.Int32");

                DataColumn DataColumn1 = new DataColumn();
                DataColumn1.ColumnName = "Setting";
                DataColumn1.DataType = System.Type.GetType("System.Boolean");

                DataColumn DataColumn2 = new DataColumn();
                DataColumn2.ColumnName = "UserType";
                DataColumn2.DataType = System.Type.GetType("System.String");

                DataColumn DataColumn3 = new DataColumn();
                DataColumn3.ColumnName = "BackGroundColor";
                DataColumn3.DataType = System.Type.GetType("System.String");

                DataColumn DataColumn4 = new DataColumn();
                DataColumn4.ColumnName = "Alert";
                DataColumn4.DataType = System.Type.GetType("System.Boolean");

                CustomUserTypeTab.Columns.Add(DataColumn0);
                CustomUserTypeTab.Columns.Add(DataColumn1);
                CustomUserTypeTab.Columns.Add(DataColumn2);
                CustomUserTypeTab.Columns.Add(DataColumn3);
                CustomUserTypeTab.Columns.Add(DataColumn4);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustomListInfoDataGrid.ItemsSource = BaseParameterSettingWindow.UserTypesParameterList;
            Getting();
        }

        /// <summary>
        /// 查询
        /// </summary>
        private void Getting()
        {
            JsonInterFace.IODataHelper.CustomUserTypeGetting(ref CustomUserTypeTab);
            BaseParameterSettingWindow.UserTypesParameterList.Clear();
            for (int i = 0; i < CustomUserTypeTab.Rows.Count; i++)
            {
                UserTypesParameterClass CustomUserType = new UserTypesParameterClass();
                CustomUserType.ID = int.Parse(CustomUserTypeTab.Rows[i][0].ToString());
                CustomUserType.Setting = Convert.ToBoolean(CustomUserTypeTab.Rows[i][1].ToString());
                CustomUserType.UserType = CustomUserTypeTab.Rows[i][2].ToString();
                CustomUserType.BackGroundColor = CustomUserTypeTab.Rows[i][3].ToString();
                CustomUserType.Alert = Convert.ToBoolean(CustomUserTypeTab.Rows[i][4].ToString());
                BaseParameterSettingWindow.UserTypesParameterList.Add(CustomUserType);
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Getting();
                MessageBox.Show("查询成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomListInfoDataGrid.SelectedItem != null)
                {
                    SelectCustomUserType.ID = (CustomListInfoDataGrid.SelectedItem as UserTypesParameterClass).ID;
                    SelectCustomUserType.Setting = (CustomListInfoDataGrid.SelectedItem as UserTypesParameterClass).Setting;
                    SelectCustomUserType.UserType = (CustomListInfoDataGrid.SelectedItem as UserTypesParameterClass).UserType;
                    SelectCustomUserType.BackGroundColor = (CustomListInfoDataGrid.SelectedItem as UserTypesParameterClass).BackGroundColor;
                    SelectCustomUserType.Alert = (CustomListInfoDataGrid.SelectedItem as UserTypesParameterClass).Alert;

                    if (MessageBox.Show("确定删除吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        if (JsonInterFace.IODataHelper.CustomUserTypeDelete(SelectCustomUserType.ID.ToString()) == 0)
                        {
                            Getting();
                            MessageBox.Show("[" + SelectCustomUserType.UserType + "]删除成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("[" + SelectCustomUserType.UserType + "]删除失败", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[" + SelectCustomUserType.UserType + "]删除失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SubWindow.CustomUserTypeAddWindow CustomUserTypeAddWin = new CustomUserTypeAddWindow();
                CustomUserTypeAddWin.ShowDialog();
                Getting();
            }
            catch (Exception ex)
            {
                MessageBox.Show("添加失败，" + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtBackGroundColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Color bgColor = new Color();
            if (Parameters.GettingColor(ref bgColor))
            {
                (sender as Label).Content = bgColor.ToString();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < BaseParameterSettingWindow.UserTypesParameterList.Count; i++)
                {
                    JsonInterFace.IODataHelper.CustomUserTypeUpdate(
                                                                    BaseParameterSettingWindow.UserTypesParameterList[i].ID.ToString(),
                                                                    BaseParameterSettingWindow.UserTypesParameterList[i].Setting,
                                                                    BaseParameterSettingWindow.UserTypesParameterList[i].UserType,
                                                                    BaseParameterSettingWindow.UserTypesParameterList[i].BackGroundColor,
                                                                    BaseParameterSettingWindow.UserTypesParameterList[i].Alert
                                                                   );
                }
                MessageBox.Show("更新成功,", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("更新失败," + ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
