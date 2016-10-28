using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Coding
{
    public static class StringManipulation
    {

        public static void LongestSubstringWithNoRepeatingCharRun()
        {
            //string[] names = new string[] { "Matt", "Joanne", "Robert" };

            var testStrings = new string[]
                {"foo",
                "google",
                "Microsoft",
                "abcqacqbzqlmn",
                };
            foreach (var str in testStrings)
            {
                LongestSubstringWithNoRepeatingChar(str);
            }

        }
        /// <summary>
        /// Returns the longest substring that has no repeating chars
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        public static string LongestSubstringWithNoRepeatingChar( string inStr)
        {
            //string resultString = null;
            var chMap = new Dictionary<char, int>();

            int strLen = inStr.Length;

            Debug.Assert( !String.IsNullOrEmpty(inStr) );

            if (strLen == 1)       {              return inStr;            }

            int st = 0, cst = 0;
            int end = 0, cend = 0;

            chMap.Add(inStr[0], 0);

            for (int idx = 1;  idx < strLen; idx++)
            {
                var ch = inStr[idx];
                if (!chMap.ContainsKey(ch))
                {
                    chMap.Add(ch, idx);
                    cend = idx;
                }
                else
                {
                    int dupIndex = chMap[ch];
                    UpdateSubstringIndices(idx, dupIndex, ref cst, ref cend, ref st, ref end);

                    // remove charecter from the char map that don't belong to the current substring
                    for (int i = st; i < dupIndex; i++)
                    {
                        chMap.Remove(inStr[i]);
                    }
                }
            }
            
            if ((cend - cst) > (end - st))
            {
                st = cst;
                end = cend;
            }

            var result = inStr.Substring(st, (end - st) + 1);
            Console.WriteLine("Input string : {0} \n\tLongest Substring without repeats : {1}",
                inStr, result
                );
            return result;
        }

        private static void  UpdateSubstringIndices(
            int cIdx,
            int dupIndex,
            ref int cSt,
            ref int cEnd,
            ref int st,
            ref int end
            )
        {
            // update st and end. These now hold the longest substring we have seen thus far
            int prevSSLen = end - st + 1;
            int curSSLen =  cIdx - cIdx;
            if (curSSLen >  prevSSLen)
            {
                st = cSt;
                end = cIdx - 1;
            }

            // update the current subString Index. This will start right after the duplicate location index and 
            // go to the current character
            cSt = dupIndex + 1;
            cEnd = cIdx;
         }
    }
}