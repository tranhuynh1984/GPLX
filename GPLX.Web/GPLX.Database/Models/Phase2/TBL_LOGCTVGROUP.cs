using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GPLX.Database.Models.Phase2
{
    public class TBL_LOGCTVGROUP : UpdateTime
    {
        public long LogCtvId { get; set; }
        public string DoctorId { get; set; }
        public int CTVGroupID { get; set; }
        public int SubId { get; set; }
        public string BP { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Note { get; set; }
        public string HTCK { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //public int IsHetHan { get; set; }

        public int IsActive { get; set; }
        public int ThuSau { get; set; }
        public int LogCtvStatus { get; set; }
    }
}