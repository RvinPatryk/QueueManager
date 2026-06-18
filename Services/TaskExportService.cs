using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QueueManager.Models;

namespace QueueManager.Services
{
    public class TaskExportService
    {
        public void ExportToCsv(IEnumerable<QueueTask> tasks, string filePath)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Id;Nazwa;Opis;Priorytet;Autor;OsobaPrzypisana;DataUtworzenia;DataRozpoczecia;DataUkonczenia;Status;PrzewidzianyCzas;Termin;RzeczywistyCzas");

            foreach (var task in tasks)
            {
                sb.AppendLine(string.Join(";",
                    Escape(task.Id.ToString()),
                    Escape(task.Nazwa),
                    Escape(task.Opis),
                    Escape(task.Priorytet.ToString()),
                    Escape(task.Autor),
                    Escape(task.OsobaPrzypisana),
                    Escape(task.DataUtworzenia.ToString("yyyy-MM-dd HH:mm:ss")),
                    Escape(task.DataRozpoczecia?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                    Escape(task.DataUkonczenia?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                    Escape(task.Status.ToString()),
                    Escape(task.PrzewidzianyCzas.ToString()),
                    Escape(task.Termin?.ToString("yyyy-MM-dd") ?? ""),
                    Escape(task.RzeczywistyCzas?.ToString() ?? "")
                ));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private static string Escape(string value)
        {
            if (value.Contains(';') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }
    }
}