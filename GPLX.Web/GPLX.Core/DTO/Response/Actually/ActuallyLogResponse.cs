﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response.Actually
{
    public class ActuallyLogResponse
    {
        public string TimeChange { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        //Tên chức vụ
        public string PositionName { get; set; }
    }
}
