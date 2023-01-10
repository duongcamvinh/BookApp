using Api.Data;
using Api.DTO;
using Api.Entities;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRespository _bookRespository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public BooksController(IBookRespository bookRespository, IMapper mapper,IPhotoService photoService)
        {
            _bookRespository = bookRespository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetBooksAsync()
        {
            var books = await _bookRespository.GetBooksAsync();
            return Ok(_mapper.Map<IEnumerable<BooksDto>>(books));
        }
        [HttpGet("{bookname}")]
        public async Task<ActionResult<BooksDto>> GetBookByNameAsync(string bookname)
        {
           var book = await _bookRespository.GetBookName(bookname);
            return Ok(_mapper.Map<BooksDto>(book));
        }
        [HttpPost("add-book")]
        public async Task<ActionResult<AppBooks>> AddBook([FromBody] AppBooks appBooks)
        {
            return await _bookRespository.AddBookd(appBooks);
        }
        [Authorize(Roles ="Member")]
        [HttpPut("update-book/{id}")]
        public async Task<ActionResult> UpdateBook( [FromBody]BookUpdateDto bookUpdateDto,  int id)
        {
            //var book= await _bookRespository.GetBookByBooknameAsync(bookname);
            //_mapper.Map(bookUpdateDto, book);
            //_bookRespository.UpdateBook(book);
            //if (await _bookRespository.SaveAllAsync())
            //    return NoContent();
            //return BadRequest("fail");
            try
            {
                var book = await _bookRespository.GetBookByIdAsync(id);
                _mapper.Map(bookUpdateDto, book);
                _bookRespository.UpdateBook(book);
                return Ok(new
                {
                    Id= book.Id,
                    BookName= book.BookName,
                    TitleBooks=book.TitleBooks,
                    Author=book.Author,
                    Introduction=book.Introduction
                });
            }
            catch
            {
                return BadRequest("Fail");
            }
        }
        [HttpDelete("delete-book/{id}")]
        public async Task<ActionResult> DeteleBook(int id)
        {
            await _bookRespository.DeleteBookAsync(id);
            return Ok();

        }

        [HttpPost("add-photo/{bookname}")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file,string bookname)
        {
            
            var book = await _bookRespository.GetBookName(bookname);
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (book.Photos.Count == 0)
            {
                photo.IsMain=true;
            }
            book.Photos.Add(photo);
            if(await _bookRespository.SaveAllAsync())
            {
                return Ok("Success");
            }
            return BadRequest("Prolem adding photo");
        }
        [HttpDelete("delete-photo/{bookname}/{photoId}")]
        public async Task<ActionResult> DeletePhoto(string bookname,int photoId)
        {    
            var book = await _bookRespository.GetBookByBooknameAsync(bookname);
            var photo = book.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null)
                return NotFound();
            if (photo.IsMain)
                return BadRequest("you cannot delete your main photo ");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    return BadRequest(result.Error.Message);
            }
            book.Photos.Remove(photo);
            if (await _bookRespository.SaveAllAsync())
                return Ok();
            return BadRequest("Failed to delete the photo");
        }
    }
}
