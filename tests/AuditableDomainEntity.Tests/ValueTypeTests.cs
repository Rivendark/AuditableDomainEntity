using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests;

public class ValueTypeTests
{
    [Fact]
    public void ValueField_Should_HandleIntTypes()
    {
        var intClass = new IntTestClass
        {
            NonNullableInteger = 12
        };
        
        intClass.FinalizeChanges();
        
        Assert.Equal(12, intClass.NonNullableInteger);
        Assert.Null(intClass.NullableInteger);

        var history = intClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        intClass.Commit();
        
        var intHistoryClass = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClass);
        Assert.Equal(12, intHistoryClass.NonNullableInteger);
        Assert.Null(intHistoryClass.NullableInteger);

        intHistoryClass.NullableInteger = 24;
        
        intHistoryClass.FinalizeChanges();
        
        Assert.Equal(12, intHistoryClass.NonNullableInteger);
        Assert.Equal(24, intHistoryClass.NullableInteger);
        
        var history2 = intHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        intHistoryClass.Commit();

        history.AddRange(history2);
        var intHistoryClassTwo = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClassTwo);
        Assert.Equal(12, intHistoryClassTwo.NonNullableInteger);
        Assert.NotNull(intHistoryClassTwo.NullableInteger);
        Assert.Equal(24, intHistoryClassTwo.NullableInteger);

        intHistoryClassTwo.NullableInteger = null;
        
        Assert.Null(intHistoryClassTwo.NullableInteger);
        
        intHistoryClassTwo.FinalizeChanges();
        var history3 = intHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        intHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var intHistoryClassThree = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClassThree);
        Assert.Equal(12, intHistoryClassThree.NonNullableInteger);
        Assert.Null(intHistoryClassThree.NullableInteger);
    }

    [Fact]
    public void ValueField_Should_HandleLongTypes()
    {
        var longTestClass = new LongTestClass
        {
            NonNullableLong = 12
        };
        
        longTestClass.FinalizeChanges();
        
        Assert.Equal(12, longTestClass.NonNullableLong);
        Assert.Null(longTestClass.NullableLong);
        
        var history = longTestClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        longTestClass.Commit();
        
        var longHistoryClass = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClass);
        Assert.Equal(12, longHistoryClass.NonNullableLong);
        Assert.Null(longHistoryClass.NullableLong);
        
        longHistoryClass.NullableLong = 24;
        
        longHistoryClass.FinalizeChanges();
        
        Assert.Equal(12, longHistoryClass.NonNullableLong);
        Assert.Equal(24, longHistoryClass.NullableLong);
        
        var history2 = longHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        longTestClass.Commit();
        
        history.AddRange(history2);
        var longHistoryClassTwo = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClassTwo);
        Assert.Equal(12, longHistoryClassTwo.NonNullableLong);
        Assert.Equal(24, longHistoryClassTwo.NullableLong);
        
        longHistoryClassTwo.NullableLong = null;
        
        Assert.Null(longHistoryClassTwo.NullableLong);
        
        longHistoryClassTwo.FinalizeChanges();
        var history3 = longHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        
        longHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var longHistoryClassThree = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClassThree);
        Assert.Equal(12, longHistoryClassThree.NonNullableLong);
        Assert.Null(longHistoryClassThree.NullableLong);
    }

    [Fact]
    public void ValueField_Should_HandleDoubleTypes()
    {
        var doubleTestClass = new DoubleTestClass
        {
            NonNullableDouble = 12.0
        };

        doubleTestClass.FinalizeChanges();

        Assert.Equal(12.0, doubleTestClass.NonNullableDouble);
        Assert.Null(doubleTestClass.NullableDouble);

        var history = doubleTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        doubleTestClass.Commit();

        var doubleHistoryClass =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClass);
        Assert.Equal(12.0, doubleHistoryClass.NonNullableDouble);
        Assert.Null(doubleHistoryClass.NullableDouble);

        doubleHistoryClass.NullableDouble = 24.0;

        doubleHistoryClass.FinalizeChanges();

        Assert.Equal(12.0, doubleHistoryClass.NonNullableDouble);
        Assert.Equal(24.0, doubleHistoryClass.NullableDouble);

        var history2 = doubleHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        doubleTestClass.Commit();

        history.AddRange(history2);
        var doubleHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClassTwo);
        Assert.Equal(12.0, doubleHistoryClassTwo.NonNullableDouble);
        Assert.Equal(24.0, doubleHistoryClassTwo.NullableDouble);

        doubleHistoryClassTwo.NullableDouble = null;

        Assert.Null(doubleHistoryClassTwo.NullableDouble);

        doubleHistoryClassTwo.FinalizeChanges();
        var history3 = doubleHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);

        doubleHistoryClassTwo.Commit();

        history.AddRange(history3);
        var doubleHistoryClassThree =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClassThree);
        Assert.Equal(12.0, doubleHistoryClassThree.NonNullableDouble);
        Assert.Null(doubleHistoryClassThree.NullableDouble);
    }

    [Fact]
    public void ValueField_Should_HandleFloatTypes()
    {
        var floatTestClass = new FloatTestClass
        {
            NonNullableFloat = 12.0f
        };

        floatTestClass.FinalizeChanges();

        Assert.Equal(12.0f, floatTestClass.NonNullableFloat);
        Assert.Null(floatTestClass.NullableFloat);

        var history = floatTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        floatTestClass.Commit();

        var floatHistoryClass = AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClass);
        Assert.Equal(12.0f, floatHistoryClass.NonNullableFloat);
        Assert.Null(floatHistoryClass.NullableFloat);

        floatHistoryClass.NullableFloat = 24.0f;

        floatHistoryClass.FinalizeChanges();

        Assert.Equal(12.0f, floatHistoryClass.NonNullableFloat);
        Assert.Equal(24.0f, floatHistoryClass.NullableFloat);

        var history2 = floatHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);

        floatHistoryClass.Commit();

        history.AddRange(history2);
        var floatHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClassTwo);
        Assert.Equal(12.0f, floatHistoryClassTwo.NonNullableFloat);
        Assert.Equal(24.0f, floatHistoryClassTwo.NullableFloat);

        floatHistoryClassTwo.NullableFloat = null;

        Assert.Null(floatHistoryClassTwo.NullableFloat);

        floatHistoryClassTwo.FinalizeChanges();
        var history3 = floatHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);

        floatHistoryClassTwo.Commit();

        history.AddRange(history3);
        var floatHistoryClassThree =
            AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClassThree);
        Assert.Equal(12.0f, floatHistoryClassThree.NonNullableFloat);
        Assert.Null(floatHistoryClassThree.NullableFloat);
    }

    [Fact]
    public void ValueField_Should_HandleDecimalTypes()
    {
        var decimalTestClass = new DecimalTestClass
        {
            NonNullableDecimal = 12.0m
        };
        
        decimalTestClass.FinalizeChanges();
        
        Assert.Equal(12.0m, decimalTestClass.NonNullableDecimal);
        Assert.Null(decimalTestClass.NullableDecimal);
        
        var history = decimalTestClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        decimalTestClass.Commit();
        
        var decimalHistoryClass = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClass);
        Assert.Equal(12.0m, decimalHistoryClass.NonNullableDecimal);
        Assert.Null(decimalHistoryClass.NullableDecimal);
        
        decimalHistoryClass.NullableDecimal = 24.0m;
        
        decimalHistoryClass.FinalizeChanges();
        
        Assert.Equal(12.0m, decimalHistoryClass.NonNullableDecimal);
        Assert.Equal(24.0m, decimalHistoryClass.NullableDecimal);
        
        var history2 = decimalHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        decimalHistoryClass.Commit();
        
        history.AddRange(history2);
        var decimalHistoryClassTwo = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClassTwo);
        Assert.Equal(12.0m, decimalHistoryClassTwo.NonNullableDecimal);
        Assert.Equal(24.0m, decimalHistoryClassTwo.NullableDecimal);
        
        decimalHistoryClassTwo.NullableDecimal = null;
        
        Assert.Null(decimalHistoryClassTwo.NullableDecimal);
        
        decimalHistoryClassTwo.FinalizeChanges();
        var history3 = decimalHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        decimalHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var decimalHistoryClassThree = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClassThree);
        Assert.Equal(12.0m, decimalHistoryClassThree.NonNullableDecimal);
        Assert.Null(decimalHistoryClassThree.NullableDecimal);
    }

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

    [Fact]
    public void ValueField_Should_HandleDateTimeTypes()
    {
        var dateTimeUtcNow = DateTime.UtcNow;
        var dateTimeTestClass = new DateTimeTestClass
        {
            NonNullableDateTime = dateTimeUtcNow
        };

        dateTimeTestClass.FinalizeChanges();

        Assert.Equal(dateTimeUtcNow, dateTimeTestClass.NonNullableDateTime);
        Assert.Null(dateTimeTestClass.NullableDateTime);

        var history = dateTimeTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        dateTimeTestClass.Commit();

        var dateTimeHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeHistoryClass);
        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClass.NonNullableDateTime);
        Assert.Null(dateTimeHistoryClass.NullableDateTime);

        dateTimeHistoryClass.NullableDateTime = dateTimeUtcNow.AddHours(1);

        dateTimeHistoryClass.FinalizeChanges();

        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClass.NonNullableDateTime);
        Assert.Equal(dateTimeUtcNow.AddHours(1), dateTimeHistoryClass.NullableDateTime);

        var history2 = dateTimeHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        dateTimeHistoryClass.Commit();

        history.AddRange(history2);
        var dateTimeHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DateTimeTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeHistoryClassTwo);
        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClassTwo.NonNullableDateTime);
        Assert.Equal(dateTimeUtcNow.AddHours(1), dateTimeHistoryClassTwo.NullableDateTime);

        dateTimeHistoryClassTwo.NullableDateTime = null;

        Assert.Null(dateTimeHistoryClassTwo.NullableDateTime);

        dateTimeHistoryClassTwo.FinalizeChanges();
        var history3 = dateTimeHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }

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

    [Fact]
    public void ValueField_Should_HandleBoolTypes()
    {
        var boolTestClass = new BoolTestClass
        {
            NonNullableBool = true
        };

        boolTestClass.FinalizeChanges();

        Assert.True(boolTestClass.NonNullableBool);
        Assert.Null(boolTestClass.NullableBool);

        var history = boolTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        boolTestClass.Commit();

        var boolHistoryClass =
            AuditableRootEntity.LoadFromHistory<BoolTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolHistoryClass);
        Assert.True(boolHistoryClass.NonNullableBool);
        Assert.Null(boolHistoryClass.NullableBool);

        boolHistoryClass.NullableBool = false;

        boolHistoryClass.FinalizeChanges();

        Assert.True(boolHistoryClass.NonNullableBool);
        Assert.False(boolHistoryClass.NullableBool);

        var history2 = boolHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        boolHistoryClass.Commit();

        history.AddRange(history2);
        var boolHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<BoolTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolHistoryClassTwo);
        Assert.True(boolHistoryClassTwo.NonNullableBool);
        Assert.False(boolHistoryClassTwo.NullableBool);

        boolHistoryClassTwo.NullableBool = null;

        Assert.Null(boolHistoryClassTwo.NullableBool);

        boolHistoryClassTwo.FinalizeChanges();
        var history3 = boolHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);

        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }

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

    [Fact]
    public void ValueField_Should_HandleDateTimeOffsetTypes()
    {
        var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;
        var dateTimeOffsetTestClass = new DateTimeOffsetTestClass
        {
            NonNullableDateTimeOffset = dateTimeOffsetUtcNow
        };

        dateTimeOffsetTestClass.FinalizeChanges();

        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetTestClass.NonNullableDateTimeOffset);
        Assert.Null(dateTimeOffsetTestClass.NullableDateTimeOffset);

        var history = dateTimeOffsetTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        dateTimeOffsetTestClass.Commit();

        var dateTimeOffsetHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeOffsetHistoryClass);
        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClass.NonNullableDateTimeOffset);
        Assert.Null(dateTimeOffsetHistoryClass.NullableDateTimeOffset);

        dateTimeOffsetHistoryClass.NullableDateTimeOffset = dateTimeOffsetUtcNow.AddHours(1);

        dateTimeOffsetHistoryClass.FinalizeChanges();

        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClass.NonNullableDateTimeOffset);
        Assert.Equal(dateTimeOffsetUtcNow.AddHours(1), dateTimeOffsetHistoryClass.NullableDateTimeOffset);

        var history2 = dateTimeOffsetHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        dateTimeOffsetHistoryClass.Commit();

        history.AddRange(history2);
        var dateTimeOffsetHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeOffsetHistoryClassTwo);
        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClassTwo.NonNullableDateTimeOffset);
        Assert.Equal(dateTimeOffsetUtcNow.AddHours(1), dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset);

        dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset = null;

        Assert.Null(dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset);

        dateTimeOffsetHistoryClassTwo.FinalizeChanges();
        var history3 = dateTimeOffsetHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }

    [Fact]
    public void ValueField_Should_HandleTimeSpanTypes()
    {
        var timeSpanTestClass = new TimeSpanTestClass
        {
            NonNullableTimeSpan = TimeSpan.FromHours(1)
        };

        timeSpanTestClass.FinalizeChanges();

        Assert.Equal(TimeSpan.FromHours(1), timeSpanTestClass.NonNullableTimeSpan);
        Assert.Null(timeSpanTestClass.NullableTimeSpan);

        var history = timeSpanTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        timeSpanTestClass.Commit();

        var timeSpanHistoryClass =
            AuditableRootEntity.LoadFromHistory<TimeSpanTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(timeSpanHistoryClass);
        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClass.NonNullableTimeSpan);
        Assert.Null(timeSpanHistoryClass.NullableTimeSpan);

        timeSpanHistoryClass.NullableTimeSpan = TimeSpan.FromHours(2);

        timeSpanHistoryClass.FinalizeChanges();

        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClass.NonNullableTimeSpan);
        Assert.Equal(TimeSpan.FromHours(2), timeSpanHistoryClass.NullableTimeSpan);

        var history2 = timeSpanHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        timeSpanHistoryClass.Commit();

        history.AddRange(history2);
        var timeSpanHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<TimeSpanTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(timeSpanHistoryClassTwo);
        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClassTwo.NonNullableTimeSpan);
        Assert.Equal(TimeSpan.FromHours(2), timeSpanHistoryClassTwo.NullableTimeSpan);

        timeSpanHistoryClassTwo.NullableTimeSpan = null;

        Assert.Null(timeSpanHistoryClassTwo.NullableTimeSpan);

        timeSpanHistoryClassTwo.FinalizeChanges();
        var history3 = timeSpanHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
}

