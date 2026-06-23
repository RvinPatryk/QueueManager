using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QueueManager.Data;
using QueueManager.Models;

namespace QueueManager.Repositories
{
    public class TaskRepository
    {
        public List<QueueTask> GetAll()
        {
            using var db = new QueueManagerDbContext();

            db.Database.Migrate();

            return db.Tasks
                .AsNoTracking()
                .OrderBy(task => task.Id)
                .ToList();
        }

        public QueueTask Add(QueueTask task)
        {
            using var db = new QueueManagerDbContext();

            db.Tasks.Add(task);
            db.SaveChanges();

            return task;
        }

        public void Update(QueueTask task)
        {
            using var db = new QueueManagerDbContext();

            db.Tasks.Update(task);
            db.SaveChanges();
        }

        public void Delete(int taskId)
        {
            using var db = new QueueManagerDbContext();

            var task = db.Tasks.Find(taskId);

            if (task == null)
                return;

            db.Tasks.Remove(task);
            db.SaveChanges();
        }

        public void DeleteMany(IEnumerable<int> taskIds)
        {
            using var db = new QueueManagerDbContext();

            var ids = taskIds.ToList();

            var tasksToDelete = db.Tasks
                .Where(task => ids.Contains(task.Id))
                .ToList();

            if (tasksToDelete.Count == 0)
                return;

            db.Tasks.RemoveRange(tasksToDelete);
            db.SaveChanges();
        }
    }
}