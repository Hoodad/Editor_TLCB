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
		bool useFullScreen;
		bool useMaxRes;

		private	KeyboardState oldState;

		public Editor(bool p_useFullScreen, bool p_useMaxRes)
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			textures = new Dictionary<string, Texture2D>();
			useFullScreen = p_useFullScreen;
			useMaxRes = p_useMaxRes;
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

			// Layouts are not used
			//manager.LayoutDirectory = "../../../Neoforce/Layouts";

			// Setting up the shared skins directory
			manager.SkinDirectory = "../../../Neoforce/Skins";
			manager.RenderTarget = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
			manager.TargetFrames = 120;
			manager.Initialize();

			world = new EntityWorld();

			if (useMaxRes)
			{
				graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				canvasRender = new RenderTarget2D(GraphicsDevice,
					GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
					GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
			}
			else
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
				canvasRender = new RenderTarget2D(GraphicsDevice, 1280, 720);
			}
			IsMouseVisible = true;

			graphics.IsFullScreen = useFullScreen;
			graphics.SynchronizeWithVerticalRetrace = true;

			graphics.ApplyChanges();

			base.Initialize();
		}

		public void InitializeAllSystem()
		{
			SystemManager systemManager = world.SystemManager;
			systemManager.SetSystem(new CanvasControlSystem(manager, canvasRender), ExecutionType.Update); // Canvas window is furthest back.
			systemManager.SetSystem(new ActionSystem(), ExecutionType.Update);
			systemManager.SetSystem(new ContentSystem(Content,graphics), ExecutionType.Update);
			systemManager.SetSystem(new ToolbarSystem(manager), ExecutionType.Update);
			systemManager.SetSystem(new UndoTreeSystem(manager,GraphicsDevice,Content), ExecutionType.Update);
			systemManager.SetSystem(new NotificationBarSystem(manager, GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new TilemapBarSystem(manager), ExecutionType.Update);
			systemManager.SetSystem(new XNAInputSystem(), ExecutionType.Update);
			systemManager.SetSystem(new StateSystem(manager), ExecutionType.Update);
			systemManager.SetSystem(new RoadAndWallMapperSystem(), ExecutionType.Update);
			systemManager.SetSystem(new RoadToolSystem(), ExecutionType.Update);
			systemManager.SetSystem(new CurrentToolSystem(manager, GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new StartupDialogSystem(manager), ExecutionType.Update);
			world.SystemManager.SetSystem(new RadialMenuSystem(GraphicsDevice, Content), ExecutionType.Update);
			systemManager.SetSystem(new MapValidationSystem(manager), ExecutionType.Update);
			
			world.SystemManager.SetSystem(new DrawCanvasSystem(textures, GraphicsDevice,
				canvasRender, manager), ExecutionType.Draw);
			world.SystemManager.InitializeAll();
		}

		private void InitializeEntities()
		{
			Entity entity = world.CreateEntity();
			entity.Tag = "mainCamera";
			entity.AddComponent(new Transform(new Vector2(200.0f, 100.0f), 1.0f));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "mainTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32));
			entity.AddComponent(new Transform(new Vector2(0, 0)));
			entity.AddComponent(new TilemapRender("tilemap_garden", false));
			entity.AddComponent(new TilemapValidate());
			entity.Refresh();
			
			entity = world.CreateEntity();
			entity.Tag = "singlesTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32));
			entity.Refresh();

			entity = world.CreateEntity();
			entity.Tag = "roadTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32));
			entity.AddComponent(new Transform(new Vector2(0, 0)));
			entity.Refresh();
			
			entity = world.CreateEntity();
			entity.Tag = "wallTilemap";
			entity.AddComponent(new Tilemap(60, 31, 32, 32));
			entity.Refresh();
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

			System.Windows.Forms.Form f = System.Windows.Forms.Form.FromHandle(Window.Handle) as System.Windows.Forms.Form;
			if (f != null)
			{
				f.FormClosing += f_FormClosing;
			}
			
			textures.Add("tilemap_garden", Content.Load<Texture2D>("TileSheets/tilemap_garden"));
			textures.Add("tilemap_winecellar", Content.Load<Texture2D>("TileSheets/tilemap_winecellar"));
			textures.Add("debugBlock", Content.Load<Texture2D>("debugBlock"));
			textures.Add("canvas_shadow", Content.Load<Texture2D>("canvas_shadow"));
			textures.Add("canvas_shadow_10px", Content.Load<Texture2D>("canvas_shadow_10px"));
			textures.Add("canvas_shadow_30px", Content.Load<Texture2D>("canvas_shadow_30px"));

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
			
			if (newState.IsKeyDown(Keys.LeftControl))
			{
				if (newState.IsKeyUp(Keys.Z))
				{
					if (oldState.IsKeyDown(Keys.Z))
					{
						ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
						sys.UndoLastPerformedAction();
					}
				}
				if (newState.IsKeyUp(Keys.Y))
				{
					if (oldState.IsKeyDown(Keys.Y))
					{
						ActionSystem sys = (ActionSystem)world.SystemManager.GetSystem<ActionSystem>()[0];
						sys.RedoLastAction();
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