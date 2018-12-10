using System;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using Common.Base;

namespace Project2.DAL
{
    public class UserDAL : BaseDAL
    {
        public UserDAL(SqlDbProvider dbProvider) : base(dbProvider)
        {

        }

        public ApiResult<User> Register(UserPostData item)
        {
            var Rs = new ApiResult<User>() {
                Data = new User()
            };
            try
            {
                DbProvider.SetCommandText2("Sp_User_Register_Serium", CommandType.StoredProcedure);
                DbProvider.AddParameter("Email", item.Email, SqlDbType.NVarChar);
                DbProvider.AddParameter("Password", item.Password, SqlDbType.NVarChar);
                DbProvider.AddParameter("FirstName", item.FirstName, SqlDbType.NVarChar);
                DbProvider.AddParameter("LastName", item.LastName, SqlDbType.NVarChar);
                DbProvider.AddParameter("RoleId", item.RoleId, SqlDbType.Int);
                DbProvider.AddParameter("CreatedUser", item.CreatedUser, SqlDbType.Int);

                DbProvider.AddParameter("PhoneNumber", item.PhoneNumber, SqlDbType.NVarChar);
                DbProvider.AddParameter("Address", item.Address, SqlDbType.NVarChar);
                DbProvider.AddParameter("ProvinceCode", item.ProvinceCode, SqlDbType.Int);
                DbProvider.AddParameter("DistrictCode", item.DistrictCode, SqlDbType.Int);

                DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 255, ParameterDirection.Output);
                DbProvider.AddParameter("UserId", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrCode"].Value.ToString();
                try {
                    Rs.Data.Id = Int32.Parse(DbProvider.Command.Parameters["UserId"].Value.ToString());
                }
                catch (Exception)
                {
                    Rs.Data.Id = 0;
                }
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }
            return Rs;
        }

