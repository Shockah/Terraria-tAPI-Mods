using C3.XNA;
using LitJson;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TAPI;
using Terraria;

namespace Shockah.Base
{
	public static class SFrameAnchorExtensions
	{
		public static Vector2 Origin(this SFrame.Anchor anchor)
		{
			switch (anchor)
			{
				case SFrame.Anchor.Top: return new Vector2(Main.screenWidth / 2, 0);
				case SFrame.Anchor.TopRight: return new Vector2(Main.screenWidth, 0);
				case SFrame.Anchor.Left: return new Vector2(0, Main.screenHeight / 2);
				case SFrame.Anchor.Center: return new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
				case SFrame.Anchor.Right: return new Vector2(Main.screenWidth, Main.screenHeight / 2);
				case SFrame.Anchor.BottomLeft: return new Vector2(0, Main.screenHeight);
				case SFrame.Anchor.Bottom: return new Vector2(Main.screenWidth / 2, Main.screenHeight);
				case SFrame.Anchor.BottomRight: return new Vector2(Main.screenWidth, Main.screenHeight);
				default: return new Vector2(0, 0);
			}
		}

		public static Vector2 ParentAnchorDrawPoint(this SFrame.Anchor anchor)
		{
			switch (anchor)
			{
				case SFrame.Anchor.Top: return new Vector2(Main.screenWidth / 2, 24);
				case SFrame.Anchor.TopRight: return new Vector2(Main.screenWidth - 24, 24);
				case SFrame.Anchor.Left: return new Vector2(24, Main.screenHeight / 2);
				case SFrame.Anchor.Center: return new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
				case SFrame.Anchor.Right: return new Vector2(Main.screenWidth - 24, Main.screenHeight / 2);
				case SFrame.Anchor.BottomLeft: return new Vector2(24, Main.screenHeight - 24);
				case SFrame.Anchor.Bottom: return new Vector2(Main.screenWidth / 2, Main.screenHeight - 24);
				case SFrame.Anchor.BottomRight: return new Vector2(Main.screenWidth - 24, Main.screenHeight - 24);
				default: return new Vector2(24, 24);
			}
		}
	}
	
	public class SFrame
	{
		public enum Anchor
		{
			TopLeft, Top, TopRight, Left, Center, Right, BottomLeft, Bottom, BottomRight
		}

		public class Resizable
		{
			public static Resizable NewNot() { return new ResizableNot(); }
			public static Resizable NewRatio(float ratio = 1f) { return new ResizableRatio(ratio); }
			public static Resizable NewFree(bool horizontal = true, bool vertical = true) { return new ResizableFree(horizontal, vertical); }
			
			internal Resizable() { }
			public virtual bool AnchorResizable(Anchor anchor) { return !(anchor == Anchor.Center || (int)anchor > (int)Anchor.BottomRight || (int)anchor < 0); }
			public bool AnchorResizable(Anchor myAnchor, Anchor anchor) { if (anchor == myAnchor) return false; return AnchorResizable(anchor); }

			public class ResizableNot : Resizable
			{
				internal ResizableNot() : base() { }
				public override bool AnchorResizable(Anchor anchor) { return false; }
			}
			public class ResizableRatio : Resizable
			{
				public float ratio;
				internal ResizableRatio(float ratio = 1f) : base() { this.ratio = ratio; }
				public override bool AnchorResizable(Anchor anchor)
				{
					if (anchor == Anchor.TopLeft || anchor == Anchor.TopRight || anchor == Anchor.BottomLeft || anchor == Anchor.BottomRight) return false;
					return base.AnchorResizable(anchor);
				}
			}
			public class ResizableFree : Resizable
			{
				public bool horizontal, vertical;
				internal ResizableFree(bool horizontal = true, bool vertical = true) : base() { this.horizontal = horizontal; this.vertical = vertical; }
				public override bool AnchorResizable(Anchor anchor)
				{
					switch (anchor)
					{
						case Anchor.TopLeft: return horizontal && vertical;
						case Anchor.Top: return vertical;
						case Anchor.TopRight: return horizontal && vertical;
						case Anchor.Left: return horizontal;
						case Anchor.Right: return horizontal;
						case Anchor.BottomLeft: return horizontal && vertical;
						case Anchor.Bottom: return vertical;
						case Anchor.BottomRight: return horizontal && vertical;
						default: return false;
					}
				}
			}
		}
		
