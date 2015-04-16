#About the project:
NyangoroPresenter is a media and information center built for the main stage of NatsuCon 2014 (http://www.natsucon.cz/).

##Features:
- Modular architecture (easy addition of display/control panel extensions)
- Video, audio and image slideshow with BGM
- Custom messages for event visitors
- rotating display of event schedule
- external XAML styling and layout of presentation screen 

##Technical info:
- modular architecture based on .NET Managed Extensibility Framework
- Media streaming done using VideoLan LibVLC (with LibVLC.NET (https://libvlcnet.codeplex.com/) wrapper)


#Installation:
Working directory path can be set in the  Config.BuildConfig() located in Config.cs
Alternatively copy contents of /NyangoroPresenter/working_dir to your build output folder
