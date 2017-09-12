using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
using Liaohuqiu.UltraPullToRefresh.Utils;
using System;

namespace Sample.Views.Header
{
    public class RentalsSunDrawable : Drawable, IAnimatable
    {
        private const float ScaleStartPercent = 0.3f;
        private const int AnimationDuration = 1000;

        private const float SKY_RATIO = 0.65f;
        private const float SKY_INITIAL_SCALE = 1.05f;

        private const float TownRatio = 0.22f;
        private const float TownInitialScale = 1.20f;
        private const float TownFinalScale = 1.30f;

        private const float SunFinalScale = 0.75f;
        private const float SunInitialRotateGrowth = 1.2f;
        private const float SunFinalRotateGrowth = 1.5f;

        private readonly IInterpolator _linearInterpolator = new LinearInterpolator();

        private readonly View _parent;
        private readonly Matrix _matrix;
        private Animation _animation;

        private int _top;
        private int _screenWidth;

        private int _skyHeight;
        private float _skyTopOffset;
        private float _skyMoveOffset;

        private int _townHeight;
        private float _townInitialTopOffset;
        private float _townFinalTopOffset;
        private float _townMoveOffset;

        private const int SunSize = 100;
        private float _sunLeftOffset;
        private float _sunTopOffset;

        private float _percent;
        private float _rotate;

        private Bitmap _sky;
        private Bitmap _sun;
        private Bitmap _town;

        private bool isRefreshing;

        private readonly Context _context;
        private int _totalDragDistance;

        public bool IsRunning => false;

        public override int Opacity => (int)Format.Translucent;

        public RentalsSunDrawable(Context context, View parent)
        {
            _context = context;
            _parent = parent;

            _matrix = new Matrix();

            InitiateDimens();
            CreateBitmaps();
            SetupAnimations();
        }

        private void InitiateDimens()
        {
            PtrLocalDisplay.Init(_context);
            _totalDragDistance = PtrLocalDisplay.Dp2px(120);

            _screenWidth = _context.Resources.DisplayMetrics.WidthPixels;
            _skyHeight = (int)(SKY_RATIO * _screenWidth);
            _skyTopOffset = -(_skyHeight * 0.28f);
            _skyMoveOffset = PtrLocalDisplay.DesignedDP2px(15);

            _townHeight = (int)(TownRatio * _screenWidth);
            _townInitialTopOffset = (_totalDragDistance - _townHeight * TownInitialScale) + _totalDragDistance * .42f;
            _townFinalTopOffset = (_totalDragDistance - _townHeight * TownFinalScale) + _totalDragDistance * .42f;
            _townMoveOffset = PtrLocalDisplay.DesignedDP2px(10);

            _sunLeftOffset = 0.3f * (float)_screenWidth;
            _sunTopOffset = (_totalDragDistance * 0.5f);

            _top = 0;
        }

        private void CreateBitmaps()
        {
            _sky = BitmapFactory.DecodeResource(_context.Resources, Resource.Drawable.sky);
            _sky = Bitmap.CreateScaledBitmap(_sky, _screenWidth, _skyHeight, true);
            _town = BitmapFactory.DecodeResource(_context.Resources, Resource.Drawable.buildings);
            _town = Bitmap.CreateScaledBitmap(_town, _screenWidth, (int)(_screenWidth * TownRatio), true);
            _sun = BitmapFactory.DecodeResource(_context.Resources, Resource.Drawable.sun);
            _sun = Bitmap.CreateScaledBitmap(_sun, SunSize, SunSize, true);
        }

        public void OffsetTopAndBottom(int offset)
        {
            _top = offset;
            InvalidateSelf();
        }

        public override void Draw(Canvas canvas)
        {
            int saveCount = canvas.Save();
            canvas.Translate(0, _totalDragDistance - _top);

            DrawSky(canvas);
            DrawSun(canvas);
            DrawTown(canvas);

            canvas.RestoreToCount(saveCount);
        }

        private void DrawSky(Canvas canvas)
        {
            Matrix matrix = _matrix;
            matrix.Reset();
            int y = Math.Max(0, _top - _totalDragDistance);

            //  0  ~ 1
            float dragPercent = Math.Min(1f, Math.Abs(_percent));

            /** Change skyScale between {@link #SKY_INITIAL_SCALE} and 1.0f depending on {@link #mPercent} */
            float skyScale;
            float scalePercentDelta = dragPercent - ScaleStartPercent;

            /** less than {@link SCALE_START_PERCENT} will be {@link SKY_INITIAL_SCALE} */
            if (scalePercentDelta > 0)
            {
                /** will change from 0 ~ 1 **/
                float scalePercent = scalePercentDelta / (1.0f - ScaleStartPercent);
                skyScale = SKY_INITIAL_SCALE - (SKY_INITIAL_SCALE - 1.0f) * scalePercent;
            }
            else
            {
                skyScale = SKY_INITIAL_SCALE;
            }

            float offsetX = -(_screenWidth * skyScale - _screenWidth) / 2.0f;

            float offsetY = y + 50 + _skyTopOffset // Offset canvas moving, goes lower when goes down
                    - _skyHeight * (skyScale - 1.0f) / 2 // Offset sky scaling, lower than 0, will go greater when goes down
                    + _skyMoveOffset * dragPercent; // Give it a little move top -> bottom  // will go greater when goes down

            matrix.PostScale(skyScale, skyScale);
            matrix.PostTranslate(offsetX, offsetY);
            canvas.DrawBitmap(_sky, matrix, null);
        }

