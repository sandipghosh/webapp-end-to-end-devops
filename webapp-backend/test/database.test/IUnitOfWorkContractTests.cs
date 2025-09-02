
using System.Linq;
using Xunit;
using dataaccess.UnitOfWork;

namespace dataaccess.tests;

public class IUnitOfWorkContractTests
{
    [Fact]
    public void Interface_Defines_Expected_Members()
    {
        var t = typeof(IUnitOfWork);
        var prop = t.GetProperty("TaskItems");
        Assert.NotNull(prop);
        var save = t.GetMethod("SaveChangesAsync");
        Assert.NotNull(save);
    }
}
