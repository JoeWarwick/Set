namespace SetImpl
{
    public class AVLTreeNode<T>(T value) where T : IComparable<T>
    {
        public T Value { get; set; } = value;
        public AVLTreeNode<T>? Left { get; set; }
        public AVLTreeNode<T>? Right { get; set; }
        public int Height { get; set; } = 1;
    }
}