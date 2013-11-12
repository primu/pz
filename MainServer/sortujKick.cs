using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class sortujKick: IComparer<Uzytkownik>
    {
        public int Compare(Uzytkownik a, Uzytkownik b)
        {
            if (a.kicker < b.kicker) return 1;
            else if (a.kicker > b.kicker) return -1;
            else return 0;
        }
    }
}