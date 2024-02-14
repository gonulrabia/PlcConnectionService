using PlcConnectionService.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcConnectionService.DAL.DTO
{
    public class PlcDataDTO
    {
        public PlcDataDTO()
        {
            this.PlcData = new PlcData();
        }
        public PlcDataDTO(PlcData plcData)
        {
            this.PlcData = plcData;
        }
        public PlcData PlcData { get; set; }
        public string Description { get; set; }
    }
}
