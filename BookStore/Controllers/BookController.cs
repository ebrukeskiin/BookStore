using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class BookController : ControllerBase
    {
        private static List<Book> BookList = new List<Book>()
        {
            new Book
            {
                Id = 1,
                Title = "Lean Startup",
                GenreId = 1,
                PageCount = 200,
                PublishDate = new DateTime(2001, 06, 12)
            },
            new Book
            {
                Id = 2,
                Title = "Herland",
                GenreId = 2,
                PageCount = 400,
                PublishDate = new DateTime(2010, 05, 19)
            },
            new Book
            {
                Id = 3,
                Title = "Dune",
                GenreId = 3,
                PageCount = 400,
                PublishDate = new DateTime(2019, 06, 26)
            },
            new Book
            {
                Id = 4,
                Title = "Harmoni",
                GenreId = 4,
                PageCount = 400,
                PublishDate = new DateTime(2010, 05, 19)
            }
        };

        [HttpGet]
        public List<Book> GetBooks()
        {
            var books = BookList.Where(x => x != null)  // Filter out null books
                     .OrderBy(x => x.Id)
                     .ToList();
            return books;
        }

        [HttpGet("{id}")]
        public Book GetById(int id)
        {
            var book = BookList.Where(book => book.Id == id).SingleOrDefault();
            return book;
        }

        [HttpPost]
        public IActionResult Create([FromBody] Book newBook)
        {
            var book = BookList.SingleOrDefault(x => x.Title == newBook.Title);

            if (book is not null)
                return BadRequest();


            BookList.Add(newBook);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Book updatedBook)
        {
            var book = BookList.SingleOrDefault(x => x.Id == id);

            if (book is null)
                return BadRequest();

            book.GenreId = updatedBook.GenreId != default ? updatedBook.GenreId : book.GenreId;
            book.Title = updatedBook.Title != default ? updatedBook.Title : book.Title;
            book.PageCount = updatedBook.PageCount != default ? updatedBook.PageCount : book.PageCount;
            book.PublishDate = updatedBook.PublishDate != default ? updatedBook.PublishDate : book.PublishDate;

            return Ok();

        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = BookList.SingleOrDefault(x => x.Id == id);
            if (book is null)
                return BadRequest();

            BookList.Remove(book);
            return Ok();
        }

        [HttpGet("list")]
        public ActionResult<IEnumerable<Book>> ListBooks([FromQuery] string name)
        {
            var books = BookList.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                books = books.Where(b => b.Title.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(books.ToList());
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Book> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            var book = BookList.SingleOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();

            patchDoc.ApplyTo(book, (error) => ModelState.AddModelError(string.Empty, error.ErrorMessage));

            if (!TryValidateModel(book))
                return BadRequest(ModelState);


            return NoContent();

        }

    }
}
