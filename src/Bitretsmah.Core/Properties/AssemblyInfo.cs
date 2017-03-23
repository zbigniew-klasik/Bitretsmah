using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Bitretsmah.Core")]
[assembly: AssemblyProduct("Bitretsmah.Core")]
[assembly: AssemblyCopyright("Copyright © Zbigniew Klasik")]
[assembly: ComVisible(false)]
[assembly: Guid("5162931a-bde0-4c66-86ee-4c16e22c8b3e")]

// Set internal classes to be visible for test project
[assembly: InternalsVisibleTo("Bitretsmah.Tests.Unit")]
[assembly: InternalsVisibleTo("Bitretsmah.Tests.Integration")]

// Version is set by Travis CI
// [assembly: AssemblyVersion("0.1.0.0")]
// [assembly: AssemblyFileVersion("0.1.0.0")]
// [assembly: AssemblyInformationalVersion("0.1.0.0")]
