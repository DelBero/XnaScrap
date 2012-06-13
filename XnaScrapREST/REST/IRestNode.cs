using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace XnaScrapREST.REST
{
    public interface IRestNode
    {
        XmlNodeList Get();
        String Post(String data);
        String Put(String data);
        String Delete();


        IRestNode OnElement(String elementName);
    }
}
