using Xunit;
using Statistics;

namespace Statistics.Tests;

public class StatisticsTests
{
    // Tests for Mean (arithmetic mean)
    
    [Fact]
    public void Mean_SingleElement_ReturnsElement()
    {
        // Arrange
        var data = new double[] { 5 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(5, result);
    }
    
    [Fact]
    public void Mean_MultipleElements_ReturnsCorrectMean()
    {
        // Arrange
        var data = new double[] { 1, 2, 3, 4, 5 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void Mean_EmptyArray_ReturnsZero()
    {
        // Arrange
        var data = new double[] { };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void Mean_NegativeValues_ReturnsCorrectMean()
    {
        // Arrange
        var data = new double[] { -5, -3, -1 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(-3, result);
    }
    
    [Fact]
    public void Mean_MixedValues_ReturnsCorrectMean()
    {
        // Arrange
        var data = new double[] { -10, 0, 10 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void Mean_DecimalValues_ReturnsCorrectMean()
    {
        // Arrange
        var data = new double[] { 1.5, 2.5, 3.5 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.Mean();
        
        // Assert
        Assert.Equal(2.5, result);
    }
    
    // Tests for Standard Deviation
    
    [Fact]
    public void StdDeviation_SingleElement_ReturnsZero()
    {
        // Arrange
        var data = new double[] { 5 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.StdDeviation();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void StdDeviation_EmptyArray_ReturnsZero()
    {
        // Arrange
        var data = new double[] { };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.StdDeviation();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void StdDeviation_TwoElements_ReturnsCorrectStdDeviation()
    {
        // Arrange
        var data = new double[] { 2, 4 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.StdDeviation();
        
        // Assert
        Assert.Equal(1.4142135623730951, result, 10);
    }
    
    [Fact]
    public void StdDeviation_MultipleElements_ReturnsCorrectStdDeviation()
    {
        // Arrange
        var data = new double[] { 2, 4, 4, 4, 5, 5, 7, 9 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.StdDeviation();
        
        // Assert
        Assert.Equal(2.138089935299395, result, 10);
    }
    
    [Fact]
    public void StdDeviation_IdenticalElements_ReturnsZero()
    {
        // Arrange
        var data = new double[] { 5, 5, 5, 5 };
        var stats = new StatisticsClass(data);
        
        // Act
        var result = stats.StdDeviation();
        
        // Assert
        Assert.Equal(0, result);
    }
    
    [Fact]
    public void N_Property_ReturnsCorrectCount()
    {
        // Arrange
        var data = new double[] { 1, 2, 3, 4, 5 };
        var stats = new StatisticsClass(data);
        
        // Assert
        Assert.Equal(5, stats.N);
    }
    
    [Fact]
    public void N_EmptyArray_ReturnsZero()
    {
        // Arrange
        var data = new double[] { };
        var stats = new StatisticsClass(data);
        
        // Assert
        Assert.Equal(0, stats.N);
    }
}
