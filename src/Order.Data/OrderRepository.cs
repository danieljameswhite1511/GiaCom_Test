using Microsoft.EntityFrameworkCore;
using Order.Data.Entities;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;

        public OrderRepository(OrderContext orderContext)
        {
            _orderContext = orderContext;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var orderEntities = await _orderContext.Order
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var orders = orderEntities.Select(x => new OrderSummary
            {
                Id = new Guid(x.Id),
                ResellerId = new Guid(x.ResellerId),
                CustomerId = new Guid(x.CustomerId),
                StatusId = new Guid(x.StatusId),
                StatusName = x.Status.Name,
                ItemCount = x.Items.Count,
                TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = x.CreatedDate
            });

            return orders;
        }

           public async Task<IEnumerable<OrderSummary>> GetOrdersByStatusAsync(string status)
        {
            var orderEntities = await _orderContext.Order
                .Where(x=>x.Status.Name==status)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var orders = orderEntities.Select(x => new OrderSummary
            {
                Id = new Guid(x.Id),
                ResellerId = new Guid(x.ResellerId),
                CustomerId = new Guid(x.CustomerId),
                StatusId = new Guid(x.StatusId),
                StatusName = x.Status.Name,
                ItemCount = x.Items.Count,
                TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = x.CreatedDate
            });

            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var orderIdBytes = orderId.ToByteArray();

            var order = await _orderContext.Order.SingleOrDefaultAsync(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes );
            if (order == null)
            {
                return null;
            }

            var orderDetail = new OrderDetail
            {
                Id = new Guid(order.Id),
                ResellerId = new Guid(order.ResellerId),
                CustomerId = new Guid(order.CustomerId),
                StatusId = new Guid(order.StatusId),
                StatusName = order.Status.Name,
                CreatedDate = order.CreatedDate,
                TotalCost = order.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value,
                TotalPrice = order.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value,
                Items = order.Items.Select(x => new Model.OrderItem
                {
                    Id = new Guid(x.Id),
                    OrderId = new Guid(x.OrderId),
                    ServiceId = new Guid(x.ServiceId),
                    ServiceName = x.Service.Name,
                    ProductId = new Guid(x.ProductId),
                    ProductName = x.Product.Name,
                    UnitCost = x.Product.UnitCost,
                    UnitPrice = x.Product.UnitPrice,
                    TotalCost = x.Product.UnitCost * x.Quantity.Value,
                    TotalPrice = x.Product.UnitPrice * x.Quantity.Value,
                    Quantity = x.Quantity.Value
                })
            };

            return orderDetail;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync()
        {
            var orders = await _orderContext.Order.ToListAsync();
            
            if (orders == null)
            {
                return null;
            }

            var orderDetails = new List<OrderDetail>();
            foreach (var order in orders)
            {
                var orderDetail = new OrderDetail
                {
                    Id = new Guid(order.Id),
                    ResellerId = new Guid(order.ResellerId),
                    CustomerId = new Guid(order.CustomerId),
                    StatusId = new Guid(order.StatusId),
                    StatusName = order.Status.Name,
                    CreatedDate = order.CreatedDate,
                    TotalCost = order.Items.Sum(x => x.Quantity * x.Product.UnitCost).Value,
                    TotalPrice = order.Items.Sum(x => x.Quantity * x.Product.UnitPrice).Value,
                    Items = order.Items.Select(x => new Model.OrderItem
                    {
                        Id = new Guid(x.Id),
                        OrderId = new Guid(x.OrderId),
                        ServiceId = new Guid(x.ServiceId),
                        ServiceName = x.Service.Name,
                        ProductId = new Guid(x.ProductId),
                        ProductName = x.Product.Name,
                        UnitCost = x.Product.UnitCost,
                        UnitPrice = x.Product.UnitPrice,
                        TotalCost = x.Product.UnitCost * x.Quantity.Value,
                        TotalPrice = x.Product.UnitPrice * x.Quantity.Value,
                        Quantity = x.Quantity.Value
                    })
                };
                orderDetails.Add(orderDetail);
            }
            return orderDetails;
        }

    public async Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail)
    {
    var orderIdBytes = orderDetail.Id.ToByteArray();
    var orderDetailToUpdate = await _orderContext.Order.FirstOrDefaultAsync(x => _orderContext.Database.IsInMemory() ? x.Id.SequenceEqual(orderIdBytes) : x.Id == orderIdBytes);

        orderDetailToUpdate.StatusId=orderDetail.StatusId.ToByteArray();

        await _orderContext.SaveChangesAsync();

        orderDetail.StatusName=orderDetailToUpdate.Status.Name;

    return orderDetail;

    }

    public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
    {
      var order = new Order.Data.Entities.Order{
          Id=Guid.NewGuid().ToByteArray(),
          CreatedDate=DateTime.Now,
          CustomerId=orderDetail.CustomerId.ToByteArray(),
          ResellerId=orderDetail.ResellerId.ToByteArray(),
          StatusId=orderDetail.StatusId.ToByteArray()
      };

       await _orderContext.AddAsync(order);
       await _orderContext.SaveChangesAsync();

       orderDetail.Id = new Guid(order.Id);
       orderDetail.CreatedDate=order.CreatedDate;
       orderDetail.CustomerId = new Guid(order.CustomerId);
       orderDetail.ResellerId = new Guid(order.ResellerId);
       orderDetail.StatusId = new Guid(order.StatusId);

       return orderDetail;

    }

    public async Task<List<OrderMonthlyProfitSummary>> MonthlyProfits()
    {
        var status = "Completed";
        var orderEntities = await _orderContext.Order
                .Where(x=>x.Status.Name==status)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var orderMonthlyProfitDetails = orderEntities.Select(x => new OrderSummary
            {
                ItemCount = x.Items.Count,
                TotalCost = x.Items.Sum(i => i.Quantity * i.Product.UnitCost).Value,
                TotalPrice = x.Items.Sum(i => i.Quantity * i.Product.UnitPrice).Value,
                CreatedDate = (new DateTime(x.CreatedDate.Year, x.CreatedDate.Month, 1).AddMonths(1).AddDays(-1))
            }).ToList();

        var orderMonthlyProfitSummaries = new List<OrderMonthlyProfitSummary>();
        
        var ProfitSummariesByMonth = orderMonthlyProfitDetails.GroupBy(x=>x.CreatedDate);    

        foreach (var Month in ProfitSummariesByMonth)
        {
            var orderMonthlyProfitSummary = new OrderMonthlyProfitSummary{
                MonthEnding = Month.Key,
                ItemCount = Month.Sum(x=>x.ItemCount),
                TotalCost = Month.Sum(x=>x.TotalCost),
                TotalPrice = Month.Sum(x=>x.TotalPrice),
                Profit = Month.Sum(x=>x.TotalPrice)- Month.Sum(x=>x.TotalCost)
            };
            orderMonthlyProfitSummaries.Add(orderMonthlyProfitSummary);
        }
        return orderMonthlyProfitSummaries;
    
    }
  }
}
