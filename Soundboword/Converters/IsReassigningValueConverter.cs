using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Soundboword.ViewModels;

namespace Soundboword.Converters;

public sealed class IsReassigningValueConverter : IValueConverter
{

    // this is so ahh
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is string s && parameter is EditSoundViewModel vm && vm.Context.ListeningMethod == s;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

}
