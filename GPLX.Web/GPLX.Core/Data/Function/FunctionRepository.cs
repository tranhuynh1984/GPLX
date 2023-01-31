using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using GPLX.Core.Contracts.Funtions;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GPLX.Core.Data.Function
{
    public class FunctionRepository : IFunctionRepository
    {
        private readonly ILogger<FunctionRepository> _logger;
        private readonly Context _ctx;

        public FunctionRepository(ILogger<FunctionRepository> logger, Context ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<IList<Functions>> GetAll()
        {
            try
            {
                var q = await _ctx.Functions.ToListAsync();
                return q;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                return null;
            }
        }
    }
}
