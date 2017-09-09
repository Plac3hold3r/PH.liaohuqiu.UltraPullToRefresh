using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.Animations;
using IN.Srain.Cube.Views.Ptr.Util;
using System;

namespace Sample.Views
{
    public class RentalsSunDrawable : Drawable, IAnimatable
    {
        private static float SCALE_START_PERCENT = 0.3f;
        private static int ANIMATION_DURATION = 1000;

        private static float SKY_RATIO = 0.65f;
        private static float SKY_INITIAL_SCALE = 1.05f;

        private static float TOWN_RATIO = 0.22f;
        private static float TOWN_INITIAL_SCALE = 1.20f;
        private static float TOWN_FINAL_SCALE = 1.30f;

        private static float SUN_FINAL_SCALE = 0.75f;
        private static float SUN_INITIAL_ROTATE_GROWTH = 1.2f;
        private static float SUN_FINAL_ROTATE_GROWTH = 1.5f;

        private static IInterpolator LINEAR_INTERPOLATOR = new LinearInterpolator();

        private View mParent;
        private Matrix mMatrix;
        private Animation mAnimation;

        private int mTop;
        private int mScreenWidth;

        private int mSkyHeight;
        private float mSkyTopOffset;
        private float mSkyMoveOffset;

        private int mTownHeight;
        private float mTownInitialTopOffset;
        private float mTownFinalTopOffset;
        private float mTownMoveOffset;

        private int mSunSize = 100;
        private float mSunLeftOffset;
        private float mSunTopOffset;

        private float mPercent = 0.0f;
        private float mRotate = 0.0f;

        private Bitmap mSky;
        private Bitmap mSun;
        private Bitmap mTown;

        private bool isRefreshing = false;

        private Context mContext;
        private int mTotalDragDistance;

        public bool IsRunning => false;

        public override int Opacity => (int)Format.Translucent;

        public RentalsSunDrawable(Context context, View parent)
        {
            mContext = context;
            mParent = parent;

            mMatrix = new Matrix();

            InitiateDimens();
            CreateBitmaps();
            SetupAnimations();
        }

        private void InitiateDimens()
        {
            PtrLocalDisplay.Init(mContext);
            mTotalDragDistance = PtrLocalDisplay.Dp2px(120);

            mScreenWidth = mContext.Resources.DisplayMetrics.WidthPixels;
            mSkyHeight = (int)(SKY_RATIO * mScreenWidth);
            mSkyTopOffset = -(mSkyHeight * 0.28f);
            mSkyMoveOffset = PtrLocalDisplay.DesignedDP2px(15);

            mTownHeight = (int)(TOWN_RATIO * mScreenWidth);
            mTownInitialTopOffset = (mTotalDragDistance - mTownHeight * TOWN_INITIAL_SCALE) + mTotalDragDistance * .42f;
            mTownFinalTopOffset = (mTotalDragDistance - mTownHeight * TOWN_FINAL_SCALE) + mTotalDragDistance * .42f;
            mTownMoveOffset = PtrLocalDisplay.DesignedDP2px(10);

            mSunLeftOffset = 0.3f * (float)mScreenWidth;
            mSunTopOffset = (mTotalDragDistance * 0.5f);

            mTop = 0;
        }

