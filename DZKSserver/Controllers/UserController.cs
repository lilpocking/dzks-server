namespace DZKSserver.Controllers
{
    [Route("users")]
    [Controller]
    public class UserController : ControllerBase
    {
        //GET to /users
        [HttpGet]
        public async void GetUsers([FromServices] UserDB db)
        {
            await Response.WriteAsJsonAsync(db.users.Include(m=>m.letters).ToList());
        }

        //GET to /users/id:{userId}
        [HttpGet("id:{userId:int}")]
        public async void GetUserById(int userId, [FromServices] UserDB db)
        {
            var user = await db.users
                .Include(m => m.letters)
                .FirstOrDefaultAsync(x => x.userId == userId);
            await Response.WriteAsJsonAsync(user);
        }

        //GET to /users/id:{userId:int}/letters
        [HttpGet("id:{userId:int}/letters")]
        public async void GetAllLettersOfUser(int userId, [FromServices] UserDB db)
        {
            var user = db.users
                .Include(m=>m.letters)
                .FirstOrDefault(x => x.userId == userId);
            if (user == null) Response.StatusCode = StatusCodes.Status404NotFound;
            else
            {
                await Response.WriteAsJsonAsync(user.letters);
            }
        }

        //POST to /users/registration
        [HttpPost("registration")]
        public async void RegistrUser([FromBody] User user, [FromServices] UserDB db)
        {
            if (await db.users.CountAsync() != 0)
            {
                var checker = await db.users.FirstOrDefaultAsync(x=>x.login == user.login);
                if (checker == null)
                {
                    db.users.Add(user);
                    await db.SaveChangesAsync();
                    Response.StatusCode = StatusCodes.Status201Created;
                }
                else
                {
                    Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                }
            }
            else
            {
                db.users.Add(user);
                await db.SaveChangesAsync();
                Response.StatusCode = StatusCodes.Status201Created;
            }
        }
        
        //POST to /users/login
        [HttpPost("login")]
        public async void LoginCheck([FromBody] Loginization data, [FromServices] UserDB db)
        {
            dlaTupich tupich = new dlaTupich();

            var userFromDb = await db.users.FirstOrDefaultAsync(h => h.login == data.login);
            if (userFromDb == null) { tupich.resultCode = 0; await Response.WriteAsJsonAsync(tupich); }
            else
            {
                tupich.resultCode = (userFromDb.login == data.login)
                    ? (userFromDb.password == data.password ? 1 : 0)
                    : 0;

                // if resultCode == 1 then data is ok and return userId for GET responses there userID is needed parametr
                tupich.userId = (tupich.resultCode == 1)
                    ? userFromDb.userId
                    : -1;
                await Response.WriteAsJsonAsync(tupich);
            }

        }
    }
}
