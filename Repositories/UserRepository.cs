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
    public class UserRepository
    {
        public User? GetByUsername(string username)
        {
            using var db = new QueueManagerDbContext();

            return db.Users
                .AsNoTracking()
                .FirstOrDefault(user => user.Username == username);
        }

        public bool UsernameExists(string username)
        {
            using var db = new QueueManagerDbContext();

            return db.Users.Any(user => user.Username == username);
        }

        public User Add(User user)
        {
            using var db = new QueueManagerDbContext();

            db.Users.Add(user);
            db.SaveChanges();

            return user;
        }

        public List<User> GetAll()
        {
            using var db = new QueueManagerDbContext();

            return db.Users
                .AsNoTracking()
                .OrderBy(user => user.Username)
                .ToList();
        }
    }
}