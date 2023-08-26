using Lumina3D.Components;
using OpenTK;
using System;
using System.Collections.Generic;

namespace Lumina3D.Internal
{
    public class GameEntity
    {
        public Engine Engine { get; set; }
        public int Id { get; internal set; }
        public string EntityName { get; set; }
        public List<GameEntity> Children = new List<GameEntity>();
        public Dictionary<Type, EntityComponent> Components { get; } = new Dictionary<Type, EntityComponent>();

        public static GameEntity Create3DObj(Engine engine, string filepath, Vector3 pos)
        {
            var ent = engine.CreateEntity(Guid.NewGuid().ToString());
            ent.AddComponent<MeshComponent>(MeshComponent.LoadFromFile(filepath));

            var transform = new TransformComponent();
            transform.Position = pos; // Set the initial position
            transform.Rotation = new Quaternion(new Vector3(0, 0, 0), 1f); // Set the initial rotation
            transform.Scale = new Vector3(1f, 1f, 1f); // Set the initial scale
            ent.AddComponent<TransformComponent>(transform);

            var meshRenderer = new MeshRenderer();
            ent.AddComponent<MeshRenderer>(meshRenderer);
            ent.Awake();
            return ent;

        }

        public GameEntity CreateChild(string name)
        {
          
            var child = Engine.CreateEntity(this, name);
            return child;
        }

        public T AddComponent<T>(T component) where T : EntityComponent
        {
            var componentType = component.GetType();
            Components.Add(componentType, component);
            component.Engine = Engine;
            component.Entity = this;
            return (T)component;
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            var type = typeof(T);
            return (T)Components[type];
        }

        public void Awake()
        {
            foreach (var component in Components.Values)
            {
                component.Awake();
            }
        }

        public void Update()
        {
            foreach (var component in Components.Values)
            {
                component.EarlyUpdate();
                component.Update();
                component.LateUpdate();
            }
        }
    }
}
