using System;
using System.Text;

public class AVLTree<T> where T : IComparable
{
    public AVLTreeNode<T>? root;

    public AVLTree(T[] items)
    {
        foreach (var item in items)
        {
            root = Insert(item);
        }
    }

    public AVLTree()
    { }

    public AVLTreeNode<T> Insert(T value)
    {
        root = Insert(root, value);
        return root;
    }

    private AVLTreeNode<T> Insert(AVLTreeNode<T>? node, T value)
    {
        if (node == null)
            return new AVLTreeNode<T>(value);

        int compareResult = value.CompareTo(node.Value);
        if (compareResult < 0)
            node.Left = Insert(node.Left, value);
        else if (compareResult > 0)
            node.Right = Insert(node.Right, value);
        else
            return node;

        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

        int balance = GetBalance(node);

        // Left heavy situation
        if (balance > 1 && node.Left != null && value.CompareTo(node.Left.Value) < 0)
            return RotateRight(node);

        // Right heavy situation
        if (balance < -1 && node.Right != null && value.CompareTo(node.Right.Value) > 0)
            return RotateLeft(node);

        // Left-Right situation
        if (balance > 1 && node.Left != null && value.CompareTo(node.Left.Value) > 0)
        {
            node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }

        // Right-Left situation
        if (balance < -1 && node.Right != null && value.CompareTo(node.Right.Value) < 0)
        {
            node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }

        return node;
    }

    public void Delete(T value)
    {
        root = Delete(root, value);
    }

    private AVLTreeNode<T>? Delete(AVLTreeNode<T>? node, T value)
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
            if ((node.Left == null) || (node.Right == null))
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

    private AVLTreeNode<T> MinValueNode(AVLTreeNode<T> node)
    {
        AVLTreeNode<T> current = node;

        while (current.Left != null)
            current = current.Left;

        return current;
    }

    private int Height(AVLTreeNode<T>? node)
    {
        return node == null ? 0 : node.Height;
    }

    private int GetBalance(AVLTreeNode<T>? node)
    {
        return node == null ? 0 : Height(node.Left) - Height(node.Right);
    }

    private AVLTreeNode<T> RotateRight(AVLTreeNode<T> y)
    {
        AVLTreeNode<T>? x = y.Left;
        AVLTreeNode<T>? T2 = x.Right;

        // Perform rotation
        x.Right = y;
        y.Left = T2;

        // Update heights
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        // Return new root
        return x;
    }

    private AVLTreeNode<T> RotateLeft(AVLTreeNode<T> x)
    {
        AVLTreeNode<T>? y = x.Right;
        AVLTreeNode<T>? T2 = y.Left;

        // Perform rotation
        y.Left = x;
        x.Right = T2;

        // Update heights
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

        // Return new root
        return y;
    }

    public string PrintTree()
    {
        if (root == null) return "EMPTY";
        StringBuilder sb = new StringBuilder();
        PrintTree(root, "", true, sb);
        return sb.ToString();
    }

    private void PrintTree(AVLTreeNode<T>? node, string indent, bool last, StringBuilder sb)
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
            sb.Append(node.Value);

            PrintTree(node.Left, indent, false, sb);
            PrintTree(node.Right, indent, true, sb);
        }
    }
}