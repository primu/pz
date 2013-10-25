using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

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

        private string connection = "Data Source=(local);"
               + "Initial Catalog=PokerDB;"
               + "Persist Security Info=False;"
               + "User ID=PokerDBUser;Password=zaq478mlp";

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
        [WebMethod]
        public string TestujPolaczenieZBaza()
        {
            SqlConnection conn = 
            new SqlConnection();
 
            conn.Open();
 
            // create a SqlCommand object for this connection
            SqlCommand command = conn.CreateCommand();
            command.CommandText = "Select * from Uzytkownik";
            command.CommandType = CommandType.Text;
 
            // execute the command that returns a SqlDataReader
            SqlDataReader reader = command.ExecuteReader();
 
            // display the results
            while (reader.Read()) 
            {
                string output = reader["Nazwa"].ToString();
                //Console.WriteLine(output);
            }
 
            // close the connection
            reader.Close();
            conn.Close();
            return "OK";
        }


    }
}