        private void DrawTown(Canvas canvas)
        {
            Matrix matrix = _matrix;
            matrix.Reset();

            int y = Math.Max(0, _top - _totalDragDistance);
            float dragPercent = Math.Min(1f, Math.Abs(_percent));

            float townScale;
            float townTopOffset;
            float townMoveOffset;
            float scalePercentDelta = dragPercent - ScaleStartPercent;
            if (scalePercentDelta > 0)
            {
                /**
                 * Change townScale between {@link #TOWN_INITIAL_SCALE} and {@link #TOWN_FINAL_SCALE} depending on {@link #mPercent}
                 * Change townTopOffset between {@link #mTownInitialTopOffset} and {@link #mTownFinalTopOffset} depending on {@link #mPercent}
                 */
                float scalePercent = scalePercentDelta / (1.0f - ScaleStartPercent);
                townScale = TownInitialScale + (TownFinalScale - TownInitialScale) * scalePercent;
                townTopOffset = _townInitialTopOffset - (_townFinalTopOffset - _townInitialTopOffset) * scalePercent;
                townMoveOffset = _townMoveOffset * (1.0f - scalePercent);
            }
            else
            {
                float scalePercent = dragPercent / ScaleStartPercent;
                townScale = TownInitialScale;
                townTopOffset = _townInitialTopOffset;
                townMoveOffset = _townMoveOffset * scalePercent;
            }

            float offsetX = -(_screenWidth * townScale - _screenWidth) / 2.0f;
            // float offsetY = (1.0f - dragPercent) * mTotalDragDistance // Offset canvas moving
            float offsetY = y +
                    +townTopOffset
                    - _townHeight * (townScale - 1.0f) / 2 // Offset town scaling
                    + townMoveOffset; // Give it a little move

            matrix.PostScale(townScale, townScale);
            matrix.PostTranslate(offsetX, offsetY);

            canvas.DrawBitmap(_town, matrix, null);
        }

        private void DrawSun(Canvas canvas)
        {
            Matrix matrix = _matrix;
            matrix.Reset();

            float dragPercent = _percent;
            if (dragPercent > 1.0f)
            { // Slow down if pulling over set height
                dragPercent = (dragPercent + 9.0f) / 10;
            }

            float sunRadius = (float)SunSize / 2.0f;
            float sunRotateGrowth = SunInitialRotateGrowth;

            float offsetX = _sunLeftOffset;
            float offsetY = _sunTopOffset
                    + (_totalDragDistance / 2) * (1.0f - dragPercent); // Move the sun up

            float scalePercentDelta = dragPercent - ScaleStartPercent;
            if (scalePercentDelta > 0)
            {
                float scalePercent = scalePercentDelta / (1.0f - ScaleStartPercent);
                float sunScale = 1.0f - (1.0f - SunFinalScale) * scalePercent;
                sunRotateGrowth += (SunFinalRotateGrowth - SunInitialRotateGrowth) * scalePercent;

                matrix.PreTranslate(offsetX + (sunRadius - sunRadius * sunScale), offsetY * (2.0f - sunScale));
                matrix.PreScale(sunScale, sunScale);

                offsetX += sunRadius;
                offsetY = offsetY * (2.0f - sunScale) + sunRadius * sunScale;
            }
            else
            {
                matrix.PostTranslate(offsetX, offsetY);

                offsetX += sunRadius;
                offsetY += sunRadius;
            }

            float r = (isRefreshing ? -360 : 360) * _rotate * (isRefreshing ? 1 : sunRotateGrowth);
            matrix.PostRotate(r, offsetX, offsetY);

            canvas.DrawBitmap(_sun, matrix, null);
        }

        public void SetPercent(float percent)
        {
            _percent = percent;
            SetRotate(percent);
        }

        public void SetRotate(float rotate)
        {
            _rotate = rotate;
            _parent.Invalidate();
            InvalidateSelf();
        }

        public override void SetAlpha(int alpha)
        {
            // Intentionally left empty
        }

        public override void SetBounds(int left, int top, int right, int bottom)
        {
            base.SetBounds(left, top, right, _skyHeight + top);
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            // Intentionally left empty
        }

        public void Start()
        {
            _animation.Reset();
            isRefreshing = true;
            _parent.StartAnimation(_animation);
        }

        public void Stop()
        {
            _parent.ClearAnimation();
            isRefreshing = false;
            ResetOriginals();
        }

        public void ResetOriginals()
        {
            SetPercent(0);
            SetRotate(0);
        }

        private void SetupAnimations()
        {
            _animation = new Animate.RotateAnimation(this, _context)
            {
                RepeatCount = Animation.Infinite,
                RepeatMode = RepeatMode.Restart,
                Interpolator = _linearInterpolator,
                Duration = AnimationDuration
            };
        }

        public int getTotalDragDistance()
        {
            return _totalDragDistance;
        }
    }
}