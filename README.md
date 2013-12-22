OmniSharpServer
===============

HTTP wrapper around NRefactory allowing C# editor plugins to be written for any editor in any language.


This is the server component for the [Vim OmniSharp plugin](https://github.com/nosami/OmniSharp), [YouCompleteMe](https://github.com/Valloric/YouCompleteMe), [Sublime OmniSharp plugin](https://github.com/PaulCampbell/OmniSharpSublimePlugin)
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

	[mono] OmniSharp.exe -p (portnumber) -s (path\to\sln)
