using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Common.Utilities;
using Project2.DAL;
using Project2.Models;
using Common;
using Newtonsoft.Json;
using Project2.CustomAttributes;
using System.Web.Http.Cors;
using Common.Base;

namespace Project2.Controllers
{
    [RoutePrefix("api/District")]
    public class DistrictController : BaseController
    {
        private DistrictDAL districtDAL;

        public DistrictController()
        {
            districtDAL = new DistrictDAL(DbProvider);
        }

        [HttpGet]
        public IHttpActionResult GetByProvinceCode(string ProvinceCode = "")
        {
            ProvinceCode = ProvinceCode.Trim();
            if (!ModelState.IsValid)
            {
                var message = string.Join(" | ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage));

                return Content(HttpStatusCode.BadRequest, message);
            } else
            {
                return Ok(districtDAL.GetByProvinceCode(ProvinceCode));
            }
        }

        [HttpGet]
        [Route("GetItem/{Id}")]
        public IHttpActionResult GetItemById(int Id  = 0)
        {
            return Ok(districtDAL.District_GetItemById_DAL(Id));
        }

        [HttpPost]
        [Route("GetPaging")]
        public IHttpActionResult GetPaging(BaseCondition<District> model)
        {
            var FilterRules = model.FilterRules;
            if (FilterRules.Count > 0)
            {
                var ProvinceField = FilterRules.FindIndex(item => (item.field.ToLower().IndexOf("province") > -1));
                if (ProvinceField > -1)
                {
                    model.FilterRules[ProvinceField].field = model.FilterRules[ProvinceField].field.Replace("Province", "D.Province");
                }

                var DistrictField = FilterRules.FindIndex(item => (item.field.ToLower().IndexOf("district") > -1));
                if (DistrictField > -1)
                {
                    model.FilterRules[DistrictField].field = model.FilterRules[DistrictField].field.Replace("District", "D.District");
                }
            }
            return Ok(districtDAL.District_GetAllSearchPaging_DAL(model));
        }

        /// <summary>
        /// API thêm mới quận huyện
        /// </summary>
        /// <param name="model">Thông tin quận huyện</param>
        /// <returns>Thông tin mới của quận huyện trong database</returns>
        [Route("Insert")]
        [HttpPost]
        [ApiCustomFilter]
        public IHttpActionResult Insert(District model)
        {
            var Rs = new ApiResult<District>();
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
            return Ok(districtDAL.District_Insert_DAL(model));
        }

        /// <summary>
        /// API cập nhật mới thông tin quận huyện
        /// </summary>
        /// <param name="model">Thông tin quận huyện</param>
        /// <returns>Thông tin quận huyện sau khi cập nhật</returns>
        [Route("Update")]
        [HttpPut]
        [ApiCustomFilter]
        public IHttpActionResult Update(District model)
        {
            var Rs = new ApiResult<District>();
            if (model == null)
            {
                Rs.Failed(new ErrorObject {
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
                    Rs.Failed(new ErrorObject {
                        Code = "EXCEPTION",
                        Description = e
                    });
                }
                
                return Content(HttpStatusCode.BadRequest, Rs);
            }
            return Ok(districtDAL.District_Update_DAL(model));
        }

        /// <summary>
        /// API xóa quận huyện
        /// </summary>
        /// <param name="model">ID của quận huyện</param>
        /// <returns>Kết quả xóa</returns>
        [Route("Delete/{Id}")]
        [HttpDelete]
        [ApiCustomFilter]
        public IHttpActionResult Delete(int Id)
        {
            return Ok(districtDAL.District_Delete_DAL(Id));
        }
    }
}