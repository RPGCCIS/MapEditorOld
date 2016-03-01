using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GDAPSMapEditor
{
	public partial class MapEditor
	{
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin();
			Window.Title = textInput == "" && waitingForText ? message : textInput;
			for(int i = -cam.X/64; i < Math.Min(map.Width, (GraphicsDevice.Viewport.Width - cam.X - 32)/64); ++i)
			{
				for(int j = -cam.Y/64; j < Math.Min(map.Height, (GraphicsDevice.Viewport.Height - cam.Y - 32)/64); ++j)
				{
					if(map[i,j].Filename != "transparent")
					{
						if(tiles[map[i,j].Filename] == null)
						{
							spriteBatch.Draw(nulltex, new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64), Color.White);
						}
						else
						{
							spriteBatch.Draw(tiles[map[i,j].Filename],
							                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
							                 Color.White);
						}
					}
					spriteBatch.Draw(up,
					                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
					                 new Color(127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.UP) ? 1 : 0)),
					                           127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.UP) ? 20 : 0))));
					spriteBatch.Draw(down,
					                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
					                 new Color(127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.DOWN) ? 1 : 0)),
					                           127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.DOWN) ? 20 : 0))));
					spriteBatch.Draw(left,
					                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
					                 new Color(127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.LEFT) ? 1 : 0)),
					                           127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.LEFT) ? 20 : 0))));
					spriteBatch.Draw(right,
					                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
					                 new Color(127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.RIGHT) ? 1 : 0)),
					                           127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.RIGHT) ? 20 : 0))));
					spriteBatch.Draw(x,
					                 new Rectangle(i*64 + cam.X, j*64 + cam.Y, 64, 64),
					                 new Color(255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.DAMAGE) ? 1 : 0)),
					                           127,
					                           127,
					                           255/(1 + (!map[i,j].Flags.HasFlag(MovementFlags.DAMAGE) ? 20 : 0))));
				}
			}
			//draw the sidebar and bottom bar
			spriteBatch.Draw(blank,
			                 new Rectangle(GraphicsDevice.Viewport.Width - 96, 0, 96, GraphicsDevice.Viewport.Height),
			                 Color.LightGray);
			spriteBatch.Draw(blank,
			                 new Rectangle(0,
			                               GraphicsDevice.Viewport.Height - 96,
			                               GraphicsDevice.Viewport.Width - 96,
			                               96),
			                 Color.LightGray);
			//draw the sidebar items
			switch(mode)
			{
				case EditMode.Tile:
					spriteBatch.Draw(blank,
					                 new Rectangle(GraphicsDevice.Viewport.Width - 96 + (tileindex%2)*48, (tileindex/2)*48, 48, 48),
					                 Color.Red);
					int k = 0;
					foreach(KeyValuePair<String, Texture2D> tile in tileList)
					{
						spriteBatch.Draw(tile.Value,
						                 new Rectangle(GraphicsDevice.Viewport.Width - 94 + (k%2)*48, 2 + (k/2)*48, 44, 44),
						                 Color.White);
						++k;
					}
					break;
			}
			//draw the bottom bar items
			spriteBatch.Draw(newMap, new Rectangle(0, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(open, new Rectangle(64, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(save, new Rectangle(128, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(exit, new Rectangle(192, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(editTiles, new Rectangle(0, GraphicsDevice.Viewport.Height - 96, 140, 32), Color.White);
			spriteBatch.Draw(chooseBG, new Rectangle(140, GraphicsDevice.Viewport.Height - 96, 141, 32), Color.White);
			spriteBatch.Draw(chooseParallax, new Rectangle(281, GraphicsDevice.Viewport.Height - 96, 141, 32), Color.White);
			spriteBatch.Draw(chooseSFG, new Rectangle(422, GraphicsDevice.Viewport.Height - 96, 141, 32), Color.White);
			spriteBatch.Draw(editEntities, new Rectangle(563, GraphicsDevice.Viewport.Height - 96, 141, 32), Color.White);
			//draw the mouse because the default cursor is wrong
			spriteBatch.Draw(blank, new Rectangle(Mouse.GetState().X - 3, Mouse.GetState().Y - 3, 6, 6), Color.Green);
			spriteBatch.Draw(blank, new Rectangle(Mouse.GetState().X - 2, Mouse.GetState().Y - 2, 4, 4), Color.Red);
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}

