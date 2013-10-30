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
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Glowny : System.Web.Services.WebService
    {
        //Tymczasowe deklaracje;
        static private Komunikat temp = new Komunikat();
        static private List<Wiadomosc> wiadomosci = new List<Wiadomosc>();
        static private List<Uzytkownik> uzytkownicy = new List<Uzytkownik>();

        static private List<Key> klucze = new List<Key>();

        static private int ILOSC = 0;

        static private StatycznaBaza baza = new StatycznaBaza();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //Logowanie
        [WebMethod]
        public Komunikat Zarejestruj(string nazwa, string haslo, string email)
        {
            if (baza.zarejestrowani.FindIndex(delegate(baza_user u) { return u.nazwa == nazwa; }) < 0)
            {
                baza.AddUser(++ILOSC, nazwa, haslo, email);

                temp.kodKomunikatu = 201;
                temp.trescKomunikatu = "Użytkownik " + nazwa + " został poprawnie zarejestrowany.\nAby zagrać zaloguj się.";
            }
            else
            {
                temp.kodKomunikatu = 409;
                temp.trescKomunikatu = "Wykryto konflikt!!! Użytkownik " + nazwa + " już istnieje!!!";
            }
            return temp;
        }

        [WebMethod]
        public Komunikat Zaloguj(string nazwa, string haslo)
        {
            int ktory = baza.zarejestrowani.FindIndex(delegate(baza_user u) { return u.nazwa == nazwa; });

            if (ktory < 0)
            {//niezarejestrowany
                temp.kodKomunikatu = 403;
                temp.trescKomunikatu = "Operacja zabroniona!!! Użytkownik " + nazwa + " nie istnieje!!!";
            }
            else
            {//zarejestrowany

                if (uzytkownicy.FindIndex(delegate(Uzytkownik u) { return u.nazwaUzytkownika == nazwa; }) < 0)
                {//niezalogowany
                    baza_user user = baza.zarejestrowani[ktory];

                    if (user.haslo == haslo)
                    {//poprawne haslo ->logowanie uzytkownika
                        uzytkownicy.Add(new Uzytkownik(user.idUzytkownika, nazwa, 0));
                        Key k = new Key(user.idUzytkownika);
                        klucze.Add(k);
                        temp.kodKomunikatu = 200;
                        temp.trescKomunikatu = k.token;
                    }
                    else
                    {//niepoprawne haslo
                        temp.kodKomunikatu = 666;
                        temp.trescKomunikatu = "Niepoprawne hasło!!!";
                    }
                }
                else
                {//zalogowany
                temp.kodKomunikatu = 403;
                temp.trescKomunikatu = "Operacja zabroniona!!! Użytkownik " + nazwa + " już jest zalogowany!!!";
                }                                           
            }                        
            return temp;
        }

        [WebMethod]
        public Komunikat ZmienHaslo(string token, string haslo)
        {
            int ktory = klucze.FindIndex(delegate(Key u) { return u.token == token; });


            return temp;
        }

        //Chat
        [WebMethod]
        public List<Uzytkownik> PobierzUzytkownikow(string token)
        {
            return uzytkownicy;
        }

        [WebMethod]
        public List<Wiadomosc> PobierzWiadomosci(string token, Int32 timT)
        {
            List<Wiadomosc> wiad = new List<Wiadomosc>();
            wiad.Clear();
            for (int i = 0; i < wiadomosci.Count; i++)
            {
                if (timT < wiadomosci.ElementAt(i).stempelCzasowy)
                {
                    wiad.Add(wiadomosci.ElementAt(i));                   
                }
            }
            return wiad;
        }

        [WebMethod]
        public Komunikat WyslijWiadomosc(string token, Wiadomosc wiadomosc)
        {
            Int32 timer;
            timer = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            wiadomosc.stempelCzasowy = timer;
            wiadomosci.Add(wiadomosc);
            temp.trescKomunikatu = "wyslano";
            return temp;
        }

        //Serwer-Serwer
        [WebMethod]
        public Komunikat ZweryfikujUzytkownika(string token, string nazwa)
        {
            return temp;
        }
        //co dalej?

    }
}
