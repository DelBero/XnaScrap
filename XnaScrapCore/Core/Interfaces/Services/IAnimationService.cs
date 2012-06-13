using System;
namespace XnaScrapCore.Core.Interfaces.Services
{
    public interface IAnimationFactory
    {
        IAbstractAnimation createAnimation();
    }

    public interface IAbstractAnimation
    {
    }

    public interface IAnimationService
    {
        IAnimationFactory DefaultSkinnedAnimationFactory(object data);
        void addAnimation(XnaScrapCore.Core.XnaScrapId id, IAnimationFactory aa);
        IAbstractAnimation getAnimation(XnaScrapId animationId);
        void removeAnimation(XnaScrapCore.Core.XnaScrapId id);
    }
}
