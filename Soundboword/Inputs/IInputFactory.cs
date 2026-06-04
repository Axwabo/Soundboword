namespace Soundboword.Inputs;

public interface IInputFactory
{

    string Name { get; }

    bool IsAvailable { get; }

    IInputMethod? Activate();

}
