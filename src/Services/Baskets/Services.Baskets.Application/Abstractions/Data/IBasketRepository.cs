using Services.Baskets.Domain.Entities;

namespace Services.Baskets.Application.Abstractions.Data; 
public interface IBasketRepository
{
    Task<IReadOnlyList<Basket>> GetAllBasketsAsync();
    Task<Basket> GetBasketByIdAsync(string basketId);
    Task<Basket> GetBasketByOrganizationIdAsync(string organizationId);
    Task<Basket> UpdateBasketAsync(Basket basket);
    Task<bool> DeleteBasketAsync(string basketId);
}