		internal static JsonData data;
		public static List<SFrame> frames;
		private static List<SFrame> framesAdd, framesRemove;
		public static SFrame actionOn;
		public static bool parentAnchorMode;

		static SFrame()
		{
			Clear();
		}

		internal static void Clear()
		{
			data = SBase.JsonObject();
			frames = new List<SFrame>();
			framesAdd = new List<SFrame>();
			framesRemove = new List<SFrame>();
			actionOn = null;
			parentAnchorMode = false;
		}

		public static void SaveAll()
		{
			File.WriteAllText(Main.SavePath + "/Mods/Shockah.Base.SFrame.json", JsonMapper.ToJson(data));
		}
		public static void LoadAll()
		{
			if (File.Exists(Main.SavePath + "/Mods/Shockah.Base.SFrame.json")) data = JsonMapper.ToObject(File.ReadAllText(Main.SavePath + "/Mods/Shockah.Base.SFrame.json"));
		}

		public static void DestroyAll(bool force = false)
		{
			if (force)
			{
				framesAdd.Clear();
				framesRemove.Clear();
				frames.Clear();
			}
			else
			{
				foreach (SFrame sf in frames) sf.Destroy();
			}
		}

		public static void UpdateAll()
		{
			foreach (SFrame sf in framesRemove) frames.Remove(sf);
			foreach (SFrame sf in framesAdd) frames.Add(sf);
			framesAdd.Clear();
			framesRemove.Clear();
			foreach (SFrame sf in frames) sf.Update();

			if (!Main.hideUI && Main.playerInventory && frames.Count != 0 && Math2.InRegion(Main.mouse, new Vector2(0, Main.screenHeight - 16), new Vector2(16, Main.screenHeight)))
			{
				Main.localPlayer.mouseInterface = true;
				SBase.tip = "Left click to (un)lock all frames\nRight click to switch anchor setting mode";
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					bool allLocked = true;
					foreach (SFrame sf in frames) if (!sf.locked) allLocked = false;
					foreach (SFrame sf in frames) sf.locked = !allLocked;
					parentAnchorMode = false;
					Main.PlaySound(22, -1, -1, 1);
				}
				if (Main.mouseRight && Main.mouseRightRelease)
				{
					parentAnchorMode = !parentAnchorMode;
				}
			}
		}
		public static void RenderAll(SpriteBatch sb)
		{
			foreach (SFrame sf in frames) sf.Render(sb);
			foreach (SFrame sf in frames) sf.Render2(sb);
			foreach (SFrame sf in frames) sf.Render3(sb);

			if (!Main.hideUI && Main.playerInventory && frames.Count != 0)
			{
				sb.Draw(Main.HBLockTexture[frames[0].locked ? 0 : 1], new Vector2(8, Main.screenHeight - 8), null, parentAnchorMode ? Color.Red : Color.White, 0f, Main.HBLockTexture[0].Size() / 2, 1f, SpriteEffects.None, 0f);
			}
		}
		
		public readonly ModBase modBase;
		public readonly string tag;
		public Vector2 pos, size;
		public bool locked = true;
		public Vector2? dragging = null;
		public Vector2? resizingMouse = null;
		public Anchor? resizing = null;
		public Anchor anchor = Anchor.TopLeft, parentAnchor = Anchor.TopLeft;
		public Resizable resizable;

