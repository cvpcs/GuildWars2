﻿using System;
using System.Windows;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class PlayerPushpin : ImagePushpin
    {
        private static BitmapImage IMAGE = LoadImageResource("/Resources/player_position.png");

        public PlayerPushpin()
            : base()
        {
            Image = IMAGE;

            Width = 32;
            Height = 32;
        }
    }
}