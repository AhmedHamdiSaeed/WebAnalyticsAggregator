using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
