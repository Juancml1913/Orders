using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations
{
    public class GenericUnitOfWork<T> : IGenericUnitOfWork<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericUnitOfWork(IGenericRepository<T> repository)
        {
            _repository = repository;
        }
        public virtual Task<ActionResponse<T>> AddAsync(T entity)
        {
            return _repository.AddAsync(entity);
        }

        public virtual Task<ActionResponse<T>> DeleteAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }

        public virtual Task<ActionResponse<T>> GetAsync(int id)
        {
            return _repository.GetAsync(id);
        }

        public virtual Task<ActionResponse<IEnumerable<T>>> GetAsync()
        {
            return _repository.GetAsync();
        }

        public virtual Task<ActionResponse<T>> UpdateAsync(T entity)
        {
            return _repository.UpdateAsync(entity);
        }
    }
}
