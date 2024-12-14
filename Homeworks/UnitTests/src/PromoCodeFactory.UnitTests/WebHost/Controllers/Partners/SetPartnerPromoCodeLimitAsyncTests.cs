using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly PartnersController _partnersController;
        private readonly SetPartnerPromoCodeLimitRequest _setPartnerPromoCodeLimitRequest;
        private readonly Mock<IRepository<Partner>> _mockRepository;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mockRepository = fixture.Freeze<Mock<IRepository<Partner>>>();
            _setPartnerPromoCodeLimitRequest = fixture.Freeze<SetPartnerPromoCodeLimitRequest>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        public Partner GetPaetnerForTest()
        {
            return new Partner()
            {
                Id = Guid.NewGuid(),
                Name = "Parner Name",
                IsActive = true,
                NumberIssuedPromoCodes = 5,
                PartnerLimits = new List<PartnerPromoCodeLimit>()
                {
                    new PartnerPromoCodeLimit()
                    {
                        Id = Guid.NewGuid(),
                        CreateDate = new DateTime(2017, 1, 1),
                        EndDate = new DateTime(2017, 12, 31),
                        Limit = 10
                    }
                }
            };
        }

        /// <summary>
        /// Если партнер не найден, то также нужно выдать ошибку 404;
        /// </summary>
        [Fact]
        public async Task GetParner_IsEmpty_NotFound()
        {
            //Arrange
            var id = Guid.NewGuid();
            Partner p = null;
            
            //Act
            _mockRepository.Setup(r=>r.GetByIdAsync(id)).ReturnsAsync((p));
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(id, _setPartnerPromoCodeLimitRequest);

            //Assert 
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        /// <summary>
        /// Если партнер заблокирован, то есть поле IsActive=false в классе Partner, то также нужно выдать ошибку 400;
        /// </summary>
        [Fact]
        public async Task PartnerIsLock_IfActiveFalse_return400code()
        {
            //Arrange
            var patner = GetPaetnerForTest();
            patner.IsActive = false;
            _mockRepository.Setup(r => r.GetByIdAsync(patner.Id)).ReturnsAsync(patner);
            
            //Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(patner.Id, _setPartnerPromoCodeLimitRequest);
            
            //Assert 
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        /// <summary>
        /// Если партнеру выставляется лимит, то мы должны обнулить количество промокодов,
        /// которые партнер выдал NumberIssuedPromoCodes, если лимит закончился, то количество не обнуляется;
        /// </summary>
        [Fact]
        public async Task UpdateLimit_IfNewLimitPromoCodeParnter()
        {
            //Arrange
            var partner = GetPaetnerForTest();
            partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue).EndDate = DateTime.Now;
            _mockRepository.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            
            //Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, _setPartnerPromoCodeLimitRequest);
            
            //Assert 
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }
        
        //При установке лимита нужно отключить предыдущий лимит;
        [Fact]
        public async Task SwitchOffLimit_If_InsatallNewLimit()
        {
            //Arrange
            var partner = GetPaetnerForTest();
            partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue).EndDate = DateTime.Now.AddDays(-1);
            _mockRepository.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            
            //Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, _setPartnerPromoCodeLimitRequest);
            
            //Assert
            partner.NumberIssuedPromoCodes.Should().NotBe((int)1);
        }
        
        //Лимит должен быть больше 0;
        [Fact]
        public async Task InstallPersonLimitPromocode_is_null()
        {
            //Arrange
            var partner = GetPaetnerForTest();
            _mockRepository.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            
            //Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, _setPartnerPromoCodeLimitRequest);
            
            //Assert
            partner.PartnerLimits.FirstOrDefault(x=>x.CancelDate.HasValue).Should().NotBeNull();
        }
        
        //Нужно убедиться, что сохранили новый лимит в базу данных
        [Fact]
        public async Task SetNewLimitFromDataBase()
        {
            //Arrange
            var partner = GetPaetnerForTest();
            _mockRepository.Setup(r => r.GetByIdAsync(partner.Id)).ReturnsAsync(partner);
            
            //Act 
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, _setPartnerPromoCodeLimitRequest);
            
            //Assert
            _mockRepository.Object.GetByIdAsync(partner.Id).Result
                .PartnerLimits.FirstOrDefault(l => l.Id.Equals(((CreatedAtActionResult)result).RouteValues["limitId"]))
                .Should().BeEquivalentTo(_setPartnerPromoCodeLimitRequest, options => options.Including(x => x.EndDate).Including(x => x.Limit));
        }
    }
}