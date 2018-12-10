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
    public class OrderDAL : BaseDAL
    {
        public OrderDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<NewOrderResult> Insert(Order Item, int UserId)
        {
            ApiResult<NewOrderResult> Rs = new ApiResult<NewOrderResult>()
            {
                Data = new NewOrderResult()
            };
            try
            {
                DbProvider.SetCommandText("sp_order_insert", CommandType.StoredProcedure);

                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);
                DbProvider.AddParameter("OrderId", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                try
                {
                    Rs.Data.Id = int.Parse(DbProvider.Command.Parameters["OrderId"].Value.ToString());
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


        public ApiResult<bool> ConfirmEmail(int Id, string Token)
        {
            ApiResult<Boolean> Rs = new ApiResult<Boolean>();
            try
            {
                DbProvider.SetCommandText("sp_Order_Confirm", CommandType.StoredProcedure);
                DbProvider.AddParameter("TokenCode", Token, SqlDbType.NVarChar);
                DbProvider.AddParameter("Id", Id, SqlDbType.Int);

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

        public ApiResult<OrderResult> GetPaging(BaseCondition<Order> model)
        {
            var result = new ApiResult<OrderResult>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_Order_GetPaging", CommandType.StoredProcedure);

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


                result.DataList = DbProvider.ExecuteListObject<OrderResult>();
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

        public ApiResult<bool> Abort(int Id, int UserId)
        {
            ApiResult<Boolean> Rs = new ApiResult<Boolean>();
            try
            {
                DbProvider.SetCommandText("sp_order_Abort", CommandType.StoredProcedure);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.NVarChar);
                DbProvider.AddParameter("OrderId", Id, SqlDbType.Int);

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


        public ApiResult<bool> Paid(int Id, int UserId)
        {
            ApiResult<Boolean> Rs = new ApiResult<Boolean>();
            try
            {
                DbProvider.SetCommandText("sp_order_Paid", CommandType.StoredProcedure);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.NVarChar);
                DbProvider.AddParameter("OrderId", Id, SqlDbType.Int);

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

        public ApiResult<OrderDetail> GetOne(int Id)
        {
            ApiResult<OrderDetail> Rs = new ApiResult<OrderDetail>();
            try
            {
                DbProvider.SetCommandText("sp_order_GetOne", CommandType.StoredProcedure);
                DbProvider.AddParameter("OrderId", Id, SqlDbType.Int);

                Rs.Data = DbProvider.ExecuteObject<OrderDetail>();
                if (Rs.Data == null || Rs.Data.Id == 0)
                {
                    Rs.Failed(new ErrorObject()
                    {
                        Code = "NOT_FOUND",
                        Description = "Order Not FOund"
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

        public ApiResult<bool> Checkin(int Id)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_order_Checkin", CommandType.StoredProcedure);
                DbProvider.AddParameter("OrderId", Id, SqlDbType.Int);

                DbProvider.ExecuteNonQuery();
                
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

        public ApiResult<bool> Checkout(int Id)
        {
            ApiResult<bool> Rs = new ApiResult<bool>();
            try
            {
                DbProvider.SetCommandText("sp_order_Checkout", CommandType.StoredProcedure);
                DbProvider.AddParameter("OrderId", Id, SqlDbType.Int);

                DbProvider.ExecuteNonQuery();

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

        public void DailyCheck()
        {
            try
            {
                DbProvider.SetCommandText("Sp_Order_dailyCheck", CommandType.StoredProcedure);
                DbProvider.ExecuteNonQuery();
                var now = DateTime.Now;

                Libs.WriteLog("Daily Log", "Logs Daily at: " + now.ToString());
            }
            catch (Exception)
            {
                var now = DateTime.Now;

                Libs.WriteLog("Error Daily Log", "Logs Daily at: " + now.ToString());
            }

           
        }
    }
}