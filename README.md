# NAME

Muldis Reference Engine: .NET (MRE) -
Reference Implementation of Muldis Data Language Over .NET

# VERSION

The fully-qualified name of what this document describes is
`Muldis_Reference_Engine_DotNet http://muldis.com 0.0.0`.

# DESCRIPTION

This distribution features the C\# library named `Muldis.ReferenceEngine` and
its support structure, particularly the C\# console
application `Muldis.ReferenceEngine.Console`.

**Muldis Reference Engine: .NET** is the reference implementation over C\# of
**Muldis Data Language** by the authority Muldis Data Systems
([http://muldis.com](http://muldis.com)), version number `0.201.0`.

The Muldis Data Language specification is under active development and its
latest version can be seen at
[https://github.com/muldis/Muldis_Data_Language](https://github.com/muldis/Muldis_Data_Language).

Muldis Data Language itself has as a primary influence the work of Chris
Date (C.J. Date) and Hugh Darwen whose home website is
[http://www.thethirdmanifesto.com/](http://www.thethirdmanifesto.com/).

This distribution bundles a non-canonical copy of the library
named `Muldis.DatabaseProtocol` / `MuldisDatabaseProtocol.cs` for build-out-of-the-box
convenience in the short term while the latter is not yet published as a
NuGet package; see [https://github.com/muldis/Muldis_Database_Protocol](
https://github.com/muldis/Muldis_Database_Protocol) for the canonical copy.

*This documentation is pending.*

# DEPENDENCIES AND COMPATIBILITY (NEW VERSION)

**Muldis Reference Engine: .NET** is defined in terms of Microsoft's .NET
"dot net" developer platform.

[https://www.microsoft.com/net](https://www.microsoft.com/net)

**Muldis Reference Engine: .NET** officially requires the .NET Standard
version 2.0 API (2017).

[https://docs.microsoft.com/en-us/dotnet/standard/net-standard](
https://docs.microsoft.com/en-us/dotnet/standard/net-standard)

The .NET Standard version 2.0 API is provided by the following .NET
implementations: .NET Core versions 2.0 (2017) and above, .NET Framework
(with .NET Core 2.0 SDK) versions 4.6.1 (2015) and above, Mono versions 5.4
(2017) and above, and others listed at the above url.

Between the various .NET Standard version 2.0 implementations, there is
coverage for all the major operating systems, including Microsoft Windows,
Apple MacOS, and a variety of Linux flavors.  As such, **Muldis Reference
Engine: .NET** is officially cross-platform and should run on anywhere that
at least one .NET Standard version 2.0 implementation does.

**Muldis Reference Engine: .NET** unofficially is also very likely to work
with older .NET implementations, with .NET Framework version 4.0 (2010)
being more the hard minimum (versions less than 4.0 are known to lack
required features), but the author does not formally test with these.

The newest .NET implementations provided by Microsoft for each major
operating system can be downloaded here, including the Visual Studio IDE
and the .NET Core SDK:

[https://www.microsoft.com/net/download](https://www.microsoft.com/net/download)

On Microsoft Windows, MS Visual Studio 2017 version 15.4.0 (2017 Oct 9) was
the first version to include stable .NET Standard 2.0 and .NET Core 2.0 by
default and so is the earliest recommended version to use.  Unofficially
any version back to MS Visual Studio 2010 inclusive would probably handle
the C\# source files.  As of this writing, MS Visual Studio 2017 version
15.8.5 (2018 Sep 20) is the newest stable version released.

On Apple MacOS, MS Visual Studio 2017 for Mac version 7.2.0 (2017 Oct 9)
was the first version to include stable .NET Standard 2.0 and .NET Core 2.0
by default and so is the earliest recommended version to use.  As of this
writing, MS Visual Studio 2017 for Mac version 7.6.7 (2018 Sep 25) is the
newest stable version released.

# DEPENDENCIES AND COMPATIBILITY (OLD VERSION, TO BE REMOVED)

**Muldis Reference Engine: .NET** is intended to be published as a NuGet package in the
near future, and that is intended to be the way that "normal" users consume
the library.  Until then, all users consume it as `*.cs` source files
that they include in and compile with their own projects.

**Muldis Reference Engine: .NET** is defined in terms of Microsoft's .NET API and is
intended to be portable across a wide range of implementations of that API.

The set of C\# source files comprising **Muldis Reference Engine: .NET**, when taken in
isolation, should work as-is with all of:  .NET Framework versions 4.0
(2010) and above (versions less than 4.0 are known to lack required
features), .NET Core versions 1.0 (2016) and above, .NET Standard versions
1.0 (2016) and above, Mono versions 2.8 (2010) and above, anything else
listed at [https://docs.microsoft.com/en-us/dotnet/standard/library](
https://docs.microsoft.com/en-us/dotnet/standard/library) as supported
platforms for .NET Standard versions 1.0 and above, and also a variety of
older Portable Class Library profiles pre-dating .NET Standard.

Between the above options, particularly .NET Core, **Muldis Reference Engine: .NET** is
cross-platform and should be immediately usable on Microsoft Windows or
Apple MacOS or various Linux flavors or other operating systems, either
desktop computers or mobile devices etc.

The C\# source files explicitly declare that they are `CLSCompliant`
and so should be useable with code written in any language that compiles to
Microsoft's CLR, including both C\# and Visual Basic.

That all being said, however, the solution/project metadata profiles that
the canonical **Muldis Reference Engine: .NET** GitHub repository includes are of a modern
format that only tooling from early 2017 understands how to read.  If you
have the 2017-plus-era tooling then you can use the whole of the given
**Muldis Reference Engine: .NET** directly, widely cross-platform; otherwise you would need
to take all the C\# files and use your own solution/project metadata files.

See [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
for instructions on how to install the dependencies.  These instructions
are further interpreted in the following sections.

## Installation of Dependencies Under Microsoft Windows

To build and run **Muldis Reference Engine: .NET** on a Microsoft Windows OS, the best
solution is to download and install MS Visual Studio 2017, which is the
first version with the tooling to understand the `Muldis.ReferenceEngine.sln`
and `*.csproj` etc that **Muldis Reference Engine: .NET** comes with.

The alternate best solution is to download and install Microsoft's .NET
Core SDK, a command-line development tool which should understand the same
solution/project files.

Otherwise, any version of MS Visual Studio back to MS Visual Studio 2010
should in theory handle the `*.cs` files with your own Visual Studio
solution and project files.

Set `Muldis.ReferenceEngine.Console` as the start project if you want to use
this standalone Muldis Data Language source file executor.

## Installation of Dependencies Under Apple Mac OS

To build and run **Muldis Reference Engine: .NET** on an Apple Mac OS, the canonical
solution is to download and install Microsoft's .NET Core SDK, a
command-line tool that understands the `Muldis.ReferenceEngine.sln`
and `*.csproj` etc that **Muldis Reference Engine: .NET** comes with.

The following procedure has been successfully used by the author.

1. Mac OS 10.12.5 was the foundation for the specific .NET Core installer
indicated next; other Mac OS versions weren't tried.

2. Download and install the .NET Core SDK, which provides the command-line
tool `dotnet` for building and running the .NET code.  To be
specific, `dotnet-dev-osx-x64.1.0.4.pkg` was the
installer used by the author.

NOTE: THE FOLLOWING SECTION MIGHT ACTUALLY BE OBSOLETE.  IT WAS NEEDED FOR
THE PREVIEW 2 OF .NET CORE BUT VERSION 1.0.4 RAN WITHOUT DOING THE LAST STEP.

To actually use the .NET Core SDK on the Mac OS a newer version of OpenSSL
must be installed than what Apple bundles with the Mac OS.  For this, the
following procedure has been successfully used by the author, which also
involved installing the Homebrew package manager.

On the command-line, run the following:

        /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"

        brew update

        brew install openssl

        sudo install_name_tool -add_rpath /usr/local/opt/openssl/lib /usr/local/share/dotnet/shared/Microsoft.NETCore.App/1.0.0/System.Security.Cryptography.Native.dylib

Note that the last line differs from Microsoft's suggestion; it is more
cautious by not making a system global change, so only the .NET Core SDK is
setup to use the newly installed OpenSSL, and anything else that was using
Apple's system-provided OpenSSL continues to get that one.

END OF THE POSSIBLY OBSOLETE SECTION.

Running `dotnet help` is quite helpful in general.

To prepare to run **Muldis Reference Engine: .NET**, at the command line, first cd
into its root directory and then run `dotnet restore`.  This causes
NuGet to fetch and make available the core .NET libraries, without which
attempting to run a program gets errors like `System` can't be found.

To build/run **Muldis Reference Engine: .NET**, at the command line, first cd
into `src/Muldis.ReferenceEngine.Console` and then run `dotnet run`.

Another good solution is to use Microsoft's free Visual Studio Community
for Mac which provides a GUI and debugger and is otherwise a lot like the
Visual Studio for MS Windows.  It also understands the `Muldis.ReferenceEngine.sln`
and `*.csproj` etc that **Muldis Reference Engine: .NET** comes with.  See
[https://www.visualstudio.com/downloads/](https://www.visualstudio.com/downloads/).

## Installation of Dependencies Under Other Operating Systems

Microsoft's .NET Core has official support for various Linux flavors also,
but the author has not tried using **Muldis Reference Engine: .NET** there and so has no
customized instructions to offer, but see the above Microsoft .NET Core url.

# AUTHOR

Darren Duncan - darren@DarrenDuncan.net

# LICENSE AND COPYRIGHT

**Muldis Reference Engine: .NET** is Copyright © 2015-2018, Muldis Data Systems, Inc.

[http://www.muldis.com/](http://www.muldis.com/)

**Muldis Reference Engine: .NET** is free software; you can redistribute it and/or modify
it under the terms of the Artistic License version 2 (AL2) as published by
the Perl Foundation ([http://www.perlfoundation.org/](
http://www.perlfoundation.org/)).  You should have received a copy of the
AL2 as part of the **Muldis Reference Engine: .NET** distribution, in the file named
"LICENSE/artistic-2\_0.txt"; if not, see
[http://www.perlfoundation.org/attachment/legal/artistic-2\_0.txt](
http://www.perlfoundation.org/attachment/legal/artistic-2\_0.txt).

Any versions of **Muldis Reference Engine: .NET** that you modify and distribute must carry
prominent notices stating that you changed the files and the date of any
changes, in addition to preserving this original copyright notice and other
credits.  **Muldis Reference Engine: .NET** is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

While it is by no means required, the copyright holder of **Muldis Reference Engine: .NET**
would appreciate being informed any time you create a modified version of
**Muldis Reference Engine: .NET** that you are willing to distribute, because that is a
practical way of suggesting improvements to the standard version.
