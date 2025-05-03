using System.Collections.Generic;
using UnityEngine;

namespace sailadex
{
    public class NotificationUiQueue : MonoBehaviour
    {
        public static NotificationUiQueue Instance { get; private set; }
        private float _timer;
        private Queue<string> _queue;
        private AudioSource _audioSource;

        private const float TIMER_DURATION = 3f;

        public void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _queue = new Queue<string>();
            _timer = 0f;

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.volume = SAD_Plugin.notificationSoundVolume.Value;
            _audioSource.spatialBlend = 1.0f;
            _audioSource.minDistance = 10f;
            _audioSource.maxDistance = 20f;
        }

        private void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                _timer = 0f;
                if (_queue.Count > 0) 
                {
                    NotificationUi.instance.ShowNotification(_queue.Dequeue());
                    if (SAD_Plugin.notificationSoundVolume.Value > 0f)
                        _audioSource.PlayOneShot(AssetsLoader.notificationSound);
                    _timer = TIMER_DURATION;
                }
            }
        }

        public void QueueNotification(string message) 
        {
            _queue.Enqueue(message);
        }
    }
}
