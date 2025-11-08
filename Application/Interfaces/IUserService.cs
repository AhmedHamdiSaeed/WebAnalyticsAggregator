using Application.DTOs.Auth;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto?> RegisterAsync(string name, string email, string password);
        Task<AuthResponseDto?> LoginAsync(string email, string password);
    }
}
