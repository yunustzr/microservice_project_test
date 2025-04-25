using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Domain.Models.DTO
{
    public class KeyPairDto
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}