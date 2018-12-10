using System.Linq;
using System.Net;
using System.Web.Http;
using Project2.DAL;
using Project2.Models;
using Common;
using Common.Base;

namespace Project2.Controllers
{
    [RoutePrefix("api/Province")]
    public class ProvinceController : BaseController
    {
        private ProvinceDAL provinceDAL;

        public ProvinceController()
        {
            provinceDAL = new ProvinceDAL(DbProvider);
        }

        [HttpGet]
        public IHttpActionResult GetProvince()
        {
            return Ok(provinceDAL.GetProvince());
        }

        [HttpGet]
        [Route("GetItem/{Id}")]
        public IHttpActionResult GetItemById(int Id = 0)
        {
            return Ok(provinceDAL.Province_GetItemById_DAL(Id));
        }

        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<Province> model)
        {
            return Ok(provinceDAL.Province_GetAllSearchPaging_DAL(model));
        }

        [HttpPost]
        [Route("Insert")]
        [ApiCustomFilter]
        public IHttpActionResult Insert(Province model)
        {
            var Rs = new ApiResult<Province>();
            if (model == null)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "EXCEPTION",
                    Description = "Data not Found"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            if (!ModelState.IsValid)
            {
                var Err = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);
                foreach (var e in Err)
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = "EXCEPTION",
                        Description = e
                    });
                }

                return Content(HttpStatusCode.BadRequest, Rs);
            }
            return Ok(provinceDAL.Province_Insert_DAL(model));
        }

        [HttpPut]
        [Route("Update")]
        [ApiCustomFilter]
        public IHttpActionResult Update(Province model)
        {
            var Rs = new ApiResult<Province>();
            if (model == null)
            {
                Rs.Failed(new ErrorObject
                {
                    Code = "EXCEPTION",
                    Description = "Data not Found"
                });
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            if (!ModelState.IsValid)
            {
                var Err = ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);
                foreach (var e in Err)
                {
                    Rs.Failed(new ErrorObject
                    {
                        Code = "EXCEPTION",
                        Description = e
                    });
                }

                return Content(HttpStatusCode.BadRequest, Rs);
            }
            return Ok(provinceDAL.Province_Update_DAL(model));
        }


        [HttpDelete]
        [Route("Delete/{Id}")]
        [ApiCustomFilter]
        public IHttpActionResult Delete(int Id)
        {
            return Ok(provinceDAL.Province_Delete_DAL(Id));
        }
    }
}