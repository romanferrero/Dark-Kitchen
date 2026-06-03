using DarkKitchen.Domain.Enums;
using DarkKitchen.IBusinessLogic.IServices;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Mappers;
using DarkKitchen.WebApi.Models.Request.DeliveryTypesModels;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.DeliveryTypesControllers;

[ApiController]
[Route("api/delivery-types")]
public class DeliveryTypesController(IDeliveryTypeService deliveryTypeService) : ControllerBase
{
    [HttpPost]
    [AuthorizationFilter(Permission.ManageDeliveryTypes)]
    public IActionResult Create(DeliveryTypeRequestModel request)
    {
        var result = deliveryTypeService.Create(DeliveryTypeMapper.ToDto(request));
        return CreatedAtAction(nameof(Create), null, DeliveryTypeMapper.ToResponse(result));
    }

    [HttpPut("{id:int}")]
    [AuthorizationFilter(Permission.ManageDeliveryTypes)]
    public IActionResult Update(int id, DeliveryTypeRequestModel request)
    {
        var result = deliveryTypeService.Update(id, DeliveryTypeMapper.ToDto(request));
        return Ok(DeliveryTypeMapper.ToResponse(result));
    }

    [HttpGet]
    [AuthorizationFilter(Permission.ViewDeliveryTypes)]
    public IActionResult GetAll()
    {
        var result = deliveryTypeService.GetAll();
        return Ok(result.Select(DeliveryTypeMapper.ToResponse).ToList());
    }

    [HttpDelete("{id:int}")]
    [AuthorizationFilter(Permission.ManageDeliveryTypes)]
    public IActionResult Delete(int id)
    {
        deliveryTypeService.Delete(id);
        return NoContent();
    }
}
