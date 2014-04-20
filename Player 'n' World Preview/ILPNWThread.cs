using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shockah.Base;
using TAPI;
using Terraria;

namespace Shockah.PNWPreview
{
	public class ILPNWThread : InterfaceLayer
	{
		public static float status = -1f;
		
		public ILPNWThread() : base(MBase.me.modName) { }

		protected override void OnDraw(SpriteBatch sb)
		{
			if (status != -1f)
			{
				string text = "Scanning world... " + (int)(status * 100f) + "%";
				Vector2 measure = Main.fontMouseText.MeasureString(text);
				Vector2 pos = new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y - 24);
				Drawing.DrawBox(sb, pos.X - 8, pos.Y - 8, measure.X + 16, measure.Y + 16);
				SDrawing.StringShadowed(sb, Main.fontMouseText, text, pos);
			}
		}
	}
}
