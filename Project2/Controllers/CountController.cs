using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using Common.Base;
using Common.BaseInfo;

namespace Project2.Controllers
{
    [RoutePrefix("API/DashBroad")]
    public class CountController : BaseController
    {
        private HomeDAL homeDAL;
        public CountController()
        {
            homeDAL = new HomeDAL(DbProvider);
        }

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            return Ok(homeDAL.GetCount());
        }
    }
}