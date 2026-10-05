using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public static partial class ThisAssembly
{
    public static string GetVersion()
    {   
        return $"{ThisAssembly.Git.Tag}--{ThisAssembly.Git.Commit}";
    }
}
