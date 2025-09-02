
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using dataaccess.Repositories;

namespace dataaccess.tests;

public class IGenericRepositoryContractTests
{
    [Fact]
    public void Interface_Defines_Expected_Methods()
    {
        var t = typeof(IGenericRepository<>);
        var names = t.GetMethods().Select(m => m.Name).ToArray();
        Assert.Contains("GetByIdAsync", names);
        Assert.Contains("GetAllAsync", names);
        Assert.Contains("AddAsync", names);
        Assert.Contains("Update", names);
        Assert.Contains("Delete", names);
        Assert.Contains("AnyAsync", names);
    }
}
