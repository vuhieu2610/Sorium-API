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
using System.Drawing.Imaging;
using System.Web.Http.ModelBinding;

namespace Project2.Controllers
{
    [RoutePrefix("API/Room")]
    public class RoomController : BaseController
    {
        private RoomDAL roomDAL;
        private FileUploadedDAL fileUplloadedDAL;

        public RoomController()
        {
            fileUplloadedDAL = new FileUploadedDAL(DbProvider);
            roomDAL = new RoomDAL(DbProvider);
        }

        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<RoomResult> condition)
        {
            List<FilterItems> FilterRules = condition.FilterRules;
            List<SortItems> SortRules = condition.SortRules;

            if (FilterRules != null && FilterRules.Count > 0)
            {
                foreach (var item in FilterRules.Select((value, i) => new { i, value }))
                {
                    FilterItems e = item.value;
                    int Index = item.i;
                    if (e.field == "HotelName")
                    {
                        condition.FilterRules[Index].field = "H.Name";
                    }
                    else
                    {
                        condition.FilterRules[Index].field = "R." + e.field;
                    }
                }
            }

            if (SortRules != null && SortRules.Count > 0)
            {
                foreach (var item in SortRules.Select((value, i) => new { i, value }))
                {
                    SortItems e = item.value;
                    int Index = item.i;
                    if (e.field == "HotelName")
                    {
                        condition.FilterRules[Index].field = "H.Name";
                    }
                    else
                    {
                        condition.FilterRules[Index].field = "R." + e.field;
                    }
                }
            }

            var req = roomDAL.GetPaging(condition);
            if (req.Succeeded)
            {
                return Ok(req);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, req);
            }
        }

        [HttpGet]
        [Route("GetDetail/{Id}")]
        public IHttpActionResult GetDetail(int Id = 0)
        {
            ApiResult<RoomDetail> rs = roomDAL.GetDetail(Id);
            if (rs.Succeeded)
            {
                return Ok(rs);
            } else
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }
        }

        [HttpPost]
        [Route("Insert")]
        [ApiCustomFilter]
        public IHttpActionResult Insert(RoomDetail Item)
        {

            ApiResult<RoomResult> rs = new ApiResult<RoomResult>();
            
            if (Item == null)
            {
                rs.Failed(new ErrorObject
                {
                    Code = "EXCEPTION",
                    Description = "Data not Found"
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
                //IEnumerable<ModelError> a = ModelState.Values.SelectMany(x => x.Errors);

                //return Content(HttpStatusCode.BadRequest, a);
            }


            UploadFileResult uploadLogo = null;

            if (Item.Avatar.StartsWith("data:image"))
            {
                uploadLogo = CommonUtil.UploadBase64File(
                            Item.Avatar,
                            string.Format("{0}_{1}.png", Item.Name.Replace(" ", "_"), "Avatar"),
                            ConfigUtil.GetConfigurationValueFromKey("RoomAvatarDerectory", false),
                            ImageFormat.Png,
                            20
                    );

                if (uploadLogo != null && !uploadLogo.HasError)
                {
                    Item.Avatar = uploadLogo.FilePath;
                }
            }


            rs = roomDAL.Insert(Item, UserInfo.Id);
            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);
            return Ok(rs);
        }

        [HttpPut]
        [Route("Update")]
        [ApiCustomFilter]
        public IHttpActionResult Update(RoomDetail Item)
        {

            ApiResult<RoomResult> rs = new ApiResult<RoomResult>();

            if (Item == null)
            {
                rs.Failed(new ErrorObject
                {
                    Code = "EXCEPTION",
                    Description = "Đéo nhận dữ liệu truyền vào! 😒 "
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


            UploadFileResult uploadLogo = null;

            if (Item.Avatar.StartsWith("data:image"))
            {
                uploadLogo = CommonUtil.UploadBase64File(
                            Item.Avatar,
                            string.Format("{0}_{1}.png", Item.Name.Replace(" ", "_"), "Avatar"),
                            ConfigUtil.GetConfigurationValueFromKey("RoomAvatarDerectory", false),
                            ImageFormat.Png,
                            20
                    );

                if (uploadLogo != null && !uploadLogo.HasError)
                {
                    Item.Avatar = uploadLogo.FilePath;
                }
            }


            rs = roomDAL.Update(Item, UserInfo.Id);
            if (!rs.Succeeded) return Content(HttpStatusCode.BadRequest, rs);
            return Ok(rs);
        }

        [HttpDelete]
        [Route("Delete/{RoomId}")]
        [ApiCustomFilter]
        public IHttpActionResult Delete(int RoomId)
        {
            ApiResult<RoomResult> rs = roomDAL.Delete(RoomId, UserInfo.Id);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);
        }
    }
}