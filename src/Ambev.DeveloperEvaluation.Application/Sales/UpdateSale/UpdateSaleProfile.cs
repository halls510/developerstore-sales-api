using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for Sale updating mappings.
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        // Mapping from UpdateSaleCommand to Sale
        CreateMap<UpdateSaleCommand, Sale>();
        //CreateMap<UpdateSaleCommand, Sale>()
        //    .ConstructUsing(src => new Sale(
        //        src.SaleNumber,
        //        src.CustomerId,
        //        src.CustomerName,
        //        src.Branch,
        //        new List<SaleItem>()
        //    ));

        // Mapping from SaleItemDto to SaleItem
        CreateMap<SaleItemDto, SaleItem>();
        //CreateMap<SaleItemDto, SaleItem>()
        //    .ConstructUsing(src => new SaleItem(
        //        0, // SaleId será definido após a venda ser criada
        //        src.ProductId,
        //        src.Quantity
        //    ));

        // Mapping from Sale to UpdateSaleResult
        CreateMap<Sale, UpdateSaleResult>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue))
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))            
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items ?? new List<SaleItem>()));

        // Mapping from SaleItem to SaleItemResult
        CreateMap<SaleItem, SaleItemResult>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
