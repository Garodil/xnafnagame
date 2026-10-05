using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using model.Models.Render;

namespace model.Interfaces.Service
{
    public interface IRenderTargetService
    {
        RenderTarget2D? GetRenderTarget(RenderTargetEnum targetTag);

        RenderTarget2D InitRenderTarget(RenderTargetEnum targetTag, int pixelizationScale, GraphicsDevice gd);

        void DisposeAllRenderTargets();
        void DisposeRenderTarget(RenderTargetEnum targetTag);
    }
}