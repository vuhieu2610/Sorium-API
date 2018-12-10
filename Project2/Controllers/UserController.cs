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
    [RoutePrefix("api/Acc")]
    public class UserController : BaseController
    {
        private UserDAL userDAL;

        public UserController()
        {
            userDAL = new UserDAL(DbProvider);
        }

        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register(UserPostData item)
        {
            var Rs = new ApiResult<User>();
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

            if (item.RoleId == 0) item.RoleId = 1;
            UserResult userInfo;
            try
            {
                userInfo = UserInfo;
            }
            catch (Exception)
            {
                userInfo = new UserResult();
            }

            if (userInfo.Id > 0)
            {
                item.CreatedUser = userInfo.Id;
            }
            item.Password = Libs.GetMd5(item.Password + EncryptCore.PassKey);
            Rs = userDAL.Register(item);
            if (Rs.Succeeded)
            {
                string OTP = userDAL.GetOtp(Rs.Data.Id);
                string Url = ConfigUtil.DomainBaseHttp + "/Api/Acc/ConfirmEmail?Id=" + Rs.Data.Id.ToString() + "&OTP=" + OTP;
                if (!String.IsNullOrEmpty(item.SucRedirectUrl))
                {
                    Url += "&SucRedirectUrl=" + item.SucRedirectUrl;
                }

                if (!String.IsNullOrEmpty(item.FailRedirectUrl))
                {
                    Url += "&FailRedirectUrl=" + item.FailRedirectUrl;
                }
                object DataContent = new
                {
                    Link = Url,
                    item.FirstName
                };
                var EmailContent = EmailContentHtml.EmailContentFormat(DataContent, "ConfirmEmail.html");
                var e = EmailUtility.SendMail(ConfigUtil.Email_DisplayName, item.Email, "Xác thực Email", EmailContent);
                return Ok(Rs);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, Rs);
            }
        }

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(UserPostLogin item)
        {
            var Rs = new ApiResult<UserResult>();
            try
            {
                if (string.IsNullOrEmpty(item.Email))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = "1",
                        Description = "Chưa nhập Email"
                    });
                }
                if (string.IsNullOrEmpty(item.Password))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = "2",
                        Description = "Chưa nhập Password"
                    });
                }

                if (Rs.Errors.Count > 0)
                {
                    return Content(HttpStatusCode.Unauthorized, Rs);
                }
                var Login = userDAL.Login(item);
                if (Login.Succeeded && Login.Data != null)
                {
                    if (Libs.GetMd5(item.Password + EncryptCore.PassKey) != Login.Data.Password)
                    {
                        Rs.Failed(new ErrorObject
                        {
                            Code = "400",
                            Description = "Sai Mật Khẩu"
                        });
                        return Content(HttpStatusCode.Unauthorized, Rs);
                    }


                    Random rnd = new Random();
                    int RndNumber = rnd.Next(1, 9999);
                    var TokenCode = "";
                    var User = Login.Data;
                    if (User.EmailConfirmed > 0)
                    {
                        TokenCode = EncryptCore.Md5Get(item.Email + User.Id + EncryptCore.PassKey + DateTime.Now.ToString("ddMMyyyyhhmmss") + RndNumber);
                    }
                    else
                    {
                        Rs.Failed(new ErrorObject
                        {
                            Code = "EMAIL_NOT_CONFIRM",
                            Description = "Email is not comfirmed"
                        });
                    }
                    var UserInf = new UserResult()
                    {
                        Id = User.Id,
                        Email = User.Email,
                        AccessToken = TokenCode,
                        FirstName = User.FirstName,
                        LastName = User.LastName,
                        PhoneNumber = User.PhoneNumber,
                        Address = User.Address,
                        DistrictCode = User.DistrictCode,
                        ProvinceCode = User.ProvinceCode,
                        RoleDesc = User.RoleDesc,
                        RoleId = User.RoleId,
                        UserAvatar = User.UserAvatar,
                        EmailConfirmed = User.EmailConfirmed

                    };
                    CacheUtil.InsertCacheObject(TokenCode, UserInf, 60 * 24 * 30);
                    Rs.Data = UserInf;
                    return Content(HttpStatusCode.OK, Rs);
                }
                else
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = Login.Errors[0].Code,
                        Description = Login.Errors[0].Description
                    });
                    return Content(HttpStatusCode.BadRequest, Rs);
                }
            }
            catch (Exception ex)
            {
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.ToString()
                });
                return Content(HttpStatusCode.Unauthorized, Rs);
            }
        }

        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout(string TokenCode = "")
        {
            HttpRuntime.Cache.Remove(TokenCode);
            return Ok(new { message = "SUC" });
        }

        [HttpGet]
        [Route("ConfirmEmail")]
        public IHttpActionResult ConfirmOTP(int Id = 0, string OTP = "", string SucRedirectUrl = "", string FailRedirectUrl = "")
        {
            var Rs = new ApiResult<User>();

            if (Id == 0) Rs.Failed(new ErrorObject { Code = "UserId", Description = "UserId is required" });

            if (OTP.Trim() == "") Rs.Failed(new ErrorObject { Code = "OTPCOde", Description = "OTPCOde is Reuqired" });

            if (Rs.Errors.Count > 0) return Content(HttpStatusCode.BadRequest, Rs);

            var item = new UserPostOTP() { Id = Id, OTP = OTP };
            var e = userDAL.ConfirmEmail(item);
            if (!e.Succeeded)
            {
                return Redirect(FailRedirectUrl);
            }
            else
            {
                return Redirect(SucRedirectUrl);
            }
        }

        [HttpPost]
        [Route("ResendOTP")]
        public IHttpActionResult ResendOTP(UserResendEmail user)
        {
            var Rs = new ApiResult<User>();
            if (user.Id == 0)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "User Id",
                    Description = "User Id is required"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }

            if (!ModelState.IsValid)
            {
                IEnumerable<string> message = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage);
                foreach (var i in message)
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = "Validate_Exception",
                        Description = i.ToString()
                    });
                }
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            var UserInf = userDAL.GetById(user.Id);
            if (UserInf.Data != null && UserInf.Data.Id > 0)
            {
                var item = UserInf.Data;
                if (item.EmailConfirmed > 0)
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = "ERR",
                        Description = "Email is confirmed"
                    });
                    return Ok(Rs);
                }
                string OTP = userDAL.GetOtp(item.Id);
                string Url = ConfigUtil.DomainBaseHttp + "/Api/Acc/ConfirmEmail?Id=" + item.Id.ToString() + "&OTP=" + OTP;

                if (!String.IsNullOrEmpty(user.SucRedirectUrl))
                {
                    Url += "&SucRedirectUrl=" + user.SucRedirectUrl;
                }

                if (!String.IsNullOrEmpty(user.FailRedirectUrl))
                {
                    Url += "&FailRedirectUrl=" + user.FailRedirectUrl;
                }
                object DataContent = new
                {
                    Link = Url,
                    item.FirstName
                };
                var EmailContent = EmailContentHtml.EmailContentFormat(DataContent, "ConfirmEmail.html");
                if (EmailContent == null || EmailContent == "")
                {
                    return BadRequest();
                }
                var e = EmailUtility.SendMail(ConfigUtil.Email_DisplayName, item.Email, "Xác thực Email", EmailContent);
            }
            else
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "NOT_FOUND",
                    Description = "User not found"
                });
                return Content(HttpStatusCode.NotFound, Rs);
            }
            return Ok(Rs);

        }

        [HttpGet]
        [Route("Get/{TokenCode}")]
        public IHttpActionResult GetUserByTokenCode(string TokenCode = "")
        {
            var Rs = new ApiResult<UserResult>();
            UserResult userInfo = CacheUtil.GetCacheObject(TokenCode);
            if (userInfo != null)
            {
                var Rq = userDAL.GetById(userInfo.Id).Data;

                Rs.Data = new UserResult
                {
                    Id = Rq.Id,
                    Email = Rq.Email,
                    AccessToken = TokenCode,
                    Address = Rq.Address,
                    DistrictCode = Rq.DistrictCode,
                    EmailConfirmed = Rq.EmailConfirmed,
                    FirstName = Rq.FirstName,
                    LastName = Rq.LastName,
                    PhoneNumber = Rq.PhoneNumber,
                    ProvinceCode = Rq.ProvinceCode,
                    RoleDesc = Rq.RoleDesc,
                    RoleId = Rq.RoleId,
                    UserAvatar = Rq.UserAvatar
                };

                return Ok(Rs);
            }
            else
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "400",
                    Description = "Sai TokenCode"
                });
                return Content(HttpStatusCode.Unauthorized, Rs);
            }
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public ApiResult<ForgetPasswordViewModel> ForgetPassword(ForgetPasswordViewModel model)
        {
            var dbResult = new ApiResult<ForgetPasswordViewModel>();
            string errorMessage = string.Empty;
            try
            {
                // Kiểm tra dữ liệu
                var ValidPassword = ValidateForgetPassword(model);
                if (!ValidPassword.Succeeded)
                {
                    return ValidPassword;
                }

                // Tạo token code và thời gian hết hạn
                string tokenCode = EncryptCore.Md5Get(DateTime.Now.Ticks + model.Email + EncryptCore.KeyEncrypt); ;
                DateTime tokenExp = DateTime.Now.AddDays(1);

                // Lấy kết quả trả về từ DB
                dbResult = userDAL.ForgetPassword(model, tokenCode, tokenExp);

                // Kiểm tra kết quả
                if (dbResult.Succeeded)
                {
                    // Tạo nội dung của email
                    var otpEmailFormat =
                        new
                        {
                            HoTen = dbResult.Data.FirstName,
                            Link = model.UrlForm + "?Token=" + tokenCode
                        };
                    var msgEmailOtp = EmailContentHtml.EmailContentFormat(otpEmailFormat, "ResetPassword.html");

                    // Gửi email cho user
                    var emailSendStt = EmailUtility.SendMail(ConfigUtil.Email_DisplayName, model.Email, "Quên mật khẩu", msgEmailOtp);
                }

                return dbResult;
            }
            catch (Exception ex)
            {
                dbResult.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
                return dbResult;
            }
        }


        private ApiResult<ForgetPasswordViewModel> ValidateForgetPassword(ForgetPasswordViewModel model)
        {
            var result = new ApiResult<ForgetPasswordViewModel>();
            // Validate dữ liệu input
            if (string.IsNullOrEmpty(model.Email))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "EMAIL_EMPTY",
                    Description = "Email không được để trống"
                });

                return result;
            }

            if (!ValidateUtil.ValidateEmail(model.Email))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "EMAIL_INCORRECT_FORMAT",
                    Description = "Email không đúng định dạng"
                });
            }

            if (string.IsNullOrEmpty(model.UrlForm))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "UrlForm_EMPTY",
                    Description = "UrlForm không được để trống"
                });
            }

            return result;
        }

        private ApiResult<UpdatePasswordViewModel> ValidateUpdatePassword(UpdatePasswordViewModel model)
        {
            var result = new ApiResult<UpdatePasswordViewModel>();

            // Kiểm tra token code
            if (string.IsNullOrEmpty(model.TokenCode))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "TOKEN_EMPTY",
                    Description = "Không có token hợp lệ"
                });

                return result;
            }

            // Kiểm tra mật khẩu hợp lệ
            if (string.IsNullOrEmpty(model.NewPassword) ||
                string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "PASSWORD_EMPTY",
                    Description = "Mật khẩu mới không được để trống"
                });
                return result;
            }

            // Kiểm tra mật khẩu trùng nhau
            if (!model.NewPassword.Equals(model.ConfirmNewPassword))
            {
                result.Failed(new ErrorObject()
                {
                    Code = "CONFIRM_PASSWORD_INCORRECT",
                    Description = "Nhập lại mật khẩu trùng với mật khẩu mới"
                });
                return result;
            }

            return result;
        }

        [HttpPost]
        [Route("UpdatePassword")]
        public ApiResult<UpdatePasswordViewModel> UpdatePassword(UpdatePasswordViewModel model)
        {
            var dbResult = new ApiResult<UpdatePasswordViewModel>();

            try
            {
                // Validate dữ liệu
                if (!ValidateUpdatePassword(model).Succeeded)
                {
                    return ValidateUpdatePassword(model);
                }

                // Đặt lại mật khẩu trong database
                model.NewPassword = Libs.GetMd5(model.NewPassword + EncryptCore.PassKey);
                model.NewPassword = model.NewPassword;

                dbResult = userDAL.UpdatePassword(model);

                return dbResult;
            }
            catch (Exception ex)
            {
                dbResult.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
                return dbResult;
            }
        }

        [HttpPost]
        [Route("UploadAvatar")]
        [ApiCustomFilter]
        public IHttpActionResult UploadAvatar(string UserAvatar = "")
        {
            var Rs = new ApiResult<UserAvatarViewModel>()
            {
                Data = new UserAvatarViewModel()
            };

            var httpRequest = HttpContext.Current.Request;
            var file = httpRequest.Files["UserAvatar"];
            if (file == null)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "FILE_IS_REQUIRED",
                    Description = "File upload not null"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            string ImageRegex = ".(image)*"; // Match tất cả các loại file chứa ký tự image

            if (!Regex.IsMatch(file.ContentType, ImageRegex))
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "FILE_TYPE",
                    Description = "You must upload image file"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            //==========================//
            String Dic = String.Format(ConfigUtil.UserAvatarDirectory, UserInfo.Id);
            var objUpload = CommonUtil.UploadFile(file, Dic, file.FileName);

            if (objUpload.HasError)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = objUpload.ErrorCode,
                    Description = objUpload.ErrorMessage
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }

            UserAvatarViewModel Item = new UserAvatarViewModel
            {
                ImagePath = objUpload.FilePath
            };

            var Req = userDAL.ChangeAvatar(UserInfo.Id, Item);

            if (Req.Succeeded)
            {
                Rs.Data.ImagePath = ConfigUtil.DomainBaseHttp + Item.ImagePath;
                return Ok(Rs);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, Rs);
            }


        }

        [HttpPost]
        [Route("UpdateInfo")]
        [ApiCustomFilter]
        public IHttpActionResult UpdateInfo(User Data)
        {
            ApiResult<Boolean> rs = new ApiResult<bool>();
            if (Data == null)
            {
                rs.Failed(new ErrorObject {
                    Code = "Error",
                    Description = "Đéo nhận Data truyền vào  😒. "
                });
                return Content(HttpStatusCode.BadRequest, rs);
            }
            if (UserInfo.Id != Data.Id)
            {
                rs.Failed(new ErrorObject
                {
                    Code = "Error",
                    Description = "UserId đéo khớp  😒. "
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

            rs = userDAL.UpdateProfile(Data);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);
        }

        [HttpPost]
        [Route("SetRole")]
        [ApiCustomFilter]
        public IHttpActionResult SetRole(SetRole Data)
        {
            ApiResult<Boolean> rs = new ApiResult<Boolean>();
            if (Data == null)
            {
                rs.Failed(new ErrorObject
                {
                    Code = "Error",
                    Description = "Đéo nhận Data truyền vào  😒. "
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
            rs = userDAL.SetRole(Data, UserInfo.Id);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);

        }


        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<GetUserRole> condition)
        {
            ApiResult<GetUserRole> rs = userDAL.GetPaging(condition);
            return !rs.Succeeded ? Content(HttpStatusCode.BadRequest, rs) : (IHttpActionResult)Ok(rs);
        }

        [HttpPost]
        [Route("ChangePassword")]
        [ApiCustomFilter]
        public IHttpActionResult ChangePassword(ChangePWModel Item)
        {
            Item.NewPassword = Libs.GetMd5(Item.NewPassword + EncryptCore.PassKey);
            Item.OldPassword = Libs.GetMd5(Item.OldPassword + EncryptCore.PassKey);

            ApiResult<bool> rs = userDAL.ChangePassword(UserInfo.Id, Item);
            return rs.Succeeded ? Ok(rs) : (IHttpActionResult)Content(HttpStatusCode.BadRequest, rs);
        }
    }
}