using Common.SqlService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project2.DAL
{
    public class BaseDAL
    {
        public BaseDAL(SqlDbProvider dbProvider)
        {
            DbProvider = dbProvider;
        }
        #region ShareProperties
        protected SqlDbProvider DbProvider { get; set; }
        #endregion
    }
}