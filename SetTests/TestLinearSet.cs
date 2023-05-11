namespace SetTests
{
    using SetImpl;

    [TestClass]
    public class TestLinearSet
    {
        [TestMethod]
        public void TestLinearSetOrder()
        {
            int[] test = { 1, 3, 2 };
            LinearSet set = new LinearSet(test);
            int[] exp = set.Values;

            Assert.AreEqual(3, exp.Length);
            Assert.AreEqual(1, exp[0]);
            Assert.AreEqual(2, exp[1]);
        }
    }
}