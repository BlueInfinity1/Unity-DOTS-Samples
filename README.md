# ECS Building Generation and Simulation System

This system is a Unity **Entity Component System (ECS)** implementation designed to generate, simulate, and render buildings composed of multiple layers and blocks. It efficiently handles the creation, motion simulation, and future GPU-optimized rendering of up to hundreds of thousands of entities.

[Video of the system behavior](https://drive.google.com/file/d/12Es2kmqOraFzIuTk4j-Zm1ThUteGYjGu/view?usp=drive_link)

[Bouncier version that works without Unity's physics systems (Details further below)](https://drive.google.com/file/d/12Es2kmqOraFzIuTk4j-Zm1ThUteGYjGu/view?usp=drive_link)

---

## Features

1. **Building Generation**:
   - Creates multiple buildings with random dimensions, positions, and numbers of layers.
   - Supports distinct layers for floors, ceilings, and walls.
   - Blocks are positioned dynamically with an efficient ECS workflow.

2. **Falling Block Simulation**:
   - Simulates block motion with physics-like velocity and acceleration.
   - Blocks fall to their target positions and snap upon reaching their target positions.
   - Uses ECS jobs for high-performance parallel processing.

3. **Collision-Like Interaction**:
   - Supports "fake collision detection" for blocks within a building stack.
   - Planned integration of bouncing logic between overlapping blocks.

---

## Core Components

### EntityReferencesAuthoring
- Links GameObject prefabs to ECS entities during the baking process.
- Provides references for building parent, layer, and block prefabs.

### BuildingGeneratorSystem
- Generates buildings with randomized parameters.
- Assigns positions, layers, and blocks to buildings and initializes their motion.

### BuildingBlockSystem
- Updates the velocity, position, and motion of blocks using ECS jobs.
- Prepares for advanced motion interactions like bouncing or adjustments for overlapping blocks.

---

## Future Improvements

1. **GPU Instancing**:
   - Integrate indirect GPU instancing to render blocks efficiently.
   - Implement per-building or single-bounding box setups for optimized draw calls.

2. **Block-Bouncing Logic**:
   - Add block interaction logic to handle bounces when blocks overlap.
   - Simulate perfectly elastic collisions for more dynamic block behavior.
     
3. **Combined Building Collider**:
   - Plan to generate a single, combined collider for each building.
   - This will allow individual building blocks to work without colliders, reducing physics overhead.
   - The combined collider will be dynamically updated based on the dimensions and layout of the building.
   - Enables efficient collision handling while maintaining performance for large-scale buildings.

4. **LOD Optimization**:
   - Add level-of-detail (LOD) management for rendering distant blocks.

5. **Physics Integration**:
   - Optionally integrate Unity Physics for more complex block interactions.

---

## How It Works

1. **Setup**:
   - Attach the `EntityReferencesAuthoring` script to a GameObject and assign prefabs.
   - Ensure prefabs include appropriate components (e.g., `BuildingBlockData`).

2. **Building Generation**:
   - `BuildingGeneratorSystem` initializes the buildings and layers during the first `OnUpdate`.

3. **Block Simulation**:
   - `BuildingBlockSystem` processes blocks in parallel using ECS jobs, simulating motion.

---

## Bouncier Version (Work in Progress)

A bouncier variant of the block system is under development, designed to operate entirely without Unity's built-in physics system, further reducing overhead.

### How It Works

- **Stack Organization**:  
  All blocks are organized into stacks, and collisions are calculated based on their relative positions within the same stack.

- **Collision Detection**:  
  For two blocks, `N` (above) and `N+1` (below), a collision is detected if the difference between their `y` coordinates is less than the `blockHeight`.

- **Collision Resolution**:  
  When a collision occurs:
  1. The blocks are separated so their distance equals or exceeds `blockHeight`.
  2. Velocities are exchanged:
     - Block `N+1` is propelled downward with Block `N`'s velocity.
     - Block `N` is propelled upward with Block `N+1`'s velocity.

- **Perfect Elasticity**:  
  Since all blocks are identical in mass, the collisions are treated as perfectly elastic, preserving total energy and momentum within the system.

---

This design allows for efficient, dynamic interactions while maintaining high performance, even with large-scale simulations.



