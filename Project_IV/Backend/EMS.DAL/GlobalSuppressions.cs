// This file is used by Code Analysis to maintain SuppressMessage attributes
// applied to this project. CA1848 warnings are suppressed because the existing
// repository logging pattern uses ILogger extension methods intentionally.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates",
    Justification = "Existing Project-2 logging pattern retained intentionally.")]
[assembly: SuppressMessage("Design", "CA1852:Seal internal types",
    Justification = "Migration snapshot types are generated code.")]
[assembly: SuppressMessage("Design", "CA1861:Prefer static readonly fields",
    Justification = "Applies to generated migration code.")]
