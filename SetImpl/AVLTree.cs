using System.Collections;
using System.Text;

namespace SetImpl
{
    public enum Traversal
    {
        InOrder,
        ReverseOrder,
        PreOrder,
        PostOrder
    }

    public class AVLTree<T> : IAVLTree<T>, IEnumerable<T> where T : IComparable<T>
    {
        public AVLTreeNode<T>? root;
        private readonly object lockObject = new();
        public Traversal traversal { get; private set; }

        public AVLTree(T[] items, Traversal traversal = Traversal.InOrder)
        {
            this.traversal = traversal;
            foreach (var item in items)
                Insert(item);
        }

        public AVLTree(Traversal traversal = Traversal.InOrder) => this.traversal = traversal;

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

            var lv = node.Left != null ? node.Left.Value : default;
            var rv = node.Right != null ? node.Right.Value : default;
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

        public bool Delete(T value)
        {
            bool removed;
            root = AVLTree<T>.Delete(root, value, out removed);
            return removed;
        }

        private static AVLTreeNode<T>? Delete(AVLTreeNode<T>? node, T value, out bool removed)
        {
            removed = false;
            if (node == null)
                return node;

            int compareResult = value.CompareTo(node.Value);
            if (compareResult < 0)
                node.Left = Delete(node.Left, value, out removed);
            else if (compareResult > 0)
                node.Right = Delete(node.Right, value, out removed);
            else
            {   // Node to delete found
                removed = true;
                // Case 1: Leaf node
                if (node.Left == null && node.Right == null)
                    return null;
                // Case 2: Node with only one child
                if (node.Left == null)
                    return node.Right;
                if (node.Right == null)
                    return node.Left;
                // Case 3: Node with two children
                var successor = FindMin(node.Right);
                node.Value = successor.Value;
                node.Right = Delete(node.Right, successor.Value, out _);
            }
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

        private static AVLTreeNode<T> FindMin(AVLTreeNode<T> node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        public int Size() => Size(root);

        private static int Size(AVLTreeNode<T>? node)
        {
            if (node == null) return 0;
            return 1 + Size(node.Left) + Size(node.Right);
        }

        private static AVLTreeNode<T> MinValueNode(AVLTreeNode<T> node)
        {
            AVLTreeNode<T> current = node;
            while (current.Left != null)
                current = current.Left;

            return current;
        }

        private static int Height(AVLTreeNode<T>? node) => node == null ? 0 : node.Height;

        private static int GetBalance(AVLTreeNode<T>? node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

        private static AVLTreeNode<T>? RotateRight(AVLTreeNode<T>? y)
        {
            AVLTreeNode<T>? x = y?.Left;
            AVLTreeNode<T>? T2 = x?.Right;

            // Perform rotation
            if (x != null) x.Right = y;
            if (y != null) y.Left = T2;

            if (y != null) y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            if (x != null) x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            // Return new root
            return x;
        }

        private static AVLTreeNode<T>? RotateLeft(AVLTreeNode<T>? x)
        {
            AVLTreeNode<T>? y = x?.Right;
            AVLTreeNode<T>? T2 = y?.Left;

            if (y != null) y.Left = x;
            if (x != null) x.Right = T2;

            if (x != null) x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            if (y != null) y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

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
        public IEnumerator<T> GetEnumerator() => Traverse().GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Perform traversal of the AVL tree
        public IEnumerable<T> Traverse() => Traverse(root);

        private IEnumerable<T> Traverse(AVLTreeNode<T>? node)
        {
            if (node != null)
            {
                if (traversal == Traversal.PreOrder) yield return node.Value;
                foreach (var n in Traverse(traversal == Traversal.ReverseOrder ? node.Right : node.Left))
                    yield return n;
                if(traversal == Traversal.InOrder || traversal == Traversal.ReverseOrder) yield return node.Value;
                foreach (var n in Traverse(traversal == Traversal.ReverseOrder ? node.Left : node.Right))
                    yield return n;
                if(traversal == Traversal.PostOrder) yield return node.Value;
            }
        }
    }
}