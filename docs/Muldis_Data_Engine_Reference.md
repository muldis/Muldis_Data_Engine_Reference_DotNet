# NAME

Muldis Data Engine Reference: .NET (MDE) - Reference interpreter implementation of Muldis Data Language

# VERSION

The fully-qualified name of what this document describes is
`Muldis_Data_Engine_Reference_DotNet https://muldis.com 0.1`.

# DESCRIPTION

MDE is a work in progress.
It is not yet ready to use for its intended purpose, nor is it yet even a
minimum viable product.

Further significant work on MDE is blocked pending the prior minimum viable
product completion of its dependency-to-be project:

[Muldis Object Notation Processor Reference: .NET (MUONP)](../Muldis_Object_Notation_Processor_Reference) - Reference implementation of MUON processing utilities

This documentation will be rewritten or updated as the project develops.

## What This Is Intended To Be

MDE is fundamentally a library that provides an embedded virtual machine
implementation of the Muldis Data Language which is also an embedded
database engine and an embedded type system for the host language.

The standard formats for moving data and code between the virtual machine
and its host are Muldis Object Notation (MUON), either the string-based
Plain Text Format or the .NET data structure based format.

A utility application is also provided that reads in a Muldis Data Language
"main program" in the form of a MUON text file and passes it to the virtual
machine to compile and execute, essentially being a runtime of a form that
is typical of scripting languages.  The program to run is specified with a
command-line argument.

MDE is expressly designed to have no dependencies besides a modern
version of the core C# language and .NET standard library,
and be a low-overhead leaf dependency for other projects.
Since modern .NET is cross-platform, MDE should work on any
operating system that it supports.

## What This Is Today

A rudimentary version of the MDE virtual machine library is implemented
now, and one can import and export data and execute some Muldis Data
Language routines on that data.  Import/export only works currently in
terms of an outdated form of the Muldis Object Notation .NET hosted format,
and the MUON Plain Text format is not yet implemented for importing.

A utility application is also implemented which performs some rudimentary
testing on the virtual machine, trying to use its API, importing some data,
and trying to invoke a Muldis Data Language routine or two.

That utility application prints out to the command-line
quasi-MUON-Plain-Text serializations of the imported data, so that it
actually looks like it is doing something.

The utility application also reads in a MUON source file as if it was going
to run it, but since parsing these is not yet implemented, that part no-ops.

## How To Use It

In order to run the MDE utility application, you first have to compile
it, which can be done in the usual manner using Microsoft's Visual Studio,
which can be obtained here (with no-cost Community licensing an option):

<https://visualstudio.microsoft.com>

MDE is implemented with .NET 8 (2023) which is cross-platform so you can
use either the Apple MacOS or Microsoft Windows version of Visual Studio.
I have done all development and testing so far on MacOS however.

Microsoft also provides command-line alternative compilers and runtimes
specific to modern .NET versions, instead of needing a full Visual Studio:

<https://dotnet.microsoft.com>

You would want to have a modern Microsoft `dotnet` tool
version on your system if you want to build it on the command line:

```
dotnet clean ; dotnet build
```

Once the MDE .NET project has been compiled to a native executable, you
can run it on the command line, such as MacOS Terminal or Windows Console.

A thin wrapper UNIX shell script has been provided that can be used once
the project is compiled.

Here is an example of its use:

```
sh muldisder.sh Muldis.Data_Engine_Reference.MuseEntrance,Muldis_Data_Engine_Reference corpora/Syntax_Plain_Text/spec_all_examples.muon
```

I have not yet produced an alternate thin wrapper Windows shell script,
but that is intended to be done soon.

# AUTHOR

Darren Duncan - darren@DarrenDuncan.net

# LICENSE AND COPYRIGHT

**Muldis Data Engine Reference: .NET** (MDE) is Copyright © 2015-2023, Muldis Data Systems, Inc.

<https://muldis.com>

MDE is free software;
you can redistribute it and/or modify it under the terms of the Apache
License, Version 2.0 (AL2) as published by the Apache Software Foundation
(<https://www.apache.org>).  You should have received a copy of the
AL2 as part of the MDE distribution, in the file
[LICENSE/Apache-2.0.txt](../LICENSE/Apache-2.0.txt); if not, see
<https://www.apache.org/licenses/LICENSE-2.0>.

Any versions of MDE that you modify and distribute must carry prominent
notices stating that you changed the files and the date of any changes, in
addition to preserving this original copyright notice and other credits.
MDE is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

While it is by no means required, the copyright holder of MDE
would appreciate being informed any time you create a modified version of
MDE that you are willing to distribute, because that is a
practical way of suggesting improvements to the standard version.
