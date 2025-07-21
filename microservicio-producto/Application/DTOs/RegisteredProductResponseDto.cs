using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs;

public class RegisteredProductResponseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public string Category { get; set; }
    public string ImageUrl { get; set; }
    
    public RegisteredProductResponseDto(Product product) { 
        
        Name = product.Name;
        Description = product.Description;
        BasePrice = product.BasePrice;
        Category = product.Category;
        ImageUrl = product.ImageUrl;  
    }
}
