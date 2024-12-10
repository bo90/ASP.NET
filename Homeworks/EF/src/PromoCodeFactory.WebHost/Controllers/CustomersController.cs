using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;

        public CustomersController(IRepository<Customer> customerRepository)
        {
            _customerRepository = customerRepository;
        }
        
        /// <summary>
        /// получить данные по всем клиентам
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {
            //TODO: Добавить получение списка клиентов
            var customers = await _customerRepository.GetAllAsync();
            var model = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
            }).ToList();
            return Ok(model); 
        }

        /// <summary>
        /// получить информацию по конкретному клиенту 
        /// </summary>
        /// <param name="id">ИД клиента</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            var model = new CustomerResponse()
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                /*PromoCodes = customer.Promocodes.Select(p => new PromoCodeShortResponse()
                {
                    Id = p.Id,
                    Code = p.Code,
                    ServiceInfo = p.ServiceInfo,
                    BeginDate = p.BeginDate.ToString("dd/MM/yyyy"),
                    EndDate = p.EndDate.ToString("dd/MM/yyyy"),
                    PartnerName = p.PartnerName
                }).ToList()*/
            };
            return Ok(model);
        }

        /// <summary>
        /// Создать нового клиента
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var newCustomer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Id = Guid.NewGuid(),
                CustomerPreferences = new List<CustomerPreference>()
                {
                    
                }
            };
            await _customerRepository.CreateNewRecordAsync(newCustomer);
            
            return Ok("Created");
        }

        /// <summary>
        /// Изменить данные клиента
        /// </summary>
        /// <param name="id">Ид клиент</param>
        /// <param name="request">новый экземпляр данных клиента</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            //ищем клиента в бд
            var customer = await _customerRepository.GetByIdAsync(id);
            if(customer == null)
                return NotFound();

            var changeCustomer = new Customer()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Id = id,
                CustomerPreferences = new List<CustomerPreference>()
            };
            await _customerRepository.UpdateRecordAsync(changeCustomer);
            return Ok("Updated");
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="id">Ид клиента</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if(customer == null)    
                return NotFound();
            await _customerRepository.DeleteRecordAsync(customer);
            return Ok("Deleted");
        }
    }
}