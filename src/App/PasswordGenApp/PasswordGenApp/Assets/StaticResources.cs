using System;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace PasswordGenApp.Assets;

public static class StaticResources
{
    public static string RemovePictureResourceLink { get; } = "avares://PasswordGenApp/Assets/remove.png";

    public static Bitmap? RemovePictureImage
    {
        get
        {
            var exist = AssetLoader.Exists(new Uri(RemovePictureResourceLink));
            if (!exist)
                return null;
            var imageStream = new MemoryStream();
            AssetLoader.Open(new Uri(RemovePictureResourceLink)).CopyTo(imageStream);
            imageStream.Position = 0;
            return new Bitmap(imageStream);
        }
    }
    
    public static string InfoPictureResourceLink { get; } = "avares://PasswordGenApp/Assets/info.png";

    public static Bitmap? InfoPictureImage
    {
        get
        {
            var exist = AssetLoader.Exists(new Uri(InfoPictureResourceLink));
            if (!exist)
                return null;
            var imageStream = new MemoryStream();
            AssetLoader.Open(new Uri(InfoPictureResourceLink)).CopyTo(imageStream);
            imageStream.Position = 0;
            return new Bitmap(imageStream);
        }
    }
    
}