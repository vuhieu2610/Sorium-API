using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project2.Models
{
    public class CommentModels
    {
        public int Id { set; get; }
        public int HotelId { set; get; }
        public int UserId { set; get; }
        public string Content { set; get; }
        public int ParentId { set; get; }

        public string UserAvatar { set; get; }
        public string UserName { set; get; }
        public string HotelName { set; get; }
    }
    public class PostComment
    {
        public int HotelId { set; get; }
        public string Content { set; get; }
        public int ParentId { set; get; }
    }

    public class RemoveComment
    {
        public int Id { set; get; }
        public int UserId { set; get; }
    }


    public class HomeSideComment
    {
        public int Id { set; get; }
        public int HotelId { set; get; }
        public int UserId { set; get; }
        public string Content { set; get; }

        public string FullName { set; get; }
        public string UserProvinceName { set; get; }
        public string UserDistrictName { set; get; }

        public string HotelName { set; get; }
        public string HotelAddress { set; get; }
        public string HotelProvinceName { set; get; }
        public string HotelDistrictName { set; get; }


    }
}