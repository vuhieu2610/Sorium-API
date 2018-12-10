using System;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using Common.Base;

namespace Project2.DAL
{
    public class RoomDAL : BaseDAL
    {
        public RoomDAL(SqlDbProvider dbProvider) :base(dbProvider) { }

        public ApiResult<RoomResult> GetPaging(BaseCondition<RoomResult> model )
        {
            var result = new ApiResult<RoomResult>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_Room_GetPaging", CommandType.StoredProcedure);

                // Input params                    
                DbProvider.AddParameter("StartRow", model.FromRecord, SqlDbType.Int);
                DbProvider.AddParameter("PageSize", model.PageSize, SqlDbType.Int);

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


                result.DataList = DbProvider.ExecuteListObject<RoomResult>();
                try
                {
                    result.TotalRecords = int.Parse(DbProvider.Command.Parameters["TotalRecords"].Value.ToString());
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
                        Description = DbProvider.Command.Parameters["ErrorMessage"].Value.ToString()
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

        public ApiResult<RoomDetail> GetDetail(int Id)
        {
            var result = new ApiResult<RoomDetail>() {
                Data = new RoomDetail()
            };
            try
            {
                DbProvider.SetCommandText2("sp_room_GetItem", CommandType.StoredProcedure);

                DbProvider.AddParameter("Id", Id, SqlDbType.Int);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 200, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteReader_ToMyReader();

                result.Data = DbProvider.GetObjectFromMyReader<RoomDetail>();
                if (result.Data != null )
                {
                    DbProvider.ExecuteReader_NextResult();
                    result.Data.ListTag = DbProvider.ExecuteReader_frmMyReader<Tag>();
                }

                DbProvider.ExecuteReader_Close();
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
            catch(Exception ex)
            {
                result.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }
            return result;
        }

        public ApiResult<RoomResult> Insert(RoomDetail Item, int UserId = 0)
        {
            ApiResult<RoomResult> rs = new ApiResult<RoomResult>() {
                Data = new RoomResult()
            };
            try
            {
                DbProvider.SetCommandText2("sp_room_Insert", CommandType.StoredProcedure);
                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.AddParameter("RoomId", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                try
                {
                    rs.Data.Id = Int32.Parse(DbProvider.Command.Parameters["RoomId"].Value.ToString());
                }
                catch (Exception)
                {
                    rs.Data.Id = 0;
                }
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            } catch(Exception ex)
            {
                rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return rs;
        }

        public ApiResult<RoomResult> Update(RoomDetail Item, int UserId = 0)
        {
            ApiResult<RoomResult> rs = new ApiResult<RoomResult>()
            {
                Data = new RoomResult()
            };
            try
            {
                DbProvider.SetCommandText2("sp_room_update", CommandType.StoredProcedure);

                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                
                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return rs;
        }

        public ApiResult<RoomResult> Delete(int RoomId, int UserId = 0)
        {
            ApiResult<RoomResult> rs = new ApiResult<RoomResult>();
            try
            {
                DbProvider.SetCommandText2("sp_Room_delete", CommandType.StoredProcedure);

                DbProvider.AddParameter("RoomId", RoomId, SqlDbType.Int);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();

                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();

                if (!errorCode.Equals(Constants.SUCCESS))
                {
                    rs.Failed(new ErrorObject()
                    {
                        Code = DbProvider.Command.Parameters["ErrorCode"].Value.ToString(),
                        Description = DbProvider.Command.Parameters["ReturnMsg"].Value.ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                rs.Failed(new ErrorObject()
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.Message
                });
            }

            return rs;
        }

    }
}