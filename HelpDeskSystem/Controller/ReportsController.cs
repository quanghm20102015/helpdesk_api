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
using Npgsql;
using NuGet.Protocol;
using System.Data;
using System.Dynamic;

namespace HelpDeskSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly EF_DataContext _context;
        private readonly IConfiguration _config;

        public ReportsController(EF_DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Route("Overview")]
        public async Task<ReportOverviewResponse> Overview([FromQuery] ReportOverviewRequest request)
        {
            //ReportOverview reportOverview = new ReportOverview();
            // var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList();
            // var listEmailInfoUnReply = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false 
            // && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList().GroupBy(t => t.messageId)
            //            .Select(t => new
            //            {
            //                key = t.Key,
            //                value = t.Count()
            //            }).ToList().Where(r => r.value == 1).ToList();

            // List<string> listMessageId = new List<string>();
            // foreach (dynamic obj in listEmailInfoUnReply)
            // {
            //     listMessageId.Add(obj.key);
            // }

            //reportOverview.Opened.Total = listEmailInfo.Where(r => r.status == 1).Count();
            //reportOverview.Resolved.Total = listEmailInfo.Where(r => r.status == 2).Count();
            //reportOverview.Unassigned.Total = listEmailInfo.Where(r => r.isAssign == false).Count();
            //reportOverview.Unattended.Total = reportOverview.Unassigned.Total + listEmailInfo.Where(r => listMessageId.Contains(r.messageId)).Count();

            // ReportOverview reportOverviewUpDown = new ReportOverview();            
            // double ttt = request.toDate.Subtract(request.fromDate).TotalDays;
            // var listEmailInfoBefor = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date <= request.fromDate.ToUniversalTime() && r.date >= request.fromDate.AddDays(-ttt).ToUniversalTime()).ToList();
            // var listEmailInfoUnReplyBefor = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false
            // && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).ToList().GroupBy(t => t.messageId)
            //.Select(t => new
            //{
            //    key = t.Key,
            //    value = t.Count()
            //}).ToList().Where(r => r.value == 1).ToList();

            // List<string> listMessageIdBefor = new List<string>();
            // foreach (dynamic obj in listEmailInfoUnReplyBefor)
            // {
            //     listMessageId.Add(obj.key);
            // }

            // reportOverview.Opened.UpDown = reportOverview.Opened.Total -listEmailInfoBefor.Where(r => r.status == 1).Count();
            // reportOverview.Resolved.UpDown = reportOverview.Resolved.Total - listEmailInfoBefor.Where(r => r.status == 2).Count();
            // reportOverview.Unassigned.UpDown = reportOverview.Unassigned.Total - listEmailInfoBefor.Where(r => r.isAssign == false).Count();
            // reportOverview.Unattended.UpDown = reportOverview.Unattended.Total - (listEmailInfoBefor.Where(r => r.isAssign == false).Count() + listEmailInfoBefor.Where(r => listMessageIdBefor.Contains(r.messageId)).Count());


            // //ListPerformentMonitor reportData = GetReportData(request.fromDate, request.toDate, listEmailInfo, request.idCompany);

            // return new ReportOverviewResponse
            // {
            //     Status = ResponseStatus.Susscess,
            //     result = reportOverview
            // };



            ReportOverview reportOverview = new ReportOverview();
            reportOverview.Opened.Total = 0;
            reportOverview.Resolved.Total = 0;
            reportOverview.Unassigned.Total = 0;
            reportOverview.Unattended.Total = 0;


            ReportOverview reportOverviewUpDown = new ReportOverview();
            reportOverviewUpDown.Opened.Total = 0;
            reportOverviewUpDown.Resolved.Total = 0;
            reportOverviewUpDown.Unassigned.Total = 0;
            reportOverviewUpDown.Unattended.Total = 0;

            string connectionString = _config.GetValue<string>("ConnectionStrings:Ef_Postgres_Db");

            NpgsqlConnection conn = null;

            try
            {
                conn = new NpgsqlConnection(connectionString);
                conn.Open();

                DateTime fromDateAgo = request.fromDate.AddDays(-request.toDate.Subtract(request.fromDate).TotalDays);
                var sql = "select * from Overview(cast('" + request.fromDate.ToString("yyyy-MM-dd") + "' as date), cast('" + request.toDate.ToString("yyyy-MM-dd") + "' as date), cast('" + fromDateAgo.ToString("yyyy-MM-dd") + "' as date)," + request.idCompany.ToString() + ")";

                using var cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connectionString);

                DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                for(int i = 0; i< dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["type"].ToString() == "0")
                    {
                        int Tong = int.Parse(dt.Rows[i]["tong"].ToString());
                        if (dt.Rows[i]["id"].ToString() == "1")
                        {
                            reportOverview.Opened.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "2")
                        {
                            reportOverview.Resolved.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "3")
                        {
                            reportOverview.Unassigned.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "4")
                        {
                            reportOverview.Unattended.Total = Tong;
                        }
                    }
                    else
                    {
                        int Tong = int.Parse(dt.Rows[i]["tong"].ToString());
                        if (dt.Rows[i]["id"].ToString() == "1")
                        {
                            reportOverviewUpDown.Opened.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "2")
                        {
                            reportOverviewUpDown.Resolved.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "3")
                        {
                            reportOverviewUpDown.Unassigned.Total = Tong;
                        }
                        else if (dt.Rows[i]["id"].ToString() == "4")
                        {
                            reportOverviewUpDown.Unattended.Total = Tong;
                        }
                    }
                }
                reportOverview.Opened.UpDown = reportOverview.Opened.Total - reportOverviewUpDown.Opened.Total;
                reportOverview.Resolved.UpDown = reportOverview.Resolved.Total - reportOverviewUpDown.Resolved.Total;
                reportOverview.Unassigned.UpDown = reportOverview.Unassigned.Total - reportOverviewUpDown.Unassigned.Total;
                reportOverview.Unattended.UpDown = reportOverview.Unattended.Total - reportOverviewUpDown.Unattended.Total;

                List<dynamic> dynamicDt = ToDynamic(dt);


                return new ReportOverviewResponse
                {
                    Status = ResponseStatus.Susscess,
                    result = reportOverview
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Close connection
                if (null != conn)
                {
                    conn.Close();

                }
            }
        }

        [HttpGet]
        [Route("PerformentMonitor")]
        public async Task<PerformentMonitorResponse> PerformentMonitor([FromQuery] ReportRequest request)
        {
            ReportOverview reportOverview = new ReportOverview();
            var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime())
                .Select(t => new
                {
                    date = t.date,
                    id = t.id,
                    messageId = t.messageId,
                    status = t.status
                }).ToList<dynamic>();

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


            result.Resolved = listEmailInfo.Where(r => r.status == Common.Resolved).Count();

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

            string str = TimeSpan.FromSeconds(timeSecond).ToString(@"hh\:mm\:ss");

            result.ResponeTime = str;



            List<ObjectReportTime> listObjectResolvedTime = new List<ObjectReportTime>();
            listEmailInfo = listEmailInfo.Where(r => r.status == Common.Resolved).ToList();
            foreach (EmailInfo obj in listEmailInfo)
            {
                ObjectReportTime obj1 = new ObjectReportTime();
                double time = obj.resolveTime.Value.Subtract(obj.date.Value).TotalSeconds;
                obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                obj1.value = time;
                listObjectResolvedTime.Add(obj1);
            }


            double resolveTime = listObjectResolvedTime.GroupBy(r => r.date)
                       .Select(t => new
                       {
                           key = t.Key,
                           value = t.Average(p => p.value)
                       }).ToList().Sum(r => r.value);


            string strResolveTime = TimeSpan.FromSeconds(resolveTime).ToString(@"hh\:mm\:ss");

            result.ResolveTime = strResolveTime;

            return new PerformentMonitorTotalResponse
            {
                Status = ResponseStatus.Susscess,
                Result = result
            };
        }

        private ObjectPerformentMonitor PerformentMonitor(DateTime fromDate, DateTime toDate, List<dynamic> listEmailInfo, int idCompany, int type)
        {
            try
            {
                ObjectPerformentMonitor result = new ObjectPerformentMonitor();

                int totalDay = (int)toDate.Subtract(fromDate).TotalDays;
                int interval = 1;
                if (totalDay > 25)
                {
                    interval = (totalDay / 20) + 1;
                }

                DateTime dt = fromDate;
                if (type == 1)//total
                {
                    var listReport = listEmailInfo.GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                               .Select(t => new
                               {
                                   key = t.Key,
                                   value = t.Count()
                               }).ToList();

                    while (dt <= toDate)
                    {
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
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
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
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
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
                    }
                }
                else if (type == 4)//IncResolveoming
                {
                    var listReport = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                               .Select(t => new
                               {
                                   key = t.Key,
                                   value = t.Count()
                               }).ToList();

                    while (dt <= toDate)
                    {
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
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
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = (int)objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
                    }
                }
                else if (type == 6)//resolvetime
                {
                    List<ObjectReportTime> listObjectResolvedTime = new List<ObjectReportTime>();
                    listEmailInfo = listEmailInfo.Where(r => r.status == Common.Resolved).ToList();
                    foreach (EmailInfo obj in listEmailInfo)
                    {
                        ObjectReportTime obj1 = new ObjectReportTime();
                        double time = obj.resolveTime.Value.Subtract(obj.date.Value).TotalSeconds;
                        obj1.date = Convert.ToDateTime(obj.date).ToString("dd-MM-yyyy");
                        obj1.value = time;
                        listObjectResolvedTime.Add(obj1);
                    }


                    var listReport = listObjectResolvedTime.GroupBy(r => r.date)
                               .Select(t => new
                               {
                                   key = t.Key,
                                   value = t.Average(p => p.value)
                               }).ToList();

                    while (dt <= toDate)
                    {
                        result.label.Add(dt.ToString("dd-MM-yyyy"));
                        ObjectReport objTotal = new ObjectReport();
                        for (int i = 1; i <= interval; i++)
                        {
                            int value = 0;
                            //add total
                            var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                            if (objectEmailInfo == null)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = (int)objectEmailInfo.value;
                            }
                            objTotal.value = objTotal.value + value;

                            dt = dt.AddDays(1);
                        }
                        result.data.Add(objTotal.value);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                obj.Name = "#" + (label == null ? "" : label.name);
                obj.Y = Math.Round((decimal)emailInfoLabel.value / Sum, 2);

                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = ColorTranslator.ToHtml(randomColor);
                colors.Add(colorHex);
                result.Add(obj);

                labelDistributionTable.Name = "#" + (label == null ? "" : label.name);
                labelDistributionTable.ClassName = colorHex;
                labelDistributionTable.Distribution = Math.Round((obj.Y * 100), 0).ToString() + "%";
                labelDistributionTable.Conversation = emailInfoLabel.value.ToString();

                topTrendingLabel.Name = "#" + (label == null ? "" : label.name);
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
            try
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
                && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).Select(t => new
                {
                    date = t.date,
                    id = t.id,
                    messageId = t.messageId,
                    status = t.status
                }).ToList<dynamic>();


                result.Total = listEmailInfo.GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();

                result.Incoming = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 1
                            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).Select(t => new
                            {
                                date = t.date,
                                id = t.id,
                                messageId = t.messageId,
                                status = t.status
                            }).ToList<dynamic>()
                            .GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();


                result.Outgoing = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 2
                            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).Select(t => new
                            {
                                date = t.date,
                                id = t.id,
                                messageId = t.messageId,
                                status = t.status
                            }).ToList<dynamic>()
                            .GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();


                result.Resolved = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();

                List<ObjectReportTime> listObjectReportTime = new List<ObjectReportTime>();
                foreach (dynamic obj in listEmailInfo)
                {
                    int id = obj.id;
                    string messageId = obj.messageId;
                    dynamic objReply = _context.EmailInfos.Where(r => r.idReference == messageId && r.type == 2 && r.id != id)
                        .Select(t => new
                        {
                            date = t.date
                        }).FirstOrDefault();
                    if (objReply != null)
                    {
                        ObjectReportTime obj1 = new ObjectReportTime();
                        double time = Convert.ToDateTime(objReply.date).Subtract(Convert.ToDateTime(obj.date)).TotalSeconds;
                        obj1.date = Convert.ToDateTime(obj.date).ToString("dd-MM-yyyy");
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


                result.ResolveTime = "0";

                return new PerformentMonitorTotalResponse
                {
                    Status = ResponseStatus.Susscess,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return null;
            }
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
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).Select(t => new
            {
                date = t.date,
                id = t.id,
                messageId = t.messageId,
                status = t.status
            }).ToList<dynamic>();


            ObjectPerformentMonitor resutl = PerformentMonitor(request.fromDate, request.toDate, listEmailInfo, request.idCompany, request.type);

            return new PerformentMonitorResponse
            {
                Status = ResponseStatus.Susscess,
                Result = resutl
            };
        }


        private ObjectPerformentMonitor AgentPerformentMonitor(DateTime fromDate, DateTime toDate, List<EmailInfo> listEmailInfo, int idCompany, int type)
        {
            ObjectPerformentMonitor result = new ObjectPerformentMonitor();

            int totalDay = (int)toDate.Subtract(fromDate).TotalDays;
            int interval = 1;
            if (totalDay > 25)
            {
                interval = (totalDay / 20) + 1;
            }

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
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    ObjectReport objTotal = new ObjectReport();
                    for (int i = 1; i <= interval; i++)
                    {
                        int value = 0;
                        //add total
                        var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                        if (objectEmailInfo == null)
                        {
                            value = 0;
                        }
                        else
                        {
                            value = objectEmailInfo.value;
                        }
                        objTotal.value = objTotal.value + value;

                        dt = dt.AddDays(1);
                    }
                    result.data.Add(objTotal.value);
                }
            }
            else if (type == 2)//Outgoing
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
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    ObjectReport objTotal = new ObjectReport();
                    for (int i = 1; i <= interval; i++)
                    {
                        int value = 0;
                        //add total
                        var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                        if (objectEmailInfo == null)
                        {
                            value = 0;
                        }
                        else
                        {
                            value = objectEmailInfo.value;
                        }
                        objTotal.value = objTotal.value + value;

                        dt = dt.AddDays(1);
                    }
                    result.data.Add(objTotal.value);
                }
            }
            else if (type == 3)//IncResolveoming
            {
                var listReport = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => t.date.Value.ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).ToList();

                while (dt <= toDate)
                {
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    ObjectReport objTotal = new ObjectReport();
                    for (int i = 1; i <= interval; i++)
                    {
                        int value = 0;
                        //add total
                        var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                        if (objectEmailInfo == null)
                        {
                            value = 0;
                        }
                        else
                        {
                            value = objectEmailInfo.value;
                        }
                        objTotal.value = objTotal.value + value;

                        dt = dt.AddDays(1);
                    }
                    result.data.Add(objTotal.value);
                }
            }
            else if (type == 4)//FirstRespone
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
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    ObjectReport objTotal = new ObjectReport();
                    for (int i = 1; i <= interval; i++)
                    {
                        int value = 0;
                        //add total
                        var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                        if (objectEmailInfo == null)
                        {
                            value = 0;
                        }
                        else
                        {
                            value = (int)objectEmailInfo.value;
                        }
                        objTotal.value = objTotal.value + value;

                        dt = dt.AddDays(1);
                    }
                    result.data.Add(objTotal.value);
                }
            }
            else if (type == 5)//resolvetime
            {
                List<ObjectReportTime> listObjectResolvedTime = new List<ObjectReportTime>();
                listEmailInfo = listEmailInfo.Where(r => r.status == Common.Resolved).ToList();
                foreach (EmailInfo obj in listEmailInfo)
                {
                    ObjectReportTime obj1 = new ObjectReportTime();
                    double time = obj.resolveTime.Value.Subtract(obj.date.Value).TotalSeconds;
                    obj1.date = obj.date.Value.ToString("dd-MM-yyyy");
                    obj1.value = time;
                    listObjectResolvedTime.Add(obj1);
                }


                var listReport = listObjectResolvedTime.GroupBy(r => r.date)
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Average(p => p.value)
                           }).ToList();

                while (dt <= toDate)
                {
                    result.label.Add(dt.ToString("dd-MM-yyyy"));
                    ObjectReport objTotal = new ObjectReport();
                    for (int i = 1; i <= interval; i++)
                    {
                        int value = 0;
                        //add total
                        var objectEmailInfo = listReport.Where(r => r.key == dt.ToString("dd-MM-yyyy")).FirstOrDefault();
                        if (objectEmailInfo == null)
                        {
                            value = 0;
                        }
                        else
                        {
                            value = (int)objectEmailInfo.value;
                        }
                        objTotal.value = objTotal.value + value;

                        dt = dt.AddDays(1);
                    }
                    result.data.Add(objTotal.value);
                }
            }

            return result;
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
            try
            {
                List<LabelDistribution> result = new List<LabelDistribution>();

                var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true
                && r.isDelete == false && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()
                && r.status == Common.Open).Select(t => new
                {
                    date = t.date,
                    id = t.id,
                    messageId = t.messageId,
                    status = t.status
                }).ToList<dynamic>();

                List<int> listIdEmail = new List<int>();
                foreach (dynamic emailInfo in listEmailInfo)
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
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GroupPerformentMonitorTotal")]
        public async Task<PerformentMonitorTotalResponse> GroupPerformentMonitorTotal([FromQuery] PerformentMonitorAgentTotalRequest request)
        {
            try
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
                && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).Select(t => new
                {
                    date = t.date,
                    id = t.id,
                    messageId = t.messageId,
                    status = t.status
                }).ToList<dynamic>();


                result.Total = listEmailInfo.GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();

                result.Incoming = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 1
                            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).Select(t => new
                            {
                                date = t.date,
                                id = t.id,
                                messageId = t.messageId,
                                status = t.status
                            }).ToList<dynamic>()
                            .GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();


                result.Outgoing = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.isDelete == false && r.type == 2
                            && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime()).Select(t => new
                            {
                                date = t.date,
                                id = t.id,
                                messageId = t.messageId,
                                status = t.status
                            }).ToList<dynamic>()
                            .GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();


                result.Resolved = listEmailInfo.Where(r => r.status == Common.Resolved).GroupBy(t => Convert.ToDateTime(t.date).ToString("dd-MM-yyyy"))
                           .Select(t => new
                           {
                               key = t.Key,
                               value = t.Count()
                           }).Count();

                List<ObjectReportTime> listObjectReportTime = new List<ObjectReportTime>();
                foreach (dynamic obj in listEmailInfo)
                {
                    int id = obj.id;
                    string messageId = obj.messageId;
                    dynamic objReply = _context.EmailInfos.Where(r => r.idReference == messageId && r.type == 2 && r.id != id)
                        .Select(t => new
                        {
                            date = t.date
                        }).FirstOrDefault();
                    if (objReply != null)
                    {
                        ObjectReportTime obj1 = new ObjectReportTime();
                        double time = Convert.ToDateTime(objReply.date).Subtract(Convert.ToDateTime(obj.date)).TotalSeconds;
                        obj1.date = Convert.ToDateTime(obj.date).ToString("dd-MM-yyyy");
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


                result.ResolveTime = "0";

                return new PerformentMonitorTotalResponse
                {
                    Status = ResponseStatus.Susscess,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return null;
            }
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
            && (listIdEmailAssign.Contains(r.id) || request.IdUser == 0)).Select(t => new
            {
                date = t.date,
                id = t.id,
                messageId = t.messageId,
                status = t.status
            }).ToList<dynamic>();


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
            List<Csat> listCsat = _context.Csats.Where(r => r.idCompany == request.idCompany && r.dateTime >= request.fromDate.ToUniversalTime() && r.dateTime <= request.toDate.ToUniversalTime()).ToList();
            CsatOverview result = new CsatOverview();
            int TotalResponses = listCsat.Where(r => r.idFeedBack != 0).Count();
            int TotalCsat = listCsat.Count();
            int SatisfactionScore = listCsat.Where(r => r.idFeedBack >= 3).Count();
            result.SatisfactionScore.Total = TotalCsat == 0 ? 0 : (int)Math.Round(((decimal)SatisfactionScore / TotalCsat) * 100, 0);
            result.ResponseRate.Total = TotalCsat == 0 ? 0 : (int)Math.Round(((decimal)TotalResponses / TotalCsat) * 100, 0);
            result.TotalResponses.Total = TotalResponses;


            double ttt = request.toDate.Subtract(request.fromDate).TotalDays; 
            List<Csat> listCsatBefor = _context.Csats.Where(r => r.idCompany == request.idCompany && r.dateTime >= request.fromDate.AddDays(-ttt).ToUniversalTime() && r.dateTime <= request.fromDate.ToUniversalTime()).ToList();
            //CsatOverview resultUpDown = new CsatOverview();
            int TotalResponsesBefor = listCsatBefor.Where(r => r.idFeedBack != 0).Count();
            int TotalCsatBefor = listCsatBefor.Count();
            int SatisfactionScoreBefor = listCsatBefor.Where(r => r.idFeedBack >= 3).Count();
            result.SatisfactionScore.UpDown = TotalCsatBefor == 0 ? 0 : result.SatisfactionScore.Total - (int)Math.Round(((decimal)SatisfactionScoreBefor / TotalCsatBefor) * 100, 0);
            result.ResponseRate.UpDown = TotalCsatBefor == 0 ? 0 : result.ResponseRate.Total - (int)Math.Round(((decimal)TotalResponsesBefor / TotalCsatBefor) * 100, 0);
            result.TotalResponses.UpDown = result.TotalResponses.Total - TotalResponsesBefor;

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

            var listCsat = _context.Csats.Where(r => r.idCompany == request.idCompany && r.idFeedBack != 0
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
            //List<Csat> listCsat = _context.Csats.Where(r => r.dateTime >= request.fromDate.ToUniversalTime() && r.dateTime <= request.toDate.ToUniversalTime() && r.idCompany == request.idCompany).ToList();
            //List<string> listGuid = new List<string>();
            //foreach(Csat cs in listCsat)
            //{
            //    listGuid.Add(cs.idGuIdEmailInfo);
            //}

            //var listResult = (from csat in listCsat
            //                  join emailInfo in _context.EmailInfos.Where(r => r.idCompany == request.idCompany).ToList() on csat.idGuIdEmailInfo equals emailInfo.idGuId
            //                  join acccount in _context.Accounts.Where(r => r.idCompany == request.idCompany).ToList() on emailInfo.idContact equals acccount.id
            //                  select new
            //                  {
            //                      contact = acccount.fullname,
            //                      assignedagent = "",
            //                      rating = csat.idFeedBack == 1 ? "Very Bad" : (csat.idFeedBack == 2 ? "Bad" : (csat.idFeedBack == 3 ? "Nomal" : (csat.idFeedBack == 4 ? "Good" : "Very Good"))),
            //                      feedback = csat.descriptionFeedBack,
            //                      datetime = csat.dateTime.ToString("HH:mm dd/MM/yyyy")
            //                  }).Skip(request.pageSize * (request.pageIndex-1))
            //                    .Take(request.pageSize).ToList<Object>();

            //return new CsatResponeDetailResponse
            //{
            //    Status = ResponseStatus.Susscess,
            //    //result = listResult,
            //    total = listCsat.Count
            //};

            string connectionString = _config.GetValue<string>("ConnectionStrings:Ef_Postgres_Db");

            NpgsqlConnection conn = null;

            try
            {
                // Initialization
                conn = new NpgsqlConnection(connectionString);

                // Open connection
                conn.Open();

                var sql = "select * from CsatResponeDetail(cast('" + request.fromDate.ToString("yyyy-MM-dd") + "' as date), cast('" + request.toDate.ToString("yyyy-MM-dd") + "' as date), " + request.idCompany.ToString() + ")";

                using var cmd = new NpgsqlCommand(sql, conn);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connectionString);

                DataTable dt = new System.Data.DataTable();
                da.Fill(dt);

                List<dynamic> dynamicDt = ToDynamic(dt);


                return new CsatResponeDetailResponse
                {
                    Status = ResponseStatus.Susscess,
                    result = dynamicDt,
                    total = dt.Rows.Count
                };
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // Close connection
                if (null != conn)
                {
                    conn.Close();

                }
            }
        }
        public static List<dynamic> ToDynamic(DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return dynamicDt;
        }

        [HttpGet]
        [Route("Traffic")]
        public async Task<TrafficResponse> Traffic([FromQuery] ReportOverviewRequest request)
        {
            try
            {
                var listEmailInfo = _context.EmailInfos.Where(r => r.idCompany == request.idCompany && r.mainConversation == true && r.isDelete == false
                && r.date >= request.fromDate.ToUniversalTime() && r.date <= request.toDate.ToUniversalTime())
                    .Select(t => new
                    {
                        date = t.date,
                        id = t.id,
                        messageId = t.messageId,
                        status = t.status
                    }).ToList<dynamic>().GroupBy(t =>
                new
                {
                    hour = Convert.ToDateTime(t.date).Hour,
                    day = Convert.ToDateTime(t.date).ToString("dd/MM/yyyy")
                }).Select(t => new
                {
                    day = t.Key.day,
                    hour = t.Key.hour,
                    value = t.Count()
                }).ToList();

                int totalDay = (int)request.toDate.Subtract(request.fromDate).TotalDays;
                int interval = 1;
                if (totalDay > 10)
                {
                    interval = (totalDay / 9) + 1;
                }

                DateTime dt = request.fromDate;
                List<TrafficDay> categories = new List<TrafficDay>();
                List<List<int>> data = new List<List<int>>();

                int k = 1;
                while (dt <= request.toDate)
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

                List<TrafficDay> categoriesResult = new List<TrafficDay>();
                for (int i = 0; i < 23; i++)
                {
                    int kk = 0;
                    int value = 0;
                    for (int j = 0; j < categories.Count; j++)
                    {
                        Random rand = new Random();
                        var objEmailInfo = listEmailInfo.Find(r => r.day == categories[j].labels && r.hour == i);
                        value = value + (objEmailInfo == null ? 0 : objEmailInfo.value);

                        if (j % interval == interval - 1)
                        {
                            List<int> obj = new List<int>();
                            obj.Add(i);
                            obj.Add(kk);
                            obj.Add(value);
                            value = 0;
                            data.Add(obj);

                            if (i == 0 && interval > 1)
                            {
                                categories[j - 1].id = kk;
                                categoriesResult.Add(categories[j - 1]);
                            }
                            else if (i == 0 && interval == 1)
                            {
                                categories[j].id = kk;
                                categoriesResult.Add(categories[j]);
                            }
                            kk++;
                        }
                    }
                }
                return new TrafficResponse
                {
                    Status = ResponseStatus.Susscess,
                    categories = categoriesResult,
                    data = data
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //[HttpGet]
        //[Route("Test")]
        //public async Task<CsatResponeDetailResponse> Test()
        //{
        //    string connectionString = _config.GetValue<string>("ConnectionStrings:Ef_Postgres_Db");

        //    NpgsqlConnection conn = null;

        //    try
        //    {
        //        // Initialization
        //        conn = new NpgsqlConnection(connectionString);

        //        // Open connection
        //        conn.Open();

        //        var sql = "select * from get_data();";

        //        using var cmd = new NpgsqlCommand(sql, conn);
        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, connectionString);

        //        DataTable dt = new System.Data.DataTable();
        //        da.Fill(dt);

        //        return new CsatResponeDetailResponse
        //        {
        //            Status = ResponseStatus.Susscess,
        //            result = dt.ToJson(),
        //            total = 10
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return null; 
        //    }
        //    finally
        //    {
        //        // Close connection
        //        if (null != conn)
        //        {
        //            conn.Close();

        //        }
        //    }
        //}

    }

}
