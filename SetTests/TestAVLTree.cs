namespace SetTests
{
    using System;
    using SetImpl;

    [TestClass]
    public class TestAVLTree
    {
        [TestMethod]
        public void TestEmptyTree() 
        {
            AVLTree<int> tree = new();
            Assert.IsNull(tree.root);
        }

        [TestMethod]
        public void TestSingleNodeTree()
        {
            AVLTree<int> tree = new();
            tree.Insert(1);
            var result = tree.ToString();
            Assert.AreEqual(8, result.Length);
            Assert.AreEqual('R', result[0]);
            Assert.IsNotNull(tree.root);
            Assert.AreEqual(1, tree.root.Value);
        }

        [TestMethod]
        public void TestAddLeft()
        {
            AVLTree<int> tree = new();
            tree.Insert(2);
            tree.Insert(1);
            Assert.AreEqual(2, tree?.root?.Value);
            Assert.IsNotNull(tree?.root?.Left);
            Assert.AreEqual(1, tree.root.Left.Value);
        }

        [TestMethod]
        public void TestAddRight()
        {
            AVLTree<int> tree = new();
            tree.Insert(2);
            tree.Insert(3);
            Assert.AreEqual(2, tree?.root?.Value);
            Assert.IsNotNull(tree?.root?.Right);
            Assert.AreEqual(3, tree.root.Right.Value);
        }

        [TestMethod]
        public void TestAddMultiple()
        {
            var tree = new AVLTree<int>(new int[] {1,3,2});
            var root = tree.root;
            Assert.IsNotNull(root);
            Assert.AreEqual(2, root.Value);
            Assert.IsNotNull(root.Right);
            Assert.AreEqual(3, root.Right.Value);
            Assert.IsNotNull(root.Left);
            Assert.AreEqual(1, root.Left.Value);
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
            int res = tree.Find(7);
            Assert.AreEqual(default, res);
        }

        [TestMethod]
        public void TestFind()
        {
            int[] test = { 5, 11, 16, 15, 4, 6, 13, 10, 17 };
            AVLTree<int> tree = new(test);
            int? res = tree.Find(10);
            Assert.AreNotEqual(default, res);
            Assert.AreEqual(10, res);
        }
        
        // Following rotation tests from https://www.geeksforgeeks.org/insertion-in-an-avl-tree/
        [TestMethod]
        public void TestRightRotate()
        {
            int[] test = { 13, 15, 10, 16, 5, 11, 4, 6 };
            AVLTree<int> tree = new(test);
            string res1 = tree.ToString();
            Assert.AreEqual("R----13\r\n   L----10\r\n   |  L----5\r\n   |  |  L----4\r\n   |  |  R----6\r\n   |  R----11\r\n   R----15\r\n      R----16\r\n", res1);
            tree.Insert(7);
            string res2 = tree.ToString();
            Assert.AreEqual("R----13\r\n   L----6\r\n   |  L----5\r\n   |  |  L----4\r\n   |  R----10\r\n   |     L----7\r\n   |     R----11\r\n   R----15\r\n      R----16\r\n", res2);
        }

        [TestMethod]
        public void TestLeftRotate()
        {
            int[] test = { 5, 30, 32, 35, 40 };
            AVLTree<int> tree = new(test);
            string res1 = tree.ToString();
            Assert.AreEqual("R----30\r\n   L----5\r\n   R----35\r\n      L----32\r\n      R----40\r\n", res1);
            tree.Insert(45);
            string res2 = tree.ToString();
            Assert.AreEqual("R----35\r\n   L----30\r\n   |  L----5\r\n   |  R----32\r\n   R----40\r\n      R----45\r\n", res2);
        }

        [TestMethod]
        public void TestLargeSet()
        {
            var tree = new AVLTree<Employee>();
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var count = 1_000_000;
            Parallel.For(0, count, i=>
            {             
                var emp = new Employee() { Id = i + 1, Salary = (decimal?)(rnd.Next() * 0.01), Name = GenerateName(3 + i % 10, rnd) };
                tree.Insert(emp);
            });
            var sz = tree.Size();
            Assert.AreEqual(count, sz);
        }

        public static string GenerateName(int len, Random r)
        {
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
    }

    public class Employee : IComparable<Employee>
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";

        public decimal? Salary { get; set; }

        public override string ToString()
        {
            return $"{this.Salary} || {this.Name}";
        }

        public int CompareTo(Employee? other)
        {
            var emp = other as Employee;
            if (this.Salary < emp?.Salary) return -1;
            else if (this.Salary > emp?.Salary) return 1;
            else return this.Name.CompareTo(emp?.Name);
        }
    }
}