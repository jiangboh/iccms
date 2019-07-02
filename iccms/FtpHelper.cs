using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using iccms.Common;
using ParameterControl;

namespace iccms
{
    public class FtpHelper
    {
        #region FTP相应码

        //private const int 110    新文件指示器上的重启标记
        //private const int 120    服务器准备就绪的时间（分钟数）
        //private const int 125    打开数据连接，开始传输
        //private const int 150    打开连接
        //private const int 200    成功
        private const int COMMAND_NO_EXECUTE = 202; //    命令没有执行
        //private const int 211    系统状态回复
        //private const int 212    目录状态回复
        //private const int 213    文件状态回复
        //private const int 214    帮助信息回复
        //private const int 215    系统类型回复
        private const int SERVER_READY_OK = 220;  //服务就绪
        //private const int 221    退出网络
        //private const int 225    打开数据连接
        //private const int 226    结束数据连接
        //private const int 227    进入被动模式(IP地址、ID端口)
        private const int LOG_INTERNET = 230;   //    登录因特网
        //private const int 250    文件行为完成
        //private const int 257    路径名建立
        private const int REQUIRE_PASSWD = 331; //    要求密码
        //private const int 332    要求帐号
        //private const int 350    文件行为暂停
        //private const int 421    服务关闭
        //private const int 425    无法打开数据连接
        //private const int 426    结束连接
        //private const int 450    文件不可用
        //private const int 451    遇到本地错误
        //private const int 452    磁盘空间不足
        //private const int 500    无效命令
        //private const int 501    错误参数
        //private const int 502    命令没有执行
        //private const int 503    错误指令序列
        //private const int 504    无效命令参数
        //private const int 530    未登录网络
        //private const int 532    存储文件需要帐号
        //private const int 550    文件不可用
        //private const int 551    不知道的页类型
        //private const int 552    超过存储分配
        //private const int 553    文件名不允许

        #endregion

        #region 变量和属性

        /// <summary>
        /// FTP服务器IP地址
        /// </summary>
        private string strRemoteHost;
        public string RemoteHost
        {
            get
            {
                return strRemoteHost;
            }
            set
            {
                strRemoteHost = value;
            }
        }

        /// <summary>
        /// FTP服务器端口
        /// </summary>
        private int strRemotePort;
        public int RemotePort
        {
            get
            {
                return strRemotePort;
            }
            set
            {
                strRemotePort = value;
            }
        }

        /// <summary>
        /// 当前服务器目录
        /// </summary>
        private string strRemotePath;
        public string RemotePath
        {
            get
            {
                return strRemotePath;
            }
            set
            {
                strRemotePath = value;
            }
        }

        /// <summary>
        /// 登录用户账号
        /// </summary>
        private string strRemoteUser;
        public string RemoteUser
        {
            set
            {
                strRemoteUser = value;
            }
        }

