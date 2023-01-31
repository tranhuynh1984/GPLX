using GPLX.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPLX.Core.Model
{
    public class LuckySheetCellModel
    {
        public int r { get; set; }
        public int c { get; set; }

        public LuckySheetCell v
        {
            get; 
            set;
        }

    }

    public class LuckySheetCell
    {
        /// <summary>
        /// celltype
        /// </summary>
        public LuckySheetCellFormat ct { get; set; }
        /// <summary>
        /// background
        /// </summary>
        public string bg { get; set; }
        /// <summary>
        /// fontfamily
        /// </summary>
        public string ff { get; set; }
        /// <summary>
        /// fontcolor
        /// </summary>
        public string fc { get; set; }
        /// <summary>
        /// bold
        /// </summary>
        public int? bl { get; set; }
        /// <summary>
        /// italic
        /// </summary>
        public int? it { get; set; }
        /// <summary>
        /// fontsize
        /// </summary>
        public int? fs { get; set; }
        /// <summary>
        /// cancelline
        /// </summary>
        public int? cl { get; set; }
        /// <summary>
        /// vt
        /// </summary>
        public int? vt { get; set; }
        /// <summary>
        /// horizontaltype
        /// </summary>
        public int? ht { get; set; }
        /// <summary>
        /// mc
        /// </summary>
        public LuckyMergeCell mc { get; set; }
        /// <summary>
        /// textrotate
        /// </summary>
        public int? tr { get; set; }
        /// <summary>
        /// rotatetext
        /// </summary>
        public int? rt { get; set; }
        /// <summary>
        /// textbeak
        /// </summary>
        public int? tb { get; set; } = 1;
        /// <summary>
        /// value
        /// </summary>
        public string v { get; set; }
        /// <summary>
        /// monitor
        /// </summary>
        public string m { get; set; }
        /// <summary>
        /// function
        /// </summary>
        public string f { get; set; }
        /// <summary>
        /// comment
        /// </summary>
        public string ps { get; set; }
    }

    public class LuckyMergeCell
    {
        public int? r { get; set; }
        public int? c { get; set; }
        public int? rs { get; set; }
        public int? cs { get; set; }
    }

    public class LuckySheetCellFormat
    {
        public string fa { get; set; }
        public string t { get; set; }
        public List<LuckyCellData> s { get; set; }
    }

    public class LuckyCellData
    {
        public string ff { get; set; }
        public string fc { get; set; }
        public int fs { get; set; }
        public string v { get; set; }
    }


    public class LuckyCell
    {
        public LuckySheetCellFormat format { get; set; }
        public LuckySheetCell cell { get; set; }

        public LuckyCell(string value, LuckyCellStyle style, LuckySheetCellFormat format = default,string function = default)
        {
            if (!Style.ContainsKey(style))
                cell = new LuckySheetCell
                {
                    v = value,
                    m = value,
                    bg = "#ffffff",
                    bl = 0,
                    ht = 1
                };
            else
            {
                Style[style].v = value;
                Style[style].m = value;
                cell = Style[style];
            }

            if (!string.IsNullOrEmpty(function))
                cell.f = function;

            if (format != null)
                this.format = format;
            else
                this.format = new LuckySheetCellFormat
                {
                    fa = "General",
                    t = "n"
                };

        }

        public Dictionary<LuckyCellStyle, LuckySheetCell> Style => new Dictionary<LuckyCellStyle, LuckySheetCell>
        {
            {
                LuckyCellStyle.HeaderSheet,new LuckySheetCell
                {
                    bg = "#f4b084",
                    bl = 1,
                    ht = 0
                }
            },
            {
                LuckyCellStyle.HeaderBigGroup,new LuckySheetCell
                {
                    bg = "#e2efd9",
                    bl = 1,
                    ht = 1
                }
            },
            {
                LuckyCellStyle.HeaderGroup,new LuckySheetCell
                {
                    bg = "#deeaf6",
                    bl = 1,
                    ht = 1
                }
            },
            {
                LuckyCellStyle.HeaderSubGroup,new LuckySheetCell
                {
                    bg = "#e7e6e6",
                    bl = 1,
                    ht = 1
                }
            },
            {
                LuckyCellStyle.Normal,new LuckySheetCell
                {
                    bg = "#ffffff",
                    bl = 0,
                    ht = 1
                }
            },
            {
                LuckyCellStyle.SumFooter,new LuckySheetCell
                {
                    bg = "#fef2cb",
                    bl = 1,
                    ht = 1
                }
            },
            {
                LuckyCellStyle.HeaderTitle,new LuckySheetCell
                {
                    bg = "#fef2cb",
                    bl = 1,
                    ht = 1,
                    mc = new LuckyMergeCell {
                        r = 0,
                        c = 0,
                        rs =  2,
                        cs =  2
                    }
                }
            },
            {
                LuckyCellStyle.HeaderTable,new LuckySheetCell
                {
                    bl = 1,
                    ht = 1,
                }
            }
        };


    }
}
