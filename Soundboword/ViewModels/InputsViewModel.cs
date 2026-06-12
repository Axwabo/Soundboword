using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Soundboword.Inputs;
using Soundboword.Models;
using Soundboword.Services;

namespace Soundboword.ViewModels;

public sealed partial class InputsViewModel : PageModelBase
{

    private static readonly string File = Path.Combine(UserData.Folder, "inputs.json");

    private readonly List<InputMethodInterface> _all;

    public InputsViewModel()
    {
        _all = [];
        Context = new InputEditingContext(new ShortcutList(null, new ShortcutAssigner()));
    }

    public InputsViewModel(IClassicDesktopStyleApplicationLifetime lifetime, InputEditingContext context, IEnumerable<IInputFactory> factories)
    {
        _all = factories.Select(e => new InputMethodInterface(e, context)).ToList();
        Context = context;
        Refresh();
        var activated = UserData.Load(File, () => new HashSet<string>());
        foreach (var input in Available)
            if (activated.Contains(input.Name))
                input.Activated = true;
        lifetime.Exit += (_, _) => UserData.Save(File, _all.Where(e => e.Activated).Select(e => e.Name));
        context.PropertyChanged += ContextOnPropertyChanged;
        context.List.ShortcutsChanged += ListOnShortcutsChanged;
    }

    public InputEditingContext Context { get; }

    public ShortcutAssigner Assigner => Context.List.Assigner;

    public ObservableCollection<InputMethodInterface> Available { get; } = [];

    public ObservableCollection<InputMethodInterface> Unavailable { get; } = [];

    [ObservableProperty]
    public partial string? StopAllShortcut { get; private set; }

    [RelayCommand]
    private void Refresh()
    {
        Available.Clear();
        Unavailable.Clear();
        foreach (var method in _all)
            if (method.IsAvailable)
                Available.Add(method);
            else
                Unavailable.Add(method);
    }

    [RelayCommand]
    private void RemoveShortcut()
    {
        if (Context.Interface != null)
            Context.List.Remove(StopAllSoundsAction.Instance);
    }

    private void ContextOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(InputEditingContext.Interface))
            ListOnShortcutsChanged();
    }

    private void ListOnShortcutsChanged()
    {
        if (Context.Interface is {Name: var name})
            StopAllShortcut = Context.List.ForStopAll(name)?.FriendlyName;
    }

    public override void OnActivated()
    {
        if (Context.Interface is { } method)
            Context.Open(method);
    }

}
