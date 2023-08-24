#!/usr/bin/env bash

# This wrapper script is very simple right now and assumes that when it is
# invoked the current working directory is Muldis_Reference_Data_Tools_DotNet
# checkout base dir; this will be improved.

./Muldis_Data_Engine_Reference_App/bin/Debug/net7.0/Muldis_Data_Engine_Reference_App "$@"
