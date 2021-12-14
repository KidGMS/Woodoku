using System.Collections.Generic;
using Script;
using UnityEngine;

public class ShapeGenerator : MonoBehaviour {
  [SerializeField]
  private List<GameObject> _shapePanel;

  [SerializeField]
  private List<GameObject> _shape;

  private List<ShapeControl> _shapeExist;

  private void Start() {
    _shapeExist = new List<ShapeControl>();
    AddShapesInPanels(CheckAllPanelsForShape());
    MyEvents.SpawnShapes += AddShapesInPanels;
    MyEvents.checkSpawnShapes += CheckAllPanelsForShape;
    MyEvents.clearShape += ClearShape;
    MyEvents.RemoveShapes += RemoveShapes;
  }

  private void OnDestroy() {
    MyEvents.SpawnShapes -= AddShapesInPanels;
    MyEvents.checkSpawnShapes -= CheckAllPanelsForShape;
    MyEvents.clearShape -= ClearShape;
    MyEvents.RemoveShapes -= RemoveShapes;
  }

  public List<ShapeControl> GetShapeExist() {
    return _shapeExist;
  }

  private void RemoveShapes(ShapeControl obj) {
    _shapeExist.Remove(obj);
    ClearShapeExist();
  }

  private int CountPanelWithoutShape() {
    var count = 0;
    foreach (var obj in _shapePanel) {
      if (obj.transform.childCount == 0) {
        count++;
      }
    }

    return count;
  }

  private bool CheckAllPanelsForShape() {
    if (CountPanelWithoutShape() == _shapePanel.Count) {
      return true;
    }


    return false;
  }

  private void ClearShapeExist() {
    for (var i = 0; i < _shapeExist.Count; i++) {
      if (!_shapeExist[i]) {
        _shapeExist.RemoveAt(i);
      }
    }
  }

  private void ClearShape() {
    foreach (var obj in _shapePanel) {
      if (obj.transform.childCount != 0) {
        Destroy(obj.transform.GetChild(0).gameObject);
      }
    }

    AddShapesInPanels(true);
  }

  private void AddShapesInPanels(bool spawn) {
    if (spawn) {
      _shapeExist.Clear();
      foreach (var obj in _shapePanel) {
        var shape = _shape[Random.Range(0, _shape.Count)];
        var gameObject = Instantiate(shape, obj.transform);
        _shapeExist.Add(gameObject.GetComponent<ShapeControl>());
        MyEvents.checkShapes?.Invoke();
      }
    }
  }
}