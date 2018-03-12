﻿using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Support.V4.Content;
using Tabi;
using Tabi.Droid.Renderers;
using Tabi.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(RouteMap), typeof(RouteMapRenderer))]
namespace Tabi.Droid.Renderers
{
    public class RouteMapRenderer : MapRenderer, IMapControl
    {
        GoogleMap googleMap;
        RouteMap formsMap;
        bool mapReady;

        public RouteMapRenderer(Context context) : base(context)
        {
           
        }

        public void Draw()
        {

            // Only run if OnMapReady() has already been called
            if (!mapReady)
            {
                Log.Info("OnMapReady not yet called. Returning");
                return;
            }

            Log.Debug("DrawRoute Executed");


            foreach (Line line in formsMap.Lines)
            {
                PolylineOptions po = new PolylineOptions();
                po.InvokeWidth(8);
                po.InvokeColor(ContextCompat.GetColor(Context, Resource.Color.mapLine));

                foreach (var position in line.Positions)
                {
                    po.Add(new LatLng(position.Latitude, position.Longitude));
                }
                googleMap.AddPolyline(po);

            }
        }

        public void Clear()
        {
            Log.Debug("RouteMap Cleared");

            if(googleMap != null)
            {
                googleMap.Clear();
            }
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe

            }

            if (e.NewElement != null)
            {
                formsMap = (RouteMap)e.NewElement;
                formsMap.MapControl = this;

                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            Log.Debug("GoogleMap Ready");
            mapReady = true;
            this.googleMap = map;

            Draw();
        }
    }
}
