using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class ByteValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleByteTypes()
    {
        var byteTestClass = new ByteTestClass
        {
            NonNullableByte = 12
        };

        byteTestClass.FinalizeChanges();

        Assert.Equal(12, byteTestClass.NonNullableByte);
        Assert.Null(byteTestClass.NullableByte);

        var history = byteTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        byteTestClass.Commit();

        var byteHistoryClass = AuditableRootEntity.LoadFromHistory<ByteTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(byteHistoryClass);
        Assert.Equal(12, byteHistoryClass.NonNullableByte);
        Assert.Null(byteHistoryClass.NullableByte);

        byteHistoryClass.NullableByte = 12;

        byteHistoryClass.FinalizeChanges();

        Assert.Equal(12, byteHistoryClass.NonNullableByte);
        Assert.Equal((byte?)12, byteHistoryClass.NullableByte);

        var history2 = byteHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        byteTestClass.Commit();

        history.AddRange(history2);
        var byteHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<ByteTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(byteHistoryClassTwo);
        Assert.Equal(12, byteHistoryClassTwo.NonNullableByte);
        Assert.Equal((byte?)12, byteHistoryClassTwo.NullableByte);

        byteHistoryClassTwo.NullableByte = null;

        Assert.Null(byteHistoryClassTwo.NullableByte);

        byteHistoryClassTwo.FinalizeChanges();
        var history3 = byteHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public class ByteTestClass : AuditableRootEntity
    {
        [AuditableValueField<byte?>(true)]
        public byte? NullableByte
        {
            get => GetValue<byte?>(nameof(NullableByte));
            set => SetValue<byte?>(value, nameof(NullableByte));
        }

        [AuditableValueField<byte>(false)]
        public byte NonNullableByte
        {
            get => GetValue<byte>(nameof(NonNullableByte));
            set => SetValue(value, nameof(NonNullableByte));
        }
    
        public ByteTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public ByteTestClass() { }
    }
}