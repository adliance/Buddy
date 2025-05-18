using Xunit;

namespace Adliance.Buddy.DateTime.Test.Extensions;

public class DateTimeExtensionsTest
{
    [Theory]
    [InlineData("2022-07-19 12:34:56", "2022-07-19 14:34:56")]
    [InlineData("2022-12-18 12:34:56", "2022-12-18 13:34:56")]
    public void Can_Convert_Utc_To_Cet(string originalDate, string expectedDate)
    {
        Assert.Equal(System.DateTime.Parse(expectedDate), System.DateTime.Parse(originalDate).UtcToCet());
    }

    [Fact]
    public void Can_Support_Null_When_Converting_Utc_To_Cet()
    {
        Assert.Null((null as System.DateTime?).UtcToCet());
    }

    [Theory]
    [InlineData("2022-07-19 12:34:56", "2022-07-19 10:34:56")]
    [InlineData("2022-12-18 12:34:56", "2022-12-18 11:34:56")]
    public void Can_Convert_Cet_To_Utc(string originalDate, string expectedDate)
    {
        Assert.Equal(System.DateTime.Parse(expectedDate), System.DateTime.Parse(originalDate).CetToUtc());
    }

    [Fact]
    public void Can_Support_Null_When_Converting_Cet_To_Utc()
    {
        Assert.Null((null as System.DateTime?).CetToUtc());
    }

    [Theory]
    [InlineData("2022-07-19 12:34:56", "19. Juli 2022")]
    [InlineData("2022-12-18 12:34:56", "18. Dezember 2022")]
    public void Can_Format_Date(string date, string expectedResult)
    {
        Assert.Equal(expectedResult, System.DateTime.Parse(date).FormatDate());
    }

    [Fact]
    public void Can_Support_Null_When_Formatting_Date()
    {
        Assert.Equal("", (null as System.DateTime?).FormatDate());
    }

    [Theory]
    [InlineData("2022-07-19 23:45:56", "20. Juli 2022")]
    [InlineData("2022-12-18 23:45:56", "19. Dezember 2022")]
    public void Can_Format_Utc_Date(string date, string expectedResult)
    {
        Assert.Equal(expectedResult, System.DateTime.Parse(date).FormatUtcAsCetDate());
    }

    [Fact]
    public void Can_Support_Null_When_Formatting_Utc_Date()
    {
        Assert.Equal("", (null as System.DateTime?).FormatUtcAsCetDate());
    }

    [Theory]
    [InlineData("2022-07-19 01:23:45", "19. Juli 2022, 01:23:45")]
    [InlineData("2022-12-18 01:23:45", "18. Dezember 2022, 01:23:45")]
    public void Can_Format_DateTime(string date, string expectedResult)
    {
        Assert.Equal(expectedResult, System.DateTime.Parse(date).FormatDateTime());
    }

    [Fact]
    public void Can_Support_Null_When_Formatting_DateTime()
    {
        Assert.Equal("", (null as System.DateTime?).FormatDateTime());
    }

    [Theory]
    [InlineData("2022-07-19 23:45:56", "20. Juli 2022, 01:45:56")]
    [InlineData("2022-12-18 23:45:56", "19. Dezember 2022, 00:45:56")]
    public void Can_Format_Utc_DateTime(string date, string expectedResult)
    {
        Assert.Equal(expectedResult, System.DateTime.Parse(date).FormatUtcAsCetDateTime());
    }

    [Fact]
    public void Can_Support_Null_When_Formatting_Utc_DateTime()
    {
        Assert.Equal("", (null as System.DateTime?).FormatUtcAsCetDateTime());
    }
}