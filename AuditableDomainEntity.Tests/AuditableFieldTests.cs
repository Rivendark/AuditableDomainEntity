namespace AuditableDomainEntity.Tests;

public class AuditableFieldTests
{
    [Fact]
    public void Should_Generate_FieldInitializedAndUpdatedEvents()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var testClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10)
        };

        var changes = testClass.Save();
        
        Assert.NotNull(changes);
        Assert.NotEmpty(changes);
        Assert.Single(changes);
        
        var domainEvent = changes.Single();
        Assert.NotNull(domainEvent);
        Assert.Equal(aggregateRootId.Value, domainEvent.EntityId);
        
        var fieldEvents = domainEvent.FieldEvents;
        
        Assert.NotNull(fieldEvents);
        Assert.NotEmpty(fieldEvents);
        Assert.Equal(2, fieldEvents.Count);
        
        testClass.Number = 4;
        
        var updatedChanges = testClass.Save();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        
        fieldEvents = updatedChanges.Single().FieldEvents;
        
        Assert.NotNull(fieldEvents);
        Assert.NotEmpty(fieldEvents);
        Assert.Equal(3, fieldEvents.Count);
    }

    [Fact]
    public void Should_LoadDataFromAuditHistory_IntoFields()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var preloadClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };
        
        var changes = preloadClass.Save();

        var testClassNew = new TestRootEntity(aggregateRootId, changes);
        
        var instantiatedChanges = testClassNew.Save();
        Assert.NotNull(instantiatedChanges);
        Assert.Empty(instantiatedChanges);
        Assert.Equal(preloadClass.Number, testClassNew.Number);
        Assert.Equal(preloadClass.Date, testClassNew.Date);
        
        testClassNew.Number = 4;
        
        var updatedChanges = testClassNew.Save();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        Assert.Equal(4, testClassNew.Number);
    }
}