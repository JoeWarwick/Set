namespace SetTests
{
    using SetImpl;

    [TestClass]
    public class TestAVLTree
    {
        [TestMethod]
        public void TestEmptyTree() 
        { 
            AVLTree<int> tree = new ();
            Assert.IsNull(tree.root);
        }

        [TestMethod]
        public void TestSingleNodeTree()
        {
            AVLTree<int> tree = new ();
            tree.Insert(1);
            var result = tree.ToString();
            Assert.AreEqual(6, result.Length);
            Assert.AreEqual('R', result[0]);
            Assert.IsNotNull(tree.root);
            Assert.AreEqual(1, tree.root.Value);
        }

        [TestMethod]
        public void TestAddLeft()
        {
            AVLTree<int> tree = new ();
            tree.Insert(2);
            var res = tree.Insert(1);
            Assert.AreEqual(2, res?.Value);
            Assert.IsNotNull(tree?.root?.Left);
            Assert.AreEqual(1, tree.root.Left.Value);
        }

        [TestMethod]
        public void TestAddRight()
        {
            AVLTree<int> tree = new ();
            tree.Insert(2);
            var res = tree.Insert(3);
            Assert.AreEqual(2, res?.Value);
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

        [TestMethod] public void TestDelete()
        {
            int[] test = { 1, 5, 6, 7, 3 };
            AVLTree<int> tree = new(test);
            tree.Delete(5);
            string res1 = tree.ToString();
            Assert.AreEqual(res1, "R----6\r\n   L----1\r\n   |  R----3\r\n   R----7\r\n");
        }

        [TestMethod]
        public void TestFindMissing()
        {
            int[] test = { 5, 11, 16, 15, 4, 6, 13, 10, 16 };
            AVLTree<int> tree = new(test);
            AVLTreeNode<int>? res = tree.Find(7);
            Assert.IsNull(res);
        }

        [TestMethod]
        public void TestFind()
        {
            int[] test = { 5, 11, 16, 15, 4, 6, 13, 10, 16 };
            AVLTree<int> tree = new(test);
            AVLTreeNode<int>? res = tree.Find(10);
            Assert.IsNotNull(res);
            Assert.AreEqual(10, res.Value);
        }

        [TestMethod]
        public void TestTreeBalance1()
        {
            int[] test = { 5, 11, 16, 15, 4, 6, 13, 10, 16 };
            AVLTree<int> tree = new(test);
            string res1 = tree.ToString();
            Assert.AreEqual("R----11\r\n   L----6\r\n   |  L----5\r\n   |  |  L----4\r\n   |  R----10\r\n   R----15\r\n      L----13\r\n      R----16\r\n", res1);
            tree.Insert(14);
            string res2 = tree.ToString();
            Assert.AreEqual("R----11\r\n   L----6\r\n   |  L----5\r\n   |  |  L----4\r\n   |  R----10\r\n   R----15\r\n      L----13\r\n      |  R----14\r\n      R----16\r\n", res2);
            tree.Insert(3); // heavy to the right??
            string res3 = tree.ToString();
            Assert.AreEqual("R----4\r\n   L----3\r\n   R----11\r\n      L----6\r\n      |  L----5\r\n      |  R----10\r\n      R----15\r\n         L----13\r\n         |  R----14\r\n         R----16\r\n", res3);
        }

        [TestMethod]
        public void TestTreeBalance2()
        {
            int[] test = { 5, 30, 32, 35, 40 };
            AVLTree<int> tree = new(test);
            string res1 = tree.ToString();
            Assert.AreEqual("R----35\r\n   L----30\r\n   |  L----5\r\n   |  R----32\r\n   R----40\r\n", res1);
            tree.Insert(31);
            string res2 = tree.ToString();
            Assert.AreEqual("R----32\r\n   L----30\r\n   |  L----5\r\n   |  R----31\r\n   R----35\r\n      R----40\r\n", res2);
        }
    }
}