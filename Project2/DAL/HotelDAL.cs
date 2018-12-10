using System;
using System.Collections.Generic;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using Common.Base;

namespace Project2.DAL
{
    public class HotelDAL : BaseDAL
    {
        public HotelDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<HotelPaging> GetPaging(BaseCondition<HotelPaging> model, int UserId)
        {
            ApiResult<HotelPaging> Rs = new ApiResult<HotelPaging>()
            {
                DataList = new List<HotelPaging>()
            };

            try
            {
                DbProvider.SetCommandText("sp_Hotel_getPaging", CommandType.StoredProcedure);
                // Input params                    
                DbProvider.AddParameter("StartRow", model.FromRecord, SqlDbType.Int);
                DbProvider.AddParameter("PageSize", model.PageSize, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                // Sắp xếp dữ liệu
                if (!string.IsNullOrEmpty(model.IN_SORT))
                {
                    DbProvider.AddParameter("InSort", model.IN_SORT, SqlDbType.NVarChar);
                }

                // Điều kiện tìm kiếm
                if (model.HasCondition && !string.IsNullOrEmpty(model.IN_WHERE))
                {
                    DbProvider.AddParameter("InWhere", model.IN_WHERE, SqlDbType.NVarChar);
                }

                // Output params
                DbProvider.AddParameter("TotalRecords", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                Rs.DataList = DbProvider.ExecuteListObject<HotelPaging>();
                try
                {
                    Rs.TotalRecords = int.Parse(DbProvider.Command.Parameters["TotalRecords"].Value.ToString());
                }
                catch (Exception)
                {
                    Rs.TotalRecords = 0;
                }

                // Kiểm tra kết quả trả về
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ErrorMessage"].Value.ToString()
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

        public ApiResult<HotelModels> Register(PostHotel Item, int UserId)
        {
            ApiResult<HotelModels> Rs = new ApiResult<HotelModels>()
            {
                Data = new HotelModels()
            };
            try
            {
                DbProvider.SetCommandText("sp_Hotel_register", CommandType.StoredProcedure);
                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);
                DbProvider.AddParameter("HotelId", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                try
                {
                    Rs.Data.Id = int.Parse(DbProvider.Command.Parameters["HotelId"].Value.ToString());
                }
                catch (Exception)
                {
                    Rs.Data.Id = 0;
                }

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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }
            return Rs;
        }

        public ApiResult<HotelModels> ConfirmHotel(int HotelId, int UserId)
        {
            ApiResult<HotelModels> Rs = new ApiResult<HotelModels>();

            try
            {
                DbProvider.SetCommandText("sp_Hotel_ConfirmHotel", CommandType.StoredProcedure);
                DbProvider.AddParameter("HotelId", HotelId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);
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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return Rs;
        }

        public ApiResult<HotelResult> GetItemByClient(int HotelId)
        {
            ApiResult<HotelResult> Rs = new ApiResult<HotelResult>();
            try
            {
                DbProvider.SetCommandText("sp_Hotel_GetItemWithListRoom", CommandType.StoredProcedure);
                DbProvider.AddParameter("HotelId", HotelId, SqlDbType.Int);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteReader_ToMyReader();

                Rs.Data = DbProvider.GetObjectFromMyReader<HotelResult>();
                DbProvider.ExecuteReader_NextResult();
                Rs.Data.Tags = DbProvider.ExecuteReader_frmMyReader<Tag>();

                DbProvider.ExecuteReader_NextResult();
                Rs.Data.Album = DbProvider.ExecuteReader_frmMyReader<FileUploadCompact>();

                DbProvider.ExecuteReader_Close();

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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return Rs;
        }

        public ApiResult<HotelResult> AdminGetItem(int HotelId, int UserId, int UserRole)
        {
            ApiResult<HotelResult> Rs = new ApiResult<HotelResult>() { };

            try
            {
                DbProvider.SetCommandText("sp_BE_Hotel_GetItem", CommandType.StoredProcedure);
                // Input params                    
                DbProvider.AddParameter("HotelId", HotelId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteReader_ToMyReader();

                if (UserRole == 2)
                {
                    Rs.Data = DbProvider.GetObjectFromMyReader<HotelResult>();
                    if (Rs.Data == null)
                    {
                        DbProvider.ExecuteReader_Close();
                    }
                    else
                    {

                        DbProvider.ExecuteReader_NextResult();
                        Rs.Data.Tags = DbProvider.ExecuteReader_frmMyReader<Tag>();
                        DbProvider.ExecuteReader_NextResult();
                        Rs.Data.Album = DbProvider.ExecuteReader_frmMyReader<FileUploadCompact>();
                    }

                }
                else
                {
                    Rs.Data = DbProvider.GetObjectFromMyReader<HotelResult>();
                    if (!Rs.Succeeded || Rs.Data == null)
                    {
                        DbProvider.ExecuteReader_Close();

                    }
                    else
                    {

                        DbProvider.ExecuteReader_NextResult();
                        Rs.Data.AnhGiayTo = DbProvider.ExecuteReader_frmMyReader<FileUploadCompact>();
                    }
                }

                DbProvider.ExecuteReader_Close();

                // Kiểm tra kết quả trả về
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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }
            return Rs;
        }

        public ApiResult<HotelModels> OwnerUpdate(PutHotel Item, int UserId)
        {
            ApiResult<HotelModels> Rs = new ApiResult<HotelModels>();
            try
            {
                DbProvider.SetCommandText("sp_Hotel_Owner_Update", CommandType.StoredProcedure);
                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                // Kiểm tra kết quả trả về
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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return Rs;
        }

        public ApiResult<bool> Activate(int HotelId, int UserId)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_hotel_activate", CommandType.StoredProcedure);
                DbProvider.AddParameter("HotelId", HotelId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                // Kiểm tra kết quả trả về
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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return Rs;
        }

        public ApiResult<bool> Deactivate(int HotelId, int UserId)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_hotel_deactivate", CommandType.StoredProcedure);
                DbProvider.AddParameter("HotelId", HotelId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                // Kiểm tra kết quả trả về
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
                Rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return Rs;
        }
    }
}