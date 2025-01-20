# ECS Building Generation and Simulation System

This system is a Unity **Entity Component System (ECS)** implementation designed to generate, simulate, and render buildings composed of multiple layers and blocks. It efficiently handles the creation, motion simulation, and future GPU-optimized rendering of up to hundreds of thousands of entities.

---

## Features

1. **Building Generation**:
   - Creates multiple buildings with random dimensions, positions, and numbers of layers.
   - Supports distinct layers for floors, ceilings, and walls.
   - Blocks are positioned dynamically with an efficient ECS workflow.

2. **Falling Block Simulation**:
   - Simulates block motion with physics-like velocity and acceleration.
   - Blocks fall to their target positions and snap upon reaching the ground.
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

3. **LOD Optimization**:
   - Add level-of-detail (LOD) management for rendering distant blocks.

4. **Physics Integration**:
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

