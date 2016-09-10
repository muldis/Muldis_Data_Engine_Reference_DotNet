# NAME

Muldis.D.Ref\_Eng -
Reference Implementation of Muldis D Over C#

# VERSION

This document describes Muldis.D.Ref\_Eng version 0.

# DESCRIPTION

This distribution features the C\# library named ```Muldis.D.Ref_Eng``` and
its support structure, particularly the C\# console
application ```Muldis.D.Ref_Eng.Console```.

Muldis.D.Ref\_Eng is the reference implementation over C\# of the **Muldis
D** language by the authority Muldis Data Systems
([http://muldis.com](http://muldis.com)), version number ```0.201.0.-9```.

The Muldis D language specification is under active development and its
latest version can be seen at
[https://github.com/muldis/Muldis-D/](https://github.com/muldis/Muldis-D/).

The Muldis D language itself has as a primary influence the work of Chris
Date (C.J. Date) and Hugh Darwen whose home website is
[http://www.thethirdmanifesto.com/](http://www.thethirdmanifesto.com/).

*This documentation is pending.*

# DEPENDENCIES

Muldis.D.Ref\_Eng requires any version of C# that is at least 1.0 plus any
version of the .Net Core framework that is at least 1.0 plus the
appropriate runtime.  *(TODO: Check / flesh out the details of this.)*

See [https://www.microsoft.com/net/core](https://www.microsoft.com/net/core)
for instructions on how to install the dependencies.  These instructions
are further interpreted in the following sections.

In theory, Muldis.D.Ref\_Eng could also be made to work without too much
trouble on either older .Net Framework versions like 4.5 or on Mono,
however the author is using .Net Core canonically because it is a decent
modern cross-platform foundation.  Patches to support the others welcome!

It appears that .Net Framework 4.0 would be the practical minimum
dependency as that version is what introduced the BigInteger type we use.

## Installation of Dependencies Under Microsoft Windows

To build and run Muldis.D.Ref\_Eng on a Microsoft Windows OS, the following
procedure has been successfully used by the author.

1. MS Windows 8.1 was the foundation; other Windows versions weren't tried.

1. Download and install MS Visual Studio 2015 with Update 3, which is the
minimum required Visual Studio version, or a newer version.  To be
specific, ```en_visual_studio_enterprise_2015_with_update_3_x86_x64_dvd_8923288.iso```
was the installer used by the author.

1. Download and install the .Net Core tooling for VS 2015.  To be
specific, ```DotNetCore.1.0.0-VS2015Tools.Preview2.0.1.exe``` was the
installer used by the author.

1. Open the solution file ```Muldis.D.Ref_Eng.sln``` in Visual Studio.

1. Set ```Muldis.D.Ref_Eng.Console``` to be the start project.

There are various altneratives to the above instructions for those who are
savvy or prefer other development environments; Visual Studio is optional.

## Installation of Dependencies Under Apple Mac OS

To build and run Muldis.D.Ref\_Eng on an Apple Mac OS, the following
procedure has been successfully used by the author.

1. Mac OS 10.9.5 was the foundation; other Mac OS versions weren't tried.

2. Download and install the .Net Core SDK, which provides the command-line
tool ```dotnet``` for building and running the C#/.Net code.  To be
specific, ```dotnet-dev-osx-x64.1.0.0-preview2-003121.pkg``` was the
installer used by the author.

Microsoft also provides the free .Net Code IDE but it is not required, and
the author did not install it, preferring to use BBEdit instead.

To actually use the .Net Core SDK on the Mac OS. a newer version of OpenSSL
must be installed than what Apple bundles with the Mac OS.  For this, the
following procedure has been successfully used by the author, which also
involved installing the Homebrew package manager.

On the command-line, run the following:

        /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"

        brew update

        brew install openssl

        sudo install_name_tool -add_rpath /usr/local/opt/openssl/lib /usr/local/share/dotnet/shared/Microsoft.NETCore.App/1.0.0/System.Security.Cryptography.Native.dylib

Note that the last line differs from Microsoft's suggestion; it is more
cautious by not making a system global change, so only the .Net Core SDK is
setup to use the newly installed OpenSSL, and anything else that was using
Apple's system-provided OpenSSL continues to get that one.

To build/run Muldis.D.Ref\_Eng, at the command line, first cd
into ```src/Muldis.D.Ref_Eng.Console``` and then run ```dotnet run```.

## Installation of Dependencies Under Other Operating Systems

Microsoft's .Net Core has official support for various Linux flavors also,
but the author has not tried using Muldis.D.Ref\_Eng there and so has no
customized instructions to offer, but see the above Microsoft url.

# AUTHOR

Darren Duncan - darren@DarrenDuncan.net

# LICENSE AND COPYRIGHT

Muldis.D.Ref\_Eng is Copyright © 2015-2016, Muldis Data Systems, Inc.

[http://www.muldis.com/](http://www.muldis.com/)

Muldis.D.Ref\_Eng is free software; you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License (LGPL) as
published by the Free Software Foundation
([http://www.fsf.org/](http://www.fsf.org/)); either
version 3 of the License, or (at your option) any later version.  You
should have received a copy of the LGPL as part of the Muldis.D.Ref_Eng
distribution, in the files named "LICENSE/LGPL" and "LICENSE/GPL" (the
LGPLv3 is defined as the terms of the GPLv3 plus extra permissions); if
not, see [http://www.gnu.org/licenses/](http://www.gnu.org/licenses/).

If it is not feasible for you to employ Muldis.D.Ref\_Eng subject to the
terms of the LGPL, then the copyright holder of Muldis.D.Ref\_Eng can
provide you a customized proprietary license, often at no cost, so that it
is still possible for you to employ Muldis.D.Ref\_Eng to meet your needs.

Any versions of Muldis.D.Ref\_Eng that you modify and distribute must carry
prominent notices stating that you changed the files and the date of any
changes, in addition to preserving this original copyright notice and other
credits.  Muldis.D.Ref_Eng is distributed in the hope that it will be
useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

While it is by no means required, the copyright holder of Muldis.D.Ref\_Eng
would appreciate being informed any time you create a modified version of
Muldis.D.Ref\_Eng that you are willing to distribute, because that is a
practical way of suggesting improvements to the standard version.
