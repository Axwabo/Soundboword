using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace Soundboword.Services;

public sealed record Host(Window TopLevel, IClassicDesktopStyleApplicationLifetime Lifetime);
