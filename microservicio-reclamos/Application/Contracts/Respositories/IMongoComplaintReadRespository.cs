using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Respositories;

public interface IMongoComplaintReadRespository
{
    Task<HttpStatusCode> AddComplaint(Complaint complaint);
    Task<List<Complaint>> GetComplaintsByStatus(ComplaintStatus status);
    Complaint GetComplaintById(Guid complaintId);
    Task UpdateComplaint(Complaint complaint);
    Task<List<Complaint>> GetComplaintsByUserId(Guid UserId);

}
