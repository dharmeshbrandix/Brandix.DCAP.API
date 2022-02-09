/*
Description: Security Controller Class
Created By : DineshWij
Created on : 2018-09-29
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Brandix.DCAP.API.Models;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Cors;
using Brandix.DCAP.API.CustomModels;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using MySql.Data;
using MySqlConnector;

namespace Brandix.DCAP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]
    public class SecurityController : ControllerBase
    {
        #region Variable Declarations
        static ILog logger = LogManager.GetLogger(typeof(SecurityController));
        private DCAPDbContext dcap;
        #endregion

        #region Constructor
        public SecurityController(DCAPDbContext context)
        {
            dcap = context;
        }
        #endregion

        #region APIs
        // GET api/Security/GetClientconfig - API to get ClientId by GUID information
        [Produces("application/json")]
        [HttpGet("GetClientByIP")]
        public TClientconfig GetClientByIP(string clientIP)
        {
            logger.InfoFormat("GetClientByIP method called with clientIP={0}", clientIP);
            TClientconfig client = null;
            try
            {
                client = dcap.TClientconfig.FromSql("CALL GetClientConfigInfor (@clientIP)", new MySqlParameter("@clientIP", clientIP)).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client clientIP {0}", e.ToString());
            }
            return client;
        }

        // GET api/Security/GetClientconfig - API to get ClientId by GUID information
        [Produces("application/json")]
        [HttpGet("GetClientByUserID")]
        public SClientconfig GetClientByUserID(string userId)
        {
            logger.InfoFormat("GetClientByUserID method called with userId={0}", userId);
            SClientconfig client = null;
            try
            {
                client = (from su in dcap.Clientconfig
                          join t in dcap.Team on su.TeamId equals t.TeamId
                          join f in dcap.Factory on t.FacCode equals f.FacCode
                          where su.UserId == userId && su.RecStatus == 1
                          select new SClientconfig
                          {
                              UserId = su.UserId,
                              ClientId = su.ClientId,
                              OpCode1 = su.OpCode1,
                              OpCode2 = su.OpCode2,
                              OperationName = su.OperationName,
                              SelectMode = su.SelectMode,
                              DataCaptureMode = su.DataCaptureMode,
                              LoginMode = su.LoginMode,
                              RecStatus = su.RecStatus,
                              TeamName = t.TeamName,
                              FacCode = f.FacCode,
                              FacName = f.FacName,
                              TeamId = (int?)t.TeamId,
                              WfdepinstId = su.WfdepinstId,
                              WfId = su.WfId,
                              ClientIP = "",
                          }).FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client clientIP {0}", e.ToString());
            }
            return client;
        }

        // GET api/Security/GetSecuser - API to get Secuser Information
        [Produces("application/json")]
        [HttpGet("GetSecuser")]
        public Secuser GetSecuser(string userId, string userIdNum, string password)
        {
            logger.InfoFormat("GetSecuser API called with userId = {0}, userIdNum = {1}, password={2}", userId, userIdNum, "********");

            Secuser secUser = null;
            try
            {
                secUser = dcap.Secuser
                            .Where(s => (s.UserIdN == userIdNum || s.UserId == userId) && s.Password == password)
                            .FirstOrDefault();
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Secuser information {0}", e.ToString());
            }
            return secUser;
        }

        // GET api/Security/GetSecuser - API to get Secuser Permissions
        [Produces("application/json")]
        [HttpGet("GetUserPermissions")]
        public IList<UserPermission> GetUserPermissions(string userId)
        {
            logger.InfoFormat("GetUserPermissions API called with userId={0}", userId);

            IList<UserPermission> userPermission = null;
            try
            {
                userPermission = (from su in dcap.Secuser
                                  join sd in dcap.Secuserrightdep on new { A = su.UserId } equals new { A = sd.UserId }
                                  join sf in dcap.Secfunction on sd.FunctionId equals sf.FunctionId
                                  where (su.UserId == userId || su.UserIdN == userId) && sd.RecStatus == (int)eRecStatus.Active
                                  select new UserPermission
                                  {
                                      UserId = su.UserId,
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
                logger.ErrorFormat("Error while retrieving Secuser information {0}", e.ToString());
            }
            return userPermission;
        }


        // GET api/Security/GetSecuser - API to get Secuser Permissions
        [Produces("application/json")]
        [HttpGet("GetUserRight")]
        public bool GetUserRight(string userId, string FnctCode)
        {
            logger.InfoFormat("GetUserPermissions API called with userId={0}", userId);

            IList<UserPermission> userPermission = null;
            try
            {
                userPermission = (from ur in dcap.Secuserrightdep
                                  where ur.UserId == userId && ur.FunctionId == FnctCode
                                  select new UserPermission
                                  {
                                      UserId = ur.UserId

                                  }).ToList();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Secuser information {0}", e.ToString());
            }
            if (userPermission == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // GET api/Security/GetClientconfig - API to get ClientId by GUID information
        [Produces("application/json")]
        [HttpGet("GetClientByWfIdandIP")]
        public SClientconfig GetClientByWfIdandIP(string clientIP, int wfid, string clientId, int scanmode)
        {
            // ClientId,OpCode1,OpCode2,OperationName,SelectMode,DataCaptureMode,LoginMode,
            // c.RecStatus,TeamName,FacName,t.TeamId,WFId,WFDEPInstId

            logger.InfoFormat("GetClientByWfIdandIP method called with clientIP={0} wfid={1} clientId={2} scanmode={3}", clientIP, wfid, clientId, scanmode);
            SClientconfig client = null;

            try
            {
                if (clientIP != null || clientId != null)
                {
                    if (clientIP == null)
                    {
                        if (scanmode == 0)
                        {
                            client = (from d in dcap.Clientconfig.Where(d => d.ClientIp == "" && d.WfId == wfid && d.ClientId == clientId).AsQueryable()
                                      join t in dcap.Team
                                      on new { A = d.TeamId } equals new { A = t.TeamId } into ts
                                      from t in ts.DefaultIfEmpty()
                                      join f in dcap.Factory
                                      on new { A = t.FacCode } equals new { A = f.FacCode } into fs
                                      from f in fs.DefaultIfEmpty()
                                          //where d.ClientIp == "" && d.WfId == wfid && d.ClientId == clientId   //scanmode: bulk=0, indvidual=1       
                                      select new SClientconfig
                                      {
                                          UserId = d.UserId,
                                          ClientId = d.ClientId,
                                          OpCode1 = d.OpCode1,
                                          OpCode2 = d.OpCode2,
                                          OperationName = d.OperationName,
                                          SelectMode = d.SelectMode,
                                          DataCaptureMode = d.DataCaptureMode,
                                          LoginMode = d.LoginMode,
                                          RecStatus = d.RecStatus,
                                          TeamId = (int)t.TeamId,
                                          TeamName = t.TeamName,
                                          FacName = f.FacName,
                                          WfId = d.WfId,
                                          WfdepinstId = d.WfdepinstId,
                                      }).FirstOrDefault();
                        }
                        else
                        {
                            client = (from d in dcap.Clientconfig.Where(d => d.ClientIp == "" && d.WfId == wfid && d.ClientId == clientId && d.TxnMode == scanmode).AsQueryable()
                                      join t in dcap.Team
                                      on new { A = d.TeamId } equals new { A = t.TeamId } into ts
                                      from t in ts.DefaultIfEmpty()
                                      join f in dcap.Factory
                                      on new { A = t.FacCode } equals new { A = f.FacCode } into fs
                                      from f in fs.DefaultIfEmpty()
                                          //where d.ClientIp == "" && d.WfId == wfid && d.ClientId == clientId && d.TxnMode == scanmode   //scanmode: bulk=0, indvidual=1       
                                      select new SClientconfig
                                      {
                                          UserId = d.UserId,
                                          ClientId = d.ClientId,
                                          OpCode1 = d.OpCode1,
                                          OpCode2 = d.OpCode2,
                                          OperationName = d.OperationName,
                                          SelectMode = d.SelectMode,
                                          DataCaptureMode = d.DataCaptureMode,
                                          LoginMode = d.LoginMode,
                                          RecStatus = d.RecStatus,
                                          TeamId = (int)t.TeamId,
                                          TeamName = t.TeamName,
                                          FacName = f.FacName,
                                          WfId = d.WfId,
                                          WfdepinstId = d.WfdepinstId,
                                      }).FirstOrDefault();
                        }

                    }
                    else
                    {
                        if (scanmode == 0)
                        {
                            client = (from d in dcap.Clientconfig.Where(d => d.ClientIp == clientIP && d.WfId == wfid && d.ClientId == clientId).AsQueryable()
                                      join t in dcap.Team
                                      on new { A = d.TeamId } equals new { A = t.TeamId } into ts
                                      from t in ts.DefaultIfEmpty()
                                      join f in dcap.Factory
                                      on new { A = t.FacCode } equals new { A = f.FacCode } into fs
                                      from f in fs.DefaultIfEmpty()
                                          //where d.ClientIp == clientIP && d.WfId == wfid && d.ClientId == clientId   //scanmode: bulk=0, indvidual=1       
                                      select new SClientconfig
                                      {
                                          UserId = d.UserId,
                                          ClientId = d.ClientId,
                                          OpCode1 = d.OpCode1,
                                          OpCode2 = d.OpCode2,
                                          OperationName = d.OperationName,
                                          SelectMode = d.SelectMode,
                                          DataCaptureMode = d.DataCaptureMode,
                                          LoginMode = d.LoginMode,
                                          RecStatus = d.RecStatus,
                                          TeamId = (int)t.TeamId,
                                          TeamName = t.TeamName,
                                          FacName = f.FacName,
                                          WfId = d.WfId,
                                          WfdepinstId = d.WfdepinstId,
                                      }).FirstOrDefault();
                        }
                        else
                        {
                            client = (from d in dcap.Clientconfig.Where(d => d.ClientIp == clientIP && d.WfId == wfid && d.ClientId == clientId && d.TxnMode == scanmode).AsQueryable()
                                      join t in dcap.Team
                                      on new { A = d.TeamId } equals new { A = t.TeamId } into ts
                                      from t in ts.DefaultIfEmpty()
                                      join f in dcap.Factory
                                      on new { A = t.FacCode } equals new { A = f.FacCode } into fs
                                      from f in fs.DefaultIfEmpty()
                                          //where d.ClientIp == clientIP && d.WfId == wfid && d.ClientId == clientId && d.TxnMode == scanmode   //scanmode: bulk=0, indvidual=1       
                                      select new SClientconfig
                                      {
                                          UserId = d.UserId,
                                          ClientId = d.ClientId,
                                          OpCode1 = d.OpCode1,
                                          OpCode2 = d.OpCode2,
                                          OperationName = d.OperationName,
                                          SelectMode = d.SelectMode,
                                          DataCaptureMode = d.DataCaptureMode,
                                          LoginMode = d.LoginMode,
                                          RecStatus = d.RecStatus,
                                          TeamId = (int)t.TeamId,
                                          TeamName = t.TeamName,
                                          FacName = f.FacName,
                                          WfId = d.WfId,
                                          WfdepinstId = d.WfdepinstId,
                                      }).FirstOrDefault();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client Error {0}", e.ToString());
            }
            return client;
        }

        [Produces("application/json")]
        [HttpGet("GetWfDEPInstByClientWF")]
        public TClientconfig GetWfDEPInstByClientWF(string ClientId, int WFId)
        {
            logger.InfoFormat("GetWfDEPInstByClientWF API called with ClientId={0},WFId={0}", ClientId, WFId);
            TClientconfig client = null;

            try
            {

                client = (from d in dcap.Clientconfig
                          where d.ClientId == ClientId && d.WfId == WFId
                          select new TClientconfig
                          {
                              WfId = d.WfId,
                              WfdepinstId = d.WfdepinstId
                          }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client Error {0}", e.ToString());
            }
            return client;
        }

        [Produces("application/json")]
        [HttpGet("GetWfDEPInstByTeamOppWF")]
        public Wfdep GetWfDEPInstByTeamOppWF(int TeamId, int OppCode, int WFId)
        {
            logger.InfoFormat("GetWfDEPInstByTeamOppWF API called with TeamId={0},OperationCode={0},WFId={0}", TeamId, OppCode, WFId);
            Wfdep wfdep = null;

            try
            {

                wfdep = (from d in dcap.Wfdep
                         where d.Wfid == WFId && d.TeamId == TeamId && d.OperationCode == OppCode
                         select new Wfdep
                         {
                             Wfid = d.Wfid,
                             WfdepinstId = d.WfdepinstId
                         }).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client Error {0}", e.ToString());
            }
            return wfdep;
        }

        [Produces("application/json")]
        [HttpGet("GetWfDEPInstByWfInstId")]
        public Wfdep GetWfDEPInstByWfInstId(uint Wfinstid)
        {
            logger.InfoFormat("GetWfDEPInstByWfInstId API called with Wfinstid={0}", Wfinstid);
            Wfdep wfdep = null;

            try
            {

                wfdep = dcap.Wfdep.Where(d => d.WfdepinstId == Wfinstid).FirstOrDefault();

            }
            catch (Exception e)
            {
                logger.ErrorFormat("Error while retrieving Client Error {0}", e.ToString());
            }
            return wfdep;
        }
        
        #endregion
    }
}