        /// <summary>
        /// 用户登录密码
        /// </summary>
        private string strRemotePass;
        public string RemotePass
        {
            set
            {
                strRemotePass = value;
            }
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        private Boolean bConnected = false;
        public bool Connected
        {
            get
            {
                return bConnected;
            }
            set
            {
                bConnected = value;
            }
        }

        /// <summary>
        /// 上传完成
        /// </summary>
        public int Complete
        {
            get
            {
                return sendedData;
            }
        }

        #endregion

        #region 内部变量

        /// <summary>
        /// 服务器返回的应答信息(包含应答码)
        /// </summary>
        private string strMsg;

        /// <summary>
        /// 服务器返回的应答信息(包含应答码)
        /// </summary>
        private string strReply;

        /// <summary>
        /// 服务器返回的应答码
        /// </summary>
        private int iReplyCode;

        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        private Socket socketControl;

        /// <summary>
        /// 传输模式
        /// </summary>
        private TransferType trType;

        /// <summary>
        /// 接收和发送数据的缓冲区
        /// </summary>
        private static int BLOCK_SIZE = 512;
        Byte[] buffer = new Byte[BLOCK_SIZE];

        /// <summary>
        /// 编码方式
        /// </summary>
        Encoding ASCII = Encoding.ASCII;

        /// <summary>
        /// 已发送数据量
        /// </summary>
        private int sendedData = 0;

        #endregion

        #region 构造函数

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public FtpHelper()
        {
            strRemoteHost = "";
            strRemotePath = "";
            strRemoteUser = "";
            strRemotePass = "";
            strRemotePort = 21;
            bConnected = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="remotePath"></param>
        /// <param name="remoteUser"></param>
        /// <param name="remotePass"></param>
        /// <param name="remotePort"></param>
        public FtpHelper(string remoteHost, string remotePath, string remoteUser, string remotePass, int remotePort)
        {
            strRemoteHost = remoteHost;
            strRemotePath = remotePath;
            strRemoteUser = remoteUser;
            strRemotePass = remotePass;
            strRemotePort = remotePort;

            Connect();
        }

        #endregion        

        #region 链接

        /// <summary>
        /// 建立连接 
        /// </summary>
        public void Connect()
        {
            socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(RemoteHost), strRemotePort);

            // 链接
            try
            {
                socketControl.Connect(ep);
            }
            catch (Exception ee)
            {
                Parameters.PrintfLogsExtended("Couldn't connect to remote server", ee.Message, ee.StackTrace);
                return;
            }


            IPAddress remote_ip = ((System.Net.IPEndPoint)socketControl.RemoteEndPoint).Address;
            int remote_port = ((System.Net.IPEndPoint)socketControl.RemoteEndPoint).Port;

            // 获取应答码
            ReadReply();

            if (iReplyCode != SERVER_READY_OK)
            {
                DisConnect();
                throw new IOException(strReply.Substring(4));
            }

            // 登陆
            SendCommand("USER " + strRemoteUser);

            if (!(iReplyCode == REQUIRE_PASSWD || iReplyCode == LOG_INTERNET))
            {
                CloseSocketConnect();//关闭连接

                Parameters.PrintfLogsExtended("连接到FTP server：" + RemoteHost + ":" + strRemotePort + ",FAILED.");
            }

            if (iReplyCode != LOG_INTERNET)
            {
                SendCommand("PASS " + strRemotePass);
                if (!(iReplyCode == LOG_INTERNET || iReplyCode == COMMAND_NO_EXECUTE))
                {
                    CloseSocketConnect();//关闭连接

                    Parameters.PrintfLogsExtended("连接到FTP server：" + RemoteHost + ":" + strRemotePort + ",FAILED.");
                }
            }

            bConnected = true;
            Parameters.PrintfLogsExtended("连接到FTP server：" + RemoteHost + ":" + strRemotePort + ",OK.");

            // 切换到目录
            ChDir(strRemotePath);
        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void DisConnect()
        {
            if (socketControl != null)
            {
                SendCommand("QUIT");
            }

            CloseSocketConnect();
        }

        #endregion

        #region 传输模式

        /// <summary>
        /// 传输模式:二进制类型、ASCII类型
        /// </summary>
        public enum TransferType { Binary, ASCII };

        /// <summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="ttType">传输模式</param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary类型传输
            }
            else
            {
                SendCommand("TYPE A");//ASCII类型传输
            }

            if (iReplyCode != 200)
            {
                throw new IOException(strReply.Substring(4));
            }
            else
            {
                trType = ttType;
            }
        }


        /// <summary>
        /// 获得传输模式
        /// </summary>
        /// <returns>传输模式</returns>
        public TransferType GetTransferType()
        {
            return trType;
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <returns></returns>
        public string[] Dir(string strMask)
        {
            // 建立链接
            if (!bConnected)
            {
                Connect();
            }

            //建立进行数据连接的socket
            Socket socketData = CreateDataSocket();

            //传送命令
            SendCommand("NLST " + strMask);

            //分析应答代码
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226))
            {
                throw new IOException(strReply.Substring(4));
            }

