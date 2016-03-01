#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Collections.Generic;

#endregion
namespace GDAPSMapEditor
{
	public partial class MapEditor : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D blank;
		Texture2D nulltex;
		Texture2D up, down, left, right, x;
		Texture2D newMap, open, save, exit;
		Map map;
		Dictionary<String, Texture2D> tiles;
		int tileindex = 0;
		List<KeyValuePair<String,Texture2D>> tileList;
		ButtonState LButton, RButton, MButton, prevLButton, prevRButton, prevMButton;
		bool l, r, u, d, b, pl, pr, pu, pd, pb;
		Point cam = new Point(0, 0);
		Point cameraOffset = new Point(0, 0);
		Tile active = new Tile(0, "");
		String textInput = "";
		String message = "";
		InputType inputType;
		bool waitingForText = false;
		int newWidth;
		EditMode mode;

		public MapEditor()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = true;		
			tiles = new Dictionary<string, Texture2D>();
			tileList = new List<KeyValuePair<string, Texture2D>>();
		}

		protected override void Initialize()
		{
			map = new Map();
			//IsMouseVisible = true;
			base.Initialize();
			Window.TextInput += ReadText;
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			blank = new Texture2D(GraphicsDevice, 1, 1);
			blank.SetData(new Color[]{ Color.White });
			nulltex = new Texture2D(GraphicsDevice, 2, 2);
			nulltex.SetData(new Color[]{ Color.Black, Color.Magenta, Color.Black, Color.Magenta });
			up = Content.Load<Texture2D>("up");
			down = Content.Load<Texture2D>("down");
			left = Content.Load<Texture2D>("left");
			right = Content.Load <Texture2D>("right");
			x = Content.Load<Texture2D>("x");
			newMap = Content.Load<Texture2D>("new");
			open = Content.Load<Texture2D>("open");
			save = Content.Load<Texture2D>("save");
			exit = Content.Load<Texture2D>("exit");
			string[] tilenames = File.ReadAllLines("Content/tiles.txt");
			mode = EditMode.Tile;
			foreach(String tile in tilenames)
			{
				tiles[tile] = Content.Load<Texture2D>("tiles/" + tile);
			}
			foreach(KeyValuePair<String, Texture2D> tile in tiles)
			{
				tileList.Add(tile);
			}
			tiles["nulltex"] = nulltex;
		}

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
			//select the tile
			else if(LButton == ButtonState.Pressed && prevLButton == ButtonState.Released)
				{
					if(Mouse.GetState().X > GraphicsDevice.Viewport.Width - 96)
					{
						tileindex = Math.Min((Mouse.GetState().Y/48)*2 + (Mouse.GetState().X > GraphicsDevice.Viewport.Width - 96 + 48 ? 1 : 0),
						                     tileList.Count - 1);
					}
					if(Mouse.GetState().Y > GraphicsDevice.Viewport.Height - 96)
					{
						if(Mouse.GetState().Y < GraphicsDevice.Viewport.Height - 64)
						{
						
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
			//draw the bottom bar items
			spriteBatch.Draw(newMap, new Rectangle(0, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(open, new Rectangle(64, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(save, new Rectangle(128, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			spriteBatch.Draw(exit, new Rectangle(192, GraphicsDevice.Viewport.Height - 64, 64, 64), Color.White);
			//draw the mouse because the default cursor is wrong
			spriteBatch.Draw(blank, new Rectangle(Mouse.GetState().X - 3, Mouse.GetState().Y - 3, 6, 6), Color.Green);
			spriteBatch.Draw(blank, new Rectangle(Mouse.GetState().X - 2, Mouse.GetState().Y - 2, 4, 4), Color.Red);
			spriteBatch.End();
			base.Draw(gameTime);
		}

		private void ReadText(object sender, Microsoft.Xna.Framework.TextInputEventArgs e)
		{
			if(e.Character == '/')
			{
				return;
			}
			if((inputType == InputType.Width || inputType == InputType.Height) && (e.Character < '0' || e.Character > '9'))
			{
				return;
			}
			if(waitingForText)
			{
				textInput += e.Character;
			}
		}
	}
}

