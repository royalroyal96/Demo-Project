var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// In-memory data store (for demo purposes)
var items = new List<TodoItem>();
int nextId = 1;

// CRUD Endpoints

// Create
app.MapPost("/api/todo", (TodoItem item) =>
{
    item.Id = nextId++;
    items.Add(item);
    return Results.Created($"/api/todo/{item.Id}", item);
})
.WithName("CreateTodo")
.WithOpenApi();

// Read (all)
app.MapGet("/api/todo", () =>
{
    return Results.Ok(items);
})
.WithName("GetAllTodos")
.WithOpenApi();

// Read (single)
app.MapGet("/api/todo/{id}", (int id) =>
{
    var item = items.FirstOrDefault(x => x.Id == id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
})
.WithName("GetTodoById")
.WithOpenApi();

// Update
app.MapPut("/api/todo/{id}", (int id, TodoItem updatedItem) =>
{
    var item = items.FirstOrDefault(x => x.Id == id);
    if (item is null) return Results.NotFound();

    item.Title = updatedItem.Title;
    item.IsComplete = updatedItem.IsComplete;
    return Results.NoContent();
})
.WithName("UpdateTodo")
.WithOpenApi();

// Delete
app.MapDelete("/api/todo/{id}", (int id) =>
{
    var item = items.FirstOrDefault(x => x.Id == id);
    if (item is null) return Results.NotFound();

    items.Remove(item);
    return Results.NoContent();
})
.WithName("DeleteTodo")
.WithOpenApi();

// TodoItem model
record TodoItem(int Id, string Title, bool IsComplete);

app.Run();