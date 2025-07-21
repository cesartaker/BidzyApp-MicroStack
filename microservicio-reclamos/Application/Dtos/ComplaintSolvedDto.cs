using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dtos;

public class ComplaintSolvedDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; }
    public string Description { get; set; }
    public string EvidenceUrl { get; set; }
    public string Solution { get; set; }
    public DateTime SolvedAt { get; set; }
    public ComplaintStatus Status { get; set; }
}
