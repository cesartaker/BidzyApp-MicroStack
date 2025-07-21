namespace Producto.API.Requests;

public record RegisterProductRequest(string Name, string Description, decimal BasePrice,
    string Category, IFormFile Image);
