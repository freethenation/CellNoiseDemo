using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CellNoiseDemo
{
	public struct PointColor
	{
		public PointColor(Vector4 point, Vector4 color)
		{
			Point = point;
			Color = color;
		}
		public Vector4 Point;
		public Vector4 Color;
	}

	public static class WorleyNoise
	{
		public static float EuclidianDistanceFunc(Vector3 p1, Vector3 p2)
		{
			return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z);
		}

		public static float ManhattanDistanceFunc(Vector3 p1, Vector3 p2)
		{
			return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y) + Math.Abs(p1.Z - p2.Z);
		}

		public static float ChebyshevDistanceFunc(Vector3 p1, Vector3 p2)
		{
			Vector3 diff = p1 - p2;
			return Math.Max(Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y)), Math.Abs(diff.Z));
		}

		/// <summary>
		/// The Worley noise function
		/// </summary>
		/// <param name="input">The location at which the Worley noise function should be evaluated at.</param>
		/// <param name="seed">The seed for the noise function</param>
		/// <param name="distanceFunc">The function used to calculate the distance between two points. Several of these are defined as statics on the WorleyNoise class.</param>
		/// <param name="distanceArray">An array which will store the distances computed by the Worley noise function. The length of the array determines how many distances will be computed.</param>
		/// <param name="worleyFunction">The function used to color the location. The color takes the populated distanceArray
		/// param and returns a float which is the greyscale value outputed by the worley noise function.</param>
		/// <returns></returns>
		public static PointColor WorleyFunc(this PointColor input, int seed, Func<Vector3, Vector3, float> distanceFunc, ref float[] distanceArray, Func<float[], float> worleyFunction)
		{
			//Init values in distance array to large values
			for (int i = 0; i < distanceArray.Length; i++)
			{
				distanceArray[i] = 6666;
			}

			uint lastRandom, numberFeaturePoints;
			Vector3 randomDiff, featurePoint;
			int x = (int)Math.Floor(input.Point.X);
			int y = (int)Math.Floor(input.Point.Y);
			int z = (int)Math.Floor(input.Point.Z);
			int cubeX, cubeY, cubeZ;

			for (int i = -1; i < 2; ++i)
				for (int j = -1; j < 2; ++j)
					for (int k = -1; k < 2; ++k)
					{
						cubeX = x + i;
						cubeY = y + j;
						cubeZ = z + k;
						lastRandom = lcgRandom(hash((uint)(cubeX + seed), (uint)(cubeY), (uint)(cubeZ)));
						// Find the number of feature points in the cube
						numberFeaturePoints = probLookup(lastRandom);

						for (uint l = 0; l < numberFeaturePoints; ++l)
						{
							lastRandom = lcgRandom(lastRandom);
							randomDiff.X = (float)lastRandom / 0x100000000;

							lastRandom = lcgRandom(lastRandom);
							randomDiff.Y = (float)lastRandom / 0x100000000;

							lastRandom = lcgRandom(lastRandom);
							randomDiff.Z = (float)lastRandom / 0x100000000;

							featurePoint = new Vector3(randomDiff.X + (float)cubeX, randomDiff.Y + (float)cubeY, randomDiff.Z + (float)cubeZ);

							insert(distanceArray, distanceFunc(input.Point.Xyz, featurePoint));
						}

					}

			float color = worleyFunction(distanceArray);
			input.Color.X = color;
			input.Color.Y = color;
			input.Color.Z = color;
			input.Color = Vector4.Clamp(input.Color, new Vector4(0,0,0,0), new Vector4(1,1,1,1));
			return input; 
		}

		// Generated with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]" //"N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]"
		private static uint probLookup(uint value)
		{
			if (value < 393325350) return 1;
			if (value < 1022645910) return 2;
			if (value < 1861739990) return 3;
			if (value < 2700834071) return 4;
			if (value < 3372109335) return 5;
			if (value < 3819626178) return 6;
			if (value < 4075350088) return 7;
			if (value < 4203212043) return 8;
			return 9;
		}

		/// <summary>
		/// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
		/// it will not be added to the array.
		/// </summary>
		/// <param name="arr">The array to insert the value into.</param>
		/// <param name="value">The value to insert into the array.</param>
		private static void insert(float[] arr, float value)
		{
			float temp;
			for (int i = arr.Length - 1; i >= 0; i--)
			{
				if (value > arr[i]) break;
				temp = arr[i];
				arr[i] = value;
				if (i + 1 < arr.Length) arr[i + 1] = temp;
			}
		}

		/// <summary>
		/// LCG Random Number Generator
		/// </summary>
		/// <param name="lastValue">The last value calculated by the lcg or a seed</param>
		/// <returns>A new random number</returns>
		public static uint lcgRandom(uint lastValue)
		{
			return (uint)((1103515245u * lastValue + 12345u) % 0x100000000u);
		}


		private const uint OFFSET_BASIS = 2166136261;
		private const uint FNV_PRIME = 16777619;

		/// <summary>
		/// Hash of three uints
		/// 
		/// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
		/// </summary>
		/// <returns>hash value</returns>
		public static uint hash(uint i, uint j, uint k)
		{
			return (uint)((((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME) ^ (uint)k) * FNV_PRIME);
		}
	}
}
