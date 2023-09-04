# Lumina3D Engine Readme

The Lumina3D Engine is a 3D game engine built using C# and OpenTK. It provides a framework for creating and managing game entities, components, and rendering. This readme provides an overview of the main components and features of the engine.

## Table of Contents

- [Engine Overview](#engine-overview)
- [Entities and Components](#entities-and-components)
- [Rendering](#rendering)
- [Usage](#usage)

## Engine Overview

The `Engine` class serves as the core of the Lumina3D engine. It handles the game loop, input, updating entities, and rendering frames.

- `StartEngine()`: Starts the engine's game loop and rendering process.
- `Awake()`: Initializes the engine and awakens all entities.
- `Update()`: Runs the main game loop, handling input, updating entities, and rendering frames.
- `HandleInput()`: Handles keyboard and mouse input, including camera movement and rotation.

## Entities and Components

The Lumina3D engine follows an entity-component-system (ECS) architecture. Entities are composed of various components that define their behavior and properties. Notable classes include:

- `GameEntity`: Represents a game entity and holds a collection of components. It supports the creation of child entities.
- `EntityComponent`: The base class for all components. It defines lifecycle methods such as `Awake()`, `Update()`, and `LateUpdate()`.
- `MeshComponent`: A component that handles mesh rendering. It supports loading meshes from both Assimp and glTF files.

## Rendering

The engine's rendering capabilities are facilitated by the `Renderer` class and OpenGL (through OpenTK). The `Renderer` works with various components, including the `MeshComponent`, to render entities on the screen.

- `DrawFrame()`: Renders a single frame using the current state of the entities and components.
- `BuildShaderCache()`: Builds a cache of shaders for rendering based on the mesh data.

## Usage

To use the Lumina3D engine, follow these steps:

1. Create an instance of the `ECSControl` class to manage the OpenGL context.
2. Create an instance of the `Engine` class, passing the `ECSControl` instance to its constructor.
3. Define entities and components as needed for your game.
4. Call the `StartEngine()` method on the `Engine` instance to begin the game loop.

Example usage:

```csharp
// Create ECSControl and Engine instances
ECSControl ecsControl = new ECSControl();
Engine engine = new Engine(ecsControl);

// Create entities and components
GameEntity entity = engine.CreateEntity("MyEntity");
MeshComponent meshComponent = MeshComponent.LoadFromFile("model.gltf");
entity.AddComponent(meshComponent);

// Start the engine
engine.StartEngine();
```

This is a high-level overview of the Lumina3D engine. For more detailed information and examples, refer to the provided codebase and explore the classes and methods mentioned in this readme.

Feel free to customize and extend the engine to suit your specific game development needs. Happy coding!
