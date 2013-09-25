/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/leaflet/leaflet.d.ts" />
/// <reference path="typings/leaflet.polylineDecorator/leaflet.polylineDecorator.d.ts" />
/// <reference path="typings/guildwars2.arenanet/guildwars2.arenanet.d.ts" />
/// <reference path="typings/guildwars2.arenanet.mumblelink/guildwars2.arenanet.mumblelink.d.ts" />
/// <reference path="guildwars2.syntaxerror.ts" />

module GuildWars2.ArenaNet.Mapper {
    var ResourceBaseUri = "Resources";

    export class ArenaNetMap extends L.Map {
        private static Instances: { [key: string]: ArenaNetMap } = {};

        private static BountPathColors: string[] = [ "#0000ff", "#ffffff", "#ffff00", "#32cd32" ];

        private currentMapId: number = -1;
        private mapData: { [key: number]: GuildWars2.ArenaNet.Model.FloorMapDetails } = {};

        private bountyPanControl: BountyPanControl = new BountyPanControl();
        private playerPositionControl: PlayerPositionControl = new PlayerPositionControl();

        private playerPosition: PlayerPositionLayer = null;

        private bounties: CustomLayerGroup = new CustomLayerGroup();
        private events: CustomLayerGroup = new CustomLayerGroup();
        private landmarks: CustomLayerGroup = new CustomLayerGroup();
        private players: CustomLayerGroup = new CustomLayerGroup();
        private sectors: CustomLayerGroup = new CustomLayerGroup();
        private skillChallenges: CustomLayerGroup = new CustomLayerGroup();
        private tasks: CustomLayerGroup = new CustomLayerGroup();
        private unlocks: CustomLayerGroup = new CustomLayerGroup();
        private vistas: CustomLayerGroup = new CustomLayerGroup();
        private waypoints: CustomLayerGroup = new CustomLayerGroup();

        private mapBounties: { [key: number]: CustomLayerGroup } = {};
        private mapEvents: { [key: number]: CustomLayerGroup } = {};
        private mapLandmarks: { [key: number]: CustomLayerGroup } = {};
        private mapSectors: { [key: number]: CustomLayerGroup } = {};
        private mapSkillChallenges: { [key: number]: CustomLayerGroup } = {};
        private mapTasks: { [key: number]: CustomLayerGroup } = {};
        private mapUnlocks: { [key: number]: CustomLayerGroup } = {};
        private mapVistas: { [key: number]: CustomLayerGroup } = {};
        private mapWaypoints: { [key: number]: CustomLayerGroup } = {};

        private eventMarkers: { [key: string]: EventMarker } = {};
        private eventPolygons: { [key: string]: EventPolygon } = {};

