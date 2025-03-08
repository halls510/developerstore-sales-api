using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// Profile for mapping between Application and API UpdateCart responses
/// </summary>
public class UpdateCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateCart feature
    /// </summary>
    public UpdateCartProfile()
    {
        // Mapeia a requisição de criação do carrinho para o comando da aplicação
 CreateMap<UpdateCartRequest, UpdateCartCommand>()
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

 // Mapeia UpdateCartResult para UpdateCartResponse (para retorno da API)
 CreateMap<UpdateCartResult, UpdateCartResponse>()
     .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

 // Mapeia CartItemResult para CartItemResponse (para retorno da API)
 CreateMap<CartItemResult, CartItemResponse>()
     .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
     .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
