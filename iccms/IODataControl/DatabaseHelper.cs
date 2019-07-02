using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ParameterControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace IODataControl
{
    public class AccessDatabaseConnectionClass
    {
        private string _dB_FILE;
        private string _dB_USER;
        private string _dB_PASS;
        private bool _connected;
        private OleDbConnection _accessDataBaseConn = null;
        private string AccessDataBaseConnectString = null;

        public OleDbConnection AccessDataBaseConn
        {
            get
            {
                return _accessDataBaseConn;
            }

            set
            {
                _accessDataBaseConn = value;
            }
        }

        public bool Connected
        {
            get
            {
                return _connected;
            }

            set
            {
                _connected = value;
            }
        }

        public string DB_FILE
        {
            get
            {
                return _dB_FILE;
            }

            set
            {
                _dB_FILE = value;
            }
        }

        public string DB_USER
        {
            get
            {
                return _dB_USER;
            }

            set
            {
                _dB_USER = value;
            }
        }

        public string DB_PASS
        {
            get
            {
                return _dB_PASS;
            }

            set
            {
                _dB_PASS = value;
            }
        }

        public AccessDatabaseConnectionClass(string OleDbFile, string UserName, string Password)
        {
            try
            {
                if (AccessDataBaseConnectString == null)
                {
                    AccessDataBaseConnectString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + OleDbFile + "; Persist Security Info=False; User ID=" + UserName + "; Password=" + Password;
                }

                if (AccessDataBaseConn == null)
                {
                    AccessDataBaseConn = new OleDbConnection(AccessDataBaseConnectString);
                    AccessDataBaseConn.Open();
                    if (AccessDataBaseConn.State == System.Data.ConnectionState.Open)
                    {
                        Connected = true;
                    }
                    else
                    {
                        Connected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
            }
        }
        public bool AccessDatabaseConnect()
        {
            try
            {
                AccessDataBaseConnectString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + DB_FILE + "; Persist Security Info=False; User ID=" + DB_USER + "; Password=" + DB_PASS;
                AccessDataBaseConn = new OleDbConnection(AccessDataBaseConnectString);
                AccessDataBaseConn.Open();
                if (AccessDataBaseConn.State == System.Data.ConnectionState.Open)
                {
                    Connected = true;
                }
                else
                {
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                AccessDataBaseConn.Close();
                Connected = false;
            }
            return Connected;
        }
    }
    public class DatabaseHelper
    {
        AccessDatabaseConnectionClass AccessDatabaseConnection = null;

        public DatabaseHelper()
        {
            AccessDatabaseConnection = new AccessDatabaseConnectionClass(Parameters.LogsDbFile, "admin", "");
        }
        public void SaveLogs(string DatatTimes, string Operations, string Messages, string MessageStackTreace)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string insertSql = string.Format("INSERT INTO LogsData([Operations],[Messages],[DateTimes],[ErrorStatus]) "
                                       + " VALUES('{0}','{1}','{2}','{3}');", Operations, Messages, DatatTimes, MessageStackTreace);

                    using (OleDbCommand inserCmd = new OleDbCommand(insertSql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        inserCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    Parameters.PrintfLogsExtended("保存日志事件", Operations, Messages);
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("保存日志事件", ex.Message, ex.StackTrace);
            }
        }

        public void SaveData(string AData)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string insertSql = string.Format("INSERT INTO DataCaching([Data]) "
                                       + " VALUES('{0}');", AData);

                    using (OleDbCommand inserCmd = new OleDbCommand(insertSql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        inserCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("数据保存失败", ex.Message, ex.StackTrace);
            }
        }

        public void LoadLogs(ref DataTable ClientDataGridView)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string QuerySql = string.Format("Select * From LogsData Order By [DateTimes] Desc");

                    using (OleDbCommand QueryCmd = new OleDbCommand(QuerySql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        using (OleDbDataReader readerCmd = QueryCmd.ExecuteReader())
                        {
                            while (readerCmd.Read())
                            {
                                DataRow rw = ClientDataGridView.NewRow();
                                if (!readerCmd.IsDBNull(0))
                                {
                                    rw["ID"] = readerCmd["ID"].ToString();
                                }
                                else
                                {
                                    rw["ID"] = "";
                                }

                                if (!readerCmd.IsDBNull(1))
                                {
                                    rw["Operations"] = readerCmd["Operations"].ToString();
                                }
                                else
                                {
                                    rw["Operations"] = "";
                                }

                                if (!readerCmd.IsDBNull(2))
                                {
                                    rw["Messages"] = readerCmd["Messages"].ToString();
                                }
                                else
                                {
                                    rw["Messages"] = "";
                                }

                                if (!readerCmd.IsDBNull(3))
                                {
                                    rw["DateTimes"] = readerCmd["DateTimes"].ToString();
                                }
                                else
                                {
                                    rw["DateTimes"] = "";
                                }

                                if (!readerCmd.IsDBNull(4))
                                {
                                    rw["ErrorStatus"] = readerCmd["ErrorStatus"].ToString();
                                }
                                else
                                {
                                    rw["ErrorStatus"] = "";
                                }

                                ClientDataGridView.Rows.Add(rw);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void LoadData(ref string RData, ref long RID)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string QuerySql = string.Format("Select top 1 [ID],[Data] From DataCaching");

                    using (OleDbCommand QueryCmd = new OleDbCommand(QuerySql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        using (OleDbDataReader readerCmd = QueryCmd.ExecuteReader())
                        {
                            while (readerCmd.Read())
                            {
                                if (!readerCmd.IsDBNull(0))
                                {
                                    RID = Convert.ToInt64(readerCmd["ID"].ToString());
                                }


                                if (!readerCmd.IsDBNull(1))
                                {
                                    RData = readerCmd["Data"].ToString();
                                }
                                else
                                {
                                    RData = "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended("读取缓存", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 事件查询
        /// </summary>
        /// <param name="ClientDataGridView">显示数据表</param>
        /// <param name="DTimeStart">开始时间</param>
        /// <param name="DTimeEnd">结束时间</param>
        /// <param name="Events">事件</param>
        /// <param name="DetailMessage">详细信息</param>
        /// <param name="ReasonsTracking">原因追踪</param>

        public void LoadLogs(ref DataTable ClientDataGridView, string DTimeStart, string DTimeEnd, string Events, string DetailMessage, string ReasonsTracking)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string QuerySql = string.Format("Select * From LogsData Where CDate([DateTimes])>=CDate('{0}') and CDate([DateTimes])<=CDate('{1}') and [Operations] like '%{2}%' and [Messages] like '%{3}%' and [ErrorStatus] like '%{4}%' Order By [DateTimes] Desc", DTimeStart, DTimeEnd, Events, DetailMessage, ReasonsTracking);

                    using (OleDbCommand QueryCmd = new OleDbCommand(QuerySql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        using (OleDbDataReader readerCmd = QueryCmd.ExecuteReader())
                        {
                            while (readerCmd.Read())
                            {
                                DataRow rw = ClientDataGridView.NewRow();
                                if (!readerCmd.IsDBNull(0))
                                {
                                    rw["ID"] = readerCmd["ID"].ToString();
                                }
                                else
                                {
                                    rw["ID"] = "";
                                }

                                if (!readerCmd.IsDBNull(1))
                                {
                                    rw["Operations"] = readerCmd["Operations"].ToString();
                                }
                                else
                                {
                                    rw["Operations"] = "";
                                }

                                if (!readerCmd.IsDBNull(2))
                                {
                                    rw["Messages"] = readerCmd["Messages"].ToString();
                                }
                                else
                                {
                                    rw["Messages"] = "";
                                }

                                if (!readerCmd.IsDBNull(3))
                                {
                                    rw["DateTimes"] = readerCmd["DateTimes"].ToString();
                                }
                                else
                                {
                                    rw["DateTimes"] = "";
                                }

                                if (!readerCmd.IsDBNull(4))
                                {
                                    rw["ErrorStatus"] = readerCmd["ErrorStatus"].ToString();
                                }
                                else
                                {
                                    rw["ErrorStatus"] = "";
                                }

                                ClientDataGridView.Rows.Add(rw);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 清空日志事件
        /// </summary>
        public void LogsClear()
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string QuerySql = string.Format("Delete From LogsData");

                    using (OleDbCommand QueryCmd = new OleDbCommand(QuerySql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        QueryCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLogs(DateTime.Now.ToString(), "清空系统日", ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// 删除指定的系统日志项
        /// </summary>
        /// <param name="LogID"></param>
        /// <param name="LogDatetime"></param>
        /// <param name="LogEvent"></param>
        public void LogsDelete(string LogID, string LogDatetime, string LogEvent)
        {
            try
            {
                if (AccessDatabaseConnection.Connected)
                {
                    string QuerySql = string.Format("Delete From LogsData Where [ID]={0} and CDate([DateTimes])=CDate('{1}') and [Operations]='{2}'", LogID, LogDatetime, LogEvent);

                    using (OleDbCommand QueryCmd = new OleDbCommand(QuerySql, AccessDatabaseConnection.AccessDataBaseConn))
                    {
                        QueryCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLogs(DateTime.Now.ToString(), "清空系统日", ex.Message, ex.StackTrace);
            }
        }

        //用户自定义类型：增，删，改，查=======
        public void CustomUserTypeSetting(bool Setting, string UserType, string BGColor, bool Alert)
        {
            string Sqlcmd = string.Format("insert into CustomUserTypes ([Setting],[UserType],[BackGroundColor],[Alert]) Values({0},'{1}','{2}',{3})", Setting, UserType, BGColor, Alert);
            using (OleDbCommand cmd = new OleDbCommand(Sqlcmd, AccessDatabaseConnection.AccessDataBaseConn))
            {
                cmd.ExecuteNonQuery();
            }
        }
        //删
        public int CustomUserTypeDelete(string ID)
        {
            try
            {
                string Sqlcmd = string.Format("Delete from CustomUserTypes Where [ID]={0}", ID);
                using (OleDbCommand cmd = new OleDbCommand(Sqlcmd, AccessDatabaseConnection.AccessDataBaseConn))
                {
                    cmd.ExecuteNonQuery();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                return -1;
            }
        }
        //改
        public void CustomUserTypeUpdate(string ID, bool Setting, string UserType, string BGColor, bool Alert)
        {
            string Sqlcmd = string.Format("Update CustomUserTypes Set [Setting]={0},[UserType]='{1}',[BackGroundColor]='{2}',[Alert]={3} Where [ID]={4}", Setting, UserType, BGColor, Alert, ID);
            using (OleDbCommand cmd = new OleDbCommand(Sqlcmd, AccessDatabaseConnection.AccessDataBaseConn))
            {
                cmd.ExecuteNonQuery();
            }
        }
        //查 DataTable
        public void CustomUserTypeGetting(ref DataTable UserTypeTab)
        {
            string Sqlcmd = string.Format("Select * from CustomUserTypes");
            using (OleDbCommand cmd = new OleDbCommand(Sqlcmd, AccessDatabaseConnection.AccessDataBaseConn))
            {
                using (OleDbDataReader CmdReader = cmd.ExecuteReader())
                {
                    UserTypeTab.Rows.Clear();
                    while (CmdReader.Read())
                    {
                        DataRow dr = UserTypeTab.NewRow();
                        dr[0] = CmdReader["ID"].ToString();
                        dr[1] = CmdReader["Setting"].ToString();
                        dr[2] = CmdReader["UserType"].ToString();
                        dr[3] = CmdReader["BackGroundColor"].ToString();
                        dr[4] = CmdReader["Alert"].ToString();
                        UserTypeTab.Rows.Add(dr);
                    }
                }
            }
        }

        //查到ComboBox
        public void CustomUserTypeGetting(ref List<string> UserTypeList)
        {
            string Sqlcmd = string.Format("Select * from CustomUserTypes");
            using (OleDbCommand cmd = new OleDbCommand(Sqlcmd, AccessDatabaseConnection.AccessDataBaseConn))
            {
                using (OleDbDataReader CmdReader = cmd.ExecuteReader())
                {
                    UserTypeList.Clear();
                    while (CmdReader.Read())
                    {
                        UserTypeList.Add(CmdReader["UserType"].ToString());
                    }
                }
            }
        }
        //=================================
    }

    /// <summary>
    /// [2003-2007版本XLS文件读写程序]
    /// </summary>
    public class ExcelHelper : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;
        public ExcelHelper(string fileName)//构造函数，读入文件名
        {
            this.fileName = fileName;
            disposed = false;
        }
        /// 将excel中的数据导入到DataTable中
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ExcelToDataTable(string sheetName, bool isFirstRowColumn)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                workbook = WorkbookFactory.Create(fs);
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号，即总的列数
                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;//得到项标题后
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }
                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = row.GetCell(j).ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)//打印错误信息
            {
                MessageBox.Show("不支持的文件格式或文件无法读取： " + ex.Message);
                return null;
            }
        }

        //将DataTable数据导入到excel中
        //<param name="data">要导入的数据</param>
        //<param name="sheetName">要导入的excel的sheet的名称</param>
        //<param name="isColumnWritten">DataTable的列名是否要导入</param>
        //<returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)     // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        //将DataTable数据导入到excel中(黑名单)
        //<param name="data">要导入的数据</param>
        //<param name="sheetName">要导入的excel的sheet的名称</param>
        //<param name="isColumnWritten">DataTable的列名是否要导入</param>
        //<returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcelForBlackList(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)     // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        if (Parameters.LanguageType == "EN")
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        else
                        {
                            if (data.Columns[j].ColumnName == "ID")
                            {
                                row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                            }
                            else if (data.Columns[j].ColumnName == "IMSI")
                            {
                                row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                            }
                            else if (data.Columns[j].ColumnName == "AliasName")
                            {
                                row.CreateCell(j).SetCellValue("别名");
                            }
                            else if (data.Columns[j].ColumnName == "Resourcese")
                            {
                                row.CreateCell(j).SetCellValue("资源");
                            }
                            else if (data.Columns[j].ColumnName == "Station")
                            {
                                row.CreateCell(j).SetCellValue("站点");
                            }
                        }
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        //将DataTable数据导入到excel中(白名单)
        //<param name="data">要导入的数据</param>
        //<param name="sheetName">要导入的excel的sheet的名称</param>
        //<param name="isColumnWritten">DataTable的列名是否要导入</param>
        //<returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcelForWhiteList(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)     // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        if (Parameters.LanguageType == "EN")
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        else
                        {
                            if (data.Columns[j].ColumnName == "ID")
                            {
                                row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                            }
                            else if (data.Columns[j].ColumnName == "IMSI")
                            {
                                row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                            }
                            else if (data.Columns[j].ColumnName == "AliasName")
                            {
                                row.CreateCell(j).SetCellValue("别名");
                            }
                            else if (data.Columns[j].ColumnName == "Station")
                            {
                                row.CreateCell(j).SetCellValue("站点");
                            }
                        }
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        //将DataTable数据导入到excel中(系统日志)
        //<param name="data">要导入的数据</param>
        //<param name="sheetName">要导入的excel的sheet的名称</param>
        //<param name="isColumnWritten">DataTable的列名是否要导入</param>
        //<returns>导入数据行数(包含列名那一行)</returns>
        public int DataTableToExcelForSysLogs(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)     // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        if (Parameters.LanguageType == "EN")
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        else
                        {
                            if (data.Columns[j].ColumnName == "DTime")
                            {
                                row.CreateCell(j).SetCellValue("日期");
                            }
                            else if (data.Columns[j].ColumnName == "Object")
                            {
                                row.CreateCell(j).SetCellValue("详细信息");
                            }
                            else if (data.Columns[j].ColumnName == "Action")
                            {
                                row.CreateCell(j).SetCellValue("操作类型");
                            }
                            else if (data.Columns[j].ColumnName == "Other")
                            {
                                row.CreateCell(j).SetCellValue("其它");
                            }
                        }
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        /// <summary>
        /// 导出捕号实时数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sheetName"></param>
        /// <param name="isColumnWritten"></param>
        /// <returns></returns>
        public int DataTableToExcelForScannerData(DataTable data, string sheetName, bool isColumnWritten)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            if (fileName.IndexOf(".xlsx") > 0)     // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        if (Parameters.LanguageType == "EN")
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        else
                        {
                            if (data.Columns[j].ColumnName == "ID")
                            {
                                row.CreateCell(j).SetCellValue("序号");
                            }
                            else if (data.Columns[j].ColumnName == "IMSI")
                            {
                                row.CreateCell(j).SetCellValue("IMSI");
                            }
                            else if (data.Columns[j].ColumnName == "DTime")
                            {
                                row.CreateCell(j).SetCellValue("日期");
                            }
                            else if (data.Columns[j].ColumnName == "UserType")
                            {
                                row.CreateCell(j).SetCellValue("用户类型");
                            }
                            else if (data.Columns[j].ColumnName == "TMSI")
                            {
                                row.CreateCell(j).SetCellValue("TMSI");
                            }
                            else if (data.Columns[j].ColumnName == "IMEI")
                            {
                                row.CreateCell(j).SetCellValue("IMEI");
                            }
                            else if (data.Columns[j].ColumnName == "Intensity")
                            {
                                row.CreateCell(j).SetCellValue("信号");
                            }
                            else if (data.Columns[j].ColumnName == "Operators")
                            {
                                row.CreateCell(j).SetCellValue("运营商");
                            }
                            else if (data.Columns[j].ColumnName == "DomainName")
                            {
                                row.CreateCell(j).SetCellValue("号码归属地");
                            }
                            else if (data.Columns[j].ColumnName == "DeviceName")
                            {
                                row.CreateCell(j).SetCellValue("设备名称");
                            }
                            else if (data.Columns[j].ColumnName == "Des")
                            {
                                row.CreateCell(j).SetCellValue("别名");
                            }
                        }
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public void Dispose()//IDisposable为垃圾回收相关的东西，用来显式释放非托管资源,这部分目前还不是非常了解
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }
                fs = null;
                disposed = true;
            }
        }
    }

}
