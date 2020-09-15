using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BoxExLogic
{
    public class Box:IComparable
    {
        double _x;
        double _y;
        double _key;
        int _amount;
        Xnode<double> _xnode;
        Ynode<double> _ynode;
        DateNode<DateTime> _dateNode;
        const int _maxAmount=50;
        const int _minAmount=10;
        DateTime _lastTimeOrder;
        public Box(){ }
        public Box(double x,double y,int amount)
        {
            if (x < 0 || y < 0) throw new ArgumentException();
            _x = x;
            _y = y;
            if (amount > _maxAmount || amount <0) throw new ArgumentException("the amount you enterd is invalid");
            _amount = amount;
            _key =((x * y)-x)/((x*y)+y);
        }
        public double Key { get { return _key; } set { _key = value; } }
        public double X { get { return _x; } set { _x = value; } }
        public double Y { get { return _y; } set { _y = value; } }
        public int Amount { get { return _amount; }set { _amount = value; } }
        public int MaxAmount { get { return _maxAmount; } }
        public int MinAmount { get { return _minAmount; } }
        public DateTime LastTimeOrder { get { return _lastTimeOrder; } set { _lastTimeOrder = value; } }
        [XmlIgnore]
        internal Xnode<double> Xnode { get => _xnode; set => _xnode = value; }
        [XmlIgnore]
        internal Ynode<double> Ynode { get => _ynode; set => _ynode = value; }
        [XmlIgnore]
        internal DateNode<DateTime> DateNode { get => _dateNode; set => _dateNode = value; }
        public int CompareTo(object obj)
        {
            Box c = obj as Box;
            if (_x == c._x && _y == c._y) return 0;
                return 1;
        }

    }
    public class BoxException : Exception
    {
        public BoxException() { }
        public BoxException(string message) : base(message) { }
        public BoxException(string message, Exception inner) : base(message, inner) { }
        protected BoxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
