using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapEditor_TLCB.Common;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MapEditor_TLCB.Actions;

namespace MapEditor_TLCB.CustomControls
{

    class UndoTreeContainer : Control
    {        
        public int m_startId = 0;
        public bool m_treestate = true;

        private Window m_windowParent;
        private GraphicsDevice m_gd;
        public UndoTree m_undoTree;

        float tck = 0.0f;


        public UndoTreeContainer(Manager p_manager, Window p_parent, GraphicsDevice p_gd,
            ContentManager p_content)
            : base(p_manager)
        {
            m_windowParent = p_parent;
            m_gd = p_gd;
            m_undoTree = new UndoTree(p_gd,p_content);
        }

        public void Update(float p_dt)
        {
            m_undoTree.update(p_dt);
            tck += 200.0f*p_dt;
            Refresh();            
            m_windowParent.Refresh();
        }


        protected override void DrawControl(TomShane.Neoforce.Controls.Renderer renderer, 
            Microsoft.Xna.Framework.Rectangle rect,
            Microsoft.Xna.Framework.GameTime gameTime)
        {
            //renderer.Draw(tilemapImage, rect, Color.White);
            //renderer.Draw(tileSelectorImage, selectorRect, Color.White);
            /*Vector2 offset = new Vector2(-m_windowParent.ScrollBarValue.Horizontal,
                -m_windowParent.ScrollBarValue.Vertical);*/

            m_undoTree.draw(renderer.SpriteBatch, 
                new Vector2(Parent.AbsoluteLeft, Parent.AbsoluteTop));
            
            // resizing
            /*
            Width = Math.Max(OriginWidth,(int)tck);
            Height = Math.Max(OriginHeight, (int)(tck*0.1f));
            //m_windowParent.Width = (int)tck;
            m_windowParent.MovableArea = new Rectangle(0,0,(int)tck,(int)(tck*0.1f));
             * */
        }

		// THIS WAS EVIL ITSELF ->
//        protected override void OnMouseMove(TomShane.Neoforce.Controls.MouseEventArgs e)
//        {
//            //Debug.Print("Mouse " + e.Position.ToString());
//            //Debug.Print( "Scrollvalue {X "+windowParent.ScrollBarValue.Horizontal + ", Y "+ windowParent.ScrollBarValue.Vertical+"}");
//
//            //selectorRect.X = (e.Position.X / tileSize.X);
//            //selectorRect.Y = (e.Position.Y / tileSize.Y);
//            ////Debug.Print( "Resulting Tile {X: "+selectorRect.X +" Y: "+ selectorRect.Y+"}");
//            //selectorRect.X *= tileSize.X;
//            //selectorRect.Y *= tileSize.Y;
//            //selectorRect.X += 6;	//Window thickness
//            //selectorRect.Y += 28;	// -||-
//            //selectorRect.X -= windowParent.ScrollBarValue.Horizontal;	//
//            //selectorRect.Y -= windowParent.ScrollBarValue.Vertical;		//
//            //
//            Refresh();
//        }


    }
}
