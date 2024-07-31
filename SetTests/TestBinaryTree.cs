namespace SetTests
{
    using SetImpl;

    [TestClass]
    public class TestBinaryTree
    {
        [TestMethod]
        public void TestEmptyTree() 
        { 
            AVLTree<int> tree = new AVLTree<int>();
            Assert.IsNull(tree.root);
        }

        [TestMethod]
        public void TestSingleNodeTree()
        {
            AVLTree<int> tree = new AVLTree<int>();
            tree.Insert(1);
            var result = tree.PrintTree();
            Assert.AreEqual(6, result.Length);
            Assert.AreEqual('R', result[0]);
            Assert.IsNotNull(tree.root);
            Assert.AreEqual(1, tree.root.Value);
        }

        [TestMethod]
        public void TestAddLeft()
        {
            AVLTree<int> tree = new AVLTree<int>();
            tree.Insert(2);
            var res = tree.Insert(1);
            Assert.AreEqual(2, res.Value);
            Assert.IsNotNull(tree?.root?.Left);
            Assert.AreEqual(1, tree.root.Left.Value);
        }

        [TestMethod]
        public void TestAddRight()
        {
            AVLTree<int> tree = new AVLTree<int>();
            tree.Insert(2);
            var res = tree.Insert(3);
            Assert.AreEqual(2, res.Value);
            Assert.IsNotNull(tree?.root?.Right);
            Assert.AreEqual(3, tree.root.Right.Value);
        }

        [TestMethod]
        public void TestAddMultiple()
        {
            var tree = new AVLTree<int>(new int[] {1,3,2});
            var root = tree.root;
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.Right);
            Assert.IsNotNull(root.Left);
        }

        [TestMethod]
        public void TestTreeBalance()
        {
            int[] test = { 5, 11, 16, 15, 4, 6, 13, 10, 16 };
            AVLTree<int> tree = new AVLTree<int>(test);
            string res1 = tree.PrintTree();
            Assert.AreEqual(res1, "R----11   L----5   |  L----4   |  R----6   |     R----10   R----15      L----13      R----16");
            tree.Insert(14);
            string res2 = tree.PrintTree();
            Assert.AreEqual(res2, "R----11   L----5   |  L----4   |  R----6   |     R----10   R----15      L----13      |  R----14      R----16");
            tree.Insert(3);
            string res3 = tree.PrintTree();
            Assert.AreEqual(res3, "R----11   L----5   |  L----4   |  |  L----3   |  R----6   |     R----10   R----15      L----13      |  R----14      R----16");
        }
    }
}