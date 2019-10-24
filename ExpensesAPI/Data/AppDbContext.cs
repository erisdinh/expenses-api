using ExpensesAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExpensesAPI.Data
{

    // Context for EF to access data
    public class AppDbContext : DbContext
    {
        // the name of the dataset is the table name
        public DbSet<Entry> Entries { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext() : base("name=ExpensesDb")
        {

        }
    }
}