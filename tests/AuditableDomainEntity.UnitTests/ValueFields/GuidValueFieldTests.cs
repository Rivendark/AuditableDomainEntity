using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class GuidValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleGuidTypes()
    {
        var guidTestClass = new GuidTestClass
        {
            NonNullableGuid = Guid.NewGuid()
        };

        guidTestClass.FinalizeChanges();

        Assert.NotEqual(Guid.Empty, guidTestClass.NonNullableGuid);
        Assert.Null(guidTestClass.NullableGuid);

        var history = guidTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        guidTestClass.Commit();

        var guidHistoryClass =
            AuditableRootEntity.LoadFromHistory<GuidTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(guidHistoryClass);
        Assert.NotEqual(Guid.Empty, guidHistoryClass.NonNullableGuid);
        Assert.Null(guidHistoryClass.NullableGuid);

        guidHistoryClass.NullableGuid = Guid.NewGuid();

        guidHistoryClass.FinalizeChanges();

        Assert.NotEqual(Guid.Empty, guidHistoryClass.NonNullableGuid);
        Assert.NotEqual(Guid.Empty, guidHistoryClass.NullableGuid);

        var history2 = guidHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        guidHistoryClass.Commit();

        history.AddRange(history2);
        var guidHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<GuidTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(guidHistoryClassTwo);
        Assert.NotEqual(Guid.Empty, guidHistoryClassTwo.NonNullableGuid);
        Assert.NotEqual(Guid.Empty, guidHistoryClassTwo.NullableGuid);

        guidHistoryClassTwo.NullableGuid = null;

        Assert.Null(guidHistoryClassTwo.NullableGuid);

        guidHistoryClassTwo.FinalizeChanges();
        var history3 = guidHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public class GuidTestClass : AuditableRootEntity
    {
        [AuditableValueField<Guid>(true)]
        public Guid? NullableGuid
        {
            get => GetValue<Guid?>(nameof(NullableGuid));
            set => SetValue<Guid?>(value, nameof(NullableGuid));
        }

        [AuditableValueField<Guid>(false)]
        public Guid NonNullableGuid
        {
            get => GetValue<Guid>(nameof(NonNullableGuid));
            set => SetValue(value, nameof(NonNullableGuid));
        }
    
        public GuidTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public GuidTestClass() { }
    }
    
    public class DateTimeOffsetTestClass : AuditableRootEntity
    {
        [AuditableValueField<DateTimeOffset>(true)]
        public DateTimeOffset? NullableDateTimeOffset
        {
            get => GetValue<DateTimeOffset?>(nameof(NullableDateTimeOffset));
            set => SetValue<DateTimeOffset?>(value, nameof(NullableDateTimeOffset));
        }

        [AuditableValueField<DateTimeOffset>(false)]
        public DateTimeOffset NonNullableDateTimeOffset
        {
            get => GetValue<DateTimeOffset>(nameof(NonNullableDateTimeOffset));
            set => SetValue(value, nameof(NonNullableDateTimeOffset));
        }
    
        public DateTimeOffsetTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DateTimeOffsetTestClass() { }
    }
}