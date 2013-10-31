using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

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
        //Konfiguracja Użytkownika
        double KasaStandard = 1500.0;
        //Konfiguracja Stołu

        //Tymczasowe deklaracje;
        static private Komunikat temp = new Komunikat();
        static private List<Wiadomosc> wiadomosci = new List<Wiadomosc>();
        static private List<Uzytkownik> uzytkownicy = new List<Uzytkownik>();

        

        private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=PokerDB;"
               + "Persist Security Info=False;"
               + "User ID=PokerDBUser;Password=zaq478mlp";

        [WebMethod]
        public string TestujPolaczenieZBaza()
        {
            string temp = "";
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                Polaczenie.Open();

                // create a SqlCommand object for this connection
                SqlCommand command = Polaczenie.CreateCommand();
                command.CommandText = "Select * from Uzytkownik";
                command.CommandType = CommandType.Text;

                // execute the command that returns a SqlDataReader
                SqlDataReader reader = command.ExecuteReader();

                // display the results
                while (reader.Read())
                {
                    temp = reader["Nazwa"].ToString();
                    
                }

                // close the connection
                reader.Close();
                Polaczenie.Close();
            }
            return "ok" + temp;
        }
        
        private Komunikat DodajUzytkownika(string nazwa, string email, string haslo)
        {
            temp = new Komunikat();
            if (!Regex.IsMatch(email, @"/^(?!(?:(?:\x22?\x5C[\x00-\x7E]\x22?)|(?:\x22?[^\x5C\x22]\x22?)){255,})(?!(?:(?:\x22?\x5C[\x00-\x7E]\x22?)|(?:\x22?[^\x5C\x22]\x22?)){65,}@)(?:(?:[\x21\x23-\x27\x2A\x2B\x2D\x2F-\x39\x3D\x3F\x5E-\x7E]+)|(?:\x22(?:[\x01-\x08\x0B\x0C\x0E-\x1F\x21\x23-\x5B\x5D-\x7F]|(?:\x5C[\x00-\x7F]))*\x22))(?:\.(?:(?:[\x21\x23-\x27\x2A\x2B\x2D\x2F-\x39\x3D\x3F\x5E-\x7E]+)|(?:\x22(?:[\x01-\x08\x0B\x0C\x0E-\x1F\x21\x23-\x5B\x5D-\x7F]|(?:\x5C[\x00-\x7F]))*\x22)))*@(?:(?:(?!.*[^.]{64,})(?:(?:(?:xn--)?[a-z0-9]+(?:-[a-z0-9]+)*\.){1,126}){1,}(?:(?:[a-z][a-z0-9]*)|(?:(?:xn--)[a-z0-9]+))(?:-[a-z0-9]+)*)|(?:\[(?:(?:IPv6:(?:(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){7})|(?:(?!(?:.*[a-f0-9][:\]]){7,})(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,5})?::(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,5})?)))|(?:(?:IPv6:(?:(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){5}:)|(?:(?!(?:.*[a-f0-9]:){5,})(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,3})?::(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,3}:)?)))?(?:(?:25[0-5])|(?:2[0-4][0-9])|(?:1[0-9]{2})|(?:[1-9]?[0-9]))(?:\.(?:(?:25[0-5])|(?:2[0-4][0-9])|(?:1[0-9]{2})|(?:[1-9]?[0-9]))){3}))\]))$/iD"))
            {
                temp.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                temp.kodKomunikatu = 110;
                return temp;
            }
            using(SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Uzytkownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet,"Uzytkownik");

                DataRow newRow = dataSet.Tables["Uzytkownik"].NewRow();
                newRow["Nazwa"] = nazwa;
                newRow["Email"] = email;
                newRow["Haslo"] = haslo;
                newRow["Kasa"] =  KasaStandard;
                newRow["Zarejestrowany"] = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dataSet.Tables["Uzytkownik"].Rows.Add(newRow);

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Uzytkownik"]);

                temp.trescKomunikatu = "OK";
                temp.kodKomunikatu = 100;
                return temp;
            }
        }

        [WebMethod]
        public Komunikat SprawdzNazwe(string nazwa)
        {
            temp = new Komunikat();
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
			    SqlCommand cmd = new SqlCommand("select count(*) from Uzytkownik where Nazwa = @Nazwa", connection);
                
			    SqlParameter param  = new SqlParameter();
			    param.ParameterName = "@Nazwa";
			    param.Value         = nazwa;

			    cmd.Parameters.Add(param);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    temp.trescKomunikatu = "ISTNIEJE";
                    temp.kodKomunikatu = 111;
                }
                else
                {
                    temp.trescKomunikatu = "OK";
                    temp.kodKomunikatu = 100;
                }
                connection.Close();
                return temp;
            }
        }
        [WebMethod]
        public Komunikat SprawdzEmail(string email)
        {
            temp = new Komunikat();
            if (!Regex.IsMatch(email, @"/^(?!(?:(?:\x22?\x5C[\x00-\x7E]\x22?)|(?:\x22?[^\x5C\x22]\x22?)){255,})(?!(?:(?:\x22?\x5C[\x00-\x7E]\x22?)|(?:\x22?[^\x5C\x22]\x22?)){65,}@)(?:(?:[\x21\x23-\x27\x2A\x2B\x2D\x2F-\x39\x3D\x3F\x5E-\x7E]+)|(?:\x22(?:[\x01-\x08\x0B\x0C\x0E-\x1F\x21\x23-\x5B\x5D-\x7F]|(?:\x5C[\x00-\x7F]))*\x22))(?:\.(?:(?:[\x21\x23-\x27\x2A\x2B\x2D\x2F-\x39\x3D\x3F\x5E-\x7E]+)|(?:\x22(?:[\x01-\x08\x0B\x0C\x0E-\x1F\x21\x23-\x5B\x5D-\x7F]|(?:\x5C[\x00-\x7F]))*\x22)))*@(?:(?:(?!.*[^.]{64,})(?:(?:(?:xn--)?[a-z0-9]+(?:-[a-z0-9]+)*\.){1,126}){1,}(?:(?:[a-z][a-z0-9]*)|(?:(?:xn--)[a-z0-9]+))(?:-[a-z0-9]+)*)|(?:\[(?:(?:IPv6:(?:(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){7})|(?:(?!(?:.*[a-f0-9][:\]]){7,})(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,5})?::(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,5})?)))|(?:(?:IPv6:(?:(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){5}:)|(?:(?!(?:.*[a-f0-9]:){5,})(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,3})?::(?:[a-f0-9]{1,4}(?::[a-f0-9]{1,4}){0,3}:)?)))?(?:(?:25[0-5])|(?:2[0-4][0-9])|(?:1[0-9]{2})|(?:[1-9]?[0-9]))(?:\.(?:(?:25[0-5])|(?:2[0-4][0-9])|(?:1[0-9]{2})|(?:[1-9]?[0-9]))){3}))\]))$/iD"))
            {
                temp.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                temp.kodKomunikatu = 110;
                return temp;
            }
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from Uzytkownik where Email = @Email", connection);

                temp = new Komunikat();

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Email";
                param.Value = email.ToLower();

                cmd.Parameters.Add(param);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    temp.trescKomunikatu = "ISTNIEJE";
                    temp.kodKomunikatu = 111;
                }
                else
                {
                    temp.trescKomunikatu = "OK";
                    temp.kodKomunikatu = 100;
                }
                connection.Close();
                return temp;
            }
        }

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

    }
}
