using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapREST.Services;
using XnaScrapREST.Commands.Queries;
using System.Xml;

namespace XnaScrapREST.Commands.RESTCommands
{
    public class PutCommand : ICommand
    {
        #region member
        private String[] m_data;

        public String[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        
        private NetCtrlService m_service = null;

        #endregion

        public PutCommand(NetCtrlService service)
        {
            m_service = service;
        }

        public String execute(Game game, out byte error)
        {
            NetCtrlService netCtrl = game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrl != null)
            {
                String resourceString = getResource(m_data[0]).TrimStart('/');
                String[] resourcePath = getSubResources(resourceString);
                String res = resourcePath[0];
                if (res == "")
                {
                    error = 0;
                    return getMain();
                }
                else
                {
                    error = 0;
                    return parseResource(resourcePath, 0, game);
                }
            }
            error = 1;
            return "";
        }

        private String getResource(String s)
        {
            String[] parts = s.Split(' ');
            if (parts.Length > 1)
            {
                return parts[1];
            }
            return "";
        }

        private String[] getSubResources(String resource)
        {
            return resource.Split('/');
        }

        private String parseResource(String[] res, int index, Game game)
        {
            String ret = "";
            // is it a REST query
            
            if (m_service.RegisteredRESTGets.Keys.Contains(res[index])) 
            {
                AbstractQuery q = m_service.RegisteredRESTGets[res[index]].query;
                if (res.Length > index + 1)
                {
                    q.Data = new String[res.Length - (index + 1)];
                    for (int i = index + 1, ind = 0; ind < q.Data.Length; ++i, ++ind)
                    {
                        q.Data[ind] = res[i];
                    }
                }
                String r = q.execute(game);
                XmlNodeList values = q.executeRest(game);
                String html = htmlHeader();

                //html += "<p>" + res[index] + "</p>\n";
                //html += List2Html(r, "/" + res[index] + "/", m_service.RegisteredRESTQueries[res[index]].canAccessSub);
                html += List2HtmlRest(values);

                html += htmlFooter();
                ret = getAnswer(html) + html;
            }
            return ret;
        }

        private String parseResource(String[] res, int index, NetCtrlService.RESTGet query, Game game)
        {
            AbstractQuery subQuery = query.query.SubQuery;
            if (subQuery != null)
            {
                if ((index + 1 < res.Length) && res[index + 1] != "")
                {
                    subQuery.Data = new String[] {"", res[index]};
                    XmlNodeList values = subQuery.executeRest(game);
                    if(values != null)
                    {
                        foreach (XmlNode node in values)
                        {
                            if (node.LocalName == res[index + 1])
                            {
                                return parseResource(res, index + 1, query, game);
                            }
                        }
                    }
                }
                else 
                {
                    //return 
                }
            }
            return String.Empty;
        }

        private String getMain()
        {
            String html = "<Html><Head><Title>";
            html += "XnaScrap";
            html += "</Title></Head>";
            // Body
            html += "<Body>";
            html += "Contens";
            foreach (String name in m_service.RegisteredRESTGets.Keys)
            {
                html += "<p><a href=\"/" + name + "\">" + name + "</a></p>";
            }
            //html += "<p><a href=\"/Subsystems\">Subsystems</a></p>";
            //html += "<p><a href=\"/Gameobjects\">Gameobjects</a></p>";
            //html += "<p><a href=\"/Services\">Services</a></p>";
            //html += "<p><a href=\"/Elements\">Elements</a></p>";
            //html += "<p><a href=\"/Elements\">Parameter</a></p>";
            html += "</Body>";
            html += "</Html>";
            

            
            return getAnswer(html) + html;
        }

        private String htmlHeader()
        {
            String html = "<Html><Head><Title>";
            html += "XnaScrap";
            html += "</Title></Head>";
            // Body
            html += "<Body>";

            return html;
        }

        private String htmlFooter()
        {
            String html = "";

            html += "</Body>";
            html += "</Html>";

            return html;
        }

        private String getAnswer(String s)
        {
            return "HTTP/1.1 200 OK \n" +
                            "Server: Apache/1.3.29 (Unix) PHP/4.3.4\n" +
                            "Content-Length: " + s.Length + "\n" +
                            "Content-Language: de \n" +
                            "Content-Type: text/html\n" +
                            "Connection: close \n\n";
        }

        private String List2Html(String list, String resource, bool accessSub)
        {
            String[] elements = list.Split(NetCtrlService.Seperator);
            String html = "";
            foreach (String element in elements)
            {
                if (accessSub)
                {
                    html += "<p><a href=\"" + resource + element + "\">" + element + "</a></p>";
                }
                else
                {
                    html += "<p>" + element + "</p>";
                }
            }

            return html;
        }

        private String List2HtmlRest(XmlNodeList list)
        {
            String html = "";
            foreach (XmlNode node in list)
            {
                html += "<" + node.Name;
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        html += " " + attr.Name + "=\"" + attr.Value + "\"";
                    }
                }
                html += ">";
                if (node.ChildNodes.Count > 0)
                {
                    html += List2HtmlRest(node.ChildNodes);
                }
                else
                {
                    html += node.Value;
                }
                html += "</" + node.Name + ">";
            }
            return html;
        }
    }
}
