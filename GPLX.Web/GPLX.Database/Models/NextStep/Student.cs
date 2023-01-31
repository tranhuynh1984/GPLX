using GPLX.Database.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Database.Models.NextStep
{
    /// <summary>
    /// Student
    /// </summary>
    public class Student : UpdateTime
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public DateTime DOB { get; set; }
        public int TeacherId { get; set; }
    }
}