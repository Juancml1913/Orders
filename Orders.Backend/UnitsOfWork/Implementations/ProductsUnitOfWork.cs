using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class ProductsUnitOfWork : GenericUnitOfWork<Product>, IProductsUnitOfWork
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsUnitOfWork(IGenericRepository<Product> repository, IProductsRepository productsRepository) : base(repository)
        {
            _productsRepository=productsRepository;
        }

        public async Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO)
        {
            return await _productsRepository.AddFullAsync(productDTO);
        }

        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
        {
            return await _productsRepository.AddImageAsync(imageDTO);
        }

        public async override Task<ActionResponse<Product>> GetAsync(int id)
        {
            return await _productsRepository.GetAsync(id);
        }

        public async override Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        {
            return await _productsRepository.GetAsync(pagination);
        }

        public async override Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            return await _productsRepository.GetTotalPagesAsync(pagination);
        }

        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        {
            return await _productsRepository.RemoveLastImageAsync(imageDTO);
        }

        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        {
            return await _productsRepository.UpdateFullAsync(productDTO);
        }
    }
}
