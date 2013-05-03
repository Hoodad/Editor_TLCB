using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.Systems
{
	class ContentSystem : EntitySystem
	{
		ContentManager contentManager;
		GraphicsDeviceManager deviceManager;

		public ContentSystem(ContentManager p_contentManager, 
			GraphicsDeviceManager p_deviceManager) : base()
		{
			contentManager = p_contentManager;
			deviceManager = p_deviceManager;
		}
		public Texture2D LoadTexture(string p_asset)
		{
			return contentManager.Load<Texture2D>(p_asset);
		}
		public Viewport GetViewport()
		{
			return deviceManager.GraphicsDevice.Viewport;
		}
		public Point GetViewportSize()
		{
			return new Point(deviceManager.GraphicsDevice.Viewport.Width, deviceManager.GraphicsDevice.Viewport.Height);
		}
		public Texture2D CreateANewTexture2D(int p_width, int p_height)
		{
			return new Texture2D(deviceManager.GraphicsDevice, p_width, p_height);
		}
	}
}
