using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetImpl
{
    public class BinarySearch
    {
        public static int BinarySearchMinBelow(int[] arr, int searchNumber)
        {
            int closestMinIdx = 0;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i] >= searchNumber) return closestMinIdx;
                if (arr[i] < searchNumber && arr[i + 1] > searchNumber)
                {
                    closestMinIdx = i;
                }
            }

            return closestMinIdx;
        }

        private static int Compare(int x, int y)
        {
            return x < y ? -1 : (y < x ? 1 : 0);
        }
    }
}
