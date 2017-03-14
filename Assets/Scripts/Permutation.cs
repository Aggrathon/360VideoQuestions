using UnityEngine;

public class Permutation
{
	int number;
	int[] array;

	int iterIndex;


	public int Number { get { return number; } }
	public int[] Array { get { return array; } }


	public Permutation(int size, bool randomize = true)
	{
		if (size > 10)
			Debug.LogError("Permutation class can only handle permutation sizes <= 10");
		array = new int[size];
		for (int i = 0; i < size; i++)
		{
			array[i] = i;
		}
		if (randomize)
			Randomize();
		else
			CalculateNumber();
	}

	void CalculateNumber()
	{
		number = 0;
		for (int i = 0; i < array.Length; i++)
		{
			number *= 10;
			number += array[i];
		}
	}

	public void Randomize()
	{
		for (int i = 0; i < array.Length; i++)
		{
			int n = Random.Range(i, array.Length);
			if (i != n)
			{
				int temp = array[i];
				array[i] = array[n];
				array[n] = temp;
			}
		}
		CalculateNumber();
		IterateReset();
	}

	public void SetPermutation(int perm)
	{
		number = perm;
		for (int i = array.Length-1; i >= 0; i--)
		{
			array[i] = perm % 10;
			perm /= 10;
		}
		IterateReset();
	}

	public int IterateNext(int limit = 10)
	{
		if (iterIndex == array.Length)
			IterateReset();
		int num = array[iterIndex];
		if (num < limit)
		{
			iterIndex++;
			return num;
		}
		else
		{
			iterIndex++;
			return IterateNext(limit);
		}
	}

	public void IterateReset()
	{
		iterIndex = 0;
	}
}