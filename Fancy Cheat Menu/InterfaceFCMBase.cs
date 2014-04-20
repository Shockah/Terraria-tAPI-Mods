using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TAPI;
using Terraria;

namespace Shockah.FCM
{
	public class InterfaceFCMBase : Interface
	{
		public string typing = null, filterText = null;

		public override void OnOpen()
		{
			typing = null;
			filterText = null;
		}

		public override void Draw(InterfaceLayer layer, SpriteBatch sb)
		{
			if (Util.KeyPressed(Keys.Enter))
			{
				Main.GetInputText("");
				if (typing == null) typing = "";
				else
				{
					filterText = typing;
					if (filterText == "") filterText = null;
					typing = null;
				}
			}
			else if (typing != null) typing = Main.GetInputText(typing);
		}
	}
}