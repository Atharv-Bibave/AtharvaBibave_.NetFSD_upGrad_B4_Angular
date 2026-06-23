// This file is used by Code Analysis to maintain SuppressMessage attributes.
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates",
    Justification = "Logging pattern intentionally uses ILogger extension methods for readability.")]
