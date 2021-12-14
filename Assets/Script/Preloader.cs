using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Script;
using UnityEngine;

public class Preloader : MonoBehaviour {

    [SerializeField]
    private List<Transform> _shapeLoader;
    [SerializeField]
    private Transform _logo;
    [SerializeField]
    private float _animTimeForSeconds;



    private IEnumerator PreLoad() {
        
        for (int i = 0; i < _shapeLoader.Count;i++) {
            _shapeLoader[i].DOScale(0, _animTimeForSeconds);
            yield return new WaitForSeconds(_animTimeForSeconds);
        }

        _logo.DOScale(0, _animTimeForSeconds);
        yield return new WaitForSeconds(_animTimeForSeconds);
    }
}

