namespace Soundboword;

public sealed partial class SoundEditingContext : ObservableObject
{

    private readonly ShortcutAssigner? _assigner;

    public SoundEditingContext(ShortcutAssigner? assigner = null) => _assigner = assigner;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name), nameof(TriggerMode), nameof(Volume), nameof(Loop), nameof(Interaction), nameof(CanRelink))]
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

    public TriggerMode TriggerMode
    {
        get => Model?.Mode ?? default;
        set
        {
            Model?.Mode = value;
            OnPropertyChanged();
        }
    }

    public bool Loop
    {
        get => Model?.Loop ?? false;
        set
        {
            Model?.Loop = value;
            OnPropertyChanged();
        }
    }

    public float Volume
    {
        get => Model?.Volume ?? 1;
        set
        {
            Model?.Volume = value;
            OnPropertyChanged();
        }
    }

    public OtherSoundInteraction Interaction
    {
        get => Model?.Interaction ?? OtherSoundInteraction.Nothing;
        set
        {
            Model?.Interaction = value;
            OnPropertyChanged();
        }
    }

    public bool CanRelink => Model?.CanRelink ?? false;

    public void Open(SoundViewModel model)
    {
        Close();
        _assigner?.Target = new TriggerSoundAction(model);
        Model = model;
        Model.PropertyChanged += ModelOnPropertyChanged;
    }

    public void Close()
    {
        _assigner?.Close();
        Model?.PropertyChanged -= ModelOnPropertyChanged;
        Model = null;
    }

    private void ModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);

    public void StartAssigning() => _assigner?.IsAssigning = true;

}
