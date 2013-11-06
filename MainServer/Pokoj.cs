using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Pokoj
    {
        public Int64 numerPokoju;
        public int iloscGraczyMax;
        public int iloscGraczyObecna;
        public bool graRozpoczeta;
        public Int64 stawkaWejsciowa;
        public Int64 duzyBlind;

        //private Uzytkownik user = new Uzytkownik();
        private List<Uzytkownik> user = new List<Uzytkownik>();

        private void pobierzUserow()
        {
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 1, nazwaUzytkownika = "pawel", numerPokoju = 1 });
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 2, nazwaUzytkownika = "Bogdan", numerPokoju = 1 });        
        }

        public string gen()
        {
            pobierzUserow();
            user[0].ukl.generujKarty();                      
            return user[0].ukl.co_mamy(); 
        }

        public List<Karta> nasze()
        {
            return user[0].ukl.reka();
        }

    }    
}