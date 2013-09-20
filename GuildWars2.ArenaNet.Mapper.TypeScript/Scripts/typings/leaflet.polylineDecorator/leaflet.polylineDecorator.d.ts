// Type definitions for Leaflet.PolylineDecorator.js (f1079a4fc8: 2013-09-18)
// Project: https://github.com/bbecquet/Leaflet.PolylineDecorator
// Definitions by: Vladimir <https://github.com/bbecquet>

/// <reference "../leaflet/leaflet.d.ts" />

declare module L {

    export interface DirectedPoint {
        pt: L.Point;
        latLng: L.LatLng;
        predecessor: number;
        heading: number;
    }

    export interface PolylineDecoratorPattern {
        offset: any;
        repeat: any;
        symbol: L.Symbol.SymbolFactory;
    }

    export interface PolylineDecoratorOptions {
        patterns?: L.PolylineDecoratorPattern[]
    }

    export class PolylineDecorator extends L.LayerGroup {
        constructor(polyline: L.Polyline, options: PolylineDecoratorOptions);
    }

    export class RotatedMarker extends L.Marker { }

    export module Symbol {

        export interface SymbolFactory {
            buildSymbol(dirPoint: L.DirectedPoint, latLngs: L.LatLng[], map: L.Map, index: number, total: number): L.Polyline;
        }

        export interface DashOptions {
            pixelSize?: number;
            pathOptions?: L.PathOptions;
        }

        export class Dash extends L.Class implements SymbolFactory {
            constructor(options?: L.Symbol.DashOptions);

            buildSymbol(dirPoint: L.DirectedPoint, latLngs: L.LatLng[], map: L.Map, index: number, total: number): L.Polyline;
        }

        export interface ArrowHeadOptions extends L.Symbol.DashOptions {
            polygon?: boolean;
            headAngle?: number;
        }

        export class ArrowHead extends L.Class implements SymbolFactory {
            constructor(options?: L.Symbol.ArrowHeadOptions);

            buildSymbol(dirPoint: L.DirectedPoint, latLngs: L.LatLng[], map: L.Map, index: number, total: number): L.Polyline;
        }

        export interface MarkerOptions {
            markerOptions?: L.MarkerOptions;
            rotate?: boolean;
        }

        export class Marker extends L.Class implements SymbolFactory {
            constructor(options?: L.Symbol.MarkerOptions);

            buildSymbol(dirPoint: L.DirectedPoint, latLngs: L.LatLng[], map: L.Map, index: number, total: number): L.Polyline;
        }
    }
}