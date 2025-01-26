using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.UnitTests.EnumerableEntityFields.Lists;

public class AuditableEntityListActionTests
{
    private const string FieldName = "TestList";

    private readonly AuditableEntityListInitialized<TestEntity> _initializedEvent;

    private readonly List<IAuditableEntityListDomainEvent> _events;

    private readonly Ulid _fieldId = Ulid.NewUlid();
    private readonly Ulid _entityId = Ulid.NewUlid();

    private readonly AggregateRootId _aggregateRootId = new(Ulid.NewUlid(), typeof(AuditableRootEntity));
    private readonly TestEntity _testEntity;

    public AuditableEntityListActionTests()
    {
        _testEntity = new TestEntity(_aggregateRootId, _entityId);
        _initializedEvent = new AuditableEntityListInitialized<TestEntity>(
            Ulid.NewUlid(),
            Ulid.NewUlid(),
            Ulid.NewUlid(), 
            "TestList",
            1,
            [_testEntity],
            DateTimeOffset.UtcNow);
        _events = [_initializedEvent];
    }
    
    [Fact]
    public void Should_GenerateListInitializedEvent()
    {
        // Act
        var auditableIntList = new AuditableEntityList<TestEntity>();
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListInitialized<TestEntity>>(auditableIntList.GetChanges()[0]);
    }

    [Fact]
    public void Should_GenerateListClearedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Clear();
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListCleared<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Empty(auditableIntList);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
    }

    [Fact]
    public void Should_GenerateItemAddedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Add(_testEntity);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListItemAdded<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(2, auditableIntList.Count);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableEntityList<TestEntity>(
            _initializedEvent.FieldId,
            auditableListDomainEvents!,
            new Dictionary<Ulid, IAuditableChildEntity>
            {
                { _testEntity.GetEntityId(), _testEntity }
            });
        
        Assert.Equal(2, auditableIntList.Count);
    }

    [Fact]
    public void Should_GenerateItemRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Remove(_testEntity);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListRemoveAt<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Empty(auditableIntList);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableEntityList<TestEntity>(
            _initializedEvent.FieldId,
            auditableListDomainEvents!,
            new Dictionary<Ulid, IAuditableChildEntity>
            {
                { _testEntity.GetEntityId(), _testEntity }
            });
        
        Assert.Empty(auditableIntList);
    }

    [Fact]
    public void Should_GenerateItemInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.Insert(0, _testEntity);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListItemInserted<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(2, auditableIntList.Count);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableEntityList<TestEntity>(
            _initializedEvent.FieldId,
            auditableListDomainEvents!,
            new Dictionary<Ulid, IAuditableChildEntity>
            {
                { _testEntity.GetEntityId(), _testEntity }
            });
        
        Assert.Equal(2, auditableIntList.Count);
    }

    [Fact]
    public void Should_GenerateRangeInsertedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        List<TestEntity> entries =
            [new TestEntity(_aggregateRootId, Ulid.NewUlid()), new TestEntity(_aggregateRootId, Ulid.NewUlid())];
        
        // Act
        auditableIntList.InsertRange(0, entries);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListRangeInserted<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Equal(3, auditableIntList.Count);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);

        var childEntities = new Dictionary<Ulid, IAuditableChildEntity>
        {
            { _testEntity.GetEntityId(), _testEntity }
        };
        
        foreach (var entry in entries)
        {
            childEntities.Add(entry.GetEntityId(), entry);
        }
        
        auditableIntList = new AuditableEntityList<TestEntity>(
            _initializedEvent.FieldId,
            auditableListDomainEvents!,
            childEntities);
        
        Assert.Equal(3, auditableIntList.Count);
    }

    [Fact]
    public void Should_GenerateRangeRemovedEvent()
    {
        // Arrange
        var auditableIntList = new AuditableEntityList<TestEntity>(_initializedEvent.FieldId, _events, []);
        auditableIntList.SetParentFieldValues(_entityId, _fieldId, FieldName);
        
        // Act
        auditableIntList.RemoveRange(0, 1);
        
        // Assert
        Assert.Single(auditableIntList.GetChanges());
        Assert.IsType<AuditableEntityListRangeRemoved<TestEntity>>(auditableIntList.GetChanges()[0]);
        Assert.Empty(auditableIntList);
        
        var history = auditableIntList.GetChanges();
        history.AddRange(_events);
        var auditableListDomainEvents = history
            .OrderBy(e => e.EventVersion)
            .Select(e => e as IAuditableEntityListDomainEvent)
            .ToList();
        
        Assert.NotNull(auditableListDomainEvents);
        
        auditableIntList = new AuditableEntityList<TestEntity>(
            _initializedEvent.FieldId,
            auditableListDomainEvents!,
            new Dictionary<Ulid, IAuditableChildEntity>
            {
                { _testEntity.GetEntityId(), _testEntity }
            });
        
        Assert.Empty(auditableIntList);
    }

    private class ParentEntity : AuditableRootEntity
    {
        [AuditableEntityListField<TestEntity>(true)]
        public AuditableEntityList<TestEntity> TestList
        {
            get => GetEntityList<TestEntity>(nameof(TestList));
            set => SetEntityList<TestEntity>(value, nameof(TestList));
        }
        
        public ParentEntity() { }

        public ParentEntity(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    }
    
    private class TestEntity : AuditableEntity
    {
        [AuditableValueField<int>(true)]
        public int IntProperty
        {
            get => GetValue<int>(nameof(IntProperty));
            set => SetValue<int>(value, nameof(IntProperty));
        }
        
        public TestEntity() { }

        public TestEntity(AggregateRootId aggregateRootId, Ulid entityId) : base(aggregateRootId, entityId) { }
    }
}
