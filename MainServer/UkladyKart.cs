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
      //Karta[] rekaIstol = new Karta[7]; 
      List<Karta> rekaIstol = new List<Karta>();

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
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K10, kolor = Karta.kolorKarty.trefl });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K5, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K8, kolor = Karta.kolorKarty.kier });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K2, kolor = Karta.kolorKarty.trefl });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K9, kolor = Karta.kolorKarty.karo });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K7, kolor = Karta.kolorKarty.kier });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K6, kolor = Karta.kolorKarty.trefl });
          //int z = (int)Karta.kolorKarty.trefl;
      }

      public bool szukaj(int n,int kol)
      {
          for (int i = 0; i < rekaIstol.Count; i++)
          {
              if (((int)rekaIstol[i].figura == n) && ((int)rekaIstol[i].kolor==kol))
                  return true;
          }
              return false;
      }

      public bool szukaj(int n)
      {
          for (int i = 0; i < rekaIstol.Count; i++)
          {
              if ((int)rekaIstol[i].figura == n)
                  return true;
          }
          return false;
      }


       public int czyPokerKrolewski()
       {
           if (szukaj((int)Karta.figuraKarty.K10, (int)Karta.kolorKarty.pik) == true)
               if (szukaj((int)Karta.figuraKarty.KJ, (int)Karta.kolorKarty.pik) == true)
                   if (szukaj((int)Karta.figuraKarty.KD, (int)Karta.kolorKarty.pik) == true)
                       if (szukaj((int)Karta.figuraKarty.KK, (int)Karta.kolorKarty.pik) == true)
                           if (szukaj((int)Karta.figuraKarty.KA, (int)Karta.kolorKarty.pik) == true)
                               return 1;
           return 0;
       }
       public int czyPoker()
       {
           int x = 0;
           for (int j = 0; j < 8; j++)
           {
               for (int i = 0; i < 5; i++)
               {
                   if (szukaj((int)Karta.figuraKarty.K9 - j + i, (int)Karta.kolorKarty.pik) == true)
                   {
                       x++;
                   }
               }
               if (x == 5)
                   return 1;
               x = 0;
           }

           for (int k = 0; k < 3; k++)
           {
               for (int j = 0; j < 9; j++)
               {
                   for (int i = 0; i < 5; i++)
                   {
                       if (szukaj((int)Karta.figuraKarty.K10 - j + i, (int)Karta.kolorKarty.kier + k) == true)
                       {
                           x++;
                       }
                   }
                   if (x == 5)
                       return 1;
                   x = 0;
               }
           }
           return 0;
       }

       public int czyStrit()
       {
           int x = 0;
           for (int j = 0; j < 9; j++)
           {
               for (int i = 0; i < 5; i++)
               {
                   if (szukaj((int)Karta.figuraKarty.K10 - j + i) == true)
                   {
                       x++;
                   }
               }
               if (x == 5)
                   return 1;
               x = 0;
           }
           return 0;
       }


       public bool szukajKolor(int kol)
       {
           int z = 0;
           for (int i = 0; i < rekaIstol.Count; i++)
           {
               if ((int)rekaIstol[i].kolor == kol)
               {
                   z++;
                   if(z==5)
                        return true;
               }
           }
           return false;
       }

       public int czyKolor()
       {          
           for (int k = 0; k < 4; k++)
           {                              
                if (szukajKolor((int)Karta.kolorKarty.pik+k) == true)             
                   return 1;                            
           }
           return 0;
       }
       public int czyKareta()
       {
           int x = 0;
           for(int j=0;j<rekaIstol.Count;j++)
           {
                for (int i = 0; i < rekaIstol.Count; i++)
                {
                    if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor!=(int)rekaIstol[i].kolor))
                    {
                        x++;
                    }
                }
                if (x == 3)               
                    return 1;
                x = 0;
                
           }
               return 0;
       }

       public int czyTrojka()
       {
           int x = 0;
           for (int j = 0; j < rekaIstol.Count; j++)
           {
               for (int i = 0; i < rekaIstol.Count; i++)
               {
                   if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[i].kolor))
                   {
                       x++;
                   }
               }
               if (x == 2)
                   return 1;
               x = 0;
           
           }
           return 0;
       }

       public int czyPara()
       {          
           for (int j = 0; j < rekaIstol.Count; j++)
           {
               for (int i = 0; i < rekaIstol.Count; i++)
               {
                   if (((int)rekaIstol[j].figura == (int)rekaIstol[i].figura) && ((int)rekaIstol[j].kolor != (int)rekaIstol[i].kolor))
                       return 1;
               }              
           }         
           return 0;
       }

       public int czyDwiePary()
       {
           for (int i = 0; i < figury.Length; i++)
           {
               if (szukaj((int)Karta.figuraKarty.KA-i) == true)
               {
               }
           }
               return 0;
       }
       
    }
}