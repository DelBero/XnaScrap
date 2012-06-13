using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SceneManagement.Service.Elements;

namespace SceneManagement.Service.Interfaces
{
    public interface ISelectionExecutor
    {
        /// <summary>
        /// Handle an element, that is selected within a selection query
        /// </summary>
        /// <param name="element"></param>
        void execute(SceneElement element);
    }
}
