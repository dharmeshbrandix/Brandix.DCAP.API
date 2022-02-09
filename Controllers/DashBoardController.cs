/*
Description: Secuser Controller Class
Created By : NalindaW
Created on : 2019-02-26
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Brandix.DCAP.API.CustomModels;
using Brandix.DCAP.API.Models;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.ComponentModel;

//Added by NS
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

namespace Brandix.DCAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]

    public class DashBoardController : ControllerBase
    {
        #region Variable Declarations
        ILog logger;
        private DCAPDbContext dcap;
        private Guid gu;

        #endregion

        #region Constructor
        public DashBoardController(DCAPDbContext context)
        {
            dcap = context;
            //Create log4net reference
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            logger = LogManager.GetLogger(typeof(Program));

        }
        #endregion

        #region APIs

        [Produces("application/json")]
        [HttpGet("GetTeamWiseHourlyProduction")]
        public List<HourlyProduction> GetTeamWiseHourlyProduction()
        {
            string FacCode = "K06";
            string LocCode = "K06";
            DateTime TxnDate = System.DateTime.Now;
            string StyleId = "";
            string ScheduleId = "";
            int ShiftID = 1;

            //logger.InfoFormat("GetProdHourByTeamId API called with TeamId={0}", TeamId);
            Dictionary<int, HourlyProduction> HourlyProd = new Dictionary<int, HourlyProduction>();

            HourlyCounts hc = new HourlyCounts();
            HourlyCounts hctemp = new HourlyCounts();
            Guid gu = Guid.NewGuid();

            //HourlyCounts hctemp = new HourlyCounts();
            //LookupController lookup = new LookupController(dcap,gu);
            LookupController lookup = new LookupController(dcap);//,gu);
            IList<Team> Teams = null;
            IList<Detxn> Detxn = null;
            List<HourlyProduction> HourlyProduction = null;
            HourlyProduction hp = null;

            try
            {
                //Detxn = lookup.GetLineWiseHourlyProduction(  FacCode,   TxnDate,   StyleId,   ScheduleId,   ShiftID);
                Teams = lookup.GetAllTeamsForFactory(FacCode, LocCode);

                foreach (Team tm in Teams)
                {
                    IList<Prodhour> Prodhours = null;
                    Prodhours = lookup.GetProdHoursForFactory(101, ShiftID);

                    foreach (Prodhour ph in Prodhours)
                    {
                        hc = lookup.GetProductionQtyForHour(101, ph.HourNo, TxnDate);

                        if (hc != null)
                        {
                            switch (Convert.ToUInt32(ph.HourName.ToString()))
                            {
                                case 1:
                                    {
                                        hp.HourNo01_GDQty = hc.CurHrGood;
                                        hp.HourNo01_SCQty = hc.CurHrScrap;
                                        hp.HourNo01_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 2:
                                    {
                                        hp.HourNo02_GDQty = hc.CurHrGood;
                                        hp.HourNo02_SCQty = hc.CurHrScrap;
                                        hp.HourNo02_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 3:
                                    {
                                        hp.HourNo03_GDQty = hc.CurHrGood;
                                        hp.HourNo03_SCQty = hc.CurHrScrap;
                                        hp.HourNo03_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 4:
                                    {
                                        hp.HourNo04_GDQty = hc.CurHrGood;
                                        hp.HourNo04_SCQty = hc.CurHrScrap;
                                        hp.HourNo04_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 5:
                                    {
                                        hp.HourNo05_GDQty = hc.CurHrGood;
                                        hp.HourNo05_SCQty = hc.CurHrScrap;
                                        hp.HourNo05_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 6:
                                    {
                                        hp.HourNo06_GDQty = hc.CurHrGood;
                                        hp.HourNo06_SCQty = hc.CurHrScrap;
                                        hp.HourNo06_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 7:
                                    {
                                        hp.HourNo07_GDQty = hc.CurHrGood;
                                        hp.HourNo07_SCQty = hc.CurHrScrap;
                                        hp.HourNo07_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 8:
                                    {
                                        hp.HourNo08_GDQty = hc.CurHrGood;
                                        hp.HourNo08_SCQty = hc.CurHrScrap;
                                        hp.HourNo08_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 9:
                                    {
                                        hp.HourNo09_GDQty = hc.CurHrGood;
                                        hp.HourNo09_SCQty = hc.CurHrScrap;
                                        hp.HourNo09_RWQty = hc.CurHrRework;
                                        break;
                                    }
                                case 10:
                                    {
                                        hp.HourNo10_GDQty = hc.CurHrGood;
                                        hp.HourNo10_SCQty = hc.CurHrScrap;
                                        hp.HourNo10_RWQty = hc.CurHrRework;
                                        break;
                                    }
                            }
                            HourlyProduction.Add(hp);


                        }
                    }
                }
                // if (hctemp != null)
                // {
                //    hc.CurHrGood = hctemp.CurHrGood;
                //    hc.CurHrScrap = hctemp.CurHrScrap;
                //    hc.CurHrRework = hctemp.CurHrRework;
                // }            
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                //throw e;
            }

            return HourlyProduction;
        }

        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStyleScheduleByBarcode")]
        public StyleScheduleColor GetStyleScheduleByBarcode(string Barcode)
        {

            logger.InfoFormat("GetColorsByStylesNo By Barcode Barcode ={0}", Barcode);
            StyleScheduleColor Color = null;
            L4 L4 = null;

            try
            {
                Color = (from l1 in dcap.L1
                         join l2 in dcap.L2 on l1.L1id equals l2.L1id
                         join lbc in dcap.L5bc on new { A = l2.L1id, B = l2.L2id } equals new { A = lbc.L1id, B = lbc.L2id }
                         where lbc.BarCodeNo == Barcode && l1.RecStatus == (int)eRecStatus.Active
                         select new StyleScheduleColor
                         {
                             StyleId = l1.L1id,
                             StyleNo = l1.L1no,
                             StyleDesc = l1.L1desc,
                             ScheduleId = l2.L2id,
                             ScheduleNo = l2.L2no,
                             ScheduleDesc = l2.L2desc,
                             Zfeature = l2.Ref02,
                             PONo = l2.Ref01,
                             SizeDesc = lbc.L5desc,
                             SizeId = lbc.L5id,
                             ColorId = lbc.L4id,
                             WFId = l2.Wfid,
                             Pattern = lbc.Pattern,
                             LotNo = lbc.LotNo
                         }).FirstOrDefault();

                if (Color != null)
                {
                    if (Color.ColorId != 0)
                    {
                        L4 = (from l4 in dcap.L4
                              where l4.L1id == Color.StyleId && l4.L2id == Color.ScheduleId && l4.L4id == Color.ColorId
                              select new L4
                              {
                                  L4no = l4.L4no
                              }).FirstOrDefault();

                        if (L4 != null)
                        {
                            Color.ColorNo = L4.L4no;
                        }
                    }
                    else
                    {
                        Color.ColorNo = "-N/A-";
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());

            }
            return Color;
        }

        #endregion

        //NEW ******************************************************************** //


        //NS
        // ***************************************************************************//



        //GET api /get Production Status by Team - Report 2 - WIP Report by team - Dropdown Data Factory
        [Produces("application/json")]
        [HttpGet("GetFactoryNames")]
        public IList<GetFactoryName> GetFactoryNames()
        {

            logger.InfoFormat("Get Factory Name");
            IList<GetFactoryName> L1Details = null;

            try
            {
                L1Details = (from factory in dcap.Factory.Where(c => c.RecStatus == 1).AsQueryable()
                             group factory by new
                             {
                                 factory.FacCode,  //Factory Code
                                 factory.FacName,  //Factory Name
                             }
                         into grp
                             orderby grp.Key.FacCode, grp.Key.FacName
                             select new GetFactoryName
                             {
                                 Factory_Code = grp.Key.FacCode,
                                 Factory_Name = grp.Key.FacName,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                //throw e;
            }

            return L1Details;
        }

        //GET api /get Production Status by Team - Report 2 - WIP Report by team - Dropdown Data Team
        [Produces("application/json")]
        [HttpGet("GetTeamNamesByFactoryCode")]
        public IList<GetTeamName> GetTeamNames(string FactoryCode)
        {

            logger.InfoFormat("Get Team Names By FactoryCode with FactoryCode={0}", FactoryCode);
            IList<GetTeamName> L1Details = null;

            try
            {
                L1Details = (from team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable()
                             group team by new
                             {
                                 team.TeamId,  //Team ID
                                 team.TeamName,  //Team Name
                             }
                         into grp
                             orderby grp.Key.TeamId, grp.Key.TeamName
                             select new GetTeamName
                             {
                                 Team_ID = grp.Key.TeamId,
                                 Team_Name = grp.Key.TeamName,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                //throw e;
            }

            return L1Details;
        }

        //GET api /get Rework by Team - Report 6  - Dropdown Data Operations
        [Produces("application/json")]
        [HttpGet("GetOperations")]
        public IList<GetOperationName> GetOperations()
        {

            logger.InfoFormat("Get Factory Name");
            IList<GetOperationName> L1Details = null;

            try
            {
                L1Details = (from dep in dcap.Dep
                             group dep by new
                             {
                                 dep.OperationCode,  //Operation Code
                                 dep.Depdesc,  //Operation Name
                             }
                         into grp
                             orderby grp.Key.OperationCode, grp.Key.Depdesc
                             select new GetOperationName
                             {
                                 Operation_ID = grp.Key.OperationCode,
                                 Operation_Name = grp.Key.Depdesc,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Operations  information {0}", e.ToString());
                //throw e;
            }

            return L1Details;
        }

        //End of Dropdown Data *********************************************************************************//

        //GET api /get Production Status by Team - Report 2 - WIP Report by team - Output Data - error
        [Produces("application/json")]
        [HttpGet("WIPReportByTeam")]
        public IList<WIPReportByTeam> WIPReportByTeam(string FactoryCode, uint Team, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("WIPReport By Team with FactoryCode={0}, StartDate={1}, EndDate={2}, Team={3}", FactoryCode, StartDate, EndDate, Team);
            IList<WIPReportByTeam> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate && detxn.TeamId == Team).AsQueryable()
                                 join dep in dcap.Dep on detxn.OperationCode equals dep.OperationCode
                                 join l1 in dcap.L1 on (uint?)detxn.L1id equals (uint?)l1.L1id
                                 join l2 in dcap.L2 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                 join l4 in dcap.L4 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                 join l5 in dcap.L5 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                 //join l5moops in dcap.L5moops.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id, F = (int?)detxn.L5moid } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id, F = (int?)l5moops.L5moid }
                                 join team in dcap.Team.Select(team => new { team.TeamId, team.FacCode }).Where(team => team.TeamId == Team).AsQueryable() on detxn.TeamId equals team.TeamId
                                 group detxn by new
                                 {
                                     l1.L1desc,  //Style
                                     l2.L2desc,  //Shedule
                                     l2.Ref01,  //PO
                                     l4.L4desc,  //Color
                                     l5.L5desc,  //Size
                                     dep.OperationCode, //key
                                     dep.Depdesc,  //Operation
                                     //l5moops.OrderQty, //Order Quantity
                                     //detxn.Qty01, //MO Quantity
                                 }
                         into grp
                                 orderby grp.Key.L1desc, grp.Key.L2desc, grp.Key.Ref01, grp.Key.L4desc, grp.Key.L5desc, grp.Key.OperationCode, grp.Key.Depdesc
                                 select new
                                 {
                                     Style = grp.Key.L1desc,
                                     Shedule = grp.Key.L2desc,
                                     PO = grp.Key.Ref01,
                                     Color = grp.Key.L4desc,
                                     Size = grp.Key.L5desc,
                                     Operation = grp.Key.Depdesc,
                                     Order_Qty = 0,//grp.Key.OrderQty,
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),//grp.Key.Qty01,//
                                 }).ToList();
                //L1Details.Sort();
                var PDetails = L1Details.ToPivotTable(
                    item => item.Operation,
                    item => new { item.Style, item.Shedule, item.PO, item.Color, item.Size, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                DataColumn[] OperationColumns = PDetails.Columns.Cast<DataColumn>().Skip(6).ToArray();
                DataColumn[] OperationColumns1 = PDetails.Columns.Cast<DataColumn>().Skip(7).ToArray();
                L2Details = PDetails.AsEnumerable()
                    .Select(r => new WIPReportByTeam
                    {
                        Style = r.Field<string>("Style"),
                        Shedule = r.Field<string>("Shedule"),
                        PO = r.Field<string>("PO"),
                        Color = r.Field<string>("Color"),
                        Size = r.Field<string>("Size"),
                        Order_Qty = Convert.ToDecimal(r.Field<string>("Order_Qty")),
                        OperationQty = OperationColumns.Select(c => new
                        {
                            Operation = c.ColumnName,
                            Qty = Convert.ToDecimal(r.Field<string>(c)),
                        }).ToDictionary(x => x.Operation, x => x.Qty),
                        WIP = OperationColumns1.Where(c => r.Field<string>(7) != null).Select(c => new
                        {
                            Operation = c.ColumnName,
                            Qty = (Convert.ToDecimal(r.Field<string>(6)) - Convert.ToDecimal(r.Field<string>(7))),
                        }).ToDictionary(x => x.Operation, x => x.Qty)
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L2Details;
        }

        //End Report 02 - WIp Report By Factory and Team ********************************************************************************//


        //GET api /get Production Status by Factory - Report 3 - WIP Report
        [Produces("application/json")]
        [HttpGet("WIPReportByFactory")]
        public IList<WIPReport> WIPReportByFactory(string FactoryCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("WIPReport By FactoryCode with FactoryCode={0}, StartDate={1}, EndDate={2}", FactoryCode, StartDate, EndDate);
            IList<WIPReport> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate).AsQueryable()
                                 join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                 join l2 in dcap.L2.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                 join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                 join l5 in dcap.L5.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                 //join l5moops in dcap.L5moops.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id, F = (int?)detxn.L5moid } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id, F = (int?)l5moops.L5moid }
                                 join team in dcap.Team.Select(team => new { team.TeamId, team.FacCode }).Where(team => team.FacCode == FactoryCode).AsQueryable() on detxn.TeamId equals team.TeamId
                                 group detxn by new
                                 {
                                     l1.L1desc,  //Style
                                     l2.L2desc,  //Shedule
                                     l2.Ref01,  //PO
                                     l4.L4desc,  //Color
                                     l5.L5desc,  //Size
                                     //l5moops.OrderQty, //Order Qty
                                     detxn.OperationCode, //operation code
                                     //l5moops.ReportedQty, //Reported Qty
                                     //detxn.Qty01, //Reported Qty
                                 }
                         into grp
                                 orderby grp.Key.L1desc, grp.Key.L2desc, grp.Key.Ref01, grp.Key.L4desc, grp.Key.L5desc, grp.Key.OperationCode//, grp.Key.OrderQty
                                 select new
                                 {
                                     Style = grp.Key.L1desc,
                                     Shedule = grp.Key.L2desc,
                                     PO = grp.Key.Ref01,
                                     Color = grp.Key.L4desc,
                                     Size = grp.Key.L5desc,
                                     Operation = grp.Key.OperationCode,
                                     Order_Qty = 0,//grp.Key.OrderQty,
                                     //Reported_Qty = grp.Key.ReportedQty,
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01), //wrong//grp.Key.Qty01,//
                                 }).ToList();
                //L1Details.Sort();
                var PDetails = L1Details.ToPivotTable(
                  item => item.Operation,
                  item => new { item.Style, item.Shedule, item.PO, item.Color, item.Size, item.Order_Qty }, //item.Reported_Qty },
                  items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                //DataColumn[] OperationColumns = PDetails.Columns.Cast<DataColumn>().Skip(7).ToArray();
                DataColumnCollection col = PDetails.Columns;
                L2Details = PDetails.AsEnumerable()
                    .Select(r => new WIPReport
                    {
                        Style = r.Field<string>("Style"),
                        Shedule = r.Field<string>("Shedule"),
                        PO = r.Field<string>("PO"),
                        Color = r.Field<string>("Color"),
                        Size = r.Field<string>("Size"),
                        Order_Qty = Convert.ToDecimal(r.Field<string>("Order_Qty")),
                        //Reported_Qty = Convert.ToDecimal(r.Field<string>("Reported_Qty")),
                        //M

                        M_Laying = col.Contains("10") ? Convert.ToDecimal(r.Field<string>("10")) : 0,
                        M_Cutting = col.Contains("15") ? Convert.ToDecimal(r.Field<string>("15")) : 0,
                        M_EM_1_SQ_PA_Send_PF = col.Contains("41") ? Convert.ToDecimal(r.Field<string>("41")) : 0,
                        M_EM_1_SQ_PA_Receive_PF = col.Contains("42") ? Convert.ToDecimal(r.Field<string>("42")) : 0,
                        M_Sewing_In = col.Contains("129") ? Convert.ToDecimal(r.Field<string>("129")) : 0,
                        M_Sew_Out = WIPOutputByOperationValue(col, r, "130", "131", "NA", "NA", "+", "+", "+", "+"),
                        M_Washing_Send = WIPOutputByOperationValue(col, r, "151", "155", "NA", "NA", "+", "+", "+", "+"),
                        M_BFL_Wash_In = col.Contains("156") ? Convert.ToDecimal(r.Field<string>("156")) : 0,
                        M_BFL_Wash_Out = col.Contains("157") ? Convert.ToDecimal(r.Field<string>("157")) : 0,
                        M_Washing_Receive = WIPOutputByOperationValue(col, r, "160", "170", "NA", "NA", "+", "+", "+", "+"),
                        M_Finishing_In = col.Contains("180") ? Convert.ToDecimal(r.Field<string>("180")) : 0,
                        M_Poly_Bag_Packing = col.Contains("190") ? Convert.ToDecimal(r.Field<string>("190")) : 0,
                        M_Carton_Packing = col.Contains("200") ? Convert.ToDecimal(r.Field<string>("200")) : 0,

                        //WIP
                        WIP_Laying = WIPOutputByOperationValue(col, r, "Order_Qty", "10", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Cutting = WIPOutputByOperationValue(col, r, "10", "15", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Send_PF = WIPOutputByOperationValue(col, r, "15", "41", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Receive_PF = WIPOutputByOperationValue(col, r, "41", "42", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sewing_In = WIPOutputByOperationValue(col, r, "42", "129", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sew_Out = WIPOutputByOperationValue(col, r, "129", "130", "131", "NA", "+", "-", "-", "+"),
                        WIP_Washing_Send = WIPOutputByOperationValue(col, r, "130", "131", "151", "155", "+", "+", "-", "-"),
                        WIP_BFL_Wash_In = WIPOutputByOperationValue(col, r, "151", "155", "156", "NA", "+", "+", "-", "+"),
                        WIP_BFL_Wash_Out = WIPOutputByOperationValue(col, r, "156", "157", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Washing_Receive = WIPOutputByOperationValue(col, r, "157", "160", "170", "NA", "+", "-", "-", "+"),
                        WIP_Finishing_In = WIPOutputByOperationValue(col, r, "160", "170", "180", "NA", "+", "+", "-", "+"),
                        WIP_Poly_Bag_Packing = WIPOutputByOperationValue(col, r, "180", "190", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Carton_Packing = WIPOutputByOperationValue(col, r, "190", "200", "NA", "NA", "+", "-", "+", "+"),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L2Details;
        }

        //Calculate WIP
        public static decimal WIPOutputByOperationValue(DataColumnCollection col, DataRow r, string cop1, string cop2, string cop3, string cop4, string sop1, string sop2, string sop3, string sop4)
        {
            decimal total = 0;
            if (col.Contains(cop1) && col.Contains(cop2) && col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (col.Contains(cop1) && col.Contains(cop2) && col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)));
            }
            else if (col.Contains(cop1) && col.Contains(cop2) && !col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (col.Contains(cop1) && col.Contains(cop2) && !col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)));
            }
            else if (col.Contains(cop1) && !col.Contains(cop2) && col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (col.Contains(cop1) && !col.Contains(cop2) && col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)));
            }
            else if (col.Contains(cop1) && !col.Contains(cop2) && !col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (col.Contains(cop1) && !col.Contains(cop2) && !col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop1 == "+" ? Convert.ToDecimal(r.Field<string>(cop1)) : -Convert.ToDecimal(r.Field<string>(cop1)));
            }
            else if (!col.Contains(cop1) && col.Contains(cop2) && col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (!col.Contains(cop1) && col.Contains(cop2) && col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)));
            }
            else if (!col.Contains(cop1) && col.Contains(cop2) && !col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (!col.Contains(cop1) && col.Contains(cop2) && !col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop2 == "+" ? Convert.ToDecimal(r.Field<string>(cop2)) : -Convert.ToDecimal(r.Field<string>(cop2)));
            }
            else if (!col.Contains(cop1) && !col.Contains(cop2) && col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)))
                + (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }
            else if (!col.Contains(cop1) && !col.Contains(cop2) && col.Contains(cop3) && !col.Contains(cop4))
            {
                total = (sop3 == "+" ? Convert.ToDecimal(r.Field<string>(cop3)) : -Convert.ToDecimal(r.Field<string>(cop3)));
            }
            else if (!col.Contains(cop1) && !col.Contains(cop2) && !col.Contains(cop3) && col.Contains(cop4))
            {
                total = (sop4 == "+" ? Convert.ToDecimal(r.Field<string>(cop4)) : -Convert.ToDecimal(r.Field<string>(cop4)));
            }

            return total;
        }
        //End Report 03 - WIp Report By Factory ********************************************************************************//

        //GET api /style/GetStatusByBarcode - API to get Style Status By Barcode Information - Report 5     
        [Produces("application/json")]
        [HttpGet("GetStatusByBarcode")]
        public IList<BarcodeStatus> GetStatusByBarcode(string Barcode)
        {

            logger.InfoFormat("Get Status By Barcode with Barcode ={0}", Barcode);
            IList<BarcodeStatus> Details = null;

            try
            {
                Details = (from detxn in dcap.Detxn.Where(detxn => detxn.BarCodeNo == Barcode && detxn.RecStatus == 1).AsQueryable()
                           join dep in dcap.Dep on detxn.Depid equals dep.Depid
                           join l1 in dcap.L1 on (uint?)detxn.L1id equals (uint?)l1.L1id
                           join l2 in dcap.L2 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                           join l4 in dcap.L4 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                           join lbc in dcap.L5bc on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)lbc.L1id, B = (uint?)lbc.L2id, C = (uint?)lbc.L3id, D = (uint?)lbc.L4id, E = (uint?)lbc.L5id }
                           group detxn by new
                           {
                               detxn.TxnDateTime,
                               //detxn.OperationCode,
                               dep.Depdesc,
                               l1.L1desc,
                               l2.L2no,
                               l2.L2desc,
                               l2.Ref01,
                               l4.L4desc,
                               lbc.L5desc,
                               detxn.Qty01,
                               detxn.Qty02,
                               detxn.Qty03,
                               lbc.LotNo,
                               lbc.Pattern,
                           }
                         into grp
                           orderby grp.Key.TxnDateTime, //grp.Key.OperationCode, 
                           grp.Key.Depdesc, grp.Key.L1desc,
                           grp.Key.L2no, grp.Key.L2desc, grp.Key.Ref01, grp.Key.L4desc, grp.Key.L5desc
                           select new BarcodeStatus
                           {
                               Transaction_Date_and_Time = grp.Key.TxnDateTime,
                               //Operation = grp.Key.OperationCode,
                               Department = grp.Key.Depdesc,
                               Style = grp.Key.L1desc,
                               Schedule_ID = grp.Key.L2desc,
                               Purchase_Order = grp.Key.Ref01,
                               Color = grp.Key.L4desc,
                               Size = grp.Key.L5desc,
                               Manufacturing_Qty = grp.Key.Qty01,//grp.Sum(detxn => detxn.Qty01), //not working
                               Qty_Report = grp.Key.Qty02,//grp.Sum(detxn => detxn.Qty02),
                               Qty_Scrap = grp.Key.Qty03,//grp.Sum(detxn => detxn.Qty03),
                               Shade_Lot = grp.Key.LotNo, //not found
                               Shrinkage = grp.Key.Pattern, //not found
                           }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                //throw e;
            }
            return Details;
        }
        //End Report 05 - Barcode Status By Factory ********************************************************************************//


        //GET api /get Operation Rework Status by Faccode, Operation and Date - Report 6
        [Produces("application/json")]
        [HttpGet("ReworkDetailsByOperation")]
        public IList<ProducrionStatus> ReworkDetailsByOperation(string FactoryCode, int OperationCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("Get Transaction Details By Style Factory Operation Date with FactoryCode ={0}, OperationCode ={1}, StartDate ={2}, EndDate ={3}", FactoryCode, OperationCode, StartDate, EndDate);
            IList<ProducrionStatus> Details = null;

            try
            {
                Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate && detxn.Rrid > 0 && detxn.OperationCode == OperationCode).AsQueryable()
                           join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable() on detxn.TeamId equals team.TeamId
                           join rejectreason in dcap.Rejectreason.AsQueryable() on (uint?)detxn.Rrid equals (uint?)rejectreason.Rrid
                           join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                           join l2 in dcap.L2.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                           join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                           join l5 in dcap.L5.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                           where detxn.RecStatus == 1 && detxn.TxnDateTime < EndDate && detxn.TxnDateTime > StartDate
                            && team.FacCode == FactoryCode && detxn.OperationCode == OperationCode
                           group detxn by new
                           {
                               detxn.TxnDateTime,
                               l1.L1desc, //Style
                               l2.L2desc, //Shedule
                               l4.L4desc, //Color
                               l5.L5desc, //Size
                               team.TeamName, //Team name
                               detxn.BarCodeNo, //Barcode
                               rejectreason.Scode, //Reject Category
                               rejectreason.Rrdesc, //Reject Reason
                               detxn.RecStatus, //Record Status
                           }
                         into grp
                           orderby grp.Key.TxnDateTime
                           select new ProducrionStatus
                           {
                               Date_and_Time = grp.Key.TxnDateTime,
                               Style = grp.Key.L1desc,
                               Shedule = grp.Key.L2desc,
                               Color = grp.Key.L4desc,
                               Size = grp.Key.L5desc,
                               Team_Name = grp.Key.TeamName,
                               Barcode = grp.Key.BarCodeNo,
                               Rework_Cat = grp.Key.Scode,
                               Rework_Reason = grp.Key.Rrdesc,
                               Rework_Quantity = grp.Sum(detxn => detxn.Qty03),
                               Record_Status = grp.Key.RecStatus,
                           }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Transaction Details By Factory Operation Date information {0}", e.ToString());
                //throw e;
            }
            return Details;
        }


        //GET api /get Hourly Production Status by Team - Report 9 //error in production target
        //Completed - need to improve eff
        [Produces("application/json")]
        [HttpGet("GetHourlyProductionStatusByOperation")]
        public IList<HourlyProductionStatus> GetHourlyProductionStatusByOperation(string FactoryCode, int OperationCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("Get Hourly Production Status By Team with FactoryCode ={0}, OperationCod ={1}, StartDate ={2}, EndDate ={3},", FactoryCode, OperationCode, StartDate, EndDate);
            IList<HourlyProductionStatus> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime >= StartDate && detxn.TxnDateTime <= EndDate && detxn.OperationCode == OperationCode && detxn.RecStatus == 1).AsQueryable()
                                 join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable() on detxn.TeamId equals team.TeamId
                                 join prodhour in dcap.Prodhour.AsQueryable() on new { A = (uint?)detxn.HourNo, B = (uint?)detxn.TeamId } equals new { A = (uint?)prodhour.HourNo, B = (uint?)prodhour.TeamId }
                                 where detxn.RecStatus == 1 && detxn.TxnDateTime < EndDate && detxn.TxnDateTime > StartDate
                                 && team.FacCode == FactoryCode && detxn.OperationCode == OperationCode
                                 group detxn by new
                                 {
                                     team.TeamId,
                                     team.TeamName,
                                     //prodtarget.Qty01,
                                     prodhour.HourNo,
                                     prodhour.HourName,
                                 }
                         into grp
                                 orderby grp.Key.TeamName, grp.Key.HourName
                                 select new
                                 {
                                     Team_Id = grp.Key.TeamId,
                                     Team_Name = grp.Key.TeamName,
                                     Team_Target = 1000,//grp.Key.Qty01,//
                                     Hour_Name = grp.Key.HourName,
                                     Team_Total_Key = "Team Total",
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                                     Qty_Report = grp.Sum(detxn => detxn.Qty02),
                                     Qty_Scrap = grp.Sum(detxn => detxn.Qty03),
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_M",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_R",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_S",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                var P1TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_M",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_R",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_S",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Team_Id"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Team_Id"] };
                P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["Team_Id"] };

                P1TDetails.PrimaryKey = new DataColumn[] { P1TDetails.Columns["Team_Id"] };
                P2TDetails.PrimaryKey = new DataColumn[] { P2TDetails.Columns["Team_Id"] };
                P3TDetails.PrimaryKey = new DataColumn[] { P3TDetails.Columns["Team_Id"] };

                P1Details.Merge(P2Details);
                P1Details.Merge(P3Details);

                P1Details.Merge(P1TDetails);
                P1Details.Merge(P2TDetails);
                P1Details.Merge(P3TDetails);

                DataColumn[] OperationColumns_M = P1Details.Columns.Cast<DataColumn>().Skip(3).ToArray();

                L2Details = P1Details.AsEnumerable()
                    .Select(r => new HourlyProductionStatus
                    {
                        Team_Id = Convert.ToInt16(r.Field<string>("Team_Id")),
                        Team_Name = r.Field<string>("Team_Name"),
                        Team_Target = Convert.ToDecimal(r.Field<string>("Team_Target")),
                        Qty = OperationColumns_M.Where(c => Convert.ToDecimal(r.Field<string>(c)) != 0).Select(c => new
                        {
                            Operation = c.ColumnName,
                            Qty = Convert.ToDecimal(r.Field<string>(c)),
                        }).ToDictionary(x => x.Operation, x => x.Qty),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetHourlyProductionStatusByTeam information {0}", e.ToString());

            }
            return L2Details;
        }

        //Part II - Hourly Total
        [Produces("application/json")]
        [HttpGet("GetHourlyTotalProductionStatusByOperation")]
        public IList<HourlyProductionStatus> GetHourlyTotalProductionStatusByOperation(string FactoryCode, int OperationCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("Get Hourly Production Status By Team with FactoryCode ={0}, OperationCod ={1}, StartDate ={2}, EndDate ={3},", FactoryCode, OperationCode, StartDate, EndDate);
            IList<HourlyProductionStatus> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate && detxn.RecStatus == 1 && detxn.OperationCode == OperationCode).AsQueryable()
                                 join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable() on detxn.TeamId equals team.TeamId
                                 join prodhour in dcap.Prodhour.AsQueryable() on new { A = (uint?)detxn.HourNo, B = (uint?)detxn.TeamId } equals new { A = (uint?)prodhour.HourNo, B = (uint?)prodhour.TeamId }
                                 where detxn.RecStatus == 1 && detxn.TxnDateTime < EndDate && detxn.TxnDateTime > StartDate
                                 && team.FacCode == FactoryCode && detxn.OperationCode == OperationCode
                                 group detxn by new
                                 {
                                     prodhour.HourNo,
                                     prodhour.HourName,
                                 }
                         into grp
                                 orderby grp.Key.HourName
                                 select new
                                 {
                                     Team_Id = "404",
                                     Team_Name = "Hourly Total",
                                     Team_Target = 1000,
                                     Hour_Name = grp.Key.HourName,
                                     Team_Total_Key = "Team Total",
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                                     Qty_Report = grp.Sum(detxn => detxn.Qty02),
                                     Qty_Scrap = grp.Sum(detxn => detxn.Qty03),
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_M",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_R",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_S",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                var P1TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_M",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_R",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_S",
                    item => new { item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Team_Id"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Team_Id"] };
                P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["Team_Id"] };

                P1TDetails.PrimaryKey = new DataColumn[] { P1TDetails.Columns["Team_Id"] };
                P2TDetails.PrimaryKey = new DataColumn[] { P2TDetails.Columns["Team_Id"] };
                P3TDetails.PrimaryKey = new DataColumn[] { P3TDetails.Columns["Team_Id"] };

                P1Details.Merge(P2Details);
                P1Details.Merge(P3Details);

                P1Details.Merge(P1TDetails);
                P1Details.Merge(P2TDetails);
                P1Details.Merge(P3TDetails);

                DataColumn[] OperationColumns_M = P1Details.Columns.Cast<DataColumn>().Skip(2).ToArray();

                L2Details = P1Details.AsEnumerable()
                    .Select(r => new HourlyProductionStatus
                    {
                        Team_Id = Convert.ToInt16(r.Field<string>("Team_Id")),
                        Team_Name = r.Field<string>("Team_Name"),
                        Team_Target = Convert.ToDecimal(r.Field<string>("Team_Target")),
                        Qty = OperationColumns_M.Where(c => Convert.ToDecimal(r.Field<string>(c)) != 0).Select(c => new
                        {
                            Operation = c.ColumnName,
                            Qty = Convert.ToDecimal(r.Field<string>(c)),
                        }).ToDictionary(x => x.Operation, x => x.Qty),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetHourlyProductionStatusByTeam information {0}", e.ToString());

            }
            return L2Details;
        }

        //Dashboard New
        //Part II - Hourly Total
        [Produces("application/json")]
        [HttpGet("GetHourlyTotalProductionStatus")]
        public IList<HourlyProductionStatus> GetHourlyTotalProductionStatus(int WFid, int OperationCode, DateTime StartDate, DateTime EndDate, int StyleId, int SheduleId)
        {

            logger.InfoFormat("Get Hourly Production Status By Team with FactoryCode ={0}, OperationCod ={1}, StartDate ={2}, EndDate ={3}, StyleId={4}, SheduleId={5}", WFid, OperationCode, StartDate, EndDate, StyleId, SheduleId);
            IList<HourlyProductionStatus> L2Details = null;

            try
            {
                IList<Detxn> Detxn = null;
                IList<Team> Team = null;
                IList<Prodhour> Prodhour = null;
                IList<Prodtarget> Prodtarget = null;

                if (StyleId == 0 && SheduleId == 0)
                {
                    Detxn = dcap.Detxn
                                    .Where(d => d.OperationCode == OperationCode && d.TxnDateTime >= StartDate && d.TxnDateTime <= EndDate && d.Wfid == WFid && d.RecStatus == 1)
                                    .GroupBy(d => new { d.OperationCode, d.TeamId, d.HourNo, d.TxnDateTime.Date })
                                    .Select(d => new Detxn { OperationCode = d.Key.OperationCode, TeamId = d.Key.TeamId, HourNo = d.Key.HourNo, TxnDateTime = d.Key.Date, Qty01 = d.Sum(c => c.Qty01), Qty02 = d.Sum(c => c.Qty02), Qty03 = d.Sum(c => c.Qty03) })
                                    .ToList();

                    var FactoryCode = dcap.Wf.Where(c => c.Wfid == WFid).Select(c => c.FacCode).FirstOrDefault();
                    if (FactoryCode != null && FactoryCode != "")
                    {
                        Team = dcap.Team.Where(c => c.FacCode == FactoryCode && c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }
                    else
                    {
                        Team = dcap.Team.Where(c => c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }

                    var maxteamid = 10000;
                    var minteamid = 0;
                    if (Team.Count != 0)
                    {
                        maxteamid = (int)Team.Max(c => c.TeamId);
                        minteamid = (int)Team.Min(c => c.TeamId);
                    }

                    Prodhour = dcap.Prodhour.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid).ToList();
                    Prodtarget = dcap.Prodtarget.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid && c.FacCode == FactoryCode && c.TxnDate.Date >= StartDate.Date && c.TxnDate.Date <= EndDate.Date && c.OperationCode == OperationCode)
                                    .GroupBy(c => new { c.FacCode, c.TeamId, c.OperationCode, c.TxnDate.Date })
                                    .Select(c => new Prodtarget { FacCode = c.Key.FacCode, TeamId = c.Key.TeamId, OperationCode = c.Key.OperationCode, TxnDate = c.Key.Date, Qty01 = c.Sum(d => d.Qty01) })
                                    .ToList();
                }
                else if (SheduleId == 0)
                {
                    Detxn = dcap.Detxn
                                    .Where(d => d.OperationCode == OperationCode && d.TxnDateTime >= StartDate && d.TxnDateTime <= EndDate && d.L1id == StyleId && d.Wfid == WFid && d.RecStatus == 1)
                                    .GroupBy(d => new { d.OperationCode, d.TeamId, d.HourNo, d.TxnDateTime.Date })
                                    .Select(d => new Detxn { OperationCode = d.Key.OperationCode, TeamId = d.Key.TeamId, HourNo = d.Key.HourNo, TxnDateTime = d.Key.Date, Qty01 = d.Sum(c => c.Qty01), Qty02 = d.Sum(c => c.Qty02), Qty03 = d.Sum(c => c.Qty03) })
                                    .ToList();

                    var FactoryCode = dcap.Wf.Where(c => c.Wfid == WFid).Select(c => c.FacCode).ToString();
                    if (FactoryCode != null && FactoryCode != "")
                    {
                        Team = dcap.Team.Where(c => c.FacCode == FactoryCode && c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }
                    else
                    {
                        Team = dcap.Team.Where(c => c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }

                    var maxteamid = 10000;
                    var minteamid = 0;
                    if (Team.Count != 0)
                    {
                        maxteamid = (int)Team.Max(c => c.TeamId);
                        minteamid = (int)Team.Min(c => c.TeamId);
                    }

                    Prodhour = dcap.Prodhour.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid).ToList();
                    Prodtarget = dcap.Prodtarget.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid && c.FacCode == FactoryCode && c.TxnDate.Date >= StartDate.Date && c.TxnDate.Date <= EndDate.Date && c.OperationCode == OperationCode)
                                    .GroupBy(c => new { c.FacCode, c.TeamId, c.OperationCode, c.TxnDate.Date })
                                    .Select(c => new Prodtarget { FacCode = c.Key.FacCode, TeamId = c.Key.TeamId, OperationCode = c.Key.OperationCode, TxnDate = c.Key.Date, Qty01 = c.Sum(d => d.Qty01) })
                                 .ToList();
                }
                else
                {
                    Detxn = dcap.Detxn
                                    .Where(d => d.OperationCode == OperationCode && d.TxnDateTime >= StartDate && d.TxnDateTime <= EndDate && d.L1id == StyleId && d.L2id == SheduleId && d.Wfid == WFid && d.RecStatus == 1)
                                    .GroupBy(d => new { d.OperationCode, d.TeamId, d.HourNo, d.TxnDateTime.Date })
                                    .Select(d => new Detxn { OperationCode = d.Key.OperationCode, TeamId = d.Key.TeamId, HourNo = d.Key.HourNo, TxnDateTime = d.Key.Date, Qty01 = d.Sum(c => c.Qty01), Qty02 = d.Sum(c => c.Qty02), Qty03 = d.Sum(c => c.Qty03) })
                                    .ToList();

                    var FactoryCode = dcap.Wf.Where(c => c.Wfid == WFid).Select(c => c.FacCode).ToString();
                    if (FactoryCode != null && FactoryCode != "")
                    {
                        Team = dcap.Team.Where(c => c.FacCode == FactoryCode && c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }
                    else
                    {
                        Team = dcap.Team.Where(c => c.RecStatus == 1).GroupBy(c => new { c.TeamId, c.TeamName }).Select(c => new Team { TeamId = c.Key.TeamId, TeamName = c.Key.TeamName }).ToList();
                    }

                    var maxteamid = 10000;
                    var minteamid = 0;
                    if (Team.Count != 0)
                    {
                        maxteamid = (int)Team.Max(c => c.TeamId);
                        minteamid = (int)Team.Min(c => c.TeamId);
                    }

                    Prodhour = dcap.Prodhour.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid)
                                .Select(c => new Prodhour { HourNo = c.HourNo, TeamId = c.TeamId, HourName = c.HourName, Stime = c.Stime, Etime = c.Etime, EffMinuttes = c.EffMinuttes })
                                .ToList();
                    Prodtarget = dcap.Prodtarget.Where(c => c.TeamId >= minteamid && c.TeamId <= maxteamid && c.FacCode == FactoryCode && c.TxnDate.Date >= StartDate.Date && c.TxnDate.Date <= EndDate.Date && c.OperationCode == OperationCode)
                                    .GroupBy(c => new { c.FacCode, c.TeamId, c.OperationCode, c.TxnDate.Date })
                                    .Select(c => new Prodtarget { FacCode = c.Key.FacCode, TeamId = c.Key.TeamId, OperationCode = c.Key.OperationCode, TxnDate = c.Key.Date, Qty01 = c.Sum(d => d.Qty01) })
                                 .ToList();
                }

                var L1Details = (from detxn in Detxn
                                 join team in Team on detxn.TeamId equals team.TeamId
                                 join prodhour in Prodhour on new { A = (uint?)detxn.TeamId, B = (uint?)detxn.HourNo } equals new { A = (uint?)prodhour.TeamId, B = (uint?)prodhour.HourNo }
                                 join prodtarget in Prodtarget on new { A = (uint?)detxn.TeamId, B = (uint?)detxn.OperationCode, C = detxn.TxnDateTime.Date } equals new { A = (uint?)prodtarget.TeamId, B = (uint?)prodtarget.OperationCode, C = prodtarget.TxnDate.Date }
                                 group new { detxn, team, prodhour } by new//, prodtarget
                                 {
                                     detxn.TxnDateTime.Date,
                                     team.TeamId,
                                     team.TeamName,
                                     prodtarget.Qty01,
                                     prodhour.HourNo,
                                     prodhour.HourName,
                                     prodhour.Stime,
                                     prodhour.Etime
                                 }
                                 into grp
                                 orderby grp.Key.HourName
                                 select new
                                 {
                                     Key = grp.Key.Date.ToString() + grp.Key.TeamId.ToString(),
                                     TxnDateTime = grp.Key.Date,
                                     Team_Id = grp.Key.TeamId,
                                     Team_Name = grp.Key.TeamName,
                                     Team_Target = grp.Key.Qty01,
                                     Hour_Name = grp.Key.Stime + "-" + grp.Key.Etime + "(" + grp.Key.HourName + ")",
                                     Team_Total_Key = "Team Total",
                                     Manufacturing_Qty = grp.Sum(d => d.detxn.Qty01),
                                     Qty_Scrap = grp.Sum(d => d.detxn.Qty02),
                                     Qty_Rework = grp.Sum(d => d.detxn.Qty03),
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_M",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_R",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Rework) : 0);

                var P3Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_S",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                var P1TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_M",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_R",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Rework) : 0);

                var P3TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_S",
                    item => new { item.Key, item.TxnDateTime, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Key"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Key"] };
                P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["Key"] };

                P1TDetails.PrimaryKey = new DataColumn[] { P1TDetails.Columns["Key"] };
                P2TDetails.PrimaryKey = new DataColumn[] { P2TDetails.Columns["Key"] };
                P3TDetails.PrimaryKey = new DataColumn[] { P3TDetails.Columns["Key"] };

                P1Details.Merge(P2Details);
                P1Details.Merge(P3Details);

                P1Details.Merge(P1TDetails);
                P1Details.Merge(P2TDetails);
                P1Details.Merge(P3TDetails);

                DataColumn[] OperationColumns_M = P1Details.Columns.Cast<DataColumn>().Skip(5).ToArray();

                L2Details = P1Details.AsEnumerable()
                    .Select(r => new HourlyProductionStatus
                    {
                        TxnDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                        Team_Id = Convert.ToInt16(r.Field<string>("Team_Id")),
                        Team_Name = r.Field<string>("Team_Name"),
                        Team_Target = Convert.ToDecimal(r.Field<string>("Team_Target")),
                        Qty = OperationColumns_M.Where(c => Convert.ToDecimal(r.Field<string>(c)) != 0).Select(c => new
                        {
                            Hour = c.ColumnName,
                            Qty = Convert.ToDecimal(r.Field<string>(c)),
                        }).OrderBy(c => c.Hour).ToDictionary(x => x.Hour, x => x.Qty),
                    })
                    .OrderBy(c => c.TxnDateTime).ThenBy(c => c.Team_Name)
                    .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetHourlyProductionStatusByTeam information {0}", e.ToString());

            }
            return L2Details;
        }

        //Part III - Team Status
        [Produces("application/json")]
        [HttpGet("GetTeamCurrentStatusByTeam")]
        public RunningStyleDetails GetTeamCurrentStatusByTeam(string FactoryCode, int OperationCode, DateTime StartDate, DateTime EndDate, int? TeamId)
        {

            logger.InfoFormat("Get Hourly Production Status By Team with FactoryCode ={0}, OperationCod ={1}, StartDate ={2}, EndDate ={3}, TeamId ={4}", FactoryCode, OperationCode, StartDate, EndDate, TeamId);
            RunningStyleDetails L1Details = null;

            try
            {
                L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate && detxn.OperationCode == OperationCode && detxn.TeamId == TeamId).AsQueryable()
                             join team in dcap.Team.Where(team => team.FacCode == FactoryCode && team.TeamId == TeamId).AsQueryable() on detxn.TeamId equals team.TeamId
                             join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                             join l2 in dcap.L2.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                             join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                             join l5 in dcap.L5.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                             where detxn.RecStatus == 1 && detxn.TxnDateTime < EndDate && detxn.TxnDateTime > StartDate
                              && team.FacCode == FactoryCode && detxn.OperationCode == OperationCode && detxn.TeamId == TeamId
                             group detxn by new
                             {
                                 detxn.TxnDateTime,
                                 l1.L1desc, //Style
                                 l2.L2desc, //Shedule
                                 l4.L4desc, //Color
                                 l5.L5desc, //Size
                             }
                         into grp
                             orderby grp.Key.TxnDateTime descending
                             select new RunningStyleDetails
                             {
                                 Team_Id = TeamId,
                                 Txn_DateTime = grp.Key.TxnDateTime,
                                 Style = grp.Key.L1desc,
                                 Shedule = grp.Key.L2desc,
                                 Color = grp.Key.L4desc,
                                 Size = grp.Key.L5desc,
                             }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetHourlyProductionStatusByTeam information {0}", e.ToString());
                //throw e;
            }
            return L1Details;
        }

        //Part IV - All Teams Names fro Idle Teams
        [Produces("application/json")]
        [HttpGet("GetAllTeamsForFactory")]
        public IList<Team> GetAllTeamsForFactory(string FactoryCode, int OperationCode, DateTime StartDate, DateTime EndDate)
        {
            logger.InfoFormat("GetAllTeamsForFactory API called with FacCode={0}", FactoryCode);
            IList<Team> Teams = null;

            try
            {

                Teams = (from t in dcap.Team
                         where t.FacCode == FactoryCode && t.RecStatus == 1
                         select new Team
                         {
                             TeamId = t.TeamId,
                             Sbucode = t.Sbucode,
                             FacCode = t.FacCode,
                             LocCode = t.LocCode,
                             DeptCode = t.DeptCode,
                             TeamCode = t.TeamCode,
                             TeamName = t.TeamName
                         }).ToList();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetAllTeamsForFactory information {0}", e.ToString());
                //throw e;
            }
            return Teams;
        }

        //Part I - Workstation Hourly Production - Workstation Summary
        [Produces("application/json")]
        [HttpGet("GetHourlyTotalProductionStatusByTeam")]
        public IList<HourlyProductionStatus> GetHourlyTotalProductionStatusByTeam(string FactoryCode, int TeamCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("Get Hourly Production Status By Team with FactoryCode ={0}, OperationCod ={1}, StartDate ={2}, EndDate ={3},", FactoryCode, TeamCode, StartDate, EndDate);
            IList<HourlyProductionStatus> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate && detxn.RecStatus == 1 && detxn.TeamId == TeamCode).AsQueryable()
                                 join team in dcap.Team.Where(team => team.FacCode == FactoryCode && team.TeamId == TeamCode).AsQueryable() on detxn.TeamId equals team.TeamId
                                 join prodhour in dcap.Prodhour.Where(Prodhour => Prodhour.TeamId == TeamCode).AsQueryable() on new { A = (uint?)detxn.HourNo, B = (uint?)detxn.TeamId } equals new { A = (uint?)prodhour.HourNo, B = (uint?)prodhour.TeamId }
                                 join dep in dcap.Dep.AsQueryable() on new { A = detxn.OperationCode, B = detxn.Depid } equals new { A = dep.OperationCode, B = dep.Depid }
                                 where detxn.RecStatus == 1 && detxn.TxnDateTime < EndDate && detxn.TxnDateTime > StartDate
                                 && team.FacCode == FactoryCode && detxn.TeamId == TeamCode
                                 group detxn by new
                                 {
                                     dep.Depid,
                                     dep.Depdesc,
                                     team.TeamId,
                                     team.TeamName,
                                     prodhour.HourNo,
                                     prodhour.HourName,
                                 }
                         into grp
                                 orderby grp.Key.Depid, grp.Key.HourNo
                                 select new
                                 {
                                     Department_Key = (int?)grp.Key.Depid + (int?)grp.Key.TeamId,
                                     Department_Id = grp.Key.Depid,
                                     Department_Name = grp.Key.Depdesc,
                                     Team_Id = grp.Key.TeamId,
                                     Team_Name = grp.Key.TeamName,
                                     Team_Target = 1000,
                                     Hour_Name = grp.Key.HourName,
                                     Team_Total_Key = "Team Total",
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                                     Qty_Report = grp.Sum(detxn => detxn.Qty02),
                                     Qty_Scrap = grp.Sum(detxn => detxn.Qty03),
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_M",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_R",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3Details = L1Details.ToPivotTable(
                    item => item.Hour_Name + "_S",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                var P1TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_M",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_R",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Report) : 0);

                var P3TDetails = L1Details.ToPivotTable(
                    item => item.Team_Total_Key + "_S",
                    item => new { item.Department_Key, item.Department_Id, item.Department_Name, item.Team_Id, item.Team_Name, item.Team_Target },
                    items => items.Any() ? items.Sum(x => x.Qty_Scrap) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Department_Key"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Department_Key"] };
                P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["Department_Key"] };

                P1TDetails.PrimaryKey = new DataColumn[] { P1TDetails.Columns["Department_Key"] };
                P2TDetails.PrimaryKey = new DataColumn[] { P2TDetails.Columns["Department_Key"] };
                P3TDetails.PrimaryKey = new DataColumn[] { P3TDetails.Columns["Department_Key"] };

                P1Details.Merge(P2Details);
                P1Details.Merge(P3Details);

                P1Details.Merge(P1TDetails);
                P1Details.Merge(P2TDetails);
                P1Details.Merge(P3TDetails);

                DataColumn[] OperationColumns_M = P1Details.Columns.Cast<DataColumn>().Skip(6).ToArray();

                L2Details = P1Details.AsEnumerable()
                    .Select(r => new HourlyProductionStatus
                    {
                        Department_Id = Convert.ToInt16(r.Field<string>("Department_Id")),
                        Department_Name = r.Field<string>("Department_Name"),
                        Team_Id = Convert.ToInt16(r.Field<string>("Team_Id")),
                        Team_Name = r.Field<string>("Team_Name"),
                        Team_Target = Convert.ToDecimal(r.Field<string>("Team_Target")),
                        Qty = OperationColumns_M.Where(c => Convert.ToDecimal(r.Field<string>(c)) != 0).Select(c => new
                        {
                            Operation = c.ColumnName,
                            Qty = Convert.ToDecimal(r.Field<string>(c)),
                        }).ToDictionary(x => x.Operation, x => x.Qty),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetHourlyProductionStatusByTeam information {0}", e.ToString());

            }
            return L2Details;
        }

        // GET api/Dashboard/GetSecuser - get Secuser Information - login
        [Produces("application/json")]
        [HttpGet("GetSecuser")]
        public Secuser GetSecuser(string UserIDNumorUserID, string Password)
        {
            logger.InfoFormat("GetSecuser API called with userId = {0}, userIdNum = {1}, password={2}", UserIDNumorUserID, "********");

            Secuser secUser = null;
            try
            {
                secUser = dcap.Secuser
                            .Where(s => (s.UserIdN == UserIDNumorUserID || s.UserId == UserIDNumorUserID) && s.Password == Password)
                            .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Secuser information {0}", e.ToString());
            }
            return secUser;
        }

        /* extra section wip breakdown*/
        //GET api /get Production Status by Factory - Report 3 - WIP Report
        [Produces("application/json")]
        [HttpGet("DeatailReportByFactory")]
        public IList<WIPReport> DeatailReportByFactory(string FactoryCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("WIPReport By FactoryCode with FactoryCode={0}, StartDate={1}, EndDate={2}", FactoryCode, StartDate, EndDate);
            IList<WIPReport> L2Details = null;

            try
            {
                L2Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime > StartDate && detxn.TxnDateTime < EndDate).AsQueryable()
                             join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                             join l2 in dcap.L2.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                             join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                             join l5 in dcap.L5.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                             join l5moops in dcap.L5moops.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id, F = (int?)detxn.L5moid } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id, F = (int?)l5moops.L5moid }
                             join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable() on detxn.TeamId equals team.TeamId
                             group detxn by new
                             {
                                 l1.L1id,
                                 l1.L1desc,  //Style
                                 l2.L2id,
                                 l2.L2desc,  //Shedule
                                 l2.Ref01,  //PO
                                 l4.L4id,
                                 l4.L4desc,  //Color
                                 //l5moops.OrderQty, //Order Qty
                             }
                         into grp
                             orderby grp.Key.L1desc, grp.Key.L2desc, grp.Key.Ref01, grp.Key.L4desc//, grp.Key.L5desc///, grp.Key.OperationCode//, grp.Key.OrderQty
                             select new WIPReport
                             {
                                 L1D = grp.Key.L1id,
                                 Style = grp.Key.L1desc,
                                 L2D = grp.Key.L2id,
                                 Shedule = grp.Key.L2desc,
                                 PO = grp.Key.Ref01,
                                 L4D = grp.Key.L4id,
                                 Color = grp.Key.L4desc,
                                 //Size = grp.Key.L5desc,
                                 //Order_Qty = grp.Sum(l5moops => l5moops.OrderQty)//grp.Key.OrderQty,
                                 //Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01), //wrong//grp.Key.Qty01,//
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }
            return L2Details;
        }

        //GET api /get Production Status by Factory - Report 3 - WIP Report part II Calculate WIP
        [Produces("application/json")]
        [HttpGet("CalculateWIPReportByFactory")]
        public IList<WIPReport> CalculateWIPReportByFactory(string FactoryCode, DateTime StartDate, DateTime EndDate, uint L1D, uint L2D, uint L4D)
        {

            logger.InfoFormat("WIPReport By FactoryCode with FactoryCode={0}, StartDate={1}, EndDate={2}, L1D={0}, L2D={1}, L4D={2}", FactoryCode, StartDate, EndDate, L1D, L2D, L4D);
            IList<WIPReport> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == L1D && detxn.L2id == L2D && detxn.L4id == L4D).AsQueryable()
                                 join l5 in dcap.L5.Where(l5 => l5.L1id == L1D && l5.L2id == L2D && l5.L4id == L4D).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                 //join l5moops in dcap.L5moops.Where(l5moops => l5moops.L1id == L1D && l5moops.L2id == L2D && l5moops.L4id == L4D).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id, F = (int?)detxn.L5moid } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id, F = (int?)l5moops.L5moid }
                                 join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable() on (uint?)detxn.TeamId equals (uint?)team.TeamId
                                 group detxn by new
                                 {
                                     l5.L5desc,  //Size
                                     //l5moops.OrderQty, //Order Qty
                                     detxn.OperationCode, //operation code
                                     //l5moops.ReportedQty, //Reported Qty
                                     //detxn.Qty01, //Reported Qty
                                 }
                         into grp
                                 orderby grp.Key.OperationCode//, grp.Key.OrderQty
                                 select new
                                 {
                                     Size = grp.Key.L5desc,
                                     Operation = grp.Key.OperationCode,
                                     Order_Qty = 0,//grp.Key.OrderQty,
                                     //Reported_Qty = grp.Key.ReportedQty,
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01), //wrong//grp.Key.Qty01,//
                                 }).ToList();
                //L1Details.Sort();
                var PDetails = L1Details.ToPivotTable(
                  item => item.Operation,
                  item => new { item.Size, item.Order_Qty }, //item.Reported_Qty },
                  items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                //DataColumn[] OperationColumns = PDetails.Columns.Cast<DataColumn>().Skip(7).ToArray();
                DataColumnCollection col = PDetails.Columns;
                L2Details = PDetails.AsEnumerable()
                    .Select(r => new WIPReport
                    {
                        Size = r.Field<string>("Size"),
                        Order_Qty = Convert.ToDecimal(r.Field<string>("Order_Qty")),
                        //Reported_Qty = Convert.ToDecimal(r.Field<string>("Reported_Qty")),
                        //M

                        M_Laying = col.Contains("10") ? Convert.ToDecimal(r.Field<string>("10")) : 0,
                        M_Cutting = col.Contains("15") ? Convert.ToDecimal(r.Field<string>("15")) : 0,
                        M_EM_1_SQ_PA_Send_PF = col.Contains("41") ? Convert.ToDecimal(r.Field<string>("41")) : 0,
                        M_EM_1_SQ_PA_Receive_PF = col.Contains("42") ? Convert.ToDecimal(r.Field<string>("42")) : 0,
                        M_Sewing_In = col.Contains("129") ? Convert.ToDecimal(r.Field<string>("129")) : 0,
                        M_Sew_Out = WIPOutputByOperationValue(col, r, "130", "131", "NA", "NA", "+", "+", "+", "+"),
                        M_Washing_Send = WIPOutputByOperationValue(col, r, "151", "155", "NA", "NA", "+", "+", "+", "+"),
                        M_BFL_Wash_In = col.Contains("156") ? Convert.ToDecimal(r.Field<string>("156")) : 0,
                        M_BFL_Wash_Out = col.Contains("157") ? Convert.ToDecimal(r.Field<string>("157")) : 0,
                        M_Washing_Receive = WIPOutputByOperationValue(col, r, "160", "170", "NA", "NA", "+", "+", "+", "+"),
                        M_Finishing_In = col.Contains("180") ? Convert.ToDecimal(r.Field<string>("180")) : 0,
                        M_Poly_Bag_Packing = col.Contains("190") ? Convert.ToDecimal(r.Field<string>("190")) : 0,
                        M_Carton_Packing = col.Contains("200") ? Convert.ToDecimal(r.Field<string>("200")) : 0,

                        //WIP
                        WIP_Laying = WIPOutputByOperationValue(col, r, "Order_Qty", "10", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Cutting = WIPOutputByOperationValue(col, r, "10", "15", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Send_PF = WIPOutputByOperationValue(col, r, "15", "41", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Receive_PF = WIPOutputByOperationValue(col, r, "41", "42", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sewing_In = WIPOutputByOperationValue(col, r, "42", "129", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sew_Out = WIPOutputByOperationValue(col, r, "129", "130", "131", "NA", "+", "-", "-", "+"),
                        WIP_Washing_Send = WIPOutputByOperationValue(col, r, "130", "131", "151", "155", "+", "+", "-", "-"),
                        WIP_BFL_Wash_In = WIPOutputByOperationValue(col, r, "151", "155", "156", "NA", "+", "+", "-", "+"),
                        WIP_BFL_Wash_Out = WIPOutputByOperationValue(col, r, "156", "157", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Washing_Receive = WIPOutputByOperationValue(col, r, "157", "160", "170", "NA", "+", "-", "-", "+"),
                        WIP_Finishing_In = WIPOutputByOperationValue(col, r, "160", "170", "180", "NA", "+", "+", "-", "+"),
                        WIP_Poly_Bag_Packing = WIPOutputByOperationValue(col, r, "180", "190", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Carton_Packing = WIPOutputByOperationValue(col, r, "190", "200", "NA", "NA", "+", "-", "+", "+"),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L2Details;
        }


        //GET api /get Daily Production Report By Factory
        [Produces("application/json")]
        [HttpGet("DailyProductionReportByFactory")]
        public IList<Test> DailyProductionReportByFactory(string FactoryCode, DateTime TodaysDateStart, DateTime TodaysDateEnd)
        {

            logger.InfoFormat("Daily Production Report By Factory with FactoryCode={0}, TodaysDate={1}", FactoryCode, TodaysDateStart, TodaysDateEnd);
            IList<Test> L3Details = null;

            try
            {
                L3Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime >= TodaysDateStart && detxn.TxnDateTime <= TodaysDateEnd).AsQueryable()
                             join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                             join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                             join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable().DefaultIfEmpty() on detxn.TeamId equals team.TeamId
                             group detxn by new
                             {
                                 detxn.TeamId,
                                 team.TeamName,
                                 detxn.L1id,
                                 l1.L1desc,
                                 detxn.L4id,
                                 l4.L4desc,
                                 detxn.OperationCode,
                             }
                           into grp
                             orderby grp.Key.TeamId, grp.Key.L1id, grp.Key.L4id, grp.Key.OperationCode
                             select new Test
                             {
                                 Team_Id = grp.Key.TeamId,
                                 Team_Name = grp.Key.TeamName,
                                 L1D = grp.Key.L1id,
                                 Style = grp.Key.L1desc,
                                 L4D = grp.Key.L4id,
                                 Operation_Code = grp.Key.OperationCode,
                                 Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Daily Production Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L3Details;
        }

        //GET api /get Daily Production Report By Factory
        [Produces("application/json")]
        [HttpGet("ManufacturingQtyByTeamStyleColorOperation")]
        public decimal ManufacturingQtyByTeamStyleColorOperation(string FactoryCode, uint TeamId, uint L1id, uint? L4id, int? OperationCode)
        {

            logger.InfoFormat("Daily Production Report By Factory with FactoryCode={0}, TodaysDate={1}", FactoryCode, TeamId, L1id, L4id, OperationCode);
            decimal L3Details = 0;

            try
            {
                //Till Date
                var total = (from detxn in dcap.Detxn.Where(detxn => detxn.TeamId == TeamId && detxn.L1id == L1id && detxn.L4id == L4id && detxn.OperationCode == OperationCode).AsQueryable()
                             join team in dcap.Team.Where(team => team.FacCode == FactoryCode).AsQueryable().DefaultIfEmpty() on detxn.TeamId equals team.TeamId
                             select new
                             {
                                 detxn.Qty01,
                             }).ToList();

                L3Details = Convert.ToDecimal(total.Sum(d => d.Qty01));
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Daily Production Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L3Details;
        }


        //GET api /get Production Status by Factory - Report 3 - WIP Report ----Promis----- Report Layout
        [Produces("application/json")]
        [HttpGet("WIPReport")]
        public IList<WIPReport> WIPReport(string FactoryCode, DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("WIPReport By FactoryCode with FactoryCode={0}, StartDate={1}, EndDate={2}", FactoryCode, StartDate, EndDate);
            IList<WIPReport> L2Details = null;

            try
            {
                var L1Details = (from l5moops in dcap.L5moops.Where(l5moops => l5moops.FacCode == FactoryCode).AsQueryable()
                                 join detxn in dcap.Dedep.AsQueryable() on new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id, F = (int?)l5moops.L5moid, G = (int?)l5moops.OperationCode } equals new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id, F = (int?)detxn.L5moid, G = (int?)detxn.OperationCode }
                                 join l1 in dcap.L1.AsQueryable() on (uint?)l5moops.L1id equals (uint?)l1.L1id
                                 join l2 in dcap.L2.AsQueryable() on new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                 join l4 in dcap.L4.AsQueryable() on new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                 join l5 in dcap.L5.AsQueryable() on new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                 //where l5moops.OrderQty > detxn.Qty01
                                 //orderby detxn.OperationCode
                                 select new
                                 {
                                     Style = l1.L1desc,
                                     PO = l2.Ref01,
                                     Shedule = l2.L2desc,
                                     Color = l4.L4desc,
                                     Size = l5.L5desc,
                                     //L1D = detxn.L1id,
                                     //L2D = detxn.L2id,
                                     //L4D = detxn.L4id,
                                     //L5D = detxn.L5id,
                                     Operation = detxn.OperationCode,
                                     Order_Qty = l5moops.OrderQty,
                                     Manufacturing_Qty = detxn.Qty01,
                                     //WIP = l5moops.OrderQty - detxn.Qty01,
                                 }).ToList();

                var PDetails = L1Details.ToPivotTable(
                  item => item.Operation,
                  item => new { item.Style, item.PO, item.Shedule, item.Color, item.Size, item.Order_Qty }, //{ item.L1D, item.L2D, item.L4D, item.L5D, item.Order_Qty },
                  items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                DataColumnCollection col = PDetails.Columns;
                L2Details = PDetails.AsEnumerable()
                    .Select(r => new WIPReport
                    {
                        Style = r.Field<string>("Style"),
                        Shedule = r.Field<string>("Shedule"),
                        PO = r.Field<string>("PO"),
                        Color = r.Field<string>("Color"),
                        Size = r.Field<string>("Size"),
                        Order_Qty = Convert.ToDecimal(r.Field<string>("Order_Qty")),
                        //M

                        M_Laying = col.Contains("10") ? Convert.ToDecimal(r.Field<string>("10")) : 0,
                        M_Cutting = col.Contains("15") ? Convert.ToDecimal(r.Field<string>("15")) : 0,
                        M_EM_1_SQ_PA_Send_PF = col.Contains("41") ? Convert.ToDecimal(r.Field<string>("41")) : 0,
                        M_EM_1_SQ_PA_Receive_PF = col.Contains("42") ? Convert.ToDecimal(r.Field<string>("42")) : 0,
                        M_Sewing_In = col.Contains("129") ? Convert.ToDecimal(r.Field<string>("129")) : 0,
                        M_Sew_Out = WIPOutputByOperationValue(col, r, "130", "131", "NA", "NA", "+", "+", "+", "+"),
                        M_Washing_Send = WIPOutputByOperationValue(col, r, "151", "155", "NA", "NA", "+", "+", "+", "+"),
                        M_BFL_Wash_In = col.Contains("156") ? Convert.ToDecimal(r.Field<string>("156")) : 0,
                        M_BFL_Wash_Out = col.Contains("157") ? Convert.ToDecimal(r.Field<string>("157")) : 0,
                        M_Washing_Receive = WIPOutputByOperationValue(col, r, "160", "170", "NA", "NA", "+", "+", "+", "+"),
                        M_Finishing_In = col.Contains("180") ? Convert.ToDecimal(r.Field<string>("180")) : 0,
                        M_Poly_Bag_Packing = col.Contains("190") ? Convert.ToDecimal(r.Field<string>("190")) : 0,
                        M_Carton_Packing = col.Contains("200") ? Convert.ToDecimal(r.Field<string>("200")) : 0,

                        //WIP
                        WIP_Laying = WIPOutputByOperationValue(col, r, "Order_Qty", "10", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Cutting = WIPOutputByOperationValue(col, r, "10", "15", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Send_PF = WIPOutputByOperationValue(col, r, "15", "41", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Receive_PF = WIPOutputByOperationValue(col, r, "41", "42", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sewing_In = WIPOutputByOperationValue(col, r, "42", "129", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sew_Out = WIPOutputByOperationValue(col, r, "129", "130", "131", "NA", "+", "-", "-", "+"),
                        WIP_Washing_Send = WIPOutputByOperationValue(col, r, "130", "131", "151", "155", "+", "+", "-", "-"),
                        WIP_BFL_Wash_In = WIPOutputByOperationValue(col, r, "151", "155", "156", "NA", "+", "+", "-", "+"),
                        WIP_BFL_Wash_Out = WIPOutputByOperationValue(col, r, "156", "157", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Washing_Receive = WIPOutputByOperationValue(col, r, "157", "160", "170", "NA", "+", "-", "-", "+"),
                        WIP_Finishing_In = WIPOutputByOperationValue(col, r, "160", "170", "180", "NA", "+", "+", "-", "+"),
                        WIP_Poly_Bag_Packing = WIPOutputByOperationValue(col, r, "180", "190", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Carton_Packing = WIPOutputByOperationValue(col, r, "190", "200", "NA", "NA", "+", "-", "+", "+"),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }

            return L2Details;
        }

        //GET api /get Production Status by Factory - Report 3 - DAY WIP Report 
        [Produces("application/json")]
        [HttpGet("DayWIPReportByFactory")]
        public IList<DayWIPReport> DayWIPReportByFactory(DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("DayWIPReport By FactoryCode with StartDate={0}, EndDate={1}", StartDate, EndDate);
            IList<DayWIPReport> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.TxnDateTime >= StartDate && detxn.TxnDateTime <= EndDate).AsQueryable()
                                 join team in dcap.Team.AsQueryable() on detxn.TeamId equals team.TeamId
                                 join l1 in dcap.L1.AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                 join l2 in dcap.L2.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                 join l4 in dcap.L4.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                 join l5 in dcap.L5.AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                 group detxn by new
                                 {
                                     team.FacCode, //Factory
                                     l1.L1desc,  //Style
                                     l2.L2desc,  //Shedule
                                     l2.Ref01,  //PO
                                     l4.L4desc,  //Color
                                     l5.L5desc, //Size
                                     detxn.OperationCode, //Operation
                                 } into grp
                                 select new
                                 {
                                     Primary_Key = grp.Key.FacCode + grp.Key.L1desc + grp.Key.Ref01 + grp.Key.L2desc + grp.Key.L4desc + grp.Key.L5desc,
                                     Factory_Name = grp.Key.FacCode,
                                     Style = grp.Key.L1desc,
                                     PO = grp.Key.Ref01,
                                     Shedule = grp.Key.L2desc,
                                     Color = grp.Key.L4desc,
                                     Size = grp.Key.L5desc,
                                     Operation = grp.Key.OperationCode,
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                                     Average_Days = grp.Count(detxn => detxn.Qty01 > 0) == 0 ? 0 : grp.Sum(detxn => (DateTime.Today - detxn.TxnDateTime).Days) / grp.Count(detxn => detxn.Qty01 > 0),
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                  item => item.Operation + "M",
                  item => new { item.Primary_Key, item.Factory_Name, item.Style, item.PO, item.Shedule, item.Color, item.Size },
                  items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                var P2Details = L1Details.ToPivotTable(
                  item => item.Operation + "T",
                  item => new { item.Primary_Key, item.Factory_Name, item.Style, item.PO, item.Shedule, item.Color, item.Size },
                  items => items.Any() ? items.Sum(x => x.Average_Days) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Primary_Key"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Primary_Key"] };

                P1Details.Merge(P2Details);

                DataColumnCollection col = P1Details.Columns;
                L2Details = P1Details.AsEnumerable()
                    .Select(r => new DayWIPReport
                    {
                        Factory_Name = r.Field<string>("Factory_Name"),
                        Style = r.Field<string>("Style"),
                        Shedule = r.Field<string>("Shedule"),
                        PO = r.Field<string>("PO"),
                        Color = r.Field<string>("Color"),
                        Size = r.Field<string>("Size"),

                        //WIP
                        WIP_Cutting = WIPOutputByOperationValue(col, r, "10M", "15M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Send_PF = WIPOutputByOperationValue(col, r, "15M", "41M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Receive_PF = WIPOutputByOperationValue(col, r, "41M", "42M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sewing_In = WIPOutputByOperationValue(col, r, "42M", "129M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sew_Out = WIPOutputByOperationValue(col, r, "129M", "130M", "131M", "NA", "+", "-", "-", "+"),
                        WIP_Washing_Send = WIPOutputByOperationValue(col, r, "130M", "131M", "151M", "155M", "+", "+", "-", "-"),
                        WIP_BFL_Wash_In = WIPOutputByOperationValue(col, r, "151M", "155M", "156M", "NA", "+", "+", "-", "+"),
                        WIP_BFL_Wash_Out = WIPOutputByOperationValue(col, r, "156M", "157M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Washing_Receive = WIPOutputByOperationValue(col, r, "157M", "160M", "170M", "NA", "+", "-", "-", "+"),
                        WIP_Finishing_In = WIPOutputByOperationValue(col, r, "160M", "170M", "180M", "NA", "+", "+", "-", "+"),
                        WIP_Poly_Bag_Packing = WIPOutputByOperationValue(col, r, "180M", "190M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Carton_Packing = WIPOutputByOperationValue(col, r, "190M", "200M", "NA", "NA", "+", "-", "+", "+"),

                        //Days
                        M_Laying = col.Contains("10T") ? Math.Round(Convert.ToDecimal(r.Field<string>("10T"))) : 0,
                        M_Cutting = col.Contains("15T") ? Math.Round(Convert.ToDecimal(r.Field<string>("15T"))) : 0,
                        M_EM_1_SQ_PA_Send_PF = col.Contains("41T") ? Math.Round(Convert.ToDecimal(r.Field<string>("41T"))) : 0,
                        M_EM_1_SQ_PA_Receive_PF = col.Contains("42T") ? Math.Round(Convert.ToDecimal(r.Field<string>("42T"))) : 0,
                        M_Sewing_In = col.Contains("129T") ? Convert.ToDecimal(r.Field<string>("129T")) : 0,
                        M_Sew_Out = col.Contains("130T") && col.Contains("131T") ? Math.Round(WIPOutputByOperationValue(col, r, "130T", "131T", "NA", "NA", "+", "+", "+", "+") / 2) : Math.Round(WIPOutputByOperationValue(col, r, "130T", "131T", "NA", "NA", "+", "+", "+", "+")),
                        M_Washing_Send = col.Contains("130T") && col.Contains("131T") ? Math.Round(WIPOutputByOperationValue(col, r, "151T", "155T", "NA", "NA", "+", "+", "+", "+") / 2) : Math.Round(WIPOutputByOperationValue(col, r, "151T", "155T", "NA", "NA", "+", "+", "+", "+")),
                        M_BFL_Wash_In = col.Contains("156T") ? Math.Round(Convert.ToDecimal(r.Field<string>("156T"))) : 0,
                        M_BFL_Wash_Out = col.Contains("157T") ? Math.Round(Convert.ToDecimal(r.Field<string>("157T"))) : 0,
                        M_Washing_Receive = col.Contains("130T") && col.Contains("131T") ? Math.Round(WIPOutputByOperationValue(col, r, "160T", "170T", "NA", "NA", "+", "+", "+", "+") / 2) : Math.Round(WIPOutputByOperationValue(col, r, "160T", "170T", "NA", "NA", "+", "+", "+", "+")),
                        M_Finishing_In = col.Contains("180T") ? Math.Round(Convert.ToDecimal(r.Field<string>("180T"))) : 0,
                        M_Poly_Bag_Packing = col.Contains("190T") ? Math.Round(Convert.ToDecimal(r.Field<string>("190T"))) : 0,
                        M_Carton_Packing = col.Contains("200T") ? Math.Round(Convert.ToDecimal(r.Field<string>("200T"))) : 0,

                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Day WIP Report By Factory information {0}", e.ToString());
                //throw e;
            }
            return L2Details;
        }

        //GET api /get Production Status by Factory - Report 3 - DAY WIP Report 
        [Produces("application/json")]
        [HttpGet("DayTotalWIPReportByFactory")]
        public IList<DayWIPReport> DayTotalWIPReportByFactory(DateTime StartDate, DateTime EndDate)
        {

            logger.InfoFormat("DayTotalWIPReport By FactoryCode with StartDate={0}, EndDate={1}", StartDate, EndDate);
            IList<DayWIPReport> L2Details = null;

            try
            {
                var L1Details = (from detxn in dcap.Detxn
                                 join team in dcap.Team.AsQueryable() on detxn.TeamId equals team.TeamId
                                 where detxn.TxnDateTime >= StartDate && detxn.TxnDateTime <= EndDate
                                 group detxn by new
                                 {
                                     team.FacCode, //Factory
                                     detxn.OperationCode, //Operation
                                 } into grp
                                 select new
                                 {
                                     Factory_Name = grp.Key.FacCode,
                                     Operation = grp.Key.OperationCode,
                                     Manufacturing_Qty = grp.Sum(detxn => detxn.Qty01),
                                     Average_Days = grp.Count(detxn => detxn.Qty01 > 0) == 0 ? "0" : grp.Sum(detxn => (DateTime.Today - detxn.TxnDateTime).Days) / grp.Count(detxn => detxn.Qty01 > 0) <= 3 ? Convert.ToString(Math.Round(Convert.ToDecimal(grp.Sum(detxn => (DateTime.Today - detxn.TxnDateTime).Days) / grp.Count(detxn => detxn.Qty01 > 0)))) + " days" : "more than 3 days",
                                 }).ToList();

                var P1Details = L1Details.ToPivotTable(
                  item => item.Operation + "M",
                  item => new { item.Factory_Name, item.Average_Days },
                  items => items.Any() ? items.Sum(x => x.Manufacturing_Qty) : 0);

                DataColumnCollection col = P1Details.Columns;
                L2Details = P1Details.AsEnumerable()
                    .Select(r => new DayWIPReport
                    {
                        Factory_Name = r.Field<string>("Factory_Name"),
                        Style = r.Field<string>("Average_Days"),

                        //WIP
                        WIP_Cutting = WIPOutputByOperationValue(col, r, "10M", "15M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Send_PF = WIPOutputByOperationValue(col, r, "15M", "41M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_EM_1_SQ_PA_Receive_PF = WIPOutputByOperationValue(col, r, "41M", "42M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sewing_In = WIPOutputByOperationValue(col, r, "42M", "129M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Sew_Out = WIPOutputByOperationValue(col, r, "129M", "130M", "131M", "NA", "+", "-", "-", "+"),
                        WIP_Washing_Send = WIPOutputByOperationValue(col, r, "130M", "131M", "151M", "155M", "+", "+", "-", "-"),
                        WIP_BFL_Wash_In = WIPOutputByOperationValue(col, r, "151M", "155M", "156M", "NA", "+", "+", "-", "+"),
                        WIP_BFL_Wash_Out = WIPOutputByOperationValue(col, r, "156M", "157M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Washing_Receive = WIPOutputByOperationValue(col, r, "157M", "160M", "170M", "NA", "+", "-", "-", "+"),
                        WIP_Finishing_In = WIPOutputByOperationValue(col, r, "160M", "170M", "180M", "NA", "+", "+", "-", "+"),
                        WIP_Poly_Bag_Packing = WIPOutputByOperationValue(col, r, "180M", "190M", "NA", "NA", "+", "-", "+", "+"),
                        WIP_Carton_Packing = WIPOutputByOperationValue(col, r, "190M", "200M", "NA", "NA", "+", "-", "+", "+"),
                    }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Day Total WIP Report By Factory information {0}", e.ToString());
                throw e;
            }
            return L2Details;
        }

        //Production Environment Reports: START
        //Report Lookup Controllers
        [Produces("application/json")]
        [HttpGet("GetBFLStockReport")]
        public List<BFLStockSummary> GetBFLStockReport(int mode, int txnMode, DateTime fromDate, DateTime toDate, int WFid, int l1id, int l2id, int l4id, int opcode)
        {

            logger.InfoFormat("GetBFLStockReport mode={0} txnMode={1} fromDate={2} toDate={3} WFid={4} l1id={5} l2id={6} l4id={7}, opcode={8}", mode, txnMode, fromDate, toDate, WFid, l1id, l2id, l4id, opcode);
            List<BFLStockSummary> L2Details = null;
            TransactionController trans = new TransactionController(dcap);

            try
            {
                trans.DataCorrection();

                DateTime cu = DateTime.MinValue;
                if (toDate == cu)
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    TimeSpan tst = new TimeSpan(0, 23, 59, 59);
                    toDate = toDate.Add(tst);
                }

                if (fromDate == cu)
                {
                    TimeSpan ts = new TimeSpan(31, 0, 0, 0);
                    fromDate = toDate.Subtract(ts);
                }

                var groupbarcodepids = dcap.GroupBarcode.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.RecStatus == 1 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id }).ToList();
                uint minL1id = 0;
                uint maxL1id = 0;
                if (groupbarcodepids.Count != 0)
                {
                    minL1id = groupbarcodepids.Min(c => c.L1id);
                    maxL1id = groupbarcodepids.Max(c => c.L1id);
                }

                IList<Detxn> grpbb = null;
                IList<Detxn> grpbt = null;

                IList<Detxn> bflqlty = null;

                IList<GroupBarcode> bgg = null;

                IList<L1> gl1 = null;
                IList<L2> gl2 = null;
                IList<L4> gl4 = null;
                IList<L5> gl5 = null;

                IList<L5moops> gl5moops = null;

                CultureInfo culture = new CultureInfo("en-US");

                if (l1id == 0 && l2id == 0 && l4id == 0 && WFid == 0)
                {
                    bgg = dcap.GroupBarcode.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode })
                                    .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = 0, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, CreatedBy = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                    if (mode >= 20 && mode < 30)
                    {

                        var bgs = dcap.Group_Barcode_Detail.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BarCodeNo, r.TxnMode })
                                                            .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.BarCodeNo, TxnMode = (int)c.Key.TxnMode, Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                        bgg = (from bg in bgg
                               join bs in bgs on new { A = (uint?)bg.L1id, B = (uint?)bg.L2id, C = (uint?)bg.L4id, D = bg.BagBarCodeNo, E = bg.TxnMode } equals new { A = (uint?)bs.L1id, B = (uint?)bs.L2id, C = (uint?)bs.L4id, D = bs.BagBarCodeNo, E = bs.TxnMode }
                               group new { bg, bs } by new
                               {
                                   bg.L1id,
                                   bg.L2id,
                                   bg.L4id,
                                   bs.L5id,
                                   bs.BagBarCodeNo,
                                   bs.TxnMode,
                                   bg.CreatedBy, //Secondarykey
                                   bs.Qty01,
                                   bs.Qty02,
                                   bs.Qty03,
                               } into c
                               select new GroupBarcode
                               {
                                   L1id = c.Key.L1id,
                                   L2id = c.Key.L2id,
                                   L4id = c.Key.L4id,
                                   L5id = c.Key.L5id,
                                   BagBarCodeNo = c.Key.BagBarCodeNo,
                                   TxnMode = c.Key.TxnMode,
                                   CreatedBy = c.Key.CreatedBy,
                                   Qty01 = c.Key.Qty01,
                                   Qty02 = c.Key.Qty02,
                                   Qty03 = c.Key.Qty03,
                               }).ToList();
                    }

                    if (mode == 14)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.TxnMode != 1 && (c.L1id <= maxL1id && c.L1id >= minL1id) && c.RecStatus == 1)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }
                    else if (mode == 23)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.TxnMode != 1 && (c.L1id <= maxL1id && c.L1id >= minL1id) && c.RecStatus == 1)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }

                    if (mode == 6) //new otd report 
                    {
                        grpbb = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.BagBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = 2, L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        grpbt = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.TravelBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.TravelBarCodeNo, TxnMode = Convert.ToInt16(c.Key.TravelBarCodeNo.Substring(1, 1)), L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        gl5 = dcap.L5.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    }

                    gl1 = dcap.L1.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl2 = dcap.L2.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl4 = dcap.L4.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl5 = dcap.L5.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();

                    if (mode != 5 && mode != 22 && mode != 23) //new otd report 
                    {

                        if (mode == 12 || mode == 13)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else if(mode >= 20 && mode < 30)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                    }
                }
                else if (l1id == 0 && l2id == 0 && l4id == 0)
                {
                    bgg = dcap.GroupBarcode.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id) && (WFid == 0 ? true : c.WFId == WFid))
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode })
                                    .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = 0, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, CreatedBy = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                    if (mode >= 20 && mode < 30)
                    {

                        var bgs = dcap.Group_Barcode_Detail.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BarCodeNo, r.TxnMode })
                                                            .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.BarCodeNo, TxnMode = (int)c.Key.TxnMode, Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                        bgg = (from bg in bgg
                               join bs in bgs on new { A = (uint?)bg.L1id, B = (uint?)bg.L2id, C = (uint?)bg.L4id, D = bg.BagBarCodeNo, E = bg.TxnMode } equals new { A = (uint?)bs.L1id, B = (uint?)bs.L2id, C = (uint?)bs.L4id, D = bs.BagBarCodeNo, E = bs.TxnMode }
                               group new { bg, bs } by new
                               {
                                   bg.L1id,
                                   bg.L2id,
                                   bg.L4id,
                                   bs.L5id,
                                   bs.BagBarCodeNo,
                                   bs.TxnMode,
                                   bg.CreatedBy, //Secondarykey
                                   bs.Qty01,
                                   bs.Qty02,
                                   bs.Qty03,
                               } into c
                               select new GroupBarcode
                               {
                                   L1id = c.Key.L1id,
                                   L2id = c.Key.L2id,
                                   L4id = c.Key.L4id,
                                   L5id = c.Key.L5id,
                                   BagBarCodeNo = c.Key.BagBarCodeNo,
                                   TxnMode = c.Key.TxnMode,
                                   CreatedBy = c.Key.CreatedBy,
                                   Qty01 = c.Key.Qty01,
                                   Qty02 = c.Key.Qty02,
                                   Qty03 = c.Key.Qty03,
                               }).ToList();
                    }

                    if (mode == 14)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.JobNo, r.Rrid, r.TxnDateTime.Year, r.TxnDateTime.Month, r.TxnDateTime.Day })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, TxnDateTime = Convert.ToDateTime(c.Key.Day + "/" + c.Key.Month + "/" + c.Key.Month + " 00:00:00 AM", culture), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }
                    else if (mode == 23)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }

                    if (mode == 6) //new otd report 
                    {
                        grpbb = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.BagBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = 2, L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        grpbt = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.TravelBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.TravelBarCodeNo, TxnMode = Convert.ToInt16(c.Key.TravelBarCodeNo.Substring(1, 1)), L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        gl5 = dcap.L5.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    }
                    gl1 = dcap.L1.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl2 = dcap.L2.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl4 = dcap.L4.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();
                    gl5 = dcap.L5.Where(c => c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id)).ToList();

                    if (mode != 5 && mode != 22 && mode != 23) //new otd report 
                    {
                        if (mode == 12 || mode == 13)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        
                        else if(mode >= 20 && mode < 30)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && (c.L1id <= maxL1id && c.L1id >= minL1id))
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                    }
                }
                else if (l2id == 0 && l4id == 0)
                {
                    bgg = dcap.GroupBarcode.Where(c => c.RecStatus == 1 && c.L1id == l1id && (WFid == 0 ? true : c.WFId == WFid))
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode })
                                    .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = 0, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, CreatedBy = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                    if (mode >= 20 && mode < 30)
                    {

                        var bgs = dcap.Group_Barcode_Detail.Where(c => c.RecStatus == 1 && c.L1id == l1id)
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BarCodeNo, r.TxnMode })
                                                            .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.BarCodeNo, TxnMode = (int)c.Key.TxnMode, Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                        bgg = (from bg in bgg
                               join bs in bgs on new { A = (uint?)bg.L1id, B = (uint?)bg.L2id, C = (uint?)bg.L4id, D = bg.BagBarCodeNo, E = bg.TxnMode } equals new { A = (uint?)bs.L1id, B = (uint?)bs.L2id, C = (uint?)bs.L4id, D = bs.BagBarCodeNo, E = bs.TxnMode }
                               group new { bg, bs } by new
                               {
                                   bg.L1id,
                                   bg.L2id,
                                   bg.L4id,
                                   bs.L5id,
                                   bs.BagBarCodeNo,
                                   bs.TxnMode,
                                   bg.CreatedBy, //Secondarykey
                                   bs.Qty01,
                                   bs.Qty02,
                                   bs.Qty03,
                               } into c
                               select new GroupBarcode
                               {
                                   L1id = c.Key.L1id,
                                   L2id = c.Key.L2id,
                                   L4id = c.Key.L4id,
                                   L5id = c.Key.L5id,
                                   BagBarCodeNo = c.Key.BagBarCodeNo,
                                   TxnMode = c.Key.TxnMode,
                                   CreatedBy = c.Key.CreatedBy,
                                   Qty01 = c.Key.Qty01,
                                   Qty02 = c.Key.Qty02,
                                   Qty03 = c.Key.Qty03,
                               }).ToList();
                    }

                    if (mode == 14)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.JobNo, r.Rrid, r.TxnDateTime.Year, r.TxnDateTime.Month, r.TxnDateTime.Day })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, TxnDateTime = Convert.ToDateTime(c.Key.Day + "/" + c.Key.Month + "/" + c.Key.Month + " 00:00:00 AM", culture), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }
                    else if (mode == 23)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }

                    if (mode == 6) //new otd report 
                    {
                        grpbb = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.BagBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = 2, L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        grpbt = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.TravelBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.TravelBarCodeNo, TxnMode = Convert.ToInt16(c.Key.TravelBarCodeNo.Substring(1, 1)), L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id).ToList();
                    }
                    gl1 = dcap.L1.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();
                    gl2 = dcap.L2.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();
                    gl4 = dcap.L4.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();
                    gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();

                    if (mode != 5 && mode != 22 && mode != 23) //new otd report 
                    {
                        if (mode == 12 || mode == 13)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else if(mode >= 20 && mode < 30)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                    }
                }
                else if (l4id == 0)
                {
                    bgg = dcap.GroupBarcode.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && (WFid == 0 ? true : c.WFId == WFid))
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode })
                                    .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = 0, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, CreatedBy = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                    if (mode >= 20 && mode < 30)
                    {

                        var bgs = dcap.Group_Barcode_Detail.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id)
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BarCodeNo, r.TxnMode })
                                                            .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.BarCodeNo, TxnMode = (int)c.Key.TxnMode, Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                        bgg = (from bg in bgg
                               join bs in bgs on new { A = (uint?)bg.L1id, B = (uint?)bg.L2id, C = (uint?)bg.L4id, D = bg.BagBarCodeNo, E = bg.TxnMode } equals new { A = (uint?)bs.L1id, B = (uint?)bs.L2id, C = (uint?)bs.L4id, D = bs.BagBarCodeNo, E = bs.TxnMode }
                               group new { bg, bs } by new
                               {
                                   bg.L1id,
                                   bg.L2id,
                                   bg.L4id,
                                   bs.L5id,
                                   bs.BagBarCodeNo,
                                   bs.TxnMode,
                                   bg.CreatedBy, //Secondarykey
                                   bs.Qty01,
                                   bs.Qty02,
                                   bs.Qty03,
                               } into c
                               select new GroupBarcode
                               {
                                   L1id = c.Key.L1id,
                                   L2id = c.Key.L2id,
                                   L4id = c.Key.L4id,
                                   L5id = c.Key.L5id,
                                   BagBarCodeNo = c.Key.BagBarCodeNo,
                                   TxnMode = c.Key.TxnMode,
                                   CreatedBy = c.Key.CreatedBy,
                                   Qty01 = c.Key.Qty01,
                                   Qty02 = c.Key.Qty02,
                                   Qty03 = c.Key.Qty03,
                               }).ToList();
                    }

                    if (mode == 14)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.JobNo, r.Rrid, r.TxnDateTime.Year, r.TxnDateTime.Month, r.TxnDateTime.Day })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, TxnDateTime = Convert.ToDateTime(c.Key.Day + "/" + c.Key.Month + "/" + c.Key.Month + " 00:00:00 AM", culture), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }
                    else if (mode == 23)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }

                    if (mode == 6) //new otd report 
                    {
                        grpbb = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.BagBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = 2, L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        grpbt = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.TravelBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.TravelBarCodeNo, TxnMode = Convert.ToInt16(c.Key.TravelBarCodeNo.Substring(1, 1)), L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id).ToList();
                    }

                    gl1 = dcap.L1.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();
                    gl2 = dcap.L2.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id).ToList();
                    gl4 = dcap.L4.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id).ToList();
                    gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id).ToList();

                    if (mode != 5 && mode != 22 && mode != 23) //new otd report 
                    {
                        if (mode == 12 || mode == 13)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else if(mode >= 20 && mode < 30)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                    }
                }
                else
                {
                    bgg = dcap.GroupBarcode.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id && (WFid == 0 ? true : c.WFId == WFid))
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode })
                                    .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = 0, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, CreatedBy = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                    if (mode >= 20 && mode < 30)
                    {

                        var bgs = dcap.Group_Barcode_Detail.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BarCodeNo, r.TxnMode })
                                                            .Select(c => new GroupBarcode { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.BarCodeNo, TxnMode = (int)c.Key.TxnMode, Qty01 = (int)c.Sum(r => r.Qty01), Qty02 = (int)c.Sum(r => r.Qty02), Qty03 = (int)c.Sum(r => r.Qty03) }).ToList();

                        bgg = (from bg in bgg
                               join bs in bgs on new { A = (uint?)bg.L1id, B = (uint?)bg.L2id, C = (uint?)bg.L4id, D = bg.BagBarCodeNo, E = bg.TxnMode } equals new { A = (uint?)bs.L1id, B = (uint?)bs.L2id, C = (uint?)bs.L4id, D = bs.BagBarCodeNo, E = bs.TxnMode }
                               group new { bg, bs } by new
                               {
                                   bg.L1id,
                                   bg.L2id,
                                   bg.L4id,
                                   bs.L5id,
                                   bs.BagBarCodeNo,
                                   bs.TxnMode,
                                   bg.CreatedBy, //Secondarykey
                                   bs.Qty01,
                                   bs.Qty02,
                                   bs.Qty03,
                               } into c
                               select new GroupBarcode
                               {
                                   L1id = c.Key.L1id,
                                   L2id = c.Key.L2id,
                                   L4id = c.Key.L4id,
                                   L5id = c.Key.L5id,
                                   BagBarCodeNo = c.Key.BagBarCodeNo,
                                   TxnMode = c.Key.TxnMode,
                                   CreatedBy = c.Key.CreatedBy,
                                   Qty01 = c.Key.Qty01,
                                   Qty02 = c.Key.Qty02,
                                   Qty03 = c.Key.Qty03,
                               }).ToList();
                    }

                    if (mode == 14)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.JobNo, r.Rrid, r.TxnDateTime.Year, r.TxnDateTime.Month, r.TxnDateTime.Day })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, TxnDateTime = Convert.ToDateTime(c.Key.Day + "/" + c.Key.Month + "/" + c.Key.Month + " 00:00:00 AM", culture), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }
                    else if (mode == 23)
                    {
                        bflqlty = dcap.Detxn.Where(c => c.CreatedDateTime >= fromDate && c.CreatedDateTime <= toDate && c.OperationCode == opcode && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.JobNo, r.Rrid, r.TxnDateTime })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, ModifiedBy = c.Key.TxnDateTime.ToShortDateString(), JobNo = c.Key.JobNo, Rrid = c.Key.Rrid, Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();
                    }

                    if (mode == 6) //new otd report 
                    {
                        grpbb = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.BagBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = 2, L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        grpbt = dcap.Detxn.Where(c => c.OperationCode == 151 && c.RecStatus == 1 && (WFid == 0 ? true : c.Wfid == WFid) && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                                .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.L5mono, r.TravelBarCodeNo })
                                                .Select(c => new Detxn { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, L5moid = c.Key.L5moid, BagBarCodeNo = c.Key.TravelBarCodeNo, TxnMode = Convert.ToInt16(c.Key.TravelBarCodeNo.Substring(1, 1)), L5mono = c.Key.L5mono, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).ToList();

                        gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id).ToList();
                    }

                    gl1 = dcap.L1.Where(c => c.RecStatus == 1 && c.L1id == l1id).ToList();
                    gl2 = dcap.L2.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id).ToList();
                    gl4 = dcap.L4.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id).ToList();
                    gl5 = dcap.L5.Where(c => c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id).ToList();

                    if (mode != 5 && mode != 22 && mode != 23) //new otd report 
                    {
                        if (mode == 12 || mode == 13)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else if(mode >= 20 && mode < 30)
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                        else
                        {
                            gl5moops = dcap.L5moops.Where(c => c.OperationCode == 15 && c.RecStatus == 1 && c.L1id == l1id && c.L2id == l2id && c.L4id == l4id)
                                     .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                     .Select(c => new L5moops { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).ToList();
                        }
                    }
                }

                if (mode == 1) //old
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.TxnMode, r.TxnStatus, r.OperationCode })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable() //, Qty01NS = c.Sum(r => r.Qty01NS), Qty02NS = c.Sum(r => r.Qty02NS), Qty03NS = c.Sum(r => r.Qty03NS)
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     //join optransactionq in optransaction on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)optransactionq.L1id, B = (uint?)optransactionq.L2id, C = (uint?)optransactionq.L3id, D = (uint?)optransactionq.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.Ref01 + grp.Key.Ref02 + grp.Key.DeliveryDate + grp.Key.L4desc,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty01) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1", "3 | 2", "3 | 3", "3 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,
                        }).ToList();
                }
                else if (mode == 2) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.TxnMode == txnMode && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode, r.CreatedDateTime, r.CreatedBy, r.ModifiedBy, r.ModifiedDateTime })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, CreatedDateTime = c.Min(s => s.CreatedDateTime), CreatedBy = c.Key.CreatedBy, ModifiedDateTime = c.Min(s => s.ModifiedDateTime), ModifiedBy = c.Key.ModifiedBy, BagBarCodeNo = c.Key.BagBarCodeNo, TxnStatus = c.Key.TxnStatus, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01NS = c.Sum(r => r.Qty01NS) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         groupbarcode.BagBarCodeNo,
                                         groupbarcode.TxnStatus,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         groupbarcode.CreatedBy,
                                         groupbarcode.CreatedDateTime,
                                         groupbarcode.ModifiedBy,
                                         groupbarcode.ModifiedDateTime
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.BagBarCodeNo,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.groupbarcode.Qty01) - (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         Qty02 = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                         CreatedDateTime = grp.Key.CreatedDateTime,
                                         CreatedBY = grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime,
                                         ModifiedBy = grp.Key.ModifiedBy,
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    var P2Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key + "S",
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty02) : 0);

                    P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Primary_Key"] };
                    P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Primary_Key"] };

                    P1Details.Merge(P2Details);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 13) //txn mode for bag with size
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.TxnMode == txnMode && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id) && c.L5id > 0)
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.TxnMode, r.TxnStatus, r.OperationCode, r.CreatedDateTime, r.CreatedBy, r.ModifiedBy, r.ModifiedDateTime })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, CreatedDateTime = c.Min(s => s.CreatedDateTime), CreatedBy = c.Key.CreatedBy, ModifiedDateTime = c.Min(s => s.ModifiedDateTime), ModifiedBy = c.Key.ModifiedBy, TxnStatus = c.Key.TxnStatus, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01NS = c.Sum(r => r.Qty01NS) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1id,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2id,
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4id,
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5id,
                                         l5.L5desc,// Size
                                         //groupbarcode.TxnStatus,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         //groupbarcode.CreatedBy,
                                         //groupbarcode.CreatedDateTime,
                                         //groupbarcode.ModifiedBy,
                                         //groupbarcode.ModifiedDateTime
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L1id + grp.Key.L2desc + grp.Key.L2id + grp.Key.L4desc + grp.Key.L4id + grp.Key.L5desc + grp.Key.L5id,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.groupbarcode.Qty01) - (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         Qty02 = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                         /*CreatedDateTime = grp.Key.CreatedDateTime,
                                         CreatedBY = grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime,
                                         ModifiedBy = grp.Key.ModifiedBy,*/
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime , item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    var P2Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key + "S",
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty02) : 0);

                    P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Primary_Key"] };
                    P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Primary_Key"] };

                    P1Details.Merge(P2Details);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            /*CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),*/

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5") ? Convert.ToDecimal(r.Field<string>("1 | 5")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1", "1 | 2", "1 | 3", "1 | 4", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 12) //txn mode for bag with size
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.TxnMode == txnMode && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id) && c.L5id > 0)
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode, r.CreatedDateTime, r.CreatedBy, r.ModifiedBy, r.ModifiedDateTime })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, CreatedDateTime = c.Min(s => s.CreatedDateTime), CreatedBy = c.Key.CreatedBy, ModifiedDateTime = c.Min(s => s.ModifiedDateTime), ModifiedBy = c.Key.ModifiedBy, BagBarCodeNo = c.Key.BagBarCodeNo, TxnStatus = c.Key.TxnStatus, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01NS = c.Sum(r => r.Qty01NS) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5id,
                                         l5.L5desc,// Size
                                         groupbarcode.BagBarCodeNo,
                                         groupbarcode.TxnStatus,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         groupbarcode.CreatedBy,
                                         groupbarcode.CreatedDateTime,
                                         groupbarcode.ModifiedBy,
                                         groupbarcode.ModifiedDateTime
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.L5desc + grp.Key.BagBarCodeNo,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.groupbarcode.Qty01) - (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         Qty02 = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                         CreatedDateTime = grp.Key.CreatedDateTime,
                                         CreatedBY = grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime,
                                         ModifiedBy = grp.Key.ModifiedBy,
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    var P2Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key + "S",
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty02) : 0);

                    P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Primary_Key"] };
                    P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Primary_Key"] };

                    P1Details.Merge(P2Details);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 3) //new otd report 
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.TxnMode, r.TxnStatus, r.OperationCode })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable() //, Qty01NS = c.Sum(r => r.Qty01NS), Qty02NS = c.Sum(r => r.Qty02NS), Qty03NS = c.Sum(r => r.Qty03NS)
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, D = (uint?)groupbarcodeids.L4id }
                                     //join optransactionq in optransaction on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)optransactionq.L1id, B = (uint?)optransactionq.L2id, C = (uint?)optransactionq.L3id, D = (uint?)optransactionq.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, D = (uint?)l4.L4id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, D = (uint?)l5moops.L4id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.Ref01 + grp.Key.Ref02 + grp.Key.DeliveryDate + grp.Key.L4desc,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.WashType, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty01) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1", "3 | 2", "3 | 3", "3 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            Bag_0_160 = col.Contains("1 | 0 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 160")) : 0,
                            Bag_1_160 = col.Contains("1 | 1 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 160")) : 0,
                            Bag_2_160 = col.Contains("1 | 2 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 160")) : 0,
                            Bag_3_160 = col.Contains("1 | 3 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 160")) : 0,
                            Bag_4_160 = col.Contains("1 | 4 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 160")) : 0,
                            Bag_5_160 = col.Contains("1 | 5 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 160")) : 0,
                            Bag_6_160 = col.Contains("1 | 6 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 160")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,
                        }).ToList();
                }
                else if (mode == 4) //txn mode for bag and travel tag summary
                {
                    var opeartion_summary = dcap.TTOpearation.Where(p => p.TxnDateTime >= fromDate)
                                                       .GroupBy(r => new { r.OperationCode, r.BarCodeNo, r.TxnMode, r.WFId, r.RecStatus })
                                                       .Select(c => new { WFId = c.Key.WFId, OperationCode = c.Key.OperationCode, BarCodeNo = c.Key.BarCodeNo, TxnMode = c.Key.TxnMode, RecStatus = c.Key.RecStatus, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01Ns = c.Sum(r => r.Qty01NS), Qty02Ns = c.Sum(r => r.Qty02NS), Qty03Ns = c.Sum(r => r.Qty03NS) }).ToList();

                    var traveltagheadeer = dcap.TravelBarcodeDetails.Where(p => p.CreatedDateTime >= fromDate).ToList();

                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.TxnMode == txnMode && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.Seq, r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.CreatedBy, r.ModifiedBy })
                                            .Select(c => new { Seq = c.Key.Seq, L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, CreatedDateTime = c.Min(s => s.CreatedDateTime), CreatedBy = c.Key.CreatedBy, ModifiedDateTime = c.Min(s => s.ModifiedDateTime), ModifiedBy = c.Key.ModifiedBy, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join opeartionsummary in opeartion_summary.Where(c => c.TxnMode == txnMode && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid))
                                            .GroupBy(r => new { r.BarCodeNo, r.TxnMode, r.OperationCode })
                                            .Select(c => new { BarCodeNo = c.Key.BarCodeNo, TxnMode = c.Key.TxnMode, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01Ns = c.Sum(r => r.Qty01Ns), Qty02Ns = c.Sum(r => r.Qty02Ns), Qty03Ns = c.Sum(r => r.Qty03Ns) }).AsQueryable()
                                        on new { B = groupbarcode.BagBarCodeNo, C = groupbarcode.TxnMode } equals new { B = opeartionsummary.BarCodeNo, C = opeartionsummary.TxnMode }
                                     join traveltagheader in traveltagheadeer on new { A = groupbarcode.BagBarCodeNo, B = groupbarcode.TxnMode } equals new { A = traveltagheader.TravelBarCodeNo, B = traveltagheader.TxnMode }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     group new { groupbarcode, traveltagheader, l1, l2, l4, opeartionsummary } by new //l5moops
                                     {
                                         //l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         //l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         //l2.L2desc,  //Shedule
                                         //l2.Ref01,  //PO
                                         //l2.Ref02,  //ZFeature
                                         //l2.DeliveryDate, //Delivery Date
                                         //l4.L4desc,  //Color
                                         traveltagheader.Weight,
                                         traveltagheader.EPF,
                                         traveltagheader.AllocationDate,
                                         traveltagheader.JobQty,
                                         groupbarcode.BagBarCodeNo,
                                         opeartionsummary.Secondary_Key,
                                         //groupbarcode.CreatedBy,
                                         //groupbarcode.CreatedDateTime,
                                         //groupbarcode.ModifiedBy,
                                         //groupbarcode.ModifiedDateTime
                                     } into grp
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + string.Join(" | ", grp.Select(d => d.l1.L1desc)) + string.Join("|", grp.Select(d => d.l2.L2desc)) + string.Join(" | ", grp.Select(d => d.l4.L4desc)), // + grp.Key.L2desc
                                         BuyerDivision = grp.Key.Name,
                                         Style = string.Join(" | ", grp.Select(d => d.l1.L1desc)),
                                         ProductionWarehouse = string.Join(" | ", grp.Select(d => d.l2.Wfid)),
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = string.Join(" | ", grp.Select(d => d.l2.L2desc)), //grp.Key.L2desc,
                                         PO = string.Join(" | ", grp.Select(d => d.l2.Ref01)), //grp.Key.Ref01,
                                         ZFeature = string.Join(" | ", grp.Select(d => d.l2.Ref02)), //grp.Key.Ref02,
                                         Color = string.Join(" | ", grp.Select(d => d.l4.L4desc)), //grp.Key.L4desc,
                                         Weight = grp.Key.Weight,
                                         EPF = grp.Key.EPF,
                                         AllocationDate = grp.Key.AllocationDate,
                                         JobQty = grp.Key.JobQty,
                                         Delivery_Date = grp.Min(d => d.l2.DeliveryDate),
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Max(g => g.opeartionsummary.Qty01) - grp.Max(g => g.opeartionsummary.Qty02) - grp.Max(g => g.opeartionsummary.Qty03),
                                         OrderQty = 0, //grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = 0, //grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                         CreatedDateTime = grp.Max(g => g.groupbarcode.CreatedDateTime), //grp.Key.CreatedDateTime,
                                         CreatedBY = "", //grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Max(g => g.groupbarcode.ModifiedDateTime), //grp.Key.ModifiedDateTime,
                                         ModifiedBy = "", //grp.Key.ModifiedBy,
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Weight, item.EPF, item.AllocationDate, item.JobQty, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Weight = r.Field<string>("Weight"),
                            EPF = r.Field<string>("EPF"),
                            AllocationDate = Convert.ToDateTime(r.Field<string>("AllocationDate")),
                            JobQty = Convert.ToInt32(r.Field<string>("JobQty")),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                        }).ToList();

                }
                else if (mode == 5) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.TxnMode == 3 && c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.BagBarCodeNo, r.TxnMode, r.TxnStatus, r.OperationCode, r.CreatedDateTime, r.CreatedBy, r.ModifiedBy, r.ModifiedDateTime })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, CreatedDateTime = c.Min(s => s.CreatedDateTime), CreatedBy = c.Key.CreatedBy, ModifiedDateTime = c.Min(s => s.ModifiedDateTime), ModifiedBy = c.Key.ModifiedBy, BagBarCodeNo = c.Key.BagBarCodeNo, TxnStatus = c.Key.TxnStatus, TxnMode = c.Key.TxnMode, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03), Qty01NS = c.Sum(r => r.Qty01NS) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join tavelbarcodedetail in dcap.TravelBarcodeDetails on new { A = groupbarcode.BagBarCodeNo, B = groupbarcode.TxnMode } equals new { A = tavelbarcodedetail.TravelBarCodeNo, B = tavelbarcodedetail.TxnMode }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     group groupbarcode by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         tavelbarcodedetail.Remarks,
                                         groupbarcode.BagBarCodeNo,
                                         groupbarcode.TxnStatus,
                                         groupbarcode.Secondary_Key,
                                         groupbarcode.CreatedBy,
                                         groupbarcode.CreatedDateTime,
                                         groupbarcode.ModifiedBy,
                                         groupbarcode.ModifiedDateTime
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.BagBarCodeNo,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Remarks = grp.Key.Remarks,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.Qty01) - (grp.Sum(g => g.Qty02) + grp.Sum(g => g.Qty03)),
                                         CreatedDateTime = grp.Key.CreatedDateTime,
                                         CreatedBY = grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime,
                                         ModifiedBy = grp.Key.ModifiedBy,
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.WashType, item.BarcodeNo, item.Remarks, item.Delivery_Date, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            Remarks = r.Field<string>("Remarks"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),

                            CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 6) //new otd report 
                {
                    if (grpbt.Count != 0)
                    {
                        grpbb = grpbb.Concat(grpbt).ToList();
                    }

                    var L1Details = (from groupbarcode in dcap.GroupBarcode.Where(c => c.RecStatus == 1 && (WFid == 0 ? true : c.WFId == WFid) && (l1id == 0 ? (c.L1id <= maxL1id && c.L1id >= minL1id) : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.TxnMode, r.OperationCode, r.TxnStatus })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, Secondary_Key = (c.Key.TxnMode + " | " + c.Key.TxnStatus + " | " + c.Key.OperationCode), Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable() //, Qty01NS = c.Sum(r => r.Qty01NS), Qty02NS = c.Sum(r => r.Qty02NS), Qty03NS = c.Sum(r => r.Qty03NS)
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join optransactionq in grpbb on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)optransactionq.L1id, B = (uint?)optransactionq.L2id, C = (uint?)optransactionq.L3id, D = (uint?)optransactionq.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)optransactionq.L1id, B = (uint?)optransactionq.L2id, C = (uint?)optransactionq.L3id, D = (uint?)optransactionq.L4id, E = (uint?)optransactionq.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     join l5moops in gl5moops
                                         on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     group new { groupbarcode, optransactionq, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5no,
                                         optransactionq.L5moid,
                                         optransactionq.L5mono,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.Ref01 + grp.Key.Ref02 + grp.Key.DeliveryDate + grp.Key.L4desc,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.WashType, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty01) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1", "3 | 2", "3 | 3", "3 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();
                }
                else if (mode == 14) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in bflqlty
                                     join rejectreason in dcap.Rejectreason.Where(c => c.DopsId > 0).AsQueryable() on new { A = (uint?)groupbarcode.Rrid } equals new { A = (uint?)rejectreason.Rrid }
                                     join tavelbarcodedetail in dcap.TravelBarcodeDetails on new { A = groupbarcode.JobNo, B = 2 } equals new { A = tavelbarcodedetail.TravelBarCodeNo, B = tavelbarcodedetail.TxnMode }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     group new { groupbarcode, tavelbarcodedetail } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l4.L4desc,  //Color
                                         tavelbarcodedetail.TravelBarCodeNo,
                                         tavelbarcodedetail.Weight,
                                         tavelbarcodedetail.PlannedMachine,
                                         groupbarcode.ModifiedBy,
                                         rejectreason.RejectType,
                                         rejectreason.Rrdesc,
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.TravelBarCodeNo + grp.Key.ModifiedBy,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         Color = grp.Key.L4desc,
                                         BarcodeNo = grp.Key.TravelBarCodeNo,
                                         Weight = grp.Key.Weight,
                                         PlannedMachine = grp.Key.PlannedMachine,
                                         TxnDateTime = grp.Key.ModifiedBy,
                                         //Secondary_Key = grp.Key.RejectType.ToString() + grp.Key.Rrdesc,
                                         RejectType = grp.Key.RejectType,
                                         RejectReason = grp.Key.Rrdesc,
                                         TracelQty = Convert.ToInt32(grp.Max(g => g.tavelbarcodedetail.Qty01)),
                                         Qty = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.RejectType,
                    item => new { item.Primary_Key, item.ProductionWarehouse, item.Style, item.ProductionWarehouseName, item.Shedule, item.Color, item.BarcodeNo, item.Weight, item.PlannedMachine, item.TracelQty, item.TxnDateTime, item.RejectReason },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            Color = r.Field<string>("Color"),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            Weight = r.Field<string>("Weight"),
                            PlannedMachine = r.Field<string>("PlannedMachine"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            Remarks = r.Field<string>("RejectReason"),
                            JobQty = Convert.ToInt32(r.Field<string>("TracelQty")),

                            Buddy_Tag_0 = col.Contains("0") ? Convert.ToDecimal(r.Field<string>("0")) : 0,
                            Buddy_Tag_1 = col.Contains("1") ? Convert.ToDecimal(r.Field<string>("1")) : 0,

                        }).ToList();

                }
                else if (mode == 20) //new otd report 
                {
                    var L1Details = (from groupbarcode in bgg
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BagBarCodeNo, r.TxnMode, r.CreatedBy })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L4id = c.Key.L4id, L5id = c.Key.L5id, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, Secondary_Key = c.Key.CreatedBy, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable() //, Qty01NS = c.Sum(r => r.Qty01NS), Qty02NS = c.Sum(r => r.Qty02NS), Qty03NS = c.Sum(r => r.Qty03NS)
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5id,
                                         l5.L5desc,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.Ref01 + grp.Key.Ref02 + grp.Key.DeliveryDate + grp.Key.L4desc + grp.Key.L5desc,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty01) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1 | 158", "3 | 2 | 158", "3 | 3", "3 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5 | 158", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            Bag_0_160 = col.Contains("1 | 0 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 160")) : 0,
                            Bag_1_160 = col.Contains("1 | 1 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 160")) : 0,
                            Bag_2_160 = col.Contains("1 | 2 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 160")) : 0,
                            Bag_3_160 = col.Contains("1 | 3 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 160")) : 0,
                            Bag_4_160 = col.Contains("1 | 4 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 160")) : 0,
                            Bag_5_160 = col.Contains("1 | 5 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 160")) : 0,
                            Bag_6_160 = col.Contains("1 | 6 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 160")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();
                }
                else if (mode == 21) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in bgg
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BagBarCodeNo, r.CreatedBy })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, BagBarCodeNo = c.Key.BagBarCodeNo, Secondary_Key = c.Key.CreatedBy, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     join l5moops in gl5moops on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, D = (uint?)l5moops.L4id, E = (uint?)l5moops.L5id }
                                     group new { groupbarcode, l5moops } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5id,
                                         l5.L5desc,
                                         groupbarcode.BagBarCodeNo,
                                         groupbarcode.Secondary_Key,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.L5desc + grp.Key.BagBarCodeNo,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.groupbarcode.Qty01) - (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         Qty02 = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    var P2Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key + "S",
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.BarcodeNo, item.Delivery_Date, item.OrderQty, item.ReportedQty },//, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty02) : 0);

                    P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Primary_Key"] };
                    P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Primary_Key"] };

                    P1Details.Merge(P2Details);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            /*CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),*/

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,
                            TravelTag_151 = col.Contains("2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 151 | 151")) : 0,
                            TravelTag_156 = col.Contains("2 | 156") ? Convert.ToDecimal(r.Field<string>("2 | 156 | 151")) : 0,
                            TravelTag_157 = col.Contains("2 | 157") ? Convert.ToDecimal(r.Field<string>("2 | 157 | 151")) : 0,
                            TravelTag_158 = col.Contains("2 | 158") ? Convert.ToDecimal(r.Field<string>("2 | 158 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 22) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in bgg
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BagBarCodeNo, r.TxnMode, r.CreatedBy })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, BagBarCodeNo = c.Key.BagBarCodeNo, TxnMode = c.Key.TxnMode, Secondary_Key = c.Key.CreatedBy, Qty01 = c.Sum(r => r.Qty01), Qty02 = c.Sum(r => r.Qty02), Qty03 = c.Sum(r => r.Qty03) }).AsQueryable()
                                     join groupbarcodeids in groupbarcodepids on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)groupbarcodeids.L1id, B = (uint?)groupbarcodeids.L2id, C = (uint?)groupbarcodeids.L3id, D = (uint?)groupbarcodeids.L4id }
                                     join tavelbarcodedetail in dcap.TravelBarcodeDetails on new { A = groupbarcode.BagBarCodeNo, B = groupbarcode.TxnMode } equals new { A = tavelbarcodedetail.TravelBarCodeNo, B = tavelbarcodedetail.TxnMode }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     group groupbarcode by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         buyerdiv.Name, //Buyer Div Name
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5.L5id,
                                         l5.L5desc,
                                         tavelbarcodedetail.Remarks,
                                         groupbarcode.BagBarCodeNo,
                                         groupbarcode.Secondary_Key,
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.L5desc + grp.Key.BagBarCodeNo,
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         BarcodeNo = grp.Key.BagBarCodeNo,
                                         Remarks = grp.Key.Remarks,
                                         Secondary_Key = grp.Key.Secondary_Key,
                                         Qty = grp.Sum(g => g.Qty01) - (grp.Sum(g => g.Qty02) + grp.Sum(g => g.Qty03)),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new { item.Primary_Key, item.BuyerDivision, item.Style, item.ProductionWarehouse, item.ProductionWarehouseName, item.Shedule, item.PO, item.ZFeature, item.Color, item.Size, item.WashType, item.BarcodeNo, item.Remarks, item.Delivery_Date }, //, item.CreatedBY, item.CreatedDateTime, item.ModifiedBy, item.ModifiedDateTime },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            WashType = r.Field<string>("WashType"),
                            Remarks = r.Field<string>("Remarks"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),

                            /*CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),*/

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,
                            Buddy_Tag_7 = col.Contains("3 | 7 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 7 | 158")) : 0,
                            Buddy_Tag_151 = col.Contains("3 | 151 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 151 | 158")) : 0,
                            Buddy_Tag_156 = col.Contains("3 | 156 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 156 | 158")) : 0,
                            Buddy_Tag_157 = col.Contains("3 | 157 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 157 | 158")) : 0,
                            Buddy_Tag_158 = col.Contains("3 | 158 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 158 | 158")) : 0,

                        }).ToList();

                }
                else if (mode == 23) //txn mode for bag and travel tag summary
                {
                    var L1Details = (from groupbarcode in bflqlty
                                     join rejectreason in dcap.Rejectreason.Where(c => c.DopsId > 0).AsQueryable() on new { A = (uint?)groupbarcode.Rrid } equals new { A = (uint?)rejectreason.Rrid }
                                     join tavelbarcodedetail in dcap.TravelBarcodeDetails on new { A = groupbarcode.JobNo, B = 2 } equals new { A = tavelbarcodedetail.TravelBarCodeNo, B = tavelbarcodedetail.TxnMode }
                                     join l1 in gl1 on (uint?)groupbarcode.L1id equals (uint?)l1.L1id
                                     join l2 in gl2 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in gl4 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, C = (uint?)groupbarcode.L3id, D = (uint?)groupbarcode.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in gl5 on new { A = (uint?)groupbarcode.L1id, B = (uint?)groupbarcode.L2id, D = (uint?)groupbarcode.L4id, E = (uint?)groupbarcode.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     group new { groupbarcode, tavelbarcodedetail } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l4.L4desc,  //Color
                                         l5.L5id,
                                         l5.L5desc,
                                         tavelbarcodedetail.TravelBarCodeNo,
                                         tavelbarcodedetail.Weight,
                                         tavelbarcodedetail.PlannedMachine,
                                         groupbarcode.ModifiedBy,
                                         rejectreason.RejectType,
                                         rejectreason.Rrdesc,
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.TravelBarCodeNo + grp.Key.ModifiedBy,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         Color = grp.Key.L4desc,
                                         Size = grp.Key.L5desc,
                                         BarcodeNo = grp.Key.TravelBarCodeNo,
                                         Weight = grp.Key.Weight,
                                         PlannedMachine = grp.Key.PlannedMachine,
                                         TxnDateTime = grp.Key.ModifiedBy,
                                         //Secondary_Key = grp.Key.RejectType.ToString() + grp.Key.Rrdesc,
                                         RejectType = grp.Key.RejectType,
                                         RejectReason = grp.Key.Rrdesc,
                                         TracelQty = Convert.ToInt32(grp.Max(g => g.tavelbarcodedetail.Qty01)),
                                         Qty = (grp.Sum(g => g.groupbarcode.Qty02) + grp.Sum(g => g.groupbarcode.Qty03)),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.RejectType,
                    item => new { item.Primary_Key, item.ProductionWarehouse, item.Style, item.ProductionWarehouseName, item.Shedule, item.Color, item.Size, item.BarcodeNo, item.Weight, item.PlannedMachine, item.TracelQty, item.TxnDateTime, item.RejectReason },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            Color = r.Field<string>("Color"),
                            Size = r.Field<string>("Size"),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            Weight = r.Field<string>("Weight"),
                            PlannedMachine = r.Field<string>("PlannedMachine"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            Remarks = r.Field<string>("RejectReason"),
                            JobQty = Convert.ToInt32(r.Field<string>("TracelQty")),

                            Buddy_Tag_0 = col.Contains("0") ? Convert.ToDecimal(r.Field<string>("0")) : 0,
                            Buddy_Tag_1 = col.Contains("1") ? Convert.ToDecimal(r.Field<string>("1")) : 0,

                        }).ToList();

                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L2Details;
        }


        [Produces("application/json")]
        [HttpGet("GetBFLDispatchReceiveSummaryByDayReport")]
        public List<BFLInandOut> GetBFLDispatchReceiveSummaryByDayReport(int mode, int txnMode, DateTime fromDate, DateTime toDate, int WFid, int l1id, int l2id, int l4id)
        {

            logger.InfoFormat("GetBFLDispatchReceiveSummaryByDayReport mode={0} txnMode={1} fromDate={2} toDate={3} WFid={4} l1id={5} l2id={6} l4id={7}", mode, txnMode, fromDate, toDate, WFid, l1id, l2id, l4id);
            List<BFLInandOut> L3Details = null;

            try
            {
                DateTime cu = DateTime.MinValue;
                if (toDate == cu)
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    TimeSpan tst = new TimeSpan(0, 23, 59, 59);
                    toDate = toDate.Add(tst);
                }

                if (fromDate == cu)
                {
                    TimeSpan ts = new TimeSpan(31, 0, 0, 0);
                    fromDate = toDate.Subtract(ts);
                }

                if (mode == 1)
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.ModifiedDateTime >= fromDate && c.ModifiedDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && c.TxnStatus >= 4 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlType, r.TxnMode, r.TxnStatus, r.BarCodeNo })
                                            .Select(c => new
                                            {
                                                L1id = c.Key.L1id,
                                                L2id = c.Key.L2id,
                                                L3id = c.Key.L3id,
                                                L4id = c.Key.L4id,
                                                BarCodeNo = c.Key.BarCodeNo,
                                                TxnMode = c.Key.TxnMode,
                                                ModifiedDateTime = c.Max(s => s.ModifiedDateTime).Date,
                                                Qty01 = c.Sum(r => r.Qty01)
                                            }).AsQueryable()
                                         //join goodcontrolids in goodcontroldetailsids on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol } by new
                                     {
                                         l2.Wfid,
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         goodcontrol.TxnMode,
                                         goodcontrol.ModifiedDateTime
                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Wfid + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         Year = grp.Key.ModifiedDateTime.Year,
                                         Month = grp.Key.ModifiedDateTime.Month,
                                         Day = grp.Key.ModifiedDateTime.Day,
                                         TxnMode = grp.Key.TxnMode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Day + "|" + item.TxnMode,
                    item => new
                    {
                        item.Primary_Key,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.Color,
                        item.WashType,
                        item.Delivery_Date,
                        item.Year,
                        item.Month
                    },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);


                    DataColumnCollection col = P1Details.Columns;
                    L3Details = P1Details.AsEnumerable()
                        .Select(r => new BFLInandOut
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            Year = r.Field<string>("Year"),
                            Month = r.Field<string>("Month"),

                            i_1 = col.Contains("1|1") ? Convert.ToDecimal(r.Field<string>("1|1")) : 0,
                            i_2 = col.Contains("2|1") ? Convert.ToDecimal(r.Field<string>("2|1")) : 0,
                            i_3 = col.Contains("3|1") ? Convert.ToDecimal(r.Field<string>("3|1")) : 0,
                            i_4 = col.Contains("4|1") ? Convert.ToDecimal(r.Field<string>("4|1")) : 0,
                            i_5 = col.Contains("5|1") ? Convert.ToDecimal(r.Field<string>("5|1")) : 0,
                            i_6 = col.Contains("6|1") ? Convert.ToDecimal(r.Field<string>("6|1")) : 0,
                            i_7 = col.Contains("7|1") ? Convert.ToDecimal(r.Field<string>("7|1")) : 0,
                            i_8 = col.Contains("8|1") ? Convert.ToDecimal(r.Field<string>("8|1")) : 0,
                            i_9 = col.Contains("9|1") ? Convert.ToDecimal(r.Field<string>("9|1")) : 0,
                            i_10 = col.Contains("10|1") ? Convert.ToDecimal(r.Field<string>("10|1")) : 0,
                            i_11 = col.Contains("11|1") ? Convert.ToDecimal(r.Field<string>("11|1")) : 0,
                            i_12 = col.Contains("12|1") ? Convert.ToDecimal(r.Field<string>("12|1")) : 0,
                            i_13 = col.Contains("13|1") ? Convert.ToDecimal(r.Field<string>("13|1")) : 0,
                            i_14 = col.Contains("14|1") ? Convert.ToDecimal(r.Field<string>("14|1")) : 0,
                            i_15 = col.Contains("15|1") ? Convert.ToDecimal(r.Field<string>("15|1")) : 0,
                            i_16 = col.Contains("16|1") ? Convert.ToDecimal(r.Field<string>("16|1")) : 0,
                            i_17 = col.Contains("17|1") ? Convert.ToDecimal(r.Field<string>("17|1")) : 0,
                            i_18 = col.Contains("18|1") ? Convert.ToDecimal(r.Field<string>("18|1")) : 0,
                            i_19 = col.Contains("19|1") ? Convert.ToDecimal(r.Field<string>("19|1")) : 0,
                            i_20 = col.Contains("20|1") ? Convert.ToDecimal(r.Field<string>("20|1")) : 0,
                            i_21 = col.Contains("21|1") ? Convert.ToDecimal(r.Field<string>("21|1")) : 0,
                            i_22 = col.Contains("22|1") ? Convert.ToDecimal(r.Field<string>("22|1")) : 0,
                            i_23 = col.Contains("23|1") ? Convert.ToDecimal(r.Field<string>("23|1")) : 0,
                            i_24 = col.Contains("24|1") ? Convert.ToDecimal(r.Field<string>("24|1")) : 0,
                            i_25 = col.Contains("25|1") ? Convert.ToDecimal(r.Field<string>("25|1")) : 0,
                            i_26 = col.Contains("26|1") ? Convert.ToDecimal(r.Field<string>("26|1")) : 0,
                            i_27 = col.Contains("27|1") ? Convert.ToDecimal(r.Field<string>("27|1")) : 0,
                            i_28 = col.Contains("28|1") ? Convert.ToDecimal(r.Field<string>("28|1")) : 0,
                            i_29 = col.Contains("29|1") ? Convert.ToDecimal(r.Field<string>("29|1")) : 0,
                            i_30 = col.Contains("30|1") ? Convert.ToDecimal(r.Field<string>("30|1")) : 0,
                            i_31 = col.Contains("31|1") ? Convert.ToDecimal(r.Field<string>("10|1")) : 0,

                            o_1 = WIPOutputByOperationValue(col, r, "1|2", "1|3", "NA", "NA", "+", "-", "+", "+"),
                            o_2 = WIPOutputByOperationValue(col, r, "2|2", "2|3", "NA", "NA", "+", "-", "+", "+"),
                            o_3 = WIPOutputByOperationValue(col, r, "3|2", "3|3", "NA", "NA", "+", "-", "+", "+"),
                            o_4 = WIPOutputByOperationValue(col, r, "4|2", "4|3", "NA", "NA", "+", "-", "+", "+"),
                            o_5 = WIPOutputByOperationValue(col, r, "5|2", "5|3", "NA", "NA", "+", "-", "+", "+"),
                            o_6 = WIPOutputByOperationValue(col, r, "6|2", "6|3", "NA", "NA", "+", "-", "+", "+"),
                            o_7 = WIPOutputByOperationValue(col, r, "7|2", "7|3", "NA", "NA", "+", "-", "+", "+"),
                            o_8 = WIPOutputByOperationValue(col, r, "8|2", "8|3", "NA", "NA", "+", "-", "+", "+"),
                            o_9 = WIPOutputByOperationValue(col, r, "9|2", "9|3", "NA", "NA", "+", "-", "+", "+"),
                            o_10 = WIPOutputByOperationValue(col, r, "10|2", "10|3", "NA", "NA", "+", "-", "+", "+"),
                            o_11 = WIPOutputByOperationValue(col, r, "11|2", "11|3", "NA", "NA", "+", "-", "+", "+"),
                            o_12 = WIPOutputByOperationValue(col, r, "12|2", "12|3", "NA", "NA", "+", "-", "+", "+"),
                            o_13 = WIPOutputByOperationValue(col, r, "13|2", "13|3", "NA", "NA", "+", "-", "+", "+"),
                            o_14 = WIPOutputByOperationValue(col, r, "14|2", "14|3", "NA", "NA", "+", "-", "+", "+"),
                            o_15 = WIPOutputByOperationValue(col, r, "15|2", "15|3", "NA", "NA", "+", "-", "+", "+"),
                            o_16 = WIPOutputByOperationValue(col, r, "16|2", "16|3", "NA", "NA", "+", "-", "+", "+"),
                            o_17 = WIPOutputByOperationValue(col, r, "17|2", "17|3", "NA", "NA", "+", "-", "+", "+"),
                            o_18 = WIPOutputByOperationValue(col, r, "18|2", "18|3", "NA", "NA", "+", "-", "+", "+"),
                            o_19 = WIPOutputByOperationValue(col, r, "19|2", "19|3", "NA", "NA", "+", "-", "+", "+"),
                            o_20 = WIPOutputByOperationValue(col, r, "20|2", "20|3", "NA", "NA", "+", "-", "+", "+"),
                            o_21 = WIPOutputByOperationValue(col, r, "21|2", "21|3", "NA", "NA", "+", "-", "+", "+"),
                            o_22 = WIPOutputByOperationValue(col, r, "22|2", "22|3", "NA", "NA", "+", "-", "+", "+"),
                            o_23 = WIPOutputByOperationValue(col, r, "23|2", "23|3", "NA", "NA", "+", "-", "+", "+"),
                            o_24 = WIPOutputByOperationValue(col, r, "24|2", "24|3", "NA", "NA", "+", "-", "+", "+"),
                            o_25 = WIPOutputByOperationValue(col, r, "25|2", "25|3", "NA", "NA", "+", "-", "+", "+"),
                            o_26 = WIPOutputByOperationValue(col, r, "26|2", "26|3", "NA", "NA", "+", "-", "+", "+"),
                            o_27 = WIPOutputByOperationValue(col, r, "27|2", "27|3", "NA", "NA", "+", "-", "+", "+"),
                            o_28 = WIPOutputByOperationValue(col, r, "28|2", "28|3", "NA", "NA", "+", "-", "+", "+"),
                            o_29 = WIPOutputByOperationValue(col, r, "29|2", "29|3", "NA", "NA", "+", "-", "+", "+"),
                            o_30 = WIPOutputByOperationValue(col, r, "30|2", "30|3", "NA", "NA", "+", "-", "+", "+"),
                            o_31 = WIPOutputByOperationValue(col, r, "31|2", "31|3", "NA", "NA", "+", "-", "+", "+"),
                        }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L3Details;
        }

        [Produces("application/json")]
        [HttpGet("GetBFLDispatchReceiveSummaryReport")]
        public List<BFLStockSummary> GetBFLDispatchReceiveSummaryReport(int mode, int txnMode, DateTime fromDate, DateTime toDate, int WFid)
        {

            logger.InfoFormat("GetBFLGatepassSummaryReport mode={0} txnMode={1} fromDate={2} toDate={3} WFid={4}", mode, txnMode, fromDate, toDate, WFid);
            List<BFLStockSummary> L3Details = null;

            try
            {
                DateTime cu = DateTime.MinValue;
                if (toDate == cu)
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    TimeSpan tst = new TimeSpan(0, 23, 59, 59);
                    toDate = toDate.Add(tst);
                }

                if (fromDate == cu)
                {
                    TimeSpan ts = new TimeSpan(31, 0, 0, 0);
                    fromDate = toDate.Subtract(ts);
                }

                if (mode == 1)
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.ModifiedDateTime >= fromDate && c.ModifiedDateTime <= toDate && c.RecStatus == 1 && c.Return == 0)
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlType, r.TxnMode, r.TxnStatus, r.BarCodeNo })
                                            .Select(c => new
                                            {
                                                L1id = c.Key.L1id,
                                                L2id = c.Key.L2id,
                                                L3id = c.Key.L3id,
                                                L4id = c.Key.L4id,
                                                BarCodeNo = c.Key.BarCodeNo,
                                                ModifiedDateTime = c.Max(s => s.ModifiedDateTime).Date,
                                                TxnMode = c.Key.TxnMode,
                                                Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus,
                                                Qty01 = c.Sum(r => r.Qty01)
                                            }).AsQueryable()
                                     join goodcontrolids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.TxnMode, F = goodcontrol.BarCodeNo } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id, E = goodcontrolids.TxnMode, F = goodcontrolids.BagBarCodeNo }
                                     join l1 in dcap.L1 on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2 on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4 on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15)
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, goodcontrolids } by new
                                     {
                                         l2.Wfid,
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         goodcontrolids.OperationCode,
                                         l4.WashType,
                                         goodcontrol.ModifiedDateTime,
                                         goodcontrol.Secondary_Key

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime.Date,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = 0,//grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = 0,//grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.ModifiedDateTime,
                        item.Color,
                        item.WashType,
                        item.Delivery_Date,
                        item.OrderQty,
                        item.ReportedQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L3Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            Bag_0_160 = col.Contains("1 | 0 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 160")) : 0,
                            Bag_1_160 = col.Contains("1 | 1 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 160")) : 0,
                            Bag_2_160 = col.Contains("1 | 2 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 160")) : 0,
                            Bag_3_160 = col.Contains("1 | 3 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 160")) : 0,
                            Bag_4_160 = col.Contains("1 | 4 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 160")) : 0,
                            Bag_5_160 = col.Contains("1 | 5 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 160")) : 0,
                            Bag_6_160 = col.Contains("1 | 6 | 160") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 160")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,

                            Buddy_Tag_0 = col.Contains("3 | 0 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 0 | 158")) : 0,
                            Buddy_Tag_1 = col.Contains("3 | 1 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 1 | 158")) : 0,
                            Buddy_Tag_2 = col.Contains("3 | 2 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 2 | 158")) : 0,
                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,

                        }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L3Details;
        }

        //Report Lookup Controllers
        [Produces("application/json")]
        [HttpGet("GetBFLGatepassSummaryReport")]
        public List<BFLStockSummary> GetBFLGatepassSummaryReport(int mode, int txnMode, DateTime fromDate, DateTime toDate, int WFid, int l1id, int l2id, int l4id)
        {

            logger.InfoFormat("GetBFLGatepassSummaryReport mode={0} txnMode={1} fromDate={2} toDate={3} WFid={4} l1id={5} l2id={6} l4id={7}", mode, txnMode, fromDate, toDate, WFid, l2id, l2id, l4id);
            List<BFLStockSummary> L2Details = null;

            try
            {
                DateTime cu = DateTime.MinValue;
                if (toDate == cu)
                {
                    toDate = DateTime.Now;
                }
                else
                {
                    TimeSpan tst = new TimeSpan(0, 23, 59, 59);
                    toDate = toDate.Add(tst);
                }

                if (fromDate == cu)
                {
                    TimeSpan ts = new TimeSpan(31, 0, 0, 0);
                    fromDate = toDate.Subtract(ts);
                }

                var goodcontroldetailsids = dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id }).ToList();

                if (mode == 1)
                {

                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlId, r.ControlType, r.TxnDateTime, r.TxnMode, r.BarCodeNo, r.TxnStatus, r.WarLocCode })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, ControlId = c.Key.ControlId, ControlType = c.Key.ControlType, TxnDateTime = c.Key.TxnDateTime, WarLocCode = c.Key.WarLocCode, BarCodeNo = c.Key.BarCodeNo, TxnMode = c.Key.TxnMode, Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus, Qty01 = c.Sum(r => r.Qty01) }).AsQueryable()
                                     join goodcontrolids in goodcontroldetailsids on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id }
                                     join groupebarcodeids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = (uint?)groupebarcodeids.L1id, B = (uint?)groupebarcodeids.L2id, C = (uint?)groupebarcodeids.L3id, D = (uint?)groupebarcodeids.L4id, E = groupebarcodeids.BagBarCodeNo, F = groupebarcodeids.TxnMode }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L3id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, l5moops, groupebarcodeids } by new
                                     {
                                         wf.Wfid, //WFid
                                                  //wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         //goodcontrol.ControlId,
                                         //goodcontrol.TxnDateTime,
                                         //goodcontrol.WarLocCode,
                                         goodcontrol.Secondary_Key,
                                         groupebarcodeids.OperationCode

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         //ControlId = grp.Key.ControlId,
                                         //TxnDateTime = grp.Key.TxnDateTime,
                                         //WarLocCode = grp.Key.WarLocCode,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.Color,
                        item.WashType,
                        item.Delivery_Date,
                        item.OrderQty,
                        item.ReportedQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty }, //, item.ControlId, item.TxnDateTime, item.WarLocCode,
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ControlId  = r.Field<string>("ControlId"),
                            //TxnDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            //WarLocCode  = r.Field<string>("WarLocCode"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            Dispatch_to_in = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            Dispatch_to_out = WIPOutputByOperationValue(col, r, "2 | 1", "2 | 2", "2 | 3", "2 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 1 | 160", "1 | 2 | 160", "1 | 3 | 160", "1 | 4 | 160", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 160", "1 | 6 | 160", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_in = WIPOutputByOperationValue(col, r, "1 | 5", "1 | 6", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_out = WIPOutputByOperationValue(col, r, "2 | 1", "2 | 2", "2 | 3", "2 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("2 | 0") ? Convert.ToDecimal(r.Field<string>("2 | 0")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            //Bag_0 = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,
                            //Bag_1 = col.Contains("1 | 1") ? Convert.ToDecimal(r.Field<string>("1 | 1")) : 0,
                            //Bag_2 = col.Contains("1 | 2") ? Convert.ToDecimal(r.Field<string>("1 | 2")) : 0,
                            //Bag_3 = col.Contains("1 | 3") ? Convert.ToDecimal(r.Field<string>("1 | 3")) : 0,
                            //Bag_4 = col.Contains("1 | 4") ? Convert.ToDecimal(r.Field<string>("1 | 4")) : 0,
                            //Bag_5 = col.Contains("1 | 5") ? Convert.ToDecimal(r.Field<string>("1 | 5")) : 0,
                            //Bag_6 = col.Contains("1 | 6") ? Convert.ToDecimal(r.Field<string>("1 | 6")) : 0,

                            //TravelTag_0 = col.Contains("2 | 0") ? Convert.ToDecimal(r.Field<string>("2 | 0")) : 0,
                            //TravelTag_1 = col.Contains("2 | 1") ? Convert.ToDecimal(r.Field<string>("2 | 1")) : 0,
                            //TravelTag_2 = col.Contains("2 | 2") ? Convert.ToDecimal(r.Field<string>("2 | 2")) : 0,
                            //TravelTag_3 = col.Contains("2 | 3") ? Convert.ToDecimal(r.Field<string>("2 | 3")) : 0,
                            //TravelTag_4 = col.Contains("2 | 4") ? Convert.ToDecimal(r.Field<string>("2 | 4")) : 0,
                            //TravelTag_5 = col.Contains("2 | 5") ? Convert.ToDecimal(r.Field<string>("2 | 5")) : 0,
                            //TravelTag_6 = col.Contains("2 | 6") ? Convert.ToDecimal(r.Field<string>("2 | 6")) : 0,
                            //TravelTag_7 = col.Contains("2 | 7") ? Convert.ToDecimal(r.Field<string>("2 | 7")) : 0,

                        }).ToList();
                }
                else if (mode == 2)
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlId, r.ControlType, r.TxnDateTime, r.BarCodeNo, r.TxnMode, r.TxnStatus, r.WarLocCode, r.CreatedDateTime, r.CreatedBy, r.ModifiedBy, r.ModifiedDateTime })
                                            .Select(c => new
                                            {
                                                L1id = c.Key.L1id,
                                                L2id = c.Key.L2id,
                                                L3id = c.Key.L3id,
                                                L4id = c.Key.L4id,
                                                ControlId = c.Key.ControlId,
                                                ControlType = c.Key.ControlType,
                                                TxnDateTime = c.Key.TxnDateTime,
                                                WarLocCode = c.Key.WarLocCode,
                                                CreatedDateTime = c.Min(s => s.CreatedDateTime),
                                                CreatedBy = c.Key.CreatedBy,
                                                ModifiedDateTime = c.Min(s => s.ModifiedDateTime),
                                                ModifiedBy = c.Key.ModifiedBy,
                                                BarCodeNo = c.Key.BarCodeNo,
                                                TxnMode = c.Key.TxnMode,
                                                Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus,
                                                Qty01 = c.Sum(r => r.Qty01)
                                            }).AsQueryable()
                                         //join goodcontrolids in goodcontroldetailsids on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id }
                                     join groupebarcodeids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = (uint?)groupebarcodeids.L1id, B = (uint?)groupebarcodeids.L2id, C = (uint?)groupebarcodeids.L3id, D = (uint?)groupebarcodeids.L4id, E = groupebarcodeids.BagBarCodeNo, F = groupebarcodeids.TxnMode }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, l5moops, groupebarcodeids } by new
                                     {
                                         l2.Wfid, //WFid
                                                  //wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         goodcontrol.ControlId,
                                         goodcontrol.BarCodeNo,
                                         goodcontrol.CreatedBy,
                                         goodcontrol.CreatedDateTime,
                                         goodcontrol.ModifiedBy,
                                         goodcontrol.ModifiedDateTime,
                                         goodcontrol.TxnDateTime,
                                         goodcontrol.WarLocCode,
                                         goodcontrol.Secondary_Key,
                                         groupebarcodeids.OperationCode

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         ControlId = grp.Key.ControlId,
                                         TxnDateTime = grp.Key.TxnDateTime,
                                         WarLocCode = grp.Key.WarLocCode,
                                         BarcodeNo = grp.Key.BarCodeNo,
                                         CreatedDateTime = grp.Key.CreatedDateTime,
                                         CreatedBY = grp.Key.CreatedBy,
                                         ModifiedDateTime = grp.Key.ModifiedDateTime,
                                         ModifiedBy = grp.Key.ModifiedBy,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.BarcodeNo,
                        item.CreatedBY,
                        item.CreatedDateTime,
                        item.ModifiedBy,
                        item.ModifiedDateTime,
                        item.Color,
                        item.WashType,
                        item.Delivery_Date,
                        item.ControlId,
                        item.TxnDateTime,
                        item.WarLocCode,
                        item.OrderQty,
                        item.ReportedQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            ControlId = r.Field<string>("ControlId"),
                            TxnDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            WarLocCode = r.Field<string>("WarLocCode"),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            CreatedBy = r.Field<string>("CreatedBy"),
                            CreatedDateTime = Convert.ToDateTime(r.Field<string>("CreatedDateTime")),
                            ModifiedBy = r.Field<string>("ModifiedBy"),
                            ModifiedDateTime = Convert.ToDateTime(r.Field<string>("ModifiedDateTime")),

                            //WIP
                            BFL_in = WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            BFL_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 1 | 160", "1 | 2 | 160", "1 | 3 | 160", "1 | 4 | 160", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 160", "1 | 6 | 160", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            Bag_0 = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,
                            Bag_1 = col.Contains("1 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 1 | 151")) : 0,
                            Bag_2 = col.Contains("1 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 2 | 151")) : 0,
                            Bag_3 = col.Contains("1 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 3 | 151")) : 0,
                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_0 = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            TravelTag_1 = col.Contains("2 | 1 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 1 | 151")) : 0,
                            TravelTag_2 = col.Contains("2 | 2 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 2 | 151")) : 0,
                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,
                            TravelTag_7 = col.Contains("2 | 7 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 7 | 151")) : 0,

                        }).ToList();

                }
                else if (mode == 3) //invoice detail
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.ControlId, r.ControlType, r.TxnDateTime, r.TxnMode, r.TxnStatus, r.BarCodeNo, r.WarLocCode })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, ControlId = c.Key.ControlId, ControlType = c.Key.ControlType, TxnDateTime = c.Key.TxnDateTime, WarLocCode = c.Key.WarLocCode, BarCodeNo = c.Key.BarCodeNo, TxnMode = c.Key.TxnMode, Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus, Qty01 = c.Sum(r => r.Qty01) }).AsQueryable()
                                     join goodcontrolqty in dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                           .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, DQty01 = c.Sum(r => r.Qty01) }).AsQueryable()
                                             on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolqty.L1id, B = (uint?)goodcontrolqty.L2id, C = (uint?)goodcontrolqty.L3id, D = (uint?)goodcontrolqty.L4id }
                                     join groupebarcodeids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = (uint?)groupebarcodeids.L1id, B = (uint?)groupebarcodeids.L2id, C = (uint?)groupebarcodeids.L3id, D = (uint?)groupebarcodeids.L4id, E = groupebarcodeids.BagBarCodeNo, F = groupebarcodeids.TxnMode }
                                     join invoicedetails in dcap.InvoiceDetails.GroupBy(c => new { c.L1Id, c.L2Id, c.L3Id, c.L4Id, c.L5Id, c.ControlId, c.InvoiceNo }).Select(c => new { c.Key.L1Id, c.Key.L2Id, c.Key.L3Id, c.Key.L4Id, c.Key.L5Id, c.Key.ControlId, c.Key.InvoiceNo }).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = (uint?)goodcontrol.L5id, F = goodcontrol.ControlId } equals new { A = (uint?)invoicedetails.L1Id, B = (uint?)invoicedetails.L2Id, C = (uint?)invoicedetails.L3Id, D = (uint?)invoicedetails.L4Id, E = (uint?)invoicedetails.L5Id, F = invoicedetails.ControlId }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, l5moops, invoicedetails, groupebarcodeids, goodcontrolqty } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocName,
                                         location.LocDescription,
                                         buyerdiv.Name, //Buyer Div Name
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.SubinPO, // subin PO
                                         //goodcontrolqty.DQty01, //cumulative quantiity
                                         //l4.WashDescription,
                                         l4.WashType,
                                         l4.UnitPrice,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         goodcontrol.ControlId,
                                         goodcontrol.TxnDateTime,
                                         invoicedetails.InvoiceNo,
                                         //goodcontrol.WarLocCode,
                                         goodcontrol.Secondary_Key,
                                         groupebarcodeids.OperationCode,

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc, // 
                                         BuyerDivision = grp.Key.Name,//
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         Factory = grp.Key.LocName,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc, //((grp.Key.SubinPO.TrimEnd() == "" || grp.Key.SubinPO.TrimEnd() == "NO NEED PO") ? grp.Key.L2desc : grp.Key.SubinPO),
                                         SubinPO = ((grp.Key.SubinPO.TrimEnd() == "" || grp.Key.SubinPO.TrimEnd() == "NO NEED PO") ? "" : grp.Key.SubinPO),
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         //WashDescription = grp.Key.WashDescription,
                                         WashType = grp.Key.WashType,
                                         UnitPrice = grp.Key.UnitPrice,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         ControlId = grp.Key.ControlId,
                                         TxnDateTime = grp.Key.TxnDateTime,
                                         InvoiceNo = grp.Key.InvoiceNo,
                                         //WarLocCode = grp.Key.WarLocCode,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         DQty = grp.Max(g => g.goodcontrolqty.DQty01),//grp.Key.DQty01,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.BuyerDivision,
                        item.Style,
                        item.ProductionWarehouse,
                        item.Factory,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.SubinPO,
                        item.PO,
                        item.ZFeature,
                        item.UnitPrice,
                        //item.WashDescription,
                        item.WashType,
                        item.ControlId,
                        item.TxnDateTime,
                        item.InvoiceNo,
                        item.Color,
                        item.Delivery_Date,
                        item.OrderQty,
                        item.ReportedQty,
                        item.DQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty }, // item.WarLocCode,
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            Factory = r.Field<string>("Factory"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            SubinPO = r.Field<string>("SubinPO"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            UnitPrice = Convert.ToDecimal(r.Field<string>("UnitPrice")),
                            //WashDescription = r.Field<string>("WashDescription"),
                            WashType = r.Field<string>("WashType"),
                            InvoiceNo = r.Field<string>("InvoiceNo"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            ControlId = r.Field<string>("ControlId"),
                            TxnDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            DQty = Convert.ToDecimal(r.Field<string>("DQty")),
                            //WarLocCode  = r.Field<string>("WarLocCode"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            Dispatch_to_in = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            Dispatch_to_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1 | 158", "3 | 2 | 158", "3 | 3 | 158", "3 | 4 | 158", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5 | 158", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 1 | 160", "1 | 2 | 160", "1 | 3 | 160", "1 | 4 | 160", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 160", "1 | 6 | 160", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_in = WIPOutputByOperationValue(col, r, "1 | 5", "1 | 6", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_out = WIPOutputByOperationValue(col, r, "2 | 1", "2 | 2", "2 | 3", "2 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 0 | 151")) : 0,

                            //Bag_0 = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,
                            //Bag_1 = col.Contains("1 | 1") ? Convert.ToDecimal(r.Field<string>("1 | 1")) : 0,
                            //Bag_2 = col.Contains("1 | 2") ? Convert.ToDecimal(r.Field<string>("1 | 2")) : 0,
                            //Bag_3 = col.Contains("1 | 3") ? Convert.ToDecimal(r.Field<string>("1 | 3")) : 0,
                            //Bag_4 = col.Contains("1 | 4") ? Convert.ToDecimal(r.Field<string>("1 | 4")) : 0,
                            //Bag_5 = col.Contains("1 | 5") ? Convert.ToDecimal(r.Field<string>("1 | 5")) : 0,
                            //Bag_6 = col.Contains("1 | 6") ? Convert.ToDecimal(r.Field<string>("1 | 6")) : 0,

                            //TravelTag_0 = col.Contains("2 | 0") ? Convert.ToDecimal(r.Field<string>("2 | 0")) : 0,
                            //TravelTag_1 = col.Contains("2 | 1") ? Convert.ToDecimal(r.Field<string>("2 | 1")) : 0,
                            //TravelTag_2 = col.Contains("2 | 2") ? Convert.ToDecimal(r.Field<string>("2 | 2")) : 0,
                            //TravelTag_3 = col.Contains("2 | 3") ? Convert.ToDecimal(r.Field<string>("2 | 3")) : 0,
                            //TravelTag_4 = col.Contains("2 | 4") ? Convert.ToDecimal(r.Field<string>("2 | 4")) : 0,
                            //TravelTag_5 = col.Contains("2 | 5") ? Convert.ToDecimal(r.Field<string>("2 | 5")) : 0,
                            //TravelTag_6 = col.Contains("2 | 6") ? Convert.ToDecimal(r.Field<string>("2 | 6")) : 0,
                            //TravelTag_7 = col.Contains("2 | 7") ? Convert.ToDecimal(r.Field<string>("2 | 7")) : 0,

                        }).ToList();
                }
                else if (mode == 4)
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlId, r.ControlType, r.TxnDateTime, r.TxnMode, r.BarCodeNo, r.TxnStatus, r.WarLocCode })
                                            .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, ControlId = c.Key.ControlId, ControlType = c.Key.ControlType, TxnDateTime = c.Key.TxnDateTime, WarLocCode = c.Key.WarLocCode, TxnMode = c.Key.TxnMode, BarCodeNo = c.Key.BarCodeNo, Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus, Qty01 = c.Sum(r => r.Qty01) }).AsQueryable()
                                         //join goodcontrolids in goodcontroldetailsids on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id }
                                     join groupebarcodeids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = (uint?)groupebarcodeids.L1id, B = (uint?)groupebarcodeids.L2id, C = (uint?)groupebarcodeids.L3id, D = (uint?)groupebarcodeids.L4id, E = groupebarcodeids.BagBarCodeNo, F = groupebarcodeids.TxnMode }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id } equals new { A = (uint?)l1.L1id }
                                     join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, l5moops, groupebarcodeids } by new
                                     {
                                         wf.Wfid, //WFid
                                                  //wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         buyerdiv.Name, //Buyer Div Name
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l4.UnitPrice, //Price
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         //goodcontrol.ControlId,
                                         //goodcontrol.TxnDateTime,
                                         //goodcontrol.WarLocCode,
                                         goodcontrol.Secondary_Key,
                                         groupebarcodeids.OperationCode

                                     } into grp
                                     orderby grp.Key.L2desc
                                     select new
                                     {
                                         Primary_Key = grp.Key.Name + grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc + grp.Key.WashType,// 
                                         BuyerDivision = grp.Key.Name,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         UnitPrice = grp.Key.UnitPrice,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         //ControlId = grp.Key.ControlId,
                                         //TxnDateTime = grp.Key.TxnDateTime,
                                         //WarLocCode = grp.Key.WarLocCode,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.BuyerDivision,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.Color,
                        item.WashType,
                        item.UnitPrice,
                        item.Delivery_Date,
                        item.OrderQty,
                        item.ReportedQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty }, //, item.ControlId, item.TxnDateTime, item.WarLocCode,
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            BuyerDivision = r.Field<string>("BuyerDivision"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            //ControlId  = r.Field<string>("ControlId"),
                            //TxnDateTime = Convert.ToDateTime(r.Field<string>("TxnDateTime")),
                            //WarLocCode  = r.Field<string>("WarLocCode"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Price = Convert.ToDecimal(r.Field<string>("UnitPrice")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            //WIP
                            Dispatch_to_in = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 151", "1 | 6 | 151", "NA", "NA", "+", "+", "+", "+"),
                            Dispatch_to_out = WIPOutputByOperationValue(col, r, "2 | 1 | 151", "2 | 2 | 151", "2 | 3 | 151", "2 | 4 | 151", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5 | 151", "2 | 6 | 151", "2 | 7 | 151", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 1 | 158", "3 | 2 | 158", "3 | 3 | 158", "3 | 4 | 158", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "3 | 5 | 158", "NA", "NA", "NA", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 1 | 160", "1 | 2 | 160", "1 | 3 | 160", "1 | 4 | 160", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "1 | 5 | 160", "1 | 6 | 160", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_in = WIPOutputByOperationValue(col, r, "1 | 5", "1 | 6", "NA", "NA", "+", "+", "+", "+"),
                            //BFL_out = WIPOutputByOperationValue(col, r, "2 | 1", "2 | 2", "2 | 3", "2 | 4", "+", "+", "+", "+") + WIPOutputByOperationValue(col, r, "2 | 5", "NA", "NA", "NA", "+", "+", "+", "+"),
                            BFL_onhand = col.Contains("2 | 0 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 0 | 151")) : 0,
                            To_be_recive = WIPOutputByOperationValue(col, r, "1 | 1 | 151", "1 | 2 | 151", "1 | 3 | 151", "1 | 4 | 151", "+", "+", "+", "+"),
                            Recive_ready = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,

                            //Bag_0 = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,
                            //Bag_1 = col.Contains("1 | 1") ? Convert.ToDecimal(r.Field<string>("1 | 1")) : 0,
                            //Bag_2 = col.Contains("1 | 2") ? Convert.ToDecimal(r.Field<string>("1 | 2")) : 0,
                            //Bag_3 = col.Contains("1 | 3") ? Convert.ToDecimal(r.Field<string>("1 | 3")) : 0,
                            //Bag_4 = col.Contains("1 | 4") ? Convert.ToDecimal(r.Field<string>("1 | 4")) : 0,
                            //Bag_5 = col.Contains("1 | 5") ? Convert.ToDecimal(r.Field<string>("1 | 5")) : 0,
                            //Bag_6 = col.Contains("1 | 6") ? Convert.ToDecimal(r.Field<string>("1 | 6")) : 0,

                            //TravelTag_0 = col.Contains("2 | 0") ? Convert.ToDecimal(r.Field<string>("2 | 0")) : 0,
                            //TravelTag_1 = col.Contains("2 | 1") ? Convert.ToDecimal(r.Field<string>("2 | 1")) : 0,
                            //TravelTag_2 = col.Contains("2 | 2") ? Convert.ToDecimal(r.Field<string>("2 | 2")) : 0,
                            //TravelTag_3 = col.Contains("2 | 3") ? Convert.ToDecimal(r.Field<string>("2 | 3")) : 0,
                            //TravelTag_4 = col.Contains("2 | 4") ? Convert.ToDecimal(r.Field<string>("2 | 4")) : 0,
                            //TravelTag_5 = col.Contains("2 | 5") ? Convert.ToDecimal(r.Field<string>("2 | 5")) : 0,
                            //TravelTag_6 = col.Contains("2 | 6") ? Convert.ToDecimal(r.Field<string>("2 | 6")) : 0,
                            //TravelTag_7 = col.Contains("2 | 7") ? Convert.ToDecimal(r.Field<string>("2 | 7")) : 0,

                        }).ToList();
                }
                else if (mode == 5)
                {
                    var L1Details = (from goodcontrol in dcap.GoodControl.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate && c.RecStatus == 1 && c.Return == 0 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                            .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.ControlType, r.TxnMode, r.TxnStatus, r.BarCodeNo })
                                            .Select(c => new
                                            {
                                                L1id = c.Key.L1id,
                                                L2id = c.Key.L2id,
                                                L3id = c.Key.L3id,
                                                L4id = c.Key.L4id,
                                                //ControlId = c.Key.ControlId,
                                                //ControlType = c.Key.ControlType,
                                                //TxnDateTime = c.Key.TxnDateTime,
                                                //WarLocCode = c.Key.WarLocCode,
                                                //CreatedDateTime = c.Min(s => s.CreatedDateTime),
                                                //CreatedBy = c.Key.CreatedBy,
                                                //ModifiedDateTime = c.Min(s => s.ModifiedDateTime),
                                                //ModifiedBy = c.Key.ModifiedBy,
                                                TxnMode = c.Key.TxnMode,
                                                CreatedDateTime = c.Key.BarCodeNo,
                                                BarCodeNo = c.Min(s => s.ModifiedDateTime).Month.ToString() + "/" + c.Min(s => s.ModifiedDateTime).Day.ToString() + "/" + c.Min(s => s.ModifiedDateTime).Year.ToString(),
                                                Secondary_Key = c.Key.TxnMode + " | " + c.Key.TxnStatus,
                                                Qty01 = c.Sum(r => r.Qty01)
                                            }).AsQueryable()
                                         //join goodcontrolids in goodcontroldetailsids on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)goodcontrolids.L1id, B = (uint?)goodcontrolids.L2id, C = (uint?)goodcontrolids.L3id, D = (uint?)goodcontrolids.L4id }
                                     join groupebarcodeids in dcap.GroupBarcode on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = goodcontrol.CreatedDateTime, F = goodcontrol.TxnMode } equals new { A = (uint?)groupebarcodeids.L1id, B = (uint?)groupebarcodeids.L2id, C = (uint?)groupebarcodeids.L3id, D = (uint?)groupebarcodeids.L4id, E = groupebarcodeids.BagBarCodeNo, F = groupebarcodeids.TxnMode }
                                     join l1 in dcap.L1.Where(c => (l1id == 0 ? true : c.L1id == l1id)).AsQueryable() on (uint?)goodcontrol.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join wf in dcap.Wf on l2.Wfid equals wf.Wfid
                                     join location in dcap.Location on wf.FacCode equals location.FacCode
                                     join l4 in dcap.L4.Where(c => (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id)).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5moops in dcap.L5moops.Where(c => c.OperationCode == 15 && (l1id == 0 ? true : c.L1id == l1id) && (l2id == 0 ? true : c.L2id == l2id) && (l4id == 0 ? true : c.L4id == l4id))
                                             .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id })
                                             .Select(c => new { L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, OrderQty = c.Sum(r => r.OrderQty), ReportedQty = c.Sum(r => r.ReportedQty) }).AsQueryable()
                                         on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id } equals new { A = (uint?)l5moops.L1id, B = (uint?)l5moops.L2id, C = (uint?)l5moops.L3id, D = (uint?)l5moops.L4id }
                                     where (WFid == 0 ? true : l2.Wfid == WFid)
                                     group new { goodcontrol, groupebarcodeids } by new
                                     {
                                         l2.Wfid, //WFid
                                         wf.Wfdesc,// Wf Des
                                         location.LocDescription,
                                         l1.L1desc,  //Style
                                         l2.L2desc,  //Shedule
                                         l2.Ref01,  //PO
                                         l2.Ref02,  //ZFeature
                                         l2.DeliveryDate, //Delivery Date
                                         l4.L4desc,  //Color
                                         l4.WashType,
                                         l5moops.OrderQty,
                                         l5moops.ReportedQty,
                                         goodcontrol.BarCodeNo,
                                         groupebarcodeids.OperationCode,
                                         //goodcontrol.CreatedBy,
                                         //goodcontrol.CreatedDateTime,
                                         //goodcontrol.ModifiedBy,
                                         //goodcontrol.ModifiedDateTime,
                                         //goodcontrol.TxnDateTime,
                                         //goodcontrol.WarLocCode,
                                         goodcontrol.Secondary_Key

                                     } into grp
                                     orderby grp.Key.BarCodeNo
                                     select new
                                     {
                                         Primary_Key = grp.Key.L1desc + grp.Key.L2desc + grp.Key.L4desc,
                                         Style = grp.Key.L1desc,
                                         ProductionWarehouse = grp.Key.Wfid,
                                         ProductionWarehouseName = grp.Key.LocDescription,
                                         Shedule = grp.Key.L2desc,
                                         PO = grp.Key.Ref01,
                                         ZFeature = grp.Key.Ref02,
                                         Color = grp.Key.L4desc,
                                         WashType = grp.Key.WashType,
                                         Delivery_Date = grp.Key.DeliveryDate,
                                         //ActualReciveDate = grp.Min(d => d.groupbarcode.CreatedDateTime),
                                         BarcodeNo = grp.Key.BarCodeNo,
                                         Secondary_Key = grp.Key.Secondary_Key + " | " + grp.Key.OperationCode,
                                         Qty = grp.Sum(g => g.goodcontrol.Qty01),
                                         OrderQty = grp.Key.OrderQty,//grp.Sum(g => g.l5moops.OrderQty),
                                         ReportedQty = grp.Key.ReportedQty,//grp.Sum(g => g.l5moops.ReportedQty),
                                     }).ToList();

                    var P1Details = L1Details.ToPivotTable(
                    item => item.Secondary_Key,
                    item => new
                    {
                        item.Primary_Key,
                        item.Style,
                        item.ProductionWarehouse,
                        item.ProductionWarehouseName,
                        item.Shedule,
                        item.PO,
                        item.ZFeature,
                        item.BarcodeNo,
                        item.Color,
                        item.WashType,
                        item.Delivery_Date,
                        item.OrderQty,
                        item.ReportedQty
                    },//, item.ActualReciveDate, item.Delivery_Date, item.Order_Qty },
                    items => items.Any() ? items.Sum(x => x.Qty) : 0);

                    DataColumnCollection col = P1Details.Columns;
                    L2Details = P1Details.AsEnumerable()
                        .Select(r => new BFLStockSummary
                        {
                            ProductionWarehouse = r.Field<string>("ProductionWarehouse"),
                            ProductionWarehouseName = r.Field<string>("ProductionWarehouseName"),
                            Style = r.Field<string>("Style"),
                            Shedule = r.Field<string>("Shedule"),
                            PO = r.Field<string>("PO"),
                            Zfeature = r.Field<string>("ZFeature"),
                            Color = r.Field<string>("Color"),
                            WashType = r.Field<string>("WashType"),
                            DeliveryDate = Convert.ToDateTime(r.Field<string>("Delivery_Date")),
                            BarcodeNo = r.Field<string>("BarcodeNo"),
                            //ActualReciveDate = Convert.ToDateTime(r.Field<string>("ActualReciveDate")),
                            Order_Qty = Convert.ToDecimal(r.Field<string>("OrderQty")),
                            Reported_Qty = Convert.ToDecimal(r.Field<string>("ReportedQty")),

                            Bag_4 = col.Contains("1 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 4 | 151")) : 0,
                            Bag_5 = col.Contains("1 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 5 | 151")) : 0,
                            Bag_6 = col.Contains("1 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("1 | 6 | 151")) : 0,

                            TravelTag_3 = col.Contains("2 | 3 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 3 | 151")) : 0,
                            TravelTag_4 = col.Contains("2 | 4 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 4 | 151")) : 0,
                            TravelTag_5 = col.Contains("2 | 5 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 5 | 151")) : 0,
                            TravelTag_6 = col.Contains("2 | 6 | 151") ? Convert.ToDecimal(r.Field<string>("2 | 6 | 151")) : 0,

                            Buddy_Tag_3 = col.Contains("3 | 3 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 3 | 158")) : 0,
                            Buddy_Tag_4 = col.Contains("3 | 4 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 4 | 158")) : 0,
                            Buddy_Tag_5 = col.Contains("3 | 5 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 5 | 158")) : 0,
                            Buddy_Tag_6 = col.Contains("3 | 6 | 158") ? Convert.ToDecimal(r.Field<string>("3 | 6 | 158")) : 0,

                        }).ToList();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L2Details;
        }
        //End of report Controllers

        //Production Environment Reports: END

        //Report Lookup Controllers
        [Produces("application/json")]
        [HttpGet("GetBFLInvoiceSummaryReport")]
        public List<InvoiceSummary> GetBFLInvoiceSummaryReport(DateTime fromDate, DateTime toDate)
        {

            logger.InfoFormat("GetBFLInvoiceSummaryReport fromDate={2} toDate={3}", fromDate, toDate);
            List<InvoiceSummary> L2Details = null;

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            DateTime cu = DateTime.MinValue;
            if (toDate == cu)
            {
                toDate = DateTime.Now;
            }
            else
            {
                TimeSpan tst = new TimeSpan(0, 23, 59, 59);
                toDate = toDate.Add(tst);
            }

            if (fromDate == cu)
            {
                TimeSpan ts = new TimeSpan(31, 0, 0, 0);
                fromDate = toDate.Subtract(ts);
            }

            try
            {
                var Locations = dcap.Location.Where(c => c.RecStatus == 1).GroupBy(c => new { c.LocCode, c.CustomerNo, c.LocName }).Select(c => new { c.Key.LocCode, c.Key.CustomerNo, c.Key.LocName }).ToList();

                var Divisions = (from invoivedetails in dcap.InvoiceDetails//.GroupBy(c => new { c.L1Id, c.InvoiceNo }).Select(c => new { c.Key.L1Id, c.Key.InvoiceNo }).AsQueryable()
                                 join l1 in dcap.L1//.GroupBy(c => new { c.L1id, c.BuyerId, c.BuyerDivId }).Select(c => new { c.Key.L1id, c.Key.BuyerId, c.Key.BuyerDivId }).AsQueryable() 
                                 on invoivedetails.L1Id equals l1.L1id
                                 join buyerdiv in dcap.Buyerdiv on new { A = l1.BuyerId, B = l1.BuyerDivId } equals new { A = buyerdiv.BuyerId, B = buyerdiv.BuyerDivId }
                                 group new { invoivedetails, l1, buyerdiv } by new
                                 {
                                     invoivedetails.InvoiceNo,
                                     buyerdiv.Name
                                 } into grp
                                 select new
                                 {
                                     InvoiceNo = grp.Key.InvoiceNo,
                                     BuyerDivison = grp.Key.Name,
                                 }).ToList();

                var invdet = dcap.InvoiceDetails.GroupBy(c => new { c.ControlId, c.InvoiceNo }).Select(c => new { c.Key.ControlId, c.Key.InvoiceNo, Qty01 = c.Sum(d => d.Qty01), Price = c.Sum(d => d.Price) }).ToList();
                var gcd = dcap.GoodControlDetails.Where(c => c.TxnDateTime >= fromDate && c.TxnDateTime <= toDate).GroupBy(c => new { c.ControlId, c.LocCode, c.LocCodeFrom }).Select(c => new { c.Key.ControlId, c.Key.LocCode, c.Key.LocCodeFrom }).ToList();


                L2Details = (from invoiceHeaderDetails in dcap.InvoiceHeaderInformation
                             join invoicedetails in invdet on invoiceHeaderDetails.InvoiceNo equals invoicedetails.InvoiceNo
                             join divisions in Divisions on invoicedetails.InvoiceNo equals divisions.InvoiceNo
                             join goodcontroldetails in gcd on invoicedetails.ControlId equals goodcontroldetails.ControlId
                             join location in Locations on goodcontroldetails.LocCodeFrom equals location.LocCode
                             join location2 in Locations.Select(c => new {LocCode = c.LocCode, CustomerNo2 = c.CustomerNo}).AsQueryable() on goodcontroldetails.LocCode equals location2.LocCode
                             //where goodcontroldetails.TxnDateTime >= fromDate && goodcontroldetails.TxnDateTime <= toDate
                             group new { invoiceHeaderDetails, invoicedetails, divisions, location } by new
                             {
                                 location.CustomerNo,
                                 location2.CustomerNo2,
                                 divisions.BuyerDivison,
                                 invoiceHeaderDetails.InvoiceNo,
                                 invoiceHeaderDetails.TxnDateTime,
                                 invoiceHeaderDetails.VAT,
                                 invoiceHeaderDetails.ExchangeRate,
                                 location.LocName,
                                 invoiceHeaderDetails.TotalPrice,
                             } into grp
                             select new InvoiceSummary
                             {
                                 Customer_Number = grp.Key.CustomerNo,
                                 Division = grp.Key.BuyerDivison,
                                 Payer = grp.Key.CustomerNo2,
                                 Invoice_Number = grp.Key.InvoiceNo,
                                 Invoice_Date = grp.Key.TxnDateTime,
                                 VAT = grp.Key.VAT,
                                 ExchangeRate = grp.Key.ExchangeRate,
                                 Dimension_1 = grp.Key.LocName,
                                 TotalQty = grp.Sum(d => d.invoicedetails.Qty01),
                                 Invoice_Amount_For = grp.Sum(d => d.invoicedetails.Price),
                                 TotalPrice = grp.Sum(d => d.invoiceHeaderDetails.TotalPrice),
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L2Details;
        }

        [Produces("application/json")]
        [HttpGet("GetBarcodeSummaryReport")]
        public List<BarcodeStatus> GetBarcodeSummaryReport(int L1id, int L2id, int L4id)
        {
            logger.InfoFormat("Get Barcode Summary Report L1id={0} L2id={1} L4id={2}", L1id, L2id, L4id);
            List<BarcodeStatus> L2Details = new List<BarcodeStatus>();

            try
            {
                L2Details = (from l5bc in dcap.L5bc.Where(c => c.L1id == L1id && c.L2id == L2id && c.L4id == L4id).AsQueryable()
                             join detxn in dcap.Detxn.Where(c => c.L1id == L1id && c.L2id == L2id && c.L4id == L4id)
                                    .GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5moid, r.BarCodeNo, r.OperationCode, r.Depid, r.Qty01, r.Qty02, r.Qty03 })
                                    .Select(c => new
                                    {
                                        L1id = c.Key.L1id,
                                        L2id = c.Key.L2id,
                                        L3id = c.Key.L3id,
                                        L4id = c.Key.L4id,
                                        L5id = c.Key.L5id,
                                        l5Moid = c.Key.L5moid,
                                        BarCodeNo = c.Key.BarCodeNo,
                                        OperationCode = c.Key.OperationCode,
                                        Depid = c.Key.Depid,
                                        Qty01 = c.Key.Qty01,//c.Sum(r => r.Qty01),
                                        Qty02 = c.Key.Qty02,//c.Sum(r => r.Qty02),
                                        Qty03 = c.Key.Qty03,//c.Sum(r => r.Qty03),
                                        //Qty01Ns = c.Sum(r => r.Qty01Ns),
                                        //Qty02Ns = c.Sum(r => r.Qty02Ns),
                                        //Qty03Ns = c.Sum(r => r.Qty03Ns),
                                    }).AsQueryable() on new { A = (uint?)l5bc.L1id, B = (uint?)l5bc.L2id, C = (uint?)l5bc.L3id, D = (uint?)l5bc.L4id, E = (uint?)l5bc.L5id } equals new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } into ps
                             from detxn in ps.DefaultIfEmpty()
                             join dep in dcap.Dep on detxn.Depid equals dep.Depid
                             join l5 in dcap.L5 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                             group new { l5bc, detxn, dep } by new
                             {
                                 l5bc.BarCodeNo,
                                 l5.L5id,
                                 l5.L5desc,
                                 detxn.OperationCode,
                                 dep.Depdesc,
                                 detxn.Qty01,
                                 detxn.Qty02,
                                 detxn.Qty03,
                             } into grp
                             orderby grp.Key.OperationCode
                             select new BarcodeStatus
                             {
                                 BarcodeNo = grp.Key.BarCodeNo,
                                 Size = grp.Key.L5desc,
                                 //OperationCode = grp.Key.OperationCode,
                                 Department = grp.Key.Depdesc,
                                 Manufacturing_Qty = grp.Key.Qty01,//grp.Sum(g => g.detxn.Qty01),
                                 Qty_Report = grp.Key.Qty02,//grp.Sum(g => g.detxn.Qty02),
                                 Qty_Scrap = grp.Key.Qty03,//grp.Sum(g => g.detxn.Qty03),
                                 //Transaction_Date_and_Time = grp.Max(g => g.detxn.TxnDateTime),
                             }).ToList();

                /*
                var P1Details = L1Details.ToPivotTable(
                item => item.OperationCode + "_G",
                item => new
                {
                    item.BarcodeNo
                },
                items => items.Any() ? items.Sum(x => x.Qty01) : 0);

                var P2Details = L1Details.ToPivotTable(
                item => item.OperationCode + "_S",
                item => new
                {
                    item.BarcodeNo
                },
                items => items.Any() ? items.Sum(x => x.Qty02) : 0);

                var P3Details = L1Details.ToPivotTable(
                item => item.OperationCode + "_R",
                item => new
                {
                    item.BarcodeNo
                },
                items => items.Any() ? items.Sum(x => x.Qty03) : 0);

                P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["BarcodeNo"] };
                P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["BarcodeNo"] };
                P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["BarcodeNo"] };

                P1Details.Merge(P2Details);
                P1Details.Merge(P3Details);

                DataColumnCollection col = P1Details.Columns;
                L2Details = P1Details.AsEnumerable()
                    .Select(r => new BFLStockSummary
                    {
                        BarcodeNo = r.Field<string>("BarcodeNo"),

                        Bag_0 = col.Contains("1 | 0") ? Convert.ToDecimal(r.Field<string>("1 | 0")) : 0,

                    }).ToList();
            */
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L2Details;
        }

        //Maintaince
        //Report Lookup Controllers
        [Produces("application/json")]
        [HttpGet("GetStyleStatustoAchival")]
        public List<L1> GetStyleStatustoAchival(int Style, int RecStatus)
        {

            logger.InfoFormat("Get Style Status to Achival Style={0}, RecStatus={1}", Style, RecStatus);
            List<L1> L1Details = null;

            try
            {
                if (Style == 0)
                {
                    var detxn = dcap.Dedep.GroupBy(c => new { c.L1id }).Select(c => new { L1id = c.Key.L1id, ModifiedDateTime = c.Max(r => r.ModifiedDateTime) }).ToList();

                    L1Details = (from l1 in dcap.L1.Where(c => (RecStatus == 0 ? true : c.RecStatus == RecStatus)).AsQueryable()
                                 join detc in detxn on l1.L1id equals detc.L1id into detxngroup
                                 from det in detxngroup.DefaultIfEmpty()
                                 orderby l1.L1id ascending
                                 select new L1
                                 {
                                     L1id = l1.L1id,
                                     L1desc = l1.L1desc,
                                     ModifiedDateTime = Convert.ToDateTime(det.ModifiedDateTime),
                                     L1status = (uint)(DateTime.Now - Convert.ToDateTime(det.ModifiedDateTime)).Days,
                                     RecStatus = l1.RecStatus,
                                     CreatedDateTime = l1.ModifiedDateTime,
                                     AchivedComments = l1.AchivedComments
                                 }).ToList();

                }
                else
                {
                    var detxn = dcap.Dedep.Where(c => c.L1id == Style).GroupBy(c => new { c.L1id }).Select(c => new { L1id = c.Key.L1id, ModifiedDateTime = c.Max(r => r.ModifiedDateTime) }).ToList();

                    L1Details = (from l1 in dcap.L1.Where(c => c.L1id == Style && (RecStatus == 0 ? true : c.RecStatus == RecStatus)).AsQueryable()
                                 join detc in detxn on l1.L1id equals detc.L1id into detxngroup
                                 from det in detxngroup.DefaultIfEmpty()
                                 orderby l1.L1id ascending
                                 select new L1
                                 {
                                     L1id = l1.L1id,
                                     L1desc = l1.L1desc,
                                     ModifiedDateTime = Convert.ToDateTime(det.ModifiedDateTime),
                                     L1status = (uint)(DateTime.Now - Convert.ToDateTime(det.ModifiedDateTime)).Days,
                                     RecStatus = l1.RecStatus,
                                     AchivedComments = l1.AchivedComments
                                 }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetBarcodeOperationScanStatus")]
        public List<BarcodeDetailAsync> GetBarcodeOperationScanStatus(int L1id, int L2id, int L4id, int Opcode)
        {

            logger.InfoFormat("Get Style Status to Achival L1id={0}, L2id={1}, L1id={2}, Opcode={3}", L1id, L2id, L4id, Opcode);
            List<BarcodeDetailAsync> output = new List<BarcodeDetailAsync>();

            try
            {
                if (Opcode == 0)
                {
                    List<BarcodeDetailAsync> preoutput = new List<BarcodeDetailAsync>();
                    if (L1id == 0 && L2id == 0 && L4id == 0)
                    {

                    }
                    else if (L2id == 0 && L4id == 0)
                    {
                        preoutput = (from detxn in dcap.L5bc.Where(c => c.L1id == L1id).AsQueryable()
                                     join l1 in dcap.L1.Where(d => d.L1id == L1id).AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(d => d.L1id == L1id && d.L2id == L2id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join l4 in dcap.L4.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in dcap.L5.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0 && d.L4id == L4id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     group new { l1, l2, l4, l5, detxn } by new
                                     {
                                         detxn.BarCodeNo,
                                         l1.L1id,
                                         l1.L1no,
                                         l2.L2id,
                                         l2.L2no,
                                         l2.Ref01,
                                         l2.Ref02,
                                         l4.L4id,
                                         l4.L4no,
                                         l4.L4desc,
                                         l5.L5id,
                                         l5.L5desc
                                     } into grp
                                     select new BarcodeDetailAsync
                                     {
                                         Barcode = grp.Key.BarCodeNo,
                                         StyleId = grp.Key.L1id,
                                         StyleNo = grp.Key.L1no,
                                         ScheduleId = grp.Key.L2id,
                                         ScheduleNo = grp.Key.L2no,
                                         PONo = grp.Key.Ref01,
                                         Zfeature = grp.Key.Ref02,
                                         ColorId = grp.Key.L4id,
                                         ColorDesc = grp.Key.L4desc,
                                         SizeId = grp.Key.L5id,
                                         SizeDesc = grp.Key.L5desc
                                     }).ToList();
                    }
                    else if (L4id == 0)
                    {
                        preoutput = (from detxn in dcap.L5bc.Where(c => c.L1id == L1id && c.L2id == L2id).AsQueryable()
                                     join l1 in dcap.L1.Where(d => d.L1id == L1id).AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(d => d.L1id == L1id && d.L2id == L2id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join l4 in dcap.L4.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in dcap.L5.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0 && d.L4id == L4id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     group new { l1, l2, l4, l5, detxn } by new
                                     {
                                         detxn.BarCodeNo,
                                         l1.L1id,
                                         l1.L1no,
                                         l2.L2id,
                                         l2.L2no,
                                         l2.Ref01,
                                         l2.Ref02,
                                         l4.L4id,
                                         l4.L4no,
                                         l4.L4desc,
                                         l5.L5id,
                                         l5.L5desc
                                     } into grp
                                     select new BarcodeDetailAsync
                                     {
                                         Barcode = grp.Key.BarCodeNo,
                                         StyleId = grp.Key.L1id,
                                         StyleNo = grp.Key.L1no,
                                         ScheduleId = grp.Key.L2id,
                                         ScheduleNo = grp.Key.L2no,
                                         PONo = grp.Key.Ref01,
                                         Zfeature = grp.Key.Ref02,
                                         ColorId = grp.Key.L4id,
                                         ColorDesc = grp.Key.L4desc,
                                         SizeId = grp.Key.L5id,
                                         SizeDesc = grp.Key.L5desc
                                     }).ToList();
                    }
                    else
                    {
                        preoutput = (from detxn in dcap.L5bc.Where(c => c.L1id == L1id && c.L2id == L2id && c.L4id == L4id).AsQueryable()
                                     join l1 in dcap.L1.Where(d => d.L1id == L1id).AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                     join l2 in dcap.L2.Where(d => d.L1id == L1id && d.L2id == L2id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join l4 in dcap.L4.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in dcap.L5.Where(d => d.L1id == L1id && d.L2id == L2id && d.L3id == 0 && d.L4id == L4id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     group new { l1, l2, l4, l5, detxn } by new
                                     {
                                         detxn.BarCodeNo,
                                         l1.L1id,
                                         l1.L1no,
                                         l2.L2id,
                                         l2.L2no,
                                         l2.Ref01,
                                         l2.Ref02,
                                         l4.L4id,
                                         l4.L4no,
                                         l4.L4desc,
                                         l5.L5id,
                                         l5.L5desc
                                     } into grp
                                     select new BarcodeDetailAsync
                                     {
                                         Barcode = grp.Key.BarCodeNo,
                                         StyleId = grp.Key.L1id,
                                         StyleNo = grp.Key.L1no,
                                         ScheduleId = grp.Key.L2id,
                                         ScheduleNo = grp.Key.L2no,
                                         PONo = grp.Key.Ref01,
                                         Zfeature = grp.Key.Ref02,
                                         ColorId = grp.Key.L4id,
                                         ColorDesc = grp.Key.L4desc,
                                         SizeId = grp.Key.L5id,
                                         SizeDesc = grp.Key.L5desc
                                     }).ToList();
                    }

                    if (preoutput.Count != 0)
                    {
                        foreach (BarcodeDetailAsync ot in preoutput)
                        {
                            List<Detxn> dt = dcap.Detxn.Where(c => c.BarCodeNo == ot.Barcode && c.L1id == ot.StyleId && c.L2id == ot.ScheduleId && c.L4id == ot.ColorId && c.L5id == ot.SizeId).ToList();
                            List<BarcodeDetailAsync> dm = new List<BarcodeDetailAsync>();

                            foreach (Detxn d in dt)
                            {
                                BarcodeDetailAsync ds = new BarcodeDetailAsync
                                {
                                    Barcode = d.BarCodeNo,
                                    StyleId = d.L1id,
                                    StyleNo = ot.StyleNo,
                                    ScheduleId = (uint)d.L2id,
                                    ScheduleNo = ot.ScheduleNo,
                                    PONo = ot.PONo,
                                    Zfeature = ot.Zfeature,
                                    ColorId = (uint)d.L4id,
                                    ColorDesc = ot.ColorDesc,
                                    SizeId = (uint)d.L5id,
                                    SizeDesc = ot.SizeDesc,
                                    L5mono = d.L5mono,
                                    OperationCode = d.OperationCode,
                                    EnteredQtyGd = (int)d.Qty01,
                                    EnteredQtyScrap = (int)d.Qty02,
                                    EnteredQtyRw = (int)d.Qty03
                                };

                                dm.Add(ds);
                            }

                            output = output.Concat(dm).ToList();
                        }

                        var P1Details = output.ToPivotTable(
                            item => "G" + item.OperationCode,
                            item => new { item.Barcode, item.StyleId, item.StyleNo, item.ScheduleId, item.ScheduleNo, item.PONo, item.Zfeature, item.ColorId, item.ColorDesc, item.SizeId, item.SizeDesc },
                            items => items.Any() ? items.Sum(x => x.EnteredQtyGd) : 0);

                        var P2Details = output.ToPivotTable(
                            item => "S" + item.OperationCode,
                            item => new { item.Barcode, item.StyleId, item.StyleNo, item.ScheduleId, item.ScheduleNo, item.PONo, item.Zfeature, item.ColorId, item.ColorDesc, item.SizeId, item.SizeDesc },
                            items => items.Any() ? items.Sum(x => x.EnteredQtyScrap) : 0);

                        var P3Details = output.ToPivotTable(
                            item => "R" + item.OperationCode,
                            item => new { item.Barcode, item.StyleId, item.StyleNo, item.ScheduleId, item.ScheduleNo, item.PONo, item.Zfeature, item.ColorId, item.ColorDesc, item.SizeId, item.SizeDesc },
                            items => items.Any() ? items.Sum(x => x.EnteredQtyRw) : 0);


                        P1Details.PrimaryKey = new DataColumn[] { P1Details.Columns["Barcode"] };
                        P2Details.PrimaryKey = new DataColumn[] { P2Details.Columns["Barcode"] };
                        P3Details.PrimaryKey = new DataColumn[] { P3Details.Columns["Barcode"] };


                        P1Details.Merge(P2Details);
                        P1Details.Merge(P3Details);

                        DataColumn[] OperationColumns_M = P1Details.Columns.Cast<DataColumn>().Skip(11).ToArray();

                        output = P1Details.AsEnumerable()
                        .Select(r => new BarcodeDetailAsync
                        {
                            Barcode = r.Field<string>("Barcode"),
                            StyleId = (uint)Convert.ToInt32(r.Field<string>("StyleId")),
                            StyleNo = r.Field<string>("StyleNo"),
                            ScheduleId = (uint)Convert.ToInt32(r.Field<string>("ScheduleId")),
                            ScheduleNo = r.Field<string>("ScheduleNo"),
                            PONo = r.Field<string>("PONo"),
                            Zfeature = r.Field<string>("Zfeature"),
                            ColorId = (uint)Convert.ToInt32(r.Field<string>("ColorId")),
                            ColorDesc = r.Field<string>("ColorDesc"),
                            SizeId = (uint)Convert.ToInt32(r.Field<string>("SizeId")),
                            SizeDesc = r.Field<string>("SizeDesc"),

                            Qty = OperationColumns_M.Where(c => Convert.ToDecimal(r.Field<string>(c)) != 0).Select(c => new
                            {
                                Operation = c.ColumnName,
                                Qty = Convert.ToDecimal(r.Field<string>(c)),
                            }).ToDictionary(x => x.Operation, x => x.Qty),
                        }).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetBarcodeOperationScanStatus information {0}", e.ToString());
                throw e;
            }
            return output;
        }
    }
}

/**/ 
