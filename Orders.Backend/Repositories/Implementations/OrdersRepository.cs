using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using Orders.Shared.Responses;
using System.Linq;

namespace Orders.Backend.Repositories.Implementations
{
    public class OrdersRepository : GenericRepository<Order>, IOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public OrdersRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context=context;
            _usersRepository=usersRepository;
        }

        public async Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user is null)
            {
                return new ActionResponse<IEnumerable<Order>>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido."
                };
            }
            var queryable = _context.Orders
                .Include(o => o.User!)
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Product)
                .AsQueryable();

            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(o => o.User!.Email == email);
            }
            return new ActionResponse<IEnumerable<Order>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderByDescending(o => o.Date)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public override async Task<ActionResponse<Order>> GetAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User!)
                .ThenInclude(u => u.City!)
                .ThenInclude(c => c.State!)
                .ThenInclude(s => s.Country)
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (order is null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Pedido no encontrado."
                };
            }

            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        public async Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user is null)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido."
                };
            }
            var queryable = _context.Orders
                .Include(o => o.User!)
                .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Product)
                .AsQueryable();

            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin)
            {
                queryable = queryable.Where(o => o.User!.Email == email);
            }
            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO)
        {
            var user = await _usersRepository.GetUserAsync(email);
            if (user is null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Usuario no válido."
                };
            }
            var isAdmin = await _usersRepository.IsUserInRoleAsync(user, UserType.Admin.ToString());
            if (!isAdmin && orderDTO.OrderStatus != OrderStatus.Cancelled)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "Solo permitido para administradores."
                };
            }
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(s => s.Id == orderDTO.Id);
            if (order is null)
            {
                return new ActionResponse<Order>
                {
                    WasSuccess = false,
                    Message = "El pedido no existe."
                };
            }

            if (orderDTO.OrderStatus == OrderStatus.Cancelled)
            {
                await ReturnStockAsync(order);
            }
            order.OrderStatus = orderDTO.OrderStatus;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return new ActionResponse<Order>
            {
                WasSuccess = true,
                Result = order
            };
        }

        private async Task ReturnStockAsync(Order order)
        {
            foreach (var orderDetail in order.OrderDetails!)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == orderDetail.ProductId);
                if (product != null)
                {
                    product.Stock += orderDetail.Quantity;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
