using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlcConnectionService.Entities.Entities;
namespace PlcConnectionService.DATA
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext()
        {
           
        }

        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }
        


        public DbSet<PlcData> PlcDatas { get; set; }

    }
}
