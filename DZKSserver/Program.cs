var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDB>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLliteUsers"));
});
builder.Services.AddDbContext<ArchiveDB>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLlite"));
});

builder.Services.AddCors();
builder.Services.AddControllers();

var app = builder.Build(); // у объекта билдер вызываем создаение веб приложения

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllerRoute(
            name: "letterController",
            pattern: "letters/"
            );
app.MapControllerRoute(
            name: "userController",
            pattern: "users/"
            );
app.MapControllerRoute(
            name: "archiveController",
            pattern: "archive/"
            );

if (app.Environment.IsDevelopment())
{
    StaticParametrs.uri = "http://localhost:5224";

    using var scope = app.Services.CreateScope();
    var archiveDB = scope.ServiceProvider.GetRequiredService<ArchiveDB>();
    var userDB = scope.ServiceProvider.GetRequiredService<UserDB>();
    archiveDB.Database.EnsureCreated();

    if (userDB.Database.EnsureCreated())
    {
        Excel excelFile = new Excel(Directory.GetCurrentDirectory() +"\\passwords.xlsx", 2);
        string excel_Add_Users = "excel_Add_Users:\t";
        Console.WriteLine(excel_Add_Users + excelFile.GetRowCount());
        Console.WriteLine(excel_Add_Users+"Start adding users.");
        for(int i = 0, login = 2, password = 3; i < excelFile.GetRowCount(); i++)
        {
            if(excelFile.ReadCell(i, login) == "Логин")
            {
                i++;
                if ((excelFile.ReadCell(i, login) == "")) continue;
                else
                {
                    User user = new User();
                    user.login = excelFile.ReadCell(i, login);
                    user.password = excelFile.ReadCell(i, password);
                    userDB.users.Add(user);
                }
            }
        }
        Console.WriteLine(excel_Add_Users + "End of adding users.");
        await userDB.SaveChangesAsync();
    }
}
if (app.Environment.IsProduction())
{
    StaticParametrs.uri = "http://localhost:5000";

    using var scope = app.Services.CreateScope();
    var archiveDB = scope.ServiceProvider.GetRequiredService<ArchiveDB>();
    var userDB = scope.ServiceProvider.GetRequiredService<UserDB>();
    archiveDB.Database.EnsureCreated();

    if (userDB.Database.EnsureCreated())
    {
        Excel excelFile = new Excel(Directory.GetCurrentDirectory() + "\\passwords.xlsx", 2);
        string excel_Add_Users = "excel_Add_Users:\t";
        Console.WriteLine(excel_Add_Users + excelFile.GetRowCount());
        Console.WriteLine(excel_Add_Users + "Start adding users.");
        for (int i = 0, login = 2, password = 3; i < excelFile.GetRowCount(); i++)
        {
            if (excelFile.ReadCell(i, login) == "Логин")
            {
                i++;
                if ((excelFile.ReadCell(i, login) == "")) continue;
                else
                {
                    User user = new User();
                    user.login = excelFile.ReadCell(i, login);
                    user.password = excelFile.ReadCell(i, password);
                    userDB.users.Add(user);
                }
            }
        }
        Console.WriteLine(excel_Add_Users + "End of adding users.");
        await userDB.SaveChangesAsync();
    }
}

app.MapGet("/startDateCheck", async (UserDB db, ArchiveDB archiveDB, HttpResponse response) =>
{
    var letters = await db.letters.ToListAsync();
    bool findOne = false;
    foreach (var letter in letters)
    {
        if ((DateTime.Now.Date - DateTime.Parse(letter.date)).Days >= 4)
        {
            if (!letter.status)
            {
                db.letters.Remove(letter);
                await archiveDB.letters.AddAsync(letter);
                findOne = true;
            }
        }
    }
    
    await db.SaveChangesAsync();
    await archiveDB.SaveChangesAsync();
    if (findOne) response.StatusCode = StatusCodes.Status200OK;
    else response.StatusCode = StatusCodes.Status404NotFound;
});
app.MapGet("/", async (HttpResponse response) => { 
    await response.WriteAsync("Server is working."); });
app.MapGet("/wallpaper", async (HttpResponse response) =>
{
    if (File.Exists("wallpaper.jpg")) { response.StatusCode = StatusCodes.Status200OK; await response.SendFileAsync("wallpaper.jpg"); }
    else response.StatusCode = StatusCodes.Status404NotFound;
});
app.MapGet("/readme", async (HttpResponse response) =>
{
    if (File.Exists("View/html/readme.html")) { response.ContentType = "text/html; charset=utf-8"; await response.SendFileAsync("View/html/readme.html"); }
    else response.StatusCode = StatusCodes.Status404NotFound;
});

app.MapPost("/setMessage:{message}",  (string message, HttpResponse response) =>
{
    StaticParametrs.messageForApp = message;
    response.StatusCode = StatusCodes.Status204NoContent;
});

//Templates
app.MapGet("/d", async (HttpResponse response) => { 
    await response.WriteAsJsonAsync(new Letter()); 
}); // getter for check is worked or not
app.MapGet("/userTemplate", async (HttpResponse response) =>
{
    await response.WriteAsJsonAsync(new User()); 
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[{DateTime.Now}] server({context.Request.Method} request to {context.Request.Path}):\t" +
        $"Response status code {context.Response.StatusCode}");
    Console.ForegroundColor = ConsoleColor.White;

    await next.Invoke();
});


MessageSender.Open();
System.Diagnostics.Process.Start($"{Directory.GetCurrentDirectory()}\\ServerJob\\ServerJobs.exe", StaticParametrs.uri);
app.Run();  // запуск веб-приложения