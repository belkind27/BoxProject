using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExLogic
{
    class Binary_Tree<T> where T : IComparable
    {
        Node<T> _root;
        public Node<T> Root { get => _root; set => _root = value; }
        public void AddItem(Node<T> node)
        {
            if (Root == null)
            {
                Root = node;
                node.NodeRemoved += Node_NodeRemoved;
                return;
            }
            Node<T> tmp = Root;
            Node<T> parent = null;
            while (tmp != null)
            {
                parent = tmp;
                if (tmp.Data.CompareTo(node.Data)>0 ) tmp = tmp.Left;
                else tmp = tmp.Right;
            }         
            if (node.Data.CompareTo(parent.Data)<0 ) parent.Left = node;
            else parent.Right = node;
                node.Parent = parent;
                node.NodeRemoved += Node_NodeRemoved; 
            
        }
        private void Node_NodeRemoved(object sender, NodeRemovedEventArgs e)
        {
            Node<T> node = sender as Node<T>;
            if(node is Xnode<T>)
            {
                Xnode <T> nx= (Xnode<T>)node;
                if (nx.Ys.Root != null) return;
            }
            InnerRemove(node);
            void InnerRemove(Node<T> root)
            {
                if (root == null) return;
                if(root.Parent==null)
                {
                    Node<T> removed = findMinVal(root.Right);
                    if (removed == null)
                    {if (root.Left == null) _root = null;
                       else { _root = root.Left; _root.Parent = null; } }
                    else
                    {
                        root.Data = removed.Data;
                        InnerRemove(removed);
                    }
                    return;

                }
                if (root.Left == null && root.Right == null)
                {
                    if (root.Parent.Left == null) { root.Parent.Right = null; }
                    else if (root.Parent.Right == null) { root.Parent.Left = null; }
                    else
                    {
                        if (root.Parent.Left.Data.CompareTo(root.Data) == 0) { root.Parent.Left = null; }
                        else { root.Parent.Right = null; }
                    }
                }
                else if (root.Right == null)
                {
                    if (root.Parent.Left == null) { root.Parent.Right = root.Left; root.Left.Parent = root.Parent; }
                    else if (root.Parent.Right == null) { root.Parent.Left = root.Left;root.Left.Parent = root.Parent; }
                    else
                    {
                        if (root.Parent.Left.Data.CompareTo(root.Data) == 0) { root.Parent.Left = root.Left; root.Left.Parent = root.Parent; }
                        else { root.Parent.Right = root.Left; root.Left.Parent = root.Parent; }
                    }
                }
                else if (root.Left == null)
                {
                    if (root.Parent.Left == null) { root.Parent.Right = root.Right; root.Right.Parent = root.Parent; }
                    else if (root.Parent.Right == null) { root.Parent.Left = root.Right; root.Right.Parent = root.Parent; }
                    else
                    {
                        if (root.Parent.Left.Data.CompareTo(root.Data) == 0) { root.Parent.Left = root.Right; root.Right.Parent = root.Parent; }
                        else { root.Parent.Right = root.Right; root.Right.Parent = root.Parent; }
                    }
                }
                else
                {
                    Node<T> removed= findMinVal(root.Right);
                    root.Data = removed.Data;
                    InnerRemove(removed);
                }
            }
            Node<T> findMinVal(Node<T> root1)
            {
                if (root1 == null) return null;
                T minVal = root1.Data;
                while (root1.Left != null)
                {
                    minVal = root1.Left.Data;
                    root1 = root1.Left;
                }
                return root1;
            }
        }
        public Node<T> FindNearest(T val)
        {
            Node<T> tmp = Root;
            Node<T> max=default;
            InnerFindNearest(tmp,val);
            return max;
            void InnerFindNearest(Node<T> root, T value)
            {
                if (root == null) return;
                if (root.Data.CompareTo(value) < 0)//smaller then val
                {
                    InnerFindNearest(root.Right, value);
                }
                if (root.Data.CompareTo(value) > 0)//bigger then val
                {
                    max = root;
                    InnerFindNearest(root.Left, value);
                }
                if (root.Data.CompareTo(value) == 0)
                {
                    InnerFindNearest(root.Right, value);
                }
            }
        }
        public Node<T> Find(T y)
        {
            Node<T> tmp = Root;
            return InnerFind(tmp, y);

        }
        private Node<T> InnerFind(Node<T> root, T value)
        {
            
            if (root == null) return null;
            if (root.Data.CompareTo(value)==0)
            {
                return root;
            }
            if (root.Left == null && root.Right == null) return null;
            else if (root.Data.CompareTo(value)>0) return InnerFind(root.Left, value);
            else return InnerFind(root.Right, value);
        }
        public bool IsEmpty()
        {
            return Root == null;
        }
    }

}

