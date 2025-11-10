# WPF Game of Life

A C# and WPF implementation of John Horton Conway's "Game of Life".

---

## 🎯 Features & Roadmap

This checklist tracks the planned features for the project.

### ⚙️ Core Simulation & Controls

* [x] **Custom Board Size:** Allow the user to define board dimensions (e.g., 100x100 or larger).
* [ ] **Manual Editing:** Allow manual toggling of cell states (alive/dead) when the simulation is paused.
* [x] **Clear and Randomize Board:** A function to populate the board with a random state.
* [ ] **Simulation Execution:**
    * [x] **Single Step:** Execute only one generation at a time.
    * [ ] **Continuous Animation:** Run the simulation automatically.
    * [ ] **Speed Control:** Include a slider or input to regulate the speed of the continuous animation.
    * [ ] **Pause & Edit:** Ability to pause the animation and return to the manual editing state.

---

### 💾 Rules & State Management

* [ ] **File I/O:**
    * [ ] **Save State:** Save the current state (board configuration + rules) to a text file.
    * [ ] **Load State:** Load a state (board + rules) from a text file.
* [x] **Customizable Rules:** Implement configurable rules using the **B/S (Born/Survives) notation**.
    * *Example: Conway's classic rules would be input as `B3/S23`.*
* [ ] **Pattern Library:** Allow the user to place pre-defined "interesting" patterns (like gliders, oscillators, etc.) onto the current board.

---

### 📊 UI/UX & Statistics

* [x] **Live Statistics:** Display running counters for:
    * [x] Generation count.
    * [x] Number of cells born in total.
    * [x] Number of cells that died in total.
* [x] **Smooth Zoom & Pan:**
    * [x] Implement smooth zooming.
    * [x] Ensure the *entire* board simulation continues to run ("live") even when zoomed in on a small fragment.
* [ ] **Visual Customization:** Provide simple options to change the presentation of cells (e.g., color and shape).
