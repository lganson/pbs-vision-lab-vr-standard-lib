# PBS Vision Lab VR Standard Library
---
*Developed for the PBS Vision Lab.*
**Repository:** [pbs-vision-lab-vr-standard-lib](https://github.com/lganson/pbs-vision-lab-vr-standard-lib)

## Project Overview
The **PBS Vision Lab VR Standard Library** is a core framework and collection of reusable tools designed to standardize and streamline Virtual Reality (VR) experiment development for the University of Iowa Psychological and Brain Sciences (PBS) Vision Lab. By providing a unified architecture, this library reduces boilerplate code, ensures consistency across research experiments, and accelerates the deployment of new VR environments.

## Purpose & Goals
- **Standardization:** Establish a consistent codebase and design pattern for all VR-based vision and perception experiments.
- **Efficiency:** Abstract complex VR hardware integrations and mechanics so researchers can focus on experimental design rather than low-level programming.
- **Modularity:** Provide plug-and-play components for data logging, participant tracking, and visual stimuli rendering.
- **Scalability:** Enable seamless updates and maintenance across multiple ongoing research projects that utilize the same core mechanics.
- **Data Driven** All experiments should be assembled through a collection of data instead of assembling a scene in engine.
  - Any parameters that may need to be changed should be added as fields in scriptable objects

## Key Features 
- **Precision Data Logging:** Automated, high-frequency tracking of head movements, gaze/eye tracking, hand positions, and interaction events, outputting to researcher-friendly formats (e.g., CSV/JSON).
- **Task System:** Collection of generic tasks and base classes of tasks to allow for creation of experiments by non-technical people within the Unity Engine.
  - Uses an existing repo, [Unity-SerializeReferenceExtensions](https://github.com/mackysoft/Unity-SerializeReferenceExtensions), to serialize a variety of task sequences.
  - Reorganize the order of experiments on the fly or change the configuration of pre-made blocks to fine-tune experiments with feedback from the creator of experiments.
  - Nest groups of tasks to create any collection of random or in-order sequences.
  - Easily extensible by creating a new class that inherits from the Task or Block Runner classes.
    
  - **Example**<img width="1056" height="718" alt="image" src="https://github.com/user-attachments/assets/8a44fdc9-0689-4dec-b2f0-3a3239a3c9d9" />
- **Input System:** Centralize the Unity input system to make hooking into complex Unity events significantly easier.
  - Built-in events for reused input schemes like pulling both triggers (with timing allowance).
  - Support for eye tracking event hooks.
  - Collect input device data such as head and hand positions.
  
## Tech Stack
- **Engine:** Unity
- **Languages:** C# 
- **SDKs & Frameworks:** OpenXR, SteamVR
- **Version Control:** Git & GitHub







# Sample Project
---
## Immersive Visual Search (IS01)

### Task System Usage
<img width="1047" height="507" alt="image" src="https://github.com/user-attachments/assets/25988ac2-88bd-4d69-9c6e-d54356ad11f0" />
<img width="1040" height="395" alt="image" src="https://github.com/user-attachments/assets/02a63453-3bde-4e8f-9c06-8c712d98e269" />
<img width="1040" height="476" alt="image" src="https://github.com/user-attachments/assets/58af86b3-0eba-4ffd-9e67-dd16177ca0e3" />
<img width="1034" height="467" alt="image" src="https://github.com/user-attachments/assets/8d60b281-e955-4889-8cb0-72978657f476" />

### Input System Usage

### Save System Usage


