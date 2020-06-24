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

namespace nl.datm.yuuta.Droid
{
    class Sticker : Java.Lang.Object, IParcelable, IParcelableCreator
    {
        internal String imageFileName;
        internal List<String> emojis;
        internal long size;

        internal Sticker(String imageFileName, List<String> emojis)
        {
            this.imageFileName = imageFileName;
            this.emojis = emojis;
        }

        private Sticker(Parcel inparcel)
        {
            imageFileName = inparcel.ReadString();
            emojis = inparcel.CreateStringArrayList().ToList();
            size = inparcel.ReadLong();
        }

        public void setSize(long size)
        {
            this.size = size;
        }


        public Java.Lang.Object CreateFromParcel(Parcel inParcel)
        {
            return new Sticker(inParcel);
        }

        public Java.Lang.Object[] NewArray(int size)
        {
            return new Sticker[size];
        }

        public int DescribeContents()
        {
            return 0;
        }

        public void WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
            dest.WriteString(imageFileName);
            dest.WriteStringList(emojis);
            dest.WriteLong(size);
        }
    }
}