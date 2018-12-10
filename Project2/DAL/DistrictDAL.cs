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
    public class DistrictDAL : BaseDAL
    {
        public DistrictDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<District> GetByProvinceCode(string ProvinceCode)
        {
            var Rs = new ApiResult<District>();
            try
            {
                DbProvider.SetCommandText2("sp_District_GetByProvinceCode", CommandType.StoredProcedure);
                DbProvider.AddParameter("ProvinceCode", ProvinceCode, SqlDbType.NVarChar);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);
                DbProvider.AddParameter("TotalRecords", DBNull.Value, SqlDbType.Int, ParameterDirection.Output);

                Rs.DataList = DbProvider.ExecuteListObject<District>();
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

        public ApiResult<District> District_Delete_DAL(int Id)
        {
            var result = new ApiResult<District>();

            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_District_Delete", CommandType.StoredProcedure);

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
        /// <returns>Danh sách quận huyện</returns>
        public ApiResult<District> District_GetAllSearchPaging_DAL(BaseCondition<District> model)
        {
            var result = new ApiResult<District>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_District_GetAllWithSearchPaging", CommandType.StoredProcedure);

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

                //Lấy về danh sách các quận huyện
                result.DataList = DbProvider.ExecuteListObject<District>();
                result.TotalRecords = int.Parse(DbProvider.Command.Parameters["TotalRecords"].Value.ToString());

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
        /// Lấy về thông tin của quận huyện theo ID truyền vào
        /// </summary>
        /// <param name="ID">ID quận huyện</param>
        /// <returns>Thông tin quận huyện</returns>
        public ApiResult<District> District_GetItemById_DAL(int ID)
        {
            var result = new ApiResult<District>();

            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_District_GetItemById", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("ID", ID, SqlDbType.Int);

                // Output params
                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ErrorMessage", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                //Lấy về danh sách các quận huyện 
                result.Data = DbProvider.ExecuteObject<District>();

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
        /// Thêm mới quận huyện vào database
        /// </summary>
        /// <param name="model">Thông tin các trường trong database</param>
        /// <returns>Kết quả sau khi thêm mới</returns>
        public ApiResult<District> District_Insert_DAL(District model)
        {
            var result = new ApiResult<District>();

            try
            {
                //Đặt tên cho stored procedures
                DbProvider.SetCommandText2("sp_District_Insert", CommandType.StoredProcedure);

                //Input params
                DbProvider.AddParameter("DistrictName", model.DistrictName, SqlDbType.NVarChar);
                DbProvider.AddParameter("ProvinceCode", model.ProvinceCode, SqlDbType.NVarChar);

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

        /// <summary>
        /// Cập nhật quận huyện vào database
        /// </summary>
        /// <param name="model">Thông tin quận huyện cần cập nhật</param>
        /// <returns>Thông tin quận huyện sau khi cập nhật</returns>
        public ApiResult<District> District_Update_DAL(District model)
        {
            var result = new ApiResult<District>();

            try
            {
                // Set tên stored procedure
                DbProvider.SetCommandText2("sp_District_Update", CommandType.StoredProcedure);

                // Input params
                DbProvider.AddParameter("ID", model.Id, SqlDbType.Int);
                DbProvider.AddParameter("DistrictName", model.DistrictName, SqlDbType.NVarChar);
                DbProvider.AddParameter("ProvinceCode", model.ProvinceCode, SqlDbType.NVarChar);

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