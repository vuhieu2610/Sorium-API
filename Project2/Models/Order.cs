using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project2.Models
{
    public class Order
    {
        public int Id { set; get; }
        public int RoomId { set; get; }
        public DateTime Checkin { set; get;}
        public DateTime Checkout { set; get; }
        public string Description { set; get; }
        public int Price { set; get; }
        public int Status { set; get; }
        public int IsPaid { set; get; } = 0;
        public string GuestName { set; get; }
        public string GuestEmail { set; get; }
        public string GuestPhoneNumber { set; get; }
        public int IsConfirmed { set; get; } = 0;
        public string Note { set; get; }
        public int PNum { set; get; }

        public string RoomName { set; get; }
        public string TokenCode { set; get; }
    }


    public class PostOrder
    {
        public Order Order { set; get; }

        public string SucUrl { set; get; }
        public string FailUrl { set; get; }
    }

    public class NewOrderResult
    {
        public int Id { set; get; }
    }

    public class OrderResult
    {
        public int Id { set; get; }
        public int RoomId { set; get; }
        public DateTime Checkin { set; get; }
        public DateTime Checkout { set; get; }
        public string Description { set; get; }
        [Required]
        public int Price { set; get; }
        public int IsPaid { set; get; } = 0;
        [Required]
        public string GuestName { set; get; }
        [Required]
        public string GuestEmail { set; get; }
        [Required]
        public string GuestPhoneNumber { set; get; }
        public int IsConfirmed { set; get; } = 0;
        public string Note { set; get; }
        [Description("Số người.")]
        public int PNum { set; get; }
        [Description("Trạng thái của hóa đơn 0 = đã bị hủy")]
        public int Status { set; get; }
        public DateTime? RealCheckin { set; get; }
        public DateTime? RealCheckout { set; get; }

        public int HotelId { set; get; }
        public string HotelName { set; get; }
        public int Owner { set; get; }
        public string RoomName { set; get; }
    }

    public class OrderDetail : OrderResult
    {
        public new string RoomName { set; get; }
    }

    public class AbortModel
    {
        public int Id { set; get; }
        public string ReasonAbort { set; get; }
    }
}