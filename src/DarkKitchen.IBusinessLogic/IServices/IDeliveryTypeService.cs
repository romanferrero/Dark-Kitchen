using DarkKitchen.IBusinessLogic.DTOs.Entry;
using DarkKitchen.IBusinessLogic.DTOs.Exit;

namespace DarkKitchen.IBusinessLogic.IServices;

public interface IDeliveryTypeService
{
    DeliveryTypeExitDto Create(DeliveryTypeEntryDto dto);
    DeliveryTypeExitDto Update(int id, DeliveryTypeEntryDto dto);
    List<DeliveryTypeExitDto> GetAll();
    void Delete(int id);
}
