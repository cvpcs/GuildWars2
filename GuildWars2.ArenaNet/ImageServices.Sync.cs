using System;
using System.Net;

using GuildWars2.ArenaNet.Model.V1;

namespace GuildWars2.ArenaNet
{
    public static partial class ImageServices
    {
        public static byte[] WorldMapTileService(int continent, int floor, int x, int y, int z)
        {
            return DownloadFile(new Uri(new Uri(WorldMapTileServiceBaseURL), string.Format("/{0}/{1}/{2}/{3}/{4}.jpg",
                    continent, floor, z, x, y)));
        }

        public static byte[] RenderService(AssetFile file, RenderServiceFormat format = RenderServiceFormat.PNG)
        {
            return DownloadFile(new Uri(new Uri(RenderServiceBaseURL), string.Format("/file/{0}/{1}.{2}",
                    file.Signature, file.FileId, Enum.GetName(typeof(RenderServiceFormat), format).ToLower())));
        }

        private static byte[] DownloadFile(Uri uri)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadData(uri);
                }
                catch (Exception)
                { }
            }

            return null;
        }
    }
}
