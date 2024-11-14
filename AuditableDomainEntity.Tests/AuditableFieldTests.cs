namespace AuditableDomainEntity.Tests;

public class AuditableFieldTests
{
    [Fact]
    public void Should_Generate_FieldInitializedAndUpdatedEvents()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid());
        var testClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };

        var changes = testClass.GetDomainChanges();
        
        Assert.NotNull(changes);
        Assert.NotEmpty(changes);
        Assert.Equal(2, changes.Count);
        // var domainEvent = changes.FirstOrDefault();
        // Assert.Equal(testClass.EntityId, domainEvent?.EntityId);
        
        testClass.Number = 4;
        
        var updatedChanges = testClass.GetDomainChanges();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Equal(3, updatedChanges.Count);
    }

    [Fact]
    public void Should_LoadDataFromAuditHistory_IntoFields()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid());
        var preloadClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };
        
        var changes = preloadClass.GetDomainChanges();

        var testClassNew = new TestRootEntity(aggregateRootId, changes);
        
        var instantiatedChanges = testClassNew.GetDomainChanges();
        Assert.NotNull(instantiatedChanges);
        Assert.Empty(instantiatedChanges);
        Assert.Equal(preloadClass.Number, testClassNew.Number);
        Assert.Equal(preloadClass.Date, testClassNew.Date);
        
        testClassNew.Number = 4;
        
        var updatedChanges = testClassNew.GetDomainChanges();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        Assert.Equal(4, testClassNew.Number);
    }
}