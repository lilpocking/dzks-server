namespace DZKSserver.Controllers
{
    [Route("/archive")]
    [Controller]
    public class ArchiveController : ControllerBase
    {
        //GET response to /archive
        [HttpGet]
        public async void GetArchiveLetters([FromServices] ArchiveDB db)
        {
            await Response.WriteAsJsonAsync(await db.letters.ToListAsync());
        }

        //GET request to //{id}
        [HttpGet("{id:int}")]
        public async void GetArchiveLetterById(int id,[FromServices] ArchiveDB db)
        {
            var letterFromArchive = await db.letters.FirstOrDefaultAsync(h => h.id == id);
            if (letterFromArchive == null) Response.StatusCode = StatusCodes.Status404NotFound;
            else
            {
                await Response.WriteAsJsonAsync(letterFromArchive);
            }
        }

        //DELETE request to /letter/{id}
        [HttpDelete("{id:int}")]
        public async void DeleteArchiveLetterById(int id, [FromServices] ArchiveDB db)
        {
            var letterFromArchive = await db.letters.FirstOrDefaultAsync(h => h.id == id);
            if (letterFromArchive == null) Response.StatusCode = StatusCodes.Status404NotFound;
            else
            {
                db.letters.Remove(letterFromArchive);
                await db.SaveChangesAsync();
            }
        }
    }
}
