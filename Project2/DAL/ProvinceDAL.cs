using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using Common.Base;

namespace Project2.DAL
{
    public class ProvinceDAL : BaseDAL
    {
        public ProvinceDAL(SqlDbProvider dbProvider) : base(dbProvider)
        {

        }

        public ApiResult<Province> GetProvince()
        {
            var Rs = new ApiResult<Province>();
            try
            {
                DbProvider.SetCommandText2("sp_Province_getall", CommandType.StoredProcedure);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);
                DbProvider.AddParameter("TotalRecords", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                Rs.DataList = DbProvider.ExecuteListObject<Province>();
                Rs.TotalRecords = Int32.Parse(DbProvider.Command.Parameters["TotalRecords"].Value.ToString());
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

        public ApiResult<Province> Province_Delete_DAL(int Id)
        {
            var result = new ApiResult<Province>();

            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_Province_Delete", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("ID", Id, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                // Xử lý thay đổi trạng thái
                DbProvider.ExecuteNonQuery();

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

        /// <summary>
        /// Lấy tất cả dữ liệu , tìm kiếm và phân trang
        /// </summary>
        /// <param name="model">Thông tin dữ liệu tìm kiếm và phân trang</param>
        /// <returns>Danh sách tỉnh thành</returns>
        public ApiResult<Province> Province_GetAllSearchPaging_DAL(BaseCondition<Province> model)
        {
            var result = new ApiResult<Province>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_Province_GetPaging", CommandType.StoredProcedure);

                // Input params                    
                DbProvider.AddParameter("PageIndex", model.FromRecord, SqlDbType.Int);
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
                DbProvider.AddParameter("TotalCount", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                //Lấy về danh sách các tỉnh thành
                result.DataList = DbProvider.ExecuteListObject<Province>();
                result.TotalRecords = int.Parse(DbProvider.Command.Parameters["TotalCount"].Value.ToString());

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

        /// <summary>
        /// Lấy về thông tin của tỉnh thành theo ID truyền vào
        /// </summary>
        /// <param name="ID">ID tỉnh thành</param>
        /// <returns>Thông tin tỉnh thành</returns>
        public ApiResult<Province> Province_GetItemById_DAL(int ID)
        {
            var result = new ApiResult<Province>();
            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_Province_GetById", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("Id", ID, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                //Lấy về tỉnh thành 
                result.Data = DbProvider.ExecuteObject<Province>();

                // Kiểm tra kết quả trả về
                string errorCode = DbProvider.Command.Parameters["ErrCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS) && errorCode != "")
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

        /// <summary>
        /// Thêm mới tỉnh thành vào database
        /// </summary>
        /// <param name="model">Thông tin các trường trong database</param>
        /// <returns>Kết quả sau khi thêm mới</returns>
        public ApiResult<Province> Province_Insert_DAL(Province model)
        {
            var result = new ApiResult<Province>();
            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_Province_Insert", CommandType.StoredProcedure);

                //Input params 
                DbProvider.AddParameter("ProvinceName", model.ProvinceName, SqlDbType.NVarChar);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                //Xử lý thêm mới
                DbProvider.ExecuteNonQuery();

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

        /// <summary>
        /// Cập nhật tỉnh thành vào database
        /// </summary>
        /// <param name="model">Thông tin tỉnh thành cần cập nhật</param>
        /// <returns>Thông tin tỉnh thành sau khi cập nhật</returns>
        public ApiResult<Province> Province_Update_DAL(Province model)
        {
            var result = new ApiResult<Province>();

            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_Province_Update", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("ID", model.Id, SqlDbType.Int);
                DbProvider.AddParameter("ProvinceName", model.ProvinceName, SqlDbType.NVarChar);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                // Xử lý 
                DbProvider.ExecuteNonQuery();

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

    }
}