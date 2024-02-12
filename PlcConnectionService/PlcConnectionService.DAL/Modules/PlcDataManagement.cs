using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlcConnectionService.DATA;
using PlcConnectionService.Entities.Entities;
using PlcConnectionService.DAL.DTO;
using Microsoft.Extensions.Options;
using SQLitePCL;
using Microsoft.Extensions.DependencyInjection;


namespace PlcConnectionService.DAL.Modules
{
    public interface IDataService
    {
        PlcDataDTO PostPlcData(PlcData plcData);
    }
    public class PlcDataManagement:IDataService
    {
        private readonly BaseDbContext _context;


        public PlcDataManagement(BaseDbContext context)
        {
            _context = context;
        }
        
        public PlcDataDTO PostPlcData(PlcData plcData)
        {
            PlcDataDTO plcDataDTO = new PlcDataDTO();
            try
            {
                if (_context.PlcDatas == null)
                {

                    plcDataDTO.Description = "Entity set 'BaseDbContext.PlcDatas' is null.";
                    return plcDataDTO;
                }            
                _context.PlcDatas.Add(plcData);
                _context.SaveChangesAsync();
                plcDataDTO.Description = "Data saving was successful.";
                plcDataDTO.PlcData = plcData;
                _context.Dispose();
                return plcDataDTO;

            }
            catch (Exception ex)
            {
                plcDataDTO.Description = ex.Message;
                return plcDataDTO;
            }
            
        }
    }
}
