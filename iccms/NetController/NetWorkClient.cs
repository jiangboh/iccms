using DataInterface;
using ParameterControl;
using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace NetController
{
    public class NetWorkClient
    {
        public static AsyncTcpClient ControllerServer = null;
        public static Parameters AppConfigPara = new Parameters();
        public static string RemoteHost = string.Empty;
        public static string RemotePort = string.Empty;

        public NetWorkClient()
        {
            InitialNetWorkClient();
        }

        /// <summary>
        /// 初始化网络并连接
        /// </summary>
        public static void InitialNetWorkClient()
        {
            //TCPClient初始化
            ControllerServer = new AsyncTcpClient(Parameters.TcpServerHost, Parameters.TcpServerPort);

            //异常事件
            ControllerServer.ServerExceptionOccurred +=
                new EventHandler<TcpServerExceptionOccurredEventArgs>(ControllerServer_ServerExceptionOccurred);

            //已连接到服务器
            ControllerServer.ServerConnected +=
                new EventHandler<TcpServerConnectedEventArgs>(ControllerServer_ServerConnected);

            //与服务器断开连接
            ControllerServer.ServerDisconnected +=
                new EventHandler<TcpServerDisconnectedEventArgs>(ControllerServer_ServerDisconnected);

            //接收到服务器的数据(字符串)
            ControllerServer.PlaintextReceived +=
                new EventHandler<TcpDatagramReceivedEventArgs<string>>(ControllerServer_PlaintextReceived);

            //接收到服务器的数据(字节流)
            ControllerServer.DatagramReceived +=
                new EventHandler<TcpDatagramReceivedEventArgs<byte[]>>(ControllerServer_DatagramReceived);

            //连接服务器
            ControllerServer.Connect();
        }

        /// <summary>
        /// 异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ControllerServer_ServerExceptionOccurred(
              object sender, TcpServerExceptionOccurredEventArgs e)
        {
            try
            {
                Parameters.PrintfLogsExtended(string.Format(CultureInfo.InvariantCulture, "TCP server {0} exception occurred, {1}.", e.ToString(), e.Exception.Message));
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 已连接到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ControllerServer_ServerConnected(
          object sender, TcpServerConnectedEventArgs e)
        {
            try
            {
                JsonInterFace.ClientIP = ControllerServer.ClientIP;
                JsonInterFace.ClientPort = ControllerServer.ClientPort;
                JsonInterFace.Connected = true;
                Parameters.PrintfLogsExtended(string.Format(CultureInfo.InvariantCulture,
                  "Tcp Client {0}:{1} To TCP server {2} has connected.", JsonInterFace.ClientIP, JsonInterFace.ClientPort.ToString(), e.ToString()));
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 与服务器断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ControllerServer_ServerDisconnected(
          object sender, TcpServerDisconnectedEventArgs e)
        {
            try
            {
                JsonInterFace.Connected = false;
                Parameters.PrintfLogsExtended(string.Format(CultureInfo.InvariantCulture,
                "Tcp Client {0}:{1} To TCP server {2} has disconnected.", JsonInterFace.ClientIP, JsonInterFace.ClientPort.ToString(), e.ToString()));
            }
            catch (Exception Ex)
            {
                Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
            }
        }

        /// <summary>
        /// 接收到服务器的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ControllerServer_PlaintextReceived(
          object sender, TcpDatagramReceivedEventArgs<string> e)
        {
            try
            {
                /*
                if (e.TcpClient != null)
                {
                    if (RemoteHost == "" || RemoteHost == null)
                    {
                        RemoteHost = ((IPEndPoint)e.TcpClient.Client.RemoteEndPoint).Address.ToString();
                    }
                    if (RemotePort == "" || RemotePort == null)
                    {
                        RemotePort = ((IPEndPoint)e.TcpClient.Client.RemoteEndPoint).Port.ToString();
                    }
                }
                */
            }
            catch (Exception ex)
            {
                if (Parameters.LogStatus == 0)
                {
                    JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "接收服务端数据异常", ex.Message, ex.StackTrace);
                }
                else
                {
                    Parameters.PrintfLogsExtended("接收服务端数据异常", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 接收到服务器的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ControllerServer_DatagramReceived(
          object sender, TcpDatagramReceivedEventArgs<byte[]> e)
        {
            try
            {
                if (e.TcpClient != null)
                {
                    if (RemoteHost == "" || RemoteHost == null)
                    {
                        RemoteHost = ((IPEndPoint)e.TcpClient.Client.RemoteEndPoint).Address.ToString();
                    }
                    if (RemotePort == "" || RemotePort == null)
                    {
                        RemotePort = ((IPEndPoint)e.TcpClient.Client.RemoteEndPoint).Port.ToString();
                    }
                }

                //处理数据
                JsonInterFace.Parse(e.Datagram, RemoteHost, RemotePort);
            }
            catch (Exception ex)
            {
                if (Parameters.LogStatus == 0)
                {
                    JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "接收服务端数据异常", ex.Message, ex.StackTrace);
                }
                else
                {
                    Parameters.PrintfLogsExtended("接收服务端数据异常", ex.Message, ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 异步TCP客户端
        /// </summary>
        public class AsyncTcpClient : IDisposable
        {
            #region Fields

            private TcpClient tcpClient;
            private bool disposed = false;
            private int retries = 0;
            private string clientIP = string.Empty;
            private int clientPort = 0;

            #endregion

            #region Ctors

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteEP">远端服务器终结点</param>
            public AsyncTcpClient(IPEndPoint remoteEP)
              : this(new[] { remoteEP.Address }, remoteEP.Port)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteEP">远端服务器终结点</param>
            /// <param name="localEP">本地客户端终结点</param>
            public AsyncTcpClient(IPEndPoint remoteEP, IPEndPoint localEP)
              : this(new[] { remoteEP.Address }, remoteEP.Port, localEP)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteIPAddress">远端服务器IP地址</param>
            /// <param name="remotePort">远端服务器端口</param>
            public AsyncTcpClient(IPAddress remoteIPAddress, int remotePort)
              : this(new[] { remoteIPAddress }, remotePort)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteIPAddress">远端服务器IP地址</param>
            /// <param name="remotePort">远端服务器端口</param>
            /// <param name="localEP">本地客户端终结点</param>
            public AsyncTcpClient(
              IPAddress remoteIPAddress, int remotePort, IPEndPoint localEP)
              : this(new[] { remoteIPAddress }, remotePort, localEP)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteHostName">远端服务器主机名</param>
            /// <param name="remotePort">远端服务器端口</param>
            public AsyncTcpClient(string remoteHostName, int remotePort)
              : this(Dns.GetHostAddresses(remoteHostName), remotePort)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteHostName">远端服务器主机名</param>
            /// <param name="remotePort">远端服务器端口</param>
            /// <param name="localEP">本地客户端终结点</param>
            public AsyncTcpClient(
              string remoteHostName, int remotePort, IPEndPoint localEP)
              : this(Dns.GetHostAddresses(remoteHostName), remotePort, localEP)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteIPAddresses">远端服务器IP地址列表</param>
            /// <param name="remotePort">远端服务器端口</param>
            public AsyncTcpClient(IPAddress[] remoteIPAddresses, int remotePort)
              : this(remoteIPAddresses, remotePort, null)
            {
            }

            /// <summary>
            /// 异步TCP客户端
            /// </summary>
            /// <param name="remoteIPAddresses">远端服务器IP地址列表</param>
            /// <param name="remotePort">远端服务器端口</param>
            /// <param name="localEP">本地客户端终结点</param>
            public AsyncTcpClient(
              IPAddress[] remoteIPAddresses, int remotePort, IPEndPoint localEP)
            {
                try
                {
                    this.Addresses = remoteIPAddresses;
                    this.Port = remotePort;
                    this.LocalIPEndPoint = localEP;
                    this.Encoding = Encoding.Default;

                    if (this.LocalIPEndPoint != null)
                    {
                        this.tcpClient = new TcpClient(this.LocalIPEndPoint);
                    }
                    else
                    {
                        this.tcpClient = new TcpClient();
                    }

                    Retries = 0;
                    RetryInterval = 655350;
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            #endregion

            #region Properties

            /// <summary>
            /// 是否已与服务器建立连接
            /// </summary>
            public bool Connected { get { return tcpClient.Client.Connected; } }
            /// <summary>
            /// 远端服务器的IP地址列表
            /// </summary>
            public IPAddress[] Addresses { get; private set; }
            /// <summary>
            /// 远端服务器的端口
            /// </summary>
            public int Port { get; private set; }
            /// <summary>
            /// 连接重试次数
            /// </summary>
            public int Retries { get; set; }
            /// <summary>
            /// 连接重试间隔
            /// </summary>
            public int RetryInterval { get; set; }
            /// <summary>
            /// 远端服务器终结点
            /// </summary>
            public IPEndPoint RemoteIPEndPoint
            {
                get { return new IPEndPoint(Addresses[0], Port); }
            }
            /// <summary>
            /// 本地客户端终结点
            /// </summary>
            protected IPEndPoint LocalIPEndPoint { get; private set; }
            /// <summary>
            /// 通信所使用的编码
            /// </summary>
            public Encoding Encoding { get; set; }

            public string ClientIP
            {
                get
                {
                    return clientIP;
                }

                set
                {
                    clientIP = value;
                }
            }

            public int ClientPort
            {
                get
                {
                    return clientPort;
                }

                set
                {
                    clientPort = value;
                }
            }

            #endregion

            #region Connect

            /// <summary>
            /// 连接到服务器
            /// </summary>
            /// <returns>异步TCP客户端</returns>
            public AsyncTcpClient Connect()
            {
                try
                {
                    if (!Connected)
                    {
                        // start the async connect operation
                        tcpClient.BeginConnect(
                          Addresses, Port, HandleTcpServerConnected, tcpClient);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("连接到服务器", string.Concat<byte>(Addresses[0].GetAddressBytes()) + ":" + Port.ToString(), ex.Message, ex.StackTrace);
                }
                return this;
            }

            /// <summary>
            /// 关闭与服务器的连接
            /// </summary>
            /// <returns>异步TCP客户端</returns>
            public AsyncTcpClient Close()
            {
                try
                {
                    if (Connected)
                    {
                        retries = 0;
                        tcpClient.Close();
                        RaiseServerDisconnected(Addresses, Port);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("关闭与服务器的连接", ex.Message, ex.StackTrace);
                }
                return this;
            }

            #endregion

            #region Receive

            private void HandleTcpServerConnected(IAsyncResult ar)
            {
                try
                {
                    tcpClient.EndConnect(ar);
                    RaiseServerConnected(Addresses, Port);
                    retries = 0;
                }
                catch (Exception ex)
                {
                    //ExceptionHandler.Handle(ex);
                    Parameters.PrintfLogsExtended("HandleTcpServerConnected", ex.Message, ex.StackTrace);
                    if (retries > 0)
                    {
                        Parameters.PrintfLogsExtended(string.Format(CultureInfo.InvariantCulture,
                          "Connect to server with retry {0} failed.", retries));
                    }

                    retries++;
                    if (retries > Retries)
                    {
                        // we have failed to connect to all the IP Addresses, 
                        // connection has failed overall.
                        RaiseServerExceptionOccurred(Addresses, Port, ex);
                        return;
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended(string.Format(CultureInfo.InvariantCulture,
                          "Waiting {0} seconds before retrying to connect to server.",
                          RetryInterval));
                        Thread.Sleep(TimeSpan.FromSeconds(RetryInterval));
                        Connect();
                        return;
                    }
                }

                // we are connected successfully and start asyn read operation.
                if (tcpClient.Connected)
                {
                    try
                    {
                        byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                        tcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
                    }
                    catch (Exception Ex)
                    {
                        Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                    }
                }
            }

            private void HandleDatagramReceived(IAsyncResult ar)
            {
                try
                {
                    if (tcpClient.Connected)
                    {
                        NetworkStream stream = tcpClient.GetStream();

                        int numberOfReadBytes = 0;
                        try
                        {
                            numberOfReadBytes = stream.EndRead(ar);
                        }
                        catch
                        {
                            numberOfReadBytes = 0;
                        }

                        if (numberOfReadBytes == 0)
                        {
                            // connection has been closed
                            Close();
                            return;
                        }

                        // received byte and trigger event notification
                        byte[] buffer = (byte[])ar.AsyncState;
                        byte[] receivedBytes = new byte[numberOfReadBytes];
                        Buffer.BlockCopy(buffer, 0, receivedBytes, 0, numberOfReadBytes);
                        RaiseDatagramReceived(tcpClient, receivedBytes);
                        RaisePlaintextReceived(tcpClient, receivedBytes);

                        // then start reading from the network again
                        stream.BeginRead(
                          buffer, 0, buffer.Length, HandleDatagramReceived, buffer);
                    }
                }
                catch (Exception ex)
                {
                    Parameters.PrintfLogsExtended("Client To Server Session", ex.Message, ex.StackTrace);
                }
            }

            #endregion

            #region Events

            /// <summary>
            /// 接收到数据报文事件
            /// </summary>
            public event EventHandler<TcpDatagramReceivedEventArgs<byte[]>> DatagramReceived;
            /// <summary>
            /// 接收到数据报文-明文事件
            /// </summary>
            public event EventHandler<TcpDatagramReceivedEventArgs<string>> PlaintextReceived;

            private void RaiseDatagramReceived(TcpClient sender, byte[] datagram)
            {
                try
                {
                    if (DatagramReceived != null)
                    {
                        DatagramReceived(this,
                          new TcpDatagramReceivedEventArgs<byte[]>(sender, datagram));
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            private void RaisePlaintextReceived(TcpClient sender, byte[] datagram)
            {
                try
                {
                    if (PlaintextReceived != null)
                    {
                        PlaintextReceived(this,
                          new TcpDatagramReceivedEventArgs<string>(
                            sender, this.Encoding.GetString(datagram, 0, datagram.Length)));
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            /// <summary>
            /// 与服务器的连接已建立事件
            /// </summary>
            public event EventHandler<TcpServerConnectedEventArgs> ServerConnected;
            /// <summary>
            /// 与服务器的连接已断开事件
            /// </summary>
            public event EventHandler<TcpServerDisconnectedEventArgs> ServerDisconnected;
            /// <summary>
            /// 与服务器的连接发生异常事件
            /// </summary>
            public event EventHandler<TcpServerExceptionOccurredEventArgs> ServerExceptionOccurred;

            private void RaiseServerConnected(IPAddress[] ipAddresses, int port)
            {
                try
                {
                    if (ServerConnected != null)
                    {
                        if (this.tcpClient != null)
                        {
                            ClientPort = ((IPEndPoint)this.tcpClient.Client.LocalEndPoint).Port;
                            ClientIP = ((IPEndPoint)this.tcpClient.Client.LocalEndPoint).Address.ToString();
                        }
                        ServerConnected(this,
                          new TcpServerConnectedEventArgs(ipAddresses, port));
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }
            //-----------------------------------------------------------------------

            private void RaiseServerDisconnected(IPAddress[] ipAddresses, int port)
            {
                try
                {
                    if (ServerDisconnected != null)
                    {
                        ServerDisconnected(this,
                          new TcpServerDisconnectedEventArgs(ipAddresses, port));
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            private void RaiseServerExceptionOccurred(
              IPAddress[] ipAddresses, int port, Exception innerException)
            {
                try
                {
                    if (ServerExceptionOccurred != null)
                    {
                        ServerExceptionOccurred(this,
                          new TcpServerExceptionOccurredEventArgs(
                            ipAddresses, port, innerException));
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            #endregion

            #region Send

            /// <summary>
            /// 发送报文
            /// </summary>
            /// <param name="datagram">报文</param>
            public void Send(byte[] datagram)
            {
                try
                {
                    if (datagram == null)
                    {
                        Parameters.PrintfLogsExtended("Send data is null!");
                        return;
                    }

                    if (!Connected)
                    {
                        RaiseServerDisconnected(Addresses, Port);
                        Parameters.PrintfLogsExtended("Error: Action Send", "This client has not connected to server! Can not Send Any Data to remote host!");
                        return;
                    }

                    if (Connected)
                    {
                        if (tcpClient != null)
                        {
                            tcpClient.GetStream().BeginWrite(
                              datagram, 0, datagram.Length, HandleDatagramWritten, tcpClient);
                        }
                    }
                }
                catch (Exception ex)
                {
                    JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "Send Messsage", ex.Message, ex.StackTrace);
                    Parameters.PrintfLogsExtended("Error: Action Send", ex.Message + " Can not Send Any Data to remote host!");
                }
            }

            private void HandleDatagramWritten(IAsyncResult ar)
            {
                try
                {
                    if (ar != null)
                    {
                        if (((TcpClient)ar.AsyncState).Connected)
                        {
                            ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            /// <summary>
            /// 发送报文
            /// </summary>
            /// <param name="datagram">报文</param>
            public void Send(string datagram)
            {
                try
                {
                    if (!Connected)
                    {
                        RaiseServerDisconnected(Addresses, Port);
                        Parameters.PrintfLogsExtended("Error: Action Send", "This client has not connected to server! Can not Send Any Data to remote host!");
                        return;
                    }

                    if (Connected)
                    {
                        Send(System.Text.Encoding.UTF8.GetBytes(datagram));
                    }

                    if (Parameters.LogStatus == 0)
                    {
                        JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "发送数据到服务器", datagram, "正常");
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("发送数据到服务器", datagram);
                    }
                }
                catch (Exception ex)
                {
                    if (Parameters.LogStatus == 0)
                    {
                        JsonInterFace.IODataHelper.SaveLogs(DateTime.Now.ToString(), "发送数据到服务器", datagram, ex.Message);
                    }
                    else
                    {
                        Parameters.PrintfLogsExtended("发送数据到服务器", datagram, "错误事件", ex.Message, ex.StackTrace);
                    }
                }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, 
            /// releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources
            /// </summary>
            /// <param name="disposing"><c>true</c> to release both managed 
            /// and unmanaged resources; <c>false</c> 
            /// to release only unmanaged resources.
            /// </param>
            protected virtual void Dispose(bool disposing)
            {
                if (!this.disposed)
                {
                    if (disposing)
                    {
                        try
                        {
                            Close();

                            if (tcpClient != null)
                            {
                                tcpClient = null;
                            }
                        }
                        catch (SocketException ex)
                        {
                            Parameters.PrintfLogsExtended(ex.Message, ex.StackTrace);
                        }
                    }

                    disposed = true;
                }
            }

            #endregion
        }

        /// <summary>
        /// 与服务器的连接已建立事件参数
        /// </summary>
        public class TcpServerConnectedEventArgs : EventArgs
        {
            /// <summary>
            /// 与服务器的连接已建立事件参数
            /// </summary>
            /// <param name="ipAddresses">服务器IP地址列表</param>
            /// <param name="port">服务器端口</param>
            public TcpServerConnectedEventArgs(IPAddress[] ipAddresses, int port)
            {
                if (ipAddresses == null)
                    Parameters.PrintfLogsExtended("与服务器的连接已建立事件参数", "IPAddresses null: TcpServerConnectedEventArgs()");

                this.Addresses = ipAddresses;
                this.Port = port;
            }

            /// <summary>
            /// 服务器IP地址列表
            /// </summary>
            public IPAddress[] Addresses { get; private set; }
            /// <summary>
            /// 服务器端口
            /// </summary>
            public int Port { get; private set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                string s = string.Empty;
                try
                {
                    if (Addresses != null)
                    {
                        foreach (var item in Addresses)
                        {
                            s = s + item.ToString() + ',';
                        }
                        s = s.TrimEnd(',');
                        s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        s = "";
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }

                return s;
            }
        }

        /// <summary>
        /// 与服务器的连接发生异常事件参数
        /// </summary>
        public class TcpServerExceptionOccurredEventArgs : EventArgs
        {
            /// <summary>
            /// 与服务器的连接发生异常事件参数
            /// </summary>
            /// <param name="ipAddresses">服务器IP地址列表</param>
            /// <param name="port">服务器端口</param>
            /// <param name="innerException">内部异常</param>
            public TcpServerExceptionOccurredEventArgs(
              IPAddress[] ipAddresses, int port, Exception innerException)
            {
                if (ipAddresses == null)
                    Parameters.PrintfLogsExtended("与服务器的连接发生异常事件参数", "ipAddresses null: TcpServerExceptionOccurredEventArgs()");

                this.Addresses = ipAddresses;
                this.Port = port;
                this.Exception = innerException;
            }

            /// <summary>
            /// 服务器IP地址列表
            /// </summary>
            public IPAddress[] Addresses { get; private set; }
            /// <summary>
            /// 服务器端口
            /// </summary>
            public int Port { get; private set; }
            /// <summary>
            /// 内部异常
            /// </summary>
            public Exception Exception { get; private set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                string s = string.Empty;
                try
                {
                    if (Addresses != null)
                    {
                        foreach (var item in Addresses)
                        {
                            s = s + item.ToString() + ',';
                        }
                        s = s.TrimEnd(',');
                        s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        s = "";
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
                return s;
            }
        }

        /// <summary>
        /// 与服务器的连接已断开事件参数
        /// </summary>
        public class TcpServerDisconnectedEventArgs : EventArgs
        {
            /// <summary>
            /// 与服务器的连接已断开事件参数
            /// </summary>
            /// <param name="ipAddresses">服务器IP地址列表</param>
            /// <param name="port">服务器端口</param>
            public TcpServerDisconnectedEventArgs(IPAddress[] ipAddresses, int port)
            {
                if (ipAddresses == null)
                    Parameters.PrintfLogsExtended("与服务器的连接已断开", "ipAddresses null: TcpServerDisconnectedEventArgs()");

                this.Addresses = ipAddresses;
                this.Port = port;
            }

            /// <summary>
            /// 服务器IP地址列表
            /// </summary>
            public IPAddress[] Addresses { get; private set; }

            /// <summary>
            /// 服务器端口
            /// </summary>
            public int Port { get; private set; }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                string s = string.Empty;
                try
                {
                    if (Addresses != null)
                    {
                        foreach (var item in Addresses)
                        {
                            s = s + item.ToString() + ',';
                        }
                        s = s.TrimEnd(',');
                        s = s + ":" + Port.ToString(CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
                return s;
            }
        }

        /// <summary>
        /// 接收到数据报文事件参数
        /// </summary>
        /// <typeparam name="T">报文类型</typeparam>
        public class TcpDatagramReceivedEventArgs<T> : EventArgs
        {
            /// <summary>
            /// 接收到数据报文事件参数
            /// </summary>
            /// <param name="tcpClient">客户端</param>
            /// <param name="datagram">报文</param>
            public TcpDatagramReceivedEventArgs(TcpClient tcpClient, T datagram)
            {
                TcpClient = tcpClient;
                Datagram = datagram;
            }

            /// <summary>
            /// 客户端
            /// </summary>
            public TcpClient TcpClient { get; private set; }
            /// <summary>
            /// 报文
            /// </summary>
            public T Datagram { get; private set; }
        }

        /// <summary>
        /// 与客户端的连接已断开事件参数
        /// </summary>
        public class TcpClientDisconnectedEventArgs : EventArgs
        {
            /// <summary>
            /// 与客户端的连接已断开事件参数
            /// </summary>
            /// <param name="tcpClient">客户端</param>
            public TcpClientDisconnectedEventArgs(TcpClient tcpClient)
            {
                if (tcpClient == null)
                    Parameters.PrintfLogsExtended("与客户端的连接已断开事件参数", "tcpClient null: TcpClientDisconnectedEventArgs()");

                this.TcpClient = tcpClient;
            }

            /// <summary>
            /// 客户端
            /// </summary>
            public TcpClient TcpClient { get; private set; }
        }

        /// <summary>
        /// 与客户端的连接已建立事件参数
        /// </summary>
        public class TcpClientConnectedEventArgs : EventArgs
        {
            /// <summary>
            /// 与客户端的连接已建立事件参数
            /// </summary>
            /// <param name="tcpClient">客户端</param>
            public TcpClientConnectedEventArgs(TcpClient tcpClient)
            {
                if (tcpClient == null)
                    Parameters.PrintfLogsExtended("与客户端的连接已建立事件参数", "tcpClient null: TcpClientConnectedEventArgs()");

                this.TcpClient = tcpClient;
            }

            /// <summary>
            /// 客户端
            /// </summary>
            public TcpClient TcpClient { get; private set; }
        }

        /// <summary>
        /// TCP客户端State
        /// Internal class to join the TCP client and buffer together
        /// for easy management in the server
        /// </summary>
        internal class TcpClientState
        {
            /// <summary>
            /// Constructor for a new Client
            /// </summary>
            /// <param name="tcpClient">The TCP client</param>
            /// <param name="buffer">The byte array buffer</param>
            public TcpClientState(TcpClient tcpClient, byte[] buffer)
            {
                try
                {
                    if (tcpClient == null)
                        Parameters.PrintfLogsExtended("TCP客户端State", "Constructor for a new Client", "tcpClient null: TcpClientState()");
                    if (buffer == null)
                        Parameters.PrintfLogsExtended("TCP客户端State", "Constructor for a new Client", "buffer null: TcpClientState()");

                    this.TcpClient = tcpClient;
                    this.Buffer = buffer;
                }
                catch (Exception Ex)
                {
                    Parameters.PrintfLogsExtended(Ex.Message, Ex.StackTrace);
                }
            }

            /// <summary>
            /// Gets the TCP Client
            /// </summary>
            public TcpClient TcpClient { get; private set; }

            /// <summary>
            /// Gets the Buffer.
            /// </summary>
            public byte[] Buffer { get; private set; }

            /// <summary>
            /// Gets the network stream
            /// </summary>
            public NetworkStream NetworkStream
            {
                get { return TcpClient.GetStream(); }
            }
        }
    }
}
