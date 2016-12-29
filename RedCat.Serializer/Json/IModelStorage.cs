using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedCat.Serializer.Json
{
    public interface IModelStorage<T>
    {
        void Insert(T Items);
        T SelectOne(Func<T, bool> predicate);
        List<T> SelectMany(Func<T, bool> predicate);
        List<T> SelectAll();
        void RemoveOne(T item);
        int RemoveWhere(Predicate<T> predicate);
        Task SaveChanges();
    }
}
