using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class District
    {
        public int Id { set; get; }
        public string DistrictName { set; get; }
        public string DistrictCode { set; get; }

        [Required(AllowEmptyStrings = false)]
        public string ProvinceCode { set; get; }
        public string ProvinceName { set; get; }
    }
}