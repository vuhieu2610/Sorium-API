using Project2.Models;
using Common.SqlService;
using Common;
using System.Data;

namespace Project2.DAL
{
    public class HomeDAL : BaseDAL
    {
        public HomeDAL(SqlDbProvider dbProvider) : base(dbProvider) { }

        public ApiResult<HomeModels> GetCount()
        {
            ApiResult<HomeModels> Rs = new ApiResult<HomeModels>();
            DbProvider.SetCommandText("sp_DashBroad_GetCounting", CommandType.StoredProcedure);
            Rs.Data = DbProvider.ExecuteObject<HomeModels>();
            return Rs;
        }
    }
}