using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coding
{
    class list
    {
        /// <summary>
        /// The list has the least significant digit at the start.
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static List<int> AddTwoListIntegers(
            List<int> list1,
            List<int> list2)
        {
            List<int> resultList = new List<int>();

            int num1 = 0, num2 = 0;
    
            var l1Enum = list1.GetEnumerator();
            var l2Enum = list2.GetEnumerator();

            while (l1Enum.MoveNext() && l2Enum.MoveNext())
            {
                num1 = l1Enum.Current;
                num2 = l2Enum.Current;
            }

            return resultList;
        }
    }
}
