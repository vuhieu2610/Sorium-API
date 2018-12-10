using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project2.Models
{
    #region Base
    public class BaseHotel
    {

        [Required]
        public int Id { set; get; }
        [Required]
        public string Name { set; get; }
        [Required]
        public string Address { set; get; }
        [Required]
        public string Description { set; get; }
        [Required]
        public string ProvinceCode { set; get; }
        [Required]
        public string DistrictCode { set; get; }
        public string Services { set; get; }
        public string Stars { set; get; }
    }
    #endregion

    public class HotelModels : BaseHotel
    {
        [Description("Id Của chủ khách sạn")] // không cần cho vào json, get từ tokenCOde
        public int Owner { set; get; }
        public int Status { set; get; }
        public DateTime CreateDate { set; get; }
        public int ConfirmUser { set; get; }
        public int IsConfirmed { set; get; }
        public string TaxCode { set; get; }
        // Base64
        public string Avatar { set; get; }
        // Base 64
        public string CoverImage { set; get; }
        public string PhoneNumber { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }
        // Array Base64 chứa list ảnh
        public List<string> Images { set; get; }

        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        
    }
    public class HotelPaging : HotelModels
    {
        public string Tags { set; get; }
        public int MinPrice { set; get; }
    }
    public class PostHotel
    {
        [Required]
        public string HotelName { set; get; }

        [EmailAddress]
        public string HotelEmail { set; get; }

        [Required]
        public string HotelPhoneNumber { set; get; }

        [Required]
        public string HotelAddress { set; get; }

        [Description("Mã số thuế")]
        public string TaxCode { set; get; }

        [Required]
        public string ProvinceCode { set; get; }

        [Required]
        public string DistrictCode { set; get; }

        [Required]
        [Description("Đánh giá số sao cho khách sạn 1-5")]
        public string HotelStar { set; get; }

        public string Description { set; get; }

        public List<string> Image { set; get; }

        [Required]
        public string Services { set; get; } = "[]";

        public string Avatar { set; get; }

        public string CoverImage { set; get; }

        public string Album { set; get; }

    }

    public class HotelResult : BaseHotel
    {
        public string Avatar { set; get; }
        public string CoverImage { set; get; }

        public string Email { set; get; }
        public string PhoneNumber { set; get; }
        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public List<FileUploadCompact> Album { set; get; }
        public List<Tag> Tags { set; get; }
        public List<RoomResult> ListRoom { set; get; }
        
        public List<FileUploadCompact> AnhGiayTo { set; get; }
    }

    public class PutHotel : BaseHotel
    {
        [Required]
        public string PhoneNumber { set; get; }
        [Required]
        public string Email { set; get; }
        [Required]
        public string Avatar { set; get; }
        [Required]
        public string CoverImage { set; get; }
        public List<string> Images { set; get; }
        public List<Tag> Tags { set; get; }

        

        public List<FileUploadCompact> Album { set; get; } = new List<FileUploadCompact>();
    }

}