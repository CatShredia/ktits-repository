namespace TestConsoleProject
{

    public class UnitTests
    {
        // ! =============== Task 10: HasUniqueSymbols Tests ===============
        // Count: 7

        [Test]
        public void Task10HasUniqueSymbols_EmptyString_ReturnsTrue()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("");
            Assert.That(result, Is.True);
        }

        [Test]
        public void Task10HasUniqueSymbols_SingleCharacter_ReturnsTrue()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("a");
            Assert.That(result, Is.True);
        }

        [Test]
        public void Task10HasUniqueSymbols_AllUniqueCharacters_ReturnsTrue()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("abc");
            Assert.That(result, Is.True);
        }

        [Test]
        public void Task10HasUniqueSymbols_HasDuplicateCharacters_ReturnsFalse()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("aab");
            Assert.That(result, Is.False);
        }

        [Test]
        public void Task10HasUniqueSymbols_AllSameCharacters_ReturnsFalse()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("777");
            Assert.That(result, Is.False);
        }

        [Test]
        public void Task10HasUniqueSymbols_DifferentCase_ReturnsTrue()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols("Aa");
            Assert.That(result, Is.True);
        }

        [Test]
        public void Task10HasUniqueSymbols_NullString_ReturnsFalse()
        {
            var result = TestConsoleProject.Program.HasUniqueSymbols(null);
            Assert.That(result, Is.False);
        }

        // ! =============== Task 11: CalculateSphereVolume Tests ===============
        // Count: 7

        [Test]
        public void Task11CalculateSphereVolume_ZeroRadius_ReturnsZero()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(0, 0);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Task11CalculateSphereVolume_Radius1Precision2_ReturnsCorrectVolume()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(1, 2);
            var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(1, 3), 2);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task11CalculateSphereVolume_Radius3Precision1_ReturnsCorrectVolume()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(3, 1);
            var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(3, 3), 1);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task11CalculateSphereVolume_Radius2_5Precision3_ReturnsCorrectVolume()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(2.5, 3);
            var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(2.5, 3), 3);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task11CalculateSphereVolume_NegativeRadius_ReturnsZero()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(-1, 2);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Task11CalculateSphereVolume_NegativePrecision_ReturnsZero()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(1, -1);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Task11CalculateSphereVolume_LargeRadiusPrecision0_ReturnsRoundedVolume()
        {
            var result = TestConsoleProject.Program.CalculateSphereVolume(100, 0);
            var expected = Math.Round((4.0 / 3.0) * Math.PI * Math.Pow(100, 3), 0);
            Assert.That(result, Is.EqualTo(expected));
        }


        // ! =============== Task 12: Cup class Test ===============
        // Опишите класс «стакан». Поля класса: высота стакана, диаметр дна стакана.
        // Предполагается, что стакан прямой (представляет собой цилиндр). 
        // Методы: определить объем стакана, определить,
        // какой процент стакана занимает жидкость, налитая на заданную высоту,
        // определить массу жидкости с заданной плотностью, налитую на заданную высоту.
        // Count: 7

        [Test]
        public void Task12Cup_VolumeTest()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.Volume();
            var expected = Math.PI * Math.Pow(2.5, 2) * 10;
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }

        [Test]
        public void Task12Cup_FillPercent_EmptyCup_ReturnsZero()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.FillPercent(0);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Task12Cup_FillPercent_HalfFull_ReturnsFifty()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.FillPercent(5);
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void Task12Cup_FillPercent_FullCup_ReturnsHundred()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.FillPercent(10);
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void Task12Cup_FillPercent_Overflow_ReturnsHundred()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.FillPercent(15);
            Assert.That(result, Is.EqualTo(100));
        }

        [Test]
        public void Task12Cup_LiquidMass_ZeroHeight_ReturnsZero()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.LiquidMass(0, 1.0);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Task12Cup_LiquidMass_HalfHeight_ReturnsCorrectMass()
        {
            var cup = new TestConsoleProject.Cup(10, 5);
            var result = cup.LiquidMass(5, 1.0);
            var expected = 1.0 * Math.PI * Math.Pow(2.5, 2) * 5;
            Assert.That(result, Is.EqualTo(expected).Within(0.01));
        }
    }
}
