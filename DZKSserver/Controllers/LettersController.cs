namespace DZKSserver.Controllers
{
    [Route("letters")]
    [Controller]
    public class LettersController : ControllerBase
    {
        //GET request to /letters
        [HttpGet]
        public async void GetAllLetters([FromServices] UserDB db)
        {
            await Response.WriteAsJsonAsync(await db.letters.ToListAsync());
        }

        //GET request to /letters/{id}
        [HttpGet("{id:int}")]
        public async void GetLetterById(int id, [FromServices] UserDB db)
        {
            if (db.letters.Count() == 0) { Response.StatusCode = StatusCodes.Status404NotFound; }
            var letterFromDb = await db.letters.FindAsync(new object[] { id });
            if (letterFromDb == null) { Response.StatusCode = StatusCodes.Status404NotFound; }
            await Response.WriteAsJsonAsync(letterFromDb);
        }

        //POST request to /letters/user:{userId:int}
        [HttpPost("user:{userId:int}")]
        public async void AddLetterToUser(int userId, [FromBody] Letter info, [FromServices] UserDB userDB)
        {
            var user = await userDB.users.FirstOrDefaultAsync(x => x.userId == userId);
            if (user == null || info == null) { Response.StatusCode = StatusCodes.Status400BadRequest; }
            else
            {
                user.letters.Add(info);
                await userDB.SaveChangesAsync();
                Response.StatusCode = StatusCodes.Status201Created;
            }
        }

        //POST to /letters
        [HttpPost]
        public async void CreateLetterOnDb([FromBody] Letter info, [FromServices] UserDB db)
        {
            await db.letters.AddAsync(info);
            await db.SaveChangesAsync();
            Response.StatusCode = StatusCodes.Status201Created;
        }

        //PUT request to /letters/id:{id:int}/status:{status:bool}
        [HttpPut("id:{id:int}/status:{status:bool}/inspect:{inspect:bool}")]
        public async void ChangeStatusForLetterWithId(int id, bool status, bool inspect, [FromServices] UserDB db, [FromServices] ArchiveDB archiveDB)
        {
            var infoFromDB = await db.letters.FindAsync(new object[] { id });
            if (infoFromDB == null) Response.StatusCode = StatusCodes.Status404NotFound;
            else
            {
                infoFromDB.inspect = inspect;
                infoFromDB.status = status;

                if (inspect)
                {
                    db.letters.Remove(infoFromDB);
                    await archiveDB.letters.AddAsync(infoFromDB);
                    await archiveDB.SaveChangesAsync();
                }
                await db.SaveChangesAsync();
                Response.StatusCode = StatusCodes.Status204NoContent;
            }
        }

        //DELETE request to /letter/{id}
        [HttpDelete("{id:int}")]
        public async void DeleteLetterFromDbById(int id, [FromServices] UserDB db)
        {
            var infoFromDB = await db.letters.FindAsync(new object[] {id});
            if (infoFromDB == null) Response.StatusCode = StatusCodes.Status404NotFound;
            else
            {
                db.letters.Remove(infoFromDB);
                await db.SaveChangesAsync();
                Response.StatusCode = StatusCodes.Status204NoContent;
            }
        }
    }
}