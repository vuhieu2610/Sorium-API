using System.Web.Http;
using Project2.DAL;

namespace Project2.Controllers
{
    [RoutePrefix("API/Role")]
    public class RoleController : BaseController
    {
        private RoleDAL roleDAL;

        public RoleController()
        {
            roleDAL = new RoleDAL(DbProvider);
        }

        [HttpGet]
        [Route("Get")]
        public IHttpActionResult Get()
        {
            return Ok(roleDAL.Get());
        }
    }
}