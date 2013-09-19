/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/leaflet/leaflet.d.ts" />
/// <reference path="typings/guildwars2.arenanet/guildwars2.arenanet.d.ts" />
/// <reference path="typings/guildwars2.syntaxerror/guildwars2.syntaxerror.d.ts" />

module GuildWars2.ArenaNet.Mapper {
    export class ArenaNetMap extends L.Map {
        private static Instances: { [key: string]: ArenaNetMap } = {};

        private currentMapId: number = -1;
        private mapData: { [key: number]: GuildWars2.ArenaNet.Model.FloorMapDetails } = {};

        private landmarks: CustomLayerGroup = new CustomLayerGroup();
        private sectors: CustomLayerGroup = new CustomLayerGroup();
        private skillChallenges: CustomLayerGroup = new CustomLayerGroup();
        private tasks: CustomLayerGroup = new CustomLayerGroup();
        private unlocks: CustomLayerGroup = new CustomLayerGroup();
        private vistas: CustomLayerGroup = new CustomLayerGroup();
        private waypoints: CustomLayerGroup = new CustomLayerGroup();

        private mapLandmarks: { [key: number]: CustomLayerGroup } = {};
        private mapSectors: { [key: number]: CustomLayerGroup } = {};
        private mapSkillChallenges: { [key: number]: CustomLayerGroup } = {};
        private mapTasks: { [key: number]: CustomLayerGroup } = {};
        private mapUnlocks: { [key: number]: CustomLayerGroup } = {};
        private mapVistas: { [key: number]: CustomLayerGroup } = {};
        private mapWaypoints: { [key: number]: CustomLayerGroup } = {};

        constructor(id: string) {
            super(id, {
                minZoom: 2,
                maxZoom: 7,
                crs: L.CRS.Simple
            });

            ArenaNetMap.Instances[id] = this;

            new L.TileLayer("https://tiles.guildwars2.com/1/1/{z}/{x}/{y}.jpg", {
                minZoom: this.getMinZoom(),
                maxZoom: this.getMaxZoom(),
                continuousWorld: true
            }).addTo(this);

            this.waypoints.addTo(this);
            this.landmarks.addTo(this);
            this.vistas.addTo(this);
            this.unlocks.addTo(this);
            this.tasks.addTo(this);
            this.skillChallenges.addTo(this);
            this.sectors.addTo(this);
            this.sectors.setVisibility(false);

            new L.Control.Layers()
                .addOverlay(this.landmarks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/poi.png\" /> <span class=\"legend\">Points of Interest</span>")
                .addOverlay(this.sectors, "<span class=\"legend\" style=\"display: inline-block; width: 20px; height: 20px; font-family: menomonia; font-size: 10pt; font-weight: 900; text-align: center; color: #d3d3d3; text-shadow: -1px -1px 0px black;\"><em>A</em></span> <span class=\"legend\">Sectors</span>")
                .addOverlay(this.skillChallenges, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/skill_point.png\" /> <span class=\"legend\">Skill Points</span>")
                .addOverlay(this.tasks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/renown_heart.png\" /> <span class=\"legend\">Renown Hearts</span>")
                .addOverlay(this.unlocks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/dungeon.png\" /> <span class=\"legend\">Dungeons</span>")
                .addOverlay(this.vistas, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/vista.png\" /> <span class=\"legend\">Vistas</span>")
                .addOverlay(this.waypoints, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"Resources/waypoint.png\" /> <span class=\"legend\">Waypoints</span>")
                .addTo(this);

            $.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (response: GuildWars2.ArenaNet.Model.MapFloorResponse): void {
                    ArenaNetMap.LoadFloorData(id, response); });

            $.get("https://api.guildwars2.com/v1/event_details.json", function (response: GuildWars2.ArenaNet.Model.EventDetailsResponse): void {
                    ArenaNetMap.LoadEventData(id, response.events); });

            this.on("moveend", function (e: L.LeafletEvent) {
                ArenaNetMap.OnMapMoveEnd(id);
            });
        }

        private getMapByCenter(center: L.Point): number {
            for (var mid in this.mapData) {
                var map = this.mapData[mid];

                if (center.x > map.continent_rect[0][0] &&
                    center.x < map.continent_rect[1][0] &&
                    center.y > map.continent_rect[0][1] &&
                    center.y < map.continent_rect[1][1])
                    return mid;
            }

            return -1;
        }

        private setMapLayerVisibility(layer: { [key: number]: CustomLayerGroup }, visible: boolean): void {
            for (var mid in layer)
                layer[mid].setVisibility(visible);
        }

