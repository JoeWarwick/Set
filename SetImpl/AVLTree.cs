using System.Text;

namespace SetImpl
{
    public class AVLTree<T> : IAVLTree<T> where T : IComparable
    {
        public AVLTreeNode<T>? root;

        public AVLTree(T[] items)
        {
            foreach (var item in items)
                root = Insert(item);

        }

        public AVLTree()
        { }

        public AVLTreeNode<T>? Insert(T value)
        {
            root = AVLTree<T>.Insert(root, value);
            return root;
        }

        private static AVLTreeNode<T>? Insert(AVLTreeNode<T>? node, T value)
        {
            if (node == null)
                return new AVLTreeNode<T>(value);

            int compareResult = value.CompareTo(node.Value);
            if (compareResult < 0)
                node.Left = AVLTree<T>.Insert(node.Left, value);
            else if (compareResult > 0)
                node.Right = AVLTree<T>.Insert(node.Right, value);
            else
                return node;

            node.Height = 1 + Math.Max(AVLTree<T>.Height(node.Left), AVLTree<T>.Height(node.Right));

            int balance = AVLTree<T>.GetBalance(node);

            // Left heavy situation
            if (balance > 1 && node.Left != null && value.CompareTo(node.Left.Value) < 0)
                return AVLTree<T>.RotateRight(node);

            // Right heavy situation
            if (balance < -1 && node.Right != null && value.CompareTo(node.Right.Value) > 0)
                return AVLTree<T>.RotateLeft(node);

            // Left-Right situation
            if (balance > 1 && node.Left != null && value.CompareTo(node.Left.Value) > 0)
            {
                node.Left = AVLTree<T>.RotateLeft(node.Left);
                return AVLTree<T>.RotateRight(node);
            }

            // Right-Left situation
            if (balance < -1 && node.Right != null && value.CompareTo(node.Right.Value) < 0)
            {
                node.Right = AVLTree<T>.RotateRight(node.Right);
                return AVLTree<T>.RotateLeft(node);
            }

            return node;
        }

        public AVLTreeNode<T>? Find(T value)
        {
            return AVLTree<T>.Find(root, value);
        }

        private static AVLTreeNode<T>? Find(AVLTreeNode<T>? node, T Value)
        {
            if (node == null)
                return node;
            int compareResult = Value.CompareTo(node.Value);
            if (compareResult < 0)
                return AVLTree<T>.Find(node.Left, Value);
            else if (compareResult > 0)
                return AVLTree<T>.Find(node.Right, Value);
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
                node.Left = AVLTree<T>.Delete(node.Left, value);
            else if (compareResult > 0)
                node.Right = AVLTree<T>.Delete(node.Right, value);
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
                    AVLTreeNode<T> temp = AVLTree<T>.MinValueNode(node.Right);

                    node.Value = temp.Value;

                    node.Right = AVLTree<T>.Delete(node.Right, temp.Value);
                }
            }

            if (node == null)
                return node;

            node.Height = Math.Max(AVLTree<T>.Height(node.Left), AVLTree<T>.Height(node.Right)) + 1;

            int balance = AVLTree<T>.GetBalance(node);

            // Left heavy situation
            if (balance > 1 && AVLTree<T>.GetBalance(node.Left) >= 0)
                return AVLTree<T>.RotateRight(node);

            // Left-Right situation
            if (balance > 1 && AVLTree<T>.GetBalance(node.Left) < 0)
            {
                node.Left = AVLTree<T>.RotateLeft(node.Left);
                return AVLTree<T>.RotateRight(node);
            }

            // Right heavy situation
            if (balance < -1 && AVLTree<T>.GetBalance(node.Right) <= 0)
                return AVLTree<T>.RotateLeft(node);

            // Right-Left situation
            if (balance < -1 && AVLTree<T>.GetBalance(node.Right) > 0)
            {
                node.Right = AVLTree<T>.RotateRight(node.Right);
                return AVLTree<T>.RotateLeft(node);
            }

            return node;
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
            return node == null ? 0 : AVLTree<T>.Height(node.Left) - AVLTree<T>.Height(node.Right);
        }

        private static AVLTreeNode<T>? RotateRight(AVLTreeNode<T>? y)
        {
            AVLTreeNode<T>? x = y?.Left;
            AVLTreeNode<T>? T2 = x?.Right;

            // Perform rotation
            if (x != null)
            {
                x.Right = y;
                x.Height = Math.Max(AVLTree<T>.Height(x.Left), AVLTree<T>.Height(x.Right)) + 1;
            }

            if (y != null)
            {
                y.Left = T2;
                y.Height = Math.Max(AVLTree<T>.Height(y.Left), AVLTree<T>.Height(y.Right)) + 1;
            }

            // Return new root
            return x;
        }

        private static AVLTreeNode<T>? RotateLeft(AVLTreeNode<T>? x)
        {
            AVLTreeNode<T>? y = x?.Right;
            AVLTreeNode<T>? T2 = y?.Left;

            // Perform rotation
            if (y != null)
            {
                y.Left = x;
                y.Height = Math.Max(AVLTree<T>.Height(y.Left), AVLTree<T>.Height(y.Right)) + 1;
            }
            if (x != null)
            {
                x.Right = T2;
                x.Height = Math.Max(AVLTree<T>.Height(x.Left), AVLTree<T>.Height(x.Right)) + 1;
            }

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

                AVLTree<T>.PrintTree(node.Left, indent, false, sb);
                AVLTree<T>.PrintTree(node.Right, indent, true, sb);
            }
        }
    }
}