using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomDatabase5
{
    public class Game
    {
        public int id; //unique, not part of a .dat file.
        public string name;
        public string description; //identical to name in TOSEC?
        public long size;
        public string crc; //hash 1
        public string md5; //hash 2
        public string sha1; //hash 3.

        //non-standard entries for console games in these dat files.
        public string console; //not contained in dat files because they;re console specific.
        public string datFile; //the datfile the entry was loaded from, useful for pruning my specific ones for dupes.
        public int consoleID;
        public int datFileID;

        //public string genre; //int and FK to a set list?
        //public int year; //generic year for sorting purposes
        //public string releaseDate; // YYYY-MM-DD where available.
        //public string region; //J, U, E, combos, others

        //public string developer;
        //public string publisher; //in case i care about this separately from developer
        //public int Is1G1R; //this entry shows up in a "1 Game 1 Rom" list, for getting rid of bad dumps, dupes, variants, regionals, etc. 0=false, 1=true
        //NOTE: the above probably means I should have a way to link a game to others as a relative/variant/parent-child/etc?
    }
}
