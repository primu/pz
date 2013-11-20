using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Akcja
    {
        public Int64 identyfikatorGracza;
        public enum nAkcji : int { FOLD, RISE, CALL, ALLIN, SYSTEM };
        public nAkcji nazwaAkcji;
        public Int32 stempelCzasowy;
        public Int64 numerStolu;
        
        public bool malyBlind;
        public bool duzyBlind;



        public List<Karta> kartyGracza;
        public List<Karta> kartyNaStole;

        public Int64 obecnaStawkaStolu; //na początku duży blind
        public Int64 obecnaStawkaGracza;    //ile dał
        public Int64 iloscKasyNaStole;  //pula
        public Int64 iloscKasyGracza;   //ile mu zostało

        public Int64 nastepnyGracz;
        

        //o czymś jeszcze myślałem, jak mi się przypomni to dopiszę
        // co z użytkownikami którzy przegrają?     

    }
}