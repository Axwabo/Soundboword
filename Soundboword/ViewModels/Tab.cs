namespace Soundboword.ViewModels;

public sealed record Tab(string Header, string Icon, ViewModelBase Content, bool IsLazy = false);
