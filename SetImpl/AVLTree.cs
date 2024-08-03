using System.Collections;
using System.Drawing;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace SetImpl
{
    public class AVLTree<T> : IAVLTree<T>, IQueryable<T> where T : IComparable<T>
    {
        public AVLTreeNode<T>? root;
        private readonly object lockObject = new();
        private IQueryProvider provider;
        // Implement the IQueryable interface
        public Type ElementType => typeof(T);

        public Expression Expression => new AVLTreeExpression<T>(this);

        public IQueryProvider Provider => provider;

        public AVLTree(T[] items)
        {
            provider = new AVLTreeQueryProvider<T>(this);
            foreach (var item in items)
                Insert(item);
        }

        public AVLTree()
        {
            provider = new AVLTreeQueryProvider<T>(this);
        }

        public void Insert(T value)
        {
            lock (lockObject)
            {
                root = Insert(root, value);
            }
        }

        private static AVLTreeNode<T>? Insert(AVLTreeNode<T>? node, T value)
        {
            if (node == null)
                return new AVLTreeNode<T>(value);

            int compareResult = value.CompareTo(node.Value);
            if (compareResult < 0)
                node.Left = Insert(node.Left, value);
            else if (compareResult > 0)
                node.Right = Insert(node.Right, value);
            else
                return node; // same value!

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

            int balance = GetBalance(node);

            var lv = node.Left != null ? node.Left.Value : default(T);
            var rv = node.Right != null ? node.Right.Value : default(T);
            // Left heavy situation
            if (balance > 1 && value.CompareTo(lv) < 0)
                return RotateRight(node);

            // Right heavy situation
            if (balance < -1 && value.CompareTo(rv) > 0)
                return RotateLeft(node);

            // Left-Right situation
            if (balance > 1 && value.CompareTo(lv) > 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right-Left situation
            if (balance < -1 && value.CompareTo(rv) < 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        public T? Find(T value)
        {
            var result = AVLTree<T>.Find(root, value);
            return result == null ? default : result.Value;  
        }

        private static AVLTreeNode<T>? Find(AVLTreeNode<T>? node, T Value)
        {
            if (node == null)
                return node;
            int compareResult = Value.CompareTo(node.Value);
            if (compareResult < 0)
                return Find(node.Left, Value);
            else if (compareResult > 0)
                return Find(node.Right, Value);
            else return node;
        }

        public void Delete(T value)
        {
            root = AVLTree<T>.Delete(root, value);
        }

        private static AVLTreeNode<T>? Delete(AVLTreeNode<T>? node, T value)
        {
            if (node == null)
                return node;

            int compareResult = value.CompareTo(node.Value);
            if (compareResult < 0)
                node.Left = Delete(node.Left, value);
            else if (compareResult > 0)
                node.Right = Delete(node.Right, value);
            else
            {
                // Node with only one child or no child
                if (node.Left == null || node.Right == null)
                {
                    AVLTreeNode<T>? temp;
                    if (node.Left == null)
                        temp = node.Right;
                    else
                        temp = node.Left;

                    if (temp == null)
                    {
                        node = null;
                    }
                    else
                        node = temp; // Copy the contents of the non-empty child
                }
                else
                {
                    // Node with two children
                    AVLTreeNode<T> temp = MinValueNode(node.Right);

                    node.Value = temp.Value;

                    node.Right = Delete(node.Right, temp.Value);
                }
            }

            if (node == null)
                return node;

            node.Height = Math.Max(Height(node.Left), Height(node.Right)) + 1;

            int balance = GetBalance(node);

            // Left heavy situation
            if (balance > 1 && GetBalance(node.Left) >= 0)
                return RotateRight(node);

            // Left-Right situation
            if (balance > 1 && GetBalance(node.Left) < 0)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }

            // Right heavy situation
            if (balance < -1 && GetBalance(node.Right) <= 0)
                return RotateLeft(node);

            // Right-Left situation
            if (balance < -1 && GetBalance(node.Right) > 0)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }

            return node;
        }

        public int Size()
        {
            return Size(root);
        }

        private static int Size(AVLTreeNode<T>? node)
        {
            if(node == null) return 0;
            return 1 + Size(node.Left) + Size(node.Right);
        }

        private static AVLTreeNode<T> MinValueNode(AVLTreeNode<T> node)
        {
            AVLTreeNode<T> current = node;

            while (current.Left != null)
                current = current.Left;

            return current;
        }

        private static int Height(AVLTreeNode<T>? node)
        {
            return node == null ? 0 : node.Height;
        }

        private static int GetBalance(AVLTreeNode<T>? node)
        {
            return node == null ? 0 : Height(node.Left) - Height(node.Right);
        }

        private static AVLTreeNode<T>? RotateRight(AVLTreeNode<T>? y)
        {
            AVLTreeNode<T>? x = y?.Left;
            AVLTreeNode<T>? T2 = x?.Right;

            // Perform rotation
            if(x!=null) x.Right = y;
            if(y!=null) y.Left = T2;

            if(y!=null) y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            if(x!=null) x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            // Return new root
            return x;
        }

        private static AVLTreeNode<T>? RotateLeft(AVLTreeNode<T>? x)
        {
            AVLTreeNode<T>? y = x?.Right;
            AVLTreeNode<T>? T2 = y?.Left;

            // Perform rotation
            if(y!=null) y.Left = x;
            if(x!=null) x.Right = T2;

            if(x!=null) x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            if(y!=null) y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            // Return new root
            return y;
        }

        public override string ToString()
        {
            if (root == null) return "EMPTY";
            StringBuilder sb = new();
            AVLTree<T>.PrintTree(root, "", true, sb);
            return sb.ToString();
        }

        private static void PrintTree(AVLTreeNode<T>? node, string indent, bool last, StringBuilder sb)
        {
            if (node != null)
            {
                sb.Append(indent);
                if (last)
                {
                    sb.Append("R----");
                    indent += "   ";
                }
                else
                {
                    sb.Append("L----");
                    indent += "|  ";
                }
                sb.AppendLine(node.Value.ToString());

                PrintTree(node.Left, indent, false, sb);
                PrintTree(node.Right, indent, true, sb);
            }
        }

        public T? Get(int index) => AVLTree<T>.Get(root, index);

        private static T? Get(AVLTreeNode<T>? node, int index)
        {
            if (node == null) return default(T); // This should not happen if the index is valid

            int leftSize = Size(node.Left);

            if (index < leftSize)
            {
                return Get(node.Left, index); // Go left
            }
            else if (index > leftSize)
            {
                return Get(node.Right, index - leftSize - 1); // Go right, adjusting index
            }
            else
            {
                return node.Value; // Current node is the one we want
            }
        }

        public int IndexOf(T value) => AVLTree<T>.IndexOf(root, value, 0);

        private static int IndexOf(AVLTreeNode<T>? node, T value, int index)
        {
            if (node == null) return -1; // Value not found

            // Count left subtree values
            int leftCount = Size(node.Left);

            if (value.CompareTo(node.Value) < 0)
            {
                return IndexOf(node.Left, value, index);
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                return IndexOf(node.Right, value, index + leftCount + 1);
            }
            else
            {
                // Value found, return the index
                return index + leftCount;
            }
        }

        private IEnumerable<T> InOrderTraversal(AVLTreeNode<T>? node)
        {
            if (node != null)
            {
                foreach (var item in InOrderTraversal(node.Left))
                {
                    yield return item;
                }
                yield return node.Value;
                foreach (var item in InOrderTraversal(node.Right))
                {
                    yield return item;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Perform an in-order traversal to yield elements in ascending order
            return InOrderTraversal(root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}