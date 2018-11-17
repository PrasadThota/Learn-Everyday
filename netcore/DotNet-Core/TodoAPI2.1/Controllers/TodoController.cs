using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using TodoAPI2.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI2.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
    public class TodoController : Controller
    {
		private readonly TodoContext _context;
		
		public TodoController(TodoContext context)
		{
			_context = context;

			if (_context.TodoItems.Count() == 0)
			{
				_context.TodoItems.Add(
					new TodoItem { Name = "Item1" }
					);
				_context.SaveChanges();
			}
		}

		#region GET TodoItems Methods
		
		
		/// <summary>
		/// GET /api/todo
		/// Returns all the items
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public ActionResult<List<TodoItem>> GetAll()
		{
			return _context.TodoItems.ToList();
		}

		/// <summary>
		/// GET /api/todo/id
		/// Returns an item that mathes id
		/// Name = "GetTodo" creates a named route. Named routes:
		/// Enable the app to create an HTTP link using the route name.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}", Name = "GetTodo")]
		public ActionResult<TodoItem> GetById(long id)
		{
			var item = _context.TodoItems.Find(id);

			if (item == null)
			{
				return NotFound();
			}
			return item;
		}

		#endregion

		#region POST Create Todo items
		[HttpPost]
		public IActionResult Create(TodoItem item)
		{
			_context.TodoItems.Add(item);
			_context.SaveChanges();

			// ???
			return CreatedAtRoute("GetTodo",
				new {id = item.Id},
				item
				);
		}

		#endregion

		#region UPDATE Update TodoItems
		[HttpPut("{id}")]
		public IActionResult Update(long id, TodoItem item)
		{
			var todoItem = _context.TodoItems.Find(id);
			if (todoItem == null)
			{
				return NotFound();
			}

			//update the item 
			todoItem.Name = item.Name;
			todoItem.Iscomplete = item.Iscomplete;

			//save the item back to the DB
			_context.TodoItems.Update(todoItem);
			_context.SaveChanges();

			// ??
			return NoContent();
		}
		#endregion

		#region DELETE Delate an item 
		[HttpDelete("{id}")]
		public IActionResult Delete(long id)
		{
			var todoItem = _context.TodoItems.Find(id);
			if (todoItem == null)
			{
				return NotFound();
			}

			_context.TodoItems.Remove(todoItem);
			_context.SaveChanges();

			return NoContent();
		}
		#endregion


		/**********
		GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
		***********/
	}
}
