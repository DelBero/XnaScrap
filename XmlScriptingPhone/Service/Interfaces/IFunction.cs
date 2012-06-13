using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using XnaScrapCore.Core;

namespace XmlScripting.Service.Interfaces
{
    public interface IFunction
    {
        /// <summary>
        /// The number of parameters this function takes.
        /// </summary>
        int ParameterCount
        {
            get;
        }

        int OpCode
        {
            get;
        }

        /// <summary>
        /// Executes the function from a uncompiled xml script.
        /// </summary>
        /// <param name="parameter">The node containing the function.</param>
        void execute(XElement parameter, XmlScriptExecutor executor);

        /// <summary>
        /// Executes a compiled script.
        /// </summary>
        /// <param name="data"></param>
        void execute(Stack data, Heap heap);

        /// <summary>
        /// Finishes the function. This is for Block functions.
        /// </summary>
        void end(Stack data, Heap heap);

        /// <summary>
        /// Compiles the nodes data into the data stack. Does NOT care about child nodes.
        /// </summary>
        /// <param name="parameter">The node containing data.</param>
        /// <param name="data">The data stack to write to</param>
        void compile(XElement parameter, Stack data);
    }

    public interface IBlockFunction : IFunction
    {

    }

    public interface INativeFunction : IFunction
    {

    }
}
