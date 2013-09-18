var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var GuildWars2;
(function (GuildWars2) {
    (function (ArenaNet) {
        (function (Mapper) {
            var ArenaNetMap = (function (_super) {
                __extends(ArenaNetMap, _super);
                function ArenaNetMap(id) {
                    _super.call(this, id, {
                        minZoom: 0,
                        maxZoom: 7,
                        crs: L.CRS.Simple
                    });

                    _super.prototype.setView.call(this, new L.LatLng(0, 0), 0);

                    new L.TileLayer("https://tiles.guildwars2.com/1/1/{z}/{x}/{y}.jpg", {
                        minZoom: _super.prototype.getMinZoom.call(this),
                        maxZoom: _super.prototype.getMaxZoom.call(this),
                        continuousWorld: true
                    }).addTo(this);

                    var anetmap = this;
                    $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (floor) {
                        anetmap.setMaxBounds(new L.LatLngBounds(anetmap.unproject(new L.Point(0 - (floor.texture_dims[1] * 0.5), floor.texture_dims[1] * 1.5), anetmap.getMaxZoom()), anetmap.unproject(new L.Point(floor.texture_dims[0] * 1.5, 0 - (floor.texture_dims[0] * 0.5)), anetmap.getMaxZoom())));

                        var poiGroup = new L.LayerGroup();
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
                                            marker.setIcon(ArenaNetMap.LandmarkIcon);
                                            break;
                                        case "vista":
                                            marker.setIcon(ArenaNetMap.VistaIcon);
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
                ArenaNetMap.LandmarkIcon = new L.Icon({ iconUrl: "Resources/poi.png" });
                ArenaNetMap.VistaIcon = new L.Icon({ iconUrl: "Resources/vista.png" });
                ArenaNetMap.WaypointIcon = new L.Icon({ iconUrl: "Resources/waypoint.png" });
                return ArenaNetMap;
            })(L.Map);
            Mapper.ArenaNetMap = ArenaNetMap;
        })(ArenaNet.Mapper || (ArenaNet.Mapper = {}));
        var Mapper = ArenaNet.Mapper;
    })(GuildWars2.ArenaNet || (GuildWars2.ArenaNet = {}));
    var ArenaNet = GuildWars2.ArenaNet;
})(GuildWars2 || (GuildWars2 = {}));
