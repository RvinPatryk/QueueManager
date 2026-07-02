using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using QueueManager.Data;
using QueueManager.Models;

namespace QueueManager.Repositories
{
    public class QueueSettingsRepository
    {
        public QueueSettings Get()
        {
            using var db = new QueueManagerDbContext();

            var settings = db.QueueSettings.FirstOrDefault();

            if (settings != null)
                return settings;

            settings = new QueueSettings
            {
                SelectedAlgorithm = SchedulingAlgorithm.FIFO
            };

            db.QueueSettings.Add(settings);
            db.SaveChanges();

            return settings;
        }

        public void UpdateAlgorithm(SchedulingAlgorithm algorithm)
        {
            using var db = new QueueManagerDbContext();

            var settings = db.QueueSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new QueueSettings
                {
                    SelectedAlgorithm = algorithm
                };

                db.QueueSettings.Add(settings);
            }
            else
            {
                settings.SelectedAlgorithm = algorithm;
            }

            db.SaveChanges();
        }
    }
}