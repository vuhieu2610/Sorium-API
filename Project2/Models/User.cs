using Common.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project2.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int RoleId { get; set; } = 1;
        public string RoleDesc { set; get; }
        public int ProvinceCode { get; set; }
        public int DistrictCode { get; set; }
        public int EmailConfirmed { get; set; }
        public string UserAvatar { set; get; }
        public string Gender { set; get; }

    }

    public class UserPostData : User
    {
        [Required]
        public string Password { get; set; }
        public string OTP { set; get; }
        public int CreatedUser { set; get; }
        [Required]
        public string SucRedirectUrl { set; get; }
        [Required]
        public string FailRedirectUrl { set; get; }
    }
    public class UserResendEmail
    {
        [Required]
        public int Id { set; get; }
        [Required]
        public string SucRedirectUrl { set; get; }
        [Required]
        public string FailRedirectUrl { set; get; }
    }
    public class UserPostLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class UserInfo : User
    {
        public string Password { set; get; }
    }
    public class UserResult : User
    {
        public string AccessToken { get; set; }
        public string GetAvatarUrl
        {
            get
            {
                if (!String.IsNullOrEmpty(UserAvatar))
                {
                    return ConfigUtil.DomainBaseHttp + UserAvatar;
                }
                else return "";
            }
        }
    }

    public class UserPostOTP
    {
        [Required(ErrorMessage = "User Id is Required")]
        public int Id { set; get; }

        [Required(ErrorMessage = "OTP Code is Required")]
        public string OTP { set; get; }
    }

    public class ChangePasswordViewModel
    {
        public string OldPasword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

    public class ChangePWModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ForgetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string UrlForm { set; get; }

    }
    public class UpdatePasswordViewModel
    {
        public string Email { get; set; }
        [Required]
        public string TokenCode { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set; }
    }

    public class UserAvatarViewModel
    {
        public string ImagePath { set; get; }
    }

    public class SetRole
    {
        [Required]
        public int RoleId { set; get; }

        [Required]
        public int UserId { set; get; }
    }

    public class GetUserRole
    {
        public int Id { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string Email { set; get; }
        public int RoleId { set; get; }
        public string PhoneNumber { set; get; }
    }
}