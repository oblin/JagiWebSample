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
    /// 使用方式：
    ///        using (var logger = new FileLog(path))
    ///        {
    ///            logger.Info("Into Index");
    ///        }
    /// 使用 using 的目的在於呼叫 Dispse() 讓記錄檔案改為預設的路徑（fileName="${basedir}/log/default.log"）
    /// </summary>
    public class FileLog : IDisposable
    {
        private Logger _logger;
        private Layout _originalLayout;

        /// <summary>
        /// 使用預設路徑
        /// </summary>
        public FileLog() : this(string.Empty) { }

        /// <summary>
        /// 指定紀錄的檔案，請務必要用 using() {} 方式，讓 Dispose() 可以改回預設的路徑
        /// </summary>
        /// <param name="filename"></param>
        public FileLog(string filename)
        {
            _logger = LogManager.GetCurrentClassLogger();
            if (!string.IsNullOrEmpty(filename))
            {
                var configuration = LogManager.Configuration;
                var target = configuration.FindTargetByName("filelog") as FileTarget;
                _originalLayout = target.FileName;
                target.FileName = filename;
            }
        }

        public virtual void Info(string messaage, params object[] args)
        {
            Info(messaage.FormatWith(args));
        }

        public virtual void Info(string message)
        {
            _logger.Info(message);
        }

        public virtual void Warn(string messaage, params object[] args)
        {
            Warn(messaage.FormatWith(args));
        }

        public virtual void Warn(string messaage)
        {
            _logger.Warn(messaage);
        }

        public virtual void Debug(string messaage, params object[] args)
        {
            Debug(messaage.FormatWith(args));
        }

        public virtual void Debug(string message)
        {
            _logger.Debug(message);
        }

        public virtual void Trace(string messaage, params object[] args)
        {
            Trace(messaage.FormatWith(args));
        }

        public virtual void Trace(string message)
        {
            _logger.Trace(message);
        }

        public virtual void Error(string messaage, params object[] args)
        {
            Error(messaage.FormatWith(args));
        }

        public virtual void Error(string message)
        {
            _logger.Error(message);
        }

        public virtual void Error(string message, Exception ex, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public virtual void Error(Exception x)
        {
            Error(LogUtility.BuildExceptionMessage(x));
        }

        public virtual void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public virtual void Fatal(Exception x)
        {
            Fatal(LogUtility.BuildExceptionMessage(x));
        }

        public void Dispose()
        {
            if (_originalLayout != null)
            {
                var configuration = LogManager.Configuration;
                var target = configuration.FindTargetByName("filelog") as FileTarget;
                target.FileName = _originalLayout;
            }

        }
    }
}
