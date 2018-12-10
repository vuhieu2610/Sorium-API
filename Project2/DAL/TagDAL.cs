using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project2.Models;
using Common;
using Common.Utilities;
using Common.SqlService;
using System.Data;
using System.Web.Security;
using Common.Base;

namespace Project2.DAL
{
    public class TagDAL : BaseDAL
    {
        public TagDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<Tag> GetAll(int type = 0)
        {
            var result = new ApiResult<Tag>();
            try
            {
                DbProvider.SetCommandText("sp_tags_getall", CommandType.StoredProcedure);
                DbProvider.AddParameter("Type", type, SqlDbType.Int);
                result.DataList = DbProvider.ExecuteListObject<Tag>();
            }
            catch (Exception ex) {
                result.Failed(new ErrorObject {
                    Code = Constants.ERR_EXCEPTION,
                    Description = ex.ToString()
                });
            }

            return result;
        }
    }
}