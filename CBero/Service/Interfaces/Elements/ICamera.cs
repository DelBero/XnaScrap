using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CBero.Service.Elements.Interfaces
{
    public interface ICamera
    {
        Matrix View
        {
            get;
        }

        Matrix Projection
        {
            get;
        }

        void set();
        void setView();

        /// <summary>
        /// Unproject screen coordinates
        /// </summary>
        /// <param name="screenCoords"></param>
        /// <param name="ray">the resulting ray</param>
        void unproject(Vector2 screenCoords, out Ray ray);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        Vector3 project(Vector3 worldPosition);
    }
}
