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
        
        [WebMethod]
       // public int gen()
        //{         
           // ukl.generujKarty();


            //return ukl.czyPara(); 
        //}


        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        //Pokoje
        [WebMethod]
        public List<Pokoj> PobierzPokoje(string token)
        {
            //return pokoje;
            List<Pokoj> a = new List<Pokoj>();
            a.Add(Baza.ZwrocPokoj(token));
            return a; 
        }
        [WebMethod]
        public Komunikat DolaczDoStolu(byte[] token, string id)
        {

            Baza.ZmienPokoj(token, id);
            Baza.ZmienPokoj(Baza.CzyZalogowany(token), Baza.DodajPokoj("asd5", 1040, 233, 48));
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
        //===========
        [WebMethod]
        public Komunikat Start(string token, Int64 numer)//
        {
            Uzytkownik user = Glowny.ZweryfikujUzytkownika(token);

            Pokoj pokoj = pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == numer; });

            pokoj.user.Find(delegate(Uzytkownik c) { return c.identyfikatorUzytkownika == user.identyfikatorUzytkownika; }).start = true;
            user.start = true;

            int t = 0;
            foreach (Uzytkownik u in pokoj.user)
            {
                t++;
            }
            if (t == pokoj.iloscGraczyMax)
            {
                pokoj.rozdanie();

                foreach (Uzytkownik u in pokoj.user)
                {
                    Akcja a = new Akcja();
                    a.identyfikatorGracza = u.identyfikatorUzytkownika;
                    a.nazwaAkcji = Akcja.nAkcji.SYSTEM;
                    a.stempelCzasowy = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    a.numerStolu = numer;
                    a.kartyGracza = u.hand;
                    a.kartyNaStole = pokoj.stol;
                    a.duzyBlind = false;
                    a.malyBlind = false;
                    a.obecnaStawkaGracza = 0;

                    if (pokoj.ktoBlind == u.identyfikatorUzytkownika)
                    {
                        a.duzyBlind = true;
                        a.obecnaStawkaGracza = pokoj.duzyBlind;
                    }
                    else
                        if (pokoj.KtoPoprzedni(pokoj.ktoBlind) == u.identyfikatorUzytkownika)
                        {
                            a.malyBlind = true;
                            a.obecnaStawkaGracza = pokoj.duzyBlind / 2;
                        }

                    if (pokoj.KtoNastepny(pokoj.ktoBlind) == u.identyfikatorUzytkownika)
                    {
                        a.nastepnyGracz = u.identyfikatorUzytkownika;
                    }

                    a.obecnaStawkaStolu = pokoj.duzyBlind;
                    a.iloscKasyNaStole = (Int64)(pokoj.duzyBlind * 1.5);
                    a.iloscKasyGracza = pokoj.stawkaWejsciowa - a.obecnaStawkaGracza;

                    pokoj.akcje.Add(a);
                }

            }
            temp.kodKomunikatu = 200;
            temp.trescKomunikatu = "ok";
            return temp;
        }

        [WebMethod]
        public Gra PobierzStanStolu(byte[] token)//prawie OK
        {
            if (Baza.CzyPoprawny(token) == true)//chyba ok
            {
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(Baza.ZwrocIdUzytkownika(token)) == true)
                    {
                        return p.zwrocGre();
                    }
                }
            }
            return null;
        }

        [WebMethod]
        public List<Karta> PobierzKarty(byte[] token)
        {
            return null;
        }

        [WebMethod]
        public Komunikat Fold(byte[] token)
        {
            if (Baza.CzyPoprawny(token) == true)
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {                 
                    if (p.jestWpokoju(id) == true)
                    {
                        if (p.zwrocGre().czyjRuch == id)
                        {
                            if (p.zwrocGre().aktywni.FindIndex(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }) >= 0)
                            {
                                p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }).stan = Gracz.StanGracza.Fold;
                                p.zwrocGre().aktualizujListeUser();// aktualizowanie na liscie userow stanu gracza
                                p.zwrocGre().aktywni.Remove(p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }));//usuwanie gracza ktory folduje
                                temp.kodKomunikatu = 200;
                            }
                        }
                        else
                        {
                            temp.kodKomunikatu = 404;
                        }
                            
                    }
                }
            }
            return temp;
        }

        [WebMethod]
        public Komunikat CallRiseAllIn(byte[] token, Int64 ile)
        {
            bool pom = false;
            if (Baza.CzyPoprawny(token) == true)
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(id) == true)
                    {
                        if (p.zwrocGre().czyjRuch == id)
                        {
                            Gracz R = p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; });//gracz ktory zostal zweryfikowany
                            if (R.kasa >= ile)
                            {
                                if (R.stawia + ile > p.zwrocGre().najwyzszaStawka)
                                {
                                    p.zwrocGre().ktoStawia = R.identyfikatorUzytkownika;
                                    R.stan = Gracz.StanGracza.Rise;
                                    pom = true;
                                }
                                else if (R.stawia + ile == p.zwrocGre().najwyzszaStawka)
                                {
                                    R.stan = Gracz.StanGracza.Call;
                                    pom = true;
                                }
                                else if (R.stawia + ile < p.zwrocGre().najwyzszaStawka)
                                {
                                    if (ile == R.kasa)
                                    {
                                        R.stan = Gracz.StanGracza.AllIn;
                                        pom = true;
                                    }
                                    else if (ile < R.kasa)
                                    {
                                        temp.kodKomunikatu = 404;
                                        break;
                                    }
                                }
                                if (pom == true)
                                {
                                    R.kasa = R.kasa - ile;
                                    p.zwrocGre().pula += ile;
                                    R.stawia += ile;
                                    temp.kodKomunikatu = 200;
                                    p.zwrocGre().KoniecRuchu();
                                    break;
                                }
                            }
                            else
                            {
                                temp.kodKomunikatu = 404;
                                break;
                            }

                        }
                        else
                        {
                            temp.kodKomunikatu = 404;
                            break;
                        }
                    }
                    else
                    {
                        temp.kodKomunikatu = 404;
                        break;
                    }
                }
            }
            return temp;
        }

        //[WebMethod]
        //public Komunikat Rise(string token, Int64 dokladam)
        //{
        //    return temp;
        //}

        //[WebMethod]
        //public Komunikat AllIn(string token)
        //{
        //    return temp;
        //}

    }
}
