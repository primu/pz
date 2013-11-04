using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Uzytkownik
    {
        public Int64 identyfikatorUzytkownika;
        public string nazwaUzytkownika;
        public Int64 numerPokoju;

        public Uzytkownik()
        {
        }

        public Uzytkownik(Int64 id, string nazwa, Int64 numer)
        {
            this.identyfikatorUzytkownika = id;
            this.nazwaUzytkownika = nazwa;
            this.numerPokoju = numer;
        }


    }
}