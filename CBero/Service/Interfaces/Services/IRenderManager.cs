using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Elements.Interfaces;

namespace CBero.Service.Interface
{
    interface IRenderManager
    {
        /// <summary>
        /// Adds a renderable to the list of drawable renderables.
        /// </summary>
        /// <param name="renderable"></param>
        void addRenderable(IRenderable3D renderable, int layer);

        /// <summary>
        /// Removes a renderable from the list of drawable renderables.
        /// </summary>
        /// <param name="renderable"></param>
        void removeRenderable(IRenderable3D renderable, int layer);

        /// <summary>
        /// Define a new renderlayer with a specific id. Throws an exception if too many layers are defined
        /// </summary>
        /// <param name="layerId"></param>
        void addRenderLayer(int layerId);

        int getMaxLayerCount();

        int getCurrentLayerCount();

        int getAvailableLayerCount();
    }
}
