namespace Marvin.Model
{
    public interface IContextMode
    {
        void Configure(ContextMode mode);

        ContextMode CurrentMode { get; }
    }
}