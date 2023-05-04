﻿using System.ComponentModel.DataAnnotations;

namespace HelpDeskSystem.Models
{
    public class Account
    {
        [Key, Required]
        public int id
        {
            get;
            set;
        }
        [Required]
        public string? fullname
        {
            get;
            set;
        }
        public string? company
        {
            get;
            set;
        }
        public string? workemail
        {
            get;
            set;
        }
        public string? password
        {
            get;
            set;
        }
        public int idCompany
        {
            get;
            set;
        }
        public bool confirm
        {
            get;
            set;
        }
        public bool login
        {
            get;
            set;
        }
        public int status
        {
            get;
            set;
        }
        public string? idGuId
        {
            get;
            set;
        }
    }
}
