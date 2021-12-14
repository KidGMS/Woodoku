using System;
using System.Collections.Generic;
using System.Linq;
using Script;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotControl : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
  private enum Direction {
    Null,
    DirA,
    DirB,
    DirC,
    DirD
  }

  private List<RectTransform> _partForSet;
  private List<RectTransform> _slotSelectForField;
  private SelectCoordinate[,] _selectCoordinatesOnField;
  private bool _slotIsFilled;
  private Transform[,] _mapShape;
  private int _slotPositionX;
  private int _slotPositionY;
  private int _coordinatePartsX;
  private int _coordinatePartsY;

  private int _fieldSize;
  private Color _colorShadowTrue;
  private Color _colorShadowFalse;


  private void Start() {
    _colorShadowTrue = new Color(0.5f, 0.5f, 1f);
    _colorShadowFalse = new Color(0.5f, 0.2f, 0f);
    _slotSelectForField = new List<RectTransform>();
    _fieldSize = (int) Math.Sqrt(CreateField.GetMapField().Length);
  }

  public void OnDrop(PointerEventData eventData) {
    var workObject = eventData.pointerDrag.GetComponent<RectTransform>();
    if (eventData.pointerDrag != null && !FilledSlot() && CheckStagingOnField(workObject) && CheckFilledAllSlot()) {
      AddPartsOnField(workObject);
      MyEvents.SoundSetSlot?.Invoke();
      MyEvents.ValidationField.Invoke(_slotPositionX, _slotPositionY);
      var check = MyEvents.checkSpawnShapes.Invoke();
      MyEvents.SpawnShapes?.Invoke(check);
      MyEvents.checkShapes?.Invoke();
      MyEvents.isGameOver?.Invoke();
    } else {
      MyEvents.SoundError?.Invoke();
    }
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<ShapeControl>().IsActiveShape()) {
      _mapShape = eventData.pointerDrag.GetComponent<Shape>().GetMapShape();
      var ShapeControl = eventData.pointerDrag.GetComponent<ShapeControl>();
      SiteSelectionByMap(ShapeControl);
      Draw(true, ShapeControl);
    }
  }

  public void OnPointerExit(PointerEventData eventData) {
    ReDraw();
  }

  public bool CheckSlotForShape(ShapeControl shapeControl, Shape shape, RectTransform rectTransform) {
    _mapShape = null;
    _mapShape = shape.GetMapShape();
    SiteSelectionByMap(shapeControl);
    Draw(false, shapeControl);
    if (!FilledSlot() && CheckStagingOnField(rectTransform) && CheckFilledAllSlot()) {
      return true;
    }

    return false;
  }

  public void SetFilledSlot(bool filled) {
    _slotIsFilled = filled;
  }

  public bool SlotIsFilled() {
    return _slotIsFilled;
  }

  private void AddPartsOnField(RectTransform transform) {
    _partForSet = transform.GetComponentsInChildren<RectTransform>().ToList();
    for (var i = 1; i < _partForSet.Count; i++) {
      _partForSet[i].SetParent(_slotSelectForField[i - 1]);
      _slotSelectForField[i - 1].GetComponent<SlotControl>().SetFilledSlot(true);
      _partForSet[i].localPosition = Vector2.zero;
    }

    MyEvents.RemoveShapes(transform.GetComponent<ShapeControl>());
    Destroy(transform.gameObject);
    ReDraw();
  }

  private bool CheckStagingOnField(RectTransform transform) {
    if (_slotSelectForField.Count >= transform.childCount) {
      return true;
    }

    return false;
  }

  private bool CheckFilledAllSlot() {
    if (CountFilledSlot() >= _slotSelectForField.Count) {
      return true;
    }

    return false;
  }

  private int CountFilledSlot() {
    var count = 0;
    for (var i = 0; i < _slotSelectForField.Count; i++) {
      if (!_slotSelectForField[i].GetComponent<SlotControl>().SlotIsFilled()) {
        count++;
      }
    }

    return count;
  }

  private void SiteSelectionByMap(ShapeControl shapeControl) {
    SelectCoordinateCreateStruct();
    GetSlotPositionInField();
    GetCoordinateParts(shapeControl);
    AddStartCoordinate();
    SiteSelection();
  }

  private void SiteSelection() {
    for (var i = 0; i < _mapShape.GetLength(0); i++) {
      for (var j = 0; j < _mapShape.GetLength(1); j++) {
        if (CheckMapShapeIsNull(i, j)) {
          Selection(i, j);
        }
      }
    }
  }

  private void Selection(int i, int j) {
    switch (CheckDir(i, j)) {
      case Direction.DirA:
        _selectCoordinatesOnField[i, j].AddCoordinate(_slotPositionX - (_coordinatePartsX - i), _slotPositionY - (_coordinatePartsY - j));
        break;
      case Direction.DirB:
        _selectCoordinatesOnField[i, j].AddCoordinate(_slotPositionX - (_coordinatePartsX - i), _slotPositionY + (j - _coordinatePartsY));
        break;
      case Direction.DirC:
        _selectCoordinatesOnField[i, j].AddCoordinate(_slotPositionX + (i - _coordinatePartsX), _slotPositionY + (j - _coordinatePartsY));
        break;
      case Direction.DirD:
        _selectCoordinatesOnField[i, j].AddCoordinate(_slotPositionX + (i - _coordinatePartsX), _slotPositionY - (_coordinatePartsY - j));
        break;
    }
  }

  private Direction CheckDir(int i, int j) {
    if (DirA(i, j)) {
      return Direction.DirA;
    }

    if (DirB(i, j)) {
      return Direction.DirB;
    }

    if (DirC(i, j)) {
      return Direction.DirC;
    }

    if (DirD(i, j)) {
      return Direction.DirD;
    }

    return Direction.Null;
  }

  private bool DirA(int i, int j) {
    if (_coordinatePartsX >= i && _coordinatePartsY >= j) {
      return true;
    }

    return false;
  }

  private bool DirB(int i, int j) {
    if (_coordinatePartsX >= i && _coordinatePartsY <= j) {
      return true;
    }

    return false;
  }

  private bool DirC(int i, int j) {
    if (_coordinatePartsX <= i && _coordinatePartsY <= j) {
      return true;
    }

    return false;
  }

  private bool DirD(int i, int j) {
    if (_coordinatePartsX <= i && _coordinatePartsY >= j) {
      return true;
    }

    return false;
  }

  private void SelectCoordinateCreateStruct() {
    _selectCoordinatesOnField = new SelectCoordinate[_mapShape.GetLength(0), _mapShape.GetLength(1)];
    for (var i = 0; i < _mapShape.GetLength(0); i++) {
      for (var j = 0; j < _mapShape.GetLength(1); j++) {
        _selectCoordinatesOnField[i, j].AddCoordinate(-1, -1);
      }
    }
  }

  private bool CheckMapShapeIsNull(int i, int j) {
    if (_mapShape[i, j] != null) {
      return true;
    }

    return false;
  }

  private void AddStartCoordinate() {
    _selectCoordinatesOnField[_coordinatePartsX, _coordinatePartsY].AddCoordinate(_slotPositionX, _slotPositionY);
  }

  private void Draw(bool addShadow, ShapeControl shapeControl) {
    _slotSelectForField.Clear();
    for (var i = 0; i < _mapShape.GetLength(0); i++) {
      for (var j = 0; j < _mapShape.GetLength(1); j++) {
        if (CheckBoundsArray(i, j)) {
          AddActiveSlot(i, j);
        }
      }
    }

    if (addShadow) {
      for (var i = 0; i < _mapShape.GetLength(0); i++) {
        for (var j = 0; j < _mapShape.GetLength(1); j++) {
          if (CheckBoundsArray(i, j)) {
            if (CountFilledSlot() == shapeControl.GetPartsCount()) {
              AddShadow(i, j, _colorShadowTrue);
            } else {
              AddShadow(i, j, _colorShadowFalse);
            }
          }
        }
      }
    }
  }

  private void AddActiveSlot(int i, int j) {
    _slotSelectForField.Add(CreateField.GetMapField()[_selectCoordinatesOnField[i, j].GetX(), _selectCoordinatesOnField[i, j].GetY()].GetComponent<RectTransform>());
  }

  private void AddShadow(int i, int j, Color color) {
    CreateField.GetMapField()[_selectCoordinatesOnField[i, j].GetX(), _selectCoordinatesOnField[i, j].GetY()].GetComponent<Image>().color = color;
  }

  private bool CheckBoundsArray(int i, int j) {
    if (BorderArrayTop(i, j) && BorderArrayBottom(i, j) && BorderArrayLeft(i, j) && BorderArrayRight(i, j)) {
      return true;
    }

    return false;
  }

  private bool BorderArrayTop(int i, int j) {
    if (_selectCoordinatesOnField[i, j].GetX() >= 0) {
      return true;
    }

    return false;
  }

  private bool BorderArrayBottom(int i, int j) {
    if (_selectCoordinatesOnField[i, j].GetX() < CreateField.GetMapField().GetLength(0)) {
      return true;
    }

    return false;
  }

  private bool BorderArrayLeft(int i, int j) {
    if (_selectCoordinatesOnField[i, j].GetY() >= 0) {
      return true;
    }

    return false;
  }

  private bool BorderArrayRight(int i, int j) {
    if (_selectCoordinatesOnField[i, j].GetY() < CreateField.GetMapField().GetLength(1)) {
      return true;
    }

    return false;
  }

  private void ReDraw() {
    for (var i = 0; i < _fieldSize; i++) {
      for (var j = 0; j < _fieldSize; j++) {
        CreateField.GetMapField()[i, j].GetComponent<Image>().color = Color.white;
      }
    }
  }

  private void GetSlotPositionInField() {
    for (var i = 0; i < _fieldSize; i++) {
      for (var j = 0; j < _fieldSize; j++) {
        if (CheckObjInPosition(i, j)) {
          _slotPositionX = i;
          _slotPositionY = j;
        }
      }
    }
  }

  private void GetCoordinateParts(ShapeControl shapeControl) {
    
    if (_mapShape.GetLength(0) == 1) {
      if (_mapShape[0, 1] != null) {
        SetCoordinateParts(0, 1);
      } else {
        SetCoordinateParts(0, 0);
      }
    } else if (_mapShape.GetLength(0) > 3) {
      if (_mapShape[1, 0] != null) {
        SetCoordinateParts(1, 0);
      } else {
        SetCoordinateParts(0, 0);
      }
    } else if (_mapShape.GetLength(0) != 3) {
      
      if (_mapShape[1, 1] != null) {
        SetCoordinateParts(1, 1);
      } else {
        SetCoordinateParts(0, 1);
      }
    } else {
      if (_mapShape[1, 1] != null && _mapShape[1, 0] == null) {
        SetCoordinateParts(1, 1);
      }

      if (_mapShape[1, 1] == null && _mapShape[1, 0] != null) {
        SetCoordinateParts(1, 0);
      }

      if (_mapShape[1, 1] != null && _mapShape[1, 0] != null) {
        SetCoordinateParts(1, 1);
      }
      if (_mapShape[1, 1] == null && _mapShape[1, 0] == null) {
        SetCoordinateParts(0, 0);
      }
    }
  }

  private void SetCoordinateParts(int x, int y) {
    _coordinatePartsX = x;
    _coordinatePartsY = y;
  }

  private bool CheckObjInPosition(int i, int j) {
    if (gameObject == CreateField.GetMapField()[i, j]) {
      return true;
    }

    return false;
  }

  private bool FilledSlot() {
    if (_slotIsFilled) {
      return true;
    }

    return false;
  }

  private struct SelectCoordinate {
    private int _x;
    private int _y;

    public void AddCoordinate(int x, int y) {
      _x = x;
      _y = y;
    }

    public int GetX() {
      return _x;
    }

    public int GetY() {
      return _y;
    }
  }
}