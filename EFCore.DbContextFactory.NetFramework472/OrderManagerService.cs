﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.DbContextFactory.Examples.Data.Entity;
using EFCore.DbContextFactory.Examples.Data.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EFCore.DbContextFactory.NetFramework472
{
    public class OrderManagerService : IHostedService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderManagerService> _logger;

        public OrderManagerService(IOrderRepository orderRepository, ILogger<OrderManagerService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var orders = await CreateOrders();
            var savedOrders = _orderRepository.GetAllOrders().ToList();
            _logger.LogInformation($"Orders saved: {savedOrders.Count}");

            await DeleteOrders(orders);
            savedOrders = _orderRepository.GetAllOrders().ToList();
            _logger.LogInformation($"Orders after delete: {savedOrders.Count}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<List<Order>> CreateOrders()
        {
            var order1Id = Guid.NewGuid();
            var newOrder1 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order1Id}",
                Id = order1Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var order2Id = Guid.NewGuid();
            var newOrder2 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order2Id}",
                Id = order2Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var order3Id = Guid.NewGuid();
            var newOrder3 = new Order
            {
                Date = DateTime.Now,
                Description = $"Order {order3Id}",
                Id = order3Id,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 1", Quantity = 1, UnitPrice = 1000},
                    new OrderItem {Id = Guid.NewGuid(), Name = "Item 2", Quantity = 4, UnitPrice = 5000}
                }
            };

            var task1 = Task.Factory.StartNew(() =>
            {
                _orderRepository.Add(newOrder2);
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                _orderRepository.Add(newOrder3);
            });

            var task3 = Task.Factory.StartNew(() =>
            {
                _orderRepository.Add(newOrder1);
            });

            await Task.WhenAll(task1, task2, task3);
            return await Task.FromResult(new List<Order>() { newOrder1, newOrder2, newOrder3 });
        }

        public async Task DeleteOrders(List<Order> orders)
        {
            var tasks = new List<Task>();
            foreach (var order in orders)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    _orderRepository.DeleteById(order.Id);
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
