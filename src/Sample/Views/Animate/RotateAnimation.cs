using Android.Content;
using Android.Views.Animations;
using Sample.Views.Header;

namespace Sample.Views.Animate
{
    public class RotateAnimation : Animation
    {
        private readonly RentalsSunDrawable _control;

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