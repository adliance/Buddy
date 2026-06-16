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
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("  \t  \n  ")]
    public void Whitespace_Only_Returns_Empty(string input)
    {
        Assert.Equal("", BundleAndMinifyCss.MinifyCss(input));
    }

    [Theory]
    [InlineData("/* comment */")]
    [InlineData("/* multi\nline\ncomment */")]
    [InlineData("/* comment 1 *//* comment 2 */")]
    public void Comments_Are_Stripped(string input)
    {
        Assert.Equal("", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Whitespace_Between_Rules_Is_Stripped()
    {
        var input = "body {\n    color: red;\n}\n\nh1 {\n    font-size: 2em;\n}";
        Assert.Equal("body{color:red;}h1{font-size:2em;}", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Double_Quoted_String_Is_Preserved()
    {
        var input = """content: "hello   world";""";
        Assert.Equal("""content:"hello   world";""", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Single_Quoted_String_Is_Preserved()
    {
        var input = "content: 'hello   world';";
        Assert.Equal("content:'hello   world';", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Escaped_Quote_Inside_String_Is_Preserved()
    {
        var input = """content: "say \"hi\"";""";
        Assert.Equal("""content:"say \"hi\"";""", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Url_With_Quoted_Path_Is_Preserved()
    {
        var input = """@font-face { src: url("fonts/my font.woff2") format("woff2"); }""";
        Assert.Equal("""@font-face{src:url("fonts/my font.woff2")format("woff2");}""", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Comment_Inside_Quoted_String_Is_Preserved()
    {
        var input = """content: "/* not a comment */";""";
        Assert.Equal("""content:"/* not a comment */";""", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Comment_With_Whitespace_Around_Rules_Is_Stripped()
    {
        var input = "/* header styles */\nbody {\n    margin: 0;\n}\n/* footer styles */\nfooter {\n    padding: 0;\n}";
        Assert.Equal("body{margin:0;}footer{padding:0;}", BundleAndMinifyCss.MinifyCss(input));
    }

    [Fact]
    public void Multi_Value_Property_Loses_Spaces_Between_Values()
    {
        var input = "body { margin: 10px 20px 10px 20px; }";
        Assert.Equal("body{margin:10px20px10px20px;}", BundleAndMinifyCss.MinifyCss(input));
    }
}
