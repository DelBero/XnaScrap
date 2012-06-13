using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace XnaScrapCore.Core.Systems.Resource
{

    /////////////////////////////
    // TODO extract Interface move this to CBero
    /////////////////////////////
    public class Material
    {
        #region structs and enums
        public enum ShaderParameterSemantic
        {
            TEXTURE2D0,
            TEXTURE2D1,
            TEXTURE2D2,
            TEXTURE2D3,
            MODEL_MATRIX,
            MODEL_INV_MATRIX,
            MODEL_INV_TRANS_MATRIX,
            MODEL_TRANS_MATRIX,
            VIEW_MATRIX,
            VIEW_INV_MATRIX,
            VIEW_TRANS_MATRIX,
            VIEW_INV_TRANS_MATRIX,
            PROJECTION_MATRIX,
            VIEW_PROJECTION_MATRIX,
            MODEL_VIEW_PROJECTION_MATRIX,
            BONE_TRANSFORMS,
            AMBIENTLIGHTING,
            BACKGROUNDCOLOR,
            FOGCOLOR,
            USER_DEFINED_FLOAT,
            USER_DEFINED_VEC2,
            USER_DEFINED_VEC3,
            USER_DEFINED_VEC4
        };

        public class ParameterMapping
        {
            public String name;  // TODO convert this to an index
            public int index;
            public ShaderParameterSemantic semantic;
            public String value;
            public bool perInstance;

            public void serialize(BinaryWriter writer)
            {
                writer.Write(name);
                writer.Write(Enum.GetName(typeof(ShaderParameterSemantic), semantic));
                if (value != null)
                    writer.Write(value);
                else
                    writer.Write("");
                writer.Write(perInstance);
            }

            public void deserialize(BinaryReader reader)
            {
                name = reader.ReadString();
                Enum.TryParse(reader.ReadString(), out semantic);
                value = reader.ReadString();
                perInstance = reader.ReadBoolean();
            }

        }
        #endregion

        #region member
        private Texture2D[] m_textures = new Texture2D[4];
        private String[] m_textureNames = new String[4];

        public Texture2D[] Textures
        {
            get { return m_textures; }
            set { m_textures = value; }
        }

        public String[] TextureNames
        {
            get { return m_textureNames; }
            set { m_textureNames = value; }
        }

        private String m_effectName = "";

        public String EffectName
        {
            get { return m_effectName; }
            set { m_effectName = value; }
        }

        private Effect m_effect;

        public Effect Effect
        {
            get { return m_effect; }
            set { m_effect = value; }
        }

        private String m_technique = "";

        public String Technique
        {
            get { return m_technique; }
            set { m_technique = value; }
        }

        private List<ParameterMapping> m_parameterMapping = new List<ParameterMapping>();

        public List<ParameterMapping> ParameterMappings
        {
            get { return m_parameterMapping; }
        }
        #endregion

        #region (de)serialisation

        /// <summary>
        /// This is used by the material importer
        /// </summary>
        /// <param name="writer"></param>
        public void serialize(BinaryWriter writer)
        {
            writer.Write(4); // num textures
            foreach (String textureName in TextureNames)
            {
                if (!string.IsNullOrEmpty(textureName))
                    writer.Write(textureName);
                else
                    writer.Write("");
            }

            writer.Write(EffectName);
            writer.Write(Technique);

            writer.Write(ParameterMappings.Count);
            foreach (ParameterMapping mapping in ParameterMappings)
            {
                mapping.serialize(writer);
            }
        }


        /// <summary>
        /// Called by the MaterialReader
        /// </summary>
        /// <param name="reader"></param>
        public void deserialize(BinaryReader reader)
        {
            int texCount = reader.ReadInt32();
            for (int i = 0; i < texCount; ++i)
            {
                TextureNames[i] = reader.ReadString();
            }

            m_effectName = reader.ReadString();
            m_technique = reader.ReadString();

            int paramCount = reader.ReadInt32();
            for (int i = 0; i < paramCount; ++i)
            {
                ParameterMapping param = new ParameterMapping();
                param.deserialize(reader);
                m_parameterMapping.Add(param);
            }
        }
        #endregion
        
    }

    //////////////////////
    // OLD VERSION
    //////////////////////

    public class XnaScrapMaterial
    {
        #region member
        private String[] m_textureNames = new String[4];

        public String[] TextureNames
        {
            get { return m_textureNames; }
            set { m_textureNames = value; }
        }


        private bool m_shadersIncluded = false;
        /// <summary>
        /// Is true when the material includes the shader code
        /// </summary>
        public bool ShadersIncluded
        {
            get { return m_shadersIncluded; }
            set { m_shadersIncluded = value; }
        }

        private String m_vertexShader = "";

        public String VertexShaderString
        {
            get { return m_vertexShader; }
            set { m_vertexShader = value; }
        }
        private String m_pixelShader = "";

        public String PixelShaderString
        {
            get { return m_pixelShader; }
            set { m_pixelShader = value; }
        }

        public struct ParameterMapping
        {
            public String name;
            public String value;
            public bool autoUpdate;
        }

        private List<ParameterMapping> m_parameterMap = new List<ParameterMapping>();
        /// <summary>
        /// Tells which parameter should be mapped to what.
        /// </summary>
        public List<ParameterMapping> ParameterMap
        {
            get { return m_parameterMap; }
            set { m_parameterMap = value; }
        }
        #endregion

        public XnaScrapMaterial()
        {
            for (int i = 0; i < m_textureNames.Length; ++i)
            {
                m_textureNames[i] = "";
            }
        }
    }
}
