using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bankd.Core
{
    public class CoroutineManager : MonoBehaviour
    {
        private static CoroutineManager _sInstance;

        private readonly Dictionary<Guid, Coroutine> _runningCoroutines = new();

        public static CoroutineManager Instance => _sInstance;

        private void Awake()
        {
            _sInstance ??= this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Starts a managed coroutine with a specific duration and callback. 
        /// Tracks the coroutine using a unique ID for control and cleanup after completion.
        /// </summary>
        public Guid StartManagedCoroutine(IEnumerator routine)
        {
            var id = Guid.NewGuid();
            var coroutine = StartCoroutine(TrackCoroutine(routine, id));
            _runningCoroutines.Add(id, coroutine);
            
            return id;
        }
        
        /// <summary>
        /// Starts a coroutine and tracks it using a unique ID. 
        /// This allows for managing its lifecycle, such as stopping it or checking its state.
        /// </summary>
        public Guid StartTrackedCoroutine(IEnumerator routine)
        {
            var id = Guid.NewGuid();
            var coroutine = StartCoroutine(TrackCoroutine(routine, id));
            
            _runningCoroutines.Add(id, coroutine);
            
            return id;
        }

        public void StopCoroutine(Guid id)
        {
            if (!_runningCoroutines.TryGetValue(id, out var coroutine)) return;
            
            StopCoroutine(coroutine);
            _runningCoroutines.Remove(id);
        }

        public void StopAllCoroutines()
        {
            foreach (var coroutine in _runningCoroutines.Values)
                StopCoroutine(coroutine);

            _runningCoroutines.Clear();
        }

        private IEnumerator TrackCoroutine(IEnumerator routine, Guid id)
        {
            yield return routine;
            
            _runningCoroutines.Remove(id);
        }
    }
}