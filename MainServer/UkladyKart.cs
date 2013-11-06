using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class UkladyKart
    {
      Karta[] tab = new Karta[52]; 
      static Karta.figuraKarty[] figury = { Karta.figuraKarty.K2, Karta.figuraKarty.K3, Karta.figuraKarty.K4, Karta.figuraKarty.K5, Karta.figuraKarty.K6, Karta.figuraKarty.K7, Karta.figuraKarty.K8, Karta.figuraKarty.K9, Karta.figuraKarty.K10, Karta.figuraKarty.KJ, Karta.figuraKarty.KD, Karta.figuraKarty.KK, Karta.figuraKarty.KA, };
      static Karta.kolorKarty[] kolory = { Karta.kolorKarty.pik, Karta.kolorKarty.kier, Karta.kolorKarty.karo, Karta.kolorKarty.trefl };
      List<Karta> najlepszyUklad = new List<Karta>();
      List<Karta> temp = new List<Karta>();
      List<Karta> rekaIstol = new List<Karta>();
      int p = -1;
      static bool sf = false;//czy w tym momencie szukamy fulla

      public string co_mamy()
      {
          if (czyPokerKrolewski() == 1)
              return "Poker krolewski!";
          else if (czyPoker() == 1)
              return "Poker!";
          else if (czyKareta() == 1)
              return "Kareta!";
          else if (czyFull() == 1)
              return "Full!";
          else if (czyKolor() == 1)
              return "Kolor!";
          else if (czyStrit() == 1)
              return "Strit!";
          else if (czyTrojka() == 1)
              return "Trojka!";
          else if (czyDwiePary() == 1)
              return "Dwie pary!";
          else if (czyPara() == 1)
              return "Para!";

          return czyWysokaKarta();
      }

      public List<Karta> reka()
      {
          return najlepszyUklad;
      }
      public void generujKarty()
      {       
          for (int i = 0; i < 52; i++)
          {
              if (i < 13)
              {
                  tab[i] = new Karta { figura = figury[i], kolor = kolory[0] };               
              }
              if (i > 12 && i<26)
              {
                  tab[i] = new Karta { figura = figury[i-13], kolor = kolory[1] };                 
              }
              if (i > 25 && i < 39)
              {
                  tab[i] = new Karta { figura = figury[i-26], kolor = kolory[2] };                
              }
              if (i > 38 && i < 52)
              {
                  tab[i] = new Karta { figura = figury[i-39], kolor = kolory[3] };                
              }              
          }
          rekaIstol.Clear();
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KA, kolor = Karta.kolorKarty.trefl });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KK, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KJ, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KD, kolor = Karta.kolorKarty.trefl });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K10, kolor = Karta.kolorKarty.kier });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K5, kolor = Karta.kolorKarty.karo });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K6, kolor = Karta.kolorKarty.karo });        
      }

      private bool szukaj(int n,int kol)
      {
          for (int i = 0; i < rekaIstol.Count; i++)
          {
              if (((int)rekaIstol[i].figura == n) && ((int)rekaIstol[i].kolor==kol))
                  return true;
          }
              return false;
      }

      private bool szukaj(int n)
      {
          for (int i = 0; i < rekaIstol.Count; i++)
          {
              if ((int)rekaIstol[i].figura == n)
                  return true;
          }
          return false;
      }


       private int czyPokerKrolewski()
       {
           najlepszyUklad.Clear();
           int x = 0;
           for (int k = 0; k < 4; k++)
           {
               for (int i = 0; i < 5; i++)
               {
                   if (szukaj((int)Karta.figuraKarty.K10 + i, (int)Karta.kolorKarty.pik + k) == true)
                   {
                       najlepszyUklad.Add(new Karta { figura = figury[(int)Karta.figuraKarty.K10 + i], kolor = Karta.kolorKarty.pik + k });
                       x++;
                   }                  
               }
               if (x == 5)
                   return 1;
               x = 0;
               najlepszyUklad.Clear();
           }
           return 0;
       }

       private int czyPoker()
       {
           najlepszyUklad.Clear();
           int x = 0;

           for (int k = 0; k < 4; k++)
           {
               for (int j = 0; j < 8; j++)
               {
                   for (int i = 0; i < 5; i++)
                   {
                       if (szukaj((int)Karta.figuraKarty.K9 - j + i, (int)Karta.kolorKarty.pik + k) == true)
                       {
                           najlepszyUklad.Add(new Karta { figura = figury[(int)Karta.figuraKarty.K9 - j + i], kolor = Karta.kolorKarty.pik+k });
                           x++;
                       }
                   }
                   if (x == 5)
                       return 1;
                   x = 0;
                   najlepszyUklad.Clear();
               }
           }
           return 0;
       }

        private int szukajKolorDlaFigury(int fig)
        {
            for (int i = 0; i < rekaIstol.Count; i++)
            {
                if ((int)rekaIstol[i].figura == fig)
                {
                    return (int)rekaIstol[i].kolor;
                }
            }
            return -1;
        }

       private int czyStrit()//dokończyć AS jako karta niska
       {
           najlepszyUklad.Clear();
           int x = 0;
           for (int j = 0; j < 9; j++)
           {
               for (int i = 0; i < 5; i++)
               {
                   if (szukaj((int)Karta.figuraKarty.K10 - j + i) == true)
                   {
                       najlepszyUklad.Add(new Karta { figura = figury[(int)Karta.figuraKarty.K10 - j + i], kolor = kolory[szukajKolorDlaFigury((int)Karta.figuraKarty.K10 - j + i)] });
                       x++;
                   }
               }
               if (x == 5)
                   return 1;
               x = 0;
               najlepszyUklad.Clear();
           }
           return 0;
       }
   
       private bool szukajKolor(int kol)
       {
           int z = 0;
           najlepszyUklad.Clear();

           for (int j = figury.Length - 1; j >= 0; j--)
           {

               for (int i = 0; i < rekaIstol.Count; i++)
               {
                   if (figury[j] == rekaIstol[i].figura)
                   {
                       if ((int)rekaIstol[i].kolor == kol)
                       {
                           z++;
                           najlepszyUklad.Add(new Karta { figura = figury[j], kolor = rekaIstol[i].kolor });
                        
                           if (z == 5)
                               return true;
                       }
                   }
               }
           }           
           return false;
       }

       private int czyKolor()
       {          
           for (int k = 0; k < 4; k++)
           {                              
                if (szukajKolor((int)Karta.kolorKarty.pik+k) == true)             
                   return 1;
                najlepszyUklad.Clear();           
           }
           return 0;
       }

       private void szukajNajwyzsze(int ile)
       {
           temp = new List<Karta> (rekaIstol);
           int x = 0;
           int z = -1; 
           int max = -1;
           int index = -1;
           int wyjatek;
           while (x < ile)
           {
               if (najlepszyUklad.Count != 0)
                   wyjatek = (int)najlepszyUklad[0].figura;
               else wyjatek = -1;
                for (int i = 0; i < temp.Count; i++)
                {                                       
                        if ((int)temp[i].figura != wyjatek)
                        {
                            if ((int)temp[i].figura > max)
                            {
                                max = (int)temp[i].figura;
                                z = (int)temp[i].kolor;
                                index = i;
                            }
                        }
                }
                najlepszyUklad.Add(new Karta { figura = figury[max], kolor = kolory[z] });
                temp.Remove(temp[index]);
                z = -1;
                max = -1;
                x++;
           }                       
       }
        
       private int czyKareta()
       {
           int x = 0;
           najlepszyUklad.Clear();
           for(int j=0;j<rekaIstol.Count;j++)
           {
                for (int i = 0; i < rekaIstol.Count; i++)
                {
                    if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor!=(int)rekaIstol[i].kolor))
                    {
                        if(x==0)
                            najlepszyUklad.Add(new Karta { figura = rekaIstol[j].figura, kolor = rekaIstol[j].kolor });
                        x++;
                        najlepszyUklad.Add(new Karta { figura = rekaIstol[i].figura, kolor = rekaIstol[i].kolor });                       
                    }
                }
                if (x == 3)
                {
                    szukajNajwyzsze(1);
                    return 1;
                }
                x = 0;
                najlepszyUklad.Clear();
                
           }
               return 0;
       }

       private int czyTrojka()
       {
           int x = 0;
           najlepszyUklad.Clear();
           for (int j = 0; j < rekaIstol.Count; j++)
           {
               for (int i = 0; i < rekaIstol.Count; i++)
               {
                   if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[i].kolor))
                   {
                       if (x == 0)
                           najlepszyUklad.Add(new Karta { figura = rekaIstol[j].figura, kolor = rekaIstol[j].kolor });
                       x++;
                       najlepszyUklad.Add(new Karta { figura = rekaIstol[i].figura, kolor = rekaIstol[i].kolor });
                       p = (int)rekaIstol[j].figura;
                   }
               }
               if (x == 2)
               {
                   if(sf==false)
                   szukajNajwyzsze(2);
                   return 1;                  
               }
               x = 0;
               najlepszyUklad.Clear();
               p = -1;
           
           }         
           return 0;
       }

       private int czyPara()
       {          
           for (int j = 0; j < rekaIstol.Count; j++)
           {
               for (int i = 0; i < rekaIstol.Count; i++)
               {
                   if ((int)rekaIstol[j].figura!=p)
                       if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[i].kolor))
                       {
                           najlepszyUklad.Add(new Karta { figura = rekaIstol[j].figura, kolor = rekaIstol[j].kolor });
                           najlepszyUklad.Add(new Karta { figura = rekaIstol[i].figura, kolor = rekaIstol[i].kolor });
                           if(sf==false)
                           szukajNajwyzsze(3);
                           return 1;
                       }
               }              
           }         
           return 0;
       }

       private void szukajKickera(int p1, int p2)
       {
           temp = new List<Karta>(rekaIstol);        
           int z = -1;
           int max = -1;
           int index = -1;         
          
               for (int i = 0; i < temp.Count; i++)
               {
                   if (((int)temp[i].figura != p1) && ((int)temp[i].figura != p2))
                   {
                       if ((int)temp[i].figura > max)
                       {
                           max = (int)temp[i].figura;
                           z = (int)temp[i].kolor;
                           index = i;
                       }
                   }                  
               }
               najlepszyUklad.Add(new Karta { figura = figury[max], kolor = kolory[z] });              
               z = -1;
               max = -1;                 
       }
       private int czyDwiePary()
       {
           int pierwsza = -1;
           int druga = -1;
           int trzecia = -1;
           int q = -1, q2 = -1, d = -1, d2 = -1, t = -1, t2 = -1; 
           int n = 0;
           for (int j = 0; j < rekaIstol.Count; j++)
           {
               if (pierwsza == -1)
               {
                   for (int i = 0; i < rekaIstol.Count; i++)
                   {
                       if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[i].kolor))
                       {
                           pierwsza = (int)rekaIstol[j].figura;
                           q = j;
                           q2 = i;
                           n++;
                           break;
                       }
                   }
               }
               else
               {
                   if (n < 3)
                   {
                       if(druga==-1)
                       if ((int)rekaIstol[j].figura != pierwsza)
                           for (int k = 0; k < rekaIstol.Count; k++)
                           {
                               if (((int)rekaIstol[j].figura == (int)rekaIstol[k].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[k].kolor))
                               {
                                   d = j;
                                   d2 = k;
                                   druga = (int)rekaIstol[j].figura;
                                   n++;
                                   break;
                                   //return 1;
                               }
                           }
                        if(trzecia==-1 && druga!=-1)
                       if ((int)rekaIstol[j].figura != pierwsza && (int)rekaIstol[j].figura != druga)
                           for (int k = 0; k < rekaIstol.Count; k++)
                           {
                               if (((int)rekaIstol[j].figura == (int)rekaIstol[k].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[k].kolor))
                               {
                                   t = j;
                                   t2 = k;
                                   trzecia = (int)rekaIstol[j].figura;
                                   n++;
                                   break;
                               }
                           }
                   }                  
               }
           }
           if (((pierwsza > druga) && (druga > trzecia) && (pierwsza > trzecia)) || ((pierwsza < druga) && (druga > trzecia) && (pierwsza > trzecia)))
           {
               najlepszyUklad.Add(new Karta { figura = rekaIstol[q].figura, kolor = rekaIstol[q].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[q2].figura, kolor = rekaIstol[q2].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d].figura, kolor = rekaIstol[d].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d2].figura, kolor = rekaIstol[d2].kolor });
               szukajKickera(pierwsza, druga);
               return 1;
           }
           else if (((trzecia > druga) && (druga > pierwsza) && (trzecia > pierwsza)) || ((trzecia < druga) && (druga > pierwsza) && (trzecia > pierwsza)))
           {
               najlepszyUklad.Add(new Karta { figura = rekaIstol[t].figura, kolor = rekaIstol[t].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[t2].figura, kolor = rekaIstol[t2].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d].figura, kolor = rekaIstol[d].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d2].figura, kolor = rekaIstol[d2].kolor });
               szukajKickera(trzecia, druga);
               return 1;
           }
           else if (((pierwsza > trzecia) && (trzecia > druga) && (pierwsza > druga)) || ((pierwsza < trzecia) && (trzecia > druga) && (pierwsza > druga)))
           {
               najlepszyUklad.Add(new Karta { figura = rekaIstol[t].figura, kolor = rekaIstol[t].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[t2].figura, kolor = rekaIstol[t2].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d].figura, kolor = rekaIstol[d].kolor });
               najlepszyUklad.Add(new Karta { figura = rekaIstol[d2].figura, kolor = rekaIstol[d2].kolor });
               szukajKickera(pierwsza, trzecia);
               return 1;
           }
           return 0;
       }

       private int czyFull()
       {
           sf = true;
           if ((czyTrojka() == 1) && (czyPara() == 1))
           {
               sf = false;
               return 1;
           }
           sf = false;
           return 0;
       }

       private String czyWysokaKarta()
       {
           szukajNajwyzsze(5);
           return "Wysoka karta!";
       }     
    }
}