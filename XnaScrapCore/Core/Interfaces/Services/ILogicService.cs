using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaScrapCore.Core.Elements;

namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface ILogicService
    {
        void addLogic(AbstractLogic logic);
        void removeLogic(AbstractLogic logic);
    }
}
