namespace AuditableDomainEntity.FunctionalTests.PersonTests;

public class PersonExampleTests
{
    [Fact]
    public void Should_Create_Person()
    {
        // Arrange
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(PersonDbo));
        var bogus = new Bogus.Faker();
        var firstName = bogus.Name.FirstName();
        var lastName = bogus.Name.LastName();
        var tag1 = bogus.Random.Word();
        var tag2 = bogus.Random.Word();
        var addressLine1 = bogus.Address.StreetAddress();
        var addressLine2 = bogus.Address.SecondaryAddress();
        var city = bogus.Address.City();
        var state = bogus.Address.StateAbbr();
        var zipCode = bogus.Address.ZipCode();
        var email1 = bogus.Internet.Email();
        var email2 = bogus.Internet.Email();
    
        // Act
        var person = new PersonDbo(aggregateRootId)
        {
            FirstName = firstName,
            LastName = lastName,
            Tags =
            [
                tag1,
                tag2
            ],
            Addresses =
            [
                new AddressDbo
                {
                    AddressLine1 = addressLine1,
                    AddressLine2 = addressLine2,
                    City = city,
                    State = state,
                    ZipCode = zipCode
                }
            ],
            Emails =
            [
                new EmailDbo
                {
                    EmailAddress = email1
                },
                new EmailDbo
                {
                    EmailAddress = email2
                }
            ]
        };
    
        // Assert
        Assert.Equal(firstName, person.FirstName);
        Assert.Equal(lastName, person.LastName);
        Assert.Equal(2, person.Tags.Count);
        Assert.Equal(tag1, person.Tags[0]);
        Assert.Equal(tag2, person.Tags[1]);
        Assert.Single(person.Addresses);
        Assert.Equal(addressLine1, person.Addresses[0].AddressLine1);
        Assert.Equal(addressLine2, person.Addresses[0].AddressLine2);
        Assert.Equal(city, person.Addresses[0].City);
        Assert.Equal(state, person.Addresses[0].State);
        Assert.Equal(zipCode, person.Addresses[0].ZipCode);
        Assert.Equal(2, person.Emails.Count);
        Assert.Equal(email1, person.Emails[0].EmailAddress);
        Assert.Equal(email2, person.Emails[1].EmailAddress);
        
        // Act
        person.FinalizeChanges();

        var history = person.GetEntityChanges();
        
        person.Commit();
        
        // Assert
        Assert.Equal(4, history.Count);
    }
}