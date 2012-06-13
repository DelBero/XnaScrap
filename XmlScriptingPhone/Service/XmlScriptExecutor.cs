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
using XnaScrapCore.Core.Systems;
using XnaScrapCore.Core.Parameter;
using XnaScrapCore.Core.Exceptions;

namespace XmlScripting.Service
{
    public class XmlScriptExecutor : IScriptExecutor
    {
        #region singelton
        private static XmlScriptExecutor m_instance = null;
        /// <summary>
        /// I know singelton is odd...
        /// </summary>
        /// <returns></returns>
        public static XmlScriptExecutor GetInstance(Game game)
        {
            if (m_instance == null)
            {
                m_instance = new XmlScriptExecutor(game);
                
            }
            return m_instance;
        }

        #endregion

        private ScriptFunctionRegistry m_registeredFunctions = null;

        #region member
        //private Dictionary<String, IFunction> m_registeredFunctions = new Dictionary<string,IFunction>();
        public enum OpCode
        {
            PUSH_INT,
            PUSH_FLOAT,
            PUSH_STRING,
            PUSH_ADDR,
            POP_INT,
            POP_FLOAT,
            POP_STRING,
            CALL_FUNCTION,
            RETURN,
            BEGIN_BLOCK,
            END_BLOCK,
            READ_INT,       // stack
            WRITE_INT,      // stack
            READ_FLOAT,     // stack
            WRITE_FLOAT,    // stack
            PUSH_EBP,       // push ebp
            // arithmetic
            ADD_INT,            // add values on stack and push them
            SUB_INT,
            MUL,
            NATIVE_CALL,
            WRITE_STRING_TO_HEAD,
            READ_STRING_FROM_HEAP,
            ALLOC_STRING_ON_HEAP,
            FREE_STRING,
            HALT            
        };

        private int m_eip = 0;
        private int m_ebp = 0;
        private OpCode m_opcode = OpCode.HALT;

        #endregion

        public XmlScriptExecutor(Game game)
        {
            m_registeredFunctions = ScriptFunctionRegistry.GetInstance(game);
            game.Services.AddService(typeof(IScriptExecutor), this);
            game.Services.AddService(typeof(XmlScriptExecutor), this);
        }

