using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.CollectionEvents.ListEvents;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Lists;

public class AuditableValueListActionTests
{
    private const string FieldName = "TestList";

    private static readonly AuditableListInitialized<int> InitializedEvent = new(
        Ulid.NewUlid(),
        Ulid.NewUlid(),
        Ulid.NewUlid(), 
        "TestList",
        1,
        [1, 2, 3],
        DateTimeOffset.UtcNow);

    private readonly List<IAuditableListDomainEvent> _events = [InitializedEvent];

    private readonly Ulid _fieldId = Ulid.NewUlid();
    private readonly Ulid _entityId = Ulid.NewUlid();

    [Fact]
    public void Should_GenerateListInitializedEvent()
    {
        // Act
        var auditableIntList = new AuditableList<int>();
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListInitialized<int>>(auditableIntList.GetChanges()[0]);
    }
    
    [Fact]
    public void Should_GenerateListClearedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Clear();
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListCleared<int>>(auditableIntList.GetChanges()[0]);
        Assert.Empty(auditableIntList);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Empty(auditableIntList);
    }
    
    [Fact]
    public void Should_GenerateItemAddedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Add(4);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListItemAdded<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[3]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[3]);
    }
    
    [Fact]
    public void Should_GenerateItemRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.RemoveAt(0);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListRemoveAt<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(2, auditableIntList[0]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(2, auditableIntList[0]);
    }
    
    [Fact]
    public void Should_GenerateItemInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Insert(1, 4);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListItemInserted<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[1]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[1]);
    }
    
    [Fact]
    public void Should_GenerateRangeInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.InsertRange(1, [4, 5]);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListRangeInserted<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[1]);
        Assert.Equal(5, auditableIntList[2]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[1]);
    }
    
    [Fact]
    public void Should_GenerateRangeRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetEntityValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.RemoveRange(1, 2);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableListRangeRemoved<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(1, auditableIntList[0]);
        Assert.Equal(1, auditableIntList.Count);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(1, auditableIntList[0]);
    }
}