public class IntTestClass : AuditableRootEntity
{
    [AuditableValueField<int?>(true)]
    public int? NullableInteger
    {
        get => GetValue<int?>(nameof(NullableInteger));
        set => SetValue<int?>(value, nameof(NullableInteger));
    }
    
    [AuditableValueField<int>(false)]
    public int NonNullableInteger
    {
        get => GetValue<int>(nameof(NonNullableInteger));
        set => SetValue<int>(value, nameof(NonNullableInteger));
    }
    
    public IntTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public IntTestClass() { }
}

public class LongTestClass : AuditableRootEntity
{
    [AuditableValueField<long?>(true)]
    public long? NullableLong
    {
        get => GetValue<long?>(nameof(NullableLong));
        set => SetValue<long?>(value, nameof(NullableLong));
    }

    [AuditableValueField<long>(false)]
    public long NonNullableLong
    {
        get => GetValue<long>(nameof(NonNullableLong));
        set => SetValue(value, nameof(NonNullableLong));
    }
    
    public LongTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public LongTestClass() { }
}

public class DoubleTestClass : AuditableRootEntity
{
    [AuditableValueField<double?>(true)]
    public double? NullableDouble
    {
        get => GetValue<double?>(nameof(NullableDouble));
        set => SetValue<double?>(value, nameof(NullableDouble));
    }

