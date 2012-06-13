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

        object Data
        {
            set;
        }

        XmlNodeList Get(String resourcePath);
        String Post(String data);
        String Put(String data);
        String Delete(String data);

        bool HasSub();


        IRestNode OnElement(String elementName);
    }
}
