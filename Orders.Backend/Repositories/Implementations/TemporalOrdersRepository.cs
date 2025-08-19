using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class TemporalOrdersRepository : GenericRepository<TemporalOrder>, ITemporalOrdersRepository
    {
        private readonly DataContext _context;
        private readonly IUsersRepository _usersRepository;

        public TemporalOrdersRepository(DataContext context, IUsersRepository usersRepository) : base(context)
        {
            _context=context;
            _usersRepository=usersRepository;
        }

        public async Task<ActionResponse<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == temporalOrderDTO.ProductId);
            if (product is null)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = "El producto no existe."
                };
            }
            var user = await _usersRepository.GetUserAsync(email);
            if (user is null)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    WasSuccess = false,
                    Message = "El usuario no existe."
                };
            }

            var temporalOrder = new TemporalOrder
            {
                User = user,
                Product = product,
                Quantity = temporalOrderDTO.Quantity,
                Remarks = temporalOrderDTO.Remarks
            };
            try
            {
                _context.Add(temporalOrder);
                await _context.SaveChangesAsync();
                return new ActionResponse<TemporalOrderDTO>
                {
                    Result = temporalOrderDTO,
                    WasSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<TemporalOrderDTO>
                {
                    Message=ex.Message,
                    WasSuccess = false,
                };
            }
        }

        public async Task<ActionResponse<IEnumerable<TemporalOrder>>> GetAsync(string email)
        {
            return new ActionResponse<IEnumerable<TemporalOrder>>
            {
                Result = await _context.TemporalOrders
                    .Include(t => t.User!)
                    .Include(t => t.Product!)
                    .ThenInclude(p => p.ProductCategories!)
                    .ThenInclude(pc => pc.Category)
                    .Include(t => t.Product!)
                    .ThenInclude(p => p.ProductImages)
                    .Where(t => t.User!.Email == email)
                    .ToListAsync(),
                WasSuccess = true,
            };
        }

        public async Task<ActionResponse<int>> GetCountAsync(string email)
        {
            var sum = await _context.TemporalOrders.Where(t => t.User!.Email == email).SumAsync(t => t.Quantity);
            return new ActionResponse<int>
            {
                Result = (int)sum,
                WasSuccess = true,
            };
        }

        public async Task<ActionResponse<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO)
        {
            var currentTemporalOrder = await _context.TemporalOrders.FirstOrDefaultAsync(x => x.Id == temporalOrderDTO.Id);
            if (currentTemporalOrder == null)
            {
                return new ActionResponse<TemporalOrder>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            currentTemporalOrder!.Remarks = temporalOrderDTO.Remarks;
            currentTemporalOrder.Quantity = temporalOrderDTO.Quantity;

            _context.Update(currentTemporalOrder);
            await _context.SaveChangesAsync();
            return new ActionResponse<TemporalOrder>
            {
                WasSuccess = true,
                Result = currentTemporalOrder
            };

        }

        public override async Task<ActionResponse<TemporalOrder>> GetAsync(int id)
        {
            var temporalOrder = await _context.TemporalOrders
            .Include(ts => ts.User!)
            .Include(ts => ts.Product!)
            .ThenInclude(p => p.ProductCategories!)
            .ThenInclude(pc => pc.Category)
            .Include(ts => ts.Product!)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (temporalOrder == null)
            {
                return new ActionResponse<TemporalOrder>
                {
                    WasSuccess = false,
                    Message = "Registro no encontrado"
                };
            }

            return new ActionResponse<TemporalOrder>
            {
                WasSuccess = true,
                Result = temporalOrder
            };

        }
    }
}