        constructor(id: string) {
            super(id, {
                minZoom: 2,
                maxZoom: 7,
                crs: L.CRS.Simple,
                attributionControl: false
            });

            var loading = jQuery("<div class=\"loading\"><div class=\"spinner circles\"><div></div><div></div><div></div><div></div><div></div><div></div><div></div><div></div></div></div>");
            loading.appendTo(jQuery("#" + id));

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
            this.bounties.addTo(this);
            this.events.addTo(this);
            this.players.addTo(this);

            this.sectors.setVisibility(false);
            this.bounties.setVisibility(false);

            new L.Control.Layers()
                .addOverlay(this.bounties, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/bounty.png\" /> <span class=\"legend\">Bounties</span>")
                .addOverlay(this.events, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/event_star.png\" /> <span class=\"legend\">Events</span>")
                .addOverlay(this.landmarks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/poi.png\" /> <span class=\"legend\">Points of Interest</span>")
                .addOverlay(this.players, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/commander.png\" /> <span class=\"legend\">Players</span>")
                .addOverlay(this.sectors, "<span class=\"legend\" style=\"display: inline-block; width: 20px; height: 20px; font-family: menomonia; font-size: 10pt; font-weight: 900; text-align: center; color: #d3d3d3; text-shadow: -1px -1px 0px black;\"><em>A</em></span> <span class=\"legend\">Sectors</span>")
                .addOverlay(this.skillChallenges, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/skill_point.png\" /> <span class=\"legend\">Skill Points</span>")
                .addOverlay(this.tasks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/renown_heart.png\" /> <span class=\"legend\">Renown Hearts</span>")
                .addOverlay(this.unlocks, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/dungeon.png\" /> <span class=\"legend\">Dungeons</span>")
                .addOverlay(this.vistas, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/vista.png\" /> <span class=\"legend\">Vistas</span>")
                .addOverlay(this.waypoints, "<img width=\"20\" height=\"20\" class=\"legend\" src=\"" + ResourceBaseUri + "/waypoint.png\" /> <span class=\"legend\">Waypoints</span>")
                .addTo(this);

            new FullscreenControl().addTo(this);
            this.bountyPanControl.addTo(this);
            this.bountyPanControl.hide();
            this.playerPositionControl.addTo(this);
            this.playerPositionControl.hide();

            super.on("overlayadd", function (event: any) {
                var map = <ArenaNetMap>this;

                if ((<L.LeafletLayerEvent>event).layer == map.bounties)
                    map.bountyPanControl.show();
            }, this);
            super.on("overlayremove", function (event: any) {
                var map = <ArenaNetMap>this;

                if ((<L.LeafletLayerEvent>event).layer == map.bounties)
                    map.bountyPanControl.hide();
            }, this);

            jQuery.get("https://api.guildwars2.com/v1/map_floor.json?continent_id=1&floor=2", function (mapFloorResponse: GuildWars2.ArenaNet.API.MapFloorResponse): void {
                ArenaNetMap.LoadFloorData(id, mapFloorResponse);

                jQuery.get("https://api.guildwars2.com/v1/event_details.json", function (eventDetailsResponse: GuildWars2.ArenaNet.API.EventDetailsResponse): void {

                    jQuery.get("http://gomgods.com/gw2/mapper/champs", function (championEventsResponse: GuildWars2.SyntaxError.API.ChampionEventsResponse): void {
                        ArenaNetMap.LoadEventData(id, eventDetailsResponse.events, championEventsResponse.champion_events);
                        ArenaNetMap.LoadEventStates(id);

                        setInterval(function () { ArenaNetMap.LoadEventStates(id); }, 30000);

                        setTimeout(function () { ArenaNetMap.LoadPlayerPositionData(id); }, 5000);

                        loading.detach();
                    });

                });
            });

            ArenaNetMap.LoadBounties(id);

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

        private setMapVisibility(mid: number, visible: boolean): void {
            if (this.mapBounties[mid] != undefined) this.mapBounties[mid].setVisibility(visible);
            if (this.mapEvents[mid] != undefined) this.mapEvents[mid].setVisibility(visible);
            if (this.mapLandmarks[mid] != undefined) this.mapLandmarks[mid].setVisibility(visible);
            if (this.mapSectors[mid] != undefined) this.mapSectors[mid].setVisibility(visible);
            if (this.mapSkillChallenges[mid] != undefined) this.mapSkillChallenges[mid].setVisibility(visible);
            if (this.mapTasks[mid] != undefined) this.mapTasks[mid].setVisibility(visible);
            if (this.mapUnlocks[mid] != undefined) this.mapUnlocks[mid].setVisibility(visible);
            if (this.mapVistas[mid] != undefined) this.mapVistas[mid].setVisibility(visible);
            if (this.mapWaypoints[mid] != undefined) this.mapWaypoints[mid].setVisibility(visible);
        }

        private setMapLayerVisibility(layer: { [key: number]: CustomLayerGroup }, visible: boolean): void {
            for (var mid in layer)
                layer[mid].setVisibility(visible);
        }

        private setAllMapLayerVisibility(visible: boolean): void {
            this.setMapLayerVisibility(this.mapBounties, visible);
            this.setMapLayerVisibility(this.mapEvents, visible);
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

            that.bountyPanControl.setMapData(that.mapData);
        }

        private static LoadEventData(id: string, events: { [key: string]: GuildWars2.ArenaNet.Model.EventDetails }, champions: string[]): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            for (var eid in events) {
                var ev = events[eid];
                var mid = ev.map_id;

                if (ev.name.toLowerCase().indexOf("skill challenge: ") == 0 || that.mapData[mid] == undefined)
                    continue;

                var hasChamp = (champions.indexOf(eid) >= 0);
                var map = that.mapData[mid];

                if (that.mapEvents[mid] == undefined) {
                    that.mapEvents[mid] = new CustomLayerGroup();
                    that.mapEvents[mid].setVisibility(false);
                    that.events.addCustomLayer(that.mapEvents[mid]);
                }

                var center = new L.Point(ArenaNetMap.TranslateX(ev.location.center[0], map), ArenaNetMap.TranslateZ(ev.location.center[1], map));

                switch (ev.location.type) {
                    case "poly":
                        var polyPoints: L.LatLng[] = [];

                        for (var i in ev.location.points) {
                            polyPoints.push(that.unproject(new L.Point(
                                ArenaNetMap.TranslateX(ev.location.points[i][0], map), ArenaNetMap.TranslateZ(ev.location.points[i][1], map)),
                                that.getMaxZoom()));
                        }

                        that.eventPolygons[eid] = new EventPolygon(polyPoints, hasChamp);
                        break;
                    case "sphere":
                    case "cylinder":
                        var polyPoints: L.LatLng[] = [];
                        var radius = ArenaNetMap.TranslateX(ev.location.center[0] + ev.location.radius, map) - center.x;

                        for (var j = 0; j < 360; j += 10) {
                            polyPoints.push(that.unproject(new L.Point(
                                center.x + radius * Math.cos(j * (Math.PI / 180)), center.y + radius * Math.sin(j * (Math.PI / 180))),
                                that.getMaxZoom()));
                        }

                        that.eventPolygons[eid] = new EventPolygon(polyPoints, hasChamp);
                        break;
                    default:
                        break;
                }

                that.eventMarkers[eid] = new EventMarker(that.unproject(center, that.getMaxZoom()), ev);
            }
        }

