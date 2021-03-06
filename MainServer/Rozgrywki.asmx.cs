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
    public class Rozgrywki : System.Web.Services.WebService
    {
        //Tymczasowe deklaracje
        private Komunikat temp = new Komunikat();
        static private List<Pokoj> pokoje =  new List<Pokoj>();
        static private List<Akcja> akcje = new List<Akcja>();
        static UkladyKart ukl = new UkladyKart();

        [WebMethod]
        public Komunikat PotwierdzZakonczenie(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p != null)
                {
                    if (p.zwrocGre().stan == Gra.Stan.END)
                    {
                        Baza.DodajZwyciezce(Baza.ZwrocUzytkownika(id).nazwaUzytkownika, p.numerPokoju, (int)(p.stawkaWejsciowa * p.iloscGraczyMax));
                        OpuscStol(token);
                        p.WyczyscPokoj();
                        temp.kodKomunikatu = 200;
                        temp.trescKomunikatu = "Gra została zakończona, pokój jest już wolny";
                    }
                }
            }

            return temp;
        }

        [WebMethod]
        public bool ustawNoweRoz(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p != null)
                {
                    if (p.zwrocGre().stan != Gra.Stan.END)
                    {
                        Gracz gr = p.zwrocGre().user.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; });
                        if (gr != null)
                        {

                            gr.czyNoweRozdanie = true;
                            int e = p.zwrocGre().user.Count<Gracz>(delegate(Gracz a) { return a.czyNoweRozdanie == true; });
                            if (e == p.zwrocGre().user.Count)
                            {
                                p.zwrocGre().NoweRozdanie();
                            }
                            return gr.czyGra;
                        }
                    }
                    else
                        return true;
                }
            }
            return false;
        }

        [WebMethod]
        public bool czyWyniki(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p != null)
                {
                    return p.zwrocGre().wyniki;
                }
            }
            return false;
        }

        [WebMethod]
        public string NazwaMojegoUkladu(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().NazwaMojegoUkladu2(id);
                }
            }
            return "";          
        }

        [WebMethod]
        public bool CzyJestWaktywnych(byte[] token,Int64 idGracza)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 idMoje = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(idMoje); });                
                if (p != null)
                {
                    if (p.zwrocGre().aktywni.Find(delegate(Gracz a) { return a.identyfikatorUzytkownika == idGracza; }) != null)
                        return true;
                    else
                        return false;   
                }
            }
            return false;
        }

        [WebMethod]
        public List<Karta> MojNajUkl(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().MojNajUkl2(id);
                }
            }
            return null;
        }

        [WebMethod]
        public List<Uzytkownik> ZwrocUserowStart(byte[] token)//zwraca uzytkownikow którzy dolaczyli do danego pokoju
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.user;
                }
            }
            return null;
        }

        [WebMethod]
        public List<Gracz> ZwrocGraczy(byte[] token)//zwraca uzytkownikow którzy wcisneli start w danym pokoju
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().user;
                }
            }
            return null;
        }

        [WebMethod]
        public List<Karta> ZwrocNajUklGraczy(byte[] token, Int64 idGracza)//zwraca najuklad userow
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().aktywni.Find(delegate(Gracz a) { return a.identyfikatorUzytkownika == idGracza; }).zwroc_najUklad();//[i].zwroc_najUklad();
                }
            }
            return null;
        }

        [WebMethod]
        public List<Karta> ZwrocHandGraczy(byte[] token, Int64 idGracza)//zwraca najuklad userow
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p != null)
                {
                    Gracz g = p.zwrocGre().aktywni.Find(delegate(Gracz a) { return a.identyfikatorUzytkownika == idGracza; });
                    if (g != null)
                        return g.zwroc_hand();
                    else
                        return new List<Karta>();
                }
            }
            return null;
        }

        //=========
        [WebMethod]
        public Gracz PobierzGracza(byte[] token,Int64 mojID)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == mojID; });
                }
                else
                    return null;
            }
            else
                return null;
        }

        [WebMethod]
        public List<Karta> zwrocStol(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    return p.zwrocGre().stol;

                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        //Pokoje
        [WebMethod]
        public List<Pokoj> PobierzPokoje(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                List<Pokoj> pok = Baza.ZwrocPokoje();
                foreach (Pokoj p in pok)
                {
                    if (pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == p.numerPokoju; }) == null)
                    {
                        pokoje.Add(p);
                    }
                }                
                return pokoje;
            }
            return null;
        }

        [WebMethod]
        public Komunikat DolaczDoStolu(byte[] token, Int64 id)
        {
            if (Baza.CzyPoprawny(token))
            {                
                Pokoj pok = pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == id && c.iloscGraczyMax >= c.user.Count; });
                Uzytkownik uzyt = Glowny.PobierzUzytkownika(Baza.ZwrocIdUzytkownika(token));   
                if (pok.DodajUzytkownika(uzyt))
                {
                    Baza.ZmienPokoj(token, id);
                    Glowny.ZmienPokoj(uzyt.identyfikatorUzytkownika, id);
                    temp.kodKomunikatu = 200;
                    temp.trescKomunikatu = "ok";
                }
                else
                {
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "Błąd!!! Pokój jest pełny lub już się w nim znajdujesz!";
                }
            }
            else
            {
                temp.kodKomunikatu = 404;
                temp.trescKomunikatu = "Jesteś nie okej!";
            }
            return temp;
        }

        [WebMethod]
        public Komunikat UtworzStol(byte[] token, string nazwa, int stawka, int blind, int ilosc)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 czy = Baza.DodajPokoj(nazwa, stawka, blind, ilosc);
                if (czy > -1)
                {
                    pokoje.Add(Baza.ZwrocPokoj(nazwa));
                    temp.kodKomunikatu = 200;
                    temp.trescKomunikatu = "Pokój został utworzony pomyślnie!";
                }
                else
                {
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "Pokój o takiej nazwie już istnieje!";
                }
            }
            else
            {
                temp.kodKomunikatu = 404;
                temp.trescKomunikatu = "Jesteś nie okej!";
            }

            return temp;
        }

        [WebMethod]
        public Komunikat OpuscStol(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                try
                {
                    Int64 id = Baza.ZwrocIdUzytkownika(token);
                    Pokoj temp2 = pokoje.Find(delegate(Pokoj p) { return p.jestWpokoju(id) == true; });
                    
                    if (pokoje.Find(delegate(Pokoj p) { return p.jestWpokoju(id) == true; }).UsunUzytkownika(id))
                    {
                        Baza.ZmienPokoj(token, 0);
                        Glowny.ZmienPokoj(id, 0);
                        temp.kodKomunikatu = 200;
                        temp.trescKomunikatu = "Pomyślnie opuściłeś pokój!";
                    }
                    else
                    {
                        temp.kodKomunikatu = 404;
                        temp.trescKomunikatu = "Błąd!!! Nie znajdujesz się w tym pokoju!";
                    }
                }
                catch (Exception)
                {
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "Nastąpił nieoczekiwany błąd!";
                }
            }
            else
            {
                temp.kodKomunikatu = 404;
                temp.trescKomunikatu = "Jesteś nie okej!";
            }
            return temp;
        }
    
        //Rozgrywka
        [WebMethod]
        public List<Karta> PobierzKarty(byte[] token) 
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    Gracz s = p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; });
                    return s.zwroc_hand();       
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        public Komunikat Start(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj pokoj = pokoje.Find(delegate(Pokoj c) { return c.jestWpokoju(id) == true; });
                int zm = pokoj.IleStart(id);
                if (zm == pokoj.iloscGraczyMax)
                {
                    if(pokoj.zwrocGre()==null)
                        pokoj.utworz();
                }
                temp.kodKomunikatu = 200;
                temp.trescKomunikatu = "ok";
            }
            else
            {
                temp.kodKomunikatu = 404;
                temp.trescKomunikatu = "not_ok";
            }
            return temp; 
        }

        [WebMethod]
        public Komunikat Fold(byte[] token)
        {
            if (Baza.CzyPoprawny(token) == true)
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });
                if (p!=null)
                {
                    if (p.zwrocGre().czyjRuch == id)
                    {
                        if (p.zwrocGre().aktywni.FindIndex(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }) >= 0)
                        {
                            p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }).stan = Gracz.StanGracza.Fold;
                            p.zwrocGre().aktualizujListeUser();// aktualizowanie na liscie userow stanu gracza    
                            p.zwrocGre().KoniecRuchu();
                            temp.kodKomunikatu = 200;
                        }
                    }
                    else
                    {
                        temp.kodKomunikatu = 404;
                    }

                }
            }
            return temp;
        }

        [WebMethod]
        public Komunikat CallRiseAllIn(byte[] token, Int32 ile)
        {
            bool pom = false;
            if (Baza.CzyPoprawny(token) == true)
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj p = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });

                if(p!=null)
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
                                p.zwrocGre().najwyzszaStawka = R.stawia + ile;//dodane niedawno
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
                                }
                            }
                            if (pom == true)
                            {
                                R.kasa = R.kasa - ile;
                                if (R.kasa == 0)
                                    R.stan = Gracz.StanGracza.AllIn;
                                p.zwrocGre().pula += ile;
                                R.stawia += ile;
                                temp.kodKomunikatu = 200;
                                temp=p.zwrocGre().KoniecRuchu();
                            }
                        }
                        else
                        {
                            temp.kodKomunikatu = 404;
                        }

                    }
                    else
                    {
                        temp.kodKomunikatu = 404;
                    }
                }
                else
                {
                    temp.kodKomunikatu = 404;
                }
            }
            return temp;
        }

        [WebMethod]
        public Gra ZwrocGre(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                Pokoj pok = pokoje.Find(delegate(Pokoj v) { return v.jestWpokoju(id); });

                if (pok != null)
                    return pok.zwrocGre();
                else
                    return null;                             
            }
            else
            {
                return null;
            }
        }

        static public void WyrzucUzytkownikowKtorzyPrzegrali(Gra gra)
        {
            Pokoj pokoik = pokoje.Find(delegate(Pokoj v) { return v.zwrocGre() == gra; });
            if (pokoik != null)
            {

                pokoik.user.RemoveAll(delegate(Uzytkownik c)
                {
                    return pokoik.zwrocGre().user.Find(delegate(Gracz v)
                    {
                        return v.identyfikatorUzytkownika == c.identyfikatorUzytkownika;
                    }) == null;
                });
            }

        }

    }
}
