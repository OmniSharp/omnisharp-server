USER="ubuntu" #your username
HOME="/home/$USER"
INSTALL_DIR_OMNISHARPSERVER="$HOME"
OMNISHARP_WORKSPACE="$HOME/omnisharp_workspace"

#install mono in /opt/monodevelop
sudo add-apt-repository -y ppa:ermshiperete/monodevelop
sudo apt-get update
sudo apt-get -y install monodevelop-current

#Install Omnisharp
git -C $INSTALL_DIR_OMNISHARPSERVER clone https://github.com/nosami/OmniSharpServer.git
git -C $INSTALL_DIR_OMNISHARPSERVER/OmniSharpServer submodule update --init --recursive
/opt/monodevelop/bin/xbuild $INSTALL_DIR_OMNISHARPSERVER/OmniSharpServer/OmniSharp.sln

#make csharp workspace
#mkdir $OMNISHARP_WORKSPACE
#mkdir $OMNISHARP_WORKSPACE/csharp

#start the Omnisharp server on the created workspace
#-p is for port number
#-s is for the workspace directory name
#/opt/monodevelop/bin/mono $INSTALL_DIR_OMNISHARPSERVER/OmniSharpServer/OmniSharp/bin/Debug/OmniSharp.exe -v Verbose -s $OMNISHARP_WORKSPACE/csharp -p 2000
