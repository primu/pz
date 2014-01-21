using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;


namespace MainServer
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class Glowny : System.Web.Services.WebService
    {
        //Tymczasowe deklaracje;
        static private Komunikat temp = new Komunikat();
        static private List<Wiadomosc> wiadomosci = new List<Wiadomosc>();
        static private List<Uzytkownik> uzytkownicy = new List<Uzytkownik>(); //zalogowani użytkownicy

        //Logowanie
        [WebMethod]
        public List<Uzytkownik> ZwrocZalogowanych()
        {
            return Baza.ZwrocUzytkownikowZalogowanych();
        }

        [WebMethod]
        public Komunikat Zarejestruj(string nazwa, string haslo, string email)
        {//rejestracja uzytkownika
            Komunikat kom = new Komunikat();
            if (Baza.CzyIstniejeUzytkownik(nazwa))
            {
                kom.kodKomunikatu = 111;
                kom.trescKomunikatu = "NAZWA ZAJĘTA!";
            }
            else
            {
                if(!Baza.CzyPoprawnyEmail(email))
                {
                    kom.trescKomunikatu="NIEPOPRAWNY FORMAT ADRESU EMAIL!";
                    return kom;
                }

                if (Baza.CzyIstniejEmail(email))
                {
                    kom.kodKomunikatu = 111;
                    kom.trescKomunikatu = "EMAIL ZAJĘTY!";
                }
                else
                {
                    if (Baza.DodajUzytkownika(nazwa, email, haslo))
                    {
                        kom.trescKomunikatu = "OK";
                        kom.kodKomunikatu = 200;
                    }
                    else
                    {
                        kom.trescKomunikatu = "Blad";
                        kom.kodKomunikatu = 404;
                    }
                }
            }
            return kom;
        }
        
        [WebMethod]
        public Komunikat SprawdzNazwe(string nazwa)
        {
            Komunikat kom = new Komunikat();
            if (Baza.CzyIstniejeUzytkownik(nazwa))
            {
                kom.trescKomunikatu = "ISTNIEJE";
                kom.kodKomunikatu = 111;
            }
            else
            {
                kom.trescKomunikatu = "OK";
                kom.kodKomunikatu = 100;
            }
            return kom;
        }
        
        [WebMethod]
        public Komunikat SprawdzEmail(string email)
        {
            Komunikat kom = new Komunikat();
            if (!Baza.CzyPoprawnyEmail(email))
            {
                kom.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                kom.kodKomunikatu = 110;
                return kom;
            }
            if (Baza.CzyIstniejEmail(email))
            {
                kom.trescKomunikatu = "ISTNIEJE";
                kom.kodKomunikatu = 111;
            }
            else
            {
                kom.trescKomunikatu = "OK";
                kom.kodKomunikatu = 100;
            }
            return kom;
        }

        [WebMethod]
        public byte[] Zaloguj(string nazwa, string haslo)
        {
            byte[] token = null;
            Komunikat kom = new Komunikat();
            if (!Baza.CzyIstniejeUzytkownik(nazwa))
            {
                kom.trescKomunikatu = "NIE ISTNIEJE";
            }
            else
            {
                if(!Baza.CzyPoprawneHaslo(haslo,nazwa))
                {
                    kom.trescKomunikatu = "NIEPOPRAWNE DANE LOGOWANIA";
                }
                else
                {
                    byte[] temp = Baza.CzyZalogowany(nazwa);
                    if (temp == null)
                    {
                        token = Baza.Zaloguj(nazwa);
                    }
                    else
                    {
                        Baza.Wyloguj(temp);
                        token = Baza.Zaloguj(nazwa);
                    }
                    uzytkownicy = Baza.ZwrocUzytkownikowZalogowanych();
                }
            }

            return token;
        }
       
        [WebMethod]
        public Komunikat Wyloguj(byte[] token)
        {
            Komunikat kom = new Komunikat();
            if (Baza.CzyPoprawny(token))
            {
                kom = Baza.Wyloguj(token);
                uzytkownicy = Baza.ZwrocUzytkownikowZalogowanych();
            }
            else
            {
                kom.kodKomunikatu = 404;
                kom.trescKomunikatu = "BLAD";
            }
            return kom;
        }

        [WebMethod]
        public Komunikat PobierzSwojeID(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                Int64 id = Baza.ZwrocIdUzytkownika(token);
                temp.kodKomunikatu = 200;
                temp.trescKomunikatu = id.ToString();
            }
            else
            {
                temp.kodKomunikatu = 404;
                temp.trescKomunikatu = "BLAD";
            }    
            return temp;
        }

        //Chat
        [WebMethod]
        public List<Uzytkownik> PobierzUzytkownikow(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                return uzytkownicy;
            }
            else
                return null;
        }

        [WebMethod]
        public List<Wiadomosc> PobierzWiadomosci(byte[] token, Int32 timT, Int64 pokoj)
        {
            if (Baza.CzyPoprawny(token))
            {
                List<Wiadomosc> wiad = new List<Wiadomosc>();
                wiad.Clear();
                for (int i = 0; i < wiadomosci.Count; i++)
                {
                    if (wiadomosci[i].numerPokoju == pokoj)
                    {
                        if (timT < wiadomosci.ElementAt(i).stempelCzasowy)
                        {
                            wiad.Add(wiadomosci.ElementAt(i));
                        }
                    }
                }
                return wiad;
            }
            else
                return null;
        }

        [WebMethod]
        public Komunikat WyslijWiadomosc(byte[] token, Wiadomosc wiadomosc)
        {
            temp.kodKomunikatu = 404;
            temp.trescKomunikatu = "Błąd uwierzytelniania!";
            if (Baza.CzyPoprawny(token))
            {
                Int32 timer;
                timer = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                wiadomosc.stempelCzasowy = timer;
                wiadomosci.Add(wiadomosc);
                temp.trescKomunikatu = "wyslano";
                temp.kodKomunikatu = 200;
            }
            return temp;
        }

        //serwer-serwer
        [WebMethod]
        static public Uzytkownik PobierzUzytkownika(Int64 id)
        {
            return uzytkownicy.Find(delegate(Uzytkownik c) { return c.identyfikatorUzytkownika == id; });
        }

        static public void ZmienPokoj(Int64 idUzytkownika, Int64 idPokoju)
        {
            Uzytkownik k = uzytkownicy.Find(delegate(Uzytkownik u){return u.identyfikatorUzytkownika == idUzytkownika;});
            k.numerPokoju = idPokoju;
            k.start = false;

        }

        [WebMethod]
        public List<Uzytkownik> PobierzListeNajlepszych(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                return Baza.ZwrocNajlepszych();
            }
            else
                return null;
        }

        [WebMethod]
        public List<Uzytkownik> PobierzListeNajbogatszych(byte[] token)
        {
            if (Baza.CzyPoprawny(token))
            {
                return Baza.ZwrocNajbogatszych();
            }
            else
                return null;
        }

    }
}
