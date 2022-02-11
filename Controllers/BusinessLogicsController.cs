/*
Description: Secuser Controller Class
Created By : NalindaW
Created on : 2018-10-04
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
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Brandix.DCAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]

    public class BusinessLogicsController : ControllerBase
    {
        #region Variable Declarations
        static ILog logger = LogManager.GetLogger(typeof(BusinessLogicsController));
        private DCAPDbContext dcap;

        #endregion

        #region Constructor
        public BusinessLogicsController(DCAPDbContext context)
        {
            dcap = context;
            //Create log4net reference          
        }
        #endregion

        #region APIs

        //Validation in Post Schedule no Enter in Bulk Data upload screen
        [Produces("application/json")]
        [HttpPost("UpdateBulkQty")]
        public List<UserInput> UpdateBulkQty([FromBody] List<UserInput> lstUserInput)
        {

            /*
                            lstUserInput[0].Responce[0] ="";
                            lstUserInput[0].Responce[0] ="";
                            lstUserInput[0].SaveSuccessfull = false;
                            return lstUserInput;
            */

            logger.InfoFormat("BusinessLogicsController-UpdateBulkQty API called with WFDEPInstId={0}", lstUserInput);

            bool ValidationPass = false;
            UserInput BulkData_Return = new UserInput();
            List<UserInput> lstBulkData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap, gu);
            LookupController Lukup = new LookupController(dcap);//, gu);
            logger.InfoFormat("BusinessLogicsController - LookupController Object created");

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();
            try
            {
                Wfdep Wfd = Lukup.GetWFConfigurationbyID(lstUserInput[0].WfdepinstId);

                foreach (UserInput ui in lstUserInput)
                {
                    //ui.guid = gu;
                    ui.Responce = new string[2];

                    if (ui.EnteredQtyGd == 0 && ui.EnteredQtyScrap == 0)
                    {
                        ui.Responce[0] = "Qty = 0, Not Processed";
                        ui.Responce[1] = "Qty = 0, Not Processed";
                        ui.SaveSuccessfull = false;
                        lstBulkData_Return.Add(ui);
                        continue;
                    }

                    // if (ui.SaveSuccessfull == true)
                    // {
                    //     ui.Responce[0] = "Not Processed";
                    //     ui.Responce[1] = "Not Processed";
                    //     ui.SaveSuccessfull = false;
                    //     lstBulkData_Return.Add(ui);
                    //     continue;
                    // }

                    ValidationPass = false;
                    ui.DCMId = Wfd.Dcmid;
                    ui.WorkCenter = Wfd.WorkCenter;
                    ui.Depid = Wfd.Depid;
                    ui.TeamId = Wfd.TeamId;
                    ui.DCMId = Wfd.Dcmid;
                    ui.DCLId = Wfd.Dclid;
                    ui.OperationCode = (int)Wfd.OperationCode;



                    int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                         currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                         prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                         prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;

                    Dedepinst objDedepinst = null;
                    Dedep objDedep = null;

                    if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                    {
                        objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                        //Get Reported Qty for Operation
                        if (objDedep != null)
                        {
                            currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                            currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                            currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                        }
                        //Get Reported Qty for Node
                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                        if (objDedepinst != null)
                        {
                            currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                            currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                            currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                        }
                        if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                            if (L5moop != null)
                            {
                                prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                if (Wfd.LimitWithWf == (int)eLimitWithWF.NA)
                                {
                                    ui.Responce[0] = "LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                    ui.Responce[1] = "LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                    ui.SaveSuccessfull = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                            {
                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPQtyGd + " (Curr Gd Qty) +" + currDEPQtyScrap + " (Curr Scrap Qty)";
                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPQtyGd + " (Curr Gd Qty) +" + currDEPQtyScrap + " (Curr Scrap Qty)";

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;

                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes)  //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                            {
                                if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    int PrevDEP = 0;
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                    if (lstTDepList != null)
                                    {
                                        if (lstTDepList.Count != 0)
                                        {
                                            objDedep = Lukup.GetQtyByDEPId((uint)lstTDepList[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                            if (objDedep != null)
                                            {
                                                prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                            }
                                        }
                                        else
                                        {
                                            ui.Responce[0] = "Invalid Configuration - This is a first node LimtWithPredecessor = Yes is invalid";
                                            ui.Responce[1] = "Invalid Configuration - This is a first node LimtWithPredecessor = Yes is invalid";
                                            ui.SaveSuccessfull = false;
                                            lstBulkData_Return.Add(ui);
                                            continue;
                                        }
                                        //Avoid posting Qty more than previous opetation
                                        if ((prevDEPQtyGd) <= (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                        {
                                            ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.EnteredQtyGd = 0;
                                            ui.EnteredQtyScrap = 0;
                                            ui.EnteredQtyRw = 0;

                                            ui.SaveSuccessfull = false;
                                            lstBulkData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                }
                                else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst)//==2  // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                    string PrevDEPInst = "";

                                    foreach (Wfdep objdep in lstTDepList)
                                    {
                                        int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                        PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                        if (objDedepinst != null)
                                        {
                                            iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                            iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                            iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                        }
                                        prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                        prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                        prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                    }
                                    if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                    {

                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.EnteredQtyGd = 0;
                                        ui.EnteredQtyScrap = 0;
                                        ui.EnteredQtyRw = 0;
                                        lstBulkData_Return.Add(ui);
                                        continue;
                                    }
                                }

                                if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.SaveSuccessfull = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                            else
                            {
                                ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))//NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                            int PrevDEP = (int)Wfd.PredDepid;

                            //Get Reported Qty for Operation
                            if (objDedep != null)
                            {
                                prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                            }

                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                            {
                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No)
                        || (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))//NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            ValidationPass = true;
                        }

                        //Stop reversing more than next operation
                        if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                        {
                            List<Wfdep> lstTDepList = null;
                            lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                            prevDEPInstQtyGd = 0;
                            prevDEPInstQtyScrap = 0;
                            prevDEPInstQtyRw = 0;

                            foreach (Wfdep objdep in lstTDepList)
                            {
                                int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;

                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                if (objDedepinst != null)
                                {
                                    nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                    nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                    nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                }

                                prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                            }

                            if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                            {

                                if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap)) //prevDEPInstQty = next
                                {
                                    ui.Responce[0] = "You caanot reverse Qty more than next operation reported Qty \n " + "Next Op Qty :" + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty) | Current Op Qty :" + currDEPInstQtyGd + "(Good Qty)";
                                    ui.Responce[1] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                    ui.EnteredQtyGd = 0;
                                    ui.EnteredQtyScrap = 0;
                                    ui.EnteredQtyRw = 0;
                                    ui.SaveSuccessfull = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }
                        }


                        if (ui.EnteredQtyGd < 0) //Qty Reverse validation - Good Qty
                        {
                            if (currDEPInstQtyGd == 0)
                            {
                                ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyGd;
                                ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyGd;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyGd < (ui.EnteredQtyGd * -1))
                            {
                                ui.Responce[0] = "Good Qty Reverse, You Cannot reverse (" + ui.EnteredQtyGd + ") more than reported Qty \n " + currDEPInstQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered)  >=" + currDEPInstQtyGd;
                                ui.Responce[1] = "Good Qty Reverse, You Cannot reverse (" + ui.EnteredQtyGd + ") more than reported Qty \n " + currDEPInstQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered)  >=" + currDEPInstQtyGd;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        if (ui.EnteredQtyScrap < 0) //Qty Reverse validation - Scrap Qty
                        {
                            if (currDEPInstQtyScrap == 0)
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyScrap;
                                ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyScrap;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyScrap < (ui.EnteredQtyScrap * -1))
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - You Cannot reverse (" + ui.EnteredQtyScrap + ") more than reported Qty \n " + currDEPInstQtyScrap.ToString() + "(Updated) + " + ui.EnteredQtyScrap + "(Entered)  >=" + currDEPInstQtyScrap;
                                ui.Responce[1] = "Scrap Qty Reverse - You Cannot reverse (" + ui.EnteredQtyScrap + ") more than reported Qty \n " + currDEPInstQtyScrap.ToString() + "(Updated) + " + ui.EnteredQtyScrap + "(Entered)  >=" + currDEPInstQtyScrap;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        if (ui.EnteredQtyRw < 0) //Qty Reverse validation - Rework Qty
                        {
                            if (currDEPInstQtyRw == 0)
                            {
                                ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyRw;
                                ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyRw;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyRw < (ui.EnteredQtyRw * -1))
                            {
                                ui.Responce[0] = "You Cannot reverse (" + ui.EnteredQtyRw + ") more than reported Qty \n " + currDEPQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered) + " + currDEPQtyScrap + "(Scrap) > " + prevDEPQtyGd;
                                ui.Responce[1] = "You Cannot reverse (" + ui.EnteredQtyRw + ") more than reported Qty \n " + currDEPQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered) + " + currDEPQtyScrap + "(Scrap) > " + prevDEPQtyGd;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }

                        //Split Qty to MOs
                        lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                        if (lstL5mo != null)
                        {
                            if (lstL5mo.Count > 1)
                            {
                                int balGdQty = 0, balScQty = 0;
                                balGdQty = ui.EnteredQtyGd;
                                balScQty = ui.EnteredQtyScrap;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;

                                foreach (L5mo l5mo in lstL5mo)
                                {
                                    Detxn detx = null;
                                    detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                    int repotdMOGdQty = 0, repotdMOSQty = 0;
                                    int possibGdQty = 0, possibScQty = 0;
                                    int MoQtyMax = 0;

                                    if (detx != null)
                                    {
                                        repotdMOGdQty = (int)detx.Qty01;
                                        repotdMOSQty = (int)detx.Qty02;
                                    }

                                    if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)
                                    {
                                        L5moop = null;
                                        L5moop = Lukup.GetReportedQtyByMONO(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId, l5mo.L5mono);

                                        if (L5moop != null)
                                        {
                                            MoQtyMax = (int)L5moop.ReportedQty + (int)L5moop.ScrappedQty;
                                        }
                                    }
                                    else
                                    {
                                        Detxn txn = null;
                                        txn = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.SizeId, ui.ScheduleId);
                                        if (txn != null)
                                        {
                                            MoQtyMax = (int)txn.Qty01 + (int)txn.Qty02;
                                        }
                                        else
                                        {
                                            //string msg= "";
                                            logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                            // ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                            // ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                            // lstBulkData_Return.Add(ui);
                                            // continue;
                                        }
                                    }
                                    if (detx != null)
                                    {
                                        repotdMOGdQty = (int)detx.Qty01;
                                        repotdMOSQty = (int)detx.Qty02;
                                    }

                                    if (MoQtyMax > (repotdMOGdQty + repotdMOSQty) && (balGdQty + balScQty) > 0)
                                    {
                                        possibGdQty = MoQtyMax - repotdMOGdQty;
                                        possibScQty = MoQtyMax - repotdMOSQty;

                                        if (balGdQty > 0) // process for good qty
                                        {
                                            if (balGdQty <= possibGdQty)
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                ui.EnteredQtyGd = ui.QtytoSaveGd = balGdQty;
                                                balGdQty = 0;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                            else
                                            {
                                                balGdQty = balGdQty - possibGdQty;
                                                ui.EnteredQtyGd = ui.QtytoSaveGd = possibGdQty;
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                        }
                                        if (balScQty > 0) // process for good qty
                                        {
                                            if (balScQty <= possibScQty)
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                ui.EnteredQtyGd = ui.QtytoSaveScrap = balScQty;
                                                balScQty = 0;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                            else
                                            {
                                                balScQty = balScQty - possibScQty;
                                                ui.EnteredQtyGd = ui.QtytoSaveScrap = possibScQty;
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                        }
                                    }
                                    else if ((balGdQty < 0 || balScQty < 0) && (repotdMOGdQty + repotdMOSQty) > 0)
                                    {
                                        possibGdQty = repotdMOGdQty;
                                        possibScQty = repotdMOSQty;

                                        if (balGdQty < 0) // process for good qty
                                        {
                                            if (balGdQty >= possibGdQty * -1)
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                ui.EnteredQtyGd = ui.QtytoSaveGd = balGdQty;
                                                balGdQty = 0;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                            else
                                            {
                                                balGdQty = balGdQty - (possibGdQty * -1);
                                                ui.EnteredQtyGd = ui.QtytoSaveGd = possibGdQty * -1;
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                        }
                                        if (balScQty < 0) // process for good qty
                                        {
                                            if (balScQty >= possibScQty * -1)
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                ui.EnteredQtyGd = ui.QtytoSaveScrap = balScQty;
                                                balScQty = 0;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                            else
                                            {
                                                balScQty = balScQty + possibScQty;
                                                ui.EnteredQtyGd = ui.QtytoSaveScrap = possibScQty * -1;
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;

                                                BulkData_Return = BulkUpdateCall(ui);
                                                //lstBulkData_Return.Add(BulkData_Return);
                                                continue;
                                            }
                                        }
                                    }
                                }
                                if (BulkData_Return.Responce == null)
                                {
                                    BulkData_Return.Responce[0] = "Qty Cannot reversed.";
                                    BulkData_Return.Responce[1] = "Qty Cannot reversed.";
                                }
                                lstBulkData_Return.Add(BulkData_Return);
                            }
                            else if (lstL5mo.Count > 0)
                            {
                                ui.L5MOID = lstL5mo[0].L5moid;
                                ui.L5MONo = lstL5mo[0].L5mono;

                                ui.QtytoSaveGd = ui.EnteredQtyGd;
                                ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                ui.QtytoSaveRw = ui.EnteredQtyRw;

                                BulkData_Return = BulkUpdateCall(ui);
                                lstBulkData_Return.Add(BulkData_Return);
                                continue;
                            }


                        }
                        else
                        {
                            ui.Responce[0] = "No MO recs found or MO is closed.";
                            ui.Responce[1] = "No MO recs found or MO is closed.";
                            ui.SaveSuccessfull = false;
                            lstBulkData_Return.Add(ui);
                            continue;
                        }
                    }
                    else
                    {
                        ui.Responce[0] = "Node is closed for data capture- For Operation " + ui.OperationCode.ToString() + " Or  " + ui.OperationCode2 + " Team Id - + " + ui.TeamId.ToString();
                        ui.Responce[1] = "Node is closed for data capture- For Operation " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - + " + ui.TeamId.ToString();
                        ui.SaveSuccessfull = false;
                        lstBulkData_Return.Add(ui);
                        continue;
                    }
                }
                return lstBulkData_Return;
            }
            catch (Exception ex)
            {
                lstUserInput[0].Responce[0] = ex.ToString();
                lstUserInput[0].Responce[0] = ex.ToString();
                lstUserInput[0].SaveSuccessfull = false;
                return lstUserInput;
            }
        }


        //Validation in Post Schedule no Enter in Bulk Data upload screen
        [Produces("application/json")]
        [HttpPost("UpdateBulkQtyBFL")]
        public List<UserInput> UpdateBulkQtyBFL([FromBody] List<UserInput> lstUserInput)
        {

            /*
                            lstUserInput[0].Responce[0] ="";
                            lstUserInput[0].Responce[0] ="";
                            lstUserInput[0].SaveSuccessfull = false;
                            return lstUserInput;
            */

            logger.InfoFormat("BusinessLogicsController-UpdateBulkQty API called with WFDEPInstId={0}", lstUserInput);

            bool ValidationPass = false;
            UserInput BulkData_Return = new UserInput();
            List<UserInput> lstBulkData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap, gu);
            LookupController Lukup = new LookupController(dcap);//, gu);
            logger.InfoFormat("BusinessLogicsController - LookupController Object created");

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();
            try
            {
                Wfdep Wfd = Lukup.GetWFConfigurationbyID(lstUserInput[0].WfdepinstId);

                foreach (UserInput ui in lstUserInput)
                {
                    //ui.guid = gu;
                    ui.Responce = new string[2];

                    if (ui.EnteredQtyGd == 0 && ui.EnteredQtyScrap == 0)
                    {
                        ui.Responce[0] = "Qty = 0, Not Processed";
                        ui.Responce[1] = "Qty = 0, Not Processed";
                        ui.SaveSuccessfull = false;
                        lstBulkData_Return.Add(ui);
                        continue;
                    }

                    ValidationPass = true;
                    ui.DCMId = Wfd.Dcmid;
                    ui.WorkCenter = Wfd.WorkCenter;
                    ui.Depid = Wfd.Depid;
                    ui.TeamId = Wfd.TeamId;
                    ui.DCMId = Wfd.Dcmid;
                    ui.DCLId = Wfd.Dclid;
                    ui.OperationCode = (int)Wfd.OperationCode;



                    int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                         currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                         prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                         prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;

                    Dedepinst objDedepinst = null;
                    Dedep objDedep = null;

                    if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                    {
                        objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                        //Get Reported Qty for Operation
                        if (objDedep != null)
                        {
                            currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                            currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                            currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                        }
                        //Get Reported Qty for Node
                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                        if (objDedepinst != null)
                        {
                            currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                            currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                            currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                        }
                        if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5  
                        {
                            L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                            if (L5moop != null)
                            {
                                prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                if (Wfd.LimitWithWf == (int)eLimitWithWF.NA)
                                {
                                    ui.Responce[0] = "LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                    ui.Responce[1] = "LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                    ui.SaveSuccessfull = false;
                                    ValidationPass = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                            {
                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPQtyGd + " (Curr Gd Qty) +" + currDEPQtyScrap + " (Curr Scrap Qty)";
                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPQtyGd + " (Curr Gd Qty) +" + currDEPQtyScrap + " (Curr Scrap Qty)";

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;

                                ui.SaveSuccessfull = false;
                                ValidationPass = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes)  //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                            {
                                if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    int PrevDEP = 0;
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                    if (lstTDepList != null)
                                    {
                                        if (lstTDepList.Count != 0)
                                        {
                                            objDedep = Lukup.GetQtyByDEPId((uint)lstTDepList[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                            if (objDedep != null)
                                            {
                                                prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                            }
                                        }
                                        else
                                        {
                                            ui.Responce[0] = "Invalid Configuration - This is a first node LimtWithPredecessor = Yes is invalid";
                                            ui.Responce[1] = "Invalid Configuration - This is a first node LimtWithPredecessor = Yes is invalid";
                                            ui.SaveSuccessfull = false;
                                            ValidationPass = false;
                                            lstBulkData_Return.Add(ui);
                                            continue;
                                        }
                                        //Avoid posting Qty more than previous opetation
                                        if ((prevDEPQtyGd) <= (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                        {
                                            ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.EnteredQtyGd = 0;
                                            ui.EnteredQtyScrap = 0;
                                            ui.EnteredQtyRw = 0;

                                            ui.SaveSuccessfull = false;
                                            ValidationPass = false;
                                            lstBulkData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                }
                                else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst)//==2  // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                    string PrevDEPInst = "";

                                    foreach (Wfdep objdep in lstTDepList)
                                    {
                                        int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                        PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                        if (objDedepinst != null)
                                        {
                                            iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                            iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                            iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                        }
                                        prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                        prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                        prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                    }
                                    if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                    {

                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.EnteredQtyGd = 0;
                                        ui.EnteredQtyScrap = 0;
                                        ui.EnteredQtyRw = 0;
                                        ui.SaveSuccessfull = false;
                                        ValidationPass = false;
                                        lstBulkData_Return.Add(ui);
                                        continue;
                                    }
                                }

                                if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                {
                                    ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.SaveSuccessfull = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                            else
                            {
                                ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))//NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                            int PrevDEP = (int)Wfd.PredDepid;

                            //Get Reported Qty for Operation
                            if (objDedep != null)
                            {
                                prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                            }

                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                            {
                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ui.SaveSuccessfull = false;
                                ValidationPass = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No) || (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))//NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                        {
                            ValidationPass = true;
                        }

                        //Stop reversing more than next operation
                        if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                        {
                            List<Wfdep> lstTDepList = null;
                            lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                            prevDEPInstQtyGd = 0;
                            prevDEPInstQtyScrap = 0;
                            prevDEPInstQtyRw = 0;

                            foreach (Wfdep objdep in lstTDepList)
                            {
                                int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;

                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                if (objDedepinst != null)
                                {
                                    nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                    nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                    nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                }

                                prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                            }

                            if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                            {

                                if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap)) //
                                {
                                    ui.Responce[0] = "You cannot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                    ui.Responce[1] = "You cannot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                    ui.EnteredQtyGd = 0;
                                    ui.EnteredQtyScrap = 0;
                                    ui.EnteredQtyRw = 0;
                                    ValidationPass = false;
                                    ui.SaveSuccessfull = false;
                                    lstBulkData_Return.Add(ui);
                                    continue;
                                }
                            }
                        }


                        if (ui.EnteredQtyGd < 0) //Qty Reverse validation - Good Qty
                        {
                            if (currDEPInstQtyGd == 0)
                            {
                                ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyGd;
                                ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyGd;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyGd < (ui.EnteredQtyGd * -1))
                            {
                                ui.Responce[0] = "Good Qty Reverse, You Cannot reverse (" + ui.EnteredQtyGd + ") more than reported Qty \n " + currDEPInstQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered)  >=" + currDEPInstQtyGd;
                                ui.Responce[1] = "Good Qty Reverse, You Cannot reverse (" + ui.EnteredQtyGd + ") more than reported Qty \n " + currDEPInstQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered)  >=" + currDEPInstQtyGd;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        if (ui.EnteredQtyScrap < 0) //Qty Reverse validation - Scrap Qty
                        {
                            if (currDEPInstQtyScrap == 0)
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyScrap;
                                ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyScrap;
                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyScrap < (ui.EnteredQtyScrap * -1))
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - You Cannot reverse (" + ui.EnteredQtyScrap + ") more than reported Qty \n " + currDEPInstQtyScrap.ToString() + "(Updated) + " + ui.EnteredQtyScrap + "(Entered)  >=" + currDEPInstQtyScrap;
                                ui.Responce[1] = "Scrap Qty Reverse - You Cannot reverse (" + ui.EnteredQtyScrap + ") more than reported Qty \n " + currDEPInstQtyScrap.ToString() + "(Updated) + " + ui.EnteredQtyScrap + "(Entered)  >=" + currDEPInstQtyScrap;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                        if (ui.EnteredQtyRw < 0) //Qty Reverse validation - Rework Qty
                        {
                            if (currDEPInstQtyRw == 0)
                            {
                                ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyRw;
                                ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported good Qty = 0 , You are trying to reverse -" + ui.EnteredQtyRw;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                            else if (currDEPInstQtyRw < (ui.EnteredQtyRw * -1))
                            {
                                ui.Responce[0] = "You Cannot reverse (" + ui.EnteredQtyRw + ") more than reported Qty \n " + currDEPQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered) + " + currDEPQtyScrap + "(Scrap) > " + prevDEPQtyGd;
                                ui.Responce[1] = "You Cannot reverse (" + ui.EnteredQtyRw + ") more than reported Qty \n " + currDEPQtyGd.ToString() + "(Updated) + " + ui.EnteredQtyGd + "(Entered) + " + currDEPQtyScrap + "(Scrap) > " + prevDEPQtyGd;

                                ui.EnteredQtyGd = 0;
                                ui.EnteredQtyScrap = 0;
                                ui.EnteredQtyRw = 0;
                                ValidationPass = false;
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }

                        if (ValidationPass)
                        {
                            //Split Qty to MOs
                            lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                            if (lstL5mo != null)
                            {
                                if (lstL5mo.Count > 1)
                                {
                                    int balGdQty = 0, balScQty = 0;
                                    balGdQty = ui.EnteredQtyGd;
                                    balScQty = ui.EnteredQtyScrap;
                                    ui.EnteredQtyGd = 0;
                                    ui.EnteredQtyScrap = 0;

                                    foreach (L5mo l5mo in lstL5mo)
                                    {
                                        Detxn detx = null;
                                        detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                        int repotdMOGdQty = 0, repotdMOSQty = 0;
                                        int possibGdQty = 0, possibScQty = 0;
                                        int MoQtyMax = 0;

                                        if (detx != null)
                                        {
                                            repotdMOGdQty = (int)detx.Qty01;
                                            repotdMOSQty = (int)detx.Qty02;
                                        }

                                        if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)
                                        {
                                            L5moop = null;
                                            L5moop = Lukup.GetReportedQtyByMONO(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId, l5mo.L5mono);

                                            if (L5moop != null)
                                            {
                                                MoQtyMax = (int)L5moop.ReportedQty + (int)L5moop.ScrappedQty;
                                            }
                                        }
                                        else
                                        {
                                            Detxn txn = null;
                                            txn = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.SizeId, ui.ScheduleId);
                                            if (txn != null)
                                            {
                                                MoQtyMax = (int)txn.Qty01 + (int)txn.Qty02;
                                            }
                                            else
                                            {
                                                //string msg= "";
                                                logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                                // ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                // ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                // lstBulkData_Return.Add(ui);
                                                // continue;
                                            }
                                        }
                                        if (detx != null)
                                        {
                                            repotdMOGdQty = (int)detx.Qty01;
                                            repotdMOSQty = (int)detx.Qty02;
                                        }

                                        if (MoQtyMax > (repotdMOGdQty + repotdMOSQty) && (balGdQty + balScQty) > 0)
                                        {
                                            possibGdQty = MoQtyMax - repotdMOGdQty;
                                            possibScQty = MoQtyMax - repotdMOSQty;

                                            if (balGdQty > 0) // process for good qty
                                            {
                                                if (balGdQty <= possibGdQty)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    ui.EnteredQtyGd = ui.QtytoSaveGd = balGdQty;
                                                    balGdQty = 0;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                                else
                                                {
                                                    balGdQty = balGdQty - possibGdQty;
                                                    ui.EnteredQtyGd = ui.QtytoSaveGd = possibGdQty;
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                            }
                                            if (balScQty > 0) // process for good qty
                                            {
                                                if (balScQty <= possibScQty)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    ui.EnteredQtyGd = ui.QtytoSaveScrap = balScQty;
                                                    balScQty = 0;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                                else
                                                {
                                                    balScQty = balScQty - possibScQty;
                                                    ui.EnteredQtyGd = ui.QtytoSaveScrap = possibScQty;
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                            }
                                        }
                                        else if ((balGdQty < 0 || balScQty < 0) && (repotdMOGdQty + repotdMOSQty) > 0)
                                        {
                                            possibGdQty = repotdMOGdQty;
                                            possibScQty = repotdMOSQty;

                                            if (balGdQty < 0) // process for good qty
                                            {
                                                if (balGdQty >= possibGdQty * -1)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    ui.EnteredQtyGd = ui.QtytoSaveGd = balGdQty;
                                                    balGdQty = 0;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                                else
                                                {
                                                    balGdQty = balGdQty - (possibGdQty * -1);
                                                    ui.EnteredQtyGd = ui.QtytoSaveGd = possibGdQty * -1;
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                            }
                                            if (balScQty < 0) // process for good qty
                                            {
                                                if (balScQty >= possibScQty * -1)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    ui.EnteredQtyGd = ui.QtytoSaveScrap = balScQty;
                                                    balScQty = 0;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                                else
                                                {
                                                    balScQty = balScQty + possibScQty;
                                                    ui.EnteredQtyGd = ui.QtytoSaveScrap = possibScQty * -1;
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;

                                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                                    //lstBulkData_Return.Add(BulkData_Return);
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    if (BulkData_Return.Responce == null)
                                    {
                                        BulkData_Return.Responce[0] = "Qty Cannot reversed.";
                                        BulkData_Return.Responce[1] = "Qty Cannot reversed.";
                                    }
                                    lstBulkData_Return.Add(BulkData_Return);
                                }
                                else if (lstL5mo.Count > 0)
                                {
                                    ui.L5MOID = lstL5mo[0].L5moid;
                                    ui.L5MONo = lstL5mo[0].L5mono;

                                    ui.QtytoSaveGd = ui.EnteredQtyGd;
                                    ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                    ui.QtytoSaveRw = ui.EnteredQtyRw;

                                    BulkData_Return = BulkUpdateCallBFL(ui);
                                    lstBulkData_Return.Add(BulkData_Return);
                                    continue;
                                }


                            }
                            else
                            {
                                ui.Responce[0] = "No MO recs found or MO is closed.";
                                ui.Responce[1] = "No MO recs found or MO is closed.";
                                ui.SaveSuccessfull = false;
                                lstBulkData_Return.Add(ui);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        ui.Responce[0] = "Node is closed for data capture- For Operation " + ui.OperationCode.ToString() + " Or  " + ui.OperationCode2 + " Team Id - + " + ui.TeamId.ToString();
                        ui.Responce[1] = "Node is closed for data capture- For Operation " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - + " + ui.TeamId.ToString();
                        ui.SaveSuccessfull = false;
                        lstBulkData_Return.Add(ui);
                        continue;
                    }
                }

                lstBulkData_Return[0].ScanCount = Lukup.GetScanCounterVal(lstBulkData_Return[0].WfdepinstId);
                return lstBulkData_Return;
            }
            catch (Exception ex)
            {
                lstUserInput[0].Responce[0] = ex.ToString();
                lstUserInput[0].Responce[0] = ex.ToString();
                lstUserInput[0].SaveSuccessfull = false;
                return lstUserInput;
            }
        }


        //Validation in Post Schedule no Enter in Barcode Data upload screen || Checked 8-1-2020'
        //Check and validate barcode details and tigger transaction
        //Used API's and UI : Barcode UI SenttoDB
        [Produces("application/json")]
        [HttpPost("UpdateBCScanData")]
        public List<UserInput> UpdateBCScanData([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateBCScanData - UpdateBCScanData API called with WFDEPInstId={0}", lstUserInput);
            bool ValidationPass = false;
            UserInput BCData_Return = new UserInput();
            List<UserInput> lstBCData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap,LogGuid);
            LookupController Lukup = new LookupController(dcap);//,LogGuid);

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();

            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    if (ui.Barcode != "")
                    {
                        int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                            currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                            prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                            prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;
                        bool bcClrAssigned = false, noPreQtyValidation = false;

                        Dedepinst objDedepinst = null;
                        Dedep objDedep = null;
                        Detxn objDetxn = null;

                        Wfdep Wfd = Lukup.GetWFConfigurationbyID(ui.WfdepinstId);
                        objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, Wfd.Depid);

                        ui.WFID = (uint)Wfd.Wfid;
                        ui.Depid = Wfd.Depid;
                        ui.DCLId = Wfd.Dclid;
                        ui.TeamId = Wfd.TeamId;
                        ui.Responce = new string[2];
                        ui.TxnDate = System.DateTime.Now;

                        //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                        if (ui.OperationCode != (int)Wfd.OperationCode)
                        {
                            ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //L5bc l5b = Lukup.GetL5BCData(ui.Barcode);
                        //Check for the exsistence of valid quantity
                        if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)
                        {
                            logger.InfoFormat("UpdateBCScanData - UpdateBulkQty API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);

                            ui.Responce[0] = "Please scan again..";
                            ui.Responce[1] = "Please scan again..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                        }

                        //Check if transaction is Scrap but Reject Reson is null
                        if (ui.EnteredQtyScrap > 0 && string.IsNullOrEmpty(ui.RRId))
                        {
                            ui.Responce[0] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.Responce[1] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Need to check previus scan is rework and now it is good or scrap
                        if (objDetxn != null)
                        {
                            //Check wether that prvious quantitiy is exsists and trying to add quantity for exsisiting record
                            if ((objDetxn.Qty01 + objDetxn.Qty02) >= 1 && (ui.EnteredQtyScrap + ui.EnteredQtyGd) > 0)
                            {
                                ui.Responce[0] = "Barcode is already Used";
                                ui.Responce[1] = "Barcode is already Used";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if ((objDetxn.Qty01 + objDetxn.Qty02) > 0 && ui.EnteredQtyRw > 0)
                            {
                                ui.Responce[0] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.Responce[1] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            #region Qty Reverse

                            if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                            {
                                if (objDetxn.Qty01 == 0)
                                {
                                    ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (objDetxn.Qty01 < (ui.EnteredQtyGd * -1))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }

                            }

                            if (ui.EnteredQtyScrap < 0) //Scrap Qty Reverse
                            {
                                if (objDetxn.Qty02 == 0)
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty02 < (ui.EnteredQtyScrap * -1))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if (ui.EnteredQtyRw < 0) //Rework Qty Reverse
                            {
                                Detxn Detxn2 = null;
                                Detxn2 = Lukup.GetRRIdToReverse(ui.Barcode);
                                if (Detxn2 != null)
                                {
                                    ui.RRId = Detxn2.Rrid.ToString();
                                    ui.DOpsId = Detxn2.DopsId.ToString();
                                }

                                if (objDetxn.Qty03 == 0)
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty03 < (ui.EnteredQtyRw * -1))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            #endregion

                        }
                        else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                        {
                            ui.Responce[0] = "No transaction recorded to reverse";
                            ui.Responce[1] = "No transaction recorded to reverse";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Assign Color for Barcode
                        L5bc l5bc = Lukup.GetL5BCData(ui.Barcode);
                        if (l5bc != null)
                        {
                            ui.StyleId = (uint)l5bc.L1id;
                            ui.ScheduleId = (uint)l5bc.L2id;
                            ui.SizeId = (uint)l5bc.L5id;

                            if (ui.ColorIdUI != 0 && ui.ColorId != 0 && (ui.ColorIdUI != ui.ColorId))
                            {
                                ui.Responce[0] = "Selected Color do not match with barcode color, Pls select correct color";
                                ui.Responce[1] = "Selected Color do not match with barcode color, Pls select correct color";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if (l5bc.L4id == 0 && ui.ColorIdUI == 0)
                            {
                                ui.Responce[0] = "Color not assign to Barcode, Pls select color";
                                ui.Responce[1] = "Color not assign to Barcode, Pls select color";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (l5bc.L4id != 0 && ui.ColorIdUI == 0) //Color not provided by user & get color from barcode
                            {
                                ui.ColorIdUI = (uint)l5bc.L4id;
                            }

                            if (l5bc.L4id == 0) //Assigning the color for barcode
                            {
                                l5bc.L4id = ui.ColorIdUI;
                                l5bc.BarCodeNo = ui.Barcode;
                                UpdateColorforBC(l5bc, ui);
                                bcClrAssigned = true;
                            }
                            ui.ColorId = ui.ColorIdUI;
                        }
                        else  //Invalid barcode input by user
                        {
                            ui.Responce[0] = "Scanned barcode does not exists";
                            ui.Responce[1] = "Scanned barcode does not exists";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Check if the barcode is scanned as a good garment in previous opperation
                        if (Wfd.Bccheck == (int)eBCCheck.DEPLevel || Wfd.Bccheck == (int)eBCCheck.DEPInstLevel) //NA = 0, No = 1, DEPLevel = 2, DEPInstLevel = 3
                        {
                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous); // 2
                            objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, depid[0].Depid);

                            if (objDetxn != null)
                            {
                                if (objDetxn.Qty01 == 1)
                                {
                                    ValidationPass = true;
                                }
                                else
                                {
                                    ValidationPass = false;
                                }
                            }
                        }
                        else
                        {
                            ValidationPass = true;
                        }

                        if (ValidationPass)
                        {
                            if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                            {
                                //Get Reported Qty for Operation
                                objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedep != null)
                                {
                                    currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                    currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                    currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                }

                                //Get Reported Qty for Node
                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedepinst != null)
                                {
                                    currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                    currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                    currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                }

                                //Get previous qty based on the configuration
                                if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                                    if (L5moop != null)
                                    {
                                        prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                        if (Wfd.LimitWithWf == (int)eLimitWithWF.NA) // NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                        {
                                            ui.Responce[0] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.Responce[1] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                    {
                                        //ui.Responce[0] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        //ui.Responce[1] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes) //== 1 //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                                    {
                                        if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            int PrevDEP = 0;
                                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);

                                            if (depid != null)
                                            {
                                                objDedep = Lukup.GetQtyByDEPId((uint)depid[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                                PrevDEP = (int)depid[0].Depid;
                                                if (objDedep != null)
                                                {
                                                    prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                    prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                    prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                                }
                                            }
                                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst) //==2  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            List<Wfdep> lstTDepList = null;
                                            lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                            string PrevDEPInst = "";

                                            //TODO
                                            //PSSIBLE TO APLY FOLLOWING QUERARY TO ELIMINATE foreach (Wfdep objdep in lstTDepList)
                                            // select sum(Qty01) , sum(Qty02) , sum(Qty03)  from Dedepinst I
                                            // inner join wfdeplink L on I.WfdepinstId =  L.WfdepinstId
                                            // INNER JOIN  wfdep w ON l.WFDEPInstId = w.WFDEPInstId  
                                            // where   I.L1Id = 116 and  I.L2Id =1 and I.L3Id =0 and I.L4Id = 2 and I.L5Id = 3 
                                            // and  L.WFDEPIdLink =  330 AND L.RecStatus = 1

                                            foreach (Wfdep objdep in lstTDepList)
                                            {
                                                int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                                PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                                if (objDedepinst != null)
                                                {
                                                    iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                                    iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                                    iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                                }
                                                prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                                prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                                prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                            }

                                            //Change Before Build NimanthaH
                                            //if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.SaveSuccessfull = false;
                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                                    else
                                    {
                                        ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.SaveSuccessfull = false;
                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))
                                {
                                    objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                    int PrevDEP = (int)Wfd.PredDepid;
// Check the barcode if already there in detxn table with the depid-predDepid

                                    //Get Reported Qty for Operation
                                    if (objDedep != null)
                                    {
                                        prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                        prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                        prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                    }

                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                    {
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No) ||
                                (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))  // / NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5      
                                {
                                    noPreQtyValidation = true;
                                }
                                //Stop reversing more than next operation
                                if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                                {
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                                    prevDEPInstQtyGd = 0;
                                    prevDEPInstQtyScrap = 0;
                                    prevDEPInstQtyRw = 0;
                                    int nxtOppCode = 0;

                                    foreach (Wfdep objdep in lstTDepList)
                                    {
                                        int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;
                                        nxtOppCode = (int)lstTDepList[0].OperationCode;

                                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                        if (objDedepinst != null)
                                        {
                                            nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                            nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                            nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                        }

                                        prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                        prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                        prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                                    }
                                    if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                                    {
                                        if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap))
                                        {
                                            if (!Lukup.ValidateBCReverse(ui.Barcode, nxtOppCode))
                                            {
                                                ui.Responce[0] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.Responce[1] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.SaveSuccessfull = false;
                                                lstBCData_Return.Add(ui);

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                        }
                                    }
                                }

                                //MO Split 
                                lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                                if (lstL5mo != null)
                                {
                                    if (lstL5mo.Count > 1)
                                    {
                                        foreach (L5mo l5mo in lstL5mo)
                                        {
                                            Detxn Detx = null;
                                            Detx = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.StyleId, ui.ScheduleId);

                                            int MoQtyMax = 0;
                                            if (Detx != null)
                                            {
                                                MoQtyMax = (int)Detx.Qty01 + (int)Detx.Qty02;
                                            }
                                            else
                                            {
                                                logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                                ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                            Detxn detx = null;
                                            detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                            int MOQty = 0, MOSQty = 0, MORWQty = 0;

                                            if (detx != null)
                                            {
                                                MOQty = (int)detx.Qty01;
                                                MOSQty = (int)detx.Qty02;
                                                MORWQty = (int)detx.Qty03;
                                            }

                                            if (ui.EnteredQtyGd < 0 || ui.EnteredQtyScrap < 0 || ui.EnteredQtyRw < 0)
                                            {
                                                detx = Lukup.GetReportedQtyforMOBC(ui.WfdepinstId, l5mo.L5mono, ui.Barcode);

                                                if (detx != null)
                                                {
                                                    if (ui.EnteredQtyGd < 0 && MOQty > 0 && detx.Qty01 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyScrap < 0 && MOSQty > 0 && detx.Qty02 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyRw < 0 && MORWQty > 0 && detx.Qty03 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (MoQtyMax > (MOQty + MOSQty))
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                break;
                                            }
                                        }
                                    }
                                    else if (lstL5mo.Count == 1)
                                    {
                                        ui.L5MOID = lstL5mo[0].L5moid;
                                        ui.L5MONo = lstL5mo[0].L5mono;
                                    }

                                    if (ui.L5MONo != null)
                                    {
                                        ui.QtytoSaveGd = ui.EnteredQtyGd;
                                        ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                        ui.QtytoSaveRw = ui.EnteredQtyRw;

                                        //PO BAG Counter Checker - Added By NimanthaH
                                        if (Wfd.POCounterEnable == 1)
                                        {
                                            if (Wfd.POCounterNumber == 1)
                                            {
                                                // 
                                                BusinessLogicsController bu = new BusinessLogicsController(dcap);
                                                //New code start - To check the flag if bagbarcode is there 02-11-2022
                                                var Response = new TeamCounterOutput();
                                                //Response= null;
                                                Response = bu.BagBarcodeChecker(ui, true);
                                                if(Wfd.AddNewBag==1)
                                                {
                                                 Response.updated=true;
                                                 Response.IsUpdateAvilable=false;
                                                 Response.BagBarCodeNo= ui.BagBarCodeNo;
                                                }
                                                
                                                if (Response != null)
                                                {
                                                    if (!Response.updated)
                                                    {
                                                        ui.Responce[0] = "PO Counter Issue (Bag Barcode update failed)";
                                                        ui.Responce[1] = "PO Counter Issue (Bag Barcode update failed)";
                                                        ui.NewCounter = false;
                                                        ui.SaveSuccessfull = false;
                                                        lstBCData_Return.Add(ui);
                                                    }
                                                    else
                                                    {
                                                        if (Response.IsUpdateAvilable)
                                                        {
                                                            ui.CounterId = Response.CounterId;
                                                           // ui.CounterId = 267324;
                                                            ui.BagBarCodeNo = Response.BagBarCodeNo;
                                                        }

                                                        ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                                        ui.NewCounter = Response.NewCounter;

                                                        BCData_Return = DbUpdate(ui);

                                                    }
                                                }
                                                else
                                                {
                                                    ui.Responce[0] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                                    ui.Responce[1] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                                    ui.NewCounter = false;
                                                    ui.SaveSuccessfull = false;
                                                    lstBCData_Return.Add(ui);
                                                }
                                                
                                                
                                                //New ocde End - 02-11-2022 
                                                // var Response = bu.BagBarcodeChecker(ui, true);
                                                // if (Response != null)
                                                // {
                                                //     if (!Response.updated)
                                                //     {
                                                //         ui.Responce[0] = "PO Counter Issue (Bag Barcode update failed)";
                                                //         ui.Responce[1] = "PO Counter Issue (Bag Barcode update failed)";
                                                //         ui.NewCounter = false;
                                                //         ui.SaveSuccessfull = false;
                                                //         lstBCData_Return.Add(ui);
                                                //     }
                                                //     else
                                                //     {
                                                //         if (Response.IsUpdateAvilable)
                                                //         {
                                                //             ui.CounterId = Response.CounterId;
                                                //             ui.BagBarCodeNo = Response.BagBarCodeNo;
                                                //         }

                                                //         ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                                //         ui.NewCounter = Response.NewCounter;

                                                //         BCData_Return = DbUpdate(ui);

                                                //     }
                                                // }
                                                // else
                                                // {
                                                //     ui.Responce[0] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                                //     ui.Responce[1] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                                //     ui.NewCounter = false;
                                                //     ui.SaveSuccessfull = false;
                                                //     lstBCData_Return.Add(ui);
                                                // }
                                            }
                                            else
                                            {
                                                if (!Lukup.POLimitCheck(ui))
                                                {
                                                    ui.Responce[0] = "PO Counter Issue, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.Responce[1] = "PO Counter Issue, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.NewCounter = false;
                                                    ui.SaveSuccessfull = false;
                                                    lstBCData_Return.Add(ui);
                                                    continue;
                                                }
                                                else
                                                {
                                                    BCData_Return = DbUpdate(ui);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ui.IsUpdateAvilable = false;
                                            ui.NewCounter = false;

                                            BCData_Return = DbUpdate(ui);
                                        }
                                    }

                                    //po

                                    if (BCData_Return.SaveSuccessfull == true)
                                    //if (true)
                                    {
                                        if (ui.QtytoSaveGd != 0)
                                        {
                                            UpdateLineCounter((int)ui.WfdepinstId, ui.QtytoSaveGd);
                                        }

                                        HourlyCounts hc = new HourlyCounts();
                                        hc = GetHourlyCounts(ui);

                                        if (hc != null)
                                        {
                                            //ui.CurHrGood = hc.CurHrGood;
                                            //ui.CurHrScrap = hc.CurHrScrap;
                                            //ui.CurHrRework = hc.CurHrRework;

                                            // ui.PrevHrGood = hc.PrevHrGood;
                                            // ui.PrevHrScrap = hc.PrevHrScrap;
                                            // ui.PrevHrRework = hc.PrevHrRework;

                                            ui.TotGood = hc.TotGood;
                                        }
                                        ui.ScanCount = Lukup.GetScanCounterVal(ui.WfdepinstId);
                                    }
                                }
                            }
                            else
                            {
                                ui.Responce[0] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.Responce[1] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                if (bcClrAssigned)
                                {
                                    RemoveColorforBC(ui);
                                }
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }
                        else
                        {
                            ui.Responce[0] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.Responce[1] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            if (bcClrAssigned)
                            {
                                RemoveColorforBC(ui);
                            }
                            lstBCData_Return.Add(ui);
                            continue;
                        }
                        lstBCData_Return.Add(BCData_Return);

                    }
                    else
                    {
                        ui.Responce[0] = "Barcode value is empty. Please Check..";
                        ui.Responce[1] = "Barcode value is empty. Please Check..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                    }
                }
                return lstBCData_Return;
            }
            catch (Exception ex)
            {
                lstBCData_Return[0].Responce = new string[2];

                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].SaveSuccessfull = false;
                return lstBCData_Return;
            }

            logger.InfoFormat("UpdateBCScanData - End Of UpdateBulkQty API call");

        }

        [Produces("application/json")]
        [HttpPost("UpdateBCScanDataBFL")]
        public List<UserInput> UpdateBCScanDataBFL([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateBCScanDataBFL - UpdateBCScanDataBFL API called with WFDEPInstId={0}", lstUserInput);
            bool ValidationPass = false;
            UserInput BCData_Return = new UserInput();
            List<UserInput> lstBCData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap,LogGuid);
            LookupController Lukup = new LookupController(dcap);//,LogGuid);

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();

            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    if (ui.Barcode != "")
                    {
                        int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                            currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                            prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                            prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;
                        bool bcClrAssigned = false, noPreQtyValidation = false;

                        Dedepinst objDedepinst = null;
                        Dedep objDedep = null;
                        Detxn objDetxn = null;

                        Wfdep Wfd = Lukup.GetWFConfigurationbyID(ui.WfdepinstId);
                        objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, Wfd.Depid);

                        ui.WFID = (uint)Wfd.Wfid;
                        ui.Depid = Wfd.Depid;
                        ui.DCLId = Wfd.Dclid;
                        ui.TeamId = Wfd.TeamId;
                        ui.Responce = new string[2];
                        ui.TxnDate = System.DateTime.Now;

                        //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                        if (ui.OperationCode != (int)Wfd.OperationCode)
                        {
                            ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //L5bc l5b = Lukup.GetL5BCData(ui.Barcode);
                        //Check for the exsistence of valid quantity
                        if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)
                        {
                            logger.InfoFormat("UpdateBCScanData - UpdateBulkQty API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);

                            ui.Responce[0] = "Please scan again..";
                            ui.Responce[1] = "Please scan again..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                        }

                        //Check if transaction is Scrap but Reject Reson is null
                        if (ui.EnteredQtyScrap > 0 && string.IsNullOrEmpty(ui.RRId))
                        {
                            ui.Responce[0] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.Responce[1] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Need to check previus scan is rework and now it is good or scrap
                        if (objDetxn != null)
                        {
                            //Check wether that prvious quantitiy is exsists and trying to add quantity for exsisiting record
                            if ((objDetxn.Qty01 + objDetxn.Qty02) >= 1 && (ui.EnteredQtyScrap + ui.EnteredQtyGd) > 0)
                            {
                                ui.Responce[0] = "Barcode is already Used";
                                ui.Responce[1] = "Barcode is already Used";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if ((objDetxn.Qty01 + objDetxn.Qty02) > 0 && ui.EnteredQtyRw > 0)
                            {
                                ui.Responce[0] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.Responce[1] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            #region Qty Reverse

                            if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                            {
                                if (objDetxn.Qty01 == 0)
                                {
                                    ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (objDetxn.Qty01 < (ui.EnteredQtyGd * -1))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }

                            }

                            if (ui.EnteredQtyScrap < 0) //Scrap Qty Reverse
                            {
                                if (objDetxn.Qty02 == 0)
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty02 < (ui.EnteredQtyScrap * -1))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if (ui.EnteredQtyRw < 0) //Rework Qty Reverse
                            {
                                Detxn Detxn2 = null;
                                Detxn2 = Lukup.GetRRIdToReverse(ui.Barcode);
                                if (Detxn2 != null)
                                {
                                    ui.RRId = Detxn2.Rrid.ToString();
                                    ui.DOpsId = Detxn2.DopsId.ToString();
                                }

                                if (objDetxn.Qty03 == 0)
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty03 < (ui.EnteredQtyRw * -1))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            #endregion

                        }
                        else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                        {
                            ui.Responce[0] = "No transaction recorded to reverse";
                            ui.Responce[1] = "No transaction recorded to reverse";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Assign Color for Barcode
                        L5bc l5bc = Lukup.GetL5BCData(ui.Barcode);
                        if (l5bc != null)
                        {
                            ui.StyleId = (uint)l5bc.L1id;
                            ui.ScheduleId = (uint)l5bc.L2id;
                            ui.SizeId = (uint)l5bc.L5id;

                            if (ui.ColorIdUI != 0 && ui.ColorId != 0 && (ui.ColorIdUI != ui.ColorId))
                            {
                                ui.Responce[0] = "Selected Color do not match with barcode color, Pls select correct color";
                                ui.Responce[1] = "Selected Color do not match with barcode color, Pls select correct color";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if (l5bc.L4id == 0 && ui.ColorIdUI == 0)
                            {
                                ui.Responce[0] = "Color not assign to Barcode, Pls select color";
                                ui.Responce[1] = "Color not assign to Barcode, Pls select color";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (l5bc.L4id != 0 && ui.ColorIdUI == 0) //Color not provided by user & get color from barcode
                            {
                                ui.ColorIdUI = (uint)l5bc.L4id;
                            }

                            if (l5bc.L4id == 0) //Assigning the color for barcode
                            {
                                l5bc.L4id = ui.ColorIdUI;
                                l5bc.BarCodeNo = ui.Barcode;
                                UpdateColorforBC(l5bc, ui);
                                bcClrAssigned = true;
                            }
                            ui.ColorId = ui.ColorIdUI;
                        }
                        else  //Invalid barcode input by user
                        {
                            ui.Responce[0] = "Scanned barcode does not exists";
                            ui.Responce[1] = "Scanned barcode does not exists";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Check if the barcode is scanned as a good garment in previous opperation
                        if (Wfd.Bccheck == (int)eBCCheck.DEPLevel || Wfd.Bccheck == (int)eBCCheck.DEPInstLevel) //NA = 0, No = 1, DEPLevel = 2, DEPInstLevel = 3
                        {
                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous); // 2
                            objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, depid[0].Depid);

                            if (objDetxn != null)
                            {
                                if (objDetxn.Qty01 == 1)
                                {
                                    ValidationPass = true;
                                }
                                else
                                {
                                    ValidationPass = false;
                                }
                            }
                        }
                        else
                        {
                            ValidationPass = true;
                        }

                        if (ValidationPass)
                        {
                            if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                            {
                                //Get Reported Qty for Operation
                                objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedep != null)
                                {
                                    currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                    currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                    currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                }

                                //Get Reported Qty for Node
                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedepinst != null)
                                {
                                    currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                    currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                    currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                }

                                //Get previous qty based on the configuration
                                if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                                    if (L5moop != null)
                                    {
                                        prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                        if (Wfd.LimitWithWf == (int)eLimitWithWF.NA) // NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                        {
                                            ui.Responce[0] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.Responce[1] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                    {
                                        //ui.Responce[0] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        //ui.Responce[1] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes) //== 1 //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                                    {
                                        if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            int PrevDEP = 0;
                                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);

                                            if (depid != null)
                                            {
                                                objDedep = Lukup.GetQtyByDEPId((uint)depid[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                                PrevDEP = (int)depid[0].Depid;
                                                if (objDedep != null)
                                                {
                                                    prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                    prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                    prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                                }
                                            }
                                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst) //==2  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            List<Wfdep> lstTDepList = null;
                                            lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                            string PrevDEPInst = "";

                                            //TODO
                                            //PSSIBLE TO APLY FOLLOWING QUERARY TO ELIMINATE foreach (Wfdep objdep in lstTDepList)
                                            // select sum(Qty01) , sum(Qty02) , sum(Qty03)  from Dedepinst I
                                            // inner join wfdeplink L on I.WfdepinstId =  L.WfdepinstId
                                            // INNER JOIN  wfdep w ON l.WFDEPInstId = w.WFDEPInstId  
                                            // where   I.L1Id = 116 and  I.L2Id =1 and I.L3Id =0 and I.L4Id = 2 and I.L5Id = 3 
                                            // and  L.WFDEPIdLink =  330 AND L.RecStatus = 1

                                            foreach (Wfdep objdep in lstTDepList)
                                            {
                                                int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                                PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                                if (objDedepinst != null)
                                                {
                                                    iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                                    iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                                    iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                                }
                                                prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                                prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                                prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                            }

                                            //Change Before Build NimanthaH
                                            //if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.SaveSuccessfull = false;
                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                                    else
                                    {
                                        ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.SaveSuccessfull = false;
                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))
                                {
                                    objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                    int PrevDEP = (int)Wfd.PredDepid;

                                    //Get Reported Qty for Operation
                                    if (objDedep != null)
                                    {
                                        prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                        prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                        prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                    }

                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                    {
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No) || (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))  // / NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5      
                                {
                                    noPreQtyValidation = true;
                                }
                                //Stop reversing more than next operation
                                if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                                {
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                                    prevDEPInstQtyGd = 0;
                                    prevDEPInstQtyScrap = 0;
                                    prevDEPInstQtyRw = 0;
                                    int nxtOppCode = 0;

                                    foreach (Wfdep objdep in lstTDepList)
                                    {
                                        int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;
                                        nxtOppCode = (int)lstTDepList[0].OperationCode;

                                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                        if (objDedepinst != null)
                                        {
                                            nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                            nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                            nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                        }

                                        prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                        prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                        prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                                    }
                                    if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                                    {
                                        if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap))
                                        {
                                            if (!Lukup.ValidateBCReverse(ui.Barcode, nxtOppCode))
                                            {
                                                ui.Responce[0] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.Responce[1] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.SaveSuccessfull = false;
                                                lstBCData_Return.Add(ui);

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                        }
                                    }
                                }

                                //MO Split 
                                lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                                if (lstL5mo != null)
                                {
                                    if (lstL5mo.Count > 1)
                                    {
                                        foreach (L5mo l5mo in lstL5mo)
                                        {
                                            Detxn Detx = null;
                                            Detx = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.StyleId, ui.ScheduleId);

                                            int MoQtyMax = 0;
                                            if (Detx != null)
                                            {
                                                MoQtyMax = (int)Detx.Qty01 + (int)Detx.Qty02;
                                            }
                                            else
                                            {
                                                logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                                ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                            Detxn detx = null;
                                            detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                            int MOQty = 0, MOSQty = 0, MORWQty = 0;

                                            if (detx != null)
                                            {
                                                MOQty = (int)detx.Qty01;
                                                MOSQty = (int)detx.Qty02;
                                                MORWQty = (int)detx.Qty03;
                                            }

                                            if (ui.EnteredQtyGd < 0 || ui.EnteredQtyScrap < 0 || ui.EnteredQtyRw < 0)
                                            {
                                                detx = Lukup.GetReportedQtyforMOBC(ui.WfdepinstId, l5mo.L5mono, ui.Barcode);

                                                if (detx != null)
                                                {
                                                    if (ui.EnteredQtyGd < 0 && MOQty > 0 && detx.Qty01 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyScrap < 0 && MOSQty > 0 && detx.Qty02 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyRw < 0 && MORWQty > 0 && detx.Qty03 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (MoQtyMax > (MOQty + MOSQty))
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                break;
                                            }
                                        }
                                    }
                                    else if (lstL5mo.Count == 1)
                                    {
                                        ui.L5MOID = lstL5mo[0].L5moid;
                                        ui.L5MONo = lstL5mo[0].L5mono;
                                    }

                                    if (ui.L5MONo != null)
                                    {
                                        ui.QtytoSaveGd = ui.EnteredQtyGd;
                                        ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                        ui.QtytoSaveRw = ui.EnteredQtyRw;

                                        //PO BAG Counter Checker - Added By NimanthaH
                                        if (Wfd.POCounterEnable == 1)
                                        {
                                            if (Wfd.POCounterNumber == 1)
                                            {
                                                BusinessLogicsController bu = new BusinessLogicsController(dcap);
                                                var Response = bu.BuddyBarcodeChecker(ui, true);
                                                if (Response != null)
                                                {
                                                    if (!Response.updated)
                                                    {
                                                        ui.Responce[0] = "Buddy Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                        ui.Responce[1] = "Buddy Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                        ui.NewCounter = false;
                                                        ui.SaveSuccessfull = false;
                                                        lstBCData_Return.Add(ui);
                                                    }
                                                    else
                                                    {
                                                        if (Response.IsUpdateAvilable)
                                                        {
                                                            ui.CounterId = Response.CounterId;
                                                            ui.BagBarCodeNo = Response.BagBarCodeNo;
                                                            ui.RefBagBarCodeNo = Response.RefBagBarCodeNo;
                                                        }

                                                        ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                                        ui.NewCounter = Response.NewCounter;

                                                        BCData_Return = DbUpdateBFL(ui);

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!Lukup.POLimitCheck(ui))
                                                {
                                                    ui.Responce[0] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.Responce[1] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.NewCounter = false;
                                                    ui.SaveSuccessfull = false;
                                                    lstBCData_Return.Add(ui);
                                                    continue;
                                                }
                                                else
                                                {
                                                    BCData_Return = DbUpdateBFL(ui);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ui.IsUpdateAvilable = false;
                                            ui.NewCounter = false;

                                            BCData_Return = DbUpdateBFL(ui);
                                        }
                                    }

                                    //po

                                    if (BCData_Return.SaveSuccessfull == true)
                                    //if (true)
                                    {
                                        if (ui.QtytoSaveGd != 0)
                                        {
                                            UpdateLineCounter((int)ui.WfdepinstId, ui.QtytoSaveGd);
                                        }

                                        HourlyCounts hc = new HourlyCounts();
                                        //hc = GetHourlyCounts(ui);

                                        if (hc != null)
                                        {
                                            //ui.CurHrGood = hc.CurHrGood;
                                            //ui.CurHrScrap = hc.CurHrScrap;
                                            //ui.CurHrRework = hc.CurHrRework;

                                            // ui.PrevHrGood = hc.PrevHrGood;
                                            // ui.PrevHrScrap = hc.PrevHrScrap;
                                            // ui.PrevHrRework = hc.PrevHrRework;

                                            ui.TotGood = hc.TotGood;
                                        }
                                        ui.ScanCount = Lukup.GetScanCounterVal(ui.WfdepinstId);
                                    }
                                }
                            }
                            else
                            {
                                ui.Responce[0] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.Responce[1] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                if (bcClrAssigned)
                                {
                                    RemoveColorforBC(ui);
                                }
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }
                        else
                        {
                            ui.Responce[0] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.Responce[1] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            if (bcClrAssigned)
                            {
                                RemoveColorforBC(ui);
                            }
                            lstBCData_Return.Add(ui);
                            continue;
                        }
                        lstBCData_Return.Add(BCData_Return);

                    }
                    else
                    {
                        ui.Responce[0] = "Barcode value is empty. Please Check..";
                        ui.Responce[1] = "Barcode value is empty. Please Check..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                    }
                }
                return lstBCData_Return;
            }
            catch (Exception ex)
            {
                lstBCData_Return[0].Responce = new string[2];

                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].SaveSuccessfull = false;
                return lstBCData_Return;
            }

            logger.InfoFormat("UpdateBCScanDataBFL - End Of UpdateBulkQty API call");

        }

        public List<UserInput> UpdateBCScanDataBFLOutSource([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateBCScanData - UpdateBCScanData API called with WFDEPInstId={0}", lstUserInput);
            bool ValidationPass = false;
            UserInput BCData_Return = new UserInput();
            List<UserInput> lstBCData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap,LogGuid);
            LookupController Lukup = new LookupController(dcap);//,LogGuid);

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();

            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    if (ui.Barcode != "")
                    {
                        int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                            currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                            prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                            prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;
                        bool bcClrAssigned = false, noPreQtyValidation = false;

                        Dedepinst objDedepinst = null;
                        Dedep objDedep = null;
                        Detxn objDetxn = null;

                        Wfdep Wfd = Lukup.GetWFConfigurationbyID(ui.WfdepinstId);
                        objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, Wfd.Depid);

                        ui.WFID = (uint)Wfd.Wfid;
                        ui.Depid = Wfd.Depid;
                        ui.DCLId = Wfd.Dclid;
                        ui.TeamId = Wfd.TeamId;
                        ui.Responce = new string[2];
                        ui.TxnDate = System.DateTime.Now;

                        //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                        if (ui.OperationCode != (int)Wfd.OperationCode)
                        {
                            ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //L5bc l5b = Lukup.GetL5BCData(ui.Barcode);
                        //Check for the exsistence of valid quantity
                        if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)
                        {
                            logger.InfoFormat("UpdateBCScanData - UpdateBulkQty API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);

                            ui.Responce[0] = "Please scan again..";
                            ui.Responce[1] = "Please scan again..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                        }

                        //Check if transaction is Scrap but Reject Reson is null
                        if (ui.EnteredQtyScrap > 0 && string.IsNullOrEmpty(ui.RRId))
                        {
                            ui.Responce[0] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.Responce[1] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Need to check previus scan is rework and now it is good or scrap
                        if (objDetxn != null)
                        {
                            //Check wether that prvious quantitiy is exsists and trying to add quantity for exsisiting record
                            if ((objDetxn.Qty01 + objDetxn.Qty02) >= 1 && (ui.EnteredQtyScrap + ui.EnteredQtyGd) > 0)
                            {
                                ui.Responce[0] = "Barcode is already Used";
                                ui.Responce[1] = "Barcode is already Used";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if ((objDetxn.Qty01 + objDetxn.Qty02) > 0 && ui.EnteredQtyRw > 0)
                            {
                                ui.Responce[0] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.Responce[1] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            #region Qty Reverse

                            if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                            {
                                if (objDetxn.Qty01 == 0)
                                {
                                    ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (objDetxn.Qty01 < (ui.EnteredQtyGd * -1))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }

                            }

                            if (ui.EnteredQtyScrap < 0) //Scrap Qty Reverse
                            {
                                if (objDetxn.Qty02 == 0)
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty02 < (ui.EnteredQtyScrap * -1))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if (ui.EnteredQtyRw < 0) //Rework Qty Reverse
                            {
                                Detxn Detxn2 = null;
                                Detxn2 = Lukup.GetRRIdToReverse(ui.Barcode);
                                if (Detxn2 != null)
                                {
                                    ui.RRId = Detxn2.Rrid.ToString();
                                    ui.DOpsId = Detxn2.DopsId.ToString();
                                }

                                if (objDetxn.Qty03 == 0)
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objDetxn.Qty03 < (ui.EnteredQtyRw * -1))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            #endregion

                        }
                        else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                        {
                            ui.Responce[0] = "No transaction recorded to reverse";
                            ui.Responce[1] = "No transaction recorded to reverse";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Check if the barcode is scanned as a good garment in previous opperation
                        if (Wfd.Bccheck == (int)eBCCheck.DEPLevel || Wfd.Bccheck == (int)eBCCheck.DEPInstLevel) //NA = 0, No = 1, DEPLevel = 2, DEPInstLevel = 3
                        {
                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous); // 2
                            objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, depid[0].Depid);

                            if (objDetxn != null)
                            {
                                if (objDetxn.Qty01 == 1)
                                {
                                    ValidationPass = true;
                                }
                                else
                                {
                                    ValidationPass = false;
                                }
                            }
                        }
                        else
                        {
                            ValidationPass = true;
                        }

                        if (ValidationPass)
                        {
                            if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                            {
                                //Get Reported Qty for Operation
                                objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedep != null)
                                {
                                    currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                    currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                    currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                }

                                //Get Reported Qty for Node
                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                if (objDedepinst != null)
                                {
                                    currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                    currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                    currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                }

                                //Get previous qty based on the configuration
                                if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                                    if (L5moop != null)
                                    {
                                        prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                        if (Wfd.LimitWithWf == (int)eLimitWithWF.NA) // NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                        {
                                            ui.Responce[0] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.Responce[1] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                    {
                                        //ui.Responce[0] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        //ui.Responce[1] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes) //== 1 //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                {
                                    if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                                    {
                                        if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            int PrevDEP = 0;
                                            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);

                                            if (depid != null)
                                            {
                                                objDedep = Lukup.GetQtyByDEPId((uint)depid[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                                PrevDEP = (int)depid[0].Depid;
                                                if (objDedep != null)
                                                {
                                                    prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                    prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                    prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                                }
                                            }
                                            if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst) //==2  // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            List<Wfdep> lstTDepList = null;
                                            lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                            string PrevDEPInst = "";

                                            //TODO
                                            //PSSIBLE TO APLY FOLLOWING QUERARY TO ELIMINATE foreach (Wfdep objdep in lstTDepList)
                                            // select sum(Qty01) , sum(Qty02) , sum(Qty03)  from Dedepinst I
                                            // inner join wfdeplink L on I.WfdepinstId =  L.WfdepinstId
                                            // INNER JOIN  wfdep w ON l.WFDEPInstId = w.WFDEPInstId  
                                            // where   I.L1Id = 116 and  I.L2Id =1 and I.L3Id =0 and I.L4Id = 2 and I.L5Id = 3 
                                            // and  L.WFDEPIdLink =  330 AND L.RecStatus = 1

                                            foreach (Wfdep objdep in lstTDepList)
                                            {
                                                int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                                PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                                objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                                if (objDedepinst != null)
                                                {
                                                    iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                                    iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                                    iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                                }
                                                prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                                prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                                prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                            }

                                            //Change Before Build NimanthaH
                                            //if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                            {
                                                ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }
                                        }
                                        else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                        {
                                            ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                            ui.SaveSuccessfull = false;
                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                                    else
                                    {
                                        ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.SaveSuccessfull = false;
                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))
                                {
                                    objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                    int PrevDEP = (int)Wfd.PredDepid;

                                    //Get Reported Qty for Operation
                                    if (objDedep != null)
                                    {
                                        prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                        prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                        prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                    }

                                    if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                    {
                                        ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No) || (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))  // / NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5      
                                {
                                    noPreQtyValidation = true;
                                }
                                //Stop reversing more than next operation
                                if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                                {
                                    List<Wfdep> lstTDepList = null;
                                    lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                                    prevDEPInstQtyGd = 0;
                                    prevDEPInstQtyScrap = 0;
                                    prevDEPInstQtyRw = 0;
                                    int nxtOppCode = 0;

                                    foreach (Wfdep objdep in lstTDepList)
                                    {
                                        int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;
                                        nxtOppCode = (int)lstTDepList[0].OperationCode;

                                        objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                        if (objDedepinst != null)
                                        {
                                            nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                            nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                            nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                        }

                                        prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                        prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                        prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                                    }
                                    if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                                    {
                                        if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap))
                                        {
                                            if (!Lukup.ValidateBCReverse(ui.Barcode, nxtOppCode))
                                            {
                                                ui.Responce[0] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.Responce[1] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                                ui.SaveSuccessfull = false;
                                                lstBCData_Return.Add(ui);

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                        }
                                    }
                                }

                                //MO Split 
                                lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                                if (lstL5mo != null)
                                {
                                    if (lstL5mo.Count > 1)
                                    {
                                        foreach (L5mo l5mo in lstL5mo)
                                        {
                                            Detxn Detx = null;
                                            Detx = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.StyleId, ui.ScheduleId);

                                            int MoQtyMax = 0;
                                            if (Detx != null)
                                            {
                                                MoQtyMax = (int)Detx.Qty01 + (int)Detx.Qty02;
                                            }
                                            else
                                            {
                                                logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                                ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                                ui.SaveSuccessfull = false;

                                                if (bcClrAssigned)
                                                {
                                                    RemoveColorforBC(ui);
                                                }
                                                lstBCData_Return.Add(ui);
                                                continue;
                                            }

                                            Detxn detx = null;
                                            detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                            int MOQty = 0, MOSQty = 0, MORWQty = 0;

                                            if (detx != null)
                                            {
                                                MOQty = (int)detx.Qty01;
                                                MOSQty = (int)detx.Qty02;
                                                MORWQty = (int)detx.Qty03;
                                            }

                                            if (ui.EnteredQtyGd < 0 || ui.EnteredQtyScrap < 0 || ui.EnteredQtyRw < 0)
                                            {
                                                detx = Lukup.GetReportedQtyforMOBC(ui.WfdepinstId, l5mo.L5mono, ui.Barcode);

                                                if (detx != null)
                                                {
                                                    if (ui.EnteredQtyGd < 0 && MOQty > 0 && detx.Qty01 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyScrap < 0 && MOSQty > 0 && detx.Qty02 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                    else if (ui.EnteredQtyRw < 0 && MORWQty > 0 && detx.Qty03 > 0)
                                                    {
                                                        ui.L5MOID = l5mo.L5moid;
                                                        ui.L5MONo = l5mo.L5mono;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (MoQtyMax > (MOQty + MOSQty))
                                            {
                                                ui.L5MOID = l5mo.L5moid;
                                                ui.L5MONo = l5mo.L5mono;
                                                break;
                                            }
                                        }
                                    }
                                    else if (lstL5mo.Count == 1)
                                    {
                                        ui.L5MOID = lstL5mo[0].L5moid;
                                        ui.L5MONo = lstL5mo[0].L5mono;
                                    }

                                    if (ui.L5MONo != null)
                                    {
                                        ui.QtytoSaveGd = ui.EnteredQtyGd;
                                        ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                        ui.QtytoSaveRw = ui.EnteredQtyRw;

                                        //PO BAG Counter Checker - Added By NimanthaH
                                        if (Wfd.POCounterEnable == 1)
                                        {
                                            if (Wfd.POCounterNumber == 1)
                                            {
                                                BusinessLogicsController bu = new BusinessLogicsController(dcap);
                                                var Response = bu.BuddyBarcodeChecker(ui, true);
                                                if (Response != null)
                                                {
                                                    if (!Response.updated)
                                                    {
                                                        ui.Responce[0] = "Buddy Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                        ui.Responce[1] = "Buddy Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                        ui.NewCounter = false;
                                                        ui.SaveSuccessfull = false;
                                                        lstBCData_Return.Add(ui);
                                                    }
                                                    else
                                                    {
                                                        if (Response.IsUpdateAvilable)
                                                        {
                                                            ui.CounterId = Response.CounterId;
                                                            ui.BagBarCodeNo = Response.BagBarCodeNo;
                                                            ui.RefBagBarCodeNo = Response.RefBagBarCodeNo;
                                                        }

                                                        ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                                        ui.NewCounter = Response.NewCounter;

                                                        BCData_Return = DbUpdateBFL(ui);

                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!Lukup.POLimitCheck(ui))
                                                {
                                                    ui.Responce[0] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.Responce[1] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                    ui.NewCounter = false;
                                                    ui.SaveSuccessfull = false;
                                                    lstBCData_Return.Add(ui);
                                                    continue;
                                                }
                                                else
                                                {
                                                    BCData_Return = DbUpdateBFL(ui);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ui.IsUpdateAvilable = false;
                                            ui.NewCounter = false;

                                            BCData_Return = DbUpdateBFL(ui);
                                        }
                                    }

                                    //po

                                    if (BCData_Return.SaveSuccessfull == true)
                                    //if (true)
                                    {
                                        if (ui.QtytoSaveGd != 0)
                                        {
                                            UpdateLineCounter((int)ui.WfdepinstId, ui.QtytoSaveGd);
                                        }

                                        HourlyCounts hc = new HourlyCounts();
                                        //hc = GetHourlyCounts(ui);

                                        if (hc != null)
                                        {
                                            //ui.CurHrGood = hc.CurHrGood;
                                            //ui.CurHrScrap = hc.CurHrScrap;
                                            //ui.CurHrRework = hc.CurHrRework;

                                            // ui.PrevHrGood = hc.PrevHrGood;
                                            // ui.PrevHrScrap = hc.PrevHrScrap;
                                            // ui.PrevHrRework = hc.PrevHrRework;

                                            ui.TotGood = hc.TotGood;
                                        }
                                        ui.ScanCount = Lukup.GetScanCounterVal(ui.WfdepinstId);
                                    }
                                }
                            }
                            else
                            {
                                ui.Responce[0] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.Responce[1] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                if (bcClrAssigned)
                                {
                                    RemoveColorforBC(ui);
                                }
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }
                        else
                        {
                            ui.Responce[0] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.Responce[1] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            if (bcClrAssigned)
                            {
                                RemoveColorforBC(ui);
                            }
                            lstBCData_Return.Add(ui);
                            continue;
                        }
                        lstBCData_Return.Add(BCData_Return);

                    }
                    else
                    {
                        ui.Responce[0] = "Barcode value is empty. Please Check..";
                        ui.Responce[1] = "Barcode value is empty. Please Check..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                    }
                }
                return lstBCData_Return;
            }
            catch (Exception ex)
            {
                lstBCData_Return[0].Responce = new string[2];

                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].SaveSuccessfull = false;
                return lstBCData_Return;
            }

            logger.InfoFormat("UpdateBCScanData - End Of UpdateBulkQty API call");

        }

        private UserInput BulkUpdateCall(UserInput ui)
        {
            UserInput objUI = new UserInput();

            objUI = DbUpdate(ui);
            if (ui.OperationCode == 200)
            {
                if (objUI.Responce[0].ToString().Contains("Record Saved succefully"))
                {
                    logger.InfoFormat("OperationCode == 200 saved successfully");

                    string RPDT = ui.TxnDate.Year.ToString() + ui.TxnDate.Month.ToString().PadLeft(2, '0') + ui.TxnDate.Day.ToString().PadLeft(2, '0');
                    string RPTM = ui.TxnDate.Hour.ToString().PadLeft(2, '0') + ui.TxnDate.Minute.ToString().PadLeft(2, '0') + ui.TxnDate.Second.ToString().PadLeft(2, '0');

                    string url070 = String.Format("http://colbdxprd.brandixlk.org:63925/m3api-rest/v2/execute/PMS070MI/RptOperation" +
                        "?CONO=200&FACI=" + ui.FacCode + "&MFNO=" + ui.L5MONo + "&OPNO=" + ui.OperationCode + "&MAQA=" + ui.EnteredQtyGd + "&DPLG=CPK01" +
                        "&RPDT=" + RPDT + "&RPTM=" + RPTM + "&DSP1=1&DSP2=1&DSP3=1&DSP4=1&REMK=DCAP" + ui.DetxnKey.ToString() + "");

                    M3DirectAPICall(url070, ui);
                }

                ui.OperationCode = 250;
                objUI = DbUpdate(ui);
                if (objUI.Responce[0].ToString().Contains("Record Saved succefully"))
                {
                    if (ui.OperationCode == 250)
                    {
                        string url050 = String.Format("http://colbdxprd.brandixlk.org:63925/m3api-rest/v2/execute/PMS050MI/RptReceipt" +
                        "?CONO=200&FACI=" + ui.FacCode + "&MFNO=" + ui.L5MONo + "&RPQA=" + ui.EnteredQtyGd + "&DSP1=1&DSP2=1&DSP3=1&DSP4=1&DSP5=1");
                        M3DirectAPICall(url050, ui);
                    }
                }
            }

            objUI.EnteredQtyGd = 0;
            objUI.EnteredQtyScrap = 0;
            objUI.EnteredQtyRw = 0;
            return objUI;
        }

        private UserInput BulkUpdateCallBFL(UserInput ui)
        {
            UserInput objUI = new UserInput();

            objUI = DbUpdateBFL(ui);

            objUI.EnteredQtyGd = 0;
            objUI.EnteredQtyScrap = 0;
            objUI.EnteredQtyRw = 0;
            return objUI;
        }


        //All Datatabase transactions after barcode scan || checked 8-2-2020
        //Detxn Based
        //Used API's and UI : BCScanData
        private UserInput DbUpdate(UserInput ui)
        {
            logger.InfoFormat("BusinessLogicsController DbUpdate API ui={0}", ui);
            LookupController Lukup = new LookupController(dcap);

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);

                try
                {
                    ui.seqIndex = 0;
                    //Add detxn records
                    TxnContrl.AddDETXN(ui);

                    Dedepinst Dedepinst = Lukup.CheckDEDEPInstRecExists(ui.WfdepinstId, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, ui.L5MOID);

                    if (Dedepinst != null)
                    {
                        TxnContrl.UpdateDEDEPInst(ui);
                    }
                    else//new Record insert DEDEPInst 
                    {
                        TxnContrl.AddDEDEPInst(ui);
                    }

                    Dedep objDedep = Lukup.CheckDEDEPRecExists(ui.WFID, ui.Depid, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, ui.L5MOID);

                    if (objDedep != null)
                    {
                        TxnContrl.UpdateDEDEP(ui);
                    }
                    else//new Record
                    {
                        TxnContrl.AddDEDEPPre(ui); //changed
                    }

                    ui.SaveSuccessfull = true;

                    //Counter API's
                    if (ui.NewCounter)
                    { //New counter
                        ui = TxnContrl.AddTeamCounter(ui);
                    }

                    if (ui.SaveSuccessfull)
                    {
                        if (ui.IsUpdateAvilable)
                        { //Update Counter
                            ui.SaveSuccessfull = TxnContrl.UpdatePOCounter(ui.CounterId, 3, ui.EnteredQtyGd, 0, ui.BagBarCodeNo, (int)ui.WfdepinstId, ui.WFID, ui.OperationCode);
                            if (!ui.SaveSuccessfull)
                            {
                                ui.Responce[0] = "The barcode you are trying to update is already dispatched.  ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                                ui.Responce[1] = "The barcode you are trying to update is already dispatched. ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                            }
                        }
                    }

                    //TxnContrl.POCounterIncrement(ui);
                    if (ui.SaveSuccessfull)
                    {
                        trans.Commit(); //trans.Rollback(); //changed 

                        ui.Responce[0] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                        ui.Responce[1] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error Occured in DB update {0}", e.GetBaseException().ToString());
                    ui.Responce[0] = e.ToString();
                    ui.Responce[1] = e.ToString();
                    ui.SaveSuccessfull = false;
                    trans.Rollback();
                }

            }
            return ui;
        }

        //DB update For Bulk API
        private UserInput DbUpdateBFL(UserInput ui)
        {
            logger.InfoFormat("BusinessLogicsController DbUpdate API ui={0}", ui);
            LookupController Lukup = new LookupController(dcap);

            TransactionController TxnContrl = new TransactionController(dcap);

            try
            {
                //Add detxn records
                TxnContrl.AddDETXNBFL(ui);

                Dedepinst Dedepinst = Lukup.CheckDEDEPInstRecExists(ui.WfdepinstId, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, ui.L5MOID);

                if (Dedepinst != null)
                {
                    TxnContrl.UpdateDEDEPInst(ui);
                }
                else//new Record insert DEDEPInst 
                {
                    TxnContrl.AddDEDEPInstBFL(ui);
                }

                Dedep objDedep = Lukup.CheckDEDEPRecExists(ui.WFID, ui.Depid, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, ui.L5MOID);

                if (objDedep != null)
                {
                    TxnContrl.UpdateDEDEP(ui);
                }
                else//new Record
                {
                    TxnContrl.AddDEDEPBFL(ui);
                }


                Boolean ttopeartionupdateavilable = TxnContrl.UpdateTTOpsRecord(ui);

                if (ttopeartionupdateavilable)
                {
                    TxnContrl.AddTTOperation(ui);
                }

                ui.SaveSuccessfull = true;

                //Counter API's
                if (ui.NewCounter)
                { //New counter
                    ui = TxnContrl.AddBuddyTagCounter(ui);
                }

                if (ui.IsUpdateAvilable)
                { //Update Counter
                    ui.SaveSuccessfull = TxnContrl.UpdateBuddyCounter(ui.CounterId, 3, (ui.EnteredQtyScrap + ui.EnteredQtyRw), 0, ui.BagBarCodeNo, (int)ui.WfdepinstId, ui.WFID, ui.OperationCode);
                    if (!ui.SaveSuccessfull)
                    {
                        ui.Responce[0] = "The barcode you are trying to update is already dispatched. ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                        ui.Responce[1] = "The barcode you are trying to update is already dispatched. ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                    }
                }

                //TxnContrl.POCounterIncrement(ui);
                if (ui.SaveSuccessfull)
                {
                    if (ui.QtytoSaveGd != 0)
                    {
                        TxnContrl.UpdateLineCounter((int)ui.WfdepinstId, ui.QtytoSaveGd);
                    }
                    ui.Responce[0] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                    ui.Responce[1] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error Occured in DB update {0}", e.GetBaseException().ToString());
                ui.Responce[0] = e.ToString();
                ui.Responce[1] = e.ToString();
                ui.SaveSuccessfull = false;
            }

            return ui;
        }

        private UserInput DbUpdateForNonApperal(UserInput ui)
        {
            logger.InfoFormat("BusinessLogicsController DbUpdate API ui={0}", ui);
            LookupController Lukup = new LookupController(dcap);

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);

                try
                {
                    //Counter API's
                    if (ui.NewCounter)
                    { //New counter
                        ui = TxnContrl.AddTeamCounterNonApperal(ui);
                    }

                    if (ui.SaveSuccessfull)
                    {
                        if (ui.IsUpdateAvilable)
                        { //Update Counter
                            ui.SaveSuccessfull = TxnContrl.UpdatePOCounterOutSource(ui.CounterId, 3, ui.EnteredQtyGd, 0, ui.BagBarCodeNo, (int)ui.WfdepinstId, ui.WFID, ui.OperationCode);
                            if (!ui.SaveSuccessfull)
                            {
                                ui.Responce[0] = "The barcode you are trying to update is already dispatched.  ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                                ui.Responce[1] = "The barcode you are trying to update is already dispatched. ( Used Bag Barcode Ref. = " + (ui.BagBarCodeNo) + " )";
                            }
                        }
                    }

                    //TxnContrl.POCounterIncrement(ui);
                    if (ui.SaveSuccessfull)
                    {
                        trans.Commit(); //changed trans.Rollback(); //

                        ui.Responce[0] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                        ui.Responce[1] = "Record Saved succefully, Saved Qty = " + (ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw);
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error Occured in DB update {0}", e.GetBaseException().ToString());
                    ui.Responce[0] = e.ToString();
                    ui.Responce[1] = e.ToString();
                    ui.SaveSuccessfull = false;
                    trans.Rollback();
                }

            }
            return ui;
        }

        public bool NextOppBCValidation(int WfdepinstId, string Barcode, LookupController Lukup)
        {

            Detxn objDetxn = null;

            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode(WfdepinstId, (int)eAdjecentNode.Next);
            if (depid != null)
            {
                if (depid.Count > 0)
                {
                    objDetxn = Lukup.GetDETxnOppQtybyBarcode(Barcode, depid[0].Depid);
                    if (objDetxn != null)
                    {
                        if ((objDetxn.Qty01 + objDetxn.Qty02) > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool NextOppTBCValidation(int WfdepinstId, string Barcode, LookupController Lukup)
        {

            TravelStatus TravelStatus = null;

            List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode(WfdepinstId, (int)eAdjecentNode.Next);
            if (depid != null)
            {
                if (depid.Count > 0)
                {
                    TravelStatus = Lukup.GetTravelStatus(Barcode, depid[0].Depid);
                    if (TravelStatus != null)
                    {
                        if ((TravelStatus.Qty01 + TravelStatus.Qty02) > 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void UpdateColorforBC(L5bc l5bc, UserInput UIn)
        {
            TransactionController TxnContrl = new TransactionController(dcap);
            l5bc.L5bcisUsed = 2;

            l5bc.ModifiedBy = UIn.ModifiedBy;
            l5bc.ModifiedMachine = UIn.ModifiedMachine;
            TxnContrl.UpdateColorforBC(l5bc);
        }

        private void RemoveColorforBC(UserInput UIn)
        {
            L5bc l5bc = new L5bc();
            l5bc.BarCodeNo = UIn.Barcode;
            l5bc.L4id = 0;
            l5bc.L5bcisUsed = 0;

            l5bc.ModifiedBy = UIn.ModifiedBy;
            l5bc.ModifiedMachine = UIn.ModifiedMachine;
            TransactionController TxnContrl = new TransactionController(dcap);
            TxnContrl.UpdateColorforBC(l5bc);
        }


        //create new dispatch Request
        [Produces("application/json")]
        [HttpPost("UpdateDedepinstTableError")]
        public List<UserInput> UpdateDedepinstTableError([FromBody] List<Dedepinst> Dedepinst)
        {
            logger.InfoFormat("UpdateDedepinstTableError Dedepinst={0}", Dedepinst);
            List<UserInput> lstBCData_Return = new List<UserInput>();
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    foreach (Dedepinst ui in Dedepinst)
                    {
                        Boolean savesuccessfull = TxnContrl.UpdateDEDEPInstError(ui);
                        if (savesuccessfull)
                        {
                            trans.Commit();
                            UserInput rp = new UserInput();
                            rp.SaveSuccessfull = true;
                            lstBCData_Return.Add(rp);
                        }
                        else
                        {
                            trans.Rollback();
                            UserInput rp = new UserInput();
                            rp.Responce = new string[2];
                            rp.Responce[0] = "Table update failed.";
                            rp.Responce[1] = "Table update failed.";
                            rp.SaveSuccessfull = false;
                            lstBCData_Return.Add(rp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return lstBCData_Return;
        }

        [Produces("application/json")]
        [HttpGet("GetHourlyCounts")]
        public HourlyCounts GetHourlyCounts(UserInput ui)
        {

            logger.InfoFormat("GetProdHourByTeamId API called with TeamId={0}", ui.TeamId);
            HourlyCounts hc = new HourlyCounts();
            HourlyCounts hctemp = new HourlyCounts();
            //LookupController lookup = new LookupController(dcap, ui.guid);
            LookupController lookup = new LookupController(dcap);//, ui.guid);

            try
            {
                // hctemp = lookup.GetCurHourQty(ui);
                // if (hctemp != null)
                // {
                //     hc.CurHrGood = hctemp.CurHrGood;
                //     hc.CurHrScrap = hctemp.CurHrScrap;
                //     hc.CurHrRework = hctemp.CurHrRework;
                // }

                // hctemp = null;
                // hctemp = lookup.GetPrevHourQty(ui,0);
                // if (hctemp != null)
                // {
                //     hc.PrevHrGood = hctemp.PrevHrGood;
                //     hc.PrevHrRework = hctemp.PrevHrRework;
                //     hc.PrevHrScrap = hctemp.PrevHrScrap;
                // }

                // hctemp = lookup.GetTotalGoodQty(ui.TeamId, ui.Depid);
                // if (hctemp != null)
                // {
                //     hc.TotGood = hctemp.TotGood;

                // }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }

            return hc;
        }

        [Produces("application/json")]
        [HttpPost("ResetCounter")]
        public bool ResetCounter(int wfdepinstId)
        {
            try
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                TxnContrl.ResetCounter(wfdepinstId);
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                throw e;
            }
            return true;
            //return HourlyProduction;
        }

        private void UpdateLineCounter(int Wfdepinst, int EnteredQtyGd)
        {
            TransactionController TxnContrl = new TransactionController(dcap);

            TxnContrl.UpdateLineCounter(Wfdepinst, EnteredQtyGd);
        }

        //Update PO Counter -FOR PO COUNTER || Checked 8-2-2020
        //Remove or Reset Details of Team Counterfor Normal PO Counter | Used as substitute for Bag Counter Controllers in PO Mode
        //Used API's and UI : POBtnReset POBtnClear WEB (Barcode) 
        [Produces("application/json")]
        [HttpPost("UpdatePOCounterForPO")]
        public Boolean UpdatePOCounterForPO(int CounterId, int Action, int WfdepinstId)
        {
            //Action = 1 Clear     |       Action = 2 Reset
            Boolean updatesucess = true;

            TransactionController TxnContrl = new TransactionController(dcap);

            try
            {
                if (CounterId != 0)
                {
                    TeamCounter TC = new TeamCounter();
                    TC = dcap.TeamCounter
                            .Where(c => c.CounterId == CounterId && c.WfdepinstId == WfdepinstId)
                            .FirstOrDefault();

                    if (TC != null)
                    {
                        DateTime ModefiedDateTime = TC.ModifiedDateTime;
                        if (Action == 1)
                        {
                            dcap.TeamCounter.Remove(TC);
                        }
                        else if (Action == 2)
                        {
                            TC.Qty01 = 0;
                            TC.ModifiedDateTime = ModefiedDateTime;
                        }
                    }
                }

                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
                updatesucess = false;
            }

            return updatesucess;
        }

        //Start of New Version APIS - FOR BAGS || Checked 8-2-2020
        //Reset Counter - Reomove counter from Team counter table and adding that counter (bag) to group barcode table
        //Used API's and UI : resetTheBag(BagSize, bagbarcode) closethebag Page Barcode
        [Produces("application/json")]
        [HttpPost("UpdatePOCounter")]

        public CommonUserOutputs UpdatePOCounter(int CounterId, int Action, int BagSize, string BagBarcode, int WfdepinstId, uint WFId, int OperationCode, string LocCode)
        {
            CommonUserOutputs Status = new CommonUserOutputs();
            using (var trans = dcap.Database.BeginTransaction())
            {

                TransactionController TxnContrl = new TransactionController(dcap);
                LookupController lookup = new LookupController(dcap);
                try
                {
                    if (Action == 1)
                    {
                        TeamCounter TC = new TeamCounter();
                        TC = dcap.TeamCounter
                            .Where(c => c.CounterId == CounterId && c.BagBarCodeNo == BagBarcode && c.WfdepinstId == WfdepinstId)
                            .FirstOrDefault();

                        if (lookup.SetFlagForMinimumPOQuantity(TC.L1id, TC.L2id, TC.L3id, TC.L4id, 151))
                        {
                            Status.Responce = "The bag you are trying to close is not yet ready to dispatch. (Minimum Order Quantiity Check Failed)";
                            Status.message = true;
                        }
                    }

                    Status.SaveSuccessfull = TxnContrl.UpdatePOCounter(CounterId, Action, 0, BagSize, BagBarcode, WfdepinstId, WFId, OperationCode);

// Code start - Added to save in Good control and Good control Details                    

                    Wfdep wfdep = new Wfdep();
                    wfdep = dcap.Wfdep
                            .Where(c => c.WfdepinstId == WfdepinstId)
                            .FirstOrDefault();
                    
                    Detxn detxn = new Detxn();
                    detxn= dcap.Detxn
                            .Where(b => b.WfdepinstId == WfdepinstId).FirstOrDefault();

                    if(wfdep.ReceiveEnable == 1)
                    {
                        DispatchInput di =new DispatchInput();

                    di.DispatchBarcode= "1234568";//1234
                    di.Type=100;// 0; //$("#newRequestTypeFilter").data("kendoDropDownList").value(),
                    di.Return= 0;// (Requesttype == 300 ? 2 : (Requesttype == 100 ? ( element.Mode == 1 ? 0 : 1 ) : ( element.Mode >= 2 ? 0 : 1 ))),
                    di.WFIdBag= 0;
                    di.DEPIdBag=0;
                    di.SeqBag=0;
                    di.L1idBag=detxn.L1id;// 748; // detxn.L1id;// TC1.L1id;
                    di.L2idBag =Convert.ToUInt32(detxn.L2id ); //12; // Convert.ToUInt32(detxn.L2id );// TC1.L2id;
                    di.L3idBag= Convert.ToUInt32(detxn.L3id);//0;//Convert.ToUInt32(detxn.L3id); // TC1.L3id;
                    di.L4idBag=  Convert.ToUInt32(detxn.L4id);//1;// Convert.ToUInt32(detxn.L4id); //TC1.L4id;
                    di.L5idBag=0;//In good control we should pass l5 - 0// Convert.ToUInt32(detxn.L5id );//TC1.L5id;
                    di.BagBarcode= BagBarcode;
                    di.Wfid= WFId;
                   di.departmentTo=0;
                    di.TxnDateandTime= DateTime.Today;
                    di.TxnMode= 1;//0;
                    di.TxnStatus= 4;// status will update to 5 //need to have >=4
                    di.Qty01= //Convert.ToDecimal( detxn.Qty01); //3
                    di.OperationCode= System.Convert.ToUInt32(OperationCode);//System.Convert.ToUInt32( detxn.OperationCode);
                    di.EnterdBy=detxn.EnteredBy;//"";//detxn.EnteredBy;
                    di.Remark="";
                    di.LocFrom="";
                    di.LocCode=LocCode;//"N23";//N23
                    di.Approver="";
                    di.ApprovalStatus= 0;
                    di.receiverName="";
                    di.receiverEmail="";
                    di.watcherEmail="";
                    di.courierNo="";
                    di.gpRemarks="";
                    di.vehicleNo="";
                    di.ModifiedBy=detxn.ModifiedBy; //"";// detxn.ModifiedBy;
                    di.ModifiedMachine= detxn.ModifiedMachine;//"";// detxn.ModifiedMachine;
                    di.CreatedBy= detxn.CreatedBy;//"";//detxn.CreatedBy;
                    di.CreatedMachine= detxn.CreatedMachine;//"";//detxn.CreatedMachine;
                    
            
            //string controlid="7820203"; int seq=118686;
            
            //if(controlid == "7820203" && seq==118686)
            //{
                TxnContrl.AddGoodControl(di);
                TxnContrl.AddGoodControlDetail(di);
            //}
            //else{
                //TxnContrl.UpdateGoodControlDetails(controlid,seq,"");
                //TxnContrl.UpdateGoodControlDetailsStatus(controlid,seq, 0, 0, "");
                    
            //}

                       // TxnContrl.AddGoodControl(di);
                        //TxnContrl.AddGoodControlDetail(di);
                        ////check if values available then update
                        //TxnContrl.UpdateGoodControlDetails();
                        //TxnContrl.UpdateGoodControlDetailsStatus();
                    }
// Code End - Added to save in Good control and Good control Details                    


                    if (Status.SaveSuccessfull)
                    {
                        trans.Commit();
                    }

                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                    Status.SaveSuccessfull = false;
                    trans.Rollback();
                    throw e;
                }
            }
            return Status;
        }

        //Reset Counter - Reomove counter from Team counter table and adding that counter (bag) to group barcode table
        //Used API's and UI : resetTheBag(BagSize, bagbarcode) closethebag Page Barcode
        [Produces("application/json")]
        [HttpPost("UpdateBuddyCounter")]

        public CommonUserOutputs UpdateBuddyCounter(int CounterId, int Action, int BagSize, string BagBarcode, int WfdepinstId, uint WFId, int OperationCode)
        {
            CommonUserOutputs Status = new CommonUserOutputs();
            using (var trans = dcap.Database.BeginTransaction())
            {

                TransactionController TxnContrl = new TransactionController(dcap);
                LookupController lookup = new LookupController(dcap);
                BuudyTagCounter TC = new BuudyTagCounter();

                try
                {
                    if (Action == 1)
                    {
                        //TC = dcap.BuudyTagCounter
                        //  .Where(c => c.CounterId == CounterId && c.BagBarCodeNo == BagBarcode && c.WfdepinstId == WfdepinstId)
                        //.FirstOrDefault();

                        if (false)//lookup.SetFlagForMinimumPOQuantity(TC.L1id, TC.L2id, TC.L3id, TC.L4id, 151)
                        {
                            Status.Responce = "The bag you are trying to close is not yet ready to dispatch. (Minimum Order Quantiity Check Failed)";
                            Status.message = true;
                        }
                    }

                    Status.SaveSuccessfull = TxnContrl.UpdateBuddyCounter(CounterId, Action, 0, BagSize, BagBarcode, WfdepinstId, WFId, OperationCode);

                    if (Action == 1)
                    {
                        Status.Responce = lookup.GetBuddyBarcodeByBuddyBagBarcode(BagBarcode);
                    }


                    if (Status.SaveSuccessfull)
                    {
                        trans.Commit();
                    }

                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error while retrieving Operation information {0}", e.ToString());
                    Status.SaveSuccessfull = false;
                    trans.Rollback();
                    throw e;
                }
            }
            return Status;
        }

        [Produces("application/json")]
        [HttpPost("M3DirectAPICall")]
        public void M3DirectAPICall(string url, UserInput ui)
        {

            logger.InfoFormat("M3DirectAPICall API Querry String is {0}", url);
            try
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                HttpMessageHandler handler = new HttpClientHandler()
                {
                };

                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(url),
                    Timeout = new TimeSpan(0, 2, 0)
                };

                //log
                httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");

                //This is the key section you were missing    
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("gtnadmin@brandix.com:gtn@12345");
                string val = System.Convert.ToBase64String(plainTextBytes);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + val);

                var method = new HttpMethod("GET");
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                //log response.StatusCode;

                HttpStatusCode satuscode = response.StatusCode;

                if (satuscode == HttpStatusCode.OK)
                {
                    string content = string.Empty;
                    RootObject Root = new RootObject();

                    using (StreamReader stream = new StreamReader(response.Content.ReadAsStreamAsync().Result, System.Text.Encoding.GetEncoding(0)))
                    {
                        content = stream.ReadToEnd();
                        JObject res = (JObject)JsonConvert.DeserializeObject(content);
                        Root = (RootObject)JsonConvert.DeserializeObject<RootObject>(content);
                    }

                    if (Root.nrOfSuccessfullTransactions == 1)
                    {
                        logger.InfoFormat("M3DirectAPICall Sueccsfull");
                        TxnContrl.UpdateStatus(1, Root.results[0].errorCode, Root.results[0].errorMessage, ui.CreatedBy, ui.DetxnKey);

                    }
                    else
                    {
                        logger.InfoFormat("M3DirectAPICall Failed");
                        TxnContrl.UpdateStatus(2, Root.results[0].errorCode, Root.results[0].errorMessage, ui.CreatedBy, ui.DetxnKey);

                    }
                }
                else
                {
                    logger.InfoFormat("M3DirectAPICall Timeout");
                    TxnContrl.UpdateStatus(2, "401", "M3 API Timeout", ui.CreatedBy, ui.DetxnKey);
                }
            }
            catch (Exception ex)
            {
                string exxx = ex.Message;
            }
        }

        //Create New Dispatch Request || checked 8-4-2020
        //Process all bags Status and add nag records to good control table an create details on good control details table
        //Used API's and UI : saverequest() (Dispatch) WEB
        [Produces("application/json")]
        [HttpPost("UpdateDispatch")]
        public List<DispatchInput> UpdateDispatch([FromBody] List<DispatchInput> DispatchInput)
        {
            logger.InfoFormat("UpdateDispatch DispatchInput={0}", DispatchInput);
            LookupController lookup = new LookupController(dcap);

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);

                try
                {
                    if (DispatchInput.Count != 0)
                    {
                        if (true) //lookup.CheckForMinimumPOQuantity(DispatchInput)
                        {
                            String DispatchId = lookup.GetDispatchId();
                            DispatchInput details = null;
                            decimal QtyTotal = 0;

                            foreach (DispatchInput ui in DispatchInput)
                            {
                                if (ui.Type == 300)
                                {
                                    ui.Return = 2;
                                }
                                else
                                {
                                    if (ui.Type == 100)
                                    {
                                        if (ui.TxnMode == 1)
                                        {
                                            if(ui.DEPIdBag == 100) {
                                                ui.Return = 0;
                                            } else {
                                                ui.Return = 1;
                                            }
                                        }
                                        else
                                        {
                                            if(ui.DEPIdBag == 100) {
                                                ui.Return = 1;
                                            } else {
                                                ui.Return = 0;
                                            }
                                        }
                                    }
                                    else if (ui.Type == 200)
                                    {
                                        if (ui.TxnMode == 1)
                                        {
                                            if(ui.DEPIdBag == 100) {
                                                ui.Return = 1;
                                            } else {
                                                ui.Return = 0;
                                            }
                                        }
                                        else
                                        {
                                            if(ui.DEPIdBag == 100) {
                                                ui.Return = 1;
                                            } else {
                                                ui.Return = 0;
                                            }
                                        }
                                    }
                                }

                                if (ui.Return == 1) //Good Control return Status 2 = returned, 1 = Return, 0 = good
                                {
                                    TxnContrl.UpdateGoodControlBarcodeReturnStatus(ui);
                                }

                                ui.DispatchBarcode = DispatchId;
                                TxnContrl.AddGoodControl(ui);
                                TxnContrl.UpdateBarcodeStatus(ui);

                                QtyTotal = QtyTotal + ui.Qty01;

                            }

                            details = DispatchInput[0];
                            details.DispatchBarcode = DispatchId;
                            details.SeqG = (uint)lookup.GetGoodControlDetailsNextSeqNo(details.Wfid, details.departmentTo);
                            details.Qty01 = QtyTotal;
                            TxnContrl.AddGoodControlDetail(details);

                            trans.Commit();
                            DispatchInput[0].SaveSuccessful = true;
                        }
                        else
                        {
                            DispatchInput[0].SaveSuccessful = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return DispatchInput;
        }

        //create new dispatch Request
        [Produces("application/json")]
        [HttpPost("UpdateDispatchStatus")]
        public List<DispatchStatusInput> UpdateDispatchStatus([FromBody] List<DispatchStatusInput> DispatchInput)
        {
            logger.InfoFormat("UpdateDispatchStatus DispatchInput={0}", DispatchInput);
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    foreach (DispatchStatusInput ui in DispatchInput)
                    {
                        DispatchInput details = null;
                        IList<DispatchInput> L1Details = null;

                        L1Details = (from detxn in dcap.GoodControl
                                     join b in dcap.GroupBarcode on new { A = detxn.L1id, B = detxn.L2id, C = detxn.L3id, D = detxn.L4id, E = detxn.L5id, F = detxn.BarCodeNo } equals new { A = b.L1id, B = b.L2id, C = b.L3id, D = b.L4id, E = b.L5id, F = b.BagBarCodeNo }
                                     join g in dcap.GoodControlDetails on new { A = detxn.ControlId, B = detxn.ControlType } equals new { A = g.ControlId, B = g.ControlType }
                                     where g.Seq == (uint)ui.Seq && g.ControlId == ui.ControlId &&
                                     g.Depid == ui.DEPId
                                     select new DispatchInput
                                     {
                                         BagBarcode = b.BagBarCodeNo,
                                         SeqBag = b.Seq, //bag barcode table seq
                                         WFIdBag = b.WFId,
                                         DEPIdBag = b.DEPId,
                                         L1idBag = b.L1id,
                                         L2idBag = b.L2id,
                                         L3idBag = b.L3id,
                                         L4idBag = b.L4id,
                                         L5idBag = b.L5id,
                                         TxnStatus = ui.Status,
                                         SeqG = detxn.Seq,
                                         DispatchBarcode = g.ControlId,
                                         Return = ui.Return,
                                         SeqGD = g.Seq,
                                         EnterdBy = ui.EnteredBy,
                                     }).ToList();

                        foreach (DispatchInput tx in L1Details)
                        {
                            if (tx.Return == 1 && tx.TxnStatus == 4)
                            {
                                tx.TxnStatus = 0;
                                TxnContrl.UpdateBarcodeStatus(tx);
                                tx.TxnStatus = 5;
                                tx.WarLocCode = "Return";
                                TxnContrl.UpdateGoodControlStatus(tx);
                            }
                            else if (tx.Return == 2 && tx.TxnStatus == 4)
                            {
                                tx.TxnStatus = 9;
                                TxnContrl.UpdateBarcodeStatus(tx);
                                tx.TxnStatus = 5;
                                tx.WarLocCode = "Production";
                                TxnContrl.UpdateGoodControlStatus(tx);
                            }
                            else
                            {
                                TxnContrl.UpdateBarcodeStatus(tx);
                                TxnContrl.UpdateGoodControlStatus(tx);
                            }

                            details = tx;
                        }

                        TxnContrl.UpdateGoodControlDetailsStatus(ui.ControlId, ui.Seq, ui.DEPId, details.TxnStatus, ui.EnteredBy);

                        trans.Commit();

                    }
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return DispatchInput;
        }

        //to return added barcodes from removed barcodes - spit and add travel tag
        [Produces("application/json")]
        [HttpPost("GetAddedBarcodesByRemoved")]
        public List<RemoveBarcodeChecker> GetAddedBarcodesByRemoved([FromBody] List<RemoveBarcodeChecker> RemovedBarcodes)
        {
            logger.InfoFormat("GetAddedBarcodesByRemoved RemoveBarcodeChecker={0}", RemovedBarcodes);
            List<RemoveBarcodeChecker> AddedBarcodes = null;
            try
            {
                List<RemoveBarcodeChecker> list1 = null;

                if (RemovedBarcodes.Count != 0)
                {
                    RemoveBarcodeChecker ui = RemovedBarcodes[0];
                    list1 = (from d in dcap.Detxn
                             where d.BagBarCodeNo == ui.BagBarcode && d.L1id == ui.L1idBag && d.L2id == ui.L2idBag && d.L3id == ui.L3idBag && d.L4id == ui.L4idBag && d.TravelBarCodeNo == ""
                             select new RemoveBarcodeChecker
                             {
                                 Seq = d.Seq,
                                 WFId = d.Wfid,
                                 WFDEPInstId = d.WfdepinstId,
                                 DEPId = d.Depid,
                                 Barcode = d.BarCodeNo,
                                 BagBarcode = d.BagBarCodeNo,
                                 L1idBag = (uint)d.L1id,
                                 L2idBag = (uint)d.L2id,
                                 L3idBag = (uint)d.L3id,
                                 L4idBag = (uint)d.L4id,
                                 L5idBag = (uint)d.L5id,
                             }).ToList();

                    HashSet<string> difBarcodes = new HashSet<string>(RemovedBarcodes.Select(s => s.Barcode));

                    AddedBarcodes = list1.Where(m => !difBarcodes.Contains(m.Barcode)).ToList();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return AddedBarcodes;
        }

        //Create Travel Tag : Get Barcodes and allocate them to a travel tag
        //Used API's and UI : saveRequest() WEB (GenerateTravelTag)
        [Produces("application/json")]
        [HttpPost("UpdateTravelTag")]
        public List<TravelBarcodeInputs> UpdateTravelTag([FromBody] List<TravelBarcodeInputs> TravelBarcodeInputs)
        {
            logger.InfoFormat("UpdateTravelTag TravelBarcodeInputs={0}", TravelBarcodeInputs);
            Boolean success = true;
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    LookupController lookup = new LookupController(dcap);
                    String TravelBarcode = lookup.GetTravelBarcode();
                    Wfdep wfdep = new Wfdep();

                    List<GroupBarcodeMapping> TC = null;

                    decimal QtyTotal = 0;
                    foreach (TravelBarcodeInputs ui in TravelBarcodeInputs)
                    {
                        ui.TravelBarCodeNo = TravelBarcode;

                        //Update Group Barcode tables : START
                        wfdep = lookup.GetWFConfigurationbyID(ui.WFDEPInstId);
                        if (wfdep == null)
                        {
                            wfdep.OperationCode = 151;
                        }

                        TravelBarcodeInputs outputa = TxnContrl.UpdateGroupBarcodesStatus(ui);


                        if (outputa.createNewTravelGroup)
                        {
                            TxnContrl.CreateTravelGroup(outputa, 2, (int)wfdep.OperationCode);
                        }

                        if (outputa.updateIndividualbarcodeScan)
                        {
                            TxnContrl.UpdateIndividualBarcodeStatus(outputa);//TxnContrl.UpdateGroupbarcodeMapping(TxnContrl.UpdateIndividualBarcodeStatus(output));
                        }

                        QtyTotal = QtyTotal + ui.Qty01;
                        //Update Group Barcode tables : END

                        //Update Group Barcode Mapping : START
                        TravelBarcodeInputs outputd = TxnContrl.UpdateGroupMappingStatus(ui);

                        if (outputa.createNewTravelMapGroup)
                        {
                            TxnContrl.CreateGroupMap(outputd);
                        }
                        //Update Group Barcode Mapping : END

                        //Update Group Barcode detail table : START
                        if (ui.TxnMode == 1) //Bag Allocation
                        {
                            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();
                            Boolean detailretsucess = false;
                            barcodedetail = TxnContrl.GetBarcodesDetailsByGroupBarcodeDetails(ui);

                            if (barcodedetail.Count == 0)
                            {
                                detailretsucess = false;
                                IList<Detxn> detxnout = lookup.GetBarcodeDetailsofTravelTag(ui.Barcode, ui.TxnMode);

                                foreach (Detxn detxn in detxnout)
                                {
                                    Group_Barcode_Detail dum = new Group_Barcode_Detail();
                                    dum.Seq = 0;
                                    dum.L1id = detxn.L1id;
                                    dum.L2id = detxn.L2id;
                                    dum.L3id = detxn.L3id;
                                    dum.L4id = detxn.L4id;
                                    dum.L5id = detxn.L5id;
                                    dum.L5moid = detxn.L5moid;
                                    dum.L5mono = detxn.L5mono;
                                    dum.BarCodeNo = ui.Barcode;
                                    dum.TxnMode = ui.TxnMode;
                                    dum.Wfid = detxn.Wfid;
                                    dum.WfdepinstId = detxn.WfdepinstId;
                                    dum.Depid = detxn.Depid;
                                    dum.TeamId = detxn.TeamId;
                                    dum.Dclid = detxn.Dclid;
                                    dum.Dcmid = detxn.Dcmid;
                                    dum.RecStatus = 1;

                                    dum.Qty01 = detxn.Qty01;
                                    dum.Qty02 = detxn.Qty02;
                                    dum.Qty03 = detxn.Qty03;
                                    dum.Qty01Ns = detxn.Qty01Ns;
                                    dum.Qty02Ns = detxn.Qty01Ns;
                                    dum.Qty03Ns = detxn.Qty01Ns;

                                    barcodedetail.Add(dum);
                                }
                                detailretsucess = true;
                            }
                            else
                            {
                                detailretsucess = true;
                            }

                            if (barcodedetail.Count != 0 && detailretsucess)
                            {
                                foreach (Group_Barcode_Detail q in barcodedetail)
                                {
                                    Boolean outputb = TxnContrl.UpdateGroupBarcodesDetailStatus((int)q.L1id, (int)q.L2id, (int)q.L3id, (int)q.L4id, (int)q.L5id, (int)q.L5moid, 2, TravelBarcode, (int)ui.Qty01);

                                    if (outputb)
                                    {
                                        TxnContrl.CreateTravelGroupDetail(q, 2, TravelBarcode, ui.CreatedBy, ui.CreatedMachine);
                                    }
                                }
                            }
                        }
                        else //Barcode Allocation
                        {
                            Detxn objdetxn = dcap.Detxn.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BarCodeNo == ui.Barcode && c.BagBarCodeNo == ui.Bag_Barcode).FirstOrDefault();

                            if (objdetxn != null)
                            {
                                if (ui.Bag_Barcode == objdetxn.BagBarCodeNo)
                                {
                                    List<Group_Barcode_Detail> barcodedetail = TxnContrl.GetBarcodesDetailsByDetxn(objdetxn);

                                    if (barcodedetail.Count != 0)
                                    {
                                        foreach (Group_Barcode_Detail q in barcodedetail)
                                        {
                                            Boolean outputc = TxnContrl.UpdateGroupBarcodesDetailStatus((int)q.L1id, (int)q.L2id, (int)q.L3id, (int)q.L4id, (int)q.L5id, (int)q.L5moid, 2, TravelBarcode, 1);

                                            if (outputc)
                                            {
                                                q.Qty01 = 1;
                                                q.Qty02 = 0;
                                                q.Qty03 = 0;
                                                TxnContrl.CreateTravelGroupDetail(q, 2, TravelBarcode, ui.CreatedBy, ui.CreatedMachine);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Update Group Barcode detail table : END
                    }

                    TravelBarcodeInputs details = null;
                    details = TravelBarcodeInputs[0];
                    details.TravelBarCodeNo = TravelBarcode;
                    details.Qty01 = QtyTotal;

                    TxnContrl.CreateTravelGroupDetails(details, 2, (int)wfdep.OperationCode);

                    trans.Commit(); // trans.Rollback(); // 
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return TravelBarcodeInputs;
        }

        //Create Travel Tag : Get Barcodes and allocate them to a travel tag
        //Used API's and UI : saveRequest() WEB (GenerateTravelTag)
        [Produces("application/json")]
        [HttpPost("UpdateTravelTagOutSource")]
        public List<UserInput> UpdateTravelTagOutSource([FromBody] List<TravelBarcodeInputs> TravelBarcodeInputs)
        {
            logger.InfoFormat("UpdateTravelTagOutSource TravelBarcodeInputs={0}", TravelBarcodeInputs);
            Boolean success = true;
            List<UserInput> lstBCData_Return = new List<UserInput>();
            UserInput Return = new UserInput();
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    LookupController lookup = new LookupController(dcap);
                    String TravelBarcode = lookup.GetTravelBarcodeOutSource();
                    Wfdep wfdep = new Wfdep();
                    Wf wfd = new Wf();

                    List<GroupBarcodeMapping> TC = null;

                    //decimal QtyTotal = 0;
                    foreach (TravelBarcodeInputs ui in TravelBarcodeInputs)
                    {
                        ui.TravelBarCodeNo = TravelBarcode;

                        //Update Group Barcode tables : START
                        wfdep = lookup.GetWFConfigurationbyID(ui.WFDEPInstId);
                        if (wfdep == null)
                        {
                            wfdep.OperationCode = 151;
                            wfd = lookup.GetWorkFlowByWfId((int)ui.WFId);
                        }
                        else
                        {
                            wfd = lookup.GetWorkFlowByWfId(wfdep.Wfid);
                        }

                        TravelBarcodeInputs outputa = TxnContrl.UpdateGroupBarcodesStatus(ui);


                        if (outputa.createNewTravelGroup)
                        {
                            TxnContrl.CreateTravelGroupOutSource(outputa, 2, (int)wfdep.OperationCode);
                        }

                        if (outputa.updateIndividualbarcodeScan)
                        {
                            TxnContrl.UpdateIndividualBarcodeStatusOutSource(outputa);//TxnContrl.UpdateGroupbarcodeMapping(TxnContrl.UpdateIndividualBarcodeStatus(output));
                        }

                        //QtyTotal = QtyTotal + ui.Qty01;
                        //Update Group Barcode tables : END

                        //Update Group Barcode Mapping : START
                        TravelBarcodeInputs outputd = TxnContrl.UpdateGroupMappingStatus(ui);

                        if (outputa.createNewTravelMapGroup)
                        {
                            TxnContrl.CreateGroupMap(outputd);
                        }
                        //Update Group Barcode Mapping : END



                        //Update Group Barcode detail table : START
                        if (ui.TxnMode == 1) //Bag Allocation
                        {
                            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();
                            Boolean detailretsucess = false;
                            barcodedetail = TxnContrl.GetBarcodesDetailsByGroupBarcodeDetails(ui);

                            if (barcodedetail.Count == 0)
                            {
                                Group_Barcode_Detail dum = new Group_Barcode_Detail();
                                dum.Seq = 0;
                                dum.L1id = ui.L1id;
                                dum.L2id = ui.L2id;
                                dum.L3id = ui.L3id;
                                dum.L4id = ui.L4id;
                                dum.L5id = ui.L5id;
                                dum.L5moid = (int)ui.L5moid;
                                dum.L5mono = ui.L5mono;
                                dum.BarCodeNo = ui.Barcode;
                                dum.TxnMode = ui.TxnMode;
                                dum.Wfid = ui.WFId;
                                dum.WfdepinstId = ui.WFDEPInstId;
                                dum.Depid = ui.DEPId;
                                dum.TeamId = 0;
                                dum.Dclid = 0;
                                dum.Dcmid = 0;
                                dum.RecStatus = 1;

                                dum.Qty01 = ui.Qty01;
                                dum.Qty02 = ui.Qty02;
                                dum.Qty03 = ui.Qty03;
                                dum.Qty01Ns = 0;
                                dum.Qty02Ns = 0;
                                dum.Qty03Ns = 0;

                                barcodedetail.Add(dum);

                                detailretsucess = true;
                            }
                            else
                            {
                                detailretsucess = true;
                            }

                            if (barcodedetail.Count != 0 && detailretsucess)
                            {
                                foreach (Group_Barcode_Detail q in barcodedetail)
                                {
                                    Boolean outputb = TxnContrl.UpdateGroupBarcodesDetailStatus((int)q.L1id, (int)q.L2id, (int)q.L3id, (int)q.L4id, (int)q.L5id, (int)q.L5moid, 2, TravelBarcode, (int)ui.Qty01);

                                    if (outputb)
                                    {
                                        TxnContrl.CreateTravelGroupDetail(q, 2, TravelBarcode, ui.CreatedBy, ui.CreatedMachine);
                                    }
                                }
                            }
                        }

                        //Update Group Barcode detail table : END
                    }

                    //Update Detxn Record : START
                    List<UserInput> UserNList = new List<UserInput>();
                    UserNList = (from tbi in TravelBarcodeInputs
                                 group tbi by new { tbi.WFId, tbi.WFDEPInstId, tbi.DEPId, tbi.L1id, tbi.L2id, tbi.L3id, tbi.L4id, tbi.L5id, tbi.Seq, tbi.Barcode, tbi.CreatedBy, tbi.CreatedMachine } into grp
                                 select new UserInput
                                 {

                                     BagSeq = grp.Key.Seq,
                                     BagTxnMode = 2,
                                     WfdepinstId = grp.Key.WFDEPInstId,
                                     WFID = grp.Key.WFId,
                                     FacCode = wfd.FacCode,
                                     Sbucode = wfd.Sbucode,
                                     GroupCode = wfd.GroupCode,
                                     StyleId = grp.Key.L1id,
                                     ScheduleId = grp.Key.L2id,
                                     ColorId = grp.Key.L4id,
                                     SizeId = grp.Key.L5id,
                                     SizeDesc = "",

                                     Barcode = grp.Key.Barcode + (grp.Key.L5id.ToString().Length == 1 ? "0" + grp.Key.L5id.ToString() : grp.Key.L5id.ToString()) + TravelBarcode.Substring(8),
                                     RefBagBarCodeNo = grp.Key.Barcode, //TravelBarcode.Substring(4),
                                     BagBarCodeNo = TravelBarcode,
                                     JobNo = "", //TravelBarcode,

                                     ProdScanType = 1, // 0 = bypass 1 = bulkupdate, 2 = scan
                                     ScanType = 0, //ScanType: 0 = group, 1 = individual

                                     EnteredQtyGd = (int)grp.Sum(c => c.Qty01),
                                     TxnMode = 1,
                                     PlussMinus = 1,

                                     TxnDate = DateTime.Now,
                                     HourNo = 0, // Lukup.GetProdHourByTeamId((int)ui.TeamId, DateTime.now(), ui.Offline),

                                     SaveSuccessfull = true,
                                     Remark = "",

                                     CreatedBy = grp.Key.CreatedBy,
                                     CreatedMachine = grp.Key.CreatedMachine,

                                     seqIndex = 0,

                                 }).ToList();


                    if (UserNList.Count != 0)
                    {
                        int seqid = 1;
                        foreach (UserInput u in UserNList)
                        {
                            string seq = seqid.ToString();
                            int len = 2 - seq.Length;
                            if (len >= 0)
                            {
                                int i = 0;
                                while (i < len)
                                {
                                    seq = "0" + seq;
                                    i++;
                                }
                            }
                            u.Barcode = u.Barcode + seq;
                            seqid++;
                        }

                        lstBCData_Return = UpdateBulkQtyBFL(UserNList);
                    }
                    else
                    {
                        Return.Responce = new string[2];
                        Return.Responce[0] = "Barcodes for the selected group barcode is empty.";
                        Return.Responce[1] = "Barcodes for the selected group barcode is empty.";
                        Return.SaveSuccessfull = false;
                        lstBCData_Return.Add(Return);
                    }
                    //Update Detxn Record : END

                    TravelBarcodeInputs details = null;
                    details = TravelBarcodeInputs[0];
                    details.TravelBarCodeNo = TravelBarcode;
                    details.Qty01 = TravelBarcodeInputs.Sum(c => c.Qty01);

                    TxnContrl.CreateTravelGroupDetailsOutSource(details, 2, (int)wfdep.OperationCode);

                    Boolean validate = true;
                    if (lstBCData_Return.Count != 0)
                    {
                        foreach (UserInput ni in lstBCData_Return)
                        {
                            if (validate)
                            {
                                validate = ni.SaveSuccessfull;
                            }
                        }
                        if (validate)
                        {
                            trans.Commit(); //trans.Rollback(); //  
                        }
                        else { trans.Rollback(); }
                    }
                    else
                    {
                        trans.Rollback();
                    }

                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return lstBCData_Return;
        }


        //Create Travel Tag : Get Barcodes and allocate them to a travel tag
        //Used API's and UI : saveRequest() WEB (GenerateTravelTag)
        [Produces("application/json")]
        [HttpPost("ManageTravelTag")]
        public List<TravelBarcodeInputs> ManageTravelTag([FromBody] List<TravelBarcodeInputs> TravelBarcodeInputs)
        {
            logger.InfoFormat("ManageTravelTag TravelBarcodeInputs={0}", TravelBarcodeInputs);
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);

                try
                {
                    LookupController lookup = new LookupController(dcap);
                    List<GroupBarcodeMapping> TC = null;
                    //String PasedTravelBarcode = lookup.GetTravelBarcode();
                    Wfdep wfdep = new Wfdep();

                    decimal QtyTotal = 0;
                    foreach (TravelBarcodeInputs ui in TravelBarcodeInputs)
                    {
                        wfdep = lookup.GetWFConfigurationbyID(ui.WFDEPInstId);
                        if (wfdep == null)
                        {
                            wfdep.OperationCode = 151;
                        }

                        if (ui.TravelBarCodeNo == "")
                        {
                            //ui.TravelBarCodeNo = PasedTravelBarcode;
                        }

                        TravelBarcodeInputs outputa = TxnContrl.ManageGroupBarcodesStatus(ui);

                        if (outputa.createNewTravelGroup)
                        {
                            TxnContrl.CreateTravelGroup(outputa, 2, (int)wfdep.OperationCode);
                        }

                        if (outputa.deleteTravelGroup)
                        {
                            //TxnContrl.CreateDeletedGroup(outputa);
                        }

                        if (outputa.updateIndividualbarcodeScan)
                        {
                            TxnContrl.DeleteIndividualBarcodeStatus(ui);
                        }

                        QtyTotal = QtyTotal + ui.Qty01;
                        //Update Group Barcode tables : END

                        //Update Group Barcode Mapping : START
                        TravelBarcodeInputs outputd = TxnContrl.UpdateGroupMappingStatus(ui);

                        if (outputa.createNewTravelMapGroup)
                        {
                            TxnContrl.CreateGroupMap(outputd);
                        }
                        //Update Group Barcode Mapping : END

                        //Update Group Barcode detail table : START
                        if (ui.TxnMode >= 1) //Bag Allocation
                        {
                            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();
                            Boolean detailretsucess = false;
                            barcodedetail = TxnContrl.UpdateAnyBarcodesDetailsByGroupBarcodeDetails(ui);

                            if (barcodedetail.Count == 0)
                            {
                                detailretsucess = false;
                                IList<Detxn> detxnout = lookup.GetBarcodeDetailsofTravelTag(ui.Barcode, ui.TxnMode);

                                foreach (Detxn detxn in detxnout)
                                {
                                    Group_Barcode_Detail dum = new Group_Barcode_Detail();
                                    dum.Seq = 0;
                                    dum.L1id = detxn.L1id;
                                    dum.L2id = detxn.L2id;
                                    dum.L3id = detxn.L3id;
                                    dum.L4id = detxn.L4id;
                                    dum.L5id = detxn.L5id;
                                    dum.L5moid = detxn.L5moid;
                                    dum.L5mono = detxn.L5mono;
                                    dum.BarCodeNo = ui.Barcode;
                                    dum.TxnMode = ui.TxnMode;
                                    dum.Wfid = detxn.Wfid;
                                    dum.WfdepinstId = detxn.WfdepinstId;
                                    dum.Depid = detxn.Depid;
                                    dum.TeamId = detxn.TeamId;
                                    dum.Dclid = detxn.Dclid;
                                    dum.Dcmid = detxn.Dcmid;
                                    dum.RecStatus = 1;

                                    dum.Qty01 = detxn.Qty01;
                                    dum.Qty02 = detxn.Qty02;
                                    dum.Qty03 = detxn.Qty03;
                                    dum.Qty01Ns = detxn.Qty01Ns;
                                    dum.Qty02Ns = detxn.Qty01Ns;
                                    dum.Qty03Ns = detxn.Qty01Ns;

                                    barcodedetail.Add(dum);
                                }
                                detailretsucess = true;
                            }
                            else
                            {
                                detailretsucess = true;
                            }

                            if (barcodedetail.Count != 0 && detailretsucess)
                            {
                                foreach (Group_Barcode_Detail q in barcodedetail)
                                {
                                    Boolean outputb = TxnContrl.ManageGroupBarcodesDetailStatus((int)q.L1id, (int)q.L2id, (int)q.L3id, (int)q.L4id, (int)q.L5id, (int)q.L5moid, 2, ui.TravelBarCodeNo, (int)ui.Qty01);

                                    if (outputb)
                                    {
                                        TxnContrl.CreateTravelGroupDetail(q, 2, ui.TravelBarCodeNo, ui.CreatedBy, ui.CreatedMachine);
                                    }
                                }
                            }
                        }
                        else //Barcode Allocation
                        {
                            Detxn objdetxn = dcap.Detxn.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BarCodeNo == ui.Barcode && c.BagBarCodeNo == ui.Bag_Barcode).FirstOrDefault();

                            if (objdetxn != null)
                            {
                                if (ui.Bag_Barcode == objdetxn.BagBarCodeNo)
                                {
                                    List<Group_Barcode_Detail> barcodedetail = TxnContrl.GetBarcodesDetailsByDetxn(objdetxn);

                                    if (barcodedetail.Count != 0)
                                    {
                                        foreach (Group_Barcode_Detail q in barcodedetail)
                                        {
                                            Boolean outputc = TxnContrl.ManageGroupBarcodesDetailStatus((int)q.L1id, (int)q.L2id, (int)q.L3id, (int)q.L4id, (int)q.L5id, (int)q.L5moid, 2, ui.TravelBarCodeNo, 1);

                                            if (outputc)
                                            {
                                                q.Qty01 = 1;
                                                q.Qty02 = 0;
                                                q.Qty03 = 0;
                                                TxnContrl.CreateTravelGroupDetail(q, 2, ui.TravelBarCodeNo, ui.CreatedBy, ui.CreatedMachine);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Update Group Barcode detail table : END
                    }

                    TravelBarcodeInputs details = null;
                    details = TravelBarcodeInputs[0];
                    details.Qty01 = QtyTotal;

                    if (QtyTotal < 0)
                    {
                        TxnContrl.ManageTravelGroupDetails(details);
                    }
                    else
                    {
                        TxnContrl.CreateTravelGroupDetails(details, 2, (int)wfdep.OperationCode);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return TravelBarcodeInputs;
        }

        //Create Travel Tag : Get Barcodes and allocate them to a travel tag
        //Used API's and UI : saveRequest() WEB (GenerateTravelTag)
        [Produces("application/json")]
        [HttpPost("UpdateBuddyTag")]
        public List<TravelBarcodeInputs> UpdateBuddyTag([FromBody] List<TravelBarcodeInputs> TravelBarcodeInputs)
        {
            logger.InfoFormat("UpdateBuddyTag RemoveBarcodeChecker={0}", TravelBarcodeInputs.ToString());
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    LookupController lookup = new LookupController(dcap);
                    String TravelBarcode = lookup.GetBuddyBarcode();

                    List<GroupBarcodeMapping> TC = null;

                    decimal QtyTotal = 0;
                    foreach (TravelBarcodeInputs ui in TravelBarcodeInputs)
                    {
                        ui.TravelBarCodeNo = TravelBarcode;

                        TravelBarcodeInputs output = TxnContrl.UpdateBuddyBarcodesStatus(ui);

                        if (output.createNewTravelGroup)
                        {
                            TxnContrl.CreateBuddyGroup(output, 3);
                        }

                        if (output.updateIndividualbarcodeScan)
                        {
                            TxnContrl.UpdateIndividualBuddyBarcodeStatus(output);//TxnContrl.UpdateGroupbarcodeMapping(TxnContrl.UpdateIndividualBarcodeStatus(output));
                        }

                        QtyTotal = QtyTotal + ui.Qty01;
                    }

                    TravelBarcodeInputs details = null;
                    details = TravelBarcodeInputs[0];
                    details.TravelBarCodeNo = TravelBarcode;
                    details.Qty01 = QtyTotal;

                    TxnContrl.CreateBuddyGroupDetails(details, 3);

                    /*
                    //Update Grouping Details
                    List<GroupBarcodeMapping> BagBarcodeFromDirect =  (from inputs in TravelBarcodeInputs
                                                                        where inputs.TxnMode == 1 && inputs.TxnMode != 0
                                                                        group inputs by new
                                                                        {
                                                                            inputs.WFDEPInstId, 
                                                                            inputs.TravelBarCodeNo,
                                                                            inputs.Barcode,

                                                                            inputs.Qty01,
                                                                            inputs.Qty02,
                                                                            inputs.Qty03,
                                                                            inputs.Qty01NS,
                                                                            inputs.Qty02NS,
                                                                            inputs.Qty03NS,
                                                                        }
                                                                    into grp
                                                                        select new GroupBarcodeMapping
                                                                        {
                                                                            WFDEPInstId = grp.Key.WFDEPInstId,
                                                                            MotherBarcode = grp.Key.TravelBarCodeNo,
                                                                            MotherTxnMode = 2,

                                                                            ChildBarcode = grp.Key.Barcode,
                                                                            ChildTxnMode = 1,

                                                                            Qty01 = grp.Key.Qty01,
                                                                            Qty02 = grp.Key.Qty02,
                                                                            Qty03 = grp.Key.Qty03,
                                                                            Qty01NS = grp.Key.Qty01NS,
                                                                            Qty02NS = grp.Key.Qty02NS,
                                                                            Qty03NS = grp.Key.Qty03NS,

                                                                            CreatedBy = TravelBarcodeInputs[0].CreatedBy,
                                                                            CreatedDateTime = TravelBarcodeInputs[0].CreatedDateTime,
                                                                            CreatedMachine = TravelBarcodeInputs[0].CreatedMachine,
                                                                            ModifiedBy = TravelBarcodeInputs[0].ModifiedBy,
                                                                            ModifiedMachine = TravelBarcodeInputs[0].ModifiedMachine,
                                                                            ModifiedDateTime = TravelBarcodeInputs[0].ModifiedDateTime,
                                                                        }).ToList();

                    //List<GroupBarcodeMapping> groupBarcodes = BagBarcodeFromDirect.Concat(BagBarcodeFromInDirect).ToList();
                    
                    foreach(GroupBarcodeMapping broupBarcodeMapping in groupBarcodes) {
                        TxnContrl.UpdateGroupbarcodeMapping(broupBarcodeMapping);
                    }*/

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    trans.Rollback();
                }
            }
            return TravelBarcodeInputs;
        }
        //Bag Barcode Contollers 

        //Bag Barcode Checker : check for bag barcode using barcode number and accordingly create update or delete counters
        //Used API's and UI : UpdateBCScanData
        [Produces("application/json")]
        [HttpPost("BagBarcodeChecker")]
        public TeamCounterOutput BagBarcodeChecker([FromBody] UserInput ui, Boolean ValidationPass)
        {
            logger.InfoFormat("BagBarcodeChecker API called with lstUserInput = {0}", ui, ValidationPass);
            TeamCounterOutput TeamCounter = new TeamCounterOutput();

            TeamCounter.NewCounter = false;
            TeamCounter.updated = false;
            TeamCounter.IsUpdateAvilable = false;

            LookupController lookup = new LookupController(dcap);
            try
            {
// put the condition to check addnew bag if 0 then not to check this function 

                //Get Bag Barcode if assigned to a Barcode in DETXN Table
                var BagBarcodeDetails = lookup.GetBagBarcodeByBarcode(ui.Barcode, (int)ui.StyleId, (int)ui.ScheduleId, 0, (int)ui.ColorId);
                if (BagBarcodeDetails != null)
                {
                    //if there is a asigend Bag Barcode then get if that bag is on ongoing counter
                    TeamCounter counterDetails = lookup.CheckForOngoingBagBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo);

                    if (counterDetails != null)
                    {
                        //Update Counter
                        TeamCounter.CounterId = counterDetails.CounterId;
                        TeamCounter.BagBarCodeNo = BagBarcodeDetails.BagBarCodeNo;

                        //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                        TeamCounter.IsUpdateAvilable = true;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                    else
                    {
                        //Set if the barcode is in abag barcode inventory   
                        TeamCounter groupBarcodeDetails = lookup.CheckForOngoingGroupBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo);
                        if (groupBarcodeDetails != null)
                        {
                            TeamCounter.CounterId = groupBarcodeDetails.CounterId;
                            TeamCounter.BagBarCodeNo = groupBarcodeDetails.BagBarCodeNo;

                            //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                            TeamCounter.IsUpdateAvilable = true;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            TeamCounter.IsUpdateAvilable = false;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = false;
                        }
                    }
                }
                else
                {
                    //CHECK WETHER QUANTITITY IS A PLUS: Because Minus quantiity cannot start or update in a counter (without bagbarcode)
                    if (ui.EnteredQtyGd > 0)
                    {
                        //if there no bag barode records for the barcode then check for ongoing counter in team counter table
                        TeamCounter counterDetails = lookup.CheckForOngoingBagBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, null);
                        if (counterDetails != null)
                        {
                            //there is a counter for this SKU => Update Counter
                            TeamCounter.CounterId = counterDetails.CounterId;
                            TeamCounter.BagBarCodeNo = counterDetails.BagBarCodeNo;

                            //TeamCounter.updated = transaction.UpdatePOCounter(counterDetails.CounterId, 3, ui.EnteredQtyGd, 0, counterDetails.BagBarCodeNo, (int)ui.WFID);

                            TeamCounter.IsUpdateAvilable = true;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            //There is no counter avilabale  procced
                            TeamCounter.IsUpdateAvilable = false;
                            TeamCounter.NewCounter = true;
                            TeamCounter.updated = true;
                        }
                    }
                    else
                    {
                        TeamCounter.IsUpdateAvilable = false;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Bag Barcode Checker {0}", e.ToString());
                throw e;
            }
            return TeamCounter;
        }

        [Produces("application/json")]
        [HttpPost("BagBarcodeCheckerForNonApperal")]
        public TeamCounterOutput BagBarcodeCheckerForNonApperal([FromBody] UserInput ui, Boolean ValidationPass)
        {
            logger.InfoFormat("BagBarcodeCheckerForNonApperal API called with lstUserInput = {0}", ui, ValidationPass);
            TeamCounterOutput TeamCounter = new TeamCounterOutput();

            TeamCounter.NewCounter = false;
            TeamCounter.updated = false;
            TeamCounter.IsUpdateAvilable = false;

            LookupController lookup = new LookupController(dcap);
            try
            {
                //if there is a asigend Bag Barcode then get if that bag is on ongoing counter
                TeamCounter counterDetails = lookup.CheckForOngoingBagBarcodeCounterExsistenceForNonApperal(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, (int)ui.SizeId, ui.Barcode);
                if (counterDetails != null)
                {
                    //Update Counter
                    TeamCounter.CounterId = counterDetails.CounterId;
                    TeamCounter.BagBarCodeNo = counterDetails.BagBarCodeNo;

                    //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                    TeamCounter.IsUpdateAvilable = true;
                    TeamCounter.NewCounter = false;
                    TeamCounter.updated = true;
                }
                else
                {
                    //Set if the barcode is in abag barcode inventory   
                    TeamCounter groupBarcodeDetails = lookup.CheckForOngoingGroupBarcodeCounterExsistenceNoApperal(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, (int)ui.SizeId, ui.Barcode);
                    if (groupBarcodeDetails != null)
                    {
                        TeamCounter.CounterId = groupBarcodeDetails.CounterId;
                        TeamCounter.BagBarCodeNo = groupBarcodeDetails.BagBarCodeNo;

                        //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                        TeamCounter.IsUpdateAvilable = true;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                    else
                    {
                        TeamCounter.IsUpdateAvilable = false;
                        TeamCounter.NewCounter = true;
                        TeamCounter.updated = true;
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Bag Barcode Checker {0}", e.ToString());
                throw e;
            }
            return TeamCounter;
        }


        //Buddy Tag Barcode Checker : check for buddy barcode using barcode number and accordingly create update or delete counters
        //Used API's and UI : UpdateBCScanDataBFL
        [Produces("application/json")]
        [HttpPost("BuddyBarcodeChecker")]
        public TeamCounterOutput BuddyBarcodeChecker([FromBody] UserInput ui, Boolean ValidationPass)
        {
            logger.InfoFormat("BagBarcodeChecker API called with lstUserInput = {0}", ui, ValidationPass);
            TeamCounterOutput TeamCounter = new TeamCounterOutput();

            TeamCounter.NewCounter = false;
            TeamCounter.updated = false;
            TeamCounter.IsUpdateAvilable = false;

            LookupController lookup = new LookupController(dcap);
            try
            {

                //Get Bag Barcode if assigned to a Barcode in DETXN Table
                Boolean prechecker = true;
                if (ui.SMode != 1)
                {
                    prechecker = lookup.CheckForBarcodeReuseinOperation(ui.Barcode, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, ui.OperationCode, 0, ui.EnteredQtyRw, ui.EnteredQtyScrap);
                }

                if (prechecker)
                {
                    var BagBarcodeDetails = lookup.GetBuddyBarcodeByBarcode(ui.Barcode, (int)ui.StyleId, (int)ui.ScheduleId, 0, (int)ui.ColorId, ui.JobNo, ui.RRType, ui.RRId);
                    if (BagBarcodeDetails != null)
                    {
                        //if there is a asigend Bag Barcode then get if that bag is on ongoing counter
                        BuudyTagCounter counterDetails = lookup.CheckForOngoingBuddyBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo, ui.JobNo, ui.RRType, ui.RRId);

                        if (counterDetails != null)
                        {
                            //Update Counter
                            TeamCounter.CounterId = counterDetails.CounterId;
                            TeamCounter.BagBarCodeNo = BagBarcodeDetails.BagBarCodeNo;

                            //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                            TeamCounter.IsUpdateAvilable = true;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            if (ui.SMode != 1)
                            {
                                //Set if the barcode is in abag barcode inventory 
                                BuudyTagCounter groupBarcodeDetails = lookup.CheckForOngoingBuddyGroupBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo, ui.OperationCode);
                                if (groupBarcodeDetails != null)
                                {
                                    TeamCounter.CounterId = groupBarcodeDetails.CounterId;
                                    TeamCounter.BagBarCodeNo = groupBarcodeDetails.BagBarCodeNo;
                                    TeamCounter.RefBagBarCodeNo = groupBarcodeDetails.TravelBarCodeNo;

                                    //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                                    TeamCounter.IsUpdateAvilable = true;
                                    TeamCounter.NewCounter = false;
                                    TeamCounter.updated = true;
                                }
                                else
                                {
                                    TeamCounter.IsUpdateAvilable = false;
                                    TeamCounter.NewCounter = false;
                                    TeamCounter.updated = false;
                                }
                            }
                            else
                            {
                                TeamCounter.IsUpdateAvilable = false;
                                TeamCounter.NewCounter = true;
                                TeamCounter.updated = true;
                            }
                        }
                    }
                    else
                    {
                        //CHECK WETHER QUANTITITY IS A PLUS: Becauseplus quantiity cannot start or update in a counter (without buddybarcode)
                        if (ui.EnteredQtyGd != 0)
                        {
                            TeamCounter.IsUpdateAvilable = false;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            //if there no bag barode records for the barcode then check for ongoing counter in team counter table
                            BuudyTagCounter counterDetails = lookup.CheckForOngoingBuddyBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, null, ui.JobNo, ui.RRType, ui.RRId);
                            if (counterDetails != null)
                            {
                                //there is a counter for this SKU => Update Counter
                                TeamCounter.CounterId = counterDetails.CounterId;
                                TeamCounter.BagBarCodeNo = counterDetails.BagBarCodeNo;

                                //TeamCounter.updated = transaction.UpdatePOCounter(counterDetails.CounterId, 3, ui.EnteredQtyGd, 0, counterDetails.BagBarCodeNo, (int)ui.WFID);

                                TeamCounter.IsUpdateAvilable = true;
                                TeamCounter.NewCounter = false;
                                TeamCounter.updated = true;
                            }
                            else
                            {
                                //There is no counter avilabale  procced
                                TeamCounter.IsUpdateAvilable = false;
                                TeamCounter.NewCounter = true;
                                TeamCounter.updated = true;
                            }
                        }
                    }
                }
                else
                {
                    /*if (ui.SMode == 1)
                    {
                        TeamCounter.IsUpdateAvilable = false;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                    else
                    {*/
                    TeamCounter.IsUpdateAvilable = false;
                    TeamCounter.NewCounter = false;
                    TeamCounter.updated = true;
                    //}
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Bag Barcode Checker {0}", e.ToString());
                throw e;
            }
            return TeamCounter;
        }


        [Produces("application/json")]
        [HttpPost("CreateBagBarcodeandUpdate")]
        public UserInput CreateBagBarcodeandUpdate([FromBody] UserInput ui)
        {
            logger.InfoFormat("CreateBagBarcodeandUpdate UserInput={0}", ui);
            UserInput updatesucess = ui;
            try
            {
                TransactionController transaction = new TransactionController(dcap);
                transaction.AddTeamCounter(ui);
                //transaction.UpdateIndividualBagBarcodeDirect(ui);
            }
            catch (Exception ex)
            {
                string exxx = ex.Message;
                throw ex;
            }
            return updatesucess;
        }

        //Update Dispatch Success Status of Bags : Mark bag as GRN Item
        //Used API's and UI : confirmSuccess(e) WEB Good Recive
        [Produces("application/json")]
        [HttpPost("UpdateDispatchSuccessStatusofBags")]
        public TeamCounterCM UpdateDispatchSuccessStatusofBags([FromBody] TeamCounterCM ui)
        {
            logger.InfoFormat("UpdateDispatchSuccessStatusofBags TeamCounterCM={0}", ui);
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController txncontroller = new TransactionController(dcap);
                LookupController lookup = new LookupController(dcap);
                TeamCounterCM updatesucess = ui;
                try
                {
                    TransactionController transaction = new TransactionController(dcap);
                    transaction.UpdateBagSuccessStatus(ui, true);

                    if (lookup.LookForCloseStatus(ui))
                    {
                        txncontroller.UpdateCloseStatus(ui);
                    }

                    trans.Commit();

                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                }
            }
            return ui;
        }

        //BFL Production Barcode API's
        //Validation in Post Schedule no Enter in Bulk Data upload screen
        [Produces("application/json")]
        [HttpPost("UpdateBFLProductionBCScanData")]
        public List<UserInput> UpdateBFLProductionBCScanData([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateBFLProductionBCScanData - UpdateBFLProductionBCScanData API called with WFDEPInstId={0}", lstUserInput);
            bool ValidationPass = false;
            UserInput BCData_Return = new UserInput();
            List<UserInput> lstBCData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap,LogGuid);
            LookupController Lukup = new LookupController(dcap);//,LogGuid);

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();

            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                         currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                         prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                         prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;
                    bool bcClrAssigned = false, noPreQtyValidation = false;

                    Dedepinst objDedepinst = null;
                    Dedep objDedep = null;
                    Detxn objDetxn = null;

                    Wfdep Wfd = Lukup.GetWFConfigurationbyID(ui.WfdepinstId);
                    objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, Wfd.Depid);

                    ui.WFID = (uint)Wfd.Wfid;
                    ui.Depid = Wfd.Depid;
                    ui.DCLId = Wfd.Dclid;
                    ui.TeamId = Wfd.TeamId;
                    ui.Responce = new string[2];
                    ui.TxnDate = System.DateTime.Now;

                    //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                    if (ui.OperationCode != (int)Wfd.OperationCode)
                    {
                        ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                        ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        continue;
                        //return lstBCData_Return;
                    }

                    //L5bc l5b = Lukup.GetL5BCData(ui.Barcode);
                    if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)
                    {
                        logger.InfoFormat("UpdateBCScanData - UpdateBulkQty API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);

                        ui.Responce[0] = "Please scan again..";
                        ui.Responce[1] = "Please scan again..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        continue;
                        //return lstBCData_Return;
                    }

                    if (ui.EnteredQtyScrap > 0 && string.IsNullOrEmpty(ui.RRId))
                    {
                        ui.Responce[0] = "Please scan again.. Scrap qty received without scrap reason";
                        ui.Responce[1] = "Please scan again.. Scrap qty received without scrap reason";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        continue;
                        //return lstBCData_Return;
                    }

                    //Need to check previus scan is rework and now it is good or scrap
                    if (objDetxn != null)
                    {
                        if ((objDetxn.Qty01 + objDetxn.Qty02) >= 1 && (ui.EnteredQtyScrap + ui.EnteredQtyGd) > 0)
                        {
                            ui.Responce[0] = "Barcode is already Used";
                            ui.Responce[1] = "Barcode is already Used";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        if ((objDetxn.Qty01 + objDetxn.Qty02) > 0 && ui.EnteredQtyRw > 0)
                        {
                            ui.Responce[0] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                            ui.Responce[1] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        #region Qty Reverse

                        if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                        {
                            if (objDetxn.Qty01 == 0)
                            {
                                ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (objDetxn.Qty01 < (ui.EnteredQtyGd * -1))
                            {
                                ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                            {
                                ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                            {
                                ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                            }

                        }

                        if (ui.EnteredQtyScrap < 0) //Scrap Qty Reverse
                        {
                            if (objDetxn.Qty02 == 0)
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            if (objDetxn.Qty02 < (ui.EnteredQtyScrap * -1))
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (!NextOppBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                            {
                                ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }

                        if (ui.EnteredQtyRw < 0) //Rework Qty Reverse
                        {
                            Detxn Detxn2 = null;
                            Detxn2 = Lukup.GetRRIdToReverse(ui.Barcode);
                            if (Detxn2 != null)
                            {
                                ui.RRId = Detxn2.Rrid.ToString();
                                ui.DOpsId = Detxn2.DopsId.ToString();
                            }

                            if (objDetxn.Qty03 == 0)
                            {
                                ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            if (objDetxn.Qty03 < (ui.EnteredQtyRw * -1))
                            {
                                ui.Responce[0] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                ui.Responce[1] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }
                            else if (!Lukup.CheckBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                            {
                                ui.Responce[0] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.Responce[1] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }
                        #endregion

                    }
                    else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                    {
                        ui.Responce[0] = "No transaction recorded to reverse";
                        ui.Responce[1] = "No transaction recorded to reverse";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        continue;
                        //return lstBCData_Return;
                    }

                    //Assign Color for Barcode
                    L5bc l5bc = Lukup.GetL5BCData(ui.Barcode);
                    if (l5bc != null)
                    {
                        ui.StyleId = (uint)l5bc.L1id;
                        ui.ScheduleId = (uint)l5bc.L2id;
                        ui.SizeId = (uint)l5bc.L5id;

                        if (ui.ColorIdUI != 0 && ui.ColorId != 0 && (ui.ColorIdUI != ui.ColorId))
                        {
                            ui.Responce[0] = "Selected Color do not match with barcode color, Pls select correct color";
                            ui.Responce[1] = "Selected Color do not match with barcode color, Pls select correct color";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        if (l5bc.L4id == 0 && ui.ColorIdUI == 0)
                        {
                            ui.Responce[0] = "Color not assign to Barcode, Pls select color";
                            ui.Responce[1] = "Color not assign to Barcode, Pls select color";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }
                        else if (l5bc.L4id != 0 && ui.ColorIdUI == 0) //Color not provided by user & get color from barcode
                        {
                            ui.ColorIdUI = (uint)l5bc.L4id;
                        }

                        if (l5bc.L4id == 0) //Assigning the color for barcode
                        {
                            l5bc.L4id = ui.ColorIdUI;
                            l5bc.BarCodeNo = ui.Barcode;
                            UpdateColorforBC(l5bc, ui);
                            bcClrAssigned = true;
                        }
                        ui.ColorId = ui.ColorIdUI;
                    }
                    else  //Invalid barcode input by user
                    {
                        ui.Responce[0] = "Scanned barcode does not exists";
                        ui.Responce[1] = "Scanned barcode does not exists";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        continue;
                        //return lstBCData_Return;
                    }

                    //Check if the barcode is scanned as a good garment in previous opperation
                    if (Wfd.Bccheck == (int)eBCCheck.DEPLevel || Wfd.Bccheck == (int)eBCCheck.DEPInstLevel) //NA = 0, No = 1, DEPLevel = 2, DEPInstLevel = 3
                    {
                        List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous); // 2
                        objDetxn = Lukup.GetDETxnOppQtybyBarcode(ui.Barcode, depid[0].Depid);

                        if (objDetxn != null)
                        {
                            if (objDetxn.Qty01 == 1)
                            {
                                ValidationPass = true;
                            }
                            else
                            {
                                ValidationPass = false;
                            }
                        }
                    }
                    else
                    {
                        ValidationPass = true;
                    }

                    if (Wfd.POCounterEnable == 1)
                    {
                        BusinessLogicsController bu = new BusinessLogicsController(dcap);
                        var Response = bu.BagBarcodeChecker(ui, ValidationPass);
                        if (Response != null)
                        {
                            if (!Response.updated)
                            {
                                ui.Responce[0] = "PO Counter Issue, Update failed, (barcode can be already used in a bag and bag is dipatched) Please Check..";
                                ui.Responce[1] = "PO Counter Issue, Update failed, (barcode can be already used in a bag and bag is dipatched) Please Check..";
                                ui.NewCounter = false;
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                            else
                            {
                                lstBCData_Return[0].NewCounter = Response.NewCounter;
                            }
                        }
                    }


                    if (ValidationPass)
                    {
                        if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                        {
                            //Get Reported Qty for Operation
                            objDedep = Lukup.GetQtyByDEPId(Wfd.Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                            if (objDedep != null)
                            {
                                currDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                currDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                currDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                            }

                            //Get Reported Qty for Node
                            objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                            if (objDedepinst != null)
                            {
                                currDEPInstQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                currDEPInstQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                currDEPInstQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                            }

                            //Get previous qty based on the configuration
                            if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                            {
                                L5moop = Lukup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                                if (L5moop != null)
                                {
                                    prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                    if (Wfd.LimitWithWf == (int)eLimitWithWF.NA) // NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                    {
                                        ui.Responce[0] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                        ui.Responce[1] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                        ui.SaveSuccessfull = false;

                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }
                                if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                {
                                    //ui.Responce[0] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                    //ui.Responce[1] = "Qty validation failed - You cannot post more than previous externl operation ExOpCode =" + (int)Wfd.ExOpCode;
                                    ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.SaveSuccessfull = false;

                                    if (bcClrAssigned)
                                    {
                                        RemoveColorforBC(ui);
                                    }
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            else if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.Yes) //== 1 //NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                            {
                                if ((Wfd.LimitWithWf == (int)eLimitWithWF.Yes) || (Wfd.LimitWithWf == (int)eLimitWithWF.No)) // NA = 0,No = 1,Yes = 2
                                {
                                    if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEP)//==1  // NA = 0, DEP = 1, DEPInst = 2
                                    {
                                        int PrevDEP = 0;
                                        List<Wfdep> depid = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);

                                        if (depid != null)
                                        {
                                            objDedep = Lukup.GetQtyByDEPId((uint)depid[0].Depid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                            PrevDEP = (int)depid[0].Depid;
                                            if (objDedep != null)
                                            {
                                                prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                                prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                                prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                            }
                                        }
                                        if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                        {
                                            ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                    else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.DEPInst) //==2  // NA = 0, DEP = 1, DEPInst = 2
                                    {
                                        List<Wfdep> lstTDepList = null;
                                        lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Previous);
                                        string PrevDEPInst = "";

                                        //TODO
                                        //PSSIBLE TO APLY FOLLOWING QUERARY TO ELIMINATE foreach (Wfdep objdep in lstTDepList)
                                        // select sum(Qty01) , sum(Qty02) , sum(Qty03)  from Dedepinst I
                                        // inner join wfdeplink L on I.WfdepinstId =  L.WfdepinstId
                                        // INNER JOIN  wfdep w ON l.WFDEPInstId = w.WFDEPInstId  
                                        // where   I.L1Id = 116 and  I.L2Id =1 and I.L3Id =0 and I.L4Id = 2 and I.L5Id = 3 
                                        // and  L.WFDEPIdLink =  330 AND L.RecStatus = 1

                                        foreach (Wfdep objdep in lstTDepList)
                                        {
                                            int iDBQtyGd = 0, iDBQtyScrap = 0, iDBQtyRw = 0;
                                            PrevDEPInst = PrevDEPInst + "," + objdep.WfdepinstId.ToString();

                                            objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                            if (objDedepinst != null)
                                            {
                                                iDBQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                                iDBQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                                iDBQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                            }
                                            prevDEPInstQtyGd = prevDEPInstQtyGd + iDBQtyGd;
                                            prevDEPInstQtyScrap = prevDEPInstQtyScrap + iDBQtyScrap;
                                            prevDEPInstQtyRw = prevDEPInstQtyRw + iDBQtyRw;
                                        }

                                        //Change Before Build NimanthaH
                                        //if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                        if ((prevDEPInstQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap))
                                        {
                                            ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n " + prevDEPInstQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }
                                    }
                                    else if (Wfd.LimitWithLevel == (int)eLimitWithLevel.NA) // NA = 0, DEP = 1, DEPInst = 2
                                    {
                                        ui.Responce[0] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.Responce[1] = "LimitWithLevel cannot be NA in configuration (LimitWithLevel.NA)";
                                        ui.SaveSuccessfull = false;
                                        if (bcClrAssigned)
                                        {
                                            RemoveColorforBC(ui);
                                        }
                                        lstBCData_Return.Add(ui);
                                        continue;
                                    }
                                }// Wfd.LimitWithWf = NA = 0   //,No = 1,Yes = 2
                                else
                                {
                                    ui.Responce[0] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.Responce[1] = "Invalid value in LimitwithWF cannot be NA in configuration (LimitWithLevel.NA)";
                                    ui.SaveSuccessfull = false;
                                    if (bcClrAssigned)
                                    {
                                        RemoveColorforBC(ui);
                                    }
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            else if ((Wfd.LimtWithPredecessor == (uint)eDEPLimtWithPredecessor.PredOpcode))
                            {
                                objDedep = Lukup.GetQtyByDEPId((uint)Wfd.PredDepid, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);
                                int PrevDEP = (int)Wfd.PredDepid;

                                //Get Reported Qty for Operation
                                if (objDedep != null)
                                {
                                    prevDEPQtyGd = objDedep.Qty01 == null ? 0 : (int)objDedep.Qty01;
                                    prevDEPQtyScrap = objDedep.Qty02 == null ? 0 : (int)objDedep.Qty02;
                                    prevDEPQtyRw = objDedep.Qty03 == null ? 0 : (int)objDedep.Qty03;
                                }

                                if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPQtyGd + currDEPQtyScrap))
                                {
                                    ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation  \n " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.SaveSuccessfull = false;

                                    if (bcClrAssigned)
                                    {
                                        RemoveColorforBC(ui);
                                    }
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            else if ((Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.No) ||
                            (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.NA))  // / NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5      
                            {
                                noPreQtyValidation = true;
                            }
                            //Stop reversing more than next operation
                            if ((ui.EnteredQtyGd + ui.EnteredQtyScrap) < 0)
                            {
                                List<Wfdep> lstTDepList = null;
                                lstTDepList = Lukup.GetAdjacentNodesForGivenNode((int)Wfd.WfdepinstId, (int)eAdjecentNode.Next);
                                prevDEPInstQtyGd = 0;
                                prevDEPInstQtyScrap = 0;
                                prevDEPInstQtyRw = 0;
                                int nxtOppCode = 0;

                                foreach (Wfdep objdep in lstTDepList)
                                {
                                    int nxtQtyGd = 0, nxtQtyScrap = 0, nxtQtyRw = 0;
                                    nxtOppCode = (int)lstTDepList[0].OperationCode;

                                    objDedepinst = Lukup.GetQtyByDEPInstId((int)eGetDEPInsDataFor.Size, (uint)objdep.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId);

                                    if (objDedepinst != null)
                                    {
                                        nxtQtyGd = objDedepinst.Qty01 == null ? 0 : (int)objDedepinst.Qty01;
                                        nxtQtyScrap = objDedepinst.Qty02 == null ? 0 : (int)objDedepinst.Qty02;
                                        nxtQtyRw = objDedepinst.Qty03 == null ? 0 : (int)objDedepinst.Qty03;
                                    }

                                    prevDEPInstQtyGd = prevDEPInstQtyGd + nxtQtyGd;
                                    prevDEPInstQtyScrap = prevDEPInstQtyScrap + nxtQtyScrap;
                                    prevDEPInstQtyRw = prevDEPInstQtyRw + nxtQtyRw;
                                }
                                if ((prevDEPInstQtyGd + prevDEPInstQtyScrap) > 0)
                                {
                                    if ((currDEPInstQtyGd + ui.EnteredQtyGd + ui.EnteredQtyScrap) < (prevDEPInstQtyGd + prevDEPInstQtyScrap))
                                    {
                                        if (!Lukup.ValidateBCReverse(ui.Barcode, nxtOppCode))
                                        {
                                            ui.Responce[0] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                            ui.Responce[1] = "You caanot reverse Qty more than next operation reported Qty \n " + prevDEPInstQtyGd + "(Good Qty)" + prevDEPInstQtyScrap + "(Scrap Qty)";
                                            ui.SaveSuccessfull = false;
                                            lstBCData_Return.Add(ui);

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }

                                    }
                                }
                            }

                            //MO Split 
                            lstL5mo = Lukup.GetMODetails(ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);
                            if (lstL5mo != null)
                            {
                                if (lstL5mo.Count > 1)
                                {
                                    foreach (L5mo l5mo in lstL5mo)
                                    {
                                        Detxn Detx = null;
                                        Detx = Lukup.GetMaxQtyforMO(ui.Depid, l5mo.L5mono, ui.StyleId, ui.ScheduleId);

                                        int MoQtyMax = 0;
                                        if (Detx != null)
                                        {
                                            MoQtyMax = (int)Detx.Qty01 + (int)Detx.Qty02;
                                        }
                                        else
                                        {
                                            logger.InfoFormat("MO Splitting- Multiple MO for a size - Previous opetation is not reported for this MO ={0}", l5mo.L5mono);
                                            ui.Responce[0] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                            ui.Responce[1] = "Previous opetation is not reported for this MO : " + l5mo.L5mono;
                                            ui.SaveSuccessfull = false;

                                            if (bcClrAssigned)
                                            {
                                                RemoveColorforBC(ui);
                                            }
                                            lstBCData_Return.Add(ui);
                                            continue;
                                        }

                                        Detxn detx = null;
                                        detx = Lukup.GetReportedQtyforMO(ui.WfdepinstId, l5mo.L5mono);
                                        int MOQty = 0, MOSQty = 0, MORWQty = 0;

                                        if (detx != null)
                                        {
                                            MOQty = (int)detx.Qty01;
                                            MOSQty = (int)detx.Qty02;
                                            MORWQty = (int)detx.Qty03;
                                        }

                                        if (ui.EnteredQtyGd < 0 || ui.EnteredQtyScrap < 0 || ui.EnteredQtyRw < 0)
                                        {
                                            detx = Lukup.GetReportedQtyforMOBC(ui.WfdepinstId, l5mo.L5mono, ui.Barcode);

                                            if (detx != null)
                                            {
                                                if (ui.EnteredQtyGd < 0 && MOQty > 0 && detx.Qty01 > 0)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    break;
                                                }
                                                else if (ui.EnteredQtyScrap < 0 && MOSQty > 0 && detx.Qty02 > 0)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    break;
                                                }
                                                else if (ui.EnteredQtyRw < 0 && MORWQty > 0 && detx.Qty03 > 0)
                                                {
                                                    ui.L5MOID = l5mo.L5moid;
                                                    ui.L5MONo = l5mo.L5mono;
                                                    break;
                                                }
                                            }
                                        }
                                        else if (MoQtyMax > (MOQty + MOSQty))
                                        {
                                            ui.L5MOID = l5mo.L5moid;
                                            ui.L5MONo = l5mo.L5mono;
                                            break;
                                        }
                                    }
                                }
                                else if (lstL5mo.Count == 1)
                                {
                                    ui.L5MOID = lstL5mo[0].L5moid;
                                    ui.L5MONo = lstL5mo[0].L5mono;
                                }

                                if (ui.L5MONo != null)
                                {
                                    ui.QtytoSaveGd = ui.EnteredQtyGd;
                                    ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                    ui.QtytoSaveRw = ui.EnteredQtyRw;

                                    if (Wfd.POCounterEnable == 1)
                                    {
                                        if (Wfd.POCounterNumber == 1)
                                        {
                                            BusinessLogicsController bu = new BusinessLogicsController(dcap);
                                            var Response = bu.BagBarcodeChecker(ui, true);
                                            if (Response != null)
                                            {
                                                if (!Response.updated)
                                                {
                                                    ui.Responce[0] = "PO Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                    ui.Responce[1] = "PO Counter Issue, Checker failed to check, (Response from Barcode Checker is Empty)";
                                                    ui.NewCounter = false;
                                                    ui.SaveSuccessfull = false;
                                                    lstBCData_Return.Add(ui);
                                                }
                                                else
                                                {
                                                    if (Response.IsUpdateAvilable)
                                                    {
                                                        ui.CounterId = Response.CounterId;
                                                        ui.BagBarCodeNo = Response.BagBarCodeNo;
                                                    }

                                                    ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                                    ui.NewCounter = Response.NewCounter;

                                                    BCData_Return = DbUpdate(ui);

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!Lukup.POLimitCheck(ui))
                                            {
                                                ui.Responce[0] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                ui.Responce[1] = "PO Counter Issue, Checker failed to check, (PO Limit Checker API Failed. Limit Excceds)";
                                                ui.NewCounter = false;
                                                ui.SaveSuccessfull = false;
                                                lstBCData_Return.Add(ui);
                                            }
                                            else
                                            {
                                                ui.IsUpdateAvilable = false;
                                                ui.NewCounter = false;

                                                BCData_Return = DbUpdate(ui);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        ui.IsUpdateAvilable = false;
                                        ui.NewCounter = false;

                                        BCData_Return = DbUpdate(ui);
                                    }

                                    ui.QtytoSaveGd = ui.EnteredQtyGd;
                                    ui.QtytoSaveScrap = ui.EnteredQtyScrap;
                                    ui.QtytoSaveRw = ui.EnteredQtyRw;
                                }

                                //po

                                if (BCData_Return.SaveSuccessfull == true)
                                //if (true)
                                {
                                    if (ui.QtytoSaveGd != 0)
                                    {
                                        UpdateLineCounter((int)ui.WfdepinstId, ui.QtytoSaveGd);
                                    }

                                    HourlyCounts hc = new HourlyCounts();
                                    hc = GetHourlyCounts(ui);

                                    if (hc != null)
                                    {
                                        //ui.CurHrGood = hc.CurHrGood;
                                        //ui.CurHrScrap = hc.CurHrScrap;
                                        //ui.CurHrRework = hc.CurHrRework;

                                        // ui.PrevHrGood = hc.PrevHrGood;
                                        // ui.PrevHrScrap = hc.PrevHrScrap;
                                        // ui.PrevHrRework = hc.PrevHrRework;

                                        ui.TotGood = hc.TotGood;
                                    }
                                    ui.ScanCount = Lukup.GetScanCounterVal(ui.WfdepinstId);
                                }
                            }
                        }
                        else
                        {
                            ui.Responce[0] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                            ui.Responce[1] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            if (bcClrAssigned)
                            {
                                RemoveColorforBC(ui);
                            }
                            lstBCData_Return.Add(ui);
                            continue;
                        }
                    }
                    else
                    {
                        ui.Responce[0] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                        ui.Responce[1] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                        if (bcClrAssigned)
                        {
                            RemoveColorforBC(ui);
                        }
                        lstBCData_Return.Add(ui);
                        continue;
                    }
                    lstBCData_Return.Add(BCData_Return);

                    //Check PO counter limit
                    if (ui.SaveSuccessfull)
                    {

                    }
                }
                return lstBCData_Return;
            }
            catch (Exception ex)
            {
                lstBCData_Return[0].Responce = new string[2];

                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].SaveSuccessfull = false;
                return lstBCData_Return;
            }

            logger.InfoFormat("UpdateBCScanData - End Of UpdateBulkQty API call");

        }

        //travel barcode transaction :START
        //Validation in Post Schedule no Enter in Bulk Data upload screen
        [Produces("application/json")]
        [HttpPost("UpdateTBCScanData")]
        public List<UserInput> UpdateTBCScanData([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateTBCScanData - UpdateTBCScanData API called with WFDEPInstId={0}", lstUserInput);
            bool ValidationPass = true;
            UserInput BCData_Return = new UserInput();
            List<UserInput> lstBCData_Return = new List<UserInput>();
            //LookupController Lukup = new LookupController(dcap,LogGuid);
            LookupController Lukup = new LookupController(dcap);//,LogGuid);

            L5moops L5moop = null;
            List<L5mo> lstL5mo = new List<L5mo>();

            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    if (ui.Barcode != "")
                    {
                        int currDEPQtyGd = 0, currDEPQtyScrap = 0, currDEPQtyRw = 0,
                            currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0,
                            prevDEPQtyGd = 0, prevDEPQtyScrap = 0, prevDEPQtyRw = 0,
                            prevDEPInstQtyGd = 0, prevDEPInstQtyScrap = 0, prevDEPInstQtyRw = 0;
                        bool bcClrAssigned = false, noPreQtyValidation = false;

                        Dedepinst objDedepinst = null;
                        Dedep objDedep = null;
                        TravelStatus objTravelStatus = null;

                        Wfdep Wfd = Lukup.GetWFConfigurationbyID(ui.WfdepinstId);
                        objTravelStatus = Lukup.GetTravelStatus(ui.Barcode, Wfd.Depid);

                        ui.WFID = (uint)Wfd.Wfid;
                        ui.Depid = Wfd.Depid;
                        ui.DCLId = Wfd.Dclid;
                        ui.TeamId = Wfd.TeamId;
                        ui.Responce = new string[2];
                        ui.TxnDate = System.DateTime.Now;

                        //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                        if (ui.OperationCode != (int)Wfd.OperationCode)
                        {
                            ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //L5bc l5b = Lukup.GetL5BCData(ui.Barcode);
                        //Check for the exsistence of valid quantity
                        if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)//ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0 changed by NimanthaH
                        {
                            logger.InfoFormat("UpdateTBCScanData API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);

                            ui.Responce[0] = "Please scan again..";
                            ui.Responce[1] = "Please scan again..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                        }

                        //Check if transaction is Scrap but Reject Reson is null
                        if (ui.EnteredQtyScrap > 0 && string.IsNullOrEmpty(ui.RRId))
                        {
                            ui.Responce[0] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.Responce[1] = "Please scan again.. Scrap qty received without scrap reason";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        //Need to check previus scan is rework and now it is good or scrap
                        if (objTravelStatus != null)
                        {
                            //Check wether that prvious quantitiy is exsists and trying to add quantity for exsisiting record
                            if ((objTravelStatus.Qty01 + objTravelStatus.Qty02) >= 1 && (ui.EnteredQtyScrap + ui.EnteredQtyGd) > 0)
                            {
                                ui.Responce[0] = "Barcode is already Used";
                                ui.Responce[1] = "Barcode is already Used";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            if ((objTravelStatus.Qty01 + objTravelStatus.Qty02) > 0 && ui.EnteredQtyRw > 0)
                            {
                                ui.Responce[0] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.Responce[1] = "Good or Scrap Qty is alredy posted, Cannot report rework Qty";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                                //return lstBCData_Return;
                            }

                            #region Qty Reverse

                            if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                            {
                                if (objTravelStatus.Qty01 == 0)
                                {
                                    ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (objTravelStatus.Qty01 < (ui.EnteredQtyGd * -1))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppTBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckTBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }

                            }

                            if (ui.EnteredQtyScrap < 0) //Scrap Qty Reverse
                            {
                                if (objTravelStatus.Qty02 == 0)
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.Responce[1] = "Scrap Qty Reverse - No records to revers, Reported scrap Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objTravelStatus.Qty02 < (ui.EnteredQtyScrap * -1))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!NextOppTBCValidation((int)ui.WfdepinstId, ui.Barcode, Lukup))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty reported in next operation for this Barcode";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckTBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Scrap Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }

                            if (ui.EnteredQtyRw < 0) //Rework Qty Reverse
                            {
                                Detxn Detxn2 = null;
                                Detxn2 = Lukup.GetRRIdToReverse(ui.Barcode);
                                if (Detxn2 != null)
                                {
                                    ui.RRId = Detxn2.Rrid.ToString();
                                    ui.DOpsId = Detxn2.DopsId.ToString();
                                }

                                if (objTravelStatus.Qty03 == 0)
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.Responce[1] = "Rework Qty Reverse - No records to revers, Reported rework Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                if (objTravelStatus.Qty03 < (ui.EnteredQtyRw * -1))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (!Lukup.CheckTBCScanforSameLine((int)ui.WfdepinstId, ui.Barcode))
                                {
                                    ui.Responce[0] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.Responce[1] = "Rework Qty Reverse - You cannot reverse, Good or Scrap Qty is not reported for this Line";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    continue;
                                }
                            }
                            #endregion

                        }
                        else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                        {
                            ui.Responce[0] = "No transaction recorded to reverse";
                            ui.Responce[1] = "No transaction recorded to reverse";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                            //return lstBCData_Return;
                        }

                        if (ValidationPass)
                        {
                            if (Wfd.Wfdepstatus == (uint)eWFDEPStatus.Open) //NA = 0,Open = 1,Close = 2
                            {
                                TransactionController bs = new TransactionController(dcap);
                                bs.AddTravelStatus(ui);
                                ui.SaveSuccessfull = true;
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                            else
                            {
                                ui.Responce[0] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.Responce[1] = "Node is closed for data capture \n Operation Code - " + ui.OperationCode.ToString() + " Or " + ui.OperationCode2 + " Team Id - " + ui.TeamId.ToString();
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                continue;
                            }
                        }
                        else
                        {
                            ui.Responce[0] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.Responce[1] = "Invalid Barcode, This Barcode scanned as scrap garment in previous operation. \n" + "Previous Operation Code - " + ui.OperationCode.ToString();
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            continue;
                        }
                        lstBCData_Return.Add(BCData_Return);

                    }
                    else
                    {
                        ui.Responce[0] = "Barcode value is empty. Please Check..";
                        ui.Responce[1] = "Barcode value is empty. Please Check..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                    }
                }
                return lstBCData_Return;
            }
            catch (Exception ex)
            {
                lstBCData_Return[0].Responce = new string[2];

                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].Responce[0] = ex.ToString();
                lstBCData_Return[0].SaveSuccessfull = false;
                return lstBCData_Return;
            }

            logger.InfoFormat("UpdateBCScanData - End Of UpdateBulkQty API call");

        }


        [Produces("application/json")]
        [HttpPost("UpdateTravelBCScanData")]
        public List<UserInput> UpdateTravelBCScanData([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("UpdateTravelBCScanData - UpdateTravelBCScanData API called with UserInput={0}", lstUserInput.ToString());
            using (var trans = dcap.Database.BeginTransaction())
            {
                bool ValidationPass = true;
                bool vappass = false;
                UserInput BCData_Return = new UserInput();
                List<UserInput> lstBCData_Return = new List<UserInput>();
                LookupController Lukup = new LookupController(dcap);
                TransactionController Transaction = new TransactionController(dcap);
                BusinessLogicsController Business = new BusinessLogicsController(dcap);

                try
                {
                    foreach (UserInput ui in lstUserInput)
                    {
                        if (ui.Barcode != "")
                        {
                            if (Lukup.CheckforOperationInTraveltag(ui.Barcode, ui.OperationCode, ui.ScanType, ui.PlussMinus, (int)ui.WfdepinstId, ui.SMode)) //ScanType: 0 = group, 1 = individual
                            {
                                if (ui.ProdScanType == 1 || ui.ProdScanType == 0) //dispatch 0 = bypass, 1 = bulkupdate, 2 = scan
                                {
                                    Boolean prodscanchecker = true;

                                    if (ui.ProdScanType == 1) // 0 = bypass 1 = bulkupdate, 2 = scan
                                    {
                                        prodscanchecker = Lukup.CheckforMutipleSheduledTags(ui);
                                    }

                                    if (prodscanchecker)
                                    {
                                        if (ui.ScanType != 1) //ScanType: 0 = group, 1 = individual
                                        {
                                            if (true) //Lukup.GetTravelTagUsedDetail(ui.Barcode, ui.OperationCode, ui.PlussMinus))
                                            {
                                                List<UserInput> UserNList = new List<UserInput>();
                                                IList<Detxn> barcodeOutPut = Lukup.GetBarcodeDetailsofTravelTag(ui.Barcode, 2);
                                                if (barcodeOutPut.Count != 0)
                                                {
                                                    int index = 0;
                                                    foreach (Detxn bi in barcodeOutPut)
                                                    {
                                                        if (bi.Qty01 != 0)
                                                        {
                                                            UserInput element = new UserInput();

                                                            element.BagSeq = bi.Seq;
                                                            element.BagTxnMode = bi.TxnMode;
                                                            element.WfdepinstId = ui.WfdepinstId;
                                                            element.WFID = ui.WFID;
                                                            element.FacCode = ui.FacCode;
                                                            element.Sbucode = ui.Sbucode;
                                                            element.GroupCode = ui.GroupCode;
                                                            element.StyleId = bi.L1id;
                                                            element.ScheduleId = (uint)bi.L2id;
                                                            element.ColorId = (uint)bi.L4id;
                                                            element.SizeId = (uint)bi.L5id;
                                                            element.SizeDesc = "";

                                                            //element.BagBarCodeNo = ui.Barcode;
                                                            element.JobNo = ui.Barcode;

                                                            element.ProdScanType = ui.ProdScanType; // 0 = bypass 1 = bulkupdate, 2 = scan
                                                            element.ScanType = ui.ScanType; //ScanType: 0 = group, 1 = individual

                                                            element.EnteredQtyGd = ui.PlussMinus == 1 ? (int)bi.Qty01 : ((-1) * (int)bi.Qty01);
                                                            element.TxnMode = 1;
                                                            element.PlussMinus = ui.PlussMinus;

                                                            element.TxnDate = DateTime.Now;
                                                            element.HourNo = 0; // Lukup.GetProdHourByTeamId((int)ui.TeamId, DateTime.now(), ui.Offline);

                                                            element.SaveSuccessfull = true;
                                                            element.Remark = "";

                                                            element.CreatedBy = ui.CreatedBy;
                                                            element.CreatedMachine = ui.CreatedMachine;

                                                            element.seqIndex = index;

                                                            UserNList.Add(element);
                                                            index++;
                                                        }
                                                    }

                                                    if (UserNList.Count != 0)
                                                    {
                                                        lstBCData_Return = Business.UpdateBulkQtyBFL(UserNList);
                                                        Transaction.UpdateQtyofTravelTag(lstBCData_Return, ui.Barcode, ui.ProdScanType, ui.ScanType); //ScanType: 0 = group, 1 = individual // 0 = bypass 1 = bulkupdate, 2 = scan
                                                        Transaction.UpdateTravelTagTravelStatus(ui.OperationCode, ui.Barcode, ui.PlussMinus, ui.WfdepinstId);
                                                        //Transaction.AddTTOpsRecord(UserNList[0], ui.Barcode, 2, ui.WfdepinstId, ui.PlussMinus);
                                                    }
                                                    else
                                                    {
                                                        ui.Responce = new string[2];
                                                        ui.Responce[0] = "Barcodes for the selected group barcode is empty.";
                                                        ui.Responce[1] = "Barcodes for the selected group barcode is empty.";
                                                        ui.SaveSuccessfull = false;
                                                        vappass = true;
                                                        lstBCData_Return.Add(ui);
                                                    }
                                                }
                                                else
                                                {
                                                    ui.Responce = new string[2];
                                                    ui.Responce[0] = "Tag is already used. (Return Array from Transaction table is empty.)";
                                                    ui.Responce[1] = "Tag is already used. (Return Array from Transaction table is empty.)";
                                                    ui.SaveSuccessfull = false;
                                                    vappass = true;
                                                    lstBCData_Return.Add(ui);
                                                }
                                            }
                                            else
                                            {
                                                ui.Responce = new string[2];
                                                ui.Responce[0] = "Tag is already used. (Previous operation quantitiy was found.)";
                                                ui.Responce[1] = "Tag is already used. (Previous operation quantitiy was found.)";
                                                ui.SaveSuccessfull = false;
                                                vappass = true;
                                                lstBCData_Return.Add(ui);
                                            }
                                        }
                                        else
                                        {
                                            var travelcode = "";
                                            Boolean tpass = true;
                                            if (ui.SMode == 1)
                                            {
                                                string trbarcode = Lukup.GetBarcodebytravelBarcodeOutsource(ui.Barcode, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, (int)ui.SizeId);
                                                if (trbarcode == null)
                                                {
                                                    trans.Rollback();
                                                    ui.Responce = new string[2];
                                                    ui.Responce[0] = "Travel Tag (" + ui.Barcode + ") - Cannot find any garments form the selected size and travel tag..";
                                                    ui.Responce[1] = "Travel Tag (" + ui.Barcode + ") - Cannot find any garments form the selected size and travel tag..";
                                                    ui.SaveSuccessfull = false;
                                                    vappass = true;
                                                    tpass = false;
                                                    lstBCData_Return.Add(ui);
                                                }
                                                else
                                                {
                                                    travelcode = ui.Barcode;
                                                    lstUserInput.ForEach(x => x.Barcode = trbarcode);
                                                }
                                            }
                                            else
                                            {
                                                travelcode = Lukup.GetTravelTagbyBarcode(ui.Barcode);
                                            }

                                            if (travelcode != "" && tpass)
                                            {
                                                lstUserInput.ForEach(x => x.JobNo = travelcode);
                                                var groupbarcodedetail = Lukup.GetTravelTagGroupeDetailbyBarcode(ui);
                                                if (groupbarcodedetail != null)
                                                {
                                                    lstUserInput.ForEach(x => x.BagSeq = groupbarcodedetail.Seq);
                                                    lstUserInput.ForEach(x => x.BagTxnMode = groupbarcodedetail.TxnMode);
                                                }
                                                else
                                                {
                                                    lstUserInput.ForEach(x => x.BagSeq = 100);
                                                    lstUserInput.ForEach(x => x.BagTxnMode = 2);
                                                }

                                                if (ui.SMode == 1)
                                                {
                                                    lstBCData_Return = Business.UpdateBCScanDataBFLOutSource(lstUserInput);
                                                }
                                                else
                                                {
                                                    lstBCData_Return = Business.UpdateBCScanDataBFL(lstUserInput);
                                                }
                                                //lstUserInput.ForEach(x => x.BagBarCodeNo = travelcode);
                                                Transaction.UpdateQtyofTravelTag(lstBCData_Return, travelcode, ui.ProdScanType, ui.ScanType); //ScanType: 0 = group, 1 = individual // 0 = bypass 1 = bulkupdate, 2 = scan
                                                //Transaction.UpdateTravelTagTravelStatus(ui.OperationCode, ui.Barcode, ui.PlussMinus, ui.WfdepinstId);
                                            }
                                            else
                                            {
                                                if (tpass)
                                                {
                                                    trans.Rollback();
                                                    ui.Responce = new string[2];
                                                    ui.Responce[0] = "Barcode (" + ui.Barcode + ") - Travel barcode validation error..";
                                                    ui.Responce[1] = "Barcode (" + ui.Barcode + ") - Travel barcode validation error..";
                                                    ui.SaveSuccessfull = false;
                                                    vappass = true;
                                                    lstBCData_Return.Add(ui);

                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ui.Responce = new string[2];
                                        ui.Responce[0] = "Tag contains multiple shedules. please select SCAN option and continue";
                                        ui.Responce[1] = "Tag contains multiple shedules. please select SCAN option and continue";
                                        ui.SaveSuccessfull = false;
                                        vappass = true;
                                        lstBCData_Return.Add(ui);
                                    }
                                }
                                else
                                {
                                    if (ui.ScanType != 1) //ScanType: 0 = group, 1 = individual
                                    {
                                        String FailedBarcodes = Lukup.CheckForSingleScanedBarcode(ui.Barcode, ui.TxnMode, ui.OperationCode);
                                        if (FailedBarcodes == "")
                                        {
                                            IList<Detxn> barcodeOutPut = Lukup.GetBarcodeDetailsofTravelTag(ui.Barcode, 2);
                                            List<UserInput> UserNList = new List<UserInput>();
                                            if (barcodeOutPut.Count != 0)
                                            {
                                                int index = 0;
                                                foreach (Detxn bi in barcodeOutPut)
                                                {
                                                    UserInput element = new UserInput();

                                                    element.BagSeq = bi.Seq;
                                                    element.WfdepinstId = ui.WfdepinstId;
                                                    element.WFID = ui.WFID;
                                                    element.FacCode = ui.FacCode;
                                                    element.Sbucode = ui.Sbucode;
                                                    element.GroupCode = ui.GroupCode;
                                                    element.StyleId = bi.L1id;
                                                    element.ScheduleId = (uint)bi.L2id;
                                                    element.ColorId = (uint)bi.L4id;
                                                    element.SizeId = (uint)bi.L5id;
                                                    element.SizeDesc = "";

                                                    element.BagBarCodeNo = ui.Barcode;
                                                    element.JobNo = ui.Barcode;

                                                    element.ProdScanType = ui.ProdScanType; // 0 = bypass 1 = bulkupdate, 2 = scan
                                                    element.ScanType = ui.ScanType; //ScanType: 0 = group, 1 = individual

                                                    element.EnteredQtyGd = ui.PlussMinus == 1 ? (int)bi.Qty01 : ((-1) * (int)bi.Qty01);
                                                    element.TxnMode = 1;
                                                    element.PlussMinus = ui.PlussMinus;

                                                    element.TxnDate = DateTime.Now;
                                                    element.HourNo = 0;

                                                    element.SaveSuccessfull = true;
                                                    element.Remark = "";

                                                    element.CreatedBy = ui.CreatedBy;
                                                    element.CreatedMachine = ui.CreatedMachine;

                                                    element.seqIndex = index;

                                                    UserNList.Add(element);
                                                    index++;
                                                }

                                                if (UserNList.Count != 0)
                                                {
                                                    Transaction.UpdateQtyofTravelTag(UserNList, ui.Barcode, ui.ProdScanType, ui.ScanType); //ScanType: 0 = group, 1 = individual // 0 = bypass 1 = bulkupdate, 2 = scan
                                                    Transaction.UpdateTravelTagTravelStatus(ui.OperationCode, ui.Barcode, ui.PlussMinus, ui.WfdepinstId);
                                                    //Transaction.AddTTOpsRecord(UserNList[0], ui.Barcode, 2, ui.WfdepinstId, ui.PlussMinus);
                                                    lstBCData_Return = UserNList;
                                                }
                                                else
                                                {
                                                    ui.Responce = new string[2];
                                                    ui.Responce[0] = "Barcodes for the selected group barcode is empty.";
                                                    ui.Responce[1] = "Barcodes for the selected group barcode is empty.";
                                                    ui.SaveSuccessfull = false;
                                                    vappass = true;
                                                    lstBCData_Return.Add(ui);
                                                }
                                            }
                                            else
                                            {
                                                ui.Responce = new string[2];
                                                ui.Responce[0] = "Tag is already used. (Return Array from Transaction table is empty.)";
                                                ui.Responce[1] = "Tag is already used. (Return Array from Transaction table is empty.)";
                                                ui.SaveSuccessfull = false;
                                                vappass = true;
                                                lstBCData_Return.Add(ui);
                                            }
                                        }
                                        else
                                        {
                                            ui.Responce = new string[2];
                                            ui.Responce[0] = "Tag is not ready to dispatch as single barcodes. (Garments are pending to scan) Pending Barcodes: " + FailedBarcodes.ToString();
                                            ui.Responce[1] = "Tag is not ready to dispatch as single barcodes. (Garments are pending to scan) Pending Barcodes: " + FailedBarcodes.ToString();
                                            ui.SaveSuccessfull = false;
                                            vappass = true;
                                            lstBCData_Return.Add(ui);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                trans.Rollback();
                                ui.Responce = new string[2];
                                ui.Responce[0] = "Barcode (" + ui.Barcode + ") is not ready to report in this opeartion in selected Action and Mode (Navigate Barcode Detail Page and see the Status for more details)..";
                                ui.Responce[1] = "Barcode (" + ui.Barcode + ") is not ready to report in this opeartion in selected Action and Mode (Navigate Barcode Detail Page and see the Status for more details)..";
                                ui.SaveSuccessfull = false;
                                vappass = true;
                                lstBCData_Return.Add(ui);
                            }
                        }
                        else
                        {
                            ui.Responce = new string[2];
                            ui.Responce[0] = "Barcode value is empty. Please Check..";
                            ui.Responce[1] = "Barcode value is empty. Please Check..";
                            ui.SaveSuccessfull = false;
                            vappass = true;
                            lstBCData_Return.Add(ui);
                        }
                    }

                    Boolean validate = true;
                    if (lstBCData_Return.Count != 0)
                    {
                        foreach (UserInput ni in lstBCData_Return)
                        {
                            if (validate)
                            {
                                validate = ni.SaveSuccessfull;
                            }
                        }
                        if (validate)
                        {
                            trans.Commit(); // trans.Rollback(); // 
                        }
                        else { if (!vappass) { trans.Rollback(); } }
                    }
                    else
                    {
                        trans.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    lstBCData_Return[0].Responce = new string[2];

                    lstBCData_Return[0].Responce[0] = ex.ToString();
                    lstBCData_Return[0].Responce[0] = ex.ToString();
                    lstBCData_Return[0].SaveSuccessfull = false;
                    return lstBCData_Return;
                }

                return lstBCData_Return;
                logger.InfoFormat("UpdateBCScanData - End Of UpdateBulkQty API call");
            }

        }

        [Produces("application/json")]
        [HttpGet("ManageBarcodeFromDispatch")]
        public Boolean ManageBarcodeFromDispatch(int mode, string travelbarcode, int txnmode, int OpCode, int txnstatus)
        {
            logger.InfoFormat("ManageBarcodeFromDispatch mode ={0}, travelcode={1} txnmode={2}, OpCode={3}, txnstatus={4}", mode, travelbarcode, txnmode, OpCode, txnstatus);
            Boolean ReturnData = true;
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    if (mode == 1)
                    {
                        TxnContrl.ReleaseBarcodeFromDispatch(travelbarcode, txnmode, OpCode);
                    }
                    else
                    {
                        TxnContrl.ReverseReleaseBarcodeFromDispatch(travelbarcode, txnmode, txnstatus);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    string exxx = ex.Message;
                    throw ex;
                }
            }
            return ReturnData;
        }

        //travel barcode transaction :End

        //Update Wash Details: START
        [Produces("application/json")]
        [HttpPost("UpdateWashDetails")]
        public IList<WashDetailUpdateInputs> UpdateWashDetails([FromBody] List<WashDetailUpdateInputs> lstUserInput)
        {
            logger.InfoFormat("UpdateWashDetails WashDetailUpdateInputs={0}", lstUserInput);
            IList<WashDetailUpdateInputs> ReturnData = null;
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                try
                {
                    foreach (WashDetailUpdateInputs ui in lstUserInput)
                    {
                        TxnContrl.UpdateWashDetails(ui);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    string exxx = ex.Message;
                    throw ex;
                }
            }
            return ReturnData;
        }
        //Update Wash Details: END

        [Produces("application/json")]
        [HttpPost("ManageInvoiceParameters")]
        public List<InvoiceParameterInput> ManageInvoiceParameters([FromBody] List<InvoiceParameterInput> lstUserInput)
        {
            logger.InfoFormat("UpdateWashDetails InvoiceParameter={0}", lstUserInput);
            List<InvoiceParameterInput> ReturnData = new List<InvoiceParameterInput>();
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                Boolean successstatus = true;
                try
                {
                    foreach (InvoiceParameterInput ui in lstUserInput)
                    {
                        InvoiceParameterInput output = TxnContrl.UpdateInvoiveParametrs(ui);
                        if (output.SaveSuccessfull == false) { successstatus = false; }
                        ReturnData.Add(output);
                    }

                    if (successstatus)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        ReturnData[0].SaveSuccessfull = false;
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    string exxx = ex.Message;
                    throw ex;
                }
            }
            return ReturnData;
        }
        //Update Wash Details: END

        //Create Invoice: STRAT

        //Create New Invoice Request || checked 8-27-2020
        //Used API's and UI : createInvoice() (Invoice) WEB Backup
        [Produces("application/json")]
        [HttpPost("CreateInvoiceBackup")]
        public List<InvoiceInputs> CreateInvoiceBackup([FromBody] List<InvoiceInputs> InvoiceInputs)
        {
            logger.InfoFormat("UpdateDispatch InvoiceInputs={0}", InvoiceInputs.ToString());
            LookupController lookup = new LookupController(dcap);
            List<InvoiceInputs> DataReturn = new List<InvoiceInputs>();
            InvoiceInputs hi = new InvoiceInputs();
            GoodControl si = new GoodControl();

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);

                try
                {
                    if (InvoiceInputs.Count != 0)
                    {
                        InvoiceParameter InvoicePara = lookup.GetInvoiceNumber();
                        if (InvoicePara != null)
                        {
                            InvoiceHeaderInformation details = new InvoiceHeaderInformation();

                            foreach (InvoiceInputs di in InvoiceInputs)
                            {
                                di.InvoiceNumber = InvoicePara.NextInvoiceNo;
                                decimal? QtyTotal = 0;
                                decimal TotalPrice = 0;

                                List<GoodControl> objgoodcontrol = lookup.GetBarcodeDetailsbyControlId(di.ControlId, di.Seq);
                                if (objgoodcontrol.Count != 0)
                                {
                                    var govalidator = true;
                                    foreach (GoodControl ui in objgoodcontrol)
                                    {
                                        decimal Price = (decimal)(ui.Qty01 * lookup.GetPricebySKU(ui.L1id, ui.L2id, ui.L3id, ui.L4id));
                                        if (govalidator)
                                        {
                                            if (false) //Price == 0
                                            {
                                                di.SaveSuccessfull = false;
                                                di.Responce = new string[2];
                                                di.Responce[0] = "Price is '0'...Price update failed ( L1: " + ui.L1id + " | L2: " + ui.L2id + " | L3: " + ui.L3id + " | L4: " + ui.L4id;
                                                di.Responce[1] = "Price is '0'...Price update failed ( L1: " + ui.L1id + " | L2: " + ui.L2id + " | L3: " + ui.L3id + " | L4: " + ui.L4id;
                                                DataReturn.Add(di);
                                                govalidator = false;
                                            }
                                            else
                                            {
                                                ui.CreatedBy = di.CreatedBy;
                                                ui.CreatedMachine = di.CreatedMachine;
                                                ui.ModifiedBy = di.ModifiedBy;
                                                ui.ModifiedMachine = di.ModifiedMachine;

                                                TxnContrl.AddInvoiceDetail(ui, Price, InvoicePara.NextInvoiceNo, 1);
                                                QtyTotal = QtyTotal + ui.Qty01;
                                                TotalPrice = TotalPrice + Price;
                                            }
                                        }
                                    }

                                    InvoiceHeaderInformation objinvoicedetails = new InvoiceHeaderInformation();
                                    if (govalidator)
                                    {
                                        objinvoicedetails.InvoiceNo = Convert.ToString(InvoicePara.NextInvoiceNo);
                                        objinvoicedetails.VAT = InvoicePara.VAT;
                                        objinvoicedetails.NBT = InvoicePara.NBT;
                                        objinvoicedetails.ExchangeRate = InvoicePara.ExchangeRate;
                                        objinvoicedetails.TotalQty = (decimal)QtyTotal;
                                        objinvoicedetails.TotalPrice = TotalPrice;
                                        objinvoicedetails.TotalPriceIncludingVAT = TotalPrice;

                                        objinvoicedetails.CreatedBy = di.CreatedBy;
                                        objinvoicedetails.CreatedMachine = di.CreatedMachine;
                                        objinvoicedetails.ModifiedMachine = di.ModifiedMachine;
                                        objinvoicedetails.ModifiedBy = di.ModifiedBy;

                                        TxnContrl.AddInvoiceHeaderDetail(objinvoicedetails);
                                        TxnContrl.UpdateGoodControlDetails(di.ControlId, di.Seq, objinvoicedetails.InvoiceNo);
                                        TxnContrl.UpdateNextInvoiceNumber();


                                        di.SaveSuccessfull = true;
                                        DataReturn.Add(di);
                                    }
                                }
                                else
                                {
                                    di.Responce = new string[2];
                                    di.Responce[0] = "Return array for dispatch id is empty..";
                                    di.Responce[1] = "Return array for dispatch id is empty..";
                                    di.SaveSuccessfull = false;
                                    DataReturn.Add(di);
                                }

                            }
                        }
                        else
                        {
                            hi.Responce = new string[2];
                            hi.Responce[0] = "No parametres were found for this date..";
                            hi.Responce[1] = "No parametres were found for this date..";
                            hi.SaveSuccessfull = false;
                            DataReturn.Add(hi);
                        }
                    }
                    else
                    {
                        hi.SaveSuccessfull = false;
                        hi.Responce = new string[2];
                        hi.Responce[0] = "User input array is empty..";
                        hi.Responce[1] = "User input array is empty..";
                        DataReturn.Add(hi);
                    }

                    Boolean validate = true;
                    foreach (InvoiceInputs ni in DataReturn)
                    {
                        if (validate) { validate = ni.SaveSuccessfull; }
                    }

                    if (validate)
                    {
                        trans.Commit();
                    }
                    else { trans.Rollback(); }
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    hi.SaveSuccessfull = false;
                    hi.Responce = new string[2];
                    hi.Responce[0] = exxx;
                    hi.Responce[1] = exxx;
                    DataReturn.Add(hi);
                    trans.Rollback();
                }
            }
            return DataReturn;
        }

        //Create New Invoice Request || checked 8-27-2020
        //Used API's and UI : createInvoice() (Invoice) WEB
        [Produces("application/json")]
        [HttpPost("CreateInvoice")]
        public List<InvoiceInputs> CreateInvoice([FromBody] List<InvoiceInputs> InvoiceInputs)
        {
            logger.InfoFormat("UpdateDispatch InvoiceInputs={0}", InvoiceInputs.ToString());
            LookupController lookup = new LookupController(dcap);
            List<InvoiceInputs> DataReturn = new List<InvoiceInputs>();
            GoodControl si = new GoodControl();

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                InvoiceInputs hi = new InvoiceInputs();

                try
                {
                    if (InvoiceInputs.Count != 0)
                    {
                        int NextInvoiceSeq = -1;
                        InvoiceParameter InvoicePara = lookup.GetInvoiceNumber();

                        if (InvoicePara != null)
                        {
                            NextInvoiceSeq = (int)InvoicePara.InvoiceKey;
                            foreach (InvoiceInputs di in InvoiceInputs)
                            {
                                List<GoodControl> objgoodcontrol = lookup.GetBarcodeDetailsbyControlId(di.ControlId, di.Seq);
                                if (objgoodcontrol.Count != 0)
                                {
                                    di.SaveSuccessfull = true;
                                    foreach (GoodControl ui in objgoodcontrol)
                                    {
                                        if (di.SaveSuccessfull == true)
                                        {
                                            decimal Price = (decimal)(ui.Qty01 * lookup.GetPricebySKU(ui.L1id, ui.L2id, ui.L3id, ui.L4id));
                                            string InvoiceNo = null;
                                            InvoiceNo = lookup.GetExsistingInvoiceNumber(NextInvoiceSeq, ui.L1id, ui.L2id);

                                            if (InvoiceNo != null)
                                            {
                                                TxnContrl.UpdateInvoiceHeaderDetails(InvoiceNo, ui.Qty01, Price);
                                            }
                                            else
                                            {
                                                InvoicePara = lookup.GetInvoiceNumber();
                                                InvoiceNo = InvoicePara.NextInvoiceNo;

                                                InvoiceHeaderInformation objinvoicedetails = new InvoiceHeaderInformation();
                                                objinvoicedetails.InvoiceNo = InvoiceNo;
                                                objinvoicedetails.VAT = InvoicePara.VAT;
                                                objinvoicedetails.NBT = InvoicePara.NBT;
                                                objinvoicedetails.ExchangeRate = InvoicePara.ExchangeRate;
                                                objinvoicedetails.TotalQty = (decimal)ui.Qty01;
                                                objinvoicedetails.TotalPrice = Price;
                                                objinvoicedetails.TotalPriceIncludingVAT = 0;

                                                objinvoicedetails.CreatedBy = di.CreatedBy;
                                                objinvoicedetails.CreatedMachine = di.CreatedMachine;
                                                objinvoicedetails.ModifiedMachine = di.ModifiedMachine;
                                                objinvoicedetails.ModifiedBy = di.ModifiedBy;
                                                TxnContrl.AddInvoiceHeaderDetail(objinvoicedetails);

                                                TxnContrl.UpdateNextInvoiceNumber();
                                            }

                                            if (false) //Price == 0
                                            {
                                                di.SaveSuccessfull = false;
                                                di.Responce = new string[2];
                                                di.Responce[0] = "Price is '0'...Price update failed ( L1: " + ui.L1id + " | L2: " + ui.L2id + " | L3: " + ui.L3id + " | L4: " + ui.L4id;
                                                di.Responce[1] = "Price is '0'...Price update failed ( L1: " + ui.L1id + " | L2: " + ui.L2id + " | L3: " + ui.L3id + " | L4: " + ui.L4id;
                                            }
                                            else
                                            {
                                                ui.CreatedBy = di.CreatedBy;
                                                ui.CreatedMachine = di.CreatedMachine;
                                                ui.ModifiedBy = di.ModifiedBy;
                                                ui.ModifiedMachine = di.ModifiedMachine;

                                                TxnContrl.AddInvoiceDetail(ui, Price, InvoiceNo, NextInvoiceSeq);
                                                di.SaveSuccessfull = di.SaveSuccessfull == false ? false : true;
                                                di.InvoiceStatus = (di.InvoiceStatus == 0 ? 0 : 1);
                                                di.InvoiceNumber = (di.InvoiceNumber == null ? InvoiceNo : (di.InvoiceNumber.Contains(InvoiceNo) ? di.InvoiceNumber : (di.InvoiceNumber + " / " + InvoiceNo)));
                                            }
                                        }
                                    }

                                    if (di.SaveSuccessfull == true)
                                    {
                                        TxnContrl.UpdateGoodControlDetails(di.ControlId, di.Seq, di.InvoiceNumber);
                                    }

                                    DataReturn.Add(di);
                                }
                                else
                                {
                                    di.Responce = new string[2];
                                    di.Responce[0] = "Return array for dispatch id is empty..";
                                    di.Responce[1] = "Return array for dispatch id is empty..";
                                    di.SaveSuccessfull = false;
                                    DataReturn.Add(di);
                                }

                            }
                        }
                        else
                        {
                            hi.Responce = new string[2];
                            hi.Responce[0] = "Invoice Seq or Invoice Parameter Error.. Invoice Seq= " + NextInvoiceSeq + ", InvoicePara= " + (InvoicePara == null ? "not found" : InvoicePara.ToString());
                            hi.Responce[1] = "Invoice Seq or Invoice Parameter Error.. Invoice Seq= " + NextInvoiceSeq + ", InvoicePara= " + (InvoicePara == null ? "not found" : InvoicePara.ToString());
                            hi.SaveSuccessfull = false;
                            DataReturn.Add(hi);
                        }
                    }
                    else
                    {
                        hi.SaveSuccessfull = false;
                        hi.Responce = new string[2];
                        hi.Responce[0] = "User input array is empty..";
                        hi.Responce[1] = "User input array is empty..";
                        DataReturn.Add(hi);
                    }

                    Boolean validate = true;
                    foreach (InvoiceInputs ni in DataReturn)
                    {
                        if (validate) { validate = ni.SaveSuccessfull; }
                    }

                    if (validate)
                    {
                        trans.Commit();
                    }
                    else { trans.Rollback(); }
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    hi.SaveSuccessfull = false;
                    hi.Responce = new string[2];
                    hi.Responce[0] = exxx;
                    hi.Responce[1] = exxx;
                    DataReturn.Add(hi);
                    trans.Rollback();
                }
            }
            return DataReturn;
        }
        //Create Invoice: END

        //OutSource Detail Generator: START

        //Bag Barcode Checker : check for bag barcode using barcode number and accordingly create update or delete counters
        //Used API's and UI : Bulk Bag Barcode generator
        [Produces("application/json")]
        [HttpPost("UpdateNonApparelGroupBarcode")]
        public List<UserInput> UpdateNonApparelGroupBarcode([FromBody] List<UserInput> lstUserInput)
        {
            logger.InfoFormat("Update Non Apparel Group Barcode API called with lstUserInput = {0}", lstUserInput.ToString());
            List<UserInput> lstBCData_Return = new List<UserInput>();
            LookupController lookup = new LookupController(dcap);
            try
            {
                foreach (UserInput ui in lstUserInput)
                {
                    if (ui.Barcode != "")
                    {
                        Wfdep Wfd = lookup.GetWFConfigurationbyID(ui.WfdepinstId);
                        Boolean validation = true;

                        ui.SaveSuccessfull = true;

                        ui.WFID = (uint)Wfd.Wfid;
                        ui.Depid = Wfd.Depid;
                        ui.DCLId = Wfd.Dclid;
                        ui.TeamId = Wfd.TeamId;
                        ui.Responce = new string[2];
                        ui.TxnDate = System.DateTime.Now;

                        //Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant
                        if (ui.OperationCode != (int)Wfd.OperationCode)
                        {
                            ui.Responce[0] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.Responce[1] = "Configuration Issue, Client config Oppcode1 and Operation code related to WfDepInstId in client config are differant..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            validation = false;
                            continue;
                        }

                        //Check for the exsistence of valid quantity
                        if (ui.EnteredQtyScrap == 0 && ui.EnteredQtyGd == 0 && ui.EnteredQtyRw == 0)
                        {
                            logger.InfoFormat("UpdateNonApparelGroupBarcode API called with EnteredQtyScrap={0},EnteredQtyScrap={1},EnteredQtyScrap={2}", ui.EnteredQtyGd, ui.EnteredQtyScrap, ui.EnteredQtyRw);
                            ui.Responce[0] = "Please scan again..";
                            ui.Responce[1] = "Please scan again..";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            validation = false;
                            continue;
                        }

                        GroupBarcode groupbarcode = new GroupBarcode();
                        TeamCounter teamcounter = new TeamCounter();

                        var pregroupbarcode = lookup.GetBagBarcodeDetailByBagBarcode(ui.Barcode, 1);
                        if (pregroupbarcode != null)
                        {
                            groupbarcode = pregroupbarcode;
                        }
                        var preteamcounter = lookup.GetTeamCounterDetailByBagBarcode(ui.Barcode);
                        if (preteamcounter != null)
                        {
                            teamcounter = preteamcounter;
                        }

                        //Need to check previus group barcode
                        if (teamcounter != null || groupbarcode != null)
                        {
                            //Check wether that prvious quantitiy is exsists and trying to add quantity for exsisiting record
                            if (groupbarcode.TxnStatus > 0 && groupbarcode.Qty01 > 0 && ui.EnteredQtyGd > 0)
                            {
                                ui.Responce[0] = "Barcode is already Used";
                                ui.Responce[1] = "Barcode is already Used";
                                ui.SaveSuccessfull = false;
                                lstBCData_Return.Add(ui);
                                validation = false;
                                continue;
                                //return lstBCData_Return;
                            }

                            #region Qty Reverse

                            if (ui.EnteredQtyGd < 0) //Good Qty Reverse
                            {
                                if ((teamcounter.Qty01 + groupbarcode.Qty01) == 0)
                                {
                                    ui.Responce[0] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.Responce[1] = "Good Qty Reverse - No records to revers, Reported good Qty = 0 ";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    validation = false;
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if ((teamcounter.Qty01 + groupbarcode.Qty01) < (ui.EnteredQtyGd * -1))
                                {
                                    ui.Responce[0] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.Responce[1] = "Good Qty Reverse - You cannot reverse more than reported Qty";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    validation = false;
                                    continue;
                                    //return lstBCData_Return;
                                }
                                else if (groupbarcode != null)
                                {
                                    if (groupbarcode.TxnStatus > 3)
                                    {
                                        ui.Responce[0] = "You cannot modify dispatched bags";
                                        ui.Responce[1] = "You cannot modify dispatched bags";
                                        ui.SaveSuccessfull = false;
                                        lstBCData_Return.Add(ui);
                                        validation = false;
                                        continue;
                                    }
                                }

                            }

                            #endregion

                        }
                        else if ((ui.EnteredQtyGd + ui.EnteredQtyScrap + ui.EnteredQtyRw) < 0)
                        {
                            ui.Responce[0] = "No transaction recorded to reverse";
                            ui.Responce[1] = "No transaction recorded to reverse";
                            ui.SaveSuccessfull = false;
                            lstBCData_Return.Add(ui);
                            validation = false;
                            continue;
                            //return lstBCData_Return;
                        }


                        if (validation)
                        {
                            //Get Reported Qty for Node
                            int currDEPInstQtyGd = 0, currDEPInstQtyScrap = 0, currDEPInstQtyRw = 0, prevDEPQtyGd = 0;
                            GroupBarcode objgroupbarcode = lookup.GetQtyByGroupBarcodeIds(ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, 3, ui.OperationCode);
                            TeamCounter objteamcounter = lookup.GetQtyByTeamCounterIds(ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, 3);

                            if (objgroupbarcode != null)
                            {
                                currDEPInstQtyGd = (int)objgroupbarcode.Qty01;
                                currDEPInstQtyScrap = (int)objgroupbarcode.Qty02;
                                currDEPInstQtyRw = (int)objgroupbarcode.Qty03;
                            }

                            if (objteamcounter != null)
                            {
                                currDEPInstQtyGd = currDEPInstQtyGd + (int)objteamcounter.Qty01;
                                currDEPInstQtyScrap = currDEPInstQtyScrap + (int)objteamcounter.Qty02;
                                currDEPInstQtyRw = currDEPInstQtyRw + (int)objteamcounter.Qty03;
                            }

                            //Get previous qty based on the configuration
                            if (Wfd.LimtWithPredecessor == (int)eDEPLimtWithPredecessor.External)// NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                            {
                                L5moops L5moop = lookup.GetReportedQtyByOperation(ui.Sbucode, ui.FacCode, (int)Wfd.ExOpCode, ui.StyleId, ui.ScheduleId, (uint)0, ui.ColorId, ui.SizeId);

                                if (L5moop != null)
                                {
                                    prevDEPQtyGd = (int)L5moop.ReportedQty - (int)L5moop.ScrappedQty;

                                    if (Wfd.LimitWithWf == (int)eLimitWithWF.NA) // NA = 0,No = 1,Yes = 2,External = 3,PredOpcode = 4,BarcodeCheck = 5
                                    {
                                        ui.Responce[0] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                        ui.Responce[1] = "config error - LimitWithWF cannot be NA in configuration(LimitWithWF.NA)";
                                        ui.SaveSuccessfull = false;
                                        lstBCData_Return.Add(ui);
                                        validation = false;
                                        continue;
                                    }
                                }
                                if ((prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap)) //(prevDEPQtyGd) < (ui.EnteredQtyGd + ui.EnteredQtyScrap + currDEPInstQtyGd + currDEPInstQtyScrap)
                                {
                                    ui.Responce[0] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.Responce[1] = "Qty validation failed - You cannot post more than previous operation \n  " + prevDEPQtyGd + " (Prev Qty) " + " < " + (ui.EnteredQtyGd + ui.EnteredQtyScrap).ToString() + " (Entered Qty) +" + currDEPInstQtyGd + " (Curr Gd Qty) +" + currDEPInstQtyScrap + " (Curr Scrap Qty)";
                                    ui.SaveSuccessfull = false;
                                    lstBCData_Return.Add(ui);
                                    validation = false;
                                    continue;
                                }

                                if (validation)
                                {
                                    var Response = BagBarcodeCheckerForNonApperal(ui, true);
                                    if (Response != null)
                                    {
                                        if (!Response.updated)
                                        {
                                            ui.Responce[0] = "PO Counter Issue (Bag Barcode update failed)";
                                            ui.Responce[1] = "PO Counter Issue (Bag Barcode update failed)";
                                            ui.NewCounter = false;
                                            ui.SaveSuccessfull = false;
                                            lstBCData_Return.Add(ui);
                                        }
                                        else
                                        {
                                            if (Response.IsUpdateAvilable)
                                            {
                                                ui.CounterId = Response.CounterId;
                                                ui.BagBarCodeNo = Response.BagBarCodeNo;
                                            }

                                            ui.IsUpdateAvilable = Response.IsUpdateAvilable;
                                            ui.NewCounter = Response.NewCounter;

                                            lstBCData_Return.Add(DbUpdateForNonApperal(ui));

                                            if (lstBCData_Return.Count != 0)
                                            {
                                                if (lstBCData_Return[0].SaveSuccessfull == true)
                                                {
                                                    if (ui.EnteredQtyGd != 0)
                                                    {
                                                        UpdateLineCounter((int)ui.WfdepinstId, ui.EnteredQtyGd);
                                                    }

                                                    ui.ScanCount = lookup.GetScanCounterVal(ui.WfdepinstId);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ui.Responce[0] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                        ui.Responce[1] = "PO Counter Issue, Bag Barcode update failed, (Response from Barcode Checker is Empty)";
                                        ui.NewCounter = false;
                                        validation = false;
                                        ui.SaveSuccessfull = false;
                                        lstBCData_Return.Add(ui);
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        ui.Responce[0] = "Barcode value is empty. Please Check..";
                        ui.Responce[1] = "Barcode value is empty. Please Check..";
                        ui.SaveSuccessfull = false;
                        lstBCData_Return.Add(ui);
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Update Non Apparel Group Barcode {0}", e.ToString());
                throw e;
            }
            return lstBCData_Return;
        }

        //Bag Barcode Checker : check for bag barcode using barcode number and accordingly create update or delete counters
        //Used API's and UI : Bulk Bag Barcode generator
        [Produces("application/json")]
        [HttpPost("BulkBagBarcodeChecker")]
        public TeamCounterOutput BulkBagBarcodeChecker([FromBody] UserInput ui, Boolean ValidationPass)
        {
            logger.InfoFormat("BagBarcodeChecker API called with lstUserInput = {0}", ui, ValidationPass);
            TeamCounterOutput TeamCounter = new TeamCounterOutput();

            TeamCounter.NewCounter = false;
            TeamCounter.updated = false;
            TeamCounter.IsUpdateAvilable = false;

            LookupController lookup = new LookupController(dcap);
            try
            {

                //Get Bag Barcode if assigned to a Barcode in DETXN Table
                var BagBarcodeDetails = lookup.GetBagBarcodeByBarcode(ui.Barcode, (int)ui.StyleId, (int)ui.ScheduleId, 0, (int)ui.ColorId);
                if (BagBarcodeDetails != null)
                {
                    //if there is a asigend Bag Barcode then get if that bag is on ongoing counter
                    TeamCounter counterDetails = lookup.CheckForOngoingBagBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo);

                    if (counterDetails != null)
                    {
                        //Update Counter
                        TeamCounter.CounterId = counterDetails.CounterId;
                        TeamCounter.BagBarCodeNo = BagBarcodeDetails.BagBarCodeNo;

                        //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                        TeamCounter.IsUpdateAvilable = true;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                    else
                    {
                        //Set if the barcode is in abag barcode inventory   
                        TeamCounter groupBarcodeDetails = lookup.CheckForOngoingGroupBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, BagBarcodeDetails.BagBarCodeNo);
                        if (groupBarcodeDetails != null)
                        {
                            TeamCounter.CounterId = groupBarcodeDetails.CounterId;
                            TeamCounter.BagBarCodeNo = groupBarcodeDetails.BagBarCodeNo;

                            //TeamCounter.updated = transaction.UpdatePOCounter(counteridextracted, 3, ui.EnteredQtyGd, 0, BagBarcodeDetails.BagBarCodeNo, (int)ui.WFID);
                            TeamCounter.IsUpdateAvilable = true;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            TeamCounter.IsUpdateAvilable = false;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = false;
                        }
                    }
                }
                else
                {
                    //CHECK WETHER QUANTITITY IS A PLUS: Because Minus quantiity cannot start or update in a counter (without bagbarcode)
                    if (ui.EnteredQtyGd > 0)
                    {
                        //if there no bag barode records for the barcode then check for ongoing counter in team counter table
                        TeamCounter counterDetails = lookup.CheckForOngoingBagBarcodeCounterExsistence(ui.WfdepinstId, (int)ui.StyleId, (int)ui.ScheduleId, (int)ui.ColorId, null);
                        if (counterDetails != null)
                        {
                            //there is a counter for this SKU => Update Counter
                            TeamCounter.CounterId = counterDetails.CounterId;
                            TeamCounter.BagBarCodeNo = counterDetails.BagBarCodeNo;

                            //TeamCounter.updated = transaction.UpdatePOCounter(counterDetails.CounterId, 3, ui.EnteredQtyGd, 0, counterDetails.BagBarCodeNo, (int)ui.WFID);

                            TeamCounter.IsUpdateAvilable = true;
                            TeamCounter.NewCounter = false;
                            TeamCounter.updated = true;
                        }
                        else
                        {
                            //There is no counter avilabale  procced
                            TeamCounter.IsUpdateAvilable = false;
                            TeamCounter.NewCounter = true;
                            TeamCounter.updated = true;
                        }
                    }
                    else
                    {
                        TeamCounter.IsUpdateAvilable = false;
                        TeamCounter.NewCounter = false;
                        TeamCounter.updated = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Bag Barcode Checker {0}", e.ToString());
                throw e;
            }
            return TeamCounter;
        }

        //OutSource Detail Generator: END


        //Mainaince :START

        //Remove Styles From DataBase || checked 12-19-2020
        //Used API's and UI : () () WEB
        [Produces("application/json")]
        [HttpPost("RemoveStyleByStyleId")]
        public List<GeneralInput> RemoveStyleByStyleId([FromBody] List<GeneralInput> GeneralInput)
        {
            logger.InfoFormat("Remove Style By StyleId GeneralInput={0}", GeneralInput.ToString());

            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                List<GeneralInput> GeneralOut = new List<GeneralInput>();
                try
                {
                    foreach (GeneralInput gi in GeneralInput)
                    {
                        if (gi.L1id != 0)
                        {
                            GeneralOut.Add(TxnContrl.RemoveData(gi));
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    string exxx = ex.Message;
                    throw ex;
                    GeneralOut[0].SaveSuccessfull = false;
                    GeneralOut[0].Responce = new string[2];
                    GeneralOut[0].Responce[0] = exxx;
                    GeneralOut[0].Responce[1] = exxx;
                    trans.Rollback();
                }
            }

            return GeneralInput;
        }
        //Mainaince :END

        #endregion
    }
}
