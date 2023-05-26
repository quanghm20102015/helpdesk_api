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
        public async Task<ReportOverviewResponse> Overview([FromQuery] ReportRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();

            reportOverview.Opened = listEmailInfo.Where(r => r.status == 1).Count();
            reportOverview.Resolved = listEmailInfo.Where(r => r.status == 2).Count();
            reportOverview.Unattended = 0;
            reportOverview.Unassigned = listEmailInfo.Where(r => r.isAssign == false).Count();


            //ListPerformentMonitor reportData = GetReportData(request.fromDate, request.toDate, listEmailInfo, request.idCompany);

            return new ReportOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                reportOverview = reportOverview
            };
        }

        [HttpGet]
        [Route("PerformentMonitor")]
        public async Task<ReportOverviewResponse> PerformentMonitor([FromQuery] ReportRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();

            ListPerformentMonitor reportData = GetReportData(request.fromDate, request.toDate, listEmailInfo, request.idCompany);

            return new ReportOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                reportData = reportData
            };
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
    }
}
