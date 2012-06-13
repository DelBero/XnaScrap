using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlScripting.Service.Interfaces;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core;
using System.IO;
using XnaScrapCore.Core.Systems.Macro;
using XnaScrapCore.Core.Interfaces.Other;

namespace XmlScripting.Service.BuiltIn
{
    public enum OpCodes
    {
        GameObjectOpCode,
        ElementOpCode,
        ParameterOpCode,
        ValueOpCode,
        MainOpCode,
        CallMacroOpCode,
        RegisterMacroOpCode
    }

    /// <summary>
    /// Create a GameObject.
    /// </summary>
    class BeginGameObject : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.GameObjectOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private IObjectBuilder m_objectBuilder;
        #region IFunction

        public int ParameterCount
        {
            get { return 1; } // the objects id
        }
        #endregion
        #endregion

        public BeginGameObject()
        {
        }

        public BeginGameObject(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_objectBuilder != null)
            {
                XAttribute name = parameter.Attribute(XName.Get("Id"));
                XnaScrapId id;
                if (name == null)
                {
                    id = XnaScrapId.CreateId();
                }
                else
                {
                    id = new XnaScrapId(name.Value);
                }
                m_objectBuilder.beginGameObject(id);
                foreach (XElement child in parameter.Elements())
                {
                    executor.execute(child);
                }
                m_objectBuilder.endGameObject();
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_objectBuilder != null)
            {
                int addr = data.popInt();
                XnaScrapId id = new XnaScrapId(heap.readString(addr));//(data.popString());
                m_objectBuilder.beginGameObject(id);
            }
        }

        public void end(Stack data, Heap heap)
        {
            m_objectBuilder.endGameObject();
        }

        public void compile(XElement parameter, Stack data)
        {
            XAttribute name = parameter.Attribute(XName.Get("Id"));
            data.push(name.Value);
        }
    }

    /// <summary>
    /// Creates a Element
    /// </summary>
    class BeginElement : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.ElementOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private IObjectBuilder m_objectBuilder;
        #region IFunction

        public int ParameterCount
        {
            get { return 1; } // the elements id
        }
        #endregion
        #endregion

        public BeginElement()
        {

        }

        public BeginElement(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_objectBuilder != null)
            {
                XAttribute name = parameter.Attribute(XName.Get("Type"));
                m_objectBuilder.beginElement(name.Value);
                foreach (XElement child in parameter.Elements())
                {
                    executor.execute(child);
                }
                m_objectBuilder.endElement();
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_objectBuilder != null)
            {
                int addr = data.popInt();
                String s = heap.readString(addr);//data.popString();
                m_objectBuilder.beginElement(s);
            }
        }

        public void end(Stack data, Heap heap)
        {
            m_objectBuilder.endElement();
        }

        public void compile(XElement parameter, Stack data)
        {
            XAttribute name = parameter.Attribute(XName.Get("Type"));
            data.push(name.Value);
        }
    }

    /// <summary>
    /// Sets parameters.
    /// </summary>
    class BeginParameter : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.ParameterOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private IObjectBuilder m_objectBuilder;
        #region IFunction

        public int ParameterCount
        {
            get { return 1; } // the parameter name
        }
        #endregion
        #endregion

        public BeginParameter()
        {

        }
        public BeginParameter(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_objectBuilder != null)
            {
                XAttribute name = parameter.Attribute(XName.Get("Name"));
                m_objectBuilder.beginParameter(name.Value);
                foreach (XElement child in parameter.Elements())
                {
                    executor.execute(child);
                }
                m_objectBuilder.endParameter();
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_objectBuilder != null)
            {
                int addr = data.popInt();
                String s = heap.readString(addr);//data.popString();
                m_objectBuilder.beginParameter(s);
            }
        }

        public void end(Stack data, Heap heap)
        {
            m_objectBuilder.endParameter();
        }

        public void compile(XElement parameter, Stack data)
        {
            XAttribute name = parameter.Attribute(XName.Get("Name"));
            data.push(name.Value);
        }
    }

    /// <summary>
    /// Sets parameters.
    /// </summary>
    class Value : INativeFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.ValueOpCode;
        public const int m_OpCodeClose = -(int)OpCodes.ValueOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private IObjectBuilder m_objectBuilder;
        #region IFunction
        
        public int ParameterCount
        {
            get { return 0; }
        }
        #endregion
        #endregion

        public Value()
        {

        }
        public Value(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_objectBuilder != null)
            {
                String values = parameter.Value;
                m_objectBuilder.setValues(values); 
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_objectBuilder != null)
            {
                int addr = data.popInt();
                m_objectBuilder.setValues(heap.readString(addr));
            }
        }

        public void end(Stack data, Heap heap)
        {
            
        }

        public void compile(XElement parameter, Stack data)
        {
            data.push(parameter.Value);
        }
    }


    /// <summary>
    /// Automatically create an ID
    /// </summary>
    class CreateId : INativeFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.ValueOpCode;
        public const int m_OpCodeClose = -(int)OpCodes.ValueOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private IObjectBuilder m_objectBuilder;
        #region IFunction
        
        public int ParameterCount
        {
            get { return 0; }
        }
        #endregion
        #endregion

        public CreateId()
        {

        }
        public CreateId(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_objectBuilder != null)
            {
                String values = parameter.Value;
                m_objectBuilder.setValues(values); 
            }
        }

        public void execute(Stack data, Heap heap)
        {
            String id = XnaScrapId.CreateId().ToString();
            int addr = heap.allocString(id);
            data.push(addr);
        }

        public void end(Stack data, Heap heap)
        {
            
        }

        public void compile(XElement parameter, Stack data)
        {
            data.push(parameter.Value);
        }
    }

    /// <summary>
    /// Entrypoint.
    /// </summary>
    class Main : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.MainOpCode;
        public const int m_OpCodeClose = -(int)OpCodes.MainOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        #region IFunction

        public int ParameterCount
        {
            get { return -1; } // arbitrary number
        }
        #endregion
        #endregion

        public Main()
        {

        }
        public Main(Game game)
        {
            m_game = game;
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            foreach (XElement child in parameter.Elements())
            {
                executor.execute(child);
            }
        }

        public void execute(Stack data, Heap heap)
        {
            //throw new Exception("Cannot call Main::execute in a compiled script");
        }

        public void end(Stack data, Heap heap)
        {

        }

        public void compile(XElement parameter, Stack data)
        {
            //data.push(parameter.Value);
        }
    }

    /// <summary>
    /// Calls a registered macro
    /// </summary>
    class CallMacro : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.CallMacroOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private MacroService m_macroService;
        #region IFunction

        public int ParameterCount
        {
            get { return -1; } // the objects id
        }
        #endregion
        #endregion

        public CallMacro()
        {

        }
        public CallMacro(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_macroService = game.Services.GetService(typeof(MacroService)) as MacroService;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_macroService != null)
            {
                XAttribute name = parameter.Attribute(XName.Get("Id"));
                XnaScrapId id;
                if (name == null)
                {
                    id = XnaScrapId.CreateId();
                }
                else
                {
                    id = new XnaScrapId(name.Value);
                }
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_macroService != null)
            {
                int addr = data.popInt();
                XnaScrapId id = new XnaScrapId(heap.readString(addr));//(data.popString());
                if (m_macroService.RegisteredMacros.Keys.Contains(id))
                {
                    IMacro macro = m_macroService.RegisteredMacros[id];
                    // get number of arguments
                    int numParameter = data.popInt();
                    for (int i = 0; i < numParameter; ++i)
                    {
                        addr = data.popInt();
                        String name = heap.readString(addr);//data.popString();
                        addr = data.popInt();
                        String value = heap.readString(addr);//data.popString();
                        macro.setParameter(name,value);
                    }                    
                    macro.execute();
                }
            }
        }

        public void end(Stack data, Heap heap)
        {
        }

        public void compile(XElement parameter, Stack data)
        {
            XAttribute name = parameter.Attribute(XName.Get("Id"));
            data.push(name.Value);
        }
    }

    /// <summary>
    /// Registeres a script as macro
    /// </summary>
    class RegisterMacro : IBlockFunction
    {
        #region member
        public const int m_OpCode = (int)OpCodes.RegisterMacroOpCode;

        public int OpCode
        {
            get { return OpCode; }
        }

        private Game m_game;
        private MacroService m_macroService;
        private IResourceService m_resourceService;
        #region IFunction

        public int ParameterCount
        {
            get { return 2; } // the macro id and the script name
        }
        #endregion
        #endregion

        public RegisterMacro()
        {

        }
        public RegisterMacro(Game game)
        {
            m_game = game;
            if (game != null)
            {
                m_macroService = game.Services.GetService(typeof(MacroService)) as MacroService;
                m_resourceService = game.Services.GetService(typeof(IResourceService)) as IResourceService;
            }
        }

        public void execute(XElement parameter, XmlScriptExecutor executor)
        {
            if (m_macroService != null)
            {
            }
        }

        public void execute(Stack data, Heap heap)
        {
            if (m_macroService != null && m_resourceService != null)
            {
                int addr = data.popInt();
                XnaScrapId macro_id = new XnaScrapId(heap.readString(addr));//(data.popString());
                addr = data.popInt();
                XnaScrapId scriptName = new XnaScrapId(heap.readString(addr));//(data.popString());
                if (!m_macroService.RegisteredMacros.Keys.Contains(macro_id))
                {
                    if (m_resourceService.Scripts.Keys.Contains(scriptName.ToString()))
                    {
                        IScript script = m_resourceService.Scripts[scriptName.ToString()];
                        ScriptedMacro newMacro = new ScriptedMacro(null);
                        newMacro.Script = script;
                        newMacro.Name = macro_id.ToString();
                        m_macroService.RegisteredMacros.Add(scriptName, newMacro);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Error Macro with Id \"" + macro_id + "\" already registered.");
                }
            }
        }

        public void end(Stack data, Heap heap)
        {
        }

        public void compile(XElement parameter, Stack data)
        {
            XAttribute name = parameter.Attribute(XName.Get("Id"));
            data.push(name.Value);
        }
    }

    ///// <summary>
    ///// Defines a function.
    ///// </summary>
    //class DefineFunction : IFunction
    //{
    //    #region member
    //    private Game m_game;

    //    private XmlScriptExecutor m_executor;
    //    #region IFunction
    //    public bool CanHandleChildren
    //    {
    //        get { return true; }
    //    }

    //    public int ParameterCount
    //    {
    //        get { return 1; } // the objects id
    //    }
    //    #endregion
    //    #endregion
    //    public DefineFunction(Game game, XmlScriptExecutor executor)
    //    {
    //        m_game = game;
    //        m_executor = executor;
    //    }

    //    public void execute(XElement parameter)
    //    {
    //        foreach (XElement child in parameter.Elements())
    //        {
    //            m_executor.execute(child);
    //        }
    //    }

    //    public void execute(BinaryReader parameter)
    //    {
    //        throw new Exception("Cannot call Main::execute in a compiled script");
    //    }
    //}

    ///// <summary>
    ///// Calls a function.
    ///// </summary>
    //class CallFunction : IFunction
    //{
    //    #region member
    //    private Game m_game;
    //    private IObjectBuilder m_objectBuilder;
    //    private XmlScriptExecutor m_executor;
    //    #region IFunction
    //    public bool CanHandleChildren
    //    {
    //        get { return true; }
    //    }

    //    public int ParameterCount
    //    {
    //        get { return 1; } // the objects id
    //    }
    //    #endregion
    //    #endregion
    //    public CallFunction(Game game, XmlScriptExecutor executor)
    //    {
    //        m_game = game;
    //        m_executor = executor;
    //        m_objectBuilder = game.Services.GetService(typeof(IObjectBuilder)) as IObjectBuilder;
    //    }

    //    public void execute(XElement parameter)
    //    {
    //        foreach (XElement child in parameter.Elements())
    //        {
    //            m_executor.execute(child);
    //        }
    //    }

    //    public void execute(BinaryReader parameter)
    //    {
    //        throw new Exception("Cannot call Main::execute in a compiled script");
    //    }
    //}
}
