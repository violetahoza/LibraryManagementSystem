using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Interfaces
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
        void SaveChanges();
    }
}