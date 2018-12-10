using System;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using Common.Base;


namespace Project2.DAL
{
    public class CommentDAL : BaseDAL
    {
        public CommentDAL(SqlDbProvider dbProvider) : base(dbProvider)
        {

        }

        public ApiResult<CommentModels> GetPaging(BaseCondition<CommentModels> condition)
        {
            var result = new ApiResult<CommentModels>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_Comment_GetPaging", CommandType.StoredProcedure);

                // Input params                    
                DbProvider.AddParameter("StartRow", condition.FromRecord, SqlDbType.Int);
                DbProvider.AddParameter("PageSize", condition.PageSize, SqlDbType.Int);

                // Sắp xếp dữ liệu
                if (!string.IsNullOrEmpty(condition.IN_SORT))
                {
                    DbProvider.AddParameter("IN_SORT", condition.IN_SORT, SqlDbType.NVarChar);
                }

                // Điều kiện tìm kiếm
                if (condition.HasCondition && !string.IsNullOrEmpty(condition.IN_WHERE))
                {
                    DbProvider.AddParameter("IN_WHERE", condition.IN_WHERE, SqlDbType.NVarChar);
                }

                // Output params
                DbProvider.AddParameter("TotalRecords", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);


                result.DataList = DbProvider.ExecuteListObject<CommentModels>();
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

        public ApiResult<bool> Insert(CommentModels Item)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_Comment_Insert", CommandType.StoredProcedure);

                DbProvider.AddParameter("UserId", Item.UserId, SqlDbType.Int);
                DbProvider.AddParameter("HotelId", Item.HotelId, SqlDbType.Int);
                DbProvider.AddParameter("ParentId", Item.ParentId, SqlDbType.Int);
                DbProvider.AddParameter("Content", Item.Content, SqlDbType.NVarChar);


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
                Rs.Failed(new ErrorObject
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.ToString()
                });
            }
            return Rs;
        }

        public ApiResult<bool> Delete(RemoveComment Item)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_Comment_Remove", CommandType.StoredProcedure);

                DbProvider.AddParameter("UserId", Item.UserId, SqlDbType.Int);
                DbProvider.AddParameter("Id", Item.Id, SqlDbType.Int);


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
                Rs.Failed(new ErrorObject
                {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.ToString()
                });
            }
            return Rs;
        }

        public ApiResult<HomeSideComment> GetTop3()
        {
            DbProvider.SetCommandText("sp_Comment_Top3", CommandType.StoredProcedure);
            ApiResult<HomeSideComment> Rs = new ApiResult<HomeSideComment>()
            {
                DataList = DbProvider.ExecuteListObject<HomeSideComment>()
            };
            return Rs;
        }
    }
}
