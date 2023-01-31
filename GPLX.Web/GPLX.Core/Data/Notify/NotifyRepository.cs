using System;
using System.Threading.Tasks;
using GPLX.Core.Contracts.Notify;
using GPLX.Database;
using Serilog;

namespace GPLX.Core.Data.Notify
{
    public class NotifyRepository : INotifyRepository
    {
        private readonly Context _ctx;

        public NotifyRepository(Context ctx)
        {
            _ctx = ctx;
        }
        public async Task<bool> AddAsync(Database.Models.Notify add)
        {
            try
            {
                await _ctx.Notify.AddAsync(add);
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error");
                return false;
            }
        }
    }
}
