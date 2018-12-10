using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project2.Models
{
    public class RoomModels
    {
        //[Required]
        public int Id { set; get; }

        [Required]
        public string Name { set; get; }

        [Required]
        public string Price { set; get; }

        [Required]
        public int RoomStatus { set; get; }

        [Required]
        public int HotelId { set; get; }

        [Required]
        public string Description { set; get; }

        public int Status { set; get; } = 1;

        [Required]
        public int BedNum { set; get; }

        [Required]
        public int Acreage { set; get; }

        [Required]
        public string Services { set; get; }
        public int IsHot { set; get; }

        [Required]
        public int Adults { set; get; }

        [Required]
        public int Children { set; get; }
        [Required]
        public string Avatar { set; get; }
    }

    public class RoomResult : RoomModels
    {
        public string HotelName { set; get; }
    }

    public class RoomDetail : RoomResult
    {
        public List<Tag> ListTag { set; get; }
    }

}