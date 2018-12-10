using Project2.Models;
using Common;
using Common.SqlService;
using System.Data;

namespace Project2.DAL
{
    public class RoleDAL : BaseDAL
    {
        public RoleDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<Role> Get()
        {
            ApiResult<Role> Rs = new ApiResult<Role>();
            DbProvider.SetCommandText("sp_Role_get", CommandType.StoredProcedure);
            Rs.DataList = DbProvider.ExecuteListObject<Role>();
            return Rs;

        }
    }
}