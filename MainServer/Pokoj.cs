using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Pokoj
    {
        public Int64 numerPokoju;
        public int iloscGraczyMax;
        public int iloscGraczyObecna;
        public bool graRozpoczeta;
        public Int64 stawkaWejsciowa;
        public Int64 duzyBlind;
        
        public List<Uzytkownik> user = new List<Uzytkownik>();
        static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
        static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };
        List<Karta> talia = new List<Karta>();
        public List<Karta> stol = new List<Karta>();
        public UkladyKart ukl = new UkladyKart();

        public void rozdanie()
        {
            generujKarty();
            /*Random rnd1 = new Random();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < user.Count; j++)
                {
                    int a=rnd1.Next(0, talia.Count);
                    user[j].hand.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }*/
            // przypisywanie kart uzytkowniom w celach testowych
            user[0].hand.Add(new Karta { figura = Karta.figuraKarty.K8, kolor = Karta.kolorKarty.pik });
            user[0].hand.Add(new Karta { figura = Karta.figuraKarty.KD, kolor = Karta.kolorKarty.karo });
            user[1].hand.Add(new Karta { figura = Karta.figuraKarty.K4, kolor = Karta.kolorKarty.kier });
            user[1].hand.Add(new Karta { figura = Karta.figuraKarty.K7, kolor = Karta.kolorKarty.karo });
            user[2].hand.Add(new Karta { figura = Karta.figuraKarty.K3, kolor = Karta.kolorKarty.pik });
            user[2].hand.Add(new Karta { figura = Karta.figuraKarty.KA, kolor = Karta.kolorKarty.karo });
        }

        public void losujNaStol(int ile)
        {
            stol.Clear();
           /* Random rnd1 = new Random();           
                for (int j = 0; j < ile; j++)
                {
                    int a = rnd1.Next(0, talia.Count);
                    stol.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            */
            //przypisywanie kart na stol w celach testowych
            stol.Add(new Karta { figura = Karta.figuraKarty.KD, kolor = Karta.kolorKarty.kier });
            stol.Add(new Karta { figura = Karta.figuraKarty.K9, kolor = Karta.kolorKarty.kier });
            stol.Add(new Karta { figura = Karta.figuraKarty.K5, kolor = Karta.kolorKarty.kier });
            stol.Add(new Karta { figura = Karta.figuraKarty.K8, kolor = Karta.kolorKarty.kier });
            stol.Add(new Karta { figura = Karta.figuraKarty.K10, kolor = Karta.kolorKarty.kier });
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
                    talia.Add( new Karta { figura = figury[i - 13], kolor = kolory[1] });
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
            pobierzUserow(); 
            rozdanie();
            losujNaStol(5);
            return "ok";//ukl.co_mamy(stol, user[1].hand,user[1].najUklad); 
        }

        public List<Karta> nasze()
        {
            //user[1].najUklad = new List<Karta> (ukl.najlepszyUklad);
           
            return user[1].najUklad;
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
            if (kk == "Strit!")
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
                user[i].nazwaUkladu=ukl.co_mamy(stol, user[i].hand, user[i].najUklad);
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
            if ((ile > 1) && (ile<user.Count) || ((ile>1) && (max==6) ))
            {
                if (max == 9)
                {
                    wygrani.Clear();                  
                        for (int i = 0; i < ile; i++)
                        {                          
                            wygrani.Add(new Uzytkownik {nazwaUkladu=user[i].nazwaUkladu ,fold = user[i].fold, hand = user[i].hand, identyfikatorUzytkownika = user[i].identyfikatorUzytkownika, kicker = user[i].kicker, najUklad = user[i].najUklad, nazwaUzytkownika = user[i].nazwaUzytkownika, numerPokoju = user[i].numerPokoju, wart = user[i].wart });                      
                        }
                        return wygrani;                 
                }
                else if (max == 8)
                {
                    wygrani.Clear();                 
                    List<Uzytkownik> temp= new List<Uzytkownik>(); 
                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker = -1;
                        for (int j = 0; j < user[i].najUklad.Count;j++ )
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
                else if (max == 7)
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
                else if (max == 6)
                {
                    wygrani.Clear();

                    for (int i = 0; i < ile; i++)
                    {
                        user[i].kicker=(int)user[i].najUklad[0].figura;                       
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
                        funkcPom(wygrani);
                    }
                    return wygrani;
                }

                else if (max == 5)
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
                    return wygrani;
                }

            }
            else if (ile == user.Count)//jezeli na stole jest najwyzszy uklad
            {
                wygrani.Clear();
                if (max == 7)
                {
                    ///dodac kod dla tego przypadku
                   
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
                    funkcPom(wygrani);
                    /*for (int i = 0; i < user.Count; i++)
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
                    int ile2 = 0;
                    for (int i = 0; i < user.Count; i++)
                    {
                        if (user[i].kicker == user[0].kicker)
                            ile2++;
                    }
                    for (int j = 0; j < ile2; j++)
                    {
                        wygrani.Add(new Uzytkownik { nazwaUkladu = user[j].nazwaUkladu, fold = user[j].fold, hand = user[j].hand, identyfikatorUzytkownika = user[j].identyfikatorUzytkownika, kicker = user[j].kicker, najUklad = user[j].najUklad, nazwaUzytkownika = user[j].nazwaUzytkownika, numerPokoju = user[j].numerPokoju, wart = user[j].wart });
                    }

                    return wygrani;
                    */
                }
            }

            wygrani.Add(new Uzytkownik { nazwaUkladu = user[0].nazwaUkladu, fold = user[0].fold, hand = user[0].hand, identyfikatorUzytkownika = user[0].identyfikatorUzytkownika, kicker = user[0].kicker, najUklad = user[0].najUklad, nazwaUzytkownika = user[0].nazwaUzytkownika, numerPokoju = user[0].numerPokoju, wart = user[0].wart });
            return wygrani;           
        }

        private List<Uzytkownik> funkcPom(List<Uzytkownik> wygrani)
        {
            for (int i = 0; i < user.Count; i++)
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
            int ile2 = 0;
            for (int i = 0; i < user.Count; i++)
            {
                if (user[i].kicker == user[0].kicker)
                    ile2++;
            }
            for (int j = 0; j < ile2; j++)
            {
                wygrani.Add(new Uzytkownik { nazwaUkladu = user[j].nazwaUkladu, fold = user[j].fold, hand = user[j].hand, identyfikatorUzytkownika = user[j].identyfikatorUzytkownika, kicker = user[j].kicker, najUklad = user[j].najUklad, nazwaUzytkownika = user[j].nazwaUzytkownika, numerPokoju = user[j].numerPokoju, wart = user[j].wart });
            }

            return wygrani;
        }
    }    
}