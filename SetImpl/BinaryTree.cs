#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SetImpl
{
    public class BinaryTree<T>
    {
        public SetNode<T> Root { get; private set; }

        public int Size { get; private set; }

        private readonly int _maxSize;
                        
        private readonly Comparer<T> _comparer;

        public BinaryTree(T[] values = null, Comparer<T> comparer = null, int maxSize = -1)
        {
            _maxSize = maxSize;
            _comparer = comparer ?? Comparer<T>.Default;
            if (values != null && values.Count() > 0)
            {
                foreach(T value in values)
                {
                    Add(value);
                }
            }
        }

        public SetNode<T> Add(T value)
        {
            var added = AddImpl(this.Root, value);
            this.Root ??= added;
            return added;
        }

        private SetNode<T> AddImpl(SetNode<T> node, T value)
        {
            if(node == null)
            {
                Size++;
                return new SetNode<T> { Data = value };
            }
            
            if (_comparer.Compare(node.Data, value) > 0)
            {
                node.LeftNode = AddImpl(node.LeftNode, value);
            }
            else
            {
                node.RightNode = AddImpl(node.RightNode, value);
            }
            return node;
        }

        public SetNode<T> Find(T value)
        {
            return this.Find(value, this.Root);
        }

        private SetNode<T> Find(T value, SetNode<T> parent)
        {
            if (parent != null)
            {
                if (_comparer.Compare(value, parent.Data) == 0) return parent;
                if (_comparer.Compare(value, parent.Data) < 0)
                    return Find(value, parent.LeftNode);
                else
                    return Find(value, parent.RightNode);
            }

            return null;
        }

        public void Remove(T value)
        {
            this.Root = Remove(this.Root, value);
            Size -= 1;
        }

        private SetNode<T> Remove(SetNode<T> parent, T key)
        {
            if (parent == null) return parent;

            if (_comparer.Compare(key, parent.Data) < 0) parent.LeftNode = Remove(parent.LeftNode, key);
            else if (_comparer.Compare(key, parent.Data) > 0)
                parent.RightNode = Remove(parent.RightNode, key);

            // if value is same as parent's value, then this is the node to be deleted  
            else
            {
                // node with only one child or no child  
                if (parent.LeftNode == null)
                    return parent.RightNode;
                else if (parent.RightNode == null)
                    return parent.LeftNode;

                // node with two children: Get the inorder successor (smallest in the right subtree)  
                parent.Data = MinValue(parent.RightNode);

                // Delete the inorder successor  
                parent.RightNode = Remove(parent.RightNode, parent.Data);
            }

            return parent;
        }

        private static T MinValue(SetNode<T> node)
        {
            T minv = node.Data;

            while (node.LeftNode != null)
            {
                minv = node.LeftNode.Data;
                node = node.LeftNode;
            }

            return minv;
        }


        public int GetTreeDepth()
        {
            return this.GetTreeDepth(this.Root);
        }

        private int GetTreeDepth(SetNode<T> parent)
        {
            return parent == null ? 0 : Math.Max(GetTreeDepth(parent.LeftNode), GetTreeDepth(parent.RightNode)) + 1;
        }

        public T[] Traverse()
        {
            SetNode<T> parent = this.Root;
            return TraverseInOrder(parent).ToArray();
        }

        private IEnumerable<T> TraverseInOrder(SetNode<T> parent)
        {
            IEnumerable<T> set = new List<T>();
            if(parent.LeftNode != null)
                set = set.Concat(TraverseInOrder(parent.LeftNode));
            set = set.Append(parent.Data);
            if(parent.RightNode != null)
                set = set.Concat(TraverseInOrder(parent.RightNode));
            return set;
        }
    }
}
