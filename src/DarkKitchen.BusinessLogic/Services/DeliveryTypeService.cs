using DarkKitchen.Domain.Entities;
using DarkKitchen.IBusinessLogic.DTOs.Entry.DeliveryTypeDTOs;
using DarkKitchen.IBusinessLogic.DTOs.Exit.DeliveryTypeDTOs;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.IDataAccess.RepositoriesInterfaces;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class DeliveryTypeService(IRepository<DeliveryType> deliveryTypeRepository) : IDeliveryTypeService
{
    public DeliveryTypeExitDto Create(DeliveryTypeEntryDto dto)
    {
        if(deliveryTypeRepository.Exists(d => d.Name == dto.Name))
        {
            throw new ArgumentException($"A delivery type with name '{dto.Name}' already exists.");
        }

        var deliveryType = DeliveryType.Create(dto.Name, dto.ShippingCost);
        deliveryTypeRepository.Add(deliveryType);

        return ToExitDto(deliveryType);
    }

    public DeliveryTypeExitDto Update(int id, DeliveryTypeEntryDto dto)
    {
        var deliveryType = deliveryTypeRepository.Get(d => d.Id == id)
            ?? throw new KeyNotFoundException($"Delivery type {id} not found.");

        deliveryType.Update(dto.Name, dto.ShippingCost);
        deliveryTypeRepository.Update(deliveryType);

        return ToExitDto(deliveryType);
    }

    public List<DeliveryTypeExitDto> GetAll()
    {
        return deliveryTypeRepository.GetAll().Select(ToExitDto).ToList();
    }

    private static DeliveryTypeExitDto ToExitDto(DeliveryType d) => new(d.Id, d.Name, d.ShippingCost);
}
