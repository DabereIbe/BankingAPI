using BankingAPI.Application.DTOs.Admin;
using BankingAPI.Application.Interfaces;
using BankingAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UsersResponse>> GetUsers()
        {
            var users = await _context.Users
            .Select(u => new UsersResponse { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role, CreatedAt = u.CreatedAt })
            .ToListAsync();

            return users;
        }
    }
}
