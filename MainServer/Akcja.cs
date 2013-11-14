using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Akcja
    {
        public Int64 identyfikatorGracza;
        public string nazwaAkcji;
        public Int32 stempelCzasowy;
        public Int64 numerStolu;
        public List<Karta> kartyGracza;
        public List<Karta> kartyNaStole;
        public bool malyBlind;
        public bool duzyBlind;

        public Int64 obecnaStawkaStolu; //duży blind
        public Int64 obecnaStawkaGracza;    //ile dał
        public Int64 iloscKasyNaStole;  //pula
        public Int64 iloscKasyGracza;   //ile mu zostało

        public Int64 nastepnyGracz;
        

        //o czymś jeszcze myślałem, jak mi się przypomni to dopiszę
        // co z użytkownikami którzy przegrają?

    }
}