using Lumina3D.Components;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lumina3D.Internal
{
    public class GameEntity
    {
        public Engine Engine { get; set; }
        public int Id { get; internal set; }
        public string EntityName { get; set; }
       
        public List<GameEntity> Children = new List<GameEntity>();
        private Dictionary<Type, EntityComponent> Components { get; } = new Dictionary<Type, EntityComponent>();
        private Queue<Tuple<Type,EntityComponent>> ComponentQue = new Queue<Tuple<Type, EntityComponent>>();

        //Helper function to create generic 3d mesh objects, without any physics
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

        public Dictionary<Type, EntityComponent> GetComponents()
        {
            var dict = new Dictionary<Type, EntityComponent>();
            foreach(var comp in Components)
            {
                dict.Add(comp.Key, comp.Value);
            }
            foreach(var comp in ComponentQue)
            {
                dict.Add(comp.Item1, comp.Item2);
            }
            return dict;
        }

        public T AddComponent<T>(T component) where T : EntityComponent
        {
            var componentType = component.GetType();
            ComponentQue.Enqueue(new Tuple<Type,EntityComponent>(componentType, component));
            component.Engine = Engine;
            component.Entity = this;
            return (T)component;
        }

        public T GetComponent<T>() where T : EntityComponent
        {
            return (T)GetComponents()[typeof(T)];
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
            if(ComponentQue.Count > 0)
            {
                for (int i = 0; i < ComponentQue.Count; i++)
                {
                    var comp = ComponentQue.Dequeue();
                    Components.Add(comp.Item1, comp.Item2);
                }
            }

            foreach (var component in Components.Values)
            {
                component.EarlyUpdate();
                component.Update();
                component.LateUpdate();
            }
        }
    }
}
