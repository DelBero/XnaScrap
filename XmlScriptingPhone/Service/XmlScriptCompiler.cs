using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using XnaScrapCore.Core.Scipting;
using XmlScripting.Service.Interfaces;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core;
using XmlScripting.Service.BuiltIn;
using XnaScrapCore.Core.Interfaces.Other;
using XnaScrapCore.Core.Interfaces.Services;
using XnaScrapCore.Core.Parameter;

namespace XmlScripting.Service
{

    public class XmlScriptCompilerService : Microsoft.Xna.Framework.GameComponent, IScriptCompiler
    {
        #region member
        XmlScriptCompiler m_compiler = new XmlScriptCompiler();
        #endregion

        #region CDtors
        public XmlScriptCompilerService(Game game)
            : base(game)
        {
            game.Components.Add(this);
            game.Services.AddService(typeof(IScriptCompiler), this);
        }
        #endregion

        private IScript Compile(XmlScriptUncompiled input)
        {
            return m_compiler.Compile(input);
        }

        public IScript Compile(String scriptCode, String name)
        {
            return m_compiler.Compile(scriptCode, name);
        }
    }

    public class XmlScriptCompiler : IScriptCompiler
    {

        #region DataTable
        private class DataTable
        {
            public DataTable Parent = null;
            public Dictionary<String, int> Data = new Dictionary<String, int>();
            public int m_ebpOffset = 0;

            public void add(String name, int index)
            {
                Data.Add(name,index);
            }

            public void incEbp()
            {
                m_ebpOffset += sizeof(int);
            }

            public void allocInt(String name)
            {
                add(name,m_ebpOffset);
                incEbp();
            }

            public bool Contains(String var)
            {
                if (Data.Keys.Contains(var))
                {
                    return true;
                }
                else if (Parent != null)
                {
                    return Parent.Contains(var);
                }
                else
                {
                    return false;
                }
            }

            public int Get(String index)
            {
                if (Data.Keys.Contains(index))
                {
                    return Data[index];
                }
                else if (Parent != null)
                {
                    return Parent.Get(index);
                }
                else
                {
                    return -1;
                }
            }

        }
        #endregion

        #region member
        private DataTable m_dataTable = new DataTable();
        private DataTable m_functionTable = new DataTable();
        private MemoryStream m_currentProgramCode;
        private int m_entryPoint = 0;
        private ParameterSequence m_parameterSequence;
        #endregion

        public XmlScriptCompiler()
        {

        }

        public IScript Compile(XmlScriptUncompiled input)
        {
            String scriptCode = input.Script;
            return Compile(scriptCode, input.Name);
        }

        public IScript Compile(String scriptCode, String name)
        {
            Stack stack = new Stack();
            //System.Diagnostics.Debugger.Launch();
            XDocument script = XDocument.Parse(scriptCode.Trim());

            m_entryPoint = -1;

            m_currentProgramCode = new MemoryStream();
            m_parameterSequence = new ParameterSequence();
            XElement root = script.Element("Root");
            if (root == null)
            {
                root = script.Element("root");
                if (root == null)
                {
                    throw new Exception("Error. Script must contain a root node.");
                }
            }
            foreach (XElement node in root.Elements())
            {
                bool isMain = false;
                if (node.Name.LocalName == "Main" || node.Name.LocalName == "main")
                {
                    // scripts starts here
                    m_entryPoint = (int)m_currentProgramCode.Position;
                    // push all attributes to the parameter sequencer (so the script can run with default parameters)
                    ParameterSequenceBuilder sequenceBuilder = new ParameterSequenceBuilder();
                    sequenceBuilder.createSequence();
                    foreach (XAttribute attr in node.Attributes())
                    {
                        sequenceBuilder.addParameter(new StringParameter(attr.Name.ToString(), attr.Value));
                    }
                    m_parameterSequence = sequenceBuilder.CurrentSequence;
                    stack = new Stack(m_parameterSequence.getMemStream());
                    isMain = true;
                }
                // add node as a function
                else
                {
                    //m_dataTable.Data.Add(node.Name.LocalName, (int)m_currentProgramCode.Position);
                    m_functionTable.add(node.Name.LocalName, (int)m_currentProgramCode.Position);
                    isMain = false;
                }
                compileRoutine(node, isMain,stack);
            }

            // end of program
            m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.HALT);

            XmlScriptCompiled compiledScript = new XmlScriptCompiled(scriptCode);
            compiledScript.Name = name;
            compiledScript.ProgramCode = m_currentProgramCode;
            compiledScript.EntryPoint = m_entryPoint;
            compiledScript.ParameterSequence = m_parameterSequence;

            if (m_entryPoint == -1)
            {
                throw new Exception("Error no entrypoint for script: " + name);
            }
            return compiledScript;
        }

