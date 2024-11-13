namespace AuditableDomainEntity.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var testClass = new TestRootEntity
        {
            Id = new AggregateRootId(Ulid.NewUlid()),
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };

        var changes = testClass.GetDomainEvents();
        
        Assert.Equal(2, changes.Count);
        var domainEvent = changes.FirstOrDefault();
        Assert.Equal(testClass.Id.Value, domainEvent?.EntityId);
        
        testClass.Number = 4;
        
        Assert.Equal(3, testClass.GetDomainEvents().Count);
    }
}