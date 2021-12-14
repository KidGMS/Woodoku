using Script;
using UnityEngine;

public class CheckShape : MonoBehaviour {
  private ShapeGenerator _shapeGenerator;
  private GameObject[,] _mapField;
  private SlotControl[,] _slotControls;

  public void Start() {
    MyEvents.checkShapes += Check;
    MyEvents.isGameOver += isGameOver;
    _shapeGenerator = transform.GetComponent<ShapeGenerator>();
    _mapField = CreateField.GetMapField();
    _slotControls = new SlotControl[_mapField.GetLength(0), _mapField.GetLength(1)];
    SetSlotContoll();
  }

  private void OnDestroy() {
    MyEvents.checkShapes -= Check;
    MyEvents.isGameOver -= isGameOver;
  }

  private void SetSlotContoll() {
    for (var i = 0; i < _mapField.GetLength(0); i++) {
      for (var j = 0; j < _mapField.GetLength(1); j++) {
        _slotControls[i, j] = _mapField[i, j].GetComponent<SlotControl>();
      }
    }
  }

  private void Check() {
    var countSet = 0;
    for (var k = 0; k < _shapeGenerator.GetShapeExist().Count; k++) {
      for (var z = 0; z < 4; z++) {
        for (var i = 0; i < 9; i++) {
          for (var j = 0; j < 9; j++) {
            if (CheckSetIsShape(i, j, k)) {
              countSet++;
            }
          }
        }
        _shapeGenerator.GetShapeExist()[k].GetComponent<Shape>().RecreateMap();
      }
      SetIsActiveInactiveShape(countSet, k);
      countSet = 0;
    }
  }

  private bool CheckSetIsShape(int x, int y, int k) {
    var shapeControl = _shapeGenerator.GetShapeExist()[k].GetComponent<ShapeControl>();
    var shape = _shapeGenerator.GetShapeExist()[k].GetComponent<Shape>();
    var rectTransform = _shapeGenerator.GetShapeExist()[k].GetComponent<RectTransform>();
    if (_slotControls[x, y].CheckSlotForShape(shapeControl, shape, rectTransform)) {
      return true;
    }

    return false;
  }

  private void SetIsActiveInactiveShape(int countSet, int k) {
    if (countSet == 0) {
      _shapeGenerator.GetShapeExist()[k].ShapeActive(false);
    } else if (!_shapeGenerator.GetShapeExist()[k].IsActiveShape()) {
      _shapeGenerator.GetShapeExist()[k].ShapeActive(true);
    }
  }

  private void isGameOver() {
    if (_shapeGenerator.GetShapeExist().Count == ShapeIsInactive()) {
      GameOver();
    }
  }

  private int ShapeIsInactive() {
    var count = 0;
    for (var k = 0; k < _shapeGenerator.GetShapeExist().Count; k++) {
      if (!_shapeGenerator.GetShapeExist()[k].IsActiveShape()) {
        count++;
      }
    }

    return count;
  }

  private void GameOver() {
    MyEvents.SoundGameOver?.Invoke();
    MyEvents.gameOver?.Invoke();
  }
}