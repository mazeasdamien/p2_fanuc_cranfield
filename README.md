# Fanuc Remote Control Framework for Unity

## Overview

This repository contains a Unity application for remotely controlling a Fanuc robotic system. The communication between Unity and the Fanuc system is facilitated through the Data Distribution Service (DDS) protocol. The application provides various functionalities such as joint angle control, object movement, point cloud visualization, and reachability feedback.

## Repository Contents

- **DDSHandler.cs:**
  - Initializes and configures DDS components.
  - Sets up DataReaders and DataWriters for dynamic data types.
  - Disposes of DDS resources properly on application exit.
  - Loads Quality of Service (QoS) settings from an external XML file.

- **FanucManager.cs:**
  - Sets target angles for each joint of the Fanuc system based on DDS communication.
  - Updates ArticulationBody joint drives to synchronize with the received data.
  - Converts Fanuc Wrist Pitch Roll (WPR) angles to Unity-compatible quaternions.

- **MovetoManager.cs:**
  - Handles DDS communication for the "Moveto" action.
  - Utilizes a DragObject component for object dragging.
  - Sends DDS samples when the object is released.

- **PointCloudManager.cs:**
  - Manages DDS communication for point cloud visualization using Visual Effect Graph.
  - Processes DDS data to update the point cloud visualization.

- **ReachableManager.cs:**
  - Handles DDS communication for reachability information.
  - Updates visualization (material color) based on reachability.

- **TeleopManager.cs:**
  - Manages DDS communication for teleoperation.
  - Converts Unity Quaternion to Fanuc World Position Representation (WPR).
