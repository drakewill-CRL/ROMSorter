using System;
using System.Collections.Generic;

namespace RomSorter5.EntitiesForce
{
    public partial class Games
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? Size { get; set; }
        public string Crc { get; set; }
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public long? Console { get; set; }
        public long? DatFile { get; set; }
    }
}
