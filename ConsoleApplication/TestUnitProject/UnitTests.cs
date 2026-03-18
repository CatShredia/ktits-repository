namespace TestConsoleProject
{

    [TestFixture]
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

        private string _testFolder = null!;

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

        // ! =============== Task 14: Export Data Class ===============
        // Count: 3

        public string textTxt;

        [Test]
        public void Task14CreateTestFileDesktopFolder()
        {
            string testString = "Test Date Desctop Folder";
            var dataExport = new TestConsoleProject.DataExport();
            string path = dataExport.ExportDataToFile(testString);

            string result = File.ReadAllText(path);

            Assert.That(result, Is.EqualTo(testString));
        }

        [Test]
        public void Task14CreateTestFileTempFolder()
        {
            string testString = "Test Date Temp Folder";
            var dataExport = new TestConsoleProject.DataExport();
            string path = dataExport.ExportDataToFile(testString, Path.Combine(_testFolder, "text.txt"));

            string result = File.ReadAllText(path);
            textTxt = result;

            Assert.That(result, Is.EqualTo(testString));
        }

        [Test]
        public void Task14OnlyReadDateFromTxtFile()
        {
            string testString = "Test Data Import";
            string filePath = Path.Combine(_testFolder, "text_import.txt");

            var dataExport = new TestConsoleProject.DataExport();
            dataExport.ExportDataToFile(testString, filePath);

            var dataImporter = new TestConsoleProject.DataImporter();
            string readResult = dataImporter.ImportDateFromTxt(filePath);

            Assert.That(readResult, Is.EqualTo(testString));
        }

        // ! =============== Task 14: DataService Tests ===============
        // Count: 14
        // Tests for DataService class including DataImporter and DataExport

        [Test]
        public void Task14DataService_TransferData_NullSourcePath_ReturnsNull()
        {
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransferData(null);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Task14DataService_TransferData_EmptySourcePath_ReturnsNull()
        {
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransferData(string.Empty);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Task14DataService_TransferData_ValidFile_ReturnsDestinationPath()
        {
            string sourcePath = Path.Combine(_testFolder, "source.txt");
            string destPath = Path.Combine(_testFolder, "dest.txt");
            string testData = "Test data for transfer";

            File.WriteAllText(sourcePath, testData);

            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransferData(sourcePath, destPath);

            Assert.That(result, Is.EqualTo(destPath));
            Assert.That(File.Exists(destPath), Is.True);
            Assert.That(File.ReadAllText(destPath), Is.EqualTo(testData));
        }

        [Test]
        public void Task14DataService_TransferData_NonExistentSourceFile_ThrowsException()
        {
            var dataService = new TestConsoleProject.DataService();
            Assert.Throws<System.IO.DirectoryNotFoundException>(() =>
            {
                dataService.TransferData("C:\\nonexistent\\file.txt", "C:\\dest.txt");
            });
        }

        [Test]
        public void Task14DataService_MergeAndExportData_NullSourceList_ReturnsNull()
        {
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.MergeAndExportData(null);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Task14DataService_MergeAndExportData_EmptySourceList_ReturnsNull()
        {
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.MergeAndExportData(new List<string>());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Task14DataService_MergeAndExportData_MultipleFiles_MergesContent()
        {
            string source1 = Path.Combine(_testFolder, "source1.txt");
            string source2 = Path.Combine(_testFolder, "source2.txt");
            string destPath = Path.Combine(_testFolder, "merged.txt");

            File.WriteAllText(source1, "First file content");
            File.WriteAllText(source2, "Second file content");

            var sourcePaths = new List<string> { source1, source2 };
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.MergeAndExportData(sourcePaths, destPath);

            Assert.That(result, Is.EqualTo(destPath));
            Assert.That(File.ReadAllText(destPath), Does.Contain("First file content"));
            Assert.That(File.ReadAllText(destPath), Does.Contain("Second file content"));
        }

        [Test]
        public void Task14DataService_MergeAndExportData_WithEmptyFilePath_SkipsEmptyPath()
        {
            string source1 = Path.Combine(_testFolder, "source1.txt");
            string destPath = Path.Combine(_testFolder, "merged.txt");

            File.WriteAllText(source1, "Valid file content");

            var sourcePaths = new List<string> { string.Empty, source1 };
            var dataService = new TestConsoleProject.DataService();
            var result = dataService.MergeAndExportData(sourcePaths, destPath);

            Assert.That(result, Is.EqualTo(destPath));
            Assert.That(File.ReadAllText(destPath), Is.EqualTo("Valid file content"));
        }

        [Test]
        public void Task14DataService_TransformAndExportData_ValidFileWithTransformation_AppliesTransformation()
        {
            string sourcePath = Path.Combine(_testFolder, "source.txt");
            string destPath = Path.Combine(_testFolder, "transformed.txt");
            string testData = "hello world";

            File.WriteAllText(sourcePath, testData);

            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransformAndExportData(sourcePath, s => s.ToUpper(), destPath);

            Assert.That(result, Is.EqualTo(destPath));
            Assert.That(File.ReadAllText(destPath), Is.EqualTo("HELLO WORLD"));
        }

        [Test]
        public void Task14DataService_TransformAndExportData_TransformationReturnsNull_ReturnsNull()
        {
            string sourcePath = Path.Combine(_testFolder, "source.txt");
            File.WriteAllText(sourcePath, "Test data");

            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransformAndExportData(sourcePath, s => null);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Task14DataService_Constructor_WithDependencies_UsesInjectedDependencies()
        {
            var mockImporter = new TestConsoleProject.DataImporter();
            var mockExport = new TestConsoleProject.DataExport();

            var dataService = new TestConsoleProject.DataService(mockImporter, mockExport);

            Assert.That(dataService, Is.Not.Null);
        }

        [Test]
        public void Task14DataService_TransferData_DefaultDestination_UsesDesktopPath()
        {
            string sourcePath = Path.Combine(_testFolder, "source.txt");
            string testData = "Test data for default destination";

            File.WriteAllText(sourcePath, testData);

            var dataService = new TestConsoleProject.DataService();
            var result = dataService.TransferData(sourcePath);

            Assert.That(result, Is.Not.Null);
            Assert.That(File.Exists(result), Is.True);
            Assert.That(File.ReadAllText(result), Is.EqualTo(testData));
        }

        // ! =============== temp folder ===============
        [SetUp]
        public void SetUp()
        {
            _testFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_testFolder);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, recursive: true);
            }
        }
    }
}