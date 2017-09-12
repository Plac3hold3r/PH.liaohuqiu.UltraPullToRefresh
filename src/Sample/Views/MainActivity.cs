using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Bumptech;
using Bumptech.Request;
using Bumptech.Request.Target;
using Sample.Views.Header;
using System;
using UltraPullToRefresh;

namespace Sample.Views
{
    [Activity(
        MainLauncher = true,
        Label = "@string/app_name",
        Icon = "@mipmap/ic_launcher")]
    public class MainActivity : Activity, IPtrHandler, IRequestListener
    {
        private int _currentImageIndex = -1;

        private ImageView _imageView;
        private PtrFrameLayout _refreshFrame;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            _imageView = FindViewById<ImageView>(Resource.Id.material_style_image_view);
            _refreshFrame = FindViewById<PtrFrameLayout>(Resource.Id.material_style_ptr_frame);

            var header = new RentalsSunHeaderView(this)
            {
                LayoutParameters = new PtrFrameLayout.LayoutParams(-1, -2)
            };
            header.SetPadding(0, Resources.GetDimensionPixelSize(Resource.Dimension.header_top_padding), 0, Resources.GetDimensionPixelSize(Resource.Dimension.header_bottom_padding));
            header.SetUp(_refreshFrame);

            _refreshFrame.SetLoadingMinTime(1000);
            _refreshFrame.SetDurationToCloseHeader(1500);
            _refreshFrame.HeaderView = header;
            _refreshFrame.AddPtrUIHandler(header);

            _refreshFrame.PostDelayed(() => _refreshFrame.AutoRefresh(true), 100);

            _refreshFrame.SetPtrHandler(this);
        }

        #region Pull to refresh callbacks

        public bool CheckCanDoRefresh(PtrFrameLayout frame, View content, View header)
        {
            return true;
        }

        public void OnRefreshBegin(PtrFrameLayout frame)
        {
            Glide.With(this).Load(GetImageUrl()).Listener(this).Into(_imageView);
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
            _refreshFrame.RefreshComplete();
            return false;
        }

        #endregion Image Loading callbacks

        #region Helpers

        private string GetImageUrl()
        {
            var random = new Random();

            int nextIndex = _currentImageIndex;

            while (nextIndex == _currentImageIndex)
                nextIndex = random.Next(0, 10);

            _currentImageIndex = nextIndex;

            var urls = Resources.GetStringArray(Resource.Array.image_urls);
            return urls[_currentImageIndex];
        }

        #endregion Helpers
    }
}