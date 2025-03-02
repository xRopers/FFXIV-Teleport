# FFXIV-Teleport

**FFXIV-Teleport** is a tool designed for teleportation and positional management within Final Fantasy XIV. This project demonstrates programming techniques for working with game memory and showcases the use of overlays and external configuration.

> **Disclaimer:** This project is for **educational purposes only**. Using this tool may violate the Final Fantasy XIV Terms of Service and could result in consequences such as bans. Use at your own risk.

---

## Features

- Save and load positions in-game.
- Overlays for teleportation management.
- Modular structure for extensibility.

---

## Getting Started

### Prerequisites

- **.NET SDK**: Install [.NET SDK](https://dotnet.microsoft.com/download) to build the project.
- **FFXIV Game Client**: The tool interacts with `ffxiv_dx11`.

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/[your-username]/FFXIV-Teleport.git
   cd FFXIV-Teleport

2. Open the solution in Visual Studio or your preferred IDE.

3. Build the project: dotnet build

### Usage

1. Launch FFXIV game client and then launch FFXIV-Teleport.

2. Use the overlay to:
   Save your current position.
   Teleport to a saved position.
   Manage saved points.


### Development

File Structure

   1. ``GameMemoryConfig.cs``: Manages memory offsets.
   2. ``TeleportFunctions.cs``: Handles teleportation and position manipulation.
   3. ``SavePointManager.cs``: Manages save point data and serialization.
   4. ``Program.cs``: The main entry point for the application.

### Contribution

We welcome contributions! Please follow these steps:

Fork the repository.
   .Create a new branch: ``git checkout -b feature-branch``
   .Submit a pull request for review.

### License
   This project is licensed under the GNU General Public License v3.0. See the LICENSE file for details.

### Disclaimer
   This tool is intended for educational purposes only. Use of this tool may violate Final Fantasy XIV's Terms of Service, and the developers are not responsible for any consequences resulting from its use.