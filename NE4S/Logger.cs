using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NE4S
{
    public sealed class Logger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warn,
            Error,
            Critical
        };

        private static readonly Logger singleInstance = new Logger();

        private readonly List<string> pool;
        private readonly string fileName = "log";
        private readonly long logSizeMaximum = 1024 * 1024;
        private readonly int poolSizeMaximum = 10;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private static readonly object logLockObj = new object();
        private readonly object writeLockObj = new object();

        private static int warnCount = 0;
        private static int errorCount = 0;
        private static int criticalCount = 0;

        private string LogDirectory
        {
            get
            {
                return $"{Directory.GetParent(Assembly.GetExecutingAssembly().Location)}\\log";
            }
        }

        private string LogFilePath
        {
            get
            {
                return $"{LogDirectory}\\{fileName}.log";
            }
        }

        private Logger()
        {
            pool = new List<string>();
            stopwatch.Start();
            Log(LogLevel.Info, "ロガーを起動します。");
            var assembly = Assembly.GetExecutingAssembly();
            var appName = $"M4ple Editor v{assembly.GetName().Version}";
            Log(LogLevel.Info, $"アプリケーション名 : {appName}");
            Log(LogLevel.Info, $"Location : {assembly.Location}");
            Log(LogLevel.Info, $"CurrentDirectory : {Environment.CurrentDirectory}");
        }

        ~Logger()
        {
            stopwatch.Stop();
            Info($"起動時間 : {stopwatch.ElapsedMilliseconds}[ms]");
            Info($"{warnCount}件の警告, {errorCount}件のエラー, {criticalCount}件の致命的エラー");
            Info("ロガーを終了します。");
            WriteToFile();
        }

        private void Log(LogLevel logLevel, string message, bool assert = false)
        {
            var logText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff ");

            switch (logLevel)
            {
                case LogLevel.Debug: logText += "[Debug] "; break;
                case LogLevel.Info: logText += "[Info] "; break;
                case LogLevel.Warn: logText += "[Warn] "; break;
                case LogLevel.Error: logText += "[Error] "; break;
                case LogLevel.Critical: logText += "[Critical] "; break;
                default: logText += "[Unknown] "; break;
            }

            var stackFrame = new StackTrace(2, true).GetFrame(0);
            logText += $"[{stackFrame.GetMethod().ReflectedType}.{stackFrame.GetMethod().Name} : Line {stackFrame.GetFileLineNumber()}] ";
            
            logText += message;

            System.Diagnostics.Debug.WriteLine(logText);
            System.Diagnostics.Debug.Assert(!assert, message);

            pool.Add(logText);
            if (pool.Count >= poolSizeMaximum)
            {
                if (!WriteToFile()) { Console.WriteLine("[Critical] ログの保存に失敗しました。"); }
            }
        }

        public static void Debug(string message, bool assert = false)
        {
            lock (logLockObj)
            {
                singleInstance.Log(LogLevel.Debug, message, assert);
            }
        }
        public static void Info(string message, bool assert = false)
        {
            lock (logLockObj)
            {
                singleInstance.Log(LogLevel.Info, message, assert);
            }
        }
        public static void Warn(string message, bool assert = false)
        {
            lock (logLockObj)
            {
                singleInstance.Log(LogLevel.Warn, message, assert);
                warnCount++;
            }
        }
        public static void Error(string message, bool assert = false)
        {
            lock (logLockObj)
            {
                singleInstance.Log(LogLevel.Error, message, assert);
                errorCount++;
            }
        }
        public static void Critical(string message, bool assert = false)
        {
            lock (logLockObj)
            {
                singleInstance.Log(LogLevel.Critical, message, assert);
                criticalCount++;
            }
        }

        private bool WriteToFile()
        {
            lock (writeLockObj)
            {
                if (!pool.Any()) { return true; }
                if (!MakeLogDirectory()) { return false; }

                var path = LogFilePath;
                if (GetFileSize(path) >= logSizeMaximum)
                {
                    if (!RenameFile(path, MakeLogFilePath(DateTime.Now))) { return false; }
                }

                try
                {
                    using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
                    {
                        var buf = "";
                        pool.ForEach(x =>
                        {
                            buf += $"{x.ToString()}\r\n";
                        });

                        writer.Write(buf);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[Critical] ログの書き出しに失敗しました。");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);

                    return false;
                }

                pool.Clear();
                return true;
            }
        }

        private bool MakeLogDirectory()
        {
            var path = LogDirectory;
            if (Directory.Exists(path)) { return true; }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Console.WriteLine("[Critical] ログファイル出力ディレクトリの作成に失敗しました。");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }

            return true;
        }

        private string MakeLogFilePath(DateTime dateTime)
        {
            return $"{LogDirectory}\\{fileName}_{dateTime.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
        }

        private long GetFileSize(string path)
        {
            FileInfo file;
            try
            {
                file = new FileInfo(path);
                if (file == null) { return 0; }
            }
            catch(Exception e)
            {
                Console.WriteLine("[Warn] ファイルサイズの取得中に例外が発生しました。");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return -1;
            }

            return (file == null || !file.Exists) ? -1 : file.Length;
        }

        private bool RenameFile(string source, string target)
        {
            try
            {
                File.Move(source, target);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Critical] ログファイルのリネームに失敗しました。");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }
    }
}
