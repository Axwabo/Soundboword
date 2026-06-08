using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Soundboword.Models;
using Soundboword.Services;
using Soundboword.ViewModels;

namespace Soundboword;

public sealed partial class SoundEditingContext : ObservableObject
{

    private readonly HashSet<InputMethodInterface> _activatedInputs = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlaybackMode), nameof(Name))]
    public partial SoundViewModel? Model { get; private set; }

    public string Name
    {
        get => Model?.Name ?? "";
        set
        {
            Model?.Name = value;
            OnPropertyChanged();
        }
    }

    public PlaybackMode PlaybackMode
    {
        get => Model?.Mode ?? default;
        set
        {
            Model?.Mode = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonText))]
    public partial bool IsListeningForShortcuts { get; set; }

    public string ButtonText => IsListeningForShortcuts ? "Listening..." : "Add Shortcut";

    public void Open(SoundViewModel model)
    {
        Close();
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

    public void ToggleListening(InputsViewModel inputs)
    {
        if (Model is not { } model)
            return;
        if (!IsListeningForShortcuts)
        {
            CancelShortcutAddition();
            return;
        }

        foreach (var input in inputs.Available)
            if (input.Activated)
            {
                input.ListenForShortcutAddition(model);
                _activatedInputs.Add(input);
            }

        if (_activatedInputs.Count == 0)
            IsListeningForShortcuts = false;
    }

    public void CancelShortcutAddition()
    {
        IsListeningForShortcuts = false;
        foreach (var input in _activatedInputs)
            input.CancelShortcutAddition();
        _activatedInputs.Clear();
    }

    public void NotifyShortcutChange(Shortcut shortcut)
    {
        CancelShortcutAddition();
        var index = ShortcutList.FindIndex(shortcut.MethodName, shortcut.Sound);
        if (index == -1)
            ShortcutList.Shortcuts.Add(shortcut);
        else
            ShortcutList.Shortcuts[index] = shortcut;
    }

}
