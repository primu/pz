using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace MainServer
{
    static public class Baza
    {
        //początkowa konfiguracja użytkownika
        static private double KasaStandard = 1500.0;
        static private double CzasAFK = 15;// liczba minut po których nastąpi wylogowanie!

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
        static public void PrzedluzToken(string token)
        {
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Sesja where Token like @Token";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.SelectCommand.Parameters.Add("@Token", SqlDbType.Text).Value = token;
                dataAdapter.Fill(dataSet, "Sesja");
                
                DataRow newRow = dataSet.Tables["Sesja"].Rows[0]; //błąd przy ponownym logowaniu, gdy zostanie podany niepoprawny token
                                                                    // lub token z bazy nie jest kompatybilny typem string ;/ ????????
                newRow["WaznyDo"] = (Int32)(DateTime.Now.AddMinutes(CzasAFK).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Sesja"]);
            }
        }
        static public bool CzyPoprawny(string token)
        {
            bool zalogowany = false;
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                connection.Open();
                var sqlQuery = "select Max(WaznyDo) from Sesja where Token like @Token";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);
                DataSet dataSet = new DataSet();
                dataAdapter.SelectCommand.Parameters.Add("@Token", SqlDbType.Text).Value = token;
                dataAdapter.Fill(dataSet,"Sesja");
                int ok = (int)dataSet.Tables["Sesja"].Rows[0].ItemArray.GetValue(0);
                if (ok > (Int32)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)
                {
                    zalogowany = true;
                }
                else
                {
                    zalogowany = false;
                }
            }
            return zalogowany;
        }
        static public string CzyZalogowany(string nazwa)
        {
            string token = null;
            using (SqlConnection connection = new SqlConnection(CiagPolaczenia))
            {
                
                connection.Open();
                var sqlQuery = "select Max(s.WaznyDo),s.Token from Sesja s join Uzytkownik u on s.Uzytkownik=u.UzytkownikID where u.Nazwa like @Nazwa group by s.Token";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, connection);

                DataSet dataSet = new DataSet();
                dataAdapter.SelectCommand.Parameters.Add("@Nazwa", SqlDbType.NVarChar).Value = nazwa;
                dataAdapter.Fill(dataSet, "Sesja");
                if (dataSet.Tables["Sesja"].Rows.Count > 0)
                {
                    int ok = (int)dataSet.Tables["Sesja"].Rows[0].ItemArray.GetValue(0);
                    if (ok > (Int32)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)
                    {
                        token = dataSet.Tables["Sesja"].Rows[0].ItemArray.GetValue(1).ToString();
                    }
                }
                
            }
            return token;
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
                using (var deriveBytes = new Rfc2898DeriveBytes(haslo, 32))
                {
                    byte[] salt = deriveBytes.Salt;
                    byte[] key = deriveBytes.GetBytes(32);  // derive a 20-byte key

                    newRow["Haslo"] = key;//mySHA256.ComputeHash(
                    newRow["Salt"] = salt;//mySHA256.ComputeHash(
                }
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
        static public bool CzyPoprawneHaslo(string haslo, string nazwa)
        {
            byte[] salt, key;

            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select Haslo, Salt from Uzytkownik where Nazwa = @Nazwa";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();

                dataAdapter.SelectCommand.Parameters.Add("@Nazwa", SqlDbType.NVarChar).Value = nazwa;
                dataAdapter.Fill(dataSet, "Uzytkownik");
                key = (byte[])dataSet.Tables["Uzytkownik"].Rows[0]["Haslo"];
                salt = (byte[])dataSet.Tables["Uzytkownik"].Rows[0]["Salt"];
            }

            using (var deriveBytes = new Rfc2898DeriveBytes(haslo, salt))
            {
                byte[] newKey = deriveBytes.GetBytes(32);

                if (!newKey.SequenceEqual(key))
                    return false;
                else
                    return true;
            }
        }
        static public Komunikat Zaloguj(string nazwa)
        {
            Komunikat kom = new Komunikat();
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Sesja";
                var sqlQuery2 = "select UzytkownikID from Uzytkownik where Nazwa = @Nazwa";// +wiad.nazwaUzytkownika;
                SqlDataAdapter dataAdapter2 = new SqlDataAdapter(sqlQuery2, Polaczenie);
                DataSet dataSet2 = new DataSet();

                dataAdapter2.SelectCommand.Parameters.Add("@Nazwa", SqlDbType.NVarChar).Value = nazwa;
                dataAdapter2.Fill(dataSet2, "Uzytkownik");
                int uzytID = (int)dataSet2.Tables["Uzytkownik"].Rows[0].ItemArray.GetValue(0);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "Sesja");

                string token = Key.Generuj();
                DataRow newRow = dataSet.Tables["Sesja"].NewRow();
                newRow["Uzytkownik"] = uzytID;
                newRow["Token"] = token;
                newRow["WaznyDo"] = (Int32)(DateTime.Now.AddMinutes(CzasAFK).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
               
                dataSet.Tables["Sesja"].Rows.Add(newRow);

                new SqlCommandBuilder(dataAdapter);
                dataAdapter.Update(dataSet.Tables["Sesja"]);

                kom.trescKomunikatu = token;
                kom.kodKomunikatu = 100;
                return kom;
            }
        }
        //static public Komunikat Wyloguj(string token)
        //{
        //    string nazwa;
        //    using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
        //    {
        //        var sqlQuery = "select Nazwa from Uzytkownik u join Sesja s on u.UzytkownikID=s.Uzytkownik where s.Token = @Token";
        //        SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
        //        DataSet dataSet = new DataSet();

        //        dataAdapter.SelectCommand.Parameters.Add("@Token", SqlDbType.NVarChar).Value = token;
        //        dataAdapter.Fill(dataSet, "Uzytkownik");
        //        nazwa = dataSet.Tables["Uzytkownik"].Rows[0].ItemArray.GetValue(0).ToString();
                
        //    }
        //    return Baza.Wyloguj(nazwa);
        //}
        static public Komunikat Wyloguj(string token)
        {
            Komunikat kom = new Komunikat();
            using (SqlConnection Polaczenie = new SqlConnection(CiagPolaczenia))
            {
                var sqlQuery = "select * from Sesja where Token like @Token and WaznyDo >= @Wazny";
              
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, Polaczenie);
                DataSet dataSet = new DataSet();
                dataAdapter.SelectCommand.Parameters.Add("@Token", SqlDbType.Text).Value = token;
                dataAdapter.SelectCommand.Parameters.Add("@Wazny", SqlDbType.Int).Value = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dataAdapter.Fill(dataSet, "Sesja");
                if (dataSet.Tables["Sesja"].Rows.Count > 0)
                {

                    foreach (DataRow row in dataSet.Tables["Sesja"].Rows)
                    {
                        row["WaznyDo"] = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    }

                    //DataRow newRow = dataSet.Tables["Sesja"].Rows[0];
                    //newRow["Token"] = Key.Generuj();
                    //newRow["WaznyDo"] = (Int32)(DateTime.Now.AddMinutes(CzasAFK).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    //dataSet.Tables["Sesja"].Rows.Add(newRow);

                    new SqlCommandBuilder(dataAdapter);
                    dataAdapter.Update(dataSet.Tables["Sesja"]);
                }
                    kom.trescKomunikatu = "OK";
                    kom.kodKomunikatu = 100;

            }
            return kom;
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
