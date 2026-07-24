namespace Soundboword.Linux.PipeWire.Wizard;

public sealed partial class PipeWireWizardWindow : Window
{

    public static async Task ShowDialogAsync(Window parent, AudioManager audioManager, DevicesViewModel devices)
    {
        var wizard = new PipeWireWizardWindow();
        wizard.DataContext = new PipeWireWizardWindowViewModel(wizard, audioManager, devices);
        await wizard.ShowDialog(parent);
    }

    public PipeWireWizardWindow() => InitializeComponent();

}
