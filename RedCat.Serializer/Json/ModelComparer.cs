using System.Collections.Generic;

namespace RedCat.Serializer.Json
{
    public abstract class ModelComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return GetHashCode(x) == GetHashCode(y) ? true : false;
        }

        public abstract int GetHashCode(T obj);
    }
}
