using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.HDCTV;
using GPLX.Core.DTO.Request.HDCTV;
using GPLX.Core.DTO.Response.HDCTV;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GPLX.Core.Data.HDCTV
{
    public class HDCTVRepository : IHDCTVRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<HDCTVRepository> _logger;

        public HDCTVRepository(Context context, IMapper mapper, ILogger<HDCTVRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }
        
    }
}
