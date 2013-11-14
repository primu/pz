﻿using System;
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
        //static UkladyKart ukl = new UkladyKart();
        //static private Pokoj pok = new Pokoj();

    /// <summary>
    
    /// </summary>
    /// <returns></returns>

        [WebMethod]
        public List<Uzytkownik> listaUzytkownikow(Int64 numer)
        {
            return pokoje.Find(delegate(Pokoj p) { return p.numerPokoju == numer; }).user;
        }

        [WebMethod]
        public List<Uzytkownik> KtoWygral(Int64 numer)
        {
            return pokoje.Find(delegate(Pokoj p) { return p.numerPokoju == numer; }).ktoWygral();
        }

        [WebMethod]
        public List<Karta> naStole(Int64 numer)
        {
            return pokoje.Find(delegate(Pokoj p) { return p.numerPokoju == numer; }).stol;
        }

        [WebMethod]
        public string gen(Int64 numer)
        {
            return pokoje.Find(delegate(Pokoj p) { return p.numerPokoju == numer; }).gen();
        }    

        //Pokoje
        [WebMethod]
        public List<Pokoj> PobierzPokoje(string token)
        {
            return pokoje;
        }

        [WebMethod]
        public Komunikat DolaczDoStolu(string token, Int64 numer)
        {
            try
            {
                if (token.Length > 30) // ;]
                {
                    //Pokoj p = pokoje.Find(delegate(Pokoj c) {return c.numerPokoju==numer;});
                    Uzytkownik user = Glowny.ZweryfikujUzytkownika(token);
                    int tmp = pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == numer; }).DodajUzytkownika(user);
                    switch (tmp)
                    {
                        case 0://user już jest w pokoju
                            temp.kodKomunikatu = 403;
                            temp.trescKomunikatu = "Jesteś już w tym pokoju.";
                            break;
                        case 1://user dodany do pokoju
                            temp.kodKomunikatu = 200;
                            temp.trescKomunikatu = "Dodano Cię do pokoju.";
                            user.numerPokoju = numer;
                            break;
                        default://pokój pełny / błąd
                            temp.kodKomunikatu = 402;
                            temp.trescKomunikatu = "Pokój jest już pełny.";
                            break;
                    }                    
                    
                }
                else
                {
                    temp.kodKomunikatu = 403;
                    temp.trescKomunikatu = "Pokój nie został wybrany. Błąd tokenu.";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 400;
                temp.trescKomunikatu = "Błąd serwera rrozgrywki.\n" + e.Source + "\n" + e.Message;
            }
            return temp;
        }

        [WebMethod]
        public Komunikat OpuscStol(string token, Int64 numer)
        {
            try
            {
                if (token.Length > 30) // ;]
                {
                    Uzytkownik user = Glowny.ZweryfikujUzytkownika(token);                    
                    int tmp = pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == numer; }).UsunUzytkownika(user);
                    switch (tmp)
                    {
                        case 1://user usuniety z pokoju
                            temp.kodKomunikatu = 200;
                            temp.trescKomunikatu = "Opuściłeś pokój.";
                            user.numerPokoju = 0;
                            break;
                        default://user nie jest w pokoju / błąd
                            temp.kodKomunikatu = 403;
                            temp.trescKomunikatu = "Nie jesteś w tym pokoju.";
                            break;
                    }      
                }
                else
                {
                    temp.kodKomunikatu = 400;
                    temp.trescKomunikatu = "Pokój nie został wybrany. Błąd tokenu.";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 400;
                temp.trescKomunikatu = "Błąd serwera rrozgrywki.\n" + e.Source + "\n" + e.Message;
            }
            return temp;
        }

        [WebMethod]
        public Komunikat UtworzStol(string token, string nazwa, int maxGraczy, Int64 stawkaWe, Int64 bigBlind)
        {
            try
            {
                if (token.Length > 30) // ;]
                {
                    Uzytkownik user = Glowny.ZweryfikujUzytkownika(token);
                    int p = pokoje.Count+1;
                    user.numerPokoju = p;
                    pokoje.Add(new Pokoj(nazwa, p, maxGraczy, stawkaWe, bigBlind, user));

                    

                    temp.kodKomunikatu = 201;
                    temp.trescKomunikatu = "Pokój został utworzony.";
                }
                else
                {
                    temp.kodKomunikatu = 400;
                    temp.trescKomunikatu = "Pokój nie został utworzony. Błąd tokenu.";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 400;
                temp.trescKomunikatu = "Błąd serwera rrozgrywki.\n" + e.Source + "\n" + e.Message;
            }
            return temp;
        }
    
        //Rozgrywka
        [WebMethod]
        public List<Akcja> PobierzStanStolu(string token)
        {
            /*
            List<Akcja> lsa = new List<Akcja>();
            List<Karta> karty = new List<Karta>();
            karty.Add(new Karta{figura = Karta.figuraKarty.K3,kolor=Karta.kolorKarty.pik});
            karty.Add(new Karta{figura = Karta.figuraKarty.KK,kolor=Karta.kolorKarty.trefl});
            lsa.Add(new Akcja { nazwaAkcji = "fold", duzyBlind = true, obecnaStawkaStolu = 350,kartyGracza=karty});
            lsa.Add(new Akcja { nazwaAkcji = "rise", obecnaStawkaStolu = 650 });
             * */
            List<Akcja> a = new List<Akcja>();
            a.Clear();/*
            for (int i = 0; i < wiadomosci.Count; i++)
            {r
                if (timT < wiadomosci.ElementAt(i).stempelCzasowy)
                {
                    wiad.Add(wiadomosci.ElementAt(i));
                }
            }*/
            return a;
        }

        [WebMethod]
        public Komunikat WyslijRuch(string token, Akcja akcja, Int64 numer)
        {
            temp = new Komunikat();
            Uzytkownik user = Glowny.ZweryfikujUzytkownika(token);
            Pokoj pokoj = pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == numer; });

            switch (pokoj.stan)
            {
                case Pokoj.Stan.STARTING:
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "";
                    break;
            }
            


            return temp;
        }

        [WebMethod]
        public Komunikat Start(string token, Int64 numer)//inicjalizacja stołu z rozgrywką
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
                    a.nazwaAkcji = "rozdanie kart";
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
                    
                }
                
            }
            temp.kodKomunikatu = 200;
            temp.trescKomunikatu = "ok";
            return temp;
        }






    }
}
