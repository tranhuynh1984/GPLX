using GPLX.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models.NextStep
{
    /// <summary>
    /// Teacher
    /// </summary>
    public class Teacher : UpdateTime
    {
        public int TeacherId { get; set; }
        public string FullName { get; set; }
    }
}