		public SFrame(ModBase modBase, string tag, Anchor parentAnchor = Anchor.TopLeft, Anchor defaultAnchor = Anchor.TopLeft, Vector2 defaultPos = default(Vector2), Vector2 defaultSize = default(Vector2)) : this(modBase, tag, parentAnchor, defaultAnchor, defaultPos, defaultSize, Resizable.NewNot()) { }
		public SFrame(ModBase modBase, string tag, Anchor parentAnchor, Anchor defaultAnchor, Vector2 defaultPos, Vector2 defaultSize, Resizable resizable)
		{
			this.modBase = modBase;
			this.tag = tag;
			pos = defaultPos;
			size = defaultSize;
			this.parentAnchor = parentAnchor;
			anchor = defaultAnchor;
			this.resizable = resizable;
		}

		public void Create()
		{
			if (data.Has(modBase.modName))
			{
				JsonData j = data[modBase.modName];
				if (j.Has(tag))
				{
					JsonData j2 = j[tag];
					pos = new Vector2((int)j2["pos"][0], (int)j2["pos"][1]);
					size = new Vector2((int)j2["size"][0], (int)j2["size"][1]);
					Anchor panchor2; if (Enum.TryParse((string)j2["parentAnchor"], out panchor2)) parentAnchor = panchor2;
					Anchor anchor2; if (Enum.TryParse((string)j2["anchor"], out anchor2)) anchor = anchor2;
					locked = (bool)j2["locked"];
					OnLoad(j2);
				}
			}
			
			framesAdd.Add(this);
			OnCreate();
		}
		public void Destroy()
		{
			OnDestroy();
			framesRemove.Add(this);

			if (!data.Has(modBase.modName)) data[modBase.modName] = SBase.JsonObject();
			JsonData j = data[modBase.modName];

			j[tag] = SBase.JsonObject(
				"pos", SBase.JsonArray((int)pos.X, (int)pos.Y),
				"size", SBase.JsonArray((int)size.X, (int)size.Y),
				"locked", locked,
				"parentAnchor", Enum.GetName(typeof(Anchor), parentAnchor),
				"anchor", Enum.GetName(typeof(Anchor), anchor)
			);
			OnSave(j[tag]);
			data[modBase.modName] = j;
		}
		public void Update()
		{
			if (!locked && (actionOn == null || object.ReferenceEquals(actionOn, this)))
			{
				if (Math2.InRegion(Main.mouse, FramePos1() - new Vector2(12, 12), FramePos2() + new Vector2(12, 12)) && !Math2.InRegion(Main.mouse, FrameLockPos() - new Vector2(8, 8), FrameLockPos() + new Vector2(8, 8)) && Main.mouseRight && Main.mouseRightRelease)
				{
					OnUnlockedRightClick();
				}
				else
				{				
					Vector2 p1 = FramePos1(), p2 = FramePos2();
					if (dragging.HasValue)
					{
						Main.localPlayer.mouseInterface = true;
						if (Main.mouse != dragging)
						{
							pos += Main.mouse - dragging.Value;

							p1 = FramePos1();
							p2 = FramePos2();
							if (p1.X < 0) pos.X -= p1.X;
							if (p1.Y < 0) pos.Y -= p1.Y;
							if (p2.X > Main.screenWidth) pos.X -= p2.X - Main.screenWidth;
							if (p2.Y > Main.screenHeight) pos.Y -= p2.Y - Main.screenHeight;
							dragging = Main.mouse;
						}
						if (!Main.mouseLeft)
						{
							dragging = null;
							actionOn = null;
						}
					}
					else
					{
						if (Math2.InRegion(Main.mouse, FrameLockPos() - new Vector2(8, 8), FrameLockPos() + new Vector2(8, 8)))
						{
							Main.localPlayer.mouseInterface = true;
							SBase.tip = "Left click to lock\nRight click to switch " + (parentAnchorMode ? "outer" : "inner") + " anchor";
							if (Main.mouseLeft && Main.mouseLeftRelease)
							{
								locked = true;
								Main.PlaySound(22, -1, -1, 1);
							}
							else if (Main.mouseRight && Main.mouseRightRelease)
							{
								if (parentAnchorMode)
								{
									Vector2 paOrigin = parentAnchor.Origin();
									parentAnchor = (Anchor)(((int)parentAnchor + 1) % 9);
									pos += paOrigin;
									pos -= parentAnchor.Origin();
								}
								else
								{
									anchor = (Anchor)(((int)anchor + 1) % 9);
								}
								Main.PlaySound(22, -1, -1, 1);
							}
						}
						else
						{
							if (resizing.HasValue)
							{
								Main.localPlayer.mouseInterface = true;
								if (Main.mouse != resizingMouse)
								{
									Anchor oldAnchor = anchor;
									ConvertToAnchor();

									float modPosX = 0, modPosY = 0, modSizeX = 0, modSizeY = 0;
									if (resizing.Value == Anchor.TopLeft || resizing.Value == Anchor.Left || resizing.Value == Anchor.BottomLeft)
									{
										modPosX = 1;
										modSizeX = -1;
									}
									if (resizing.Value == Anchor.TopLeft || resizing.Value == Anchor.Top || resizing.Value == Anchor.TopRight)
									{
										modPosY = 1;
										modSizeY = -1;
									}
									if (resizing.Value == Anchor.TopRight || resizing.Value == Anchor.Right || resizing.Value == Anchor.BottomRight)
									{
										modSizeX = 1;
									}
									if (resizing.Value == Anchor.BottomLeft || resizing.Value == Anchor.Bottom || resizing.Value == Anchor.BottomRight)
									{
										modSizeY = 1;
									}
									pos.X += (Main.mouse.X - resizingMouse.Value.X) * modPosX;
									pos.Y += (Main.mouse.Y - resizingMouse.Value.Y) * modPosY;
									size.X += (Main.mouse.X - resizingMouse.Value.X) * modSizeX;
									size.Y += (Main.mouse.Y - resizingMouse.Value.Y) * modSizeY;

									ConvertToAnchor(oldAnchor);
									size = new Vector2(Math.Max(size.X, 0), Math.Max(size.Y, 0));
									resizingMouse = Main.mouse;
								}
								if (!Main.mouseLeft)
								{
									resizing = null;
									resizingMouse = null;
									actionOn = null;
								}
							}
							else
							{
								bool corners = size.X >= 32 && size.Y >= 32;
								Func<Anchor, Vector2, bool, bool> checkCorner = (_anchor, _p, _corners) =>
								{
									if (resizable.AnchorResizable(anchor, _anchor) && _corners && Math2.InRegion(Main.mouse, _p - new Vector2(8, 8), 16, 16))
									{
										Main.localPlayer.mouseInterface = true;
										SBase.tip = "Drag to resize";
										if (Main.mouseLeft)
										{
											resizing = _anchor;
											resizingMouse = Main.mouse;
											actionOn = this;
										}
										return true;
									}
									return false;
								};
								bool aCorner = false;
								aCorner |= checkCorner(Anchor.TopLeft, p1, corners);
								aCorner |= checkCorner(Anchor.Top, new Vector2((p1.X + p2.X) / 2, p1.Y), true);
								aCorner |= checkCorner(Anchor.TopRight, new Vector2(p2.X, p1.Y), corners);
								aCorner |= checkCorner(Anchor.Left, new Vector2(p1.X, (p1.Y + p2.Y) / 2), true);
								aCorner |= checkCorner(Anchor.Right, new Vector2(p2.X, (p1.Y + p2.Y) / 2), true);
								aCorner |= checkCorner(Anchor.BottomLeft, new Vector2(p1.X, p2.Y), corners);
								aCorner |= checkCorner(Anchor.Bottom, new Vector2((p1.X + p2.X) / 2, p2.Y), true);
								aCorner |= checkCorner(Anchor.BottomRight, p2, corners);

								if (!aCorner)
								{
									if (Math2.InRegion(Main.mouse, FramePos1() - new Vector2(12, 12), FramePos2() + new Vector2(12, 12)) && !Math2.InRegion(Main.mouse, new Vector2(0, Main.screenHeight - 16), new Vector2(16, Main.screenHeight)))
									{
										Main.localPlayer.mouseInterface = true;
										if (Main.mouseLeft)
										{
											dragging = Main.mouse;
											actionOn = this;
										}
									}
								}
							}
						}
					}
				}
			}
			
			OnUpdate();
		}
		public void Render(SpriteBatch sb)
		{
			if (Main.hideUI || !IsVisible()) return;
			Vector2 p1 = FramePos1(), p2 = FramePos2();

			if (Main.playerInventory && ShouldRenderFrame()) Drawing.DrawBox(sb, p1.X - 12, p1.Y - 12, p2.X - p1.X + 24, p2.Y - p1.Y + 24);
			OnRender(sb);
			if (!locked && Main.playerInventory && !parentAnchorMode)
			{
				Texture2D texResizer = MBase.me.textures["Resizer.png"];
				bool corners = size.X >= 32 && size.Y >= 32;
				if (resizable.AnchorResizable(anchor, Anchor.TopLeft) && corners) sb.Draw(texResizer, p1, null, Color.White, Math2.ToRadians(-45f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.Top)) sb.Draw(texResizer, new Vector2((p1.X + p2.X) / 2, p1.Y), null, Color.White, Math2.ToRadians(0f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.TopRight) && corners) sb.Draw(texResizer, new Vector2(p2.X, p1.Y), null, Color.White, Math2.ToRadians(45f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.Left)) sb.Draw(texResizer, new Vector2(p1.X, (p1.Y + p2.Y) / 2), null, Color.White, Math2.ToRadians(-90f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.Right)) sb.Draw(texResizer, new Vector2(p2.X, (p1.Y + p2.Y) / 2), null, Color.White, Math2.ToRadians(90f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.BottomLeft) && corners) sb.Draw(texResizer, new Vector2(p1.X, p2.Y), null, Color.White, Math2.ToRadians(-135f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.Bottom)) sb.Draw(texResizer, new Vector2((p1.X + p2.X) / 2, p2.Y), null, Color.White, Math2.ToRadians(180f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
				if (resizable.AnchorResizable(anchor, Anchor.BottomRight) && corners) sb.Draw(texResizer, p2, null, Color.White, Math2.ToRadians(135f), texResizer.Size() / 2, 1f, SpriteEffects.None, 0f);
			}
		}
		public void Render2(SpriteBatch sb)
		{
			if (Main.hideUI || !IsVisible()) return;
			if (!locked && Main.playerInventory && parentAnchorMode)
			{
				sb.DrawCircle(parentAnchor.ParentAnchorDrawPoint(), 12f, 16, Color.Red);
				sb.DrawCircle(parentAnchor.ParentAnchorDrawPoint(), 11f, 16, Color.Red);
				sb.DrawCircle(parentAnchor.ParentAnchorDrawPoint(), 10f, 16, Color.Red);

				sb.DrawCircle(FrameLockPos(), 12f, 16, Color.Red);
				sb.DrawCircle(FrameLockPos(), 11f, 16, Color.Red);
				sb.DrawCircle(FrameLockPos(), 10f, 16, Color.Red);

				float angle = parentAnchor.ParentAnchorDrawPoint().Direction(FrameLockPos());
				sb.DrawLine(parentAnchor.ParentAnchorDrawPoint() + Math2.LdirVector2(12, angle), FrameLockPos() - Math2.LdirVector2(12, angle), Color.Red);
				if (parentAnchor != Anchor.Center)
				{
					float angle2 = parentAnchor.ParentAnchorDrawPoint().Direction(parentAnchor.Origin());
					sb.DrawLine(parentAnchor.ParentAnchorDrawPoint() + Math2.LdirVector2(12, angle2), parentAnchor.Origin(), Color.Red);
				}
			}
		}
		public void Render3(SpriteBatch sb)
		{
			if (Main.hideUI || !IsVisible()) return;
			if (!locked && Main.playerInventory)
			{
				sb.Draw(Main.HBLockTexture[locked ? 0 : 1], FrameLockPos(), null, parentAnchorMode ? Color.Red : Color.White, 0f, Main.HBLockTexture[0].Size() / 2, 1f, SpriteEffects.None, 0f);
			}
		}

		public Vector2 FramePos1()
		{
			return FramePos1NoParent() + parentAnchor.Origin();
		}
		public Vector2 FramePos1NoParent()
		{
			switch (anchor)
			{
				case Anchor.Top: return new Vector2(pos.X - size.X * .5f, pos.Y);
				case Anchor.TopRight: return new Vector2(pos.X - size.X, pos.Y);
				case Anchor.Left: return new Vector2(pos.X, pos.Y - size.Y * .5f);
				case Anchor.Center: return new Vector2(pos.X - size.X * .5f, pos.Y - size.Y * .5f);
				case Anchor.Right: return new Vector2(pos.X - size.X, pos.Y - size.Y * .5f);
				case Anchor.BottomLeft: return new Vector2(pos.X, pos.Y - size.Y);
				case Anchor.Bottom: return new Vector2(pos.X - size.X * .5f, pos.Y - size.Y);
				case Anchor.BottomRight: return new Vector2(pos.X - size.X, pos.Y - size.Y);
				default: return pos;
			}
		}
		public Vector2 FramePos2()
		{
			return FramePos2NoParent() + parentAnchor.Origin();
		}
		public Vector2 FramePos2NoParent()
		{
			switch (anchor)
			{
				case Anchor.Bottom: return new Vector2(pos.X + size.X * .5f, pos.Y);
				case Anchor.BottomLeft: return new Vector2(pos.X + size.X, pos.Y);
				case Anchor.Right: return new Vector2(pos.X, pos.Y + size.Y * .5f);
				case Anchor.Center: return new Vector2(pos.X + size.X * .5f, pos.Y + size.Y * .5f);
				case Anchor.Left: return new Vector2(pos.X + size.X, pos.Y + size.Y * .5f);
				case Anchor.TopRight: return new Vector2(pos.X, pos.Y + size.Y);
				case Anchor.Top: return new Vector2(pos.X + size.X * .5f, pos.Y + size.Y);
				case Anchor.TopLeft: return new Vector2(pos.X + size.X, pos.Y + size.Y);
				default: return pos;
			}
		}
		public Vector2 FrameLockPos()
		{
			return pos + parentAnchor.Origin();
		}
		public void ConvertToAnchor(Anchor anchor = Anchor.TopLeft)
		{
			pos = FramePos1NoParent();
			this.anchor = Anchor.TopLeft;
			if (anchor == Anchor.TopLeft) return;
			switch (anchor)
			{
				case Anchor.Top: pos.X += size.X * .5f; break;
				case Anchor.TopRight: pos.X += size.X; break;
				case Anchor.Left: pos.Y += size.Y * .5f; break;
				case Anchor.Center: pos.X += size.X * .5f; pos.Y += size.Y * .5f; break;
				case Anchor.Right: pos.X += size.X; pos.Y += size.Y * .5f; break;
				case Anchor.BottomLeft: pos.Y += size.Y; break;
				case Anchor.Bottom: pos.X += size.X * .5f; pos.Y += size.Y; break;
				case Anchor.BottomRight: pos.X += size.X; pos.Y += size.Y; break;
			}
			this.anchor = anchor;
		}

		protected virtual void OnCreate() { }
		protected virtual void OnDestroy() { }
		protected virtual void OnUpdate() { }
		protected virtual bool ShouldRenderFrame() { return !locked; }
		protected virtual void OnRender(SpriteBatch sb) { }
		protected virtual void OnSave(JsonData j) { }
		protected virtual void OnLoad(JsonData j) { }
		protected virtual bool IsVisible() { return true; }
		protected virtual void OnUnlockedRightClick() { }
	}
}