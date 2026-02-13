using netcrud.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Built-in OpenAPI generator (keep this)
builder.Services.AddOpenApi();

builder.Services.AddScoped<IProductRepository, ProductRepository>();

// ────────────────────────────────────────────────
// Optional: customize OpenAPI document if needed
// builder.Services.AddOpenApi(options =>
// {
//     options.AddDocumentTransformer(...);
// });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();           // serves /openapi/v1.json

    // ────────────────────────────────────────────────
    // Add Swagger UI – points to the built-in OpenAPI endpoint
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
        // Optional: nicer defaults
        options.RoutePrefix = "swagger";           // → /swagger
        options.DocumentTitle = "My Product API";
        // options.DefaultModelsExpandDepth(-1);   // hide schemas by default, etc.
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();