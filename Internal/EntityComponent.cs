using Lumina3D.Components;
using Lumina3D.Internal;

namespace Lumina3D.Internal
{
    public abstract class EntityComponent
    {
        public Engine Engine;
        public GameEntity Entity { get; internal set; }
        public bool Enabled { get; internal set; } = true;

        public void Enable()
        {
            if (!Enabled)
            {
                OnEnable();
                Enabled = true;

            }
        }

        public void Disable()
        {
            if (Enabled)
            {
                OnDisable();
                Enabled = false;

            }
        }

        public virtual void Awake()
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void EarlyUpdate()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void LateUpdate()
        {

        }
    }
}
