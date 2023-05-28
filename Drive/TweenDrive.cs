
using System.Collections.Generic;

namespace WooTween
{
    public class TweenDrive : System.IDisposable
    {
        private List<TweenValue> tweens;
        private Queue<TweenValue> queue;
        private Queue<Tween> wait;
        public TweenDrive()
        {
            tweens = new List<TweenValue>();
            queue = new Queue<TweenValue>();
            wait = new Queue<Tween>();
        }


        public void Dispose()
        {
            tweens.Clear();
            queue.Clear();
            wait.Clear();
        }

        public void Update()
        {
            while (wait.Count != 0)
            {
                wait.Dequeue().Run();
            }
            while (queue.Count != 0)
            {
                var tv = queue.Dequeue();
                tweens.Remove(tv);
                tv.Recycle();
            }
            for (int i = tweens.Count - 1; i >= 0; i--)
            {
                var tv = tweens[i];
                tv.Update();
                if (tv.compelete)
                {
                    queue.Enqueue(tv);
                }
            }

        }

        public void Subscribe(TweenValue tv)
        {
            if (tweens.Contains(tv)) return;
            tweens.Add(tv);
        }
        public void WaitRun(Tween tween)
        {
            wait.Enqueue(tween);
        }

        private static Dictionary<EnvironmentType, TweenDrive> dic = new Dictionary<EnvironmentType, TweenDrive>();

        public static TweenDrive GetDrive(EnvironmentType env)
        {
            if (!dic.ContainsKey(env))
            {
                dic.Add(env, new TweenDrive());
            }
            return dic[env];
        }
    }
}
