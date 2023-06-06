using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float _aliveTime;
    private float _counter;

    private void Update()
    {
        _counter += Time.deltaTime;
        if (_counter >= _aliveTime)
        {
            Destroy(this.gameObject);
        }
    }
}
