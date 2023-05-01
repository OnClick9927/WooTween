
using System;

namespace WooTween
{
    public abstract class Tween : TweenObject, ITween
    {
        private bool _autoRecyle = true;
        private TweenDirection _direction = TweenDirection.Forward;
        public TweenDirection direction { get { return _direction; } protected set { _direction = value; } }

        public event Action onCompelete;
        public float duration;
        public bool autoRecyle { get { return _autoRecyle; }set { _autoRecyle = value; } }
        public bool snap { get; set; }
        public LoopType loopType { get; set; }
        public abstract int loop { get; set; }
        protected static IPercentConverter defaultConverter = EaseCoverter.Default;
        public abstract IPercentConverter converter { get; set; }


        public abstract void Run();
        public abstract void ReStart();
        public abstract void Rewind(float duration,bool snap=false);
        public abstract void Complete(bool invoke);

        protected void InvokeCompelete()
        {
            if (onCompelete!=null)
            {
                onCompelete.Invoke();
            }
        }
        protected override void Reset()
        {
            snap = false;
            onCompelete = null;
        }

        public ITween SetConverter(IPercentConverter converter)
        {
            var last = this.converter;
            this.converter = converter;
            if (last != null && last != defaultConverter)
                last.Recyle();
            return this;
        }
        public void WaitToRun()
        {
            TweenDrive container = TweenDrive.GetDrive(envType);
            container.WaitRun(this);
        }
    }
}
