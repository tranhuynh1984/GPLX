using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Payment;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ILogger<PaymentRepository> _logger;
        private readonly Context _ctx;

        public PaymentRepository(ILogger<PaymentRepository> logger, Context ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }
        public async Task<IList<Database.Models.Payment>> AllPayments()
        {
            try
            {
                var query = await _ctx.Payments.ToListAsync();
                return query;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e,e.Message);
                return null;
            }
        }

        public async Task<IList<CostEstimateItemType>> AllTypes(string unitType)
        {
            try
            {
                var query = await _ctx.CostEstimateItemTypes.Where(x => x.ForUnitType.Contains(unitType)).ToListAsync();
                return query;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return null;
            }
        }
    }
}
