using WebAPITemplate.BusinessModel;

namespace WebAPITemplate.Interface
{
    public interface ISalesService
    {
        Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetTotalSales();
        Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMonthlySales();
        Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetMostPopularItemEachMonth();
        Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetTopRevenueItemsEachMonth();
        Task<(bool IsSuccess, ClsReturnResult clsReturnResult)> GetPopularItemStats();
    }
}