        public ApiResult<UserInfo> Login(UserPostLogin item)
        {
            var result = new ApiResult<UserInfo>();

            try
            {
                DbProvider.SetCommandText2("sp_User_Login", CommandType.StoredProcedure);

                // Input parameter
                DbProvider.AddParameter("Email", item.Email, SqlDbType.NVarChar);

                // Output parameter
                DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 1000, ParameterDirection.Output);

                // Xử lý thủ tục và kết quả trả về từ DB
                result.Data = DbProvider.ExecuteObject<UserInfo>();
                string errorCode = DbProvider.Command.Parameters["ErrCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                result.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return result;
        }

        public String GetOtp(int Id)
        {
            try
            {
                DbProvider.SetCommandText2("sp_User_GenOTPCode", CommandType.StoredProcedure);

                // Input parameter
                DbProvider.AddParameter("UserId", Id, SqlDbType.NVarChar);

                // Output parameter
                DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 1000, ParameterDirection.Output);
                DbProvider.AddParameter("OTPCode", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                // Xử lý thủ tục và kết quả trả về từ DB
                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrCode"].Value.ToString();

                if (errorCode != "" && !errorCode.Equals(Constants.SUCCESS))
                {
                    return "";
                }
                return DbProvider.Command.Parameters["OTPCode"].Value.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        public ApiResult<User> ConfirmEmail(UserPostOTP item)
        {
            var result = new ApiResult<User>();

            try
            {
                DbProvider.SetCommandText2("sp_User_ConfirmEmail", CommandType.StoredProcedure);

                // Input parameter
                DbProvider.AddParameter("UserId", item.Id, SqlDbType.Int);
                DbProvider.AddParameter("OTP", item.OTP, SqlDbType.NVarChar);

                // Output parameter
                DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.NVarChar,1000, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                // Xử lý thủ tục và kết quả trả về từ DB
                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                result.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return result;
        }

        public ApiResult<User> GetById(int Id) {
            var result = new ApiResult<User>();

            DbProvider.SetCommandText2("sp_User_GetById", CommandType.StoredProcedure);

            // Input parameter
            DbProvider.AddParameter("UserId", Id, SqlDbType.Int);

            // Output parameter
            DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
            DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 1000, ParameterDirection.Output);

            // Xử lý thủ tục và kết quả trả về từ DB
            result.Data = DbProvider.ExecuteObject<User>();
            return result;
        }


        public ApiResult<ForgetPasswordViewModel> ForgetPassword(
            ForgetPasswordViewModel model,
            string tokenCode,
            DateTime tokenExp)
        {
            try
            {
                var result = new ApiResult<ForgetPasswordViewModel>();
                DbProvider.SetCommandText2("sp_User_ForgetPassword", CommandType.StoredProcedure);

                // Input Parameter
                DbProvider.AddParameter("Email", model.Email, SqlDbType.NVarChar);
                DbProvider.AddParameter("TokenCode", tokenCode, SqlDbType.NVarChar);
                DbProvider.AddParameter("TokenExp", tokenExp, SqlDbType.DateTime);

                // Output Parameter
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 400, ParameterDirection.Output);

                // Xứ lý thủ tục và trả về kết quả
                result.Data = DbProvider.ExecuteObject<ForgetPasswordViewModel>();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = errorCode,
                        Description = DbProvider.Command.Parameters["ErrorMessage"].Value.ToString()
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ApiResult<UpdatePasswordViewModel> UpdatePassword(UpdatePasswordViewModel model)
        {
            var result = new ApiResult<UpdatePasswordViewModel>();

            try
            {
                DbProvider.SetCommandText2("sp_User_UpdatePassword", CommandType.StoredProcedure);

                // Input Parameter
                DbProvider.AddParameter("TokenCode", model.TokenCode, SqlDbType.NVarChar);
                DbProvider.AddParameter("NewPassword", model.NewPassword, SqlDbType.NVarChar);

                // Output Parameter
                DbProvider.AddParameter("Email", DBNull.Value, SqlDbType.NVarChar, 255, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMessage", DBNull.Value, SqlDbType.NVarChar, 255, ParameterDirection.Output);

                // Xử lý kết quả trả về
                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = errorCode,
                        Description = DbProvider.Command.Parameters["ReturnMessage"].Value.ToString()
                    });
                }
                else
                {
                    result.Data = new UpdatePasswordViewModel()
                    {
                        Email = DbProvider.Command.Parameters["Email"].Value.ToString()
                    };
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ApiResult<bool> ChangePassword(
            int Id,
            ChangePWModel model)
        {
            var result = new ApiResult<bool>();

            try
            {
                DbProvider.SetCommandText2("sp_User_ChangePassword", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("Id", Id, SqlDbType.Int);
                DbProvider.AddParameter("OldPassword", model.OldPassword, SqlDbType.NVarChar);
                DbProvider.AddParameter("NewPassword", model.NewPassword, SqlDbType.NVarChar);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMessage", DBNull.Value, SqlDbType.NVarChar, 255, ParameterDirection.Output);

                // Xử lý thủ tục và trả về kết quả
                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = errorCode,
                        Description = DbProvider.Command.Parameters["ReturnMessage"].Value.ToString()
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });

                return result;
            }

        }


        public ApiResult<UserAvatarViewModel> ChangeAvatar(int applicantId, UserAvatarViewModel model)
        {
            DbProvider.SetCommandText2("sp_User_UploadAvatar", CommandType.StoredProcedure);

            // Input value
            DbProvider.AddParameter("UserId", applicantId, SqlDbType.Int);
            DbProvider.AddParameter("Image", model.ImagePath, SqlDbType.NVarChar);

            // Output value
            DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
            DbProvider.AddParameter("ReturnMessage", DBNull.Value, SqlDbType.NVarChar, 255, ParameterDirection.Output);

            // Execute and return result
            DbProvider.ExecuteNonQuery();
            var result = new ApiResult<UserAvatarViewModel>();
            string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
            if (!errorCode.Equals(Constants.SUCCESS))
            {
                result.Failed(new ErrorObject()
                {
                    Code = errorCode,
                    Description = DbProvider.Command.Parameters["ReturnMessage"].Value.ToString()
                });
            }

            return result;
        }

        public ApiResult<Boolean> UpdateProfile(User Item)
        {
            ApiResult<Boolean> Rs = new ApiResult<Boolean>();
            try
            {
                DbProvider.SetCommandText("sp_User_UpdateProfile", CommandType.StoredProcedure);
                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();

                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Rs.Failed(new ErrorObject {
                    Code = "EX",
                    Description = ex.ToString()
                });
            }

            return Rs;
        }

        public ApiResult<Boolean> SetRole(SetRole Item, int userId)
        {
            ApiResult<Boolean> Rs = new ApiResult<Boolean>();
            try
            {
                DbProvider.SetCommandText("sp_User_SetRole", CommandType.StoredProcedure);
                DbProvider.AddParameter("AdminId", userId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", Item.UserId, SqlDbType.Int);
                DbProvider.AddParameter("RoleId", Item.RoleId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();

                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "EX",
                    Description = ex.ToString()
                });
            }

            return Rs;
        }

        public ApiResult<GetUserRole> GetPaging(BaseCondition<GetUserRole> condition)
        {
            var result = new ApiResult<GetUserRole>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_User_GetPaging", CommandType.StoredProcedure);

                // Input params                    
                DbProvider.AddParameter("PageIndex", condition.FromRecord, SqlDbType.Int);
                DbProvider.AddParameter("PageSize", condition.PageSize, SqlDbType.Int);

                // Sắp xếp dữ liệu
                if (!string.IsNullOrEmpty(condition.IN_SORT))
                {
                    DbProvider.AddParameter("InSort", condition.IN_SORT, SqlDbType.NVarChar);
                }

                // Điều kiện tìm kiếm
                if (condition.HasCondition && !string.IsNullOrEmpty(condition.IN_WHERE))
                {
                    DbProvider.AddParameter("InWhere", condition.IN_WHERE, SqlDbType.NVarChar);
                }

                // Output params
                DbProvider.AddParameter("TotalCount", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);


                result.DataList = DbProvider.ExecuteListObject<GetUserRole>();
                try
                {
                    result.TotalRecords = int.Parse(DbProvider.Command.Parameters["TotalCount"].Value.ToString());
                }
                catch (Exception)
                {
                    result.TotalRecords = 0;
                }

                // Kiểm tra kết quả trả về
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    result.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                result.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }
            return result;
        }

    }
}