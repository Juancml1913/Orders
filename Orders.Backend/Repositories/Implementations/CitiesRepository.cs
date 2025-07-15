using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations
{
    public class CitiesRepository : GenericRepository<City>, ICitiesRepository
    {
        private readonly DataContext _context;

        public CitiesRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// Retrieves a paginated list of cities filtered by state and optional search criteria.
        /// </summary>
        /// <remarks>This method queries the database for cities associated with the specified state and
        /// applies optional filtering based on the city name. The results are paginated according to the provided
        /// pagination parameters.</remarks>
        /// <param name="pagination">The pagination parameters, including the state identifier, page size, page number, and optional filter. The
        /// <paramref name="pagination.Id"/> specifies the state to filter cities by. The <paramref
        /// name="pagination.Filter"/> allows filtering cities by name.</param>
        /// <returns>An <see cref="ActionResponse{T}"/> containing a collection of cities that match the specified criteria. The
        /// <see cref="ActionResponse{T}.Result"/> will contain the paginated list of cities, ordered by name.</returns>
        public override async Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Cities
                .Where(x => x.State!.Id == pagination.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync()
            };
        }

        /// <summary>
        /// Retrieves a collection of cities associated with the specified state.
        /// </summary>
        /// <remarks>This method queries the database asynchronously to retrieve cities associated with
        /// the given state. The cities are returned in alphabetical order based on their names.</remarks>
        /// <param name="stateId">The unique identifier of the state for which cities are to be retrieved.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an  IEnumerable{City} of cities
        /// belonging to the specified state, ordered by name.</returns>
        public async Task<IEnumerable<City>> GetComboAsync(int stateId)
        {
            return await _context.Cities.Where(c => c.StateId == stateId).OrderBy(c => c.Name).ToListAsync();
        }

        /// <summary>
        /// Calculates the total number of pages based on the specified pagination criteria.
        /// </summary>
        /// <remarks>This method filters cities by the specified state ID and applies an optional name
        /// filter. The total pages are calculated by dividing the total number of matching records by the  number of
        /// records per page, rounding up to the nearest whole number.</remarks>
        /// <param name="pagination">The pagination criteria, including the ID of the state to filter cities by,  the number of records per page,
        /// and an optional filter string for city names.</param>
        /// <returns>An <see cref="ActionResponse{T}"/> containing the total number of pages as an integer. The response
        /// indicates success and includes the calculated total pages.</returns>
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Cities.Where(x => x.State!.Id == pagination.Id).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }
    }
}
