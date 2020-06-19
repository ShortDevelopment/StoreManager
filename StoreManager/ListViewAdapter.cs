using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace StoreManager
{
    struct ListViewItem
    {
        public string ProduktName { get; set; }
        public int Count { get; set; }
        public ListViewItem(string ProduktName, int Count)
        {
            this.ProduktName = ProduktName;
            this.Count = Count;
        }
    }
    class ListViewAdapter : BaseAdapter<ListViewItem>
    {
        List<ListViewItem> items = new List<ListViewItem>();
        Activity context;
        public ListViewAdapter(Activity activity, List<ListViewItem> _items) : base()
        {
            items = _items;
            context = activity;
        }
        public override ListViewItem this[int position]
        {
            get
            {
                return items[position];
            }
        }
        public override int Count
        {
            get
            {
                return items.Count();
            }
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if(convertView == null)
            {
                convertView = context.LayoutInflater.Inflate(Resource.Layout.lvitem, parent, false);
            }
            var item = this[position];
            convertView.FindViewById<TextView>(Resource.Id.textView1).Text = item.ProduktName;
            convertView.FindViewById<TextView>(Resource.Id.textView2).Text = item.Count.ToString() + " Stück";
            return convertView;
        }
    }
}