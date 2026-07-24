namespace Soundboword.Linux.PipeWire.Wizard;

public sealed partial class PipeWireWizardWindow : Window
{

    public static async Task ShowDialogAsync(Window parent)
    {
        var wizard = new PipeWireWizardWindow();
        wizard.DataContext = new PipeWireWizardWindowViewModel(wizard);
        await wizard.ShowDialog(parent);
    }

    public PipeWireWizardWindow() => InitializeComponent();

}
