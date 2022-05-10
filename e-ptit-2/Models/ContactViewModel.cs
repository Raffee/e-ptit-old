using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace e_ptit_2.Models
{
    public class ContactViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string name { get; set; }

        [StringLength(60, MinimumLength = 6)]
        [EmailAddress]
        [Required]
        public string email { get; set; }

        [Required]
        public string country { get; set; }

        [Required]
        [StringLength(6000, MinimumLength = 5)]
        public string text { get; set; }
    }
}