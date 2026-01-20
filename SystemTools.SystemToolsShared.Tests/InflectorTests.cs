using Xunit;

namespace SystemTools.SystemToolsShared.Tests;

public sealed class InflectorTests
{
    [Theory]
    [InlineData("cat", "cats")]
    [InlineData("bus", "buses")]
    [InlineData("octopus", "octopi")]
    [InlineData("person", "people")]
    [InlineData("child", "children")]
    [InlineData("equipment", "equipment")] // uncountable
    public void Pluralize_ReturnsExpected(string singular, string plural)
    {
        Assert.Equal(plural, singular.Pluralize());
    }

    [Theory]
    [InlineData("cats", "cat")]
    [InlineData("buses", "bus")]
    [InlineData("octopi", "octopus")]
    [InlineData("people", "person")]
    [InlineData("children", "child")]
    [InlineData("equipment", "equipment")] // uncountable
    public void Singularize_ReturnsExpected(string plural, string singular)
    {
        Assert.Equal(singular, plural.Singularize());
    }

    [Theory]
    [InlineData("CamelCaseWord", "Camel Case Word")]
    [InlineData("XmlHttpRequest", "Xml Http Request")]
    [InlineData("simple", "simple")]
    [InlineData("", "")]
    public void SplitWithSpacesCamelParts_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.SplitWithSpacesCamelParts());
    }

    [Theory]
    [InlineData("CatsAndDogs", "CatAndDog")]
    [InlineData("PeopleChildren", "PersonChild")]
    [InlineData("Equipment", "Equipment")]
    public void SingularizeCamelParts_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.SingularizeCamelParts());
    }

    [Theory]
    [InlineData("my_class_name", "My class name")]
    [InlineData("another_example", "Another example")]
    public void Humanize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Humanize());
    }

    [Theory]
    [InlineData("my_class_name", "MyClassName")]
    [InlineData("another_example", "AnotherExample")]
    public void Pascalize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Pascalize());
    }

    [Theory]
    [InlineData("my_class_name", "myClassName")]
    [InlineData("another_example", "anotherExample")]
    public void Camelize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Camelize());
    }

    [Theory]
    [InlineData("MyClassName", "my_class_name")]
    [InlineData("AnotherExample", "another_example")]
    [InlineData("XMLHttpRequest", "xml_http_request")]
    public void Underscore_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Underscore());
    }

    [Theory]
    [InlineData("word", "Word")]
    [InlineData("WORD", "Word")]
    [InlineData("wORD", "Word")]
    public void Capitalize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Capitalize());
    }

    [Theory]
    [InlineData("word", "Word")]
    [InlineData("Word", "Word")]
    public void CapitalizeCamel_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.CapitalizeCamel());
    }

    [Theory]
    [InlineData("Word", "word")]
    [InlineData("WORD", "wORD")]
    public void UnCapitalize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.UnCapitalize());
    }

    [Theory]
    [InlineData("1", "1st")]
    [InlineData("2", "2nd")]
    [InlineData("3", "3rd")]
    [InlineData("4", "4th")]
    [InlineData("11", "11th")]
    [InlineData("12", "12th")]
    [InlineData("13", "13th")]
    [InlineData("21", "21st")]
    public void Ordinalize_String_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Ordinalize());
    }

    [Theory]
    [InlineData(1, "1st")]
    [InlineData(2, "2nd")]
    [InlineData(3, "3rd")]
    [InlineData(4, "4th")]
    [InlineData(11, "11th")]
    [InlineData(12, "12th")]
    [InlineData(13, "13th")]
    [InlineData(21, "21st")]
    public void Ordinalize_Int_ReturnsExpected(int input, string expected)
    {
        Assert.Equal(expected, input.Ordinalize());
    }

    [Theory]
    [InlineData("my_class_name", "my-class-name")]
    [InlineData("another_example", "another-example")]
    public void Dasherize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Dasherize());
    }

    [Theory]
    [InlineData("CamelCaseWord", new[] { "Camel", "Case", "Word" })]
    [InlineData("XmlHttpRequest", new[] { "Xml", "Http", "Request" })]
    [InlineData("simple", new[] { "simple" })]
    [InlineData("", new[] { "" })]
    public void SplitUpperCase_ReturnsExpected(string input, string[] expected)
    {
        Assert.Equal(expected, input.SplitUpperCase());
    }

    [Theory]
    [InlineData("my_class_name", "My Class Name")]
    [InlineData("another_example", "Another Example")]
    public void Titleize_ReturnsExpected(string input, string expected)
    {
        Assert.Equal(expected, input.Titleize());
    }

    [Fact]
    public void AddIrregular_AddsCustomIrregular()
    {
        Inflector.AddIrregular("mouse", "mice");
        Assert.Equal("mice", "mouse".Pluralize());
        Assert.Equal("mouse", "mice".Singularize());
    }

    [Fact]
    public void AddUncountable_AddsCustomUncountable()
    {
        Inflector.AddUncountable("foobar");
        Assert.Equal("foobar", "foobar".Pluralize());
        Assert.Equal("foobar", "foobar".Singularize());
    }

    [Fact]
    public void AddPlural_AddsCustomPluralRule()
    {
        Inflector.AddPlural("foo$", "foos");
        Assert.Equal("foos", "foo".Pluralize());
    }

    [Fact]
    public void AddSingular_AddsCustomSingularRule()
    {
        Inflector.AddSingular("bars$", "bar");
        Assert.Equal("bar", "bars".Singularize());
    }
}
