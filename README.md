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
- **Data-Driven:** All experiments should be assembled through a collection of data instead of assembling a scene in-engine.
  - Any parameters that may need to be changed should be added as fields in `TaskSequence` `ScriptableObject`s

## Key Features 
- **Data Logging:** Automated, high-frequency tracking of head movements, gaze/eye tracking, hand positions, and interaction events, outputting to researcher-friendly formats (e.g., CSV/JSON).
  - Defining new data formats is as simple as creating a new class that extends from `BlockData` and making fields public AND serializable. (If a variable is not natively serializable by `Newtonsoft` or Unity, it is possible to create your own serialization, but it is more involved).
- **Task System:** Collection of generic tasks and base classes of tasks to allow for the creation of experiments by non-technical people within the Unity Engine.
  - Uses an existing repo, [Unity-SerializeReferenceExtensions](https://github.com/mackysoft/Unity-SerializeReferenceExtensions), to serialize a variety of task sequences.
  - Reorganize the order of experiments on the fly or change the configuration of pre-made blocks to fine-tune experiments with feedback from the creator of experiments.
  - Nest groups of tasks to create any collection of random or in-order sequences.
  - Easily extensible by creating a new class that inherits from the `Task` or `BlockRunner` classes.
  - **Example:**  
    <img width="1056" height="718" alt="image" src="https://github.com/user-attachments/assets/8a44fdc9-0689-4dec-b2f0-3a3239a3c9d9" />
- **Input System:** Centralize the Unity Input System to make hooking into complex Unity events significantly easier.
  - Built-in events for reused input schemes like pulling both triggers (with timing allowance).
  - Support for eye-tracking event hooks.
  - Collect input device data such as head and hand positions.
- **Editor Additions:** Add a couple of editor attributes and rendering utilities that enable the Task System, as well as some minor helpers for selecting scenes, tags, and validating/selecting file paths.
  
## Tech Stack
- **Engine:** Unity
- **Languages:** C# 
- **SDKs & Frameworks:** OpenXR, SteamVR
- **Version Control:** Git & GitHub

---

# Sample Project
## Immersive Visual Search (IS01)

### Task System Usage
**General Overview**
- Single task sequence that defines the order to execute a variety of different tasks throughout the experiment.
- Tasks can be selected from the dropdown next to their name, which changes all of the fields available to users.
- In this experiment, 4 different kinds of tasks were used:
  - `ValidationTask`
  - `VisualSearchHandCheckTask`, which is derived from the `HandCheckTask`.
  - `BlockTask`, which takes a `BlockRunner` that will be executed and provides some basic saving utility.
  - `TaskGroup`, which can hold other tasks (including other `TaskGroup`s).
    - `TaskGroup`s are a large structural piece in experiments as they allow for grouping logical tasks while allowing for randomness or other forms of organization, such as counterbalancing across different participants.
- Names can be assigned to each task to make it easier to follow the flow of experiments.

<img width="1047" height="507" alt="image" src="https://github.com/user-attachments/assets/25988ac2-88bd-4d69-9c6e-d54356ad11f0" />

**Block Task Example**
- This is an example of a `BlockTask` that uses the `VisualSearchRunner` to define its behavior.
- `VisualSearchRunner` is derived from the base `BlockRunner` provided in the *pbs-vision-lab-vr-standard-lib* and provides multiple fields that can be used to manipulate behavior:
  - `SpawnManager` Prefabs: Used to generate a profile for how objects are spawned inside of the Immersive Search.
  - Occlusion Enabled and Active Scotoma: Used to determine if a scotoma is present and, if so, what texture is passed to the shader used to render it.
  - Movement Enabled: Enables movement of the objects spawned by the `SpawnManager`s.
  - Anything else could be added to the `VisualSearchRunner` as long as it is serializable within the Unity Engine. 
    - If it isn't natively serializable, writing custom serialization is possible (and has been done with some editor attributes like the scene attribute).
  - If there are objects within the scene that need to be captured on scene load, they can be found either by giving them a tag or by looking them up by name, as seen in the Left Controller Name and Right Controller Name fields.
    - One good use case for using a tag here is for capturing spawn locations that you'd like to set via invisible `GameObject`s in a scene. This solution was used in the Simon Effect experiments (Barrier and Hot Poles).
- This is a single example of how a runner might look in the inspector, but anything is possible as long as the runner can be defined with a single entry and exit point (*entering and exiting the same trial is a little more complex and will require some extra effort*).

<img width="1040" height="395" alt="image" src="https://github.com/user-attachments/assets/02a63453-3bde-4e8f-9c06-8c712d98e269" />  
<img width="1040" height="476" alt="image" src="https://github.com/user-attachments/assets/58af86b3-0eba-4ffd-9e67-dd16177ca0e3" />

### Input System Usage
- The Input System is plug-and-play with whatever `InputActionAsset`s you throw at it, so all that's needed is to subscribe to the correct events.
- IS01 mainly uses the "both triggers pressed" event and the "either trigger pressed" event to determine when it should move between trials and when the user has pulled a trigger to indicate they have seen something, along with which trigger they pressed.
- The `EyeTrackingManager` and `InputManager` were extended via events to support gaze-contingent input requirements.
  - This is used to force participants to fixate on a cross between events and track when the target of the search enters or exits their view, along with what region they are looking in.
  - Parameters for this process include the allowable view angle delta for both head and eyes, as well as the time they must be looking in the correct region.
- Eye tracking is also used to drive a "Scotoma," which is the major focus of this experiment. *(Scotomas are partial or complete blind spots in vision).*
  - IS01 simulates a scotoma that follows the participant's eyes to observe how it might affect their search task effectiveness and what behavioral changes it might create.
  - This scotoma is implemented via a screen-space shader that is shifted and rotated on each eye's screen to create binocular fusion in the user's central vision.
  - The end effect is a black dot (or any customizable texture) that perfectly tracks the user's focus in real-time.
#### Scotoma Example, WARNING!: The GIF is significantly more lo-fi than the actual experiment and the scotoma looks more like the image below
<img width="1396" height="810" alt="Scotoma GIF" src="https://github.com/user-attachments/assets/ed84f6bf-6cb7-4b81-97e2-64540a133e1d" />
<img width="2448" height="2448" alt="Scotoma Med" src="https://github.com/user-attachments/assets/d893f0d7-43b0-4b9f-8e30-44cb7863e7f6" />

### Save System Usage
- The save system is used to capture all of the data in the experiment in JSON format.
- JSON is used because it helps keep the logical structure of the experiment intact and can help with the process of transforming the data in the future.
- A secondary program processes all experiment save data simultaneously. It converts raw JSON data that cannot be cleanly stored in a `.csv` format (such as head positions) into a generalized dataset.
  - Examples of this include what region the participant is looking at on the screen.
  - Head velocity.
  - Miss events and their related data.
- Eye-tracking data is also being saved as JSON but can, and should, easily be converted to `.csv` for further processing.
