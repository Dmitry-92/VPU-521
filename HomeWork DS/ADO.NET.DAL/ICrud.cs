namespace ADO.NET.DAL;

public interface ICrud<T>
{
    public T Create(T entity);
    public T Read(int id);
    public T Update(int id, T value);
    public T Delete(int id);
}