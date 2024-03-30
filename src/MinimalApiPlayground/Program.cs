using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using MinimalApiPlayground.Models;

//NuGet: Microsoft.AspNetCore.OpenApi v8.0.0
//NuGet: Swashbuckle.AspNetCore v6.5.0

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen(); //=> Swashbuckle.AspNetCore v6.5.0

builder.Services.AddSingleton<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger(); //=> Swashbuckle.AspNetCore v6.5.0
    app.UseSwaggerUI(); //=> Swashbuckle.AspNetCore v6.5.0
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Example 1: Hello World!
//app.MapGet("/", () => "Hello, World!");

//Example 2: Fetching All Books
app.MapGet("/books", (IBookService bookService) =>
    TypedResults.Ok(bookService.GetBooks()))
    .WithName("GetBooks")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Library Books",
        Description = "Returns information about all the available books from the Amy's library.",
        Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
    });

//Example 3: Fetching a Specific Book by ID
app.MapGet("/books/{id}", Results<Ok<Book>, NotFound> (IBookService bookService, int id) =>
        bookService.GetBook(id) is { } book
            ? TypedResults.Ok(book)
            : TypedResults.NotFound()
    )
    .WithName("GetBookById")
    .WithOpenApi(x => new OpenApiOperation(x)
    {
        Summary = "Get Library Book By Id",
        Description = "Returns information about selected book from the Amy's library.",
        Tags = new List<OpenApiTag> { new() { Name = "Amy's Library" } }
    });

app.Run();
