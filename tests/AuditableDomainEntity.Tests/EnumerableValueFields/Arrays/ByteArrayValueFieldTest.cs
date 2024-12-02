using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Arrays;

public class ByteArrayValueFieldTest
{
    private static readonly byte[] ExpectedNonNull = [1, 2, 3];
    private static readonly byte[] ExpectedNullable = [4, 5, 6];

    [Fact]
    public void ValueField_Should_HandleByteArrayTypes()
    {
        var byteArrayClass = new ByteArrayTestClass
        {
            NonNullableByteArray = [1, 2, 3]
        };
        
        byteArrayClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, byteArrayClass.NonNullableByteArray);
        Assert.Null(byteArrayClass.NullableByteArray);

        var history = byteArrayClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        byteArrayClass.Commit();
        
        var byteArrayHistoryClass = AuditableRootEntity.LoadFromHistory<ByteArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(byteArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, byteArrayHistoryClass.NonNullableByteArray);
        Assert.Null(byteArrayHistoryClass.NullableByteArray);

        byteArrayHistoryClass.NullableByteArray = [4, 5, 6];
        
        byteArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, byteArrayHistoryClass.NonNullableByteArray);
        Assert.Equal(ExpectedNullable, byteArrayHistoryClass.NullableByteArray);
        
        var history2 = byteArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        
        var valueFieldEvents = auditableEntityUpdated.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents);
        Assert.NotEmpty(valueFieldEvents);
        Assert.Single(valueFieldEvents);
        
        byteArrayHistoryClass.Commit();
        history.AddRange(history2);
        var byteArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<ByteArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(byteArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, byteArrayHistoryClass2.NonNullableByteArray);
        
        Assert.Equal(ExpectedNullable, byteArrayHistoryClass2.NullableByteArray);
        
        byteArrayHistoryClass2.NullableByteArray = null;
        
        byteArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, byteArrayHistoryClass2.NonNullableByteArray);
        Assert.Null(byteArrayHistoryClass2.NullableByteArray);
        
        var history3 = byteArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        
        var valueFieldEvents2 = auditableEntityUpdated2.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents2);
        Assert.NotEmpty(valueFieldEvents2);
        Assert.Single(valueFieldEvents2);
        
        byteArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        var byteArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<ByteArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(byteArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, byteArrayHistoryClass3.NonNullableByteArray);
        Assert.Null(byteArrayHistoryClass3.NullableByteArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var byteArrayClass = new ByteArrayTestClass
        {
            NonNullableByteArray = Array.Empty<byte>()
        };
        
        byteArrayClass.FinalizeChanges();
        
        Assert.Empty(byteArrayClass.NonNullableByteArray);
        Assert.Null(byteArrayClass.NullableByteArray);

        var history = byteArrayClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        byteArrayClass.Commit();
        
        var byteArrayHistoryClass = AuditableRootEntity.LoadFromHistory<ByteArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(byteArrayHistoryClass);
        Assert.Empty(byteArrayHistoryClass.NonNullableByteArray);
        Assert.Null(byteArrayHistoryClass.NullableByteArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var byteArrayClass = new ByteArrayTestClass
        {
            NonNullableByteArray = null
        };

        byteArrayClass.FinalizeChanges();

        Assert.Null(byteArrayClass.NonNullableByteArray);
        Assert.Null(byteArrayClass.NullableByteArray);

        var history = byteArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        byteArrayClass.Commit();

        var byteArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<ByteArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(byteArrayHistoryClass);
        Assert.Null(byteArrayHistoryClass.NonNullableByteArray);
        Assert.Null(byteArrayHistoryClass.NullableByteArray);
    }

    public class ByteArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<byte[]>(true)]
        public byte[]? NullableByteArray
        {
            get => GetValue<byte[]?>(nameof(NullableByteArray));
            set => SetValue<byte[]?>(value, nameof(NullableByteArray));
        }
        
        [AuditableValueField<byte[]>(false)]
        public byte[] NonNullableByteArray
        {
            get => GetValue<byte[]>(nameof(NonNullableByteArray));
            set => SetValue<byte[]>(value, nameof(NonNullableByteArray));
        }
        
        public ByteArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public ByteArrayTestClass() { }
    }
}