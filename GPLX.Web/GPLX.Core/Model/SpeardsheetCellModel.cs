using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.Model
{
    public class SpeardSheetCells
    {
        public Dictionary<int, SpeardSheetCell> cells { get; set; }
        public int height { get; set; }
    }


    public class SpeardSheetCell
    {
        public string text { get; set; }
        public int style { get; set; }
    }
}