    [AuditableValueField<double>(false)]
    public double NonNullableDouble
    {
        get => GetValue<double>(nameof(NonNullableDouble));
        set => SetValue(value, nameof(NonNullableDouble));
    }
    
    public DoubleTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DoubleTestClass() { }
}

public class FloatTestClass : AuditableRootEntity
{
    [AuditableValueField<float?>(true)]
    public float? NullableFloat
    {
        get => GetValue<float?>(nameof(NullableFloat));
        set => SetValue<float?>(value, nameof(NullableFloat));
    }

    [AuditableValueField<float>(false)]
    public float NonNullableFloat
    {
        get => GetValue<float>(nameof(NonNullableFloat));
        set => SetValue(value, nameof(NonNullableFloat));
    }
    
    public FloatTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public FloatTestClass() { }
}

public class DecimalTestClass : AuditableRootEntity
{
    [AuditableValueField<decimal?>(true)]
    public decimal? NullableDecimal
    {
        get => GetValue<decimal?>(nameof(NullableDecimal));
        set => SetValue<decimal?>(value, nameof(NullableDecimal));
    }

    [AuditableValueField<decimal>(false)]
    public decimal NonNullableDecimal
    {
        get => GetValue<decimal>(nameof(NonNullableDecimal));
        set => SetValue(value, nameof(NonNullableDecimal));
    }
    
