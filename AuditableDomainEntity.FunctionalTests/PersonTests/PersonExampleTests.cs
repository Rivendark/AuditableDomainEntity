using AuditableDomainEntity.Interfaces;

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
            PrimaryAddress = new AddressDbo
            {
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                ZipCode = zipCode
            },
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
        Assert.Equal(5, history.Count);
    }

    [Fact]
    public void Should_LoadPerson_FromHistory()
    {
        var person = GeneratePerson();
        var (rootId, history) = GeneratePersonHistory(person);
        
        // Act
        var personFromHistory = AuditableRootEntity.LoadFromHistory<PersonDbo>(rootId, history);
        
        // Assert
        Assert.NotNull(personFromHistory);
        Assert.Equal(person.FirstName, personFromHistory.FirstName);
        Assert.Equal(person.LastName, personFromHistory.LastName);
        
        Assert.NotNull(person.Tags);
        Assert.NotEmpty(person.Tags);
        Assert.NotNull(personFromHistory.Tags);
        Assert.NotEmpty(personFromHistory.Tags);
        
        Assert.Equal(person.Tags.Count, personFromHistory.Tags.Count);
        Assert.Equal(person.Tags[0], personFromHistory.Tags[0]);
        Assert.Equal(person.Tags[1], personFromHistory.Tags[1]);
        
        Assert.NotNull(person.PrimaryAddress);
        Assert.NotNull(personFromHistory.PrimaryAddress);
        Assert.Equal(person.PrimaryAddress.AddressLine1, personFromHistory.PrimaryAddress.AddressLine1);
        Assert.Equal(person.PrimaryAddress.AddressLine2, personFromHistory.PrimaryAddress.AddressLine2);
        Assert.Equal(person.PrimaryAddress.City, personFromHistory.PrimaryAddress.City);
        Assert.Equal(person.PrimaryAddress.State, personFromHistory.PrimaryAddress.State);
        Assert.Equal(person.PrimaryAddress.ZipCode, personFromHistory.PrimaryAddress.ZipCode);
        
        Assert.NotNull(person.Addresses);
        Assert.NotEmpty(person.Addresses);
        Assert.NotNull(personFromHistory.Addresses);
        Assert.NotEmpty(personFromHistory.Addresses);
        
        Assert.Equal(person.Addresses.Count, personFromHistory.Addresses.Count);
        Assert.Equal(person.Addresses[0].AddressLine1, personFromHistory.Addresses[0].AddressLine1);
        Assert.Equal(person.Addresses[0].AddressLine2, personFromHistory.Addresses[0].AddressLine2);
        Assert.Equal(person.Addresses[0].City, personFromHistory.Addresses[0].City);
        Assert.Equal(person.Addresses[0].State, personFromHistory.Addresses[0].State);
        Assert.Equal(person.Addresses[0].ZipCode, personFromHistory.Addresses[0].ZipCode);
        
        Assert.NotNull(person.Emails);
        Assert.NotEmpty(person.Emails);
        Assert.NotNull(personFromHistory.Emails);
        Assert.NotEmpty(personFromHistory.Emails);
        
        Assert.Equal(person.Emails.Count, personFromHistory.Emails.Count);
        Assert.Equal(person.Emails[0].EmailAddress, personFromHistory.Emails[0].EmailAddress);
        Assert.Equal(person.Emails[1].EmailAddress, personFromHistory.Emails[1].EmailAddress);
    }

    private PersonDbo GeneratePerson()
    {
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
        var person = new PersonDbo
        {
            FirstName = firstName,
            LastName = lastName,
            Tags =
            [
                tag1,
                tag2
            ],
            PrimaryAddress = new AddressDbo
            {
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                State = state,
                ZipCode = zipCode
            },
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

        return person;
    }
    
    private (AggregateRootId, List<IDomainEntityEvent>) GeneratePersonHistory(PersonDbo person)
    {
        person.FinalizeChanges();

        var history = person.GetEntityChanges();
        
        person.Commit();

        return (person.AggregateRootId, history);
    }
}