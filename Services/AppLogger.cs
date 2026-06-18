using System;
using System.IO;

namespace QueueManager.Services
{
    public static class AppLogger
    {
        private static readonly object _lock = new();
        private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string _logFilePath = Path.Combine(_logDirectory, "app-log.txt");

        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public static void Warning(string message)
        {
            WriteLog("WARNING", message);
        }

        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        public static void Error(string message, Exception exception)
        {
            WriteLog("ERROR", $"{message} | Exception: {exception.Message}");
        }

        private static void WriteLog(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(_logDirectory))
                        Directory.CreateDirectory(_logDirectory);

                    string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                    File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                }
            }
            catch
            {
                // Nie wywalamy aplikacji, jeśli zapis logu się nie uda.
            }
        }
    }
}