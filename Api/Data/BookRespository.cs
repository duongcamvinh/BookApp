using Api.DTO;
using Api.Entities;
using Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Data
{
    public class BookRespository : IBookRespository
    {
        private readonly DataContext _context;
        public BookRespository(DataContext context)
        {
            _context = context;
        }

        public async Task<AppBooks> AddBookd(AppBooks appBooks)
        {
            var books = new AppBooks()
            {
                BookName = appBooks.BookName,
                Author = appBooks.Author,
                TitleBooks = appBooks.TitleBooks,
                Introduction = appBooks.Introduction
            };
            _context.Books.Add(books);
            await _context.SaveChangesAsync();
            return books;

        }

        public async Task DeleteBookAsync(int bookid)
        {
            var book = _context.Books.FirstOrDefault(x => x.Id == bookid);
            _context.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<AppBooks> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }
        public async Task<AppBooks> GetBookName(string bookname)
        {
            //return await _context.Books.Where(x => x.BookName == bookname)
            //    .Select
            //    (book => new BooksDto
            //    {
            //        Id = book.Id,
            //        BookName = book.BookName,
            //        TitleBooks = book.TitleBooks,
            //        Author = book.Author,
            //        Introduction = book.Introduction,
            //        Photos = book.Photos.Select(p => new PhotoDto
            //        {
            //            Id = p.Id,
            //            Url = p.Url,
            //            IsMain = p.IsMain
            //        }).ToList()
            //    }).SingleOrDefaultAsync();
            return await _context.Books.Include(p => p.Photos).SingleOrDefaultAsync(x => x.BookName == bookname);
        }
        public async Task<IEnumerable<AppBooks>> GetBooksAsync()
        {

            //return await _context.Books.Include(p => p.Photos)
            //    .Select(book => new BooksDto
            //    {
            //        Id = book.Id,
            //        TitleBooks = book.TitleBooks,
            //        BookName = book.BookName,
            //        Author = book.Author,
            //        Introduction = book.Introduction,
            //        Photos = book.Photos.Select(p => new PhotoDto
            //        {
            //            Id = p.Id,
            //            Url = p.Url,
            //            IsMain = p.IsMain
            //        }).ToList()
            //    })
            //    .ToListAsync();
            return await _context.Books.Include(p=>p.Photos).ToListAsync();
        }
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateBook(AppBooks appbooks)
        {
            //var book = await _context.Books.FindAsync(bookId);
            //if (book != null)
            //{
            //    book.TitleBooks = appbooks.TitleBooks;
            //    book.Author = appbooks.Author;
            //    book.Introduction = appbooks.Introduction;
            //    book.BookName = appbooks.BookName;
            //    await _context.SaveChangesAsync();
            //}
            //return book;
            _context.Entry(appbooks).State=EntityState.Modified;
            _context.SaveChangesAsync();
        }
        public async Task<AppBooks> GetBookByBooknameAsync(string bookname)
        {
            return await _context.Books.Include(p=>p.Photos).SingleOrDefaultAsync(x=>x.BookName == bookname);
        }
    }
}


