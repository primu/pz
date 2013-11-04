using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace MainServer
{
    static public class Baza
    {
        static private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=PokerDB;"
               + "Persist Security Info=False;"
               + "User ID=PokerDBUser;Password=zaq478mlp";
        static public bool CzyIstniejeUzytkownik(string nazwa)
        {
            bool istnieje = false;
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from Uzytkownik where Nazwa = @Nazwa", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Nazwa";
                param.Value = nazwa;

                cmd.Parameters.Add(param);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    istnieje = true;
                }
                else
                {
                    istnieje = false;
                }
                connection.Close();

            }
            return istnieje;
        }
        static public bool CzyIstniejEmail(string email)
        {
            bool istnieje = false;
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("select count(*) from Uzytkownik where Email = @Email", connection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Email";
                param.Value = email.ToLower();

                cmd.Parameters.Add(param);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    istnieje = true;
                }
                else
                {
                    istnieje = false;
                }
                connection.Close();
            }
            return istnieje;
        }
        static public Komunikat DodajUzytkownika(string nazwa, string email, string haslo)
        {
            Komunikat kom = new Komunikat();
            if (!CzyPoprawnyEmail(email))
            {
                kom.trescKomunikatu = "NIEPOPRAWNY FORMAT";
                kom.kodKomunikatu = 110;
                return kom;
            }
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Uzytkownik";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Uzytkownik");

                DataRow newRow = dataSet.Tables["Uzytkownik"].NewRow();
                newRow["Nazwa"] = nazwa;
                newRow["Email"] = email.ToLower();
                newRow["Haslo"] = haslo;
                newRow["Kasa"] = KasaStandard;
                newRow["Zarejestrowany"] = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dataSet.Tables["Uzytkownik"].Rows.Add(newRow);

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Uzytkownik"]);

                kom.trescKomunikatu = "OK";
                kom.kodKomunikatu = 100;
                return kom;
            }
        }
        static public Komunikat DodajWiadomosc(Wiadomosc wiad)
        {
            Komunikat kom = new Komunikat();
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Wiadomosc";
                var sqlQuery2 = "select UzytkownikID from Uzytkownik where Nazwa = @Nazwa";// +wiad.nazwaUzytkownika;
                SqlDataAdapter dataAdapter2 = new SqlDataAdapter(sqlQuery2, Polaczenie);
                DataSet dataSet2 = new DataSet();

                dataAdapter2.SelectCommand.Parameters.Add("@Nazwa", SqlDbType.NVarChar).Value = wiad.nazwaUzytkownika;
                dataAdapter2.Fill(dataSet2, "Uzytkownik");
                string uzytID = dataSet2.Tables["Uzytkownik"].Rows[0].ItemArray.GetValue(0).ToString();
                //string uzytID = (string)dataAdapter2.SelectCommand.ExecuteScalar();
                //string uzytID = dataSet2.Tables["Uzytkownik"].Rows[0].ItemArray[0].ToString();


                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Wiadomosc");



                DataRow newRow = dataSet.Tables["Wiadomosc"].NewRow();
                newRow["Czas"] = wiad.stempelCzasowy;
                newRow["Tresc"] = wiad.trescWiadomosci;
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
        static public bool CzyPoprawnyEmail(string email)
        {
            if (!Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+(?:[a-zA-Z]{2}||com|org|net|edu|gov|mil|biz|info|mobi|name|aero|asia|jobs|museum)\b"))
                return false;
            else
                return true;
        }
    }
}
