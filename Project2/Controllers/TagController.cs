using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using Newtonsoft.Json;
using Project2.CustomAttributes;
using System.Web.Http.Cors;
using Common.Base;

namespace Project2.Controllers
{
    [RoutePrefix("API/Tag")]
    public class TagController : BaseController
    {
        private TagDAL tagDAL;
        public TagController() => tagDAL = new TagDAL(DbProvider);

        [HttpGet]
        [Route("GetHotelTag")]
        public IHttpActionResult GetHotelTag()
        {
            return Ok(tagDAL.GetAll(0));
        }

        [HttpGet]
        [Route("GetRoomTag")]
        public IHttpActionResult GetRoomTag()
        {
            return Ok(tagDAL.GetAll(1));
        }
    }
}