using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlcConnectionService.DATA;
using PlcConnectionService.Entities.Entities;
using PlcConnectionService.DAL.DTO;


namespace PlcConnectionService.DAL.Modules
{
    public class PlcDataManagement
    {
        private readonly BaseDbContext _context;

        public PlcDataManagement()
        {
            BaseDbContext context = new BaseDbContext();
            _context = context;
        }
        
        public async Task<PlcDataDTO> PostPlcData(PlcData plcData)
        {
            PlcDataDTO plcDataDTO = new PlcDataDTO();

            if (_context.PlcDatas == null)
            {
                plcDataDTO.Description = "Entity set 'BaseDbContext.PlcDatas'  is null.";
                return plcDataDTO;
            }
            _context.PlcDatas.Add(plcData);
            await _context.SaveChangesAsync();
            plcDataDTO.Description = "Data saving was successful.";
            plcDataDTO.PlcData = plcData;
            _context.Dispose();
            return plcDataDTO;
        }
    }
}
