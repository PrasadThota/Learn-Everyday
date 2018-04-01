using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private readonly TodoContext _context;

        /// <summary>
        /// preceding code: 
        ///The constructor uses Dependency Injection to inject the database context (TodoContext) into the controller. The database context is used in each of the CRUD methods in the controller.
        /// The constructor adds an item to the in-memory database if one doesn't exist.
        /// </summary>
        /// <param name="context"></param>
        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        #region GET_METHODS

        //These methods implement the two GET methods:
        // 1 - GET /api/todo
        // 2 - GET /api/todo/{id}
        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList<TodoItem>();
        }

        [HttpGet( "{id}", Name = "GetTodo")]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.First
        }
#endregion


    }

   
}
}
