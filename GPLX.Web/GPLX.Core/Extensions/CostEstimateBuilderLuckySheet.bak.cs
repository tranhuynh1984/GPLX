using GPLX.Core.Enum;
using GPLX.Core.Model;
using GPLX.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPLX.Core.Extensions
{
    public static class CostEstimateBuilderLuckySheet2
    {
//        public static Dictionary<int, SpeardSheetCells> Build(this Dictionary<int, SpeardSheetCells> dic, IList<CostEstimateItem> items, Database.Models.CostEstimate data)
//        {
//            BuildHeader(dic, data);
//            var count = dic.Count();


//            ////Chi hoạt động thường quy
//            {
//                dic.Add(count, new SpeardSheetCells
//                {
//                    cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text = "1.1",style = 6  } },
//                            { 1,new SpeardSheetCell(){  text = "Chi hoạt động thường quy",style = 6   } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.RoutineCost),style = 6   } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 6   } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 6  } }
//                        }
//                });
//                count++;
//                BuildBody(count, items.Where(a => a.CostEstimateGroupName == "Chi hoạt động thường quy").ToList(), dic);
//                count = dic.Count();
//            }


//            //Chi hoạt động không thường quy
//            {
//                dic.Add(count, new SpeardSheetCells
//                {
//                    cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text = "1.2",style = 6  } },
//                            { 1,new SpeardSheetCell(){ text = "Chi hoạt động không thường quy",style = 6   } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.NonRoutineCost),style = 6  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 6   } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 6   } }
//                        }
//                });
//                count++;
//                BuildBody(count, items.Where(a => a.CostEstimateGroupName == "Chi hoạt động không thường quy").ToList(), dic);
//                count = dic.Count();
//            }


//            //Chi hoạt động thường quy
//            {
//                dic.Add(count, new SpeardSheetCells
//                {
//                    cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text = "2",style = 3  } },
//                            { 1,new SpeardSheetCell(){ text = "Các khoản chi đầu tư",style = 3    } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.InvestmentCost),style = 3   } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 3    } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 3    } }
//                        }
//                });
//                count++;
//                BuildBody(count, items.Where(a => a.CostEstimateGroupName == "Các khoản chi đầu tư").ToList(), dic);
//                count = dic.Count();
//            }

//            //Chi hoạt động thường quy
//            {
//                dic.Add(count, new SpeardSheetCells
//                {
//                    cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text="3",style = 3   } },
//                            { 1,new SpeardSheetCell(){ text = "Các khoản chi hoạt động tài chính",style = 3    } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.FinancialCost),style = 3   } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 3    } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 3    } }
//                        }
//                });
//                count++;
//                BuildBody(count, items.Where(a => a.CostEstimateGroupName == "Các khoản chi hoạt động tài chính").ToList(), dic);
//                count = dic.Count();
//            }




//            BuildFooter(dic, data);

//            return dic;
//        }

//        private static void BuildBody(int start, List<CostEstimateItem> items, Dictionary<int, SpeardSheetCells> dic)
//        {
//            var count = 1;
//            foreach (var item in items)
//            {
//                dic.Add(start, new SpeardSheetCells
//                {
//                    cells = new Dictionary<int, SpeardSheetCell>
//                    {
//                        { 0, new SpeardSheetCell() { text = item.RequestCode ,style=2} },
//                        { 1, new SpeardSheetCell() { text = item.RequestContent ?? "",style=2} },
//                        { 2, new SpeardSheetCell() { text = string.Format("{0:n0}", item.Cost),style=2 } },
//                        { 3, new SpeardSheetCell() { text = item.SupplierName ?? "" ,style=2} },
//                        { 4, new SpeardSheetCell() { text = item.PayForm ?? "" ,style=2} }
//                    },
//                    height = 30
//                });
//                start++;
//            }
//        }

//        private static void BuildFooter(Dictionary<int, SpeardSheetCells> dic, Database.Models.CostEstimate data)
//        {
//            var start = dic.Count();

//            dic.Add(start, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text="",style = 4  } },
//                            { 1,new SpeardSheetCell(){  text = "Tổng chi bằng vốn tự có", style = 4  } },
//                            { 2,new SpeardSheetCell(){  text = string.Format("{0:n0}", data.TotalExpenditureCapital),style = 4  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 4  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 4  } }
//                        }
//            });
//            dic.Add(start + 1, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text="",style = 4  } },
//                    { 1,new SpeardSheetCell(){ text = "Tổng chi bằng vay lưu động", style = 4  } },
//                    { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.TotalSpendingLoan),style = 4  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 4  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 4  } }
//                }
//            });
//            dic.Add(start + 2, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "IV",style = 1  } },
//                    { 1,new SpeardSheetCell(){ text = "Số dư khả dụng cuối kỳ (1+2-3)",style = 1  } },
//                    { 2,new SpeardSheetCell(){  text = string.Format("{0:n0}", data.EndAvailableBalance),style = 1  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//                }
//            });
//            dic.Add(start + 3, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "1",style = 2  } },
//                    { 1,new SpeardSheetCell(){ text = "Vốn tự có (4)",style = 2  } },
//                    { 2,new SpeardSheetCell(){  text = string.Format("{0:n0}", data.Funds),style = 2  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//                }
//            });
//            dic.Add(start + 4, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "1",style = 2  } },
//                    { 1,new SpeardSheetCell(){ text = "Số dư khả dụng vay lưu động",style = 2  } },
//                    { 2,new SpeardSheetCell(){  text = string.Format("{0:n0}", data.WorkingBalanceCost),style = 2  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//                }
//            });
//            dic.Add(start + 5, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "V",style = 1  } },
//                    { 1,new SpeardSheetCell(){ text = "Định mức tồn quỹ (5)",style = 1  } },
//                    { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.EquityCost), style = 1  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//                }
//            });
//            dic.Add(start + 6, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "VI",style = 1  } },
//                    { 1,new SpeardSheetCell(){ text = "Dự kiến cắt tiền về dòng tiền tập trung (4-5)",style = 1  } },
//                    { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.PlanCutCost),style = 1  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//                }
//            });
//            dic.Add(start + 7, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "VII",style = 1  } },
//                    { 1,new SpeardSheetCell(){ text = "Đề xuất luân chuyển tiền về ĐVTV",style = 1  } },
//                    { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.RotationProposal),style = 1  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//                }
//            });
//            dic.Add(start + 8, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "1",style = 2  } },
//                    { 1,new SpeardSheetCell(){ text = "Chuyển về thứ 3",style = 2  } },
//                    { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//                }
//            });
//            dic.Add(start + 9, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                {
//                    { 0,new SpeardSheetCell(){ text = "2",style = 2  } },
//                    { 1,new SpeardSheetCell(){ text = "Chuyển về thứ 5",style = 2  } },
//                    { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                    { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//                }
//            });
//        }

//        private static void BuildHeader(List<LuckySheetCellModel> dic, Database.Models.CostEstimate data)
//        {
//            var money_format = new LuckySheetCellFormat
//            {
//                fa= "#,##0",
//                t = "n"
//            };

//            //dự trù tên
//            // dic.AddRange(BuildCell(0, new LuckyCell[]
//            //{
//            //     new LuckyCell(data.Name,LuckyCellStyle.HeaderTitle)
//            //}.ToList()));

//            dic.AddRange(BuildCell(2, new LuckyCell[]
//           {
//                new LuckyCell("I. Dự trù chi tiết",LuckyCellStyle.HeaderTable),
//           }.ToList()));

//            dic.AddRange(BuildCell(3, new LuckyCell[]
//            {
//                new LuckyCell("STT/Mã dự trù",LuckyCellStyle.HeaderSheet),
//                new LuckyCell("Nội dung",LuckyCellStyle.HeaderSheet),
//                new LuckyCell("Số tiền",LuckyCellStyle.HeaderSheet),
//                new LuckyCell("Nhà cung cấp",LuckyCellStyle.HeaderSheet),
//                new LuckyCell("Hình thức chi",LuckyCellStyle.HeaderSheet)
//            }.ToList()));


//            dic.AddRange(BuildCell(4, new LuckyCell[]
//            {
//                new LuckyCell("I",LuckyCellStyle.HeaderBigGroup),
//                new LuckyCell("Số dư khả dụng đầu kì (1)",LuckyCellStyle.HeaderBigGroup),
//                new LuckyCell(string.Format("{0:n0}", data.BeginAvailableBalance),LuckyCellStyle.HeaderBigGroup,money_format),
//                new LuckyCell("",LuckyCellStyle.HeaderBigGroup),
//                new LuckyCell("",LuckyCellStyle.HeaderBigGroup)
//            }.ToList()));

//            dic.AddRange(BuildCell(5, new LuckyCell[]
//            {
//                new LuckyCell("1",LuckyCellStyle.Normal),
//                new LuckyCell("Tiền mặt",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal,money_format),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//            }.ToList()));

//            dic.AddRange(BuildCell(6, new LuckyCell[]
//           {
//                new LuckyCell("2",LuckyCellStyle.Normal),
//                new LuckyCell("Số dư tài khoản thanh toán",LuckyCellStyle.Normal),
//                new LuckyCell(string.Format("{0:n0}", data.AccountBalance),LuckyCellStyle.Normal,money_format),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//           }.ToList()));

//            dic.AddRange(BuildCell(7, new LuckyCell[]
//           {
//                new LuckyCell("2.1",LuckyCellStyle.Normal),
//                new LuckyCell("Ngân hàng Techcombank cty",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal,money_format),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//           }.ToList()));

//            dic.AddRange(BuildCell(8, new LuckyCell[]
//           {
//                new LuckyCell("2.2",LuckyCellStyle.Normal),
//                new LuckyCell("Ngân hàng BIDV cty",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal,money_format),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//           }.ToList()));

//            dic.AddRange(BuildCell(9, new LuckyCell[]
//           {
//                new LuckyCell("2.3",LuckyCellStyle.Normal),
//                new LuckyCell("Ngân hàng BIDV cty2 (TK chuyên thu)",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//           }.ToList()));

//            dic.AddRange(BuildCell(10, new LuckyCell[]
//          {
//                new LuckyCell("3",LuckyCellStyle.Normal),
//                new LuckyCell("Vay lưu động – số dư khả dụng",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//                new LuckyCell("",LuckyCellStyle.Normal),
//          }.ToList()));


//            dic.Add(7, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//    {
//                            { 0,new SpeardSheetCell(){ text = "3",style = 2  } },
//                            { 1,new SpeardSheetCell(){ text = "Vay lưu động – số dư khả dụng",style = 2  } },
//                            { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//    }
//            });
//            dic.Add(8, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//{
//                            { 0,new SpeardSheetCell(){ text = "II",style = 1  } },
//                            { 1,new SpeardSheetCell(){ text = "Số tiền thu dự kiến (2)",style = 1  } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.ExpectRevenue),style = 1  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//}
//            });
//            dic.Add(9, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//{
//                            { 0,new SpeardSheetCell(){ text = "1",style = 2  } },
//                            { 1,new SpeardSheetCell(){ text = "Thu từ hoạt động kinh doanh",style = 2  } },
//                            { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//}
//            });
//            dic.Add(10, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//{
//                            { 0,new SpeardSheetCell(){ text = "1",style = 2  } },
//                            { 1,new SpeardSheetCell(){ text = "Thu từ hoạt động góp vốn",style = 2  } },
//                            { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//}
//            });
//            dic.Add(11, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//{
//                            { 0,new SpeardSheetCell(){ text = "3",style = 2  } },
//                            { 1,new SpeardSheetCell(){ text = "Thu khác",style = 2  } },
//                            { 2,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 2  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 2  } }
//}
//            });
//            dic.Add(12, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text = "III",style = 1  } },
//                            { 1,new SpeardSheetCell(){ text = "Số tiền chi phí dự kiến (3)",style = 1  } },
//                            { 2,new SpeardSheetCell(){ text = string.Format("{0:n0}", data.EstimatedCost),style = 1  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 1  } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 1  } }
//                        }
//            });

//            dic.Add(13, new SpeardSheetCells
//            {
//                cells = new Dictionary<int, SpeardSheetCell>
//                        {
//                            { 0,new SpeardSheetCell(){ text = "1",style = 3  } },
//                            { 1,new SpeardSheetCell(){ text = "Các khoản chi hoạt động",style = 3 } },
//                            { 2,new SpeardSheetCell(){  text = string.Format("{0:n0}", data.OperatingCost),style = 3  } },
//                            { 3,new SpeardSheetCell(){ text = "",style = 3 } },
//                            { 4,new SpeardSheetCell(){ text = "",style = 3  } }
//                        }
//            });


//        }

//        private static IList<LuckySheetCellModel> BuildCell(int row, List<LuckyCell> data)
//        {
//            return data.Select((a, i) => new LuckySheetCellModel
//            {
//                r = row,
//                c = i,
//                v = new LuckySheetCell
//                {
//                    v = a.cell.v,
//                    m = a.cell.m,
//                    bl = a.cell.bl,
//                    bg = a.cell.bg,
//                    ht = a.cell.ht,
//                    ct = a.format
//                }
//            }).ToList();
//        }



    }
}
