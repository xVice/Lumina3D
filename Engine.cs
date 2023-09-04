using Lumina3D.Components;
using Lumina3D.Internal;
using Silk.NET.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace Lumina3D
{
    public class Engine
    {
        public CameraComponent activeCamera = null;

        private Queue<GameEntity> entityQue = new Queue<GameEntity>(); //Used to add entitys to the below list just before updating to avoid editing the collection while enumerating.
        private List<GameEntity> entities = new List<GameEntity>();
        public double DeltaTime { get; set; }
        public Int64 Time = 0;
        public Renderer renderer { get; set; }

        bool isRunning = false;
        private Thread updaterThread;

        public Engine()
        {

        }

        #region "gameloop"/global functions / ecs stuff / gamemanager
        public void StartEngine()
        {
            renderer = new Renderer(this);
            renderer.InitializeGL();
            isRunning = true;
            
        }

        public void Awake()
        {
            foreach (var entity in GetEntities())
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

        private Vector2 lastMousePos;

        public void MoveCame()
        {
            var kb = renderer.GetInputContext().Keyboards[0];
            if (kb.IsKeyPressed(Key.W))
            {
                activeCamera.Move(CameraComponent.CameraMovement.Forward);
            }
            else if (kb.IsKeyPressed(Key.S))
            {
                activeCamera.Move(CameraComponent.CameraMovement.Backward);
            }
            else if (kb.IsKeyPressed(Key.A))
            {
                activeCamera.Move(CameraComponent.CameraMovement.Left);
            }
            else if (kb.IsKeyPressed(Key.D))
            {
                activeCamera.Move(CameraComponent.CameraMovement.Right);
            }
        }

        public void RotateCam()
        {
            // Retrieve the mouse device
            var mouse = renderer.GetInputContext().Mice[0];

            // Check if the left mouse button is pressed
            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                // Calculate the mouse delta
                var mouseDelta = mouse.Position - lastMousePos;

                // Rotate the camera using the mouse delta
                activeCamera.Rotate(mouseDelta);


                // Redraw the scene
                // RenderFrame();
            }

            // Update the last mouse position
            lastMousePos = mouse.Position;
        }

        public void HandleInput()
        {
            RotateCam();
            MoveCame();
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
