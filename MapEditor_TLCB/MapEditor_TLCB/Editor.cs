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

namespace MapEditor_TLCB
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		private EntityWorld world;
		private Manager manager;

		private Window dialog;
		private	KeyboardState oldState;

		public Game1(bool p_useFullScreen, bool p_useMaxiumRes)
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			IsMouseVisible = true;

			graphics.IsFullScreen = p_useFullScreen;
			graphics.SynchronizeWithVerticalRetrace = true;

			if (p_useMaxiumRes)
			{
				graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}
			else
			{
				graphics.PreferredBackBufferWidth = 1280;
				graphics.PreferredBackBufferHeight = 720;
			}
			graphics.ApplyChanges();
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

			// Create and setup Window control.
			dialog = new Window(manager);
			dialog.Init();
			dialog.Text = "Render Targets";
			dialog.Width = 480;
			dialog.Height = 200;
			dialog.Center();
			dialog.Visible = true;

			manager.Add(dialog);
	

			world = new EntityWorld();
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

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
			KeyboardState newState = Keyboard.GetState(0);
			if (newState.IsKeyUp(Keys.Escape))
			{
				if (oldState.IsKeyDown(Keys.Escape))
				{
					this.Exit();
				}
			}

			if (newState.IsKeyDown(Keys.LeftControl))
			{
				if (newState.IsKeyUp(Keys.Z))
				{
					if (oldState.IsKeyDown(Keys.Z))
					{
						//ActionSystem sys = (ActionSystem)m_world.SystemManager.GetSystem<ActionSystem>()[0];
						//sys.UndoLastPerformedAction();
					}
				}
				if (newState.IsKeyUp(Keys.Y))
				{
					if (oldState.IsKeyDown(Keys.Y))
					{
						//ActionSystem sys = (ActionSystem)m_world.SystemManager.GetSystem<ActionSystem>()[0];
						//sys.RedoLastAction();
					}
				}
			}
			// Call manager updates.
			manager.Update(gameTime);
			world.Update(gameTime.ElapsedGameTime.Seconds);

			oldState = newState;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			manager.BeginDraw(gameTime);

			spriteBatch.Begin();
			GraphicsDevice.Clear(Color.White);

			spriteBatch.End();
			base.Draw(gameTime);
			manager.EndDraw();
		}
	}
}