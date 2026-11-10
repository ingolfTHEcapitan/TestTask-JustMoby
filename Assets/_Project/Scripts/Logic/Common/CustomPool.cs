using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Logic.Common
{
    public class CustomPool<T> where T : MonoBehaviour
    {
        private readonly Func<T> _createFunc;
        private readonly bool _isAutoExpand;

        private Queue<T> _pool;

        public CustomPool(int count, Func<T> createFunc, bool isAutoExpand)
        {
            _createFunc = createFunc;
            _isAutoExpand = isAutoExpand;

            CreatePool(count);
        }

        private void CreatePool(int count)
        {
            _pool = new Queue<T>(count);

            for (int i = 0; i < count; i++)
                _pool.Enqueue(CreateObject());

        }

        private T CreateObject(bool isActiveByDefault = false)
        {
            T newObject = _createFunc.Invoke();
            newObject.gameObject.SetActive(isActiveByDefault);
            return newObject;
        }

        public T GetFreeElement()
        {
            if (_pool.Count > 0)
            {
                T element = _pool.Dequeue();
                element.gameObject.SetActive(true);
                return element;
            }

            if (_isAutoExpand)
                return CreateObject(isActiveByDefault: true);

            throw new Exception($"[CUSTOM POOL] There is no free element in the pool of type {typeof(T).Name}");
        }

        public void ReturnToPool(T element)
        {
            if (element == null)
                return;

            if (!element.gameObject.activeSelf)
                return;

            element.gameObject.SetActive(false);
            _pool.Enqueue(element);
        }
    }
}