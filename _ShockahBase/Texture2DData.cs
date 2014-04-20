using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TAPI;

namespace Shockah.Base
{
	public class Texture2DData
	{
		public readonly int width, height;
		internal readonly Color[] colors;

		public Texture2DData(int width, int height)
		{
			this.width = width;
			this.height = height;
			colors = new Color[width * height];
		}

		public Color this[int x, int y]
		{
			get { return colors[x + y * width]; }
			set { colors[x + y * width] = value; }
		}

		public Texture2D CreateTexture()
		{
			Texture2D tex = new Texture2D(API.main.GraphicsDevice, width, height);
			tex.SetData(colors);
			return tex;
		}
	}
}
