using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GPLX.Core.Contracts;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.Supplier
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly Context _ctx;
        private readonly ILogger<SupplierRepository> _logger;

        public SupplierRepository(Context ctx, ILogger<SupplierRepository> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<IList<Suppliers>> Gets()
        {
            return await _ctx.Suppliers.ToListAsync();

        }

        public async Task<IList<Suppliers>> Searching(string keyword)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword.Trim()))
                    return new List<Suppliers>();
                var response = await _ctx.Suppliers.Where(x => x.SupplierName.ToLower().Contains(keyword.ToLower())).ToListAsync();
                return response;
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, e.Message);
                Log.Error(e, e.Message);

                return new List<Suppliers>();
            }
        }

        public async Task<Suppliers> GetIdByName(string supplierName)
        {
            var item = await _ctx.Suppliers.FirstOrDefaultAsync(a => a.SupplierName.Equals(supplierName.Trim()));

            if (item == null)
            {
                var supplier = new Suppliers()
                {
                    SupplierName = supplierName
                };
                await _ctx.Suppliers.AddAsync(supplier);
                _ctx.SaveChanges();

                return supplier;
            }
            else
                return item;
        }
    }
}
