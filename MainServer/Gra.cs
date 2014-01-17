using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Gra
    {
        public enum Stan : int { PREFLOP, FLOP, TURN, RIVER, SHOWDOWN, STARTING, END};
        public List<Gracz> user= new List<Gracz>();
        public List<Gracz> aktywni = new List<Gracz>();

        public Stan stan=Stan.PREFLOP;   // obecny stan gry

        public Int64 ktoBigBlind = 0;   // id gracza, który jest w obecnym rozdaniu na BigBlind
        public Int64 ktoStawia; // id gracza, który stawia
        public Int64 czyjRuch;  // id gracza, który ma wykonać teraz ruch
        public Int64 najwyzszaStawka;  //ile wynosi najwyższa stawka
        public Int64 pula;  //wartość stołu       
        public Int64 duzyBlind;
        public List<Karta> stol = new List<Karta>();
        //double start, stop;
        private int licznik = 0;

        private static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
        private static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };
        private List<Karta> talia = new List<Karta>();
//        private UkladyKart ukl = new UkladyKart();
        public List<Gracz> listaWin;
        public Int64 ktoDealer;
        public bool wyniki = false;

        public Gra() { }
        public Gra(Int64 duzyBlind, List<Uzytkownik> u, Int64 stawkaWejsciowa)
        {
            this.duzyBlind = duzyBlind;
            najwyzszaStawka = duzyBlind;
            foreach (Uzytkownik q in u)
            {
               // q.kasiora -= stawkaWejsciowa;
                user.Add(new Gracz(q,stawkaWejsciowa));
            }
        }
      

        //Zwrócmy uwagę na metody!!!
        public bool WszyscyGotowi() // gdy wszyscy gracze gotowi -> true 
        {
            if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.stan == Gracz.StanGracza.Ready; }) == aktywni.Count)
                return true;
            else
                return false;
        }

        public void StartujGre() // inicjalizuje rozgrywkę, jeśli się ona jeszcze nie rozpoczęła 
        {
            ktoBigBlind = user.ElementAt(0).identyfikatorUzytkownika;
            //ktoStawia = ktoBigBlind;
            //czyjRuch = KtoNastepny(user, ktoBigBlind);
            for (int i = 0; i < user.Count; i++)
            {
                Baza.AktualizujKaseUzytkownika(user[i].identyfikatorUzytkownika, -user[i].kasa);
            }
        }
        //ok
        public void NoweRozdanie() // 
        {
            
            wyniki = false;
            
            pula = 0;
            stol.Clear();
            rozdanie();
            licznik++;
            najwyzszaStawka = duzyBlind * licznik;
            foreach (Gracz a in aktywni)
            {
                a.czyNoweRozdanie = false;
                a.stawia = 0;
                a.stan = Gracz.StanGracza.Ready;
            }


            if (stan == Stan.SHOWDOWN)
            {
                Rozgrywki.WyrzucUzytkownikowKtorzyPrzegrali(this);
                aktywni.RemoveAll(delegate(Gracz y) { return y.kasa == 0; });
                user.RemoveAll(delegate(Gracz y) { return y.kasa == 0; });
            }


            if (KoniecGry())
            {
                ZakonczGre();
            }
            else
            {//czyli nie jest to koniec gry

                //=========================================================================================================================

                stan = Stan.PREFLOP;

                //dealer         
                Gracz v = aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == KtoPoprzedni(aktywni, ktoBigBlind); });
                v.stan = Gracz.StanGracza.Dealer;
                ktoDealer = v.identyfikatorUzytkownika;
                //smallBlind
                Gracz x = aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == ktoBigBlind; });
                if (x.kasa > najwyzszaStawka / 2)
                {
                    x.stan = Gracz.StanGracza.SmallBlind;
                    x.kasa -= najwyzszaStawka / 2;
                    x.stawia = najwyzszaStawka / 2;
                    pula += najwyzszaStawka / 2;
                }
                else
                {
                    x.stan = Gracz.StanGracza.AllIn;
                    pula += x.kasa;
                    x.stawia = x.kasa;
                    x.kasa = 0;
                }
                //BigBlind
                Gracz b = aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == KtoNastepny(aktywni, ktoBigBlind); });
                if (b.kasa > najwyzszaStawka)
                {
                    b.stan = Gracz.StanGracza.BigBlind;
                    b.kasa -= najwyzszaStawka;
                    b.stawia = najwyzszaStawka;
                    pula += najwyzszaStawka;
                }
                else
                {
                    b.stan = Gracz.StanGracza.AllIn;
                    pula += b.kasa;
                    b.stawia = b.kasa;
                    b.kasa = 0;
                }

                //ustawienia poczatkowe
                ktoBigBlind = b.identyfikatorUzytkownika;//KtoNastepny(ktoBigBlind);          
                ktoStawia = KtoNastepny(aktywni, ktoBigBlind);
                czyjRuch = KtoNastepny(aktywni, ktoBigBlind);//ktoStawia;
            }//else - czyli nie jest to koniec gry
        }

        public void ZakonczGre() // under construction 
        {
            Baza.AktualizujKaseUzytkownika(listaWin[0].identyfikatorUzytkownika, listaWin[0].kasa);
            stan = Stan.END;
            czyjRuch = 0;
            pula = 0;
        }

        //ok
        public bool KoniecLicytacji() // gdy wszyscy Call do jednej stawki lub Fold 
        {
            if ((ktoStawia == KtoNastepny(aktywni,czyjRuch)) || (aktywni.Count==1) )
            {
                return true;
            }
            else
                return false;
        }
        //ok
        public bool KoniecRozdania() // gdy wszyscy gracze Fold lub ma być SHOWDOWN 
        {
            if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.stawia == najwyzszaStawka || c.stan == Gracz.StanGracza.AllIn; }) > 1)
            {
                if (stan == Stan.RIVER)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }
        //powiedzmy ok
