﻿using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Algorithms
{
    public abstract class Promise
    {
        protected Thread Thread;
        private bool _finished;
        public bool Finished
        {
            get { return _finished; }
            set
            {
                _finished = value;
                if (_finished && OnFinish != null)
                {
                    MainThread.Execute(OnFinish);
                }
            }
        }

        public Action OnFinish;
    }

    public class MainThread : MonoBehaviour
    {
        private static MainThread _mt;
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        public static void Execute(Action a)
        {
            _mt._actionQueue.Enqueue(a);
        }

        void Update()
        {
            while (_actionQueue.Count > 0)
            {
                _actionQueue.Dequeue()();
            }
        }

        public static void Instantiate()
        {
            var mtObj = Instantiate(new GameObject());
            mtObj.name = "MainThread";
            _mt = mtObj.AddComponent<MainThread>();
        }
    }
}
