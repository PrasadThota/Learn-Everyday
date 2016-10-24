using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Coding
{
    public static class Arrays
    {
        public delegate int[] TSDelegate(int[] sArr, int target);

        /// <summary>
        /// Tests for TwoSum
        /// </summary>
        public static void TwoSumRun(TSDelegate TSFunc)
        {
            var array1 = new int[] { 1, 4, 6, 9, 10 };
            int[] sumArray = null;

            var array2 = new int[] { 3, 4 };
            sumArray = TSFunc(array2, 7);

            var array3 = new int[] { 5, 9, -15, -10, 25, 64, 100, 200 };
            sumArray = TSFunc(array3, 15);
            sumArray = TSFunc(array3, 300);
            sumArray = TSFunc(array3, 14);

            //var array = new int[] { };
            //sumArray = TwoSum();

        }


        /// <summary>
        /// Returns the indices of the two elements in the array that add up to a given sum
        /// </summary>
        /// <param name="sourceArr"></param>
        /// <param name="targetSum"></param>
        /// <returns></returns>
        public static int[] TwoSumBF(
            int[] sourceArr,
            int targetSum
            )
        {
            int[] sumIndices = new int[2];
            string result = null;

            Debug.Assert(sourceArr.Length >= 2);

            var startIdx = 0;
            for (int oIdx = 0; oIdx < sourceArr.Length - 1; oIdx++)
            {
                startIdx = oIdx;
                for (int idx = startIdx + 1; idx < sourceArr.Length; idx++)
                {
                    var sum = sourceArr[startIdx] + sourceArr[idx];
                    if (sum == targetSum)
                    {
                        sumIndices[0] = sourceArr[startIdx]; //soustartIdx;
                        sumIndices[1] = sourceArr[idx];   // idx;

                        result = String.Format(
                            "\n Target Sum : {0} \n" +
                            "\t First : {1} @ {2} \n\t Second : {3} @ {4} \n",
                           targetSum,
                           sumIndices[0], startIdx,
                           sumIndices[1], idx
                           );
                        break;
                    }
                }
            }

            Console.WriteLine(result);
            return sumIndices;
        }

        /// <summary>
        /// Solves the TwoSum problem using a hashtable.
        /// </summary>
        /// <param name="sArr"></param>
        /// <param name="targetSum"></param>
        /// <returns></returns>
        public static int[] TwoSumHF(
            int[] sArr,
            int targetSum
            )
        {
            var tsArr = new int[2];
            string result = null;
            var nMap = new Dictionary<int, int>();

            Debug.Assert(sArr.Length >= 2);

            for (int idx = 0; idx < sArr.Length; idx++)
            {
                int complement = targetSum - sArr[idx];

                if (nMap.ContainsKey(complement))
                {
                    tsArr[0] = nMap[complement];
                    tsArr[1] = idx;

                    result = String.Format(
                            "\n Target Sum : {0} \n" +
                            "\t First : {1} @ {2} \n\t Second : {3} @ {4} \n",
                           targetSum,
                           complement,  tsArr[0], complement, 
                           sArr[idx], tsArr[1] 
                           );
                    break;
                }
                else
                {
                    nMap.Add(sArr[idx], idx);
                }
            }

            Console.WriteLine(result);
            return tsArr;

        }
    }
}