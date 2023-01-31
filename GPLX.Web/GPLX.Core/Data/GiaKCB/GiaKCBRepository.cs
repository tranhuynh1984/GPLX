using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GPLX.Core.Contracts;
using GPLX.Core.Contracts.Actions;
using GPLX.Core.Contracts.DeXuat;
using GPLX.Core.Contracts.DeXuatGhiChu;
using GPLX.Core.Contracts.GiaKCB;
using GPLX.Core.DTO.Request.DeXuat;
using GPLX.Core.DTO.Request.DeXuatGhiChu;
using GPLX.Core.DTO.Response.DeXuat;
using GPLX.Core.Enum;
using GPLX.Database;
using GPLX.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace GPLX.Core.Data.GiaKCB
{
    public class GiaKCBRepository : IGiaKCBRepository
    {
        private readonly Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GiaKCBRepository> _logger;

        public GiaKCBRepository(Context context, IMapper mapper, ILogger<GiaKCBRepository> logger)
        {
            this._context = context;
            _mapper = mapper;
            _logger = logger;
        }
    }
}
