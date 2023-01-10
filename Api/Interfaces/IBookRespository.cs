using Api.DTO;
using Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Interfaces
{
   public interface IBookRespository
    {
        Task<IEnumerable<AppBooks>> GetBooksAsync();
        Task<AppBooks> GetBookName(string bookname);
        Task<AppBooks> AddBookd(AppBooks appbooks);
        Task<AppBooks> GetBookByIdAsync(int id);
        void UpdateBook(AppBooks appbooks);
        Task<bool> SaveAllAsync();
        Task DeleteBookAsync(int bookid);
        Task<AppBooks> GetBookByBooknameAsync(string bookname);
    }
}
