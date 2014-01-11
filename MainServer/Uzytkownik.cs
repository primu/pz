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

        public Int64 kasiora;
        public bool start = false;

        public Uzytkownik()
        {
        }

        public Uzytkownik(Int64 id, string nazwa, Int64 numer)
        {
            identyfikatorUzytkownika = id;
            nazwaUzytkownika = nazwa;
            numerPokoju = numer;
        }

        public Uzytkownik(Int64 id, string nazwa, Int64 numer, Int64 kasa)
        {
            identyfikatorUzytkownika = id;
            nazwaUzytkownika = nazwa;
            numerPokoju = numer;
            kasiora = kasa;
        }
    }
}