#!/usr/bin/env bash

# This wrapper script is very simple right now and assumes that when it is
# invoked the current working directory is Muldis_Reference_Data_Tools_DotNet
# checkout base dir; this will be improved.

./Muldis_Object_Notation_Processor_Reference/Muldis.Object_Notation_Processor_Reference_App/bin/Debug/net7.0/Muldis.Object_Notation_Processor_Reference_App "$@"
