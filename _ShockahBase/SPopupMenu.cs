using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public class SPopupMenu
	{
		public class Element
		{
			public string name;
			public Action action;

			public Element(string name, Action action)
			{
				this.name = name;
				this.action = action;
			}

			public virtual Vector2 Size()
			{
				return Main.fontMouseText.MeasureString(name) + new Vector2(16, 0);
			}

			public virtual bool Render(SpriteBatch sb, Vector2 pos, Vector2 size, float scale)
			{
				if (Math2.InRegion(Main.mouse, pos, pos + size))
				{
					sb.FillRectangle(pos + new Vector2(2, 0), size - new Vector2(4, 0), Color.White * .5f);
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						if (action != null) action();
						return true;
					}
				}
				Drawing.StringShadowed(sb, Main.fontMouseText, name, new Vector2(pos.X + 8, pos.Y), Color.White, scale);
				return false;
			}
		}
		public class ElementSeparator : Element
		{
			public ElementSeparator() : base("", null) { }

			public override Vector2 Size()
			{
				return new Vector2(0, Main.fontMouseText.MeasureString("x").Y);
			}

			public override bool Render(SpriteBatch sb, Vector2 pos, Vector2 size, float scale)
			{
				sb.DrawLine(new Vector2(pos.X, pos.Y + size.Y / 2), new Vector2(pos.X + size.X, pos.Y + size.Y / 2), Color.White * .5f);
				return false;
			}
		}

		public readonly SPopupMenuManager manager;
		public float scale = .75f;
		public List<Element> elements = new List<Element>();
		public Vector2 pos;

		public SPopupMenu(SPopupMenuManager manager)
		{
			this.manager = manager;
		}

		public SPopupMenu Add(params Element[] elements)
		{
			foreach (Element element in elements)
				this.elements.Add(element);
			return this;
		}

		public void Create()
		{
			if (elements.Count == 0) return;
			manager.menus.Add(this);
			pos = Main.mouse;
		}

		public void Destroy()
		{
			int index = manager.menus.IndexOf(this);
			if (index < 0) return;
			while (manager.menus.Count > index)
				manager.menus.RemoveAt(index);
		}

		public bool Render(SpriteBatch sb, out bool destroyMe)
		{
			Vector2 size = new Vector2(64, 24);
			foreach (Element element in elements)
			{
				Vector2 esize = element.Size();
				size.X = Math.Max(size.X, esize.X);
				size.Y += esize.Y;
			}
			size *= scale;

			Vector2 pos = this.pos;
			pos.X = Math.Min(pos.X, Main.screenWidth - size.X);
			pos.Y = Math.Min(pos.Y, Main.screenHeight - size.Y);

			bool mouseHover = Math2.InRegion(Main.mouse, pos, pos + size);

			Drawing.DrawBox(sb, pos.X, pos.Y, size.X, size.Y);
			pos.Y += 12 * scale;
			destroyMe = false;
			foreach (Element element in elements) destroyMe |= element.Render(sb, pos, new Vector2(size.X, element.Size().Y * scale), scale);

			return mouseHover;
		}
	}
}