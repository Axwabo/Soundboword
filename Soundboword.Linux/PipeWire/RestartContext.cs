namespace Soundboword.Linux.PipeWire;

public sealed record RestartContext(AudioManager AudioManager, DevicesViewModel Devices, InputsViewModel Inputs, DeviceSwitchHandler SwitchHandler);
