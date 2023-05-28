using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HelpDeskSystem.Models;
using Interfaces.Model.Account;
using Interfaces.Constants;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using Interfaces.Model.EmailInfoLabel;
using static Interfaces.Model.EmailInfoLabel.ReportOverviewResponse;
using Org.BouncyCastle.Asn1.Ocsp;
using Interfaces.Base;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Interfaces.Model.EmailInfo;
using static Interfaces.Model.EmailInfo.PerformentMonitorTotalResponse;
using static Interfaces.Model.EmailInfo.PerformentMonitorResponse;
using static Interfaces.Model.EmailInfoLabel.LabelDistributionResponse;
using System.Drawing;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly EF_DataContext _context;

        public ReportsController(EF_DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Overview")]
        public async Task<ReportOverviewResponse> Overview([FromQuery] ReportOverviewRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();

            reportOverview.Opened.Total = listEmailInfo.Where(r => r.status == 1).Count();
            reportOverview.Resolved.Total = listEmailInfo.Where(r => r.status == 2).Count();
            reportOverview.Unattended.Total = 0;
            reportOverview.Unassigned.Total = listEmailInfo.Where(r => r.isAssign == false).Count();

            ReportOverview reportOverviewUpDown = new ReportOverview();            
            double ttt = request.toDate.Subtract(request.fromDate).TotalDays;
            var listEmailInfoBefor = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date <= request.fromDate.ToUniversalTime() && r.date >= request.fromDate.AddDays(-ttt).ToUniversalTime()).ToList();

            reportOverviewUpDown.Opened.UpDown = reportOverview.Opened.Total -listEmailInfoBefor.Where(r => r.status == 1).Count();
            reportOverviewUpDown.Resolved.UpDown = reportOverview.Resolved.Total - listEmailInfoBefor.Where(r => r.status == 2).Count();
            reportOverviewUpDown.Unattended.UpDown = reportOverview.Unattended.Total - 0;
            reportOverviewUpDown.Unassigned.UpDown = reportOverview.Unassigned.Total - listEmailInfoBefor.Where(r => r.isAssign == false).Count();


            //ListPerformentMonitor reportData = GetReportData(request.fromDate, request.toDate, listEmailInfo, request.idCompany);

            return new ReportOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                result = reportOverview
            };
        }

        [HttpGet]
        [Route("PerformentMonitor")]
        public async Task<PerformentMonitorResponse> PerformentMonitor([FromQuery] ReportRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();

            ObjectPerformentMonitor resutl = PerformentMonitor(request.fromDate, request.toDate, listEmailInfo, request.idCompany, request.type);

            return new PerformentMonitorResponse
            {
                Status = ResponseStatus.Susscess,
                Result = resutl
            };
        }

        [HttpGet]
        [Route("PerformentMonitorTotal")]
        public async Task<PerformentMonitorTotalResponse> PerformentMonitorTotal([FromQuery] PerformentMonitorTotalRequest request)
        {
            ObjectPerformentMonitorTotal result = new ObjectPerformentMonitorTotal();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();


            result.Total = listEmailInfo.GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).Count();

            result.Incoming = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 1
                        && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList()
                        .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).Count();


            result.Outgoing = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 2
                        && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList()
                        .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).Count();


            result.Resolved = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).Count();

            List<ObjectReportTime> listObjectReportTime = new List<ObjectReportTime>();
            foreach (EmailInfo obj in listEmailInfo)
            {
                EmailInfo objReply = _context.EmailInfos.Where(r => r.idReference == obj.messageId && r.type == 2 && r.id != obj.id).FirstOrDefault();
                if (objReply != null)
                {
                    ObjectReportTime obj1 = new ObjectReportTime();
                    double time = objReply.date.Value.Subtract(obj.date.Value).TotalSeconds;
                    obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                    obj1.value = time;
                    listObjectReportTime.Add(obj1);
                }
            }

            result.ResponeTime = listObjectReportTime.GroupBy(r => r.date)
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Average(p => p.value)
                       }).Count();


            result.ResolveTime = 0;

            return new PerformentMonitorTotalResponse
            {
                Status = ResponseStatus.Susscess,
                Result = result
            };
        }

        private ObjectPerformentMonitor PerformentMonitor(DateTime fromDate, DateTime toDate, List<EmailInfo> listEmailInfo, int idCompany, int type)
        {
            ObjectPerformentMonitor result = new ObjectPerformentMonitor();

            DateTime dt = fromDate;
            if (type == 1)//total
            {
                var listReport = listEmailInfo.GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }
            else if (type == 2)//Incoming
            {
                var listReport = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.isDelete == false && r.type == 1
                            && r.date >= fromDate.ToUniversalTime() && r.date <= toDate.ToUniversalTime()).ToList()
                            .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }
            else if (type == 3)//Outgoing
            {
                var listReport = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.isDelete == false && r.type == 2
                            && r.date >= fromDate.ToUniversalTime() && r.date <= toDate.ToUniversalTime()).ToList()
                            .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }
            else if (type == 4)//IncResolveoming
            {
                var listReport = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }
            else if (type == 5)//FirstRespone
            {
                List<ObjectReportTime> listObjectReportTime = new List<ObjectReportTime>();
                foreach (EmailInfo obj in listEmailInfo)
                {
                    EmailInfo objReply = _context.EmailInfos.Where(r => r.idReference == obj.messageId && r.type == 2 && r.id != obj.id).FirstOrDefault();
                    if (objReply != null)
                    {
                        ObjectReportTime obj1 = new ObjectReportTime();
                        double time = objReply.date.Value.Subtract(obj.date.Value).TotalSeconds;
                        obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                        obj1.value = time;
                        listObjectReportTime.Add(obj1);
                    }
                }


                var listReport = listObjectReportTime.GroupBy(r => r.date)
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Average(p => p.value)
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = (int)objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }
            else if (type == 6)//Incoming
            {
                List<ObjectReportTime> listObjectResolvedTime = new List<ObjectReportTime>();
                foreach (EmailInfo obj in listEmailInfo)
                {
                    EmailInfo objResolved = _context.EmailInfos.Where(r => r.idReference == obj.messageId && r.status == Common.Resolved && r.id != obj.id).FirstOrDefault();
                    if (objResolved != null)
                    {
                        ObjectReportTime obj1 = new ObjectReportTime();
                        double time = objResolved.date.Value.Subtract(obj.date.Value).TotalSeconds;
                        obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                        obj1.value = time;
                        listObjectResolvedTime.Add(obj1);
                    }
                }


                var listReport = listObjectResolvedTime.GroupBy(r => r.date)
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Average(p => p.value)
                           }).ToList();

                while (dt <= toDate)
                {
                    //add total
                    var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                    ObjectReport objTotal = new ObjectReport();
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    if (objectEmailInfo == null)
                    {
                        objTotal.value = 0;
                    }
                    else
                    {
                        objTotal.value = (int)objectEmailInfo.value;
                    }
                    result.data.Add(objTotal.value);

                    dt = dt.AddDays(1);
                }
            }

            return result;
        }
        private ListPerformentMonitor GetReportData(DateTime fromDate, DateTime toDate, List<EmailInfo> listEmailInfo, int idCompany) 
        {
            var listReport = listEmailInfo.GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();

            var listReportIncoming = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.isDelete == false && r.type == 1
                        && r.date >= fromDate.ToUniversalTime() && r.date <= toDate.ToUniversalTime()).ToList()
                        .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();


            var listReportOutgoing = _context.EmailInfos.Where(r => r.idCompany == idCompany && r.isDelete == false && r.type == 2
                        && r.date >= fromDate.ToUniversalTime() && r.date <= toDate.ToUniversalTime()).ToList()
                        .GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();


            var listReportResolve = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();

            List<ObjectReportTime> listObjectReportTime = new List<ObjectReportTime>();
            foreach (EmailInfo obj in listEmailInfo)
            {
                EmailInfo objReply = _context.EmailInfos.Where(r => r.idReference == obj.messageId && r.type == 2 && r.id != obj.id).FirstOrDefault();
                if (objReply != null)
                {
                    ObjectReportTime obj1 = new ObjectReportTime();
                    double time = objReply.date.Value.Subtract(obj.date.Value).TotalSeconds;
                    obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                    obj1.value = time;
                    listObjectReportTime.Add(obj1);
                }
            }


            var listFirstRespone = listObjectReportTime.GroupBy(r => r.date)
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Average(p => p.value)
                       }).ToList();

            ListPerformentMonitor result = new ListPerformentMonitor();
            result.Total = new ObjectPerformance();
            result.Total.Report = new List<ObjectReport>();
            result.Total.SumReport = listReport.Count();
            result.Incoming = new ObjectPerformance();
            result.Incoming.Report = new List<ObjectReport>();
            result.Incoming.SumReport = listReportIncoming.Count();
            result.Outgoing = new ObjectPerformance();
            result.Outgoing.Report = new List<ObjectReport>();
            result.Outgoing.SumReport = listReportOutgoing.Count();
            result.Resolved = new ObjectPerformance();
            result.Resolved.Report = new List<ObjectReport>();
            result.Resolved.SumReport = listReportResolve.Count();
            result.ResponeTime = new ObjectPerformance();
            result.ResponeTime.Report = new List<ObjectReport>();
            result.ResponeTime.SumReport = (int)listFirstRespone.Sum(r => r.value);
            DateTime dt = fromDate;
            while (dt < toDate)
            {
                //add total
                var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                ObjectReport objTotal = new ObjectReport();
                objTotal.key = dt.ToString("dd-MM-yyyy");
                if (objectEmailInfo == null)
                {
                    objTotal.value = 0;
                }
                else
                {
                    objTotal.value = objectEmailInfo.value;
                }
                result.Total.Report.Add(objTotal);

                //add incoming
                var objectIncoming = listReportIncoming.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                ObjectReport objIncoming = new ObjectReport();
                objIncoming.key = dt.ToString("dd-MM-yyyy");
                if (objectIncoming == null)
                {
                    objIncoming.value = 0;
                }
                else
                {
                    objIncoming.value = objectIncoming.value;
                }
                result.Incoming.Report.Add(objIncoming);

                //add outgoing
                var objectOutgoing = listReportOutgoing.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                ObjectReport objOutgoing = new ObjectReport();
                objOutgoing.key = dt.ToString("dd-MM-yyyy");
                if (objectOutgoing == null)
                {
                    objOutgoing.value = 0;
                }
                else
                {
                    objOutgoing.value = objectOutgoing.value;
                }
                result.Outgoing.Report.Add(objOutgoing);


                //add resolve
                var objectResolve = listReportResolve.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                ObjectReport objResolve = new ObjectReport();
                objResolve.key = dt.ToString("dd-MM-yyyy");
                if (objectResolve == null)
                {
                    objResolve.value = 0;
                }
                else
                {
                    objResolve.value = objectResolve.value;
                }
                result.Resolved.Report.Add(objResolve);

                //add respone time
                var objectResponeTime = listFirstRespone.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                ObjectReport objResponeTime = new ObjectReport();
                objResponeTime.key = dt.ToString("dd-MM-yyyy");
                if (objectResponeTime == null)
                {
                    objResponeTime.value = 0;
                }
                else
                {
                    objResponeTime.value = (int)objectResponeTime.value;
                }
                result.ResponeTime.Report.Add(objResponeTime);


                dt = dt.AddDays(1);
            }

            return result;
        }

        [HttpGet]
        [Route("LabelDistribution")]
        public async Task<LabelDistributionResponse> LabelDistribution([FromQuery] LabelDistributionRequest request)
        {
            List<LabelDistribution> result = new List<LabelDistribution>();

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true
            && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();
            List<int> listIdEmail = new List<int>();
            foreach (EmailInfo emailInfo in listEmailInfo)
            {
                listIdEmail.Add(emailInfo.id);
            }

            var listEmailInfoLabel = _context.EmailInfoLabels.Where(r => (r.idLabel == request.idLabel || request.idLabel == 0) && listIdEmail.Contains(r.idEmailInfo.Value)).ToList()
                .GroupBy(t => t.idLabel.Value)
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();


            Random rnd = new Random();
            List<string> colors = new List<string>();
            List<LabelDistributionTable> listLabelDistributionTable = new List<LabelDistributionTable>();
            List<TopTrendingLabel> listTopTrendingLabel = new List<TopTrendingLabel>();

            int Sum = listEmailInfoLabel.Sum(r => r.value);
            foreach (var emailInfoLabel in listEmailInfoLabel)
            {
                LabelDistributionTable labelDistributionTable = new LabelDistributionTable();
                LabelDistribution obj = new LabelDistribution();
                TopTrendingLabel topTrendingLabel = new TopTrendingLabel();
                var label = _context.Labels.Where(r => r.id == emailInfoLabel.key).FirstOrDefault();
                obj.Name = "#" + label.name;
                obj.Y = Math.Round((decimal)emailInfoLabel.value / Sum, 2);

                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = ColorTranslator.ToHtml(randomColor);
                colors.Add(colorHex);
                result.Add(obj);

                labelDistributionTable.Name = "#" + label.name;
                labelDistributionTable.ClassName = colorHex;
                labelDistributionTable.Distribution = Math.Round((obj.Y * 100), 0).ToString() + "%";
                labelDistributionTable.Conversation = emailInfoLabel.value.ToString();

                topTrendingLabel.Name = "#" + label.name;
                topTrendingLabel.UserTime = "0";
                topTrendingLabel.Conversation = emailInfoLabel.value.ToString();

                listLabelDistributionTable.Add(labelDistributionTable);
                listTopTrendingLabel.Add(topTrendingLabel);
            }

            return new LabelDistributionResponse
            {
                Status = ResponseStatus.Susscess,
                result = result,
                colors = colors,
                resultTable = listLabelDistributionTable,
                topTrending = listTopTrendingLabel
            };
        }
    }

}
