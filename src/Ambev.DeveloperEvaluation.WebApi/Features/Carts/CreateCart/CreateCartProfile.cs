using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Profile for mapping between Application and API CreateCart responses
/// </summary>
public class CreateCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateCart feature
    /// </summary>
    public CreateCartProfile()
    {
        // Mapeia a requisição de criação do carrinho para o comando da aplicação
        CreateMap<CreateCartRequest, CreateCartCommand>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Products))
            .ReverseMap();

        // Mapeia a requisição de um item do carrinho para a entidade CartItem
        CreateMap<CartItemRequest, CartItem>()
            .ForMember(dest => dest.CartId, opt => opt.Ignore()) // O CartId será atribuído no contexto correto
            .ForMember(dest => dest.ProductName, opt => opt.Ignore()) // Nome do produto será buscado do repositório
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()); // Preço será preenchido do repositório

        // Mapeia a entidade CartItem para a resposta da API (opcional, mas útil)
        CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        // Mapeia CreateCartResult para CreateCartResponse (para retorno da API)
        CreateMap<CreateCartResult, CreateCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount));

        // Mapeia CartItemResult para CartItemResponse (para retorno da API)
        CreateMap<CartItemResult, CartItemResponse>()
             .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
