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
        private Komunikat temp = new Komunikat();
        private List<Wiadomosc> wiadomosci = new List<Wiadomosc>();
        private List<Uzytkownik> uzytkownicy = new List<Uzytkownik>();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //Logowanie
        [WebMethod]
        public Komunikat Zarejestruj(string nazwa, string haslo, string email)
        {
            return temp;
        }
        [WebMethod]
        public Komunikat Zaloguj(string nazwa, string haslo)
        {
            return temp;
        }
        [WebMethod]
        public Komunikat ZmienHaslo(string token, string haslo)
        {
            return temp;
        }

        //Chat
        [WebMethod]
        public List<Uzytkownik> PobierzUzytkownikow(string token)
        {
            return uzytkownicy;
        }
        [WebMethod]
        public List<Wiadomosc> PobierzWiadomosci(string token)
        {
            return wiadomosci;
        }
        [WebMethod]
        public Komunikat WyslijWiadomosc(string token, Wiadomosc wiadomosc)
        {
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
