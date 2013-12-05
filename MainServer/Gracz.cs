using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Gracz : Uzytkownik
    {
        private List<Karta> hand = new List<Karta>();
        private List<Karta> najUklad = new List<Karta>();
        private int kicker;

        public enum StanGracza : int { Fold, Call, Rise, AllIn, BigBlind, SmallBlind, Dealer, Ready, NotReady};
        
        public StanGracza stan;
        public int wart;//wartość układu kart
        public string nazwaUkladu;


        //Nowe
        public Int64 kasa;  //ile pieniędzy pozostało w obecnej grze
        public Int64 stawia;    //ile pieniędzi stawia w obecnym rozdaniu
        //public bool start = false;
    }
}