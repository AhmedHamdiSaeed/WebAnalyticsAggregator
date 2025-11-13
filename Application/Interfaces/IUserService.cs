
using DTOs.Auth;
using DTOs;
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
        Task<Result<AuthResponseDto>> RegisterAsync(string name, string email, string password);
        Task<Result<AuthResponseDto>> LoginAsync(string email, string password);
    }
}
