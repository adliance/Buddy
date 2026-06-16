using Xunit;

namespace Adliance.AspNetCore.Buddy.GuiKit.BuildTasks.Test;

public class BundleAndMinifyCssTest
{
    [Fact]
    public void Empty_Input_Returns_Empty()
    {
        Assert.Equal("", BundleAndMinifyCss.MinifyCss(""));
    }

    [Theory]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r")]
    [InlineData("\r\n")]
    public void Tabs_And_Newlines_Are_Removed(string input)
    {
        Assert.Equal("", BundleAndMinifyCss.MinifyCss(input));
    }

    [Theory]
    [InlineData("  ", " ")]
    [InlineData("   ", " ")]
    [InlineData("    ", " ")]
    public void Multiple_Spaces_Are_Collapsed_To_Single_Space(string input, string expected)
    {
        Assert.Equal(expected, BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Multiline_CSS_Is_Collapsed_To_Single_Line()
    {
        var input = "body {\n    color: red;\n}\n\nh1 {\n    font-size: 2em;\n}";
        Assert.Equal("body{color:red;}h1{font-size:2em;}", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Multi_Value_Property_Preserves_Spaces_Between_Values()
    {
        var input = "body { margin: 10px 20px 10px 20px; }";
        Assert.Equal("body{margin:10px 20px 10px 20px;}", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Calc_Preserves_Operator_Whitespace()
    {
        var input = ".foo { width: calc(100% - 20px); }";
        Assert.Equal(".foo{width:calc(100% - 20px);}", BundleAndMinifyCss.MinifyCss(input));
    }
}
