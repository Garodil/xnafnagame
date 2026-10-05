using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Vector2 = nkast.Aether.Physics2D.Common.Vector2;

namespace model.Interfaces.Objects
{
    public interface IDrawable
    {
        int DrawOrder
		{
			get;
		}

		bool Visible
		{
			get;
		}

		event EventHandler<EventArgs> DrawOrderChanged;
		event EventHandler<EventArgs> VisibleChanged;

        Texture2D GetTexture();
        Vector2 GetTexturePosition();
		float GetTextureRotation();
		Vector2 GetTextureScale();
		Vector2 GetTextureOrigin();
		Rectangle? GetTextureRectangle();
    }
}