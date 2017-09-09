using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using IN.Srain.Cube.Views.Ptr;
using IN.Srain.Cube.Views.Ptr.Indicator;

namespace Sample.Views
{
    [Register("sample.views." + nameof(RentalsSunHeaderView))]
    public class RentalsSunHeaderView : View, IPtrUIHandler
    {
        private RentalsSunDrawable mDrawable;
        private PtrFrameLayout mPtrFrameLayout;
        private PtrTensionIndicator mPtrTensionIndicator;

        public RentalsSunHeaderView(Context context)
            : this(context, null)
        {
        }

        public RentalsSunHeaderView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public RentalsSunHeaderView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public void SetUp(PtrFrameLayout ptrFrameLayout)
        {
            mPtrFrameLayout = ptrFrameLayout;
            mPtrTensionIndicator = new PtrTensionIndicator();
            mPtrFrameLayout.SetPtrIndicator(mPtrTensionIndicator);
        }

        private void Init()
        {
            mDrawable = new RentalsSunDrawable(Context, this);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = mDrawable.getTotalDragDistance() * 5 / 4;
            heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height + PaddingTop + PaddingBottom, MeasureSpecMode.Exactly);
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            int pl = PaddingLeft;
            int pt = PaddingTop;
            mDrawable.SetBounds(pl, pt, pl + right - left, pt + bottom - top);
        }

        public void OnUIPositionChange(PtrFrameLayout frame, bool isUnderTouch, sbyte status, PtrIndicator ptrIndicator)
        {
            float percent = mPtrTensionIndicator.OverDragPercent;
            mDrawable.OffsetTopAndBottom(mPtrTensionIndicator.CurrentPosY);
            mDrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshBegin(PtrFrameLayout frame)
        {
            mDrawable.Start();
            float percent = mPtrTensionIndicator.OverDragPercent;
            mDrawable.OffsetTopAndBottom(mPtrTensionIndicator.CurrentPosY);
            mDrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshComplete(PtrFrameLayout frame)
        {
            float percent = mPtrTensionIndicator.OverDragPercent;
            mDrawable.Stop();
            mDrawable.OffsetTopAndBottom(mPtrTensionIndicator.CurrentPosY);
            mDrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshPrepare(PtrFrameLayout frame)
        {
            // Intentionally left empty
        }

        public void OnUIReset(PtrFrameLayout frame)
        {
            mDrawable.ResetOriginals();
        }

        protected override void OnDraw(Canvas canvas)
        {
            mDrawable.Draw(canvas);
            float percent = mPtrTensionIndicator.OverDragPercent;
        }

        public override void InvalidateDrawable(Drawable drawable)
        {
            if (drawable == mDrawable)
            {
                Invalidate();
            }
            else
            {
                base.InvalidateDrawable(drawable);
            }
        }
    }
}