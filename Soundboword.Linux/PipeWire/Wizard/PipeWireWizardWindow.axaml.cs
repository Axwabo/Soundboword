namespace Soundboword.Linux.PipeWire.Wizard;

public sealed partial class PipeWireWizardWindow : Window
{

    public static async Task ShowDialogAsync(Window parent, RestartContext context)
    {
        var wizard = new PipeWireWizardWindow();
        wizard.DataContext = new PipeWireWizardWindowViewModel(wizard, context);
        await wizard.ShowDialog(parent);
    }

    public PipeWireWizardWindow() => InitializeComponent();

}
