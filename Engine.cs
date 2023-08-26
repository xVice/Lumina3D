using Assimp.Configs;
using ECS3D.ECSEngine.Control;
using Lumina3D.Components;
using Lumina3D.Internal;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lumina3D
{
    public class Engine
    {
        public CameraComponent activeCamera = null;

        private ECSControl activeECSControl = null;

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
            foreach (var entity in entities)
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
            foreach (var ent in entities)
            {
                ent.Update();
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
            entities.Add(entity);

            return entity;
        }

        public List<GameEntity> GetEntities()
        {
            return entities;
        }
        #endregion

        #region Component functions

        public List<T> GetComponents<T>() where T : EntityComponent
        {
            var type = typeof(T);

            var components = entities
                .SelectMany(ent => ent.Components.Values.Where(x => x.GetType() == type))
                .Cast<T>() // Explicitly cast to type T
                .ToList();

            return components;
        }

        public T GetComponent<T>(GameEntity entity) where T : EntityComponent
        {
            Type componentType = typeof(T);
            if (entity.Components.TryGetValue(componentType, out var component))
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

                foreach (var component in entity.Components)
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
