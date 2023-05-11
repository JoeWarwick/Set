namespace SetTests
{
    using SetImpl;

    [TestClass]
    public class TestBinaryTree
    {
        [TestMethod]
        public void TestEmptyTree() 
        { 
            BinaryTree<int> tree = new BinaryTree<int>();
            Assert.AreEqual(0, tree.Size);
            Assert.IsNull(tree.Root);
        }

        [TestMethod]
        public void TestSingleNodeTree()
        {
            BinaryTree<int> tree = new BinaryTree<int>( new int[] { 1 }); 
            var result = tree.Traverse();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.IsNotNull(tree.Root);
            Assert.AreEqual(1, tree.Root.Data);
        }

        [TestMethod]
        public void TestAddLeft()
        {
            var tree = new BinaryTree<int>(new int[] { 2 });
            var res = tree.Add(1);
            Assert.AreEqual(2, res.Data);
            Assert.IsNotNull(tree.Root.LeftNode);
            Assert.AreEqual(1, tree.Root.LeftNode.Data);
        }

        [TestMethod]
        public void TestAddRight()
        {
            var tree = new BinaryTree<int>(new int[] { 2 });
            var res = tree.Add(3);
            Assert.AreEqual(2, res.Data);
            Assert.IsNotNull(tree.Root.RightNode);
            Assert.AreEqual(3, tree.Root.RightNode.Data);
        }

        [TestMethod]
        public void TestAddMultiple()
        {
            var tree = new BinaryTree<int>(new int[] {1,3,2});
            var root = tree.Root;
            Assert.AreEqual(tree.Size, 3);
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.RightNode);
            Assert.IsNotNull(root.RightNode.LeftNode);
        }

        [TestMethod]
        public void TestTraverseOrder()
        {
            int[] test = { 1, 3, 2 };
            BinaryTree<int> tree = new BinaryTree<int>(test);
            int[] exp = tree.Traverse();

            Assert.AreEqual(exp.Length, 3);
            Assert.AreEqual(exp[0], 1);
            Assert.AreEqual(exp[1], 2);
        }
    }
}