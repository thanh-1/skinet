using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, opt => opt.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, opt => opt.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, opt => opt.MapFrom<ProductUrlResolver>());

            // No need to add more config, AutoMapper can map since properties name are the same
            // Also map the other way around
            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<CustomerBasketDto, CustomerBasket>();

            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, opt => opt.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice, opt => opt.MapFrom(s => s.DeliveryMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl, opt => opt.MapFrom<OrderItemUrlResolver>());
        }
    }
}