using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MainServer
{
    public class Key
    {
        public Int64 identyfikatorUzytkownika;
        public string token;

        public Key(Int64 idUzytkownika)
        {
            this.identyfikatorUzytkownika = idUzytkownika;
            Random rand = new Random();
            byte[] temp = new byte[32];
            rand.NextBytes(temp);
            this.token = "";
            for (int i = 0; i < 32; i++)
            {
                this.token += (char)( (temp[i] % 74) + 48 );
            }
        }
    }
}