using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shape : MonoBehaviour {
  private const int SIZE_MAP_DIRECT = 4;

  private const int SIZE_MAP_NOT_DIRECT = 3;

  private const float POZSQUARE = 55;

  private enum PartPosition {
    Null,
    TopLeft,
    TopMiddle,
    TopRight,
    BottomLeft,
    BottomMiddle,
    BottomRight
  }

  private Transform[,] _mapShape;
  private List<Transform> _partsTransforms;
  private List<Parts> _parts;

  // Start is called before the first frame update
  private void Awake() {
    _parts = new List<Parts>();
    _partsTransforms = GetComponentsInChildren<Transform>().ToList();
    _partsTransforms.RemoveAt(0);
    foreach (var VARIABLE in _partsTransforms) {
      _parts.Add(VARIABLE.GetComponent<Parts>());
    }

    CheckShapeForStraightness();
    CreateMap();
  }

  public Transform[,] GetMapShape() {
    return _mapShape;
  }

  public List<Parts> GetParts() {
    return _parts;
  }

  public void RecreateMap() {
    _mapShape = Rotate(_mapShape);
  }

  private Transform[,] Rotate(Transform[,] array) {
    int a = array.GetLength(0);
    int b = array.GetLength(1);
    var NewMap = new Transform[b, a];
    for (var i = 0; i < a; i++) {
      for (var j = 0; j < b; j++) {
        NewMap[j, a - i - 1] = array[i, j];
      }
    }

    return NewMap;
  }

  private void CreateMap() {
    if (CheckShapeForStraightness()) {
      CreateDirectMap();
    } else {
      CreateNotDirectMap();
    }
  }

  private void CreateDirectMap() {
    _mapShape = new Transform[1, SIZE_MAP_DIRECT];
    for (var i = 0; i < _partsTransforms.Count; i++) {
      foreach (var obj in _partsTransforms) {
        _mapShape[0, i] = _partsTransforms[i];
      }
    }
  }

  private void CreateNotDirectMap() {
    _mapShape = new Transform[2, SIZE_MAP_NOT_DIRECT];
    AddPartToMap();
  }

  private void AddPartToMap() {
    foreach (var obj in _partsTransforms) {
      switch (CheckPositionPart(obj)) {
        case PartPosition.TopLeft:
          _mapShape[0, 0] = obj;
          break;
        case PartPosition.TopMiddle:
          _mapShape[0, 1] = obj;
          break;
        case PartPosition.TopRight:
          _mapShape[0, 2] = obj;
          break;
        case PartPosition.BottomLeft:
          _mapShape[1, 0] = obj;
          break;
        case PartPosition.BottomMiddle:
          _mapShape[1, 1] = obj;
          break;
        case PartPosition.BottomRight:
          _mapShape[1, 2] = obj;
          break;
      }
    }
  }

  private PartPosition CheckPositionPart(Transform part) {
    if (TopLeft(part)) {
      return PartPosition.TopLeft;
    }

    if (TopMiddle(part)) {
      return PartPosition.TopMiddle;
    }

    if (TopRight(part)) {
      return PartPosition.TopRight;
    }

    if (BottomLeft(part)) {
      return PartPosition.BottomLeft;
    }

    if (BottomMiddle(part)) {
      return PartPosition.BottomMiddle;
    }

    if (BottomRight(part)) {
      return PartPosition.BottomRight;
    }

    return PartPosition.Null;
  }

  private bool TopLeft(Transform part) {
    if (part.localPosition.x < 0 && part.localPosition.y > 0) {
      return true;
    }

    return false;
  }

  private bool TopMiddle(Transform part) {
    if (part.localPosition.x == 0 && part.localPosition.y > 0 || part.localPosition.x == POZSQUARE && part.localPosition.y == POZSQUARE) {
      return true;
    }

    return false;
  }

  private bool TopRight(Transform part) {
    if (part.localPosition.x > 0 && part.localPosition.y > 0) {
      return true;
    }

    return false;
  }

  private bool BottomLeft(Transform part) {
    if (part.localPosition.x < 0 && part.localPosition.y < 0) {
      return true;
    }

    return false;
  }

  private bool BottomMiddle(Transform part) {
    if (part.localPosition.x == 0 && part.localPosition.y < 0 || part.localPosition.x == POZSQUARE && part.localPosition.y == -POZSQUARE) {
      return true;
    }

    return false;
  }

  private bool BottomRight(Transform part) {
    if (part.localPosition.x > 0 && part.localPosition.y < 0) {
      return true;
    }

    return false;
  }

  private bool CheckShapeForStraightness() {
    if (CountPartsStandingOneLevel() == _partsTransforms.Count) {
      return true;
    }

    return false;
  }

  private int CountPartsStandingOneLevel() {
    var counter = 0;
    foreach (var obj in _partsTransforms) {
      if (obj.localPosition.y == 0) {
        counter++;
      }
    }

    return counter;
  }
}