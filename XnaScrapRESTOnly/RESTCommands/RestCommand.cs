using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapREST.Services;
using System.Xml;
using XnaScrapREST.REST;
using XnaScrapREST.RESTCommands;

namespace XnaScrapREST.Commands.RESTCommands
{
    public class RestCommand : ICommand
    {
        #region member

        public enum AnserType
        {
            XML,
            HTML
        };

        private CommandParser.CommandType m_commnadType = CommandParser.CommandType.C_GET;

        public CommandParser.CommandType CommnadType
        {
            get { return m_commnadType; }
            set { m_commnadType = value; }
        }

        private AnserType m_answerType = AnserType.XML;

        public AnserType AnswerType
        {
            get { return m_answerType; }
            set { m_answerType = value; }
        }

        private String[] m_data;

        public String[] Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
        
        private NetCtrlService m_service = null;

        #endregion

        public RestCommand(NetCtrlService service)
        {
            m_service = service;
        }

        public String execute(Game game, out byte error)
        {
            NetCtrlService netCtrl = game.Services.GetService(typeof(NetCtrlService)) as NetCtrlService;
            if (netCtrl != null)
            {
                string[] resources = null;
                // check whether we need a html or xml response
                parseReturnType(m_data[0]);

                String res;
                ParseResources(m_data[0], out resources, out res);
                IRestNode node = m_service.RestResources.getResource(resources);
                //IRestNode node = m_service.RestResources.getResource(resources);
                if (node != null)
                {
                    switch (m_commnadType)
                    {
                        case CommandParser.CommandType.C_GET:
                            {
                                XmlNodeList result = node.Get(res);
                                String html = htmlHeader();
                                if (result != null)
                                {
                                    if (m_answerType == AnserType.HTML)
                                    {
                                        html += List2Html(result, node.HasSub());
                                    }
                                    else
                                    {
                                        html += List2HtmlRest(result);
                                    }
                                }
                                html += htmlFooter();
                                error = 0;
                                return getAnswer(html) + html;
                                break;
                            }

                        case CommandParser.CommandType.C_POST:
                            {
                                error = 0;
                                return node.Post(res);
                                break;
                            }

                        case CommandParser.CommandType.C_PUT:
                            {
                                error = 0;
                                return node.Put(res);
                                break;
                            }

                        case CommandParser.CommandType.C_DELETE:
                            {
                                error = 0;
                                return node.Delete(res);
                                break;
                            }

                        default:
                            {
                                error = 1;
                                return "error";
                                break;
                            }
                    }
                }
            }
            error = 1;
            return "";
        }

        private void parseReturnType(String resource)
        {
            String[] lines = resource.Split('\n');
            foreach (String line in lines)
            {
                if (line.StartsWith("Accept:"))
                {
                    String acceptLine = line.Substring("Accept:".Length).Trim();
                    String[] accepts = acceptLine.Split(',');
                    foreach (String accept in accepts)
                    {
                        if (accept.Trim() == "text/html")
                        {
                            m_answerType = AnserType.HTML;
                            return;
                        }
                        if (accept.Trim() == "text/xml")
                        {
                            m_answerType = AnserType.XML;
                            return;
                        }
                    }
                }
            }
        }

        public static void ParseResources(String resource, out string[] path, out String data)
        {
            String trimmed = resource.Trim();
            int start = 0;

            // remove REST command
            while (trimmed[start++] != ' ') { }
            trimmed = trimmed.Substring(start);

            // remove HTTP 1.1
            int index = trimmed.IndexOf("HTTP/1.1");
            if (index >= 0)
            {
                trimmed = trimmed.Substring(0, index).Trim();
            }

            data = string.Empty;
            // split data and resourcepath
            index = trimmed.IndexOf(' ');
            if (index > 0)
            {
                // get data
                data = trimmed.Substring(index).Trim();
                // remove data from path
                trimmed = trimmed.Substring(0,index).Trim();
            }
            trimmed = trimmed.Trim(new char[] {' ','/'});
            path = trimmed.Split('/');

            // OLD
            //// get resourcepath
            //int last = 1;
            //start = 1;
            //while (trimmed[start] != ' ' && start < trimmed.Length)
            //{
            //    if (trimmed[start] == '/')
            //    {
            //        path.Add(trimmed.Substring(last,start-last));
            //        last = start;
            //    }
            //    ++start;
            //}

            //// get data
            //data = trimmed.Substring(last).Trim();                      
        }

