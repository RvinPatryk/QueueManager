using System;
using System.IO;

namespace QueueManager.Services
{
    public static class AppLogger
    {
        private static readonly object _lock = new();

        private static readonly string _logDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        private static readonly string _logFilePath =
            Path.Combine(_logDirectory, "app-log.txt");

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

        public static string ReadAll()
        {
            try
            {
                lock (_lock)
                {
                    if (!File.Exists(_logFilePath))
                        return "Brak logów.";

                    return File.ReadAllText(_logFilePath);
                }
            }
            catch (Exception ex)
            {
                return $"Nie udało się odczytać logów. Szczegóły: {ex.Message}";
            }
        }

        public static void Clear()
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(_logDirectory))
                        Directory.CreateDirectory(_logDirectory);

                    File.WriteAllText(_logFilePath, string.Empty);
                }
            }
            catch
            {
                // Nie wywalamy aplikacji, jeśli czyszczenie logu się nie uda.
            }
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