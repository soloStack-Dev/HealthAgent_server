using MediAgent.Api.Models.Requests;
using MediAgent.Api.Models.Responses;

namespace MediAgent.Api.Services;

public interface IHealthAgentService
{
    Task<SymptomAnalysisResponse> AnalyzeSymptomsAsync(string userId, SymptomAnalysisRequest request);
}
