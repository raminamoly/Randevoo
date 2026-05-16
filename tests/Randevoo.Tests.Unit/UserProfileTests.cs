using System.Reflection;
using Xunit;

 
using System;
using System.Linq;
using Xunit;
using Randevoo.Domain.Entities;
using Randevoo.Domain.ValueObjects;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Tests.Unit
{
    public class UserProfileTests
    {
        private static User CreateUser() {
            return new User(email : "Ramin.Amoly@gamil.com", passwordHash : "123");
        }
          

        private static Location CreateLocation() =>
            new Location("USA", "Seattle", new Coordinates(47.6062m, -122.3321m));

        private static Interest CreateInterest(string name = "Hiking") =>
            new Interest(name);

        private static UserProfile CreateValidProfile(DateOnly? dob = null) =>
            new UserProfile(
                user: CreateUser(),
                displayName: "Alice",
                dateOfBirth: dob ?? DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30),
                gender: Gender.Female,
                location: CreateLocation(),
                height: new Height(165)
            );

        [Fact]
        public void Constructor_WithValidData_SetsDefaultsAndAddsCreatedEvent()
        {
            var profile = CreateValidProfile();

            Assert.Equal("Alice", profile.DisplayName);
            Assert.Equal(Gender.Female, profile.Gender);
            Assert.Equal(EducationLevel.NotSpecified, profile.EducationLevel);
            Assert.False(profile.Smoking);
            Assert.NotNull(profile.Location);
            Assert.True(profile.Age >= 30 - 1); // basic sanity; exact depends on date
            Assert.Contains(profile.DomainEvents, e => e is EntityCreatedEvent<UserProfile>);
        }

        [Fact]
        public void UpdateDisplayName_ValidName_UpdatesAndAddsEvent()
        {
            var profile = CreateValidProfile();
            profile.ClearDomainEvents();

            profile.UpdateDisplayName("Bob");

            Assert.Equal("Bob", profile.DisplayName);
            Assert.Contains(profile.DomainEvents, e => e is EntityUpdatedEvent<UserProfile>);
        }

        [Fact]
        public void AddInterest_NewInterest_AddsAndIncrementsUsageAndAddsEvent()
        {
            var profile = CreateValidProfile();
            var interest = CreateInterest("Hiking");
            profile.ClearDomainEvents();

            profile.AddInterest(interest);

            Assert.Contains(interest, profile.Interests);
            Assert.Equal(1, interest.UsageCount);
            Assert.Contains(profile.DomainEvents, e => e is InterestAddedEvent);
        }

        [Fact]
        public void AddInterest_Duplicate_ThrowsBusinessRuleViolationException()
        {
            var profile = CreateValidProfile();
            var interest = CreateInterest("Cooking");

            profile.AddInterest(interest);
            var ex = Assert.Throws<BusinessRuleViolationException>(() => profile.AddInterest(interest));
            Assert.NotNull(ex);
        }

        [Fact]
        public void AddInterest_ExceedMax_ThrowsBusinessRuleViolationException()
        {
            var profile = CreateValidProfile();

            for (int i = 0; i < 10; i++)
            {
                profile.AddInterest(CreateInterest($"I{i}"));
            }

            var ex = Assert.Throws<BusinessRuleViolationException>(() => profile.AddInterest(CreateInterest("Overflow")));
            Assert.NotNull(ex);
        }

        [Fact]
        public void RemoveInterest_NotFound_ThrowsBusinessRuleViolationException()
        {
            var profile = CreateValidProfile();
            var interest = CreateInterest("NonExisting");

            var ex = Assert.Throws<BusinessRuleViolationException>(() => profile.RemoveInterest(interest));
            Assert.NotNull(ex);
        }

        [Fact]
        public void RemoveInterest_Existing_RemovesAndDecrementsUsageAndAddsEvent()
        {
            var profile = CreateValidProfile();
            var interest = CreateInterest("Travel");
            profile.AddInterest(interest);
            profile.ClearDomainEvents();

            profile.RemoveInterest(interest);

            Assert.DoesNotContain(interest, profile.Interests);
            Assert.Equal(0, interest.UsageCount);
            Assert.Contains(profile.DomainEvents, e => e is InterestRemovedEvent);
        }

        [Fact]
        public void SoftDelete_SetsIsDeletedAndAddsUpdatedEvent()
        {
            var profile = CreateValidProfile();
            profile.ClearDomainEvents();

            profile.SoftDelete();

            Assert.True(profile.IsDeleted);
            Assert.Contains(profile.DomainEvents, e => e is EntityUpdatedEvent<UserProfile>);
        }

        [Fact]  
        public void Age_Calculation_IsConsistent()
        {
            // Ensure a deterministic birthday: today minus 20 years (birthday today)
            var birth = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-20);
            var profile = CreateValidProfile(birth);

            Assert.Equal(20, profile.Age);
        }
    }
}