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

        public long ReceteID { get; set; }
        public long PartiID { get; set; }
        public long HammaddeID { get; set; }
        public long Alinacak { get; set; }
        public long Alinan { get; set; }
        public long Shut { get; set; }
    }
}
