using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.DTO.Response
{
    public class MedApiResponse<T>
    {
        public T[] items { get; set; }
        public int totalRows { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class MedTelegramBotResponse
    {
        public string title { get; set; }
        public string message { get; set; }
        public int code { get; set; }
    }
}
