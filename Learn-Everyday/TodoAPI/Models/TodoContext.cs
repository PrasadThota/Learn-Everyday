using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {

        }

        /// <summary>
        /// Gets a set of Todo Items from the DB
        /// </summary>
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}
