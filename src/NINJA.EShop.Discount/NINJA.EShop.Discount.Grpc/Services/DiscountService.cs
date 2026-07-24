using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;
using NINJA.EShop.Discount.Grpc.Data;
using NINJA.EShop.Discount.Grpc.Protos;
namespace NINJA.EShop.Discount.Grpc.Services
{
    public class DiscountService(DiscountContext discount,ILogger<DiscountService> logger)
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request,ServerCallContext context)
        {
            var coupon = await discount
                .Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon == null)
                return new CouponModel
                {
                    ProductName = request.ProductName,
                    Description = "No discount available",
                    Amount = 0
                };
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,ServerCallContext context)
        {
            var coupon = await discount.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.NotFound,$"Discount with ProductName={request.ProductName} is not found."));
            discount.Coupons.Remove(coupon);
            var result = await discount.SaveChangesAsync();
            if (result > 0)
                return new DeleteDiscountResponse { IsSuccess = true };
            return new DeleteDiscountResponse { IsSuccess = false };
        }
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request,ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Models.Coupon>();
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument,"Invalid coupon data."));
            discount.Coupons.Add(coupon);
            await discount.SaveChangesAsync();
            logger.LogInformation("Discount is successfully created. ProductName: {ProductName}",coupon.ProductName);
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request,ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Models.Coupon>();
            if (coupon == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument,"Invalid coupon data."));
            discount.Coupons.Update(coupon);
            await discount.SaveChangesAsync();
            logger.LogInformation("Discount is successfully updated. ProductName: {ProductName}",coupon.ProductName);
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
        public override async Task<GetDiscountListResponse> GetDiscountList(GetDiscountListRequest request,ServerCallContext context)
        {
            var coupons = await discount.Coupons.AsNoTracking().ToListAsync();
            var copuponModels = coupons.Adapt<List<CouponModel>>();
            return new GetDiscountListResponse { Coupons = { copuponModels } };
        }
    }
}