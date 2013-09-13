using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

using GuildWars2.ArenaNet.Model;
using GuildWars2.SyntaxError.Model;

namespace GuildWars2.ArenaNet.Mapper
{
    public class PointOfInterestPushpin : ImagePushpin
    {
        private static IDictionary<PointOfInterestType, BitmapImage> IMAGES = new Dictionary<PointOfInterestType, BitmapImage>() {
                { PointOfInterestType.Landmark, ResourceUtility.LoadBitmapImage("/Resources/poi.png") },
                { PointOfInterestType.Vista, ResourceUtility.LoadBitmapImage("/Resources/vista.png") },
                { PointOfInterestType.Waypoint, ResourceUtility.LoadBitmapImage("/Resources/waypoint.png") }
            };

        public PointOfInterestPushpin(PointOfInterest poi)
            : base()
        {
            if (IMAGES.ContainsKey(poi.TypeEnum))
                Image = IMAGES[poi.TypeEnum];

            if (!string.IsNullOrWhiteSpace(poi.Name))
            {
                ToolTip = poi.Name;

                PopupContent = new PopupContentFactory()
                        .AppendWikiLink(poi.Name)
                        .AppendChatCode(ChatCode.CreateMapLink((uint)poi.PoiId))
                        .Content;
            }
        }
    }
}
