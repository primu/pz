using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace MainServer
{
    /// <summary>
    /// Summary description for Rozgrywki
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Rozgrywki : System.Web.Services.WebService
    {
        //Tymczasowe deklaracje
        private Komunikat temp = new Komunikat();
        static private List<Pokoj> pokoje =  new List<Pokoj>();
        static private List<Akcja> akcje = new List<Akcja>();
        static UkladyKart ukl = new UkladyKart();

        private Glowny WS = new Glowny();
       
        [WebMethod]
        public int gen()
        {         
            ukl.generujKarty();

            return ukl.czyPokerKrolewski(); 
        }


        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        //Pokoje
        [WebMethod]
        public List<Pokoj> PobierzPokoje(string token)
        {
            return pokoje;
        }
        [WebMethod]
        public Komunikat DolaczDoStolu(string token, Uzytkownik uzytkownik)
        {
            return temp;
        }
        [WebMethod]
        public Komunikat OpuscStol(string token, Uzytkownik uzytkownik)
        {
            return temp;
        }
    
        //Rozgrywka
        [WebMethod]
        public List<Akcja> PobierzStanStolu(string token)
        {
            List<Akcja> lsa = new List<Akcja>();
            List<Karta> karty = new List<Karta>();
            karty.Add(new Karta{figura = Karta.figuraKarty.K3,kolor=Karta.kolorKarty.pik});
            karty.Add(new Karta{figura = Karta.figuraKarty.KK,kolor=Karta.kolorKarty.trefl});
            lsa.Add(new Akcja { nazwaAkcji = "fold", duzyBlind = true, obecnaStawkaStolu = 350,kartyGracza=karty});
            lsa.Add(new Akcja { nazwaAkcji = "rise", obecnaStawkaStolu = 650 });
            return lsa;
        }
        [WebMethod]
        public Komunikat WyslijRuch(string token, Akcja akcja)
        {
            return temp;
        }
    }
}
