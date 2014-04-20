using Microsoft.Xna.Framework.Graphics;
using System;

namespace Shockah.FCM
{
	public class Filter<T>
	{
		public readonly string name;
		public readonly Texture2D tex;
		public Func<T, bool> matches;
		public bool? mode = null;

		public Filter(string name, Texture2D tex, Func<T, bool> matches)
		{
			this.name = name;
			this.tex = tex;
			this.matches = matches;
		}
	}
}