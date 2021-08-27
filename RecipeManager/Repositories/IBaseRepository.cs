using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeManager.Repositories {
    public interface IBaseRepository<T> where T : class {
        public Task<Guid> CreateAsync(T item);
        public Task<IEnumerable<T>> ReadAllAsync();
        public Task<IEnumerable<T>> ReadByQueryAsync(string queryString);
        public Task<T> ReadByIdAsync(Guid id);
        public Task<T> UpdateAsync(T item);
        public Task<T> SoftUpdateAsync(T item);
        public Task DeleteAsync(Guid id);
    }
}