    public DecimalTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DecimalTestClass() { }
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

public class DateTimeTestClass : AuditableRootEntity
{
    [AuditableValueField<DateTime>(true)]
    public DateTime? NullableDateTime
    {
        get => GetValue<DateTime?>(nameof(NullableDateTime));
        set => SetValue<DateTime?>(value, nameof(NullableDateTime));
    }

    [AuditableValueField<DateTime>(false)]
    public DateTime NonNullableDateTime
    {
        get => GetValue<DateTime>(nameof(NonNullableDateTime));
        set => SetValue(value, nameof(NonNullableDateTime));
    }
    
    public DateTimeTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DateTimeTestClass() { }
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

public class BoolTestClass : AuditableRootEntity
{
    [AuditableValueField<bool>(true)]
    public bool? NullableBool
    {
        get => GetValue<bool?>(nameof(NullableBool));
        set => SetValue<bool?>(value, nameof(NullableBool));
    }

    [AuditableValueField<bool>(false)]
    public bool NonNullableBool
    {
        get => GetValue<bool>(nameof(NonNullableBool));
        set => SetValue(value, nameof(NonNullableBool));
    }
    
    public BoolTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public BoolTestClass() { }
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

public class TimeSpanTestClass : AuditableRootEntity
{
    [AuditableValueField<TimeSpan?>(true)]
    public TimeSpan? NullableTimeSpan
    {
        get => GetValue<TimeSpan?>(nameof(NullableTimeSpan));
        set => SetValue<TimeSpan?>(value, nameof(NullableTimeSpan));
    }

    [AuditableValueField<TimeSpan>(false)]
    public TimeSpan NonNullableTimeSpan
    {
        get => GetValue<TimeSpan>(nameof(NonNullableTimeSpan));
        set => SetValue(value, nameof(NonNullableTimeSpan));
    }
    
    public TimeSpanTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public TimeSpanTestClass() { }
}