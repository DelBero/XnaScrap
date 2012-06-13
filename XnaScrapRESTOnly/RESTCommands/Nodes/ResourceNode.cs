using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace XnaScrapREST.REST.Nodes
{
    public class ResourceNode : IRestNode
    {
        public enum ResourceNodeType
        {
            MESH,
            TEXTURE,
            MATERIAL,
            SHADER,
            SCRIPT,
            MACRO,
            FONT
        }
        #region member
        private Game m_game;
        private NodeList m_subNodes;
        private ResourceNodeType m_type;
        private String m_name;

        public String Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public object Data
        {
            set { }
        }
        #endregion
        public ResourceNode(Game game, NodeList subNode, String name, ResourceNodeType type = ResourceNodeType.MESH)
        {
            m_game = game;
            m_subNodes = subNode;
            m_name = name;
            m_type = type;
        }

        #region interface
        public bool HasSub() { return (m_subNodes != null) && (m_subNodes.Count() > 0); }

        public XmlNodeList Get(String resourcePath)
        {
            
            IResourceService resourceService = m_game.Services.GetService(typeof(IResourceService)) as IResourceService;
            if (resourceService != null)
            {
                // get the resource
                if (resourcePath.EndsWith("/"))
                {
                    return getResources(resourceService);
                }
                else
                {
                    String res = resourcePath.TrimEnd('/');
                    int index = res.LastIndexOf('/');
                    if (index > 0)
                    {
                        res = res.Substring(index+1);
                    }
                    return getResource(res, resourceService);
                }

            }
            else
            {
                return null;
            }
        }

        public String Post(String data)
        {
            return "ok";
        }

        public String Put(String data)
        {
            return "ok";
        }

        public String Delete(String data)
        {
            return "ok";
        }

        public IRestNode OnElement(String elementName)
        {
            return null;
        }
        #endregion


        private XmlNodeList getResources(IResourceService resourceService)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement resources = doc.CreateElement("Resources");
            switch (m_type)
            {
                case ResourceNodeType.MESH:
                    {
                        // Meshes
                        XmlElement meshes = doc.CreateElement("Meshes");
                        foreach (String meshName in resourceService.Models.Keys)
                        {
                            XmlElement mesh = doc.CreateElement("Mesh");
                            mesh.SetAttribute("Name", meshName);
                            meshes.AppendChild(mesh);
                        }
                        resources = meshes;
                        break;
                    }
                case ResourceNodeType.TEXTURE:
                    {
                        // Textures
                        XmlElement textures = doc.CreateElement("Textures");
                        foreach (String textureName in resourceService.Textures.Keys)
                        {
                            XmlElement texture = doc.CreateElement("Texture");
                            texture.SetAttribute("Name", textureName);
                            textures.AppendChild(texture);
                        }
                        resources = textures;
                        break;
                    }
                case ResourceNodeType.SCRIPT:
                    {
                        // Scripts
                        XmlElement scripts = doc.CreateElement("Scripts");
                        foreach (String scriptName in resourceService.Scripts.Keys)
                        {
                            XmlElement script = doc.CreateElement("Script");
                            script.SetAttribute("Name", scriptName);
                            scripts.AppendChild(script);
                        }
                        resources = scripts;
                        break;
                    }
                case ResourceNodeType.MATERIAL:
                    {
                        // Materials
                        XmlElement materials = doc.CreateElement("Materials");
                        foreach (String materialName in resourceService.Materials.Keys)
                        {
                            XmlElement material = doc.CreateElement("Material");
                            material.SetAttribute("Name", materialName);
                            materials.AppendChild(material);
                        }
                        resources = materials;
                        break;
                    }
                case ResourceNodeType.FONT:
                    {
                        // Fonts
                        XmlElement fonts = doc.CreateElement("Fonts");
                        foreach (String fontName in resourceService.Fonts.Keys)
                        {
                            XmlElement font = doc.CreateElement("Font");
                            font.SetAttribute("Name", fontName);
                            fonts.AppendChild(font);
                        }
                        resources = fonts;
                        break;
                    }

                //// Macros
                //XmlElement macros = doc.CreateElement("Macros");
                //foreach (String macroName in resourceService.Macros.Keys)
                //{
                //    XmlElement macro = doc.CreateElement("Macro");
                //    macro.SetAttribute("Name", macroName);
                //    macros.AppendChild(macro);
                //}



                //resources.AppendChild(meshes);
                //resources.AppendChild(textures);
                //resources.AppendChild(scripts);
                //resources.AppendChild(materials);
                //resources.AppendChild(fonts);
            }
            return resources.ChildNodes;
        }

        private XmlNodeList getResource(String elementName, IResourceService resourceService)
        {
            switch (m_type)
            {
                case ResourceNodeType.MESH:
                    {
                        ModelMesh modelMesh = null;
                        if (resourceService.Models.TryGetValue(elementName, out modelMesh))
                        {
                            XmlDocument doc = new XmlDocument();
                            XmlElement meshes = doc.CreateElement("Meshes");
                            XmlElement mesh = doc.CreateElement("Mesh");
                            meshes.AppendChild(mesh);
                            mesh.SetAttribute("Name", modelMesh.Name);
                            foreach (ModelMeshPart m in modelMesh.MeshParts)
                            {
                                XmlElement meshPart = doc.CreateElement("MeshPart");
                                meshPart.SetAttribute("Name",modelMesh.Name);
                                mesh.AppendChild(meshPart);

                                XmlElement vertices = doc.CreateElement("Vertices");
                                XmlElement normals = doc.CreateElement("Normals");
                                XmlElement texcoords = doc.CreateElement("TexCoords");
                                XmlElement indices = doc.CreateElement("Indices");


                                meshPart.AppendChild(vertices);
                                meshPart.AppendChild(normals);
                                meshPart.AppendChild(texcoords);
                                meshPart.AppendChild(indices);
                                // get vertex data
                                float[] vertexdata = new float[m.NumVertices*8]; // 3 per vertex 3 per normal 2 per texturecoord
                                int stride = m.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
                                m.VertexBuffer.GetData<float>(vertexdata);
                                for (int i = 0; i < m.NumVertices*8;)
                                {
                                    CultureInfo culture = new CultureInfo("en-US");
                                    // vertex
                                    XmlElement vertex = doc.CreateElement("Vertex");
                                    vertex.SetAttribute("X", vertexdata[i].ToString(culture)); vertex.SetAttribute("Y", vertexdata[i + 2].ToString(culture)); vertex.SetAttribute("Z", vertexdata[i + 1].ToString(culture));
                                    System.Console.WriteLine(vertexdata[i].ToString(culture) + " " + vertexdata[i + 2].ToString(culture) + " " + vertexdata[i + 1].ToString(culture));
                                    //normal
                                    XmlElement normal = doc.CreateElement("Normal");
                                    normal.SetAttribute("X", vertexdata[i + 3].ToString(culture)); normal.SetAttribute("Y", vertexdata[i + 5].ToString(culture)); normal.SetAttribute("Z", vertexdata[i + 4].ToString(culture));
                                    //texcoord
                                    XmlElement texcoord = doc.CreateElement("TexCoord");
                                    texcoord.SetAttribute("X", vertexdata[i + 6].ToString(culture)); texcoord.SetAttribute("Y", vertexdata[i + 7].ToString(culture));

                                    vertices.AppendChild(vertex);
                                    normals.AppendChild(normal);
                                    texcoords.AppendChild(texcoord);
                                    i += stride;
                                }

                                // get index data
                                short[] indexdata = new short[m.IndexBuffer.IndexCount];
                                //IndexElementSize size = m.IndexBuffer.IndexElementSize;
                                m.IndexBuffer.GetData<short>(indexdata);
                                for (int i = 0; i < m.IndexBuffer.IndexCount; ++i)
                                {
                                    XmlElement index = doc.CreateElement("Index");
                                    index.SetAttribute("Value", indexdata[i].ToString());

                                    indices.AppendChild(index);
                                }
                            }
                            return meshes.ChildNodes;
                        }
                        return null;
                    }
                case ResourceNodeType.TEXTURE:
                    {
                        return null;
                    }
                case ResourceNodeType.SCRIPT:
                    {
                        return null;
                    }
                case ResourceNodeType.MATERIAL:
                    {
                        return null;
                    }
                case ResourceNodeType.FONT:
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }
    }
}
