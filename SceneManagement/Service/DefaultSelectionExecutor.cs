using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneManagement.Service.Interfaces;
using SceneManagement.Service.Elements;

namespace SceneManagement.Service
{
    public class DefaultSelectionExecutor : ISelectionExecutor
    {
        #region members
        private List<SceneElement> m_selected = new List<SceneElement>();

        public List<SceneElement> Selected
        {
            get { return m_selected; }
        }
        #endregion

        public DefaultSelectionExecutor()
        {

        }

        /// <summary>
        /// Handle an element, that is selected within a selection query
        /// </summary>
        /// <param name="element"></param>
        public void execute(SceneElement element)
        {
            m_selected.Add(element);
        }
    }
}
