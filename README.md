# WireForm

WireForm is a .NET Standard 2.0 library created to simulate logic gates and other digital circuitry through a simple graphical interface. Currently, the library has an implementation provided in Windows Forms (.NET Framework). An online Blazor implementation is also being worked on as a subset of SharpVM.

#### Please refer to the [Project Board](https://github.com/RyanAlameddine/WireForm/projects/1) for information on the current progress of the WireForm.

## Features
- Many simple logic gates such as AND, OR, XOR, NOT, etc.
- The ability to rotate gates for improved control of circuit board
- Wires with variable bitDepth (running multiple bits through a single wire)
- The ability to split or weave wires to and from different bitDepths using the special Splitter gate
- Gates with a dynamic input/output count
- Undo-Redo functionality
- Copy-Cut-Paste functionality
- Smart selection features which prohibit gates from overlapping and provide visual feedback of intersection and connection between gates and wires.
- If multiple objects are selected who share a property (like bitDepth, rotation, etc) all intersecting properties will be displayed and editable
- Other ease of use features such as additive selections (holding shift while selecting to add items), etc.

## Examples

 - Controls Sample
   <img src="https://i.imgur.com/9nEu6wQ.gif">
 - Full Adder:
   <img src="https://i.imgur.com/fdPFBw5.gif">
 - Splitter Example:
   <img src="https://i.imgur.com/XKX1Yov.gif">

## Installation

Currently, there's no published version of this project, but feel free to build the project on your own.

## Extension

Gates can be added by extending the abstract class Gate and registering your new class.
