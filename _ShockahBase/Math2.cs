using Microsoft.Xna.Framework;
using System;

namespace Shockah.Base
{
	public static class Math2
	{
		public static int Min(params int[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			int min = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] < min) min = nums[i];
			return min;
		}
		public static float Min(params float[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			float min = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] < min) min = nums[i];
			return min;
		}
		public static double Min(params double[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			double min = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] < min) min = nums[i];
			return min;
		}

		public static int Max(params int[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			int max = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] > max) max = nums[i];
			return max;
		}
		public static float Max(params float[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			float max = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] > max) max = nums[i];
			return max;
		}
		public static double Max(params double[] nums)
		{
			if (nums.Length == 0) throw new ArgumentException();
			double max = nums[0];
			for (int i = 1; i < nums.Length; i++) if (nums[i] > max) max = nums[i];
			return max;
		}
		
		public static float LdirX(float dist, float angle)
		{
			return (float)(-Math.Cos(ToRadians(angle + 180f)) * dist);
		}
		public static float LdirY(float dist, float angle)
		{
			return (float)(Math.Sin(ToRadians(angle + 180f)) * dist);
		}
		public static Vector2 LdirVector2(float dist, float angle)
		{
			return new Vector2((float)Math2.LdirX(dist, angle), (float)Math2.LdirY(dist, angle));
		}

		public static float ToRadians(float degrees)
		{
			return (float)(degrees * Math.PI / 180f);
		}
		public static float ToDegrees(float radians)
		{
			return (float)(radians * 180f / Math.PI);
		}

		public static bool InRegion(Vector2 pos, Vector2 pos1, float w, float h)
		{
			return pos.X >= pos1.X && pos.Y >= pos1.Y && pos.X < pos1.X + w && pos.Y < pos1.Y + h;
		}
		public static bool InRegion(Vector2 pos, Vector2 pos1, Vector2 pos2)
		{
			return pos.X >= pos1.X && pos.Y >= pos1.Y && pos.X < pos2.X && pos.Y < pos2.Y;
		}

		public static float Direction(this Vector2 v1, Vector2 v2)
		{
			return ToDegrees((float)Math.Atan2(v1.Y - v2.Y, v2.X - v1.X));
		}
	}
}
