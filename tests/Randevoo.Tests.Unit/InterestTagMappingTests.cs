using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Xunit;

namespace Randevoo.Tests.Unit;

public class InterestTagMappingTests
{
    [Fact]
    public void Constructor_StoresInterestTagAndWeight()
    {
        var interest = new Interest("Live Music");
        var tag = new Tag("Concert");

        var mapping = new InterestTagMapping(interest, tag, 75);

        Assert.Same(interest, mapping.Interest);
        Assert.Same(tag, mapping.Tag);
        Assert.Equal(75, mapping.RelevanceWeight);
        Assert.True(mapping.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Constructor_RejectsWeightOutsideRange(int weight)
    {
        Assert.Throws<BusinessRuleViolationException>(() =>
            new InterestTagMapping(new Interest("Art"), new Tag("Gallery"), weight));
    }
}
