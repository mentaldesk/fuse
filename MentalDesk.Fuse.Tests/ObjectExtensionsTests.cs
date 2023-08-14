using FluentAssertions;
using Xunit;

namespace MentalDesk.Fuse.Tests;

public class ObjectExtensionsTests
{
    [Fact]
    public void SetFused_AutoPropertyName_StoresProperty()
    {
        var obj = new object();
        obj.SetFused("Value");

        var result = obj.GetFused<string>();

        Assert.Equal("Value", result);
    }

    [Fact]
    public void GetFused_ValidProperty_ReturnsValue()
    {
        var obj = new object();
        obj.SetFused("Test", "Value");

        var result = obj.GetFused<string>("Test");

        Assert.Equal("Value", result);
    }

    [Fact]
    public void GetFused_UnassignedProperty_ReturnsNull()
    {
        var obj = new object();

        var result = obj.GetFused<string>("Invalid");

        result.Should().BeNull();
    }
    
    [Fact]
    public void GetFused_InvalidPropertyType_ReturnsNull()
    {
        var obj = new object();
        obj.SetFused("StringProperty", "StringValue");

        var result = obj.GetFused<int?>("StringProperty");

        result.Should().BeNull();
    }

    [Fact]
    public void Fused_CreatesNewInstance()
    {
        var obj = new object();

        var result = obj.Fused<TestClass>();

        Assert.IsType<TestClass>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public void Fused_ReturnsSameInstance()
    {
        var obj = new object();

        var result1 = obj.Fused<TestClass>();
        var result2 = obj.Fused<TestClass>();

        Assert.Same(result1, result2);
    }

    class TestClass
    {
        public int X { get; set; }
    }
}