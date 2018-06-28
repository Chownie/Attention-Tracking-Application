# Attention Tracking Application

This is an application for tracking the effects binocular rivalry on a user's attention.

---

# How to use

For each trial, launch the application on the device and repeat the steps below.

**Configuration Steps**:
1. Position the cursor on each control (button, textfield etc) by rotating the device.
2. Once aligned, tap to engage the respective control
3. Use the same name for every trial with the same participant. The log files will have the trial condition appended, no files will be overwritten.
4. Once the trial is over close the application.

---

# Log files

Files are logged to the available local user storage. On a windows machine this is often ```%APPDATA%\..\LocalLow\DefaultCompany\Attention Study````

On the device it was trialed upon originally this is ```/Android/data/com.AsaDawson.AttentionStudy/files```

The log files are split in two, a file labelled \_ACTIONS and a file labelled \_GAZE, these files describe events such as button presses and the user's head movements respectively.

## \_ACTION file format

The \_ACTION file will contain a csv list of events. Types are listed below, note that every line finishes with the current millisecond count since the application started.

**Example file**:

```
CONDITION,Hypercolor,43.999267578125
STARTED,53.49609375
LOCKED,4024.22094726563
NEW TARGET,0,22.07284,1.67807,6031.24487304688
BUTTON PRESSED,6586.18969726563
LOCKED,9624.890625
FINISHED,29742.7490234375
```

**Format explained**:

| State          |                                           |                          |                          |           |
|----------------|-------------------------------------------|--------------------------|--------------------------|-----------|
| CONDITION      | (Hypercolour, Transparency, Size, Colour) | Time (ms)                |                          |           |
| STARTED        | Time (ms)                                 |                          |                          |           |
| LOCKED         | Time (ms)                                 |                          |                          |           |
| NEW TARGET     | Number of nodes skipped to gain this node | X coordinate on the grid | Y Coordinate on the grid | Time (ms) |
| BUTTON PRESSED | Time (ms)                                 |                          |                          |           |
| LOCKED         | Time (ms)                                 |                          |                          |           |
| FINISHED       | Time (ms)                                 |                          |                          |           |

## \_GAZE file format

**Example gaze log table**:

| X Position | Y Position | Head rotation angles (X, Y, Z) | Time (ms)      |
|------------|------------|--------------------------------|----------------|
| 21.43213   | 2.407375   | (5.9, 64.3, 0.0)               | 7046.345703125 |
