using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Artemis;

using TomShane.Neoforce.Controls;
using MapEditor_TLCB.Systems;
using MapEditor_TLCB.Components;
using System.Diagnostics;
using System.IO;

namespace MapEditor_TLCB
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Editor : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private EntityWorld world;
		private Manager manager;
		Dictionary<string, Texture2D> textures;
		RenderTarget2D canvasRender;
		bool enableTilemapAutoSize = false;
		bool useMaxRes = false;
		float repeatDelay;
		float repeatTime;

		private KeyboardState oldState;

		public Editor(string[] arguments)
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			textures = new Dictionary<string, Texture2D>();

			foreach (string arg in arguments)
			{
				if (arg == "maximizeWindow")
				{
					useMaxRes = true;
				}
				else if (arg == "enableAutoSize")
				{
					enableTilemapAutoSize = true;
				}
			}

			repeatDelay = 0.3f;
			repeatTime = 0;			
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// Create an instance of manager using Default skin. We set the fourth parameter to false,
			// so the instance of manager is not registered as an XNA game component and methods
			// like Initialize(), Update() and Draw() are called manually in the game loop.
			manager = new Manager(this, graphics, "Default");

			// Setting up the shared skins directory
			manager.SkinDirectory = "Content/Skins/";
			manager.RenderTarget = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
			manager.TargetFrames = 120;
			manager.Initialize();
#if(!DEBUG)
			manager.LogUnhandledExceptions = false;
