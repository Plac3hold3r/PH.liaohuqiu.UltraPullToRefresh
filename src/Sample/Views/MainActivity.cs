using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech;
using Bumptech.Request;
using Bumptech.Request.Target;
using IN.Srain.Cube.Views.Ptr;
using System;

namespace Sample.Views
{
    [Activity(
        Label = "Sample",
        MainLauncher = true)]
    public class MainActivity : Activity, IPtrHandler, IRequestListener
    {
        private int currentImageIndex = -1;

        private ImageView imageView;
        private PtrFrameLayout refreshFrame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            imageView = FindViewById<ImageView>(Resource.Id.material_style_image_view);
            refreshFrame = FindViewById<PtrFrameLayout>(Resource.Id.material_style_ptr_frame);

            var header = new RentalsSunHeaderView(this)
            {
                LayoutParameters = new PtrFrameLayout.LayoutParams(-1, -2)
            };
            header.SetPadding(0, (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 15, Resources.DisplayMetrics), 0, (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Resources.DisplayMetrics));
            header.SetUp(refreshFrame);

            refreshFrame.SetLoadingMinTime(1000);
            refreshFrame.SetDurationToCloseHeader(1500);
            refreshFrame.HeaderView = header;
            refreshFrame.AddPtrUIHandler(header);

            refreshFrame.PostDelayed(() => refreshFrame.AutoRefresh(true), 100);

            refreshFrame.SetPtrHandler(this);
        }

        #region Pull to refresh callbacks

        public bool CheckCanDoRefresh(PtrFrameLayout frame, View content, View header)
        {
            return true;
        }

        public void OnRefreshBegin(PtrFrameLayout frame)
        {
            Glide.With(this).Load(GetImageUrl()).Listener(this).Into(imageView);
        }

        #endregion Pull to refresh callbacks

        #region Image Loading callbacks

        public bool OnException(Java.Lang.Exception p0, Java.Lang.Object p1, ITarget p2, bool p3)
        {
            Toast.MakeText(this, Resource.String.error_showing_image, ToastLength.Long)
                .Show();

            return false;
        }

        public bool OnResourceReady(Java.Lang.Object p0, Java.Lang.Object p1, ITarget p2, bool p3, bool p4)
        {
            refreshFrame.RefreshComplete();
            return false;
        }

        #endregion Image Loading callbacks

        #region Helpers

        private string GetImageUrl()
        {
            var random = new Random();

            int nextIndex = currentImageIndex;

            while (nextIndex == currentImageIndex)
                nextIndex = random.Next(0, 10);

            currentImageIndex = nextIndex;

            var urls = Resources.GetStringArray(Resource.Array.image_urls);
            return urls[currentImageIndex];
        }

        #endregion Helpers
    }
}