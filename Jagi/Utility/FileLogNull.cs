using Jagi.Helpers;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jagi.Utility
{
    /// <summary>
    /// 主要目的：使用 Nlog 作為記錄檔案的工具，但提供在程式中可以動態指定紀錄的檔案
    /// 如果是 null 則不會紀錄
    /// 使用方式：
    ///        using (var logger = new FileLog(path))
    ///        {
    ///            logger.Info("Into Index");
    ///        }
    /// 使用 using 的目的在於呼叫 Dispse() 讓記錄檔案改為預設的路徑（fileName="${basedir}/log/default.log"）
    /// </summary>
    public class FileLogNull : FileLog
    {
        private bool isLog = true;
        /// <summary>
        /// 指定紀錄的檔案，如果是 null 則不會紀錄
        /// </summary>
        /// <param name="filename"></param>
        public FileLogNull(string filename) : base(filename)
        {
            if (string.IsNullOrEmpty(filename))
                isLog = false;
        }

        public override void Info(string messaage, params object[] args)
        {
            if (isLog)
            Info(messaage.FormatWith(args));
        }

        public override void Info(string message)
        {
            if (isLog)
                base.Info(message);
        }

        public override void Warn(string messaage, params object[] args)
        {
            if (isLog)
                Warn(messaage.FormatWith(args));
        }

        public override void Warn(string messaage)
        {
            if (isLog)
                base.Warn(messaage);
        }

        public override void Debug(string messaage, params object[] args)
        {
            if (isLog)
                Debug(messaage.FormatWith(args));
        }

        public override void Debug(string message)
        {
            if (isLog)
                base.Debug(message);
        }

        public override void Trace(string messaage, params object[] args)
        {
            if (isLog)
                Trace(messaage.FormatWith(args));
        }

        public override void Trace(string message)
        {
            if (isLog)
                base.Trace(message);
        }

        public override void Error(string messaage, params object[] args)
        {
            if (isLog)
                Error(messaage.FormatWith(args));
        }

        public override void Error(string message)
        {
            if (isLog)
                base.Error(message);
        }

        public override void Error(string message, Exception ex, params object[] args)
        {
            if (isLog)
                base.Error(message, ex, args);
        }

        public override void Error(Exception x)
        {
            if (isLog)
                base.Error(LogUtility.BuildExceptionMessage(x));
        }

        public override void Fatal(string message)
        {
            if (isLog)
                base.Fatal(message);
        }

        public override void Fatal(Exception x)
        {
            if (isLog)
                base.Fatal(LogUtility.BuildExceptionMessage(x));
        }
    }
}
