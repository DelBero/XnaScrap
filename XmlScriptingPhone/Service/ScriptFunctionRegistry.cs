using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlScripting.Service.Interfaces;
using XmlScripting.Service.BuiltIn;
using Microsoft.Xna.Framework;

namespace XmlScripting.Service
{
    public class ScriptFunctionRegistry
    {
        #region singelton
        private static ScriptFunctionRegistry m_instance = null;
        /// <summary>
        /// I know singelton is odd...
        /// </summary>
        /// <returns></returns>
        public static ScriptFunctionRegistry GetInstance(Game game)
        {
            if (m_instance == null)
            {
                m_instance = new ScriptFunctionRegistry();
                m_instance.registerBuiltIn(game);
            }
            return m_instance;
        }
        public static ScriptFunctionRegistry GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new ScriptFunctionRegistry();
                m_instance.registerBuiltIn();
            }
            return m_instance;
        }
        #endregion

        #region member
        private List<String> m_registeredFunctions = new List<String>();
        private List<IFunction> m_registeredFuncAddr = new List<IFunction>();

        public List<IFunction> RegisteredFunctionAddresses
        {
            get { return m_registeredFuncAddr; }
            set { m_registeredFuncAddr = value; }
        }

        public List<String> RegisteredFunctions
        {
            get { return m_registeredFunctions; }
            set { m_registeredFunctions = value; }
        }
        #endregion

        /// <summary>
        /// Registeres all builtin functions.
        /// </summary>
        public void registerBuiltIn(Game game)
        {
            //m_registeredFunctions.Add(typeof(Main).ToString());
            register("InvalidOperation", null);
            register("GameObject", new BeginGameObject(game));
            register("Element", new BeginElement(game));
            register("Parameter", new BeginParameter(game));
            register("Value", new Value(game));
            register("Main", new Main(game));
            register("Macro", new CallMacro(game));
            register("RegisterMacro", new RegisterMacro(game));
            register("CreateId", new CreateId(game));
        }

        public void registerBuiltIn()
        {
            //m_registeredFunctions.Add(typeof(Main).ToString());
            register("InvalidOperation", null);
            register("GameObject", new BeginGameObject());
            register("Element", new BeginElement());
            register("Parameter", new BeginParameter());
            register("Value", new Value());
            register("Main", new Main());
            register("Macro", new CallMacro());
            register("RegisterMacro", new RegisterMacro());
            register("CreateId", new CreateId());
        }

        public void register(String name, IFunction func)
        {
            m_registeredFunctions.Add(name);
            m_registeredFuncAddr.Add(func);
        }

    }
}
