﻿using Order.Data;
using Order.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            return orders;
        }

        public async Task<OrderDetail> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            return order;
        }

        public async Task<IEnumerable<OrderSummary>> GetOrdersByStatusAsync(string statusName)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(statusName);
            return orders;
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync()
        {
            var orderDetails = await _orderRepository.GetOrderDetailsAsync();
            return orderDetails;
        }

        public async Task<OrderDetail> UpdateOrderDetailAsync(OrderDetail orderDetail)
        {
            var orderDetailToUpdate = await _orderRepository.UpdateOrderDetailAsync(orderDetail);

            return orderDetailToUpdate;
        }

        public async Task<OrderDetail> CreateOrderDetailAsync(OrderDetail orderDetail)
        {
        var orderDetailToCreate = await _orderRepository.CreateOrderDetailAsync(orderDetail);

        return orderDetailToCreate;
        }

    public async Task<List<OrderMonthlyProfitSummary>> MonthlyProfits()
    {
      var ordersMonthlyProfitSummary = await _orderRepository.MonthlyProfits();

      return ordersMonthlyProfitSummary;
    }
  }
}
