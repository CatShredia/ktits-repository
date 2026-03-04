namespace TestConsoleProject
{

    public class UnitTests
    {
        // ! =============== Task 10: HasUniqueSymbols Tests ===============
        // Count: 7
        // 1. Пустая строка - считается, что все символы уникальны (возвращает true)
        // 2. Строка из одного символа - всегда уникальна (возвращает true)
        // 3. Строка с полностью уникальными символами - возвращает true
        // 4. Строка с повторяющимися символами - возвращает false
        // 5. Строка из одинаковых символов - возвращает false
        // 6. Строка с символами разного регистра - считается, что символы уникальны (A и a - разные)
        // 7. Null строка - возвращает false как некорректный ввод

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
        // 1. Нулевой радиус - объем равен 0
        // 2. Радиус = 1, точность = 2 - проверка корректности вычисления с заданной точностью
        // 3. Радиус = 3, точность = 1 - проверка вычисления для другого радиуса
        // 4. Радиус = 2.5, точность = 3 - проверка вычисления с дробным радиусом
        // 5. Отрицательный радиус - возвращает 0 как некорректный ввод
        // 6. Отрицательная точность - возвращает 0 как некорректный ввод
        // 7. Большой радиус (100), точность = 0 - проверка вычисления для больших значений

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
        // Count: 7
        // 1. Вычисление полного объема стакана - проверка корректности формулы
        // 2. Пустой стакан (высота жидкости = 0) - процент заполнения равен 0
        // 3. Половина высоты стакана - процент заполнения равен 50%
        // 4. Полный стакан - процент заполнения равен 100%
        // 5. Перелив (высота жидкости больше высоты стакана) - процент заполнения равен 100%
        // 6. Масса жидкости при нулевой высоте - масса равна 0
        // 7. Масса жидкости на половине высоты - проверка корректности вычисления массы

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

        // ! =============== Task 13: LandPlotList class Test ===============
        // Count: 7
        // 1. Добавление трех участков - проверка корректности добавления и подсчета количества
        // 2. Сортировка участков по возрастанию - проверка корректности сортировки
        // 3. Удаление участков ниже порога - проверка удаления маленьких участков
        // 4. Удаление при высоком пороге - все участки удаляются
        // 5. Удаление при низком пороге - ни один участок не удаляется
        // 6. Полный сценарий: сортировка с последующим удалением - проверка комбинации методов
        // 7. Добавление участка с отрицательной площадью - выбрасывает ArgumentException

        [Test]
        public void Task13TaskLandPlot_AddPlots_CountIsCorrect()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(100.0);
            list.AddPlot(200.0);
            list.AddPlot(50.0);

            var count = list.Count;
            var expected = 3;

            Assert.That(count, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_SortAscending_OrderIsCorrect()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(300.0);
            list.AddPlot(100.0);
            list.AddPlot(200.0);

            list.SortByAreaAscending();

            var result = list.GetPlots();
            var expected = new List<double> { 100.0, 200.0, 300.0 };

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_RemoveBelowThreshold_RemovesSmallPlots()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(50.0);
            list.AddPlot(150.0);
            list.AddPlot(80.0);
            list.AddPlot(200.0);

            list.RemovePlotsBelowThreshold(100.0);

            var result = list.GetPlots();
            var expected = new List<double> { 150.0, 200.0 };

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_RemoveBelowThreshold_AllRemoved_WhenThresholdHigh()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(10.0);
            list.AddPlot(20.0);

            list.RemovePlotsBelowThreshold(100.0);

            var count = list.Count;
            var expected = 0;

            Assert.That(count, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_RemoveBelowThreshold_NoneRemoved_WhenThresholdLow()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(100.0);
            list.AddPlot(200.0);

            list.RemovePlotsBelowThreshold(50.0);

            var count = list.Count;
            var expected = 2;

            Assert.That(count, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_FullScenario_SortThenRemove()
        {
            var list = new TestConsoleProject.LandPlotList();
            list.AddPlot(500.0);
            list.AddPlot(10.0);
            list.AddPlot(300.0);
            list.AddPlot(50.0);

            list.SortByAreaAscending();
            list.RemovePlotsBelowThreshold(100.0);

            var result = list.GetPlots();
            var expected = new List<double> { 300.0, 500.0 };

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void Task13TaskLandPlot_AddNegativeArea_ThrowsArgumentException()
        {
            var list = new TestConsoleProject.LandPlotList();

            Assert.Throws<ArgumentException>(() =>
            {
                list.AddPlot(-10.0);
            });
        }
    }
}
