using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Vision.Barcodes;
using static Android.Gms.Vision.Detector;
using Android.Graphics;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Graphics.Drawables;
using Android.Text;
using System.Threading;
using Android.Content.Res;
using System.Xml;
using System.IO;

namespace StoreManager
{
    [Activity(Label = "StoreManager", Icon = "@drawable/store", Theme = "@style/MyTheme", ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation)]
    public class ScanActivity : Activity, ISurfaceHolderCallback, IProcessor
    {
        #region Variablen
        SurfaceView surfaceview;
        CameraSource cSource;
        BarcodeDetector detector;
        #endregion
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScanLayout);
            mode = this.Intent.GetStringExtra("mode");
            InitActionBar();
            surfaceview = FindViewById<SurfaceView>(Resource.Id.surfaceView1);
            detector = new BarcodeDetector.Builder(this)
                .SetBarcodeFormats(BarcodeFormat.Ean13 | BarcodeFormat.Ean8 | BarcodeFormat.QrCode)
                .Build();
            cSource = new CameraSource.Builder(this, detector)
                .SetRequestedPreviewSize(1000, 1000)
                .SetAutoFocusEnabled(true)
                .Build();
            surfaceview.Holder.AddCallback(this);
            detector.SetProcessor(this);
        }
        #region Camera
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height) { }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                cSource.Start(surfaceview.Holder);
            }
            catch (Exception ex)
            { }
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            try
            {
                cSource.Stop();
            }
            catch (Exception ex)
            { }
        }
        #endregion
        #region Detector
        bool detected = false;
        public void ReceiveDetections(Detections detections)
        {
            if (detected) { return; }
            detected = true;
            var items = detections.DetectedItems;
            if (items.Size() != 0)
            {
                RunOnUiThread(delegate
                {
                    var vibr = (Vibrator)GetSystemService(Android.Content.Context.VibratorService);
                    if (vibr.HasVibrator) { vibr.Vibrate(500); }
                    var txt = ((Barcode)items.ValueAt(0)).RawValue;
                    ProgressData(txt);
                    Toast.MakeText(this, txt, ToastLength.Long).Show();
                });
            }
            else
            {
                detected = false;
            }
        }
        string mode;
        void ProgressData(string txt)
        {
            var xmldoc = new XmlDocument();
            bool found = false;
            xmldoc.Load(MainActivity.Path);
            foreach (XmlNode node in xmldoc.GetElementsByTagName("product"))
            {
                try
                {
                    System.Diagnostics.Debug.Print(node.OuterXml);
                    var ac = node.ChildNodes[1];
                    var ad = ac.InnerText;
                    if (txt == ad)
                    {
                        found = true;
                        detected = true;
                        if (mode == "add")
                        {
                            node.ChildNodes[2].InnerXml = (int.Parse(node.ChildNodes[2].InnerText) + 1).ToString();
                        }
                        else if (mode == "remove")
                        {
                            node.ChildNodes[2].InnerXml = (int.Parse(node.ChildNodes[2].InnerText) - 1).ToString();
                        }
                        xmldoc.Save(MainActivity.Path);
                        this.Finish();
                        return;
                    }
                } catch { }
            }
            if (!found)
            {
                EditText edt = FindViewById<EditText>(Resource.Id.editText1);
                var dialog1 = new AlertDialog.Builder(this)
                    .SetView(Resource.Layout.Dialog1)
                    .SetNegativeButton("Abbrechen", delegate { })
                    .SetPositiveButton("Speichern", delegate {
                        var n = xmldoc.CreateElement("product");
                        n.InnerXml = "<name>" + edt.Text + "</name>" + System.Environment.NewLine + "<id>" + txt + "</id>" + System.Environment.NewLine + "<count>1</count>";
                        xmldoc.GetElementsByTagName("product")[0].ParentNode.AppendChild(n);
                        xmldoc.Save(MainActivity.Path);
                        this.Finish();
                        return;
                    })
                    .Show();
                edt = dialog1.FindViewById<EditText>(Resource.Id.editText1);
            }
        }
        void InitActionBar()
        {
            ActionBar.SetBackgroundDrawable(new ColorDrawable(Android.Graphics.Color.ParseColor("#1f669b")));
            ActionBar.TitleFormatted = Html.FromHtml("<font color='#FFFFFF'>" + this.Title + "</font>");
            ActionBar.SubtitleFormatted = Html.FromHtml("<font color='#D3D3D3'>" + "Scannen" + "</font>");
        }
        public void Release() { }
        public static Android.Hardware.Camera GetCamera(CameraSource cameraSource)
        {
            var javaHero = cameraSource.JavaCast<Java.Lang.Object>();
            var fields = javaHero.Class.GetDeclaredFields();
            foreach (var field in fields)
            {
                if (field.Type.CanonicalName.Equals("android.hardware.camera", StringComparison.OrdinalIgnoreCase))
                {
                    field.Accessible = true;
                    var camera = field.Get(javaHero);
                    var cCamera = (Android.Hardware.Camera)camera;
                    return cCamera;
                }
            }

            return null;
        }
        public void ToggleFlashLight(Android.Hardware.Camera _myCamera)
        {
            var prams = _myCamera.GetParameters();
            if(prams.FlashMode == Android.Hardware.Camera.Parameters.FlashModeOn)
            {
                prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOff;
            }
            else
            {
                prams.FlashMode = Android.Hardware.Camera.Parameters.FlashModeOn;
            }
            _myCamera.SetParameters(prams);
        }
        #endregion
    }
}