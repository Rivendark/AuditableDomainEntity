using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class EnumArrayValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleEnumArrayTypes()
    {
        var enumArrayTestClass = new EnumArrayTestClass
        {
            NonNullableBasicEnumArray = [TestBasicEnum.Value1, TestBasicEnum.Value2],
            NullableBasicEnumArray = [TestBasicEnum.Value1, TestBasicEnum.Value2],
            NonNullableStringEnumArray = [TestStringEnum.Value1, TestStringEnum.Value2],
            NullableStringEnumArray = [TestStringEnum.Value1, TestStringEnum.Value2]
        };

        enumArrayTestClass.FinalizeChanges();

        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayTestClass.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayTestClass.NullableBasicEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayTestClass.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayTestClass.NullableStringEnumArray);
        
        var history = enumArrayTestClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Equal(4, auditableEntityCreated.ValueFieldEvents.Count);
        
        enumArrayTestClass.Commit();
        
        var enumArrayHistoryClass = AuditableRootEntity.LoadFromHistory<EnumArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(enumArrayHistoryClass);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass.NullableBasicEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass.NullableStringEnumArray);
        
        enumArrayHistoryClass.NullableBasicEnumArray = [TestBasicEnum.Value2, TestBasicEnum.Value1];
        enumArrayHistoryClass.NullableStringEnumArray = [TestStringEnum.Value2, TestStringEnum.Value1];
        
        enumArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value2, TestBasicEnum.Value1], enumArrayHistoryClass.NullableBasicEnumArray);
        
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value2, TestStringEnum.Value1], enumArrayHistoryClass.NullableStringEnumArray);
        
        var history2 = enumArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        
        var valueFieldEvents = auditableEntityUpdated.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents);
        Assert.NotEmpty(valueFieldEvents);
        Assert.Equal(2, valueFieldEvents.Count);
        
        enumArrayHistoryClass.Commit();
        
        history.AddRange(history2);
        
        var enumArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<EnumArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(enumArrayHistoryClass2);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass2.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value2, TestBasicEnum.Value1], enumArrayHistoryClass2.NullableBasicEnumArray);
        
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass2.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value2, TestStringEnum.Value1], enumArrayHistoryClass2.NullableStringEnumArray);
        
        enumArrayHistoryClass2.NullableBasicEnumArray = [TestBasicEnum.Value1, TestBasicEnum.Value2];
        enumArrayHistoryClass2.NullableStringEnumArray = [TestStringEnum.Value1, TestStringEnum.Value2];
        
        enumArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass2.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass2.NullableBasicEnumArray);
        
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass2.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass2.NullableStringEnumArray);
        
        var history3 = enumArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        
        var valueFieldEvents2 = auditableEntityUpdated2.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents2);
        Assert.NotEmpty(valueFieldEvents2);
        Assert.Equal(2, valueFieldEvents2.Count);
        
        enumArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        
        var enumArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<EnumArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(enumArrayHistoryClass3);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass3.NonNullableBasicEnumArray);
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass3.NullableBasicEnumArray);
        
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass3.NonNullableStringEnumArray);
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass3.NullableStringEnumArray);
        
        enumArrayHistoryClass3.NullableBasicEnumArray = null;
        enumArrayHistoryClass3.NullableStringEnumArray = null;
        
        enumArrayHistoryClass3.FinalizeChanges();
        
        Assert.Equal([TestBasicEnum.Value1, TestBasicEnum.Value2], enumArrayHistoryClass3.NonNullableBasicEnumArray);
        Assert.Null(enumArrayHistoryClass3.NullableBasicEnumArray);
        
        Assert.Equal([TestStringEnum.Value1, TestStringEnum.Value2], enumArrayHistoryClass3.NonNullableStringEnumArray);
        Assert.Null(enumArrayHistoryClass3.NullableStringEnumArray);
        
        var history4 = enumArrayHistoryClass3.GetEntityChanges();
        
        Assert.Single(history4);
        
        var firstEvent4 = history4[0];
        
        Assert.NotNull(firstEvent4);
        Assert.IsType<AuditableEntityUpdated>(firstEvent4);
        
        var auditableEntityUpdated3 = firstEvent4 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated3);
        
        var valueFieldEvents3 = auditableEntityUpdated3.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents3);
        Assert.NotEmpty(valueFieldEvents3);
        Assert.Equal(2, valueFieldEvents3.Count);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var enumArrayClass = new EnumArrayTestClass
        {
            NonNullableBasicEnumArray = [],
            NullableBasicEnumArray = [],
            NonNullableStringEnumArray = [],
            NullableStringEnumArray = []
        };

        enumArrayClass.FinalizeChanges();

        Assert.Empty(enumArrayClass.NonNullableBasicEnumArray);
        Assert.Empty(enumArrayClass.NullableBasicEnumArray);
        Assert.Empty(enumArrayClass.NonNullableStringEnumArray);
        Assert.Empty(enumArrayClass.NullableStringEnumArray);
        
        var history = enumArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Equal(4, auditableEntityCreated.ValueFieldEvents.Count);
        
        enumArrayClass.Commit();
        
        var enumArrayHistoryClass = AuditableRootEntity.LoadFromHistory<EnumArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(enumArrayHistoryClass);
        Assert.NotNull(enumArrayHistoryClass.NonNullableBasicEnumArray);
        Assert.Empty(enumArrayHistoryClass.NonNullableBasicEnumArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var enumArrayClass = new EnumArrayTestClass
        {
            NonNullableBasicEnumArray = null,
            NullableBasicEnumArray = null,
            NonNullableStringEnumArray = null,
            NullableStringEnumArray = null
        };

        enumArrayClass.FinalizeChanges();

        Assert.Null(enumArrayClass.NonNullableBasicEnumArray);
        Assert.Null(enumArrayClass.NullableBasicEnumArray);
        Assert.Null(enumArrayClass.NonNullableStringEnumArray);
        Assert.Null(enumArrayClass.NullableStringEnumArray);
        
        var history = enumArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        enumArrayClass.Commit();
        
        var enumArrayHistoryClass = AuditableRootEntity.LoadFromHistory<EnumArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(enumArrayHistoryClass);
        Assert.Null(enumArrayHistoryClass.NonNullableBasicEnumArray);
        Assert.Null(enumArrayHistoryClass.NullableBasicEnumArray);
        Assert.Null(enumArrayHistoryClass.NonNullableStringEnumArray);
        Assert.Null(enumArrayHistoryClass.NullableStringEnumArray);
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
    
    public class EnumArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<TestBasicEnum[]>(true)]
        public TestBasicEnum[]? NullableBasicEnumArray
        {
            get => GetValue<TestBasicEnum[]?>(nameof(NullableBasicEnumArray));
            set => SetValue<TestBasicEnum[]?>(value, nameof(NullableBasicEnumArray));
        }

        [AuditableValueField<TestBasicEnum[]>(false)]
        public TestBasicEnum[] NonNullableBasicEnumArray
        {
            get => GetValue<TestBasicEnum[]>(nameof(NonNullableBasicEnumArray));
            set => SetValue(value, nameof(NonNullableBasicEnumArray));
        }

        [AuditableValueField<TestStringEnum[]>(true)]
        public TestStringEnum[]? NullableStringEnumArray
        {
            get => GetValue<TestStringEnum[]?>(nameof(NullableStringEnumArray));
            set => SetValue(value, nameof(NullableStringEnumArray));
        }

        [AuditableValueField<TestStringEnum[]>(false)]
        public TestStringEnum[] NonNullableStringEnumArray
        {
            get => GetValue<TestStringEnum[]>(nameof(NonNullableStringEnumArray));
            set => SetValue(value, nameof(NonNullableStringEnumArray));
        }
        
        public EnumArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public EnumArrayTestClass() { }
    }
}