namespace Soundboword.Inputs;

public interface IInputFactory
{

    string Name { get; }

    bool IsAvailable { get; }

    IAsyncRelayCommand? Configure => null;

    Task<IInputMethod?> ActivateAsync();

}
