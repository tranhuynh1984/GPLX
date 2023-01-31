﻿using System;
using GPLX.Core.Extensions;

namespace GPLX.Core.DTO.Request.CostStatus
{
    public class CostStatusCreateRequest
    {
        public string Record { get; set; }

        public Guid RawId => !string.IsNullOrEmpty(RequestPage) && !string.IsNullOrEmpty(Record)
            ? Guid.TryParse(Record.StringAesDecryption(RequestPage, true), out var g) ? g : Guid.Empty: Guid.Empty;

        /// <summary>
        /// Số của trạng thái
        /// i.e 0,1,2 ...
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// Tên trạng thái
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Loại trạng thái
        /// Trạng thái của yêu cầu | Trạng thái của dự trù
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Trạng thái để xác định xem có sử dụng hay không
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Dùng cho loại nào
        /// Năm - tuần
        /// </summary>
        public string StatusForCostEstimateType { get; set; }
        /// <summary>
        /// Trạng thái dùng cho đối tượng nào
        /// SUB | Đơn vị thành viên
        /// </summary>
        public string StatusForSubject { get; set; }

        /// <summary>
        /// Thứ tự của trạng thái
        /// Để xác định tiếp theo là trạng thái nào
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Trạng thái duyệt
        /// Bởi vì cùng level nhưng  trạng thái khác nhau
        /// Phê duyệt - Từ chối
        /// Nếu cột này = 1 -> trạng thái duyệt
        /// </summary>
        public int IsApprove { get; set; }
        public bool Signed { get; set; }

        public string RequestPage { get; set; }

        public int Creator { get; set; }
        public string CreatorName { get; set; }

    }
}