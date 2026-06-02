using BankingAPI.Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Application.Interfaces
{
    public interface IAdminService
    {
        Task<List<UsersResponse>> GetUsers();
    }
}
