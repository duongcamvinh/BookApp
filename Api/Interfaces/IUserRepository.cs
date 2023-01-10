using Api.DTO;
using Api.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUsers>> GetUsersAsync();
        Task<AppUsers> GetUserByIdAsnyc(string id);
        Task<MemberDto> GetUserByNameAsync(string username);
        void UpdateUser(AppUsers user);
        Task<bool> SaveAllAsync();
        Task DeleteUserAsync(int id);
        Task<AppUsers> GetUserByUserNameAsync(string username); 
    }
}
