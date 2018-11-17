using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI2.Models
{
    public class TodoItem
    {
		public long Id { get; set; }
		public string Name { get; set; }
		public bool Iscomplete { get; set; }
    }
}
