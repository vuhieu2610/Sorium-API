using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class Province
    {
        public int Id { set; get; }
        public string ProvinceCode { set; get; }
        [Required]
        public string ProvinceName { set; get; }
    }
}