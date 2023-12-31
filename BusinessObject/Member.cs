﻿using System.ComponentModel.DataAnnotations;

#nullable disable

namespace BusinessObject
{
    public partial class Member
    {
        public Member()
        {
            Orders = new HashSet<Order>();
        }

        [Display(Name = "Member ID")]
        public int MemberId { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [Required]
        [Phone]
        public string Phone { get; set; }

        [Display(Name = "Address")]
        [StringLength(100, MinimumLength = 5)]
        [Required]
        public string Address { get; set; }

        [Display(Name = "City")]
        [Required]
        public string City { get; set; }

        [Display(Name = "Country")]
        [Required]
        public string Country { get; set; }


        [Display(Name = "Member Name")]
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Fullname { get; set; }

        [Display(Name = "Username")]
        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required]
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }

        [Display(Name = "Role")]
        public int Role { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
