﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding
{
    class Program
    {
        static void Main(string[] args)
        {
            #region STRINGS
            //StringManipulation.LongestSubstringWithNoRepeatingCharRun();
            #endregion

            #region ARRAYS

            //Arrays.TwoSumRun(Arrays.TwoSumHF);

            #endregion

            #region THREADS
            var server = new Server();
            server.DoWork();
            #endregion

            Console.ReadLine();
        }
    }
}
