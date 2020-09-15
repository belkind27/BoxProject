using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BoxExLogic
{
    class SizeSearch
    {
        Binary_Tree<double> XBox;
        public SizeSearch()
        {
            XBox = new Binary_Tree<double>();
        }
        public void Add(Box box)
        {
            Xnode<double> node = (Xnode<double>)XBox.Find(box.X);
            Ynode<double> yn = new Ynode<double>(box.Y);
            if (node == null)
            {
                node = new Xnode<double>(box.X);
                XBox.AddItem(node);
                node.Ys.AddItem(yn);
            }
            else
            {
                node.Ys.AddItem(yn);
            }
            box.Ynode = yn;
            box.Xnode = node;
        }
        public Tuple<double, double, double> Find(double x, double y)
        {
            double _x, _y, _key;
            Xnode<double> node = null;
            Ynode<double> yn = null;
            double xToSearch = x;
            node = (Xnode<double>)XBox.Find(x);
            if (node == null)
            {
                node = (Xnode<double>)XBox.FindNearest(x);
                if (node == null) return null;
                _x = node.Data;
                xToSearch = _x;
            }
            else _x = node.Data;
            yn = (Ynode<double>)node.Ys.Find(y);
            if (yn == null)
            {
                yn = (Ynode<double>)node.Ys.FindNearest(y);
                if (yn == null)
                    while (yn == null)
                    {
                        node = (Xnode<double>)node.FindNearest();
                        if (node == null) return null;
                        _x = node.Data;
                        xToSearch = _x;
                        yn = (Ynode<double>)node.Ys.Find(y);
                        if (yn == null)
                        {
                            yn = (Ynode<double>)node.Ys.FindNearest(y);
                        }
                    }
            }
            _y = yn.Data;
            _key = ((_x * _y) - x) / ((_x * _y) + _y);
            Tuple<double, double, double> t = new Tuple<double, double, double>(_x, _y, _key);
            return t;

        }
        public Tuple<double, double, double> FindForMany(double x, double y)
        {
            double _x, _y, _key;
            Xnode<double> node = null;
            Ynode<double> yn = null;
            double xToSearch = x;
             node = (Xnode<double>)XBox.Find(x);
            if (node == null)
            {
                node = (Xnode<double>)XBox.FindNearest(x);
                if (node == null) return null;
                _x = node.Data;
                xToSearch = _x;
            }
            else _x = node.Data;
             yn = (Ynode<double>)node.Ys.FindNearest(y);
            if (yn == null)
            {
                while (yn == null)
                {
                    node = (Xnode<double>)node.FindNearest();
                    if (node == null) return null;
                    _x = node.Data;
                    xToSearch = _x;
                    yn = (Ynode<double>)node.Ys.Find(y);
                    if (yn == null)
                    {
                        yn = (Ynode<double>)node.Ys.FindNearest(y);
                    }
                }
            }
            _y = yn.Data;
            _key = ((_x * _y) - _x) / ((_x * _y) + _y);
            Tuple<double, double, double> t = new Tuple<double, double, double>(_x, _y, _key);
            return t;

        }

    }

}
