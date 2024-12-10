using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения клиента
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class PreferenceController : ControllerBase
{
    private readonly IRepository<Preference> _preferenceRepository;

    public PreferenceController(IRepository<Preference> preferenceRepository)
    {
        _preferenceRepository = preferenceRepository;
    }

    public async Task<List<PreferenceResponse>> GetAllPreferences()
    {
        var pref = await _preferenceRepository.GetAllAsync();
        var prefModel = pref.Select(x=>
            new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name,
            }).OrderBy(x=>x.Name).ToList();
        return prefModel;
    }
}