using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.PromotionDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.ProductDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.PromotionDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class PromotionService(IPromotionRepository promotionRepository, IProductRepository productRepository)
    : IPromotionService
{
    public PromotionExitDto CreatePromotion(CreatePromotionEntryDto dto)
    {
        var promotion = Promotion.Create(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Add(promotion);

        return ToExitDTO(promotion);
    }

    public PromotionExitDto UpdatePromotion(UpdatePromotionEntryDto dto)
    {
        var promotion = promotionRepository.Get(p => p.Id == dto.Id)
                        ?? throw new KeyNotFoundException($"Promotion {dto.Id} not found");

        promotion.Update(dto.Name, dto.Discount, dto.DateFrom, dto.DateTo);

        promotionRepository.Update(promotion);

        return ToExitDTO(promotion);
    }

    public ProductExitDto AddProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = productRepository.Get(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found");

        promotion.AddProduct(product);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public ProductExitDto RemoveProduct(int promotionId, string productCode)
    {
        var promotion = promotionRepository.Get(p => p.Id == promotionId)
                        ?? throw new KeyNotFoundException($"Promotion {promotionId} not found");

        var product = promotion.Products.FirstOrDefault(p => p.Code == productCode)
                      ?? throw new KeyNotFoundException($"Product {productCode} not found in promotion");

        promotion.RemoveProduct(productCode);
        promotionRepository.Update(promotion);
        return ToExitDTO(product);
    }

    public List<PromotionExitDto> GetPromotions(DateOnly? date, string? line, string? product)
    {
        var allPromotions = promotionRepository.GetFiltered();

        var result = new List<PromotionExitDto>();
        foreach(var promotion in allPromotions)
        {
            if(!MatchesDate(promotion, date))
            {
                continue;
            }

            if(!HasProductInLine(promotion, line))
            {
                continue;
            }

            if(!HasProductMatching(promotion, product))
            {
                continue;
            }

            result.Add(ToExitDTO(promotion));
        }

        return result;
    }

    private static bool MatchesDate(Promotion promotion, DateOnly? date)
    {
        if(!date.HasValue)
        {
            return true;
        }

        return promotion.DateFrom <= date.Value && promotion.DateTo >= date.Value;
    }

    private static bool HasProductInLine(Promotion promotion, string? line)
    {
        if(string.IsNullOrEmpty(line))
        {
            return true;
        }

        foreach(var product in promotion.Products)
        {
            if(product.Line == line)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasProductMatching(Promotion promotion, string? search)
    {
        if(string.IsNullOrEmpty(search))
        {
            return true;
        }

        var searchLower = search.ToLower();
        foreach(var product in promotion.Products)
        {
            var matchesByCode = product.Code == search;
            var matchesByName = product.Name.ToLower().Contains(searchLower);

            if(matchesByCode || matchesByName)
            {
                return true;
            }
        }

        return false;
    }

    private static PromotionExitDto ToExitDTO(Promotion promotion)
    {
        return new PromotionExitDto
        {
            Id = promotion.Id,
            Name = promotion.Name,
            DiscountPercentage = promotion.DiscountPercentage,
            DateFrom = promotion.DateFrom,
            DateTo = promotion.DateTo,
            Products = [.. promotion.Products.Select(p => p.Code)]
        };
    }

    private static ProductExitDto ToExitDTO(Product product)
    {
        return new ProductExitDto
        {
            Code = product.Code,
            Name = product.Name,
            Price = product.Price,
            Line = product.Line,
            Category = product.Category,
            ImageUrls = [.. product.Images.Select(i => i.Url)]
        };
    }
}
