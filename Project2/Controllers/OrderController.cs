using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using System.Web;
using System.Web.Http.ModelBinding;
using Common.Base;

namespace Project2.Controllers
{
    [RoutePrefix("API/Order")]
    public class OrderController : BaseController
    {
        #region PrivateArea
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion

        private OrderDAL orderDAL;

        public OrderController()
        {
            orderDAL = new OrderDAL(DbProvider);
        }

        [HttpPost]
        [Route("DailyCheck")]
        public void DailyCheck() => orderDAL.DailyCheck();

        [HttpPost]
        [Route("Insert")]
        public IHttpActionResult Insert(PostOrder Item)
        {
            string tokenCode = HttpContext.Current.Request.Headers["TokenCode"] != null ? HttpContext.Current.Request.Headers["TokenCode"].ToString() : "";
            Models.UserResult UserInfo = null;

            if (!string.IsNullOrEmpty(tokenCode))
            {
                UserInfo = CacheUtil.GetCacheObject(tokenCode);
            }
            int UserId = (UserInfo != null && UserInfo.Id > 0) ? UserInfo.Id : 0;

            ApiResult<NewOrderResult> rs = new ApiResult<NewOrderResult>()
            {
                Data = new NewOrderResult()
            };

            if(Item == null || Item.Order == null )
            {
                rs.Failed(new ErrorObject
                {
                    Code = "EXCEPTION",
                    Description = "Đéo nhận Data truyền vào."
                });
                return Content(HttpStatusCode.BadRequest, rs);
            }

            if (!ModelState.IsValid)
            {
                // Lỗi validate dữ liệu trả ra từ model 
                foreach (string key in ModelState.Keys)
                {
                    ModelState current = ModelState[key];
                    foreach (ModelError error in current.Errors)
                    {
                        rs.Failed(new ErrorObject()
                        {
                            Code = key,
                            Description = error.ErrorMessage
                        });
                    }
                }

                return Content(HttpStatusCode.BadRequest, rs);
            }

            Item.Order.TokenCode = RandomString(15);
            rs = orderDAL.Insert(Item.Order, UserId);

            if (!rs.Succeeded)
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }

            string Link = ConfigUtil.DomainBaseHttp + "/API/Order/Confirm?Id="
                                                    + rs.Data.Id.ToString()
                                                    + "&TokenCode=" + Item.Order.TokenCode
                                                    + "&SucUrl=" + Item.SucUrl
                                                    + "&FailUrl=" + Item.FailUrl;

            object EmailData = new
            {
                Link,
                Item.Order.GuestName,
                SetTime = DateTime.Now.ToString(),
                Item.Order.RoomName
            };

            string EmailContent = EmailContentHtml.EmailContentFormat(EmailData, "ConfirmOrder.html");
            bool e = EmailUtility.SendMail(ConfigUtil.Email_DisplayName, Item.Order.GuestEmail, "Xác thực Đơn đặt phòng", messages: EmailContent);
            return Ok(rs);
        }

        [HttpGet]
        [Route("Confirm")]
        public IHttpActionResult ConfirmOrder(int Id = 0, string TokenCode = "", string SucUrl = "", string FailUrl = "")
        {
            ApiResult<bool> rs = new ApiResult<bool>();
            if (Id == 0)
            {
                rs.Failed(new ErrorObject {
                    Code="Id",
                    Description = "Id không được trống"
                });
            }
            if (string.IsNullOrEmpty(TokenCode) || string.IsNullOrWhiteSpace(TokenCode))
            {
                rs.Failed(new ErrorObject
                {
                    Code = "TokenCode",
                    Description = "TokenCode không được trống"
                });
            }
            if (string.IsNullOrEmpty(SucUrl) || string.IsNullOrWhiteSpace(SucUrl))
            {
                rs.Failed(new ErrorObject
                {
                    Code = "SucUrl",
                    Description = "SucUrl không được trống"
                });
            }
            if (string.IsNullOrEmpty(FailUrl) || string.IsNullOrWhiteSpace(FailUrl))
            {
                rs.Failed(new ErrorObject
                {
                    Code = "FailUrl",
                    Description = "FailUrl không được trống"
                });
            }

            if (rs.Errors.Count > 0)
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }
            rs = orderDAL.ConfirmEmail(Id, TokenCode);
            return rs.Succeeded ? Redirect(SucUrl) : Redirect(FailUrl);
        }


        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaing(BaseCondition<Order> Item)
        {
            ApiResult<OrderResult> rs = orderDAL.GetPaging(Item);
            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);
            return Ok(rs);
        }

        [HttpPost]
        [Route("Abort")]
        [ApiCustomFilter]
        public IHttpActionResult Abort(AbortModel Item)
        {
            ApiResult<bool> rs = orderDAL.Abort(Item.Id, UserInfo.Id);
            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);
            OrderDetail Order = orderDAL.GetOne(Item.Id).Data;
            object EmailData = new
            {
                Order.RoomName,
                Reason = Item.ReasonAbort
            };

            string EmailContent = EmailContentHtml.EmailContentFormat(EmailData, "Abort.html");
            bool e = EmailUtility.SendMail(ConfigUtil.Email_DisplayName, Order.GuestEmail, "Thông báo hủy đơn đặt phòng.", messages: EmailContent);
            return Ok(rs);
        }

        [HttpPost]
        [Route("MarkIsPaid/{RoomId}")]
        [ApiCustomFilter]
        public IHttpActionResult MarkIsPaid(int RoomId)
        {
            ApiResult<bool> rs = orderDAL.Paid(RoomId, UserInfo.Id);

            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);
            
            return Ok(rs);
        }


        [HttpGet]
        [Route("GetItem/{OrderId}")]
        public IHttpActionResult GetItem(int OrderId)
        {
            ApiResult<OrderDetail> rs = orderDAL.GetOne(OrderId);

            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);

            return Ok(rs);
        }

        [HttpPost]
        [Route("Checkin/{OrderId}")]
        public IHttpActionResult Checkin(int OrderId)
        {
            return Ok(orderDAL.Checkin(OrderId));
        }

        [HttpPost]
        [Route("Checkout/{OrderId}")]
        public IHttpActionResult Checkout(int OrderId)
        {
            return Ok(orderDAL.Checkout(OrderId));
        }

        

    }
}