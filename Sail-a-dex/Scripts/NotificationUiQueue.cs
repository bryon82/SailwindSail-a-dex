using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static sailadex.Configs;

namespace sailadex
{
    public class NotificationUiQueue : MonoBehaviour
    {
        public static NotificationUiQueue Instance { get; private set; }
        private Coroutine _queueCoroutine;
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
            _queueCoroutine = null;

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.volume = notificationSoundVolume.Value;
            _audioSource.spatialBlend = 1.0f;
            _audioSource.minDistance = 10f;
            _audioSource.maxDistance = 20f;
        }

        private IEnumerator ProcessQueue()
        {
            while (_queue.Count > 0)
            {
                NotificationUi.instance.ShowNotification(_queue.Dequeue());
                if (notificationSoundVolume.Value > 0f)
                    _audioSource.PlayOneShot(AssetsLoader.NotificationSound);

                yield return new WaitForSeconds(TIMER_DURATION);
            }

            _queueCoroutine = null;
        }

        public void QueueNotification(string message) 
        {
            _queue.Enqueue(message);
            if (_queueCoroutine == null)
                _queueCoroutine = StartCoroutine(ProcessQueue());
        }

        //Testing
        //public void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        QueueNotification("Test Notification");
        //        QueueNotification("Test Notification 2");
        //    }
        //}
    }
}
