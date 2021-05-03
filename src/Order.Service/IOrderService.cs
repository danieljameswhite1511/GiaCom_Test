using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderSummary>> GetOrdersAsync();
        
        Task<OrderDetail> GetOrderByIdAsync(Guid orderId);

        Task<IEnumerable<OrderSummary>> GetOrdersByStatusAsync(string statusName);

        Task<List<OrderDetail>> GetOrderDetailsAsync();

        Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail);

        Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail);

        Task<List<OrderMonthlyProfitSummary>> MonthlyProfits();
    }

}
