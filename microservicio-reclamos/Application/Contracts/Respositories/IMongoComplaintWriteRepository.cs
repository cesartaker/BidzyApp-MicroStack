using System.Net;
using Domain.Entities;

namespace Application.Contracts.Respositories;

public interface IMongoComplaintWriteRepository
{
    Task<HttpStatusCode> AddComplaint(Complaint complaint);
    Task UpdateComplaint(Complaint complaint);
}
