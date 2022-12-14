using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelnetAdjustment {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            try {

                #region 线程异常处理 全局的异常处理
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);//处理处理未捕获的异常 
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);  //处理UI线程异常  
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);//处理非UI线程异常
                #endregion
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex) {
                string str = GetExceptionMsg(ex, string.Empty);
                MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
            string str = GetExceptionMsg(e.Exception, e.ToString());
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("*************************异常文本*************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null) {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                //sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("*********************************************************");
            return sb.ToString();
        }

    }
}
