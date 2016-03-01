using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GDAPSMapEditor
{
	public partial class MapEditor
	{
		protected override void Update(GameTime gameTime)
		{
			prevLButton = LButton;
			prevRButton = RButton;
			prevMButton = MButton;
			LButton = Mouse.GetState().LeftButton;
			RButton = Mouse.GetState().RightButton;
			MButton = Mouse.GetState().MiddleButton;
			pl = l;
			pr = r;
			pu = u;
			pd = d;
			pb = b;
			l = Keyboard.GetState().IsKeyDown(Keys.Left);
			r = Keyboard.GetState().IsKeyDown(Keys.Right);
			u = Keyboard.GetState().IsKeyDown(Keys.Up);
			d = Keyboard.GetState().IsKeyDown(Keys.Down);
			b = Keyboard.GetState().IsKeyDown(Keys.Back);

			if(waitingForText)
			{
				if(!Keyboard.GetState().IsKeyDown(Keys.Enter))
				{
					if(b && !pb)
					{
						textInput = textInput.Substring(0, Math.Max(0, textInput.Length - 1));
					}
				}
				else if(textInput != "")
				{
					waitingForText = false;
					if(inputType == InputType.Saving)
					{
						map.WriteMap(textInput);
					}
					else if(inputType == InputType.Loading)
					{
						map.LoadMap(textInput);
					}
					else if(inputType == InputType.Width)
					{
						newWidth = Int32.Parse(textInput);
						textInput = "";
						message = "Enter a height for the new map";
						waitingForText = true;
						inputType = InputType.Height;
					}
					else
					{
						map = new Map(newWidth, Int32.Parse(textInput));
						textInput = "Unsaved Map";
					}
				}
			}
			else
			{
				switch(mode)
				{
					case EditMode.Tile:
						try
						{
							active = map[(Mouse.GetState().X - cam.X)/64,(Mouse.GetState().Y - cam.Y)/64];
						}
						catch(Exception)
						{
						}
						//change movement flags
						active.Flags ^= (!pl && l ? MovementFlags.LEFT : 0);
						active.Flags ^= (!pr && r ? MovementFlags.RIGHT : 0);
						active.Flags ^= (!pu && u ? MovementFlags.UP : 0);
						active.Flags ^= (!pd && d ? MovementFlags.DOWN : 0);
						//draw the tile or change the movement flags
						if(Mouse.GetState().X < GraphicsDevice.Viewport.Width - 96 && Mouse.GetState().Y < GraphicsDevice.Viewport.Height - 96)
						{
							if(LButton == ButtonState.Pressed)
							{
								active.Filename = tileList[tileindex].Key;
							}
							if(RButton == ButtonState.Pressed && prevRButton == ButtonState.Released)
							{
								active.Flags ^= (Mouse.GetState().Y - cam.Y)%64 <= 21 && (Mouse.GetState().Y - cam.Y)%64 < (Mouse.GetState().X - cam.X)%64 && (Mouse.GetState().Y - cam.Y)%64 < 64 - (Mouse.GetState().X - cam.X)%64 ? MovementFlags.UP : 0;
								active.Flags ^= (Mouse.GetState().Y - cam.Y)%64 >= 43 && (Mouse.GetState().Y - cam.Y)%64 > (Mouse.GetState().X - cam.X)%64 && (Mouse.GetState().Y - cam.Y)%64 > 64 - (Mouse.GetState().X - cam.X)%64 ? MovementFlags.DOWN : 0;
								active.Flags ^= (Mouse.GetState().X - cam.X)%64 <= 21 && (Mouse.GetState().X - cam.X)%64 < (Mouse.GetState().Y - cam.Y)%64 && (Mouse.GetState().X - cam.X)%64 < 64 - (Mouse.GetState().Y - cam.Y)%64 ? MovementFlags.LEFT : 0;
								active.Flags ^= (Mouse.GetState().X - cam.X)%64 >= 43 && (Mouse.GetState().X - cam.X)%64 > (Mouse.GetState().Y - cam.Y)%64 && (Mouse.GetState().X - cam.X)%64 > 64 - (Mouse.GetState().Y - cam.Y)%64 ? MovementFlags.RIGHT : 0;
								active.Flags ^= (Mouse.GetState().X - cam.X)%64 > 21 && (Mouse.GetState().X - cam.X)%64 < 43 && (Mouse.GetState().Y - cam.Y)%64 > 21 && (Mouse.GetState().Y - cam.Y)%64 < 43 ? MovementFlags.DAMAGE : 0;
							}
						}
						else if(LButton == ButtonState.Pressed && prevLButton == ButtonState.Released)
						{
							if(Mouse.GetState().X > GraphicsDevice.Viewport.Width - 96)
							{
								tileindex = Math.Min((Mouse.GetState().Y/48)*2 + (Mouse.GetState().X > GraphicsDevice.Viewport.Width - 96 + 48 ? 1 : 0),
								                     tileList.Count - 1);
							}
						}
						break;
				}
				//select something from the sidebar or bottom bar
				if(LButton == ButtonState.Pressed && prevLButton == ButtonState.Released)
				{
					if(Mouse.GetState().Y > GraphicsDevice.Viewport.Height - 96)
					{
						if(Mouse.GetState().Y < GraphicsDevice.Viewport.Height - 64)
						{
							if(Mouse.GetState().X < 140 && !waitingForText)
							{
								mode = EditMode.Tile;
							}
							else if(Mouse.GetState().X < 281 && !waitingForText)
							{
								mode = EditMode.BG;
							}
							else if(Mouse.GetState().X < 422 && !waitingForText)
							{
								mode = EditMode.Parallax;
							}
							else if(Mouse.GetState().X < 563 && !waitingForText)
							{
								mode = EditMode.SFG;
							}
							else if(Mouse.GetState().X < 704 && !waitingForText)
							{
								mode = EditMode.Entity;
							}
						}
						else
						{
							if(Mouse.GetState().X < 64 && !waitingForText)
							{
								textInput = "";
								message = "Enter a width for the new map";
								waitingForText = true;
								inputType = InputType.Width;
							}
							else if(Mouse.GetState().X < 128)
							{
								textInput = "";
								message = "Enter the name of a map to open (no extension)";
								waitingForText = true;
								inputType = InputType.Loading;
							}
							else if(Mouse.GetState().X < 196)
							{
								textInput = "";
								message = "Enter the name to save this map as (no extension)";
								waitingForText = true;
								inputType = InputType.Saving;
							}
							else if(Mouse.GetState().X < 260)
							{
								Exit();
							}
						}
					}
				}

				//drag the map around with the middle button
				if(MButton == ButtonState.Pressed)
				{
					if(prevMButton == ButtonState.Released)
					{
						cameraOffset = new Point(Mouse.GetState().X, Mouse.GetState().Y);
					}
					cam = new Point(Mouse.GetState().X + cam.X - cameraOffset.X, Mouse.GetState().Y + cam.Y - cameraOffset.Y);
					cameraOffset = new Point(Mouse.GetState().X, Mouse.GetState().Y);
				}
				//constrain the map edges to the window edges
				cam = new Point(Math.Min(Math.Max(cam.X, -((map.Width - 1)*64) + GraphicsDevice.Viewport.Width - 96), 0),
				                Math.Min(Math.Max(cam.Y, -((map.Height - 1)*64 - GraphicsDevice.Viewport.Height + 96)), 0));

			}
			base.Update(gameTime);
		}
	}
}

