#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SetImpl
{
    public class SetNode<T>
    {
        public SetNode<T> LeftNode { get; set; }
        public SetNode<T> RightNode { get; set; }
        public T Data { get; set; }
    }
}
