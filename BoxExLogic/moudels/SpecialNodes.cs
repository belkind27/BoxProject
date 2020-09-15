using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExLogic
{
     class Node<T> where T : IComparable
    {
        T _data;
        Node<T> _left;
        Node<T> _right;
        Node<T> _parent;
        public event NodeRemovedEventHandler NodeRemoved;
        public T Data { get { return _data; } set { _data = value; } }
        public Node<T> Left { get{return  _left;} set{ _left= value; } }
        public Node<T> Parent { get { return _parent; } set { _parent = value; } }
        public Node<T> Right { get { return _right; } set { _right = value; } }
        public Node(T data)
        {
            _data = data;

        }
        public Node<T> FindNearest()
        {
            if (_right == null)
            {
                if (Parent == null) return null;
                else if (this == Parent.Right)
                {
                    return Parent.Parent;
                }
                else return _parent;
            }
            else
            {
                Node<T> tmp = _right;
                while (true)
                {
                    if (tmp._left == null) return tmp;
                    else tmp = tmp._left;
                }
            }
        }
        public void Remove()
        {
            Box box= Data as Box ;
            box.Ynode.NodeRemoved.Invoke(box.Ynode, new NodeRemovedEventArgs());
            box.Xnode.NodeRemoved.Invoke(box.Xnode, new NodeRemovedEventArgs());
            box.DateNode.NodeRemoved.Invoke(box.DateNode, new NodeRemovedEventArgs());
        }
        public void RemoveDate()
        {
            Box box = Data as Box;
            box.DateNode.NodeRemoved.Invoke(box.DateNode, new NodeRemovedEventArgs());
        }
    }
     class Ynode<T>: Node<T> where T : IComparable
    {
        public Ynode(T y):base(y)
        {
        }
    }
     class Xnode<T> : Node<T> where T : IComparable
    {
        Binary_Tree<T> _Ys;
        public Binary_Tree<T> Ys { get { return _Ys; } }
        public Xnode(T x) : base(x)
        {
            _Ys = new Binary_Tree<T>();
        }
    }
     class DateNode<T> : Node<T> where T : IComparable
    {
        Box _box;
        public Box Box { get { return _box; } }
        public DateNode(T lastTimeOrder, Box box) : base(lastTimeOrder)
        {
            _box = box;           
        }

    }
    public delegate void NodeRemovedEventHandler(object sender, NodeRemovedEventArgs e);
    public class NodeRemovedEventArgs : EventArgs
    {

    }

}
