using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using Common.Security;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Text.RegularExpressions;
using Common.Base;

namespace Project2.Controllers
{
    [RoutePrefix("API/Comment")]
    public class CommentController : BaseController
    {
        private CommentDAL CommentDAL;

        public CommentController() => CommentDAL = new CommentDAL(DbProvider);

        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<CommentModels> condition)
        {
            return Ok(CommentDAL.GetPaging(condition));
        }

        [HttpPost]
        [Route("Post")]
        [ApiCustomFilter]
        public IHttpActionResult Insert(PostComment Item)
        {
            CommentModels Model = new CommentModels
            {
                Content = Item.Content,
                ParentId = Item.ParentId,
                HotelId = Item.HotelId,
                UserId = UserInfo.Id
            };

            ApiResult<bool> rs = CommentDAL.Insert(Model);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);
        }

        [HttpDelete]
        [Route("Delete")]
        [ApiCustomFilter]
        public IHttpActionResult Remove(int Id)
        {

            RemoveComment Model = new RemoveComment
            {
                Id = Id,
                UserId = UserInfo.Id
            };
            ApiResult<bool> rs = CommentDAL.Delete(Model);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);
        }

        [HttpGet]
        [Route("GetTop3")]
        public IHttpActionResult GetTop3()
        {
            return Ok(CommentDAL.GetTop3());
        }

    }
}