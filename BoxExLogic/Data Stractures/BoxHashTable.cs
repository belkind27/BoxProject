using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace BoxExLogic
{
    class BoxHashTable
    {
        Linked_List<Box>[] _boxes;
        int items = 0;
        int maxItems;
        public BoxHashTable(int maxItems = 1024)
        {
            _boxes = new Linked_List<Box>[maxItems];
            this.maxItems = maxItems;
        }
        public Node<Box> GetValue(double key, double x, double y)
        {
            Box value = new Box(x, y, 0);
            int ind = CalcHashCode(key);
            if (_boxes[ind] == null) return null;
            else
            {
                return _boxes[ind].Find(value);
            }
        }
        public void Add(double key, Node<Box> value,out string errorMessage,out bool isExist)
        {
            isExist = false;
            int ind = CalcHashCode(key);
            if (_boxes[ind] == null)
            {
                _boxes[ind] = new Linked_List<Box>();
                _boxes[ind].Add(value);
                items++;
                errorMessage = "box enterd succssesfully";
            }
            else
            {
                if (_boxes[ind].GetArray().Any(b=>b.Data.Key==key&&b.Data.CompareTo(value.Data)!=0))
                {
                    throw new ArgumentException($"due to system key overload you cannot enter this box");
                }
                if (_boxes[ind].Find(value.Data)!=null)
                {
                    if (_boxes[ind].Find(value.Data).Data.Amount + value.Data.Amount > value.Data.MaxAmount)
                    {
                        _boxes[ind].Find(value.Data).Data.Amount = value.Data.MaxAmount;
                        errorMessage = "the box you enterd allready exists,and the amount is to big so we added as much as we could";
                        isExist = true;
                    }
                    else
                    {
                        _boxes[ind].Find(value.Data).Data.Amount += value.Data.Amount;
                        errorMessage = "the box you enterd allready exists so just added the amount";
                        isExist = true;
                    }
                }
                else
                {
                    _boxes[ind].Add(value);
                    items++;
                    errorMessage = "box enterd succssesfully";

                }
            }
            if (items > maxItems)
            {
                ReHash();
            }
        }
        public void Remove(double key, double x, double y)
        {
            Node<Box> node= GetValue(key, x, y);
            node.Remove();
            _boxes[CalcHashCode(key)].Remove(node.Data);
            items--;
        }
        private void ReHash()
        {
            items = 0;
            maxItems *= 2;
            Linked_List<Box>[] TmpArr = _boxes;
            _boxes = new Linked_List<Box>[maxItems];
            for (int i = 0; i < TmpArr.Length; i++)
            {
                if (TmpArr[i] != null)
                {
                   Node<Box>[] d = TmpArr[i].GetArray();
                    for (int j = 0; j < d.Length; j++)
                    {
                        Add(d[j].Data.Key, d[j],out string s,out bool f);
                    }
                }
            }
        }
        private int CalcHashCode(double key)
        {
            int res = key.GetHashCode();
            return Math.Abs(res) % _boxes.Length; 
        }
        public Box[] ToArray()
        {
            Box[] boxes = new Box[items];
            int counter = 0;
            for (int i = 0; i < _boxes.Length; i++)
            {
                if(_boxes[i]!=null)
                {
                    Node<Box>[] d = _boxes[i].GetArray();
                    for (int j = 0; j < d.Length; j++)
                    {
                        if (d[j]!=null)
                        {
                            boxes[counter] = d[j].Data;
                            counter++; 
                        }
                    }
                }         
            }
            return boxes;
        }
    }
}
