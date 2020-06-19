using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using System.Xml;
using System.IO;
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using System;
using static Android.Gms.Vision.Detector;
using Android.Gms.Vision;
using Android.Content;

namespace StoreManager
{
    [Activity(Label = "StoreManager", MainLauncher = true, Icon = "@drawable/store", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class MainActivity : Activity
    {
        public static string Path { get; } = "/sdcard/storedata.xml";
        ListView lv1;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            InitActionBar();
            SetContentView (Resource.Layout.Main);
            lv1 = FindViewById<ListView>(Resource.Id.listView1);
            update();
        }
        void InitActionBar()
        {
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.ParseColor("#1f669b")));
            ActionBar.TitleFormatted = Html.FromHtml("<font color='#FFFFFF'>" + this.Title + "</font>");
            ActionBar.SubtitleFormatted = Html.FromHtml("<font color='#D3D3D3'>" + "Bestand" + "</font>");
        }
        void update()
        {
            if (!File.Exists(Path))
            {
                using (StreamReader sr = new StreamReader(Assets.Open("data.xml")))
                {
                    File.WriteAllText(Path, sr.ReadToEnd());
                }
            }
            var items = new List<ListViewItem>();

            //foreach (XmlNode node in xmldoc.ChildNodes)
            //{
            //    try
            //    {
            //        System.Diagnostics.Debug.Print(node.ChildNodes[0].ChildNodes[1].InnerXml);
            //        items.Add(new ListViewItem(node.ChildNodes[0].ChildNodes[0].InnerText, int.Parse(node.ChildNodes[0].ChildNodes[2].InnerText)));
            //    }
            //    catch { }
            //}
            var xmldoc = new XmlDocument();
            xmldoc.Load(Path);
            foreach (XmlNode node in xmldoc.GetElementsByTagName("product"))
                {
                    try
                    {
                        System.Diagnostics.Debug.Print(node.OuterXml);
                        items.Add(new ListViewItem(node.ChildNodes[0].InnerText, int.Parse(node.ChildNodes[2].InnerText)));
                    }
                    catch { }
                }
                lv1.Adapter = new ListViewAdapter(this, items);
            try
            {
            }
            catch { }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_add:
                    var i = new Intent(this, typeof(ScanActivity));
                    i.PutExtra("mode", "add");
                    StartActivity(i);
                    break;
                case Resource.Id.menu_remove:
                    var i2 = new Intent(this, typeof(ScanActivity));
                    i2.PutExtra("mode", "remove");
                    StartActivity(i2);
                    break;
                case Resource.Id.menu_settings:

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnRestart()
        {
            update();
            base.OnRestart();
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }        
    }
}

