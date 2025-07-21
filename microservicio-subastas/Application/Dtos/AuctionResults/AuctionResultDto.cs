using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.AuctionResults;

public class AuctionResultDto
{

    // Auction
    public Guid AuctionId { get; set; }
    public string Tittle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    public string ImageUrl { get; set; }

    // Producto subastado
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }


    // Información de cierre
    public decimal HighestBid { get; set; }

    // Datos del ganador
    public Guid? WinnerId { get; set; }
    public string? WinnerName { get; set; }

    //Datos de participación
    public bool IsParticipated { get; set; }

}


