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


            List<PerformentMonitor> reportData = GetReportData(request.fromDate, request.toDate, listEmailInfo);
            //if (request.toDate.Subtract(request.fromDate).TotalDays == 7)
            //{
            //    var report = listEmailInfo.GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
            //               .Select(t => new
            //               {
            //                   date = t.Key,
            //                   Count = t.Count()
            //               }).ToList();
            //}

            return new ReportOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                reportOverview = reportOverview
            };
        }

        private List<PerformentMonitor> GetReportData(DateTime fromDate, DateTime toDate, List<EmailInfo> listEmailInfo) 
        {
            var listReport = listEmailInfo.GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();

            List<PerformentMonitor> listRespone = new List<PerformentMonitor>();
            DateTime dt = fromDate;
            while (dt < toDate)
            {
                var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                if (objectEmailInfo == null)
                {
                    PerformentMonitor obj = new PerformentMonitor();
                    obj.key = dt.ToString("dd-MM-yyyy");
                    obj.value = 0;
                    listRespone.Add(obj);
                }
                else
                {
                    PerformentMonitor obj = new PerformentMonitor();
                    obj.key = dt.ToString("dd-MM-yyyy");
                    obj.value = objectEmailInfo.value;
                    listRespone.Add(obj);
                }
                dt = dt.AddDays(1);
            }

            return listRespone;
        }
    }
}
