using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;

using GuildWars2.ArenaNet.API;
using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FilesResponse fr = new FilesRequest().Execute();

            ItemDetailsResponse idr = new ItemDetailsRequest(44977).Execute();

            byte[] img1 = ImageServices.WorldMapTileService(1, 2, 1, 1, 1);
            byte[] img2 = ImageServices.RenderService(idr.IconFile);
            byte[] img3 = ImageServices.RenderService(fr.MapWaypointContested, ImageServices.RenderServiceFormat.JPG);

            BinaryWriter bw = new BinaryWriter(new FileStream(@"D:\tmp\img1.jpg", FileMode.Create));
            bw.Write(img1);
            bw.Close();
            bw = new BinaryWriter(new FileStream(@"D:\tmp\img2.png", FileMode.Create));
            bw.Write(img2);
            bw.Close();
            bw = new BinaryWriter(new FileStream(@"D:\tmp\img3.jpg", FileMode.Create));
            bw.Write(img3);
            bw.Close();
        }
    }
}
