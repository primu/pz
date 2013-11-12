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

        public List<Karta> hand = new List<Karta>();
        public List<Karta> najUklad= new List<Karta>();
        public int kicker;
        public bool fold; 
        public int wart;//wartość układu kart

      
    }
}