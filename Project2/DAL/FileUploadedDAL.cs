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
using System.Web.Helpers;
using Newtonsoft.Json;

namespace Project2.DAL
{
    public class FileUploadedDAL : BaseDAL
    {
        public FileUploadedDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public Boolean Insert (FileUploaded Item, int UserId)
        {
            try
            {
                DbProvider.SetCommandText2("sp_FileUploaded_Insert", CommandType.StoredProcedure);
                DbProvider.AddParameter("InJson", Libs.SerializeObject(Item), SqlDbType.NVarChar);
                DbProvider.AddParameter("UserId", UserId, SqlDbType.Int);

                DbProvider.AddParameter("ErrorCode", DBNull.Value, SqlDbType.NVarChar, 100, ParameterDirection.Output);
                DbProvider.AddParameter("ReturnMsg", DBNull.Value, SqlDbType.NVarChar, 4000, ParameterDirection.Output);

                DbProvider.ExecuteNonQuery();
                string errorCode = DbProvider.Command.Parameters["ErrorCode"].Value.ToString();
                if (!errorCode.Equals(Constants.SUCCESS)) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}