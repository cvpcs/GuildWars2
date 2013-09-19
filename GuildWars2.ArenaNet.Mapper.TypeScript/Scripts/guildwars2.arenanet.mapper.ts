/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/leaflet/leaflet.d.ts" />
/// <reference path="typings/guildwars2.arenanet/guildwars2.arenanet.d.ts" />
/// <reference path="typings/guildwars2.syntaxerror/guildwars2.syntaxerror.d.ts" />

module GuildWars2.ArenaNet.Mapper {
    export class ArenaNetMap extends L.Map {
        private static Instances: { [key: string]: ArenaNetMap } = { };

        constructor(id: string) {
            super(id, {
                minZoom: 0,
                maxZoom: 7,
                crs: L.CRS.Simple
            });

            ArenaNetMap.Instances[id] = this;

            super.setView(new L.LatLng(0, 0), 0);

            new L.TileLayer("https://tiles.guildwars2.com/1/1/{z}/{x}/{y}.jpg", {
                minZoom: super.getMinZoom(),
                maxZoom: super.getMaxZoom(),
                continuousWorld: true
            }).addTo(this);

            $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (response: GuildWars2.ArenaNet.Model.MapFloorResponse): void {
                ArenaNetMap.LoadFloorData(id, response);
            });

            $.get("https://api.guildwars2.com/v1/event_details.json", function (response: GuildWars2.ArenaNet.Model.EventDetailsResponse): void {
                ArenaNetMap.LoadEventData(id, response.events);
            });
        }

        private static LoadFloorData(id: string, floor: GuildWars2.ArenaNet.Model.Floor): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var anetmap = ArenaNetMap.Instances[id];

            anetmap.setMaxBounds(new L.LatLngBounds(
                    anetmap.unproject(new L.Point(0 - (floor.texture_dims[1] * 0.5), floor.texture_dims[1] * 1.5), anetmap.getMaxZoom()),
                    anetmap.unproject(new L.Point(floor.texture_dims[0] * 1.5, 0 - (floor.texture_dims[0] * 0.5)), anetmap.getMaxZoom())
                ));

            var poiGroup: L.LayerGroup = new L.LayerGroup();
            poiGroup.addTo(anetmap);

            for (var i in floor.regions) {
                var region = floor.regions[i];
                for (var j in region.maps) {
                    var map = region.maps[j];

                    for (var k in map.points_of_interest) {
                        var poi = map.points_of_interest[k];
                        var marker = new PointOfInterestMarker(anetmap.unproject(new L.Point(poi.coord[0], poi.coord[1]), anetmap.getMaxZoom()), poi);
                        poiGroup.addLayer(marker);
                    }
                }
            }
        }

        private static LoadEventData(id: string, events: { [key: string]: GuildWars2.ArenaNet.Model.EventDetails }): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var anetmap = ArenaNetMap.Instances[id];
        }
    }

    class ArenaNetMapLayerContainer extends L.LayerGroup {
        constructor() {
            super();
        }
    }

    class PointOfInterestMarker extends L.Marker {
        private static Icons: { [key: string]: L.Icon } = {
            "landmark": new L.Icon({ iconUrl: "Resources/poi.png" }),
            "vista": new L.Icon({ iconUrl: "Resources/vista.png" }),
            "waypoint": new L.Icon({ iconUrl: "Resources/waypoint.png" })
        };

        constructor(latlng: L.LatLng, poi: GuildWars2.ArenaNet.Model.PointOfInterest) {
            super(latlng, { title: (poi.name != "" ? poi.name : undefined) });

            if (PointOfInterestMarker.Icons[poi.type] != undefined)
                super.setIcon(PointOfInterestMarker.Icons[poi.type]);

            if (poi.name != "") {
                super.bindPopup(new PopupContentFactory()
                    .appendWikiLink(poi.name)
                    .getContent());
            }
        }
    }

    class PopupContentFactory {
        private static DulfyBountyLinks: { [key: string]: string } =
            {
                "2-MULT": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#0",
                "Ander \"Wildman\" Westward": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#1",
                "Big Mayana": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#1b",
                "Bookworm Bwikki": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#2",
                "Brekkabek": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#3",
                "Crusader Michiele": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#4",
                "\"Deputy\" Brooke": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#5",
                "Devious Teesa": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#6",
                "Diplomat Tarban": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#7",
                "Half-Baked Komali": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#8",
                "Poobadoo": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#9",
                "Prisoner 1141": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#10",
                "Shaman Arderus": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#11",
                "Short-Fuse Felix": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#12",
                "Sotzz the Scallywag": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#13",
                "Tricksy Trekksa": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#14",
                "Trillia Midwell": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#15",
                "Yanonka the Rat-Wrangler": "http://dulfy.net/2013/02/27/gw2-guild-bounty-guide/#16"
            };

        private lines: string[];

        constructor() {
            this.lines = new Array< string >();
        }

        public appendLink(label: string, text: string, uri: string, target?: string): PopupContentFactory {
            if (target == undefined) target = "_blank";
            this.lines.push("<p>" + label + ": <a href=\"" + uri + "\" target=\"" + target + "\">" + text + "</a></p>");
            return this;
        }

        public appendWikiLink(article: string): PopupContentFactory {
            var articleTrimmed: string;

            switch (article.charAt(article.length - 1)) {
                case "!":
                case "?":
                case ".":
                    articleTrimmed = article.substr(0, article.length - 1);
                    break;
                default:
                    articleTrimmed = article;
            }

            this.appendLink("Wiki page", articleTrimmed, "http://wiki.guildwars2.com/wiki/Special:Search/" + articleTrimmed);

            return this;
        }

        public getContent(): string {
            return "<div>" + this.lines.join() + "</div>";
        }
    }
}