        private static LoadEventStates(id: string): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            jQuery.get("https://api.guildwars2.com/v1/events.json?world_id=1007", function (response: GuildWars2.ArenaNet.API.EventsResponse): void {
                for (var mid in that.mapEvents)
                    that.mapEvents[mid].clearLayers();

                for (var i in response.events) {
                    var es = response.events[i];
                    var eid = es.event_id;

                    var eventLayer = new L.LayerGroup();

                    switch (es.state) {
                        case "Active":
                            if (that.eventPolygons[eid] != undefined)
                                eventLayer.addLayer(that.eventPolygons[eid]);
                        case "Preparation":
                            if (that.eventMarkers[eid] != undefined) {
                                that.eventMarkers[eid].setEventState(es.state);
                                eventLayer.addLayer(that.eventMarkers[eid]);
                                that.mapEvents[es.map_id].addLayer(eventLayer);
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private static LoadBounties(id: string): void {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            for (var i in GuildWars2.SyntaxError.Model.GuildBountyDefinitions.Bounties) {
                var bounty = GuildWars2.SyntaxError.Model.GuildBountyDefinitions.Bounties[i];
                var mid = bounty.map_id;

                if (that.mapBounties[mid] == undefined) {
                    that.mapBounties[mid] = new CustomLayerGroup();
                    that.mapBounties[mid].setVisibility(false);
                    that.bounties.addCustomLayer(that.mapBounties[mid]);
                }

                var b = new BountyLayerGroup(bounty.name);

                if (bounty.spawns != undefined) {
                    for (var j in bounty.spawns) {
                        var p = bounty.spawns[j];
                        b.addSpawningPoint(that.unproject(new L.Point(p[0], p[1]), that.getMaxZoom()));
                    }
                }

                if (bounty.paths != undefined) {
                    var c = 0;

                    for (var j in bounty.paths) {
                        var path = bounty.paths[j];
                        var locs: L.LatLng[] = [];

                        for (var k in path.points) {
                            var p = path.points[k];
                            locs.push(that.unproject(new L.Point(p[0], p[1]), that.getMaxZoom()));
                        }

                        b.addPath(locs, ArenaNetMap.BountPathColors[c], path.direction);
                        c = (c + 1) % ArenaNetMap.BountPathColors.length;
                    }
                }

                that.mapBounties[mid].addLayer(b);
            }
        }

        private static LoadPlayerPositionData(id: string) {
            if (ArenaNetMap.Instances[id] == undefined)
                return;

            var that = ArenaNetMap.Instances[id];

            var timeout = 5000;

            jQuery.ajax("http://localhost:38139", {
                complete: function () { setTimeout(function () { ArenaNetMap.LoadPlayerPositionData(id); }, timeout); },
                error: function () {
                    if (that.playerPosition != null) {
                        that.removeLayer(that.playerPosition);
                        that.playerPositionControl.hide();
                        that.playerPosition = null;
                    }
                },
                success: function (data: GuildWars2.ArenaNet.MumbleLink.MumbleData) {
                    if (data.data_available && that.mapData[data.map] != undefined) {
                        var map = that.mapData[data.map];

                        var loc = that.unproject(new L.Point(
                            ArenaNetMap.TranslateX(data.pos_x * 39.3700787, map),
                            ArenaNetMap.TranslateZ(data.pos_z * 39.3700787, map)), that.getMaxZoom());

                        var locChanged = false;

                        if (that.playerPosition == null) {
                            that.playerPosition = new PlayerPositionLayer(loc, data.player_name, data.player_is_commander);
                            that.addLayer(that.playerPosition);
                            that.playerPositionControl.show();

                            locChanged = true;
                        } else {
                            that.playerPosition.setRotation(data.rot_player);
                            that.playerPosition.setCommander(data.player_is_commander);

                            if (!loc.equals(that.playerPosition.getLatLng())) {
                                that.playerPosition.setLatLng(loc);
                                locChanged = true;
                            }
                        }

                        if (locChanged && that.playerPositionControl.followPlayer)
                            that.panTo(loc);
                    }

                    timeout = 250;
                },
                timeout: 250
            });
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
            that.setMapVisibility(mid, true);
            that.currentMapId = mid;
        }

        private static TranslateX(x: number, map: GuildWars2.ArenaNet.Model.FloorMapDetails): number {
            return (x - map.map_rect[0][0]) / (map.map_rect[1][0] - map.map_rect[0][0]) * (map.continent_rect[1][0] - map.continent_rect[0][0]) + map.continent_rect[0][0];
        }

        private static TranslateZ(z: number, map: GuildWars2.ArenaNet.Model.FloorMapDetails): number {
            return (1 - ((z - map.map_rect[0][1]) / (map.map_rect[1][1] - map.map_rect[0][1]))) * (map.continent_rect[1][1] - map.continent_rect[0][1]) + map.continent_rect[0][1];
        }
    }

    class FullscreenControl extends L.Control {
        private isFullscreen: boolean = false;
        private oldStyle: string;
        private oldParent: HTMLElement = null;
        private oldSibling: HTMLElement = null;

        constructor() {
            super({ position: "bottomright" });
        }

        public onAdd(map: L.Map): HTMLElement {
            var container = L.DomUtil.create("div", "leaflet-control-fullscreen");

            var icon = <HTMLImageElement>L.DomUtil.create("img", "leaflet-control-fullscreen-icon", container);
            var jqIcon = jQuery(icon);
            jqIcon.attr("width", 20);
            jqIcon.attr("height", 20);
            jqIcon.attr("src", ResourceBaseUri + "/fullscreen_enter.png");
            jqIcon.attr("title", "Fill window");

            var that = this;

            L.DomEvent.addListener(icon, "click", L.DomEvent.stop);
            jqIcon.click(function () {
                var mapContainer = map.getContainer();
                var jqMapContainer = jQuery(mapContainer);

                if (that.isFullscreen) {
                    jqMapContainer.detach();

                    // Drupal-specific
                    if (jQuery('#page').length > 0) jQuery('#page').show();

                    if (that.oldSibling != null) {
                        jqMapContainer.insertAfter(jQuery(that.oldSibling));
                        that.oldSibling = null;
                    } else {
                        jqMapContainer.prependTo(jQuery(that.oldParent));
                        that.oldParent = null;
                    }

                    jqMapContainer.attr("style", (that.oldStyle != undefined ? that.oldStyle : ""));
                    jqIcon.attr("src", ResourceBaseUri + "/fullscreen_enter.png");
                    jqIcon.attr("title", "Fill window");
                    that.isFullscreen = false;
                } else {
                    that.oldStyle = jqMapContainer.attr("style");
                    var prevSibling = jqMapContainer.prev();
                    if (prevSibling.length > 0)
                        that.oldSibling = prevSibling[0];
                    else
                        that.oldParent = jqMapContainer.parent()[0];

                    // Drupal-specific
                    if (jQuery('#page').length > 0) jQuery('#page').hide();

                    jqMapContainer.detach().appendTo(jQuery('body'));
                    jqMapContainer.attr("style", that.oldStyle + "; position: absolute; top: 0px; bottom: 0px; left: 0px; right: 0px; z-index: 5; width: auto; height: auto;");
                    jqIcon.attr("src", ResourceBaseUri + "/fullscreen_exit.png");
                    jqIcon.attr("title", "Exit fill window");
                    that.isFullscreen = true;
                }

                map.invalidateSize();
            });

            return container;
        }
    }

    class PlayerPositionControl extends L.Control {
        private mapContainer: HTMLElement;

        public followPlayer: boolean = false;
        public reportPosition: boolean = false;

        constructor() {
            super({ position: "bottomright" });
        }

        public onAdd(map: L.Map): HTMLElement {
            var container = L.DomUtil.create("div", "leaflet-control-playerposition");

            var icon = <HTMLImageElement>L.DomUtil.create("img", "leaflet-control-playerposition-icon", container);
            var jqIcon = jQuery(icon);
            jqIcon.attr("width", 24);
            jqIcon.attr("height", 24);
            jqIcon.attr("src", ResourceBaseUri + "/player_position.png");

            var div = <HTMLDivElement>L.DomUtil.create("div", "leaflet-control-playerposition-legend", container);
            var jqDiv = jQuery(div);
            jqDiv.hide();

            jqIcon.mouseenter(function () {
                jqIcon.hide();
                jqDiv.show();
            });
            jqDiv.mouseleave(function () {
                jqDiv.hide();
                jqIcon.show();
            });

            var that = this;

            var jqCbFollowPlayer = jQuery("<input type=\"checkbox\" class=\"legend\" />");
            jqCbFollowPlayer.click(function () { that.followPlayer = jqCbFollowPlayer.is(":checked"); });
            var jqCbReportPosition = jQuery("<input type=\"checkbox\" class=\"legend\" />");
            jqCbReportPosition.click(function () { that.reportPosition = jqCbReportPosition.is(":checked"); });

            jqDiv.append(jqCbFollowPlayer);
            jqDiv.append(" <span class=\"legend\">Follow player</span><br />");
            jqDiv.append(jqCbReportPosition);
            jqDiv.append(" <span class=\"legend\">Report position</span>");

            this.mapContainer = container;

            return container;
        }

        public show(): void { jQuery(this.mapContainer).show(); }
        public hide(): void { jQuery(this.mapContainer).hide(); }
    }

    class BountyPanControl extends L.Control {
        private mapData: { [key: number]: GuildWars2.ArenaNet.Model.FloorMapDetails } = {};
        private mapContainer: HTMLElement;

        constructor() {
            super({ position: "bottomright" });
        }

        public setMapData(mapData: { [key: number]: GuildWars2.ArenaNet.Model.FloorMapDetails }) {
            this.mapData = mapData;
        }

        public onAdd(map: L.Map): HTMLElement {
            var container = L.DomUtil.create("div", "leaflet-control-bountypan");

            var icon = <HTMLImageElement>L.DomUtil.create("img", "leaflet-control-bountypan-icon", container);
            var jqIcon = jQuery(icon);
            jqIcon.attr("width", 20);
            jqIcon.attr("height", 20);
            jqIcon.attr("src", ResourceBaseUri + "/bounty.png");

            var list = <HTMLDivElement>L.DomUtil.create("div", "leaflet-control-bountypan-list", container);
            var jqList = jQuery(list);
            jqList.hide();

            jqIcon.mouseenter(function () {
                jqIcon.hide();
                jqList.show();
            });
            jqList.mouseleave(function () {
                jqList.hide();
                jqIcon.show();
            });

            for (var i in GuildWars2.SyntaxError.Model.GuildBountyDefinitions.Bounties) {
                var bounty = GuildWars2.SyntaxError.Model.GuildBountyDefinitions.Bounties[i];

                this.addButton(bounty, map, list);
                L.DomUtil.create("br", null, list);
            }

            this.mapContainer = container;

            return container;
        }

        public show() { jQuery(this.mapContainer).show(); }
        public hide() { jQuery(this.mapContainer).hide(); }

        private addButton(bounty: GuildWars2.SyntaxError.Model.GuildBounty, map: L.Map, container: HTMLElement) {
            var link = <HTMLAnchorElement>L.DomUtil.create("a", "leaflet-control-bountypan-link", container);
            var jqLink = jQuery(link);
            jqLink.attr("href", "#");
            jqLink.attr("title", bounty.name);
            jqLink.text(bounty.name);

            var that = this;

            L.DomEvent.addListener(link, "click", L.DomEvent.stop);
            L.DomEvent.addListener(link, "click", function () {
                if (that.mapData[bounty.map_id] != undefined) {
                    var m = that.mapData[bounty.map_id];
                    map.fitBounds(new L.LatLngBounds(
                        map.unproject(new L.Point(m.continent_rect[0][0], m.continent_rect[1][1]), map.getMaxZoom()),
                        map.unproject(new L.Point(m.continent_rect[1][0], m.continent_rect[0][1]), map.getMaxZoom())));
                }
            });
        }
    }

    class BountyLayerGroup extends L.LayerGroup {
        private static Icon: L.Icon = new L.Icon({ iconUrl: ResourceBaseUri + "/bounty.png", iconSize: new L.Point(20, 20) });

        private bountyName: string;

        constructor(name: string, layers?: L.ILayer[]) {
            super(layers);

            this.bountyName = name;
        }

        public addSpawningPoint(loc: L.LatLng) {
            var marker = new L.Marker(loc, {
                icon: BountyLayerGroup.Icon,
                title: this.bountyName + " (Spawning Point)"
            });
            marker.bindPopup(new PopupContentFactory()
                .appendWikiLink(this.bountyName)
                .appendDulfyLink(this.bountyName)
                .getContent(), { offset: new L.Point(0, -10) });
            super.addLayer(marker);
        }

        public addPath(polyPoints: L.LatLng[], color: string, direction: string): void {
            var poly = new L.Polygon(polyPoints, {
                color: color,
                weight: 3,
                opacity: 0.8,
                fill: false
            });

            poly.bindPopup(new PopupContentFactory()
                .appendWikiLink(this.bountyName)
                .appendDulfyLink(this.bountyName)
                .getContent());
            super.addLayer(poly);

            var patterns: L.PolylineDecoratorPattern[] = [{
                offset: 0,
                repeat: 150,
                symbol: new L.Symbol.Marker({
                    markerOptions: {
                        icon: BountyLayerGroup.Icon,
                        title: this.bountyName + " (Path)"
                    },
                    rotate: false
                })
            }]

            if (direction != "invalid") {
                patterns.push({
                    offset: 75,
                    repeat: 150,
                    symbol: new L.Symbol.ArrowHead({
                        pixelSize: 10,
                        polygon: false,
                        pathOptions: {
                            color: color,
                            weight: 3,
                            opacity: 0.8,
                            clickable: false
                        }
                    })
                });
            }

            var decorator = new L.PolylineDecorator(poly, { patterns: patterns });
            super.addLayer(decorator);
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

    class EventPolygon extends L.Polygon {
        constructor(latlngs: L.LatLng[], hasChampion: boolean) {
            super(latlngs, {
                color: (hasChampion ? "#00008b" : "#800000"),
                weight: 2,
                opacity: 0.5,
                fillColor: (hasChampion ? "#0000ff" : "#ff0000"),
                fillOpacity: 0.5,
                clickable: false
            });
        }
    }

    class EventMarker extends L.Marker {
        private static Icons: { [key: string]: L.Icon[] } = {
            "none": [new L.Icon({ iconUrl: ResourceBaseUri + "/event_star_gray.png", iconSize: new L.Point(20, 20) }),
                new L.Icon({ iconUrl: ResourceBaseUri + "/event_star.png", iconSize: new L.Point(20, 20) })],
            "group_event": [new L.Icon({ iconUrl: ResourceBaseUri + "/event_boss_gray.png", iconSize: new L.Point(20, 20) }),
                new L.Icon({ iconUrl: ResourceBaseUri + "/event_boss.png", iconSize: new L.Point(20, 20) })]
        };

        private preparationIcon: L.Icon;
        private activeIcon: L.Icon;

        constructor(latlng: L.LatLng, event: GuildWars2.ArenaNet.Model.EventDetails) {
            super(latlng, { title: (event.name != "" ? event.name : undefined) });

            if (event.name != "") {
                super.bindPopup(new PopupContentFactory()
                    .appendWikiLink(event.name)
                    .getContent(), { offset: new L.Point(0, -10) });
            }

            if (event.flags.indexOf("group_event") < 0) {
                this.preparationIcon = EventMarker.Icons["none"][0];
                this.activeIcon = EventMarker.Icons["none"][1];
            } else {
                this.preparationIcon = EventMarker.Icons["group_event"][0];
                this.activeIcon = EventMarker.Icons["group_event"][1];
            }

            this.setEventState(null);
        }

        public setEventState(state: string) {
            switch (state) {
                case "Active":
                    this.setIcon(this.activeIcon);
                    break;
                case "Preparation":
                    this.setIcon(this.preparationIcon);
                    break;
                default:
                    break;
            }
        }
    }

    class PlayerPositionLayer extends L.LayerGroup {
        private static CommanderIcon: L.Icon = new L.Icon({ iconUrl: ResourceBaseUri + "/commander.png", iconSize: new L.Point(32, 32) });

        private commander: boolean;

        private commanderMarker: L.Marker;
        private positionMarker: L.Marker;

        constructor(latlng: L.LatLng, name: string, commander: boolean) {
            super();

            this.commanderMarker = new L.Marker(latlng, {
                icon: PlayerPositionLayer.CommanderIcon,
                clickable: false,
                zIndexOffset: 998
            });

            this.positionMarker = new L.Marker(latlng, {
                title: name,
                icon: this.createRotatedIcon(0),
                clickable: false,
                zIndexOffset: 999
            });

            this.commander = commander;
            if (commander)
                this.addLayer(this.commanderMarker);

            this.addLayer(this.positionMarker);
        }

        public getLatLng(): L.LatLng { return this.positionMarker.getLatLng(); }
        public setLatLng(latlng: L.LatLng): void {
            this.commanderMarker.setLatLng(latlng);
            this.commanderMarker.update();
            this.positionMarker.setLatLng(latlng);
            this.positionMarker.update();
        }

        public setCommander(commander: boolean): void {
            if (this.commander != commander) {
                if (commander)
                    this.addLayer(this.commanderMarker);
                else
                    this.removeLayer(this.commanderMarker);

                this.commander = commander;
            }
        }

        public setRotation(rotation: number): void {
            this.positionMarker.setIcon(this.createRotatedIcon(rotation));
        }

        private createRotatedIcon(rotation: number): L.Icon {
            return new L.DivIcon({
                iconSize: new L.Point(32, 32),
                className: "leaflet-player-marker",
                html: "<img width=\"32\" height=\"32\" style=\"transform: rotate(" + rotation + "deg);\" src=\"" + ResourceBaseUri + "/player_position.png\" />"
            });
        }
    }

    class PointOfInterestMarker extends L.Marker {
        private static Icons: { [key: string]: L.Icon } = {
            "landmark": new L.Icon({ iconUrl: ResourceBaseUri + "/poi.png", iconSize: new L.Point(20, 20) }),
            "unlock": new L.Icon({ iconUrl: ResourceBaseUri + "/dungeon.png", iconSize: new L.Point(20, 20) }),
            "vista": new L.Icon({ iconUrl: ResourceBaseUri + "/vista.png", iconSize: new L.Point(20, 20) }),
            "waypoint": new L.Icon({ iconUrl: ResourceBaseUri + "/waypoint.png", iconSize: new L.Point(20, 20) })
        };

        constructor(latlng: L.LatLng, poi: GuildWars2.ArenaNet.Model.PointOfInterest) {
            super(latlng, {
                title: (poi.name != "" ? poi.name : undefined),
                clickable: (poi.name != "")
            });

            if (PointOfInterestMarker.Icons[poi.type] != undefined)
                super.setIcon(PointOfInterestMarker.Icons[poi.type]);

            if (poi.name != "") {
                var popupFactory = new PopupContentFactory()
                    .appendWikiLink(poi.name);

                if (poi.type == "landmark" || poi.type == "waypoint")
                    popupFactory.appendChatCode(GuildWars2.SyntaxError.Model.ChatCode.CreateMapLink(poi));

                super.bindPopup(popupFactory.getContent(), { offset: new L.Point(0, -10) });
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
        private static Icon: L.Icon = new L.Icon({ iconUrl: ResourceBaseUri + "/skill_point.png", iconSize: new L.Point(20, 20) });

        constructor(latlng: L.LatLng) {
            super(latlng, {
                icon: SkillChallengeMarker.Icon,
                clickable: false
            });
        }
    }

    class TaskMarker extends L.Marker {
        private static Icon: L.Icon = new L.Icon({ iconUrl: ResourceBaseUri + "/renown_heart.png", iconSize: new L.Point(20, 20) });

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

        public appendChatCode(code: string): PopupContentFactory {
            this.lines.push("<p>Chat code: <span onclick=\"javascript: GuildWars2.ArenaNet.Mapper.GlobalHandlers.Popup_ChatCodeClicked(this);\">" + code + "</span></p>");
            return this;
        }

        public appendDulfyLink(bountyName: string): PopupContentFactory {
            if (PopupContentFactory.DulfyBountyLinks[bountyName] != undefined)
                this.appendLink("Dulfy page", bountyName, PopupContentFactory.DulfyBountyLinks[bountyName]);

            return this;
        }

        public appendLink(label: string, text: string, uri: string, target: string = "_blank"): PopupContentFactory {
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
            return "<div>" + this.lines.join("") + "</div>";
        }
    }

    export class GlobalHandlers {

        public static Popup_ChatCodeClicked(element: HTMLElement) {
            if (document.selection) {
                document.selection.clear();
                var drange = document.selection.createRange();
                drange.moveToElementText(element);
                drange.select();
            } else if (window.getSelection) {
                window.getSelection().removeAllRanges();
                var wrange = document.createRange();
                wrange.selectNode(element);
                window.getSelection().addRange(wrange);
            }

            return true;
        }

    }
}
