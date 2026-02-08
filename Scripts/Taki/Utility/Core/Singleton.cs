using UnityEngine;

namespace Taki.Utility.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] instances = FindObjectsByType<T>(FindObjectsSortMode.None);
                    if (instances.Length > 0)
                    {
                        instance = instances[0];
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }
                    }

                    else
                    {
                        Debug.Log(
                            $"シーン内で、" +
                            $"{typeof(T).Name}のシングルトンインスタンスが見つかりませんでした。");
                        return null;
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }

            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}