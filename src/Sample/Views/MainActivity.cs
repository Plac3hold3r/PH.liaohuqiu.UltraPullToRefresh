using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using IN.Srain.Cube.Views.Ptr;

namespace Sample.Views
{
    [Activity(
        Label = "Sample",
        MainLauncher = true)]
    public class MainActivity : Activity, IPtrHandler
    {
        private string mUrl = "http://img4.duitang.com/uploads/blog/201407/07/20140707113856_hBf3R.thumb.jpeg";
        private long mStartLoadingTime = -1;
        private bool mImageHasLoaded = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            //CubeImageView imageView = (CubeImageView)view.findViewById(R.id.material_style_image_view);
            //ImageLoader imageLoader = ImageLoaderFactory.create(getContext());

            PtrFrameLayout frame = FindViewById<PtrFrameLayout>(Resource.Id.material_style_ptr_frame);

            // header
            RentalsSunHeaderView header = new RentalsSunHeaderView(this);
            header.LayoutParameters = new PtrFrameLayout.LayoutParams(-1, -2);
            header.SetPadding(0, (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 15, Resources.DisplayMetrics), 0, (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 10, Resources.DisplayMetrics));
            header.SetUp(frame);

            frame.SetLoadingMinTime(1000);
            frame.SetDurationToCloseHeader(1500);
            frame.HeaderView = header;
            frame.AddPtrUIHandler(header);

            frame.PostDelayed(() => frame.AutoRefresh(true), 100);

            frame.SetPtrHandler(this);
        }

        public bool CheckCanDoRefresh(PtrFrameLayout frame, View content, View header)
        {
            return true;
        }

        public void OnRefreshBegin(PtrFrameLayout frame)
        {
            frame.PostDelayed(() => frame.RefreshComplete(), 1500);
        }
    }
}