        private void CreateBitmaps()
        {
            mSky = BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.sky);
            mSky = Bitmap.CreateScaledBitmap(mSky, mScreenWidth, mSkyHeight, true);
            mTown = BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.buildings);
            mTown = Bitmap.CreateScaledBitmap(mTown, mScreenWidth, (int)(mScreenWidth * TOWN_RATIO), true);
            mSun = BitmapFactory.DecodeResource(mContext.Resources, Resource.Drawable.sun);
            mSun = Bitmap.CreateScaledBitmap(mSun, mSunSize, mSunSize, true);
        }

        public void OffsetTopAndBottom(int offset)
        {
            mTop = offset;
            InvalidateSelf();
        }

        public override void Draw(Canvas canvas)
        {
            int saveCount = canvas.Save();
            canvas.Translate(0, mTotalDragDistance - mTop);

            DrawSky(canvas);
            DrawSun(canvas);
            DrawTown(canvas);

            canvas.RestoreToCount(saveCount);
        }

        private void DrawSky(Canvas canvas)
        {
            Matrix matrix = mMatrix;
            matrix.Reset();
            int y = Math.Max(0, mTop - mTotalDragDistance);

            //  0  ~ 1
            float dragPercent = Math.Min(1f, Math.Abs(mPercent));

            /** Change skyScale between {@link #SKY_INITIAL_SCALE} and 1.0f depending on {@link #mPercent} */
            float skyScale;
            float scalePercentDelta = dragPercent - SCALE_START_PERCENT;

            /** less than {@link SCALE_START_PERCENT} will be {@link SKY_INITIAL_SCALE} */
            if (scalePercentDelta > 0)
            {
                /** will change from 0 ~ 1 **/
                float scalePercent = scalePercentDelta / (1.0f - SCALE_START_PERCENT);
                skyScale = SKY_INITIAL_SCALE - (SKY_INITIAL_SCALE - 1.0f) * scalePercent;
            }
            else
            {
                skyScale = SKY_INITIAL_SCALE;
            }

            float offsetX = -(mScreenWidth * skyScale - mScreenWidth) / 2.0f;

            float offsetY = y + 50 + mSkyTopOffset // Offset canvas moving, goes lower when goes down
                    - mSkyHeight * (skyScale - 1.0f) / 2 // Offset sky scaling, lower than 0, will go greater when goes down
                    + mSkyMoveOffset * dragPercent; // Give it a little move top -> bottom  // will go greater when goes down

            matrix.PostScale(skyScale, skyScale);
            matrix.PostTranslate(offsetX, offsetY);
            canvas.DrawBitmap(mSky, matrix, null);
        }

        private void DrawTown(Canvas canvas)
        {
            Matrix matrix = mMatrix;
            matrix.Reset();

            int y = Math.Max(0, mTop - mTotalDragDistance);
            float dragPercent = Math.Min(1f, Math.Abs(mPercent));

            float townScale;
            float townTopOffset;
            float townMoveOffset;
            float scalePercentDelta = dragPercent - SCALE_START_PERCENT;
            if (scalePercentDelta > 0)
            {
                /**
                 * Change townScale between {@link #TOWN_INITIAL_SCALE} and {@link #TOWN_FINAL_SCALE} depending on {@link #mPercent}
                 * Change townTopOffset between {@link #mTownInitialTopOffset} and {@link #mTownFinalTopOffset} depending on {@link #mPercent}
                 */
                float scalePercent = scalePercentDelta / (1.0f - SCALE_START_PERCENT);
                townScale = TOWN_INITIAL_SCALE + (TOWN_FINAL_SCALE - TOWN_INITIAL_SCALE) * scalePercent;
                townTopOffset = mTownInitialTopOffset - (mTownFinalTopOffset - mTownInitialTopOffset) * scalePercent;
                townMoveOffset = mTownMoveOffset * (1.0f - scalePercent);
            }
            else
            {
                float scalePercent = dragPercent / SCALE_START_PERCENT;
                townScale = TOWN_INITIAL_SCALE;
                townTopOffset = mTownInitialTopOffset;
                townMoveOffset = mTownMoveOffset * scalePercent;
            }

            float offsetX = -(mScreenWidth * townScale - mScreenWidth) / 2.0f;
            // float offsetY = (1.0f - dragPercent) * mTotalDragDistance // Offset canvas moving
            float offsetY = y +
                    +townTopOffset
                    - mTownHeight * (townScale - 1.0f) / 2 // Offset town scaling
                    + townMoveOffset; // Give it a little move

            matrix.PostScale(townScale, townScale);
            matrix.PostTranslate(offsetX, offsetY);

            canvas.DrawBitmap(mTown, matrix, null);
        }

        private void DrawSun(Canvas canvas)
        {
            Matrix matrix = mMatrix;
            matrix.Reset();

            float dragPercent = mPercent;
            if (dragPercent > 1.0f)
            { // Slow down if pulling over set height
                dragPercent = (dragPercent + 9.0f) / 10;
            }

            float sunRadius = (float)mSunSize / 2.0f;
            float sunRotateGrowth = SUN_INITIAL_ROTATE_GROWTH;

            float offsetX = mSunLeftOffset;
            float offsetY = mSunTopOffset
                    + (mTotalDragDistance / 2) * (1.0f - dragPercent); // Move the sun up

            float scalePercentDelta = dragPercent - SCALE_START_PERCENT;
            if (scalePercentDelta > 0)
            {
                float scalePercent = scalePercentDelta / (1.0f - SCALE_START_PERCENT);
                float sunScale = 1.0f - (1.0f - SUN_FINAL_SCALE) * scalePercent;
                sunRotateGrowth += (SUN_FINAL_ROTATE_GROWTH - SUN_INITIAL_ROTATE_GROWTH) * scalePercent;

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

            float r = (isRefreshing ? -360 : 360) * mRotate * (isRefreshing ? 1 : sunRotateGrowth);
            matrix.PostRotate(r, offsetX, offsetY);

            canvas.DrawBitmap(mSun, matrix, null);
        }

        public void SetPercent(float percent)
        {
            mPercent = percent;
            SetRotate(percent);
        }

        public void SetRotate(float rotate)
        {
            mRotate = rotate;
            mParent.Invalidate();
            InvalidateSelf();
        }

        public override void SetAlpha(int alpha)
        {
            // Intentionally left empty
        }

        public override void SetBounds(int left, int top, int right, int bottom)
        {
            base.SetBounds(left, top, right, mSkyHeight + top);
        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {
            // Intentionally left empty
        }

        public void Start()
        {
            mAnimation.Reset();
            isRefreshing = true;
            mParent.StartAnimation(mAnimation);
        }

        public void Stop()
        {
            mParent.ClearAnimation();
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
            mAnimation = new RotateAnimation(this, mContext);
            mAnimation.RepeatCount = Animation.Infinite;
            mAnimation.RepeatMode = RepeatMode.Restart;
            mAnimation.Interpolator = LINEAR_INTERPOLATOR;
            mAnimation.Duration = ANIMATION_DURATION;
        }

        public int getTotalDragDistance()
        {
            return mTotalDragDistance;
        }
    }

    public class RotateAnimation : Animation
    {
        RentalsSunDrawable _control;

        public RotateAnimation(RentalsSunDrawable control, Context context) : base(context, null)
        {
            _control = control;
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            _control.SetRotate(interpolatedTime);
        }
    }
}