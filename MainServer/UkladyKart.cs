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
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K3, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KA, kolor = Karta.kolorKarty.kier });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K2, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KK, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KD, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.KJ, kolor = Karta.kolorKarty.pik });
          rekaIstol.Add(new Karta { figura = Karta.figuraKarty.K10, kolor = Karta.kolorKarty.pik });
      }

      public bool szukaj(int n,Karta.kolorKarty kol)
      {
          for (int i = 0; i < rekaIstol.Count; i++)
          {
              if (((int)rekaIstol[i].figura == n) && (rekaIstol[i].kolor==kol))
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
           if (szukaj((int)Karta.figuraKarty.K10, Karta.kolorKarty.pik) == true)
               if (szukaj((int)Karta.figuraKarty.KJ, Karta.kolorKarty.pik) == true)
                   if (szukaj((int)Karta.figuraKarty.KD, Karta.kolorKarty.pik) == true)
                       if (szukaj((int)Karta.figuraKarty.KK, Karta.kolorKarty.pik) == true)
                           if (szukaj((int)Karta.figuraKarty.KA, Karta.kolorKarty.pik) == true)
                               return 1;
           return 0;
       }
       public int czyPoker()
       {

           return 0;
       }
       
    }
}