using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GPLX.Core.Contracts.PdfLogs;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GPLX.Core.Data.PdfLogsRepository
{
    public class PdfLogsRepository : IPdfLogsRepository
    {
        private readonly Context _ctx;

        public PdfLogsRepository(Context ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> CreateAsync(FilePdfCreateLogs create)
        {
            try
            {
                create.Id = Guid.NewGuid();
                await _ctx.FilePdfCreateLogs.AddAsync(create);
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }

        public async Task<int> CounterDay(int unit, string type)
        {
            try
            {
                var q = await _ctx.FilePdfCreateLogs.Where(x => x.Type.Equals(type) && x.UnitId == unit).ToListAsync();
                var c = q.Count(x => (x.CreatedDate - DateTime.Now).Days == 0);
                return c;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return 1000;
            }
        }
    }
}
