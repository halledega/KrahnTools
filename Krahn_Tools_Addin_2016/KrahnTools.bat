@ ECHO OFF
TITLE Setting up KrahnTools
ECHO Setting up KrahnTools addin for Revit 2016...
ROBOCOPY %~dp0 C:\ProgramData\Autodesk\Revit\Addins\2016 /xf *.bat /S 
ECHO KrahnTools adin has been successfully copied to C:\ProgramData\Autodesk\Revit\Addins\2016.
ECHO Save and restart Revit (if open) to activate add on.
PAUSE

