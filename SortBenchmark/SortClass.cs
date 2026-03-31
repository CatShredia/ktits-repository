namespace SortBenchmark;

public class SortClass
{
    public int[] arraySort;
    private int countNumbers;

    public SortClass(int[] _arraySort)
    {
        arraySort = _arraySort;
        countNumbers = _arraySort.Length;
    }

    public int[] ArraySort1()
    {
        Array.Sort(arraySort, 0, countNumbers);
        return arraySort;
    }

    public int[] ArraySort2()
    {
        // Пузырьковая
        int n = arraySort.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arraySort[j] > arraySort[j + 1])
                {
                    int temp = arraySort[j];
                    arraySort[j] = arraySort[j + 1];
                    arraySort[j + 1] = temp;
                }
            }
        }
        return arraySort;
    }

    public int[] ArraySort3()
    {
        // Быстрая сортировка
        QuickSort(0, countNumbers - 1);
        return arraySort;
    }

    private void QuickSort(int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(low, high);
            QuickSort(low, pi - 1);
            QuickSort(pi + 1, high);
        }
    }

    private int Partition(int low, int high)
    {
        int pivot = arraySort[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (arraySort[j] <= pivot)
            {
                i++;
                int temp = arraySort[i];
                arraySort[i] = arraySort[j];
                arraySort[j] = temp;
            }
        }

        int temp2 = arraySort[i + 1];
        arraySort[i + 1] = arraySort[high];
        arraySort[high] = temp2;

        return i + 1;
    }

    public int[] ArraySort4()
    {
        // Сортировка слияния
        int[] temp = new int[countNumbers];
        MergeSort(0, countNumbers - 1, temp);
        return arraySort;
    }

    private void MergeSort(int left, int right, int[] temp)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            MergeSort(left, mid, temp);
            MergeSort(mid + 1, right, temp);
            Merge(left, mid, right, temp);
        }
    }

    private void Merge(int left, int mid, int right, int[] temp)
    {
        int i = left;
        int j = mid + 1;
        int t = 0;

        while (i <= mid && j <= right)
        {
            if (arraySort[i] <= arraySort[j])
            {
                temp[t++] = arraySort[i++];
            }
            else
            {
                temp[t++] = arraySort[j++];
            }
        }

        while (i <= mid)
        {
            temp[t++] = arraySort[i++];
        }

        while (j <= right)
        {
            temp[t++] = arraySort[j++];
        }

        t = 0;
        while (left <= right)
        {
            arraySort[left++] = temp[t++];
        }
    }
}
