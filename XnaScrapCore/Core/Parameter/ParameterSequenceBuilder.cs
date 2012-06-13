using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaScrapCore.Core.Parameter
{
    public class ParameterSequenceBuilder
    {
        private ParameterSequence m_currentSequence = null;

        public ParameterSequence CurrentSequence
        {
            get { return m_currentSequence; }
        }

        public void createSequence()
        {
            m_currentSequence = new ParameterSequence();
            m_currentSequence.ParameterStack.Push(m_currentSequence.Root);
        }

        public void addParameter(Parameter parameter)
        {
            m_currentSequence.Root.addParameter(parameter);
        }
    }
}
