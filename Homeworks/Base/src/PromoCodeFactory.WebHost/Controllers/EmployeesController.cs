using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Создать новую запись сотрудника
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [HttpPost(Name = "AddNewEmployee")]
        public async Task<ActionResult> AddNewEmployee(Employee emp)
        {
            if(emp == null)
                return NotFound();

            var empExists = _employeeRepository.GetByIdAsync(emp.Id);
            if(empExists.Result != null) 
                return BadRequest($"Employee exists");

            if (emp.Id.Equals(Guid.Empty))
            {
                emp.Id = Guid.NewGuid();
                emp.Roles = new List<Role>()
                {
                    new Role()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Test",
                        Description = "TestDescription"
                    }
                };
            }
            
            await _employeeRepository.CreateNewEmployeeAsync(emp);
            return Ok(emp);
        }
        
        /// <summary>
        /// Обновить запись сотрудника
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [HttpPut(Name = "UpdateEmployee")]
        public async Task<ActionResult> UpdateEmployee(Employee emp)
        {
            if(emp == null)
                return NotFound();

            var checkEmp = await _employeeRepository.GetByIdAsync(emp.Id);
            if(checkEmp == null)
                return NotFound();
            
            await _employeeRepository.UpdateEmployeeAsync(emp);
            return Ok();
        }

        /// <summary>
        /// Удалить запись сотрудника по его ID 
        /// </summary>
        /// <param name="id">GUID ID</param>
        /// <returns></returns>
        [HttpDelete("{id:guid}", Name = "DeleteEmployee")]
        public async Task<ActionResult> DeleteEmployee(Guid id)
        {
            var emp = await _employeeRepository.GetByIdAsync(id);
            if(emp == null)
                return NotFound();

            await _employeeRepository.DeleteEmployeeAsync(id);
            return Ok();
        }
    }
}