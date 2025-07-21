using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Contracts.Services;

public interface IComplaintService
{
    /// <summary>
    /// Creates a new complaint.
    /// </summary>
    /// <param name="userId">The ID of the user making the complaint.</param>
    /// <param name="reason">The reason for the complaint.</param>
    /// <param name="description">A detailed description of the complaint.</param>
    /// <param name="evidence">Optional evidence file related to the complaint.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Complaint> RegisterComplaintAsync(Guid userId, string reason, string description, IFormFile? evidence = null);
    Task<List<ComplaintCreatedDto>> GetComplaintsAsync(ComplaintStatus status);
    Task<Complaint> UpdateComplaintAsync(Guid complaintId, string solution);
    Task SendSolveComplaintNotification(Complaint complaint);
    Task<List<Complaint>> GetComplaintsByUserId(Guid userId);
}