/**/    public void NastepnyStan() // przejście do następnego stanu gry 
        {
            stan++;
            switch (stan)
            {
                case Stan.FLOP:
                    losujNaStol(3);
                    break;

                case Stan.TURN:
                    losujNaStol(1);
                    break;

                case Stan.RIVER:
                    losujNaStol(1);
                    break;
            }
            //ktoStawia = KtoPoprzedni(aktywni, KtoPoprzedni(aktywni, ktoBigBlind));
            //czyjRuch = ktoStawia;
            ktoStawia = KtoNastepny(aktywni, ktoDealer);
            //nowe
            czyjRuch = ktoDealer;

            Gracz x;
            int controla = aktywni.Count;
            do
            {
                czyjRuch = KtoNastepny(aktywni, czyjRuch);
                x = aktywni.Find(delegate(Gracz v) { return v.identyfikatorUzytkownika == czyjRuch && (v.stan == Gracz.StanGracza.Fold || v.stan == Gracz.StanGracza.AllIn); });
                controla--;
            } while (x != null && controla > 0);

            if (controla == 0)//wszyscy w grze są All-In
            {
                czyjRuch = -1;
                if (stan < Stan.RIVER)
                    NastepnyStan();
                else
                {
                    ZakonczenieRozdania();
                    wyniki = true;
                    //if (KoniecGry())
                    //{
                    //    ZakonczGre();
                    //}
                }

            }
        }
        //ok
        public bool KoniecGry() // czy w grze został tylko jeden gracz 
        {
            if (user.Count<Gracz>(delegate(Gracz c) { return c.kasa > 0; }) == 1)
            //if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.stan !=Gracz.StanGracza.Fold; }) == 1)
                return true;
            else
                return false;
        }

        public void aktualizujListeUser()
        {
            foreach (Gracz c in user)
            {
                int ko = aktywni.FindIndex(delegate(Gracz a) { return a.identyfikatorUzytkownika == c.identyfikatorUzytkownika; });
                if (ko >= 0)
                {
                    c.stan = aktywni[ko].stan;
                    c.kasa = aktywni[ko].kasa;
                    c.stawia = aktywni[ko].stawia;
                }

            }
        }
        //ok
        public Komunikat KoniecRuchu() // działania na końcu akcji gracza (Fold, Rise, Call, AllIn) 
        {
            Komunikat k = new Komunikat();
            //nowe
            Gracz x = aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == czyjRuch && c.stan == Gracz.StanGracza.Fold; });
            if (x != null)
            {
                if (ktoStawia == x.identyfikatorUzytkownika)
                    ktoStawia = KtoPoprzedni(aktywni, x.identyfikatorUzytkownika);
                czyjRuch = KtoPoprzedni(aktywni, x.identyfikatorUzytkownika);
                aktywni.Remove(x);//usuwanie gracza ktory folduje

            }
            if (KoniecLicytacji() == true)
            {
                if (KoniecRozdania() == true)
                {
                    ZakonczenieRozdania();
                    wyniki = true;

                    //if (KoniecGry() == true)
                    //    ZakonczGre();
                    //else
                    //{
                    //    k.kodKomunikatu = 213;
                    //    return k;
                    //}


                    //stare
                    //else
                    //{
                    //    NoweRozdanie();
                    //}
                }
                else
                    NastepnyStan();
            }
            else
            //czyjRuch = KtoNastepny(aktywni, czyjRuch);
            //do
            //{
            //    czyjRuch = KtoNastepny(aktywni, czyjRuch);
            //} while (aktywni.Find(delegate(Gracz v) { return v.identyfikatorUzytkownika == czyjRuch && (v.stan == Gracz.StanGracza.Fold || v.stan == Gracz.StanGracza.AllIn); }) != null);
            {
                //ktoStawia = KtoNastepny(aktywni, ktoDealer);
                //nowe
                //czyjRuch = ktoDealer;

                Gracz q;
                int controla = aktywni.Count;
                do
                {
                    czyjRuch = KtoNastepny(aktywni, czyjRuch);
                    q = aktywni.Find(delegate(Gracz v) { return v.identyfikatorUzytkownika == czyjRuch && (v.stan == Gracz.StanGracza.Fold || v.stan == Gracz.StanGracza.AllIn); });
                    controla--;
                } while (q != null && controla > 0);

                if (controla == 0)//wszyscy w grze są All-In
                {
                    czyjRuch = -1;
                    if (stan < Stan.RIVER)
                        NastepnyStan();
                    else
                    {
                        ZakonczenieRozdania();
                        wyniki = true;
                        //if (KoniecGry())
                        //{
                        //    ZakonczGre();
                        //}
                    }

                }

            }

            k.kodKomunikatu = 200;
            return k;
        }
        //chyba ok
        public void ZakonczenieRozdania() // akcja na zakończenie rozdania, przydzielenie zwyciestwa w rozdaniu 
        {
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].stan == Gracz.StanGracza.Fold)
                {
                    aktywni.RemoveAt(i);
                }
            }

            if (aktywni.Count == 1)
                listaWin = new List<Gracz>(aktywni);
            else
                listaWin = new List<Gracz>(ktoWygral());
            //foreach (Gracz a in aktywni)
            for (int i = 0; i < aktywni.Count; i++)//modyfikacja dla błędu folda przy all-in'ach
            {
                Gracz a = aktywni[i];
                if (listaWin.FindIndex(delegate(Gracz c) { return c.identyfikatorUzytkownika == a.identyfikatorUzytkownika; }) >= 0)
                {
                    a.handWin = a.zwroc_hand();
                    a.najUkladWin = a.zwroc_najUklad();
                    a.kasa += pula / listaWin.Count;
                    a.stan = Gracz.StanGracza.Winner;
                }
                else
                {
                    if (a.kasa == 0)
                    {
                        if (ktoBigBlind == a.identyfikatorUzytkownika)
                            ktoBigBlind = KtoNastepny(aktywni, ktoBigBlind);
                        a.czyGra = false;
                    }
                }
            }
            stan = Stan.SHOWDOWN;

            aktualizujListeUser();                

            //aktywni.RemoveAll(delegate(Gracz c) { return c.kasa == 0; });
            //user.RemoveAll(delegate(Gracz c) { return c.kasa == 0; });
            //Rozgrywki.WyrzucUzytkownikowKtorzyPrzegrali(this);
                            
            czyjRuch = -1;
        }

        public bool czyWszyscyPobraliKarty()
        {
            int licz = 0;
            foreach (Gracz g in aktywni)
            {
                if (g.czyPobralKarty ==true)
                {
                    licz++;
                }
            }
            if (licz == aktywni.Count)
                return true;
            else
                return false;
        }

        //================================================================================================================================

        public void rozdanie()
        {
            aktywni = new List<Gracz>(user); ;
            generujKarty();
            foreach (Gracz c in aktywni)
            {
                c.zwroc_hand().Clear();
                c.zwroc_najUklad().Clear();
            }
            stol.Clear();
            Random rnd1 = new Random();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < aktywni.Count; j++)
                {
                    int a = rnd1.Next(0, talia.Count);
                    aktywni[j].zwroc_hand().Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }

            //stawia = ktoBlind;
            //stan = Stan.PREFLOP;        
        }

        public void losujNaStol(int ile)
        {
            if (ile == 3) stol.Clear();
            Random rnd1 = new Random();
            for (int j = 0; j < ile; j++)
            {
                int a = rnd1.Next(0, talia.Count);
                stol.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                talia.RemoveAt(a);
            }
           
        }

        public void generujKarty()
        {
            talia.Clear();
            for (int i = 0; i < 52; i++)
            {
                if (i < 13)
                {
                    talia.Add(new Karta { figura = figury[i], kolor = kolory[0] });
                }
                if (i > 12 && i < 26)
                {
                    talia.Add(new Karta { figura = figury[i - 13], kolor = kolory[1] });
                }
                if (i > 25 && i < 39)
                {
                    talia.Add(new Karta { figura = figury[i - 26], kolor = kolory[2] });
                }
                if (i > 38 && i < 52)
                {
                    talia.Add(new Karta { figura = figury[i - 39], kolor = kolory[3] });
                }
            }
        }
      
        public string gen()
        {
            //pobierzUserow();
            rozdanie();
            losujNaStol(3);
            return "ok";//ukl.co_mamy(stol, user[1].hand,user[1].najUklad); 
        }

        public int wartosci(string kk)
        {
            if (kk == "Poker krolewski!")
                return 9;
            if (kk == "Poker!")
                return 8;
            if (kk == "Kareta!")
                return 7;
            if (kk == "Full!")
                return 6;
            if (kk == "Kolor!")
                return 5;
            if (kk == "Strit!")//poprawic remisy dla 2 kickera, as jako najslabsza karta
                return 4;
            if (kk == "Trojka!")
                return 3;
            if (kk == "Dwie pary!")
                return 2;
            if (kk == "Para!")
                return 1;
            if (kk == "Wysoka karta!")
                return 0;
            return -1;
        }

        public List<Gracz> ktoWygral()
        {
            int max;
            sortujWart sw = new sortujWart();
            List<Gracz> wygrani = new List<Gracz>();
            for (int i = 0; i < aktywni.Count; i++)
            {
                Gracz g = aktywni[i];
                aktywni[i].nazwaUkladu = g.ukl.co_mamy(stol, g);//aktywni[i]);//ukl.co_mamy(stol, aktywni[i].zwroc_hand(), aktywni[i].zwroc_najUklad());
                if (aktywni[i].stan != Gracz.StanGracza.Fold)//zmienione MB
                {
                    aktywni[i].wart = wartosci(aktywni[i].nazwaUkladu);
                }
                aktywni.Sort(sw);
            }
            max = aktywni[0].wart;
            int ile = 0; // ile osob ma najwyzszy uklad
            for (int j = 0; j < aktywni.Count; j++)
            {
                if (aktywni[j].stan != Gracz.StanGracza.Fold)//zmienione MB
                {
                    if (aktywni[j].wart == max)
                    {
                        ile++;
                    }
                }
            }
            if ((ile > 1))
            {
                if (max == 9)//poker krolewski
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                    }
                    return wygrani;
                }
                else if (max == 8)//poker
                {
                    PokerComp(wygrani, ile);
                }
                else if (max == 7)//kareta
                {
                    KaretaComp(wygrani, ile);
                }
                else if (max == 6)//full
                {
                    FullComp(wygrani, ile);
                }
                else if (max == 5)//kolor
                {
                    KolorComp(wygrani, ile);
                }
                else if (max == 4)//strit
                {
                    StritComp(wygrani, ile);
                }
                else if (max == 3)//trójka
                {
                    TrojkaComp(wygrani, ile);
                }
                else if (max == 2)//dwie pary
                {
                    DwieParyComp(wygrani, ile);
                }
                else if (max == 1)//para
                {
                    ParaComp(wygrani, ile);
                }
                else if (max == 0)//wysoka karta ----dopisac koncowke
                {
                    WysokaKartaComp(wygrani, ile);
                }

            }

            if (ile == 1)
                wygrani.Add(aktywni[0]);//new Gracz { nazwaUkladu = aktywni[0].nazwaUkladu, fold = aktywni[0].fold, hand = aktywni[0].hand, identyfikatorUzytkownika = aktywni[0].identyfikatorUzytkownika, kicker = aktywni[0].kicker, najUklad = aktywni[0].najUklad, nazwaUzytkownika = aktywni[0].nazwaUzytkownika, numerPokoju = aktywni[0].numerPokoju, wart = aktywni[0].wart });
            return wygrani;
        }

        private void funkcPom(List<Gracz> wygrani, List<Gracz> user2) //sprawdza kickery w rece
        {
            for (int i = 0; i < user2.Count; i++)
            {
                user2[i].kicker = -1;
                for (int j = 0; j < user2[i].zwroc_hand().Count; j++)
                {
                    if ((int)user2[i].zwroc_hand()[j].figura > user2[i].kicker)
                    {
                        user2[i].kicker = (int)user2[i].zwroc_hand()[j].figura;
                    }
                }
            }
            sortujKick sk = new sortujKick();
            user2.Sort(sk);
            for (int j = 0; j < user2.Count; j++)
            {
                if (user2[j].kicker == user2[0].kicker)
                    wygrani.Add(user2[j]);//new Gracz { nazwaUkladu = user2[j].nazwaUkladu, fold = user2[j].fold, hand = user2[j].hand, identyfikatorUzytkownika = user2[j].identyfikatorUzytkownika, kicker = user2[j].kicker, najUklad = user2[j].najUklad, nazwaUzytkownika = user2[j].nazwaUzytkownika, numerPokoju = user2[j].numerPokoju, wart = user2[j].wart });
            }
            if (wygrani.Count > 1)
            {
                int k1 = wygrani[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].zwroc_hand().Count; j++)
                    {
                        if (((int)wygrani[i].zwroc_hand()[j].figura > wygrani[i].kicker) && ((int)wygrani[i].zwroc_hand()[j].figura != k1))
                        {
                            wygrani[i].kicker = (int)wygrani[i].zwroc_hand()[j].figura;
                        }
                    }
                }
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private List<Gracz> PokerComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            List<Gracz> temp = new List<Gracz>();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = -1;
                for (int j = 0; j < aktywni[i].zwroc_najUklad().Count; j++)
                {
                    if ((int)aktywni[i].zwroc_najUklad()[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[j].figura;
                    }
                }
            }
            for (int i = 0; i < ile; i++)
            {
                temp.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });

            }
            sortujKick sk = new sortujKick();
            temp.Sort(sk);
            int ile2 = 0;
            for (int k = 0; k < temp.Count; k++)
            {
                if (temp[k].kicker == temp[0].kicker)
                    ile2++;
            }
            for (int i = 0; i < ile2; i++)
            {
                wygrani.Add(temp[i]);//new Gracz { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
            }
            if (wygrani.Count > 1)
            {
                wygrani.Clear();
                funkcPom(wygrani, aktywni);
            }
            return wygrani;
        }

        private List<Gracz> KaretaComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            int wartK = -1;//maksymalna wartość karety
            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].zwroc_najUklad()[0].figura > wartK)
                {
                    wartK = (int)aktywni[i].zwroc_najUklad()[0].figura;
                }
            }

            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].zwroc_najUklad()[0].figura == wartK)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[4].figura;
                }
                sortujKick sk = new sortujKick();
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
                if (wygrani.Count > 1)
                {
                    wygrani.Clear();
                    funkcPom(wygrani, aktywni);
                }
                return wygrani;
            }

            return wygrani;
        }

        private List<Gracz> FullComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[0].figura;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[3].figura;
                }
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
                if (wygrani.Count > 1)
                {
                    wygrani.Clear();
                    funkcPom(wygrani, aktywni);
                }
                return wygrani;
            }
            return wygrani;
        }

        private List<Gracz> KolorComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = -1;
                for (int j = 0; j < aktywni[i].zwroc_najUklad().Count; j++)
                {
                    if ((int)aktywni[i].zwroc_najUklad()[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[j].figura;
                    }
                }
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            int temp = wygrani[0].kicker;

            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].zwroc_najUklad().Count; j++)
                    {
                        if (((int)wygrani[i].zwroc_najUklad()[j].figura > wygrani[i].kicker) && ((int)wygrani[i].zwroc_najUklad()[j].figura != temp))
                        {
                            wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[j].figura;
                        }
                    }
                }
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
                if (wygrani.Count > 1)
                {
                    //wygrani.Clear();
                    List<Gracz> temp2 = new List<Gracz>(wygrani);
                    wygrani.Clear();
                    funkcPom(wygrani, temp2);
                }

            }

            return wygrani;
        }

        private List<Gracz> StritComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            List<Gracz> temp = new List<Gracz>();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = -1;
                for (int j = 0; j < aktywni[i].zwroc_najUklad().Count; j++)
                {
                    if ((int)aktywni[i].zwroc_najUklad()[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[j].figura;
                    }
                }
            }
            for (int i = 0; i < ile; i++)
            {
                temp.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
            }
            sortujKick sk = new sortujKick();
            temp.Sort(sk);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].kicker == temp[0].kicker)
                    wygrani.Add(temp[i]);//new Gracz { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
            }
            if (wygrani.Count > 1)
            {
                wygrani.Clear();
                funkcPom(wygrani, aktywni);
            }
            return wygrani;
        }

        private List<Gracz> TrojkaComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            int wartK = -1;//maksymalna wartość trojki
            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].zwroc_najUklad()[0].figura > wartK)
                {
                    wartK = (int)aktywni[i].zwroc_najUklad()[0].figura;
                }
            }
            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].zwroc_najUklad()[0].figura == wartK)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int j = 0; j < wygrani.Count; j++)
                {
                    wygrani[j].kicker = -1;
                    for (int i = 0; i < wygrani[j].zwroc_najUklad().Count; i++)
                    {
                        if (((int)wygrani[j].zwroc_najUklad()[i].figura != (int)wygrani[j].zwroc_najUklad()[0].figura) && ((int)wygrani[j].zwroc_najUklad()[i].figura > wygrani[j].kicker))
                        {
                            wygrani[j].kicker = (int)wygrani[j].zwroc_najUklad()[i].figura;
                        }
                    }
                }
                sortujKick sk = new sortujKick();
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
                if (wygrani.Count > 1)
                {
                    int k1 = wygrani[0].kicker;
                    for (int j = 0; j < wygrani.Count; j++)
                    {
                        wygrani[j].kicker = -1;
                        for (int i = 0; i < wygrani[j].zwroc_najUklad().Count; i++)
                        {
                            if (((int)wygrani[j].zwroc_najUklad()[i].figura != k1) && ((int)wygrani[j].zwroc_najUklad()[i].figura != (int)wygrani[j].zwroc_najUklad()[0].figura) && ((int)wygrani[j].zwroc_najUklad()[i].figura > wygrani[j].kicker))
                            {
                                wygrani[j].kicker = (int)wygrani[j].zwroc_najUklad()[i].figura;
                            }
                        }
                    }
                    wygrani.Sort(sk);
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        if (wygrani[i].kicker != wygrani[0].kicker)
                        {
                            wygrani.RemoveAt(i);
                            i--;
                        }
                    }
                    if (wygrani.Count > 1)
                    {
                        wygrani.Clear();
                        funkcPom(wygrani, aktywni);
                    }
                    return wygrani;
                }
                return wygrani;
            }
            return wygrani;
        }

        private List<Gracz> DwieParyComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].zwroc_najUklad()[0].figura > (int)aktywni[i].zwroc_najUklad()[2].figura)
                {
                    aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[0].figura;
                }
                else
                    aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[2].figura;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)//szukanie drugiej najwyzszej pary
            {
                int p1 = aktywni[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if ((int)wygrani[i].zwroc_najUklad()[0].figura == p1)
                    {
                        wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[2].figura;
                    }
                    else
                        wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[0].figura;
                }
                wygrani.Sort(sk);

                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }
                if (wygrani.Count > 1)//szukanie kickera
                {
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[4].figura;
                    }
                    wygrani.Sort(sk);
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        if (wygrani[i].kicker != wygrani[0].kicker)
                        {
                            wygrani.RemoveAt(i);
                            i--;
                        }
                    }
                    if (wygrani.Count > 1)
                    {
                        wygrani.Clear();
                        funkcPom(wygrani, aktywni);
                    }
                    return wygrani;
                }
                return wygrani;
            }
            return wygrani;
        } //poprawic w ukladach kart

        private List<Gracz> ParaComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[0].figura; ;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(aktywni[i]);//(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].zwroc_najUklad().Count; j++)
                    {
                        if (((int)wygrani[i].zwroc_najUklad()[j].figura > wygrani[i].kicker) && ((int)wygrani[i].zwroc_najUklad()[j].figura != (int)wygrani[i].zwroc_najUklad()[0].figura))
                        {
                            wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[j].figura;
                        }
                    }
                }
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }

                if (wygrani.Count > 1)
                {
                    int k1 = wygrani[0].kicker;
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        wygrani[i].kicker = -1;
                        for (int j = 0; j < wygrani[i].zwroc_najUklad().Count; j++)
                        {
                            if (((int)wygrani[i].zwroc_najUklad()[j].figura > wygrani[i].kicker) && ((int)wygrani[i].zwroc_najUklad()[j].figura != (int)wygrani[i].zwroc_najUklad()[0].figura) && ((int)wygrani[i].zwroc_najUklad()[j].figura != k1))
                            {
                                wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[j].figura;
                            }
                        }
                    }
                    wygrani.Sort(sk);
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        if (wygrani[i].kicker != wygrani[0].kicker)
                        {
                            wygrani.RemoveAt(i);
                            i--;
                        }
                    }

                    if (wygrani.Count > 1)
                    {
                        int k2 = wygrani[0].kicker;
                        for (int i = 0; i < wygrani.Count; i++)
                        {
                            wygrani[i].kicker = -1;
                            for (int j = 0; j < wygrani[i].zwroc_najUklad().Count; j++)
                            {
                                if (((int)wygrani[i].zwroc_najUklad()[j].figura > wygrani[i].kicker) && ((int)wygrani[i].zwroc_najUklad()[j].figura != (int)wygrani[i].zwroc_najUklad()[0].figura) && ((int)wygrani[i].zwroc_najUklad()[j].figura != k1) && ((int)wygrani[i].zwroc_najUklad()[j].figura != k2))
                                {
                                    wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[j].figura;
                                }
                            }
                        }
                        wygrani.Sort(sk);
                        for (int i = 0; i < wygrani.Count; i++)
                        {
                            if (wygrani[i].kicker != wygrani[0].kicker)
                            {
                                wygrani.RemoveAt(i);
                                i--;
                            }
                        }
                        if (wygrani.Count > 1)
                        {
                            wygrani.Clear();
                            funkcPom(wygrani, aktywni);
                        }
                        return wygrani;
                    }

                    return wygrani;
                }

                return wygrani;
            }

            return wygrani;
        }

        private List<Gracz> WysokaKartaComp(List<Gracz> wygrani, int ile)
        {
            wygrani.Clear();
            for (int i = 0; i < ile; i++)
            {
                aktywni[i].kicker = (int)aktywni[i].zwroc_najUklad()[0].figura; ;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(aktywni[i]);//new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                int k1 = wygrani[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[1].figura;
                }
                wygrani.Sort(sk);
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if (wygrani[i].kicker != wygrani[0].kicker)
                    {
                        wygrani.RemoveAt(i);
                        i--;
                    }
                }

                if (wygrani.Count > 1)
                {
                    int k2 = wygrani[0].kicker;
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        wygrani[i].kicker = (int)wygrani[i].zwroc_najUklad()[2].figura;
                    }
                    wygrani.Sort(sk);
                    for (int i = 0; i < wygrani.Count; i++)
                    {
                        if (wygrani[i].kicker != wygrani[0].kicker)
                        {
                            wygrani.RemoveAt(i);
                            i--;
                        }
                    }
                    if (wygrani.Count > 1)
                    {
                        wygrani.Clear();
                        funkcPom(wygrani, aktywni);
                    }
                    return wygrani;
                }
                return wygrani;
            }
            return wygrani;
        }

        //=======================================================================================================================================

        public Int64 KtoNastepny(List<Gracz> lista, Int64 numer)
        {
            int i = lista.FindIndex(delegate(Gracz a) { return numer == a.identyfikatorUzytkownika; });
            //if(lista[i].)
            if (i == lista.Count - 1)
                return lista[0].identyfikatorUzytkownika;
            else
                return lista[i + 1].identyfikatorUzytkownika;
        }

        public Int64 KtoPoprzedni(List<Gracz> lista, Int64 numer)
        {
            int i = lista.FindIndex(delegate(Gracz a) { return numer == a.identyfikatorUzytkownika; });
            if (i == 0)
                return lista[lista.Count - 1].identyfikatorUzytkownika;
            else
                return lista[i - 1].identyfikatorUzytkownika;
        }

        public string NazwaMojegoUkladu2(Int64 id)
        {
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].identyfikatorUzytkownika == id)
                {
                    Gracz g = aktywni[i];
                    //List<Karta> cc = aktywni[i].zwroc_hand();
                    return g.ukl.co_mamy(stol,g);//ukl.co_mamy(stol, aktywni[i].zwroc_hand(), aktywni[i].zwroc_najUklad());
                }
            }
                return "";
        }

        public List<Karta> MojNajUkl2(Int64 id)
        {
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].identyfikatorUzytkownika == id)
                {
                    Gracz g = aktywni[i];
                    return g.zwroc_najUklad();
                }
            }
            return null;
        }

        //================================================================================================================================
    }
}