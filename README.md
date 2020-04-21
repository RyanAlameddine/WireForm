# WireForm

WireForm is a .NET Standard 2.0 library created to simulate logic gates and other digital circuitry through a simple graphical interface. Currently, the library has an implementation in Windows Forms (.NET Framework).

## Features
- Many simple logic gates such as AND, OR, XOR, NOT, etc.
- The ability to rotate gates for improved control of circuit board
- Wires with variable bitDepth (running multiple bits through a single wire)
- The ability to split or weave wires too and from different bitDepths using the special Splitter gate
- Undo-Redo functionality
- Copy-Cut-Paste functionality
- Other ease of use features such as additive selections (holding shift while selecting)

## Installation

Currently, there's no published version of this project, but feel free to build the project on your own.

## Extension

Gates can be added by extending the abstract class Gate and registering your new class.

## Tracking progress and future features

Please refer to the [Project](https://github.com/RyanAlameddine/WireForm/projects/1) for information on the current progress of the WireForm project.