        /// <summary>
        /// Compile a routine
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="bIsMain">The main function doesn't need a RETURN commands</param>
        private void compileRoutine(XElement routine, bool bIsMain, Stack stack)
        {
            DataTable localDataTable = new DataTable();

            // push all parameters(attributes)
            int index = 2 + routine.Attributes().Count(); // 0 points to next free,-1 is eip, -2 is ebp, -3 is first param
            foreach (XAttribute attr in routine.Attributes())
            {
                //if (bIsMain)
                //{
                    //m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.ALLOC_STRING_ON_HEAP);
                    //stack.push();
                //}
                localDataTable.add(attr.Name.LocalName, -1 * index * sizeof(int));
                --index;
            }
            foreach (XElement instruction in routine.Elements())
            {
                // push parameters
                foreach (XAttribute attr in instruction.Attributes())
                {
                    pushParameter(localDataTable, attr.Value);
                }
                compile(instruction, localDataTable, instruction.Attributes().Count());
            }
            if (!bIsMain)
            {
                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.RETURN);
            }
        }


        /// <summary>
        /// Compile child elements of a routine
        /// </summary>
        /// <param name="node"></param>
        /// <param name="inheritedDataTable"></param>
        private void compile(XElement node, DataTable inheritedDataTable, int numParameter)
        {
            // local variables
            DataTable localDataTable = new DataTable();
            localDataTable.Parent = inheritedDataTable;

            String name = node.Name.LocalName;

            // Its a native Function
            if (ScriptFunctionRegistry.GetInstance().RegisteredFunctions.Contains(name))
            {
                int f = ScriptFunctionRegistry.GetInstance().RegisteredFunctions.IndexOf(name);

                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.NATIVE_CALL);
                m_currentProgramCode.Writer.Write(f);
                // push number of attributes
                if (numParameter != ScriptFunctionRegistry.GetInstance().RegisteredFunctionAddresses[f].ParameterCount)
                {
                    //throw new Exception("Error number of parameters doesn't match!");
                }

                // compile child
                foreach (XElement child in node.Elements())
                {
                    // push parameters
                    foreach (XAttribute attr in child.Attributes())
                    {
                        pushParameter(localDataTable,attr.Value);
                    }
                    // push value if this is a leaf
                    if (!child.HasElements)
                    {
                        if (child.Value.Length > 0)
                        {
                            pushParameter(localDataTable, child.Value);
                        }
                    }
                    compile(child, localDataTable, child.Attributes().Count());
                }

                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.NATIVE_CALL);
                m_currentProgramCode.Writer.Write(-1 * f);
            }
            // is this a function call?
            else if (m_functionTable.Contains(name))
            {
                doFunctionCall(node, inheritedDataTable, name);
            }
            else
            {
                throw new Exception("Unknown function " + name);
            }
        }

        /// <summary>
        /// Pushes all the parameters for a function call. The order of parameters is strict and there are no default values
        /// </summary>
        /// <param name="dataTable">Collection of known variables</param>
        /// <param name="param">Value of the parameter</param>
        private void pushParameter(DataTable dataTable, String param)
        {
            if (dataTable.Contains(param))
            {
                int parameterAddr = dataTable.Get(param);
                // push parameters value (i.e. an address to a string on the heap)
                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.PUSH_ADDR);
                m_currentProgramCode.Writer.Write(parameterAddr);
                //m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.READ_STRING_FROM_HEAP);
            }
            else
            {
                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.PUSH_STRING);
                m_currentProgramCode.Writer.Write(param);
                // pop the string from the stack, alloc space for it and push its address onto the stack 
                // (could use a register here, and then push it from the register onto the stack)
                m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.ALLOC_STRING_ON_HEAP);
                //m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.READ_STRING_FROM_HEAP);
            }
        }

        /// <summary>
        /// compiles a function call
        /// </summary>
        /// <param name="node">This node is the fucntion to call</param>
        /// <param name="inheritedDataTable"></param>
        /// <param name="name">name of the function</param>
        private void doFunctionCall(XElement node, DataTable inheritedDataTable, String name)
        {
            foreach (XAttribute attrib in node.Attributes())
            {
                pushParameter(inheritedDataTable, attrib.Value);
            }

            // begin the local function block
            m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.BEGIN_BLOCK);

            // push addres
            m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.PUSH_INT);
            m_currentProgramCode.Writer.Write(m_functionTable.Data[name]);
            // call function
            m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.CALL_FUNCTION);
            // end the block after the function returned
            m_currentProgramCode.Writer.Write((int)XmlScriptExecutor.OpCode.END_BLOCK);

            // a function call must not contain children
            if (node.Elements().Count() > 0)
            {
                throw new Exception("Error function call hast child nodes!");
            }
        }

        private bool checkParameterCount(int count, IFunction func)
        {
            if (count < 0) // any number will do
            {
                return true;
            }
            else if (func.ParameterCount <= count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
