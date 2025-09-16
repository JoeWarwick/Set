namespace SetImpl
{
    internal class Program
    {
        static void Main()
        {
            AVLTree<Volume> tree = new();

            tree.Insert(new Volume(1, "C1", 1223, 324));
            tree.Insert(new Volume(5, "C5", 1234656, 23636));
            tree.Insert(new Volume(3, "F", 34734, 23465));
            tree.Insert(new Volume(6, "B", 3245, 122));
            tree.Insert(new Volume(2, "A", 2145, 212));
            tree.Insert(new Volume(4, "D", 314245, 21354));

            var query = tree.Where(x => x.Free > 2800);
            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
        }

        class Volume : IComparable<Volume>
        {
            public Volume(int id, string name, int size, int used)
            {
                Id = id;
                Name = name;
                Size = size;
                Used = used;
            }
        
            public int Id { get; set; }
            public string Name { get; set; }
            public int Size { get; set; }
            public int Used { get; set; }
            public int Free => Size - Used;
            public override string ToString()
            {
                return $"{Id}: {Name} ({Size} - {Used})";
            }

            public int CompareTo(Volume? other)
            {
                if (other == null) return 1;
                return Size.CompareTo(other.Size);
            }
        }
    }
}