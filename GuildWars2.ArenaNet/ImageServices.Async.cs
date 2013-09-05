using System;
using System.IO;
using System.Net;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet
{
    public static partial class ImageServices
    {
        public static void WorldMapTileServiceAsync(Action<byte[]> callback, int continent, int floor, int x, int y, int z)
        {
            DownloadFileAsync(callback, new Uri(new Uri(WorldMapTileServiceBaseURL), string.Format("/{0}/{1}/{2}/{3}/{4}.jpg",
                    continent, floor, z, x, y)));
        }

        public static void RenderServiceAsync(Action<byte[]> callback, AssetFile file, RenderServiceFormat format = RenderServiceFormat.PNG)
        {
            DownloadFileAsync(callback, new Uri(new Uri(RenderServiceBaseURL), string.Format("/file/{0}/{1}.{2}",
                    file.Signature, file.FileId, Enum.GetName(typeof(RenderServiceFormat), format).ToLower())));
        }

        private static void DownloadFileAsync(Action<byte[]> callback, Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.BeginGetResponse(asyncResult => {
                    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[32768];

                            using (Stream rs = response.GetResponseStream())
                            {
                                int bytesRead;
                                while ((bytesRead = rs.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, bytesRead);
                            }

                            callback(ms.ToArray());
                        }
                    }
                    else
                    {
                        callback(null);
                    }
                }, request);
        }
    }
}