        private setAllMapLayerVisibility(visible: boolean): void {
            this.setMapLayerVisibility(this.mapLandmarks, visible);
            this.setMapLayerVisibility(this.mapSectors, visible);
            this.setMapLayerVisibility(this.mapSkillChallenges, visible);
            this.setMapLayerVisibility(this.mapTasks, visible);
            this.setMapLayerVisibility(this.mapUnlocks, visible);
            this.setMapLayerVisibility(this.mapVistas, visible);
            this.setMapLayerVisibility(this.mapWaypoints, visible);
        }

        private static LoadFloorData(id: string, floor: GuildWars2.ArenaNet.Model.Floor): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            that.setMaxBounds(new L.LatLngBounds(
                that.unproject(new L.Point(0 - (floor.texture_dims[1] * 0.5), floor.texture_dims[1] * 1.5), that.getMaxZoom()),
                that.unproject(new L.Point(floor.texture_dims[0] * 1.5, 0 - (floor.texture_dims[0] * 0.5)), that.getMaxZoom())
                ));

            that.setView(that.unproject(new L.Point(floor.texture_dims[0] / 2, floor.texture_dims[1] / 2), that.getMaxZoom()),
                that.getMinZoom());

            for (var i in floor.regions) {
                var region = floor.regions[i];
                for (var j in region.maps) {
                    var mid = <number>j;
                    var map = region.maps[mid];

                    that.mapData[mid] = map;

                    if (that.mapLandmarks[mid] == undefined) {
                        that.mapLandmarks[mid] = new CustomLayerGroup();
                        that.mapLandmarks[mid].setVisibility(false);
                        that.landmarks.addCustomLayer(that.mapLandmarks[mid]);
                    }
                    if (that.mapSectors[mid] == undefined) {
                        that.mapSectors[mid] = new CustomLayerGroup();
                        that.mapSectors[mid].setVisibility(false);
                        that.sectors.addCustomLayer(that.mapSectors[mid]);
                    }
                    if (that.mapSkillChallenges[mid] == undefined) {
                        that.mapSkillChallenges[mid] = new CustomLayerGroup();
                        that.mapSkillChallenges[mid].setVisibility(false);
                        that.skillChallenges.addCustomLayer(that.mapSkillChallenges[mid]);
                    }
                    if (that.mapTasks[mid] == undefined) {
                        that.mapTasks[mid] = new CustomLayerGroup();
                        that.mapTasks[mid].setVisibility(false);
                        that.tasks.addCustomLayer(that.mapTasks[mid]);
                    }
                    if (that.mapUnlocks[mid] == undefined) {
                        that.mapUnlocks[mid] = new CustomLayerGroup();
                        that.mapUnlocks[mid].setVisibility(false);
                        that.unlocks.addCustomLayer(that.mapUnlocks[mid]);
                    }
                    if (that.mapVistas[mid] == undefined) {
                        that.mapVistas[mid] = new CustomLayerGroup();
                        that.mapVistas[mid].setVisibility(false);
                        that.vistas.addCustomLayer(that.mapVistas[mid]);
                    }
                    if (that.mapWaypoints[mid] == undefined) {
                        that.mapWaypoints[mid] = new CustomLayerGroup();
                        that.mapWaypoints[mid].setVisibility(false);
                        that.waypoints.addCustomLayer(that.mapWaypoints[mid]);
                    }

                    for (var k in map.points_of_interest) {
                        var poi = map.points_of_interest[k];
                        var poiMarker = new PointOfInterestMarker(that.unproject(new L.Point(poi.coord[0], poi.coord[1]), that.getMaxZoom()), poi);

                        switch (poi.type) {
                            case "landmark":
                                that.mapLandmarks[mid].addLayer(poiMarker);
                                break;
                            case "unlock":
                                that.mapUnlocks[mid].addLayer(poiMarker);
                                break;
                            case "vista":
                                that.mapVistas[mid].addLayer(poiMarker);
                                break;
                            case "waypoint":
                                that.mapWaypoints[mid].addLayer(poiMarker);
                                break;
                            default:
                                break;
                        }
                    }

                    for (var k in map.sectors) {
                        var sector = map.sectors[k];
                        var sectorMarker = new SectorMarker(that.unproject(new L.Point(sector.coord[0], sector.coord[1]), that.getMaxZoom()), sector);
                        that.mapSectors[mid].addLayer(sectorMarker);
                    }

                    for (var k in map.skill_challenges) {
                        var skillChallenge = map.skill_challenges[k];
                        var skillChallengeMarker = new SkillChallengeMarker(that.unproject(new L.Point(skillChallenge.coord[0], skillChallenge.coord[1]),
                            that.getMaxZoom()));
                        that.mapSkillChallenges[mid].addLayer(skillChallengeMarker);
                    }

                    for (var k in map.tasks) {
                        var task = map.tasks[k];
                        var taskMarker = new TaskMarker(that.unproject(new L.Point(task.coord[0], task.coord[1]), that.getMaxZoom()), task);
                        that.mapTasks[mid].addLayer(taskMarker);
                    }
                }
            }
        }

        private static LoadEventData(id: string, events: { [key: string]: GuildWars2.ArenaNet.Model.EventDetails }): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];
        }

        private static OnMapMoveEnd(id: string) {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            var mid = that.getMapByCenter(that.project(that.getCenter(), that.getMaxZoom()));

            if (that.getZoom() < 3 || mid < 0) {
                that.currentMapId = -1;
                that.setAllMapLayerVisibility(false);
                return;
            }

            if (that.currentMapId == mid)
                return;

            that.setAllMapLayerVisibility(false);
            that.mapLandmarks[mid].setVisibility(true);
            that.mapSectors[mid].setVisibility(true);
            that.mapSkillChallenges[mid].setVisibility(true);
            that.mapTasks[mid].setVisibility(true);
            that.mapUnlocks[mid].setVisibility(true);
            that.mapVistas[mid].setVisibility(true);
            that.mapWaypoints[mid].setVisibility(true);
            that.currentMapId = mid;
        }
    }

    class CustomLayerGroup extends L.LayerGroup {
        private mapParent: L.Map;
        private lgParent: CustomLayerGroup;
        private visible: boolean = true;

        constructor(layers?: L.ILayer[]) {
            super(layers);
        }

        public addTo(map: L.Map): L.LayerGroup {
            this.mapParent = map;

            if (this.visible)
                super.addTo(map);

            return this;
        }

        public addCustomLayer(layer: CustomLayerGroup): CustomLayerGroup {
            layer.lgParent = this;

            if (layer.visible)
                super.addLayer(layer);

            return this;
        }

        public setVisibility(visible: boolean): void {
            if (this.visible != visible) {
                this.visible = visible;

                if (this.mapParent != undefined) {
                    if (this.visible) {
                        this.mapParent.addLayer(this);
                    } else {
                        this.mapParent.removeLayer(this);
                    }
                } else if (this.lgParent != undefined) {
                    if (this.visible) {
                        this.lgParent.addLayer(this);
                    } else {
                        this.lgParent.removeLayer(this);
                    }
                }
            }
        }
    }

    class PointOfInterestMarker extends L.Marker {
        private static Icons: { [key: string]: L.Icon } = {
            "landmark": new L.Icon({ iconUrl: "Resources/poi.png", iconSize: new L.Point(20, 20) }),
            "unlock": new L.Icon({ iconUrl: "Resources/dungeon.png", iconSize: new L.Point(20, 20) }),
            "vista": new L.Icon({ iconUrl: "Resources/vista.png", iconSize: new L.Point(20, 20) }),
            "waypoint": new L.Icon({ iconUrl: "Resources/waypoint.png", iconSize: new L.Point(20, 20) })
        };

        constructor(latlng: L.LatLng, poi: GuildWars2.ArenaNet.Model.PointOfInterest) {
            super(latlng, { title: (poi.name != "" ? poi.name : undefined) });

            if (PointOfInterestMarker.Icons[poi.type] != undefined)
                super.setIcon(PointOfInterestMarker.Icons[poi.type]);

            if (poi.name != "") {
                super.bindPopup(new PopupContentFactory()
                    .appendWikiLink(poi.name)
                    .getContent(), { offset: new L.Point(0, -10) });
            }
        }
    }

    class SectorMarker extends L.Marker {
        constructor(latlng: L.LatLng, sector: GuildWars2.ArenaNet.Model.Sector) {
            super(latlng, {
                icon: new L.DivIcon({
                    html: "<div><em>" + sector.name + "</em></div>" + (sector.level > 0 ? "<div style=\"font-size: 75%;\"><em>Level " + sector.level + " </em></div>" : ""),
                    iconSize: new L.Point(200, 32)
                }),
                clickable: false,
                opacity: 0.7
            });
        }
    }

    class SkillChallengeMarker extends L.Marker {
        private static Icon: L.Icon = new L.Icon({ iconUrl: "Resources/skill_point.png", iconSize: new L.Point(20, 20) });

        constructor(latlng: L.LatLng) {
            super(latlng, { icon: SkillChallengeMarker.Icon });
        }
    }

    class TaskMarker extends L.Marker {
        private static Icon: L.Icon = new L.Icon({ iconUrl: "Resources/renown_heart.png", iconSize: new L.Point(20, 20) });

        constructor(latlng: L.LatLng, task: GuildWars2.ArenaNet.Model.Task) {
            super(latlng, {
                title: task.objective + "(" + task.level + ")",
                icon: TaskMarker.Icon
            });

            super.bindPopup(new PopupContentFactory()
                .appendWikiLink(task.objective)
                .getContent(), { offset: new L.Point(0, -10) });
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
