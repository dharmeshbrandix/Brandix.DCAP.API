/*
Description: Secuser Controller Class
Created By : NalindaW
Created on : 2019-09-25
 */
 
using log4net;
using Microsoft.AspNetCore.Http;
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
using System.Web;

 namespace Brandix.DCAP.API.Controllers
{
    public class RequestLogger {
        static ILog logger = LogManager.GetLogger(typeof(RequestLogger));  
        private readonly RequestDelegate _next;

        public RequestLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //Continue down the Middleware pipeline, eventually returning to this class          
            LogicalThreadContext.Properties["guid"] = Guid.NewGuid().ToString();
            DateTime start=  DateTime.Now; 
            HttpRequest request = context.Request;
            //string combindedString = string.Join( ",", request.Headers.ToArray());
            
            logger.InfoFormat("Request Start {0} : {1} ",   request.Path, request.QueryString);

            await _next(context); 

            logger.InfoFormat("Request Ended - {0} : {1} ms ", request.Path, (DateTime.Now - start).TotalMilliseconds );

        }
    }
}
