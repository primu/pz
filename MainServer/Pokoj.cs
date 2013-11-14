using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Pokoj
    {
        //pola POKOJU
        public string nazwaPokoju;
        public Int64 numerPokoju;
        public int iloscGraczyMax;
        public int iloscGraczyObecna;
        public bool graRozpoczeta;
        public Int64 stawkaWejsciowa;
        public Int64 duzyBlind;



        //pola GRY
        public Int64 ktoBlind = 0;        
        public Int64 stawia;
        public Int64 obecnaStawka;
        
        public List<Uzytkownik> user = new List<Uzytkownik>();
        static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
        static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };
        List<Karta> talia = new List<Karta>();
        public List<Karta> stol = new List<Karta>();
        public UkladyKart ukl = new UkladyKart();

        public void rozdanie()
        {
            generujKarty();
            Random rnd1 = new Random();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < user.Count; j++)
                {
                    int a = rnd1.Next(0, talia.Count);
                    user[j].hand.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }

            stawia = ktoBlind;
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
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 1, nazwaUzytkownika = "Pawel", numerPokoju = 1 });
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 2, nazwaUzytkownika = "Bogdan", numerPokoju = 1 });
            user.Add(new Uzytkownik { identyfikatorUzytkownika = 3, nazwaUzytkownika = "Primu", numerPokoju = 1 });
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

        public List<Uzytkownik> ktoWygral()
        {
            int max;
            sortujWart sw = new sortujWart();
            List<Uzytkownik> wygrani = new List<Uzytkownik>();
            for (int i = 0; i < user.Count; i++)
            {
                user[i].nazwaUkladu = ukl.co_mamy(stol, user[i].hand, user[i].najUklad);
                if (user[i].fold == false)
                {
                    user[i].wart = wartosci(user[i].nazwaUkladu);//wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad));                   
                }
                user.Sort(sw);
            }
            max = user[0].wart;
            int ile = 0; // ile osob ma najwyzszy uklad
            for (int j = 0; j < user.Count; j++)
            {
                if (user[j].fold == false)
                {
                    if (user[j].wart == max)
                    {
                        ile++;
                    }
                }
            }
            if ((ile > 1) && (ile < user.Count) || ((ile > 1) && (max == 6)) || ((ile > 1) && (max == 5)) || ((ile > 1) && (max == 3)) || ((ile > 1) && (max == 2)) || ((ile > 1) && (max == 1)) || ((ile > 1) && (max == 0)))
            {
                if (max == 9)//poker krolewski
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                    }
                    return wygrani;
                }
                else if (max == 8)//poker
                {
                    wygrani.Clear();
                    List<Uzytkownik> temp = new List<Uzytkownik>();
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = -1;
                        for (int j = 0; j < user[i].najUklad.Count; j++)
                        {
                            if ((int)user[i].najUklad[j].figura > user[i].kicker)
                            {
                                user[i].kicker = (int)user[i].najUklad[j].figura;
                            }
                        }
                    }
                    for (int i = 0; i < ile; i++)
                    {
                        temp.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });

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
                        wygrani.Add(new Uzytkownik { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
                    }
                    return wygrani;


                }
                else if (max == 7)//kareta
                {
                    wygrani.Clear();
                    int wartK = -1;//maksymalna wartość karety
                    for (int i = 0; i < ile; i++)
                    {
                        if ((int)user[i].najUklad[0].figura > wartK)
                        {
                            wartK = (int)user[i].najUklad[0].figura;
                        }
                    }

                    for (int i = 0; i < ile; i++)
                    {
                        if ((int)user[i].najUklad[0].figura == wartK)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                        }
                    }
                    /*
                                        for(int j=0;j<wygrani.Count;j++)
                                        {
                                            wygrani[j].kicker=-1;
                                            for (int i = 0; i < wygrani.Count; i++)
                                            {                           
                                                if ((int)wygrani[j].najUklad[i].figura != (int)wygrani[j].najUklad[0].figura)
                                                {
                                                    wygrani[j].kicker = (int)wygrani[j].najUklad[i].figura;
                                                }
                                            }
                                        }
                    
                                        sortujKick sk = new sortujKick();
                                        wygrani.Sort(sk);
                                        for (int i = 1; i < wygrani.Count; i++)
                                        {
                                            if (wygrani[i].kicker < wygrani[0].kicker)
                                            {
                                                wygrani.RemoveAt(i);
                                            }
                                        }
                      */
                    return wygrani;

                }
                else if (max == 6)//full
                {
                    wygrani.Clear();

                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = (int)user[i].najUklad[0].figura;
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                        }
                    }
                    if (wygrani.Count > 1)
                    {
                        wygrani.Clear();
                        funkcPom(wygrani, user);
                    }
                    return wygrani;
                }

                else if (max == 5)//kolor
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = -1;
                        for (int j = 0; j < user[i].najUklad.Count; j++)
                        {
                            if ((int)user[i].najUklad[j].figura > user[i].kicker)
                            {
                                user[i].kicker = (int)user[i].najUklad[j].figura;
                            }
                        }
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
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
                            List<Uzytkownik> temp2 = new List<Uzytkownik>(wygrani);
                            wygrani.Clear();
                            funkcPom(wygrani, temp2);
                        }

                    }

                    return wygrani;
                }
                else if (max == 4)//strit
                {
                    wygrani.Clear();
                    List<Uzytkownik> temp = new List<Uzytkownik>();
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = -1;
                        for (int j = 0; j < user[i].najUklad.Count; j++)
                        {
                            if ((int)user[i].najUklad[j].figura > user[i].kicker)
                            {
                                user[i].kicker = (int)user[i].najUklad[j].figura;
                            }
                        }
                    }
                    for (int i = 0; i < ile; i++)
                    {
                        temp.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });

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
                        wygrani.Add(new Uzytkownik { nazwaUkladu = temp[i].nazwaUkladu, fold = temp[i].fold, hand = temp[i].hand, identyfikatorUzytkownika = temp[i].identyfikatorUzytkownika, kicker = temp[i].kicker, najUklad = temp[i].najUklad, nazwaUzytkownika = temp[i].nazwaUzytkownika, numerPokoju = temp[i].numerPokoju, wart = temp[i].wart });
                    }
                    return wygrani;
                }
                else if (max == 3)//trójka
                {
                    wygrani.Clear();
                    int wartK = -1;//maksymalna wartość trojki
                    for (int i = 0; i < ile; i++)
                    {
                        if ((int)user[i].najUklad[0].figura > wartK)
                        {
                            wartK = (int)user[i].najUklad[0].figura;
                        }
                    }

                    for (int i = 0; i < ile; i++)
                    {
                        if ((int)user[i].najUklad[0].figura == wartK)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                        }
                    }

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
                        if (wygrani[i].kicker < wygrani[0].kicker)
                        {
                            wygrani.RemoveAt(i);
                            i--;
                        }
                    }

                    return wygrani;
                }
                else if (max == 2)//dwie pary
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        if ((int)user[i].najUklad[0].figura > (int)user[i].najUklad[2].figura)
                        {
                            user[i].kicker = (int)user[i].najUklad[0].figura;
                        }
                        else
                            user[i].kicker = (int)user[i].najUklad[2].figura;
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                        }
                    }
                    if (wygrani.Count > 1)//szukanie drugiej najwyzszej pary
                    {
                        int p1 = user[0].kicker;
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
                            return wygrani;
                        }
                        return wygrani;
                    }
                    return wygrani;
                }
                else if (max == 1)//para
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = (int)user[i].najUklad[0].figura; ;
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
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
                                //dopisac porównywanie kart z ręki, w przypadku najwyższego układu na stole
                                return wygrani;
                            }

                            return wygrani;
                        }

                        return wygrani;
                    }

                    return wygrani;
                }
                else if (max == 0)//wysoka karta ----dopisac koncowke
                {
                    wygrani.Clear();
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = (int)user[i].najUklad[0].figura; ;
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
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
                            return wygrani;
                        }
                        return wygrani;
                    }
                    return wygrani;
                }

            }
            else if (ile == user.Count)//jezeli na stole jest najwyzszy uklad
            {
                wygrani.Clear();
                if ((max == 7))
                {
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = -1;
                        for (int j = 0; j < user[i].hand.Count; j++)
                        {
                            if ((int)user[i].hand[j].figura > user[i].kicker)
                            {
                                user[i].kicker = (int)user[i].hand[j].figura;
                            }
                        }
                    }
                    sortujKick sk = new sortujKick();
                    user.Sort(sk);
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                        {
                            wygrani.Add(new Uzytkownik { nazwaUkladu = user[i].nazwaUkladu, fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });
                        }
                    }
                    return wygrani;
                }
                else
                {
                    funkcPom(wygrani, user);
                }
            }

            wygrani.Add(new Uzytkownik { nazwaUkladu = user[0].nazwaUkladu, fold = user[0].fold, hand = user[0].hand, identyfikatorUzytkownika = user[0].identyfikatorUzytkownika, kicker = user[0].kicker, najUklad = user[0].najUklad, nazwaUzytkownika = user[0].nazwaUzytkownika, numerPokoju = user[0].numerPokoju, wart = user[0].wart });
            return wygrani;
        }

        private void funkcPom(List<Uzytkownik> wygrani, List<Uzytkownik> user2)
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
            int ile2 = 0;
            for (int i = 0; i < user2.Count; i++)
            {
                if (user2[i].kicker == user2[0].kicker)
                    ile2++;
            }
            for (int j = 0; j < ile2; j++)
            {
                wygrani.Add(new Uzytkownik { nazwaUkladu = user2[j].nazwaUkladu, fold = user2[j].fold, hand = user2[j].hand, identyfikatorUzytkownika = user2[j].identyfikatorUzytkownika, kicker = user2[j].kicker, najUklad = user2[j].najUklad, nazwaUzytkownika = user2[j].nazwaUzytkownika, numerPokoju = user2[j].numerPokoju, wart = user2[j].wart });
            }
        }

//=======================================================================================================================================
        public Pokoj() { }

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
        }

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
                return 0;
            }
            else
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
               
        }

        public Int64 KtoNastepny(Int64 numer)
        {
            int i = user.FindIndex(delegate(Uzytkownik a) { return numer == a.identyfikatorUzytkownika; });
            if (i == user.Count - 1)
                return user[1].identyfikatorUzytkownika;
            else
                return user[i + 1].identyfikatorUzytkownika;            
        }

        public Int64 KtoPoprzedni(Int64 numer)
        {
            int i = user.FindIndex(delegate(Uzytkownik a) { return numer == a.identyfikatorUzytkownika; });
            if (i == 0)
                return user[user.Count-1].identyfikatorUzytkownika;
            else
                return user[i - 1].identyfikatorUzytkownika;
        }


    }    
}