﻿
using System;

namespace WooTween
{
    public class SingleTween<T> : Tween, ISingleTween<T> where T : struct
    {
        private TweenValue<T> value;
        private T _end;
        private T _start;
        private IPercentConverter _converter = defaultConverter;
        private Action<T> setter;
        private Func<T> getter;
        private int _loop = 1;
        private int current_loop = 0;

        public T end { get { return _end; } set { _end = value; } }
        public T start { get { return _start; } set { _start = value; } }

        public override int loop
        {
            get => _loop;
            set => _loop = value;
        }
        public override IPercentConverter converter
        {
            get => _converter;
            set
            {
                _converter = value;
                if (this.value != null)
                {
                    this.value.converter = value;
                }
            }
        }


        public virtual void Config(T start, T end, float duration, Func<T> getter, Action<T> setter, bool snap)
        {
            this.snap = snap;
            this._start = start;
            this._end = end;
            this.duration = duration;
            this.getter = getter;
            this.setter = setter;
        }

        private void OnLoopBegin()
        {
            UnbindTweenValue();
            value = TweenValue.Get<T>(envType);
            value.converter = converter;
            var plugin = new TweenPlugin<T>();
            switch (loopType)
            {
                case LoopType.ReStart:
                    {
                        plugin.Config(start, end, duration, getter, setter, snap);
                    }
                    break;
                case LoopType.PingPong:
                    if (direction == TweenDirection.Forward)
                    {
                        plugin.Config(start, end, duration, getter, setter, snap);
                        direction = TweenDirection.Back;
                    }
                    else
                    {
                        plugin.Config(end, start, duration, getter, setter, snap);
                        direction = TweenDirection.Forward;
                    }
                    break;
                default:
                    break;
            }
            value.Config(plugin, OnLoopEnd);
            value.Run();
        }
        private void OnLoopEnd()
        {
            UnbindTweenValue();
            current_loop--;
            if (loop != -1 && current_loop <= 0)
            {
                InvokeComplete();
                RecyleSelf();
            }
            else
            {

                OnLoopBegin();
            }
        }

        public override void Run()
        {
            current_loop = loop;
            direction = TweenDirection.Forward;
            OnLoopBegin();
        }





        public override void ReStart()
        {
            direction = TweenDirection.Forward;
            Run();
        }
        public override void Complete(bool invoke)
        {
            direction = TweenDirection.Forward;
            if (invoke) InvokeComplete();
            RecyleSelf();
        }
        public override void Rewind(float duration, bool snap = false)
        {
            UnbindTweenValue();
            direction = TweenDirection.Forward;
            value = TweenValue.Get<T>(envType);
            value.converter = converter;
            var plugin = new TweenPlugin<T>();
            var current = getter();
            plugin.Config(current, start, duration, getter, setter, snap);
            value.Config(plugin, RecyleSelf);
            value.Run();
        }
        protected override void Reset()
        {
            base.Reset();
            direction = TweenDirection.Forward;
            UnbindTweenValue();
            _start = _end = default(T);
            duration = 0;
            _loop = 1;
            autoRecycle = true;
            if (_converter != defaultConverter)
            {
                _converter.Recycle();
                _converter = defaultConverter;
            }
            loopType = LoopType.ReStart;
            setter = null;
            getter = null;
        }

        private void RecyleSelf()
        {
            UnbindTweenValue();
            if (autoRecycle)
            {
                Recycle();
            }
        }
        private void UnbindTweenValue()
        {
            if (value != null)
            {
                value.ResetPlugin();
                value = null;
            }
        }
    }
}
