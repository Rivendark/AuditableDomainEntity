using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class StringValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleStringTypes()
    {
        var stringTestClass = new StringTestClass
        {
            NonNullableString = "Test"
        };

        stringTestClass.FinalizeChanges();

        Assert.Equal("Test", stringTestClass.NonNullableString);
        Assert.Null(stringTestClass.NullableString);

        var history = stringTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        stringTestClass.Commit();

        var stringHistoryClass =
            AuditableRootEntity.LoadFromHistory<StringTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(stringHistoryClass);
        Assert.Equal("Test", stringHistoryClass.NonNullableString);
        Assert.Null(stringHistoryClass.NullableString);

        stringHistoryClass.NullableString = "Test2";

        stringHistoryClass.FinalizeChanges();

        Assert.Equal("Test", stringHistoryClass.NonNullableString);
        Assert.Equal("Test2", stringHistoryClass.NullableString);

        var history2 = stringHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        stringHistoryClass.Commit();

        history.AddRange(history2);
        var stringHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<StringTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(stringHistoryClassTwo);
        Assert.Equal("Test", stringHistoryClassTwo.NonNullableString);
        Assert.Equal("Test2", stringHistoryClassTwo.NullableString);

        stringHistoryClassTwo.NullableString = null;

        Assert.Null(stringHistoryClassTwo.NullableString);

        stringHistoryClassTwo.FinalizeChanges();
        var history3 = stringHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);

        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public class StringTestClass : AuditableRootEntity
    {
        [AuditableValueField<string>(true)]
        public string? NullableString
        {
            get => GetValue<string?>(nameof(NullableString));
            set => SetValue<string?>(value, nameof(NullableString));
        }

        [AuditableValueField<string>(false)]
        public string NonNullableString
        {
            get => GetValue<string>(nameof(NonNullableString));
            set => SetValue<string>(value, nameof(NonNullableString));
        }
    
        public StringTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public StringTestClass() { }
    }
}