        /// <summary>
        /// Register all methods with an AttrXmlFunc attribute of an object to be used as XmlScripting Functions.
        /// </summary>
        /// <param name="pTarget"></param>
        public void registerFunction(Object pTarget)
        {
            Type targetType = pTarget.GetType();

            foreach (MethodInfo mInfo in targetType.GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(AttrXmlFunc))
                    {
                        AttrXmlFunc xmlAttr = attr as AttrXmlFunc;
                        
                    }
                }
            }
        }

        /// <summary>
        /// Registeres all builtin functions.
        /// </summary>
        /// <param name="game"></param>
        //public void registerBuiltIn(Game game)
        //{
        //    m_registeredFunctions.Add("Main", new XmlScriptingPhone.Service.BuiltIn.Main(game));
        //    m_registeredFunctions.Add("GameObject", new XmlScriptingPhone.Service.BuiltIn.BeginGameObject(game));
        //    m_registeredFunctions.Add("Element", new XmlScriptingPhone.Service.BuiltIn.BeginElement(game));
        //    m_registeredFunctions.Add("Parameter", new XmlScriptingPhone.Service.BuiltIn.BeginParameter(game));
        //    m_registeredFunctions.Add("Value", new XmlScriptingPhone.Service.BuiltIn.Value(game));
        //}

        public void execute(String scriptCode)
        {
            XDocument script = XDocument.Parse(scriptCode);
            foreach (XElement element in script.Elements())
            {
                if ("Main" == element.Name.LocalName)
                {
                    execute(element);
                }
            }
        }

        public void execute(XElement node)
        {
            if (m_registeredFunctions.RegisteredFunctions.Contains(node.Name.LocalName))
            {
                int index = m_registeredFunctions.RegisteredFunctions.IndexOf(node.Name.LocalName);
                IFunction function = m_registeredFunctions.RegisteredFunctionAddresses[index] as IFunction;
                function.execute(node,this);
            }
        }

        // URGENT: Use Parameter Sequencer
        /// <summary>
        /// Execute a script with the given parameters.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="parameters"></param>
        public void execute(XmlScriptCompiled script, String[] parameters)
        {
            Stack data = new Stack();
            Heap heap = new Heap();
            // parameters in reverse order
            for(int i = parameters.Length-1; i >= 0; --i)
            {
                int a = heap.allocString(parameters[i]);
                data.push(a);
            }
            // push eip
            data.push(0);
            // push base pointer
            data.push(0);

            execute(script, data, heap);
        }

        /// <summary>
        /// Execute a script with the given parameters.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="parameters"></param>
        public void execute(IScript script)
        {
            if (!(script is XmlScriptCompiled))
            {
                throw new InvalidOperationException("Wrong script type!");
            }
            XmlScriptCompiled xmlScript = script as XmlScriptCompiled;
            Heap heap = new Heap();

            //// push eip
            //parameters.push(0);
            //// push base pointer
            //parameters.push(0);

            //execute(script, parameters, heap);

            Stack stack = new Stack();
            // go through intern parametersequencer and alloc strings
            if (script.ParameterSequence.Root != null)
            {
                //stack = new Stack(script.ParameterSequence.getMemStream(false));
                foreach (Parameter p in script.ParameterSequence.Root.Parameters.Values)
                {
                    // for now we only allow strings
                    if (p is StringParameter)
                    {
                        StringParameter stringParam = p as StringParameter;
                        stack.push(heap.allocString(stringParam.Value));
                    }
                    else
                    {
                        throw new WrongParameterTypeException("Only StringParameters are allowed when initialising a script.", typeof(StringParameter));
                    }
                    
                }
            }

            // push base pointer
            stack.push(0);
            // push instruction pointer
            stack.push(0);

            execute(xmlScript, stack, heap);
        }

        /// <summary>
        /// Executes a script from the beginging.
        /// </summary>
        /// <param name="script">The script tu execute.</param>
        /// <param name="data">Stack with the addresses of parameters on the heap. These addresses point to the beginning of the parameters in the heap Stack.</param>
        /// <param name="heap">Stack containing the parameter.</param>
        public void execute(XmlScriptCompiled script, Stack data, Heap heap)
        {
            script.reset();
            executeSub(script.ProgramCode, script.EntryPoint, data, heap);
        }

        /// <summary>
        /// Executes a subroutine. All parameters must be written to the heap before execution.
        /// Afterwards the addresses of the parameters have to be stored in the stack (realtive to stackpointer).
        /// Mind this if you want to execute a script with external parameters.
        /// </summary>
        /// <param name="programCode">The script's code.</param>
        /// <param name="data">The data stack of the current script execution.</param>
        public void executeSub(MemoryStream programCode, int entryPoint, Stack data, Heap heap)
        {
            m_eip = entryPoint;
            m_ebp = (int)data.Position; // base pointer
            readNextOpCode(programCode);
            while (m_opcode != OpCode.HALT)
            {
                //eip = (int)programCode.Position;
                switch (m_opcode)
                {
                    case OpCode.POP_INT:
                        {
                            break;
                        }
                    case OpCode.POP_FLOAT:
                        {
                            break;
                        }
                    case OpCode.POP_STRING:
                        {
                            break;
                        }
                    case OpCode.PUSH_INT:
                        {
                            data.push(readNextProgramInt(programCode));
                            break;
                        }
                    case OpCode.PUSH_FLOAT:
                        {
                            data.push(readNextProgramFloat(programCode));
                            break;
                        }
                    case OpCode.PUSH_STRING:
                        {
                            data.push(readNextProgramString(programCode));
                            break;
                        }
                    case OpCode.PUSH_ADDR:
                        {
                            int addr = readNextProgramInt(programCode); // address relative to ebp
                            // read heap address from stack
                            int heapAddr = data.readInt(m_ebp + addr);
                            data.push(heapAddr);
                            break;
                        }
                    case OpCode.PUSH_EBP:
                        {
                            data.push(m_ebp);
                            break;
                        }
                    case OpCode.ADD_INT:
                        {
                            int val1 = data.popInt();
                            int val2 = data.popInt();
                            data.push(val1 + val2);
                            break;
                        }
                    case OpCode.SUB_INT:
                        {
                            int val1 = data.popInt();
                            int val2 = data.popInt();
                            data.push(val1 - val2);
                            break;
                        }
                    case OpCode.READ_INT:
                        {
                            int addr = data.popInt();
                            data.push(data.readInt(addr));
                            break;
                        }
                    case OpCode.WRITE_INT:
                        {
                            // read address
                            int addr = data.popInt();
                            // read value
                            int value = data.popInt();
                            // write
                            data.writeInt(addr,value);
                            break;
                        }
                    case OpCode.READ_FLOAT:
                        {
                            int addr = data.popInt();
                            data.push(data.readFlt(addr));
                            break;
                        }
                    case OpCode.WRITE_FLOAT:
                        {
                            // read address
                            int addr = data.popInt();
                            // read value
                            float value = data.popInt();
                            // write
                            data.writeFlt(addr, value);
                            break;
                        }
                    case OpCode.ALLOC_STRING_ON_HEAP:
                        {
                            String s = data.popString();
                            int address = heap.allocString(s);
                            data.push(address);
                            break;
                        }
                    case OpCode.READ_STRING_FROM_HEAP:
                        {
                            //int ptr = data.readInt((int)data.Position + addr * sizeof(int));
                            int ptr = data.popInt();
                            String s = heap.readString(ptr);
                            data.push(s);
                            break;
                        }
                    case OpCode.BEGIN_BLOCK:
                        {
                            // save base pointer
                            m_ebp = (int)data.Position;
                            data.push(m_ebp);
                            break;
                        }
                    case OpCode.CALL_FUNCTION:
                        {
                            // get function address
                            int func_addr = data.popInt();
                            // save instruction pointer
                            data.push(m_eip);

                            m_eip = func_addr;
                            programCode.Position = m_eip;
                            break;
                        }
 
                    case OpCode.RETURN:
                        {
                            m_eip = data.popInt(); // restore eip
                            programCode.Position = m_eip;
                            break;
                        }
                    case OpCode.END_BLOCK:
                        {
                            m_ebp = data.popInt(); // restore ebp
                            data.set(m_ebp);
                            break;
                        }
                        // Built in function
                    case OpCode.NATIVE_CALL:
                        {
                            int f = readNextProgramInt(programCode);
                            if (f > 0)
                            {
                                m_registeredFunctions.RegisteredFunctionAddresses[f].execute(data,heap);
                            }
                            else if (f < 0)
                            {
                                m_registeredFunctions.RegisteredFunctionAddresses[-1*f].end(data,heap);
                            }
                            break;
                        }
                }
                readNextOpCode(programCode);
            }
        }

        public static String readStringFromHeap(Stack data, Heap heap)
        {
            // get address
            int addr = data.popInt();
            int ptr = data.readInt((int)data.Position + addr * sizeof(int));
            return heap.readString(ptr);
        }

        private OpCode readNextOpCode(MemoryStream programCode)
        {            
            programCode.Position = m_eip;
            m_eip += sizeof(int);
            m_opcode = (OpCode)programCode.Reader.ReadInt32();
            return m_opcode;
        }

        private int readNextProgramInt(MemoryStream programCode)
        {
            programCode.Position = m_eip;
            m_eip += sizeof(int);
            return programCode.Reader.ReadInt32();
        }

        private float readNextProgramFloat(MemoryStream programCode)
        {
            programCode.Position = m_eip;
            m_eip += sizeof(float);
            return programCode.Reader.ReadSingle();
        }

        private String readNextProgramString(MemoryStream programCode)
        {
            programCode.Position = m_eip;
            String ret = programCode.Reader.ReadString();
            m_eip += ret.Length+1;
            return ret;
        }
    }
}
