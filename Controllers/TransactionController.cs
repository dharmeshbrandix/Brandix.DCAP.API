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
using Brandix.DCAP.API.Models;
using Brandix.DCAP.API.CustomModels;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.ComponentModel;

namespace Brandix.DCAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]

    public class TransactionController : ControllerBase
    {
        #region Variable Declarations     
        static ILog logger = LogManager.GetLogger(typeof(TransactionController));
        private DCAPDbContext dcap;
        #endregion

        #region Constructor
        public TransactionController(DCAPDbContext context)
        {
            dcap = context;
        }

        #endregion

        #region APIs

        //Update Color for Barcpde
        [Produces("application/json")]
        [HttpGet("UpdateColorforBC")]
        public void UpdateColorforBC(L5bc l5bc)
        {
            L5bc objL5bc = new L5bc();
            try
            {
                objL5bc = dcap.L5bc
                           .Where(c => c.BarCodeNo == l5bc.BarCodeNo)
                           .FirstOrDefault();

                if (objL5bc != null)
                {
                    objL5bc.L4id = l5bc.L4id;
                    objL5bc.L5bcisUsed = l5bc.L5bcisUsed;

                    objL5bc.ModifiedBy = l5bc.ModifiedBy;
                    objL5bc.ModifiedDateTime = System.DateTime.Now;
                    objL5bc.ModifiedMachine = l5bc.ModifiedMachine;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Insert data to detxn 
        [Produces("application/json")]
        [HttpPost("AddDETXN")]
        public void AddDETXN(UserInput ui)
        {
            //logger.InfoFormat("Error while retrieving Schedule information");
            logger.InfoFormat("AddDETXN ui={0}", ui);
            //LookupController ojbLukUp = new LookupController(dcap, ui.guid);
            LookupController ojbLukUp = new LookupController(dcap);
            int SeqNo = 0;
            int ProdHour = 0;
            int UploadStatus = 99;
            try
            {
                SeqNo = 0; //ojbLukUp.GetDETxnNextSeqNo(ui);
                ProdHour = ojbLukUp.GetProdHourByTeamId((int)ui.TeamId, ui.TxnDate, ui.Offline);
                UploadStatus = ojbLukUp.GetDeTxnUploadStatus((int)ui.TeamId, ui.OperationCode);

                Detxn objdetxn = new Detxn();
                objdetxn.WfdepinstId = ui.WfdepinstId;
                objdetxn.Seq = (uint)SeqNo;
                objdetxn.L1id = ui.StyleId;
                objdetxn.L2id = ui.ScheduleId;
                objdetxn.L3id = 0;
                objdetxn.L4id = ui.ColorId;
                objdetxn.L5id = ui.SizeId;
                objdetxn.L5moid = (int)ui.L5MOID;
                objdetxn.L5mono = ui.L5MONo;
                objdetxn.BarCodeNo = ui.Barcode;
                objdetxn.BagBarCodeNo = ui.BagBarCodeNo; //Changed by NimanthaH
                objdetxn.TravelBarCodeNo = ""; //Changed by NimanthaH

                objdetxn.Wfid = ui.WFID;
                objdetxn.Depid = ui.Depid;
                objdetxn.TeamId = ui.TeamId;
                objdetxn.Dclid = ui.DCLId;
                objdetxn.TxnDateTime = Convert.ToDateTime(ui.TxnDate);
                objdetxn.HourNo = ProdHour;
                objdetxn.TxnMode = ui.TxnMode;
                objdetxn.PlussMinus = (uint)Convert.ToInt32(ui.PlussMinus);
                objdetxn.Rrid = (uint)Convert.ToInt32(ui.RRId);
                objdetxn.DopsId = (uint)Convert.ToInt32(ui.DOpsId);
                objdetxn.Qty01 = ui.EnteredQtyGd;
                objdetxn.Qty02 = ui.EnteredQtyScrap;
                objdetxn.Qty03 = ui.EnteredQtyRw;
                objdetxn.JobNo = ui.JobNo;
                objdetxn.Qty01Ns = 0;
                objdetxn.Qty02Ns = 0;
                objdetxn.Qty03Ns = 0;
                objdetxn.OperationCode = ui.OperationCode;
                objdetxn.EnteredBy = ui.CreatedBy;
                objdetxn.AppStatus = (int)AppStatus.Pending;
                objdetxn.UploadStatus = UploadStatus;
                objdetxn.ErrorCode = 0;
                objdetxn.HasError = 0;
                objdetxn.RecStatus = 1;

                objdetxn.CreatedBy = ui.CreatedBy;
                objdetxn.CreatedDateTime = DateTime.Now;
                objdetxn.CreatedMachine = ui.CreatedMachine;
                objdetxn.ModifiedBy = ui.ModifiedBy;
                objdetxn.ModifiedDateTime = DateTime.Now;
                objdetxn.ModifiedMachine = ui.ModifiedMachine;

                dcap.Detxn.Add(objdetxn);
                dcap.SaveChanges();

                ui.DetxnKey = objdetxn.DetxnKey;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Detxn Save successfull");

            }
        }

        //Insert data to detxn travel tag 
        [Produces("application/json")]
        [HttpPost("AddDETXNBFL")]
        public void AddDETXNBFL(UserInput ui)
        {
            //logger.InfoFormat("Error while retrieving Schedule information");
            logger.InfoFormat("AddDETXN ui={0}", ui);
            //LookupController ojbLukUp = new LookupController(dcap, ui.guid);
            LookupController ojbLukUp = new LookupController(dcap);
            int SeqNo = 0;
            int ProdHour = 0;
            int UploadStatus = 99;
            try
            {
                SeqNo = 0; //ojbLukUp.GetDETxnNextSeqNo(ui);
                ProdHour = ojbLukUp.GetProdHourByTeamId((int)ui.TeamId, ui.TxnDate, ui.Offline);
                UploadStatus = ojbLukUp.GetDeTxnUploadStatus((int)ui.TeamId, ui.OperationCode);

                Detxn objdetxn = new Detxn();
                objdetxn.WfdepinstId = ui.WfdepinstId;
                objdetxn.Seq = (uint)SeqNo;
                objdetxn.L1id = ui.StyleId;
                objdetxn.L2id = ui.ScheduleId;
                objdetxn.L3id = 0;
                objdetxn.L4id = ui.ColorId;
                objdetxn.L5id = ui.SizeId;
                objdetxn.L5moid = (int)ui.L5MOID;
                objdetxn.L5mono = ui.L5MONo;
                objdetxn.BarCodeNo = ui.Barcode;
                objdetxn.BagBarCodeNo = ((ui.RefBagBarCodeNo == "" || ui.RefBagBarCodeNo == null) ? ui.BagBarCodeNo : ui.RefBagBarCodeNo); //Changed by NimanthaH
                objdetxn.TravelBarCodeNo = ui.BagBarCodeNo; //Changed by NimanthaH

                objdetxn.Wfid = ui.WFID;
                objdetxn.Depid = ui.Depid;
                objdetxn.TeamId = ui.TeamId;
                objdetxn.Dclid = ui.DCLId;
                objdetxn.TxnDateTime = Convert.ToDateTime(ui.TxnDate);
                objdetxn.HourNo = ProdHour;
                objdetxn.TxnMode = ui.TxnMode;
                objdetxn.PlussMinus = (uint)Convert.ToInt32(ui.PlussMinus);
                objdetxn.Rrid = (uint)Convert.ToInt32(ui.RRId);
                objdetxn.DopsId = (uint)Convert.ToInt32(ui.DOpsId);
                objdetxn.Qty01 = ui.EnteredQtyGd;
                objdetxn.Qty02 = ui.EnteredQtyScrap;
                objdetxn.Qty03 = ui.EnteredQtyRw;
                objdetxn.Qty01Ns = 0;
                objdetxn.Qty02Ns = 0;
                objdetxn.Qty03Ns = 0;
                objdetxn.JobNo = ui.JobNo;
                objdetxn.OperationCode = ui.OperationCode;
                objdetxn.EnteredBy = ui.CreatedBy;
                objdetxn.AppStatus = (int)AppStatus.Pending;
                objdetxn.UploadStatus = UploadStatus;
                objdetxn.ErrorCode = 0;
                objdetxn.HasError = 0;
                objdetxn.RecStatus = 1;

                objdetxn.CreatedBy = ui.CreatedBy;
                objdetxn.CreatedDateTime = DateTime.Now;
                objdetxn.CreatedMachine = ui.CreatedMachine;
                objdetxn.ModifiedBy = ui.ModifiedBy;
                objdetxn.ModifiedDateTime = DateTime.Now;
                objdetxn.ModifiedMachine = ui.ModifiedMachine;

                dcap.Detxn.Add(objdetxn);
                dcap.SaveChanges();

                ui.DetxnKey = objdetxn.DetxnKey;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Detxn Save successfull");

            }
        }

        //Add DE DEP table data
        [Produces("application/json")]
        [HttpPost("AddDEDEP")]
        public void AddDEDEP(UserInput ui)
        {
            try
            {
                int SeqNo = 0;
                // LookupController trnsCon = new LookupController(dcap, ui.guid);
                LookupController trnsCon = new LookupController(dcap);
                SeqNo = trnsCon.GetDEDEPNextSeqNo(ui.WFID, ui.Depid) + ui.seqIndex;

                //using (DCAPDbContext context = new DCAPDbContext())
                //{
                Dedep objdedep = new Dedep();
                objdedep.Wfid = ui.WFID;
                objdedep.Depid = ui.Depid;
                objdedep.Seq = SeqNo;

                objdedep.Dclid = ui.DCLId;
                objdedep.L1id = ui.StyleId;
                objdedep.L2id = ui.ScheduleId;
                objdedep.L3id = 0;
                objdedep.L4id = ui.ColorId;
                objdedep.L5id = ui.SizeId;
                objdedep.L5moid = (int)ui.L5MOID;
                objdedep.TxnMode = ui.TxnMode;
                objdedep.Qty01 = ui.EnteredQtyGd;
                objdedep.Qty02 = ui.EnteredQtyScrap;
                objdedep.Qty03 = ui.EnteredQtyRw;

                objdedep.WorkCenter = ui.WorkCenter;
                objdedep.OperationCode = ui.OperationCode;
                objdedep.RecStatus = 1;
                objdedep.CreatedBy = ui.CreatedBy;
                objdedep.CreatedDateTime = DateTime.Now;
                objdedep.CreatedMachine = ui.CreatedMachine;
                objdedep.ModifiedBy = ui.ModifiedBy;
                objdedep.ModifiedDateTime = DateTime.Now;
                objdedep.ModifiedMachine = ui.ModifiedMachine;

                dcap.Dedep.Add(objdedep);
                dcap.SaveChanges();
                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        [Produces("application/json")]
        [HttpPost("AddDEDEPPre")]
        public void AddDEDEPPre(UserInput ui)
        {
            try
            {
                int SeqNo = 0;
                // LookupController trnsCon = new LookupController(dcap, ui.guid);
                LookupController trnsCon = new LookupController(dcap);
                SeqNo = trnsCon.GetDEDEPNextSeqNo(ui.WFID, ui.Depid);

                using (DCAPDbContext context = new DCAPDbContext())
                {
                    Dedep objdedep = new Dedep();
                    objdedep.Wfid = ui.WFID;
                    objdedep.Depid = ui.Depid;
                    objdedep.Seq = SeqNo;

                    objdedep.Dclid = ui.DCLId;
                    objdedep.L1id = ui.StyleId;
                    objdedep.L2id = ui.ScheduleId;
                    objdedep.L3id = 0;
                    objdedep.L4id = ui.ColorId;
                    objdedep.L5id = ui.SizeId;
                    objdedep.L5moid = (int)ui.L5MOID;
                    objdedep.TxnMode = ui.TxnMode;
                    objdedep.Qty01 = ui.EnteredQtyGd;
                    objdedep.Qty02 = ui.EnteredQtyScrap;
                    objdedep.Qty03 = ui.EnteredQtyRw;

                    objdedep.WorkCenter = ui.WorkCenter;
                    objdedep.OperationCode = ui.OperationCode;
                    objdedep.RecStatus = 1;
                    objdedep.CreatedBy = ui.CreatedBy;
                    objdedep.CreatedDateTime = DateTime.Now;
                    objdedep.CreatedMachine = ui.CreatedMachine;
                    objdedep.ModifiedBy = ui.ModifiedBy;
                    objdedep.ModifiedDateTime = DateTime.Now;
                    objdedep.ModifiedMachine = ui.ModifiedMachine;

                    context.Dedep.Add(objdedep);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Add DE DEP table data
        [Produces("application/json")]
        [HttpPost("AddDEDEPBFL")]
        public void AddDEDEPBFL(UserInput ui)
        {
            try
            {
                int SeqNo = 0;
                // LookupController trnsCon = new LookupController(dcap, ui.guid);
                LookupController trnsCon = new LookupController(dcap);
                SeqNo = trnsCon.GetDEDEPNextSeqNo(ui.WFID, ui.Depid) + ui.seqIndex;

                //using (DCAPDbContext context = new DCAPDbContext())
                //{
                Dedep objdedep = new Dedep();
                objdedep.Wfid = ui.WFID;
                objdedep.Depid = ui.Depid;
                objdedep.Seq = SeqNo;

                objdedep.Dclid = ui.DCLId;
                objdedep.L1id = ui.StyleId;
                objdedep.L2id = ui.ScheduleId;
                objdedep.L3id = 0;
                objdedep.L4id = ui.ColorId;
                objdedep.L5id = ui.SizeId;
                objdedep.L5moid = (int)ui.L5MOID;
                objdedep.TxnMode = ui.TxnMode;
                objdedep.Qty01 = ui.EnteredQtyGd;
                objdedep.Qty02 = ui.EnteredQtyScrap;
                objdedep.Qty03 = ui.EnteredQtyRw;

                objdedep.WorkCenter = ui.WorkCenter;
                objdedep.OperationCode = ui.OperationCode;
                objdedep.RecStatus = 1;
                objdedep.CreatedBy = ui.CreatedBy;
                objdedep.CreatedDateTime = DateTime.Now;
                objdedep.CreatedMachine = ui.CreatedMachine;
                objdedep.ModifiedBy = ui.ModifiedBy;
                objdedep.ModifiedDateTime = DateTime.Now;
                objdedep.ModifiedMachine = ui.ModifiedMachine;

                dcap.Dedep.Add(objdedep);
                dcap.SaveChanges();
                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }


        //Update DEDEP table API
        [Produces("application/json")]
        [HttpGet("UpdateDEDEP")]
        public void UpdateDEDEP(UserInput ui)
        {
            try
            {
                Dedep objdedep = new Dedep();
                objdedep = dcap.Dedep
                        .Where(c => c.Depid == ui.Depid && c.L1id == ui.StyleId
                        && c.L2id == ui.ScheduleId && c.L3id == 0 && c.L4id == ui.ColorId
                        && c.L5id == ui.SizeId && c.L5moid == ui.L5MOID && c.Wfid == ui.WFID)
                       .FirstOrDefault();

                /* objdedep = dcap.Dedep
                        .Where(c => c.Wfid == ui.WFID && c.Depid == ui.Depid && c.L1id == ui.StyleId
                        && c.L2id == ui.ScheduleId && c.L3id == 0 && c.L4id == ui.ColorId && c.L5moid == ui.L5MOID)
                       .FirstOrDefault();
                */
                if (objdedep != null)
                {
                    decimal QtyGd = (decimal)objdedep.Qty01 + ui.QtytoSaveGd;
                    decimal QtySC = (decimal)objdedep.Qty02 + ui.QtytoSaveScrap;
                    decimal QtyRW = (decimal)objdedep.Qty03 + ui.QtytoSaveRw;

                    objdedep.Qty01 = QtyGd >= 0 ? QtyGd : 0;
                    objdedep.Qty02 = QtySC >= 0 ? QtySC : 0;
                    objdedep.Qty03 = QtyRW >= 0 ? QtyRW : 0;
                    objdedep.ModifiedDateTime = DateTime.Now;
                    objdedep.ModifiedBy = ui.ModifiedBy;
                    objdedep.ModifiedMachine = ui.ModifiedMachine;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        //Update DEDEP Inastance table API  
        [Produces("application/json")]
        [HttpPost("AddDEDEPInst")]
        public void AddDEDEPInst(UserInput ui)
        {
            try
            {
                //LookupController ojbLukUp = new LookupController(dcap, ui.guid);
                LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);
                int SeqNo = ojbLukUp.GetDEDEPInstNextSeqNo(ui.WfdepinstId) + ui.seqIndex;

                //using (DCAPDbContext context = new DCAPDbContext())
                //{
                Dedepinst objdedepInst = new Dedepinst();

                objdedepInst.WfdepinstId = ui.WfdepinstId;
                objdedepInst.Seq = (uint)SeqNo;
                objdedepInst.L1id = ui.StyleId;
                objdedepInst.L2id = ui.ScheduleId;
                objdedepInst.L3id = 0;

                objdedepInst.L4id = ui.ColorId;
                objdedepInst.L5id = ui.SizeId;
                objdedepInst.L5moid = (int)ui.L5MOID;
                objdedepInst.Wfid = ui.WFID;
                objdedepInst.Depid = ui.Depid;

                objdedepInst.TeamId = ui.TeamId;
                objdedepInst.Dclid = ui.DCLId;
                objdedepInst.OperationCode = ui.OperationCode;
                //objdedepInst.WorkCenter = ui.WorkCenter;

                objdedepInst.Qty01 = ui.EnteredQtyGd;
                objdedepInst.Qty02 = ui.EnteredQtyScrap;
                objdedepInst.Qty03 = ui.EnteredQtyRw;

                objdedepInst.TxnMode = ui.TxnMode;
                objdedepInst.PlussMinus = ui.PlussMinus;
                objdedepInst.RecStatus = 1;
                objdedepInst.CreatedBy = ui.CreatedBy;
                objdedepInst.CreatedDateTime = DateTime.Now;
                objdedepInst.CreatedMachine = ui.CreatedMachine;

                objdedepInst.ModifiedBy = ui.ModifiedBy == null ? ui.CreatedBy : ui.ModifiedBy;
                objdedepInst.ModifiedDateTime = DateTime.Now;
                objdedepInst.ModifiedMachine = ui.ModifiedMachine == null ? ui.CreatedMachine : ui.ModifiedMachine;
                dcap.Dedepinst.Add(objdedepInst);
                dcap.SaveChanges();
                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        //Update DEDEP Inastance table API  
        [Produces("application/json")]
        [HttpPost("AddDEDEPInstBFL")]
        public void AddDEDEPInstBFL(UserInput ui)
        {
            LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);

            try
            {
                int SeqNo = ojbLukUp.GetDEDEPInstNextSeqNo(ui.WfdepinstId) + ui.seqIndex;

                Dedepinst objdedepInst = new Dedepinst();

                objdedepInst.WfdepinstId = ui.WfdepinstId;
                objdedepInst.Seq = (uint)SeqNo;
                objdedepInst.L1id = ui.StyleId;
                objdedepInst.L2id = ui.ScheduleId;
                objdedepInst.L3id = 0;

                objdedepInst.L4id = ui.ColorId;
                objdedepInst.L5id = ui.SizeId;
                objdedepInst.L5moid = (int)ui.L5MOID;
                objdedepInst.Wfid = ui.WFID;
                objdedepInst.Depid = ui.Depid;

                objdedepInst.TeamId = ui.TeamId;
                objdedepInst.Dclid = ui.DCLId;
                objdedepInst.OperationCode = ui.OperationCode;
                //objdedepInst.WorkCenter = ui.WorkCenter;

                objdedepInst.Qty01 = ui.EnteredQtyGd;
                objdedepInst.Qty02 = ui.EnteredQtyScrap;
                objdedepInst.Qty03 = ui.EnteredQtyRw;

                objdedepInst.TxnMode = ui.TxnMode;
                objdedepInst.PlussMinus = ui.PlussMinus;
                objdedepInst.RecStatus = 1;
                objdedepInst.CreatedBy = ui.CreatedBy;
                objdedepInst.CreatedDateTime = DateTime.Now;
                objdedepInst.CreatedMachine = ui.CreatedMachine;

                objdedepInst.ModifiedBy = ui.ModifiedBy == null ? ui.CreatedBy : ui.ModifiedBy;
                objdedepInst.ModifiedDateTime = DateTime.Now;
                objdedepInst.ModifiedMachine = ui.ModifiedMachine == null ? ui.CreatedMachine : ui.ModifiedMachine;

                dcap.Dedepinst.Add(objdedepInst);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        //Update DEDEP Inastance table API
        [Produces("application/json")]
        [HttpGet("UpdateDEDEPInst")]
        public void UpdateDEDEPInst(UserInput ui)
        {
            logger.InfoFormat("UpdateDEDEPInst UserInput={0}", ui);
            try
            {
                Dedepinst objdedep = new Dedepinst();
                objdedep = dcap.Dedepinst
                        .Where(c => c.WfdepinstId == ui.WfdepinstId && c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L3id == 0 && c.L4id == ui.ColorId && c.L5id == ui.SizeId && c.L5moid == ui.L5MOID)
                        .FirstOrDefault();

                if (objdedep != null)
                {
                    decimal QtyGd = (decimal)objdedep.Qty01 + ui.QtytoSaveGd;
                    decimal QtySC = (decimal)objdedep.Qty02 + ui.QtytoSaveScrap;
                    decimal QtyRW = (decimal)objdedep.Qty03 + ui.QtytoSaveRw;

                    objdedep.Qty01 = QtyGd >= 0 ? QtyGd : 0;
                    objdedep.Qty02 = QtySC >= 0 ? QtySC : 0;
                    objdedep.Qty03 = QtyRW >= 0 ? QtyRW : 0;
                    objdedep.ModifiedDateTime = DateTime.Now;
                    objdedep.ModifiedBy = ui.CreatedBy;
                    objdedep.ModifiedMachine = ui.CreatedMachine;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Update DEDEP Inastance table API
        [Produces("application/json")]
        [HttpGet("UpdateL5bc")]
        public void UpdateL5bc(UserInput ui)
        {
            try
            {
                L5bc objL5bc = new L5bc();
                objL5bc = dcap.L5bc
                        .Where(c => c.BarCodeNo == ui.Barcode)
                        .FirstOrDefault();

                if (objL5bc != null)
                {
                    objL5bc.L4id = ui.ColorId;
                    objL5bc.L5bcisUsed = (int)eL5bcisUsed.Yes;
                    objL5bc.ModifiedDateTime = DateTime.Now;
                    objL5bc.ModifiedBy = ui.ModifiedBy;
                    objL5bc.ModifiedMachine = ui.ModifiedMachine;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Update DEDEP Inastance table API
        [Produces("application/json")]
        [HttpPost("UpdateLineCounter")]
        public void UpdateLineCounter(int Wfdepinst, int EnteredQtyGd)
        {

            uint WfdepinstId = (uint)Wfdepinst;
            logger.InfoFormat("Call UpdateLineCounter for the WfdepinstId = {0}", WfdepinstId);

            try
            {
                Wfdep objwfd = new Wfdep();
                objwfd = dcap.Wfdep
                        .Where(c => c.WfdepinstId == WfdepinstId)
                        .FirstOrDefault();

                if (objwfd != null)
                {
                    objwfd.ScanCounter = objwfd.ScanCounter + EnteredQtyGd;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Add TeamCounter table data
        [Produces("application/json")]
        [HttpPost("UpdateGroupbarcodeMapping")]
        public Boolean UpdateGroupbarcodeMapping(GroupBarcodeMapping broupBarcodeMapping)
        {
            logger.InfoFormat("Update Group barcode Mapping broupBarcodeMapping=[{0}]", broupBarcodeMapping.ToString());
            Boolean updateSuccess = true;
            try
            {
                GroupBarcodeMapping TC = new GroupBarcodeMapping();

                TC.WFDEPInstId = broupBarcodeMapping.WFDEPInstId;
                TC.MotherBarcode = broupBarcodeMapping.MotherBarcode;
                TC.MotherTxnMode = broupBarcodeMapping.MotherTxnMode;
                TC.ChildBarcode = broupBarcodeMapping.ChildBarcode;
                TC.ChildTxnMode = broupBarcodeMapping.ChildTxnMode;

                TC.Qty01 = broupBarcodeMapping.Qty01;
                TC.Qty02 = broupBarcodeMapping.Qty02;
                TC.Qty03 = broupBarcodeMapping.Qty03;
                TC.Qty01NS = broupBarcodeMapping.Qty01NS;
                TC.Qty02NS = broupBarcodeMapping.Qty02NS;
                TC.Qty03NS = broupBarcodeMapping.Qty03NS;

                TC.CreatedDateTime = broupBarcodeMapping.CreatedDateTime;
                TC.CreatedBy = broupBarcodeMapping.CreatedBy;
                TC.CreatedMachine = broupBarcodeMapping.CreatedMachine;
                TC.ModifiedBy = broupBarcodeMapping.ModifiedBy;
                TC.ModifiedDateTime = broupBarcodeMapping.ModifiedDateTime;
                TC.ModifiedMachine = broupBarcodeMapping.ModifiedMachine;

                dcap.GroupBarcodeMapping.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
                updateSuccess = false;
            }
            finally
            {
                logger.InfoFormat("Team Counter Save successfull");
                updateSuccess = true;

            }
            return updateSuccess;
        }

        //Update DEDEP Inastance table API
        [Produces("application/json")]
        [HttpGet("ResetCounter")]
        public void ResetCounter(int wfdepinstId)
        {
            logger.InfoFormat("Call ResetCounter for the WfdepinstId = {0}", wfdepinstId);

            try
            {
                Wfdep objwfd = new Wfdep();
                objwfd = dcap.Wfdep
                        .Where(c => c.WfdepinstId == wfdepinstId)
                        .FirstOrDefault();

                if (objwfd != null)
                {
                    objwfd.ScanCounter = 0;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Add TeamCounter table data
        [Produces("application/json")]
        [HttpPost("AddTeamCounter")]
        public UserInput AddTeamCounter(UserInput ui)
        {
            logger.InfoFormat("AddTeamCounter ui=[{0}]", ui);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);               

                int SeqNo = 0;

                if (ojbLukUp.CheckForBagBarcode(ui.BagBarCodeNo, 1))
                {
                    decimal cutqty = (int)ojbLukUp.GetQuantityByDetails(3, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, 0, 15, 151);

                    TeamCounter TC = new TeamCounter();
                    TC.WfdepinstId = ui.WfdepinstId;
                    TC.CounterType = 1;
                    TC.L1id = ui.StyleId;
                    TC.L2id = ui.ScheduleId;
                    TC.L3id = 0;
                    TC.L4id = ui.ColorId;
                    TC.L5id = 0;

                    TC.Qty01 = ui.EnteredQtyGd;
                    TC.Qty02 = 0;
                    TC.Qty03 = 0;

                    TC.CutQty = (int)(cutqty - ojbLukUp.GetQuantityByDetails(2, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, 0, 15, 151));//op1 from l5moops table and op2 from dedep
                    TC.IntCutQty = (int)cutqty;

                    TC.BagBarCodeNo = ui.BagBarCodeNo;
                    TC.BagSize = ui.BagSize;
                    TC.BagStatus = ui.BagStatus;

                    TC.CounterNumber = ui.CounterNumber;

                    TC.RecStatus = 1;
                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.TeamCounter.Add(TC);
                    dcap.SaveChanges();

                    ui.SaveSuccessfull = true;
                }
                else
                {
                    ui.Responce[0] = "The mentiioned Bag Barcode is in already used. Try again with another Bag Barcode";
                    ui.Responce[1] = "The mentiioned Bag Barcode is in already used. Try again with another Bag Barcode";

                    ui.SaveSuccessfull = false;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Team Counter Save successfull");

            }
            return ui;
        }

        public UserInput AddTeamCounterNonApperal(UserInput ui)
        {
            logger.InfoFormat("AddTeamCounter ui=[{0}]", ui);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);               

                int SeqNo = 0;

                if (ojbLukUp.CheckForBagBarcode(ui.BagBarCodeNo, 1))
                {
                    decimal cutqty = (int)ojbLukUp.GetQuantityByDetails(4, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, 15, 151);
                    GroupBarcode objgroupbarcode = ojbLukUp.GetQtyByGroupBarcodeIds(ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, 3, ui.OperationCode);
                    TeamCounter objteamcounter = ojbLukUp.GetQtyByTeamCounterIds(ui.WfdepinstId, ui.WFID, ui.StyleId, ui.ScheduleId, 0, ui.ColorId, ui.SizeId, 3);


                    TeamCounter TC = new TeamCounter();
                    TC.WfdepinstId = ui.WfdepinstId;
                    TC.CounterType = 1;
                    TC.L1id = ui.StyleId;
                    TC.L2id = ui.ScheduleId;
                    TC.L3id = 0;
                    TC.L4id = ui.ColorId;
                    TC.L5id = ui.SizeId;

                    TC.Qty01 = ui.EnteredQtyGd;
                    TC.Qty02 = 0;
                    TC.Qty03 = 0;

                    TC.CutQty = (int)(cutqty - ((objgroupbarcode == null ? 0 : objgroupbarcode.Qty01) + (int)ui.EnteredQtyGd + (objteamcounter == null ? 0 : objteamcounter.Qty01)));//op1 from l5moops table and op2 from dedep
                    TC.IntCutQty = (int)cutqty;

                    TC.BagBarCodeNo = ui.BagBarCodeNo;
                    TC.BagSize = ui.BagSize;
                    TC.BagStatus = ui.BagStatus;

                    TC.CounterNumber = ui.CounterNumber;

                    TC.RecStatus = 1;
                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.TeamCounter.Add(TC);
                    dcap.SaveChanges();

                    ui.SaveSuccessfull = true;
                }
                else
                {
                    ui.Responce[0] = "The mentiioned Bag Barcode is in already used. Try again with another Bag Barcode";
                    ui.Responce[1] = "The mentiioned Bag Barcode is in already used. Try again with another Bag Barcode";

                    ui.SaveSuccessfull = false;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Team Counter Save successfull");

            }
            return ui;
        }

        //Add TeamCounter table data
        [Produces("application/json")]
        [HttpPost("AddBuddyTagCounter")]
        public UserInput AddBuddyTagCounter(UserInput ui)
        {
            logger.InfoFormat("AddTeamCounter ui=[{0}]", ui);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);               

                int SeqNo = 0;

                if (true)
                {
                    BuudyTagCounter TC = new BuudyTagCounter();
                    TC.WfdepinstId = ui.WfdepinstId;
                    TC.CounterType = 1;
                    TC.L1id = ui.StyleId;
                    TC.L2id = ui.ScheduleId;
                    TC.L3id = 0;
                    TC.L4id = ui.ColorId;
                    TC.L5id = 0;

                    TC.TravelBarCodeNo = ui.JobNo;
                    TC.RRType = ui.RRType;
                    TC.RRId = Convert.ToInt16(ui.RRId);
                    TC.RRName = ui.RRName;

                    TC.Qty01 = 1;//ui.EnteredQtyRw + ui.EnteredQtyScrap;
                    TC.Qty02 = 0;
                    TC.Qty03 = 0;

                    TC.CutQty = 0;

                    TC.BagBarCodeNo = ui.BagBarCodeNo;
                    TC.BagSize = ui.BagSize;
                    TC.BagStatus = ui.BagStatus;

                    TC.JobQty = 0;
                    TC.Weight = 0;
                    TC.TrollyNo = "";
                    TC.Remarks = "";

                    TC.CounterNumber = ui.CounterNumber;

                    TC.RecStatus = 1;
                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.BuudyTagCounter.Add(TC);
                    dcap.SaveChanges();
                }
                else
                {
                    ui.Responce[0] = "The mentiioned Buddy Barcode is in already used. Try again with another Bag Barcode";
                    ui.Responce[1] = "The mentiioned Buddy Barcode is in already used. Try again with another Bag Barcode";

                    ui.SaveSuccessfull = false;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Team Counter Save successfull");

            }
            return ui;
        }

        //Add team counter when there is in no bagbarcode configuration : last test:8/2/2020
        //Used API's and UI : POLimitCheck 
        [Produces("application/json")]
        [HttpPost("AddTeamCounterWithoutBagBarcode")]
        public UserInput AddTeamCounterWithoutBagBarcode(UserInput ui)
        {
            logger.InfoFormat("AddTeamCounterWithoutBagBarcode ui=[{0}]", ui);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);//, ui.guid);               

                int SeqNo = 0;

                TeamCounter TC = new TeamCounter();
                TC.WfdepinstId = ui.WfdepinstId;
                TC.CounterType = 1;
                TC.L1id = ui.StyleId;
                TC.L2id = ui.ScheduleId;
                TC.L3id = 0;
                TC.L4id = ui.ColorId;
                TC.L5id = 0;

                TC.Qty01 = ui.EnteredQtyGd;
                TC.Qty02 = 0;
                TC.Qty03 = 0;

                TC.CutQty = 0; //op1 from l5moops table and op2 from dedep 

                TC.BagBarCodeNo = "";
                TC.BagSize = 0;
                TC.BagStatus = 0;

                TC.CounterNumber = ui.CounterNumber;

                TC.RecStatus = 1;
                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.TeamCounter.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Team Counter Save successfull");

            }
            return ui;
        }

        //Start of New Version APIS - Created By NimanthaH || Checked 8-1-2020
        //Create Group - Create a new record in group barcode table using mode for wfid where wfid (wfid 100 : factories and wfid : 200 BFL)
        //Used API's and UI : UpdatePOCounter
        [Produces("application/json")]
        [HttpPost("CreateGroup")]
        public void CreateGroup(TeamCounter ui, int mode, uint WFId, int OperationCode)
        {
            logger.InfoFormat("CreateGroup ui=[{0}], mode={1}, WFId={1}", ui, mode, WFId);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);
                //if (true)
                //{
                //using (DCAPDbContext context = new DCAPDbContext())
                //{
                GroupBarcode TC = new GroupBarcode();
                TC.WFId = WFId;
                TC.WFDEPInstId = ui.WfdepinstId;
                TC.DEPId = (uint)ojbLukUp.GetBagDepIdbyWFInstId((int)ui.WfdepinstId);

                TC.L1id = ui.L1id;
                TC.L2id = ui.L2id;
                TC.L3id = ui.L3id;
                TC.L4id = ui.L4id;
                TC.L5id = ui.L5id;
                TC.BagBarCodeNo = ui.BagBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;

                TC.Qty01 = ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty03;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.CutQty = (int)ui.CutQty; // ojbLukUp.GetQuantityByDetails(ui.L1id, ui.L2id, ui.L3id, ui.L4id, 15, OperationCode); //op1 from l5moops table and op2 from dedep 

                TC.OperationCode = OperationCode;
                TC.WorkCenter = "";
                TC.RecStatus = ui.RecStatus;
                TC.TxnStatus = 0;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.GroupBarcode.Add(TC);
                dcap.SaveChanges();
                //}
                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        public Boolean UpdateTTOpsRecord(UserInput ui)
        {
            Boolean updateavilale = false;
            try
            {
                TTOpearation ttoperation = dcap.TTOpearation.Where(c => c.BarCodeNo == ui.JobNo && c.TxnMode == ui.BagTxnMode && c.Seq == ui.BagSeq && c.OperationCode == ui.OperationCode).FirstOrDefault();

                if (ttoperation != null)
                {
                    ttoperation.Qty01 = ttoperation.Qty01 + ui.QtytoSaveGd;
                    ttoperation.Qty02 = ttoperation.Qty02 + ui.QtytoSaveScrap;
                    ttoperation.Qty03 = ttoperation.Qty03 + ui.QtytoSaveRw;

                    ttoperation.ModifiedBy = ui.CreatedBy;
                    ttoperation.ModifiedMachine = ui.CreatedMachine;
                    ttoperation.ModifiedDateTime = DateTime.Now;

                    dcap.SaveChanges();
                    updateavilale = false;
                }
                else
                {
                    updateavilale = true;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

            return updateavilale;
        }

        public void AddTTOperation(UserInput ui)
        {
            logger.InfoFormat("CreateGroup ui=[{0}]", ui.ToString());
            try
            {
                TTOpearation objttopeartion = new TTOpearation();

                objttopeartion.Seq = ui.BagSeq;
                objttopeartion.BarCodeNo = ui.JobNo;
                objttopeartion.WFDEPInstId = ui.WfdepinstId;
                objttopeartion.WFId = ui.WFID;
                objttopeartion.TxnMode = ui.BagTxnMode == null ? 2 : (int)ui.BagTxnMode;
                objttopeartion.TxnDateTime = DateTime.Now;

                objttopeartion.Qty01 = (int)ui.QtytoSaveGd;
                objttopeartion.Qty02 = (int)ui.QtytoSaveScrap;
                objttopeartion.Qty03 = (int)ui.QtytoSaveRw;
                objttopeartion.Qty01NS = 0;
                objttopeartion.Qty02NS = 0;
                objttopeartion.Qty03NS = 0;

                objttopeartion.OperationCode = ui.OperationCode;
                objttopeartion.PlussMinus = ui.PlussMinus;
                objttopeartion.TeamId = (int)ui.TeamId;
                objttopeartion.RecStatus = 1;

                objttopeartion.CreatedBy = ui.CreatedBy;
                objttopeartion.CreatedDateTime = DateTime.Now;
                objttopeartion.CreatedMachine = ui.CreatedMachine;
                objttopeartion.ModifiedBy = ui.CreatedBy;
                objttopeartion.ModifiedDateTime = DateTime.Now;
                objttopeartion.ModifiedMachine = ui.CreatedMachine;

                dcap.TTOpearation.Add(objttopeartion);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Start of New Version APIS - Created By NimanthaH || Checked 8-1-2020
        //Create Group - Create a new record in group barcode table using mode for wfid where wfid (wfid 100 : factories and wfid : 200 BFL)
        //Used API's and UI : UpdatePOCounter
        [Produces("application/json")]
        [HttpPost("CreateBuddyGroup")]
        public void CreateBuddyGroup(BuudyTagCounter ui, int mode, uint WFId, int OperationCode, string BuddyBarcode)
        {
            logger.InfoFormat("CreateGroup ui=[{0}], mode={1}, WFId={1}", ui, mode, WFId);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);
                BusinessLogicsController business = new BusinessLogicsController(dcap);

                if (ojbLukUp.CheckForBuddyBarcode(ui.BagBarCodeNo))
                {
                    //using (DCAPDbContext context = new DCAPDbContext())
                    //{
                    var opcode = ojbLukUp.GetAdjacentNodesForGivenNode((int)ui.WfdepinstId, 2);
                    var PreOpcode = OperationCode;
                    if (opcode.Count != 0)
                    {
                        PreOpcode = (int)opcode[0].OperationCode;
                    }

                    GroupBarcode TC = new GroupBarcode();
                    TC.WFId = WFId;
                    TC.WFDEPInstId = ui.WfdepinstId;
                    TC.DEPId = 200;

                    TC.L1id = ui.L1id;
                    TC.L2id = ui.L2id;
                    TC.L3id = ui.L3id;
                    TC.L4id = ui.L4id;
                    TC.L5id = ui.L5id;
                    TC.BagBarCodeNo = BuddyBarcode;
                    TC.TxnMode = mode;
                    TC.SplitStatus = 0;

                    TC.Qty01 = ui.Qty01;
                    TC.Qty02 = ui.Qty02;
                    TC.Qty03 = ui.Qty03;
                    TC.Qty01NS = 0;
                    TC.Qty02NS = 0;
                    TC.Qty03NS = 0;

                    TC.CutQty = 0;//(int)ojbLukUp.GetQuantityByDetails(ui.L1id, ui.L2id, ui.L3id, ui.L4id, 15, 151); //op1 from l5moops table and op2 from dedep 

                    TC.OperationCode = OperationCode;
                    TC.WorkCenter = ui.BagBarCodeNo;
                    TC.RecStatus = ui.RecStatus;
                    TC.TxnStatus = PreOpcode;

                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.GroupBarcode.Add(TC);
                    dcap.SaveChanges();
                    //}
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        public void CreateBuddyGroupMapping(BuudyTagCounter ui, int mode, uint WFId, int OperationCode, string BuddyBarcode)
        {
            logger.InfoFormat("CreateGroup ui=[{0}], mode={1}, WFId={1}", ui, mode, WFId);
            try
            {
                GroupBarcodeMapping TC = new GroupBarcodeMapping();

                TC.WFDEPInstId = (int)ui.WfdepinstId;
                TC.MotherBarcode = BuddyBarcode;
                TC.MotherTxnMode = mode;
                TC.ChildBarcode = ui.TravelBarCodeNo;
                TC.ChildTxnMode = 2;
                TC.Qty01 = 0;
                TC.Qty02 = ui.RRType == 2 ? ui.Qty01 : 0;
                TC.Qty03 = ui.RRType == 3 ? ui.Qty01 : 0;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;
                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = ui.CreatedDateTime;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.ModifiedBy;
                TC.ModifiedDateTime = ui.ModifiedDateTime;
                TC.ModifiedMachine = ui.ModifiedMachine;

                dcap.GroupBarcodeMapping.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        public void UpdateTravelBarcodeNo(string BuudyBagBarcode, uint Wfinstid, uint L1id, int RRid, string TravelBarcode, string BuudyBarcode)
        {
            try
            {
                List<Detxn> detxn = dcap.Detxn.Where(c => c.L1id == L1id && c.WfdepinstId == Wfinstid && c.Rrid == RRid && c.TravelBarCodeNo == BuudyBagBarcode && c.JobNo == TravelBarcode).ToList();

                detxn.ForEach(c => c.TravelBarCodeNo = BuudyBarcode);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        [Produces("application/json")]
        [HttpGet("POCounterIncrement")]
        public void POCounterIncrement(UserInput ui)
        {
            using (var trans = dcap.Database.BeginTransaction())
            {
                TransactionController TxnContrl = new TransactionController(dcap);
                logger.InfoFormat("POCounterIncrement ui=[{0}]", ui);
                try
                {
                    TeamCounter TC = new TeamCounter();
                    TC = dcap.TeamCounter
                            .Where(c => c.L1id == ui.StyleId &&
                            c.L2id == ui.ScheduleId && c.L4id == ui.ColorId &&
                            c.RecStatus == (int)eRecStatus.Active &&
                            c.WfdepinstId == ui.WfdepinstId)
                            .FirstOrDefault();

                    if (TC != null)
                    {
                        TxnContrl.UpdatePOCounter(TC.CounterId, 3, ui.EnteredQtyGd, 0, ui.BagBarCodeNo, (int)ui.WfdepinstId, ui.WFID, ui.OperationCode);
                        trans.Commit();
                    }
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                    throw e;
                    trans.Rollback();
                }
            }
        }

        //Update PO Counter -FOR BAG COUNTER || Checked 8-2-2020
        //Add Remove or Update Details of Team Counter And Group Barcode Table
        //Used API's and UI : UpdatePOCounter (Business controller) , BCScanData => DbUpdate (Business controller)
        [Produces("application/json")]
        [HttpGet("UpdatePOCounter")]
        public Boolean UpdatePOCounter(int CounterId, int Action, int Qty01, int BagSize, string BagBarcode, int WfdepinstId, uint WFId, int OperationCode)
        {
            logger.InfoFormat("UpdatePOCounter CounterId={0}, Action={0}, Qty01={0}, BagSize={0}, BagBarcode={0}, WfdepinstId={0}, WFId={0}, ", CounterId, Action, Qty01, BagSize, BagBarcode, WfdepinstId, WFId);
            //Action = 1 Clear     |       Action = 2 Reset      |       Action = 3 Increment
            Boolean updatesucess = true;

            TransactionController TxnContrl = new TransactionController(dcap);
            LookupController ojbLukUp = new LookupController(dcap);

            try
            {
                if (CounterId != 0)
                {
                    TeamCounter TC = new TeamCounter();
                    TC = dcap.TeamCounter
                            .Where(c => c.CounterId == CounterId && c.WfdepinstId == WfdepinstId && c.BagBarCodeNo == BagBarcode)
                            .FirstOrDefault();

                    if (TC != null)
                    {
                        DateTime ModefiedDateTime = TC.ModifiedDateTime;
                        if (Action == 1)
                        {
                            if (ojbLukUp.CheckForBagBarcode(TC.BagBarCodeNo, 2))
                            {
                                TxnContrl.CreateGroup(TC, 1, WFId, OperationCode);
                                TxnContrl.CreateBagGroupDetails(TC, OperationCode);
                                dcap.TeamCounter.Remove(TC);
                            }
                            else
                            {
                                updatesucess = false;
                            }
                        }
                        else if (Action == 2)
                        {
                            TC.BagSize = BagSize;
                            TC.ModifiedDateTime = ModefiedDateTime;
                            TC.CutQty = TC.IntCutQty - (int)ojbLukUp.GetQuantityByDetails(2, TC.L1id, TC.L2id, TC.L3id, TC.L4id, 0, 15, OperationCode);
                        }
                        else if (Action == 3)
                        {
                            if (TC.Qty01 + Qty01 == 0)
                            {
                                dcap.TeamCounter.Remove(TC);
                            }
                            else
                            {
                                TC.Qty01 = TC.Qty01 + Qty01;

                                TC.CutQty = TC.IntCutQty - (int)ojbLukUp.GetQuantityByDetails(2, TC.L1id, TC.L2id, TC.L3id, TC.L4id, 0, 15, OperationCode);

                                TC.ModifiedDateTime = DateTime.Now;
                                TC.ModifiedBy = TC.ModifiedBy;
                                TC.ModifiedMachine = TC.ModifiedMachine;
                            }
                        }
                    }
                }
                else
                {
                    if (Action == 3)
                    {
                        GroupBarcode TB = new GroupBarcode();
                        TB = dcap.GroupBarcode
                                .Where(c => c.BagBarCodeNo == BagBarcode && c.TxnMode == 1 && c.TxnStatus == 0 && c.WFDEPInstId == WfdepinstId)
                                .FirstOrDefault();

                        if (TB != null)
                        {
                            if (TB.TxnStatus == 0)
                            {
                                if (TB.Qty01 + Qty01 == 0)
                                {
                                    dcap.GroupBarcode.Remove(TB);
                                }
                                else
                                {
                                    TB.Qty01 = TB.Qty01 + Qty01;
                                    TB.CutQty = TB.CutQty - Qty01;

                                    TB.ModifiedDateTime = DateTime.Now;
                                    TB.ModifiedBy = TB.ModifiedBy;
                                    TB.ModifiedMachine = TB.ModifiedMachine;
                                }
                            }
                            else
                            {
                                updatesucess = false;
                            }
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


        public Boolean UpdatePOCounterOutSource(int CounterId, int Action, int Qty01, int BagSize, string BagBarcode, int WfdepinstId, uint WFId, int OperationCode)
        {
            logger.InfoFormat("UpdatePOCounter CounterId={0}, Action={0}, Qty01={0}, BagSize={0}, BagBarcode={0}, WfdepinstId={0}, WFId={0}, ", CounterId, Action, Qty01, BagSize, BagBarcode, WfdepinstId, WFId);
            //Action = 1 Clear     |       Action = 2 Reset      |       Action = 3 Increment
            Boolean updatesucess = true;

            TransactionController TxnContrl = new TransactionController(dcap);
            LookupController ojbLukUp = new LookupController(dcap);

            try
            {
                if (CounterId != 0)
                {
                    TeamCounter TC = new TeamCounter();
                    TC = dcap.TeamCounter
                            .Where(c => c.CounterId == CounterId && c.WfdepinstId == WfdepinstId && c.BagBarCodeNo == BagBarcode)
                            .FirstOrDefault();

                    if (TC != null)
                    {
                        DateTime ModefiedDateTime = TC.ModifiedDateTime;
                        if (Action == 1)
                        {
                            if (ojbLukUp.CheckForBagBarcode(TC.BagBarCodeNo, 2))
                            {
                                TxnContrl.CreateGroup(TC, 1, WFId, OperationCode);
                                TxnContrl.CreateBagGroupDetails(TC, OperationCode);
                                dcap.TeamCounter.Remove(TC);
                            }
                            else
                            {
                                updatesucess = false;
                            }
                        }
                        else if (Action == 2)
                        {
                            TC.BagSize = BagSize;
                            TC.ModifiedDateTime = ModefiedDateTime;
                            GroupBarcode objgroupbarcode = ojbLukUp.GetQtyByGroupBarcodeIds(0, 0, TC.L1id, TC.L2id, 0, TC.L4id, TC.L5id, 3, OperationCode);
                            TeamCounter objteamcounter = ojbLukUp.GetQtyByTeamCounterIds(0, 0, TC.L1id, TC.L2id, 0, TC.L4id, TC.L5id, 3);
                            TC.CutQty = TC.IntCutQty - (int)(((objgroupbarcode == null ? 0 : objgroupbarcode.Qty01) + (int)Qty01 + (objteamcounter == null ? 0 : objteamcounter.Qty01)));
                        }
                        else if (Action == 3)
                        {
                            if (TC.Qty01 + Qty01 == 0)
                            {
                                dcap.TeamCounter.Remove(TC);
                            }
                            else
                            {
                                TC.Qty01 = TC.Qty01 + Qty01;
                                GroupBarcode objgroupbarcode = ojbLukUp.GetQtyByGroupBarcodeIds(0, 0, TC.L1id, TC.L2id, 0, TC.L4id, TC.L5id, 3, OperationCode);
                                TeamCounter objteamcounter = ojbLukUp.GetQtyByTeamCounterIds(0, 0, TC.L1id, TC.L2id, 0, TC.L4id, TC.L5id, 3);
                                TC.CutQty = TC.IntCutQty - (int)(((objgroupbarcode == null ? 0 : objgroupbarcode.Qty01) + (int)Qty01 + (objteamcounter == null ? 0 : objteamcounter.Qty01)));

                                //TC.CutQty = TC.IntCutQty - (int)ojbLukUp.GetQuantityByDetails(2, TC.L1id, TC.L2id, TC.L3id, TC.L4id, 15, OperationCode);

                                TC.ModifiedDateTime = DateTime.Now;
                                TC.ModifiedBy = TC.ModifiedBy;
                                TC.ModifiedMachine = TC.ModifiedMachine;
                            }
                        }
                    }
                }
                else
                {
                    if (Action == 3)
                    {
                        GroupBarcode TB = new GroupBarcode();
                        TB = dcap.GroupBarcode
                                .Where(c => c.BagBarCodeNo == BagBarcode && c.TxnMode == 1 && c.TxnStatus == 0 && c.WFDEPInstId == WfdepinstId)
                                .FirstOrDefault();

                        if (TB != null)
                        {
                            if (TB.TxnStatus == 0)
                            {
                                if (TB.Qty01 + Qty01 == 0)
                                {
                                    dcap.GroupBarcode.Remove(TB);
                                }
                                else
                                {
                                    TB.Qty01 = TB.Qty01 + Qty01;
                                    TB.CutQty = TB.CutQty - Qty01;

                                    TB.ModifiedDateTime = DateTime.Now;
                                    TB.ModifiedBy = TB.ModifiedBy;
                                    TB.ModifiedMachine = TB.ModifiedMachine;
                                }
                            }
                            else
                            {
                                updatesucess = false;
                            }
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

        //Update PO Counter -FOR BAG COUNTER || Checked 8-2-2020
        //Add Remove or Update Details of Team Counter And Group Barcode Table
        //Used API's and UI : UpdatePOCounter (Business controller) , BCScanData => DbUpdate (Business controller)
        [Produces("application/json")]
        [HttpGet("UpdateBuddyCounter")]
        public Boolean UpdateBuddyCounter(int CounterId, int Action, int Qty01, int BagSize, string BagBarcode, int WfdepinstId, uint WFId, int OperationCode)
        {
            logger.InfoFormat("UpdatePOCounter CounterId={0}, Action={0}, Qty01={0}, BagSize={0}, BagBarcode={0}, WfdepinstId={0}, WFId={0}, ", CounterId, Action, Qty01, BagSize, BagBarcode, WfdepinstId, WFId, OperationCode);
            //Action = 1 Clear     |       Action = 2 Reset      |       Action = 3 Increment
            Boolean updatesucess = true;

            TransactionController TxnContrl = new TransactionController(dcap);
            LookupController lookup = new LookupController(dcap);

            try
            {
                if (CounterId != 0)
                {
                    BuudyTagCounter TC = new BuudyTagCounter();
                    TC = dcap.BuudyTagCounter
                            .Where(c => c.CounterId == CounterId && c.BagBarCodeNo == BagBarcode && c.WfdepinstId == WfdepinstId)
                            .FirstOrDefault();

                    if (TC != null)
                    {
                        DateTime ModefiedDateTime = TC.ModifiedDateTime;
                        if (Action == 1)
                        {
                            String BuddyBarcode = lookup.GetBuddyBarcode();
                            TxnContrl.UpdateTravelBarcodeNo(TC.BagBarCodeNo, TC.WfdepinstId, TC.L1id, TC.RRId, TC.TravelBarCodeNo, BuddyBarcode);
                            TxnContrl.CreateBuddyGroupDetailsTC(TC, 3, WFId, OperationCode, BuddyBarcode);
                            TxnContrl.CreateBuddyGroup(TC, 3, WFId, OperationCode, BuddyBarcode);
                            TxnContrl.CreateBuddyGroupMapping(TC, 3, WFId, OperationCode, BuddyBarcode);
                            dcap.BuudyTagCounter.Remove(TC);
                        }
                        else if (Action == 2)
                        {
                            TC.BagSize = BagSize;
                            TC.ModifiedDateTime = ModefiedDateTime;
                            //LookupController ojbLukUp = new LookupController(dcap);
                            //TC.CutQty = (int)ojbLukUp.GetQuantityByDetails(TC.L1id, TC.L2id, TC.L3id, TC.L4id, 15, 151);
                        }
                        else if (Action == 3)
                        {
                            if (TC.Qty01 + Qty01 == 0)
                            {
                                dcap.BuudyTagCounter.Remove(TC);
                            }
                            else
                            {
                                TC.Qty01 = TC.Qty01 + Qty01;

                                //LookupController ojbLukUp = new LookupController(dcap);
                                //TC.CutQty = (int)ojbLukUp.GetQuantityByDetails(TC.L1id, TC.L2id, TC.L3id, TC.L4id, 15, 151);

                                TC.ModifiedDateTime = DateTime.Now;
                                TC.ModifiedBy = TC.ModifiedBy;
                                TC.ModifiedMachine = TC.ModifiedMachine;
                            }
                        }
                    }
                }
                else
                {
                    if (Action == 3)
                    {
                        GroupBarcode TB = new GroupBarcode();
                        TB = dcap.GroupBarcode
                                .Where(c => c.BagBarCodeNo == BagBarcode && c.TxnMode == 3 && c.TxnStatus == 6 && c.WFDEPInstId == WfdepinstId)
                                .FirstOrDefault();

                        if (TB != null)
                        {
                            if (TB.TxnStatus == 6)
                            {
                                if (TB.Qty01 + Qty01 == 0)
                                {
                                    TravelBarcodeDetails TBD = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == TB.BagBarCodeNo).FirstOrDefault();
                                    dcap.TravelBarcodeDetails.Remove(TBD);
                                    dcap.GroupBarcode.Remove(TB);
                                }
                                else
                                {
                                    TB.Qty01 = TB.Qty01 + Qty01;
                                    //TB.CutQty = TB.CutQty - Qty01;

                                    TB.ModifiedDateTime = DateTime.Now;
                                    TB.ModifiedBy = TB.ModifiedBy;
                                    TB.ModifiedMachine = TB.ModifiedMachine;
                                }
                            }
                            else
                            {
                                updatesucess = false;
                            }
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

        [Produces("application/json")]
        [HttpPost("CreateBagGroupDetails")]
        public void CreateBagGroupDetails(TeamCounter ui, int OperationCode)
        {
            logger.InfoFormat("CreateGroup ui=[{0}], OperationCode={1}", ui, OperationCode);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);
                SecurityController ojbSecUp = new SecurityController(dcap);
                List<Detxn> L1Details = new List<Detxn>();
                if (ui.L5id == 0)
                {
                    L1Details = L1Details.Concat(ojbLukUp.GetBarcodeDetailsFromBagTag(ui.BagBarCodeNo, ui.L1id, ui.L2id, ui.L3id, ui.L4id, OperationCode)).ToList();

                    if (L1Details.Count != 0)
                    {
                        foreach (Detxn qi in L1Details)
                        {
                            Group_Barcode_Detail TC = new Group_Barcode_Detail();

                            TC.WfdepinstId = qi.WfdepinstId;
                            TC.Seq = 0;

                            TC.L1id = qi.L1id;
                            TC.L2id = qi.L2id;
                            TC.L3id = qi.L3id;
                            TC.L4id = qi.L4id;
                            TC.L5id = qi.L5id;
                            TC.L5moid = qi.L5moid;
                            TC.L5mono = qi.L5mono;
                            TC.BarCodeNo = qi.BagBarCodeNo;
                            TC.TxnMode = 1;
                            TC.AlterBarcode = "";
                            TC.AlterTxnMode = 0;

                            TC.Wfid = qi.Wfid;
                            TC.Depid = qi.Depid;
                            TC.TeamId = qi.TeamId;
                            TC.Dclid = qi.Dclid;
                            TC.Dcmid = qi.Dcmid;

                            TC.TxnDateTime = DateTime.Now;

                            TC.EnteredBy = ui.CreatedBy;
                            TC.RecStatus = 1;

                            TC.Qty01 = qi.Qty01;
                            TC.Qty02 = qi.Qty02;
                            TC.Qty03 = qi.Qty03;
                            TC.Qty01Ns = 0;
                            TC.Qty02Ns = 0;
                            TC.Qty03Ns = 0;

                            TC.CreatedBy = ui.CreatedBy;
                            TC.CreatedDateTime = DateTime.Now;
                            TC.CreatedMachine = ui.CreatedMachine;
                            TC.ModifiedBy = ui.CreatedBy;
                            TC.ModifiedDateTime = DateTime.Now;
                            TC.ModifiedMachine = ui.CreatedMachine;

                            dcap.Group_Barcode_Detail.Add(TC);
                            dcap.SaveChanges();
                        }
                    }
                }
                else
                {
                    var wfdep = ojbSecUp.GetWfDEPInstByWfInstId(ui.WfdepinstId);
                    Group_Barcode_Detail TC = new Group_Barcode_Detail();

                    TC.WfdepinstId = ui.WfdepinstId;
                    TC.Seq = 0;

                    TC.L1id = ui.L1id;
                    TC.L2id = ui.L2id;
                    TC.L3id = ui.L3id;
                    TC.L4id = ui.L4id;
                    TC.L5id = ui.L5id;
                    TC.L5moid = 0; //ui.L5moid;
                    TC.L5mono = ""; //ui.L5mono;
                    TC.BarCodeNo = ui.BagBarCodeNo;
                    TC.TxnMode = 1;
                    TC.AlterBarcode = "";
                    TC.AlterTxnMode = 0;

                    TC.Wfid = (uint)wfdep.Wfid;
                    TC.Depid = wfdep.Depid;
                    TC.TeamId = wfdep.TeamId;
                    TC.Dclid = wfdep.Dclid;
                    TC.Dcmid = wfdep.Dcmid;

                    TC.TxnDateTime = DateTime.Now;

                    TC.EnteredBy = ui.CreatedBy;
                    TC.RecStatus = 1;

                    TC.Qty01 = ui.Qty01;
                    TC.Qty02 = ui.Qty02;
                    TC.Qty03 = ui.Qty03;
                    TC.Qty01Ns = 0;
                    TC.Qty02Ns = 0;
                    TC.Qty03Ns = 0;

                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.Group_Barcode_Detail.Add(TC);
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //NO
        public void UpdateStatus(int IsSucess, string ErrorCode, string ErrorDescription, string CreatedBy, uint DetxnKey)
        {
            logger.InfoFormat("UpdatePOCounter IsSucess={0}, ErrorCode={0}, ErrorDescription={0}, CreatedBy={0}, DetxnKey={0}", IsSucess, ErrorCode, ErrorDescription, CreatedBy, DetxnKey);
            try
            {
                Detxn objdetxn = new Detxn();
                objdetxn = dcap.Detxn
                        .Where(c => c.DetxnKey == DetxnKey)
                        .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.UploadStatus = IsSucess == 1 ? 2 : 4;
                    objdetxn.ErrorCode = ErrorCode != null ? Convert.ToInt32(ErrorCode) : 0;
                    objdetxn.ErrorDescription = ErrorDescription != null ? ErrorDescription.Trim() : "";
                    objdetxn.HasError = IsSucess == 2 ? 1 : 0;
                    objdetxn.UploadBy = CreatedBy;
                    objdetxn.UploadTime = DateTime.Now;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        /*
        //Update DEDEP table API
        [Produces("application/json")]
        [HttpGet("UpdateDEDEPInst")]
        public void UpdateDEDEP(UserInput ui)
        {
            Dedep objdedep = new Dedep();
            objdedep = dcap.Dedep
                    .Where(c => c.Wfid == ui.WFID && c.Depid == ui.Depid && c.L1id == ui.StyleId
                    && c.L2id == ui.ScheduleId && c.L3id == 0 && c.L4id == ui.ColorId && c.L5moid == ui.L5MOID)
                   .FirstOrDefault();

            objdedep.Qty01 = ui.EnteredQty;
            objdedep.Qty02 = ui.EnteredQty1;
            objdedep.Qty03 = ui.EnteredQty2;
            objdedep.ModifiedDateTime = DateTime.Now;
            objdedep.ModifiedBy = ui.ModifiedBy;
            objdedep.ModifiedMachine = ui.ModifiedMachine;
            dcap.SaveChanges();

        }
         */

        //Add Good Control table data || checked 8-4-2020
        //Used API's and UI : UpdateDispatch (Businesscontrollers)
        [Produces("application/json")]
        [HttpPost("AddGoodControl")]
        public void AddGoodControl(DispatchInput ui)
        {
            logger.InfoFormat("AddGoodControl ui={0}", ui);
            try
            {
                GoodControl objgoodcontrol = new GoodControl();

                objgoodcontrol.ControlId = ui.DispatchBarcode;
                objgoodcontrol.ControlType = ui.Type;
                objgoodcontrol.BarCodeNo = ui.BagBarcode;
                objgoodcontrol.L1id = ui.L1idBag;
                objgoodcontrol.L2id = ui.L2idBag;
                objgoodcontrol.L3id = ui.L3idBag;
                objgoodcontrol.L4id = ui.L4idBag;
                objgoodcontrol.L5id = ui.L5idBag;

                objgoodcontrol.TxnMode = (int)ui.TxnMode;
                objgoodcontrol.TxnDateTime = DateTime.Now;
                objgoodcontrol.Qty01 = ui.Qty01;
                objgoodcontrol.Qty02 = 0;
                objgoodcontrol.Qty03 = 0;
                objgoodcontrol.TxnStatus = (int)ui.TxnStatus;
                objgoodcontrol.Remark = ui.Remark;
                objgoodcontrol.IsSucess = 0;
                objgoodcontrol.RecStatus = 1;
                objgoodcontrol.Return = ui.Return;

                objgoodcontrol.CreatedDateTime = DateTime.Now;
                objgoodcontrol.CreatedBy = ui.CreatedBy;
                objgoodcontrol.CreatedMachine = ui.CreatedMachine;
                objgoodcontrol.ModifiedDateTime = DateTime.Now;
                objgoodcontrol.ModifiedBy = ui.ModifiedBy;
                objgoodcontrol.ModifiedMachine = ui.ModifiedMachine;

                dcap.GoodControl.Add(objgoodcontrol);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving information {0}", e.ToString());
                throw e;
            }

        }

        //Add Good Control table detail data || checked 8-4-2020
        //Used API's and UI : UpdateDispatch (Businesscontrollers)
        [Produces("application/json")]
        [HttpPost("AddGoodControlDetail")]
        public void AddGoodControlDetail(DispatchInput ui)
        {
            logger.InfoFormat("AddGoodControlDetail ui={0}", ui);
            try
            {
                int SeqNo = 0;
                string locfromcode = null;

                GoodControlDetails objgoodcontrol = new GoodControlDetails();

                objgoodcontrol.Seq = (uint)ui.SeqG;
                objgoodcontrol.ControlId = ui.DispatchBarcode;
                objgoodcontrol.ControlType = ui.Type;
                objgoodcontrol.Return = ui.Return;
                objgoodcontrol.WFId = (int)ui.Wfid;
                objgoodcontrol.Depid = (int)ui.departmentTo;
                objgoodcontrol.TxnDateTime = DateTime.Now;
                objgoodcontrol.Qty01 = ui.Qty01;
                objgoodcontrol.Qty02 = 0;
                objgoodcontrol.Qty03 = 0;
                objgoodcontrol.JobNo = "A";
                objgoodcontrol.OperationCode = (int)ui.OperationCode;
                objgoodcontrol.EnteredBy = ui.EnterdBy;
                objgoodcontrol.TxnStatus = (int)ui.TxnStatus;
                objgoodcontrol.ErrorCode = 0;
                objgoodcontrol.Remark = ui.Remark;
                objgoodcontrol.Approver = ui.Approver;
                objgoodcontrol.LocCodeFrom = ui.LocFrom;
                objgoodcontrol.LocCode = ui.LocCode;
                objgoodcontrol.ReceiverName = ui.receiverName;
                objgoodcontrol.ReceiverEmail = ui.receiverEmail;
                objgoodcontrol.WatcherEmail = ui.watcherEmail;
                objgoodcontrol.VehicleNo = ui.vehicleNo;
                objgoodcontrol.ApprovalStatus = ui.ApprovalStatus;
                objgoodcontrol.InvoiceStatus = 0;

                objgoodcontrol.CreatedDateTime = DateTime.Now;
                objgoodcontrol.CreatedBy = ui.CreatedBy;
                objgoodcontrol.CreatedMachine = ui.CreatedMachine;
                objgoodcontrol.ModifiedDateTime = DateTime.Now;
                objgoodcontrol.ModifiedBy = ui.ModifiedBy;
                objgoodcontrol.ModifiedMachine = ui.ModifiedMachine;

                dcap.GoodControlDetails.Add(objgoodcontrol);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving information {0}", e.ToString());
                throw e;
            }

        }

        //Upload Barcode Status
        //Update Txn Status of individual barcode to given status || checked 8-4-2020
        //Used API's and UI : UpdateDispatch, UpdateDispatchStatus (Businesscontrollers)
        public void UpdateBarcodeStatus(DispatchInput ui)
        {
            logger.InfoFormat("UpdateBarcodeStatus ui={0}", ui);
            try
            {
                GroupBarcode objdetxn = new GroupBarcode();
                objdetxn = dcap.GroupBarcode
                            .Where(c => c.L1id == ui.L1idBag && c.L2id == ui.L2idBag && c.L3id == ui.L3idBag && c.L4id == ui.L4idBag && c.L5id == ui.L5idBag &&
                            c.BagBarCodeNo == ui.BagBarcode && c.Seq == ui.SeqBag && c.WFId == ui.WFIdBag && c.DEPId == ui.DEPIdBag && c.RecStatus == 1)
                            .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.TxnStatus = (int)ui.TxnStatus;
                    objdetxn.ModifiedDateTime = DateTime.Now;
                    objdetxn.ModifiedBy = ui.EnterdBy;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        public void UpdateGoodControlBarcodeReturnStatus(DispatchInput ui)
        {
            logger.InfoFormat("UpdateGoodControlBarcodeReturnStatus ui={0}", ui);
            try
            {
                GoodControl objdetxn = new GoodControl();
                objdetxn = dcap.GoodControl
                            .Where(c => c.L1id == ui.L1idBag && c.L2id == ui.L2idBag && c.L3id == ui.L3idBag && c.L4id == ui.L4idBag && c.L5id == ui.L5idBag &&
                            c.BarCodeNo == ui.BagBarcode && c.RecStatus == 1 && c.TxnStatus == 5)
                            .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.Return = 2; //Returened
                    objdetxn.ModifiedDateTime = DateTime.Now;
                    objdetxn.ModifiedBy = ui.EnterdBy;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }

        //Update Dispatch Status
        //Update Txn Status of individual barcode to given status in goodcontrol table|| checked 8-4-2020
        //Used API's and UI : UpdateDispatchStatus (Businesscontrollers)
        public void UpdateGoodControlStatus(DispatchInput ui)
        {
            logger.InfoFormat("UpdateGoodControlStatus ui={0}", ui);
            try
            {
                GoodControl objdetxn = new GoodControl();
                objdetxn = dcap.GoodControl
                        .Where(c => c.BarCodeNo == ui.BagBarcode && c.Seq == ui.SeqG && c.ControlId == ui.DispatchBarcode &&
                        c.L1id == ui.L1idBag && c.L2id == ui.L2idBag && c.L3id == ui.L3idBag && c.L4id == ui.L4idBag && c.L5id == ui.L5idBag)
                        .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.TxnStatus = (int)ui.TxnStatus;
                    objdetxn.WarLocCode = ui.WarLocCode;
                    if (ui.TxnStatus == 5)
                    {
                        objdetxn.IsSucess = 1;
                    }
                    else
                    {
                        objdetxn.IsSucess = 0;
                    }
                    objdetxn.ModifiedDateTime = DateTime.Now;
                    objdetxn.ModifiedBy = ui.ModifiedBy;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        //Update Dispatch Status in Good Control Details
        //Update Txn Statusin goodcontrol details table|| checked 8-4-2020
        //Used API's and UI : UpdateDispatchStatus (Businesscontrollers)
        public void UpdateGoodControlDetailsStatus(string controlid, int seq, int depid, int status, string enterdby)
        {
            logger.InfoFormat("UpdateGoodControlDetailsStatus controlid={0}, seq={1}, depid={2}, status={3}, enterdby={4}", controlid, seq, depid, status, enterdby);

            try
            {
                GoodControlDetails objdetxn = new GoodControlDetails();
                objdetxn = dcap.GoodControlDetails
                        .Where(c => c.Seq == seq && c.ControlId == controlid)
                        .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.TxnStatus = (int)status;
                    if (status == 2)
                    {
                        objdetxn.Approver = enterdby;
                        objdetxn.ApprovalStatus = 1;
                        objdetxn.ApprovedDateTime = DateTime.Now;
                        objdetxn.ModifiedDateTime = DateTime.Now;
                    }
                    else if (status == 3)
                    {
                        objdetxn.SecurityPassedBy = enterdby;
                        objdetxn.SecurityPassedDateTime = DateTime.Now;
                        objdetxn.ModifiedDateTime = DateTime.Now;
                    }
                    else if (status == 4)
                    {
                        objdetxn.SecurityReceivedBy = enterdby;
                        objdetxn.SecurityReceivedDateTime = DateTime.Now;
                        objdetxn.ModifiedDateTime = DateTime.Now;
                    }
                    else if (status == 5)
                    {
                        objdetxn.ClosedBy = enterdby;
                        objdetxn.ClosedDateTime = DateTime.Now;
                        objdetxn.ModifiedDateTime = DateTime.Now;
                    }
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        //Create Travel Tag to Group
        //Create Travel Group Details on group_barcode table|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatus (Transactioncontrollers)
        public void CreateTravelGroup(TravelBarcodeInputs ui, int mode, int OperationCode)
        {
            logger.InfoFormat("CreateTravelGroupDetail ui={0}", ui);
            try
            {
                GroupBarcode TC = new GroupBarcode();

                TC.WFId = ui.WFId;
                TC.WFDEPInstId = ui.WFDEPInstId;
                TC.DEPId = 200;

                TC.L1id = ui.L1id;
                TC.L2id = ui.L2id;
                TC.L3id = ui.L3id;
                TC.L4id = ui.L4id;
                TC.L5id = 0;
                TC.BagBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;

                TC.Qty01 = (int)ui.Qty01;
                TC.Qty02 = 0;
                TC.Qty03 = 0;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.CutQty = 0;

                TC.OperationCode = OperationCode;
                TC.WorkCenter = ui.WorkCenter;
                TC.RecStatus = 1;
                TC.TxnStatus = OperationCode;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.GroupBarcode.Add(TC);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        public void CreateTravelGroupOutSource(TravelBarcodeInputs ui, int mode, int OperationCode)
        {
            logger.InfoFormat("CreateTravelGroupDetail ui={0}", ui);
            try
            {
                GroupBarcode TC = new GroupBarcode();

                TC.WFId = ui.WFId;
                TC.WFDEPInstId = ui.WFDEPInstId;
                TC.DEPId = 200;

                TC.L1id = ui.L1id;
                TC.L2id = ui.L2id;
                TC.L3id = ui.L3id;
                TC.L4id = ui.L4id;
                TC.L5id = 0;
                TC.BagBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;
                TC.SMode = 1;

                TC.Qty01 = (int)ui.Qty01;
                TC.Qty02 = 0;
                TC.Qty03 = 0;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.CutQty = 0;

                TC.OperationCode = OperationCode;
                TC.WorkCenter = ui.WorkCenter;
                TC.RecStatus = 1;
                TC.TxnStatus = OperationCode;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.GroupBarcode.Add(TC);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Update Deleted Travel Tag to Group
        public void CreateDeletedGroup(TravelBarcodeInputs qi)
        {
            logger.InfoFormat("CreateDeletedGroup TravelBarcodeInputs={0}", qi);
            try
            {
                GroupBarcode ui = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == qi.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == qi.L1id && c.L2id == qi.L2id && c.L3id == qi.L3id && c.L4id == qi.L4id)
                        .FirstOrDefault();

                if (ui != null)
                {
                    DeletedGroupBarcode TC = new DeletedGroupBarcode();

                    TC.WFId = ui.WFId;
                    TC.WFDEPInstId = ui.WFDEPInstId;
                    TC.DEPId = ui.DEPId;

                    TC.L1id = ui.L1id;
                    TC.L2id = ui.L2id;
                    TC.L3id = ui.L3id;
                    TC.L4id = ui.L4id;
                    TC.L5id = ui.L5id;
                    TC.BagBarCodeNo = ui.BagBarCodeNo;
                    TC.TxnMode = ui.TxnMode;
                    TC.SplitStatus = ui.SplitStatus;

                    TC.Qty01 = ui.Qty01;
                    TC.Qty02 = ui.Qty02;
                    TC.Qty03 = ui.Qty03;
                    TC.Qty01NS = ui.Qty01NS;
                    TC.Qty02NS = ui.Qty02NS;
                    TC.Qty03NS = ui.Qty03NS;

                    TC.CutQty = ui.CutQty;

                    TC.OperationCode = ui.OperationCode;
                    TC.WorkCenter = ui.WorkCenter;
                    TC.RecStatus = ui.RecStatus;
                    TC.TxnStatus = ui.TxnStatus;

                    TC.CreatedBy = ui.CreatedBy;
                    TC.CreatedDateTime = DateTime.Now;
                    TC.CreatedMachine = ui.CreatedMachine;
                    TC.ModifiedBy = ui.CreatedBy;
                    TC.ModifiedDateTime = DateTime.Now;
                    TC.ModifiedMachine = ui.CreatedMachine;

                    dcap.DeletedGroupBarcode.Add(TC);
                    dcap.SaveChanges();
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Create New Group Mapping Details
        public void CreateGroupMap(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("CreateTravelGroupDetail ui={0}", ui);
            try
            {
                GroupBarcodeMapping TR = new GroupBarcodeMapping();

                TR.WFDEPInstId = (int)ui.WFDEPInstId;
                TR.MotherBarcode = ui.TravelBarCodeNo;
                TR.MotherTxnMode = 2;

                TR.ChildBarcode = (ui.TxnMode == 0 ? ui.Bag_Barcode : ui.Barcode);
                TR.ChildTxnMode = 1;

                TR.Qty01 = ui.Qty01NS;
                TR.Qty02 = ui.Qty02NS;
                TR.Qty03 = ui.Qty03NS;
                TR.Qty01NS = ui.Qty01;
                TR.Qty02NS = ui.Qty02;
                TR.Qty03NS = ui.Qty03;

                TR.CreatedBy = ui.CreatedBy;
                TR.CreatedDateTime = DateTime.Now;
                TR.CreatedMachine = ui.CreatedMachine;
                TR.ModifiedBy = ui.ModifiedBy;
                TR.ModifiedMachine = ui.ModifiedMachine;
                TR.ModifiedDateTime = DateTime.Now;

                dcap.GroupBarcodeMapping.Add(TR);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
        }
        //Create Travel Tag to  Detail
        //Create Travel Group Details on group_barcode table
        public void CreateTravelGroupDetail(Group_Barcode_Detail ui, int Txnmode, string Barcode, string createdby, string createdmachine)
        {
            logger.InfoFormat("CreateTravelGroup ui={0}, Txnmode={1}, Barcode={2}", ui, Txnmode, Barcode);
            try
            {
                Group_Barcode_Detail TC = new Group_Barcode_Detail();

                TC.Wfid = ui.Wfid;
                TC.WfdepinstId = ui.WfdepinstId;
                TC.Depid = ui.Depid;

                TC.L1id = ui.L1id;
                TC.L2id = ui.L2id;
                TC.L3id = ui.L3id;
                TC.L4id = ui.L4id;
                TC.L5id = ui.L5id;
                TC.L5moid = (int?)ui.L5moid;
                TC.L5mono = ui.L5mono;
                TC.Dclid = ui.Dclid;
                TC.Dcmid = ui.Dcmid;
                TC.BarCodeNo = Barcode;
                TC.TxnMode = Txnmode;
                TC.TxnDateTime = DateTime.Now;
                TC.AlterBarcode = "";
                TC.AlterTxnMode = 0;

                TC.Qty01 = (int)ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty02;
                TC.Qty01Ns = 0;
                TC.Qty02Ns = 0;
                TC.Qty03Ns = 0;

                TC.RecStatus = 1;

                TC.CreatedBy = createdby;
                TC.EnteredBy = createdby;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = createdmachine;
                TC.ModifiedBy = createdby;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = createdmachine;

                dcap.Group_Barcode_Detail.Add(TC);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Update Qty01NS in group barcode detail when creating travel tag - bag mode
        public List<Group_Barcode_Detail> GetBarcodesDetailsByGroupBarcodeDetails(TravelBarcodeInputs ui)
        {

            logger.InfoFormat("Get Barcodes Details By Travel Barcode ui ={0}", ui);
            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();

            try
            {
                barcodedetail = dcap.Group_Barcode_Detail.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BarCodeNo == ui.Barcode && c.TxnMode == ui.TxnMode).ToList();

                if (barcodedetail.Count != 0)
                {
                    if (ui.TxnMode == 1)
                    {
                        barcodedetail.ForEach(c => c.Qty01Ns = (c.Qty01Ns + c.Qty01));
                        barcodedetail.ForEach(c => c.AlterBarcode = ui.TravelBarCodeNo);
                        barcodedetail.ForEach(c => c.AlterTxnMode = 2);
                        dcap.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return barcodedetail;
        }

        public List<Group_Barcode_Detail> UpdateAnyBarcodesDetailsByGroupBarcodeDetails(TravelBarcodeInputs ui)
        {

            logger.InfoFormat("UpdateAnyBarcodesDetailsByGroupBarcodeDetails ui ={0}", ui);
            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();

            try
            {
                if (ui.TxnMode == 2)
                {
                    barcodedetail = dcap.Group_Barcode_Detail.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BarCodeNo == ui.TravelBarCodeNo && c.TxnMode == ui.TxnMode).ToList();
                }
                else
                {
                    barcodedetail = dcap.Group_Barcode_Detail.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BarCodeNo == ui.Barcode && c.TxnMode == ui.TxnMode).ToList();
                }

                if (barcodedetail.Count != 0)
                {
                    if (ui.TxnMode == 1)
                    {
                        foreach (Group_Barcode_Detail c in barcodedetail)
                        {
                            c.Qty01Ns = (c.Qty01Ns + c.Qty01);
                            if (ui.Qty01 < 0)
                            {
                                if (c.Qty01Ns == 0)
                                {
                                    dcap.Group_Barcode_Detail.Remove(c);
                                }
                            }
                            else
                            {
                                c.AlterBarcode = ui.TravelBarCodeNo;
                                c.AlterTxnMode = 2;
                            }
                        }

                        dcap.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return barcodedetail;
        }

        //Update Qty01NS in group barcode detail when creating travel tag - garment mode
        public List<Group_Barcode_Detail> GetBarcodesDetailsByDetxn(Detxn ui)
        {

            logger.InfoFormat("Get Barcodes Details By Travel Barcode ui ={0}", ui);
            List<Group_Barcode_Detail> barcodedetail = new List<Group_Barcode_Detail>();

            try
            {
                barcodedetail = dcap.Group_Barcode_Detail.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.L5moid == ui.L5moid && c.BarCodeNo == ui.BagBarCodeNo && c.TxnMode == 1).ToList();

                if (barcodedetail.Count != 0)
                {
                    //if(ui.TxnMode == 1){
                    barcodedetail.ForEach(c => c.Qty01Ns = (c.Qty01Ns + 1));
                    dcap.SaveChanges();
                    //}
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorSheduleByBarcode information {0}", e.ToString());

            }
            return barcodedetail;
        }
        //Create Buddy Tag to Group
        //Create Buddy Group Details on group_barcode table|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatus (Transactioncontrollers)
        public void CreateBuddyGroup(TravelBarcodeInputs ui, int mode)
        {
            logger.InfoFormat("CreateBuddyGroup ui={0}", ui);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);
                int SeqNo = 0;
                GroupBarcode TC = new GroupBarcode();

                TC.WFId = ui.WFId;
                TC.WFDEPInstId = ui.WFDEPInstId;
                TC.DEPId = 200;

                TC.L1id = ui.L1id;
                TC.L2id = ui.L2id;
                TC.L3id = ui.L3id;
                TC.L4id = ui.L4id;
                TC.L5id = 0;
                TC.BagBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = ui.GroupMode;
                TC.SplitStatus = 0;

                TC.Qty01 = (int)ui.Qty01;
                TC.Qty02 = 0;
                TC.Qty03 = 0;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.CutQty = 0;

                TC.OperationCode = 158;
                TC.WorkCenter = ui.WorkCenter;
                TC.RecStatus = 1;
                TC.TxnStatus = ui.GroupMode == 3 ? 0 : 0;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.GroupBarcode.Add(TC);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Create Travel Group Details on travel-group table|| checked 8-4-2020
        //Used API's and UI : UpdateTravelTag (Businesscontrollers)
        public void CreateTravelGroupDetails(TravelBarcodeInputs ui, int mode, int OperationCode)
        {
            logger.InfoFormat("CreateTravelGroupDetails ui={0}, mode={1}", ui, mode);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);

                TravelBarcodeDetails TC = new TravelBarcodeDetails();
                TC.WFId = ui.WFId;
                TC.DEPId = 200;

                TC.Color = ui.Color;
                TC.TravelBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;

                TC.Qty01 = ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty03;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.OperationCode = OperationCode;
                TC.WorkCenter = ui.WorkCenter;
                TC.RecStatus = 1;

                TC.JobQty = ui.JobQty;
                TC.Weight = ui.Weight;
                TC.TrollyNo = ui.TrollyNo;
                TC.AllocationDate = ui.AllocationDate;
                TC.TravelStatus = OperationCode;
                TC.EPF = ui.EPF;
                TC.FacCode = ojbLukUp.GetFactoryByWfid(ui.Bag_Barcode == "" ? ui.Barcode : ui.Bag_Barcode);
                TC.PlannedMachine = ui.PlannedMachine;
                TC.PlanedDateTime = ui.PlanedDateTime;
                TC.Remarks = ui.gpRemarks;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.TravelBarcodeDetails.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Create Travel Group Details on travel-group table|| checked 8-4-2020
        //Used API's and UI : UpdateTravelTag (Businesscontrollers)
        public void CreateTravelGroupDetailsOutSource(TravelBarcodeInputs ui, int mode, int OperationCode)
        {
            logger.InfoFormat("CreateTravelGroupDetails ui={0}, mode={1}", ui, mode);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);

                TravelBarcodeDetails TC = new TravelBarcodeDetails();
                TC.WFId = ui.WFId;
                TC.DEPId = 200;

                TC.Color = ui.Color;
                TC.TravelBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;
                TC.SMode = 1;

                TC.Qty01 = ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty03;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.OperationCode = OperationCode;
                TC.WorkCenter = ui.WorkCenter;
                TC.RecStatus = 1;

                TC.JobQty = ui.JobQty;
                TC.Weight = ui.Weight;
                TC.TrollyNo = ui.TrollyNo;
                TC.AllocationDate = ui.AllocationDate;
                TC.TravelStatus = OperationCode;
                TC.EPF = ui.EPF;
                TC.FacCode = ojbLukUp.GetFactoryByWfid(ui.Bag_Barcode == "" ? ui.Barcode : ui.Bag_Barcode);
                TC.PlannedMachine = ui.PlannedMachine;
                TC.PlanedDateTime = ui.PlanedDateTime;
                TC.Remarks = ui.gpRemarks;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.TravelBarcodeDetails.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        public void ManageTravelGroupDetails(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("CreateTravelGroupDetails ui={0}, mode={1}", ui);
            try
            {
                TravelBarcodeDetails TC = new TravelBarcodeDetails();

                TC = dcap.TravelBarcodeDetails.Where(c => c.TravelBarCodeNo == ui.TravelBarCodeNo && c.TxnMode >= 2).FirstOrDefault();

                if (TC != null)
                {
                    TC.Qty01 = TC.Qty01 + ui.Qty01;
                    TC.ModifiedBy = ui.CreatedBy;

                    if (TC.Qty01 == 0)
                    {
                        dcap.TravelBarcodeDetails.Remove(TC);
                    }

                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        //Create Travel Group Details on travel-group table|| checked 8-4-2020
        //Used API's and UI : UpdateTravelTag (Businesscontrollers)
        public void CreateBuddyGroupDetails(TravelBarcodeInputs ui, int mode)
        {
            logger.InfoFormat("CreateTravelGroupDetails ui={0}, mode={1}", ui, mode);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);

                TravelBarcodeDetails TC = new TravelBarcodeDetails();
                TC.WFId = ui.WFId;
                TC.DEPId = 200;

                TC.Color = "";
                TC.TravelBarCodeNo = ui.TravelBarCodeNo;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;

                TC.Qty01 = ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty03;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.OperationCode = ui.OperationCode;
                TC.WorkCenter = "";
                TC.RecStatus = 1;

                TC.JobQty = ui.JobQty;
                TC.Weight = ui.Weight;
                TC.TrollyNo = ui.TrollyNo;
                TC.TravelStatus = 0;
                TC.EPF = "";
                TC.FacCode = "BFL";
                TC.Remarks = "";

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.TravelBarcodeDetails.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        public void CreateBuddyGroupDetailsTC(BuudyTagCounter ui, int mode, uint WFId, int OperationCode, string BuddyBarcode)
        {
            logger.InfoFormat("CreateTravelGroupDetails ui={0}, mode={1}", ui, mode);
            try
            {
                LookupController ojbLukUp = new LookupController(dcap);

                TravelBarcodeDetails traveltaginfo = ojbLukUp.GetColorByTravelBarcode(ui.TravelBarCodeNo);
                var opcode = ojbLukUp.GetAdjacentNodesForGivenNode((int)ui.WfdepinstId, 2);
                var PreOpcode = OperationCode;
                if (opcode.Count != 0)
                {
                    PreOpcode = (int)opcode[0].OperationCode;
                }

                TravelBarcodeDetails TC = new TravelBarcodeDetails();
                TC.WFId = WFId;
                TC.DEPId = 200;

                TC.Color = traveltaginfo.Color;
                TC.TravelBarCodeNo = BuddyBarcode;
                TC.TxnMode = mode;
                TC.SplitStatus = 0;

                TC.Qty01 = ui.Qty01;
                TC.Qty02 = ui.Qty02;
                TC.Qty03 = ui.Qty03;
                TC.Qty01NS = 0;
                TC.Qty02NS = 0;
                TC.Qty03NS = 0;

                TC.OperationCode = OperationCode;
                TC.WorkCenter = ui.WfdepinstId.ToString();
                TC.RecStatus = 1;

                TC.JobQty = ui.BagSize;
                TC.Weight = ui.Weight;
                TC.TrollyNo = ui.BagBarCodeNo;
                TC.AllocationDate = traveltaginfo.AllocationDate;
                TC.TravelStatus = PreOpcode;
                TC.EPF = ui.ModifiedBy;
                TC.FacCode = traveltaginfo.FacCode;
                TC.Remarks = ui.TravelBarCodeNo + ":" + (ui.RRType == 3 ? "Rework" : "Scrap") + ":" + ui.RRName;

                TC.CreatedBy = ui.CreatedBy;
                TC.CreatedDateTime = DateTime.Now;
                TC.CreatedMachine = ui.CreatedMachine;
                TC.ModifiedBy = ui.CreatedBy;
                TC.ModifiedDateTime = DateTime.Now;
                TC.ModifiedMachine = ui.CreatedMachine;

                dcap.TravelBarcodeDetails.Add(TC);
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }

        }

        [Produces("application/json")]
        [HttpGet("UpdateQtyofTravelTag")]
        public IList<UserInput> UpdateQtyofTravelTag(List<UserInput> output, string traveltag, int ProdScanType, int ScanType)
        {

            logger.InfoFormat("Get Factory Name output={0}", output.ToString());
            List<UserInput> L1Details = new List<UserInput>();
            UserInput backup1 = new UserInput();
            UserInput backup2 = new UserInput();
            LookupController lookup = new LookupController(dcap);
            TransactionController Transaction = new TransactionController(dcap);

            try
            {
                foreach (UserInput qi in output)
                {
                    if (backup1.StyleId == qi.StyleId && backup1.ScheduleId == qi.ScheduleId && backup1.ColorId == qi.ColorId)
                    {
                        //skip
                    }
                    else
                    {
                        backup1 = qi;
                        if (qi.SaveSuccessfull)
                        {
                            GroupBarcode GroupDetails = new GroupBarcode();
                            GroupDetails = dcap.GroupBarcode.Where(groupbarcode => groupbarcode.L1id == qi.StyleId && groupbarcode.L2id == qi.ScheduleId && groupbarcode.L4id == qi.ColorId && groupbarcode.BagBarCodeNo == traveltag && groupbarcode.TxnMode >= 2)
                                    .FirstOrDefault();

                            if (GroupDetails != null)
                            {
                                GroupDetails.DispatchMode = ProdScanType;

                                if (ProdScanType != 2) // 0 = bypass 1 = bulkupdate, 2 = scan
                                {
                                    if (ScanType == 1) //ScanType: 0 = group, 1 = individual
                                    {
                                        if (qi.PlussMinus == 1)
                                        {
                                            if (qi.EnteredQtyRw > 0)
                                            {
                                                if (lookup.CheckForBarcodeReuseinOperation(qi.Barcode, qi.StyleId, qi.ScheduleId, 0, qi.ColorId, qi.SizeId, qi.OperationCode, qi.DetxnKey, qi.EnteredQtyRw, qi.EnteredQtyScrap))
                                                {
                                                    GroupDetails.Qty02 = GroupDetails.Qty02 + qi.EnteredQtyScrap;
                                                    GroupDetails.Qty03 = GroupDetails.Qty03 + qi.EnteredQtyRw;
                                                }
                                            }
                                            else
                                            {
                                                GroupDetails.Qty02 = GroupDetails.Qty02 + qi.EnteredQtyScrap;
                                                GroupDetails.Qty03 = GroupDetails.Qty03 + qi.EnteredQtyRw;
                                            }
                                        }
                                        else
                                        {
                                            GroupDetails.Qty02 = GroupDetails.Qty02 + qi.EnteredQtyScrap;
                                            GroupDetails.Qty03 = GroupDetails.Qty03 + qi.EnteredQtyRw;
                                        }
                                    }
                                    else
                                    {
                                        GroupDetails.Qty02 = GroupDetails.Qty02 + qi.EnteredQtyScrap;
                                        GroupDetails.Qty03 = GroupDetails.Qty03 + qi.EnteredQtyRw;
                                    }

                                    if (ScanType != 1) //ScanType: 0 = group, 1 = individual
                                    {
                                        if (ProdScanType == 1) // 0 = bypass 1 = bulkupdate, 2 = scan
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.OperationCode == GroupDetails.OperationCode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.TravelBarCodeNo == traveltag
                                                         && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1)
                                                        .ToList();
                                            Detxn.ForEach(c => c.Qty01Ns = 1);

                                            int dispatchedqty = 0;
                                            if (GroupDetails.TxnMode == 2)
                                            {
                                                dispatchedqty = (int)Detxn.Sum(c => c.Qty01);
                                            }
                                            else
                                            {
                                                dispatchedqty = (int)Detxn.Sum(c => c.Qty02) + (int)Detxn.Sum(c => c.Qty03);
                                            }

                                            GroupDetails.TxnStatus = 0;

                                            GroupDetails.DispatchReadyQty = GroupDetails.DispatchReadyQty + dispatchedqty;
                                        }
                                        else
                                        {
                                            if (qi.PlussMinus == 2)
                                            {
                                                if (GroupDetails.TxnStatus == 6)
                                                {
                                                    //Skip
                                                }
                                                else
                                                {
                                                    var opcode = lookup.GetAdjacentNodesForGivenNode((int)qi.WfdepinstId, 2);
                                                    if (opcode.Count != 0)
                                                    {
                                                        GroupDetails.TxnStatus = (int)opcode[0].OperationCode;
                                                    }
                                                    else
                                                    {
                                                        GroupDetails.TxnStatus = GroupDetails.TxnStatus - 1;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                GroupDetails.TxnStatus = qi.OperationCode; //GroupDetails.TxnStatus + 1;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ProdScanType == 1) // 0 = bypass 1 = bulkupdate, 2 = scan
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.BarCodeNo == qi.Barcode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.OperationCode == GroupDetails.OperationCode && detxn.TravelBarCodeNo == traveltag
                                                         && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1)
                                                        .ToList();
                                            Detxn.ForEach(c => c.Qty01Ns = 1);

                                            GroupDetails.DispatchReadyQty = GroupDetails.DispatchReadyQty + 1;
                                            if (GroupDetails.DispatchReadyQty == ((GroupDetails.Qty01 - GroupDetails.Qty02 - GroupDetails.Qty03) - (GroupDetails.Qty01NS - GroupDetails.Qty02NS - GroupDetails.Qty03NS))) //
                                            {
                                                GroupDetails.TxnStatus = 0;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (ProdScanType != 0)
                                    {
                                        if (ScanType != 1) //ScanType: 0 = group, 1 = individual
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.OperationCode == GroupDetails.OperationCode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.TravelBarCodeNo == traveltag
                                                                && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1)
                                                            .ToList();
                                            Detxn.ForEach(c => c.Qty01Ns = 1);

                                            int dispatchedqty = 0;
                                            if (GroupDetails.TxnMode == 2)
                                            {
                                                dispatchedqty = (int)Detxn.Sum(c => c.Qty01);
                                            }
                                            else
                                            {
                                                dispatchedqty = (int)Detxn.Sum(c => c.Qty02) + (int)Detxn.Sum(c => c.Qty03);
                                            }

                                            GroupDetails.DispatchReadyQty = GroupDetails.DispatchReadyQty + dispatchedqty;//((GroupDetails.Qty01 - GroupDetails.Qty02 - GroupDetails.Qty03) - (GroupDetails.Qty01NS - GroupDetails.Qty02NS - GroupDetails.Qty03NS));
                                            if (GroupDetails.DispatchReadyQty == ((GroupDetails.Qty01 - GroupDetails.Qty02 - GroupDetails.Qty03) - (GroupDetails.Qty01NS - GroupDetails.Qty02NS - GroupDetails.Qty03NS)))
                                            {
                                                GroupDetails.TxnStatus = 0;
                                            }
                                        }
                                        else
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.BarCodeNo == qi.Barcode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.OperationCode == GroupDetails.OperationCode && detxn.TravelBarCodeNo == traveltag
                                                                && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1)
                                                            .ToList();
                                            Detxn.ForEach(c => c.Qty01Ns = 1);

                                            GroupDetails.DispatchReadyQty = GroupDetails.DispatchReadyQty + 1;
                                            if (GroupDetails.DispatchReadyQty == ((GroupDetails.Qty01 - GroupDetails.Qty02 - GroupDetails.Qty03) - (GroupDetails.Qty01NS - GroupDetails.Qty02NS - GroupDetails.Qty03NS)))
                                            {
                                                GroupDetails.TxnStatus = 0;
                                            }
                                        }
                                    }
                                }

                                if (ScanType == 1) //ScanType: 0 = group, 1 = individual
                                {
                                    if (qi.EnteredQtyRw != 0 || qi.EnteredQtyScrap != 0)
                                    {
                                        if (qi.SMode == 1)
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.BarCodeNo == qi.Barcode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.OperationCode == GroupDetails.OperationCode && detxn.TravelBarCodeNo == traveltag
                                                            && detxn.Qty01 > 0 && detxn.Qty01Ns != detxn.Qty01 && detxn.Qty02Ns != detxn.Qty01 && detxn.Qty03Ns != detxn.Qty01)
                                                            .ToList();

                                            if (Detxn.Count != 0)
                                            {
                                                decimal Q3 = qi.EnteredQtyRw, Q2 = qi.EnteredQtyScrap;
                                                foreach (Detxn d in Detxn)
                                                {
                                                    decimal rema = (decimal)((d.Qty01 - d.Qty02 - d.Qty03) - (d.Qty01Ns + d.Qty02Ns + d.Qty03Ns));
                                                    if (Q2 != 0)
                                                    {
                                                        if (Q2 > 0)
                                                        {
                                                            if (Q2 > rema)
                                                            {
                                                                d.Qty02Ns = (d.Qty02Ns == null ? 0 : d.Qty02Ns) + rema;
                                                            }
                                                            else
                                                            {
                                                                d.Qty02Ns = (d.Qty02Ns == null ? 0 : d.Qty02Ns) + Q2;
                                                            }
                                                            Q2 = Q2 - rema;
                                                        }
                                                        else
                                                        {
                                                            if (d.Qty02Ns < (-1 * Q2))
                                                            {
                                                                d.Qty02Ns = (d.Qty02Ns == null ? 0 : d.Qty02Ns) - d.Qty02Ns;
                                                            }
                                                            else
                                                            {
                                                                d.Qty02Ns = (d.Qty02Ns == null ? 0 : d.Qty02Ns) + Q2;
                                                            }

                                                            Q2 = Q2 + (decimal)d.Qty02Ns;
                                                        }
                                                    }

                                                    if (Q3 != 0)
                                                    {
                                                        if (Q3 > 0)
                                                        {
                                                            if (Q3 > rema)
                                                            {
                                                                d.Qty03Ns = (d.Qty03Ns == null ? 0 : d.Qty03Ns) + rema;
                                                            }
                                                            else
                                                            {
                                                                d.Qty03Ns = (d.Qty03Ns == null ? 0 : d.Qty03Ns) + Q2;
                                                            }
                                                            Q3 = Q3 - rema;
                                                        }
                                                        else
                                                        {
                                                            if (d.Qty03Ns < (-1 * Q3))
                                                            {
                                                                d.Qty03Ns = (d.Qty02Ns == null ? 0 : d.Qty02Ns) - d.Qty03Ns;
                                                            }
                                                            else
                                                            {
                                                                d.Qty03Ns = (d.Qty03Ns == null ? 0 : d.Qty03Ns) + Q3;
                                                            }

                                                            Q3 = Q3 + (decimal)d.Qty03Ns;
                                                        }
                                                    }
                                                }

                                                if (Q2 + Q3 > 0)
                                                {
                                                    if (Q2 > 0 || Q3 > 0)
                                                    {
                                                        qi.Responce = new string[2];
                                                        qi.Responce[0] = "Error Occured when reporting Rejected Quantiity for mother Operation.. Q2=" + Q2 + ", Q3=" + Q3;
                                                        qi.Responce[1] = "Error Occured when reporting Rejected Quantiity for mother Operation.. Q2=" + Q2 + ", Q3=" + Q3;
                                                        qi.SaveSuccessfull = false;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Q2 < 0 || Q3 < 0)
                                                    {
                                                        qi.Responce = new string[2];
                                                        qi.Responce[0] = "Error Occured when reporting Rejected Quantiity for mother Operation.. Q2=" + Q2 + ", Q3=" + Q3;
                                                        qi.Responce[1] = "Error Occured when reporting Rejected Quantiity for mother Operation.. Q2=" + Q2 + ", Q3=" + Q3;
                                                        qi.SaveSuccessfull = false;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            List<Detxn> Detxn = new List<Detxn>();
                                            Detxn = dcap.Detxn.Where(detxn => detxn.BarCodeNo == qi.Barcode && detxn.L1id == qi.StyleId && detxn.L2id == qi.ScheduleId && detxn.L3id == 0 && detxn.L4id == qi.ColorId && detxn.OperationCode == GroupDetails.OperationCode && detxn.TravelBarCodeNo == traveltag
                                                            && detxn.Qty01 > 0 && detxn.Qty01Ns != 1 && detxn.Qty02Ns != 1 && detxn.Qty03Ns != 1)
                                                            .ToList();

                                            Detxn.ForEach(c => c.Qty02Ns = (c.Qty02Ns == null ? 0 : c.Qty02Ns) + qi.EnteredQtyScrap);
                                            Detxn.ForEach(c => c.Qty03Ns = (c.Qty03Ns == null ? 0 : c.Qty03Ns) + qi.EnteredQtyRw);

                                        }
                                    }
                                }

                                dcap.SaveChanges();
                            }
                            else
                            {
                                qi.Responce = new string[2];
                                qi.Responce[0] = "Return Array for the selcted crieriea in empty - UpdateQtyofTravelTag - Get Group Details";
                                qi.Responce[1] = "Return Array for the selcted crieriea in empty - UpdateQtyofTravelTag - Get Group Details";
                                qi.SaveSuccessfull = false;
                            }
                        }
                    }

                    //group barcode details
                    if (backup2.StyleId == qi.StyleId && backup2.ScheduleId == qi.ScheduleId && backup2.ColorId == qi.ColorId && backup2.SizeId == qi.SizeId && backup2.L5MOID == qi.L5MOID)
                    {
                        //skip
                    }
                    else
                    {
                        backup2 = qi;
                        if (qi.SaveSuccessfull)
                        {
                            Group_Barcode_Detail Group_Barcode_Detail = new Group_Barcode_Detail();
                            Group_Barcode_Detail = dcap.Group_Barcode_Detail.Where(groupbarcode => groupbarcode.L1id == qi.StyleId && groupbarcode.L2id == qi.ScheduleId && groupbarcode.L4id == qi.ColorId && groupbarcode.L5id == qi.SizeId && groupbarcode.L5moid == qi.L5MOID && groupbarcode.BarCodeNo == traveltag && groupbarcode.TxnMode > 0)
                                    .FirstOrDefault();

                            if (Group_Barcode_Detail != null)
                            {
                                if (ProdScanType != 2) // 0 = bypass 1 = bulkupdate, 2 = scan
                                {
                                    if (ScanType == 1) //ScanType: 0 = group, 1 = individual
                                    {
                                        if (qi.PlussMinus == 1)
                                        {
                                            if (qi.EnteredQtyRw > 0)
                                            {
                                                if (lookup.CheckForBarcodeReuseinOperation(qi.Barcode, qi.StyleId, qi.ScheduleId, 0, qi.ColorId, qi.SizeId, qi.OperationCode, qi.DetxnKey, qi.EnteredQtyRw, qi.EnteredQtyScrap))
                                                {
                                                    Group_Barcode_Detail.Qty02 = Group_Barcode_Detail.Qty02 + qi.EnteredQtyScrap;
                                                    Group_Barcode_Detail.Qty03 = Group_Barcode_Detail.Qty03 + qi.EnteredQtyRw;
                                                }
                                            }
                                            else
                                            {
                                                Group_Barcode_Detail.Qty02 = Group_Barcode_Detail.Qty02 + qi.EnteredQtyScrap;
                                                Group_Barcode_Detail.Qty03 = Group_Barcode_Detail.Qty03 + qi.EnteredQtyRw;
                                            }
                                        }
                                        else
                                        {
                                            Group_Barcode_Detail.Qty02 = Group_Barcode_Detail.Qty02 + qi.EnteredQtyScrap;
                                            Group_Barcode_Detail.Qty03 = Group_Barcode_Detail.Qty03 + qi.EnteredQtyRw;
                                        }
                                    }
                                    else
                                    {
                                        Group_Barcode_Detail.Qty02 = Group_Barcode_Detail.Qty02 + qi.EnteredQtyScrap;
                                        Group_Barcode_Detail.Qty03 = Group_Barcode_Detail.Qty03 + qi.EnteredQtyRw;
                                    }
                                }

                                dcap.SaveChanges();
                            }
                            else
                            {
                                //qi.Responce = new string[2];
                                //qi.Responce[0] = "Return Array for the selcted crieriea in empty - UpdateQtyofTravelTag - Get Group Details";
                                //qi.Responce[1] = "Return Array for the selcted crieriea in empty - UpdateQtyofTravelTag - Get Group Details";
                                //qi.SaveSuccessfull = false;
                            }
                        }
                    }

                    L1Details.Add(qi);
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public IList<Detxn> ReleaseBarcodeFromDispatch(string travelbarcode, int txnmode, int OpCode)
        {

            logger.InfoFormat("Get Factory Name travelcode={0} txnmode={1}, OpCode={2}", travelbarcode, txnmode, OpCode);
            List<Detxn> L1Details = new List<Detxn>();

            try
            {
                IList<GroupBarcode> L2Details = dcap.GroupBarcode.Where(detxn => detxn.BagBarCodeNo == travelbarcode && detxn.TxnMode >= txnmode).ToList();

                if (L2Details.Count != 0)
                {
                    foreach (GroupBarcode qi in L2Details)
                    {
                        List<Detxn> Det = new List<Detxn>();

                        if (qi.TxnMode < 3)
                        {
                            Det = dcap.Detxn.Where(detxn => detxn.OperationCode == qi.OperationCode && detxn.L1id == qi.L1id &&
                                     detxn.L2id == qi.L2id && detxn.L3id == qi.L3id && detxn.L4id == qi.L4id && detxn.TravelBarCodeNo == qi.BagBarCodeNo
                                     && (detxn.Qty02 != 1 || detxn.Qty03 != 1) && detxn.Qty01Ns == 1).ToList();

                            Det.ForEach(d => d.Qty01Ns = 0);
                        }
                        else
                        {
                            Det = dcap.Detxn.Where(detxn => detxn.OperationCode == qi.OperationCode && detxn.L1id == qi.L1id &&
                                     detxn.L2id == qi.L2id && detxn.L3id == qi.L3id && detxn.L4id == qi.L4id && detxn.TravelBarCodeNo == qi.BagBarCodeNo
                                     && detxn.Qty01 != 1 && detxn.Qty01Ns == 1).ToList();

                            Det.ForEach(d => d.Qty01Ns = 0);
                        }

                        qi.TxnStatus = OpCode;
                        qi.DispatchMode = 0;
                        qi.DispatchReadyQty = 0;
                    }
                }

                TravelBarcodeDetails L3Details = dcap.TravelBarcodeDetails.Where(detxn => detxn.TravelBarCodeNo == travelbarcode && detxn.TxnMode >= txnmode).FirstOrDefault();

                if (L2Details != null)
                {
                    L3Details.TravelStatus = OpCode;
                }

                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public IList<Detxn> ReverseReleaseBarcodeFromDispatch(string travelbarcode, int txnmode, int TxnStatus)
        {

            logger.InfoFormat("Get Factory Name travelcode={0} txnmode={1}, TxnStatus={2}", travelbarcode, txnmode, TxnStatus);
            List<Detxn> L1Details = new List<Detxn>();

            try
            {
                IList<GroupBarcode> L2Details = dcap.GroupBarcode.Where(detxn => detxn.BagBarCodeNo == travelbarcode && detxn.TxnMode >= txnmode).ToList();

                if (L2Details.Count != 0)
                {
                    foreach (GroupBarcode qi in L2Details)
                    {
                        List<Detxn> Det = new List<Detxn>();

                        if (qi.TxnMode < 3)
                        {
                            Det = dcap.Detxn.Where(detxn => detxn.L1id == qi.L1id &&
                                     detxn.L2id == qi.L2id && detxn.L3id == qi.L3id && detxn.L4id == qi.L4id && detxn.OperationCode == qi.OperationCode && detxn.TravelBarCodeNo == qi.BagBarCodeNo
                                     && (detxn.Qty02 != 1 || detxn.Qty03 != 1) && detxn.Qty03Ns != 1).ToList();

                            Det.ForEach(d => d.Qty01Ns = 1);
                        }
                        else
                        {
                            Det = dcap.Detxn.Where(detxn => detxn.L1id == qi.L1id &&
                                     detxn.L2id == qi.L2id && detxn.L3id == qi.L3id && detxn.L4id == qi.L4id && detxn.OperationCode == qi.OperationCode && detxn.TravelBarCodeNo == qi.BagBarCodeNo
                                     && detxn.Qty01 != 1 && detxn.Qty03Ns != 1).ToList();

                            Det.ForEach(d => d.Qty01Ns = 1);
                        }

                        qi.TxnStatus = TxnStatus;
                    }
                }

                TravelBarcodeDetails L3Details = dcap.TravelBarcodeDetails.Where(detxn => detxn.TravelBarCodeNo == travelbarcode && detxn.TxnMode >= txnmode).FirstOrDefault();

                if (L2Details != null)
                {
                    L3Details.TravelStatus = 160;
                }

                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return L1Details;
        }

        public void UpdateTravelTagTravelStatus(int operationcode, string traveltag, int Plusminus, uint WfdepinstId)
        {
            TravelBarcodeDetails TravelDetails = new TravelBarcodeDetails();
            LookupController lookup = new LookupController(dcap);

            try
            {
                TravelDetails = dcap.TravelBarcodeDetails.Where(travelbarcode => travelbarcode.TravelBarCodeNo == traveltag)
                                .FirstOrDefault();

                if (TravelDetails != null)
                {
                    if (Plusminus == 1)
                    {
                        TravelDetails.TravelStatus = operationcode;
                    }
                    else
                    {
                        //TravelDetails.TravelStatus = operationcode - 1;
                        var opcode = lookup.GetAdjacentNodesForGivenNode((int)WfdepinstId, 2); //previous op code = 2
                        if (opcode.Count != 0)
                        {
                            TravelDetails.TravelStatus = (int)opcode[0].OperationCode;
                        }
                        else
                        {
                            TravelDetails.TravelStatus = TravelDetails.TravelStatus - 1;
                        }
                    }
                }

                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        //Update Barcodes Status on group_barcode table and create travel tag group if needed|| checked 8-4-2020
        //Used API's and UI : UpdateTravelTag (Businesscontrollers)
        public TravelBarcodeInputs UpdateGroupBarcodesStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateBarcodesStatus ui={0}", ui);
            ui.createNewTravelGroup = false;
            ui.updateIndividualbarcodeScan = false;
            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                if (ui.TxnMode == 1)
                {
                    ui.updateIndividualbarcodeScan = true;
                    //TxnContrl.UpdateIndividualBarcodeStatus(ui);

                    GroupBarcode objdetxn2 = new GroupBarcode();
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        objdetxn2.Qty01 = objdetxn2.Qty01 + (int)ui.Qty01;
                        objdetxn2.ModifiedDateTime = DateTime.Now;
                        dcap.SaveChanges();
                        ui.createNewTravelGroup = false;
                    }
                    else
                    {
                        //TxnContrl.CreateTravelGroup(ui, 2);
                        ui.createNewTravelGroup = true;
                    }

                }
                else if (ui.TxnMode == 0)
                {
                    //TxnContrl.UpdateIndividualBarcodeStatus(ui);
                    ui.updateIndividualbarcodeScan = true;

                    GroupBarcode objdetxn2 = new GroupBarcode();
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        objdetxn2.Qty01 = objdetxn2.Qty01 + (int)ui.Qty01;
                        objdetxn2.ModifiedDateTime = DateTime.Now;
                        dcap.SaveChanges();
                        ui.createNewTravelGroup = false;
                    }
                    else
                    {
                        //TxnContrl.CreateTravelGroup(ui, 2);
                        ui.createNewTravelGroup = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        //Update Barcodes Status on group_barcode table and create travel tag group if needed|| checked 8-4-2020
        //Used API's and UI : UpdateTravelTag (Businesscontrollers)
        public TravelBarcodeInputs ManageGroupBarcodesStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateBarcodesStatus ui={0}", ui);

            GroupBarcode objdetxn2 = new GroupBarcode();
            ui.createNewTravelGroup = false;
            ui.updateIndividualbarcodeScan = false;
            ui.deleteTravelGroup = false;

            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                if (ui.TxnMode == 2)
                {
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        ui.deleteTravelGroup = true;
                        ui.updateIndividualbarcodeScan = true;
                        dcap.GroupBarcode.Remove(objdetxn2);
                        dcap.SaveChanges();
                    }
                }
                else if (ui.TxnMode == 1)
                {
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        if ((objdetxn2.Qty01 - objdetxn2.Qty02 - objdetxn2.Qty03) >= (int)ui.Qty01)
                        {
                            objdetxn2.Qty01 = objdetxn2.Qty01 - (int)ui.Qty01;
                            objdetxn2.ModifiedDateTime = DateTime.Now;
                            ui.updateIndividualbarcodeScan = true;
                        }

                        if (objdetxn2.Qty01 == 0)
                        {
                            ui.deleteTravelGroup = true;
                            dcap.GroupBarcode.Remove(objdetxn2);
                        }

                        dcap.SaveChanges();
                    }
                    else
                    {
                        ui.createNewTravelGroup = true;
                    }

                }
                else if (ui.TxnMode == 0)
                {
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == 2 &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        if ((objdetxn2.Qty01 - objdetxn2.Qty02 - objdetxn2.Qty03) >= (int)ui.Qty01)
                        {
                            objdetxn2.Qty01 = objdetxn2.Qty01 - (int)ui.Qty01;
                            objdetxn2.ModifiedDateTime = DateTime.Now;
                            ui.updateIndividualbarcodeScan = true;
                        }

                        if (objdetxn2.Qty01 == 0)
                        {
                            ui.deleteTravelGroup = true;
                            dcap.GroupBarcode.Remove(objdetxn2);
                        }

                        dcap.SaveChanges();
                    }
                    else
                    {
                        ui.createNewTravelGroup = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        public TravelBarcodeInputs UpdateGroupMappingStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateBarcodesStatus ui={0}", ui);
            ui.createNewTravelMapGroup = false;
            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                if (ui.TxnMode == 1)
                {
                    ui.createNewTravelMapGroup = true;
                }
                else if (ui.TxnMode == 0)
                {
                    GroupBarcodeMapping objdetxn2 = new GroupBarcodeMapping();
                    objdetxn2 = dcap.GroupBarcodeMapping.Where(c => c.WFDEPInstId == (int)ui.WFDEPInstId && c.MotherBarcode == ui.TravelBarCodeNo && c.MotherTxnMode == 2 && c.ChildBarcode == ui.Bag_Barcode && c.ChildTxnMode == 1).FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        objdetxn2.ModifiedDateTime = DateTime.Now;
                        objdetxn2.Qty01NS = objdetxn2.Qty01NS + ui.Qty01;
                        dcap.SaveChanges();
                        ui.createNewTravelMapGroup = false;
                    }
                    else
                    {
                        ui.createNewTravelMapGroup = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        public TravelBarcodeInputs ManageGroupMapping(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateBarcodesStatus ui={0}", ui);
            GroupBarcodeMapping objdetxn2 = new GroupBarcodeMapping();
            ui.createNewTravelMapGroup = false;

            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                if (ui.TxnMode == 2)
                {
                    if (ui.Qty01 < 0)
                    {
                        List<GroupBarcodeMapping> objdetxn3 = dcap.GroupBarcodeMapping.Where(c => c.MotherBarcode == ui.TravelBarCodeNo && c.MotherTxnMode == 2).ToList();
                        if (objdetxn3.Count != 0)
                        {
                            foreach (GroupBarcodeMapping di in objdetxn3)
                            {
                                dcap.GroupBarcodeMapping.Remove(di);
                            }
                        }
                    }
                }
                else if (ui.TxnMode == 1)
                {
                    if (ui.Qty01 < 0)
                    {
                        objdetxn2 = dcap.GroupBarcodeMapping.Where(c => c.WFDEPInstId == (int)ui.WFDEPInstId && c.MotherBarcode == ui.TravelBarCodeNo && c.MotherTxnMode == 2 && c.ChildBarcode == ui.Barcode && c.ChildTxnMode == 1).FirstOrDefault();
                        if (objdetxn2 != null)
                        {
                            dcap.GroupBarcodeMapping.Remove(objdetxn2);
                        }
                    }
                    else
                    {
                        ui.createNewTravelMapGroup = true;
                    }
                }
                else if (ui.TxnMode == 0)
                {
                    objdetxn2 = dcap.GroupBarcodeMapping.Where(c => c.WFDEPInstId == (int)ui.WFDEPInstId && c.MotherBarcode == ui.TravelBarCodeNo && c.MotherTxnMode == 2 && c.ChildBarcode == ui.Bag_Barcode && c.ChildTxnMode == 1).FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        objdetxn2.ModifiedDateTime = DateTime.Now;
                        objdetxn2.Qty01NS = objdetxn2.Qty01NS + ui.Qty01;
                        if (objdetxn2.Qty01 == 0)
                        {
                            dcap.GroupBarcodeMapping.Remove(objdetxn2);
                        }
                        ui.createNewTravelMapGroup = false;
                    }
                    else
                    {
                        ui.createNewTravelMapGroup = true;
                    }
                }

                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        public Boolean UpdateGroupBarcodesDetailStatus(int L1id, int L2id, int L3id, int L4id, int L5id, int L5moid, int Txnmode, string BarcodeNo, int Qty01)
        {
            logger.InfoFormat("UpdateBarcodesStatus L1id={0}, L2id={2}, L3id={3}, L4id={4}, L5id={5}, L5moid={6}, Txnmode={7}, BarcodeNo={8}, Qty01={9}", L1id, L2id, L3id, L4id, L5id, L5moid, Txnmode, BarcodeNo, Qty01);
            Boolean createNewTravelGroupDetail = false;
            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                //if (true)
                //{
                Group_Barcode_Detail objdetxn2 = new Group_Barcode_Detail();
                objdetxn2 = dcap.Group_Barcode_Detail
                    .Where(c => c.L1id == L1id && c.L2id == L2id && c.L3id == L3id && c.L4id == L4id && c.L5id == L5id && c.L5moid == L5moid && c.TxnMode == Txnmode && c.BarCodeNo == BarcodeNo)
                    .FirstOrDefault();

                if (objdetxn2 != null)
                {
                    objdetxn2.Qty01 = objdetxn2.Qty01 + Qty01;
                    objdetxn2.ModifiedDateTime = DateTime.Now;
                    dcap.SaveChanges();
                    createNewTravelGroupDetail = false;
                }
                else
                {
                    createNewTravelGroupDetail = true;
                }

                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return createNewTravelGroupDetail;
        }

        public Boolean ManageGroupBarcodesDetailStatus(int L1id, int L2id, int L3id, int L4id, int L5id, int L5moid, int Txnmode, string BarcodeNo, int Qty01)
        {
            logger.InfoFormat("UpdateBarcodesStatus L1id={0}, L2id={2}, L3id={3}, L4id={4}, L5id={5}, L5moid={6}, Txnmode={7}, BarcodeNo={8}, Qty01={9}", L1id, L2id, L3id, L4id, L5id, L5moid, Txnmode, BarcodeNo, Qty01);
            Boolean createNewTravelGroupDetail = false;
            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                //if (true)
                //{
                Group_Barcode_Detail objdetxn2 = new Group_Barcode_Detail();
                objdetxn2 = dcap.Group_Barcode_Detail
                    .Where(c => c.L1id == L1id && c.L2id == L2id && c.L3id == L3id && c.L4id == L4id && c.L5id == L5id && c.L5moid == L5moid && c.TxnMode == Txnmode && c.BarCodeNo == BarcodeNo)
                    .FirstOrDefault();

                if (objdetxn2 != null)
                {
                    objdetxn2.Qty01 = objdetxn2.Qty01 + Qty01;
                    objdetxn2.ModifiedDateTime = DateTime.Now;

                    if (objdetxn2.Qty01 == 0)
                    {
                        dcap.Group_Barcode_Detail.Remove(objdetxn2);
                    }
                    dcap.SaveChanges();
                    createNewTravelGroupDetail = false;
                }
                else
                {
                    createNewTravelGroupDetail = true;
                }

                //}
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return createNewTravelGroupDetail;
        }

        //Update Barcodes Status on group_barcode table and create travel tag group if needed|| checked 8-4-2020
        //Used API's and UI : UpdateBuudyTag (Businesscontrollers)
        public TravelBarcodeInputs UpdateBuddyBarcodesStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateBuddyBarcodesStatus ui={0}", ui);
            ui.createNewTravelGroup = false;
            ui.updateIndividualbarcodeScan = false;
            try
            {
                //TransactionController TxnContrl = new TransactionController(dcap);
                if (ui.TxnMode == 0)
                {
                    //TxnContrl.UpdateIndividualBarcodeStatus(ui);
                    ui.updateIndividualbarcodeScan = true;

                    GroupBarcode objdetxn2 = new GroupBarcode();
                    objdetxn2 = dcap.GroupBarcode
                        .Where(c => c.BagBarCodeNo == ui.TravelBarCodeNo && c.TxnMode == ui.GroupMode &&
                        c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn2 != null)
                    {
                        objdetxn2.Qty01 = objdetxn2.Qty01 + (int)ui.Qty01;
                        objdetxn2.ModifiedDateTime = DateTime.Now;
                        dcap.SaveChanges();
                        ui.createNewTravelGroup = false;
                    }
                    else
                    {
                        //TxnContrl.CreateTravelGroup(ui, 2);
                        ui.createNewTravelGroup = true;
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        //Update Individual Barcodes Status on detxn|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatus (Transactioncontrollers)
        public TravelBarcodeInputs UpdateIndividualBarcodeStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateIndividualBarcodeStatus ui={0}", ui);

            GroupBarcodeMapping TR = new GroupBarcodeMapping();
            try
            {
                List<Detxn> objdetxn = new List<Detxn>();
                if (ui.TxnMode == 0)
                {
                    objdetxn = dcap.Detxn.Where(c => c.BarCodeNo == ui.Barcode && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BagBarCodeNo == ui.Bag_Barcode && c.TravelBarCodeNo == "").ToList();
                }
                else
                {
                    objdetxn = dcap.Detxn.Where(c => c.BagBarCodeNo == ui.Barcode && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.TravelBarCodeNo == "").ToList();
                }

                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.TravelBarCodeNo = ui.TravelBarCodeNo);
                    objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);

                    GroupBarcode objdetxn1 = new GroupBarcode();
                    if (ui.TxnMode == 0)
                    {
                        objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BagBarCodeNo == ui.Bag_Barcode).FirstOrDefault();
                    }
                    else
                    {
                        objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BagBarCodeNo == ui.Barcode).FirstOrDefault();
                    }


                    if (objdetxn1 != null)
                    {
                        if ((int)objdetxn1.Qty01 <= (int)objdetxn1.Qty01NS + (int)ui.Qty01 || (int)objdetxn1.Qty01 <= (int)ui.Qty01)
                        {
                            objdetxn1.TxnStatus = 6;
                        }

                        objdetxn1.Qty01NS = objdetxn1.Qty01NS + (int)ui.Qty01;
                        objdetxn1.WorkCenter = (objdetxn1.WorkCenter == null ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter.Contains(ui.TravelBarCodeNo) ? objdetxn1.WorkCenter : (objdetxn1.WorkCenter == "" ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter + ", " + ui.TravelBarCodeNo))));
                        objdetxn1.ModifiedDateTime = DateTime.Now;
                        //objdetxn1.ModifiedBy = ui.ModifiedBy;
                        //ui.updateIndividualbarcodeScan = true;
                    }

                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        //Update Individual Barcodes Status on detxn|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatusOutSource (Transactioncontrollers
        public TravelBarcodeInputs UpdateIndividualBarcodeStatusOutSource(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateIndividualBarcodeStatusOutSource ui={0}", ui);

            GroupBarcodeMapping TR = new GroupBarcodeMapping();
            try
            {
                GroupBarcode objdetxn1 = new GroupBarcode();
                if (ui.TxnMode == 0)
                {
                    objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BagBarCodeNo == ui.Bag_Barcode).FirstOrDefault();
                }
                else
                {
                    objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BagBarCodeNo == ui.Barcode).FirstOrDefault();
                }


                if (objdetxn1 != null)
                {
                    if ((int)objdetxn1.Qty01 <= (int)objdetxn1.Qty01NS + (int)ui.Qty01 || (int)objdetxn1.Qty01 <= (int)ui.Qty01)
                    {
                        objdetxn1.TxnStatus = 6;
                    }

                    objdetxn1.Qty01NS = objdetxn1.Qty01NS + (int)ui.Qty01;
                    objdetxn1.WorkCenter = (objdetxn1.WorkCenter == null ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter.Contains(ui.TravelBarCodeNo) ? objdetxn1.WorkCenter : (objdetxn1.WorkCenter == "" ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter + ", " + ui.TravelBarCodeNo))));
                    objdetxn1.ModifiedDateTime = DateTime.Now;
                    //objdetxn1.ModifiedBy = ui.ModifiedBy;
                    //ui.updateIndividualbarcodeScan = true;
                }

                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        public TravelBarcodeInputs DeleteIndividualBarcodeStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateIndividualBarcodeStatus ui={0}", ui);

            GroupBarcodeMapping TR = new GroupBarcodeMapping();
            try
            {
                List<Detxn> objdetxn = new List<Detxn>();
                if (ui.TxnMode == 0)
                {
                    objdetxn = dcap.Detxn.Where(c => c.BarCodeNo == ui.Barcode && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id && c.BagBarCodeNo == ui.Bag_Barcode && c.TravelBarCodeNo == ui.TravelBarCodeNo).ToList();
                }
                else if (ui.TxnMode == 1)
                {
                    objdetxn = dcap.Detxn.Where(c => c.BagBarCodeNo == ui.Barcode && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.TravelBarCodeNo == ui.TravelBarCodeNo).ToList();
                }
                else if (ui.TxnMode == 2)
                {
                    objdetxn = dcap.Detxn.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.TravelBarCodeNo == ui.TravelBarCodeNo).ToList();
                }

                if (objdetxn != null)
                {
                    if (ui.Qty01 < 0)
                    {
                        objdetxn.ForEach(a => a.TravelBarCodeNo = "");
                        objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                        objdetxn.ForEach(a => a.ModifiedMachine = "Removed From TT");
                    }
                    else
                    {
                        objdetxn.ForEach(a => a.TravelBarCodeNo = ui.TravelBarCodeNo);
                        objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                        objdetxn.ForEach(a => a.ModifiedMachine = "Added For TT");
                    }

                    GroupBarcode objdetxn1 = new GroupBarcode();
                    if (ui.TxnMode == 0)
                    {
                        objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BagBarCodeNo == ui.Bag_Barcode).FirstOrDefault();
                    }
                    else if (ui.TxnMode == 1)
                    {
                        objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BagBarCodeNo == ui.Barcode).FirstOrDefault();
                    }
                    else if (ui.TxnMode == 2)
                    {
                        objdetxn1 = dcap.GroupBarcode.Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.BagBarCodeNo == ui.TravelBarCodeNo).FirstOrDefault();
                    }


                    if (objdetxn1 != null)
                    {
                        if ((int)objdetxn1.Qty01 <= (int)objdetxn1.Qty01NS + (int)ui.Qty01 || (int)objdetxn1.Qty01 <= (int)ui.Qty01)
                        {
                            objdetxn1.TxnStatus = 6;
                        }
                        else
                        {
                            objdetxn1.TxnStatus = 5;
                            objdetxn1.WorkCenter = "";
                        }

                        objdetxn1.Qty01NS = objdetxn1.Qty01NS + (int)ui.Qty01;
                        if (ui.Qty01 > 0)
                        {
                            objdetxn1.WorkCenter = (objdetxn1.WorkCenter == null ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter.Contains(ui.TravelBarCodeNo) ? objdetxn1.WorkCenter : (objdetxn1.WorkCenter == "" ? ui.TravelBarCodeNo : (objdetxn1.WorkCenter + ", " + ui.TravelBarCodeNo))));
                        }
                        else
                        {
                            objdetxn1.WorkCenter = objdetxn1.WorkCenter + "(R: " + ui.Qty01 + " F: " + ui.TravelBarCodeNo + ")";
                        }
                        objdetxn1.ModifiedDateTime = DateTime.Now;
                    }

                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        //Update Individual Barcodes Status on detxn|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatus (Transactioncontrollers)
        public List<Detxn> UpdateIndividualBuddyBarcodeStatus(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateIndividualBuddyBarcodeStatus ui={0}", ui);

            GroupBarcodeMapping TR = new GroupBarcodeMapping();
            List<Detxn> objdetxn = new List<Detxn>();
            try
            {
                var objdetxndetail = dcap.Detxn
                        .Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && (ui.TxnMode == 0 ? c.L5id == ui.L5id : true)).ToList();

                objdetxn = objdetxndetail
                        .Where(c => (ui.TxnMode == 0 ? c.BarCodeNo == ui.Barcode : true) && c.OperationCode == ui.OperationCode && ui.GroupMode == 3 ? c.Qty03 > 0 : c.Qty02 > 0
                         && c.TravelBarCodeNo == "" && c.BagBarCodeNo != ""
                         && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && (ui.TxnMode == 0 ? c.L5id == ui.L5id : true))
                        .ToList();


                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.TravelBarCodeNo = ui.TravelBarCodeNo);
                    objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                    /*foreach(Detxn det in objdetxn) {
                        Detxn de = new Detxn();
                        de = det;
                        de.Qty01 = -1;
                        dcap.Detxn.Add(de);
                    }*/

                    dcap.SaveChanges();


                    GroupBarcode objdetxn1 = new GroupBarcode();
                    objdetxn1 = dcap.GroupBarcode
                        .Where(c => (ui.TxnMode == 0 ? c.BagBarCodeNo == ui.Bag_Barcode : c.BagBarCodeNo == ui.Barcode)
                         && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                        .FirstOrDefault();

                    if (objdetxn1 != null)
                    {
                        objdetxn1.Qty01NS = objdetxn1.Qty01NS + (int)ui.Qty01;
                        if (objdetxn1.Qty01NS == objdetxn1.Qty01) { objdetxn1.TxnStatus = 6; }
                        objdetxn1.ModifiedDateTime = DateTime.Now;

                        dcap.SaveChanges();
                        //ui.updateIndividualbarcodeScan = true;

                        /*
                        TR.WFDEPInstId = ui.WFDEPInstId;
                        TR.MotherBarcode = ui.TravelBarCodeNo;
                        TR.MotherTxnMode = 2;

                        TR.ChildBarcode = (ui.TxnMode == 0 ?  ui.Bag_Barcode : ui.Barcode);
                        TR.ChildTxnMode = 1;

                        TR.Qty01 = ui.Qty01;
                        TR.Qty02 = ui.Qty02;
                        TR.Qty03 = ui.Qty03;
                        TR.Qty01NS = ui.Qty01NS;
                        TR.Qty02NS = ui.Qty02NS;
                        TR.Qty03NS = ui.Qty03NS;

                        TR.CreatedBy = ui.CreatedBy;
                        TR.CreatedDateTime = ui.CreatedDateTime;
                        TR.CreatedMachine = ui.CreatedMachine;
                        TR.ModifiedBy = ui.ModifiedBy;
                        TR.ModifiedMachine = ui.ModifiedMachine;
                        TR.ModifiedDateTime = ui.ModifiedDateTime;
                        */
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return objdetxn;
        }

        //Direct Update Individual and Group Barcodes Status on tables Detxn and Group_barcode|| checked 8-4-2020
        //Used API's and UI : UpdateBarcodesStatus (Transactioncontrollers)
        public void UpdateIndividualBarcodeStatusDirect(TravelBarcodeInputs ui)
        {
            logger.InfoFormat("UpdateIndividualBarcodeStatusDirect ui={0}", ui);
            try
            {
                List<Detxn> objdetxn = dcap.Detxn
                        .Where(c => c.BarCodeNo == ui.Barcode && c.BagBarCodeNo == ui.Bag_Barcode && c.Seq == ui.Seq
                         && c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id && c.L5id == ui.L5id)
                        .ToList();

                if (objdetxn != null)
                {
                    foreach (Detxn det in objdetxn)
                    {
                        det.TravelBarCodeNo = ui.TravelBarCodeNo;

                        GroupBarcode objgroupbarcode = dcap.GroupBarcode
                                .Where(c => c.BagBarCodeNo == det.BagBarCodeNo
                                && c.L1id == det.L1id && c.L2id == det.L2id && c.L3id == det.L3id && c.L4id == det.L4id)
                                .FirstOrDefault();

                        if (objgroupbarcode != null)
                        {
                            objgroupbarcode.Qty01NS = objgroupbarcode.Qty01NS + 1;
                            objgroupbarcode.ModifiedDateTime = DateTime.Now;
                        }
                    }

                    dcap.SaveChanges();
                }


            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        public void UpdateIndividualBagBarcodeDirect(UserInput ui)
        {
            logger.InfoFormat("UpdateIndividualBagBarcodeDirect ui={0}", ui);
            try
            {
                List<Detxn> objdetxn = dcap.Detxn
                        .Where(c => c.BarCodeNo == ui.Barcode && c.OperationCode == ui.OperationCode && c.Wfid == ui.WFID
                         && c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L4id == ui.ColorId && c.L5id == ui.SizeId)
                        .ToList();

                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.BagBarCodeNo = ui.BagBarCodeNo);
                    objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        public void RemoveBagBarcodeeIndividualBagBarcodeDirect(UserInput ui)
        {
            logger.InfoFormat("RemoveBagBarcodeeIndividualBagBarcodeDirect ui={0}", ui);
            try
            {
                List<Detxn> objdetxn = dcap.Detxn
                        .Where(c => c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L4id == ui.ColorId && c.L5id == ui.SizeId
                        && c.BarCodeNo == ui.Barcode)
                        .ToList();

                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.BagBarCodeNo = "");
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        public void RemoveDetxnLineByKey(uint key)
        {
            logger.InfoFormat("RemoveDetxnLineByKey jey={0}", key);
            try
            {
                List<Detxn> objdetxn = dcap.Detxn
                        .Where(c => c.DetxnKey == key)
                        .ToList();

                if (objdetxn != null)
                {
                    foreach (Detxn TC in objdetxn)
                    {
                        dcap.Detxn.Remove(TC);
                    }
                }
                dcap.SaveChanges();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }

        //Update Dispatch Success Status of Bags : Mark bag as GRN Item and Update Location Code
        //Used API's and UI : UpdateDispatchSuccessStatusofBags API (Businesscontroller) 
        public Boolean UpdateBagSuccessStatus(TeamCounterCM ui, Boolean gstatus)
        {
            logger.InfoFormat("UpdateBagSuccessStatus ui={0}, gstatus={1}", ui, gstatus);
            Boolean GRNStatus = true;
            try
            {
                List<GoodControl> objdetxn = dcap.GoodControl.Where(c => c.L1id == ui.StyleId && c.L2id == ui.ScheduleId && c.L4id == ui.ColorId
                                                && c.BarCodeNo == ui.BagBarCode && c.Seq == ui.Seq && c.TxnMode == ui.TxnMode && c.ControlId == ui.ControlId)
                                                .ToList();

                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.IsSucess = ui.BagStatus);
                    objdetxn.ForEach(a => a.WarLocCode = ui.WarLocCode);
                    objdetxn.ForEach(a => a.ModifiedBy = ui.EnteredBy);
                    //objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                    if (gstatus)
                    {
                        objdetxn.ForEach(a => a.TxnStatus = 5);
                        objdetxn.ForEach(a => a.RecivedDateTime = DateTime.Now);
                    }

                    foreach (GoodControl r in objdetxn)
                    {
                        List<GroupBarcode> objgroupbarcode = dcap.GroupBarcode
                                .Where(c => c.L1id == r.L1id && c.L2id == r.L2id && c.L3id == r.L3id && c.L4id == r.L4id
                                && c.BagBarCodeNo == r.BarCodeNo && c.TxnMode == ui.TxnMode)
                                .ToList();

                        if (objgroupbarcode != null)
                        {
                            objgroupbarcode.ForEach(a => a.TxnStatus = 5);
                            objgroupbarcode.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                        }
                    }

                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return GRNStatus;
        }

        public Boolean UpdateCloseStatus(TeamCounterCM ui)
        {
            //TransactionController txncontroller = new TransactionController(dcap);
            Boolean validate = true;
            try
            {
                GoodControlDetails objdetxn = dcap.GoodControlDetails
                        .Where(c => c.ControlId == ui.ControlId)
                        .FirstOrDefault();

                if (objdetxn != null)
                {
                    objdetxn.TxnStatus = 5;
                    objdetxn.ClosedBy = ui.EnteredBy;
                    objdetxn.ClosedDateTime = DateTime.Now;
                    objdetxn.ModifiedDateTime = DateTime.Now;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return validate;
        }

        //Update Dispatch Success Status of Bags : Mark Bags Status in Group Barcode Table
        //Used API's and UI : UpdateBagSuccessStatus() API (Transactioncontroller) 
        public Boolean UpdateBagSuccessStatusinGroupBarcode(GoodControl ui)
        {
            logger.InfoFormat("UpdateBagSuccessStatus GoodControl={0}", ui);
            Boolean GRNStatus = true;
            try
            {
                List<GroupBarcode> objdetxn = dcap.GroupBarcode
                        .Where(c => c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id
                        && c.BagBarCodeNo == ui.BarCodeNo && c.TxnMode == ui.TxnMode)
                        .ToList();

                if (objdetxn != null)
                {
                    objdetxn.ForEach(a => a.TxnStatus = ui.TxnStatus);
                    objdetxn.ForEach(a => a.ModifiedDateTime = DateTime.Now);
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
            return GRNStatus;
        }

        //Travel Status Update :START
        //Insert data to detxn 
        [Produces("application/json")]
        [HttpPost("AddTravelStatus")]
        public void AddTravelStatus(UserInput ui)
        {
            logger.InfoFormat("AddTravelStatus UserInput={0}", ui);

            //LookupController ojbLukUp = new LookupController(dcap, ui.guid);
            LookupController ojbLukUp = new LookupController(dcap);
            int SeqNo = 0;
            int ProdHour = 0;
            int UploadStatus = 99;
            try
            {
                SeqNo = 0; //ojbLukUp.GetDETxnNextSeqNo(ui);
                ProdHour = ojbLukUp.GetProdHourByTeamId((int)ui.TeamId, ui.TxnDate, ui.Offline);
                UploadStatus = ojbLukUp.GetDeTxnUploadStatus((int)ui.TeamId, ui.OperationCode);

                TravelStatus objdetxn = new TravelStatus();
                objdetxn.WfdepinstId = ui.WfdepinstId;
                objdetxn.Seq = (uint)SeqNo;
                objdetxn.L1id = ui.StyleId;
                objdetxn.L2id = ui.ScheduleId;
                objdetxn.L3id = 0;
                objdetxn.L4id = ui.ColorId;
                objdetxn.L5id = ui.SizeId;
                objdetxn.BarCodeNo = ui.Barcode;

                objdetxn.Wfid = ui.WFID;
                objdetxn.Depid = ui.Depid;
                objdetxn.TeamId = ui.TeamId;
                objdetxn.Dclid = ui.DCLId;
                objdetxn.TxnDateTime = Convert.ToDateTime(ui.TxnDate);
                objdetxn.HourNo = ProdHour;
                objdetxn.TxnMode = ui.TxnMode;
                objdetxn.PlussMinus = (uint)Convert.ToInt32(ui.PlussMinus);
                objdetxn.Rrid = (uint)Convert.ToInt32(ui.RRId);
                objdetxn.DopsId = (uint)Convert.ToInt32(ui.DOpsId);
                objdetxn.Qty01 = ui.EnteredQtyGd;
                objdetxn.Qty02 = ui.EnteredQtyScrap;
                objdetxn.Qty03 = ui.EnteredQtyRw;
                objdetxn.JobNo = ui.JobNo;
                objdetxn.OperationCode = ui.OperationCode;
                objdetxn.EnteredBy = ui.CreatedBy;
                objdetxn.AppStatus = (int)AppStatus.Pending;
                objdetxn.UploadStatus = UploadStatus;
                objdetxn.ErrorCode = 0;
                objdetxn.HasError = 0;
                objdetxn.RecStatus = 1;

                objdetxn.CreatedBy = ui.CreatedBy;
                objdetxn.CreatedDateTime = DateTime.Now;
                objdetxn.CreatedMachine = ui.CreatedMachine;
                objdetxn.ModifiedBy = ui.ModifiedBy;
                objdetxn.ModifiedDateTime = DateTime.Now;
                objdetxn.ModifiedMachine = ui.ModifiedMachine;

                dcap.TravelStatus.Add(objdetxn);
                dcap.SaveChanges();

                ui.DetxnKey = objdetxn.DetxnKey;
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Schedule information {0}", e.ToString());
                throw e;
            }
            finally
            {
                logger.InfoFormat("Detxn Save successfull");

            }
        }

        //Update Wash Details: START
        //Update Wash details to L4 Table
        public void UpdateWashDetails(WashDetailUpdateInputs ui)
        {
            logger.InfoFormat("UpdateWashDetails WashDetailUpdateInputs={0}", ui);
            try
            {
                List<L4> objdetxn = new List<L4>();
                if (ui.ApplyMode == 1) //Style
                {
                    objdetxn = dcap.L4
                        .Where(c => c.L1id == ui.L1)
                        .ToList();
                }
                else if (ui.ApplyMode == 2) //Shedule
                {
                    objdetxn = dcap.L4
                        .Where(c => c.L1id == ui.L1 && c.L2id == ui.L2)
                        .ToList();
                }
                else if (ui.ApplyMode == 3)
                {
                    objdetxn = dcap.L4
                        .Where(c => c.L1id == ui.L1 && c.L4id == ui.L4)
                        .ToList();
                }
                else if (ui.ApplyMode == 4)
                { //Color
                    objdetxn = dcap.L4
                        .Where(c => c.L1id == ui.L1 && c.L2id == ui.L2 && c.L4id == ui.L4)
                        .ToList();

                }


                if (objdetxn != null)
                {
                    foreach (L4 di in objdetxn)
                    {
                        di.SubinPO = ui.SubinPO;
                        di.WashDescription = ui.WashDescription;
                        di.WashType = ui.WashType;
                        di.GarmentWeight = ui.GMTWeight;
                        di.WashDuration = ui.WashDuration;
                        di.UnitPrice = ui.UnitPrice;
                        di.Category = ui.Category;

                        di.ModifiedDateTime = DateTime.Now;
                    }

                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }
        }
        //Update Wash Details: END

        //Update Invoice: START
        public InvoiceParameterInput UpdateInvoiveParametrs(InvoiceParameterInput ui)
        {
            logger.InfoFormat("UpdateInvoiveParametrs InvoiceParameterInput={0}", ui);
            InvoiceParameter inp = new InvoiceParameter();
            try
            {
                ui.SaveSuccessfull = true;
                if (ui.Mode == 1)
                {    //Update
                    inp = dcap.InvoiceParameter.Where(c => c.InvoiceKey == ui.InvoiceKey).FirstOrDefault();

                    if (inp != null)
                    {
                        inp.VAT = ui.VAT;
                        inp.NBT = ui.NBT;
                        inp.ExchangeRate = ui.ExchangeRate;
                        inp.EffectiveDateFrom = ui.EffectiveDateFrom;
                        inp.EffectiveDateTo = ui.EffectiveDateTo;
                        inp.ModifiedDateTime = DateTime.Now;
                        inp.ModifiedBy = ui.ModifiedBy;
                        dcap.SaveChanges();
                    }
                    else
                    {
                        ui.Responce = new string[2];
                        ui.Responce[0] = "No records found for the selected criteria";
                        ui.Responce[1] = "please check the database.";
                        ui.SaveSuccessfull = false;
                    }
                }
                else if (ui.Mode == 2)
                {   //delete
                    inp = dcap.InvoiceParameter.Where(c => c.InvoiceKey == ui.InvoiceKey).FirstOrDefault();

                    if (inp != null)
                    {
                        inp.RecStatus = 2;
                        dcap.SaveChanges();
                    }
                    else
                    {
                        ui.Responce = new string[2];
                        ui.Responce[0] = "No records found for the selected criteria";
                        ui.Responce[1] = "please check the database.";
                        ui.SaveSuccessfull = false;
                    }
                }
                else if (ui.Mode == 3)
                {   //Create
                    var inpc = dcap.InvoiceParameter.Where(c => c.EffectiveDateFrom >= ui.EffectiveDateFrom && c.EffectiveDateFrom <= ui.EffectiveDateTo).FirstOrDefault();
                    if (inpc == null)
                    {
                        int maxid = dcap.InvoiceParameter.Max(c => Convert.ToInt16(c.NextInvoiceNo));

                        var maxtextid = Convert.ToString(maxid + 1);
                        int len = 5 - maxtextid.Length;
                        int i = 0;
                        while (i < len)
                        {
                            maxtextid = "0" + maxtextid;
                            i++;
                        }

                        inp.NextInvoiceNo = maxtextid;
                        inp.VAT = ui.VAT;
                        inp.NBT = ui.NBT;
                        inp.ExchangeRate = ui.ExchangeRate;
                        inp.EffectiveDateFrom = ui.EffectiveDateFrom;
                        inp.EffectiveDateTo = ui.EffectiveDateTo;
                        inp.RecStatus = 1;
                        inp.CreatedDateTime = DateTime.Now;
                        inp.CreatedBy = ui.ModifiedBy;
                        inp.CreatedMachine = ui.ModifiedMachine;
                        inp.ModifiedDateTime = DateTime.Now;
                        inp.ModifiedBy = ui.ModifiedBy;
                        inp.ModifiedMachine = ui.ModifiedMachine;

                        dcap.InvoiceParameter.Add(inp);
                        dcap.SaveChanges();
                    }
                    else
                    {
                        ui.Responce = new string[2];
                        ui.Responce[0] = "There is already and record for the selected effective date range.";
                        ui.Responce[1] = "Check the date range.";
                        ui.SaveSuccessfull = false;
                    }
                }
                else
                {
                    ui.Responce = new string[2];
                    ui.Responce[0] = "Selcted transaction mode error.";
                    ui.Responce[1] = "Please try again !";
                    ui.SaveSuccessfull = false;
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving UpdateInvoiveParametrs {0}", e.ToString());
                throw e;
            }
            return ui;
        }

        [Produces("application/json")]
        [HttpPost("AddInvoiceDetail")]
        public void AddInvoiceDetail(GoodControl ui, decimal Price, string InvoiceNumber, int NextInvoiceSeq)
        {
            logger.InfoFormat("AddGoodControl ui={0}", ui);
            try
            {
                InvoiceDetails objinvoicedetails = new InvoiceDetails();

                objinvoicedetails.InvoiceNo = InvoiceNumber;
                objinvoicedetails.InvoiceSeq = NextInvoiceSeq;
                objinvoicedetails.ControlId = ui.ControlId;
                objinvoicedetails.ControlType = (int)ui.ControlType;
                objinvoicedetails.Seq = (int)ui.Seq;
                objinvoicedetails.L1Id = ui.L1id;
                objinvoicedetails.L2Id = ui.L2id;
                objinvoicedetails.L3Id = ui.L3id;
                objinvoicedetails.L4Id = ui.L4id;
                objinvoicedetails.L5Id = ui.L5id;

                objinvoicedetails.BarcodeNo = ui.BarCodeNo;
                objinvoicedetails.TxnDateTime = DateTime.Now;
                objinvoicedetails.Qty01 = (decimal)ui.Qty01;
                objinvoicedetails.Qty02 = (decimal)ui.Qty02;
                objinvoicedetails.Qty03 = (decimal)ui.Qty03;

                objinvoicedetails.Price = Price;
                objinvoicedetails.Remark = "";

                objinvoicedetails.CreatedBy = ui.CreatedBy;
                objinvoicedetails.CreatedDateTime = DateTime.Now;
                objinvoicedetails.CreatedMachine = ui.CreatedMachine;

                objinvoicedetails.ModifiedBy = ui.ModifiedBy;
                objinvoicedetails.ModifiedDateTime = DateTime.Now;
                objinvoicedetails.ModifiedMachine = ui.ModifiedMachine;

                dcap.InvoiceDetails.Add(objinvoicedetails);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving information {0}", e.ToString());
                throw e;
            }

        }

        //Update Barcodes Status on invoice details table and pass create new invoice detail status if needed|| checked 8-4-2020
        //Used API's and UI : CreateInvoice (Businesscontrollers)
        /*  public Boolean UpdateInvoiceDetailData(GoodControl ui, int InvoiceSeq)
          {
              logger.InfoFormat("UpdateInvoiceDetailData ui={0}", ui);
              Boolean createnewInvoiceDetailRow = false;
              try
              {


                      InvoiceDetails objdetxn2 = new InvoiceDetails();
                      objdetxn2 = dcap.InvoiceDetails
                          .Where(c =>  c.L1Id == ui.L1id && c.L2Id == c.L2Id &&
                          c.L1id == ui.L1id && c.L2id == ui.L2id && c.L3id == ui.L3id && c.L4id == ui.L4id)
                          .FirstOrDefault();

                      if (objdetxn2 != null)
                      {
                          objdetxn2.Qty01 = objdetxn2.Qty01 + (int)ui.Qty01;
                          objdetxn2.ModifiedDateTime = DateTime.Now;
                          dcap.SaveChanges();
                          ui.createNewTravelGroup = false;
                      }
                      else
                      {
                          //TxnContrl.CreateTravelGroup(ui, 2);
                          ui.createNewTravelGroup = true;
                      }

              }
              catch (Exception e)
              {
                  logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                  throw e;
              }
              return createnewInvoiceDetailRow;
          }*/

        public void AddInvoiceHeaderDetail(InvoiceHeaderInformation ui)
        {
            logger.InfoFormat("AddInvoiceHeaderDetail ui={0}", ui);
            try
            {
                InvoiceHeaderInformation objinvoicedetails = new InvoiceHeaderInformation();

                objinvoicedetails.InvoiceNo = ui.InvoiceNo;
                objinvoicedetails.TxnDateTime = DateTime.Now;
                objinvoicedetails.VAT = ui.VAT;
                objinvoicedetails.NBT = ui.NBT;
                objinvoicedetails.TotalQty = ui.TotalQty;
                objinvoicedetails.TotalPrice = ui.TotalPrice;

                objinvoicedetails.CreatedBy = ui.CreatedBy;
                objinvoicedetails.CreatedDateTime = DateTime.Now;
                objinvoicedetails.CreatedMachine = ui.CreatedMachine;

                objinvoicedetails.ModifiedBy = ui.ModifiedBy;
                objinvoicedetails.ModifiedDateTime = DateTime.Now;
                objinvoicedetails.ModifiedMachine = ui.ModifiedMachine;

                dcap.InvoiceHeaderInformation.Add(objinvoicedetails);
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving information {0}", e.ToString());
                throw e;
            }
        }

        public GoodControlDetails UpdateGoodControlDetails(string controlid, int seq, string invoiceNumber)
        {

            logger.InfoFormat("UpdateDispatchStatus controlid={0}, seq={0}", controlid, seq);
            GoodControlDetails detail = null;

            try
            {
                detail = dcap.GoodControlDetails.Where(l => l.ControlId == controlid && l.Seq == seq).FirstOrDefault();

                if (detail != null)
                {
                    detail.InvoiceStatus = 1;
                    detail.InvoiceNumber = (detail.InvoiceNumber == null ? invoiceNumber : (detail.InvoiceNumber.Contains(invoiceNumber) ? detail.InvoiceNumber : (detail.InvoiceNumber + "/" + invoiceNumber)));
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Get Factory Name information {0}", e.ToString());
                throw e;
            }

            return detail;
        }

        public InvoiceParameter UpdateNextInvoiceNumber()
        {
            logger.InfoFormat("UpdateNextInvoiceNumber");

            DateTime TodaysDate = DateTime.Now;
            InvoiceParameter output = null;
            LookupController lookup = new LookupController(dcap);

            try
            {
                output = dcap.InvoiceParameter.Where(invoiceparameters => invoiceparameters.EffectiveDateFrom <= TodaysDate && invoiceparameters.EffectiveDateTo >= TodaysDate).FirstOrDefault();

                if (output != null)
                {
                    output.NextInvoiceNo = lookup.GetNextInvoiceNumber();
                }
                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return output;
        }

        public void UpdateInvoiceHeaderDetails(string InvoiceNo, decimal? Qty, decimal Price)
        {
            logger.InfoFormat("Update Invoice Header Details InvoiceNo={0}", InvoiceNo);
            InvoiceHeaderInformation invoicegeader = null;
            try
            {
                invoicegeader = dcap.InvoiceHeaderInformation.Where(c => c.InvoiceNo == InvoiceNo).FirstOrDefault();

                if (invoicegeader != null)
                {
                    invoicegeader.TotalQty = invoicegeader.TotalQty + (decimal)Qty;
                    invoicegeader.TotalPrice = invoicegeader.TotalPrice + Price;
                    dcap.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
        }

        //Update Invoice: START

        //Mainaince : START

        //Data achival
        public GeneralInput RemoveData(GeneralInput gi)
        {
            logger.InfoFormat("Remove Data GeneralInput={0}", gi);

            try
            {
                gi.Counts = new string[20];

                List<Dedep> dedep = dcap.Dedep.Where(c => c.L1id == gi.L1id).ToList();
                if (dedep.Count != 0)
                {
                    gi.Counts[0] = "Dedep : " + Convert.ToString(dedep.Count);
                    dcap.Dedep.RemoveRange(dedep);
                }

                List<Dedepinst> dedepinst = dcap.Dedepinst.Where(c => c.L1id == gi.L1id).ToList();
                if (dedepinst.Count != 0)
                {
                    gi.Counts[1] = "Dedepinst : " + Convert.ToString(dedepinst.Count);
                    dcap.Dedepinst.RemoveRange(dedepinst);
                }

                List<Detxn> detxn = dcap.Detxn.Where(c => c.L1id == gi.L1id).ToList();
                if (detxn.Count != 0)
                {
                    gi.Counts[2] = "Detxn : " + Convert.ToString(detxn.Count);
                    dcap.Detxn.RemoveRange(detxn);
                }

                List<L5bc> l5bc = dcap.L5bc.Where(c => c.L1id == gi.L1id).ToList();
                if (l5bc.Count != 0)
                {
                    gi.Counts[3] = "L5bc : " + Convert.ToString(l5bc.Count);
                    dcap.L5bc.RemoveRange(l5bc);
                }

                List<L5bcPrint> l5bcPrint = dcap.L5bcPrint.Where(c => c.L1id == gi.L1id).ToList();
                if (l5bcPrint.Count != 0)
                {
                    gi.Counts[4] = "L5bcPrint : " + Convert.ToString(l5bcPrint.Count);
                    dcap.L5bcPrint.RemoveRange(l5bcPrint);
                }

                List<L5moops> l5moops = dcap.L5moops.Where(c => c.L1id == gi.L1id).ToList();
                if (l5moops.Count != 0)
                {
                    gi.Counts[5] = "L5moops : " + Convert.ToString(l5moops.Count);
                    dcap.L5moops.RemoveRange(l5moops);
                }

                List<L5mo> l5mo = dcap.L5mo.Where(c => c.L1id == gi.L1id).ToList();
                if (l5mo.Count != 0)
                {
                    gi.Counts[6] = "L5mo : " + Convert.ToString(l5mo.Count);
                    dcap.L5mo.RemoveRange(l5mo);
                }

                List<L5> l5 = dcap.L5.Where(c => c.L1id == gi.L1id).ToList();
                if (l5.Count != 0)
                {
                    gi.Counts[7] = "L5 : " + Convert.ToString(l5.Count);
                    dcap.L5.RemoveRange(l5);
                }

                List<L4> l4 = dcap.L4.Where(c => c.L1id == gi.L1id).ToList();
                if (l4.Count != 0)
                {
                    gi.Counts[8] = "L4 : " + Convert.ToString(l4.Count);
                    dcap.L4.RemoveRange(l4);
                }

                List<L3> l3 = dcap.L3.Where(c => c.L1id == gi.L1id).ToList();
                if (l3.Count != 0)
                {
                    gi.Counts[8] = "L3 : " + Convert.ToString(l3.Count);
                    dcap.L3.RemoveRange(l3);
                }

                List<L2> l2 = dcap.L2.Where(c => c.L1id == gi.L1id).ToList();
                if (l2.Count != 0)
                {
                    gi.Counts[8] = "L2 : " + Convert.ToString(l2.Count);
                    dcap.L2.RemoveRange(l2);
                }

                List<Group_Barcode_Detail> group_Barcode_Detail = dcap.Group_Barcode_Detail.Where(c => c.L1id == gi.L1id).ToList();
                if (group_Barcode_Detail.Count != 0)
                {
                    gi.Counts[9] = "Group_Barcode_Detail : " + Convert.ToString(group_Barcode_Detail.Count);
                    dcap.Group_Barcode_Detail.RemoveRange(group_Barcode_Detail);
                }

                List<GroupBarcode> group_Barcode = dcap.GroupBarcode.Where(c => c.L1id == gi.L1id).ToList();
                if (group_Barcode.Count != 0)
                {
                    gi.Counts[10] = "Group_Barcode : " + Convert.ToString(group_Barcode.Count);
                    dcap.GroupBarcode.RemoveRange(group_Barcode);
                }

                List<GoodControl> good_Control = dcap.GoodControl.Where(c => c.L1id == gi.L1id).ToList();
                if (good_Control.Count != 0)
                {
                    var preControlId = "";
                    gi.Counts[11] = "Good_Control_Detail : ";
                    foreach (GoodControl g in good_Control)
                    {
                        if (preControlId != g.ControlId)
                        {
                            List<GoodControlDetails> good_Control_Detail = dcap.GoodControlDetails.Where(c => c.ControlId == g.ControlId).ToList();
                            if (good_Control_Detail.Count != 0)
                            {
                                gi.Counts[11] = gi.Counts[11] + " / " + Convert.ToString(good_Control.Count);
                                dcap.GoodControlDetails.RemoveRange(good_Control_Detail);
                            }
                        }
                        preControlId = g.ControlId;
                    }

                    gi.Counts[12] = "Good_Control : " + Convert.ToString(good_Control.Count);
                    dcap.GoodControl.RemoveRange(good_Control);
                }

                L1 l1 = dcap.L1.Where(c => c.L1id == gi.L1id).FirstOrDefault();
                if (l1 != null)
                {
                    l1.AchivedComments = "Removed All records from master and txn tables";
                    l1.ModifiedBy = gi.ModifiedBy;
                    l1.ModifiedMachine = gi.ModifiedMachine;
                    l1.ModifiedDateTime = DateTime.Now;
                    l1.RecStatus = 2;
                }

                gi.SaveSuccessfull = true;

                dcap.SaveChanges();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
            return gi;
        }

        //DataCorrection
        public void DataCorrection()
        {
            logger.InfoFormat("DataCorrection");
            try
            {
                List<GroupBarcode> gb = dcap.GroupBarcode.Where(c => c.TxnStatus == 5 && c.Qty01 == c.Qty01NS).ToList();

                if (gb.Count != 0)
                {
                    foreach (GroupBarcode g in gb)
                    {
                        g.TxnStatus = 6;
                        g.ModifiedBy = "Manual Correction";
                        dcap.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving GetColorsByStylesNo information {0}", e.ToString());
                throw e;
            }
        }


        public Boolean UpdateDEDEPInstError(Dedepinst ui)
        {
            logger.InfoFormat("UpdateDEDEPInstError Dedepinst={0}", ui);
            Boolean savesuccessfull = false;
            try
            {
                Dedepinst objdedep = new Dedepinst();
                objdedep = dcap.Dedepinst
                        .Where(c => c.dedepinstKey == ui.dedepinstKey).FirstOrDefault();

                if (objdedep != null)
                {
                    objdedep.Qty01 = ui.Qty01;
                    objdedep.Qty02 = ui.Qty02;
                    objdedep.Qty03 = ui.Qty03;
                    objdedep.ModifiedDateTime = DateTime.Now;
                    objdedep.ModifiedBy = ui.CreatedBy;
                    objdedep.ModifiedMachine = ui.CreatedMachine;

                    dcap.SaveChanges();
                    savesuccessfull = true;
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving UpdateDEDEPInstError information {0}", e.ToString());
                throw e;
            }

            return savesuccessfull;
        }
        //Maintaine : END

        #endregion
    }
}