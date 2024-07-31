namespace SetImpl
{
    internal class Program
    {
        static void Main()
        {
            AVLTree<int> tree = new AVLTree<int>();

            tree.Insert(10);
            tree.Insert(20);
            tree.Insert(30);
            tree.Insert(40);
            tree.Insert(50);
            tree.Insert(25);

            Console.Write(tree.ToString());
        }
    }
}