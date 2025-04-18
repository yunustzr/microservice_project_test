using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Domain.Models.DTO
{
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
    }
}