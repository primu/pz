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
            Komunikat kom = new Komunikat();
            if (!CzyPoprawnyEmail(email))
            {
                kom.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                kom.kodKomunikatu = 110;
                return kom;
            }
            using(SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Uzytkownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet,"Uzytkownik");

                DataRow newRow = dataSet.Tables["Uzytkownik"].NewRow();
                newRow["Nazwa"] = nazwa;
                newRow["Email"] = email.ToLower();
                newRow["Haslo"] = haslo;
                newRow["Kasa"] =  KasaStandard;
                newRow["Zarejestrowany"] = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dataSet.Tables["Uzytkownik"].Rows.Add(newRow);

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Uzytkownik"]);

                kom.trescKomunikatu = "OK";
                kom.kodKomunikatu = 100;
                return kom;
            }
        }
        private Komunikat DodajWiadomosc(Wiadomosc wiad)
        {
            Komunikat kom = new Komunikat();
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Wiadomosc";
                var sqlQuery2 = "select UzytkownikID from Uzytkownik where Nazwa = @Nazwa";// +wiad.nazwaUzytkownika;
                SqlDataAdapter dataAdapter2 = new SqlDataAdapter(sqlQuery2, Polaczenie);
                DataSet dataSet2 = new DataSet();
                
                dataAdapter2.SelectCommand.Parameters.Add("@Nazwa",SqlDbType.NVarChar).Value=wiad.nazwaUzytkownika;
                dataAdapter2.Fill(dataSet2, "Uzytkownik");
                string uzytID=dataSet2.Tables["Uzytkownik"].Rows[0].ItemArray.GetValue(0).ToString();
                //string uzytID = (string)dataAdapter2.SelectCommand.ExecuteScalar();
                //string uzytID = dataSet2.Tables["Uzytkownik"].Rows[0].ItemArray[0].ToString();


                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Wiadomosc");



                DataRow newRow = dataSet.Tables["Wiadomosc"].NewRow();
                newRow["Czas"] = wiad.stempelCzasowy;
                newRow["Tresc"] =wiad.trescWiadomosci;
                newRow["Uzytkownik"] = uzytID;
                if (wiad.numerPokoju != 0)
                    newRow["Gra"] = wiad.numerPokoju;
                //newRow["Zarejestrowany"] = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dataSet.Tables["Wiadomosc"].Rows.Add(newRow);

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Wiadomosc"]);

                kom.trescKomunikatu = "OK";
                kom.kodKomunikatu = 100;
                return kom;
            }
        }

        [WebMethod]
        public Komunikat SprawdzNazwe(string nazwa)
        {
            Komunikat kom = new Komunikat();
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
                    kom.trescKomunikatu = "ISTNIEJE";
                    kom.kodKomunikatu = 111;
                }
                else
                {
                    kom.trescKomunikatu = "OK";
                    kom.kodKomunikatu = 100;
                }
                connection.Close();
                return kom;
            }
        }
        private bool CzyPoprawnyEmail(string email)
        {
            if (!Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+(?:[a-zA-Z]{2}||com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b"))
                return false;
            else
                return true;
        }
        [WebMethod]
        public Komunikat SprawdzEmail(string email)
        {
            Komunikat kom = new Komunikat();
            if (!CzyPoprawnyEmail(email))
            {
                kom.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                kom.kodKomunikatu = 110;
                return kom;
            }
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from Uzytkownik where Email = @Email", connection);

                kom = new Komunikat();

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Email";
                param.Value = email.ToLower();

                cmd.Parameters.Add(param);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    kom.trescKomunikatu = "ISTNIEJE";
                    kom.kodKomunikatu = 111;
                }
                else
                {
                    kom.trescKomunikatu = "OK";
                    kom.kodKomunikatu = 100;
                }
                connection.Close();
                return kom;
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
            DodajWiadomosc(wiadomosc);
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
