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
        static private List<Uzytkownik> uzytkownicy = new List<Uzytkownik>(); //zalogowani użytkownicy

        static private List<Key> klucze = new List<Key>(); //klucze(id,token) zalogowanych użytkowników
        private string CiagPolaczenia = "Data Source=(local);"
               + "Initial Catalog=PokerDB;"
               + "Persist Security Info=False;"
               + "User ID=PokerDBUser;Password=zaq478mlp";

        double KasaStandard = 1500.0;
        static private StatycznaBaza baza = new StatycznaBaza();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //Logowanie
        [WebMethod]
        public Komunikat Zarejestruj(string nazwa, string haslo, string email)
        {//rejestracja uzytkownika
            if (baza.zarejestrowani.FindIndex(delegate(baza_user u) { return u.nazwa == nazwa; }) < 0)
            {//niezarejestrowana nazwa uzytkownika
                if (baza.zarejestrowani.FindIndex(delegate(baza_user u) { return u.email == email; }) < 0)
                {//niezarejestrowany email
                    baza.AddUser(baza.zarejestrowani.Count + 1, nazwa, haslo, email);
                    temp.kodKomunikatu = 201;
                    temp.trescKomunikatu = "Użytkownik " + nazwa + " został poprawnie zarejestrowany.\nAby zagrać zaloguj się.";
                }
                else
                {//zajety email
                    temp.kodKomunikatu = 409;
                    temp.trescKomunikatu = "Wykryto konflikt!!! Na email " + email + " jest już zarejestrowany gracz!!!";
                }
            }
            else
            {//zajeta nazwa uzytkownika
                temp.kodKomunikatu = 409;
                temp.trescKomunikatu = "Wykryto konflikt!!! Login " + nazwa + " jest zajęty!!!";
            }
            return temp;
        }
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
        private Komunikat DodajWiadomosc(Wiadomosc wiad)
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

        [WebMethod]
        public Komunikat SprawdzNazwe(string nazwa)
        {
            Komunikat kom = new Komunikat();
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
        public Komunikat Zaloguj(string nazwa, string haslo)
        {
            try
            {
                baza_user user = baza.zarejestrowani.Find(delegate(baza_user u) { return u.nazwa == nazwa; });
                if (user == null)
                {//niezarejestrowany
                    temp.kodKomunikatu = 403;
                    temp.trescKomunikatu = "Operacja zabroniona!!! Użytkownik " + nazwa + " nie istnieje!!!";
                }
                else
                {//zarejestrowany

                    if (uzytkownicy.FindIndex(delegate(Uzytkownik u) { return u.nazwaUzytkownika == nazwa; }) < 0)
                    {//niezalogowany                       
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
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n" + e.ToString();
            }
            return temp;
        }

        [WebMethod]
        public Komunikat Wyloguj(string token)
        {
            try
            {
                Key klucz = klucze.Find(delegate(Key u) { return u.token == token; });

                if (klucz == null)
                {//niezalogowany
                    temp.kodKomunikatu = 666;
                    temp.trescKomunikatu = "Uzytkownik nie jest zalogowany!!!";
                }
                else
                {//zalogowany                    
                    Uzytkownik user = uzytkownicy.Find(delegate(Uzytkownik u)
                    {
                        return u.identyfikatorUzytkownika == klucz.identyfikatorUzytkownika;
                    });
                    uzytkownicy.Remove(user);
                    klucze.Remove(klucz);
                    temp.kodKomunikatu = 200;
                    temp.trescKomunikatu = "Nastąpiło wylogowanie!!!";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n" + e.ToString();
            }
            return temp;
        }

        [WebMethod]
        public Komunikat ZmienHaslo(string token, string haslo)
        {
            try
            {
                Key klucz = klucze.Find(delegate(Key u) { return u.token == token; });
                if (klucz == null)
                {//nieprawidlowy token
                    temp.kodKomunikatu = 666;
                    temp.trescKomunikatu = "Nie powiodło się!!! Nieprawidłowy token!!!";
                }
                else
                {//prawidlowy token -> user zalogowany                    
                    baza_user user = baza.zarejestrowani.Find(delegate(baza_user u)
                    {
                        return u.idUzytkownika == klucz.identyfikatorUzytkownika;
                    });

                    baza.zarejestrowani.Remove(user);
                    user.haslo = haslo;
                    baza.zarejestrowani.Add(user);

                    temp.kodKomunikatu = 200;
                    temp.trescKomunikatu = "Hasło zostało zmienione!";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n" + e.ToString();
            }
            return temp;
        }

        [WebMethod]
        public Komunikat ResetujHaslo(string email)
        {
            try
            {
                baza_user user = baza.zarejestrowani.Find(delegate(baza_user u) { return u.email == email; });

                if (user == null)
                {//brak zarejestrowanego uzytkownika na podany email
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "Wykryto konflikt!!! Brak adresu email " + email + " w bazie!!!";
                }
                else
                {//email wystepuje w bazie                    
                    if (uzytkownicy.FindIndex(delegate(Uzytkownik u) { return u.identyfikatorUzytkownika == user.idUzytkownika; }) < 0)
                    {//błąd, użytkownik jest zalogowany
                        temp.kodKomunikatu = 404;
                        temp.trescKomunikatu = "Wykryto konflikt!!! Użytkownik jest zalogowany.";
                    }
                    else
                    {//użytkownik niezalogowany -> generacja nowego hasła
                        string noweHaslo = new Key(0).token;
                        baza.zarejestrowani.Remove(user);
                        user.haslo = noweHaslo;
                        baza.zarejestrowani.Add(user);

                        // WYSŁANIE EMAIL'a Z NOWYM HASŁEM

                        System.Net.Mail.MailMessage text = new System.Net.Mail.MailMessage(
                            //nadawca
                            "system@poker.pl",
                            //odbiorca
                            email,
                            //temat
                            "Odzyskiwanie hasla",
                            //<treść>
                            "[wiadomość wygenerowana automatycznie, nie odpowiadaj na nią]\n\n" +
                            "Witaj " + user.nazwa + " !!!\n\n" +
                            "Twoje nowe hasło wygenerowane automatycznie to:\n" +
                            noweHaslo +
                            "\n\nMożesz teraz się zalogować do gry Poker!!!"
                            //</treść>
                            );

                        System.Net.Mail.SmtpClient SMTPserwer = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                        SMTPserwer.Credentials = new System.Net.NetworkCredential("pokertxh@gmail.com", "bacillo52");
                        SMTPserwer.EnableSsl = true;

                        SMTPserwer.Send(text);

                        temp.kodKomunikatu = 201;
                        temp.trescKomunikatu = "Mail wysłany!!!";
                    }
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n"+e.ToString();
            }

            return temp;
        }

        [WebMethod]
        public Komunikat UsunKonto(string token)
        {//trzeba byc zalogowanym, aby usunac konto
            try
            {
                Key klucz = klucze.Find(delegate(Key u) { return u.token == token; });
                if (klucz == null)
                {//nieprawidlowy token
                    temp.kodKomunikatu = 666;
                    temp.trescKomunikatu = "Nie powiodło się!!! Nieprawidłowy token!!!";
                }
                else
                {//prawidlowy token -> user zalogowany
                    baza_user user = baza.zarejestrowani.Find(delegate(baza_user u)
                    {
                        return u.idUzytkownika == klucz.identyfikatorUzytkownika;
                    });

                    baza.zarejestrowani.Remove(user);

                    temp.kodKomunikatu = 200;
                    temp.trescKomunikatu = "Konto zostało usuniętę!!!";
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n" + e.ToString();
            }
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
            try
            {
                Key klucz = klucze.Find(delegate(Key u) { return u.token == token; });
                if (klucz == null)
                {//token nie istnieje
                    temp.kodKomunikatu = 404;
                    temp.trescKomunikatu = "Token nie został znaleziony!!!";
                }
                else
                {//token istnieje
                    Uzytkownik user = uzytkownicy.Find(delegate(Uzytkownik u) { return u.nazwaUzytkownika == nazwa; });
                    if (user == null)
                    {//nazwa użytkownika nie istnieje
                        temp.kodKomunikatu = 404;
                        temp.trescKomunikatu = "Nazwa użytkownika nie została znaleziona!!!";
                    }
                    else
                    {//nazwa użytkownika istnieje
                        temp.kodKomunikatu = 201;
                        temp.trescKomunikatu = "Nazwa użytkownika została znaleziona!!!";
                    }
                }
            }
            catch (Exception e)
            {
                temp.kodKomunikatu = 666;
                temp.trescKomunikatu = "Fatal error!!!\n" + e.ToString();
            }
            return temp;
        }
        //co dalej?

    }
}
