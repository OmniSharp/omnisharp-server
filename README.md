OmniSharpServer
===============

[![Build Status](https://travis-ci.org/nosami/OmniSharpServer.png?branch=master)](https://travis-ci.org/nosami/OmniSharpServer)

HTTP wrapper around NRefactory allowing C# editor plugins to be written for any editor in any language.


This is the server component for the [Vim OmniSharp plugin](https://github.com/nosami/OmniSharp), [YouCompleteMe](https://github.com/Valloric/YouCompleteMe), [Sublime Text 2 OmniSharp plugin](https://github.com/PaulCampbell/OmniSharpSublimePlugin), [Sublime Text 3](https://github.com/n-yoda/OmniSharpSublime)
and [Emacs OmniSharp plugin](https://github.com/sp3ctum/omnisharp-emacs).


#Build

####OSX / Linux
    git clone https://github.com/nosami/OmniSharpServer.git
    cd OmniSharpServer
    xbuild /p:Platform="Any CPU"

####Windows
    git clone https://github.com/nosami/OmniSharpServer.git
    cd OmniSharpServer
    msbuild /p:Platform="Any CPU"
    

To start the Omnisharp server manually (The Vim OmniSharp client and YouCompleteMe may start this for you automatically):

	[mono] OmniSharp.exe -s (path\to\sln)


[![Bitdeli Badge](https://d2weczhvl823v0.cloudfront.net/nosami/omnisharpserver/trend.png)](https://bitdeli.com/free "Bitdeli Badge")