            //获得结果
            strMsg = "";
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += Encoding.Default.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }

            char[] seperator = "\r\n".ToCharArray();
            string[] strsFileList = strMsg.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            socketData.Close();//数据socket关闭时也会有返回码

            if (iReplyCode != 226)
            {
                ReadReply();
                if (iReplyCode != 226)
                {
                    throw new IOException(strReply.Substring(4));
                }
            }

            return strsFileList;
        }


        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;

            if (iReplyCode == 213)
            {
                lSize = Int64.Parse(strReply.Substring(4));
            }
            else
            {
                throw new IOException(strReply.Substring(4));
            }

            return lSize;
        }

        /// <summary>
        /// 获得文件详细信息列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <returns></returns>
        public string[] List(string strMask)
        {
            // 建立链接
            if (!bConnected)
            {
                Connect();
            }

            //建立进行数据连接的socket
            Socket socketData = CreateDataSocket();

            //传送命令
            SendCommand("LIST " + strMask);

            //分析应答代码
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226))
            {
                throw new IOException(strReply.Substring(4));
            }

            //获得结果
            strMsg = "";
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += Encoding.Default.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }

            char[] seperator = "\r\n".ToCharArray();
            string[] strsFileList = strMsg.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            socketData.Close();//数据socket关闭时也会有返回码
            if (iReplyCode != 226)
            {
                ReadReply();
                if (iReplyCode != 226)
                {
                    throw new IOException(strReply.Substring(4));
                }
            }

            return strsFileList;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strFileName">待删除文件名</param>
        public void Delete(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("DELE " + strFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        /// <summary>
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// </summary>
        /// <param name="strOldFileName">旧文件名</param>
        /// <param name="strNewFileName">新文件名</param>
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("RNFR " + strOldFileName);
            if (iReplyCode != 350)
            {
                throw new IOException(strReply.Substring(4));
            }

            //  如果新文件名与原有文件重名,将覆盖原有文件
            SendCommand("RNTO " + strNewFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }

        #endregion

        #region 上传和下载

        /// <summary>
        /// 下载一批文件
        /// </summary>
        /// <param name="strFileNameMask">文件名的匹配字符串</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        public void Get(string strFileNameMask, string strFolder)
        {
            if (!bConnected)
            {
                Connect();
            }

            string[] strFiles = Dir(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                //一般来说strFiles的最后一个元素可能是空字符串
                if (!strFile.Equals(""))
                {
                    Get(strFile, strFolder, strFile);
                }
            }
        }


        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        public void Get(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SetTransferType(TransferType.Binary);
            if (strLocalFileName.Equals(""))
            {
                strLocalFileName = strRemoteFileName;
            }
            //if (!File.Exists(strLocalFileName))
            //{
            //    Stream st = File.Create(strLocalFileName);
            //    st.Close();
            //}

            FileStream output = new FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);

            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);

            if (!(iReplyCode == 150 || iReplyCode == 125
                || iReplyCode == 226 || iReplyCode == 250))
            {
                throw new IOException(strReply.Substring(4));
            }

            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }

            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }

            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strReply.Substring(4));
                }
            }

            Parameters.PrintfLogsExtended("下载文件：" + strRemoteFileName + ",OK.");
        }


        /// <summary>
        /// 上传一批文件
        /// </summary>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strFileNameMask">文件名匹配字符(可以包含*和?)</param>
        public void Put(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);

            foreach (string strFile in strFiles)
            {
                //strFile是完整的文件名(包含路径)
                Put(strFile);
            }
        }


        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="strFileName">本地文件名</param>
        public int Put(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }

            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));

            if (!(iReplyCode == 125 || iReplyCode == 150))
            {
                throw new IOException(strReply.Substring(4));
            }

            FileStream input = new FileStream(strFileName, FileMode.Open);
            int iBytes = 0;
            sendedData = iBytes;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                sendedData += iBytes;
                socketData.Send(buffer, iBytes, SocketFlags.None);
            }

            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }

            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    //throw new IOException(strReply.Substring(4));
                    return -1;
                }
            }

            //Logger.Trace(LogInfoType.EROR, "上传文件：" + strFileName + ",OK.");
            //FrmMainController.add_log_info(LogInfoType.EROR, "上传文件：" + strFileName + ",OK.");
            return 0;
        }

        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="strFileName">本地文件名</param>
        public void Put(string strFileName, byte[] data)
        {
            if (!bConnected)
            {
                Connect();
            }

            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));

            if (!(iReplyCode == 125 || iReplyCode == 150))
            {
                throw new IOException(strReply.Substring(4));
            }

            if (data.Length <= 0)
            {
                throw new IOException("data.Length <= 0");
            }

            int iBytes = 0;
            int cpCount = data.Length / buffer.Length + 1;

            for (int i = 0; i < cpCount; i++)
            {
                if (i == (cpCount - 1))
                {
                    iBytes = data.Length - i * BLOCK_SIZE;
                    Array.Copy(data, i * BLOCK_SIZE, buffer, 0, iBytes);
                    socketData.Send(buffer, iBytes, 0);
                }
                else
                {
                    Array.Copy(data, i * BLOCK_SIZE, buffer, 0, buffer.Length);
                    socketData.Send(buffer, BLOCK_SIZE, 0);
                }
            }

            if (socketData.Connected)
            {
                socketData.Close();
            }

            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strReply.Substring(4));
                }
            }

            //Logger.Trace(LogInfoType.EROR, "上传文件：" + strFileName + ",OK.");
            //FrmMainController.add_log_info(LogInfoType.EROR, "上传文件：" + strFileName + ",OK.");
        }

        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="strFileName">本地文件名</param>
        public void Put(string strFileName, MemoryStream ms)
        {
            if (!bConnected)
            {
                Connect();
            }

            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));

            if (!(iReplyCode == 125 || iReplyCode == 150))
            {
                throw new IOException(strReply.Substring(4));
            }

            //FileStream input = new FileStream(strFileName, FileMode.Open);
            //int iBytes = 0;

            //while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            //{
            //    socketData.Send(buffer, iBytes, 0);
            //}

            //input.Close();

            int iBytes = 0;

            while ((iBytes = ms.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }

            if (socketData.Connected)
            {
                socketData.Close();
            }

            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strReply.Substring(4));
                }
            }

            Parameters.PrintfLogsExtended("上传文件：" + strFileName + ",OK.");
        }

        #endregion

        #region 目录操作

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void MkDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("MKD " + strDirName);
            if (iReplyCode != 257)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void RmDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("RMD " + strDirName);

            if (iReplyCode != 250)
            {

                throw new IOException(strReply.Substring(4));
            }
        }


        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="strDirName">新的工作目录名</param>
        public void ChDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }

            SendCommand("CWD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
            strRemotePath = strDirName;
        }

        #endregion        

        #region 内部函数

        /// <summary>
        /// 将一行应答字符串记录在strReply和strMsg
        /// 应答码记录在iReplyCode
        /// </summary>
        private void ReadReply()
        {
            strMsg = "";
            strReply = ReadLine();

            Parameters.PrintfLogsExtended("ReadLine:" + strReply);

            iReplyCode = Int32.Parse(strReply.Substring(0, 3));
        }

        /// <summary>
        /// 建立进行数据连接的socket
        /// </summary>
        /// <returns>数据连接socket</returns>
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (iReplyCode != 227)
            {
                throw new IOException(strReply.Substring(4));
            }

            int index1 = strReply.IndexOf('(');
            int index2 = strReply.IndexOf(')');

            string ipData = strReply.Substring(index1 + 1, index2 - index1 - 1);

            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = "";

            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));

                if (Char.IsDigit(ch))
                {
                    buf += ch;
                }
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " +
                        strReply);
                }

                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch (Exception ee)
                    {
                        Parameters.PrintfLogsExtended("Malformed PASV strReply: ", ee.Message, ee.StackTrace);
                    }
                }
            }

            string ipAddress = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), port);

            try
            {
                s.Connect(ep);
            }
            catch (Exception ee)
            {
                Parameters.PrintfLogsExtended("Can't connect to remote server", ee.Message, ee.StackTrace);
            }

            return s;
        }

        /// <summary>
        /// 关闭socket连接(用于登录以前)
        /// </summary>
        private void CloseSocketConnect()
        {
            if (socketControl != null)
            {
                socketControl.Close();
                socketControl = null;
            }

            bConnected = false;
        }

        /// <summary>
        /// 读取Socket返回的所有字符串
        /// </summary>
        /// <returns>包含应答码的字符串行</returns>
        private string ReadLine()
        {
            while (true)
            {
                int iBytes = socketControl.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);

                if (iBytes < buffer.Length)
                {
                    break;
                }
            }

            char[] seperator = { '\n' };
            string[] mess = strMsg.Split(seperator);

            if (strMsg.Length > 2)
            {
                strMsg = mess[mess.Length - 2];
                //seperator[0]是10,换行符是由13和0组成的,分隔后10后面虽没有字符串,
                //但也会分配为空字符串给后面(也是最后一个)字符串数组,
                //所以最后一个mess是没用的空字符串
                //但为什么不直接取mess[0],因为只有最后一行字符串应答码与信息之间有空格
            }
            else
            {
                strMsg = mess[0];
            }

            //返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)
            if (!strMsg.Substring(3, 1).Equals(" "))
            {
                return ReadLine();
            }

            return strMsg;
        }


        /// <summary>
        /// 发送命令并获取应答码和最后一行应答字符串
        /// </summary>
        /// <param name="strCommand">命令</param>
        private void SendCommand(String strCommand)
        {
            try
            {
                Byte[] cmdBytes = Encoding.Default.GetBytes((strCommand + "\r\n").ToCharArray());
                socketControl.Send(cmdBytes, cmdBytes.Length, 0);
                ReadReply();
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        #endregion

        #region 检查ftp是否通畅

        private ManualResetEvent timeoutObject;
        private Socket socket = null;
        private bool isConn = false;

        private void callBackMethod(IAsyncResult asyncResult)
        {
            try
            {
                socket = asyncResult.AsyncState as Socket;
                if (socket != null)
                {
                    socket.EndConnect(asyncResult);
                    isConn = true;
                }
            }
            catch (Exception ex)
            {
                isConn = false;
                Parameters.PrintfLogsExtended("检查ftp是否通畅内部故障", ex.Message);
            }
            finally
            {
                timeoutObject.Set();
            }
        }


        /// <summary>
        /// 传递FTP返回的byte数组和长度,返回状态码(int)
        /// </summary>
        /// <param name="retByte"></param>
        /// <param name="retLen"></param>
        /// <returns></returns>
        private int getFtpReturnCode(byte[] retByte, int retLen)
        {
            try
            {
                string str = Encoding.ASCII.GetString(retByte, 0, retLen).Trim();
                return int.Parse(str.Substring(0, 3));
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 通过socket判断ftp是否通畅(异步socket连接,同步发送接收数据)
        /// </summary> 
        /// <returns></returns>
        public bool CheckFtpConnStatus(string ip, string ftpuser, string ftppas, out string errmsg, int port = 21, int timeout = 2000)
        {
            #region 输入数据检查

            if (ftpuser.Trim().Length == 0)
            {
                errmsg = "FTP用户名不能为空,请检查设置!";
                return false;
            }

            if (ftppas.Trim().Length == 0)
            {
                errmsg = "FTP密码不能为空,请检查设置!";
                return false;
            }

            IPAddress address;
            try
            {
                address = IPAddress.Parse(ip);
            }
            catch
            {
                errmsg = string.Format("FTP服务器IP:{0}解析失败,请检查是否设置正确!", ip);
                return false;
            }

            #endregion

            isConn = false;
            bool ret = false;
            byte[] result = new byte[1024];

            int pingStatus = 0;  //连接返回
            int userStatus = 0;  //用户名返回
            int pasStatus = 0;   //密码返回
            int exitStatus = 0;  //退出返回

            timeoutObject = new ManualResetEvent(false);

            try
            {
                int receiveLength;

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SendTimeout = timeout;

                //超时设置成2000毫秒
                socket.ReceiveTimeout = timeout;

                try
                {
                    //开始异步连接请求
                    socket.BeginConnect(new IPEndPoint(address, port), new AsyncCallback(callBackMethod), socket);

                    if (!timeoutObject.WaitOne(timeout, false))
                    {
                        socket.Close();
                        socket = null;
                        pingStatus = -1;
                    }

                    if (isConn)
                    {
                        pingStatus = 200;
                    }
                    else
                    {
                        pingStatus = -1;
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("通过socket判断ftp是否通畅(异步socket连接,同步发送接收数据)内部故障", ex.Message);
                    pingStatus = -1;
                }

                if (pingStatus == 200) //状态码200 - TCP连接成功
                {
                    receiveLength = socket.Receive(result);
                    pingStatus = getFtpReturnCode(result, receiveLength); //连接状态

                    if (pingStatus == 220)//状态码220 - FTP返回欢迎语
                    {
                        socket.Send(Encoding.Default.GetBytes(string.Format("{0}{1}", "USER " + ftpuser, Environment.NewLine)));
                        receiveLength = socket.Receive(result);
                        userStatus = getFtpReturnCode(result, receiveLength);

                        if (userStatus == 331)//状态码331 - 要求输入密码
                        {
                            socket.Send(Encoding.Default.GetBytes(string.Format("{0}{1}", "PASS " + ftppas, Environment.NewLine)));
                            receiveLength = socket.Receive(result);
                            pasStatus = getFtpReturnCode(result, receiveLength);

                            if (pasStatus == 230)//状态码230 - 登入因特网
                            {
                                errmsg = string.Format("FTP:{0}@{1}登陆成功", ip, port);
                                ret = true;
                                socket.Send(Encoding.Default.GetBytes(string.Format("{0}{1}", "QUIT", Environment.NewLine))); //登出FTP
                                receiveLength = socket.Receive(result);
                                exitStatus = getFtpReturnCode(result, receiveLength);
                            }
                            else
                            {
                                // 状态码230的错误
                                errmsg = string.Format("FTP:{0}@{1}登陆失败,用户名或密码错误({2})", ip, port, pasStatus);
                            }
                        }
                        else
                        {
                            // 状态码331的错误 
                            errmsg = string.Format("使用用户名:'{0}'登陆FTP:{1}@{2}时发生错误({3}),请检查FTP是否正常配置!", ftpuser, ip, port, userStatus);
                        }
                    }
                    else
                    {
                        // 状态码220的错误 
                        errmsg = string.Format("FTP:{0}@{1}返回状态错误({2}),请检查FTP服务是否正常运行!", ip, port, pingStatus);
                    }
                }
                else
                {
                    // 状态码200的错误
                    errmsg = string.Format("无法连接FTP服务器:{0}@{1},请检查FTP服务是否启动!", ip, port);
                }
            }
            catch (Exception ex)
            {
                //连接出错 
                errmsg = string.Format("FTP:{0}@{1}连接出错:", ip, port) + ex.Message;
                Parameters.PrintfLogsExtended(errmsg);
                ret = false;
            }
            finally
            {
                if (socket != null)
                {
                    socket.Close(); //关闭socket
                    socket = null;
                }
            }

            return ret;
        }

        #endregion
    }
}
