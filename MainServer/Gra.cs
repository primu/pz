using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Gra
    {
        public enum Stan : int { PREFLOP, FLOP, TURN, RIVER, SHOWDOWN, STARTING };
        static  public List<Gracz> user = new List<Gracz>();
        public List<Gracz> aktywni = new List<Gracz>();

        public Stan stan;   // obecny stan gry

        public Int64 ktoBigBlind = 0;   // id gracza, który jest w obecnym rozdaniu na BigBlind
        public Int64 ktoStawia; // id gracza, który stawi
        public Int64 czyjRuch;  // id gracza, który ma wykonać teraz ruch
        public Int64 najwyzszaStawka;  //ile wynosi najwyższa stawka
        public Int64 pula;  //wartość stołu       
        public Int64 duzyBlind;
        //public List<Akcja> akcje = new List<Akcja>();
        public List<Karta> stol = new List<Karta>();

        
        private static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
        private static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };        
        private List<Karta> talia = new List<Karta>();
        private UkladyKart ukl = new UkladyKart();

        public Gra(Int64 duzyBlind)
        {
            this.duzyBlind = duzyBlind;
            najwyzszaStawka = duzyBlind;
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
            ktoBigBlind = aktywni.ElementAt(0).identyfikatorUzytkownika;
            najwyzszaStawka=
            //if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.stan == Gracz.StanGracza.Ready; }) == aktywni.Count)
                
        }

        public void NoweRozdanie() // 
        {

            stan = Stan.PREFLOP;
            generujKarty();
            rozdanie();
            aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == KtoPoprzedni(ktoBigBlind); }).stan = Gracz.StanGracza.Dealer;
            Gracz x = aktywni.Find(delegate(Gracz c) { return c.identyfikatorUzytkownika == ktoBigBlind; });
            if (x.kasa > najwyzszaStawka / 2)
            {
                x.stan = Gracz.StanGracza.SmallBlind;
                x.kasa -= najwyzszaStawka / 2;
            }

            ktoBigBlind =KtoNastepny(ktoBigBlind);
            ktoStawia = ktoBigBlind;
            czyjRuch = KtoNastepny(ktoStawia);
        }

        public void ZakonczGre() // 
        {
            
        }

        //ok
        public bool KoniecLicytacji() // gdy wszyscy Call do jednej stawki lub Fold 
        {
            if (ktoStawia == KtoNastepny(czyjRuch))
            {
                return true;
            }
            else
                return false;
        }
        //ok
        public bool KoniecRozdania() // gdy wszyscy gracze Fold lub ma być SHOWDOWN 
        {
            if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.stawia == najwyzszaStawka; }) > 1)
            {
                if (stan == Stan.SHOWDOWN)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public void NastepnyStan() // przejście do następnego stanu gry 
        {

        }
        //ok
        public bool KoniecGry() // czy w grze został tylko jeden gracz 
        {
            if (aktywni.Count<Gracz>(delegate(Gracz c) { return c.kasa > 0; }) == 1)
                return true;
            else
                return false;
        }
        //ok
        public void KoniecRuchu() // działania na końcu akcji gracza (Fold, Rise, Call, AllIn 
        {
            if (KoniecLicytacji() == true)
            {
                if (KoniecRozdania() == true)
                {
                    if (KoniecGry() == true)
                        ZakonczGre();
                    else
                    {
                        ZakonczenieRozdania();
                        NoweRozdanie();
                    }
                }
                else
                    NastepnyStan();
            }
            else
                czyjRuch = KtoNastepny(czyjRuch);
        }
        
        public void ZakonczenieRozdania() // akcja na zakończenie rozdania, przydzielenie zwyciestwa w rozdaniu 
        {

        }
    
//================================================================================================================================
       
        public void rozdanie()
        {
            aktywni = new List<Gracz>(user); ;
            generujKarty();
            Random rnd1 = new Random();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < aktywni.Count; j++)
                {
                    int a = rnd1.Next(0, talia.Count);
                    aktywni[j].hand.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }

            //stawia = ktoBlind;
            //stan = Stan.PREFLOP;

            // przypisywanie kart uzytkowniom w celach testowych
            /*  user[0].hand.Add(new Karta { figura = Karta.figuraKarty.KA, kolor = Karta.kolorKarty.kier });
              user[0].hand.Add(new Karta { figura = Karta.figuraKarty.K7, kolor = Karta.kolorKarty.pik });
              user[1].hand.Add(new Karta { figura = Karta.figuraKarty.K4, kolor = Karta.kolorKarty.pik });
              user[1].hand.Add(new Karta { figura = Karta.figuraKarty.K5, kolor = Karta.kolorKarty.kier });
              user[2].hand.Add(new Karta { figura = Karta.figuraKarty.K8, kolor = Karta.kolorKarty.kier });
              user[2].hand.Add(new Karta { figura = Karta.figuraKarty.KA, kolor = Karta.kolorKarty.karo });*/
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

            //przypisywanie kart na stol w celach testowych
            /* stol.Add(new Karta { figura = Karta.figuraKarty.K2, kolor = Karta.kolorKarty.karo });
             stol.Add(new Karta { figura = Karta.figuraKarty.K6, kolor = Karta.kolorKarty.trefl });
             stol.Add(new Karta { figura = Karta.figuraKarty.KK, kolor = Karta.kolorKarty.pik });
             stol.Add(new Karta { figura = Karta.figuraKarty.K9, kolor = Karta.kolorKarty.kier });
             stol.Add(new Karta { figura = Karta.figuraKarty.K10, kolor = Karta.kolorKarty.karo });*/
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

        private void pobierzUserow()
        {
            user.Clear();
            user.Add(new Gracz { identyfikatorUzytkownika = 1, nazwaUzytkownika = "Pawel", numerPokoju = 1 });
            user.Add(new Gracz { identyfikatorUzytkownika = 2, nazwaUzytkownika = "Bogdan", numerPokoju = 1 });
            user.Add(new Gracz { identyfikatorUzytkownika = 3, nazwaUzytkownika = "Primu", numerPokoju = 1 });
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
                aktywni[i].nazwaUkladu = ukl.co_mamy(stol, aktywni[i].hand, aktywni[i].najUklad);
                if (aktywni[i].fold == false)
                {
                    aktywni[i].wart = wartosci(aktywni[i].nazwaUkladu);
                }
                aktywni.Sort(sw);
            }
            max = aktywni[0].wart;
            int ile = 0; // ile osob ma najwyzszy uklad
            for (int j = 0; j < aktywni.Count; j++)
            {
                if (aktywni[j].fold == false)
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
                        wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
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
                wygrani.Add(new Gracz { nazwaUkladu = aktywni[0].nazwaUkladu, fold = aktywni[0].fold, hand = aktywni[0].hand, identyfikatorUzytkownika = aktywni[0].identyfikatorUzytkownika, kicker = aktywni[0].kicker, najUklad = aktywni[0].najUklad, nazwaUzytkownika = aktywni[0].nazwaUzytkownika, numerPokoju = aktywni[0].numerPokoju, wart = aktywni[0].wart });
            return wygrani;
        }

        private void funkcPom(List<Gracz> wygrani, List<Gracz> user2) //sprawdza kickery w rece
        {
            for (int i = 0; i < user2.Count; i++)
            {
                user2[i].kicker = -1;
                for (int j = 0; j < user2[i].hand.Count; j++)
                {
                    if ((int)user2[i].hand[j].figura > user2[i].kicker)
                    {
                        user2[i].kicker = (int)user2[i].hand[j].figura;
                    }
                }
            }
            sortujKick sk = new sortujKick();
            user2.Sort(sk);
            for (int j = 0; j < user2.Count; j++)
            {
                if (user2[j].kicker == user2[0].kicker)
                    wygrani.Add(new Gracz { nazwaUkladu = user2[j].nazwaUkladu, fold = user2[j].fold, hand = user2[j].hand, identyfikatorUzytkownika = user2[j].identyfikatorUzytkownika, kicker = user2[j].kicker, najUklad = user2[j].najUklad, nazwaUzytkownika = user2[j].nazwaUzytkownika, numerPokoju = user2[j].numerPokoju, wart = user2[j].wart });
            }
            if (wygrani.Count > 1)
            {
                int k1 = wygrani[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].hand.Count; j++)
                    {
                        if (((int)wygrani[i].hand[j].figura > wygrani[i].kicker) && ((int)wygrani[i].hand[j].figura != k1))
                        {
                            wygrani[i].kicker = (int)wygrani[i].hand[j].figura;
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
                for (int j = 0; j < aktywni[i].najUklad.Count; j++)
                {
                    if ((int)aktywni[i].najUklad[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].najUklad[j].figura;
                    }
                }
            }
            for (int i = 0; i < ile; i++)
            {
                temp.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });

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
                wygrani.Add(new Gracz { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
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
                if ((int)aktywni[i].najUklad[0].figura > wartK)
                {
                    wartK = (int)aktywni[i].najUklad[0].figura;
                }
            }

            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].najUklad[0].figura == wartK)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].najUklad[4].figura;
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
                aktywni[i].kicker = (int)aktywni[i].najUklad[0].figura;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].najUklad[3].figura;
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
                for (int j = 0; j < aktywni[i].najUklad.Count; j++)
                {
                    if ((int)aktywni[i].najUklad[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].najUklad[j].figura;
                    }
                }
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            int temp = wygrani[0].kicker;

            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].najUklad.Count; j++)
                    {
                        if (((int)wygrani[i].najUklad[j].figura > wygrani[i].kicker) && ((int)wygrani[i].najUklad[j].figura != temp))
                        {
                            wygrani[i].kicker = (int)wygrani[i].najUklad[j].figura;
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
                for (int j = 0; j < aktywni[i].najUklad.Count; j++)
                {
                    if ((int)aktywni[i].najUklad[j].figura > aktywni[i].kicker)
                    {
                        aktywni[i].kicker = (int)aktywni[i].najUklad[j].figura;
                    }
                }
            }
            for (int i = 0; i < ile; i++)
            {
                temp.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
            }
            sortujKick sk = new sortujKick();
            temp.Sort(sk);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].kicker == temp[0].kicker)
                    wygrani.Add(new Gracz { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
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
                if ((int)aktywni[i].najUklad[0].figura > wartK)
                {
                    wartK = (int)aktywni[i].najUklad[0].figura;
                }
            }
            for (int i = 0; i < ile; i++)
            {
                if ((int)aktywni[i].najUklad[0].figura == wartK)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int j = 0; j < wygrani.Count; j++)
                {
                    wygrani[j].kicker = -1;
                    for (int i = 0; i < wygrani[j].najUklad.Count; i++)
                    {
                        if (((int)wygrani[j].najUklad[i].figura != (int)wygrani[j].najUklad[0].figura) && ((int)wygrani[j].najUklad[i].figura > wygrani[j].kicker))
                        {
                            wygrani[j].kicker = (int)wygrani[j].najUklad[i].figura;
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
                        for (int i = 0; i < wygrani[j].najUklad.Count; i++)
                        {
                            if (((int)wygrani[j].najUklad[i].figura != k1) && ((int)wygrani[j].najUklad[i].figura != (int)wygrani[j].najUklad[0].figura) && ((int)wygrani[j].najUklad[i].figura > wygrani[j].kicker))
                            {
                                wygrani[j].kicker = (int)wygrani[j].najUklad[i].figura;
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
                if ((int)aktywni[i].najUklad[0].figura > (int)aktywni[i].najUklad[2].figura)
                {
                    aktywni[i].kicker = (int)aktywni[i].najUklad[0].figura;
                }
                else
                    aktywni[i].kicker = (int)aktywni[i].najUklad[2].figura;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)//szukanie drugiej najwyzszej pary
            {
                int p1 = aktywni[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    if ((int)wygrani[i].najUklad[0].figura == p1)
                    {
                        wygrani[i].kicker = (int)wygrani[i].najUklad[2].figura;
                    }
                    else
                        wygrani[i].kicker = (int)wygrani[i].najUklad[0].figura;
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
                        wygrani[i].kicker = (int)wygrani[i].najUklad[4].figura;
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
                aktywni[i].kicker = (int)aktywni[i].najUklad[0].figura; ;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = -1;
                    for (int j = 0; j < wygrani[i].najUklad.Count; j++)
                    {
                        if (((int)wygrani[i].najUklad[j].figura > wygrani[i].kicker) && ((int)wygrani[i].najUklad[j].figura != (int)wygrani[i].najUklad[0].figura))
                        {
                            wygrani[i].kicker = (int)wygrani[i].najUklad[j].figura;
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
                        for (int j = 0; j < wygrani[i].najUklad.Count; j++)
                        {
                            if (((int)wygrani[i].najUklad[j].figura > wygrani[i].kicker) && ((int)wygrani[i].najUklad[j].figura != (int)wygrani[i].najUklad[0].figura) && ((int)wygrani[i].najUklad[j].figura != k1))
                            {
                                wygrani[i].kicker = (int)wygrani[i].najUklad[j].figura;
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
                            for (int j = 0; j < wygrani[i].najUklad.Count; j++)
                            {
                                if (((int)wygrani[i].najUklad[j].figura > wygrani[i].kicker) && ((int)wygrani[i].najUklad[j].figura != (int)wygrani[i].najUklad[0].figura) && ((int)wygrani[i].najUklad[j].figura != k1) && ((int)wygrani[i].najUklad[j].figura != k2))
                                {
                                    wygrani[i].kicker = (int)wygrani[i].najUklad[j].figura;
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
                aktywni[i].kicker = (int)aktywni[i].najUklad[0].figura; ;
            }
            sortujKick sk = new sortujKick();
            aktywni.Sort(sk);
            for (int i = 0; i < aktywni.Count; i++)
            {
                if (aktywni[i].kicker == aktywni[0].kicker)
                {
                    wygrani.Add(new Gracz { nazwaUkladu = aktywni[i].nazwaUkladu, fold = aktywni[i].fold, hand = aktywni[i].hand, identyfikatorUzytkownika = aktywni[i].identyfikatorUzytkownika, kicker = aktywni[i].kicker, najUklad = aktywni[i].najUklad, nazwaUzytkownika = aktywni[i].nazwaUzytkownika, numerPokoju = aktywni[i].numerPokoju, wart = aktywni[i].wart });
                }
            }
            if (wygrani.Count > 1)
            {
                int k1 = wygrani[0].kicker;
                for (int i = 0; i < wygrani.Count; i++)
                {
                    wygrani[i].kicker = (int)wygrani[i].najUklad[1].figura;
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
                        wygrani[i].kicker = (int)wygrani[i].najUklad[2].figura;
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
       /* public Pokoj() { }

        public Pokoj(string nazwa, int nr, int maxGraczy, Int64 stawkaWe, Int64 bigBlind, Uzytkownik u)
        {
            nazwaPokoju = nazwa;
            numerPokoju = nr;
            iloscGraczyMax = maxGraczy;
            iloscGraczyObecna = 1;
            graRozpoczeta = false;
            stawkaWejsciowa = stawkaWe;
            duzyBlind = bigBlind;
            DodajUzytkownika(u);
            ktoBlind = u.identyfikatorUzytkownika;
            stan = Stan.STARTING;
        } */

        public int DodajUzytkownika(Uzytkownik u)
        {
            if (iloscGraczyObecna < iloscGraczyMax)
            {
                if (user.Exists(delegate(Uzytkownik a) { return u.identyfikatorUzytkownika == a.identyfikatorUzytkownika; }))
                {
                    return 0;
                }
                else
                {
                    user.Add(u);
                    if (iloscGraczyObecna == 1)
                        ktoBlind = u.identyfikatorUzytkownika;
                    return 1;
                }
            }
            return -1;
        }

        public int UsunUzytkownika(Uzytkownik u)
        {
            if (user.Exists(delegate(Uzytkownik a) { return u.identyfikatorUzytkownika == a.identyfikatorUzytkownika; }))
            {
                if (iloscGraczyObecna == 1)
                    ktoBlind = 0;
                else
                {
                    int i = user.FindIndex(delegate(Uzytkownik a) { return u.identyfikatorUzytkownika == a.identyfikatorUzytkownika; });
                    if (i == user.Count - 1)
                        ktoBlind = user[1].identyfikatorUzytkownika;
                    else
                        ktoBlind = user[i + 1].identyfikatorUzytkownika;
                }

                user.Remove(u);

                return 1;                
            }
            else
            {                                
                return 0;     
            }
               
        }

        public Int64 KtoNastepny(Int64 numer)
        {
            int i = user.FindIndex(delegate(Gracz a) { return numer == a.identyfikatorUzytkownika; });
            if (i == user.Count - 1)
                return user[1].identyfikatorUzytkownika;
            else
                return user[i + 1].identyfikatorUzytkownika;            
        }

        public Int64 KtoPoprzedni(Int64 numer)
        {
            int i = user.FindIndex(delegate(Gracz a) { return numer == a.identyfikatorUzytkownika; });
            if (i == 0)
                return user[user.Count-1].identyfikatorUzytkownika;
            else
                return user[i - 1].identyfikatorUzytkownika;
        }        
        
//================================================================================================================================

        









    }
}