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
        
        public List<Uzytkownik> user = new List<Uzytkownik>();
        static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
        static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };
        List<Karta> talia = new List<Karta>();
        public List<Karta> stol = new List<Karta>();
        public UkladyKart ukl = new UkladyKart();

        public void rozdanie()
        {
            generujKarty();
            Random rnd1 = new Random();
            for (int i = 0; i < user.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int a=rnd1.Next(0, talia.Count);
                    user[i].hand.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }
        }

        public void losujNaStol(int ile)
        {
            Random rnd1 = new Random();           
                for (int j = 0; j < ile; j++)
                {
                    int a = rnd1.Next(0, talia.Count);
                    stol.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }          
        }

        public void generujKarty()
        {
            talia.Clear();
            for (int i = 0; i < 52; i++)
            {
                if (i < 13)
                {
                    talia.Add(new Karta { figura = figury[i], kolor = kolory[0] });
                    //talia[i] = new Karta { figura = figury[i], kolor = kolory[0] };
                }
                if (i > 12 && i < 26)
                {
                    talia.Add( new Karta { figura = figury[i - 13], kolor = kolory[1] });
                }
                if (i > 25 && i < 39)
                {
                    talia.Add(new Karta { figura = figury[i - 26], kolor = kolory[2] });
                }
                if (i > 38 && i < 52)
                {
                    talia.Add(new Karta { figura = figury[i - 39], kolor = kolory[3] });
                }
            }         
        }

        private void pobierzUserow()
        {
            user.Clear();
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 1, nazwaUzytkownika = "Pawel", numerPokoju = 1 });
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 2, nazwaUzytkownika = "Bogdan", numerPokoju = 1 });        
        }

        public string gen()
        {
            pobierzUserow();
            ukl.generujKarty();  
            rozdanie();        
            return ukl.co_mamy(); 
        }

        public List<Karta> nasze()
        {
            return ukl.reka();
        }

        public void ktoWygral()
        {

        }
    }    
}