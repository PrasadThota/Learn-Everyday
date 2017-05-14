using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaManager
{
    class Program
    {
        static void Main(string[] args)
        {

            #region THREADS
            var server = new Server();
            server.DoWork();
            #endregion

        }
    }
}
