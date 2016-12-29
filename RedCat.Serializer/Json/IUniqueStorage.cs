namespace RedCat.Serializer.Json
{
    public interface IUniqueStorage<T>
    {
        bool Contains(T item, ModelComparer<T> compare);
        void Insert(T item, ModelComparer<T> compare);
    }
}
