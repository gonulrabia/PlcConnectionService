using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcConnectionService.Entities.Entities
{
    public class PlcData
    {
        public int Id { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int Counter { get; set; }
        public int BatchNo { get; set; }
        public int TN { get; set; }
        public int AdimNo { get; set; }
        public int SiloNo { get; set; }

        public int ReceteID { get; set; }
        public int PartiID { get; set; }
        public int HammaddeID { get; set; }
        public int Alinacak { get; set; }
        public int Alinan { get; set; }
        public int Shut { get; set; }
    }
}
