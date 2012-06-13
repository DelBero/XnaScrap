using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XnaScrapCore.Core.Interfaces.Services;
using System.IO;
using XnaScrapCore.Core.Systems.Logic;

namespace XnaScrapCore.Core.Elements
{
    public abstract class AbstractLogic : AbstractElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">The parameters of the element.</param>
        public AbstractLogic(IDataReader state) : base (state)
        {

        }

        public override void doInitialize()
        {
            ILogicService logicService = Owner.Game.Services.GetService(typeof(ILogicService)) as ILogicService;
            logicService.addLogic(this);
            base.doInitialize();
        }

        public override void doDestroy()
        {
            ILogicService logicService = Owner.Game.Services.GetService(typeof(ILogicService)) as ILogicService;
            logicService.removeLogic(this);
            base.doDestroy();
        }

        public virtual void doUpdate(GameTime time)
        {

        }
    }
}
