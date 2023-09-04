using ECS3D.ECSEngine.Control;
using Lumina3D.Components;
using Lumina3D.Internal;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Lumina3D
{
    public class Engine
    {
        public CameraComponent activeCamera = null;

        private ECSControl activeECSControl = null;

        private Queue<GameEntity> entityQue = new Queue<GameEntity>(); //Used to add entitys to the below list just before updating to avoid editing the collection while enumerating.
        private List<GameEntity> entities = new List<GameEntity>();
        public float DeltaTime { get; set; }
        public Int64 Time = 0;
        public Renderer renderer { get; set; }

        bool isRunning = false;
        private Thread updaterThread;

        public Engine(ECSControl control)
        {
            activeECSControl = control;
        }

        #region "gameloop"/global functions / ecs stuff / gamemanager
        public void StartEngine()
        {
            renderer = new Renderer(activeECSControl);
            isRunning = true;
            updaterThread = new Thread(Update)
            {
                Name = "ECSUpdaterThread",
                IsBackground = true
            };
            updaterThread.Start();
        }

        public void Awake()
        {
            foreach (var entity in GetEntities()) //<- Im not sure if i should use GetEntitys() instead here, well see.
            {
                entity.Awake();
            }
        }

        public void Update()
        {
            Awake();
            while (isRunning)
            {
                Time++;
                HandleInput();
                UpdateEntitys();
                renderer.DrawFrame();
            }
        }

        private void UpdateEntitys()
        {
            if(entityQue.Count > 0) //Check if any ents are qued
            {
                for (int i = 0; i < entityQue.Count; i++)
                {
                    entities.Add(entityQue.Dequeue()); //merge queded entites into the list, did this to avoid editing the collection while enumerating.
                }
            }

            foreach (var ent in entities)
            {
                ent.Update(); //Update all entities instantiated in the engine, should be only in a scene though.
            }
        }

        private void HandleInput()
        {
            var kbState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            if(activeCamera != null)
            {
                activeCamera.MoveCam(kbState);
                activeCamera.RotateCam(mouseState);
            }
           
        }
        #endregion 

        #region Entity functions
        public GameEntity CreateEntity(GameEntity Parent, string name)
        {
            var ent = CreateEntity(name);
            Parent.Children.Add(ent);
            return ent;
        }

        public GameEntity CreateEntity(string name)
        {
            GameEntity entity = new GameEntity { Id = entities.Count + 1, EntityName = name };
            entity.Engine = this;
            entityQue.Enqueue(entity);
            return entity;
        }

        public List<GameEntity> GetEntities()
        {
            //make sure we allways have accsess to both, un"cached" entitys and "cached"/listed items.
            var entities = new List<GameEntity>();
            entities.AddRange(entities.ToArray());
            entities.AddRange(entityQue.ToArray());
            return entities;
        }
        #endregion

        #region Component functions

        public List<T> GetComponents<T>() where T : EntityComponent
        {
            var type = typeof(T);

            var components = GetEntities()
                .SelectMany(ent => ent.GetComponents().Values.Where(x => x.GetType() == type))
                .Cast<T>() // Explicitly cast to type T
                .ToList();

            return components;
        }

        public T GetComponent<T>(GameEntity entity) where T : EntityComponent
        {
            Type componentType = typeof(T);
            if (entity.GetComponents().TryGetValue(componentType, out var component))
            {
                return (T)component;
            }

            return null;
        }

        #endregion

        #region Helper functions
        public void FixAspect()
        {
            if (activeCamera != null)
            {
                activeCamera.AspectRatio = (float)activeECSControl.ClientSize.Width / (float)activeECSControl.ClientSize.Height;
            }
        }

        public ECSControl GetControl()
        {
            return activeECSControl;
        }

        public TreeNode[] GetExplorerNodes()
        {
            var nodes = new List<TreeNode>();
            foreach (var entity in GetEntities())
            {
                TreeNode entityNode = new TreeNode($"{entity.EntityName}");

                // Node for general information about the entity
                TreeNode infoNode = new TreeNode("General Info");

                TreeNode nameNode = new TreeNode($"Entity Name: {entity.EntityName}");
                infoNode.Nodes.Add(nameNode);

                TreeNode idNode = new TreeNode($"Entity ID: {entity.Id}");
                infoNode.Nodes.Add(idNode);
                entityNode.Nodes.Add(infoNode);

                // Node for listing components
                TreeNode componentsNode = new TreeNode("Components");

                foreach (var component in entity.GetComponents())
                {
                    TreeNode componentNode = new TreeNode(component.Key.ToString());

                    // Node for general information about the component
                    TreeNode componentInfoNode = new TreeNode("General Info");

                    TreeNode componentEnabledNode = new TreeNode($"Component Enabled: {component.Value.Enabled}");
                    componentInfoNode.Nodes.Add(componentEnabledNode);
                    componentNode.Nodes.Add(componentInfoNode);

                    // Node for properties of the component
                    TreeNode propertiesNode = new TreeNode("Properties");

                    foreach (var property in component.Value.GetType().GetProperties())
                    {
                        string propertyName = property.Name;
                        string propertyValue = property.GetValue(component.Value)?.ToString();
                        if (propertyValue != null)
                        {
                            TreeNode propertyNode = new TreeNode($"{propertyName} | {propertyValue}");
                            propertiesNode.Nodes.Add(propertyNode);
                        }
                    }

                    // Node for methods (functions) of the component
                    TreeNode functionsNode = new TreeNode("Functions");

                    foreach (var method in component.Value.GetType().GetMethods())
                    {
                        TreeNode methodNode = new TreeNode(method.Name);
                        functionsNode.Nodes.Add(methodNode);
                    }

                    componentNode.Nodes.Add(propertiesNode);
                    componentNode.Nodes.Add(functionsNode);

                    componentsNode.Nodes.Add(componentNode); // Add the component node to the componentsNode
                }

                entityNode.Nodes.Add(componentsNode); // Add the componentsNode to the entityNode
                nodes.Add(entityNode);
            }
            return nodes.ToArray();

        }
        #endregion
    }
}
