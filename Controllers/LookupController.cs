/*
Description: Secuser Controller Class
Created By : NalindaW
Created on : 2018-10-04
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Brandix.DCAP.API.CustomModels;
using Brandix.DCAP.API.Models;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;


namespace Brandix.DCAP.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]

    public class LookupController : ControllerBase
    {
        #region Variable Declarations
        static ILog logger = LogManager.GetLogger(typeof(LookupController));
        private DCAPDbContext dcap;
        private System.Guid guid;

        #endregion

        #region Constructor

        public LookupController(DCAPDbContext context)
        {
            dcap = context;
        }

        #endregion

        #region APIs

        //GET api /shedule/GetScheduleByID - API to get Schedule By ID Information
        [Produces("application/json")]
        [HttpGet("GetScheduleDetailBySchedule")]
        public StyleScheduleColor GetScheduleDetailBySchedule(string ScheduleNo)
        {

            logger.InfoFormat("Get ScheduleIDBy Schedule API called with Schedule={0}", ScheduleNo);
            StyleScheduleColor StScheCol = null;
            try
            {
                StScheCol = (from l1 in dcap.L1
                             join l2 in dcap.L2 on new { A = l1.L1id } equals new { A = l2.L1id }
                             where l2.L2no == ScheduleNo && l1.RecStatus == (int)eRecStatus.Active
                             select new StyleScheduleColor
                             {
                                 StyleId = l1.L1id,
                                 StyleNo = l1.L1no,
                                 StyleDesc = l1.L1desc,
                                 Ref01 = l1.Ref01,
                                 ScheduleId = l2.L2id,
                                 ScheduleNo = l2.L2no,
                                 ScheduleDesc = l2.L2desc,
                                 Zfeature = l2.Ref01,
                                 PONo = l2.Ref02,
                                 WFId = l2.Wfid
                             })
                         .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            return StScheCol;

        }

        //GET Factory combo date in Bulk update screen , Display values is concatednated string and ID is WFID
        [Produces("application/json")]
        [HttpGet("GetFactoryWFComboData")]
        public IList<ComboItemlList> GetFactoryWFComboData(string ScheduleNo)
        {
            logger.InfoFormat("Get ScheduleIDBy Schedule API called with Schedule={0}", ScheduleNo);
            IList<ComboItemlList> ComboItemlList = null;
            try
            {
                ComboItemlList = (from wf in dcap.Wf
                                  join l2 in dcap.L2 on wf.Wfid equals l2.Wfid
                                  // join l2 in dcap.L2 on new { A = l1.L1id } equals new { A = l2.L1id }
                                  where l2.L2no == ScheduleNo && l2.RecStatus == (int)eRecStatus.Active
                                  select new ComboItemlList
                                  {
                                      DisplayVal = wf.WfidRef.ToString().Replace("-", "") + "-" + wf.Wfdesc.ToString().Replace("-", "") + "-" + wf.Sbucode.ToString().Replace("-", "") + "-" + wf.FacCode.ToString().Replace("-", ""),
                                      WFID = wf.Wfid

                                  }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetFactoryWFComboData {0}", e.ToString());
                throw e;
            }
            return ComboItemlList;
        }

        //GET Operation combo in Bulk upload scereen and display value is a concatenated string and ID is WFDEPInstId
        [Produces("application/json")]
        [HttpGet("GetOpetationTeamCombo")]
        public ComboItemlList GetOpetationTeamCombo(int WFID)
        {
            logger.InfoFormat("Get GetOpetationTeamCombo Schedule API called with WFID={0}", WFID);
            ComboItemlList ComboItemlList = null;
            try
            {
                ComboItemlList = (from w in dcap.Wfdep
                                  join d in dcap.Dep on w.Depid equals d.Depid
                                  join t in dcap.Team on new { A = w.TeamId } equals new { A = t.TeamId }
                                  where w.Wfid == WFID && w.Wfdepstatus == 1 && w.RecStatus == (int)eRecStatus.Active && w.DataCaptureMode == 2
                                  select new ComboItemlList
                                  {
                                      DisplayVal = d.Depdesc + "-" + t.TeamName,
                                      WfdepinstId = w.WfdepinstId
                                  }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetOpetationTeamCombo {0}", e.ToString());
                throw e;
            }
            return ComboItemlList;
        }

        //Get production hours by Team ID
        [Produces("application/json")]
        [HttpGet("GetProductionHoursComboData")]
        public IList<ComboItemlList> GetProductionHoursComboData(uint TeamId)
        {
            logger.InfoFormat("GetProductionHoursByTeamID API called with TeamID={0}", TeamId);
            IList<ComboItemlList> ComboItemlList = null;
            // IList<Prodhour> prodhour = null;

            try
            {
                ComboItemlList = (from p in dcap.Prodhour
                                  where p.TeamId == TeamId
                                  select new ComboItemlList
                                  {
                                      DisplayVal = p.HourName + "-" + p.Stime + " To " + p.Etime,
                                      HourNo = p.HourNo
                                  }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetProductionHoursComboData {0}", e.ToString());
                throw e;
            }
            return ComboItemlList;
        }

        //GET Operation combo in Bulk upload scereen and display value is a concatenated string and ID is WFDEPInstId
        [Produces("application/json")]
        [HttpGet("GetRelatedNodes")]
        public IList<TRelatednodes> GetRelatedNodes(int WFDEPInstId, int NextNodes)
        {
            logger.InfoFormat("Get Related for given node API called with WFDEPInstId={0},NextNodes{0}", WFDEPInstId, NextNodes);
            IList<TRelatednodes> TNextnodeslst = null;

            try
            {
                string strSql = "";

                if (NextNodes == 1)
                {
                    strSql = " select k.WFDEPIdLink   ,  d.DEPId , d.DEPDesc , t.TeamName, t.TeamId "
                             + " from wfdeplink k inner join wfdep w  on k.WFDEPIdLink = w.WFDEPInstId "
                             + " inner join  dep d  on w.DEPId = d.DEPId and k.WFDEPIdLink = w.WFDEPInstId "
                             + " inner join team t on w.TeamId = t.TeamId WHERE k.WFDEPInstId = {0} AND k.RecStatus = 1";
                }
                else
                {
                    strSql = " select k.WFDEPIdLink   ,  d.DEPId , d.DEPDesc , t.TeamName, t.TeamId "
                             + " from wfdeplink k inner join wfdep w  on k.WFDEPIdLink = w.WFDEPInstId "
                             + " inner join  dep d  on w.DEPId = d.DEPId and k.WFDEPIdLink = w.WFDEPInstId "
                             + " inner join team t on w.TeamId = t.TeamId WHERE k.WFDEPIdLink = {0} AND k.RecStatus = 1";
                }

                TNextnodeslst = dcap.TRelatednodes
                            .FromSql(strSql, WFDEPInstId)
                            .ToList();

                // var aa = dcap.Database.ExecuteSqlCommand(sql);
                //  dcap.Database.ExecuteSqlCommand("CreateStudents @p0, @p1", parameters: new[] { "Bill", "Gates" });
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetRelatedNodes {0}", e.ToString());
                throw e;
            }
            return TNextnodeslst;
        }


        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStyleByID")]
        public L1 GetStyleByID(int StyleID)
        {

            logger.InfoFormat("Get Style ByID API called with Style ID={0}", StyleID);
            L1 style = null;

            try
            {
                style = dcap.L1
               .Where(l => l.L1id == StyleID && l.RecStatus == (int)eRecStatus.Active)
               .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Style information {0}", e.ToString());
                throw e;
            }
            return style;
        }

        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStylesByStyleNo")]
        public IList<L1> GetStylesByStyleNo(string Style)
        {

            logger.InfoFormat("Get Styles By StyleNo called with Style No={0}", Style);
            IList<L1> Styles = null;

            try
            {
                if (Style != null)
                {

                    Styles = (from l in dcap.L1
                              where l.L1no.Contains(Style) && l.RecStatus == (int)eRecStatus.Active
                              select new L1
                              {
                                  L1id = l.L1id,
                                  L1no = l.L1no,
                                  L1desc = l.L1desc,
                                  Ref01 = l.Ref01,
                                  Ref02 = l.Ref02,
                                  Ref03 = l.Ref03
                              }).ToList();

                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Style information {0}", e.ToString());
                throw e;
            }
            return Styles;
        }

        //GET api /style/GetSheduleByID - API to get Shedule By ID Information      
        [Produces("application/json")]
        [HttpGet("GetShedulesByStyleNo")]
        public IList<L2> GetShedulesByStyleNo(string Shedule, int L1id)
        {

            logger.InfoFormat("Get Shedules By SheduleNo called with Shedule No={0}, L1id={1}", Shedule, L1id);
            IList<L2> Shedules = null;

            Shedule = Shedule == null ? "%" : Shedule;

            try
            {
                if (Shedule != null)
                {

                    Shedules = (from l in dcap.L2
                                where l.L1id == L1id && EF.Functions.Like(l.L2desc, "%" + Shedule + "%") && l.RecStatus == (int)eRecStatus.Active
                                select new L2
                                {
                                    L2id = l.L2id,
                                    L2no = l.L2no,
                                    L2desc = l.L2desc,
                                    Ref01 = l.Ref01,
                                    Ref02 = l.Ref02
                                }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Shedule information {0}", e.ToString());
                throw e;
            }
            return Shedules;
        }

        //GET api /style/GetColorsByStyleSheduleNo - API to get Color By ID Information      
        [Produces("application/json")]
        [HttpGet("GetColorsByStyleSheduleNo")]
        public IList<L4> GetColorsByStyleSheduleNo(string Color, int L1id, int L2id)
        {

            logger.InfoFormat("Get Shedules By SheduleNo called with Color No={0}, L1id={1}, L2id={2}", Color, L1id, L2id);
            IList<L4> Colors = null;

            Color = Color == null ? "%" : Color;

            try
            {
                if (Color != null)
                {

                    Colors = (from l in dcap.L4
                              where l.L1id == L1id && l.L2id == L2id && EF.Functions.Like(l.L4desc, "%" + Color + "%") && l.RecStatus == (int)eRecStatus.Active
                              select new L4
                              {
                                  L4id = l.L4id,
                                  L4no = l.L4no,
                                  L4desc = l.L4desc,
                              }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Color information {0}", e.ToString());
                throw e;
            }
            return Colors;
        }

        //GET api /style/GetColorsByStyleSheduleNo - API to get Color By ID Information      
        [Produces("application/json")]
        [HttpGet("GetSizesByStyleSheduleColorNo")]
        public IList<L5> GetSizesByStyleSheduleColorNo(string Size, int L1id, int L2id, int L4id)
        {

            logger.InfoFormat("Get Shedules By SheduleNo Color called with Size No={0}, L1id={1}, L2id={2}", Size, L1id, L2id, L4id);
            IList<L5> Sizes = null;

            Size = Size == null ? "%" : Size;

            try
            {
                if (Size != null)
                {

                    Sizes = (from l in dcap.L5
                             where l.L1id == L1id && l.L2id == L2id && l.L4id == L4id && EF.Functions.Like(l.L5desc, "%" + Size + "%") && l.RecStatus == (int)eRecStatus.Active
                             select new L5
                             {
                                 L5id = l.L5id,
                                 L5no = l.L5no,
                                 L5desc = l.L5desc,
                             }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Color information {0}", e.ToString());
                throw e;
            }
            return Sizes;
        }

        //GET api to retrive Colors By Styles No   
        [Produces("application/json")]
        [HttpGet("GetColorsByStylesNo")]
        public IList<L4> GetColorsByStylesNo(string Style, string Color)
        {

            logger.InfoFormat("GetColorsByStylesNo Style ID={0},  Color={1}", Style, Color);
            IList<L4> Colors = null;

            Style = Style == null ? "" : Style;
            Color = Color == null ? "%" : Color;

            try
            {

                Colors = (from l1 in dcap.L1
                          join l4 in dcap.L4 on l1.L1id equals l4.L1id
                          where l1.L1no.Contains(Style) && EF.Functions.Like(l4.L4no, "%" + Color + "%") && l1.RecStatus == (int)eRecStatus.Active
                          group l4 by new { l4.L4id, l4.L4no }
                          into grp
                          select new L4
                          {
                              L4id = grp.Max(l4 => l4.L4id),
                              L4no = grp.Max(l4 => l4.L4no),
                              L4desc = grp.Max(l4 => l4.L4desc)
                          }).ToList();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return Colors;
        }

        //GET api /workflow/GetWFDetailByID - API to get WF By ID Information        
        [Produces("application/json")]
        [HttpGet("GetWFDetailByID")]
        public Wf GetWFDetailByID(int WFID)
        {

            logger.InfoFormat("Get WFDetailByID API called with WFID={0}", WFID);
            Wf WF_1 = null;

            try
            {
                WF_1 = dcap.Wf
                .Where(w => w.Wfid == WFID && w.RecStatus == (int)eRecStatus.Active)
                .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetWFDetailByID {0}", e.ToString());
                throw e;
            }
            return WF_1;
        }

        //GET api /Operations/GetOpperationsByWFID - API to get WF By ID Information        
        [Produces("application/json")]
        [HttpGet("GetOpperationsByWFID")]
        public IList<WFOperations> GetOpperationsByWFID(int Wfid)
        {

            logger.InfoFormat("GetOpperationsByWFID API called with WFID={0}", Wfid);

            IList<WFOperations> WFOperations = null;
            try
            {
                WFOperations = (from wf in dcap.Wfdep
                                join de in dcap.Dep on wf.Depid equals de.Depid
                                join te in dcap.Team on wf.TeamId equals te.TeamId
                                where wf.Wfid == Wfid && wf.Wfdepstatus == 1 && wf.RecStatus == (int)eRecStatus.Active
                                && wf.DataCaptureMode == 2
                                orderby wf.OperationCode, te.TeamId
                                select new WFOperations
                                {
                                    WfdepinstId = wf.WfdepinstId,
                                    Depid = wf.Depid,
                                    Depdesc = de.Depdesc,
                                    TeamName = te.TeamName,
                                    OperationCode = wf.OperationCode,
                                    OppTeamName = de.Depdesc + " - " + te.TeamName
                                }).ToList();

                //  }).ToList().FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetOpperationsByWFID {0}", e.ToString());
                throw e;
            }
            return WFOperations;
        }

        //GET api /Operations/GetOpperationsByWFID - API to get WF By ID Information        
        [Produces("application/json")]
        [HttpGet("GetOpperationsByUser")]
        public IList<ComboItemlList> GetOpperationsByUser(string UserId, string ClientIp)
        {

            logger.InfoFormat("GetOpperationsByUser API called with UserId={0}", UserId);

            IList<WFOperations> WFOperations = null;
            IList<ComboItemlList> ComboItemlList = null;
            try
            {
                WFOperations = (from d in dcap.Dep
                                join s in dcap.Secuserrightdep on d.OperationCode equals (int)s.Depid
                                join w in dcap.Wfdep on d.Depid equals w.Depid
                                join t in dcap.Team on w.TeamId equals t.TeamId
                                join c in dcap.Clientconfig on new { A = (int)w.Wfid, B = w.OperationCode, C = (int)w.WfdepinstId }
                                                        equals new { A = (int)c.WfId, B = c.OpCode1, C = (int)c.WfdepinstId }
                                where s.UserId == UserId && w.RecStatus == (int)eRecStatus.Active && c.ClientIp == ClientIp
                                group new { w, d, t } by new { w.OperationCode, d.Depid, d.Depdesc, t.TeamName, t.TeamId }
                                into grp
                                orderby grp.Key.OperationCode, grp.Key.TeamId
                                select new WFOperations
                                {
                                    //WfdepinstId = grp.Key.WfdepinstId,
                                    Depid = grp.Key.Depid,
                                    Depdesc = grp.Key.Depdesc,
                                    TeamName = grp.Key.TeamId.ToString(),
                                    OperationCode = grp.Key.OperationCode,
                                    OppTeamName = grp.Key.Depdesc + " - " + grp.Key.TeamName
                                }).ToList();

                if (WFOperations != null)
                {
                    ComboItemlList = (from w in WFOperations
                                      group new { w } by new { w.OperationCode, w.OppTeamName, w.TeamName } into grp
                                      select new ComboItemlList
                                      {
                                          ValueField = "[" + grp.Key.OperationCode.ToString() + "]" + "-[" + grp.Key.TeamName + "]",
                                          DisplayVal = grp.Key.OppTeamName
                                      }).ToList();


                }

                // WFOperations = (from d in dcap.Dep
                //                 join s in dcap.Secuserrightdep on d.OperationCode equals (int)s.Depid  
                //                 join w in dcap.Wfdep on d.Depid equals w.Depid
                //                 join t in dcap.Team on w.TeamId equals t.TeamId
                //                 where s.UserId == UserId && w.RecStatus == (int)eRecStatus.Active
                //                 group new { w,d,t } by new { w.WfdepinstId, w.OperationCode, d.Depid,d.Depdesc, t.TeamName, t.TeamId} 
                //                 into grp
                //                 orderby grp.Key.OperationCode,grp.Key.TeamId
                //                 select new WFOperations
                //                 {
                //                     WfdepinstId = grp.Key.WfdepinstId,
                //                     Depid = grp.Key.Depid,
                //                     Depdesc = grp.Key.Depdesc,
                //                     TeamName = grp.Key.TeamName,
                //                     OperationCode = grp.Key.OperationCode,
                //                     OppTeamName =  grp.Key.Depdesc + " - " +  grp.Key.TeamName + "-[" +  grp.Key.OperationCode.ToString() + "]" + "-[" +  grp.Key.TeamId.ToString() + "]"
                //                 }).ToList();    
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetOpperationsByUser {0}", e.ToString());
                throw e;
            }
            return ComboItemlList;
        }

        //GET api /Operations/GetOpperationsByWFID - API to get WF By ID Information        
        [Produces("application/json")]
        [HttpGet("GetAllOperations")]
        public IList<Dep> GetAllOperations()
        {

            logger.InfoFormat("GetAllOperations API called");

            IList<Dep> WFOperations = null;
            try
            {
                WFOperations = dcap.Dep.Where(c => c.RecStatus == 1).OrderBy(c => c.OperationCode).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetOpperationsByUser {0}", e.ToString());
                throw e;
            }
            return WFOperations;
        }

        //Get Colors for give style & Schedule
        [Produces("application/json")]
        [HttpGet("GetColorsForStyleSchedule")]
        public IList<L4> GetColorsForStyleSchedule(int StyleID, int SheduleID)
        {
            logger.InfoFormat("GetColorsForStyleSchedule API called with StyleID={0}, SheduleID={1}", StyleID, SheduleID);
            IList<L4> L4 = null;

            try
            {
                L4 = (from l in dcap.L4
                      where l.L1id == StyleID && l.L2id == SheduleID && l.L3id == 0 && l.L4status == 1 && l.RecStatus == (int)eRecStatus.Active
                      select new L4
                      {
                          L4id = l.L4id,
                          L4no = l.L4no
                      }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsForStyleSchedule information {0}", e.ToString());
                throw e;
            }
            return L4;
        }

        //Get Workflow configuration  by workflow ID
        [Produces("application/json")]
        [HttpGet("GetWFConfigurationbyID")]
        public Wfdep GetWFConfigurationbyID(uint WfdepinstId)
        {
            logger.InfoFormat("GetWFConfigurationbyID API called with WFDEPInstId={0}", WfdepinstId);
            Wfdep Wfdep = null;

            try
            {
                Wfdep = (from w in dcap.Wfdep
                         where w.WfdepinstId == WfdepinstId
                         select new Wfdep
                         {
                             WfdepinstId = w.WfdepinstId,
                             Wfid = w.Wfid,
                             Depid = w.Depid,
                             TeamId = w.TeamId,
                             L1id = w.L1id,
                             Dclid = w.Dclid,
                             Dcmid = w.Dcmid,
                             InheritDepid = w.InheritDepid,
                             NoOfMulQty = w.NoOfMulQty,
                             LimtWithPredecessor = w.LimtWithPredecessor,
                             ExOpCode = w.ExOpCode,
                             PredDepid = w.PredDepid,
                             LimitWithPredDclid = w.LimitWithPredDclid,
                             LimitWithPredScrapDclid = w.LimitWithPredScrapDclid,
                             LimitWithWf = w.LimitWithWf,
                             LimitWithLevel = w.LimitWithLevel,
                             LimitDclid = w.LimitDclid,
                             Bqsplit = w.Bqsplit,
                             SplitDclid = w.SplitDclid,
                             OperationCode = w.OperationCode,
                             ScanOpMode = w.ScanOpMode,
                             Bccheck = w.Bccheck,
                             WorkCenter = w.WorkCenter,
                             Wfdepstatus = w.Wfdepstatus,
                             RecStatus = w.RecStatus,
                             POCounterEnable = w.POCounterEnable,
                             POCounterNumber = w.POCounterNumber,
                             RejectReasonSelectMode = w.RejectReasonSelectMode,
                             ValidateSheduleChange = w.ValidateSheduleChange
                         }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return Wfdep;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("GetWFConfigurationbyWFOpp")]
        public Wfdep GetWFConfigurationbyWFOpp(uint WFId, int OperationCode, int TeamId)
        {
            logger.InfoFormat("GetWFConfigurationbyWFOpp API called with WFId={0}, OperationCode={1}, TeamId={2} ", WFId, OperationCode, TeamId);
            Wfdep Wfdep = null;

            try
            {
                Wfdep = (from w in dcap.Wfdep
                         where w.Wfid == WFId && w.OperationCode == OperationCode && w.TeamId == TeamId
                         select new Wfdep
                         {
                             WfdepinstId = w.WfdepinstId,
                             Wfid = w.Wfid,
                             Depid = w.Depid,
                             TeamId = w.TeamId,
                             L1id = w.L1id,
                             Dclid = w.Dclid,
                             Dcmid = w.Dcmid,
                             InheritDepid = w.InheritDepid,
                             NoOfMulQty = w.NoOfMulQty,
                             LimtWithPredecessor = w.LimtWithPredecessor,
                             ExOpCode = w.ExOpCode,
                             PredDepid = w.PredDepid,
                             LimitWithPredDclid = w.LimitWithPredDclid,
                             LimitWithPredScrapDclid = w.LimitWithPredScrapDclid,
                             LimitWithWf = w.LimitWithWf,
                             LimitWithLevel = w.LimitWithLevel,
                             LimitDclid = w.LimitDclid,
                             Bqsplit = w.Bqsplit,
                             SplitDclid = w.SplitDclid,
                             OperationCode = w.OperationCode,
                             ScanOpMode = w.ScanOpMode,
                             Bccheck = w.Bccheck,
                             WorkCenter = w.WorkCenter,
                             Wfdepstatus = w.Wfdepstatus,
                             RecStatus = w.RecStatus,
                             POCounterEnable = w.POCounterEnable,
                             ValidateSheduleChange = w.ValidateSheduleChange
                         }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetWFConfigurationbyWFOpp {0}", e.ToString());
                throw e;
            }
            return Wfdep;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("GetConfigurationWFDEPInstId")]
        public Wfdep GetConfigurationWFDEPInstId(int WFDEPInstId)
        {
            logger.InfoFormat("GetConfigurationWFDEPInstId API called with WFDEPInstId={0}", WFDEPInstId);
            Wfdep Wfdep = null;

            try
            {
                Wfdep = (from w in dcap.Wfdep
                         where w.WfdepinstId == WFDEPInstId  // && w.OperationCode == OperationCode && w.TeamId == TeamId
                         select new Wfdep
                         {
                             WfdepinstId = w.WfdepinstId,
                             Wfid = w.Wfid,
                             Depid = w.Depid,
                             TeamId = w.TeamId,
                             L1id = w.L1id,
                             Dclid = w.Dclid,
                             Dcmid = w.Dcmid,
                             InheritDepid = w.InheritDepid,
                             NoOfMulQty = w.NoOfMulQty,
                             LimtWithPredecessor = w.LimtWithPredecessor,
                             ExOpCode = w.ExOpCode,
                             PredDepid = w.PredDepid,
                             LimitWithPredDclid = w.LimitWithPredDclid,
                             LimitWithPredScrapDclid = w.LimitWithPredScrapDclid,
                             LimitWithWf = w.LimitWithWf,
                             LimitWithLevel = w.LimitWithLevel,
                             LimitDclid = w.LimitDclid,
                             Bqsplit = w.Bqsplit,
                             SplitDclid = w.SplitDclid,
                             OperationCode = w.OperationCode,
                             ScanOpMode = w.ScanOpMode,
                             ScanCounter = w.ScanCounter,
                             Bccheck = w.Bccheck,
                             WorkCenter = w.WorkCenter,
                             Wfdepstatus = w.Wfdepstatus,
                             RecStatus = w.RecStatus,
                             POCounterEnable = w.POCounterEnable,
                             POCounterNumber = w.POCounterNumber,
                             OppValidationQty = w.OppValidationQty,
                             RejectReasonSelectMode = w.RejectReasonSelectMode,
                             ValidateSheduleChange = w.ValidateSheduleChange
                         }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetConfigurationWFDEPInstId {0}", e.ToString());
                throw e;
            }
            return Wfdep;
        }

        //Get production hours by Team ID
        [Produces("application/json")]
        [HttpGet("GetProductionHoursByTeamID")]
        public IList<Prodhour> GetProductionHoursByTeamID(uint TeamId)
        {
            logger.InfoFormat("GetProductionHoursByTeamID API called with TeamID={0}", TeamId);
            IList<Prodhour> prodhour = null;

            try
            {
                prodhour = (from p in dcap.Prodhour
                            where p.TeamId == TeamId
                            select new Prodhour
                            {
                                TeamId = p.TeamId,
                                ShiftId = p.ShiftId,
                                SeqId = p.SeqId,
                                HourNo = p.HourNo,
                                HourName = p.HourName,
                                Stime = p.Stime,
                                Etime = p.Etime,
                                EffMinuttes = p.EffMinuttes,
                                RecStatus = p.RecStatus
                            }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetProductionHoursByTeamID information {0}", e.ToString());
                throw e;
            }
            return prodhour;
        }


        //Get list of sizes by stylee schedule color
        [Produces("application/json")]
        [HttpGet("GetSizesByStyleSheduleColor")]
        public IList<L5> GetSizesByStyleSheduleColor(int L1id, int L2id, int L3id, int L4id)
        {
            logger.InfoFormat("GetSizesByStyleSheduleColor API called with Style={0}, Schedule={1}, PO?={2}, Color={3}", L1id, L2id, L3id, L4id);
            IList<L5> SizeLst = null;

            try
            {
                SizeLst = (from l in dcap.L5
                           where l.L1id == L1id && l.L2id == L2id && l.L3id == L3id && l.L4id == L4id && l.L5status == 1 && l.RecStatus == (int)eRecStatus.Active
                           select new L5
                           {
                               L5id = l.L5id,
                               L5no = l.L5no
                           }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetSizesByStyleSheduleColor information {0}", e.ToString());
                throw e;
            }
            return SizeLst;
        }

        //Get Reported Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetReportedQtyByOperation")]
        public L5moops GetReportedQtyByOperation(string SBUCode, string FacCode, int OperationCode, uint L1id, uint L2id, uint L3id, uint L4id, uint L5id)
        {
            logger.InfoFormat("GetReportedQtyByOperation API called with SBUCode={0}, FacCode={1}, OperationCode={2}, L1Id={3} , L2Id={4} , L3Id={5} , L4Id={6} , L5Id={7}", SBUCode, FacCode, OperationCode, L1id, L2id, L3id, L4id, L5id);
            L5moops L5moop = new L5moops();

            try
            {
                var xL5moop = (from l in dcap.L5moops
                               where l.GroupCode == "Brandix" && l.Sbucode == SBUCode && l.FacCode == FacCode
                               && l.OperationCode == OperationCode && l.L1id == L1id && l.L2id == L2id
                               && l.L3id == L3id && l.L4id == L4id && l.L5id == L5id && l.L5moopsStatus < 90 && l.RecStatus == (int)eRecStatus.Active
                               select new
                               {
                                   OrderQty = l.OrderQty,
                                   ReportedQty = l.ReportedQty,
                                   ScrappedQty = l.ScrappedQty,
                                   TxnDateTimeMax = l.TxnDateTimeMax
                               }).ToList();

                // it was written to get sum by Grouping the recored send and for some reason it did not wark, had to put a loop to meet the time lines 
                //Nalinda
                //01/03/2019

                foreach (var itm in xL5moop)
                {
                    L5moop.OrderQty = (int)L5moop.OrderQty + (int)itm.OrderQty;
                    L5moop.ReportedQty = (int)L5moop.ReportedQty + (int)itm.ReportedQty;
                    L5moop.ScrappedQty = (int)L5moop.ScrappedQty + (int)itm.ScrappedQty;
                    L5moop.TxnDateTimeMax = itm.TxnDateTimeMax;
                }

                //   group l by new { l.Sbucode, l, FacCode, l.OperationCode, l.L1id, l.L2id, l.L3id, l.L4id, l.L5id }
                //    into grp
                //   select new L5moops
                //   {
                //       // L5moid = grp.L5moid,
                //       // L5mono = grp.L5mono,
                //       // WorkCenter = grp.WorkCenter,
                //       OrderQty = grp.Sum(d => d.OrderQty),
                //       ReportedQty = grp.Sum(d => d.ReportedQty),
                //       ScrappedQty = grp.Sum(d => d.ScrappedQty),
                //       TxnDateTimeMax = grp.Max(d => d.TxnDateTimeMax)
                //       //, L5moopsStatus = grp.L5moopsStatus
                //   }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetReportedQtyByOperation information {0}", e.ToString());
                throw e;
            }
            return L5moop;
        }

        //Get Reported Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetReportedQtyByMONO")]
        public L5moops GetReportedQtyByMONO(string SBUCode, string FacCode, int OperationCode, uint L1id, uint L2id, uint L3id, uint L4id, uint L5id, string MONO)
        {
            logger.InfoFormat("GetReportedQtyByMONO API called with SBUCode={0}, FacCode={1}, OperationCode={2}, L1Id={3} , L2Id={4} , L3Id={5} , L4Id={6} , L5Id={7} , MONO={8}", SBUCode, FacCode, OperationCode, L1id, L2id, L3id, L4id, L5id, MONO);
            L5moops L5moop = new L5moops();

            try
            {
                var XL5moop = (from l in dcap.L5moops
                               where l.GroupCode == "Brandix" && l.Sbucode == SBUCode && l.FacCode == FacCode
                               && l.OperationCode == OperationCode && l.L1id == L1id && l.L2id == L2id
                               && l.L3id == L3id && l.L4id == L4id && l.L5id == L5id && l.L5moopsStatus
                               < 90 && l.RecStatus == (int)eRecStatus.Active && l.L5mono == MONO
                               select l
                                        ).ToList();
                // it was written to get sum by Grouping the recored send and for some reason it did not wark, had to put a loop to meet the time lines 
                //Nalinda
                //01/03/2019
                foreach (var itm in XL5moop)
                {
                    L5moop.OrderQty = (int)L5moop.OrderQty + (int)itm.OrderQty;
                    L5moop.ReportedQty = (int)L5moop.ReportedQty + (int)itm.ReportedQty;
                    L5moop.ScrappedQty = (int)L5moop.ScrappedQty + (int)itm.ScrappedQty;
                }


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetReportedQtyByMONO information {0}", e.ToString());
                throw e;
            }
            return L5moop;
        }

        //Get MO Details Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetMODetails")]
        public List<L5mo> GetMODetails(uint L1id, uint L2id, uint L3id, uint L4id, uint L5id)
        {
            logger.InfoFormat("GetMODetails API called with L1id={0}, L2id={1}, L3id={2}, L4id={3} , L5id={4} ", L1id, L2id, L3id, L4id, L5id);
            List<L5mo> L5mo = null;

            try
            {
                L5mo = (from l in dcap.L5mo
                        where l.L1id == L1id && l.L2id == L2id
                        && l.L3id == L3id && l.L4id == L4id && l.L5id == L5id && l.L5mostatus < 90 && l.RecStatus == (int)eRecStatus.Active
                        select new L5mo
                        {
                            L5moid = l.L5moid,
                            L5mono = l.L5mono,
                            QtyMax = l.QtyMax

                        }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return L5mo;
        }


        //Get MO Details Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetReportedQtyforMO")]
        public Detxn GetReportedQtyforMO(uint WfdepinstId, string L5mono)
        {
            logger.InfoFormat("GetReportedQtyforMO API called with WfdepinstId={0}, L5mono={1}", WfdepinstId, L5mono);
            Detxn detx = null;

            try
            {
                detx = (from l in dcap.Detxn
                        where l.WfdepinstId == WfdepinstId && l.L5mono == L5mono
                        group l by new { l.WfdepinstId, l.L5mono }
                        into grp
                        orderby grp.Key.L5mono ascending
                        select new Detxn
                        {
                            Qty01 = grp.Sum(l => l.Qty01),
                            Qty02 = grp.Sum(l => l.Qty02),
                            Qty03 = grp.Sum(l => l.Qty03)

                        }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetReportedQtyforMO information {0}", e.ToString());
                throw e;
            }
            return detx;
        }

        //Get MO Details Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetReportedQtyforMO")]
        public Detxn GetReportedQtyforMOBC(uint WfdepinstId, string L5mono, string Barcode)
        {
            logger.InfoFormat("GetReportedQtyforMOBC API called with WfdepinstId={0}, L5mono={1}", WfdepinstId, L5mono);
            Detxn detx = null;

            try
            {
                detx = (from l in dcap.Detxn
                        where l.WfdepinstId == WfdepinstId && l.L5mono == L5mono && l.BarCodeNo == Barcode
                        group l by new { l.WfdepinstId, l.L5mono }
                        into grp
                        orderby grp.Key.L5mono ascending
                        select new Detxn
                        {
                            Qty01 = grp.Sum(l => l.Qty01),
                            Qty02 = grp.Sum(l => l.Qty02),
                            Qty03 = grp.Sum(l => l.Qty03)

                        }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return detx;
        }

        //Get MO Details Qty By Operation
        [Produces("application/json")]
        [HttpGet("GetMaxQtyforMO")]
        public Detxn GetMaxQtyforMO(uint Depid, string L5mono, uint L1Id, uint L2Id) //string SBUCode, string FacCode, int OperationCode, uint L1id, uint L2id, uint L3id, uint L4id, uint L5id, string MONO)
        {

            logger.InfoFormat("GetMaxQtyforMO API called with Depid={0}, L5mono={1}, L1Id={2}, L2Id={3}", Depid, L5mono, L1Id, L2Id);
            Detxn txn = null;
            Dep dep = null;
            L5moops moops = null;

            try
            {

                txn = (from m in dcap.L5moops
                       where m.OperationCode == 15 && m.L5mono == L5mono
                       select new Detxn
                       {
                           Qty01 = m.ReportedQty,
                           Qty02 = m.ScrappedQty
                       }).FirstOrDefault();

                // dep =  r in dcap.Dedep.Where(d => d.Depid < Depid && d.L1id == L1Id && d.L2id == L2Id).OrderByDescending(d => d.Depid).Take(1)
                // if(dep != null)
                // {
                //     Detxn = (from d in dcap.Detxn
                //                         where d.HourNo == HourNo && d.TeamId == TeamId && d.TxnDateTime.Date == TxnDate.Date
                //                         // && d.OperationCode == OppCode
                //                         group d by new  { d.WfdepinstId, d.L5mono }
                //                         into grp
                //                         select new Detxn
                //                         {
                //                             Qty01 = grp.Sum(d => d.Qty01),
                //                             Qty02 = grp.Sum(d => d.Qty02),
                //                             Qty03 = grp.Sum(d => d.Qty03),
                //                         }
                //                 ).FirstOrDefault();

                // }
                // txn = (from l in dcap.Detxn
                //        join r in dcap.Dedep.Where(d => d.Depid < Depid && d.L1id == L1Id && d.L2id == L2Id).OrderByDescending(d => d.Depid).Take(1)
                //        on new { A = l.Depid, B = l.L5mono } equals new { A = r.Depid, B = L5mono }
                //        group l by new { l.WfdepinstId, l.L5mono }
                //             into grp
                //        orderby grp.Key.L5mono ascending
                //        select new Detxn
                //        {
                //            Qty01 = grp.Sum(l => l.Qty01),
                //            Qty02 = grp.Sum(l => l.Qty02),
                //            Qty03 = grp.Sum(l => l.Qty03)

                //        }).ToList().FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetMaxQtyforMO information {0}", e.ToString());
                throw e;
            }
            return txn;
        }


        //Get Qty By DEPId
        [Produces("application/json")]
        [HttpGet("GetQtyByDEPId")]
        public Dedep GetQtyByDEPId(uint DEPId, uint WFId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id)
        {
            logger.InfoFormat("GetQtyByDEPId API called with DEPId={0}, WFId={1}, L1Id={2}, L2Id={3} , L3Id={4} , L4Id={5} , L5Id={6}", DEPId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id);
            Dedep Dedep = null;

            try
            {
                Dedep = (from d in dcap.Dedep
                         where d.Depid == DEPId && d.L1id == L1Id
                         && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.Wfid == WFId
                         group d by new { d.Depid, d.Wfid, d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                          into grp
                         select new Dedep
                         {
                             Qty01 = grp.Sum(d => d.Qty01),
                             Qty02 = grp.Sum(d => d.Qty02),
                             Qty03 = grp.Sum(d => d.Qty03)
                         }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetQtyByDEPId information {0}", e.ToString());
                throw e;
            }

            return Dedep;
        }

        //Get Qty By DEPId
        [Produces("application/json")]
        [HttpGet("GetDEDEPQtyforMO")]
        public IList<Dedep> GetDEDEPQtyforMO(uint DEPId, uint WFId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id, uint L5MOId)
        {
            logger.InfoFormat("GetDEDEPQtyforMO API called with DEPId={0}, WFId={1}, L1Id={2}, L2Id={3} , L3Id={4} , L4Id={5} , L5Id={6}, L5MOId={7}", DEPId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id, L5MOId);
            IList<Dedep> Dedep = null;

            try
            {
                Dedep = (from d in dcap.Dedep
                         where d.Depid == DEPId && d.Wfid == WFId && d.L1id == L1Id
                         && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.L5moid == L5MOId
                         select new Dedep
                         {
                             Qty01 = d.Qty01,
                             Qty02 = d.Qty02,
                             Qty03 = d.Qty03
                         }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetDEDEPQtyforMO information {0}", e.ToString());
                throw e;
            }
            return Dedep;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("GetQtyByDEPInstId")]
        public Dedepinst GetQtyByDEPInstId(int GetDEPInsDataFor, uint WFDEPInstId, uint WFId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id)
        {
            logger.InfoFormat("GetQtyByDEPInstId API called with GetDEPInsDataFor={0}, WFDEPInstId={1}, WFId={2}, L1Id={3}, L2Id={4} , L3Id={5} , L4Id={6} , L5Id={7}", GetDEPInsDataFor, WFDEPInstId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id);
            Dedepinst Dedepinst = null;
            //  Dedepinst Dedepinst = null;

            try
            {
                /* 
                Dedepinst = (from d in dcap.Dedepinst
                             where d.WfdepinstId == WFDEPInstId && d.Wfid == WFId && d.L1id == L1Id
                               && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id
                             group d by new { d.WfdepinstId, d.Wfid, d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                               into grp
                             select new Dedepinst
                             {

                                 Qty01 = grp.Sum(d => d.Qty01),
                                 Qty02 = grp.Sum(d => d.Qty02),
                                 Qty03 = grp.Sum(d => d.Qty03)
                             }).FirstOrDefault();
                */
                if (GetDEPInsDataFor == (int)eGetDEPInsDataFor.Style)
                {
                    Dedepinst = (from d in dcap.Dedepinst
                                 where d.WfdepinstId == WFDEPInstId && d.Wfid == WFId && d.L1id == L1Id
                                 group d by new { d.WfdepinstId, d.Wfid, d.L1id }
                                               into grp
                                 select new Dedepinst
                                 {
                                     Qty01 = grp.Sum(d => d.Qty01),
                                     Qty02 = grp.Sum(d => d.Qty02),
                                     Qty03 = grp.Sum(d => d.Qty03)
                                 }).FirstOrDefault();
                }
                if (GetDEPInsDataFor == (int)eGetDEPInsDataFor.Size)
                {
                    Dedepinst = (from d in dcap.Dedepinst
                                 where d.WfdepinstId == WFDEPInstId && d.Wfid == WFId && d.L1id == L1Id
                                   && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id// 
                                 group d by new { d.WfdepinstId, d.Wfid, d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                                  into grp
                                 select new Dedepinst
                                 {
                                     Qty01 = grp.Sum(d => d.Qty01),
                                     Qty02 = grp.Sum(d => d.Qty02),
                                     Qty03 = grp.Sum(d => d.Qty03)
                                 }).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }

            return Dedepinst;


        }

        //Get MO By Style Schedule Color
        [Produces("application/json")]
        [HttpGet("GetMOByStyleScheduleColor")]
        public IList<L5mo> GetMOByStyleScheduleColor(int L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id)
        {
            logger.InfoFormat("GetMOByStyleScheduleColor API called with L1Id={0}, L2Id={1} , L3Id={2} , L4Id={3} , L5Id={4}", L1Id, L2Id, L3Id, L4Id, L5Id);
            IList<L5mo> L5Mo = null;

            try
            {
                L5Mo = (from d in dcap.L5mo
                        where d.L1id == L1Id && d.L2id == L2Id && d.L3id == L3Id
                          && d.L4id == L4Id && d.L5id == L5Id && d.L4id == L4Id
                          && d.L5mostatus < 90 && d.RecStatus == (int)eRecStatus.Active
                        select new L5mo
                        {
                            L5moid = d.L5moid,
                            L5mono = d.L5mono
                        }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return L5Mo;
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
                         join lbc in dcap.L5bc.Where(c => c.BarCodeNo == Barcode).AsQueryable() on new { A = l2.L1id, B = l2.L2id } equals new { A = lbc.L1id, B = lbc.L2id } //
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
                              where l4.L1id == Color.StyleId && l4.L2id == Color.ScheduleId && l4.L3id == 0 && l4.L4id == Color.ColorId
                              select new L4
                              {
                                  L4no = l4.L4no,
                                  L4desc = l4.L4desc
                              }).FirstOrDefault();

                        if (L4 != null)
                        {
                            Color.ColorNo = L4.L4no;
                            Color.ColorDesc = L4.L4desc;
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

        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStyleScheduleByBarcodeforBarcodeDetail")]
        public StyleScheduleColor GetStyleScheduleByBarcodeforBarcodeDetail(string Barcode)
        {

            logger.InfoFormat("GetColorsByStylesNo By Barcode Barcode ={0}", Barcode);
            StyleScheduleColor Color = null;
            L4 L4 = null;

            try
            {
                List<L5bc> l5b = dcap.L5bc.Where(c => c.BarCodeNo == Barcode).ToList();
                if (l5b.Count == 0)
                {
                    l5b = dcap.Detxn.Where(c => c.BarCodeNo == Barcode).GroupBy(c => new { c.L1id, c.L2id, c.L3id, c.L4id, c.L5id, c.BarCodeNo }).Select(c => new L5bc { L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L3id = (uint)c.Key.L3id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BarCodeNo = c.Key.BarCodeNo }).ToList();
                }
                Color = (from l1 in dcap.L1
                         join l2 in dcap.L2 on l1.L1id equals l2.L1id
                         join lbc in l5b.Where(c => c.BarCodeNo == Barcode).AsQueryable() on new { A = l2.L1id, B = l2.L2id } equals new { A = lbc.L1id, B = lbc.L2id } //
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
                              where l4.L1id == Color.StyleId && l4.L2id == Color.ScheduleId && l4.L3id == 0 && l4.L4id == Color.ColorId
                              select new L4
                              {
                                  L4no = l4.L4no,
                                  L4desc = l4.L4desc
                              }).FirstOrDefault();

                        if (L4 != null)
                        {
                            Color.ColorNo = L4.L4no;
                            Color.ColorDesc = L4.L4desc;
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

        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetL5BCData")]
        public L5bc GetL5BCData(string Barcode)
        {
            logger.InfoFormat("Get GetL5BCData API called with Barcode ={0}", Barcode);
            L5bc L5bc = null;

            try
            {
                L5bc = (from l5b in dcap.L5bc
                        where l5b.BarCodeNo == Barcode && l5b.RecStatus == (int)eRecStatus.Active
                        select new L5bc
                        {
                            L1id = l5b.L1id,
                            L2id = l5b.L2id,
                            L3id = l5b.L3id,
                            L4id = l5b.L4id,
                            L5id = l5b.L5id,
                            QtyMax = l5b.QtyMax,
                            L5bcstatus = l5b.L5bcstatus,
                            L5bcisUsed = l5b.L5bcisUsed
                        }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetL5BCData information {0}", e.ToString());
                throw e;
            }
            return L5bc;
        }

        //Get Color By Style Schedule PO
        [Produces("application/json")]
        [HttpGet("GetColorByStyleSchedulePO")]
        public IList<L4> GetColorByStyleSchedulePO(uint L1Id, uint L2Id, uint L3Id, uint L4Id)
        {

            logger.InfoFormat("GetColorByStyleSchedulePO API called with Style ={0} , Schedule ={1} , PO ={2} ,Color ={3}", L1Id, L2Id, L3Id, L4Id);
            IList<L4> Colors = null;

            try
            {
                Colors = (from l4 in dcap.L4
                          where l4.L1id == L1Id && l4.L2id == L2Id
                          && l4.L3id == L3Id && l4.L4id == L4Id
                          && l4.RecStatus == (int)eRecStatus.Active
                          && l4.L4status == 1
                          select new L4 { L1id = l4.L1id, L2id = l4.L2id, L3id = l4.L3id, L4id = l4.L4id, L4no = l4.L4no, L4desc = l4.L4desc }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorByStyleSchedulePO information {0}", e.ToString());
                throw e;
            }
            return Colors;
        }

        //GET api /shedule/GetScheduleByID - API to get Schedule By ID Information
        [Produces("application/json")]
        [HttpGet("GetRejectReasons")]
        public IList<Rejectreason> GetRejectReasons(string RRCode)
        {
            RRCode = RRCode == null ? "%" : RRCode;
            logger.InfoFormat("GetRejectReasons API called with RRCode = {0}", RRCode);
            IList<Rejectreason> RRList = null;

            try
            {
                RRList = (from r in dcap.Rejectreason
                          where r.RecStatus == (int)eRecStatus.Active
                          && EF.Functions.Like(r.Scode.ToUpper(), "%" + RRCode.ToUpper() + "%")
                          && r.RrcatId == "1"
                          orderby r.Scode
                          select new Rejectreason
                          {
                              Rrid = r.Rrid,
                              Rrname = r.Rrname,
                              Rrdesc = r.Rrdesc,
                              RrnameSin = r.RrnameSin,
                              RrdescSin = r.RrdescSin,
                              RrnameTem = r.RrnameTem,
                              RrdescTem = r.RrdescTem,
                              Scode = r.Scode
                          })
                         .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetRejectReasons information {0}", e.ToString());
                throw e;
            }
            return RRList;
        }

        [Produces("application/json")]
        [HttpGet("GetRejectReasonsBFL")]
        public IList<Rejectreason> GetRejectReasonsBFL(string RRCode)
        {
            RRCode = RRCode == null ? "%" : RRCode;
            logger.InfoFormat("GetRejectReasons BFL API called with RRCode = {0}", RRCode);
            IList<Rejectreason> RRList = null;

            try
            {
                RRList = (from r in dcap.Rejectreason
                          where r.RecStatus == (int)eRecStatus.Active && r.Rrtype == 3
                          && EF.Functions.Like(r.Scode.ToUpper(), "%" + RRCode.ToUpper() + "%")
                          orderby r.Scode
                          select new Rejectreason
                          {
                              Rrid = r.Rrid,
                              Rrname = r.Rrname,
                              Rrdesc = r.Rrdesc,
                              RrnameSin = r.RrnameSin,
                              RrdescSin = r.RrdescSin,
                              RrnameTem = r.RrnameTem,
                              RrdescTem = r.RrdescTem,
                              Scode = r.Scode
                          })
                         .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetRejectReasons information {0}", e.ToString());
                throw e;
            }
            return RRList;
        }

        // //GET api /shedule/GetScheduleByID - API to get Schedule By ID Information
        // [Produces("application/json")]
        // [HttpGet("GetNextOpperation")]
        // public IList<Rejectreason> GetNextOpperation()
        // {
        //     logger.InfoFormat("Get Next Opperation..");
        //     IList<Rejectreason> RRList = null;
        //     try
        //     {
        //         RRList = (from r in dcap.Rejectreason
        //                   where r.RecStatus == (int)eRecStatus.Active
        //                   orderby r.Rrname ascending
        //                   select new Rejectreason
        //                   {
        //                       Rrid = r.Rrid,
        //                       Rrname = r.Rrname,
        //                       Rrdesc = r.Rrname,
        //                       RrnameSin = r.RrnameSin,
        //                       RrdescSin = r.RrdescSin,
        //                       RrnameTem = r.RrnameTem,
        //                       RrdescTem = r.RrdescTem,
        //                        Scode = r.Scode
        //                   })
        //                  .ToList();
        //     }
        //     catch (Exception e)
        //     {
        //         logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
        //         throw e;
        //     }
        //     return RRList;
        // }

        //GET api /shedule/GetScheduleByID - API to get Schedule By ID Information

        [Produces("application/json")]
        [HttpGet("GetAdjacentNodesForGivenNode")]
        public List<Wfdep> GetAdjacentNodesForGivenNode(int WfdepinstId, int Previous)
        {
            logger.InfoFormat("GetAdjacentNodesForGivenNode API called with WfdepinstId= {0}, Previous = {1}", WfdepinstId, Previous);
            List<Wfdep> DepLst = null;

            try
            {
                string strSql = "";

                if (Previous == 2)
                {
                    strSql = " SELECT w.*  FROM wfdeplink l INNER JOIN  wfdep w ON l.WFDEPInstId = w.WFDEPInstId "
                               + " WHERE l.WFDEPIdLink =  " + WfdepinstId.ToString() + " AND l.RecStatus = 1 ";

                }
                else
                {
                    strSql = " SELECT w.*  FROM wfdeplink l INNER JOIN  wfdep w ON l.WFDEPIdLink = w.WFDEPInstId "
                             + " WHERE l.WFDEPInstId =  " + WfdepinstId.ToString() + "   AND l.RecStatus = 1 ";


                }

                DepLst = dcap.Wfdep
                                 .FromSql(strSql)
                                 .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetAdjacentNodesForGivenNode information {0}", e.ToString());
                throw e;
            }
            return DepLst;
        }



        //GET api /shedule/GetScheduleByID - API to get Schedule By ID Information
        [Produces("application/json")]
        [HttpGet("GetDETxnNextSeqNo")]
        public int GetDETxnNextSeqNo(UserInput ui)
        {
            //log4net.GlobalContext.Properties["pid"] = Process.GetCurrentProcess().Id;               
            logger.InfoFormat("GetDETxnNextSeqNo API Called with WfdepinstId={0}", ui.WfdepinstId);
            uint RetVal = 0;
            string strSql = "";

            IList<Detxn> Det = null;
            Detxn Det2 = null;
            Wfdep Wfdep1 = new Wfdep();

            try
            {
                //  strSql = "SELECT * FROM detxn WHERE  WFDEPInstId =" + WfdepinstId.ToString() + " order by Seq desc limit 1";
                //  Det2 = dcap.Detxn.FromSql(strSql).ToList();              

                //    Det = (from d in dcap.Detxn
                //               where d.WfdepinstId  == WfdepinstId 
                //               group d by new { d.WfdepinstId }
                //               into grp
                //               select new Detxn
                //                  {
                //                  Seq = grp.Max(d => d.Seq)
                //                }).ToList(); 


                //Det2 = (from d in dcap.Detxn
                //        where d.WfdepinstId == WfdepinstId
                //        orderby d.Seq descending
                //        select new Detxn
                //        {
                //           Seq = d.Seq
                //       }).FirstOrDefault();
                //RetVal = Det2 == null ? 0 : Det2.Seq; 

                // Wfdep1 = (from d in dcap.Wfdep
                //        where d.WfdepinstId == ui.WfdepinstId  && d.RecStatus == (int)eRecStatus.Active                       
                //         select new Wfdep
                //         {
                //            NextSeqNo = d.NextSeqNo
                //        }).FirstOrDefault();




                Wfdep1 = null;
                Wfdep1 = dcap.Wfdep
                         .Where(c => c.WfdepinstId == ui.WfdepinstId && c.RecStatus == (int)eRecStatus.Active)
                         .FirstOrDefault();

                RetVal = Wfdep1 == null ? 0 : (uint)Wfdep1.NextSeqNo;

                Wfdep1.NextSeqNo = (int)(RetVal + 1);
                Wfdep1.ModifiedDateTime = DateTime.Now;
                Wfdep1.ModifiedBy = ui.CreatedBy;
                Wfdep1.ModifiedMachine = ui.CreatedMachine;
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.ErrorFormat("GetDETxnNextSeqNo is Successful.");
            }
            return (int)RetVal;
        }


        //Get next DEDEP sequance no
        [Produces("application/json")]
        [HttpGet("GetDEDEPNextSeqNo")]
        public int GetDEDEPNextSeqNo(uint WFID, uint DEPID)
        {
            logger.InfoFormat("GetDEDEPNextSeqNo API called with WFID={0}, DEPID={1} ", WFID, DEPID);
            int RetVal = 0;
            IList<Dedep> lstDedep = null;
            try
            {
                /*lstDedep = (from d in dcap.Dedep
                            where d.Wfid == WFID && d.Depid == DEPID
                            select new Dedep{
                                Seq = d.Seq
                            }).ToList();*/

                //RetVal = lstDedep.Count == 0 ? 0 : lstDedep.Max(x => x.Seq);
                RetVal = (dcap.Dedep.Where(d => d.Wfid == WFID && d.Depid == DEPID).Max(x => (int?)x.Seq)) ?? 0;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetDEDEPNextSeqNo information {0}", e.ToString());
                throw e;
            }
            return RetVal + 1;
        }

        //Get next Good Control sequance no
        [Produces("application/json")]
        [HttpGet("GetGoodControlNextSeqNo")]
        public int GetGoodControlNextSeqNo(uint WFID, uint DEPID)
        {
            logger.InfoFormat("GetGoodControlNextSeqNo API called with WFID={0}, DEPID={1} ", WFID, DEPID);
            int RetVal = 0;
            IList<GoodControl> lstGoodControl = null;
            try
            {
                lstGoodControl = (from d in dcap.GoodControl
                                  select d).ToList();

                RetVal = lstGoodControl.Count == 0 ? 0 : (int)lstGoodControl.Max(x => x.Seq);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetGoodControlNextSeqNo information {0}", e.ToString());
                throw e;
            }
            return RetVal + 1;
        }

        //Get next Good Control detail sequance no
        [Produces("application/json")]
        [HttpGet("GetGoodControlDetailsNextSeqNo")]
        public int GetGoodControlDetailsNextSeqNo(uint WFID, uint DEPID)
        {
            logger.InfoFormat("GetGoodControlDetailsNextSeqNo API called with WFID={0}, DEPID={1} ", WFID, DEPID);
            int RetVal = 0;
            IList<GoodControlDetails> lstGoodControl = null;
            try
            {
                lstGoodControl = (from d in dcap.GoodControlDetails
                                  where d.WFId == WFID && d.Depid == DEPID
                                  select d).ToList();

                RetVal = lstGoodControl.Count == 0 ? 0 : (int)lstGoodControl.Max(x => x.Seq);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetGoodControlNextSeqNo information {0}", e.ToString());
                throw e;
            }
            return RetVal + 1;
        }

        //Get next DEDEP sequance no
        [Produces("application/json")]
        [HttpGet("GetDEDEPInstNextSeqNo")]
        public int GetDEDEPInstNextSeqNo(uint WFDEPInstId)
        {
            logger.InfoFormat("GetDEDEPNextSeqNo API called with WFDEPInstId={0}", WFDEPInstId);
            uint RetVal = 0;
            Dedepinst lstDedepinst = null;
            try
            {

                //Det2 = (from d in dcap.Detxn
                //        where d.WfdepinstId == WfdepinstId
                //        orderby d.Seq descending
                //        select new Detxn
                //        {
                //           Seq = d.Seq
                //       }).FirstOrDefault();
                //RetVal = Det2 == null ? 0 : Det2.Seq; 



                lstDedepinst = (from d in dcap.Dedepinst
                                where d.WfdepinstId == WFDEPInstId
                                orderby d.Seq descending
                                select new Dedepinst
                                {
                                    Seq = d.Seq
                                }).FirstOrDefault();

                if (lstDedepinst != null)
                {
                    RetVal = lstDedepinst.Seq;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetDEDEPInstNextSeqNo information {0}", e.ToString());
                throw e;
            }
            return (int)RetVal + 1;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("CheckDEDEPInstRecExists")]
        public Dedepinst CheckDEDEPInstRecExists(uint WFDEPInstId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id, uint L5MOId)
        {
            logger.InfoFormat("CheckDEDEPInstRecExists API called with WFDEPInstId={0}, L1Id={1}, L2Id={2} , L3Id={3} , L4Id={4} , L5Id={5}, L5MOId={6}", WFDEPInstId, L1Id, L2Id, L3Id, L4Id, L5Id, L5MOId);
            Dedepinst Dedepinst = null;

            try
            {
                Dedepinst = (from d in dcap.Dedepinst
                             where d.WfdepinstId == WFDEPInstId && d.L1id == L1Id && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.L5moid == L5MOId
                             select new Dedepinst
                             {
                                 Qty01 = d.Qty01,
                                 Qty02 = d.Qty02,
                                 Qty03 = d.Qty03,
                             }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckDEDEPInstRecExists information {0}", e.ToString());
                throw e;
            }
            return Dedepinst;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("CheckDEDEPRecExists")]
        public Dedep CheckDEDEPRecExists(uint WFId, uint DEPId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id, uint L5MOId)
        {
            logger.InfoFormat("CheckDEDEPRecExists API called with WFId={0}, DEPId={1}, L1Id={2}, L2Id={3} , L3Id={4} , L4Id={5} , L5Id={6}", DEPId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id, L5MOId);
            Dedep Dedep = null;

            try
            {
                Dedep = (from d in dcap.Dedep
                         where d.Depid == DEPId && d.L1id == L1Id && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.L5moid == L5MOId && d.Wfid == WFId
                         select new Dedep
                         {
                             Qty01 = d.Qty01,
                             Qty02 = d.Qty02,
                             Qty03 = d.Qty03,
                         }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckDEDEPRecExists information {0}", e.ToString());
                throw e;
            }
            return Dedep;
        }

        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("GetDETxn")]
        public Detxn GetDETxnOppQtybyBarcode(string BarcodeNo, uint DEPID)
        {
            logger.InfoFormat("GetDETxnOppQtybyBarcode API called with BarcodeNo={0}, DEPID={1}", BarcodeNo, DEPID);

            Detxn Detxn = null;

            try
            {
                Detxn = (from d in dcap.Detxn
                         where d.BarCodeNo == BarcodeNo && d.Depid == DEPID && d.RecStatus == (int)eRecStatus.Active
                         group d by new { d.BarCodeNo, d.Depid }
                            into grp
                         select new Detxn
                         {
                             Qty01 = grp.Sum(d => d.Qty01),
                             Qty02 = grp.Sum(d => d.Qty02),
                             Qty03 = grp.Sum(d => d.Qty03)
                         }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetDETxnOppQtybyBarcode information {0}", e.ToString());
                throw e;
            }
            return Detxn;
        }

        [Produces("application/json")]
        [HttpGet("GetProdHourByTeamId")]
        public int GetProdHourByTeamId(int TeamId, DateTime txnDtTime, int Offline)
        {
            //dcap.Connection.Timeout = 60;
            logger.InfoFormat("GetProdHourByTeamId API called with TeamId={0}", TeamId);

            Prodhour Prodhour = null;
            int Hour = 0, Minute = 0;

            if (Offline == (int)eOffline.Offline)
            {
                Hour = txnDtTime.Hour;
                Minute = txnDtTime.Minute;
            }
            else
            {
                Hour = System.DateTime.Now.Hour;
                Minute = System.DateTime.Now.Minute;
            }

            string time = Hour.ToString().PadLeft(2, '0') + Minute.ToString().PadLeft(2, '0');
            int HourNo = 0;

            logger.InfoFormat("string time = Hour.ToString(). API called with time={0}", time);

            try
            {
                Prodhour = (from d in dcap.Prodhour
                            where Convert.ToInt32(d.Stime) <= Convert.ToInt32(time) && Convert.ToInt32(d.Etime) >= Convert.ToInt32(time)
                            && d.RecStatus == (int)eRecStatus.Active
                            && d.TeamId == TeamId
                            select new Prodhour
                            {
                                TeamId = d.TeamId,
                                ShiftId = d.ShiftId,
                                SeqId = d.SeqId,
                                HourNo = (uint)d.HourNo,
                                Stime = d.Stime
                            }).FirstOrDefault();

                logger.InfoFormat("Hour No Retrival successful HourNo={0}", Prodhour);

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetProdHourByTeamId information {0}", e.ToString());
                throw e;
            }

            if (Prodhour != null)
            {
                HourNo = (int)Prodhour.HourNo;
            }
            return HourNo;
        }

        [Produces("application/json")]
        [HttpGet("GetDeTxnUploadStatus")]
        public int GetDeTxnUploadStatus(int TeamId, int OperationCode)
        {
            //dcap.Connection.Timeout = 60;
            logger.InfoFormat("GetDeTxnUploadStatus API called with TeamId={0}, OperationCode={1}", TeamId, OperationCode);
            int UploadStatus = 99;
            Teamopp Teamopp = null;
            try
            {
                Teamopp = (from d in dcap.Teamopp
                           where d.TeamId == (uint)TeamId && d.OperationCode == (uint)OperationCode
                           && d.RecStatus == (int)eRecStatus.Active
                           select new Teamopp
                           {
                               UploadToM3 = d.UploadToM3
                           }).FirstOrDefault();

                logger.InfoFormat("Hour No Retrival successful HourNo={0}", Teamopp);

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetDeTxnUploadStatus information {0}", e.ToString());
                throw e;
            }
            if (Teamopp != null)
            {
                UploadStatus = (int)Teamopp.UploadToM3;
            }

            return UploadStatus;
        }

        public HourlyCounts GetCurHourQty(UserInput ui)
        {
            logger.InfoFormat("GetCurHourQty API called with TeamId={0}, OppCode={1}", ui.TeamId, ui.OperationCode);
            HourlyCounts hc = new HourlyCounts();

            Prodhour Prodhour = null;
            int Hour, Minute;

            if (ui.Offline == (int)eOffline.Offline)
            {
                Hour = ui.TxnDate.Hour;
                Minute = ui.TxnDate.Minute;
            }
            else
            {
                Hour = System.DateTime.Now.Hour;
                Minute = System.DateTime.Now.Minute;

            }
            string time = Hour.ToString().PadLeft(2, '0') + Minute.ToString().PadLeft(2, '0');
            int HourNo = 0;
            uint TeamId = ui.TeamId;
            // int OppCode = ui.OperationCode;
            uint depid = ui.Depid;

            try
            {
                Prodhour = (from d in dcap.Prodhour
                            where Convert.ToInt32(d.Stime) <= Convert.ToInt32(time) && Convert.ToInt32(d.Etime) >= Convert.ToInt32(time)
                            && d.RecStatus == (int)eRecStatus.Active
                            && d.TeamId == TeamId
                            select new Prodhour
                            {
                                TeamId = d.TeamId,
                                ShiftId = d.ShiftId,
                                SeqId = d.SeqId,
                                HourNo = (uint)d.HourNo,
                                Stime = d.Stime,
                                Etime = d.Stime
                            }).FirstOrDefault();

                if (Prodhour != null)
                {
                    Detxn Detxn = null;
                    HourNo = (int)Prodhour.HourNo;

                    Detxn = (from d in dcap.Detxn
                             where d.HourNo == HourNo && d.TeamId == TeamId && d.TxnDateTime.Date == System.DateTime.Now.Date
                             // && d.TxnDateTime.Day == System.DateTime.Now.Day
                              && d.Depid == depid
                             group d by new { d.TeamId, d.HourNo }
                            into grp
                             select new Detxn
                             {
                                 Qty01 = grp.Sum(d => d.Qty01),
                                 Qty02 = grp.Sum(d => d.Qty02),
                                 Qty03 = grp.Sum(d => d.Qty03),
                             }
                    ).FirstOrDefault();

                    if (Detxn != null)
                    {
                        hc.CurHrGood = (int)Detxn.Qty01;
                        hc.CurHrScrap = (int)Detxn.Qty02;
                        hc.CurHrRework = (int)Detxn.Qty03;
                        hc.CurHourNo = HourNo;
                    }
                }
                return hc;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetCurHourQty information {0}", e.ToString());
                return hc;
                throw e;
            }
        }

        public HourlyCounts GetPrevHourQty(UserInput ui, int HourNo)
        {
            logger.InfoFormat("GetPrevHourQty API called with TeamId={0}, OppCode={1}", ui.TeamId, ui.OperationCode);
            HourlyCounts hc = new HourlyCounts();

            Prodhour Prodhour = null;
            int Hour, Minute;

            if (ui.Offline == (int)eOffline.Offline)
            {
                Hour = ui.TxnDate.Hour;
                Minute = ui.TxnDate.Minute;
            }
            else
            {
                Hour = System.DateTime.Now.Hour;
                Minute = System.DateTime.Now.Minute;
            }

            string time = Hour.ToString().PadLeft(2, '0') + Minute.ToString().PadLeft(2, '0');
            //int HourNo = 0;
            uint TeamId = ui.TeamId;
            int OppCode = ui.OperationCode;

            try
            {
                Prodhour = (from d in dcap.Prodhour
                            where Convert.ToInt32(d.Stime) <= Convert.ToInt32(time) && Convert.ToInt32(d.Etime) >= Convert.ToInt32(time)
                            && d.RecStatus == (int)eRecStatus.Active
                            && d.TeamId == TeamId
                            select new Prodhour
                            {
                                TeamId = d.TeamId,
                                ShiftId = d.ShiftId,
                                SeqId = d.SeqId,
                                HourNo = (uint)d.HourNo,
                                Stime = d.Stime,
                                Etime = d.Stime
                            }).FirstOrDefault();

                if (Prodhour != null)
                {
                    Prodhour = (from d in dcap.Prodhour
                                where Convert.ToInt32(d.Etime) == Convert.ToInt32(Prodhour.Stime)
                                && d.RecStatus == (int)eRecStatus.Active
                                && d.TeamId == TeamId
                                select new Prodhour
                                {
                                    TeamId = d.TeamId,
                                    ShiftId = d.ShiftId,
                                    SeqId = d.SeqId,
                                    HourNo = (uint)d.HourNo,
                                    Stime = d.Stime,
                                    Etime = d.Stime
                                }).FirstOrDefault();

                    if (Prodhour != null)
                    {
                        Detxn Detxn = null;
                        HourNo = (int)Prodhour.HourNo;

                        Detxn = (from d in dcap.Detxn
                                 where d.HourNo == HourNo && d.TeamId == TeamId && d.TxnDateTime.Date == System.DateTime.Now.Date
                                 && d.OperationCode == OppCode
                                 group d by new { d.TeamId, d.HourNo }
                                into grp
                                 select new Detxn
                                 {
                                     Qty01 = grp.Sum(d => d.Qty01),
                                     Qty02 = grp.Sum(d => d.Qty02),
                                     Qty03 = grp.Sum(d => d.Qty03),
                                 }
                        ).FirstOrDefault();

                        if (Detxn != null)
                        {
                            hc.PrevHrGood = (int)Detxn.Qty01;
                            hc.PrevHrScrap = (int)Detxn.Qty02;
                            hc.PrevHrRework = (int)Detxn.Qty03;
                        }
                    }
                }
                return hc;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetPrevHourQty information {0}", e.ToString());
                return hc;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("GetTotalGoodQty")]
        public HourlyCounts GetTotalGoodQty(uint TeamId, uint depid)
        {
            logger.InfoFormat("GetTotalGoodQty API called with TeamId={0}, depid={1}", TeamId, depid);
            HourlyCounts hc = new HourlyCounts();

            try
            {
                Detxn Detxn = null;


                Detxn = (from d in dcap.Detxn
                         where d.TxnDateTime.Date == System.DateTime.Now.Date && d.TeamId == TeamId
                         && d.Depid == depid
                         group d by new { d.TeamId }
                        into grp
                         select new Detxn
                         {
                             Qty01 = grp.Sum(d => d.Qty01),
                             Qty02 = grp.Sum(d => d.Qty02),
                             Qty03 = grp.Sum(d => d.Qty03),
                         }
                ).FirstOrDefault();

                if (Detxn != null)
                {
                    hc.TotGood = (int)Detxn.Qty01;

                }

                return hc;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTotalGoodQty information {0}", e.ToString());
                return hc;
                throw e;
            }
        }

        //GET api /shedule/GetRejectReasonCategory - API to get Schedule By ID Information
        [Produces("application/json")]
        [HttpGet("GetRejectReasonCategory")]
        public IList<Rrcat> GetRejectReasonCategory(int RRType)
        {
            //RRCode = RRCode == null ? "%" : RRCode;
            logger.InfoFormat("GetRejectReasonCategory API called with TeamId={0}", RRType);
            IList<Rrcat> RrcatList = null;

            try
            {
                RrcatList = (from r in dcap.Rrcat
                             where r.RecStatus == (int)eRecStatus.Active
                             && r.Rrtype == RRType
                             select new Rrcat
                             {
                                 RrcatId = r.RrcatId,
                                 RrcatName = r.RrcatName
                             })
                         .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            return RrcatList;
        }

        [Produces("application/json")]
        [HttpGet("GetReworkReasonsByCategory")]
        public IList<Rejectreason> GetReworkReasonsByCategory(int RRType, string RRCat, string SCode, int DopsId)
        {
            RRCat = RRCat == null ? "%" : RRCat;
            SCode = SCode == null ? "%" : SCode;
            //RRCode = RRCode == null ? "%" : RRCode; 
            logger.InfoFormat("GetRejectReasonCategory API called with TeamId={0}, RRCat={1} , SCode={2}", RRType, RRCat, SCode);
            IList<Rejectreason> RRList = null;

            try
            {
                RRList = (from r in dcap.Rejectreason
                          where r.RecStatus == (int)eRecStatus.Active
                          && EF.Functions.Like(r.RrcatId, "%" + RRCat + "%")
                          && EF.Functions.Like(r.Scode.ToUpper(), "%" + SCode.ToUpper() + "%")
                          && r.Rrtype == RRType
                          && r.DopsId == DopsId
                          orderby r.Scode
                          select new Rejectreason
                          {
                              Rrid = r.Rrid,
                              Rrname = r.Rrname,
                              Rrdesc = r.Rrdesc,
                              RrnameSin = r.RrnameSin,
                              RrdescSin = r.RrdescSin,
                              RrnameTem = r.RrnameTem,
                              RrdescTem = r.RrdescTem,
                              Scode = r.Scode
                          })
                       .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetRejectReasonCategory information {0}", e.ToString());
                throw e;
            }
            return RRList;
        }

        public IList<Team> GetAllTeamsForFactory(string FacCode, string LocCode)
        {
            logger.InfoFormat("GetAllTeamsForFactory API called with FacCode={0}, LocCode={0}", FacCode, LocCode);
            IList<Team> Teams = null;

            try
            {

                Teams = (from t in dcap.Team
                         where t.FacCode == FacCode && t.RecStatus == (int)eRecStatus.Active
                            && EF.Functions.Like(t.LocCode, "%" + LocCode + "%")
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
                throw e;
            }
            return Teams;
        }

        [Produces("application/json")]
        [HttpGet("GetProdHoursForFactory")]
        public IList<Prodhour> GetProdHoursForFactory(uint TeamId, int ShiftId)
        {
            logger.InfoFormat("GetProdHoursForFactory API called with TeamId={0}, ShiftId={1}", TeamId, ShiftId);
            IList<Prodhour> Prodhour = null;

            try
            {
                Prodhour = (from p in dcap.Prodhour
                            where p.TeamId == TeamId && p.RecStatus == (int)eRecStatus.Active &&
                            p.ShiftId == ShiftId
                            orderby p.SeqId
                            select new Prodhour
                            {
                                HourNo = p.HourNo,
                                TeamId = p.TeamId,
                                HourName = p.HourName
                            }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetProdHoursForFactory information {0}", e.ToString());
                throw e;
            }
            return Prodhour;
        }

        // public bool ValueExists(UserInput ui)
        // {
        //     logger.InfoFormat("ValueExists for given DepId and Barcode. DepId = {0}, Barcode = {1}", ui.Depid, ui.Barcode);
        //     Detxn Detxn = null;

        //     try
        //     {
        //         Detxn = (from p in dcap.Detxn
        //                  where p.Depid == ui.Depid && p.BarCodeNo == ui.Barcode
        //                  group p by new { p.Depid, p.BarCodeNo }
        //                 into grp
        //                  select new Detxn
        //                  {
        //                      Qty01 = grp.Sum(p => p.Qty01),

        //                  }).FirstOrDefault();

        //         if (Detxn.Qty01 > 0)
        //         {
        //             logger.InfoFormat("Duplicate entry,Trying to same barcode of same operation as good garment");
        //             return true;
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         logger.ErrorFormat("Error while retrieving ValueExists information {0}", e.ToString());
        //         throw e;
        //     }
        //     return false;
        // }

        //Get Scan conunter value

        [Produces("application/json")]
        [HttpGet("GetScanCounterVal")]
        public int GetScanCounterVal(uint WfdepinstId)
        {
            logger.InfoFormat("GetScanCounterVal for given WfdepinstId = {0}", WfdepinstId);

            try
            {
                Wfdep objwfd = new Wfdep();
                objwfd = dcap.Wfdep
                        .Where(c => c.WfdepinstId == WfdepinstId)
                        .FirstOrDefault();

                return objwfd.ScanCounter;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetScanCounterVal information {0}", e.ToString());
                throw e;
            }

        }

        //Get Scan conunter value
        [Produces("application/json")]
        [HttpGet("GetProductionQtyForHour")]
        public HourlyCounts GetProductionQtyForHour(uint TeamId, uint HourNo, DateTime TxnDate)
        {
            logger.InfoFormat("GetProductionQtyForHour TeamId = {0}, HourNo = HourNo{0}", TeamId, HourNo);

            HourlyCounts hc = new HourlyCounts();
            Detxn Detxn = null;

            try
            {
                Detxn = (from d in dcap.Detxn
                         where d.HourNo == HourNo && d.TeamId == TeamId && d.TxnDateTime.Date == TxnDate.Date
                         // && d.OperationCode == OppCode
                         group d by new { d.TeamId, d.HourNo }
                        into grp
                         select new Detxn
                         {
                             Qty01 = grp.Sum(d => d.Qty01),
                             Qty02 = grp.Sum(d => d.Qty02),
                             Qty03 = grp.Sum(d => d.Qty03),
                         }
                ).FirstOrDefault();

                if (Detxn != null)
                {
                    hc.CurHrGood = (int)Detxn.Qty01;
                    hc.CurHrScrap = (int)Detxn.Qty02;
                    hc.CurHrRework = (int)Detxn.Qty03;
                }
                return hc;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetProductionQtyForHour {0}", e.ToString());
                return hc;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("GetSoberSheetDetails")]
        public List<StyleScheduleColor> GetSoberSheetDetails(int SoberSheetNo)
        {
            logger.InfoFormat("GetSoberSheetDetails for given SoberSheetNo = {0}", SoberSheetNo);

            List<SoberSheet> sSheet = new List<SoberSheet>(); // new SoberSheet();
            List<StyleScheduleColor> stSchClr = new List<StyleScheduleColor>(); // new SoberSheet();
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                using (SqlConnection sqlConn = new SqlConnection("Data Source=bci-app-03;Initial Catalog=FOSSV3;User ID=FossUser;Password=FossUser; Connection Timeout=180"))
                {
                    string sql = "SPCAS_Get_LaySheet_Summary";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@LAYSHEET_NO", SoberSheetNo);
                        sqlCmd.Parameters.AddWithValue("@GROUP_ID", 1);
                        sqlCmd.Parameters.AddWithValue("@COM_ID", 1);
                        sqlConn.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                    }
                }

                if (dt != null)
                {
                    int a = dt.Rows.Count;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SoberSheet sS = new SoberSheet();
                        sS.StyleNo = dt.Rows[i]["STYLE_NAME"].ToString().Trim();
                        sS.ScheduleNo = dt.Rows[i]["DELIVERY_NO"].ToString().Trim();
                        sS.ColorName = dt.Rows[i]["COLOR_NAME"].ToString().Trim();
                        sS.Size = dt.Rows[i]["SIZE_INSEAM"].ToString().TrimEnd('-');
                        sS.Pattern = dt.Rows[i]["PATTERN_NAME"].ToString().Trim();
                        sS.ShadeLot = dt.Rows[i]["SHADE_LOT"].ToString().Trim();
                        sS.Qty = Convert.ToInt32(dt.Rows[i]["QUANTITY"].ToString().Trim());

                        sSheet.Add(sS);
                    }

                    stSchClr = (from l1 in dcap.L1
                                join ss in sSheet on l1.L1no equals ss.StyleNo
                                join l2 in dcap.L2 on new { A = l1.L1id, B = ss.ScheduleNo } equals new { A = l2.L1id, B = l2.L2no }
                                //join l3 in dcap.L3 on new { A = l1.L1id, B = l2.L2id, C = 0 } equals new { A = l3.L1id, B = l3.L2id, C = l3.L3id }
                                join l4 in dcap.L4 on new { A = l1.L1id, B = l2.L2id, C = ss.ColorName } equals new { A = l4.L1id, B = l4.L2id, C = l4.L4desc }
                                join l5 in dcap.L5 on new { A = l1.L1id, B = l2.L2id, C = l4.L4id, D = ss.Size } equals new { A = l5.L1id, B = l5.L2id, C = l5.L4id, D = l5.L5desc }
                                select new StyleScheduleColor
                                {
                                    StyleId = l1.L1id,
                                    StyleNo = l1.L1no,
                                    StyleDesc = l1.L1desc,
                                    ScheduleId = l2.L2id,
                                    ScheduleNo = l2.L2no,
                                    ScheduleDesc = l2.L2desc,
                                    SizeDesc = l5.L5desc,
                                    SizeId = l5.L5id,
                                    ColorId = l4.L4id,
                                    ColorNo = l4.L4no,
                                    Quantity = ss.Qty
                                }).ToList();

                    if (stSchClr.Count != sSheet.Count)
                    {
                        stSchClr = null;
                    }
                    else
                    {
                        stSchClr = (from d in stSchClr
                                    group d by new
                                    {
                                        d.StyleId,
                                        d.StyleNo,
                                        d.StyleDesc,
                                        d.ScheduleId,
                                        d.ScheduleNo,
                                        d.ScheduleDesc,
                                        d.SizeDesc,
                                        d.SizeId,
                                        d.ColorId,
                                        d.ColorNo
                                    }
                                    into grp
                                    select new StyleScheduleColor
                                    {
                                        StyleId = grp.Key.StyleId,
                                        StyleNo = grp.Key.StyleNo,
                                        StyleDesc = grp.Key.StyleDesc,
                                        ScheduleId = grp.Key.ScheduleId,
                                        ScheduleNo = grp.Key.ScheduleNo,
                                        ScheduleDesc = grp.Key.ScheduleDesc,
                                        SizeDesc = grp.Key.SizeDesc,
                                        SizeId = grp.Key.SizeId,
                                        ColorId = grp.Key.ColorId,
                                        ColorNo = grp.Key.ColorNo,
                                        Quantity = grp.Sum(d => d.Quantity)
                                    }).ToList();
                    }
                }
                return stSchClr;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetSoberSheetDetails information {0}", e.ToString());
                return stSchClr;
                throw e;
            }
        }


        //Get Is Sober Sheet Alredy Saved
        [Produces("application/json")]
        [HttpGet("GetIsSoberSheetAlredySaved")]
        public bool GetIsSoberSheetAlredySaved(string SoberSheetNo)
        {
            logger.InfoFormat("Get Is Sober Sheet Alredy Saved SoberSheetNo = {0}", SoberSheetNo);

            Detxn Detxn = null;
            System.Data.DataTable dt = new System.Data.DataTable();
            uint l1id = 0;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection("Data Source=bci-app-03;Initial Catalog=FOSSV3;User ID=FossUser;Password=FossUser; Connection Timeout=180"))
                {
                    string sql = "SPCAS_Get_LaySheet_Summary";
                    using (SqlCommand sqlCmd = new SqlCommand(sql, sqlConn))
                    {
                        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@LAYSHEET_NO", SoberSheetNo);
                        sqlCmd.Parameters.AddWithValue("@GROUP_ID", 1);
                        sqlCmd.Parameters.AddWithValue("@COM_ID", 1);
                        sqlConn.Open();
                        using (SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd))
                        {
                            sqlAdapter.Fill(dt);
                        }
                    }
                }

                if (dt != null)
                {
                    string StyleNo = dt.Rows[0]["STYLE_NAME"].ToString().Trim();
                    l1id = dcap.L1.Where(c => c.L1no == StyleNo).Select(c => c.L1id).FirstOrDefault();
                }

                Detxn = (from d in dcap.Detxn
                         where d.L1id == l1id && d.JobNo == SoberSheetNo
                         select new Detxn
                         {
                             JobNo = d.JobNo
                         }
                ).FirstOrDefault();

                if (Detxn != null)
                {
                    return true;
                }
                return false;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetIsSoberSheetAlredySaved {0}", e.ToString());
                return false;
                throw e;
            }
        }

        //Get Is Sober Sheet Alredy Saved
        [Produces("application/json")]
        [HttpGet("CheckBCSCanforSameLine")]
        public bool CheckBCScanforSameLine(int WfdepinstId, string BarcodeNo)
        {
            logger.InfoFormat("Get Check Barcode Scan for SameLine BarcodeNo = {0},WfdepinstId = {1}", BarcodeNo, WfdepinstId);

            Detxn Detxn = null;

            try
            {
                Detxn = (from d in dcap.Detxn
                         where d.BarCodeNo == BarcodeNo && d.WfdepinstId == WfdepinstId
                         select new Detxn
                         {
                             BarCodeNo = d.BarCodeNo,
                             Qty01 = d.Qty01,
                             Qty02 = d.Qty02,
                             Qty03 = d.Qty03
                         }

                ).FirstOrDefault();

                if (Detxn != null)
                {
                    return true;
                }
                return false;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckBCScanforSameLine {0}", e.ToString());
                return false;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("CheckBCSCanforSameLine")]
        public bool CheckTBCScanforSameLine(int WfdepinstId, string BarcodeNo)
        {
            logger.InfoFormat("Get Check Barcode Scan for SameLine BarcodeNo = {0},WfdepinstId = {1}", BarcodeNo, WfdepinstId);

            TravelStatus TravelStatus = null;

            try
            {
                TravelStatus = (from d in dcap.TravelStatus
                                where d.WfdepinstId == WfdepinstId && d.BarCodeNo == BarcodeNo
                                select new TravelStatus
                                {
                                    BarCodeNo = d.BarCodeNo,
                                    Qty01 = d.Qty01,
                                    Qty02 = d.Qty02,
                                    Qty03 = d.Qty03
                                }

                ).FirstOrDefault();

                if (TravelStatus != null)
                {
                    return true;
                }
                return false;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckBCScanforSameLine {0}", e.ToString());
                return false;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("GetDefectOperatios")]
        public IList<Defectops> GetDefectOperatios(string dopsScode)
        {
            logger.InfoFormat("GetDefectOperatios Defect Opps Scode = {1}", 1);

            IList<Defectops> Defectops = null;
            dopsScode = dopsScode == null ? "%" : dopsScode;

            try
            {
                Defectops = (from d in dcap.Defectops
                             where d.RecStatus == (int)eRecStatus.Active &&
                             EF.Functions.Like(d.DopsScode.ToUpper(), "%" + dopsScode.ToUpper() + "%")
                             orderby d.DopsScode
                             select new Defectops
                             {
                                 DopsId = d.DopsId,
                                 DopsCatId = d.DopsCatId,
                                 DopsScode = d.DopsScode,
                                 DopsName = d.DopsName,
                                 DopsDesc = d.DopsDesc,
                                 DopsNameS = d.DopsNameS,
                                 DopsDescS = d.DopsDescS
                             }).ToList();


                return Defectops;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetDefectOperatios {0}", e.ToString());
                return Defectops;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("GetDefectOperatiosBFL")]
        public IList<Defectops> GetDefectOperatiosBFL(string operationCode, string dopsScode)
        {
            logger.InfoFormat("GetDefectOperatiosBFL Defect Opps Scode = {1}", 1, operationCode);

            IList<Defectops> Defectops = null;
            dopsScode = dopsScode == null ? "%" : dopsScode;

            try
            {
                Defectops = (from d in dcap.Defectops
                             where d.RecStatus == (int)eRecStatus.Active && d.DopsCatId == operationCode &&
                             EF.Functions.Like(d.DopsScode.ToUpper(), "%" + dopsScode.ToUpper() + "%")
                             orderby d.DopsScode
                             select new Defectops
                             {
                                 DopsId = d.DopsId,
                                 DopsCatId = d.DopsCatId,
                                 DopsScode = d.DopsScode,
                                 DopsName = d.DopsName,
                                 DopsDesc = d.DopsDesc,
                                 DopsNameS = d.DopsNameS,
                                 DopsDescS = d.DopsDescS
                             }).ToList();


                return Defectops;
            }

            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetDefectOperatios {0}", e.ToString());
                return Defectops;
                throw e;
            }
        }

        [Produces("application/json")]
        [HttpGet("GetRRIdToReverse")]
        public Detxn GetRRIdToReverse(string Barcode)
        {
            logger.InfoFormat("GetRRIdToReverse API Called with Barcode. Barcode = {0}", Barcode);
            Detxn Detxn = null;

            try
            {
                Detxn = (from p in dcap.Detxn
                         where p.BarCodeNo == Barcode && p.Qty03 > 0
                         orderby p.TxnDateTime descending
                         select new Detxn
                         {
                             Rrid = p.Rrid,
                             DopsId = p.DopsId

                         }).FirstOrDefault();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetRRIdToReverse {0}", e.ToString());
                throw e;
            }
            return Detxn;
        }

        [Produces("application/json")]
        [HttpGet("GetTeamCounterValues")]

        public List<TeamCounterCM> GetTeamCounterValues(uint WFDEPInstId, int CounterType)
        {
            logger.InfoFormat("GetTeamCounterValues API Called with. WFDEPInstId = {0}, CounterType = {1}", WFDEPInstId, CounterType);
            List<TeamCounterCM> LstTeamCounterCM = new List<TeamCounterCM>();

            try
            {
                LstTeamCounterCM = (from p in dcap.TeamCounter
                                    join l2 in dcap.L2 on new { A = p.L1id, B = p.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                    where p.WfdepinstId == WFDEPInstId && p.CounterType == CounterType && p.RecStatus == (int)eRecStatus.Active
                                    select new TeamCounterCM
                                    {
                                        WFDEPInstId = p.WfdepinstId,
                                        PONo = l2.Ref01,
                                        Zfeature = l2.Ref02

                                    }).ToList();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTeamCounterValues {0}", e.ToString());
                throw e;
            }
            return LstTeamCounterCM;
        }

        [Produces("application/json")]
        [HttpGet("CheckCounterAvailable")]
        public bool CheckCounterAvailable(uint WFDEPInstId)
        {
            logger.InfoFormat("CheckCounterAvailable API called with WFDEPInstId = {0}", WFDEPInstId);
            uint RetVal = 0;
            IList<TeamCounter> lstTeamCounter = null;
            try
            {
                lstTeamCounter = (from d in dcap.TeamCounter
                                  where d.WfdepinstId == WFDEPInstId
                                  select new TeamCounter
                                  {
                                      CounterId = d.CounterId,
                                      WfdepinstId = d.WfdepinstId,
                                      CounterType = d.CounterType,
                                      L1id = d.L1id,
                                      L2id = d.L2id,
                                      L3id = d.L3id,
                                      L4id = d.L4id,
                                      L5id = d.L5id,
                                      Qty01 = d.Qty01

                                  }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckCounterAvailable {0}", e.ToString());
                throw e;
            }
            return false;
        }

        [Produces("application/json")]
        [HttpGet("GetPOcounterValues")]
        public IList<TeamCounterCM> GetPOcounterValues(uint WFDEPInstId)
        {
            logger.InfoFormat("GetPOcounterValues API called with WFDEPInstId = {0}", WFDEPInstId);
            uint RetVal = 0;
            IList<TeamCounterCM> lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.TeamCounter
                                  join l1 in dcap.L1 on d.L1id equals l1.L1id
                                  join l2 in dcap.L2 on new { A = d.L1id, B = d.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                  join l4 in dcap.L4 on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                  //join l5moops in dcap.L5moops on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id, E = d.L5id } equals new { A = l5moops.L1id, B = l5moops.L2id, C = l5moops.L3id, D = l5moops.L4id, E = l5moops.L5id }
                                  where d.WfdepinstId == WFDEPInstId && d.RecStatus == (int)eRecStatus.Active
                                  orderby d.CreatedDateTime ascending //Always new bag should added  as 1st from right, bag positions should not change unless one bag is closed - As Per UAT - decending to ascending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                      StyleId = l1.L1id,
                                      StyleNo = l1.L1no,
                                      StyleDesc = l1.L1desc,
                                      Ref01 = l1.Ref01,
                                      ScheduleId = l2.L2id,
                                      ScheduleNo = l2.L2no,
                                      ScheduleDesc = l2.L2desc,
                                      Zfeature = l2.Ref02,
                                      PONo = l2.Ref01,
                                      ColorId = l4.L4id,
                                      ColorDesc = l4.L4desc,
                                      BagBarCode = d.BagBarCodeNo,
                                      BagSize = d.BagSize,
                                      CounterNumber = d.CounterNumber,
                                      Qty01 = d.Qty01,
                                      CutQuantity = d.CutQty,
                                      BagStatus = d.BagStatus
                                  }).ToList();

                if (lstTeamCounter != null)
                {
                    return lstTeamCounter;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        [Produces("application/json")]
        [HttpGet("GetPOcounterValuesNonApperal")]
        public IList<TeamCounterCM> GetPOcounterValuesNonApperal(uint WFDEPInstId)
        {
            logger.InfoFormat("GetPOcounterValuesNonApperal API called with WFDEPInstId = {0}", WFDEPInstId);
            //uint RetVal = 0;
            IList<TeamCounterCM> lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.TeamCounter
                                  join l1 in dcap.L1 on d.L1id equals l1.L1id
                                  join l2 in dcap.L2 on new { A = d.L1id, B = d.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                  join l4 in dcap.L4 on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                  join l5 in dcap.L5 on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id, E = d.L5id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id, E = l5.L5id }
                                  //join l5moops in dcap.L5moops on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id, E = d.L5id } equals new { A = l5moops.L1id, B = l5moops.L2id, C = l5moops.L3id, D = l5moops.L4id, E = l5moops.L5id }
                                  where d.WfdepinstId == WFDEPInstId && d.RecStatus == (int)eRecStatus.Active
                                  orderby d.CreatedDateTime ascending //Always new bag should added  as 1st from right, bag positions should not change unless one bag is closed - As Per UAT - decending to ascending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                      StyleId = l1.L1id,
                                      StyleNo = l1.L1no,
                                      StyleDesc = l1.L1desc,
                                      Ref01 = l1.Ref01,
                                      ScheduleId = l2.L2id,
                                      ScheduleNo = l2.L2no,
                                      ScheduleDesc = l2.L2desc,
                                      Zfeature = l2.Ref02,
                                      PONo = l2.Ref01,
                                      ColorId = l4.L4id,
                                      ColorDesc = l4.L4desc,
                                      SizeId = l5.L5id,
                                      SizeDesc = l5.L5desc,
                                      BagBarCode = d.BagBarCodeNo,
                                      BagSize = d.BagSize,
                                      CounterNumber = d.CounterNumber,
                                      Qty01 = d.Qty01,
                                      CutQuantity = d.CutQty,
                                      BagStatus = d.BagStatus
                                  }).ToList();

                if (lstTeamCounter != null)
                {
                    return lstTeamCounter;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        [Produces("application/json")]
        [HttpGet("GetBuddyTagCounters")]
        public IList<TeamCounterCM> GetBuddyTagCounters(string clientId)
        {
            logger.InfoFormat("Get Buddy Tag Counter Values API called with clientId = {0}", clientId);
            uint RetVal = 0;
            IList<TeamCounterCM> lstTeamCounter = new List<TeamCounterCM>();

            try
            {
                IList<Clientconfig> clientconfid = (from c in dcap.Clientconfig
                                                    where c.ClientId == clientId
                                                    group c by new
                                                    {
                                                        c.WfdepinstId
                                                    } into grp
                                                    select new Clientconfig
                                                    {
                                                        WfdepinstId = grp.Key.WfdepinstId
                                                    }).ToList();

                foreach (Clientconfig q in clientconfid)
                {
                    lstTeamCounter = lstTeamCounter.Concat(GetBuddyTagCounterValues((uint)q.WfdepinstId)).ToList();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }


        [Produces("application/json")]
        [HttpGet("GetBuddyTagCounterValues")]
        public IList<TeamCounterCM> GetBuddyTagCounterValues(uint WFDEPInstId)
        {
            logger.InfoFormat("Get Buddy Tag Counter Values API called with WFDEPInstId = {0}", WFDEPInstId);
            uint RetVal = 0;
            IList<TeamCounterCM> lstTeamCounter = new List<TeamCounterCM>();

            try
            {
                lstTeamCounter = (from d in dcap.BuudyTagCounter
                                  join l1 in dcap.L1 on d.L1id equals l1.L1id
                                  join l2 in dcap.L2 on new { A = d.L1id, B = d.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                  join l4 in dcap.L4 on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                  //join l5moops in dcap.L5moops on new { A = d.L1id, B = d.L2id, C = d.L3id, D = d.L4id, E = d.L5id } equals new { A = l5moops.L1id, B = l5moops.L2id, C = l5moops.L3id, D = l5moops.L4id, E = l5moops.L5id }
                                  where d.WfdepinstId == WFDEPInstId && d.RecStatus == (int)eRecStatus.Active
                                  orderby d.CreatedDateTime ascending //Always new bag should added  as 1st from right, bag positions should not change unless one bag is closed - As Per UAT - decending to ascending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                      StyleId = l1.L1id,
                                      StyleNo = l1.L1no,
                                      StyleDesc = l1.L1desc,
                                      Ref01 = l1.Ref01,
                                      ScheduleId = l2.L2id,
                                      ScheduleNo = l2.L2no,
                                      ScheduleDesc = l2.L2desc,
                                      Zfeature = l2.Ref02,
                                      PONo = l2.Ref01,
                                      ColorId = l4.L4id,
                                      ColorDesc = l4.L4desc,
                                      BagBarCode = d.BagBarCodeNo,
                                      BagSize = d.BagSize,
                                      CounterNumber = d.CounterNumber,
                                      TravelBarCodeNo = d.TravelBarCodeNo,
                                      RRType = d.RRType,
                                      RRId = d.RRId,
                                      RRName = d.RRName,
                                      Qty01 = d.Qty01,
                                      JobQty = d.JobQty,
                                      Weight = d.Weight,
                                      TrollyNo = d.TrollyNo,
                                      Remarks = d.Remarks,
                                      CutQuantity = d.CutQty,
                                      BagStatus = d.BagStatus
                                  }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        //GetUpdatedPOcounterValues : last checked 8-2-2020
        //If there any counter change avilable after barcode is sacned then get details
        //Used API's and UI : updatePOcounterValues (WEB Barcode)
        [Produces("application/json")]
        [HttpGet("GetUpdatedPOcounterValues")]
        public TeamCounterCM GetUpdatedPOcounterValues(uint WFDEPInstId, int CounterId)
        {
            logger.InfoFormat("GetUpdatedPOcounterValues API called with WFDEPInstId = {0}, CounterId id = {1}", WFDEPInstId, CounterId);
            uint RetVal = 0;
            TeamCounterCM lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.TeamCounter
                                  where d.CounterId == CounterId && d.WfdepinstId == WFDEPInstId
                                  orderby d.CreatedDateTime ascending //Always new bag should added  as 1st from right, bag positions should not change unless one bag is closed - As Per UAT - decending to ascending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                      BagBarCode = d.BagBarCodeNo,
                                      BagSize = d.BagSize,
                                      CounterNumber = d.CounterNumber,
                                      Qty01 = d.Qty01,
                                      CutQuantity = d.CutQty,
                                      BagStatus = d.BagStatus
                                  }).FirstOrDefault();

                if (lstTeamCounter != null)
                {
                    return lstTeamCounter;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        //GetUpdatedPOcounterValues : last checked 8-2-2020
        //If there any counter change avilable after barcode is sacned then get details
        //Used API's and UI : updatePOcounterValues (WEB Barcode)
        [Produces("application/json")]
        [HttpGet("GetUpdatedBuddyTagcounterValues")]
        public TeamCounterCM GetUpdatedBuddyTagcounterValues(uint WFDEPInstId, int CounterId)
        {
            logger.InfoFormat("GetUpdatedBuddyTagcounterValues API called with WFDEPInstId = {0}, CounterId id = {1}", WFDEPInstId, CounterId);
            uint RetVal = 0;
            TeamCounterCM lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.BuudyTagCounter
                                  where d.WfdepinstId == WFDEPInstId && d.CounterId == CounterId
                                  orderby d.CreatedDateTime ascending //Always new bag should added  as 1st from right, bag positions should not change unless one bag is closed - As Per UAT - decending to ascending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                      BagBarCode = d.BagBarCodeNo,
                                      BagSize = d.BagSize,
                                      CounterNumber = d.CounterNumber,
                                      Qty01 = d.Qty01,
                                      CutQuantity = d.CutQty,
                                      BagStatus = d.BagStatus
                                  }).FirstOrDefault();

                if (lstTeamCounter != null)
                {
                    return lstTeamCounter;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        //Latest PO Coubter
        //UI: focousPOcounterValues
        [Produces("application/json")]
        [HttpGet("GetMostUpdatedPOcounter")]
        public TeamCounterCM GetMostUpdatedPOcounter(uint WFDEPInstId)
        {
            logger.InfoFormat("GetMostUpdatedPOcounter API called with WFDEPInstId = {0}", WFDEPInstId);
            TeamCounterCM lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.TeamCounter
                                  where d.WfdepinstId == WFDEPInstId && d.RecStatus == (int)eRecStatus.Active
                                  orderby d.ModifiedDateTime descending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                  }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        //Latest Buddy Tag Coubter
        //UI: focousPOcounterValues 
        [Produces("application/json")]
        [HttpGet("GetMostUpdatedBuddyTagcounter")]
        public TeamCounterCM GetMostUpdatedBuddyTagcounter(uint WFDEPInstId)
        {
            logger.InfoFormat("GetMostUpdatedBuddyTagcounter API called with WFDEPInstId = {0}", WFDEPInstId);
            TeamCounterCM lstTeamCounter = null;

            try
            {
                lstTeamCounter = (from d in dcap.BuudyTagCounter
                                  where d.WfdepinstId == WFDEPInstId && d.RecStatus == (int)eRecStatus.Active
                                  orderby d.ModifiedDateTime descending
                                  select new TeamCounterCM
                                  {
                                      CounterId = d.CounterId,
                                  }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }


        [Produces("application/json")]
        [HttpGet("ValidateBCReverse")]
        public bool ValidateBCReverse(string Barcode, int NxtOppcode)
        {
            logger.InfoFormat("ValidateBCReverse API Called with Barcode and NxtOppcode. Barcode = {0},  NxtOppcode = {1}", Barcode, NxtOppcode);

            Detxn Detxn = null;

            try
            {
                Detxn = (from p in dcap.Detxn
                         where p.BarCodeNo == Barcode && p.OperationCode == NxtOppcode && p.RecStatus == (int)eRecStatus.Active
                         select new Detxn
                         {
                             BarCodeNo = p.BarCodeNo

                         }).FirstOrDefault();

                if (Detxn != null)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving ValidateBCReverse information {0}", e.ToString());
                return false;
                throw e;

            }
            return true;
        }


        [Produces("application/json")]
        [HttpGet("POLimitCheck")]
        public bool POLimitCheck(UserInput ui)
        {
            logger.InfoFormat("POLimitCheck API called with WFDEPInstId = {0}", ui.WfdepinstId);
            uint RetVal = 0;
            IList<TeamCounterCM> lstTeamCounter = null;

            try
            {
                /* lstTeamCounter = (from d in dcap.TeamCounter
                                   where d.WfdepinstId == ui.WfdepinstId &&
                                   d.RecStatus == (int)eRecStatus.Active
                                   select new TeamCounterCM
                                   {
                                       CounterId = d.CounterId
                                   }).ToList();

                 if (lstTeamCounter.Count == 20) //Chanhed by NimanthaH
                 {
                     return false;
                 }
                 else
                 {*/
                //lstTeamCounter = null;
                TeamCounter obf = new TeamCounter();
                obf = dcap.TeamCounter
                            .Where(c => c.WfdepinstId == ui.WfdepinstId && c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L4id == ui.ColorId)
                            .FirstOrDefault();

                if (obf == null)
                {
                    TransactionController TxnContrl = new TransactionController(dcap);
                    TxnContrl.AddTeamCounterWithoutBagBarcode(ui);
                }
                else
                {
                    obf.Qty01 = obf.Qty01 + ui.EnteredQtyGd;
                    dcap.SaveChanges();
                }
                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving POLimitCheck {0}", e.ToString());
                return false;
                throw e;

            }
            return true;
        }

        //Added by NimanthaH

        //Bag Barcode Lookup : load Group Barcodes acording to the selected location and etc
        //GET api /get BagBarcodeDetails Status by Factory
        //Used API's and UI : Add Items Window - getAddItemData() WEB (Dispatch)
        [Produces("application/json")]
        [HttpGet("GetBarcodeDetails")]
        public List<BagBarcodeTransactions> GetBarcodeDetails(uint Wfid, DateTime fromDate, DateTime toDate, int type, int mode, int opcode, int smode)
        {

            logger.InfoFormat("Get Factory Name");
            List<BagBarcodeTransactions> L1Details = null;

            //TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            //toDate = toDate.Add(ts);

            try
            {
                if (opcode != 0)
                {
                    if (smode == 2)
                    {
                        IList<GroupBarcode> gba = dcap.GroupBarcode.Where(gb => (Wfid == 0 ? true : (type == 100 ? gb.WFId == Wfid : (type == 300 ? gb.WFId == Wfid : gb.WFId > 0))) && gb.CreatedDateTime >= fromDate && gb.CreatedDateTime <= toDate && gb.OperationCode == opcode).ToList();

                        L1Details = (from detxn in gba
                                     join l1 in dcap.L1 on (uint?)detxn.L1id equals (uint?)l1.L1id
                                     join wf in dcap.Wf on detxn.WFId equals wf.Wfid
                                     join l2 in dcap.L2 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join l4 in dcap.L4 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     join l5 in dcap.L5 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                                     where (mode == 0 ? true : detxn.TxnMode == mode) && detxn.RecStatus == 1 &&
                                     ((type == 100 || type == 300) ? ((detxn.TxnStatus == 0 && detxn.DEPId == 100) || (detxn.TxnStatus == 5 && detxn.DEPId == 200)) : ((detxn.TxnStatus == 5 && detxn.DEPId == 100) || (detxn.TxnStatus == 0 && detxn.DEPId == 200)))
                                     orderby detxn.ModifiedDateTime
                                     select new BagBarcodeTransactions
                                     {
                                         Key = detxn.WFDEPInstId + "|" + detxn.Seq + "|" + detxn.BagBarCodeNo + "|" + detxn.TxnMode + "|" + detxn.L1id + "|" + detxn.L2id + "|" + detxn.L3id + "|" + detxn.L4id,
                                         WfdepinstId = detxn.WFDEPInstId,
                                         Style = l1.L1desc,
                                         Shedule = l2.L2desc,
                                         PO = l2.Ref01,
                                         Z_Feature = l2.Ref02,
                                         Color = l4.L4desc,
                                         Size = l5.L5desc,
                                         BagBarCodeNo = detxn.BagBarCodeNo,
                                         WFIdBag = detxn.WFId,
                                         Factory = wf.FacCode,
                                         DEPIdBag = detxn.DEPId,
                                         SeqBag = (int)detxn.Seq,
                                         L1idBag = detxn.L1id,
                                         L2idBag = detxn.L2id,
                                         L3idBag = detxn.L3id,
                                         L4idBag = detxn.L4id,
                                         L5idBag = detxn.L5id,
                                         TxnMode = detxn.TxnMode,
                                         Qty01 = (detxn.Qty01 - (detxn.Qty02 + detxn.Qty03)) - (detxn.Qty01NS - (detxn.Qty02NS + detxn.Qty03NS)),
                                         Qty02 = detxn.Qty02,
                                         Qty03 = detxn.Qty03,
                                         BagStatus = 1,//detxn.RecStatus,
                                         CreatedDateTime = detxn.CreatedDateTime,
                                     }).ToList();
                        //L1Details = SetFlagForMinimumPOQuantityinGroupBarcodes(L1Details);
                    }
                    else
                    {
                        IList<GroupBarcode> gba = dcap.GroupBarcode.Where(gb => (Wfid == 0 ? true : (type == 100 ? gb.WFId == Wfid : (type == 300 ? gb.WFId == Wfid : gb.WFId > 0))) && gb.CreatedDateTime >= fromDate && gb.CreatedDateTime <= toDate && gb.OperationCode == opcode).ToList();

                        L1Details = (from detxn in gba
                                     join l1 in dcap.L1 on (uint?)detxn.L1id equals (uint?)l1.L1id
                                     join wf in dcap.Wf on detxn.WFId equals wf.Wfid
                                     join l2 in dcap.L2 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                     join l4 in dcap.L4 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                     where (mode == 0 ? true : detxn.TxnMode == mode) && detxn.RecStatus == 1 &&
                                     ((type == 100 || type == 300) ? ((detxn.TxnStatus == 0 && detxn.DEPId == 100) || (detxn.TxnStatus == 5 && detxn.DEPId == 200)) : ((detxn.TxnStatus == 5 && detxn.DEPId == 100) || (detxn.TxnStatus == 0 && detxn.DEPId == 200)))
                                     orderby detxn.ModifiedDateTime
                                     select new BagBarcodeTransactions
                                     {
                                         Key = detxn.WFDEPInstId + "|" + detxn.Seq + "|" + detxn.BagBarCodeNo + "|" + detxn.TxnMode + "|" + detxn.L1id + "|" + detxn.L2id + "|" + detxn.L3id + "|" + detxn.L4id,
                                         WfdepinstId = detxn.WFDEPInstId,
                                         Style = l1.L1desc,
                                         Shedule = l2.L2desc,
                                         PO = l2.Ref01,
                                         Z_Feature = l2.Ref02,
                                         Color = l4.L4desc,
                                         BagBarCodeNo = detxn.BagBarCodeNo,
                                         WFIdBag = detxn.WFId,
                                         Factory = wf.FacCode,
                                         DEPIdBag = detxn.DEPId,
                                         SeqBag = (int)detxn.Seq,
                                         L1idBag = detxn.L1id,
                                         L2idBag = detxn.L2id,
                                         L3idBag = detxn.L3id,
                                         L4idBag = detxn.L4id,
                                         L5idBag = detxn.L5id,
                                         TxnMode = detxn.TxnMode,
                                         Qty01 = (detxn.Qty01 - (detxn.Qty02 + detxn.Qty03)) - (detxn.Qty01NS - (detxn.Qty02NS + detxn.Qty03NS)),
                                         Qty02 = detxn.Qty02,
                                         Qty03 = detxn.Qty03,
                                         BagStatus = 1,//detxn.RecStatus,
                                         CreatedDateTime = detxn.CreatedDateTime,
                                     }).ToList();
                        //L1Details = SetFlagForMinimumPOQuantityinGroupBarcodes(L1Details);
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Dispatch Lookup - Get All dispatces according to
        //GET api /get DispatchDetails Status by Factory
        //Used API's and UI : RefreshRequestGrid WEB (Dispatch)
        [Produces("application/json")]
        [HttpGet("GetDispatchDetails")]
        public IList<GoodControlDetailsSummary> GetDispatchDetails(uint Wfid, DateTime todate, int status, string loccode, string createdby, int type, string requestid, string approver, string faccode)
        {

            logger.InfoFormat("GetDispatchDetails Wfid, todate, status, loccode, createdby, type, requestid, approver, faccode", Wfid, todate, status, loccode, createdby, type, requestid, approver, faccode);
            IList<GoodControlDetailsSummary> L1Details = null;

            DateTime fromdate = DateTime.MinValue;
            TimeSpan tst = new TimeSpan(31, 0, 0, 0);
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
                fromdate = todate.Subtract(tst);
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
                fromdate = todate.Subtract(tst);
            }
            else
            {
                fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                todate = todate.Add(ts);
            }

            try
            {
                var loca = dcap.Location.GroupBy(c => new { c.LocCode, c.LocDescription, c.LocAddress }).Select(c => new { c.Key.LocCode, c.Key.LocDescription, c.Key.LocAddress }).ToList();

                L1Details = (from detxn in dcap.GoodControlDetails //.Where(c => (faccode != null ? c.LocCodeFrom == faccode : true)).AsQueryable()
                             join loc in loca on detxn.LocCode equals loc.LocCode
                             where (requestid != "" && requestid != null) ? detxn.ControlId == requestid : (detxn.TxnDateTime <= todate && detxn.TxnDateTime >= fromdate &&
                             (status == 0 ? true : detxn.TxnStatus == status) && (loccode == null ? true : detxn.LocCode == loccode) && //detxn.WFId == Wfid && 
                             (createdby == null ? true : detxn.CreatedBy == createdby) && (type == 0 ? true : detxn.ControlType == type)
                             && (requestid == null || requestid == "" ? true : detxn.ControlId == requestid) && (approver == null || approver == "" ? true : detxn.Approver == approver))
                             orderby detxn.TxnDateTime descending
                             select new GoodControlDetailsSummary
                             {
                                 TxnStatus = detxn.TxnStatus,
                                 ControlType = detxn.ControlType,
                                 Return = detxn.Return,
                                 ControlId = detxn.ControlId,
                                 Seq = detxn.Seq,
                                 Approver = detxn.Approver,
                                 LocFromCode = detxn.LocCodeFrom,
                                 LocName = loc.LocDescription,
                                 Depid = detxn.Depid,
                                 Created = detxn.CreatedBy,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 TxnDateTime = detxn.TxnDateTime,
                                 Qty01 = detxn.Qty01,
                                 Remark = detxn.Remark,
                                 LocAddress = loc.LocAddress,
                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Dispatch Lookup - Get All dispatces according to Gnerate Invoice
        //GET api /get DispatchDetails Status by
        //Used API's and UI : RefreshRequestGrid WEB (Invoice)
        [Produces("application/json")]
        [HttpGet("GetInvoiceDispatchDetails")]
        public IList<GoodControlDetailsSummary> GetInvoiceDispatchDetails(DateTime fromdate, DateTime todate, string requestid, int invoiceStatus, string faccode)
        {

            logger.InfoFormat("GetInvoiceDispatchDetails fromdate={0}, todate={1}, requestid={2}, invoiceStatus={3}");
            IList<GoodControlDetailsSummary> L1Details = null;

            //fromdate = DateTime.MinValue;
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
            }
            else
            {
                //fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                //todate = todate.Add(ts);
            }

            try
            {
                L1Details = (from detxn in dcap.GoodControlDetails.Where(c => c.Return == 0)
                             join loc in dcap.Location.GroupBy(c => new { c.LocCode, c.LocDescription, c.LocAddress }).Select(c => new { c.Key.LocCode, c.Key.LocDescription, c.Key.LocAddress }).AsQueryable() on detxn.LocCode equals loc.LocCode
                             where (requestid != "" && requestid != null) ? (detxn.ControlId == requestid && detxn.TxnStatus >= 2 && detxn.TxnStatus <= 5) : ((detxn.TxnDateTime <= todate && detxn.TxnDateTime >= fromdate && ((faccode == "" || faccode == null) ? true : (detxn.LocCode == faccode))
                             && detxn.TxnStatus >= 3 && detxn.TxnStatus <= 5 && (invoiceStatus == 2 ? true : detxn.InvoiceStatus == invoiceStatus) &&
                             (detxn.ControlType == 200))) && detxn.Return == 0
                             orderby detxn.TxnDateTime descending
                             select new GoodControlDetailsSummary
                             {
                                 TxnStatus = detxn.TxnStatus,
                                 ControlType = detxn.ControlType,
                                 ControlId = detxn.ControlId,
                                 Seq = detxn.Seq,
                                 Approver = detxn.Approver,
                                 LocFromCode = detxn.LocCodeFrom,
                                 LocName = loc.LocDescription,
                                 Depid = detxn.Depid,
                                 Created = detxn.CreatedBy,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 TxnDateTime = detxn.TxnDateTime,
                                 InvoiceNumber = detxn.InvoiceNumber,
                                 InvoiceStatus = detxn.InvoiceStatus,

                                 Qty01 = detxn.Qty01,
                                 Remark = detxn.Remark,
                                 LocAddress = loc.LocAddress,
                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Dispatch Lookup - Get All dispatces according to Gnerate Invoice
        //GET api /get DispatchDetails Status by
        //Used API's and UI : RefreshRequestGrid WEB (Invoice)
        [Produces("application/json")]
        [HttpGet("GetInvoiceDispatchDetailsbyInvoice")]
        public IList<GoodControlDetailsSummary> GetInvoiceDispatchDetailsbyInvoice(DateTime fromdate, DateTime todate, string requestid, int invoiceStatus, string faccode)
        {

            logger.InfoFormat("GetInvoiceDispatchDetails fromdate={0}, todate={1}, requestid={2}, invoiceStatus={3}");
            IList<GoodControlDetailsSummary> L1Details = null;

            //fromdate = DateTime.MinValue;
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
            }
            else
            {
                //fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                //todate = todate.Add(ts);
            }

            try
            {
                if (invoiceStatus == 1)
                {
                    L1Details = (from detxn in dcap.GoodControlDetails.Where(c => c.Return == 0)
                                 join goodcontrol in dcap.GoodControl on new { A = detxn.ControlId, B = detxn.ControlType } equals new { A = goodcontrol.ControlId, B = goodcontrol.ControlType }
                                 join invoicedetails in dcap.InvoiceDetails.GroupBy(c => new { c.L1Id, c.L2Id, c.L3Id, c.L4Id, c.L5Id, c.ControlId, c.InvoiceNo }).Select(c => new { c.Key.L1Id, c.Key.L2Id, c.Key.L3Id, c.Key.L4Id, c.Key.L5Id, c.Key.ControlId, c.Key.InvoiceNo }).AsQueryable() on new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = (uint?)goodcontrol.L5id, F = goodcontrol.ControlId } equals new { A = (uint?)invoicedetails.L1Id, B = (uint?)invoicedetails.L2Id, C = (uint?)invoicedetails.L3Id, D = (uint?)invoicedetails.L4Id, E = (uint?)invoicedetails.L5Id, F = invoicedetails.ControlId }
                                 join loc in dcap.Location.GroupBy(c => new { c.LocCode, c.LocDescription, c.LocAddress }).Select(c => new { c.Key.LocCode, c.Key.LocDescription, c.Key.LocAddress }).AsQueryable() on detxn.LocCode equals loc.LocCode
                                 where (requestid != "" && requestid != null) ? (detxn.ControlId == requestid && detxn.TxnStatus >= 2 && detxn.TxnStatus <= 5) : ((detxn.TxnDateTime <= todate && detxn.TxnDateTime >= fromdate && ((faccode == "" || faccode == null) ? true : (detxn.LocCode == faccode))
                                 && detxn.TxnStatus >= 3 && detxn.TxnStatus <= 5 && (invoiceStatus == 2 ? true : detxn.InvoiceStatus == invoiceStatus) &&
                                 (detxn.ControlType == 200))) && detxn.Return == 0
                                 orderby detxn.TxnDateTime descending
                                 select new GoodControlDetailsSummary
                                 {
                                     TxnStatus = detxn.TxnStatus,
                                     ControlType = detxn.ControlType,
                                     ControlId = detxn.ControlId,
                                     Seq = detxn.Seq,
                                     Approver = detxn.Approver,
                                     LocFromCode = detxn.LocCodeFrom,
                                     LocName = loc.LocDescription,
                                     Depid = detxn.Depid,
                                     Created = detxn.CreatedBy,
                                     CreatedDateTime = detxn.CreatedDateTime,
                                     TxnDateTime = detxn.TxnDateTime,
                                     InvoiceNumber = invoicedetails.InvoiceNo,
                                     InvoiceStatus = detxn.InvoiceStatus,
                                     Qty01 = detxn.Qty01,
                                     Remark = detxn.Remark,
                                     LocAddress = loc.LocAddress,
                                 }).ToList();

                }
                else
                {
                    L1Details = (from detxn in dcap.GoodControlDetails.Where(c => c.Return == 0)
                                 join loc in dcap.Location.GroupBy(c => new { c.LocCode, c.LocDescription, c.LocAddress }).Select(c => new { c.Key.LocCode, c.Key.LocDescription, c.Key.LocAddress }).AsQueryable() on detxn.LocCode equals loc.LocCode
                                 join inv in dcap.InvoiceDetails.GroupBy(c => new { c.ControlId, c.InvoiceNo }).Select(c => new { c.Key.ControlId, c.Key.InvoiceNo }).AsQueryable() on detxn.ControlId equals inv.ControlId into ps
                                 from p in ps.DefaultIfEmpty()
                                 where (requestid != "" && requestid != null) ? (detxn.ControlId == requestid && detxn.TxnStatus >= 2 && detxn.TxnStatus <= 5) : ((detxn.TxnDateTime <= todate && detxn.TxnDateTime >= fromdate && ((faccode == "" || faccode == null) ? true : (detxn.LocCode == faccode))
                                 && detxn.TxnStatus >= 3 && detxn.TxnStatus <= 5 && (invoiceStatus == 2 ? true : detxn.InvoiceStatus == invoiceStatus) &&
                                 (detxn.ControlType == 200))) && detxn.Return == 0
                                 orderby detxn.TxnDateTime ascending
                                 select new GoodControlDetailsSummary
                                 {
                                     TxnStatus = detxn.TxnStatus,
                                     ControlType = detxn.ControlType,
                                     ControlId = detxn.ControlId,
                                     Seq = detxn.Seq,
                                     Approver = detxn.Approver,
                                     LocFromCode = detxn.LocCodeFrom,
                                     LocName = loc.LocDescription,
                                     Depid = detxn.Depid,
                                     Created = detxn.CreatedBy,
                                     CreatedDateTime = detxn.CreatedDateTime,
                                     TxnDateTime = detxn.TxnDateTime,
                                     InvoiceNumber = p.InvoiceNo,
                                     InvoiceStatus = detxn.InvoiceStatus,

                                     Qty01 = detxn.Qty01,
                                     Remark = detxn.Remark,
                                     LocAddress = loc.LocAddress,
                                 }).ToList();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }


        [Produces("application/json")]
        [HttpGet("GetFacNameFromLocCode")]
        public String GetFacNameFromLocCode(string locCode)
        {

            logger.InfoFormat("GetAllSFProcess");
            String FacName = null;

            try
            {
                var facs = (from l in dcap.Location
                            where l.LocCode == locCode
                            select new { l.LocDescription }).FirstOrDefault();

                if (facs != null)
                {
                    FacName = facs.LocDescription;
                }
                else
                {
                    FacName = locCode;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return FacName;
        }

        //Travel Tag Lookup
        //GET api /get DispatchDetails Status by Factory
        [Produces("application/json")]
        [HttpGet("GetTravelTagDetails")]
        public IList<TravelBarcodeDetailsOut> GetTravelTagDetails(DateTime todate, string travelbarcodeno, int status, int mode)
        {

            logger.InfoFormat("GetTravelTagDetails API called with todate = {0}, travelbarcodeno = {1}, status = {2}", todate, travelbarcodeno, status);
            IList<TravelBarcodeDetailsOut> L1Details = null;

            if (todate == null)
            {
                todate = DateTime.Now;
            }

            DateTime fromdate = DateTime.MinValue;
            TimeSpan tm = new TimeSpan(31, 0, 0, 0);
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
                fromdate = todate.Subtract(tm);
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
                fromdate = todate.Subtract(tm);
            }
            else
            {
                fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                todate = todate.Add(ts);
            }


            try
            {
                L1Details = (from detxn in dcap.TravelBarcodeDetails
                             join dep in dcap.Dep on detxn.TravelStatus equals dep.OperationCode
                             where ((travelbarcodeno == null || travelbarcodeno == "") ? (detxn.CreatedDateTime <= todate && detxn.CreatedDateTime >= fromdate && (status == 0 ? true : detxn.TravelStatus == status) && (mode == 0 ? detxn.TxnMode > 1 : detxn.TxnMode == mode)) : detxn.TravelBarCodeNo == travelbarcodeno)
                             orderby detxn.CreatedDateTime descending
                             select new TravelBarcodeDetailsOut
                             {
                                 TravelBarCodeNo = detxn.TravelBarCodeNo,
                                 TxnMode = detxn.TxnMode,
                                 Status = detxn.TravelStatus,
                                 TravelStatus = dep.Depdesc,
                                 Qty01 = detxn.Qty01,
                                 JobQty = detxn.JobQty,
                                 Weight = detxn.Weight,
                                 Color = detxn.Color,
                                 TrollyNo = detxn.TrollyNo,
                                 AllocationDate = detxn.AllocationDate,
                                 Remarks = detxn.Remarks,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 CreatedBy = detxn.CreatedBy,

                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //bUDDY Tag Lookup
        //GET api /get BuddyDetails Status by Factory
        [Produces("application/json")]
        [HttpGet("GetBuddyTagDetails")]
        public IList<BuddyBagBarcode> GetBuddyTagDetails(DateTime todate, string travelbarcodeno, int status)
        {

            logger.InfoFormat("GetBuddyTagDetails API called with todate = {0}, travelbarcodeno = {1}, status = {2}", todate, travelbarcodeno, status);
            IList<BuddyBagBarcode> L1Details = null;

            if (todate == null)
            {
                todate = DateTime.Now;
            }

            DateTime fromdate = DateTime.MinValue;
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
            }
            else
            {
                fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                todate = todate.Add(ts);
            }


            try
            {
                L1Details = (from detxn in dcap.BuddyBagBarcode
                             where detxn.CreatedDateTime <= todate && detxn.CreatedDateTime >= fromdate && (status == 0 ? true : detxn.TravelStatus == status) &&
                             ((travelbarcodeno == null || travelbarcodeno == "") ? true : detxn.BuddyBagBarcodeNo == travelbarcodeno)
                             orderby detxn.CreatedDateTime descending
                             select new BuddyBagBarcode
                             {
                                 BuddyBagBarcodeNo = detxn.BuddyBagBarcodeNo,
                                 TravelStatus = detxn.TravelStatus,
                                 Qty01 = detxn.Qty01,
                                 JobQty = detxn.JobQty,
                                 Weight = detxn.Weight,
                                 //Color = detxn.Color,
                                 TrollyNo = detxn.TrollyNo,
                                 AllocationDate = detxn.AllocationDate,
                                 Remarks = detxn.Remarks,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 CreatedBy = detxn.CreatedBy,

                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        /*
        //GET api /get DispatchDetails Status by Factory
        [Produces("application/json")]
        [HttpGet("GetBuddyTagDetails")]
        public IList<TravelBarcodeDetails> GetBuddyTagDetails(DateTime todate, string barcode, int status)
        {

            logger.InfoFormat("GetBuddyTagDetails API called with todate = {0}, barcode = {1}, status = {2}", todate, barcode, status);
            IList<TravelBarcodeDetails> L1Details = null;

            if (todate == null)
            {
                todate = DateTime.Now;
            }

            DateTime fromdate = DateTime.MinValue;
            if (todate.Year == DateTime.Now.Year && todate.Month == DateTime.Now.Month && todate.Date == DateTime.Now.Date)
            {
                todate = DateTime.Now;
            }
            else if (todate == DateTime.MinValue)
            {
                todate = DateTime.Now;
            }
            else
            {
                fromdate = todate;
                TimeSpan ts = new TimeSpan(1, 0, 0, 0);
                todate = todate.Add(ts);
            }


            try
            {
                /*L1Details = (from detxn in dcap.TravelStatus
                             join l1 in dcap.L1 on detxn.L1id equals l1.L1id
                             join l2 in dcap.L2 on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4 on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             
                             where detxn.CreatedDateTime <= todate && detxn.CreatedDateTime >= fromdate && (status == 0 ? true : detxn.TxnMode == status) &&
                             ((barcode == null || barcode == "") ? true : detxn.BarCodeNo == barcode)
                             orderby detxn.CreatedDateTime descending
                             select new TravelBarcodeDetails
                             {
                                 TravelBarCodeNo = detxn.BuddyBagBarcodeNo,
                                 TravelStatus = detxn.TxnStatus,
                                 Qty01 = detxn.Qty01,
                                 JobQty = detxn.JobQty,
                                 Weight = detxn.Weight,
                                 Color = detxn.Color,
                                 TrollyNo = detxn.TrollyNo,
                                 AllocationDate = detxn.AllocationDate,
                                 Remarks = detxn.Remarks,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 CreatedBy = detxn.CreatedBy,

                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        } */

        // GET api/Security/GetSecuser - API to get Secuser by Functions
        [Produces("application/json")]
        [HttpGet("GetUsersByFunction")]
        public IList<UserPermission> GetUsersByrFunction(string FunctionId, string FacCode)
        {
            logger.InfoFormat("GetUsersByFunction API called with userId={0}, FacCode={1}", FunctionId, FacCode);

            IList<UserPermission> userPermission = null;
            try
            {
                userPermission = (from su in dcap.Secuser
                                  join sd in dcap.Secuserrightdep on new { A = su.UserId } equals new { A = sd.UserId }
                                  join sf in dcap.Secfunction on sd.FunctionId equals sf.FunctionId
                                  where sf.FunctionId == FunctionId && sd.RecStatus == (int)eRecStatus.Active && (FacCode == null ? true : sd.FacCode == FacCode)
                                  select new UserPermission
                                  {
                                      UserId = su.UserId,
                                      UserName = su.Name,
                                      FunctionId = sd.FunctionId,
                                      FuncName = sf.FuncName,
                                      GroupCode = sd.GroupCode,
                                      SBUCode = sd.Sbucode,
                                      FacCode = sd.FacCode,
                                      DEPId = sd.Depid
                                  }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Users By Function information {0}", e.ToString());
            }
            return userPermission;
        }

        // GET api/Security/GetSecuser - API to get Secuser by Functions
        [Produces("application/json")]
        [HttpGet("GetLocationsByUser")]
        public string GetLocationsByUser(string userid)
        {
            logger.InfoFormat("GetUsersByFunction API called with userId={0}", userid);

            string locname = null;
            try
            {
                var userPermission = (from su in dcap.Secuserrightdep
                                      join sd in dcap.Location on new { A = su.FacCode } equals new { A = sd.FacCode }
                                      where su.UserId == userid
                                      select new Location
                                      {
                                          LocName = sd.LocName,
                                      }).FirstOrDefault();
                if (userPermission != null)
                {
                    locname = userPermission.LocAddress;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Users By Function information {0}", e.ToString());
            }
            return locname;
        }

        //GET api /get Production Status by Team - Report 2 - WIP Report by team - Dropdown Data Factory
        [Produces("application/json")]
        [HttpGet("GetFactoryNames")]
        public IList<GetFactoryName> GetFactoryNames()
        {

            logger.InfoFormat("Get Factory Name");
            IList<GetFactoryName> L1Details = null;

            try
            {
                L1Details = (from factory in dcap.Factory
                             join location in dcap.Location on new { A = factory.GroupCode, B = factory.FacCode, C = factory.Sbucode } equals new { A = location.GroupCode, B = location.FacCode, C = location.Sbucode }
                             group factory by new
                             {
                                 factory.FacCode,  //Factory Code
                                 factory.FacName,  //Factory Name
                                 location.LocCode,
                                 location.LocDescription,
                                 location.LocAddress,
                             }
                         into grp
                             orderby grp.Key.FacCode, grp.Key.FacName
                             select new GetFactoryName
                             {
                                 Factory_Code = grp.Key.FacCode,
                                 Factory_Name = grp.Key.FacName,
                                 Loc_Code = grp.Key.LocCode,
                                 Loc_Address = grp.Key.LocAddress,
                                 Loc_Description = grp.Key.LocDescription,
                                 IsDefault = false,
                                 IsSelected = false,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetLocationAddressByCode")]
        public IList<GetFactoryName> GetLocationAddressByCode(string inputLocCode)
        {

            logger.InfoFormat("Get Factory Name inputLocCode={0}", inputLocCode);
            IList<GetFactoryName> L1Details = null;

            try
            {
                L1Details = (from factory in dcap.Factory
                             join location in dcap.Location on new { A = factory.GroupCode, B = factory.FacCode, C = factory.Sbucode } equals new { A = location.GroupCode, B = location.FacCode, C = location.Sbucode }
                             where location.LocCode == inputLocCode
                             group factory by new
                             {
                                 factory.FacCode,  //Factory Code
                                 factory.FacName,  //Factory Name
                                 location.LocCode,
                                 location.LocDescription,
                                 location.LocAddress,
                             }
                         into grp
                             orderby grp.Key.FacCode, grp.Key.FacName
                             select new GetFactoryName
                             {
                                 Factory_Code = grp.Key.FacCode,
                                 Factory_Name = grp.Key.FacName,
                                 Loc_Code = grp.Key.LocCode,
                                 Loc_Address = grp.Key.LocAddress,
                                 Loc_Description = grp.Key.LocDescription,
                                 IsDefault = false,
                                 IsSelected = false,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //GET api /Operations/GetFactoryByWFID - API to get Factory By ID Information        
        [Produces("application/json")]
        [HttpGet("GetFactoryByWFID")]
        public WFFactories GetFactoryByWFID(uint Wfid)
        {

            logger.InfoFormat("GetFactoryByWFID API called with WFID={0}", Wfid);

            WFFactories WFFactories = null;
            try
            {
                WFFactories = (from wf in dcap.Wfdep
                               join te in dcap.Team on wf.TeamId equals te.TeamId
                               where wf.Wfid == Wfid && wf.Wfdepstatus == 1 && wf.RecStatus == (int)eRecStatus.Active
                               && wf.DataCaptureMode == 2
                               orderby wf.OperationCode, te.TeamId
                               select new WFFactories
                               {
                                   FacCode = te.FacCode,
                                   LocCode = te.LocCode,
                               }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetFactoryByWFID {0}", e.ToString());
                throw e;
            }
            return WFFactories;
        }

        //Travel Barcode Lookups
        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStyleScheduleByBagBarcode")]
        public StyleScheduleColor GetStyleScheduleByBagBarcode(string Barcode)
        {

            logger.InfoFormat("GetStyleScheduleByBagBarcode By Bag Barcode Barcode ={0}", Barcode);
            StyleScheduleColor Details = null;

            try
            {
                Details = (from bagbarcode in dcap.GroupBarcode
                           join l1 in dcap.L1 on bagbarcode.L1id equals l1.L1id
                           join l2 in dcap.L2 on new { A = bagbarcode.L1id, B = bagbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                           join l4 in dcap.L4 on new { A = bagbarcode.L1id, B = bagbarcode.L2id, C = bagbarcode.L3id, D = bagbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                           where bagbarcode.BagBarCodeNo == Barcode && l1.RecStatus == (int)eRecStatus.Active && bagbarcode.TxnMode == 1 && bagbarcode.TxnStatus == 5
                           select new StyleScheduleColor
                           {

                               Seq = bagbarcode.Seq,
                               WFId = bagbarcode.WFId,
                               WFDEPInstId = bagbarcode.WFDEPInstId,
                               Depid = bagbarcode.DEPId,
                               ReceviedDateTime = bagbarcode.ModifiedDateTime,
                               StyleId = l1.L1id,
                               StyleNo = l1.L1no,
                               StyleDesc = l1.L1desc,
                               ScheduleId = l2.L2id,
                               ScheduleNo = l2.L2no,
                               ScheduleDesc = l2.L2desc,
                               DeliveryDate = l2.DeliveryDate,
                               Zfeature = l2.Ref02,
                               PONo = l2.Ref01,
                               l3id = bagbarcode.L3id,
                               ColorId = l4.L4id,
                               ColorDesc = l4.L4desc,
                               SizeId = bagbarcode.L5id,
                               Qty01 = bagbarcode.Qty01,
                               Qty02 = bagbarcode.Qty02,
                               Qty03 = bagbarcode.Qty03,
                               Qty01NS = bagbarcode.Qty01NS,
                               Qty02NS = bagbarcode.Qty02NS,
                               Qty03NS = bagbarcode.Qty03NS,

                           }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetStyleScheduleByBagBarcode information {0}", e.ToString());

            }
            return Details;
        }

        //Travel Barcode Lookups
        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetStyleScheduleByBagBarcodeOutSource")]
        public StyleScheduleColor GetStyleScheduleByBagBarcodeOutSource(string Barcode)
        {

            logger.InfoFormat("GetStyleScheduleByBagBarcode By Bag Barcode Barcode ={0}", Barcode);
            StyleScheduleColor Details = null;

            try
            {
                Details = (from bagbarcode in dcap.GroupBarcode
                           join l1 in dcap.L1 on bagbarcode.L1id equals l1.L1id
                           join l2 in dcap.L2 on new { A = bagbarcode.L1id, B = bagbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                           join l4 in dcap.L4 on new { A = bagbarcode.L1id, B = bagbarcode.L2id, C = bagbarcode.L3id, D = bagbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                           join l5 in dcap.L5 on new { A = bagbarcode.L1id, B = bagbarcode.L2id, C = bagbarcode.L3id, D = bagbarcode.L4id, E = bagbarcode.L5id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id, E = l5.L5id }
                           where bagbarcode.BagBarCodeNo == Barcode && l1.RecStatus == (int)eRecStatus.Active && bagbarcode.TxnMode == 1 && bagbarcode.TxnStatus == 5
                           select new StyleScheduleColor
                           {

                               Seq = bagbarcode.Seq,
                               WFId = bagbarcode.WFId,
                               WFDEPInstId = bagbarcode.WFDEPInstId,
                               Depid = bagbarcode.DEPId,
                               ReceviedDateTime = bagbarcode.ModifiedDateTime,
                               StyleId = l1.L1id,
                               StyleNo = l1.L1no,
                               StyleDesc = l1.L1desc,
                               ScheduleId = l2.L2id,
                               ScheduleNo = l2.L2no,
                               ScheduleDesc = l2.L2desc,
                               DeliveryDate = l2.DeliveryDate,
                               Zfeature = l2.Ref02,
                               PONo = l2.Ref01,
                               l3id = bagbarcode.L3id,
                               ColorId = l4.L4id,
                               ColorDesc = l4.L4desc,
                               SizeId = l5.L5id,
                               SizeDesc = l5.L5desc,
                               Qty01 = bagbarcode.Qty01,
                               Qty02 = bagbarcode.Qty02,
                               Qty03 = bagbarcode.Qty03,
                               Qty01NS = bagbarcode.Qty01NS,
                               Qty02NS = bagbarcode.Qty02NS,
                               Qty03NS = bagbarcode.Qty03NS,

                           }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetStyleScheduleByBagBarcode information {0}", e.ToString());

            }
            return Details;
        }

        //GET api /style/GetStyleByID - API to get Style By ID Information      
        [Produces("application/json")]
        [HttpGet("GetQuantityByStyleScheduleColor")]
        public StyleScheduleColor GetQuantityByStyleScheduleColor(int L1id, int L2id, int L4id)
        {

            logger.InfoFormat("GetQuantity By Style Schedule Color L1id ={0} L2id ={1} L4id ={2}", L1id, L2id, L4id);
            StyleScheduleColor Details = null;

            try
            {
                Details = (from bagbarcode in dcap.GroupBarcode
                           join l1 in dcap.L1 on bagbarcode.L1id equals l1.L1id
                           join l2 in dcap.L2 on new { A = bagbarcode.L1id, B = bagbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                           join l4 in dcap.L4 on new { A = bagbarcode.L1id, B = bagbarcode.L2id, C = bagbarcode.L3id, D = bagbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }

                           where bagbarcode.L1id == L1id && bagbarcode.L2id == L2id && bagbarcode.L4id == L4id && l1.RecStatus == (int)eRecStatus.Active && bagbarcode.TxnMode == 11 && bagbarcode.TxnStatus == 5
                           select new StyleScheduleColor
                           {

                               Seq = bagbarcode.Seq,
                               WFId = bagbarcode.WFId,
                               WFDEPInstId = bagbarcode.WFDEPInstId,
                               Depid = bagbarcode.DEPId,
                               ReceviedDateTime = bagbarcode.ModifiedDateTime,
                               StyleId = l1.L1id,
                               StyleNo = l1.L1no,
                               StyleDesc = l1.L1desc,
                               ScheduleId = l2.L2id,
                               ScheduleNo = l2.L2no,
                               ScheduleDesc = l2.L2desc,
                               DeliveryDate = l2.DeliveryDate,
                               Zfeature = l2.Ref02,
                               PONo = l2.Ref01,
                               l3id = bagbarcode.L3id,
                               ColorId = l4.L4id,
                               ColorDesc = l4.L4desc,
                               SizeId = bagbarcode.L5id,
                               Qty01 = bagbarcode.Qty01,
                               Qty02 = bagbarcode.Qty02,
                               Qty03 = bagbarcode.Qty03,
                               Qty01NS = bagbarcode.Qty01NS,
                               Qty02NS = bagbarcode.Qty02NS,
                               Qty03NS = bagbarcode.Qty03NS,
                           }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetStyleScheduleByBagBarcode information {0}", e.ToString());

            }
            return Details;
        }

        //Travel Barcode Lookups
        //GET api /style/GetStyleByID - API to get Style By ID Information   

        [Produces("application/json")]
        [HttpGet("GetStyleSheduleColorforRejectedfromBarcode")]
        public StyleScheduleColor GetStyleSheduleColorforRejectedfromBarcode(string Barcode, uint WfdepinstId, int GroupMode)
        {

            logger.InfoFormat("Get Style Shedule Color for Rejected from Barcode By Bag Barcode Barcode ={0} WfdepinstId={1} GroupMode={2}", Barcode, WfdepinstId, GroupMode);
            StyleScheduleColor Color = null;
            L4 L4 = null;
            Detxn objDetxn = null;

            LookupController lookup = new LookupController(dcap);

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
                    Color.RetiriveSuccessfull = true;
                    if (Color.ColorId != 0)
                    {
                        L4 = (from l4 in dcap.L4
                              where l4.L1id == Color.StyleId && l4.L2id == Color.ScheduleId && l4.L4id == Color.ColorId
                              select new L4
                              {
                                  L4no = l4.L4no,
                                  L4desc = l4.L4desc

                              }).FirstOrDefault();

                        if (L4 != null)
                        {
                            Color.ColorNo = L4.L4no;
                            Color.ColorDesc = L4.L4desc;
                        }
                    }
                    else
                    {
                        Color.ColorNo = "-N/A-";
                    }

                    Wfdep Wfd = lookup.GetWFConfigurationbyID(WfdepinstId);
                    objDetxn = lookup.GetDETxnOppQtybyBarcode(Barcode, Wfd.Depid);
                    if (GroupMode == 3)
                    {
                        if (objDetxn.Qty03 == 0)
                        {
                            Color.Responce = new string[2];
                            Color.Responce[0] = "No records found on Rework.";
                            Color.Responce[1] = "No records found on Rework.";
                            Color.RetiriveSuccessfull = false;
                        }
                        else
                        {
                            Color.RetiriveSuccessfull = true;
                        }
                    }
                    else
                    {
                        if (objDetxn.Qty02 == 0)
                        {
                            Color.Responce = new string[2];
                            Color.Responce[0] = "No records found on Scrap.";
                            Color.Responce[1] = "No records found on Scrap.";
                            Color.RetiriveSuccessfull = false;
                        }
                        else
                        {
                            Color.RetiriveSuccessfull = true;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());

            }
            return Color;
        }


        //GET api /style/GetStatusByBarcode - API to get Style Status By Barcode Information - Report 5     
        [Produces("application/json")]
        [HttpGet("GetStatusByBagBarcode")]
        public IList<BarcodeStatus> GetStatusByBagBarcode(string Barcode)
        {

            logger.InfoFormat("Get Status By Bag Barcode with Barcode ={0}", Barcode);
            IList<BarcodeStatus> Details = null;

            try
            {
                Details = (from detxn in dcap.Detxn
                               //join dep in dcap.Dep on detxn.Depid equals dep.Depid
                               //join l1 in dcap.L1 on (uint?)detxn.L1id equals (uint?)l1.L1id
                               //join l2 in dcap.L2 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                               //join l4 in dcap.L4 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                               //join l5 in dcap.L5 on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
                           where detxn.BagBarCodeNo == Barcode
                           select new BarcodeStatus
                           {
                               Transaction_Date_and_Time = detxn.TxnDateTime,
                               BarCodeNo = detxn.BarCodeNo,
                               //Operation = grp.Key.OperationCode,
                               //Department = dep.Depdesc,
                               //Style = l1.L1desc,
                               //Schedule_ID = l2.L2desc,
                               //Purchase_Order = l2.Ref01,
                               //Color = l4.L4desc,
                               //Size = l5.L5desc,
                           }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                //throw e;
            }
            return Details;
        }

        //GET api /style/GetStatusByBarcode - API to get Style Status By Barcode Information - Report 5     
        [Produces("application/json")]
        [HttpGet("GetQuantityByDetails")]
        public decimal GetQuantityByDetails(int mode, uint l1id, uint l2id, uint l3id, uint l4id, uint l5id, int opcode1, int opcode2)
        {

            logger.InfoFormat("Get Cut Quantity By Details with l1id ={0}, l2id ={1}, l3id ={2}, l4id ={3} opcode1={4} opcode2={5} mode={6}", l1id, l2id, l3id, l4id, opcode1, opcode2, mode);
            decimal cuttingwashingsendwip = 0;

            try
            {
                if (mode == 0)
                {
                    decimal totalop1qty = (decimal)dcap.L5moops.Where(l5moops => l5moops.L1id == l1id && l5moops.L2id == l2id && l5moops.L3id == l3id && l5moops.L4id == l4id && l5moops.OperationCode == opcode1).Sum(c => c.ReportedQty);

                    decimal totalop2qty = (decimal)dcap.Dedep.Where(dedep => dedep.L1id == l1id && dedep.L2id == l2id && dedep.L3id == l3id && dedep.L4id == l4id && dedep.OperationCode == opcode2).Sum(c => c.Qty01);

                    cuttingwashingsendwip = totalop1qty - totalop2qty;
                }
                else if (mode == 2)
                {
                    cuttingwashingsendwip = (decimal)dcap.Dedep.Where(dedep => dedep.OperationCode == opcode2 && dedep.L1id == l1id && dedep.L2id == l2id && dedep.L3id == l3id && dedep.L4id == l4id).Sum(c => c.Qty01);
                }
                else if (mode == 3)
                {
                    cuttingwashingsendwip = (decimal)dcap.L5moops.Where(l5moops => l5moops.L1id == l1id && l5moops.L2id == l2id && l5moops.L3id == l3id && l5moops.L4id == l4id && l5moops.OperationCode == opcode1).Sum(c => c.ReportedQty);
                }
                else if (mode == 4)
                {
                    cuttingwashingsendwip = (decimal)dcap.L5moops.Where(l5moops => l5moops.L1id == l1id && l5moops.L2id == l2id && l5moops.L3id == l3id && l5moops.L4id == l4id && l5moops.L5id == l5id && l5moops.OperationCode == opcode1).Sum(c => c.ReportedQty);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return cuttingwashingsendwip;
        }

        //GET api /style/GetBagBarcodeByBarcode Get Latest Assigned Bag Barcode if Assign Status is Avilabale (When Sum Qty=0)
        //Used API's and UI : BagBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("GetBagBarcodeByBarcode")]
        public Detxn GetBagBarcodeByBarcode(string barcode, int l1id, int l2id, int l3id, int l4id)
        {

            logger.InfoFormat("GetBagBarcodeByBarcode with barcode ={0} l1id={1} l2id={2} l3id={3} l4id={4}", barcode, l1id, l2id, l3id, l4id);
            Detxn bagbarcode = null;

            try
            {
                List<Detxn> prebagbarcode = (from detxn in dcap.Detxn
                                             where detxn.BarCodeNo == barcode && detxn.L1id == l1id && detxn.L2id == l2id && detxn.L3id == l3id
                                             && detxn.L4id == l4id && detxn.BagBarCodeNo != ""
                                             orderby detxn.DetxnKey descending
                                             select new Detxn
                                             {
                                                 DetxnKey = detxn.DetxnKey,
                                                 BagBarCodeNo = detxn.BagBarCodeNo,
                                                 Seq = detxn.Seq,
                                                 Wfid = detxn.Wfid,
                                                 WfdepinstId = detxn.WfdepinstId,
                                                 Depid = detxn.Depid,
                                                 L1id = detxn.L1id,
                                                 L2id = detxn.L2id,
                                                 L3id = detxn.L3id,
                                                 L4id = detxn.L4id,
                                                 L5id = detxn.L5id,
                                                 Qty01 = detxn.Qty01,
                                             }).ToList();
                if (prebagbarcode.Count > 0)
                {
                    decimal? totalqty = prebagbarcode.Select(c => c.Qty01).Sum();
                    if (totalqty != 0)
                    {
                        bagbarcode = prebagbarcode[0];
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return bagbarcode;
        }

        //GET api /style/GetBuddyBarcodeByBarcode Get Latest Assigned Buddy Barcode if Assign Status is Avilabale (When Sum Qty=0)
        //Used API's and UI : BuddyBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("GetBuddyBarcodeByBarcode")]
        public Detxn GetBuddyBarcodeByBarcode(string barcode, int l1id, int l2id, int l3id, int l4id, string TravelBarcode, int rrType, string rrID)
        {

            logger.InfoFormat("GetBuddyBarcodeByBarcode with barcode ={0}", barcode, l1id, l2id, l3id, l4id, TravelBarcode, rrType, rrID);
            Detxn bagbarcode = null;

            try
            {
                List<Detxn> prebagbarcode = (from detxn in dcap.Detxn
                                             where detxn.BarCodeNo == barcode && detxn.BagBarCodeNo != "" && detxn.L1id == l1id && detxn.L2id == l2id && detxn.L3id == l3id
                                             && detxn.L4id == l4id && detxn.JobNo == TravelBarcode && detxn.TxnMode == rrType && detxn.Rrid == Convert.ToInt32(rrID)
                                             orderby detxn.DetxnKey descending
                                             select new Detxn
                                             {
                                                 DetxnKey = detxn.DetxnKey,
                                                 BagBarCodeNo = detxn.BagBarCodeNo,
                                                 Seq = detxn.Seq,
                                                 Wfid = detxn.Wfid,
                                                 Depid = detxn.Depid,
                                                 L1id = detxn.L1id,
                                                 L2id = detxn.L2id,
                                                 L3id = detxn.L3id,
                                                 L4id = detxn.L4id,
                                                 L5id = detxn.L5id,
                                                 Qty01 = detxn.Qty01,
                                                 Qty02 = detxn.Qty02,
                                                 Qty03 = detxn.Qty03,
                                             }).ToList();
                if (prebagbarcode.Count > 0)
                {
                    decimal? totalqtyr = prebagbarcode.Select(c => c.Qty03).Sum();
                    decimal? totalqtys = prebagbarcode.Select(c => c.Qty02).Sum();

                    if (totalqtyr != 0 || totalqtys != 0)
                    {
                        bagbarcode = prebagbarcode[0];
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return bagbarcode;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BagBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingBagBarcodeCounterExsistence")]
        public TeamCounter CheckForOngoingBagBarcodeCounterExsistence(uint WfdepinstId, int styleId, int sheduleId, int colorId, string BagBarcode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, BagBarcode={4}", WfdepinstId, styleId, sheduleId, colorId, BagBarcode);
            TeamCounter objteamcounter = new TeamCounter();

            try
            {
                if (BagBarcode != null)
                {
                    objteamcounter = dcap.TeamCounter.Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId && c.BagBarCodeNo == BagBarcode).FirstOrDefault();
                }
                else
                {
                    objteamcounter = dcap.TeamCounter.Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BagBarcodeCheckerNonApperal (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingBagBarcodeCounterExsistenceForNonApperal")]
        public TeamCounter CheckForOngoingBagBarcodeCounterExsistenceForNonApperal(uint WfdepinstId, int styleId, int sheduleId, int colorId, int sizId, string BagBarcode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, sizId={4}, BagBarcode={5}", WfdepinstId, styleId, sheduleId, colorId, sizId, BagBarcode);
            TeamCounter objteamcounter = new TeamCounter();

            try
            {
                if (BagBarcode != null)
                {
                    objteamcounter = dcap.TeamCounter.Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId && c.L5id == sizId && c.BagBarCodeNo == BagBarcode).FirstOrDefault();
                }
                else
                {
                    objteamcounter = dcap.TeamCounter.Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId && c.L5id == sizId).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BuddyBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingBagBarcodeCounterExsistence")]
        public BuudyTagCounter CheckForOngoingBuddyBarcodeCounterExsistence(uint WfdepinstId, int styleId, int sheduleId, int colorId, string BagBarcode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, BagBarcode={4}", WfdepinstId, styleId, sheduleId, colorId, BagBarcode);
            BuudyTagCounter objteamcounter = new BuudyTagCounter();

            try
            {
                objteamcounter = dcap.BuudyTagCounter
                        .Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId
                        && (BagBarcode != null ? c.BagBarCodeNo == BagBarcode : true))
                        .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BuddyBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingBuddyBarcodeCounterExsistence")]
        public BuudyTagCounter CheckForOngoingBuddyBarcodeCounterExsistence(uint WfdepinstId, int styleId, int sheduleId, int colorId, string BagBarcode, string TravelBarcode, int rrType, string rrID)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, BagBarcode={4}", WfdepinstId, styleId, sheduleId, colorId, BagBarcode);
            BuudyTagCounter objteamcounter = new BuudyTagCounter();

            try
            {
                objteamcounter = dcap.BuudyTagCounter
                        .Where(c => c.WfdepinstId == WfdepinstId && c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId && c.TravelBarCodeNo == TravelBarcode && c.RRType == rrType && c.RRId == Convert.ToInt16(rrID)
                        && (BagBarcode != null ? c.BagBarCodeNo == BagBarcode : true))
                        .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BagBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingBuddyGroupBarcodeCounterExsistence")]
        public BuudyTagCounter CheckForOngoingBuddyGroupBarcodeCounterExsistence(uint WfdepinstId, int styleId, int sheduleId, int colorId, string BagBarcode, int OperationCode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, BagBarcode={4}", WfdepinstId, styleId, sheduleId, colorId, BagBarcode);
            BuudyTagCounter objteamcounter = new BuudyTagCounter();

            try
            {
                objteamcounter = dcap.GroupBarcode
                        .Where(c => c.L1id == styleId && c.L2id == sheduleId && c.L4id == colorId
                        && c.WorkCenter == BagBarcode && c.TxnMode == 3 && c.TxnStatus < OperationCode).Select(c => new BuudyTagCounter { BagBarCodeNo = c.BagBarCodeNo, CounterId = 0, TravelBarCodeNo = c.WorkCenter })
                        .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BagBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingGroupBarcodeCounterExsistence")]
        public TeamCounter CheckForOngoingGroupBarcodeCounterExsistence(uint WfdepinstId, int styleId, int sheduleId, int colorId, string BagBarcode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, BagBarcode={4}", WfdepinstId, styleId, sheduleId, colorId, BagBarcode);
            TeamCounter objteamcounter = new TeamCounter();

            try
            {
                var op = dcap.Wfdep.Where(c => c.WfdepinstId == WfdepinstId).AsQueryable().Select(c => (int?)c.OperationCode).FirstOrDefault();

                objteamcounter = dcap.GroupBarcode.Where(c => c.L1id == styleId && c.L2id == sheduleId && c.L3id == 0 && c.L4id == colorId && c.OperationCode == (int?)op && c.BagBarCodeNo == BagBarcode && c.TxnMode == 1 && c.TxnStatus == 0).Select(c => new TeamCounter { BagBarCodeNo = c.BagBarCodeNo, CounterId = 0 }).FirstOrDefault();
            //objteamcounter = dcap.GroupBarcode.Where(c => c.L1id == styleId && c.L2id == sheduleId && c.L3id == 0 && c.L4id == colorId && c.BagBarCodeNo == BagBarcode && c.TxnMode == 1 && c.TxnStatus == 0).Select(c => new TeamCounter { BagBarCodeNo = c.BagBarCodeNo, CounterId = 0 }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Ongoing Counter data in team Counter Table
        //Used API's and UI : BagBarcodeChecker (Business Controller)
        [Produces("application/json")]
        [HttpGet("CheckForOngoingGroupBarcodeCounterExsistenceNoApperal")]
        public TeamCounter CheckForOngoingGroupBarcodeCounterExsistenceNoApperal(uint WfdepinstId, int styleId, int sheduleId, int colorId, int sizeId, string BagBarcode)
        {

            logger.InfoFormat("Get Cut Quantity By Details with WfdepinstId ={0}, styleId ={1}, sheduleId ={2}, colorId ={3}, sizeId={4}, BagBarcode={5}", WfdepinstId, styleId, sheduleId, colorId, sizeId, BagBarcode);
            TeamCounter objteamcounter = new TeamCounter();

            try
            {
                objteamcounter = dcap.GroupBarcode.Where(c => c.L1id == styleId && c.L2id == sheduleId && c.L3id == 0 && c.L4id == colorId && c.L5id == sizeId && c.BagBarCodeNo == BagBarcode && c.TxnMode == 1 && c.TxnStatus == 0).Select(c => new TeamCounter { BagBarCodeNo = c.BagBarCodeNo, CounterId = 0 }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objteamcounter;
        }

        //GET Request Data
        [Produces("application/json")]
        [HttpGet("GetRequestyDetails")]
        public GoodControlDetails GetRequestyDetails(string controlid, int seq, int depid)
        {

            logger.InfoFormat("Get Cut Quantity By Details with controlid ={0}, seq ={1}, depid ={2}", controlid, seq, depid);
            GoodControlDetails objdetxn = new GoodControlDetails();

            try
            {
                objdetxn = dcap.GoodControlDetails
                        .Where(c => c.Seq == seq && c.ControlId == controlid && c.Depid == depid)
                        .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objdetxn;
        }

        //Get all colors of Travelbarcodes
        [Produces("application/json")]
        [HttpGet("GetColors")]
        public IList<TravelBarcodeDetails> GetColors(string Color)
        {

            logger.InfoFormat("GetColorsByStylesNo Style Color={1}", Color);
            IList<TravelBarcodeDetails> Colors = null;

            Color = Color == null ? "%" : Color;

            try
            {

                Colors = (from tbd in dcap.TravelBarcodeDetails
                          where EF.Functions.Like(tbd.Color, "%" + Color + "%")
                          group tbd by new { tbd.Color }
                          into grp
                          select new TravelBarcodeDetails
                          {
                              Color = grp.Key.Color,
                          }).ToList();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return Colors;
        }

        //Get next dispatch id
        [Produces("application/json")]
        [HttpGet("GetDispatchId")]
        public String GetDispatchId()
        {
            logger.InfoFormat("GetDispatchId Style");

            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            String dy = datevalue.Day.ToString();
            String mn = datevalue.Month.ToString();
            String yy = datevalue.Year.ToString();

            if (mn.Length == 1)
            {
                mn = "0" + mn;
            }

            if (dy.Length == 1)
            {
                dy = "0" + dy;
            }

            String datekey = yy + mn + dy;

            String output = "";

            DateTime fromdate = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, 0, 0, 0);
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            DateTime todate = fromdate.Add(ts);

            try
            {

                var ExsistIds = (from d in dcap.GoodControlDetails
                                 where d.TxnDateTime >= fromdate && d.TxnDateTime <= todate
                                 select new
                                 {
                                     ControlId = Convert.ToInt32(d.ControlId.Substring(datekey.Length, d.ControlId.Length)),
                                 }).ToList();

                if (ExsistIds.Count != 0)
                {
                    int maxid = ExsistIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 4 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }
                    output = datekey + maxtextid;
                }
                else
                {
                    output = datekey + "0001";
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        //Get next Travel barcode no.
        [Produces("application/json")]
        [HttpGet("GetTravelBarcode")]
        public String GetTravelBarcode()
        {
            logger.InfoFormat("GetTravelBarcode");

            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            String dy = datevalue.Day.ToString();
            String mn = datevalue.Month.ToString();
            String yy = datevalue.Year.ToString();

            if (mn.Length == 1)
            {
                mn = "0" + mn;
            }

            if (dy.Length == 1)
            {
                dy = "0" + dy;
            }

            String datekey = yy + mn + dy;

            String output = "";

            DateTime fromdate = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, 0, 0, 0);
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            DateTime todate = fromdate.Add(ts);

            try
            {

                var ExsistIds = (from d in dcap.TravelBarcodeDetails
                                 where d.CreatedDateTime >= fromdate && d.CreatedDateTime <= todate && d.TxnMode == 2
                                 select new
                                 {
                                     ControlId = Convert.ToInt32(d.TravelBarCodeNo.Substring(datekey.Length + 4, d.TravelBarCodeNo.Length)),
                                 }).ToList();

                if (ExsistIds.Count != 0)
                {
                    int maxid = ExsistIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 4 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }
                    output = "2002" + datekey + maxtextid;
                }
                else
                {
                    output = "2002" + datekey + "0001";
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        //Get next Travel barcode no.
        [Produces("application/json")]
        [HttpGet("GetTravelBarcodeOutSource")]
        public String GetTravelBarcodeOutSource()
        {
            logger.InfoFormat("GetTravelBarcode");

            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            String dy = datevalue.Day.ToString();
            String mn = datevalue.Month.ToString();
            String yy = datevalue.Year.ToString();

            if (mn.Length == 1)
            {
                mn = "0" + mn;
            }

            if (dy.Length == 1)
            {
                dy = "0" + dy;
            }

            String datekey = yy + mn + dy;

            String output = "";

            DateTime fromdate = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, 0, 0, 0);
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            DateTime todate = fromdate.Add(ts);

            try
            {

                var ExsistIds = (from d in dcap.TravelBarcodeDetails
                                 where d.CreatedDateTime >= fromdate && d.CreatedDateTime <= todate && d.TxnMode == 2
                                 select new
                                 {
                                     ControlId = Convert.ToInt32(d.TravelBarCodeNo.Substring(datekey.Length + 4, d.TravelBarCodeNo.Length)),
                                 }).ToList();

                if (ExsistIds.Count != 0)
                {
                    int maxid = ExsistIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 4 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }
                    output = "2003" + datekey + maxtextid;
                }
                else
                {
                    output = "2003" + datekey + "0001";
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        //Get Next Buddy Barcode No.
        [Produces("application/json")]
        [HttpGet("GetBuddyBarcode")]
        public String GetBuddyBarcode()
        {
            logger.InfoFormat("GetBuddyBarcode");

            String sDate = DateTime.Now.ToString();
            DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));

            String dy = datevalue.Day.ToString();
            String mn = datevalue.Month.ToString();
            String yy = datevalue.Year.ToString();

            if (mn.Length == 1)
            {
                mn = "0" + mn;
            }

            if (dy.Length == 1)
            {
                dy = "0" + dy;
            }

            String datekey = yy + mn + dy;

            String output = "";

            DateTime fromdate = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, 0, 0, 0);
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            DateTime todate = fromdate.Add(ts);

            try
            {

                var ExsistIds = (from d in dcap.TravelBarcodeDetails
                                 where d.CreatedDateTime >= fromdate && d.CreatedDateTime <= todate && d.TxnMode == 3
                                 select new
                                 {
                                     ControlId = Convert.ToInt32(d.TravelBarCodeNo.Substring(datekey.Length + 4, d.TravelBarCodeNo.Length)),
                                 }).ToList();

                if (ExsistIds.Count != 0)
                {
                    int maxid = ExsistIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 4 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }
                    output = "3003" + datekey + maxtextid;
                }
                else
                {
                    output = "3003" + datekey + "0001";
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        public TravelBarcodeDetails GetColorByTravelBarcode(string TravelBarcode)
        {
            TravelBarcodeDetails output = new TravelBarcodeDetails();
            try
            {
                output = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == TravelBarcode).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;

        }

        [Produces("application/json")]
        [HttpGet("GetBuddyBarcodeByInstId")]
        public String GetBuddyBarcodeByInstId(int WfdepinstId, int CounterNumber)
        {
            logger.InfoFormat("GetBuddyBarcodeByInstId WfdepinstId={0} CounterNumber={1}", WfdepinstId, CounterNumber);

            DateTime datevalue = DateTime.Now;
            DateTime initialdate = new DateTime(2020, 01, 01);
            TimeSpan spanner = datevalue - initialdate;
            String DayKey = Convert.ToString((spanner.TotalDays) * 1000000);

            String datekey = DayKey.Substring(0, 6); //

            String output = "";
            String output1 = "";
            String output2 = "";

            DateTime fromdate = new DateTime(datevalue.Year, datevalue.Month, datevalue.Day, 0, 0, 0);
            TimeSpan ts = new TimeSpan(1, 0, 0, 0);
            DateTime todate = fromdate.Add(ts);

            try
            {

                var ExsistIds = (from d in dcap.TravelBarcodeDetails
                                 where d.CreatedDateTime >= fromdate && d.CreatedDateTime <= todate && d.TxnMode == 3
                                 select new
                                 {
                                     ControlId = Convert.ToInt32(d.TravelBarCodeNo.Substring(datekey.Length + 6, d.TravelBarCodeNo.Length)),
                                 }).ToList();

                var ExsistCounterIds = (from d in dcap.BuudyTagCounter
                                        select new
                                        {
                                            ControlId = Convert.ToInt32(d.TravelBarCodeNo.Substring(datekey.Length + 6, d.TravelBarCodeNo.Length)),
                                        }).ToList();

                var wfinst = Convert.ToString(WfdepinstId);

                /*
                var counternum = Convert.ToString(CounterNumber);
                int counternumlen = 3 - counternum.Length;
                int k = 0;
                while (k < counternumlen)
                {
                    counternum = "0" + counternum;
                    k++;
                }*/

                if (ExsistIds.Count != 0)
                {
                    int maxid = ExsistIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 3 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }

                    output1 = maxtextid;
                }
                else
                {
                    output1 = "001";
                }

                if (ExsistCounterIds.Count != 0)
                {
                    int maxid = ExsistCounterIds.Max(x => x.ControlId);
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 3 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }

                    output2 = maxtextid;
                }
                else
                {
                    output2 = "001";
                }

                output = wfinst + output1 + datekey + output2;

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        //Get next Travel barcode no.
        [Produces("application/json")]
        [HttpGet("GetNextInvoiceNumber")]
        public String GetNextInvoiceNumber()
        {
            logger.InfoFormat("GetNextInvoiceNumber");

            DateTime TodaysDate = DateTime.Now;
            String output = null;

            try
            {
                var ExsistIds = (from invoiceparameters in dcap.InvoiceParameter
                                 where invoiceparameters.EffectiveDateFrom <= TodaysDate && invoiceparameters.EffectiveDateTo >= TodaysDate
                                 select new
                                 {
                                     NextInvoiceNo = Convert.ToInt32(invoiceparameters.NextInvoiceNo) //Convert.ToInt32(invoiceparameters.TravelBarCodeNo.Substring(datekey.Length + 4, d.TravelBarCodeNo.Length)),
                                 }).FirstOrDefault();

                if (ExsistIds != null)
                {
                    int maxid = ExsistIds.NextInvoiceNo;
                    var maxtextid = Convert.ToString(maxid + 1);
                    int len = 5 - maxtextid.Length;
                    int i = 0;
                    while (i < len)
                    {
                        maxtextid = "0" + maxtextid;
                        i++;
                    }
                    output = maxtextid;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        [Produces("application/json")]
        [HttpGet("GetInvoiceNumber")]

        public InvoiceParameter GetInvoiceNumber()
        {
            logger.InfoFormat("GetInvoiceNumber");

            DateTime TodaysDate = DateTime.Now;
            InvoiceParameter output = null;

            try
            {
                output = dcap.InvoiceParameter.Where(invoiceparameters => invoiceparameters.EffectiveDateFrom <= TodaysDate && invoiceparameters.EffectiveDateTo >= TodaysDate).FirstOrDefault();
                /*
                if (ExsistIds != null)
                {
                    output = ExsistIds.NextInvoiceNo;
                }
                */
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        public int GetNextInvoiceSeq()
        {
            logger.InfoFormat("GetNextInvoiceSeq");

            DateTime TodaysDate = DateTime.Now;
            int output = 0;

            try
            {
                var ExsistIds = (from d in dcap.InvoiceDetails
                                 select new
                                 {
                                     InvoiceSeq = d.InvoiceSeq,
                                 }).ToList();

                if (ExsistIds.Count != 0)
                {
                    output = ExsistIds.Max(x => x.InvoiceSeq) + 1;
                }
                else
                {
                    output = 0;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        public String GetExsistingInvoiceNumber(int InvoiceSeq, uint L1id, uint L2id)
        {
            logger.InfoFormat("Get Exsisting Invoice Number InvoiceSeq={0}, L1id={1} L2id={2}", InvoiceSeq, L1id, L2id);
            String InvoiceNo = null;

            try
            {
                InvoiceNo = dcap.InvoiceDetails.Where(d => d.InvoiceSeq == InvoiceSeq && d.L1Id == L1id && d.L2Id == L2id).Select(d => d.InvoiceNo).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return InvoiceNo;
        }

        [Produces("application/json")]
        [HttpGet("GetGroupBarcodeDetails")]
        public IList<TeamCounterCM> GetBagBarcodeDetails(string controlId, int seq, int smode)
        {

            logger.InfoFormat("Get Factory Name controlId={0}, seq={1}, smode={2}", controlId, seq, smode);
            IList<TeamCounterCM> L1Details = null;

            try
            {
                if (smode == 2)
                {
                    L1Details = (from detxn in dcap.GoodControl
                                 join l1 in dcap.L1 on detxn.L1id equals l1.L1id
                                 join l2 in dcap.L2 on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                 join l4 in dcap.L4 on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                 join l5 in dcap.L5 on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id, E = detxn.L5id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id, E = l5.L5id }
                                 where detxn.ControlId == controlId
                                 orderby l1.L1id, l2.L2id, l4.L4id
                                 select new TeamCounterCM
                                 {
                                     Seq = detxn.Seq,
                                     BagBarCode = detxn.BarCodeNo,
                                     TxnMode = detxn.TxnMode,
                                     StyleId = detxn.L1id,
                                     StyleDesc = l1.L1desc,
                                     ScheduleId = detxn.L2id,
                                     ScheduleDesc = l2.L2desc,
                                     PONo = l2.Ref01,
                                     Zfeature = l2.Ref02,
                                     ColorId = l4.L4id,
                                     SizeDesc = l5.L5desc,
                                     ColorDesc = l4.L4desc,
                                     Qty01 = (int)detxn.Qty01,
                                     BagStatus = (int)detxn.IsSucess,
                                     Remark = detxn.Remark,
                                     Location = detxn.WarLocCode
                                 }).ToList();

                }
                else
                {
                    L1Details = (from detxn in dcap.GoodControl
                                 join l1 in dcap.L1 on detxn.L1id equals l1.L1id
                                 join l2 in dcap.L2 on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                 join l4 in dcap.L4 on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                 where detxn.ControlId == controlId
                                 orderby l1.L1id, l2.L2id, l4.L4id
                                 select new TeamCounterCM
                                 {
                                     Seq = detxn.Seq,
                                     BagBarCode = detxn.BarCodeNo,
                                     TxnMode = detxn.TxnMode,
                                     StyleId = detxn.L1id,
                                     StyleDesc = l1.L1desc,
                                     ScheduleId = detxn.L2id,
                                     ScheduleDesc = l2.L2desc,
                                     PONo = l2.Ref01,
                                     Zfeature = l2.Ref02,
                                     ColorId = l4.L4id,
                                     ColorDesc = l4.L4desc,
                                     Qty01 = (int)detxn.Qty01,
                                     BagStatus = (int)detxn.IsSucess,
                                     Remark = detxn.Remark,
                                     Location = detxn.WarLocCode
                                 }).ToList();

                }
                /*foreach (GoodControl gc in L1Details) {
                    if(gc.TxnMode != 1) {
                        
                    }
                }*/
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public List<GoodControl> GetBarcodeDetailsbyControlId(string controlId, int seq)
        {

            logger.InfoFormat("Get Factory Name controlId={0}, seq={1}", controlId, seq);
            List<GoodControl> L1Details = null;

            try
            {
                L1Details = dcap.GoodControl.Where(c => c.ControlId == controlId && c.Return == 0).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetGroupBarcodeDetailsbyGroupBarcode")]
        public IList<TeamCounterCM> GetGroupBarcodeDetailsbyGroupBarcode(string barcode, string faccode)
        {

            logger.InfoFormat("Get Factory Name barcode={0} faccode={1}", barcode, faccode);
            IList<TeamCounterCM> L1Details = null;

            try
            {
                IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.BarCodeNo == barcode).AsQueryable()
                                                   join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                   select new GoodControl
                                                   {
                                                       Seq = gdc.Seq,
                                                       ControlId = gdc.ControlId,
                                                       L1id = gdc.L1id,
                                                       L2id = gdc.L2id,
                                                       L3id = gdc.L3id,
                                                       L4id = gdc.L4id,
                                                       L5id = gdc.L5id,
                                                       BarCodeNo = gdc.BarCodeNo,
                                                       TxnMode = gdc.TxnMode,
                                                       TxnStatus = gdc.TxnStatus,
                                                       WarLocCode = gdc.WarLocCode,
                                                       RecStatus = gdc.RecStatus,
                                                       Return = gdc.Return,
                                                       Qty01 = gdc.Qty01,
                                                       IsSucess = gdc.IsSucess
                                                   }).ToList();

                L1Details = (from detxn in goodControls
                             join l1 in dcap.L1 on detxn.L1id equals l1.L1id
                             join l2 in dcap.L2 on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4 on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             where detxn.BarCodeNo == barcode && detxn.RecStatus == 1 && detxn.Return == 0
                             orderby l1.L1id, l2.L2id, l4.L4id
                             select new TeamCounterCM
                             {
                                 Seq = detxn.Seq,
                                 BagBarCode = detxn.BarCodeNo,
                                 ControlId = detxn.ControlId,
                                 TxnStatus = detxn.TxnStatus,
                                 TxnMode = detxn.TxnMode,
                                 StyleId = detxn.L1id,
                                 StyleDesc = l1.L1desc,
                                 ScheduleId = detxn.L2id,
                                 ScheduleDesc = l2.L2desc,
                                 PONo = l2.Ref01,
                                 Zfeature = l2.Ref02,
                                 ColorId = l4.L4id,
                                 ColorDesc = l4.L4desc,
                                 Qty01 = (int)detxn.Qty01,
                                 BagStatus = (int)detxn.IsSucess,
                             }).ToList();

                /*foreach (GoodControl gc in L1Details) {
                    if(gc.TxnMode != 1) {
                        
                    }
                }*/
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Check that the barcode is ever used before
        //Used API's and UI : CreateGroup
        [Produces("application/json")]
        [HttpGet("CheckForBagBarcode")]
        public Boolean CheckForBagBarcode(string bagbarcode, int mode)
        {
            logger.InfoFormat("CheckForBagBarcode bagbarcode={0} mode={1}", bagbarcode, mode);
            Boolean nobarcodes = true;
            try
            {
                if (mode == 2)
                {
                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = true;
                    }
                    else
                    {
                        nobarcodes = false;
                    }

                }
                else if (mode == 1)
                {
                    var ExsistCounterIds = (from d in dcap.TeamCounter
                                            where d.BagBarCodeNo == bagbarcode
                                            select new
                                            {
                                                bagbarcode = d.BagBarCodeNo,
                                            }).ToList();

                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistCounterIds.Count == 0 && ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = true;
                    }
                    else
                    {
                        nobarcodes = false;
                    }
                }
                else if (mode == 3)
                {
                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode && d.TxnStatus > 0
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = true;
                    }
                    else
                    {
                        nobarcodes = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return nobarcodes;
        }

        //Check that the barcode is ever used before
        //Used API's and UI : CreateGroup
        [Produces("application/json")]
        [HttpGet("CheckForBuddyBarcode")]
        public Boolean CheckForBuddyBarcode(string bagbarcode)
        {
            logger.InfoFormat("CheckForBuddyBarcode bagbarcode={0}", bagbarcode);
            Boolean nobarcodes = true;
            try
            {

                var ExsistInventoryIds = (from d in dcap.BuddyBagBarcode
                                          where d.BuddyBagBarcodeNo == bagbarcode
                                          select new
                                          {
                                              bagbarcode = d.BuddyBagBarcodeNo,
                                          }).ToList();

                var ExsistCounterIds = (from d in dcap.BuudyTagCounter
                                        where d.BagBarCodeNo == bagbarcode
                                        select new
                                        {
                                            bagbarcode = d.BagBarCodeNo,
                                        }).ToList();

                if (ExsistInventoryIds.Count == 0 || ExsistCounterIds.Count == 0)
                {
                    nobarcodes = true;
                }
                else
                {
                    nobarcodes = false;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return nobarcodes;
        }

        [Produces("application/json")]
        [HttpGet("GetLocaionsByUserFactory")]
        public IList<WarehouseLocation> GetLocaionsByUserFactory(string faccode)
        {

            logger.InfoFormat("Get Factory Name faccode={0}", faccode);
            IList<WarehouseLocation> L1Details = null;

            try
            {
                L1Details = dcap.WarehouseLocation
                        .Where(c => c.FacCode == faccode)
                        .ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Get Warehouselayout by Factory
        //Get the layout by grouped tables for warehouse location
        //Used API's and UI :  (Good Recive)
        [Produces("application/json")]
        [HttpGet("GetAllLocaionsByUserFactory")]
        public IEnumerable<IGrouping<string, temp>> GetAllLocaionsByUserFactory(string faccode)
        {

            logger.InfoFormat("Get Factory Name faccode={0}", faccode);
            IEnumerable<IGrouping<string, temp>> byWarloc = null;
            List<temp> L2Details = new List<temp>();
            LookupController lookup = new LookupController(dcap);

            try
            {
                List<temp> L1Details = (from detxn in dcap.WarehouseLocation
                                        where detxn.FacCode == faccode && detxn.RecStatus == 1
                                        orderby detxn.WarCode ascending
                                        select new temp
                                        {
                                            WarCode = detxn.WarCode,
                                            WarLocCode = detxn.WarLocCode,
                                            Qty01 = 0
                                        }).ToList();
                //LDetails.ForEach(x => x.Qty01 = lookup.GetQtybyLocode(x.WarLocCode));

                //LDetails.ForEach(x => x.Qty01 = lookup.GetQtybyLocode(x.WarLocCode));
                //byWarloc = LDetails.GroupBy(x => x.WarCode);
                IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(g => g.WarLocCode != null && g.TxnStatus == 5 && g.Return == 0).AsQueryable()
                                                   join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                   select new GoodControl
                                                   {
                                                       L1id = gdc.L1id,
                                                       L2id = gdc.L2id,
                                                       L3id = gdc.L3id,
                                                       L4id = gdc.L4id,
                                                       L5id = gdc.L5id,
                                                       BarCodeNo = gdc.BarCodeNo,
                                                       TxnMode = gdc.TxnMode,
                                                       WarLocCode = gdc.WarLocCode,
                                                       RecStatus = gdc.RecStatus
                                                   }).ToList();

                var Summary = (from goodcontrol in goodControls
                               join groupbarcode in dcap.GroupBarcode.Where(g => g.Qty01NS != g.Qty01 && g.TxnStatus == 5) on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id, E = groupbarcode.BagBarCodeNo, F = groupbarcode.TxnMode }
                               where goodcontrol.RecStatus == 1
                               group new { goodcontrol, groupbarcode } by new
                               {
                                   goodcontrol.WarLocCode,
                               } into grp
                               select new temp
                               {
                                   WarLocCode = grp.Key.WarLocCode,
                                   Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)) - (grp.Sum(g => g.groupbarcode.Qty01NS) - grp.Sum(g => g.groupbarcode.Qty02NS) - grp.Sum(g => g.groupbarcode.Qty03NS))
                               }).ToList();

                foreach (temp gl in L1Details)
                {

                    temp se = new temp();
                    se.WarCode = gl.WarCode;
                    se.WarLocCode = gl.WarLocCode;
                    se.Qty01 = Summary.Where(g => g.WarLocCode == gl.WarLocCode).Sum(d => d.Qty01);

                    L2Details.Add(se);
                }

                byWarloc = L2Details.GroupBy(x => x.WarCode);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return byWarloc;
        }

        [Produces("application/json")]
        [HttpGet("GetLocaionsByWarLocCode")]
        public IEnumerable<IGrouping<string, temp>> GetLocaionsByWarLocCode(string warLocCode)
        {

            logger.InfoFormat("Get Factory Name warLocCode={0}", warLocCode);
            IEnumerable<IGrouping<string, temp>> byWarloc = null;
            List<temp> L2Details = new List<temp>();
            LookupController lookup = new LookupController(dcap);

            try
            {
                List<temp> L1Details = (from detxn in dcap.WarehouseLocation
                                        where detxn.WarLocCode == warLocCode && detxn.RecStatus == 1
                                        orderby detxn.WarCode ascending
                                        select new temp
                                        {
                                            WarCode = detxn.WarCode,
                                            WarLocCode = detxn.WarLocCode,
                                            Qty01 = 0
                                        }).ToList();
                //LDetails.ForEach(x => x.Qty01 = lookup.GetQtybyLocode(x.WarLocCode));

                //LDetails.ForEach(x => x.Qty01 = lookup.GetQtybyLocode(x.WarLocCode));
                //byWarloc = LDetails.GroupBy(x => x.WarCode);
                var Summary = (from goodcontrol in dcap.GoodControl.Where(g => g.WarLocCode != null && g.TxnStatus == 5 && g.WarLocCode == warLocCode && g.Return == 0)
                               join groupbarcode in dcap.GroupBarcode.Where(g => g.Qty01NS != g.Qty01 && g.TxnStatus == 5) on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id, E = groupbarcode.BagBarCodeNo, F = groupbarcode.TxnMode }
                               where goodcontrol.RecStatus == 1
                               group new { goodcontrol, groupbarcode } by new
                               {
                                   goodcontrol.WarLocCode,
                               } into grp
                               select new temp
                               {
                                   WarLocCode = grp.Key.WarLocCode,
                                   Qty01 = (grp.Sum(g => g.groupbarcode.Qty01) - grp.Sum(g => g.groupbarcode.Qty02) - grp.Sum(g => g.groupbarcode.Qty03)) - (grp.Sum(g => g.groupbarcode.Qty01NS) - grp.Sum(g => g.groupbarcode.Qty02NS) - grp.Sum(g => g.groupbarcode.Qty03NS))
                               }).ToList();

                foreach (temp gl in L1Details)
                {

                    temp se = new temp();
                    se.WarCode = gl.WarCode;
                    se.WarLocCode = gl.WarLocCode;
                    se.Qty01 = Summary.Where(g => g.WarLocCode == gl.WarLocCode).Sum(d => d.Qty01);

                    L2Details.Add(se);
                }

                byWarloc = L2Details.GroupBy(x => x.WarCode);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return byWarloc;
        }

        [Produces("application/json")]
        [HttpGet("GetWarlocCodeByShedule")]
        public List<temp> GetWarlocCodeByShedule(int styleid, int sheduleid, int colorid)
        {

            logger.InfoFormat("Get Factory Name styleid={0}, sheduleid={1}, colorid={2}", styleid, sheduleid, colorid);
            List<temp> L1Details = new List<temp>();

            try
            {
                L1Details = (from goodcontrol in dcap.GoodControl.Where(g => g.WarLocCode != null && g.TxnStatus == 5 && g.L1id == styleid && g.L2id == sheduleid && g.L3id == 0 && g.L4id == colorid && g.Return == 0)
                             join groupbarcode in dcap.GroupBarcode.Where(g => g.Qty01NS != g.Qty01 && g.TxnStatus == 5 && g.L1id == styleid && g.L2id == sheduleid && g.L3id == 0 && g.L4id == colorid) on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id, E = groupbarcode.BagBarCodeNo, F = groupbarcode.TxnMode }
                             where goodcontrol.RecStatus == 1 && groupbarcode.Qty01 != groupbarcode.Qty01NS
                             group new { goodcontrol, groupbarcode } by new
                             {
                                 goodcontrol.WarLocCode,
                             } into grp
                             select new temp
                             {
                                 WarLocCode = grp.Key.WarLocCode,
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public Decimal GetQtybyLocode(string loccode)
        {
            Decimal qty = 0;
            try
            {
                qty = (decimal)(from gd in dcap.GoodControl.Where(x => x.WarLocCode == loccode && x.TxnStatus == 5).AsQueryable()
                                join gb in dcap.GroupBarcode on new { A = (uint?)gd.L1id, B = (uint?)gd.L2id, C = (uint?)gd.L3id, D = (uint?)gd.L4id, E = gd.BarCodeNo, F = gd.TxnMode } equals new { A = (uint?)gb.L1id, B = (uint?)gb.L2id, C = (uint?)gb.L3id, D = (uint?)gb.L4id, E = gb.BagBarCodeNo, F = gb.TxnMode }
                                where gd.WarLocCode == loccode
                                select new
                                {
                                    gb.Qty01,
                                    gb.Qty02,
                                    gb.Qty03,
                                    gb.Qty01NS,
                                    gb.Qty02NS,
                                    gb.Qty03NS
                                }).Sum(x => ((x.Qty01 - (x.Qty02 + x.Qty03)) - (x.Qty01NS - (x.Qty02 + x.Qty03))));
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return qty;
        }

        //This varable struct is used to above API to render within parameter
        public struct temp
        {
            public string WarCode;
            public string WarLocCode;
            public decimal Qty01;
        }


        [Produces("application/json")]
        [HttpGet("CheckConfirmationsAreCompleted")]
        public Boolean CheckConfirmationsAreCompleted(string controlId, int seq)
        {
            logger.InfoFormat("CheckConfirmationsAreCompleted controlId={0}, seq={0}", controlId, seq);
            Boolean success = true;
            try
            {
                var objgoodcontrol = dcap.GoodControl
                         .Where(c => c.ControlId == controlId).ToList();

                foreach (GoodControl ui in objgoodcontrol)
                {
                    if (ui.IsSucess == 1)
                    {
                        success = true;
                    }
                    else
                    {
                        success = false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return success;
        }

        [Produces("application/json")]
        [HttpGet("GetRequestHeaderDetails")]
        public GoodControlDetailsSummary GetRequestHeaderDetails(string controlid)
        {
            GoodControlDetailsSummary L1Details = null;

            logger.InfoFormat("Get Request Header Details with controlid ={0}", controlid);

            try
            {
                L1Details = (from detxn in dcap.GoodControlDetails
                             join loc in dcap.Location on detxn.LocCode equals loc.LocCode
                             where detxn.ControlId == controlid
                             orderby detxn.TxnDateTime
                             select new GoodControlDetailsSummary
                             {
                                 TxnStatus = detxn.TxnStatus,
                                 ControlType = detxn.ControlType,
                                 Return = detxn.Return,
                                 ControlId = detxn.ControlId,
                                 Seq = detxn.Seq,
                                 Approver = detxn.Approver,
                                 LocFromCode = detxn.LocCodeFrom,
                                 LocName = loc.LocDescription,
                                 Depid = detxn.Depid,
                                 Created = detxn.CreatedBy,
                                 TxnDateTime = detxn.TxnDateTime,
                                 Qty01 = detxn.Qty01,
                                 Remark = detxn.Remark,
                                 LocAddress = loc.LocAddress,

                                 CreatedBy = detxn.CreatedBy,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 ModifiedBy = detxn.ModifiedBy,
                                 ModifiedDateTime = detxn.ModifiedDateTime,
                             }).FirstOrDefault();

                if (L1Details != null)
                {
                    L1Details.LocFromCode = GetFacNameFromLocCode(L1Details.LocFromCode);
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }

        public Boolean LookForCloseStatus(TeamCounterCM ui)
        {
            //TransactionController txncontroller = new TransactionController(dcap);
            Boolean validate = true;
            try
            {
                List<GoodControl> objdetxn2 = dcap.GoodControl
                        .Where(c => c.ControlId == ui.ControlId)
                        .ToList();

                if (objdetxn2.Count != 0)
                {
                    foreach (GoodControl si in objdetxn2)
                    {
                        if (si.TxnStatus < 5)
                        {
                            validate = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return validate;
        }

        [Produces("application/json")]
        [HttpGet("GetAllDispatchIds")]
        public IList<GoodControlDetails> GetAllDispatchIds()
        {
            logger.InfoFormat("GetAllDispatchIds");
            IList<GoodControlDetails> ExsistIds = null;
            try
            {

                ExsistIds = (from d in dcap.GoodControlDetails
                             select new GoodControlDetails
                             {
                                 ControlId = d.ControlId,
                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return ExsistIds;
        }

        //Check if there is any entitiy which that not satisfiy the minimum quantitity validation || checked 8-2-2020
        //Get dispatch input data and loop through l1, l2, l4 to check with cut quantitiy
        //Used API's and UI : UpdateDispatch (Businesscontrollers)
        [Produces("application/json")]
        [HttpGet("CheckForMinimumPOQuantity")]
        public Boolean CheckForMinimumPOQuantity(List<DispatchInput> DispatchInput)
        {
            logger.InfoFormat("CheckForMinimumPOQuantity DispatchInput=[{0}]", DispatchInput);
            Boolean haveMinimumQtyEntries = true;
            try
            {
                foreach (DispatchInput ui in DispatchInput)
                {

                    decimal totaldi1qty = DispatchInput.Where(f => f.L1idBag == ui.L1idBag && f.L2idBag == ui.L2idBag && f.L3idBag == ui.L3idBag && f.L4idBag == ui.L4idBag).ToList().Select(c => c.Qty01).Sum();

                    if (totaldi1qty <= 200)
                    {
                        var op1qty = from l5moops in dcap.L5moops
                                     where l5moops.L1id == ui.L1idBag && l5moops.L2id == ui.L2idBag && l5moops.L3id == ui.L3idBag && l5moops.L4id == ui.L4idBag && l5moops.OperationCode == 15
                                     select new { l5moops.OrderQty };
                        decimal totalop1qty = op1qty.ToList().Select(c => c.OrderQty).Sum();

                        if (totalop1qty != totaldi1qty && totalop1qty <= 200)
                        {
                            haveMinimumQtyEntries = false;
                        }
                    }

                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckForMinimumPOQuantity information {0}", e.ToString());
                throw e;
            }
            return haveMinimumQtyEntries;
        }

        //Set a flag for the records which orders that failed to complete 200 quantitiy : minimum wash quantitiy  validation on dyanamic op - FOR BAGS || Checked 8-2-2020
        //Set Flag for incomplete minimum orders
        //Used API's and UI : UpdatePOCounter (Businnesslogiccontrollers)
        [Produces("application/json")]
        [HttpGet("SetFlagForMinimumPOQuantity")]
        public Boolean SetFlagForMinimumPOQuantity(uint l1, uint l2, uint l3, uint l4, int OperationCode)
        {
            logger.InfoFormat("SetFlagForMinimumPOQuantity l1={0}, l2={1}, l3={2}, l4={3}, OperationCode={4}", l1, l2, l3, l4, OperationCode);
            Boolean flag = false;

            try
            {
                //Cut Qunatity for given parameters
                var orderqty = from l5moops in dcap.L5moops
                               where l5moops.L1id == l1 && l5moops.L2id == l2 && l5moops.L3id == l3 && l5moops.L4id == l4 && l5moops.OperationCode == 15
                               select new { l5moops.OrderQty, l5moops.ReportedQty };

                decimal totalOrderQty = orderqty.ToList().Select(c => c.OrderQty).Sum(); //400
                decimal totalReportedQty = orderqty.ToList().Select(c => c.ReportedQty).Sum(); //410

                if (totalOrderQty <= 500)
                {
                    var operationqty = from detxn in dcap.Detxn
                                       where detxn.L1id == l1 && detxn.L2id == l2 && detxn.L3id == l3 && detxn.L4id == l4 && detxn.OperationCode == OperationCode
                                       select new { detxn.Qty01 };

                    decimal totalop1qty = (decimal)operationqty.ToList().Select(c => c.Qty01).Sum(); //300

                    decimal variance = totalReportedQty - totalop1qty; //110

                    if (variance <= 200) //200 <= 200
                    {
                        flag = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckForMinimumPOQuantity information {0}", e.ToString());
                throw e;
            }
            return flag;
        }

        //Get Locationcode by Bag barcode and Control Id
        //Used API's and UI : getandSetLocationofBarcode WEB (Good Recive)
        [Produces("application/json")]
        [HttpGet("GetLocationByBarcode")]
        public GoodControl GetLocationByBarcode(string Barcode, string ControlId)
        {

            logger.InfoFormat("Get Location By Barcode Barcode={0}, ControlId={1}", Barcode, ControlId);
            GoodControl WarlocCode = null;

            try
            {
                WarlocCode = (from w in dcap.GoodControl
                              where w.BarCodeNo == Barcode && w.ControlId == ControlId
                              select new GoodControl
                              {
                                  WarLocCode = w.WarLocCode,
                              }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return WarlocCode;
        }

        //Set a flag for the records which orders that failed to complete 200 quantitiy : minimum operation quantitiy  validation on dyanamic op - FOR Group Barcodes || Checked 8-3-2020
        //Set Flag for incomplete minimum orders
        //Used API's and UI : GetBarcodeDetails (LookupControllers)
        [Produces("application/json")]
        [HttpGet("SetFlagForMinimumPOQuantityinGroupBarcodes")]
        public List<BagBarcodeTransactions> SetFlagForMinimumPOQuantityinGroupBarcodes(List<BagBarcodeTransactions> GroupBarcodes)
        {
            logger.InfoFormat("SetFlagForMinimumPOQuantityinGroupBarcodes GroupBarcodes=[{0}]", GroupBarcodes);
            try
            {
                foreach (BagBarcodeTransactions ui in GroupBarcodes)
                {
                    //Group Barcode Qunatity for given parameters
                    decimal totalgroupqty = GroupBarcodes.Where(f => f.L1idBag == ui.L1idBag && f.L2idBag == ui.L2idBag && f.L3idBag == ui.L3idBag && f.L4idBag == ui.L4idBag).ToList().Select(c => c.Qty01).Sum();

                    if (totalgroupqty <= 200)
                    {

                        var operationqty = from detxn in dcap.Detxn
                                           where detxn.L1id == ui.L1idBag && detxn.L2id == ui.L2idBag && detxn.L3id == ui.L3idBag && detxn.L4id == ui.L4idBag && detxn.OperationCode == 151
                                           select new { detxn.Qty01 };

                        decimal totalop1qty = (decimal)operationqty.ToList().Select(c => c.Qty01).Sum();

                        if (totalop1qty <= 200)
                        {
                            var Orderqty = from l5moops in dcap.L5moops
                                           where l5moops.L1id == ui.L1idBag && l5moops.L2id == ui.L2idBag && l5moops.L3id == ui.L3idBag && l5moops.L4id == ui.L4idBag && l5moops.OperationCode == 15
                                           select new { l5moops.OrderQty };

                            decimal totalOrderQty = (decimal)Orderqty.ToList().Select(c => c.OrderQty).Sum();

                            if (totalop1qty <= 200)
                            {
                                if (totalop1qty >= totalOrderQty) //OrdereQty => ( OrderQty - Cum washsend ) < 200
                                {
                                    ui.BagStatus = 0;
                                    ui.OperationQty = totalop1qty;
                                    ui.OrderQty = totalOrderQty;

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckForMinimumPOQuantity information {0}", e.ToString());
                throw e;
            }
            return GroupBarcodes;
        }

        [Produces("application/json")]
        [HttpGet("GetAllBagBarcodesReady")]
        public List<StyleScheduleColor> GetAllBagBarcodesReady()
        {

            logger.InfoFormat("GetAllBagBarcodesReady By Bag Barcode");
            List<StyleScheduleColor> Details = null;

            try
            {
                Details = (from bagbarcode in dcap.GroupBarcode
                           where bagbarcode.RecStatus == (int)eRecStatus.Active && bagbarcode.TxnMode == 1 && bagbarcode.TxnStatus == 5
                           select new StyleScheduleColor
                           {
                               BagBarcode = bagbarcode.BagBarCodeNo,
                           }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetAllBagBarcodesReady information {0}", e.ToString());

            }
            return Details;
        }

        [Produces("application/json")]
        [HttpGet("GetAllTravelTagIds")]
        public IList<TravelBarcodeDetails> GetAllTravelTagIds()
        {
            logger.InfoFormat("GetAllTravelTagIds");
            IList<TravelBarcodeDetails> ExsistIds = null;
            try
            {

                ExsistIds = (from d in dcap.TravelBarcodeDetails
                             select new TravelBarcodeDetails
                             {
                                 TravelBarCodeNo = d.TravelBarCodeNo,
                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return ExsistIds;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelTagHeaderDetails")]
        public List<TravelBarcodeDetails> GetTravelTagHeaderDetails(string traveltagId, int wfinstid)
        {

            logger.InfoFormat("Get Cut Quantity By Details with controlid ={0}, wfinstid={1}", traveltagId, wfinstid);
            List<TravelBarcodeDetails> objdetxn1 = new List<TravelBarcodeDetails>();

            traveltagId = traveltagId == null ? "%" : traveltagId;

            try
            {
                objdetxn1 = (from travelta in dcap.TravelBarcodeDetails
                             where EF.Functions.Like(travelta.TravelBarCodeNo, "%" + traveltagId + "%") // && (wfinstid <= 0 ? true : travelta.TravelStatus == wfinstid)
                             select new TravelBarcodeDetails
                             {
                                 TravelBarCodeNo = travelta.TravelBarCodeNo,
                                 Color = travelta.Color,
                                 Qty01 = travelta.Qty01,
                                 WFId = travelta.WFId,
                                 SMode = travelta.SMode
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objdetxn1;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelTagSizeDetailsOutSource")]
        public List<L5> GetTravelTagSizeDetailsOutSource(string traveltagId, string sizeno)
        {

            logger.InfoFormat("Get Cut Quantity By Details with controlid ={0}, sizeno={1}", traveltagId, sizeno);
            List<L5> L5 = new List<L5>();
            sizeno = sizeno == null ? "%" : sizeno;

            try
            {
                L5 = (from g in dcap.GroupBarcode.Where(c => c.BagBarCodeNo == traveltagId && c.TxnMode >= 2).AsQueryable()
                      join l5 in dcap.L5 on new { A = g.L1id, B = g.L2id, C = g.L3id, D = g.L4id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id }
                      where g.BagBarCodeNo == traveltagId && g.TxnMode >= 2 && EF.Functions.Like(l5.L5no, "%" + sizeno + "%")
                      select new L5
                      {
                          L1id = l5.L1id,
                          L2id = l5.L2id,
                          L3id = l5.L3id,
                          L4id = l5.L4id,
                          L5id = l5.L5id,
                          L5no = l5.L5no,
                          L5desc = l5.L5desc
                      }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return L5;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelTagHeaderDetailsForPrint")]
        public TravelBarcodeDetails GetTravelTagHeaderDetailsForPrint(string traveltagId)
        {

            logger.InfoFormat("Get Cut Quantity By Details with controlid ={0}", traveltagId);
            TravelBarcodeDetails objdetxn1 = new TravelBarcodeDetails();

            try
            {
                objdetxn1 = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == traveltagId).FirstOrDefault();
                if (objdetxn1 != null)
                {
                    objdetxn1.FacCode = dcap.Location.Where(d => d.FacCode == objdetxn1.FacCode).Select(c => c.LocName).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return objdetxn1;
        }

        [Produces("application/json")]
        [HttpGet("GetBagBarcodeDetailsbyTravelBarcode")]
        public List<TeamCounterCM> GetBagBarcodeDetailsbyTravelBarcode(string travelcode)
        {

            logger.InfoFormat("Get Travel Barcode Details travelcode={0}", travelcode);
            List<TeamCounterCM> L1Details = new List<TeamCounterCM>();
            List<Detxn> detx = new List<Detxn>();

            try
            {
                List<GroupBarcode> group = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == travelcode && c.TxnMode >= 2).ToList();

                if (group.Count != 0)
                {
                    var Barcodey = group[0].BagBarCodeNo;
                    var Txnmodey = group[0].TxnMode;

                    List<Detxn> gbm = (from d in dcap.GroupBarcodeMapping.Where(d => d.MotherBarcode == Barcodey && d.MotherTxnMode == Txnmodey).AsQueryable()
                                       join gb in dcap.GroupBarcode on new { A = d.ChildBarcode, B = d.ChildTxnMode } equals new { A = gb.BagBarCodeNo, B = gb.TxnMode }
                                       group new { d, gb } by new { gb.WFDEPInstId, gb.L1id, gb.L2id, gb.L3id, gb.L4id, d.ChildBarcode } into grp
                                       select new Detxn
                                       {
                                           BagBarCodeNo = grp.Key.ChildBarcode,
                                           WfdepinstId = (uint)grp.Key.WFDEPInstId,
                                           L1id = grp.Key.L1id,
                                           L2id = grp.Key.L2id,
                                           L3id = grp.Key.L3id,
                                           L4id = grp.Key.L4id,
                                           Qty01Ns = grp.Sum(c => c.d.Qty01NS),
                                       }).ToList();
                    if (gbm.Count == 0)
                    {
                        foreach (GroupBarcode g in group)
                        {
                            List<Detxn> det = (from d in dcap.Detxn.Where(d => d.OperationCode == g.OperationCode && d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && d.TravelBarCodeNo == g.BagBarCodeNo).AsQueryable()
                                               group d by new { d.L1id, d.L2id, d.L3id, d.L4id, d.BagBarCodeNo } into grp
                                               select new Detxn
                                               {
                                                   BagBarCodeNo = grp.Key.BagBarCodeNo,
                                                   L1id = grp.Key.L1id,
                                                   L2id = grp.Key.L2id,
                                                   L3id = grp.Key.L3id,
                                                   L4id = grp.Key.L4id,
                                               }).ToList();

                            detx = detx.Concat(det).ToList();
                        }
                    }
                    else
                    {
                        detx = detx.Concat(gbm).ToList();
                    }

                    if (detx.Count != 0)
                    {
                        foreach (Detxn g in detx)
                        {
                            List<TeamCounterCM> L2Details = (from detxn in dcap.GroupBarcode
                                                             join l1 in dcap.L1 on detxn.L1id equals l1.L1id
                                                             join l2 in dcap.L2 on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                                             join l4 in dcap.L4 on new { P = detxn.L1id, Q = detxn.L2id, R = detxn.L3id, S = detxn.L4id } equals new { P = l4.L1id, Q = l4.L2id, R = l4.L3id, S = l4.L4id }
                                                             where detxn.L1id == g.L1id && detxn.L2id == g.L2id && detxn.L3id == g.L3id && detxn.L4id == g.L4id && detxn.BagBarCodeNo == g.BagBarCodeNo
                                                             orderby detxn.ModifiedDateTime
                                                             select new TeamCounterCM
                                                             {
                                                                 Seq = detxn.Seq,
                                                                 BagBarCode = detxn.BagBarCodeNo,
                                                                 TxnMode = detxn.TxnMode,
                                                                 StyleId = detxn.L1id,
                                                                 StyleDesc = l1.L1desc,
                                                                 ScheduleId = detxn.L2id,
                                                                 ScheduleDesc = l2.L2desc,
                                                                 DeliveryDate = l2.DeliveryDate,
                                                                 PONo = l2.Ref01,
                                                                 Zfeature = l2.Ref02,
                                                                 ColorId = l4.L4id,
                                                                 ColorDesc = l4.L4desc,
                                                                 Qty01 = (int)detxn.Qty01,
                                                                 Qty02 = (int)detxn.Qty02,
                                                                 Qty03 = (int)detxn.Qty03,
                                                                 Qty01Ns = (int)detxn.Qty01NS,
                                                                 Qty02Ns = (int)detxn.Qty02NS,
                                                                 Qty03Ns = ((int)detxn.Qty01NS - (int)g.Qty01Ns),
                                                                 TxnStatus = detxn.TxnStatus,
                                                                 Remark = detxn.WorkCenter
                                                             }).ToList();

                            L1Details = L1Details.Concat(L2Details).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelBarcodeDetails")]
        public IList<TeamCounterCM> GetTravelBarcodeDetails(string travelcode)
        {

            logger.InfoFormat("Get Bag Barcode Details by Travel Barcode travelcode={0}", travelcode);
            IList<TeamCounterCM> L1Details = null;

            try
            {
                IList<GroupBarcode> gb = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == travelcode).ToList();
                var maxl1id = gb.Max(c => c.L1id);
                var minl1id = gb.Min(c => c.L1id);
                IList<GroupBarcode> dt = new List<GroupBarcode>();

                if (gb.Count != 0)
                {
                    foreach (GroupBarcode g in gb)
                    {
                        IList<GroupBarcode> s = dcap.Detxn.Where(c => c.L1id == g.L1id && c.L2id == g.L2id && c.L4id == g.L4id && c.TravelBarCodeNo == g.BagBarCodeNo && c.OperationCode == g.OperationCode && c.Wfid == g.WFId).GroupBy(c => new {c.L1id, c.L2id, c.L4id, c.L5id, c.TravelBarCodeNo}).Select(c => new GroupBarcode{ L1id = c.Key.L1id, L2id = (uint)c.Key.L2id, L4id = (uint)c.Key.L4id, L5id = (uint)c.Key.L5id, BagBarCodeNo = c.Key.TravelBarCodeNo, Qty01 = (int)c.Sum(n => n.Qty01)}).ToList();
                        dt = dt.Concat(s).ToList();
                    }
                }

                L1Details = (from detxn in gb
                             join l1 in dcap.L1.Where(c => c.L1id >= minl1id && c.L1id <= maxl1id).AsQueryable() on detxn.L1id equals l1.L1id
                             join l2 in dcap.L2.Where(c => c.L1id >= minl1id && c.L1id <= maxl1id).AsQueryable() on new { A = detxn.L1id, B = detxn.L2id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4.Where(c => c.L1id >= minl1id && c.L1id <= maxl1id).AsQueryable() on new { P = detxn.L1id, Q = detxn.L2id, R = detxn.L3id, S = detxn.L4id } equals new { P = l4.L1id, Q = l4.L2id, R = l4.L3id, S = l4.L4id }
                             join dts in dt on new { P = detxn.L1id, Q = detxn.L2id, R = detxn.L3id, S = detxn.L4id, T = detxn.BagBarCodeNo } equals new { P = dts.L1id, Q = dts.L2id, R = dts.L3id, S = dts.L4id, T = dts.BagBarCodeNo }
                             join l5 in dcap.L5.Where(c => c.L1id >= minl1id && c.L1id <= maxl1id).AsQueryable() on new { P = dts.L1id, Q = dts.L2id, R = dts.L3id, S = dts.L4id, T = dts.L5id } equals new { P = l5.L1id, Q = l5.L2id, R = l5.L3id, S = l5.L4id, T = l5.L5id }
                             where detxn.BagBarCodeNo == travelcode
                             orderby detxn.ModifiedDateTime
                             select new TeamCounterCM
                             {
                                 Seq = detxn.Seq,
                                 BagBarCode = detxn.BagBarCodeNo,
                                 TxnMode = detxn.TxnMode,
                                 StyleId = detxn.L1id,
                                 StyleDesc = l1.L1desc,
                                 ScheduleId = detxn.L2id,
                                 ScheduleDesc = l2.L2desc,
                                 DeliveryDate = l2.DeliveryDate,
                                 PONo = l2.Ref01,
                                 Zfeature = l2.Ref02,
                                 ColorId = l4.L4id,
                                 ColorDesc = l4.L4desc,
                                 SizeDesc = l5.L5desc,
                                 Qty01 = (int)dts.Qty01,
                                 Qty02 = (int)detxn.Qty02,
                                 Qty03 = (int)detxn.Qty03,
                                 Qty01Ns = (int)detxn.Qty01NS,
                                 Qty02Ns = (int)detxn.Qty01NS,
                                 Qty03Ns = (int)detxn.Qty01NS,
                                 TxnStatus = detxn.TxnStatus
                             }).ToList();

                /*foreach (GoodControl gc in L1Details) {
                    if(gc.TxnMode != 1) {
                        
                    }
                }*/
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public IList<Detxn> GetBarcodeDetailsFromTravelTag(string travelcode, uint l1id, uint l2id, uint l3id, uint l4id, int? TxnMode, int OperationCode, UInt32 Seq)
        {

            logger.InfoFormat("Get Factory Name travelcode={0}, l1id={1}, l2id={2}, l3id={3}, l4id={4}, TxnMode={5}, OperationCode={6}, Seq={7}", travelcode, l1id, l2id, l3id, l4id, TxnMode, OperationCode, Seq);
            List<Detxn> L1Details = null;

            try
            {
                if (TxnMode < 3)
                {
                    L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.BagBarCodeNo != "" && detxn.TravelBarCodeNo == travelcode
                             && detxn.OperationCode == OperationCode && (detxn.Qty02 != 1 || detxn.Qty03 != 1) && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1).AsQueryable()
                                 group detxn by new
                                 {
                                     detxn.L1id,
                                     detxn.L2id,
                                     detxn.L3id,
                                     detxn.L4id,
                                     detxn.L5id,
                                     detxn.L5mono,
                                 } into grp
                                 select new Detxn
                                 {
                                     Seq = 100,
                                     TxnMode = (int)TxnMode,
                                     L1id = grp.Key.L1id,
                                     L2id = grp.Key.L2id,
                                     L3id = grp.Key.L3id,
                                     L4id = grp.Key.L4id,
                                     L5id = grp.Key.L5id,
                                     Qty01 = grp.Sum(detxn => detxn.Qty01)
                                 }).ToList();
                }
                else
                {
                    L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.BagBarCodeNo != "" && detxn.TravelBarCodeNo == travelcode
                             && detxn.OperationCode == OperationCode && detxn.Qty01 != 1 && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1).AsQueryable()
                                 group detxn by new
                                 {
                                     detxn.L1id,
                                     detxn.L2id,
                                     detxn.L3id,
                                     detxn.L4id,
                                     detxn.L5id,
                                     detxn.L5mono,
                                 } into grp
                                 select new Detxn
                                 {
                                     Seq = 100,
                                     TxnMode = (int)TxnMode,
                                     L1id = grp.Key.L1id,
                                     L2id = grp.Key.L2id,
                                     L3id = grp.Key.L3id,
                                     L4id = grp.Key.L4id,
                                     L5id = grp.Key.L5id,
                                     Qty01 = (grp.Sum(detxn => detxn.Qty02) + grp.Sum(detxn => detxn.Qty03))
                                 }).ToList();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public IList<Detxn> GetBarcodeDetailsFromBagTag(string bagtag, uint l1id, uint l2id, uint l3id, uint l4id, int OperationCode)
        {

            logger.InfoFormat("Get Factory Name bagtag={0}", bagtag);
            List<Detxn> L1Details = null;

            try
            {
                L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.OperationCode == OperationCode && detxn.BagBarCodeNo == bagtag
                             && detxn.Qty02 != 1 && detxn.Qty03 != 1 && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1).AsQueryable()
                             group detxn by new
                             {
                                 detxn.Wfid,
                                 detxn.WfdepinstId,
                                 detxn.Depid,
                                 detxn.L1id,
                                 detxn.L2id,
                                 detxn.L3id,
                                 detxn.L4id,
                                 detxn.BagBarCodeNo,
                                 detxn.L5id,
                                 detxn.L5mono,
                                 detxn.L5moid,
                                 detxn.Dclid,
                                 detxn.Dcmid
                             } into grp
                             select new Detxn
                             {
                                 Wfid = grp.Key.Wfid,
                                 WfdepinstId = grp.Key.WfdepinstId,
                                 Depid = grp.Key.Depid,
                                 L1id = grp.Key.L1id,
                                 L2id = grp.Key.L2id,
                                 L3id = grp.Key.L3id,
                                 L4id = grp.Key.L4id,
                                 BagBarCodeNo = grp.Key.BagBarCodeNo,
                                 L5id = grp.Key.L5id,
                                 L5mono = grp.Key.L5mono,
                                 L5moid = grp.Key.L5moid,
                                 Dclid = grp.Key.Dclid,
                                 Dcmid = grp.Key.Dcmid,
                                 Qty01 = grp.Sum(detxn => detxn.Qty01),
                                 Qty02 = grp.Sum(detxn => detxn.Qty02),
                                 Qty03 = grp.Sum(detxn => detxn.Qty03),
                                 Qty01Ns = grp.Sum(detxn => detxn.Qty01Ns),
                                 Qty02Ns = grp.Sum(detxn => detxn.Qty02Ns),
                                 Qty03Ns = grp.Sum(detxn => detxn.Qty03Ns),
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }


        [Produces("application/json")]
        [HttpGet("GetBarcodeDetailsofTravelTag")]
        public IList<Detxn> GetBarcodeDetailsofTravelTag(string travelbarcode, int txnmode)
        {

            logger.InfoFormat("Get Factory Name travelcode={0} txnmode={1}", travelbarcode, txnmode);
            List<Detxn> L1Details = new List<Detxn>();

            try
            {
                IList<GroupBarcode> L2Details = (from groupbarcode in dcap.GroupBarcode.Where(groupbarcode => groupbarcode.BagBarCodeNo == travelbarcode && groupbarcode.TxnMode >= txnmode).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     Seq = groupbarcode.Seq,
                                                     L1id = groupbarcode.L1id,
                                                     L2id = groupbarcode.L2id,
                                                     L3id = groupbarcode.L3id,
                                                     L4id = groupbarcode.L4id,
                                                     BagBarCodeNo = groupbarcode.BagBarCodeNo,
                                                     TxnMode = groupbarcode.TxnMode,
                                                     OperationCode = groupbarcode.OperationCode
                                                 }).ToList();

                foreach (GroupBarcode qi in L2Details)
                {
                    if (txnmode == 2)
                    {
                        L1Details = L1Details.Concat(GetBarcodeDetailsFromTravelTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id, qi.TxnMode, qi.OperationCode, qi.Seq)).ToList();
                    }
                    else if (txnmode == 1)
                    {
                        L1Details = L1Details.Concat(GetBarcodeDetailsFromBagTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id, qi.OperationCode)).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetBarcodeDetailsofBagTag")]
        public IList<Detxn> GetBarcodeDetailsofBagTag(string bagtag, int operationcode)
        {

            logger.InfoFormat("Get Factory Name bagtag={0} operationcode={1}", bagtag, operationcode);
            List<Detxn> L1Details = new List<Detxn>();

            try
            {
                IList<GroupBarcode> L2Details = (from detxn in dcap.TeamCounter.Where(detxn => detxn.BagBarCodeNo == bagtag).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     L1id = detxn.L1id,
                                                     L2id = detxn.L2id,
                                                     L3id = detxn.L3id,
                                                     L4id = detxn.L4id,
                                                     BagBarCodeNo = detxn.BagBarCodeNo
                                                 }).ToList();

                foreach (GroupBarcode qi in L2Details)
                {
                    L1Details = L1Details.Concat(GetBarcodeDetailsFromBagTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id, operationcode)).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public String GetTravelTagbyBarcode(string barcode)
        {

            logger.InfoFormat("Get Travel Tag by Barcode barcode={0}", barcode);
            String travelbarcode = null;

            try
            {
                travelbarcode = dcap.Detxn.Where(x => x.BarCodeNo == barcode && x.BagBarCodeNo != "" && x.TravelBarCodeNo != "").OrderByDescending(d => d.TxnDateTime).Select(x => x.TravelBarCodeNo).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return travelbarcode;
        }

        public String GetBarcodebytravelBarcodeOutsource(string traveltag, int l1id, int l2id, int l4id, int l5id)
        {

            logger.InfoFormat("Get Travel Tag by Barcode traveltag={0}, l1id={1}, l2id={2}, l4id={3}, l5id={4}", traveltag, l1id, l2id, l4id, l5id);
            String travelbarcode = null;

            try
            {
                travelbarcode = dcap.Detxn.Where(x => x.L1id == l1id && x.L2id == l2id && x.L4id == l4id && x.BagBarCodeNo != "" && x.TravelBarCodeNo == traveltag && x.L5id == l5id && (x.Qty01 - x.Qty02 - x.Qty03) > (x.Qty01Ns + x.Qty02Ns + x.Qty03Ns)).OrderByDescending(d => d.TxnDateTime).Select(x => x.BarCodeNo).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return travelbarcode;
        }

        public GroupBarcode GetTravelTagGroupeDetailbyBarcode(UserInput ui)
        {

            logger.InfoFormat("Get Travel Tag by Barcode UserInput={0}", ui);
            GroupBarcode travelbarcode = new GroupBarcode();

            try
            {
                travelbarcode = dcap.GroupBarcode.Where(x => x.L1id == ui.StyleId && x.L2id == ui.ScheduleId && x.L4id == ui.ColorId && x.BagBarCodeNo == ui.JobNo && x.TxnMode >= 2).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return travelbarcode;
        }

        [Produces("application/json")]
        [HttpGet("GetTansactionDetailsByIds")]
        public IList<Dedepinst> GetTansactionDetailsByIds(int l1id, int l2id, int l3id, int l4id, int l5id, int OpeartionCode)
        {

            logger.InfoFormat("Get Tansaction Details By Ids l1id={0}, l2id={1}, l3id={2} l4id={3}, l5id={4}, OpeartionCode={5}", l1id, l2id, l3id, l4id, l5id, OpeartionCode);
            List<Dedepinst> L1Details = null;

            try
            {
                L1Details = dcap.Dedepinst.Where(detinst => detinst.L1id == l1id && detinst.L2id == l2id && detinst.L3id == l3id && detinst.L4id == l4id && detinst.L5id == l5id && detinst.OperationCode == OpeartionCode).ToList();

                if (L1Details.Count != 0)
                {
                    foreach (Dedepinst d in L1Details)
                    {
                        var Det = dcap.Detxn.Where(detxn => detxn.L1id == d.L1id && detxn.L2id == d.L2id && detxn.L4id == d.L4id && detxn.L5id == d.L5id && detxn.OperationCode == d.OperationCode && detxn.WfdepinstId == d.WfdepinstId && detxn.L5moid == d.L5moid && detxn.Depid == d.Depid && detxn.TeamId == d.TeamId && detxn.Wfid == d.Wfid).ToList();
                        d.WorkCenter = dcap.Team.Where(c => c.TeamId == d.TeamId).Select(c => c.TeamName).FirstOrDefault();
                        d.Qty01Ns = Det.Sum(c => c.Qty01);
                        d.Qty02Ns = Det.Sum(c => c.Qty02);
                        d.Qty03Ns = Det.Sum(c => c.Qty03);
                    }
                }
                else
                {
                    L1Details = null;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        #endregion

        //Travel Barcode API's: START
        //Get All used Colors for travel Barcodes
        //Used API's and UI : Getcolors() BFL Travel Tag
        [Produces("application/json")]
        [HttpGet("GetTravelTagColors")]
        public IList<TravelBarcodeOutputDetails> GetTravelTagColors(string Color)
        {

            logger.InfoFormat("GetTravelTagColors Style Color={1}", Color);
            IList<TravelBarcodeOutputDetails> Colors = null;

            Color = Color == null ? "%" : Color;

            try
            {

                Colors = (from tbd in dcap.GroupBarcode
                          join l4 in dcap.L4 on new { A = (uint?)tbd.L1id, B = (uint?)tbd.L2id, C = (uint?)tbd.L3id, D = (uint?)tbd.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                          where EF.Functions.Like(l4.L4desc, "%" + Color + "%") && tbd.TxnMode == 2
                          group tbd by new { l4.L4no, l4.L4desc }
                          into grp
                          select new TravelBarcodeOutputDetails
                          {
                              l4no = grp.Key.L4no,
                              l4desc = grp.Key.L4desc,
                          }).ToList();


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTravelTagColors information {0}", e.ToString());
                throw e;
            }
            return Colors;
        }

        public String GetFactoryByWfid(string BagBarcode)
        {
            logger.InfoFormat("GetTravelTagColors Style BagBarcode={1}", BagBarcode);
            String FacCode = null;

            try
            {
                uint WFId = dcap.GroupBarcode.Where(det => det.BagBarCodeNo == BagBarcode).Select(x => x.WFId).FirstOrDefault();
                FacCode = dcap.Wf.Where(det => det.Wfid == WFId).Select(x => x.FacCode).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTravelTagColors information {0}", e.ToString());
                throw e;
            }
            return FacCode;
        }

        public String GetBuddyBarcodeByBuddyBagBarcode(string BuddyBagBarcode)
        {
            logger.InfoFormat("GetTravelTagColors Style WFId={1}", BuddyBagBarcode);
            String BuddyBarcode = null;

            try
            {
                BuddyBarcode = dcap.GroupBarcode.Where(det => det.BagBarCodeNo == BuddyBagBarcode && det.TxnMode == 3).Select(x => x.BagBarCodeNo).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTravelTagColors information {0}", e.ToString());
                throw e;
            }
            return BuddyBarcode;
        }

        [Produces("application/json")]
        [HttpGet("GetColorSheduleByBarcode")]
        public IList<StyleScheduleColor> GetColorSheduleByBarcode(string Barcode)
        {

            logger.InfoFormat("GetColorSheduleByBarcode By Barcode Barcode ={0}", Barcode);
            IList<StyleScheduleColor> Detail = null;

            try
            {
                Detail = (from gb in dcap.GroupBarcode
                          join l1 in dcap.L1 on new { A = gb.L1id } equals new { A = l1.L1id }
                          join l2 in dcap.L2 on new { A = gb.L1id, B = gb.L2id } equals new { A = l2.L1id, B = l2.L2id }
                          join l4 in dcap.L4 on new { A = gb.L1id, B = gb.L2id, C = gb.L3id, D = gb.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                          join l5bc in dcap.L5bc on new { A = gb.L1id, B = gb.L2id, C = gb.L3id, D = gb.L4id } equals new { A = l5bc.L1id, B = l5bc.L2id, C = l5bc.L3id, D = l5bc.L4id }
                          where gb.BagBarCodeNo == Barcode && gb.TxnStatus == 0
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
                              ColorId = l4.L4id,
                              ColorNo = l4.L4no,
                              ColorDesc = l4.L4desc,
                              WFId = l2.Wfid,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Detail;
        }

        [Produces("application/json")]
        [HttpGet("GetColorSheduleByTravelBarcode")]
        public List<StyleScheduleColor> GetColorSheduleByTravelBarcode(string Barcode, int Option)
        {

            logger.InfoFormat("GetColorSheduleByTravelBarcode By Barcode Barcode ={0}, Option ={0}", Barcode, Option);
            List<StyleScheduleColor> Output = null;
            //IList<StyleScheduleColor> Output = null;

            try
            {
                if (Option == 0)
                {
                    Output = (from gb in dcap.GroupBarcode
                              join l1 in dcap.L1 on new { A = gb.L1id } equals new { A = l1.L1id }
                              join l2 in dcap.L2 on new { A = gb.L1id, B = gb.L2id } equals new { A = l2.L1id, B = l2.L2id }
                              join l4 in dcap.L4 on new { A = gb.L1id, B = gb.L2id, C = gb.L3id, D = gb.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                              //join l5 in dcap.L5 on new { A = gb.L1id, B = gb.L2id, C = gb.L3id, D = gb.L4id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id }
                              where gb.BagBarCodeNo == Barcode && gb.TxnStatus >= 6 && gb.TxnMode >= 2
                              group gb by new
                              {
                                  l1.L1id,
                                  l1.L1no,
                                  l1.L1desc,
                                  l2.L2id,
                                  l2.Wfid,
                                  l2.L2no,
                                  l2.L2desc,
                                  l2.Ref01,
                                  l2.Ref02,
                                  l4.L4id,
                                  l4.L4no,
                                  l4.L4desc,
                                  //l5.L5id,
                                  //l5.L5desc,
                                  gb.BagBarCodeNo,
                                  gb.SMode
                              } into grp
                              select new StyleScheduleColor
                              {
                                  StyleId = grp.Key.L1id,
                                  StyleNo = grp.Key.L1no,
                                  StyleDesc = grp.Key.L1desc,
                                  WFId = grp.Key.Wfid,
                                  ScheduleId = grp.Key.L2id,
                                  ScheduleNo = grp.Key.L2no,
                                  ScheduleDesc = grp.Key.L2desc,
                                  Zfeature = grp.Key.Ref02,
                                  PONo = grp.Key.Ref01,
                                  ColorId = grp.Key.L4id,
                                  ColorNo = grp.Key.L4no,
                                  ColorDesc = grp.Key.L4desc,
                                  //SizeId = grp.Key.L5id,
                                  //SizeDesc = grp.Key.L5desc,
                                  BagBarcode = grp.Key.BagBarCodeNo,
                                  SMode = grp.Key.SMode,
                                  Qty01 = grp.Sum(gb => gb.Qty01),
                                  Qty02 = grp.Sum(gb => gb.Qty02),
                                  Qty03 = grp.Sum(gb => gb.Qty03),
                                  Qty01NS = grp.Sum(gb => gb.Qty01NS),
                                  Qty02NS = grp.Sum(gb => gb.Qty01NS),
                                  Qty03NS = grp.Sum(gb => gb.Qty01NS)
                              }).ToList();
                }
                else
                {
                    L4 L4 = null;
                    StyleScheduleColor Color = null;

                    Output = (from l1 in dcap.L1
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
                                  LotNo = lbc.LotNo,
                                  Qty01 = 1,
                              }).ToList();

                    if (Output != null)
                    {
                        Color = Output[0];

                        Color.BagBarcode = GetGroupBarcodebyBarcode(Barcode, Output[0].StyleId, Output[0].ScheduleId, 0, Output[0].ColorId, Output[0].SizeId, 2);
                        if (Color.ColorId != 0)
                        {
                            L4 = (from l4 in dcap.L4
                                  where l4.L1id == Color.StyleId && l4.L2id == Color.ScheduleId && l4.L4id == Color.ColorId
                                  select new L4
                                  {
                                      L4no = l4.L4no,
                                      L4desc = l4.L4desc
                                  }).FirstOrDefault();

                            if (L4 != null)
                            {
                                Color.ColorNo = L4.L4no;
                                Color.ColorDesc = L4.L4desc;
                            }
                        }
                        else
                        {
                            Color.ColorNo = "-N/A-";
                        }

                        Output[0] = Color;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByTravelBarcode information {0}", e.ToString());

            }
            return Output;
        }

        //Travel Barcode Scan Update : START
        //Get Qty By DEP Inst Id
        [Produces("application/json")]
        [HttpGet("GetTravelStatus")]
        public TravelStatus GetTravelStatus(string BarcodeNo, uint DEPID)
        {
            logger.InfoFormat("GetTravelStatus API called with BarcodeNo={0}, DEPID={1}", BarcodeNo, DEPID);

            TravelStatus TravelStatus = null;

            try
            {
                TravelStatus = (from d in dcap.TravelStatus
                                where d.BarCodeNo == BarcodeNo && d.Depid == DEPID && d.RecStatus == (int)eRecStatus.Active
                                group d by new { d.BarCodeNo, d.Depid }
                            into grp
                                select new TravelStatus
                                {
                                    Qty01 = grp.Sum(d => d.Qty01),
                                    Qty02 = grp.Sum(d => d.Qty02),
                                    Qty03 = grp.Sum(d => d.Qty03)
                                }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetTravelStatus information {0}", e.ToString());
                throw e;
            }
            return TravelStatus;
        }
        //Travel Barcode Scan Update : START

        [Produces("application/json")]
        [HttpGet("GetBarcodesByTravelBarcode")]
        public IList<Detxn> GetBarcodesByTravelBarcode(string Barcode, int L4)
        {

            logger.InfoFormat("GetBarcodesByTravelBarcode By Barcode Barcode ={0}, L4 ={1}", Barcode, L4);
            IList<Detxn> Detail = null;

            try
            {
                Detail = (from gb in dcap.Detxn
                          where gb.BarCodeNo == Barcode
                          select new Detxn
                          {
                              BarCodeNo = gb.BarCodeNo,
                          }).ToList();
                if (Detail != null)
                {

                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Detail;
        }


        [Produces("application/json")]
        [HttpGet("ValidateTravelBarcodebyBarcode")]
        public Boolean ValidateTravelBarcodebyBarcode(string Barcode, string TravelBarcode)
        {

            logger.InfoFormat("Validate Travel Barcode by Barcode By Barcode Barcode ={0}, TravelBarcode={1}", Barcode, TravelBarcode);
            Boolean checker = false;
            List<Detxn> Detail = null;

            try
            {
                Detail = (from gb in dcap.Detxn
                          where gb.BarCodeNo == Barcode && gb.OperationCode == 151 && gb.TravelBarCodeNo != ""
                          select new Detxn
                          {
                              TravelBarCodeNo = gb.TravelBarCodeNo,
                          }).ToList();

                if (Detail.Count != 0)
                {
                    foreach (Detxn d in Detail)
                    {
                        if (d.TravelBarCodeNo == TravelBarcode)
                        {
                            checker = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return checker;
        }

        public String GetGroupBarcodebyBarcode(string Barcode, uint l1, uint l2, uint l3, uint l4, uint l5, int TxnMode)
        {

            logger.InfoFormat("GetBarcodesByTravelBarcode By Barcode Barcode ={0}, L1 ={1}, L2 ={2}, L3 ={3}, L4 ={4}, L4 ={5}", Barcode, l1, l2, l3, l4, l5);
            Detxn Detail = null;
            String Tag = null;

            try
            {
                Detail = (from gb in dcap.Detxn
                          where gb.BarCodeNo == Barcode && gb.L1id == l1 && gb.L2id == l2 && gb.L3id == l3 && gb.L4id == l4 && gb.L5id == l5 && gb.BagBarCodeNo != "" && gb.TravelBarCodeNo != "" //&& gb.OperationCode == 151
                          orderby gb.TxnDateTime descending
                          select new Detxn
                          {
                              BarCodeNo = TxnMode == 1 ? gb.BagBarCodeNo : gb.TravelBarCodeNo,
                          }).FirstOrDefault();
                if (Detail != null)
                {
                    Tag = Detail.BarCodeNo;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Tag;
        }

        public Boolean CheckforOperationInTraveltag(string TravelTag, int OpCode, int ScanType, int Plussminus, int WfdepinstId, int SMode)
        {
            logger.InfoFormat("GetBarcodesByTravelBarcode By Barcode TravelTag ={0}", TravelTag);
            Boolean Tag = false;
            TravelBarcodeDetails Detail = null;
            LookupController lookup = new LookupController(dcap);

            try
            {
                if (ScanType != 1)
                {
                    if (Plussminus == 1)
                    {
                        Detail = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == TravelTag).FirstOrDefault();
                        if (Detail != null)
                        {
                            if (Detail.TravelStatus != 0)
                            {
                                var opcode = lookup.GetAdjacentNodesForGivenNode(WfdepinstId, 2);
                                if (opcode.Count != 0)
                                {
                                    var maxopcode = opcode.Max(c => c.OperationCode);
                                    if (Detail.TravelStatus == (int)maxopcode)
                                    {
                                        Tag = true;
                                    }
                                }
                            }
                            else
                            {
                                Tag = true;
                            }
                        }
                    }
                    else
                    {
                        Detail = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == TravelTag).FirstOrDefault();
                        if (Detail != null)
                        {
                            if (Detail.TravelStatus == OpCode)
                            {
                                Tag = true;
                            }
                        }
                    }
                }
                else
                {
                    if (SMode == 1)
                    {
                        Detail = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == TravelTag).FirstOrDefault();
                        if (Detail != null)
                        {
                            if (Detail.TravelStatus != 0)
                            {
                                var opcode = lookup.GetAdjacentNodesForGivenNode(WfdepinstId, 2);
                                if (opcode.Count != 0)
                                {
                                    var maxopcode = opcode.Max(c => c.OperationCode);
                                    if (Detail.TravelStatus == (int)maxopcode)
                                    {
                                        Tag = true;
                                    }
                                }
                            }
                            else
                            {
                                Tag = true;
                            }
                        }
                    }
                    else
                    {
                        var TravelTagDetail = lookup.GetTravelTagDetailsbyGarmentBarcode(TravelTag, 0);
                        if (TravelTagDetail != null)
                        {
                            var opcode = lookup.GetAdjacentNodesForGivenNode(WfdepinstId, 2);
                            if (opcode.Count != 0)
                            {
                                if (TravelTagDetail.TravelStatus == (int)opcode[0].OperationCode)
                                {
                                    Tag = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Tag;
        }

        public Boolean CheckforMutipleSheduledTags(UserInput ui)
        {

            logger.InfoFormat("CheckforMutipleSheduledTags By Barcode UserInput ={0}", ui);
            Boolean Tag = true;
            List<GroupBarcode> Detail = new List<GroupBarcode>();

            try
            {
                Detail = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == ui.Barcode && c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L4id == ui.ColorId).ToList();
                if (Detail.Count != 1)
                {
                    Tag = false;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Tag;
        }

        public Boolean CheckBarcodeisinTravelBarcode(string barcode, string travelbarcode)
        {

            logger.InfoFormat("Check Barcode is in Travel Barcode barcode ={0}, travelbarcode={1}", barcode, travelbarcode);
            Boolean Tag = true;

            try
            {

                List<Detxn> Detail = dcap.Detxn.Where(c => c.BarCodeNo == barcode && c.TravelBarCodeNo != "").ToList();
                if (Detail.Count != 0)
                {
                    foreach (Detxn det in Detail)
                    {
                        if (det.TravelBarCodeNo != travelbarcode)
                        {
                            Tag = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return Tag;
        }
        //Travel Barcode API's: END

        [Produces("application/json")]
        [HttpGet("GetAllProductionWarehouses")]
        public IList<Wf> GetAllProductionWarehouses()
        {

            logger.InfoFormat("Get All Production Warehouses");
            IList<Wf> WF = null;

            try
            {
                WF = dcap.Wf.Where(w => w.RecStatus == 1).OrderBy(c => c.Wfdesc).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return WF;
        }

        //Update Style Details: START
        //Get ALl Styles
        [Produces("application/json")]
        [HttpGet("GetAllStyles")]
        public IList<L1> GetAllStyles()
        {

            logger.InfoFormat("GetAllStyles");
            IList<L1> Styles = null;

            try
            {
                Styles = (from l1 in dcap.L1.Where(c => c.RecStatus == 1).AsQueryable()
                          group l1 by new { l1.L1id, l1.L1desc } into grp//sandeepa@1234
                          orderby grp.Key.L1desc
                          select new L1
                          {
                              L1id = grp.Key.L1id,
                              L1desc = grp.Key.L1desc,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        //Get ALl Shedules
        [Produces("application/json")]
        [HttpGet("GetAllShedules")]
        public IList<L2> GetAllShedules(int l1)
        {

            logger.InfoFormat("GetAllShedules l1={0}", l1);
            IList<L2> Styles = null;

            try
            {
                Styles = (from l2 in dcap.L2.Where(c => c.RecStatus == 1).AsQueryable()
                          where l2.L1id == l1
                          group l2 by new { l2.L2id, l2.L2desc } into grp
                          orderby grp.Key.L2desc
                          select new L2
                          {
                              L2id = grp.Key.L2id,
                              L2desc = grp.Key.L2desc,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        [Produces("application/json")]
        [HttpGet("GetAllColors")]
        public IList<L4> GetAllColors(int l1, int l2)
        {

            logger.InfoFormat("GetAllColors l1={0}, l2={1}", l1, l2);
            IList<L4> Styles = null;

            try
            {
                Styles = (from l4 in dcap.L4
                          where l4.L1id == l1 && l4.L2id == l2
                          group l2 by new { l4.L4id, l4.L4desc } into grp
                          select new L4
                          {
                              L4id = grp.Key.L4id,
                              L4desc = grp.Key.L4desc,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        [Produces("application/json")]
        [HttpGet("GetAllWashes")]
        public IList<Washes> GetAllWashes(string WashName)
        {

            logger.InfoFormat("GetAllWashes WashName={0}", WashName);
            IList<Washes> Styles = null;
            WashName = WashName == null ? "%" : WashName;

            try
            {
                Styles = (from w in dcap.Washes
                          where EF.Functions.Like(w.WashName.ToUpper(), "%" + WashName.ToUpper() + "%")
                          select new Washes
                          {
                              WashCatId = w.WashCatId,
                              WashName = w.WashName,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        [Produces("application/json")]
        [HttpGet("GetAllSFProcess")]
        public IList<SFprocess> GetAllSFProcess(string SFName)
        {

            logger.InfoFormat("GetAllSFProcess SFName={0}", SFName);
            IList<SFprocess> Styles = null;
            SFName = SFName == null ? "%" : SFName;

            try
            {
                Styles = (from w in dcap.SFprocess
                          where EF.Functions.Like(w.SFName.ToUpper(), "%" + SFName.ToUpper() + "%")
                          select new SFprocess
                          {
                              SFCatId = w.SFCatId,
                              SFName = w.SFName,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        //Get location GRed bag summary
        //Used API's and UI : getLocationDetails WEB (GoodRecive)
        [Produces("application/json")]
        [HttpGet("GetLocationDetailsbyLoccode")]
        public IList<TeamCounterCM> GetLocationDetailsbyLoccode(string Loccode, int smode, string faccode)
        {

            logger.InfoFormat("Get Location Details by Loccode Loccode={0}, smode={1} faccode={2}", Loccode, smode, faccode);
            IList<TeamCounterCM> Styles = null;

            try
            {
                if (smode == 2)
                {
                    IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.WarLocCode == Loccode).AsQueryable()
                                                       join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                       select new GoodControl
                                                       {
                                                           L1id = gdc.L1id,
                                                           L2id = gdc.L2id,
                                                           L3id = gdc.L3id,
                                                           L4id = gdc.L4id,
                                                           L5id = gdc.L5id,
                                                           BarCodeNo = gdc.BarCodeNo,
                                                           TxnMode = gdc.TxnMode,
                                                           TxnStatus = gdc.TxnStatus,
                                                           WarLocCode = gdc.WarLocCode,
                                                           RecStatus = gdc.RecStatus,
                                                           Return = gdc.Return,
                                                           Qty01 = gdc.Qty01,
                                                           Qty02 = gdc.Qty02,
                                                           Qty03 = gdc.Qty03,
                                                       }).ToList();

                    Styles = (from goodcontrol in goodControls
                              join l1 in dcap.L1 on new { A = goodcontrol.L1id } equals new { A = l1.L1id }
                              join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                              join l4 in dcap.L4 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                              join l5 in dcap.L5 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.L5id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id, E = l5.L5id }
                              join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                              join wf in dcap.Wf on groupbrcode.WFId equals wf.Wfid
                              where goodcontrol.WarLocCode == Loccode && goodcontrol.TxnStatus == 5 && groupbrcode.Qty01 != groupbrcode.Qty01NS && groupbrcode.RecStatus == 1 && goodcontrol.Return == 0
                              group goodcontrol by new
                              {
                                  wf.Wfid,
                                  wf.Wfdesc,
                                  goodcontrol.L1id,
                                  l1.L1desc,
                                  goodcontrol.L2id,
                                  l2.L2desc,
                                  goodcontrol.L4id,
                                  l4.L4desc,
                                  goodcontrol.L5id,
                                  l5.L5desc,
                                  goodcontrol.BarCodeNo,
                                  goodcontrol.TxnMode,
                                  goodcontrol.WarLocCode,
                                  //goodcontrol.Qty01
                              }
                        into grp
                              select new TeamCounterCM
                              {
                                  WFIdBag = grp.Key.Wfid,
                                  WFIdDesc = grp.Key.Wfdesc,
                                  StyleDesc = grp.Key.L1desc,
                                  ScheduleDesc = grp.Key.L2desc,
                                  ColorDesc = grp.Key.L4desc,
                                  SizeDesc = grp.Key.L5desc,
                                  BagBarCode = grp.Key.BarCodeNo,
                                  TxnMode = grp.Key.TxnMode,
                                  WarLocCode = grp.Key.WarLocCode,
                                  ModifiedDateTime = grp.Max(c => c.RecivedDateTime),
                                  Qty01 = (int)(grp.Sum(c => c.Qty01) - grp.Sum(c => c.Qty02) - grp.Sum(c => c.Qty03)),
                              }).ToList();
                }
                else
                {
                    IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.WarLocCode == Loccode).AsQueryable()
                                                       join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                       select new GoodControl
                                                       {
                                                           L1id = gdc.L1id,
                                                           L2id = gdc.L2id,
                                                           L3id = gdc.L3id,
                                                           L4id = gdc.L4id,
                                                           L5id = gdc.L5id,
                                                           BarCodeNo = gdc.BarCodeNo,
                                                           TxnMode = gdc.TxnMode,
                                                           TxnStatus = gdc.TxnStatus,
                                                           WarLocCode = gdc.WarLocCode,
                                                           RecStatus = gdc.RecStatus,
                                                           Return = gdc.Return,
                                                           Qty01 = gdc.Qty01,
                                                           Qty02 = gdc.Qty02,
                                                           Qty03 = gdc.Qty03,
                                                       }).ToList();

                    Styles = (from goodcontrol in goodControls
                              join l1 in dcap.L1 on new { A = goodcontrol.L1id } equals new { A = l1.L1id }
                              join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                              join l4 in dcap.L4 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                              join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                              join wf in dcap.Wf on groupbrcode.WFId equals wf.Wfid
                              where goodcontrol.WarLocCode == Loccode && goodcontrol.TxnStatus == 5 && groupbrcode.Qty01 != groupbrcode.Qty01NS && groupbrcode.RecStatus == 1 && goodcontrol.Return == 0
                              group goodcontrol by new
                              {
                                  wf.Wfid,
                                  wf.Wfdesc,
                                  goodcontrol.L1id,
                                  l1.L1desc,
                                  goodcontrol.L2id,
                                  l2.L2desc,
                                  goodcontrol.L4id,
                                  l4.L4desc,
                                  goodcontrol.BarCodeNo,
                                  goodcontrol.TxnMode,
                                  goodcontrol.WarLocCode,
                                  //goodcontrol.Qty01
                              }
                        into grp
                              select new TeamCounterCM
                              {
                                  WFIdBag = grp.Key.Wfid,
                                  WFIdDesc = grp.Key.Wfdesc,
                                  StyleDesc = grp.Key.L1desc,
                                  ScheduleDesc = grp.Key.L2desc,
                                  ColorDesc = grp.Key.L4desc,
                                  BagBarCode = grp.Key.BarCodeNo,
                                  TxnMode = grp.Key.TxnMode,
                                  WarLocCode = grp.Key.WarLocCode,
                                  ModifiedDateTime = grp.Max(c => c.RecivedDateTime),
                                  Qty01 = (int)(grp.Sum(c => c.Qty01) - grp.Sum(c => c.Qty02) - grp.Sum(c => c.Qty03)),
                              }).ToList();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        //Get details by control id summaryf
        //Used API's and UI : searchForDispatchItemDetail WEB (GoodRecive)
        [Produces("application/json")]
        [HttpGet("GetDetailsbyControlId")]
        public IList<TeamCounterCM> GetDetailsbyControlId(string ControlId)
        {

            logger.InfoFormat("Get Details by ControlId ControlId={0}", ControlId);
            IList<TeamCounterCM> Styles = null;

            try
            {
                Styles = (from goodcontrol in dcap.GoodControl
                          join l1 in dcap.L1 on new { A = goodcontrol.L1id } equals new { A = l1.L1id }
                          join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                          join l4 in dcap.L4 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                          join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                          where goodcontrol.ControlId == ControlId && groupbrcode.Qty01 != groupbrcode.Qty01NS
                          group goodcontrol by new
                          {
                              goodcontrol.L1id,
                              l1.L1desc,
                              goodcontrol.L2id,
                              l2.L2desc,
                              l2.Ref01,
                              l2.Ref02,
                              goodcontrol.L4id,
                              l4.L4desc,
                              goodcontrol.BarCodeNo,
                              goodcontrol.TxnMode,
                              goodcontrol.WarLocCode,
                              groupbrcode.TxnStatus,
                              groupbrcode.Qty01,
                              groupbrcode.Qty01NS
                          }
                        into grp
                          select new TeamCounterCM
                          {
                              StyleDesc = grp.Key.L1desc,
                              ScheduleDesc = grp.Key.L2desc,
                              PONo = grp.Key.Ref01,
                              Zfeature = grp.Key.Ref02,
                              ColorDesc = grp.Key.L4desc,
                              BagBarCode = grp.Key.BarCodeNo,
                              TxnMode = grp.Key.TxnMode,
                              TxnStatus = grp.Key.TxnStatus,
                              WarLocCode = grp.Key.WarLocCode == null ? "Receive Pending" : grp.Key.WarLocCode,
                              ModifiedDateTime = grp.Max(c => c.RecivedDateTime),
                              Qty01 = (int)grp.Key.Qty01 - (int)grp.Key.Qty01NS,
                          }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        //Get all locations GRed bag summary
        //Used API's and UI : getLocationWiseBagDetails WEB (GoodRecive)
        [Produces("application/json")]
        [HttpGet("GetLocationBagDetails")]
        public IList<TeamCounterCM> GetLocationBagDetails(string style, string shedule, string color, int wfid, int smode, string faccode)
        {

            logger.InfoFormat("Get Location Bag Details style={0} shedule={1} color={2}, wfid={3}, smode={4}", style, shedule, color, wfid, smode, faccode);
            IList<TeamCounterCM> Styles = null;

            style = style == null ? "" : style;
            shedule = shedule == null ? "%" : shedule;
            color = color == null ? "%" : color;

            try
            {
                if (smode == 2)
                {
                    IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.Return == 0).AsQueryable()
                                                       join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                       select new GoodControl
                                                       {
                                                           L1id = gdc.L1id,
                                                           L2id = gdc.L2id,
                                                           L3id = gdc.L3id,
                                                           L4id = gdc.L4id,
                                                           L5id = gdc.L5id,
                                                           BarCodeNo = gdc.BarCodeNo,
                                                           TxnMode = gdc.TxnMode,
                                                           TxnStatus = gdc.TxnStatus,
                                                           WarLocCode = gdc.WarLocCode,
                                                           RecStatus = gdc.RecStatus,
                                                           Return = gdc.Return,
                                                       }).ToList();

                    Styles = (from goodcontrol in goodControls
                              join l1 in dcap.L1 on new { A = goodcontrol.L1id } equals new { A = l1.L1id }
                              join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                              join l4 in dcap.L4 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                              join l5 in dcap.L5 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.L5id } equals new { A = l5.L1id, B = l5.L2id, C = l5.L3id, D = l5.L4id, E = l5.L5id }
                              join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                              join wf in dcap.Wf on groupbrcode.WFId equals wf.Wfid
                              where (wfid == 0 ? true : groupbrcode.WFId == wfid) && goodcontrol.TxnStatus == 5 && groupbrcode.Qty01 != groupbrcode.Qty01NS && EF.Functions.Like(l4.L4desc, "%" + color + "%") && goodcontrol.Return == 0
                              && EF.Functions.Like(l2.L2desc, "%" + shedule + "%") && EF.Functions.Like(l1.L1desc, "%" + style + "%") && goodcontrol.RecStatus == 1
                              group new { goodcontrol, groupbrcode } by new
                              {
                                  wf.Wfid,
                                  wf.Wfdesc,
                                  goodcontrol.WarLocCode,
                                  goodcontrol.L1id,
                                  l1.L1desc,
                                  goodcontrol.L2id,
                                  l2.L2desc,
                                  goodcontrol.L4id,
                                  l4.L4desc,
                                  goodcontrol.L5id,
                                  l5.L5desc,
                                  goodcontrol.BarCodeNo,
                                  goodcontrol.TxnMode,
                                  groupbrcode.Qty01,
                                  groupbrcode.Qty01NS
                              }
                                            into grp
                              select new TeamCounterCM
                              {
                                  WFIdBag = grp.Key.Wfid,
                                  WFIdDesc = grp.Key.Wfdesc,
                                  Location = grp.Key.WarLocCode,
                                  StyleDesc = grp.Key.L1desc,
                                  ScheduleDesc = grp.Key.L2desc,
                                  ColorDesc = grp.Key.L4desc,
                                  SizeDesc = grp.Key.L5desc,
                                  BagBarCode = grp.Key.BarCodeNo,
                                  TxnMode = grp.Key.TxnMode,
                                  WarLocCode = grp.Key.WarLocCode,
                                  ModifiedDateTime = grp.Max(c => c.goodcontrol.RecivedDateTime),
                                  Qty01 = (int)grp.Key.Qty01 - (int)grp.Key.Qty01NS
                              }).ToList();

                }
                else
                {
                    IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.Return == 0).AsQueryable()
                                                       join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                       select new GoodControl
                                                       {
                                                           L1id = gdc.L1id,
                                                           L2id = gdc.L2id,
                                                           L3id = gdc.L3id,
                                                           L4id = gdc.L4id,
                                                           BarCodeNo = gdc.BarCodeNo,
                                                           TxnMode = gdc.TxnMode,
                                                           TxnStatus = gdc.TxnStatus,
                                                           WarLocCode = gdc.WarLocCode,
                                                           RecStatus = gdc.RecStatus,
                                                           Return = gdc.Return,
                                                       }).ToList();

                    Styles = (from goodcontrol in goodControls
                              join l1 in dcap.L1 on new { A = goodcontrol.L1id } equals new { A = l1.L1id }
                              join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                              join l4 in dcap.L4 on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                              join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                              join wf in dcap.Wf on groupbrcode.WFId equals wf.Wfid
                              where (wfid == 0 ? true : groupbrcode.WFId == wfid) && goodcontrol.TxnStatus == 5 && groupbrcode.Qty01 != groupbrcode.Qty01NS
                               && EF.Functions.Like(l4.L4desc, "%" + color + "%") 
                               && goodcontrol.Return == 0
                              && EF.Functions.Like(l2.L2desc, "%" + shedule + "%") && EF.Functions.Like(l1.L1desc, "%" + style + "%") 
                              && goodcontrol.RecStatus == 1
                              group new { goodcontrol, groupbrcode } by new
                              {
                                  wf.Wfid,
                                  wf.Wfdesc,
                                  goodcontrol.WarLocCode,
                                  goodcontrol.L1id,
                                  l1.L1desc,
                                  goodcontrol.L2id,
                                  l2.L2desc,
                                  goodcontrol.L4id,
                                  l4.L4desc,
                                  goodcontrol.BarCodeNo,
                                  goodcontrol.TxnMode,
                                  groupbrcode.Qty01,
                                  groupbrcode.Qty01NS
                              }
                            into grp
                              select new TeamCounterCM
                              {
                                  WFIdBag = grp.Key.Wfid,
                                  WFIdDesc = grp.Key.Wfdesc,
                                  Location = grp.Key.WarLocCode,
                                  StyleDesc = grp.Key.L1desc,
                                  ScheduleDesc = grp.Key.L2desc,
                                  ColorDesc = grp.Key.L4desc,
                                  BagBarCode = grp.Key.BarCodeNo,
                                  TxnMode = grp.Key.TxnMode,
                                  WarLocCode = grp.Key.WarLocCode,
                                  ModifiedDateTime = grp.Max(c => c.goodcontrol.RecivedDateTime),
                                  Qty01 = (int)grp.Key.Qty01 - (int)grp.Key.Qty01NS
                              }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }

        [Produces("application/json")]
        [HttpGet("GetAllStyleSheduleColorDetails")]
        public IList<RunningStyleDetails> GetAllStyleSheduleColorDetails(int l1id, int l2id, int l4id)
        {

            logger.InfoFormat("GetAllColors l1={0}, l2={1}, l2={2}", l1id, l2id, l4id);
            IList<RunningStyleDetails> Styles = null;

            try
            {
                Styles = (from l1 in dcap.L1
                          join l2 in dcap.L2 on new { A = (uint?)l1.L1id } equals new { A = (uint?)l2.L1id }
                          join l4 in dcap.L4 on new { A = (uint?)l2.L1id, B = (uint?)l2.L2id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id }
                          join sf in dcap.SFprocess on l4.SFCatId equals sf.SFCatId
                          join wt in dcap.Washes on l4.WashCatId equals wt.WashCatId
                          where (l1id == -1 ? true : l1.L1id == l1id) && (l2id == -1 ? true : l2.L2id == l2id) && (l4id == -1 ? true : l4.L4id == l4id)
                          group l4 by new
                          {
                              l1.L1id,
                              l1.L1desc,
                              l2.L2id,
                              l2.L2desc,
                              l2.Ref01,
                              l2.Ref02,
                              l4.L4id,
                              l4.L4desc,
                              l4.WashDescription,
                              l4.WashType,
                              sf.SFCatId,
                              sf.SFName,
                              wt.WashCatId,
                              wt.WashName,
                              l4.SubinPO,
                              l4.GarmentWeight,
                              l4.WashDuration,
                              l4.UnitPrice,
                              l4.Category
                          } into grp
                          select new RunningStyleDetails
                          {
                              Key = grp.Key.L1id + "|" + grp.Key.L2id + "|" + grp.Key.L4id,
                              L1 = grp.Key.L1id,
                              Style = grp.Key.L1desc,
                              L2 = grp.Key.L2id,
                              Shedule = grp.Key.L2desc,
                              PO = grp.Key.Ref01,
                              Zfeature = grp.Key.Ref02,
                              L4 = grp.Key.L4id,
                              Color = grp.Key.L4desc,
                              SubinPO = grp.Key.SubinPO,
                              WashDescription = grp.Key.WashDescription,
                              WashType = grp.Key.WashType,
                              WashProcessID = grp.Key.SFCatId,
                              WashProcess = grp.Key.SFName,
                              WashTypeID = grp.Key.WashCatId,
                              WashName = grp.Key.WashName,
                              GMTWeight = grp.Key.GarmentWeight,
                              WashDuration = grp.Key.WashDuration,
                              UnitPrice = grp.Key.UnitPrice,
                              Category = grp.Key.Category,
                          }).ToList();

                if (Styles.Count == 0)
                {
                    Styles = (from l1 in dcap.L1
                              join l2 in dcap.L2 on new { A = (uint?)l1.L1id } equals new { A = (uint?)l2.L1id }
                              join l4 in dcap.L4 on new { A = (uint?)l2.L1id, B = (uint?)l2.L2id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id }
                              where (l1id == -1 ? true : l1.L1id == l1id) && (l2id == -1 ? true : l2.L2id == l2id) && (l4id == -1 ? true : l4.L4id == l4id)
                              group l4 by new
                              {
                                  l1.L1id,
                                  l1.L1desc,
                                  l2.L2id,
                                  l2.L2desc,
                                  l2.Ref01,
                                  l2.Ref02,
                                  l4.L4id,
                                  l4.L4desc,
                                  l4.GarmentWeight,
                                  l4.WashDuration,
                                  l4.UnitPrice,
                                  l4.Category
                              } into grp
                              select new RunningStyleDetails
                              {
                                  Key = grp.Key.L1id + "|" + grp.Key.L2id + "|" + grp.Key.L4id,
                                  L1 = grp.Key.L1id,
                                  Style = grp.Key.L1desc,
                                  L2 = grp.Key.L2id,
                                  Shedule = grp.Key.L2desc,
                                  PO = grp.Key.Ref01,
                                  Zfeature = grp.Key.Ref02,
                                  L4 = grp.Key.L4id,
                                  Color = grp.Key.L4desc,

                                  WashProcess = "",
                                  WashType = "",
                                  GMTWeight = grp.Key.GarmentWeight,
                                  WashDuration = grp.Key.WashDuration,
                                  UnitPrice = grp.Key.UnitPrice,
                                  Category = grp.Key.Category,
                              }).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());
            }
            return Styles;
        }
        //Update Style Details: END

        //Error Reporting: START
        //Travel Barcode Dispatch Operation Error
        [Produces("application/json")]
        [HttpGet("GetTravelTagErrorDetail")]
        public List<BarcodeChecker> GetTravelTagErrorDetail()
        {

            logger.InfoFormat("Get Travel Tag Error Detail");
            List<BarcodeChecker> L1Details = new List<BarcodeChecker>();

            try
            {
                IList<GroupBarcode> L2Details = (from detxn in dcap.GroupBarcode.Where(detxn => detxn.TxnMode == 2 && detxn.TxnStatus > 0 && detxn.TxnStatus < 6).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     L1id = detxn.L1id,
                                                     L2id = detxn.L2id,
                                                     L3id = detxn.L3id,
                                                     L4id = detxn.L4id,
                                                     BagBarCodeNo = detxn.BagBarCodeNo
                                                 }).ToList();

                foreach (GroupBarcode qi in L2Details)
                {
                    L1Details = L1Details.Concat(CheckOerationsBarcodeDetailsFromTravelTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id)).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public List<BarcodeChecker> CheckOerationsBarcodeDetailsFromTravelTag(string travelcode, uint l1id, uint l2id, uint l3id, uint l4id)
        {

            logger.InfoFormat("Get Factory Name travelcode={0}, l1id={1}, l1id={2}, l1id={3}, l1id={4}", travelcode, l1id, l2id, l3id, l4id);
            List<BarcodeChecker> BarcodeDetail = null;

            try
            {
                String Barcode = dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.TravelBarCodeNo == travelcode).Select(c => c.BarCodeNo).FirstOrDefault();

                BarcodeDetail = (from detxn in dcap.Detxn.Where(detxn => detxn.BarCodeNo == Barcode).AsQueryable()
                                 select new BarcodeChecker
                                 {
                                     BarCodeNo = detxn.BarCodeNo,
                                     TravelBarCodeNo = travelcode,
                                     OperationCode = detxn.OperationCode,
                                     DetxnKey = detxn.DetxnKey
                                 }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return BarcodeDetail;
        }

        public List<BarcodeChecker> GetTravelTagPrevDetail(UserInput element)
        {

            logger.InfoFormat("Get Travel Tag Error Detail");
            List<BarcodeChecker> L1Details = new List<BarcodeChecker>();

            try
            {
                IList<GroupBarcode> L2Details = (from detxn in dcap.GroupBarcode.Where(detxn => detxn.TxnMode == 2 && detxn.BagBarCodeNo == element.BagBarCodeNo).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     L1id = detxn.L1id,
                                                     L2id = detxn.L2id,
                                                     L3id = detxn.L3id,
                                                     L4id = detxn.L4id,
                                                     BagBarCodeNo = detxn.BagBarCodeNo
                                                 }).ToList();

                foreach (GroupBarcode qi in L2Details)
                {
                    decimal? PrevQty01 = 0; decimal? PrevQty02 = 0; decimal? PrevQty03 = 0;
                    List<Detxn> Output = CheckOperationsDetailsFromTravelTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id, element.OperationCode);

                    foreach (Detxn di in Output)
                    {
                        PrevQty01 = PrevQty01 + di.Qty01;
                        PrevQty02 = PrevQty02 + di.Qty02;
                        PrevQty03 = PrevQty03 + di.Qty03;
                    }


                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public List<Detxn> CheckOperationsDetailsFromTravelTag(string travelcode, uint l1id, uint l2id, uint l3id, uint l4id, int OpCode)
        {

            logger.InfoFormat("Get Factory Name travelcode={0}, l1id={1}, l1id={2}, l1id={3}, l1id={4}", travelcode, l1id, l2id, l3id, l4id);
            List<Detxn> BarcodeDetail = null;

            try
            {
                BarcodeDetail = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.TravelBarCodeNo == travelcode && detxn.JobNo == travelcode && detxn.OperationCode == OpCode)
                                 group detxn by new
                                 {
                                     detxn.L1id,
                                     detxn.L2id,
                                     detxn.L3id,
                                     detxn.L4id,
                                     detxn.OperationCode
                                 } into grp
                                 select new Detxn
                                 {
                                     Qty01 = grp.Sum(c => c.Qty01),
                                     Qty02 = grp.Sum(c => c.Qty02),
                                     Qty03 = grp.Sum(c => c.Qty03)
                                 }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return BarcodeDetail;
        }

        public Boolean GetTravelTagUsedDetail(string travelbarcode, int operationcode, int Plusminus)
        {

            logger.InfoFormat("Get Travel Tag Error Detail travelbarcode={0}, operationcode={1}, Plusminus={2}", travelbarcode, operationcode, Plusminus);
            Boolean checker = true;

            try
            {
                IList<GroupBarcode> L2Details = (from detxn in dcap.GroupBarcode.Where(detxn => detxn.BagBarCodeNo == travelbarcode && detxn.TxnMode >= 2).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     L1id = detxn.L1id,
                                                     L2id = detxn.L2id,
                                                     L3id = detxn.L3id,
                                                     L4id = detxn.L4id,
                                                     BagBarCodeNo = detxn.BagBarCodeNo

                                                 }).ToList();

                foreach (GroupBarcode qi in L2Details)
                {
                    if (checker)
                    {
                        checker = CheckOperationsBarcodeDetailsFromTravelTag(qi.BagBarCodeNo, qi.L1id, qi.L2id, qi.L3id, qi.L4id, operationcode, Plusminus);
                        if (checker == false)
                        {
                            return checker;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return checker;
        }

        public Boolean CheckOperationsBarcodeDetailsFromTravelTag(string travelcode, uint l1id, uint l2id, uint l3id, uint l4id, int operationcode, int plusminus)
        {

            logger.InfoFormat("Get Factory Name travelcode={0}, l1id={1}, l1id={2}, l1id={3}, l1id={4}, operationcode={5}", travelcode, l1id, l2id, l3id, l4id, operationcode);
            Boolean checker = true;

            try
            {
                /*List<Detxn> Barcodes = dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                             detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id).ToList();

                if (Barcodes.Count != 0)
                {
                    foreach (Detxn det in Barcodes)
                    {
                        if(checker){
                            var BarcodeDetail1 = (from detxn in Barcodes.Where(detxn => detxn.L1id == det.L1id && detxn.L2id == det.L2id && detxn.L3id == det.L3id && detxn.L4id == l4id && detxn.L5id == det.L5id && detxn.BarCodeNo == det.BarCodeNo && detxn.OperationCode == operationcode).AsQueryable()
                                                select new BarcodeChecker
                                                {
                                                    BarCodeNo = detxn.BarCodeNo,
                                                    TravelBarCodeNo = travelcode,
                                                    OperationCode = detxn.OperationCode,
                                                    DetxnKey = detxn.DetxnKey,
                                                    Qty01 = detxn.Qty01
                                                }).ToList();

                            if (BarcodeDetail1.Count != 0)
                            {
                                if (BarcodeDetail1.Sum(d => d.Qty01) != 0)
                                {
                                    checker = false;
                                    return checker;
                                }
                            }
                        }
                    }
                }*/


                if (checker)
                {
                    var BarcodeDetail2 = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == l1id &&
                                detxn.L2id == l2id && detxn.L3id == l3id && detxn.L4id == l4id && detxn.OperationCode == operationcode && detxn.JobNo == travelcode).AsQueryable()
                                          select new BarcodeChecker
                                          {
                                              BarCodeNo = detxn.BarCodeNo,
                                              TravelBarCodeNo = travelcode,
                                              OperationCode = detxn.OperationCode,
                                              DetxnKey = detxn.DetxnKey,
                                              Qty01 = detxn.Qty01
                                          }).ToList();

                    if (BarcodeDetail2.Count != 0)
                    {
                        if (BarcodeDetail2.Sum(d => d.Qty01) != 0)
                        {
                            checker = false;
                            return checker;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return checker;
        }

        [Produces("application/json")]
        [HttpGet("CheckForSingleScanedBarcode")]
        public String CheckForSingleScanedBarcode(string travelbarcode, int txnmode, int OperationCode)
        {
            logger.InfoFormat("Get All Barcode Details by Barcode travelbarcode={0}, txnmode={1}, OperationCode={2}", travelbarcode, txnmode, OperationCode);
            String checker = "";
            //List<Detxn> detxn = new List<Detxn>();

            try
            {
                IList<GroupBarcode> LcDetails = (from detxn in dcap.GroupBarcode.Where(detxn => detxn.BagBarCodeNo == travelbarcode && detxn.TxnMode >= txnmode).AsQueryable()
                                                 select new GroupBarcode
                                                 {
                                                     L1id = detxn.L1id,
                                                     L2id = detxn.L2id,
                                                     L3id = detxn.L3id,
                                                     L4id = detxn.L4id,
                                                     BagBarCodeNo = detxn.BagBarCodeNo,
                                                     TxnMode = detxn.TxnMode,
                                                     OperationCode = detxn.OperationCode
                                                 }).ToList();

                foreach (GroupBarcode qi in LcDetails)
                {
                    List<Detxn> L1Details = new List<Detxn>();

                    if (qi.TxnMode == 2)
                    {
                        L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == qi.L1id &&
                                            detxn.L2id == qi.L2id && detxn.L4id == qi.L4id && detxn.TravelBarCodeNo == qi.BagBarCodeNo && detxn.OperationCode == qi.OperationCode
                                            && detxn.Qty01 != 0 && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1).AsQueryable()
                                     group detxn by new
                                     {
                                         detxn.BarCodeNo
                                     } into grp
                                     select new Detxn
                                     {
                                         BarCodeNo = grp.Key.BarCodeNo,
                                         Qty01 = (grp.Sum(detxn => detxn.Qty01))
                                     }).ToList();

                        if (L1Details.Count != 0)
                        {
                            foreach (Detxn d in L1Details)
                            {
                                if (d.Qty01 != 0)
                                {
                                    var L2Details = (from detxn in dcap.Detxn.Where(detxn => detxn.BarCodeNo == d.BarCodeNo && detxn.OperationCode == OperationCode).AsQueryable()
                                                     select new Detxn
                                                     {
                                                         Qty01 = ((detxn.Qty01 == null ? 0 : detxn.Qty01) - (detxn.Qty02 == null ? 0 : detxn.Qty02) - (detxn.Qty03 == null ? 0 : detxn.Qty03)) - ((detxn.Qty01Ns == null ? 0 : detxn.Qty01Ns) + (detxn.Qty02Ns == null ? 0 : detxn.Qty02Ns) + (detxn.Qty03Ns == null ? 0 : detxn.Qty03Ns)),
                                                     }).ToList();

                                    if (L2Details.Count != 0)
                                    {
                                        if (L2Details.Sum(ds => ds.Qty01) == 0)
                                        {
                                            checker = (checker == "" ? d.BarCodeNo : (checker + ", " + d.BarCodeNo));
                                        }
                                    }
                                    else
                                    {
                                        checker = (checker == "" ? d.BarCodeNo : (checker + ", " + d.BarCodeNo));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        L1Details = (from detxn in dcap.Detxn.Where(detxn => detxn.L1id == qi.L1id &&
                                        detxn.L2id == qi.L2id && detxn.L3id == qi.L3id && detxn.L4id == qi.L4id && detxn.OperationCode == qi.OperationCode && detxn.TravelBarCodeNo == qi.BagBarCodeNo
                                        && (detxn.Qty02 + detxn.Qty03) != 0 && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1).AsQueryable()
                                     group detxn by new
                                     {
                                         detxn.BarCodeNo
                                     } into grp
                                     select new Detxn
                                     {
                                         BarCodeNo = grp.Key.BarCodeNo,
                                         Qty02 = (grp.Sum(detxn => detxn.Qty02)),
                                         Qty03 = (grp.Sum(detxn => detxn.Qty03))
                                     }).ToList();

                        if (L1Details.Count != 0)
                        {
                            foreach (Detxn d in L1Details)
                            {
                                if ((d.Qty02 + d.Qty03) != 0)
                                {
                                    var L2Details = (from detxn in dcap.Detxn.Where(detxn => detxn.BarCodeNo == d.BarCodeNo && detxn.OperationCode == OperationCode).AsQueryable()
                                                     select new Detxn
                                                     {
                                                         Qty01 = ((detxn.Qty01 == null ? 0 : detxn.Qty01) - (detxn.Qty02 == null ? 0 : detxn.Qty02) - (detxn.Qty03 == null ? 0 : detxn.Qty03)) - ((detxn.Qty01Ns == null ? 0 : detxn.Qty01Ns) + (detxn.Qty02Ns == null ? 0 : detxn.Qty02Ns) + (detxn.Qty03Ns == null ? 0 : detxn.Qty03Ns)),
                                                     }).ToList();

                                    if (L2Details.Count != 0)
                                    {
                                        if (L2Details.Sum(ds => ds.Qty01) == 0)
                                        {
                                            checker = (checker == "" ? d.BarCodeNo : (checker + ", " + d.BarCodeNo));
                                        }
                                    }
                                    else
                                    {
                                        checker = (checker == "" ? d.BarCodeNo : (checker + ", " + d.BarCodeNo));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return checker;
        }

        //Error Reporting: END

        //Invoice API: START

        [Produces("application/json")]
        [HttpGet("GetInvoiceParameters")]
        public IList<InvoiceParameter> GetInvoiceParameters()
        {

            logger.InfoFormat("GetInvoiceParameters");
            IList<InvoiceParameter> L1Details = null;

            try
            {
                L1Details = dcap.InvoiceParameter.Where(c => c.RecStatus == 1).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Dispatch Lookup - Get All dispatces according to
        //GET api /get DispatchDetails Status by Factory
        //Used API's and UI : RefreshRequestGrid WEB (Dispatch)
        [Produces("application/json")]
        [HttpGet("GetDispatchForInvoiceDetails")]
        public IList<GoodControlDetailsSummary> GetDispatchForInvoiceDetails(DateTime fromDate, DateTime toDate, string loccode)
        {

            logger.InfoFormat("GetDispatchForInvoiceDetails");
            IList<GoodControlDetailsSummary> L1Details = null;

            try
            {
                L1Details = (from detxn in dcap.GoodControlDetails
                             join loc in dcap.Location on detxn.LocCode equals loc.LocCode
                             where detxn.TxnDateTime <= toDate && detxn.TxnDateTime >= fromDate && detxn.LocCode == loccode && detxn.TxnStatus == 5 && detxn.ControlType == 200
                             orderby detxn.TxnDateTime descending
                             select new GoodControlDetailsSummary
                             {
                                 TxnStatus = detxn.TxnStatus,
                                 ControlType = detxn.ControlType,
                                 ControlId = detxn.ControlId,
                                 Seq = detxn.Seq,
                                 Approver = detxn.Approver,
                                 LocFromCode = detxn.LocCodeFrom,
                                 LocName = loc.LocDescription,
                                 Depid = detxn.Depid,
                                 Created = detxn.CreatedBy,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 TxnDateTime = detxn.TxnDateTime,
                                 Qty01 = detxn.Qty01,
                                 Remark = detxn.Remark,
                                 LocAddress = loc.LocAddress,
                             }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetPricebySKU")]
        public Decimal GetPricebySKU(uint l1id, uint l2id, uint l3id, uint l4id)
        {

            logger.InfoFormat("GetPricebySKU l1id={0}, l1id={0}, l2id={1}, l3id={2}, l4id={3}", l1id, l2id, l3id, l4id);
            Decimal Price = 0;

            try
            {
                var Output = (from l4 in dcap.L4
                              where l4.L1id == l1id && l4.L2id == l2id && l4.L3id == l3id && l4.L4id == l4id
                              select new L4 { UnitPrice = l4.UnitPrice }).FirstOrDefault();

                if (Output != null)
                {
                    Price = Output.UnitPrice;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return Price;
        }

        [Produces("application/json")]
        [HttpGet("GetInvoiceHeaderDetails")]
        public InvoiceHeaderInformationOutput GetInvoiceHeaderDetails(string invoiceid)
        {
            InvoiceHeaderInformationOutput L1Details = null;

            logger.InfoFormat("Get Invoice Header Details with invoiceid ={0}", invoiceid);

            try
            {
                L1Details = (from detxn in dcap.InvoiceHeaderInformation
                             join invoicedetails in dcap.InvoiceDetails.GroupBy(c => new { c.L1Id, c.L2Id, c.L3Id, c.L4Id, c.L5Id, c.ControlId, c.InvoiceNo }).Select(c => new { c.Key.L1Id, c.Key.L2Id, c.Key.L3Id, c.Key.L4Id, c.Key.L5Id, c.Key.ControlId, c.Key.InvoiceNo }).AsQueryable() on new { F = detxn.InvoiceNo } equals new { F = invoicedetails.InvoiceNo }
                             join goodcontrol in dcap.GoodControl on new { A = (uint?)invoicedetails.L1Id, B = (uint?)invoicedetails.L2Id, C = (uint?)invoicedetails.L3Id, D = (uint?)invoicedetails.L4Id, E = (uint?)invoicedetails.L5Id, F = invoicedetails.ControlId } equals new { A = (uint?)goodcontrol.L1id, B = (uint?)goodcontrol.L2id, C = (uint?)goodcontrol.L3id, D = (uint?)goodcontrol.L4id, E = (uint?)goodcontrol.L5id, F = goodcontrol.ControlId }
                             join dispatchheaderinformation in dcap.GoodControlDetails on new { A = goodcontrol.ControlId, B = goodcontrol.ControlType } equals new { A = dispatchheaderinformation.ControlId, B = dispatchheaderinformation.ControlType }
                             join loc in dcap.Location on dispatchheaderinformation.LocCode equals loc.LocCode
                             where detxn.InvoiceNo == invoiceid
                             orderby detxn.TxnDateTime
                             select new InvoiceHeaderInformationOutput
                             {
                                 InvoiceNo = detxn.InvoiceNo,
                                 TxnDateTime = detxn.TxnDateTime,
                                 VAT = detxn.VAT,
                                 NBT = detxn.NBT,
                                 ExchangeRate = detxn.ExchangeRate,
                                 TotalQty = detxn.TotalQty,
                                 TotalPrice = detxn.TotalPrice,
                                 BillTo = loc.LocAddress,
                                 DispatchTo = loc.LocAddress,
                                 BillFrom = dispatchheaderinformation.LocCodeFrom,
                                 VATNoTo = loc.VATNo,
                                 SVATNoTo = loc.SVATNo,
                                 Atten = loc.Atten,

                                 CreatedBy = detxn.CreatedBy,
                                 CreatedDateTime = detxn.CreatedDateTime,
                                 ModifiedBy = detxn.ModifiedBy,
                                 ModifiedDateTime = detxn.ModifiedDateTime,
                             }).FirstOrDefault();

                if (L1Details != null)
                {
                    var locFromDetails = GetLocationInfobyLocCode(L1Details.BillFrom);
                    L1Details.BillFrom = locFromDetails.LocAddress;
                    L1Details.VATNoFrom = locFromDetails.VATNo;
                    L1Details.SVATNoFrom = locFromDetails.SVATNo;
                    L1Details.TelNo = locFromDetails.TelNo;

                    L1Details.NBTValue = L1Details.NBT;
                    L1Details.ValueAddedTaxInUSD = L1Details.NBTValue * L1Details.TotalPrice;
                    L1Details.TotalInvoiceVal = L1Details.ValueAddedTaxInUSD;
                    L1Details.VatinLKR = (L1Details.TotalInvoiceVal * L1Details.ExchangeRate) * L1Details.VAT;
                    L1Details.VatSuspendinLKR = L1Details.VatinLKR + L1Details.TotalInvoiceVal;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Status By Barcode information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }

        public Location GetLocationInfobyLocCode(string loccode)
        {

            logger.InfoFormat("Get Location Details by LocCode loccode={0}", loccode);
            Location L1Details = null;

            try
            {
                L1Details = dcap.Location.Where(l => l.LocCode == loccode).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetInvoiceItemDetails")]
        public List<InvoiceDetailsOutput> GetInvoiceItemDetails(string invoiceid)
        {
            logger.InfoFormat("Get Invoice Item Details invoiceid={0}", invoiceid);
            List<InvoiceDetailsOutput> L1Details = null;

            try
            {
                L1Details = (from invoicedetails in dcap.InvoiceDetails
                             join l1 in dcap.L1 on invoicedetails.L1Id equals l1.L1id
                             join l2 in dcap.L2 on new { A = invoicedetails.L1Id, B = invoicedetails.L2Id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4 on new { A = invoicedetails.L1Id, B = invoicedetails.L2Id, C = invoicedetails.L3Id, D = invoicedetails.L4Id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             where invoicedetails.InvoiceNo == invoiceid
                             orderby l1.L1id, l2.L2id, l4.L4id
                             select new InvoiceDetailsOutput
                             {
                                 Seq = invoicedetails.Seq,
                                 Style = l1.L1desc,
                                 Shedule = l2.L2desc,
                                 PO = l2.Ref01,
                                 Color = l4.L4desc,
                                 ControlId = invoicedetails.ControlId,
                                 BarcodeNo = invoicedetails.BarcodeNo,
                                 Qty01 = invoicedetails.Qty01,
                                 UnitPrice = l4.UnitPrice,
                                 Price = invoicedetails.Price,
                                 Remark = invoicedetails.Remark
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }
        //Invoice API: END

        //chckers
        public Boolean CheckForBarcodeReuseinOperation(string barcode, uint l1id, uint l2id, uint l3id, uint l4id, uint l5id, int opcode, uint DetxnKey, int EnteredQtyRw, int EnteredQtyScrap)
        {

            logger.InfoFormat("Check For Barcode Reuse in Operation barcode={0}, l1id={1}, l2id={2}, l3id={3}, l4id={4}, l5id={5}, opcode={6}, DetxnKey={7}", barcode, l1id, l2id, l3id, l4id, l5id, opcode, DetxnKey);
            Boolean checker = true;
            List<Detxn> L1Details = new List<Detxn>();

            try
            {
                if (DetxnKey == 0)
                {
                    if (EnteredQtyRw <= 0 && EnteredQtyScrap <= 0)
                    {
                        //skip
                    }
                    else
                    {
                        var L2Details = dcap.Detxn.Where(l => l.BarCodeNo == barcode && l.L1id == l1id && l.L2id == l2id && l.L3id == l3id && l.L4id == l4id && l.L5id == l5id && l.OperationCode == opcode)
                        .GroupBy(r => new { r.BarCodeNo, r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.BagBarCodeNo, r.TravelBarCodeNo, r.OperationCode }).Select(c => new { BagBarCodeNo = c.Key.BagBarCodeNo, TravelBarCodeNo = c.Key.TravelBarCodeNo, Qty02 = (int)c.Sum(g => g.Qty02), Qty03 = (int)c.Sum(g => g.Qty03) }).ToList();

                        if (L2Details.Count != 0)
                        {
                            foreach (var qi in L2Details)
                            {
                                if (qi.TravelBarCodeNo == qi.BagBarCodeNo)
                                {
                                    if (qi.Qty02 > 0 || qi.Qty03 > 0)
                                    {
                                        checker = false;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (EnteredQtyRw <= 0 && EnteredQtyScrap <= 0)
                    {
                        //skip
                    }
                    else
                    {
                        var L2Details = dcap.Detxn.Where(l => l.DetxnKey == DetxnKey && l.BarCodeNo == barcode && l.L1id == l1id && l.L2id == l2id && l.L3id == l3id && l.L4id == l4id && l.L5id == l5id && l.OperationCode == opcode)
                        .GroupBy(r => new { r.BarCodeNo, r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.OperationCode }).Select(c => new { Qty02 = (int)c.Sum(g => g.Qty02), Qty03 = (int)c.Sum(g => g.Qty03) }).ToList();

                        if (L1Details.Count != 0)
                        {
                            foreach (Detxn qi in L1Details)
                            {
                                if (qi.Qty02 > 0 || qi.Qty03 > 0)
                                {
                                    checker = false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return checker;
        }

        //Barcode Checker API's :START
        [Produces("application/json")]
        [HttpGet("GetBagTagDetailsbyGarmentBarcode")]
        public BagBarcodeTransactions GetBagTagDetailsbyGarmentBarcode(string barcode, int OperationCode)
        {
            logger.InfoFormat("Get Bag Tag Details by Garment Barcode barcode={0}", barcode);
            BagBarcodeTransactions L1Details = null;

            try
            {
                Detxn detxn = dcap.Detxn.Where(det => det.BarCodeNo == barcode && det.BagBarCodeNo != "" && det.OperationCode == OperationCode).OrderByDescending(det => det.TxnDateTime).FirstOrDefault();

                if (detxn != null)
                {
                    L1Details = (from groupbarcode in dcap.GroupBarcode
                                 join l1 in dcap.L1 on groupbarcode.L1id equals l1.L1id
                                 join l2 in dcap.L2 on new { A = groupbarcode.L1id, B = groupbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                 join l4 in dcap.L4 on new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                                 where groupbarcode.L1id == detxn.L1id && groupbarcode.L2id == detxn.L2id && groupbarcode.L3id == detxn.L3id && groupbarcode.L4id == detxn.L4id && groupbarcode.L5id == 0 && groupbarcode.BagBarCodeNo == detxn.BagBarCodeNo && groupbarcode.TxnMode == 1
                                 orderby l1.L1id, l2.L2id, l4.L4id
                                 select new BagBarcodeTransactions
                                 {
                                     WfdepinstId = groupbarcode.WFDEPInstId,
                                     WFIdBag = groupbarcode.WFId,
                                     BagBarCodeNo = groupbarcode.BagBarCodeNo,
                                     Style = l1.L1desc,
                                     Shedule = l2.L2desc,
                                     Color = l1.L1desc,

                                     L1idBag = groupbarcode.L1id,
                                     L2idBag = groupbarcode.L2id,
                                     L3idBag = groupbarcode.L3id,
                                     L4idBag = groupbarcode.L4id,
                                     L5idBag = groupbarcode.L5id,

                                     TxnMode = groupbarcode.TxnMode,

                                     Qty01 = groupbarcode.Qty01,
                                     Qty02 = groupbarcode.Qty02,
                                     Qty03 = groupbarcode.Qty03,

                                     Qty01NS = groupbarcode.Qty01NS,
                                     Qty02NS = groupbarcode.Qty02NS,
                                     Qty03NS = groupbarcode.Qty03NS,

                                     BagStatus = groupbarcode.TxnStatus,

                                     CreatedDateTime = groupbarcode.CreatedDateTime
                                 }).FirstOrDefault();

                    if (L1Details != null)
                    {
                        GoodControl goodcontrol = dcap.GoodControl.Where(g => g.L1id == L1Details.L1idBag && g.L2id == L1Details.L2idBag && g.L3id == L1Details.L3idBag && g.L4id == L1Details.L4idBag && g.L5id == L1Details.L5idBag && g.BarCodeNo == L1Details.BagBarCodeNo && g.TxnMode == L1Details.TxnMode).FirstOrDefault();
                        if (goodcontrol != null)
                        {
                            L1Details.LocationCode = goodcontrol.WarLocCode;
                            L1Details.ControlId = goodcontrol.ControlId;
                            L1Details.ControlCreatedBy = goodcontrol.CreatedBy;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelTagDetailsbyGarmentBarcode")]
        public TravelBarcodeDetails GetTravelTagDetailsbyGarmentBarcode(string barcode, int OperationCode)
        {
            logger.InfoFormat("Get Bag Tag Details by Garment Barcode barcode={0}", barcode);
            TravelBarcodeDetails travelBarcodeDetails = null;

            try
            {
                Detxn detxn = null;
                if (OperationCode == 0)
                {
                    detxn = dcap.Detxn.Where(det => det.BarCodeNo == barcode && det.BagBarCodeNo != "" && det.TravelBarCodeNo != "").OrderByDescending(det => det.TxnDateTime).FirstOrDefault();
                }
                else
                {
                    detxn = dcap.Detxn.Where(det => det.BarCodeNo == barcode && det.BagBarCodeNo != "" && det.TravelBarCodeNo != "" && det.OperationCode == OperationCode).OrderByDescending(det => det.TxnDateTime).FirstOrDefault();
                }

                if (detxn != null)
                {
                    travelBarcodeDetails = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == detxn.TravelBarCodeNo).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return travelBarcodeDetails;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelBarcodeDetailsbyTravelTag")]
        public List<TeamCounterCM> GetTravelBarcodeDetailsbyTravelTag(string travelcode)
        {

            logger.InfoFormat("Get Factory Name travelcode={0}", travelcode);
            List<TeamCounterCM> L1Details = new List<TeamCounterCM>();

            try
            {
                var PL1Details = (from groupbarcode in dcap.GroupBarcode
                                  join l1 in dcap.L1 on groupbarcode.L1id equals l1.L1id
                                  join l2 in dcap.L2 on new { A = groupbarcode.L1id, B = groupbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                                  join l4 in dcap.L4 on new { P = groupbarcode.L1id, Q = groupbarcode.L2id, R = groupbarcode.L3id, S = groupbarcode.L4id } equals new { P = l4.L1id, Q = l4.L2id, R = l4.L3id, S = l4.L4id }
                                  where groupbarcode.BagBarCodeNo == travelcode && groupbarcode.TxnMode == 2
                                  orderby groupbarcode.ModifiedDateTime
                                  select new TeamCounterCM
                                  {
                                      Seq = groupbarcode.Seq,
                                      WFIdBag = groupbarcode.WFId,
                                      BagBarCode = groupbarcode.BagBarCodeNo,
                                      L1idBag = groupbarcode.L1id,
                                      L2idBag = groupbarcode.L2id,
                                      L3idBag = groupbarcode.L3id,
                                      L4idBag = groupbarcode.L4id,
                                      L5idBag = groupbarcode.L5id,

                                      TxnMode = groupbarcode.TxnMode,
                                      StyleId = groupbarcode.L1id,
                                      StyleDesc = l1.L1desc,
                                      ScheduleId = groupbarcode.L2id,
                                      ScheduleDesc = l2.L2desc,
                                      DeliveryDate = l2.DeliveryDate,
                                      PONo = l2.Ref01,
                                      Zfeature = l2.Ref02,
                                      ColorId = l4.L4id,
                                      ColorDesc = l4.L4desc,
                                      Qty01 = (int)groupbarcode.Qty01,
                                      Qty02 = (int)groupbarcode.Qty02,
                                      Qty03 = (int)groupbarcode.Qty03,
                                      Qty01Ns = (int)groupbarcode.Qty01NS,
                                      Qty02Ns = (int)groupbarcode.Qty01NS,
                                      Qty03Ns = (int)groupbarcode.Qty01NS,
                                      TxnStatus = groupbarcode.TxnStatus,

                                  }).ToList();

                foreach (TeamCounterCM gc in PL1Details)
                {
                    GoodControl goodcontrol = dcap.GoodControl.Where(g => g.L1id == gc.L1idBag && g.L2id == gc.L2idBag && g.L3id == gc.L3idBag && g.L4id == gc.L4idBag && g.L5id == gc.L5idBag && g.BarCodeNo == gc.BagBarCode && g.TxnMode == gc.TxnMode).FirstOrDefault();
                    if (goodcontrol != null)
                    {
                        gc.Location = goodcontrol.WarLocCode;
                        gc.ControlId = goodcontrol.ControlId;
                        gc.ControlCeatedBy = goodcontrol.CreatedBy;
                    }
                    L1Details.Add(gc);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetOperationDetailsbyGarmentBarcode")]
        public List<BarcodeStatus> GetOperationDetailsbyGarmentBarcode(string barcode)
        {

            logger.InfoFormat("Get Operation Details by Garment Barcode barcode={0}", barcode);
            List<BarcodeStatus> Details = new List<BarcodeStatus>();

            try
            {
                Details = (from detxn in dcap.Detxn.Where(detxn => detxn.BarCodeNo == barcode && detxn.RecStatus == 1).AsQueryable()
                           join dep in dcap.Dep on detxn.Depid equals dep.Depid
                           orderby dep.OperationCode ascending
                           select new BarcodeStatus
                           {
                               Transaction_Date_and_Time = detxn.TxnDateTime,
                               //Operation = grp.Key.OperationCode,
                               Department = dep.Depdesc,
                               Manufacturing_Qty = detxn.Qty01,//grp.Sum(detxn => detxn.Qty01), //not working
                               Qty_Report = detxn.Qty02,//grp.Sum(detxn => detxn.Qty02),
                               Qty_Scrap = detxn.Qty03,//grp.Sum(detxn => detxn.Qty03),
                           }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return Details;
        }

        [Produces("application/json")]
        [HttpGet("GetBagTagDetailsbyBagBarcode")]
        public BagBarcodeTransactions GetBagTagDetailsbyBagBarcode(string barcode)
        {
            logger.InfoFormat("Get Bag Tag Details by Garment Barcode barcode={0}", barcode);
            BagBarcodeTransactions L1Details = null;

            try
            {
                L1Details = (from groupbarcode in dcap.GroupBarcode
                             join l1 in dcap.L1 on groupbarcode.L1id equals l1.L1id
                             join l2 in dcap.L2 on new { A = groupbarcode.L1id, B = groupbarcode.L2id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4 on new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             where groupbarcode.BagBarCodeNo == barcode && groupbarcode.TxnMode == 1
                             orderby l1.L1id, l2.L2id, l4.L4id
                             select new BagBarcodeTransactions
                             {
                                 WfdepinstId = groupbarcode.WFDEPInstId,
                                 WFIdBag = groupbarcode.WFId,
                                 BagBarCodeNo = groupbarcode.BagBarCodeNo,
                                 Style = l1.L1desc,
                                 Shedule = l2.L2desc,
                                 Color = l1.L1desc,

                                 L1idBag = groupbarcode.L1id,
                                 L2idBag = groupbarcode.L2id,
                                 L3idBag = groupbarcode.L3id,
                                 L4idBag = groupbarcode.L4id,
                                 L5idBag = groupbarcode.L5id,

                                 TxnMode = groupbarcode.TxnMode,

                                 Qty01 = groupbarcode.Qty01,
                                 Qty02 = groupbarcode.Qty02,
                                 Qty03 = groupbarcode.Qty03,

                                 Qty01NS = groupbarcode.Qty01NS,
                                 Qty02NS = groupbarcode.Qty02NS,
                                 Qty03NS = groupbarcode.Qty03NS,

                                 BagStatus = groupbarcode.TxnStatus,

                                 CreatedDateTime = groupbarcode.CreatedDateTime
                             }).FirstOrDefault();

                if (L1Details != null)
                {
                    GoodControl goodcontrol = dcap.GoodControl.Where(g => g.L1id == L1Details.L1idBag && g.L2id == L1Details.L2idBag && g.L3id == L1Details.L3idBag && g.L4id == L1Details.L4idBag && g.L5id == L1Details.L5idBag && g.BarCodeNo == L1Details.BagBarCodeNo && g.TxnMode == L1Details.TxnMode).FirstOrDefault();
                    if (goodcontrol != null)
                    {
                        L1Details.LocationCode = goodcontrol.WarLocCode;
                        L1Details.ControlId = goodcontrol.ControlId;
                        L1Details.ControlCreatedBy = goodcontrol.CreatedBy;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return L1Details;
        }

        [Produces("application/json")]
        [HttpGet("GetTravelTagDetailsbyTravelBarcode")]
        public TravelBarcodeDetails GetTravelTagDetailsbyTravelBarcode(string barcode)
        {
            logger.InfoFormat("Get Bag Tag Details by Garment Barcode barcode={0}", barcode);
            TravelBarcodeDetails travelBarcodeDetails = null;

            try
            {
                travelBarcodeDetails = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == barcode).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return travelBarcodeDetails;
        }

        [Produces("application/json")]
        [HttpGet("GetAllBarcodeDetailsbyBarcode")]
        public IList<Detxn> GetAllBarcodeDetailsbyBarcode(string barcode, int txnmode)
        {
            logger.InfoFormat("Get All Barcode Details by Barcode barcode={0} txnmode={1}", barcode, txnmode);
            List<Detxn> detxn = new List<Detxn>();

            try
            {
                List<GroupBarcode> groupbarcode = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == barcode && c.TxnMode == txnmode).ToList();

                if (groupbarcode.Count != 0)
                {
                    foreach (GroupBarcode g in groupbarcode)
                    {
                        IList<Detxn> det = dcap.Detxn.Where(d => d.OperationCode == 151 && d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && (txnmode == 1 ? d.BagBarCodeNo == barcode : d.TravelBarCodeNo == barcode)).ToList();
                        detxn = detxn.Concat(det).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return detxn;
        }

        [Produces("application/json")]
        [HttpGet("GetFirstGarmentBarcodeDetailsbyBarcode")]
        public List<Detxn> GetFirstGarmentBarcodeDetailsbyBarcode(string barcode, int txnmode)
        {
            logger.InfoFormat("Get All Barcode Details by Barcode barcode={0} txnmode={1}", barcode, txnmode);
            List<Detxn> detxn = new List<Detxn>();

            try
            {
                List<GroupBarcode> group = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == barcode && c.TxnMode == txnmode).ToList();

                if (group.Count != 0)
                {
                    foreach (GroupBarcode g in group)
                    {
                        List<Detxn> det = (from d in dcap.Detxn.Where(d => d.OperationCode == 151 && d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && (txnmode == 1 ? d.BagBarCodeNo == barcode : d.TravelBarCodeNo == barcode)).AsQueryable()
                                           group d by new { d.BarCodeNo } into grp
                                           select new Detxn
                                           {
                                               BarCodeNo = grp.Key.BarCodeNo
                                           }).ToList();
                        detxn = detxn.Concat(det).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return detxn;
        }

        [Produces("application/json")]
        [HttpGet("GetFirstTravelTagBarcodeDetailsbyBarcode")]
        public List<Detxn> GetFirstTravelTagBarcodeDetailsbyBarcode(string barcode, int txnmode)
        {
            logger.InfoFormat("Get All Barcode Details by Barcode barcode={0} txnmode={1}", barcode, txnmode);
            List<Detxn> detxn = new List<Detxn>();

            try
            {
                List<GroupBarcode> group = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == barcode && c.TxnMode == txnmode).ToList();

                if (group.Count != 0)
                {
                    foreach (GroupBarcode g in group)
                    {
                        List<Detxn> det = (from d in dcap.Detxn.Where(d => d.OperationCode == 151 && d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && (txnmode == 1 ? d.BagBarCodeNo == barcode : d.TravelBarCodeNo == barcode)).AsQueryable()
                                           group d by new { d.TravelBarCodeNo } into grp
                                           select new Detxn
                                           {
                                               TravelBarCodeNo = grp.Key.TravelBarCodeNo
                                           }).ToList();

                        detxn = detxn.Concat(det).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return detxn;
        }

        [Produces("application/json")]
        [HttpGet("GetFirstBagTagBarcodeDetailsbyBarcode")]
        public List<Detxn> GetFirstBagTagBarcodeDetailsbyBarcode(string barcode, int txnmode)
        {
            logger.InfoFormat("Get All Barcode Details by Barcode barcode={0} txnmode={1}", barcode, txnmode);
            List<Detxn> detxn = new List<Detxn>();

            try
            {
                List<GroupBarcode> group = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == barcode && c.TxnMode == txnmode).ToList();


                if (group.Count != 0)
                {
                    foreach (GroupBarcode g in group)
                    {
                        List<Detxn> det = (from d in dcap.Detxn.Where(d => d.OperationCode == 151 && d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && (txnmode == 1 ? d.BagBarCodeNo == barcode : d.TravelBarCodeNo == barcode)).AsQueryable()
                                           group d by new { d.BagBarCodeNo } into grp
                                           select new Detxn
                                           {
                                               BagBarCodeNo = grp.Key.BagBarCodeNo
                                           }).ToList();

                        detxn = detxn.Concat(det).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return detxn;
        }
        //Barcode Checker API's :END

        //Picking List API's
        [Produces("application/json")]
        [HttpGet("GetAllShedulesofUnaloocatedGroupBarcodes")]
        public IList<TeamCounterCM> GetAllShedulesofUnaloocatedGroupBarcodes(string faccode)
        {
            logger.InfoFormat("Get All Shedules of Unaloocated Group Barcodes faccode={0}", faccode);
            List<TeamCounterCM> output = new List<TeamCounterCM>();

            try
            {
                IList<GoodControl> goodControls = (from gdc in dcap.GoodControl.Where(c => c.Return == 0).AsQueryable()
                                                   join gcdc in dcap.GoodControlDetails.Where(c => c.LocCode == faccode).AsQueryable() on new { A = gdc.ControlId, B = gdc.ControlType } equals new { A = gcdc.ControlId, B = gcdc.ControlType }
                                                   select new GoodControl
                                                   {
                                                       L1id = gdc.L1id,
                                                       L2id = gdc.L2id,
                                                       BarCodeNo = gdc.BarCodeNo,
                                                       TxnMode = gdc.TxnMode,
                                                       TxnStatus = gdc.TxnStatus,
                                                       RecStatus = gdc.RecStatus,
                                                       Return = gdc.Return,
                                                   }).ToList();

                output = (from goodcontrol in goodControls
                          join l2 in dcap.L2 on new { A = goodcontrol.L1id, B = goodcontrol.L2id } equals new { A = l2.L1id, B = l2.L2id }
                          join groupbrcode in dcap.GroupBarcode on new { A = goodcontrol.BarCodeNo, B = goodcontrol.TxnMode } equals new { A = groupbrcode.BagBarCodeNo, B = groupbrcode.TxnMode }
                          join wf in dcap.Wf on groupbrcode.WFId equals wf.Wfid
                          where goodcontrol.TxnStatus == 5 && groupbrcode.Qty01 != groupbrcode.Qty01NS && goodcontrol.RecStatus == 1 && goodcontrol.Return == 0
                          group goodcontrol by new
                          {
                              wf.Wfid,
                              wf.Wfdesc,
                              goodcontrol.L1id,
                              goodcontrol.L2id,
                              l2.L2desc,
                              //goodcontrol.L4id,
                          }
                        into grp
                          select new TeamCounterCM
                          {
                              WFIdBag = grp.Key.Wfid,
                              WFIdDesc = grp.Key.Wfdesc,
                              L1idBag = grp.Key.L1id,
                              L2idBag = grp.Key.L2id,
                              ScheduleDesc = grp.Key.L2desc,
                          }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        [Produces("application/json")]
        [HttpGet("GetPickListByShedule")]
        public List<TeamCounterCM> GetPickListByShedule(int styleid, int sheduleid)
        {

            logger.InfoFormat("Get Factory Name styleid={0}, sheduleid={1}", styleid, sheduleid);
            List<TeamCounterCM> L1Details = new List<TeamCounterCM>();

            try
            {
                L1Details = (from goodcontrol in dcap.GoodControl.Where(g => g.WarLocCode != null && g.TxnStatus == 5 && g.L1id == styleid && g.L2id == sheduleid && g.Return == 0)
                             join groupbarcode in dcap.GroupBarcode.Where(g => g.Qty01NS != g.Qty01 && g.TxnStatus == 5 && g.L1id == styleid && g.L2id == sheduleid) on new { A = goodcontrol.L1id, B = goodcontrol.L2id, C = goodcontrol.L3id, D = goodcontrol.L4id, E = goodcontrol.BarCodeNo, F = goodcontrol.TxnMode } equals new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id, E = groupbarcode.BagBarCodeNo, F = groupbarcode.TxnMode }
                             join l4 in dcap.L4.Where(g => g.L1id == styleid && g.L2id == sheduleid) on new { A = groupbarcode.L1id, B = groupbarcode.L2id, C = groupbarcode.L3id, D = groupbarcode.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             where goodcontrol.RecStatus == 1 && groupbarcode.Qty01 != groupbarcode.Qty01NS
                             group new { goodcontrol, groupbarcode } by new
                             {
                                 goodcontrol.WarLocCode,
                                 l4.L4id,
                                 l4.L4desc,
                                 groupbarcode.BagBarCodeNo,
                             } into grp
                             select new TeamCounterCM
                             {
                                 WarLocCode = grp.Key.WarLocCode,
                                 ColorDesc = grp.Key.L4desc,
                                 BagBarCode = grp.Key.BagBarCodeNo,
                                 Qty01 = ((grp.Sum(c => c.groupbarcode.Qty01) - grp.Sum(c => c.groupbarcode.Qty02) - grp.Sum(c => c.groupbarcode.Qty03)) - (grp.Sum(c => c.groupbarcode.Qty01NS) - grp.Sum(c => c.groupbarcode.Qty02NS) - grp.Sum(c => c.groupbarcode.Qty03NS)))
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        //Non Apperal Bag Barcode Generator
        public GroupBarcode GetBagBarcodeDetailByBagBarcode(string barcode, int txnmode)
        {

            logger.InfoFormat("Get Factory Name barcode={0}, txnmode={1} txnstatus={2}", barcode, txnmode);
            GroupBarcode L1Details = new GroupBarcode();

            try
            {
                L1Details = (from groupbarcode in dcap.GroupBarcode.Where(g => g.BagBarCodeNo == barcode && g.TxnMode == txnmode)
                             group new { groupbarcode } by new
                             {
                                 groupbarcode.BagBarCodeNo,
                             } into grp
                             select new GroupBarcode
                             {
                                 TxnStatus = grp.Max(c => c.groupbarcode.TxnStatus),
                                 Qty01 = grp.Sum(c => c.groupbarcode.Qty01),
                                 Qty02 = grp.Sum(c => c.groupbarcode.Qty02),
                                 Qty03 = grp.Sum(c => c.groupbarcode.Qty03),
                             }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public TeamCounter GetTeamCounterDetailByBagBarcode(string barcode)
        {

            logger.InfoFormat("Get Factory Name barcode={0}", barcode);
            TeamCounter L1Details = new TeamCounter();

            try
            {
                L1Details = (from teamcounter in dcap.TeamCounter.Where(g => g.BagBarCodeNo == barcode)
                             group new { teamcounter } by new
                             {
                                 teamcounter.BagBarCodeNo,
                             } into grp
                             select new TeamCounter
                             {
                                 Qty01 = grp.Sum(c => c.teamcounter.Qty01),
                                 Qty02 = grp.Sum(c => c.teamcounter.Qty02),
                                 Qty03 = grp.Sum(c => c.teamcounter.Qty03),
                             }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public GroupBarcode GetQtyByGroupBarcodeIds(uint WFDEPInstId, uint WFId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id, int GetGroupBarcodeDataFor, int OpCode)
        {
            logger.InfoFormat("GetQtyByDEPInstId API called with GetGroupBarcodeDataFor={0}, WFDEPInstId={1}, WFId={2}, L1Id={3}, L2Id={4} , L3Id={5} , L4Id={6} , L5Id={7}, OpCode={8}", GetGroupBarcodeDataFor, WFDEPInstId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id, OpCode);
            GroupBarcode GroupBarcode = null;

            try
            {
                if (GetGroupBarcodeDataFor == 1) //Color
                {
                    GroupBarcode = (from d in dcap.GroupBarcode
                                    where d.WFDEPInstId == WFDEPInstId && d.WFId == WFId && d.L1id == L1Id && d.OperationCode == OpCode
                                    group d by new { d.WFDEPInstId, d.WFId, d.L1id }
                                               into grp
                                    select new GroupBarcode
                                    {
                                        Qty01 = grp.Sum(d => d.Qty01),
                                        Qty02 = grp.Sum(d => d.Qty02),
                                        Qty03 = grp.Sum(d => d.Qty03)
                                    }).FirstOrDefault();
                }
                if (GetGroupBarcodeDataFor == 2) //Size
                {
                    GroupBarcode = (from d in dcap.GroupBarcode
                                    where d.WFDEPInstId == WFDEPInstId && d.WFId == WFId && d.L1id == L1Id
                                      && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.OperationCode == OpCode// 
                                    group d by new { d.WFDEPInstId, d.WFId, d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                                  into grp
                                    select new GroupBarcode
                                    {
                                        Qty01 = grp.Sum(d => d.Qty01),
                                        Qty02 = grp.Sum(d => d.Qty02),
                                        Qty03 = grp.Sum(d => d.Qty03)
                                    }).FirstOrDefault();
                }
                if (GetGroupBarcodeDataFor == 3) //Size for All
                {
                    GroupBarcode = (from d in dcap.GroupBarcode
                                    where d.L1id == L1Id
                                      && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id && d.OperationCode == OpCode// 
                                    group d by new { d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                                  into grp
                                    select new GroupBarcode
                                    {
                                        Qty01 = grp.Sum(d => d.Qty01),
                                        Qty02 = grp.Sum(d => d.Qty02),
                                        Qty03 = grp.Sum(d => d.Qty03)
                                    }).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return GroupBarcode;
        }

        public TeamCounter GetQtyByTeamCounterIds(uint WFDEPInstId, uint WFId, uint L1Id, uint L2Id, uint L3Id, uint L4Id, uint L5Id, int GetGroupBarcodeDataFor)
        {
            logger.InfoFormat("GetQtyByDEPInstId API called with GetQtyByTeamCounterIds={0}, WFDEPInstId={1}, WFId={2}, L1Id={3}, L2Id={4} , L3Id={5} , L4Id={6} , L5Id={7}", GetGroupBarcodeDataFor, WFDEPInstId, WFId, L1Id, L2Id, L3Id, L4Id, L5Id);
            TeamCounter TeamCounter = null;

            try
            {
                if (GetGroupBarcodeDataFor == 1) //Color
                {
                    TeamCounter = (from d in dcap.TeamCounter
                                   where d.WfdepinstId == WFDEPInstId && d.L1id == L1Id
                                   group d by new { d.WfdepinstId, d.L1id }
                                               into grp
                                   select new TeamCounter
                                   {
                                       Qty01 = grp.Sum(d => d.Qty01),
                                       Qty02 = grp.Sum(d => d.Qty02),
                                       Qty03 = grp.Sum(d => d.Qty03)
                                   }).FirstOrDefault();
                }
                if (GetGroupBarcodeDataFor == 2) //Size
                {
                    TeamCounter = (from d in dcap.TeamCounter
                                   where d.WfdepinstId == WFDEPInstId && d.L1id == L1Id
                                     && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id// 
                                   group d by new { d.WfdepinstId, d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                                  into grp
                                   select new TeamCounter
                                   {
                                       Qty01 = grp.Sum(d => d.Qty01),
                                       Qty02 = grp.Sum(d => d.Qty02),
                                       Qty03 = grp.Sum(d => d.Qty03)
                                   }).FirstOrDefault();
                }
                if (GetGroupBarcodeDataFor == 3) //Size in ALL
                {
                    TeamCounter = (from d in dcap.TeamCounter
                                   where d.L1id == L1Id
                                     && d.L2id == L2Id && d.L3id == L3Id && d.L4id == L4Id && d.L5id == L5Id// 
                                   group d by new { d.L1id, d.L2id, d.L3id, d.L4id, d.L5id }
                                  into grp
                                   select new TeamCounter
                                   {
                                       Qty01 = grp.Sum(d => d.Qty01),
                                       Qty02 = grp.Sum(d => d.Qty02),
                                       Qty03 = grp.Sum(d => d.Qty03)
                                   }).FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return TeamCounter;
        }

        //Check that the barcode is ever used before
        //Used API's and UI : CreateGroup
        [Produces("application/json")]
        [HttpGet("CheckForBagBarcodeExistence")]
        public int CheckForBagBarcodeExistence(string bagbarcode, int mode, int l1id, int l2id, int l4id, int l5id)
        {
            logger.InfoFormat("CheckForBagBarcodeExistence bagbarcode={0} mode={1}, l1id={2}, l2id={3}, l4id={4}, l5id={5}", bagbarcode, mode, l1id, l2id, l4id, l5id);
            int nobarcodes = 1;
            try
            {
                if (mode == 2)
                {
                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode && d.L1id == l1id && d.L2id == l2id && d.L4id == l4id && d.L5id == l5id
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = 1;
                    }
                    else
                    {
                        nobarcodes = 0;
                    }

                }
                else if (mode == 1)
                {
                    var ExsistCounterIds = (from d in dcap.TeamCounter
                                            where d.BagBarCodeNo == bagbarcode && d.L1id == l1id && d.L2id == l2id && d.L4id == l4id && d.L5id == l5id
                                            select new
                                            {
                                                bagbarcode = d.BagBarCodeNo,
                                            }).ToList();

                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode && d.L1id == l1id && d.L2id == l2id && d.L4id == l4id && d.L5id == l5id
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistCounterIds.Count == 0 && ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = 1;
                    }
                    else if (ExsistCounterIds.Count != 0)
                    {
                        nobarcodes = 2;
                    }
                    else if (ExsistInventoryIds.Count != 0)
                    {
                        nobarcodes = 3;
                    }
                    else
                    {
                        nobarcodes = 0;
                    }
                }
                else if (mode == 3)
                {
                    var ExsistInventoryIds = (from d in dcap.GroupBarcode
                                              where d.BagBarCodeNo == bagbarcode && d.TxnStatus > 0 && d.L1id == l1id && d.L2id == l2id && d.L4id == l4id && d.L5id == l5id
                                              select new
                                              {
                                                  bagbarcode = d.BagBarCodeNo,
                                              }).ToList();

                    if (ExsistInventoryIds.Count == 0)
                    {
                        nobarcodes = 1;
                    }
                    else
                    {
                        nobarcodes = 0;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return nobarcodes;
        }

        public Wf GetWorkFlowByWfId(int WFid)
        {
            logger.InfoFormat("CheckForBagBarcodeExistence WFid={0}", WFid);
            Wf wf = null;
            try
            {
                wf = dcap.Wf.Where(c => c.Wfid == WFid).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving CheckForBagBarcodeExistence information {0}", e.ToString());
                throw e;
            }
            return wf;
        }

        //Used API's and UI : updateTableValues (WEB Barcode)
        [Produces("application/json")]
        [HttpGet("GetUpdatedTableValues")]
        public Dedepinst GetUpdatedTableValues(int dedepinstKey)
        {
            logger.InfoFormat("Get Updated Table Values API called with dedepinstKey = {0}", dedepinstKey);
            Dedepinst lstTeamCounter = null;

            try
            {
                lstTeamCounter = dcap.Dedepinst.Where(c => c.dedepinstKey == dedepinstKey).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while GetPOcounterValues {0}", e.ToString());
                throw e;
            }
            return lstTeamCounter;
        }

        //Used API's and UI : updateTableValues (WEB Barcode)
        [Produces("application/json")]
        [HttpGet("GetGroupTagDetailsByGroupBarcode")]
        public IList<StyleScheduleColor> GetGroupTagDetailsByGroupBarcode(string groupTagNo)
        {
            logger.InfoFormat("Get Group Barcode Details By Group Tag No called with Group Tag No={0}", groupTagNo);
            IList<StyleScheduleColor> GroupTags = null;

            try
            {
                GroupTags = (from gb in dcap.GroupBarcode.Where(g => g.BagBarCodeNo.Contains(groupTagNo) && g.RecStatus == (int)eRecStatus.Active).AsQueryable()
                             join l1 in dcap.L1 on new { A = gb.L1id } equals new { A = l1.L1id }
                             join l2 in dcap.L2 on new { A = gb.L1id, B = gb.L2id } equals new { A = l2.L1id, B = l2.L2id }
                             join l4 in dcap.L4 on new { A = gb.L1id, B = gb.L2id, C = gb.L3id, D = gb.L4id } equals new { A = l4.L1id, B = l4.L2id, C = l4.L3id, D = l4.L4id }
                             group gb by new
                             {
                                 l1.L1id,
                                 l1.L1no,
                                 l2.L2id,
                                 l2.L2no,
                                 l4.L4id,
                                 l4.L4no,
                                 l4.L4desc,
                                 gb.BagBarCodeNo,
                                 gb.TxnMode
                             } into grp
                             select new StyleScheduleColor
                             {
                                 StyleId = grp.Key.L1id,
                                 StyleNo = grp.Key.L1no,
                                 ScheduleId = grp.Key.L2id,
                                 ScheduleNo = grp.Key.L2no,
                                 ColorId = grp.Key.L4id,
                                 ColorNo = grp.Key.L4desc,
                                 BagBarcode = grp.Key.BagBarCodeNo,
                                 TxnMode = grp.Key.TxnMode,
                                 Qty01 = grp.Sum(c => c.Qty01),
                                 Qty02 = grp.Sum(c => c.Qty02),
                                 Qty03 = grp.Sum(c => c.Qty03)
                             }).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Group Barcode information {0}", e.ToString());
                throw e;
            }
            return GroupTags;
        }

        //Get 
        [Produces("application/json")]
        [HttpGet("GetGatePassDetailsByGroupTag")]
        public IList<GoodControlDetails> GetGatePassDetailsByGroupTag(string groupTagNo, string gatePassNo)
        {
            logger.InfoFormat("Get Group Barcode Details By Group Tag No called with Group Tag No={0}, gatePassNo={1}", groupTagNo, gatePassNo);
            IList<GoodControlDetails> GatePasses = new List<GoodControlDetails>();

            try
            {
                List<GoodControl> ControlIds = dcap.GoodControl.Where(c => c.BarCodeNo == groupTagNo).ToList();

                if (ControlIds.Count != 0)
                {
                    foreach (GoodControl q in ControlIds)
                    {
                        if (gatePassNo != "" && gatePassNo != null)
                        {
                            if (q.ControlId.Contains(groupTagNo))
                            {
                                GoodControlDetails PGatePasses = dcap.GoodControlDetails.Where(c => c.ControlId == q.ControlId).FirstOrDefault();
                                if (PGatePasses != null)
                                {
                                    GatePasses.Add(PGatePasses);
                                }
                            }
                        }
                        else
                        {
                            GoodControlDetails PGatePasses = dcap.GoodControlDetails.Where(c => c.ControlId == q.ControlId).FirstOrDefault();
                            if (PGatePasses != null)
                            {
                                GatePasses.Add(PGatePasses);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Group Barcode information {0}", e.ToString());
                throw e;
            }
            return GatePasses;
        }

        [Produces("application/json")]
        [HttpGet("GetGarametsBarcodeByGroupTag")]
        public List<BarcodeDetailAsync> GetGarametsBarcodeByGroupTag(string barcode, int txnmode)
        {
            logger.InfoFormat("Get All Barcode Details by group tag barcode={0} txnmode={1}", barcode, txnmode);
            List<BarcodeDetailAsync> output = new List<BarcodeDetailAsync>();

            try
            {
                List<GroupBarcode> group = dcap.GroupBarcode.Where(c => c.BagBarCodeNo == barcode && c.TxnMode == txnmode).ToList();

                if (group.Count != 0)
                {
                    foreach (GroupBarcode g in group)
                    {
                        if (g.TxnMode == 1)
                        {
                            List<BarcodeDetailAsync> det = (from detxn in dcap.Detxn.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && d.BagBarCodeNo == barcode && d.OperationCode == d.OperationCode).GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5mono, r.BarCodeNo, r.BagBarCodeNo, r.TravelBarCodeNo }).Select(c => new Detxn { BarCodeNo = c.Key.BarCodeNo, L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, L5mono = c.Key.L5mono, BagBarCodeNo = c.Key.BagBarCodeNo, TravelBarCodeNo = c.Key.TravelBarCodeNo, Qty01 = (int)c.Sum(s => s.Qty01), Qty02Ns = (int)c.Sum(s => s.Qty02Ns), Qty03Ns = (int)c.Sum(s => s.Qty03Ns) }).AsQueryable()
                                                            join l1 in dcap.L1.Where(d => d.L1id == g.L1id).AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                                            join l2 in dcap.L2.Where(d => d.L1id == g.L1id && d.L2id == g.L2id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                                            join l4 in dcap.L4.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                                            join l5 in dcap.L5.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
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
                                                                l5.L5desc,
                                                                detxn.L5mono,
                                                                detxn.BagBarCodeNo,
                                                                detxn.TravelBarCodeNo
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
                                                                SizeDesc = grp.Key.L5desc,
                                                                L5mono = grp.Key.L5mono,
                                                                BagBarCodeNo = grp.Key.BagBarCodeNo,
                                                                RefBagBarCodeNo = grp.Key.TravelBarCodeNo,
                                                                EnteredQtyGd = (int)grp.Sum(c => c.detxn.Qty01),
                                                                EnteredQtyScrap = (int)grp.Sum(c => c.detxn.Qty02Ns),
                                                                EnteredQtyRw = (int)grp.Sum(c => c.detxn.Qty03Ns),
                                                            }).ToList();

                            output = output.Concat(det).ToList();
                        }
                        else if (g.TxnMode >= 2)
                        {
                            List<BarcodeDetailAsync> det = (from detxn in dcap.Detxn.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id && d.TravelBarCodeNo == barcode && d.OperationCode == d.OperationCode).GroupBy(r => new { r.L1id, r.L2id, r.L3id, r.L4id, r.L5id, r.L5mono, r.BarCodeNo, r.BagBarCodeNo, r.TravelBarCodeNo }).Select(c => new Detxn { BarCodeNo = c.Key.BarCodeNo, L1id = c.Key.L1id, L2id = c.Key.L2id, L3id = c.Key.L3id, L4id = c.Key.L4id, L5id = c.Key.L5id, L5mono = c.Key.L5mono, BagBarCodeNo = c.Key.BagBarCodeNo, TravelBarCodeNo = c.Key.TravelBarCodeNo, Qty01 = (int)c.Sum(s => s.Qty01), Qty02Ns = (int)c.Sum(s => s.Qty02Ns), Qty03Ns = (int)c.Sum(s => s.Qty03Ns) }).AsQueryable()
                                                            join l1 in dcap.L1.Where(d => d.L1id == g.L1id).AsQueryable() on (uint?)detxn.L1id equals (uint?)l1.L1id
                                                            join l2 in dcap.L2.Where(d => d.L1id == g.L1id && d.L2id == g.L2id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id } equals new { A = (uint?)l2.L1id, B = (uint?)l2.L2id }
                                                            join l4 in dcap.L4.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id } equals new { A = (uint?)l4.L1id, B = (uint?)l4.L2id, C = (uint?)l4.L3id, D = (uint?)l4.L4id }
                                                            join l5 in dcap.L5.Where(d => d.L1id == g.L1id && d.L2id == g.L2id && d.L3id == g.L3id && d.L4id == g.L4id).AsQueryable() on new { A = (uint?)detxn.L1id, B = (uint?)detxn.L2id, C = (uint?)detxn.L3id, D = (uint?)detxn.L4id, E = (uint?)detxn.L5id } equals new { A = (uint?)l5.L1id, B = (uint?)l5.L2id, C = (uint?)l5.L3id, D = (uint?)l5.L4id, E = (uint?)l5.L5id }
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
                                                                l5.L5desc,
                                                                detxn.L5mono,
                                                                detxn.BagBarCodeNo,
                                                                detxn.TravelBarCodeNo
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
                                                                SizeDesc = grp.Key.L5desc,
                                                                L5mono = grp.Key.L5mono,
                                                                BagBarCodeNo = grp.Key.BagBarCodeNo,
                                                                RefBagBarCodeNo = grp.Key.TravelBarCodeNo,
                                                                EnteredQtyGd = (int)grp.Sum(c => c.detxn.Qty01),
                                                                EnteredQtyScrap = (int)grp.Sum(c => c.detxn.Qty02Ns),
                                                                EnteredQtyRw = (int)grp.Sum(c => c.detxn.Qty03Ns),
                                                            }).ToList();

                            output = output.Concat(det).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetGarametsBarcodeByGroupTag information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        [Produces("application/json")]
        [HttpGet("GetDepDetailsByBagCreationLogic")]
        public IList<Dep> GetDepDetailsByBagCreationLogic()
        {
            logger.InfoFormat("Get Dep Details By Bag Creation Logic called");
            IList<Dep> dep = new List<Dep>();

            try
            {
                dep = dcap.Dep.Where(c => c.CreateBag == 1).ToList();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Dep Details By Bag Creation Logic information {0}", e.ToString());
                throw e;
            }
            return dep;
        }

        public int GetBagDepIdbyWFInstId(int WFinstId)
        {
            logger.InfoFormat("Get BagDepId by WFInstId WFinstId={0}", WFinstId);
            int BagDepID = 0;

            try
            {
                var BagWf = dcap.Wfdep.Where(c => c.WfdepinstId == WFinstId).FirstOrDefault();

                if (BagWf != null)
                {
                    BagDepID = BagWf.BagDepId;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Dep Details By Bag Creation Logic information {0}", e.ToString());
                throw e;
            }
            return BagDepID;
        }
    }
}