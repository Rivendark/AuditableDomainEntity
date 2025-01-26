using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class EnumValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleEnumTypes()
    {
        var enumTestClass = new EnumTestClass
        {
            NonNullableBasicEnum = TestBasicEnum.Value1,
            NonNullableStringEnum = TestStringEnum.Value1
        };

        enumTestClass.FinalizeChanges();

        Assert.Equal(TestBasicEnum.Value1, enumTestClass.NonNullableBasicEnum);
        Assert.Null(enumTestClass.NullableBasicEnum);
        Assert.Equal(TestStringEnum.Value1, enumTestClass.NonNullableStringEnum);
        Assert.Null(enumTestClass.NullableStringEnum);

        var history = enumTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Equal(2, auditableEntityCreated.ValueFieldEvents.Count);

        enumTestClass.Commit();

        var enumHistoryClass = AuditableRootEntity.LoadFromHistory<EnumTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(enumHistoryClass);
        Assert.Equal(TestBasicEnum.Value1, enumHistoryClass.NonNullableBasicEnum);

        Assert.Null(enumHistoryClass.NullableBasicEnum);
        Assert.Equal(TestStringEnum.Value1, enumHistoryClass.NonNullableStringEnum);
        Assert.Null(enumHistoryClass.NullableStringEnum);

        enumHistoryClass.NullableBasicEnum = TestBasicEnum.Value2;
        enumHistoryClass.NullableStringEnum = TestStringEnum.Value2;

        enumHistoryClass.FinalizeChanges();

        Assert.Equal(TestBasicEnum.Value1, enumHistoryClass.NonNullableBasicEnum);
        Assert.Equal(TestBasicEnum.Value2, enumHistoryClass.NullableBasicEnum);

        Assert.Equal(TestStringEnum.Value1, enumHistoryClass.NonNullableStringEnum);
        Assert.Equal(TestStringEnum.Value2, enumHistoryClass.NullableStringEnum);

        var history2 = enumHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Equal(2, auditableEntityUpdated.ValueFieldEvents.Count);

        enumHistoryClass.Commit();

        history.AddRange(history2);
        var enumHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<EnumTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(enumHistoryClassTwo);
        Assert.Equal(TestBasicEnum.Value1, enumHistoryClassTwo.NonNullableBasicEnum);
        Assert.Equal(TestBasicEnum.Value2, enumHistoryClassTwo.NullableBasicEnum);

        Assert.Equal(TestStringEnum.Value1, enumHistoryClassTwo.NonNullableStringEnum);
        Assert.Equal(TestStringEnum.Value2, enumHistoryClassTwo.NullableStringEnum);

        enumHistoryClassTwo.NullableBasicEnum = null;
        enumHistoryClassTwo.NullableStringEnum = null;

        Assert.Null(enumHistoryClassTwo.NullableBasicEnum);
        Assert.Null(enumHistoryClassTwo.NullableStringEnum);

        enumHistoryClassTwo.FinalizeChanges();
        var history3 = enumHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public enum TestBasicEnum
    {
        Value1,
        Value2,
    }

    public enum TestStringEnum
    {
        Value1 = 'A',
        Value2 = 'B',
    }

    public class EnumTestClass : AuditableRootEntity
    {
        [AuditableValueField<TestBasicEnum>(true)]
        public TestBasicEnum? NullableBasicEnum
        {
            get => GetValue<TestBasicEnum?>(nameof(NullableBasicEnum));
            set => SetValue<TestBasicEnum?>(value, nameof(NullableBasicEnum));
        }

        [AuditableValueField<TestBasicEnum>(false)]
        public TestBasicEnum NonNullableBasicEnum
        {
            get => GetValue<TestBasicEnum>(nameof(NonNullableBasicEnum));
            set => SetValue(value, nameof(NonNullableBasicEnum));
        }

        [AuditableValueField<TestStringEnum>(true)]
        public TestStringEnum? NullableStringEnum
        {
            get => GetValue<TestStringEnum?>(nameof(NullableStringEnum));
            set => SetValue(value, nameof(NullableStringEnum));
        }

        [AuditableValueField<TestStringEnum>(false)]
        public TestStringEnum NonNullableStringEnum
        {
            get => GetValue<TestStringEnum>(nameof(NonNullableStringEnum));
            set => SetValue(value, nameof(NonNullableStringEnum));
        }
    
        public EnumTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public EnumTestClass() { }
    }
}