
using UnityEngine;

namespace WooTween
{
    class TweenSingleton:MonoBehaviour
    {
        public const EnvironmentType envType = EnvironmentType.RT;
        public static TweenUpdateType updateType = TweenUpdateType.Update;
        TweenDrive container;
        private static TweenSingleton _Instance;

        static TweenSingleton Instance
        {
            get
            {
                if (_Instance==null)
                {
                    _Instance = new GameObject("TweenSingleton").AddComponent<TweenSingleton>();
                    DontDestroyOnLoad(_Instance.gameObject);
                }
                return _Instance;
            }
        }
        public static bool Initialized()
        {
            return Instance == null;
        }
        private void Awake()
        {
             container = TweenDrive.GetDrive(envType);
        }


        private void Update()
        {
            if (updateType== TweenUpdateType.Update)
            {
                TweenValue.deltaTime = Time.deltaTime;

                container.Update();
            }
        }
        private void FixedUpdate()
        {
            if (updateType == TweenUpdateType.FixedUpdate)
            {
                TweenValue.deltaTime = Time.fixedDeltaTime;

                container.Update();
            }
        }
        private void LateUpdate()
        {
            if (updateType == TweenUpdateType.LateUpdate)
            {
                container.Update();
            }
        }
        private void OnDisable()
        {
            container.Dispose();
        }

    }
}