        private String htmlHeader()
        {
            if (m_answerType == AnserType.HTML)
            {
                StringBuilder html = new StringBuilder();
                html.Append("<Html><Head><Title>");
                html.Append("XnaScrap");
                html.Append("</Title></Head>");
                // Body
                html.Append("<Body>");

                return html.ToString();
            }
            else if (m_answerType == AnserType.XML)
            {
                String html = "<Root>";
                return html;
            }
            else
            {
                return "";
            }
        }

        private String htmlFooter()
        {
            if (m_answerType == AnserType.HTML)
            {
                String html = "";

                html += "</Body>";
                html += "</Html>";

                return html;
            }
            else if (m_answerType == AnserType.XML)
            {
                String html = "</Root>";
                return html;
            }
            else
            {
                return "";
            }
        }

        private String getAnswer(String s)
        {
            String type = (m_answerType == AnserType.HTML) ? "Content-Type: text/html\n" : "Content-Type: text/xml\n";

            return "HTTP/1.1 200 OK \n" +
                            "Server: Apache/1.3.29 (Unix) PHP/4.3.4\n" +
                            "Content-Length: " + s.Length + "\n" +
                            "Content-Language: de \n" +
                            type + 
                            //"Content-Type: text/html\n" +
                            //"Content-Type: text/xml\n" +
                            "Connection: close \n\n";
        }

        //private String List2Html(String list, String resource, bool accessSub)
        //{
        //    String[] elements = list.Split(NetCtrlService.Seperator);
        //    String html = "";
        //    foreach (String element in elements)
        //    {
        //        if (accessSub)
        //        {
        //            html += "<p><a href=\"" + resource + element + "\">" + element + "</a></p>";
        //        }
        //        else
        //        {
        //            html += "<p>" + element + "</p>";
        //        }
        //    }

        //    return html;
        //}

        private String List2Html(XmlNodeList list, bool hasSub)
        {
            if (list.Count <= 0)
                return "";
            StringBuilder html = new StringBuilder();
            foreach (XmlNode node in list)
            {
                if (node.Attributes != null && node.Attributes.Count > 0)
                {
                    string name = string.Empty;
                    string value = string.Empty;
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == "Name")
                        {
                            name = attr.Value;
                        }
                        else if (attr.Name == "Value")
                        {
                            value = attr.Value;
                        }
                    }
                    // write Name
                    html.Append("<p>");
                    if (hasSub)
                    {
                        html.Append("<a href=\"");
                        html.Append(name);
                        html.Append("/\">");
                    }
                    html.Append(name);
                    if (hasSub)
                    {
                        html.Append("</a>");
                    }

                    //write value
                    if (value != string.Empty)
                    {
                        html.Append(" ");
                        html.Append(value);
                    }

                    html.Append("</p>");

                    html.Append(List2Html(node.ChildNodes, hasSub));
                }
                else
                {
                    html.Append("<p>");
                    if (hasSub)
                    {
                        html.Append("<a href=\"");
                        html.Append(node.Name);
                        html.Append("/\">");
                    }
                    html.Append(node.Name);
                    if (hasSub)
                    {
                        html.Append("</a>");
                    }
                    html.Append("</p>");
                }
            }

            return html.ToString();
        }

        private String List2HtmlRest(XmlNodeList list)
        {
            StringBuilder html = new StringBuilder();
            foreach (XmlNode node in list)
            {
                html.Append("<");
                String nodeName;
                if (node.Name.StartsWith("#"))
                    nodeName = node.Name.Substring(1);
                else
                    nodeName = node.Name;
                html.Append(nodeName);
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        html.Append(" "); html.Append(attr.Name); html.Append("=\""); html.Append(attr.Value); html.Append("\"");
                    }
                }
                html.Append(">");
                if (node.ChildNodes.Count > 0)
                {
                    html.Append(List2HtmlRest(node.ChildNodes));
                }
                else
                {
                    html.Append(node.Value);
                }
                html.Append("</"); html.Append(nodeName); html.Append(">");
            }
            return html.ToString();
        }
    }
}
