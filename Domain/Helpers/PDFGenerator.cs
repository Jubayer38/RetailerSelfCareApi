///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	
///	Creation Date :	17-Jan-2024
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl    No. Date:		 Author:			Ver:	   Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.ResponseModel;
using Domain.StaticClass;
using Domain.ViewModel;
using Domain.ViewModel.LogModels;
using System.Text;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Domain.Helpers
{
    public class PDFGenerator
    {
        public static byte[] ConvertHtmlTextToPdf(VMCommissionReport model, IConverter _converter, string salescommDataMonth = "thismonth")
        {
            try
            {
                string htmlStr = string.Empty;
                switch (model.ReportType)
                {
                    case "Commission":
                        htmlStr = CommissionHtmlStr(model);
                        break;
                    case "SalesVsIncome":
                        htmlStr = SalesVsIncomeHtmlStr(model);
                        break;
                    case "SalesVsCommission":
                        htmlStr = SalesVsCommissionHtmlStr(model, salescommDataMonth);
                        break;
                    case "RStatement":
                        htmlStr = RetailerStatementHtmlStr(model);
                        break;
                }

                if (string.IsNullOrEmpty(htmlStr)) return [];

                string rootPath = Directory.GetCurrentDirectory();
                string cssFile = string.Empty;

                if(AppSettingsKeys.IsWindows)
                    cssFile = Path.Combine(rootPath, "wwwroot\\css\\commission.css");
                else
                    cssFile = $"{rootPath}/wwwroot/css/commission.css";

                var document = new HtmlToPdfDocument
                {
                    GlobalSettings =
                    {
                        Outline = true,
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings
                        {   Top = 0.8,
                            Right = 0.5,
                            Bottom = 0.5,
                            Left = 0.5,
                            Unit = Unit.Inches
                        },
                        DocumentTitle = "CommissionReport"
                    },

                    Objects =
                    {
                        new ObjectSettings
                        {
                            HeaderSettings = { FontName = "kalpurush", FontSize = 10, Center = model.PageHeader, Line = true, Spacing = 2.812 },
                            PagesCount = true,
                            HtmlContent = htmlStr,
                            WebSettings =
                            {
                                DefaultEncoding = "utf-8",
                                MinimumFontSize = 10,
                                PrintMediaType = true,
                                UserStyleSheet= cssFile,
                            },
                            FooterSettings =
                            {
                                FontName = "kalpurush",
                                FontSize = 9,
                                Right = "Page [page] of [toPage]",
                                Line = true,
                                Spacing = 2.5,
                            },
                        },
                    },
                };

                SynchronizedConverter converter = PdfConverter.Instance;
                byte[] fileBytes = converter.Convert(document);

                return fileBytes;

            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "ConvertHtmlTextToPdf"));
            }
        }


        #region==========| Start of Private Methods |==========

        private static string CommissionHtmlStr(VMCommissionReport model)
        {
            try
            {
                string totalStrByLan = model.lan == "bn" ? "মোটঃ" : "Total:";
                StringBuilder theadrow = new();
                StringBuilder tbodyrows = new();
                StringBuilder tfootrows = new();
                StringBuilder disclaimer = new();

                theadrow.AppendFormat("<th style=\"width:220px;\">{0}</th>", model.ReportHeaders[0]);
                theadrow.AppendFormat("<th>{0}</th>", model.ReportHeaders[1]);
                theadrow.AppendFormat("<th style=\"width:170px;\">{0}</th>", model.ReportHeaders[2]);

                foreach (CommissionDetails item in model.DataModel.Items)
                {
                    tbodyrows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>৳{2}</td></tr>", item.date, item.description, item.commissionDisbursed);
                }

                tfootrows.AppendFormat("<td></td><td>{0}</td><td>৳{1}</td>", totalStrByLan, model.DataModel.total);
                disclaimer.Append("This is electronically generated statement which is valid without signature. Discuss with your RSO for any concern.");

                StringBuilder htmlSb = new();
                htmlSb.AppendFormat("<div id=\"dailyCommissionDiv\"><table id=\"dailyCommissionTbl\"><thead><tr>{0}</tr></thead><tbody>{1}</tbody><tfoot><tr>{2}</tr></tfoot></table><p>{3}</p></div>", theadrow, tbodyrows, tfootrows, disclaimer);

                return htmlSb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "CommissionHtmlStr"));
            }
        }

        private static string SalesVsIncomeHtmlStr(VMCommissionReport model)
        {
            try
            {
                string totalStrByLan = model.lan == "bn" ? "মোটঃ" : "Total:";
                string dateCol = model.lan == "bn" ? "তারিখ" : "Date";
                string ce = model.lan == "bn" ? "অর্জিত কমিশন" : "Commission Earned";
                string cd = model.lan == "bn" ? "প্রাপ্ত কমিশন" : "Commission Disbursed";
                string ait = model.lan == "bn" ? "অগ্রিম আয়কর" : "AIT";
                string oc = model.lan == "bn" ? "অন্যান্য চার্জ" : "Other Charges";
                string dic = model.lan == "bn" ? "ডিসকাউন্ট" : "Discount";
                string its = model.lan == "bn" ? "iTopUP বিক্রি" : "iTopUp Sales";
                string scs = model.lan == "bn" ? "স্ক্র্যাচ কার্ড বিক্রি" : "Scratch and Sales";
                string simsc = model.lan == "bn" ? "সিম বিক্রি (সংখ্যা)" : "SIM Sales (Count)";
                string simsa = model.lan == "bn" ? "সিম বিক্রি (পরিমাণ)" : "SIM Sales (Amount)";

                StringBuilder incomeTblRows = new();
                incomeTblRows.AppendFormat("<tr><td>OTF (NRT)</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.otf + "</td></tr>");
                incomeTblRows.AppendFormat("<tr><td>" + ce + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.commissionEarned + "</td></tr>");
                incomeTblRows.AppendFormat("<tr><td>" + cd + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.commissionDisbursed + "</td></tr>");
                incomeTblRows.AppendFormat("<tr><td>" + ait + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.ait + "</td></tr>");
                incomeTblRows.AppendFormat("<tr><td>" + oc + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.otherCharge + "</td></tr>");
                incomeTblRows.AppendFormat("<tr><td>" + dic + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.discount + "</td></tr>");

                StringBuilder incomeFootRows = new();
                incomeFootRows.AppendFormat("<tr><td>" + totalStrByLan + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.incomeTotal + "</td></tr>");

                StringBuilder incomeTbl = new();
                incomeTbl.AppendFormat("<table id=\"incometbl\"><tbody>{0}</tbody><tfoot>{1}</tfoot></table>", incomeTblRows, incomeFootRows);

                StringBuilder salesTblRows = new();
                salesTblRows.AppendFormat("<tr><td>" + its + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.iTopUpSales + "</td></tr>");
                salesTblRows.AppendFormat("<tr><td>" + scs + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.scSales + "</td></tr>");
                salesTblRows.AppendFormat("<tr><td>" + simsc + "</td><td style=\"width: 10px;\">:</td><td>" + model.DataModel.simSalesCount + "</td></tr>");
                salesTblRows.AppendFormat("<tr><td>" + simsa + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.simSalesAmount + "</td></tr>");

                StringBuilder salesFootRows = new();
                salesFootRows.AppendFormat("<tr><td>" + totalStrByLan + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.salesTotal + "</td></tr>");

                StringBuilder salesTbl = new();
                salesTbl.AppendFormat("<table id=\"salestbl\"><tbody>{0}</tbody><tfoot>{1}</tfoot></table>", salesTblRows, salesFootRows);

                StringBuilder disclaimer = new();
                disclaimer.Append("This is electronically generated statement which is valid without signature. Discuss with your RSO for any concern.");

                StringBuilder htmlSb = new();
                htmlSb.AppendFormat("<div id=\"salesVsIncomeDiv\">" +
                    "<p class=\"dateRow\"><span>" + dateCol + "</span><span>:</span><span>" + model.DataModel.month + "</span></p>" +
                    "<div id=\"incomeDiv\"><p id=\"tag\"><span>" + model.ReportHeaders[1] + "</span></p>{0}</div>" +
                    "<div id=\"salesDiv\"><p id=\"tag\"><span>" + model.ReportHeaders[2] + "</span></p>{1}</div>" +
                    "<p class=\"disclaimer\">{2}</p></div>", incomeTbl, salesTbl, disclaimer);

                return htmlSb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SalesVsIncomeHtmlStr"));
            }
        }

        private static string RetailerStatementHtmlStr(VMCommissionReport model)
        {
            try
            {
                #region Summary Table Design

                string month = model.lan == "bn" ? "তারিখঃ " : "Statement Date: ";
                string rn = model.lan == "bn" ? "রিটেইলারের নামঃ " : "Retailer Name: ";
                string rc = model.lan == "bn" ? "রিটেইলারের কোডঃ " : "Retailer Code: ";
                string add = model.lan == "bn" ? "ঠিকানাঃ " : "Address: ";
                string mn = model.lan == "bn" ? "মোবাইল নম্বরঃ " : "Mobile No: ";
                string prvBlnc = model.lan == "bn" ? "পূর্ববর্তী ব্যালেন্সঃ " : "Previous Balance: ";
                string rl = model.lan == "bn" ? "রিচার্জ লিফটিংঃ " : "Recharge Lifting: ";
                string siml = model.lan == "bn" ? "সিম লিফটিংঃ " : "SIM Lifting: ";
                string scl = model.lan == "bn" ? "স্ক্র্যাচ কার্ড লিফটিংঃ " : "SC Lifting: ";
                string tc = model.lan == "bn" ? "মোট কমিশনঃ " : "Total Commission: ";
                string lpe = model.lan == "bn" ? "অর্জিত লয়েলিটি পয়েন্টঃ " : "Loyality Point Earned: ";
                string lpr = model.lan == "bn" ? "ব্যবহৃত লয়েলিটি পয়েন্টঃ " : "Loyality Point Redeemed: ";
                string alp = model.lan == "bn" ? "লয়েলিটি পয়েন্টঃ " : "Available Loyality Point: ";
                string clBlnc = model.lan == "bn" ? "ক্লোজিং ব্যালেন্সঃ " : "Closing Balance: ";

                StatementMasterModel dm = model.DataModel;
                StringBuilder summaryTbltbodyrows = new();
                summaryTbltbodyrows.AppendFormat("<tr><td>" + month + "{0}</td><td>" + prvBlnc + "৳{1}</td></tr>", dm.month, dm.previousBalance);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + rn + "{0}</td><td>" + rl + "৳{1}</td></tr>", dm.retailerName, dm.rechargeLifting);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + rc + "{0}</td><td>" + siml + "৳{1}</td></tr>", dm.retailerCode, dm.simLifting);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + add + "{0}</td><td>" + scl + "৳{1}</td></tr>", dm.retailerCode, dm.scLifting);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + mn + "{0}</td><td>" + tc + "৳{1}</td></tr>", dm.retailerMsisdn, dm.totalCommission);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + lpe + "৳{0}</td><td>" + lpr + "৳{1}</td></tr>", dm.loyaltiPointEarned, dm.loyaltiPointRedeemed);
                summaryTbltbodyrows.AppendFormat("<tr><td>" + alp + "৳{0}</td><td>" + clBlnc + "৳{1}</td></tr>", dm.availableLoyaltiPoint, dm.closingBalance);

                StringBuilder summaryTbl = new();
                summaryTbl.AppendFormat("<table class=\"summarytbl\"><tbody>{0}</tbody></table>", summaryTbltbodyrows);

                #endregion

                #region Details Table Design

                string totalStrByLan = model.lan == "bn" ? "মোটঃ" : "Total:";

                StringBuilder detailTbltheadrow = new();
                foreach (string item in model.ReportHeaders)
                {
                    detailTbltheadrow.Append("<th>" + item + "</th>");
                }

                StringBuilder detailTbltbodyrows = new();
                foreach (StatementDetailsModel item in dm.statementDetails)
                {
                    detailTbltbodyrows.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>৳{2}</td><td>৳{3}</td><td>৳{4}</td><td>৳{5}</td><td>৳{6}</td></tr>", item.date, item.description, item.liftingAmount, item.salesAmount, item.commission, item.advanceIncomeTax, item.amountReceived);
                }

                StringBuilder detailTbltfootrow = new();
                detailTbltfootrow.AppendFormat("<td colspan=\"6\">{0}</td><td>৳{1}</td>", totalStrByLan, dm.total);

                StringBuilder detailTbl = new();
                detailTbl.AppendFormat("<table class=\"detailtbl\"><thead><tr>{0}</tr></thead><tbody>{1}</tbody><tfoot><tr>{2}</tr></tfoot></table>", detailTbltheadrow, detailTbltbodyrows, detailTbltfootrow);

                #endregion

                StringBuilder disclaimer = new();
                disclaimer.Append("This is electronically generated statement which is valid without signature. Discuss with your RSO for any concern.");

                StringBuilder htmlSb = new();
                htmlSb.AppendFormat("<div id=\"retailerStatementDiv\"><div id=\"summaryDiv\">{0}</div><br /><div id=\"detailDiv\">{1}</div><p class=\"disclaimer\">{2}</p></div>", summaryTbl, detailTbl, disclaimer);

                return htmlSb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "RetailerStatementHtmlStr"));
            }
        }

        private static string SalesVsCommissionHtmlStr(VMCommissionReport model, string dataMonth)
        {
            try
            {
                string dateColStr = model.lan == "bn" ? "বিবৃতি মাস" : "Statement Month";
                string totalSalesStr = model.lan == "bn" ? "সর্বমোট বিক্রি" : "Total Sales";
                string totalCommissionStr = model.lan == "bn" ? "সর্বমোট কমিশন" : "Total Commission";
                string simCommissionStr = model.lan == "bn" ? "সিম কমিশন" : "SIM Commission";
                string upfrontStr = model.lan == "bn" ? "আপফ্রন্ট" : "Upfront";
                string activationCommStr = model.lan == "bn" ? "এক্টিভেশন কমিশন" : "Activation Commission";
                string frCommissionStr = model.lan == "bn" ? "FR কমিশন" : "FR Commission";
                string complianceStr = model.lan == "bn" ? "কমপ্লায়েন্স" : "Compliance";
                string itopUpCommStr = model.lan == "bn" ? "iTopUp কমিশন" : "iTopUp Commission";
                string amarOfferStr = model.lan == "bn" ? "আমার অফার" : "Amar Offer";
                string regularCommStr = model.lan == "bn" ? "রেগুলার কমিশন" : "Regular Commission";
                string campaignCommStr = model.lan == "bn" ? "ক্যাম্পেইন কমিশন" : "Campaign Commission";
                string iTopUpStr = model.lan == "bn" ? "iTopUp" : "iTopUp";
                string simSalesCountStr = model.lan == "bn" ? "সিম বিক্রি (সংখ্যা)" : "SIM Sales (Count)";

                DateTime today = DateTime.Today;
                string monthYear = dataMonth.Equals("previousmonth", StringComparison.OrdinalIgnoreCase) ? today.AddMonths(-1).ToEnUSDateString("MMM-yyyy") : today.ToEnUSDateString("MMM-yyyy");

                StringBuilder commissionTblRows = new();
                commissionTblRows.AppendFormat("<tr id=\"tagHeader\"><td>" + simCommissionStr + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.totalSIMCommission + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + upfrontStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.simUpfront + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + activationCommStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.activationCommission + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + frCommissionStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.frCommission + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + complianceStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.compliance + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagHeader\"><td>" + itopUpCommStr + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.totalITopUpCommission + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + amarOfferStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.amarOffer + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + regularCommStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.regularCommission + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagSubHeader\"><td><span>" + upfrontStr + "</span></td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.iTopUpUpfront + "</td></tr>");
                commissionTblRows.AppendFormat("<tr id=\"tagHeader\"><td>" + campaignCommStr + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.totalCampaignCommission + "</td></tr>");

                StringBuilder commissionTbl = new();
                commissionTbl.AppendFormat("<table id=\"incometbl\"><tbody>{0}</tbody></table>", commissionTblRows);

                StringBuilder salesTblRows = new();
                salesTblRows.AppendFormat("<tr id=\"tagHeader\"><td>" + iTopUpStr + "</td><td style=\"width: 10px;\">:</td><td>৳" + model.DataModel.totalITopUpSales + "</td></tr>");
                salesTblRows.AppendFormat("<tr id=\"tagHeader\"><td>" + simSalesCountStr + "</td><td style=\"width: 10px;\">:</td><td>" + model.DataModel.totalSimSalesCount + "</td></tr>");

                StringBuilder salesTbl = new();
                salesTbl.AppendFormat("<table id=\"salestbl\"><tbody>{0}</tbody></table>", salesTblRows);

                StringBuilder disclaimer = new();
                disclaimer.Append("This is electronically generated statement which is valid without signature. Discuss with your RSO for any concern.");

                StringBuilder htmlSb = new();
                htmlSb.AppendFormat("<div id=\"salesVsIncomeDiv\">" +
                    "<p class=\"dateRow\"><span>" + dateColStr + "</span><span>:</span><span>" + monthYear + "</span></p>" +
                    "<p class=\"dateRow\"><span>" + totalSalesStr + "</span><span>:</span><span>৳" + model.DataModel.totalSales + "</span></p>" +
                    "<p class=\"dateRow\"><span>" + totalCommissionStr + "</span><span>:</span><span>৳" + model.DataModel.totalCommission + "</span></p>" +
                    "<div id=\"incomeDiv\"><p id=\"tag\"><span>" + model.ReportHeaders[1] + "</span></p>{0}</div>" +
                    "<div id=\"salesDiv\"><p id=\"tag\"><span>" + model.ReportHeaders[2] + "</span></p>{1}</div>" +
                    "<p class=\"disclaimer\">{2}</p></div>", commissionTbl, salesTbl, disclaimer);

                return htmlSb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(HelperMethod.ExMsgBuild(ex, "SalesVsCommissionHtmlStr"));
            }
        }

        #endregion==========| End of Private Methods |==========
    }
}
