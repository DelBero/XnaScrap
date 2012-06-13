using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using System.Diagnostics;

namespace XnaScrapCore.Core.Systems.Resource
{
    public class MaterialCompiler
    {
        public Material Compile(MaterialUncompiled input)
        {
            Material ret = new Material();
            process(input.MaterialString,ret);
            return ret;
        }

        #region new
        private void process(String xml, Material m)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode n = doc.FirstChild;
            if (n.Name != "Material")
            {
                return;
            }
            foreach (XmlNode node in n.ChildNodes)
            {
                if (node.Name == "Textures")
                {
                    parseTextures(node, m);
                }
                if (node.Name == "Effect")
                {
                    parseEffect(node, m);
                }
                if (node.Name == "ParameterMap")
                {
                    parseParameterMap(node, m);
                }
            }
        }

        private void parseTextures(XmlNode node, Material m)
        {
            m.TextureNames[0] = m.TextureNames[1] = m.TextureNames[2] = m.TextureNames[3] = "";
            foreach (XmlNode textureNode in node.ChildNodes)
            {
                if (textureNode.Name == "Texture")
                {
                    int textureUnit = 0;
                    String texName = "";
                    foreach (XmlAttribute attrib in textureNode.Attributes)
                    {
                        if (attrib.Name == "TextureUnit")
                        {
                            textureUnit = Int32.Parse(attrib.Value);
                        }
                        if (attrib.Name == "Texture")
                        {
                            texName = attrib.Value;
                        }
                    }
                    if (textureUnit >= 0 && textureUnit < 4)
                    {
                        m.TextureNames[textureUnit] = texName;
                    }
                }
            }
        }

        private void parseEffect(XmlNode node, Material m)
        {
            foreach (XmlNode effectNode in node.ChildNodes)
            {
                if (effectNode.Name == "EffectName")
                {
                    m.EffectName = effectNode.InnerText;
                }
                else if (effectNode.Name == "Technique")
                {
                    m.Technique = effectNode.InnerText;
                }
            }
        }

        private void parseParameterMap(XmlNode node, Material m)
        {
            foreach (XmlNode parameter in node.ChildNodes)
            {
                if (parameter.Name == "Parameter")
                {
                    Material.ParameterMapping mapping = new Material.ParameterMapping();
                    foreach (XmlAttribute attrib in parameter.Attributes)
                    {
                        if (attrib.Name == "Name")
                        {
                            mapping.name = attrib.Value;
                        }
                        else if (attrib.Name == "Value")
                        {
                            mapping.value = attrib.Value;
                        }
                        else if (attrib.Name == "perInstance")
                        {
                            mapping.perInstance = bool.Parse(attrib.Value);
                        }
                        else if (attrib.Name == "Semantic")
                        {
                            Enum.TryParse(attrib.Value, out mapping.semantic);
                        }
                    }
                    if (mapping.name.Length > 0)
                    {
                        m.ParameterMappings.Add(mapping);
                    }
                }
            }
        }
        #endregion
    }

    public class ParameterMappingCompiler
    {
        public Material.ParameterMapping Compile(ParameterMappingUncompiled input)
        {
            Material.ParameterMapping mapping = new Material.ParameterMapping();
            process(input.MappingString, mapping);
            return mapping;
        }

        public void process(String xml,Material.ParameterMapping mapping)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode n = doc.FirstChild;
            if (n.Name != "ParameterMapping")
            {
                return;
            }
            foreach (XmlNode node in n.ChildNodes)
            {
                if (node.Name == "Mapping")
                {
                    foreach (XmlAttribute attrib in node.Attributes)
                    {

                    }
                }
            }
        }
    }
}
