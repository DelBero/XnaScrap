using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace XnaScrapCore.Core.Systems.Resource
{
    public class MaterialReader : ContentTypeReader<Material>
    {
        /// <summary>
        /// Loads an imported shader.
        /// </summary>
        protected override Material Read(ContentReader input,
            Material existingInstance)
        {
            Material ret = new Material();
            ret.deserialize(input);
            #region old
            //int numTex = input.ReadInt32();
            //for (int i = 0; i < ret.TextureNames.Length; i++)
            //{
            //    int unit = input.ReadInt32();
            //    String texture = input.ReadString();
            //    if (unit >= 0 && unit < ret.TextureNames.Length)
            //    {
            //        ret.TextureNames[unit] = texture;
            //    }
            //}

            //int parameterCount = input.ReadInt32();
            
            //for (int i = 0; i < parameterCount; ++i )
            //{
            //    Material.ParameterMapping pm = new Material.ParameterMapping();
            //    pm.name = input.ReadString();
            //    pm.value= input.ReadString();
            //    pm.autoUpdate = input.ReadBoolean();
            //    ret.ParameterMap.Add(pm);
            //}

            //bool shadersIncluded = input.ReadBoolean();
            //if (shadersIncluded)
            //{
            //    ret.VertexShaderString = input.ReadString(); 
            //    ret.PixelShaderString = input.ReadString();
            //}
            #endregion
            return ret;
        }
    }

    public class ParameterMappingReader : ContentTypeReader<Material.ParameterMapping>
    {
        /// <summary>
        /// Loads an imported shader.
        /// </summary>
        protected override Material.ParameterMapping Read(ContentReader input,
            Material.ParameterMapping existingInstance)
        {
            Material.ParameterMapping ret = new Material.ParameterMapping();
            ret.deserialize(input);
            return ret;
        }
    }
}
