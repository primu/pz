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
      
        //[WebMethod]
        //public int gen()
        //{         
        //    ukl.generujKarty();


        //    return ukl.czyPara(); 
        //}
        [WebMethod]
        public Gracz PobierzGracza(byte[] token,Int64 mojID)
        {
            if (Baza.CzyPoprawny(token))
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(id))
                    {
                        return p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == mojID; });
                    }
                    else
                        return null;
                }
            }
            else
                return null;
            return null;
        }


        [WebMethod]
        public List<Karta> zwrocStol(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(id))
                    {
                        return p.zwrocGre().stol;//aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }).zwroc_hand();

                    }
                    else
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return null;
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
            //return pokoje;
            //List<Pokoj> a = new List<Pokoj>();
            //a.Add(Baza.ZwrocPokoj(token));
            //return a; 
        }

        [WebMethod]
        public Komunikat DolaczDoStolu(byte[] token, Int64 id)
        {
            if (Baza.CzyPoprawny(token))
            {
                Baza.ZmienPokoj(token, id);
                pokoje.Find(delegate(Pokoj c) { return c.numerPokoju == id && c.iloscGraczyMax >= c.user.Count; }).DodajUzytkownika(Glowny.PobierzUzytkownika(Baza.ZwrocIdUzytkownika(token)));
                //Baza.ZmienPokoj(Baza.CzyZalogowany(token), Baza.DodajPokoj("asd5", 1040, 233, 48));
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
        public Komunikat UtworzStol(byte[] token, string nazwa, int stawka, int blind, int ilosc)
        {
            if (Baza.CzyPoprawny(token))
            {
                Baza.DodajPokoj(nazwa, stawka, blind, ilosc);

                pokoje.Add(Baza.ZwrocPokoj(nazwa));

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
        public Komunikat OpuscStol(byte[] token)// NIE ZROBIONE 
        {
            return temp;
        }
    
        //Rozgrywka
        [WebMethod]
        public List<Karta> PobierzKarty(byte[] token) 
        {
            if (Baza.CzyPoprawny(token))
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(id))
                    {
                        Gracz s = p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; });
                        //s.czyPobralKarty = true;
                        ////List<Karta> t = p.zwrocGre().aktywni.Find(delegate(Gracz c){return c.identyfikatorUzytkownika==id;}).hand;
                        //if (p.zwrocGre().czyWszyscyPobraliKarty() == true)                           
                        //    p.zwrocGre().NastepnyStan();
                        return s.zwroc_hand();
                       
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return null;
        }

        [WebMethod]
        public Komunikat Start(byte[] token)//do skończenia? 
        {
            if (Baza.CzyPoprawny(token))
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                //Uzytkownik u = Glowny.PobierzUzytkownika(id);
                //u.start = true;
                Pokoj pokoj = pokoje.Find(delegate(Pokoj c) { return c.jestWpokoju(id) == true; });
                //int zm = pokoj.user.Count<Uzytkownik>(delegate(Uzytkownik v) { return v.start == true; });
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
                temp.trescKomunikatu = "ok";
            }
            return temp;
            
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
                               
                                //p.zwrocGre().aktywni.Remove(p.zwrocGre().aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; }));//usuwanie gracza ktory folduje
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

        [WebMethod]
        public Gra ZwrocGre(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                int id = Baza.ZwrocIdUzytkownika(token);
                foreach (Pokoj p in pokoje)
                {
                    if (p.jestWpokoju(id))
                    {
                        return p.zwrocGre();
                    }
                    else
                    {
                        return null;
                    }
                }
               
            }
            else
            {
                return null;
            }

            return null;
        }


        [WebMethod]
        public void CzyscStoly()
        {
            foreach (Pokoj p in pokoje)
            {
                p.user.Clear();
                

            }

        }


    }
}
