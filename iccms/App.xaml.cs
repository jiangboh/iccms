using Lierda.WPFHelper;
using System;
using System.Windows;
using System.Windows.Threading;

namespace iccms
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        LierdaCracker MemoryCracker = new LierdaCracker();

        protected override void OnStartup(StartupEventArgs e)
        {
            //垃圾回收间隔时间:秒
            MemoryCracker.Cracker(30);
            base.OnStartup(e);

            //注册Application_Error件事
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }

        //异常处理逻辑
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //处理完后，我们需要将Handler=true表示已此异常已处理过
            try
            {
                e.Handled = true;
                ParameterControl.Parameters.PrintfLogsExtended("捕获到全局异常事件信息：" + e.Exception.Message, e.Exception.StackTrace);
            }
            catch (Exception Ex)
            {
                ParameterControl.Parameters.PrintfLogsExtended("处理全局异常事件失败：" + Ex.Message);
            }
        }
    }
}