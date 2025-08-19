using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly IUsersUnitOfWork _usersUnitOfWork;
        private readonly ITemporalOrdersUnitOfWork _temporalOrdersUnitOfWork;
        private readonly IOrdersUnitOfWork _ordersUnitOfWork;
        private readonly IProductsUnitOfWork _productsUnitOfWork;

        public OrdersHelper(IUsersUnitOfWork usersUnitOfWork, ITemporalOrdersUnitOfWork temporalOrdersUnitOfWork, IProductsUnitOfWork productsUnitOfWork, IOrdersUnitOfWork ordersUnitOfWork)
        {
            _usersUnitOfWork=usersUnitOfWork;
            _temporalOrdersUnitOfWork=temporalOrdersUnitOfWork;
            _ordersUnitOfWork=ordersUnitOfWork;
            _productsUnitOfWork=productsUnitOfWork;
        }
        public async Task<ActionResponse<bool>> ProcessOrderAsync(string email, string remarks)
        {
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user is null)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido."
                };
            }

            var actionTemporalOrder = await _temporalOrdersUnitOfWork.GetAsync(email);
            if (!actionTemporalOrder.WasSuccess)
            {
                return new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "El detalle de la orden no existe."
                };
            }
            var temporalOrders = actionTemporalOrder.Result as List<TemporalOrder>;
            var response = await CheckInventaryAsync(temporalOrders!);
            if (!response.WasSuccess)
            {
                return response;
            }
            var order = new Order
            {
                Date = DateTime.UtcNow,
                User = user,
                Remarks = remarks,
                OrderDetails = new List<OrderDetail>(),
                OrderStatus = OrderStatus.New
            };
            foreach (var temporalOrder in temporalOrders!)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    Product = temporalOrder.Product,
                    Quantity = temporalOrder.Quantity,
                    Remarks = temporalOrder.Remarks,
                });
                var actionProduct = await _productsUnitOfWork.GetAsync(temporalOrder.Product!.Id);
                if (actionProduct.WasSuccess)
                {
                    var product = actionProduct.Result;
                    if (product != null)
                    {
                        product.Stock -= temporalOrder.Quantity;
                        await _productsUnitOfWork.UpdateAsync(product);
                    }
                }
                await _temporalOrdersUnitOfWork.DeleteAsync(temporalOrder.Id);
            }
            await _ordersUnitOfWork.AddAsync(order);
            return response;
        }

        private async Task<ActionResponse<bool>> CheckInventaryAsync(List<TemporalOrder> temporalOrders)
        {
            var response = new ActionResponse<bool> { WasSuccess = true };
            foreach (var temporalOrder in temporalOrders)
            {
                var actionProduct = await _productsUnitOfWork.GetAsync(temporalOrder.Product!.Id);
                if (!actionProduct.WasSuccess)
                {
                    response.WasSuccess = false;
                    response.Message = $"El producto con Id {temporalOrder.Product.Id} no existe.";
                    return response;
                }
                var product = actionProduct.Result;
                if (product == null)
                {
                    response.WasSuccess = false;
                    response.Message = $"El producto con Id {temporalOrder.Product.Id} no existe.";
                    return response;
                }
                if (product.Stock < temporalOrder.Quantity)
                {
                    response.WasSuccess = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto con Id {temporalOrder.Product.Id} para tomar su pedido por favor disminuya la cantidad.";
                    return response;
                }
            }
            return response;
        }
    }
}
