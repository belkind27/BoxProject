using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;

namespace BoxExLogic
{
    public class BoxManger
    {
        BoxHashTable _boxCollection;
        DateSearch _dateSearch;
        SizeSearch _sizeSearch;
        List<Box> _serilizeDb;
        DispatcherTimer _timer;
        LinkedList<Box> buyManyRes;
        bool isBuyManyStopped;
        public event DateRemovedEventHandler DateRemoved;

        public BoxManger(DateRemovedEventHandler eventHandler)
        {
            DateRemoved += eventHandler;
            _serilizeDb = new List<Box>();
            Load();
            //Test();
            _boxCollection = new BoxHashTable();
            _dateSearch = new DateSearch();
            _sizeSearch = new SizeSearch();
            _timer = new DispatcherTimer();
            string s;
            if (_serilizeDb.Count != 0) foreach (Box item in _serilizeDb)
                {
                    if (item == null) break;
                    Add(item.X, item.Y, item.Amount, out s, item.LastTimeOrder);
                }
            _timer_Tick(new object(), new object());
            _timer.Interval = new TimeSpan(24, 0, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }
        private void _timer_Tick(object sender, object e)
        {
            List<Box> list = _dateSearch.Remove();
            if (list.Count != 0)
            {
                if (DateRemoved.GetInvocationList()!=null)
                {
                    DateRemoved.Invoke(this, new DateRemovedEventArgs(list));
                }
                foreach (var item in list)
                {
                    _boxCollection.Remove(item.Key, item.X, item.Y);
                }
            }
        }
        public void Add(double x, double y, int amount, out string message)
        {
            Add(x, y, amount, out message, DateTime.Now);
        }
        public void Add(double x, double y, int amount, out string message, DateTime date)
        {            
            Box box = new Box(x, y, amount);
            box.LastTimeOrder = date;
            Node<Box> node = new Node<Box>(box);
            _boxCollection.Add(box.Key, node, out message, out bool isExist);//bool status
            if (!isExist)
            {
                DateNode<DateTime> nd = new DateNode<DateTime>(box.LastTimeOrder, box);
                box.DateNode = nd;
                _dateSearch.Add(nd);
                _sizeSearch.Add(box);
            }
        }
        public Box Buy(double x, double y, out string message)
        {
            message = "";
            double searchKey = ((x * y) - x) / ((x * y) + y);
            var box = _boxCollection.GetValue(searchKey, x, y);
            if (box == null)
            {
                Tuple<double, double, double> t = _sizeSearch.Find(x, y);
                if (t == null) return null;
                box = _boxCollection.GetValue(t.Item3, t.Item1, t.Item2);
            }
            if (box == null) throw new BoxException("we currentlly dont have any boxes");
            box.Data.Amount--;
            if (box.Data.Amount < box.Data.MinAmount)
            {
                message = ",you are about to run out of this box";
            }
            if (box.Data.Amount == 0)
            {
                message = ",you ran out of this box";
                _boxCollection.Remove(box.Data.Key, box.Data.X, box.Data.Y);
            }
            else
            {
                box.RemoveDate();
                box.Data.LastTimeOrder = DateTime.Now;
                DateNode<DateTime> node = new DateNode<DateTime>(DateTime.Now, box.Data);
                _dateSearch.Add(node);
                box.Data.DateNode = node;
            }
            return box.Data;
        }
        public string BuyManyOrder(double x, double y, int amount, out bool isSuccesful)
        {
            isSuccesful = false;
            StringBuilder sb = new StringBuilder();
            string response = "";
            int amountAsked = amount;
            LinkedList<Box> boxes = new LinkedList<Box>();
            double searchKey = ((x * y) - x) / ((x * y) + y);
            var box = _boxCollection.GetValue(searchKey, x, y);
            if (box != null)
            {
                boxes.AddLast(box.Data);
                if (box.Data.Amount >= amountAsked)
                {
                    box.Data.Amount -= amountAsked;
                    if (box.Data.Amount == 0) _boxCollection.Remove(box.Data.Key, box.Data.X, box.Data.Y);
                    isSuccesful = true;
                    response = "you succssesfully bought the boxes";
                    return response;

                }
            }
            response = "in order to fullfil your order with our current stock we offer this boxes";
            while (amountAsked > 0)
            {
                Tuple<double, double, double> t = _sizeSearch.FindForMany(x, y);
                if (t == null)
                {
                    response = "unfotunatly we couldent complete your order due to shortage, but you can still buy this sub order";
                    isBuyManyStopped = true;
                    break;
                }
                box = _boxCollection.GetValue(t.Item3, t.Item1, t.Item2);
                boxes.AddLast(box.Data);
                amountAsked -= box.Data.Amount;
                if (amountAsked == 0) isBuyManyStopped = false;
                if (amountAsked < 0)
                {
                    box.Data.Amount = Math.Abs(amountAsked);
                    isBuyManyStopped = false;
                }
                y = box.Data.Y;
                x = box.Data.X;
            }
            sb.AppendLine(response);
            buyManyRes = boxes;
            foreach (var item in boxes)
            {
                sb.AppendLine($"box amount: {item.Amount} , box width: {item.X}, box height: {item.Y}");
            }
            return sb.ToString();
        }
        public void BuyManyChoise(bool decision)
        {
            if(decision)
            {if (buyManyRes.Last.Value.Amount > 0&&!isBuyManyStopped) buyManyRes.RemoveLast();
                foreach (var item in buyManyRes)
                {
                    _boxCollection.Remove(item.Key, item.X, item.Y);
                }
            }
        }
        public Box Show(double x, double y)
        {
            double searchKey = ((x * y) - x) / ((x * y) + y);
            if (_boxCollection.GetValue(searchKey, x, y) == null) return null;
            return _boxCollection.GetValue(searchKey, x, y).Data;
        }
        public void Save()
        {
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(Box[]));
            FileStream s = new FileStream($@"{ApplicationData.Current.LocalFolder.Path}\BoxFile.xml", FileMode.Create);
            xmlSerializerItem.Serialize(s, _boxCollection.ToArray());
            s.Close();
        }
        public void Load()
        {
            bool isFailed = false;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Box>));
            FileStream s;
            try { s = new FileStream($@"{ApplicationData.Current.LocalFolder.Path}\BoxFile.xml", FileMode.Open); }
            catch (Exception)
            {
                isFailed = true;
                return;
                s = new FileStream($@"{ApplicationData.Current.LocalFolder.Path}\itemFile.xml", FileMode.Create);
            }
            if (!isFailed) { _serilizeDb = (List<Box>)xmlSerializer.Deserialize(s); s.Close(); }

        }
        public void Test()
        {
            double x, y;
            Random r = new Random();
            for (int i = 0; i < 1000; i++)
            {
                x = r.Next(1, 10);
                y = r.Next(1, 1000000);
                _serilizeDb.Add(new Box(x, y, r.Next(1, 20)));
            }
        }
    }
    public delegate void DateRemovedEventHandler(object sender, DateRemovedEventArgs e);
    public class DateRemovedEventArgs : EventArgs
    {
        private List<Box> _deleted;

        public List<Box> Deleted
        {
            get { return _deleted; }
        } 
        public DateRemovedEventArgs(List<Box> DeletedNodes)
        {
            _deleted = DeletedNodes;
        }

    }

}
