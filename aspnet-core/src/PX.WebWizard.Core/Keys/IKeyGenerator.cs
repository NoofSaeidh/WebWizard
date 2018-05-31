namespace PX.WebWizard.Keys
{
    public interface IKeyGenerator
    {
        T Generate<T>();
    }
}