#endif
			world = new EntityWorld();


			if (useMaxRes && GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height > 960)
			{
				graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height-22;
				canvasRender = new RenderTarget2D(GraphicsDevice,
					GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
					GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 22);
			}
			else
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
				canvasRender = new RenderTarget2D(GraphicsDevice, 1280, 720);
			}
			IsMouseVisible = true;

			graphics.SynchronizeWithVerticalRetrace = true;
			graphics.ApplyChanges();

			Window.AllowUserResizing = false; //DONT CHANGE!
			Window.ClientSizeChanged += new EventHandler<System.EventArgs>(Window_ClientSizeChanged);


			System.Windows.Forms.Form xnaWindow = System.Windows.Forms.Form.FromHandle(Window.Handle) as System.Windows.Forms.Form;
			if (xnaWindow != null)
			{
				xnaWindow.FormClosing += f_FormClosing;
				xnaWindow.Text = "The Little Cheese Boy Editor - Pre Alpha";

				if (useMaxRes && GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height > 960)
				{
					xnaWindow.WindowState = System.Windows.Forms.FormWindowState.Maximized;
				}
			}

			base.Initialize();

			Debug.Print("Finished Initializing");
		}

		public void InitializeAllSystem()
		{
			SystemManager systemManager = world.SystemManager;
			systemManager.SetSystem(new InputDeltaSystem(), ExecutionType.Update);
			systemManager.SetSystem(new EventSystem(), ExecutionType.Update);
			systemManager.SetSystem(new CanvasControlSystem(manager, canvasRender), ExecutionType.Update); // Canvas window is furthest back.
			systemManager.SetSystem(new ContentSystem(Content, graphics, textures), ExecutionType.Update);
			systemManager.SetSystem(new ToolbarSystem(manager), ExecutionType.Update);
			systemManager.SetSystem(new UndoTreeSystem(manager, GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new ActionSystem(), ExecutionType.Update);
			systemManager.SetSystem(new NotificationBarSystem(manager, GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new TilemapBarSystem(manager, Content, enableTilemapAutoSize), ExecutionType.Update);
			systemManager.SetSystem(new XNAInputSystem(), ExecutionType.Update);
			systemManager.SetSystem(new StateSystem(manager), ExecutionType.Update);
			systemManager.SetSystem(new RoadAndWallMapperSystem(), ExecutionType.Update);
			systemManager.SetSystem(new RoadToolSystem(), ExecutionType.Update);
			systemManager.SetSystem(new CurrentToolSystem(manager, GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new StartupDialogSystem(manager, textures), ExecutionType.Update);
			systemManager.SetSystem(new RadialMenuSystem(GraphicsDevice, Content, manager), ExecutionType.Update);
			systemManager.SetSystem(new ExportMapSystem(), ExecutionType.Update); //Have to be run after tilemaphandling systems
			systemManager.SetSystem(new MapValidationSystem(manager), ExecutionType.Update);

			world.SystemManager.SetSystem(new DrawCanvasSystem(textures, GraphicsDevice,
				canvasRender, manager), ExecutionType.Draw);
			world.SystemManager.InitializeAll();
		}

		private void InitializeEntities()
		{
			Entity entity = world.CreateEntity();
			entity.Tag = "mainCamera";

			float zoomLevel = GraphicsDevice.Viewport.Width / 3000.0f;

			Vector2 pos = new Vector2();
			pos.X = Window.ClientBounds.Width/2; //center position
			pos.Y = Window.ClientBounds.Height/2; // -||-

			pos.X -= (1920 * zoomLevel) / 2; //offset the center dependent of the zoom level
			pos.Y -= (992 * zoomLevel) / 2; // -||-

			entity.AddComponent(new Transform(pos, zoomLevel));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "mainTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32, Tilemap.TilemapType.FinalTilemap, 31));
			entity.AddComponent(new Transform(new Vector2(0, 0)));
			entity.AddComponent(new TilemapRender("tilemap_garden", false));
			entity.AddComponent(new TilemapValidate());
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "singlesTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32, Tilemap.TilemapType.SingleTilemap));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "roadTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32, Tilemap.TilemapType.RoadTilemap));
			entity.AddComponent(new Transform(new Vector2(0, 0)));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "wallTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32, Tilemap.TilemapType.WallTilemap));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "input";
			entity.AddComponent(new InputDelta());
			entity.Refresh();
		}

		void Window_ClientSizeChanged(object sender, System.EventArgs e)
		{
			/*manager.Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
			manager.Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

			canvasRender = new RenderTarget2D(graphics.GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
			((DrawCanvasSystem)world.SystemManager.GetSystem<DrawCanvasSystem>()[0]).updateWindowSize(canvasRender);
			manager.Graphics.ApplyChanges();*/
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			KeyDelta.initialize();

			using (FileStream fileStream = new FileStream(@"Content\TileSheets\tilemap_garden.png", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				textures.Add("tilemap_garden", Texture2D.FromStream(graphics.GraphicsDevice, fileStream));
			}
			using (FileStream fileStream = new FileStream(@"Content\TileSheets\tilemap_winecellar.png", FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				textures.Add("tilemap_winecellar", Texture2D.FromStream(graphics.GraphicsDevice, fileStream));
			}
			textures.Add("debugBlock", Content.Load<Texture2D>("debugBlock"));
			textures.Add("canvas_shadow", Content.Load<Texture2D>("canvas_shadow"));
			textures.Add("canvas_shadow_10px", Content.Load<Texture2D>("canvas_shadow_10px"));
			textures.Add("canvas_shadow_30px", Content.Load<Texture2D>("canvas_shadow_30px"));
			textures.Add("TileSelector", Content.Load<Texture2D>("TileSelector_v3"));
			textures.Add("canvas_grid", Content.Load<Texture2D>("canvas_grid"));

            textures.Add("player", Content.Load<Texture2D>("OtherSheets/player"));
            textures.Add("rat", Content.Load<Texture2D>("OtherSheets/rat"));
            textures.Add("rat2", Content.Load<Texture2D>("OtherSheets/rat2"));
            textures.Add("robot", Content.Load<Texture2D>("OtherSheets/robotaparte"));
            textures.Add("trap", Content.Load<Texture2D>("OtherSheets/Trap_Spikes"));
            textures.Add("switch", Content.Load<Texture2D>("OtherSheets/Switch_Tileset"));
            textures.Add("block", Content.Load<Texture2D>("OtherSheets/Blockade_Tileset"));

            textures.Add("bomb", Content.Load<Texture2D>("OtherSheets/bombitem"));
            textures.Add("speed", Content.Load<Texture2D>("OtherSheets/speedpowerup"));
            textures.Add("super", Content.Load<Texture2D>("OtherSheets/Item_SuperCheesy"));

			InitializeAllSystem();
			InitializeEntities();

			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			KeyDelta.update();
			StateSystem stateSys = (StateSystem)world.SystemManager.GetSystem<StateSystem>()[0];
			ActionSystem actionSys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
			if (stateSys.ShouldShutDown())
			{
				this.Exit();
			}
			KeyboardState newState = Keyboard.GetState(0);
			if (newState.IsKeyUp(Keys.Escape))
			{
				if (oldState.IsKeyDown(Keys.Escape))
				{
					stateSys.RequestToShutdown();
				}
			}

			if (stateSys.CanCanvasBeReached())
			{
				if (newState.IsKeyDown(Keys.LeftControl))
				{
					if (newState.IsKeyUp(Keys.Z))
					{
						if (oldState.IsKeyDown(Keys.Z))
						{
							ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
							sys.UndoLastPerformedAction();
							repeatTime = 0;
						}
					}
					if (newState.IsKeyUp(Keys.Y))
					{
						if (oldState.IsKeyDown(Keys.Y))
						{
							ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
							sys.RedoLastAction();
							repeatTime = 0;
						}
					}
					//Repeat delay
					if (newState.IsKeyDown(Keys.Z) && oldState.IsKeyDown(Keys.Z))
					{
						repeatTime += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

						if (repeatTime > repeatDelay)
						{
							ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
							sys.UndoLastPerformedAction();
							repeatTime -= 0.1f;
						}
					}
					if (newState.IsKeyDown(Keys.Y) && oldState.IsKeyDown(Keys.Y))
					{
						repeatTime += gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

						if (repeatTime > repeatDelay)
						{
							ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
							sys.RedoLastAction();
							repeatTime -= 0.1f;
						}
					}

				}
			}
			// Call manager updates.
			manager.Update(gameTime);
			world.Delta = gameTime.ElapsedGameTime.Milliseconds;
			world.LoopStart();
			world.SystemManager.UpdateSynchronous(ExecutionType.Update);

			oldState = newState;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			manager.BeginDraw(gameTime);
			world.SystemManager.UpdateSynchronous(ExecutionType.Draw);
			base.Draw(gameTime);
			manager.EndDraw();

			spriteBatch.Begin();
			RadialMenuSystem radial = (RadialMenuSystem)world.SystemManager.GetSystem<RadialMenuSystem>()[0];
			radial.Render(spriteBatch);
			spriteBatch.End();
		}
		void f_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			StateSystem stateSys = (StateSystem)world.SystemManager.GetSystem<StateSystem>()[0];
			// Check if our systems has confirmed the closing of the window
			if (stateSys.ShouldShutDown() == false)
			{
				e.Cancel = true;
				stateSys.RequestToShutdown();
			}
		}
	}
}