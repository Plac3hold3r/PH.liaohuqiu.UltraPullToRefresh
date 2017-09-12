using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using UltraPullToRefresh;
using UltraPullToRefresh.Indicators;

namespace Sample.Views.Header
{
    [Register(nameof(RentalsSunHeaderView))]
    public class RentalsSunHeaderView : View, IPtrUIHandler
    {
        private RentalsSunDrawable _rentalsdrawable;
        private PtrFrameLayout _refreshptrFrameLayout;
        private PtrTensionIndicator _ptrTensionIndicator;

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

        private void Init()
        {
            _rentalsdrawable = new RentalsSunDrawable(Context, this);
        }

        public void SetUp(PtrFrameLayout ptrFrameLayout)
        {
            _refreshptrFrameLayout = ptrFrameLayout;
            _ptrTensionIndicator = new PtrTensionIndicator();
            _refreshptrFrameLayout.SetPtrIndicator(_ptrTensionIndicator);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int height = _rentalsdrawable.getTotalDragDistance() * 5 / 4;
            heightMeasureSpec = MeasureSpec.MakeMeasureSpec(height + PaddingTop + PaddingBottom, MeasureSpecMode.Exactly);
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            int pl = PaddingLeft;
            int pt = PaddingTop;
            _rentalsdrawable.SetBounds(pl, pt, pl + right - left, pt + bottom - top);
        }

        public void OnUIPositionChange(PtrFrameLayout frame, bool isUnderTouch, sbyte status, PtrIndicator ptrIndicator)
        {
            float percent = _ptrTensionIndicator.OverDragPercent;
            _rentalsdrawable.OffsetTopAndBottom(_ptrTensionIndicator.CurrentPosY);
            _rentalsdrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshBegin(PtrFrameLayout frame)
        {
            _rentalsdrawable.Start();
            float percent = _ptrTensionIndicator.OverDragPercent;
            _rentalsdrawable.OffsetTopAndBottom(_ptrTensionIndicator.CurrentPosY);
            _rentalsdrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshComplete(PtrFrameLayout frame)
        {
            float percent = _ptrTensionIndicator.OverDragPercent;
            _rentalsdrawable.Stop();
            _rentalsdrawable.OffsetTopAndBottom(_ptrTensionIndicator.CurrentPosY);
            _rentalsdrawable.SetPercent(percent);
            Invalidate();
        }

        public void OnUIRefreshPrepare(PtrFrameLayout frame)
        {
            // Intentionally left empty
        }

        public void OnUIReset(PtrFrameLayout frame)
        {
            _rentalsdrawable.ResetOriginals();
        }

        protected override void OnDraw(Canvas canvas)
        {
            _rentalsdrawable.Draw(canvas);
            float percent = _ptrTensionIndicator.OverDragPercent;
        }

        public override void InvalidateDrawable(Drawable drawable)
        {
            if (drawable == _rentalsdrawable)
                Invalidate();
            else
                base.InvalidateDrawable(drawable);
        }
    }
}