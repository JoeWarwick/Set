#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetImpl
{
    public class LinearSet
    {
        private long _maxSize;
        private int[] _linearValues = Array.Empty<int>();
        public LinearSet(int[] values = null, long maxSize = long.MaxValue) 
        { 
            values ??= Array.Empty<int>();
            _maxSize = maxSize;
            foreach (int value in values)
            {
                Insert(value);
            }
        }

        public int[] Values { get { return _linearValues; } }

        public int Size => _linearValues.Length;

        public bool Contains(int value) => _linearValues.Contains(value);

        public void Empty()
        {
            _linearValues = Array.Empty<int>();
        }

        public bool TryInsert(int value)
        {
            if(_linearValues.Length == _maxSize)
            {
                return false;
            }
            if (_linearValues.Contains(value))
            {
                return false;
            }
            if (_linearValues.Length == 0)
            {
                Insert(value);
                return true;
            }
            Insert(value);
            return true;
        }

        private void Insert(int value)
        {
            int index = BinarySearch.BinarySearchMinBelow(_linearValues, value);

            // create a new array of size n+1
            int[] newarr = new int[_linearValues.Length + 1];

            // insert the elements from the 
            // old array into the new array up to index
            // then insert value at index
            // then insert rest of the elements
            for (int i = 0; i < _linearValues.Length + 1; i++)
            {
                if (index < 0)
                    newarr[0] = value;
                else if (index == 0 && i >= 0)
                    newarr[0] = _linearValues[i - 1];
                else if (i < index - 1)
                    newarr[i] = _linearValues[i];
                else if (i == index - 1)
                    newarr[i] = value;
                else
                    newarr[i] = _linearValues[i - 1];
            }
            _linearValues = newarr;
        }

        public void Remove(int value)
        {
            if (!_linearValues.Contains(value)) return;
    
            int[] newArr = new int[_linearValues.Length - 1];

            for (int i = 0, j = 0; i < _linearValues.Length; i++)
            {
                if (_linearValues[i] != value)
                {
                    newArr[j] = _linearValues[i];
                    j++;
                }
            }

            _linearValues = newArr;
        }
    }
}
