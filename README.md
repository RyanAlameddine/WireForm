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

## Library Usage

Both the WireForm and the WireFormInput libraries are available in the NuGet packages section above.

If you would like to use the back-end wire and gate simulation framework WireForm in your own project, feel free to check out [this link](https://github.com/RyanAlameddine/WireForm/packages/578229). This library supports the ability for you to define gates, wires, and connections as well as create your own custom gates and experiment with propogating electricity through each circuit. 

If you would like to also use the built in input system defined for wireform, use [WireFormInput](https://github.com/RyanAlameddine/WireForm/packages/578231) as well. This library was specifically designed to be a separate layer from the drawing and circuitry layers, and thus you are welcome to provide any front-end you want. However, WireFormInput provides key functionality transforming key presses and clicks to changes in circuit tool states and easy access to things like circuit properties and circuit actions.

## Installation

Currently, there's no published version of the front-end project, but feel free to build the project on your own.

## Extension

Both Wireform (the base library which handles all the circuitry and electricity) and WireformInput (the layer which handles all the user input, dropdowns, hotkeys, and changes in the circuitboard) are available as NuGet packages on this repository.

Gates can be added by extending the abstract class Gate and registering your new class.
