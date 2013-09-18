/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/leaflet/leaflet.d.ts" />
/// <reference path="typings/guildwars2.arenanet/guildwars2.arenanet.d.ts" />
/// <reference path="typings/guildwars2.syntaxerror/guildwars2.syntaxerror.d.ts" />

module GuildWars2.ArenaNet.Mapper {
    export class ArenaNetMap extends L.Map {
        private static LandmarkIcon: L.Icon = new L.Icon({ iconUrl: "Resources/poi.png" });
        private static VistaIcon: L.Icon = new L.Icon({ iconUrl: "Resources/vista.png" });
        private static WaypointIcon: L.Icon = new L.Icon({ iconUrl: "Resources/waypoint.png" });

        constructor(id: string) {
            super(id, {
                minZoom: 0,
                maxZoom: 7,
                crs: L.CRS.Simple
            });

            super.setView(new L.LatLng(0, 0), 0);

            new L.TileLayer("https://tiles.guildwars2.com/1/1/{z}/{x}/{y}.jpg", {
                minZoom: super.getMinZoom(),
                maxZoom: super.getMaxZoom(),
                continuousWorld: true
            }).addTo(this);

            var anetmap = this;
            $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (floor: GuildWars2.ArenaNet.Model.Floor): void {
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
                            var marker = new L.Marker(anetmap.unproject(new L.Point(poi.coord[0], poi.coord[1]), anetmap.getMaxZoom()), {
                                title: (poi.name != "" ? poi.name : undefined)
                            });

                            switch (poi.type) {
                                case "landmark":
                                    marker.setIcon(ArenaNetMap.LandmarkIcon)
                                    break;
                                case "vista":
                                    marker.setIcon(ArenaNetMap.VistaIcon)
                                    break;
                                case "waypoint":
                                    marker.setIcon(ArenaNetMap.WaypointIcon);
                                    break;
                                default:
                                    continue;
                            }

                            poiGroup.addLayer(marker);
                        }
                    }
                }
            });
        }
    }
}
