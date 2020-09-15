using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace BoxExLogic
{

    class Linked_List<T> where T : IComparable
    {

        Node<T> _start ;
        Node<T> _end ;
        int _counter;
        public Linked_List()
        {
            _counter = 0;
        }
        public void Add(Node<T> n)
        {
            n.Right = n.Left = null;
            if (_start == null) _start = _end = n;
            else
            {
                n.Left = _end;
                _end.Right = n;
                _end = n;
            }
            _counter++;
        }
        public Node<T> Find(T value)
        {
            Node<T> tmp = _start;
            while (tmp != null)
            {
                if (tmp.Data.CompareTo(value) == 0) return tmp; ;
                tmp = tmp.Right;
            }
            return null;
        }

        public void Remove(T value)
        {
            Node<T> removed = Find(value);
            if (_start == removed || _end == removed)
            {
                if (_start == removed)
                {
                    if (removed.Right != null) { _start = removed.Right;_start.Left = null; }
                    else _start = null;
                }
                if (_end == removed)
                {
                    if (removed.Left != null) { _end = removed.Left; _end.Right = null; }
                    else _end = null;
                }
            }
            else
            {
                removed.Left.Right = removed.Right;
                removed.Right.Left = removed.Left;
            }
            _counter--;
        }

        public Node<T>[] GetArray()
        {
            Node<T>[] arr = new Node<T>[_counter];
            Node<T> tmp = _start;
            int i = 0;
            while (tmp != null)
            {
                arr[i]=tmp;
                tmp = tmp.Right;
                i++;
            }
            return arr;
        }


    }
}


