using System;
using System.Linq;
using System.Collections.Generic;

namespace webapp.Services;

public static class Utilities
{
    public static string? GetEnvironmentVariableByName(string envName)
    {
        if (Environment.GetEnvironmentVariables().Keys.Cast<string>().Any(x => x == envName))
            return Environment.GetEnvironmentVariable(envName);

        return string.Empty;
    }
}
