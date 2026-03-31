using NUnit.Framework;
using Statistics;

namespace Statistics.Tests;

[TestFixture]
public class StatisticsTests
{
    // Тесты для метода Mean (среднее арифметическое)
    
    // ! Тест: вычисление среднего арифметического для одного элемента
    [Test]
    public void Mean_SingleElement_ReturnsElement()
    {
        var data = new double[] { 5 };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(5, result);
    }

    // ! Тест: вычисление среднего арифметического для нескольких элементов
    [Test]
    public void Mean_MultipleElements_ReturnsCorrectMean()
    {
        var data = new double[] { 1, 2, 3, 4, 5 };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(3, result);
    }

    // ! Тест: вычисление среднего арифметического для пустого массива (должно вернуть 0)
    [Test]
    public void Mean_EmptyArray_ReturnsZero()
    {
        var data = new double[] { };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(0, result);
    }

    // ! Тест: вычисление среднего арифметического для отрицательных значений
    [Test]
    public void Mean_NegativeValues_ReturnsCorrectMean()
    {
        var data = new double[] { -5, -3, -1 };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(-3, result);
    }

    // ! Тест: вычисление среднего арифметического для смешанных значений (отрицательные, ноль, положительные)
    [Test]
    public void Mean_MixedValues_ReturnsCorrectMean()
    {
        var data = new double[] { -10, 0, 10 };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(0, result);
    }

    // ! Тест: вычисление среднего арифметического для дробных значений
    [Test]
    public void Mean_DecimalValues_ReturnsCorrectMean()
    {
        var data = new double[] { 1.5, 2.5, 3.5 };
        var stats = new StatisticsClass(data);

        var result = stats.Mean();

        Assert.AreEqual(2.5, result);
    }


    // Тесты для метода StdDeviation (среднеквадратичное отклонение)
    
    // ! Тест: вычисление среднеквадратичного отклонения для одного элемента (должно вернуть 0)
    [Test]
    public void StdDeviation_SingleElement_ReturnsZero()
    {
        var data = new double[] { 5 };
        var stats = new StatisticsClass(data);

        var result = stats.StdDeviation();

        Assert.AreEqual(0, result);
    }

    // ! Тест: вычисление среднеквадратичного отклонения для пустого массива (должно вернуть 0)
    [Test]
    public void StdDeviation_EmptyArray_ReturnsZero()
    {
        var data = new double[] { };
        var stats = new StatisticsClass(data);

        var result = stats.StdDeviation();

        Assert.AreEqual(0, result);
    }

    // ! Тест: вычисление среднеквадратичного отклонения для двух элементов
    [Test]
    public void StdDeviation_TwoElements_ReturnsCorrectStdDeviation()
    {
        var data = new double[] { 2, 4 };
        var stats = new StatisticsClass(data);

        var result = stats.StdDeviation();

        Assert.AreEqual(1.4142135623730951, result, 1e-10);
    }

    // ! Тест: вычисление среднеквадратичного отклонения для нескольких элементов
    [Test]
    public void StdDeviation_MultipleElements_ReturnsCorrectStdDeviation()
    {
        var data = new double[] { 2, 4, 4, 4, 5, 5, 7, 9 };
        var stats = new StatisticsClass(data);

        var result = stats.StdDeviation();

        Assert.AreEqual(2.138089935299395, result, 1e-10);
    }

    // ! Тест: вычисление среднеквадратичного отклонения для одинаковых элементов (должно вернуть 0)
    [Test]
    public void StdDeviation_IdenticalElements_ReturnsZero()
    {
        var data = new double[] { 5, 5, 5, 5 };
        var stats = new StatisticsClass(data);

        var result = stats.StdDeviation();

        Assert.AreEqual(0, result);
    }


    // Тесты для свойства N (количество элементов в выборке)
    
    // ! Тест: свойство N возвращает правильное количество элементов
    [Test]
    public void N_Property_ReturnsCorrectCount()
    {
        var data = new double[] { 1, 2, 3, 4, 5 };
        var stats = new StatisticsClass(data);

        Assert.AreEqual(5, stats.N);
    }

    // ! Тест: свойство N возвращает 0 для пустого массива
    [Test]
    public void N_EmptyArray_ReturnsZero()
    {
        var data = new double[] { };
        var stats = new StatisticsClass(data);

        Assert.AreEqual(0, stats.N);
    }
}
