# Prior Scientific NPC-D-6330 C# Example
 A very simple example of using some of the native functions from controller_interface64.dll to send commands to a 3 channel Prior Scientific NanoScan PZT  controller over USB.

Uses the native C library controller_interface64.dll.

Not all DLL functions are implemented: Init(), OpenSession(), GetChannels(), DoCommand(), GetResult(), CloseSession() and Uninit()
are included as a demonstration of how to import them.

The full DLL function set can be found in "NPC-D Series Digital Controller Interface Library.pdf" from Prior

Commands to control the PZTs with DoCommand() can be found in "NPC-D-6xx0_NanoMechanism_Controller_Interface_Command_Set_And_Control_System_6.6.31.pdf"

An example of requesting motion status using DoCommand() and stage-moving.get is included below but different commands will require the response handling differently depending upon the data retrieved in the byte buffer returned from GetResult().
