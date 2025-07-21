using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs;

public class GetProductsResponseDto
{
    public List <Product> Products { get; set; }

    public GetProductsResponseDto(List<Product> products)
    {
        Products = products;
    }
}
