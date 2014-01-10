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
        public bool graRozpoczeta = false;
        public Int64 stawkaWejsciowa;
        public Int64 duzyBlind; // około 1/100 stawki wejściowej
        public List<Uzytkownik> user = new List<Uzytkownik>();

        private Gra gra;//= new Gra(duzyBlind);

        public bool jestWpokoju(Int64 idUsera)//sprawdza czy uzytkownik znajduje sie wpokoju, zwraca nr pokoju
        {
            //if (user.FindIndex(delegate(Uzytkownik c) { return c.identyfikatorUzytkownika == idUsera; }) >= 0)
            Uzytkownik u=user.Find(delegate(Uzytkownik c) { return c.identyfikatorUzytkownika == idUsera; });
            if (u !=null)
                return true;
            else
                return false;
        }

        public int IleStart(Int64 id)
        {
            int ile=0;
            for (int i = 0; i < user.Count; i++)
            {
                if (user[i].identyfikatorUzytkownika == id)
                    user[i].start = true;
                if (user[i].start == true)
                    ile++;
                
            }
                return ile;
        }

        public Gra zwrocGre()
        {
            return gra;
        }

        public bool DodajUzytkownika(Uzytkownik u)
        {
            if (user.Count < iloscGraczyMax)
            {
                if (user.Exists(delegate(Uzytkownik a) { return u.identyfikatorUzytkownika == a.identyfikatorUzytkownika; }))
                {
                    return false;
                }
                else
                {
                    user.Add(u);
                    iloscGraczyObecna = user.Count;
                    return true;
                }
            }
            return false;
        }

        public void utworz()
        {
            gra = new Gra(duzyBlind, user, stawkaWejsciowa);
            gra.StartujGre();
            graRozpoczeta = true;
            gra.NoweRozdanie();
        }

        public bool UsunUzytkownika(Int64 u)
             {
                 if (user.Exists(delegate(Uzytkownik a) { return u == a.identyfikatorUzytkownika; }))
                 {
                     if (graRozpoczeta)
                     {
                         if (gra.user.Find(delegate(Gracz g) { return g.identyfikatorUzytkownika == u && g.stan == Gracz.StanGracza.Fold; }) == null)
                         {
                             if (gra.ktoBigBlind == u)
                                 gra.ktoBigBlind = gra.KtoNastepny(gra.user, u);
                             if (gra.ktoDealer == u)
                                 gra.ktoDealer = gra.KtoPoprzedni(gra.user, u);
                             if (gra.czyjRuch == u)
                             {
                                 gra.KoniecRuchu();
                                 if (gra.ktoStawia == u)
                                     gra.ktoStawia = gra.KtoNastepny(gra.user, u);                                    
                             }
                             else
                                 if (gra.ktoStawia == u)
                                     gra.ktoStawia = gra.KtoPoprzedni(gra.user, u); // słaby punkt..

                         }
                         gra.pula += gra.user.Find(delegate(Gracz v) { return v.identyfikatorUzytkownika == u; }).kasa;
                         gra.user.RemoveAll(delegate(Gracz v) { return v.identyfikatorUzytkownika == u; });
                         gra.aktywni.RemoveAll(delegate(Gracz v) { return v.identyfikatorUzytkownika == u; });
                     }
                     user.RemoveAll(delegate(Uzytkownik v) { return v.identyfikatorUzytkownika == u; });
                     iloscGraczyObecna = user.Count;
                     if (graRozpoczeta && iloscGraczyObecna == 0)
                         gra = null;
                     return true;
                 }
                 return false;
             }

        public void WyczyscPokoj() // akcja po zakończeniu gry 
        {
            iloscGraczyObecna = 0;
            graRozpoczeta = false;
            //user.Clear(); // sprawa dyskusyjna..
            gra = null;
        }

    }
}