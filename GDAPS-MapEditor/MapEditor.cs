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
		Texture2D editTiles, chooseBG, chooseParallax, chooseSFG, editEntities;
		Map map;
		Dictionary<String, Texture2D> tiles;
		Dictionary<String, Texture2D> backgrounds;
		Dictionary<String, Texture2D> parallaxes;
		Dictionary<String, Texture2D> superForegrounds;
		int tileindex = 0;
		int backgroundindex = 0;
		int parallaxindex = 0;
		int sfgindex = 0;
		List<KeyValuePair<String,Texture2D>> tileList;
		List<KeyValuePair<String, Texture2D>> backgroundList;
		List<KeyValuePair<String, Texture2D>> parallaxList;
		List<KeyValuePair<String, Texture2D>> sfgList;
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
		Entity activeEntity;

		public MapEditor()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = true;		
			tiles = new Dictionary<string, Texture2D>();
			backgrounds = new Dictionary<string, Texture2D>();
			parallaxes = new Dictionary<string, Texture2D>();
			superForegrounds = new Dictionary<string, Texture2D>();
			tileList = new List<KeyValuePair<string, Texture2D>>();
			backgroundList = new List<KeyValuePair<string, Texture2D>>();
			parallaxList = new List<KeyValuePair<string, Texture2D>>();
			sfgList = new List<KeyValuePair<string, Texture2D>>();
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
			editTiles = Content.Load<Texture2D>("tiles");
			chooseBG = Content.Load<Texture2D>("bg");
			chooseParallax = Content.Load<Texture2D>("parallax");
			chooseSFG = Content.Load<Texture2D>("superForeground");
			editEntities = Content.Load<Texture2D>("entities");
			string[] tilenames = File.ReadAllLines("Content/map_assets/tiles.txt");
			string[] backgroundnames = File.ReadAllLines("Content/map_assets/backgrounds.txt");
			string[] parallaxnames = File.ReadAllLines("Content/map_assets/parallaxes.txt");
			string[] superforegroundnames = File.ReadAllLines("Content/map_assets/superForegrounds.txt");
			foreach(String tile in tilenames)
			{
				tiles[tile] = Content.Load<Texture2D>("map_assets/" + tile);
			}
			foreach(KeyValuePair<String, Texture2D> tile in tiles)
			{
				tileList.Add(tile);
			}
			foreach(String background in backgroundnames)
			{
				backgrounds[background] = Content.Load<Texture2D>("map_assets/" + background);
			}
			foreach(KeyValuePair<String, Texture2D> background in backgrounds)
			{
				backgroundList.Add(background);
			}
			foreach(String parallax in parallaxnames)
			{
				parallaxes[parallax] = Content.Load<Texture2D>("map_assets/" + parallax);
			}
			foreach(KeyValuePair<String, Texture2D> parallax in parallaxes)
			{
				parallaxList.Add(parallax);
			}
			foreach(String sfg in superforegroundnames)
			{
				superForegrounds[sfg] = Content.Load<Texture2D>("map_assets/" + sfg);
			}
			foreach(KeyValuePair<String, Texture2D> sfg in superForegrounds)
			{
				sfgList.Add(sfg);
			}
			tiles["nulltex"] = nulltex;
			mode = EditMode.Tile;
		}

		private void ReadText(object sender, Microsoft.Xna.Framework.TextInputEventArgs e)
		{
			if((inputType == InputType.Loading || inputType == InputType.Saving) && e.Character == '/')
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

