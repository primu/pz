using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Pokoj
    {
        //pola POKOJU
        public string nazwaPokoju;
        public Int64 numerPokoju;
        public int iloscGraczyMax;
        public int iloscGraczyObecna;
        public bool graRozpoczeta;
        public Int64 stawkaWejsciowa;
        public Int64 duzyBlind; // około 1/100 stawki wejściowej
        public List<Uzytkownik> user = new List<Uzytkownik>();

        private Gra gra;//= new Gra(duzyBlind);

        public bool jestWpokoju(Int64 idUsera)//sprawdza czy uzytkownik znajduje sie wpokoju, zwraca nr pokoju
        {
            if (user.FindIndex(delegate(Uzytkownik c) { return c.identyfikatorUzytkownika == idUsera; }) > 0)
                return true;
            else
                return false;
        }
        public Gra zwrocGre()
        {
            return gra;
        }
        //pola GRY


    }
}