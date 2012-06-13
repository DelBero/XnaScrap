using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XnaScrapREST.REST
{
    public interface IRestNode
    {
        String Name
        {
            get;
        }

        XmlNodeList Get(String resourcePath);
        String Post(String data);
        String Put(String data);
        String Delete();


        IRestNode OnElement(String elementName);
    }
}
