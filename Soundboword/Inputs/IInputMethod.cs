using System;
using Soundboword.ViewModels;

namespace Soundboword.Inputs;

public interface IInputMethod : IDisposable
{

    void ListenForShortcutAddition(SoundViewModel target);

    void ClearShortcut(SoundViewModel target);

    void CancelShortcutAddition();

}
