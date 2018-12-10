using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Common;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.BaseInfo;
using Common.SqlService;
using Common.Utilities;
using Project2.Models;

namespace Project2.Controllers
{
    public class BaseController :  Base
    {
        protected SqlDbProvider DbProvider { get; private set; }

        public BaseController()
        {
            DbProvider = new SqlDbProvider();
        }
        public UserResult UserInfo
        {
            get
            {
                var tokenCode = HttpContext.Current.Request.Headers["TokenCode"].ToString();
                return CacheUtil.GetCacheObject(tokenCode);
            }
        }
        public void Dispose()
        {
            DbProvider.Dispose();
        }
    }

    public class Base : ApiController, IDisposable
    {
        public int ReturnInt = 0;
        public bool ReturnBool = true;
        public string ReturnMsg;
        //public object ReturnObj;
        //public static readonly object CacheLockObject = new object();

    }
    public class ApiCustomFilterAttribute : ActionFilterAttribute
    {
        //private static readonly object CacheLockObject = new object();
        public override void OnActionExecuting(HttpActionContext context)
        {

            var result = false;
            var headerTokenCode = context.Request.Headers.SingleOrDefault(x => x.Key == "TokenCode");
            //var headerUserName = context.Request.Headers.SingleOrDefault(x => x.Key == "UserName");
            //var headerUserInfo = context.Request.Headers.SingleOrDefault(x => x.Key == "UserInfo");
            //var tk = "";
            if (headerTokenCode.Value != null)
            {
                var tokenHeader = headerTokenCode.Value.First();
                //var userNameHeader = headerUserName.Value.First();
                //var tokenClient = EncryptCore.Md5Get(userNameHeader + EncryptCore.PassKey + EncryptCore.TimeToken);               
                Project2.Models.UserResult userInfo = CacheUtil.GetCacheObject(tokenHeader);
                if (userInfo != null)
                {
                    //tk = userInfo.TokenCode;
                    if (userInfo.AccessToken == tokenHeader)
                    {
                        result = true;
                    }
                }
            }
            //if (headerUserInfo.Value != null)
            //{
            //    UserInfo userInfo = Libs.DeserializeObject<UserInfo>(headerUserInfo.Value.First());
            //    var tokenHeader = userInfo.TokenCode;
            //    var userNameHeader = userInfo.Username;
            //    var tokenClient = "";
            //    lock (CacheUtil.CacheLockObject)
            //    {
            //        tokenClient = HttpRuntime.Cache[userNameHeader] as string;
            //    }
            //    if (tokenClient == tokenHeader)
            //    {
            //        result = true;
            //    }
            //}
            if (!result)
            {
                //Libs.WriteLog("xxxx", tk);
                //Libs.WriteLog("zzzz", headerTokenCode.Value.First());
                var Rs = new ApiResult<UserResult>() { };
                Rs.Failed(new ErrorObject {
                    Code = "400",
                    Description = "Sai TokenCode"
                });
                context.Response = context.Request.CreateResponse(HttpStatusCode.Forbidden, Rs, context.ControllerContext.Configuration.Formatters.JsonFormatter);
            }
        }


        //public override void OnActionExecuting(HttpActionContext context)
        //{
        //    var header = context.Request.Headers.SingleOrDefault(x => x.Key == "ApiKey");

        //    dynamic result = new JsonObject();

        //    var valid = header.Value != null || ApiKeyManager.ResetTimer(header.Value.First());

        //    if (!valid)
        //    {
        //        result.Message = "Invalid Authorization Key";
        //        result.Location = "/api/Authentication";
        //        context.Response = new HttpResponseMessage<JsonValue>(result, HttpStatusCode.Forbidden);
        //    }

        //}

    }
}