using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExLogic
{
    class DateSearch
    {
        LinkedLine _dates;
        public DateSearch()
        {
            _dates = new LinkedLine();
        }
        public void Add(DateNode<DateTime> node)
        {
            _dates.Add(node);
        }
        public List<Box> Remove()
        {
            return _dates.Remove();
        }

    }
}
