using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Lists;

public class AuditableValueListActionTests
{
    private const string FieldName = "TestList";

    private static readonly AuditableValueValueListInitialized<int> InitializedEvent = new(
        Ulid.NewUlid(),
        Ulid.NewUlid(),
        Ulid.NewUlid(), 
        "TestList",
        1,
        [1, 2, 3],
        DateTimeOffset.UtcNow);

    private readonly List<IAuditableValueListDomainEvent> _events = [InitializedEvent];

    private readonly Ulid _fieldId = Ulid.NewUlid();
    private readonly Ulid _entityId = Ulid.NewUlid();

    [Fact]
    public void Should_GenerateListInitializedEvent()
    {
        // Act
        var auditableIntList = new AuditableValueList<int>();
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListInitialized<int>>(auditableIntList.GetChanges()[0]);
    }
    
    [Fact]
    public void Should_GenerateListClearedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Clear();
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListCleared<int>>(auditableIntList.GetChanges()[0]);
        Assert.Empty(auditableIntList);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Empty(auditableIntList);
    }
    
    [Fact]
    public void Should_GenerateItemAddedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Add(4);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListItemAdded<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[3]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[3]);
    }
    
    [Fact]
    public void Should_GenerateItemRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.RemoveAt(0);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListRemoveAt<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(2, auditableIntList[0]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(2, auditableIntList[0]);
    }
    
    [Fact]
    public void Should_GenerateItemInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Insert(1, 4);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListItemInserted<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[1]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[1]);
    }
    
    [Fact]
    public void Should_GenerateRangeInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.InsertRange(1, [4, 5]);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListRangeInserted<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(4, auditableIntList[1]);
        Assert.Equal(5, auditableIntList[2]);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(4, auditableIntList[1]);
    }
    
    [Fact]
    public void Should_GenerateRangeRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, _events);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.RemoveRange(1, 2);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableValueValueListRangeRemoved<int>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(1, auditableIntList[0]);
        Assert.Equal(1, auditableIntList.Count);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableValueListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableValueList<int>(InitializedEvent.FieldId, auditableListDomainEvents!);
        
        Assert.Equal(1, auditableIntList[0]);
    }
}