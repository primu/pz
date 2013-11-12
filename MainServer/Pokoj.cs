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
            Random rnd1 = new Random();
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < user.Count; j++)
                {
                    int a=rnd1.Next(0, talia.Count);
                    user[j].hand.Add(new Karta { figura = talia[a].figura, kolor = talia[a].kolor });
                    talia.RemoveAt(a);
                }
            }
        }

        public void losujNaStol(int ile)
        {
            stol.Clear();
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
                    //talia[i] = new Karta { figura = figury[i], kolor = kolory[0] };
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

        public string ktoWygral()
        {
            int max;
            sortujWart sw = new sortujWart();
            for (int i = 0; i < user.Count; i++)
            {
                if (user[i].fold == false)
                {
                    user[i].wart = wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad));                   
                }
                user.Sort(sw);
            }
            max = user[0].wart;
            int ile = 0;
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
            if (ile > 1)
            {
                if (max == 9)
                {
                    /*if (ile == 1)
                    {
                        return "Wygral " + user[0].nazwaUzytkownika;
                    }*/
                    //else if (ile > 1)
                    //{
                        string osoby = "Wygrali: ";
                        for (int i = 0; i < ile; i++)
                        {
                            osoby = osoby + user[i].nazwaUzytkownika + " ";
                        }
                        return osoby;
                    //}
                }
                else if (max == 8)
                {
                    //int maxCard = -1;
                    List<Uzytkownik> wygrani= new List<Uzytkownik>(); 
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
                        wygrani.Add(new Uzytkownik { fold=user[i].fold,hand=user[i].hand,identyfikatorUzytkownika=user[i].identyfikatorUzytkownika,kicker=user[i].kicker,najUklad=user[i].najUklad,nazwaUzytkownika=user[i].nazwaUzytkownika,numerPokoju=user[i].numerPokoju,wart=user[i].wart});                      
                        //if (user[i].kicker > maxCard)
                        //{
                           // maxCard = user[i].kicker;
                        //}
                    }
                    sortujKick sk = new sortujKick();
                    wygrani.Sort(sk);
                    int ile2 = 0;
                    for (int k = 0; k < wygrani.Count; k++)
                    {
                        if (wygrani[k].kicker == wygrani[0].kicker)
                            ile2++;
                    }
                    if (ile2 == 1)
                        return "Wygral " + wygrani[0].nazwaUzytkownika;
                    else if (ile2 > 1)
                    {
                        string osoby = "Wygrali: ";
                        for (int i = 0; i < ile2; i++)
                        {
                            osoby = osoby + wygrani[i].nazwaUzytkownika + " ";
                        }
                        return osoby;
                    }
                    
                }
            }

                /*  int max = -1;
            
                  int index=-1;
                  for (int i = 0; i < user.Count; i++)
                  {
               
                      if(wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad))>max)
                      {
                          max = wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad));
                          index = i;
                      }
                      else if (wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad)) == max)
                      {
                          if(max==9)
                              return "remis";
                          if (max == 8)
                          {
                              for (int k = 0; k < user.Count; k++)
                              {
                            
                              }

                              int maxCard = -1;
                              for (int j = 0; j < user[i].najUklad.Count; j++)
                              {
                                  if ((int)user[i].najUklad[j].figura > maxCard)
                                      maxCard = (int)user[i].najUklad[j].figura;
                              }
                          }
                      }
                      else if (wartosci(ukl.co_mamy(stol, user[i].hand, user[i].najUklad)) < max)
                      {

                      }
                  }*/
                return "Wygral " +user[0].nazwaUzytkownika;           
        }
    }    
}