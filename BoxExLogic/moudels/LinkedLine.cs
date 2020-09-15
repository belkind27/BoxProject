using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExLogic
{
    class LinkedLine
    {
        DateNode<DateTime> _first;
        DateNode<DateTime> _last;
        public void Add(DateNode<DateTime> node)
        {
            node.NodeRemoved += nodeRemove;
            if (_first == null) { _first = _last = node; }
            else
            {
                _last.Right = node;
                node.Left = _last;
                _last = node;

            }
        }
        public List<Box> Remove()
        {
            List<Box> deleted = new List<Box>();
            DateNode<DateTime> tmp = _first;
            while (tmp != null)
            {
                DateTime d= tmp.Data.AddDays(30);
                if (d < DateTime.Now)
                {
                    deleted.Add(tmp.Box);
                    tmp = (DateNode<DateTime>)tmp.Right;
                    _first = tmp;
                }
                else break;
            }
            return deleted;
        }
        public void nodeRemove(object sender, NodeRemovedEventArgs e)
        {
                   
                DateNode<DateTime> removed = sender as DateNode<DateTime>;
            if (removed==_first||removed==_last)
            {
                if (_first == removed)
                {
                    if (removed.Right != null) { _first = removed.Right as DateNode<DateTime>; _first.Left = null; }
                    else _first = null;
                }
                if (_last == removed)
                {
                    if (removed.Left != null) { _last = removed.Left as DateNode<DateTime>; _last.Right = null; }
                    else _last = null;
                }  
            }
            else
            {
                if (removed.Left == null) removed.Right.Left = null;
                else
                {
                    removed.Left.Right = removed.Right;
                    removed.Right.Left = removed.Left; 
                }
            }


        }
    }
}
