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
using static Interfaces.Model.EmailInfoLabel.TopConversationAgentResponse;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using static Interfaces.Model.EmailInfoLabel.ReportAgentOverviewResponse;
using static Interfaces.Model.EmailInfoLabel.CsatResponeDistributionResponse;
using System.Linq.Expressions;
using static Interfaces.Model.EmailInfoLabel.CsatOverviewResponse;
using static Interfaces.Model.EmailInfoLabel.TrafficResponse;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

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

            reportOverview.Opened.UpDown = reportOverview.Opened.Total -listEmailInfoBefor.Where(r => r.status == 1).Count();
            reportOverview.Resolved.UpDown = reportOverview.Resolved.Total - listEmailInfoBefor.Where(r => r.status == 2).Count();
            reportOverview.Unattended.UpDown = reportOverview.Unattended.Total - 0;
            reportOverview.Unassigned.UpDown = reportOverview.Unassigned.Total - listEmailInfoBefor.Where(r => r.isAssign == false).Count();


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


            result.Total = listEmailInfo.Count();

            result.Incoming = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 1
                        && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList().Count();


            result.Outgoing = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 2
                        && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList().Count();


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


            double timeSecond = listObjectReportTime.GroupBy(r => r.date)
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Average(p => p.value)
                       }).ToList().Sum(r => r.value);

            TimeSpan timeSpan = TimeSpan.FromSeconds(timeSecond);
            string str = timeSpan.ToString(@"HH\:mm\:ss\");


            result.ResponeTime = str;
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

        [HttpPost]
        [Route("LabelDistribution")]
        public async Task<LabelDistributionResponse> LabelDistribution([FromBody] LabelDistributionRequest request)
        {
            List<LabelDistribution> result = new List<LabelDistribution>();

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true
            && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();
            List<int> listIdEmail = new List<int>();
            foreach (EmailInfo emailInfo in listEmailInfo)
            {
                listIdEmail.Add(emailInfo.id);
            }

            var listEmailInfoLabel = _context.EmailInfoLabels.Where(r => (request.idLabel.Contains(r.idLabel.Value) || request.idLabel.Count == 0) && listIdEmail.Contains(r.idEmailInfo.Value)).ToList()
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
                topTrending = listTopTrendingLabel,
                total = 100
            };
        }

        [HttpGet]
        [Route("AgentTopConversation")]
        public async Task<TopConversationAgentResponse> AgentTopConversation([FromQuery] TopConversationAgentRequest request)
        {
            List<LabelDistribution> result = new List<LabelDistribution>();

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true
            && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && r.status == Common.Open).ToList();

            List<int> listIdEmail = new List<int>();
            foreach (EmailInfo emailInfo in listEmailInfo)
            {
                listIdEmail.Add(emailInfo.id);
            }


            var partialResult = (from c in listEmailInfo
                                 join o in _context.EmailInfoAssigns.Where(r => listIdEmail.Contains(r.idEmailInfo.Value)).ToList() on c.id equals o.idEmailInfo
                                 join agent in _context.Accounts.Where(r => r.idCompany == request.idCompany).ToList() on o.idUser equals agent.id
                                 select new
                                 {
                                     agent.fullname, agent.workemail, c.status,
                                     agent.id
                                 }).GroupBy(t => new
                                 {
                                     t.fullname, t.workemail, t.status, t.id
                                 })
                                   .Select(t => new
                                   {
                                       IdUser = t.Key.id,
                                       Agent = t.Key.fullname,
                                       Mail = t.Key.workemail,
                                       Open = t.Count()
                                   }).ToList();

            List<TopConversationAgentObject> Result = new List<TopConversationAgentObject>();
            foreach (dynamic obj in partialResult)
            {
                TopConversationAgentObject objTopConversationAgent = new TopConversationAgentObject();                
                objTopConversationAgent.IdUser = obj.IdUser;
                objTopConversationAgent.Agent = obj.Agent;
                objTopConversationAgent.Mail = obj.Mail;
                objTopConversationAgent.Open = obj.Open;
                objTopConversationAgent.Unattended = 0;
                Result.Add(objTopConversationAgent);
            }

            return new TopConversationAgentResponse
            {
                Status = ResponseStatus.Susscess,
                Result = Result
            };
        }

        [HttpGet]
        [Route("AgentPerformentMonitorTotal")]
        public async Task<PerformentMonitorTotalResponse> AgentPerformentMonitorTotal([FromQuery] PerformentMonitorAgentTotalRequest request)
        {
            ObjectPerformentMonitorTotal result = new ObjectPerformentMonitorTotal();

            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.IdUser).ToList();

            List<int> listIdEmailAssign = new List<int>();
            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false 
            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).ToList();


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
                       }).Count().ToString();


            result.ResolveTime = 0;

            return new PerformentMonitorTotalResponse
            {
                Status = ResponseStatus.Susscess,
                Result = result
            };
        }

        [HttpGet]
        [Route("AgentPerformentMonitor")]
        public async Task<PerformentMonitorResponse> AgentPerformentMonitor([FromQuery] PerformentMonitorAgentRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            //var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();


            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.IdUser || request.IdUser == 0).ToList();

            List<int> listIdEmailAssign = new List<int>();
            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).ToList();


            ObjectPerformentMonitor resutl = PerformentMonitor(request.fromDate, request.toDate, listEmailInfo, request.idCompany, request.type);

            return new PerformentMonitorResponse
            {
                Status = ResponseStatus.Susscess,
                Result = resutl
            };
        }

        [HttpGet]
        [Route("AgentOverview")]
        public async Task<ReportAgentOverviewResponse> AgentOverview([FromQuery] int idCompany)
        {
            ReportAgentOverviewObject result = new ReportAgentOverviewObject();

            var listAccount = _context.Accounts.Where(x => x.idCompany == idCompany && x.status != 0)
                .GroupBy(t => t.status)
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Count()
                       }).ToList();

            var totalAccount = _context.Accounts.Where(x => x.idCompany == idCompany).Count();
            result.Total = totalAccount;

            listAccount.ForEach(x =>
            {
                if(x.key == 1)
                {
                    result.Online = x.value;
                }
                if (x.key == 2)
                {
                    result.Busy = x.value;
                }
                if (x.key == 3)
                {
                    result.Online = x.value;
                }
            });

            return new ReportAgentOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                result = result
            };
        }

        [HttpGet]
        [Route("GroupTopConversation")]
        public async Task<TopConversationAgentResponse> GroupTopConversation([FromQuery] TopConversationAgentRequest request)
        {
            List<LabelDistribution> result = new List<LabelDistribution>();

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true
            && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && r.status == Common.Open).ToList();

            List<int> listIdEmail = new List<int>();
            foreach (EmailInfo emailInfo in listEmailInfo)
            {
                listIdEmail.Add(emailInfo.id);
            }


            var partialResult = (from c in listEmailInfo
                                 join o in _context.EmailInfoAssigns.Where(r => listIdEmail.Contains(r.idEmailInfo.Value)).ToList() on c.id equals o.idEmailInfo
                                 join agent in _context.Accounts.Where(r => r.idCompany == request.idCompany).ToList() on o.idUser equals agent.id
                                 select new
                                 {
                                     agent.fullname,
                                     agent.workemail,
                                     c.status,
                                     agent.id
                                 }).GroupBy(t => new
                                 {
                                     t.fullname,
                                     t.workemail,
                                     t.status,
                                     t.id
                                 })
                                   .Select(t => new
                                   {
                                       IdUser = t.Key.id,
                                       Agent = t.Key.fullname,
                                       Mail = t.Key.workemail,
                                       Open = t.Count()
                                   }).ToList();

            List<TopConversationAgentObject> Result = new List<TopConversationAgentObject>();
            foreach (dynamic obj in partialResult)
            {
                TopConversationAgentObject objTopConversationAgent = new TopConversationAgentObject();
                objTopConversationAgent.IdUser = obj.IdUser;
                objTopConversationAgent.Agent = obj.Agent;
                objTopConversationAgent.Mail = obj.Mail;
                objTopConversationAgent.Open = obj.Open;
                objTopConversationAgent.Unattended = 0;
                Result.Add(objTopConversationAgent);
            }

            return new TopConversationAgentResponse
            {
                Status = ResponseStatus.Susscess,
                Result = Result
            };
        }

        [HttpGet]
        [Route("GroupPerformentMonitorTotal")]
        public async Task<PerformentMonitorTotalResponse> GroupPerformentMonitorTotal([FromQuery] PerformentMonitorAgentTotalRequest request)
        {
            ObjectPerformentMonitorTotal result = new ObjectPerformentMonitorTotal();

            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.IdUser).ToList();

            List<int> listIdEmailAssign = new List<int>();
            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).ToList();


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
                       }).Count().ToString();


            result.ResolveTime = 0;

            return new PerformentMonitorTotalResponse
            {
                Status = ResponseStatus.Susscess,
                Result = result
            };
        }

        [HttpGet]
        [Route("GroupPerformentMonitor")]
        public async Task<PerformentMonitorResponse> GroupPerformentMonitor([FromQuery] PerformentMonitorAgentRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            //var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();


            List<EmailInfoAssign> listEmailInfoAssign = _context.EmailInfoAssigns.Where(x => x.idUser == request.IdUser || request.IdUser == 0).ToList();

            List<int> listIdEmailAssign = new List<int>();
            foreach (EmailInfoAssign emailInfoAssign in listEmailInfoAssign)
            {
                listIdEmailAssign.Add(emailInfoAssign.idEmailInfo.Value);
            }

            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).ToList();


            ObjectPerformentMonitor resutl = PerformentMonitor(request.fromDate, request.toDate, listEmailInfo, request.idCompany, request.type);

            return new PerformentMonitorResponse
            {
                Status = ResponseStatus.Susscess,
                Result = resutl
            };
        }


        [HttpGet]
        [Route("CsatOverview")]
        public async Task<CsatOverviewResponse> CsatOverview([FromQuery] CsatOverviewRequest request)
        {
            CsatOverview result = new CsatOverview();
            result.SatisfactionScore.Total = 10;
            result.ResponseRate.Total = 20;
            result.TotalResponses.Total = 30;
            CsatOverview resultUpDown = new CsatOverview();
            resultUpDown.SatisfactionScore.UpDown = 5;
            resultUpDown.ResponseRate.UpDown = -2;
            resultUpDown.TotalResponses.UpDown = -4;

            return new CsatOverviewResponse
            {
                Status = ResponseStatus.Susscess,
                result = result
            };
        }

        [HttpGet]
        [Route("CsatResponeDistribution")]
        public async Task<CsatResponeDistributionResponse> CsatResponeDistribution([FromQuery] CsatResponeDistributionRequest request)
        {
            List<LabelDistribution> result = new List<LabelDistribution>();

            var listCsat = _context.Csats.Where(r => r.idCompany == request.idCompany
            && r.dateTime >= request.fromDate.ToUniversalTime() && r.dateTime <= request.toDate.ToUniversalTime()).GroupBy(t => t.idFeedBack)
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

            Random rnd = new Random();
            List<string> colors = new List<string>();
            List<CsatResponeDistribution> listCsatResponeDistribution = new List<CsatResponeDistribution>();
            List<CsatResponeDistributionTable> listCsatResponeDistributionTable = new List<CsatResponeDistributionTable>();

            int Sum = listCsat.Sum(r => r.value);
            foreach (var csat in listCsat)
            {
                CsatResponeDistributionTable objTable = new CsatResponeDistributionTable();
                CsatResponeDistribution obj = new CsatResponeDistribution();
                //TopTrendingLabel topTrendingLabel = new TopTrendingLabel();
                //var label = _context.Labels.Where(r => r.id == emailInfoLabel.key).FirstOrDefault();
                string nameRate = "";
                switch (csat.key)
                {
                    case 1:
                        nameRate = "Very Bad";
                        break;
                    case 2:
                        nameRate = "Bad";
                        break;
                    case 3:
                        nameRate = "Nomal";
                        break;
                    case 4:
                        nameRate = "Good";
                        break;
                    case 5:
                        nameRate = "Very Good";
                        break;
                    default:
                        // code block
                        break;
                }
                obj.Name = nameRate;
                obj.Y = Math.Round((decimal)csat.value / Sum, 2);

                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = ColorTranslator.ToHtml(randomColor);
                colors.Add(colorHex);
                listCsatResponeDistribution.Add(obj);

                objTable.ClassName = nameRate;
                objTable.Distribution = Math.Round((obj.Y * 100), 0).ToString() + "%";
                objTable.Conversation = csat.value.ToString();

                listCsatResponeDistributionTable.Add(objTable);
            }

            return new CsatResponeDistributionResponse
            {
                Status = ResponseStatus.Susscess,
                result = listCsatResponeDistribution,
                colors = colors,
                resultTable = listCsatResponeDistributionTable,
                total = Sum
            };
        }


        [HttpGet]
        [Route("CsatResponeDetail")]
        public async Task<CsatResponeDetailResponse> CsatResponeDetail([FromQuery] CsatResponeDetailRequest request)
        {
            List<Csat> listCsat = _context.Csats.Where(r => r.dateTime >= request.fromDate.ToUniversalTime() && r.dateTime <= request.toDate.ToUniversalTime() && r.idCompany == request.idCompany).ToList();
            List<string> listGuid = new List<string>();
            foreach(Csat cs in listCsat)
            {
                listGuid.Add(cs.idGuIdEmailInfo);
            }

            var listResult = (from csat in listCsat
                              join emailInfo in _context.EmailInfos.Where(r => r.idCompany == request.idCompany).ToList() on csat.idGuIdEmailInfo equals emailInfo.idGuId
                              join acccount in _context.Accounts.Where(r => r.idCompany == request.idCompany).ToList() on emailInfo.idContact equals acccount.id
                              select new
                              {
                                  contact = acccount.fullname,
                                  assignedagent = "",
                                  rating = csat.idFeedBack,
                                  feedback = csat.descriptionFeedBack,
                                  datetime = csat.dateTime.ToString("HH:mm dd/MM/yyyy")
                              }).Skip(request.pageSize * (request.pageIndex-1))
                                .Take(request.pageSize).ToList<Object>();

            return new CsatResponeDetailResponse
            {
                Status = ResponseStatus.Susscess,
                result = listResult,
                total = listCsat.Count
            };
        }


        [HttpGet]
        [Route("Traffic")]
        public async Task<TrafficResponse> Traffic([FromQuery] ReportOverviewRequest request)
        {
            DateTime dt = request.fromDate;
            List<TrafficDay> categories = new List<TrafficDay>();
            List<List<int>> data = new List<List<int>>();

            int k = 1;
            while(dt <= request.toDate)
            {
                TrafficDay obj = new TrafficDay();
                obj.id = k;
                obj.labels = dt.ToString("dd/MM/yyyy");

                DateTimeOffset dateOffsetValue = new DateTimeOffset(dt, TimeZoneInfo.Local.GetUtcOffset(dt));
                obj.days = dateOffsetValue.ToString("dddd");

                dt = dt.AddDays(1);
                categories.Add(obj);
                k++;
            }

            for(int i = 0; i < 23; i++)
            {
                for(int j = 0; j< categories.Count; j++)
                {
                    List<int> obj = new List<int>();
                    obj.Add(i);
                    obj.Add(j);
                    Random rand = new Random();
                    obj.Add(rand.Next(0, 100));
                    data.Add(obj);
                }
            }
            return new TrafficResponse
            {
                Status = ResponseStatus.Susscess,
                categories = categories,
                data = data
            };
        }

    }

}
