namespace TestUnitProject;

public class UnitTests
{
    // =============== Task 10: HasUniqueSymbols Tests ===============

    [Test]
    public void HasUniqueSymbols_EmptyString_ReturnsTrue()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("");
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasUniqueSymbols_SingleCharacter_ReturnsTrue()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("a");
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasUniqueSymbols_AllUniqueCharacters_ReturnsTrue()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("abc");
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasUniqueSymbols_HasDuplicateCharacters_ReturnsFalse()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("aab");
        Assert.That(result, Is.False);
    }

    [Test]
    public void HasUniqueSymbols_AllSameCharacters_ReturnsFalse()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("777");
        Assert.That(result, Is.False);
    }

    [Test]
    public void HasUniqueSymbols_DifferentCase_ReturnsTrue()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols("Aa");
        Assert.That(result, Is.True);
    }

    [Test]
    public void HasUniqueSymbols_NullString_ReturnsFalse()
    {
        var result = TestConsoleProject.Program.HasUniqueSymbols(null);
        Assert.That(result, Is.False);
    }

    // =============== Task 11: CalculateSphereVolume Tests ===============

    [Test]
    public void CalculateSphereVolume_ZeroRadius_ReturnsZero()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(0, 0);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateSphereVolume_Radius1Precision2_ReturnsCorrectVolume()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(1, 2);
        var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(1, 3), 2);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateSphereVolume_Radius3Precision1_ReturnsCorrectVolume()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(3, 1);
        var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(3, 3), 1);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateSphereVolume_Radius2_5Precision3_ReturnsCorrectVolume()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(2.5, 3);
        var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(2.5, 3), 3);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void CalculateSphereVolume_NegativeRadius_ReturnsZero()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(-1, 2);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateSphereVolume_NegativePrecision_ReturnsZero()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(1, -1);
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void CalculateSphereVolume_LargeRadiusPrecision0_ReturnsRoundedVolume()
    {
        var result = TestConsoleProject.Program.CalculateSphereVolume(100, 0);
        var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(100, 3), 0);
        Assert.That(result, Is.EqualTo(expected));
    }
}
