﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCommon.DTOs.User
{
    internal class RegisterUserDTO
    {
        public int UsertId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Password { get; set; }
    }
}
