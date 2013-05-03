using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor_TLCB.CustomControls
{
	class ImageBasedButton : Button
	{
		public Texture2D image;
		public byte imageOpacity;

		public ImageBasedButton(Manager p_manager)
			: base(p_manager)
		{
			imageOpacity = 96;
		}

		protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, Microsoft.Xna.Framework.Rectangle rect, Microsoft.Xna.Framework.GameTime gameTime)
		{
			Color color = Color.White;
			color.A = imageOpacity;

			base.DrawControl(renderer, rect, gameTime);
			renderer.Draw(image, rect, color);
		}
		public void GenerateFirstTile(MapEditor_TLCB.Systems.ContentSystem p_contentSystem)
		{
			Texture2D originalTexture = image;
			Rectangle sourceRectangle = new Rectangle(0, 0, 32, 32);

			Texture2D cropTexture = p_contentSystem.CreateANewTexture2D(32, 32);
			Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
			originalTexture.GetData(0, sourceRectangle, data, 0, data.Length);
			cropTexture.SetData(data);

			image = cropTexture;
		}
	}
}
