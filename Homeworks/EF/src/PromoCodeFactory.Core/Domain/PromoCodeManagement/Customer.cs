using PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;

namespace PromoCodeFactory.Core.Domain.PromoCodeManagement
{
    public class Customer
        : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public int Age { get; set; }
        public string Email { get; set; }
        public List<CustomerPreference> CustomerPreferences { get; set; }
        public List<PromoCode> Promocodes { get; set; }

        //TODO: Списки Preferences и Promocodes 
    }
}