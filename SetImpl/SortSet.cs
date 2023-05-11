using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetImpl
{
    public class SortSet<T>
    {
        internal BinaryTree<T> BST { get; }
        public SortSet() {
            this.BST = new BinaryTree<T>();
        }

        public void Add(T value)
        {
            BST.Add(value);
        }

        public void Remove(T value) 
        { 
            BST.Remove(value);
        }

        public bool Contains(T value)
        {
            return BST.Find(value) != null;
        }

        public int Size()
        {
            return BST.Size;
        }

        public T[] Values()
        {
            return BST.Traverse();
        }
        
    }
}
