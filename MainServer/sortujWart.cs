using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class sortujWart : IComparer<Uzytkownik>
    {
        public int Compare(Uzytkownik a, Uzytkownik b)
        {
            if (a.wart < b.wart) return 1;
            else if (a.wart > b.wart) return -1;
            else return 0;
        }
    }
}