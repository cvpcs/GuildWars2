using System;
using System.Runtime.Serialization;

namespace GuildWars2.ArenaNet.MumbleLink
{
    [DataContract]
    public class MumbleData
    {
        [DataMember(Name = "data_available")]
        public bool DataAvaliable { get; set; }

        [DataMember(Name = "game_name")]
        public string GameName { get; set; }

        [DataMember(Name = "player_name")]
        public string PlayerName { get; set; }

        [DataMember(Name = "player_is_commander")]
        public bool PlayerIsCommander { get; set; }

        [DataMember(Name = "player_team_color_id")]
        public int PlayerTeamColorId { get; set; }

        [DataMember(Name = "server")]
        public int Server { get; set; }

        [DataMember(Name = "map")]
        public int Map { get; set; }

        [DataMember(Name = "pos_x")]
        public float PositionX { get; set; }

        [DataMember(Name = "pos_y")]
        public float PositionY { get; set; }

        [DataMember(Name = "pos_z")]
        public float PositionZ { get; set; }

        [DataMember(Name = "rot_player")]
        public int RotationPlayer { get; set; }

        [DataMember(Name = "rot_camera")]
        public int RotationCamera { get; set; }
    }
}
