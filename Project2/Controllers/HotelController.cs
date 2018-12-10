using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using System.Web;
using System.Web.Http.ModelBinding;
using System.Drawing.Imaging;
using Common.Base;
using Common.BaseInfo;

namespace Project2.Controllers
{
    [RoutePrefix("API/Hotel")]
    public class HotelController : BaseController
    {
        private HotelDAL hotelDAL;
        private FileUploadedDAL fileUplloadedDAL;
        private RoomDAL roomDAL;
        private UserDAL userDAL;

        public HotelController()
        {
            fileUplloadedDAL = new FileUploadedDAL(DbProvider);
            hotelDAL = new HotelDAL(DbProvider);
            roomDAL = new RoomDAL(DbProvider);
            userDAL = new UserDAL(DbProvider);
        }

        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<HotelPaging> condition)
        {
            string tokenCode = HttpContext.Current.Request.Headers["TokenCode"] != null ? HttpContext.Current.Request.Headers["TokenCode"].ToString() : "";
            Models.UserResult UserInfo = null;

            if (!string.IsNullOrEmpty(tokenCode))
            {
                UserInfo = CacheUtil.GetCacheObject(tokenCode);
            }
            int UserId = (UserInfo != null && UserInfo.Id > 0) ? UserInfo.Id : 0;

            List<FilterItems> FilterRules = condition.FilterRules;
            List<SortItems> SortRules = condition.SortRules;
            
            if (SortRules != null && SortRules.Count > 0)
            {
                foreach (var item in SortRules.Select((value, i) => new { i, value }))
                {
                    SortItems e = item.value;
                    int Index = item.i;
                    if (e.field.ToLower() == "minprice")
                    {
                        condition.SortRules[Index].field = "CAST(MinPrice as Int)";
                        continue;
                    }

                }
            }


            var req = hotelDAL.GetPaging(condition, UserId);
            if (req.Succeeded)
            {
                return Ok(req);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, req);
            }
        }

        [HttpPost]
        [Route("RegisterHotel")]
        [ApiCustomFilter]
        public IHttpActionResult ClientInsert(PostHotel Item)
        {
            ApiResult<HotelModels> rs = new ApiResult<HotelModels>()
            {
                Data = new HotelModels()
            };

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
            }
            rs = hotelDAL.Register(Item, UserInfo.Id);
            if (!rs.Succeeded || !rs.HasData || rs.Data.Id <= 0)
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }

            // ==== Upload ảnh chứng minh == //

            List<string> Images = Item.Image;
            if (Images != null && Images.Count > 0)
            {
                foreach (string i in Images)
                {
                    string FileName = RandomString(10) + ".png";
                    string UpRs = UploadHotelImage(i, rs.Data.Id, FileName);
                    if (String.IsNullOrEmpty(UpRs)) break;
                    Models.FileUploaded FileItem = new Models.FileUploaded
                    {
                        Url = UpRs,
                        FileName = FileName,
                        Table = "Hotel",
                        RowId = rs.Data.Id
                    };

                    fileUplloadedDAL.Insert(FileItem, UserInfo.Id);

                }
            }

            // ============================= //



            return Ok(rs);
        }


        [HttpPost]
        [ApiCustomFilter]
        [Route("ConfirmHotel/{HotelId}")]
        public IHttpActionResult ConfirmHotel(int HotelId)
        {
            var rs = hotelDAL.ConfirmHotel(HotelId, UserInfo.Id);
            if (!rs.Succeeded)
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }
            return Ok(rs);
        }

        [HttpPost]
        [Route("GetItemByClient/{HotelId}")]
        public IHttpActionResult GetItemByClient(BaseCondition<RoomResult> condition, int HotelId)
        {
            ApiResult<HotelResult> rs = hotelDAL.GetItemByClient(HotelId);
            if (condition == null)
            {
                condition = new BaseCondition<RoomResult>()
                {
                    PageIndex = 1,
                    PageSize = 10,
                };
            };
            condition.FilterRules = new List<FilterItems>();
            FilterItems FilterItems = new FilterItems
            {
                field = "R.HotelId",
                op = "=",
                value = HotelId.ToString()
            };
            condition.FilterRules.Add(FilterItems);
            ApiResult<RoomResult> ListRoom = roomDAL.GetPaging(condition);
            if (ListRoom.Succeeded)
            {
                rs.Data.ListRoom = ListRoom.DataList;
                rs.TotalRecords = ListRoom.TotalRecords;
            }
            if (!rs.Succeeded)
            {
                return Content(HttpStatusCode.BadRequest, rs);
            }
            return Ok(rs);
        }

        [HttpGet]
        [Route("BEGetitem/{HotelId}")]
        [ApiCustomFilter]
        public IHttpActionResult BEGetItem(int HotelId)
        {
            int UserId = UserInfo.Id;
            User If = userDAL.GetById(UserId).Data;
            int RoleId = If.RoleId;

            ApiResult<HotelResult> Rs = hotelDAL.AdminGetItem(HotelId, UserId, RoleId);
            if (!Rs.Succeeded) return Content(HttpStatusCode.BadRequest, Rs);
            return Ok(Rs);
        }

        [HttpPut]
        [Route("OwnerUpdate")]
        [ApiCustomFilter]
        public IHttpActionResult OwnerUpdate([FromBody] PutHotel Item)
        {
            ApiResult<HotelModels> Rs = new ApiResult<HotelModels>();
            if (Item == null)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = "Đéo nhận Data truyền vào"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }

            if (!ModelState.IsValid)
            {
                // Lỗi validate dữ liệu trả ra từ model 
                foreach (string key in ModelState.Keys)
                {
                    ModelState current = ModelState[key];
                    foreach (ModelError error in current.Errors)
                    {
                        Rs.Failed(new ErrorObject()
                        {
                            Code = key,
                            Description = error.ErrorMessage
                        });
                    }
                }

                return Content(HttpStatusCode.BadRequest, Rs);
            }
            int UserId = UserInfo.Id;

            UploadFileResult UploadAvatar = null;
            UploadFileResult UploadCoverImage = null;

            if (Item.Avatar != null && Item.Avatar.StartsWith("data:image") && Item.Avatar.IndexOf(";base64,") > -1)
            {
                UploadAvatar = CommonUtil.UploadBase64File(
                            Item.Avatar,
                            string.Format("{0}_{1}.png", Item.Name.Replace(" ", "_"), "Avatar"),
                            string.Format(ConfigUtil.GetConfigurationValueFromKey("HotelAvatarDerectory", false), Item.Id),
                            ImageFormat.Png,
                            20
                    );

                if (UploadAvatar != null && !UploadAvatar.HasError)
                {
                    Item.Avatar = UploadAvatar.FilePath;
                }
                else
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = Constants.ERR_EXCEPTION,
                        Description = "Cannot Upload Avatar"
                    });

                    return Content(HttpStatusCode.BadRequest, Rs);
                }
            }

            if (Item.CoverImage != null && Item.CoverImage.StartsWith("data:image") && Item.CoverImage.IndexOf(";base64,") > -1)
            {
                UploadCoverImage = CommonUtil.UploadBase64File(
                            Item.CoverImage,
                            string.Format("{0}_{1}.png", Item.Name.Replace(" ", "_"), "CoverImage"),
                            string.Format(ConfigUtil.GetConfigurationValueFromKey("HotelCoverDerectory", false), Item.Id),
                            ImageFormat.Png,
                            20
                    );

                if (UploadCoverImage != null && !UploadCoverImage.HasError)
                {
                    Item.CoverImage = UploadCoverImage.FilePath;
                }
                else
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = Constants.ERR_EXCEPTION,
                        Description = "Cannot Upload Cover Image"
                    });

                    return Content(HttpStatusCode.BadRequest, Rs);
                }
            }

            if (Item.Images != null && Item.Images.Count > 0)
            {
                foreach (var item in Item.Images.Select((value, i) => new { i, value }))
                {
                    int Index = item.i;
                    var E = item.value;
                    UploadFileResult rt = null;
                    string FileName = RandomString(16) + ".png";

                    try
                    {
                        rt = CommonUtil.UploadBase64File(
                            E,
                            FileName,
                            string.Format(ConfigUtil.GetConfigurationValueFromKey("HotelAlbumDirectory", false), Item.Id),
                            ImageFormat.Png,
                            20
                        );
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (rt != null && !rt.HasError)
                    {
                        //Item.Images[Index] = "";
                        FileUploadCompact fi = new FileUploadCompact
                        {
                            FIleName = FileName,
                            Url = rt.FilePath
                        };
                        Item.Album.Add(fi);
                    }
                    else
                    {
                        continue;
                    }
                }
                Item.Images = null;
            }

            Rs = hotelDAL.OwnerUpdate(Item, UserId);

            if (!Rs.Succeeded) return Content(HttpStatusCode.BadRequest, Rs);

            return Ok(Rs);
        }

        [HttpGet]
        [Route("Activate/{HotelId}")]
        [ApiCustomFilter]
        public IHttpActionResult Activate(int HotelId = 0)
        {
            ApiResult<bool> Rs = hotelDAL.Activate(HotelId, UserInfo.Id);
            return Rs.Succeeded ? Ok(Rs) : (IHttpActionResult)Content(HttpStatusCode.BadRequest, Rs);
        }

        /// <summary>
        /// Vô hiệu hóa Hotel. 
        /// </summary>
        /// <param name="HotelId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Deactivate/{HotelId}")]
        [ApiCustomFilter]
        public IHttpActionResult Deactivate(int HotelId = 0)
        {
            ApiResult<bool> Rs = hotelDAL.Deactivate(HotelId, UserInfo.Id);
            return Rs.Succeeded ? Ok(Rs) : (IHttpActionResult)Content(HttpStatusCode.BadRequest, Rs);
        }



        #region PrivateArea
        private string UploadHotelImage(string Base64 = "", int Id = 0, string FileName = "")
        {
            string rs = "";
            UploadFileResult UploadResult = null;
            if (Base64.StartsWith("data:image") && Base64.Contains(";base64,"))
            {
                string Url = string.Format(ConfigUtil.GetConfigurationValueFromKey("HotelImagesDerectory", false), Id);
                try
                {
                    UploadResult = CommonUtil.UploadBase64File(
                                Base64,
                                FileName,
                                Url,
                                ImageFormat.Png,
                                20
                    );
                }
                catch (Exception)
                {
                    return "";
                }
                if (UploadResult == null || UploadResult.HasError)
                {
                    return "";
                }
                else
                {
                    return UploadResult.FilePath;
                }
            }

            return rs;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
    }
}