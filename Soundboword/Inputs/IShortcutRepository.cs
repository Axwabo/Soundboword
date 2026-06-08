using System.Collections.Generic;
using Soundboword.Models;
using Soundboword.ViewModels;

namespace Soundboword.Inputs;

public interface IShortcutRepository
{

    IEnumerable<Shortcut> GetAll(SoundViewModel sound);

    void RemoveAll(SoundViewModel sound);

    void Commit();

}
