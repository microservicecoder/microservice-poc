using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    /*
     Note: this Discount service class is used to expose the grpc endpoints outside.
    In order to do that:
                    a) we need to inherit the GRPC protos generated by VS. This step will
    make this class grpc service class.
                    b) this class is used to expose the CRUD operations for discount.
                    c) this class is similar to api controller. To make a controller as API
    controller we inherit from "ControllerBase" like wise to make a class as a grpc service
    class we will inherit from "DiscountProtoService.DiscountProtoServiceBase".
                    d) DiscountProtoService is from discount.proto file [name space Discount.Grpc.Protos]
    which will consist the "DiscountProtoServiceBase".
     */
    public class DiscountService: DiscountProtoService.DiscountProtoServiceBase
    {
        //create the variables for dependency injection.

        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        //generate the constructor
        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger,IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /*
         Override the method:

        a) Now we will override the protobuff methods, by this way we will expose the methods 
        outside.
        b) type override and click space, we will see the list of protobuff methods. 
        c) select the methods and click, to geneare the override method.
        d) convert the method into async.
         */
        
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            /* 
            we will not use the base method, as this base method will return the 
            empty grpc response.
            */

            //return base.GetDiscount(request, context);

            /*
            Our code:
             
            GetDiscountRequest consist of product name as param (request.ProductName).
            [check the request message definition from discount.proto file]
            */

            var coupon = await _repository.GetDiscount(request.ProductName);

            if (coupon == null)
            {
                //to throw the rpc exception
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
            }
            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            //automapper is required as GetDiscount Method return the result of type,  grpc coupon model.
            //to convert the grpc coupon model into our entity class we use automapper.
            var couponModel= _mapper.Map<CouponModel>(coupon);
            return couponModel;

        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _repository.CreateDiscount(coupon);
            _logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);

            await _repository.UpdateDiscount(coupon);
            _logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

            var couponModel = _mapper.Map<CouponModel>(coupon);
            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
