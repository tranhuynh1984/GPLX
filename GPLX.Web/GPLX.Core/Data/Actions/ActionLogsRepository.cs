using System;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Database;
using GPLX.Database.Models;
using Serilog;

namespace GPLX.Core.Data.Actions
{
    public class ActionLogsRepository : IActionLogsRepository
    {
        private readonly Context _ctx;
        public static int AllStat = 100;
        public ActionLogsRepository(Context ctx)
        {
            _ctx = ctx;
        }

        public async Task AddLogAsync(ActionLogs item)
        {
            try
            {
                await _ctx.ActionLogs.AddAsync(item);
                await _ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
            }
        }
    }
}
