using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CBero.Service.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CBero.Service.Elements.Interfaces;
using XnaScrapCore.Core.Systems.Resource;

namespace CBero.Service
{
    public class RenderState
    {
        RenderManager m_renderManager;
        public Stack<ICamera> m_currentCamera = new Stack<ICamera>();
        public Stack<Matrix> m_currentWorld = new Stack<Matrix>();
        public Stack<Matrix> m_currentView = new Stack<Matrix>();
        public Stack<Matrix> m_currentProj = new Stack<Matrix>();
        public Stack<Texture2D>[] m_currentTexture = new Stack<Texture2D>[RenderManager.MAX_TEXTURES];
        public Stack<Material> m_currentMaterial = new Stack<Material>();
        public Stack<IRenderTarget> m_currentRenderTarget = new Stack<IRenderTarget>();
        public Stack<Effect> m_currentEffect = new Stack<Effect>();
        public Stack<IRenderable3D> m_currentRenderable = new Stack<IRenderable3D>();
        private EffectPass m_currentEffectPass = null;
        private Color ClearColor = Color.CornflowerBlue;
        private Color AmientLighting = Color.White;
        private Color FogColor = Color.CornflowerBlue;

        public RenderState(RenderManager renderManager)
        {
            m_renderManager = renderManager;
            for(int i = 0; i < RenderManager.MAX_TEXTURES; ++i)
            {
                m_currentTexture[i] = new Stack<Texture2D>();
            }
        }

        public ICamera Camera
        {
            get
            {
                return m_currentCamera.Peek();
            }
            set
            {
                PopCamera();
                PushCamera(value);
            }
        }

        public void setTexture(Texture2D tex, int unit)
        {
            if (unit > RenderManager.MAX_TEXTURES)
                return;

            if (m_currentTexture[unit].Count > 0)
                m_currentTexture[unit].Pop();

            m_currentTexture[unit].Push(tex);
            try
            {
                m_renderManager.GraphicsDevice.Textures[unit] = tex;
            }
            catch (ObjectDisposedException od)
            {
                System.Console.WriteLine(od.Message);
            }
            catch (InvalidOperationException io)
            {
                System.Console.WriteLine(io.Message);
            }
        }

        public void PushCamera(ICamera camera)
        {
            m_currentCamera.Push(camera);
            m_currentView.Push(camera.View);
            m_currentProj.Push(camera.Projection);
        }

        public void PopCamera()
        {
            if (m_currentCamera.Count > 0)
                m_currentCamera.Pop();
            if (m_currentView.Count > 0)
                m_currentView.Pop();
            if (m_currentProj.Count > 0)
                m_currentProj.Pop();
        }

        public void PushRenderable(IRenderable3D renderable)
        {
            m_currentRenderable.Push(renderable);
            Matrix scale = Matrix.CreateScale(renderable.Scale);
            Matrix rot = Matrix.CreateFromQuaternion(renderable.Orientation);
            Matrix trans = Matrix.CreateTranslation(renderable.Position);
            m_currentWorld.Push(scale * rot * trans);
        }

        public void PopRenderable()
        {
            m_currentWorld.Pop();
            m_currentRenderable.Pop();
        }

        public void PushRenderTarget(IRenderTarget renderTarget)
        {
            m_currentRenderTarget.Push(renderTarget);
            m_renderManager.setRenderTarget(renderTarget);
        }

        public void PopRenderTarget()
        {
            m_currentRenderTarget.Pop();
            if (m_currentRenderTarget.Count > 0)
                m_renderManager.setRenderTarget(m_currentRenderTarget.Peek());
        }

        #region colors
        public void SetAmbientLighting(Color color)
        {
            this.AmientLighting = color;
        }
        public Color GetAmbientLighting()
        {
            return this.AmientLighting;
        }
        public void SetClearColor(Color color)
        {
            this.ClearColor = color;
        }
        public Color GetClearColor()
        {
            return this.ClearColor;
        }
        public void SetFogColor(Color color)
        {
            this.FogColor = color;
        }
        public Color GetFogColor()
        {
            return this.FogColor;
        }
        #endregion

        #region material
        public void ApplyEffectPass()
        {
            if (m_currentEffectPass != null)
            {
                m_currentEffectPass.Apply();
            }
        }

        public void PushMaterial(Material mat)
        {
            if (mat == null)
            {
                mat = RenderManager.BasicEffectMaterial;
            }
            m_currentMaterial.Push(mat);
            m_currentEffect.Push(mat.Effect);
            ApplyMaterial(mat);
        }

        public void PopMaterial()
        {
            m_currentMaterial.Pop();
            m_currentEffect.Pop();
            ApplyMaterial(m_currentMaterial.Peek());
        }

        public void ApplyEffectParameterPerInstance()
        {
            // apply shader data
            foreach (Material.ParameterMapping mapping in m_currentMaterial.Peek().ParameterMappings)
            {
                if (mapping.perInstance)
                {
                    EffectParameter effectParameter = m_currentEffect.Peek().Parameters[mapping.name];
                    m_renderManager.setBuiltInShaderParameter(effectParameter, mapping.semantic);
                }
            }
        }

        public void ApplyEffectParameterPerFrame()
        {
            // apply shader data
            foreach (Material.ParameterMapping mapping in m_currentMaterial.Peek().ParameterMappings)
            {
                if (!mapping.perInstance)
                {
                    EffectParameter effectParameter = m_currentEffect.Peek().Parameters[mapping.name];
                    m_renderManager.setBuiltInShaderParameter(effectParameter, mapping.semantic);
                }
            }
        }

        private void ApplyMaterial(Material material)
        {
            m_currentEffect.Peek().CurrentTechnique = m_currentEffect.Peek().Techniques[material.Technique];
            m_currentEffectPass = m_currentEffect.Peek().CurrentTechnique.Passes.First();
            // apply textures
            for (int i = 0; i < RenderManager.MAX_TEXTURES; ++i)
            {
                setTexture(material.Textures[i], i);
            }

            // apply shader data
            ApplyEffectParameterPerFrame();
        }

        #endregion
    }
}
