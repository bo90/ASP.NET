using System;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.WebHost.Models;

public class PreferenceResponse 
{
    /// <summary>
    /// ИД
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }
}