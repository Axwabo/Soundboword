using System.Collections.Generic;
using Soundboword.Models;

namespace Soundboword.Inputs;

public interface IShortcutRepository
{

    IEnumerable<Shortcut> GetAll(ShortcutAction action);

    void RemoveAll(ShortcutAction action);

    void Commit();

}
