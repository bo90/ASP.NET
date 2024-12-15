using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Промокоды
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PromocodesController
        : ControllerBase
    {
        private readonly IRepository<PromoCode> _promoCodeRepository;

        public PromocodesController(IRepository<PromoCode> promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }
        
        /// <summary>
        /// Получить все промокоды
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PromoCodeShortResponse>>> GetPromocodesAsync()
        {
            var promos = await _promoCodeRepository.GetAllAsync();
            var promModel = promos.Select(x=> 
                new PromoCodeShortResponse()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ServiceInfo = x.ServiceInfo,
                    BeginDate = x.BeginDate.ToString("dd/MM/yyyy"),
                    EndDate = x.EndDate.ToString("dd/MM/yyyy"),
                    PartnerName = x.PartnerName,
                }).ToList();
            return promModel;
        }

        /// <summary>
        /// Создать промокод и выдать его клиентам с указанным предпочтением
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            PromoCode pCode = new PromoCode()
            {
                Code = request.PromoCode,
                ServiceInfo = request.ServiceInfo,
                PartnerName = request.PartnerName,
                Preference = new Preference(),
            };
            await _promoCodeRepository.CreateNewRecordAsync(pCode);
            return Ok(pCode);
        }
    }
}