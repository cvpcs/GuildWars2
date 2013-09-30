using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading;

namespace GuildWars2.ArenaNet.MumbleLink
{
    public class Program
    {
        private static readonly MumbleLink MUMBLE = new MumbleLink();
        private static readonly DataContractJsonSerializer SERIALIZER = new DataContractJsonSerializer(typeof(MumbleData));
        private static readonly HttpJsonServer JSON_SERVER = new HttpJsonServer(38139);

        public static void Main(string[] args)
        {
            JSON_SERVER.OnRequestReceived += GetJson;
            JSON_SERVER.Start();

            Console.WriteLine("Player position server is running. Press any key to exit this program.");
            Console.ReadKey();

            JSON_SERVER.Stop();
        }

        private static string GetJson()
        {
            string data = string.Empty;
            MumbleData linkData = new MumbleData();

            try
            {
                if (linkData.DataAvaliable = MUMBLE.DataAvailable)
                {
                    linkData.GameName = MUMBLE.GameName;
                    linkData.PlayerName = MUMBLE.PlayerName;
                    linkData.PlayerIsCommander = MUMBLE.PlayerIsCommander;
                    linkData.PlayerProfession = MUMBLE.PlayerProfession;
                    linkData.PlayerTeamColorId = MUMBLE.PlayerTeamColorId;
                    linkData.Server = MUMBLE.Server;
                    linkData.Map = MUMBLE.Map;
                    linkData.PositionX = MUMBLE.PositionX;
                    linkData.PositionY = MUMBLE.PositionY;
                    linkData.PositionZ = MUMBLE.PositionZ;
                    linkData.RotationPlayer = MUMBLE.RotationPlayer;
                    linkData.RotationCamera = MUMBLE.RotationCamera;
                }

                MemoryStream stream = new MemoryStream();
                SERIALIZER.WriteObject(stream, linkData);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                data = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return data;
        }
    }
}
