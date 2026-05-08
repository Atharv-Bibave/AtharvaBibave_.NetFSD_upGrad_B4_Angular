// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
using System.Diagnostics.CodeAnalysis;

// CA1707: Identifiers should not contain underscores — NUnit test methods use
// descriptive names; underscores are not used here (PascalCase throughout).

// CA1001: Types that own disposable fields should be disposable — all test
// fixtures implement IDisposable explicitly.
[assembly: SuppressMessage("Design", "CA1001", Justification = "Disposable pattern implemented explicitly in each TestFixture class.")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Underscore-separated naming is standard convention for test methods.")]