using Soundboword.Inputs;
using Soundboword.OutputDevices;

namespace Soundboword.Linux.PipeWire;

public sealed record RestartContext(AudioManager AudioManager, DevicesViewModel Devices, InputsViewModel Inputs, NodeManager NodeManager);
