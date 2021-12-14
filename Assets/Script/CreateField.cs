using Script;
using UnityEngine;

public class CreateField : MonoBehaviour {
  private static GameObject[,] _mapField;

  public static GameObject[,] GetMapField() {
    return _mapField;
  }

  private enum Color {
    ColorA,
    ColorB
  }

  [SerializeField]
  private GameObject _cellA;

  [SerializeField]
  private GameObject _cellB;

  [SerializeField]
  private int _sizeField;

  private GameObject _field;

  private void Awake() {
    _field = gameObject;
    _mapField = new GameObject[_sizeField, _sizeField];
    CreateGameField(_cellA, _cellB, _field);
  }

  public void Start() {
    MyEvents.clearField += ClearField;
  }

  private void OnDestroy() {
    MyEvents.clearField -= ClearField;
  }

  public int GetSizeField() {
    return _sizeField;
  }

  private void ClearField() {
    for (var i = 0; i < _sizeField; i++) {
      for (var j = 0; j < _sizeField; j++) {
        if (_mapField[i, j].transform.childCount != 0) {
          Destroy(_mapField[i, j].transform.GetChild(0).gameObject);
        }

        _mapField[i, j].GetComponent<SlotControl>().SetFilledSlot(false);
      }
    }
  }

  private void CreateGameField(GameObject cellA, GameObject cellB, GameObject field) {
    for (var i = 0; i < _sizeField; i++) {
      for (var j = 0; j < _sizeField; j++) {
        Create(i, j, cellA, cellB, field);
      }
    }
  }

  private void Create(int i, int j, GameObject cellA, GameObject cellB, GameObject field) {
    switch (DrawCellTwoColors(i, j)) {
      case Color.ColorA:
        _mapField[i, j] = Instantiate(cellA, Vector2.zero, Quaternion.identity, field.transform);
        break;
      case Color.ColorB:
        _mapField[i, j] = Instantiate(cellB, Vector2.zero, Quaternion.identity, field.transform);
        break;
    }
  }

  private Color DrawCellTwoColors(int i, int j) {
    if (TopSquare(i, j) || LeftSquare(i, j) || RightSquare(i, j) || BottomSquare(i, j)) {
      return Color.ColorB;
    }

    return Color.ColorA;
  }

  private bool TopSquare(int i, int j) {
    if (i > 2 && i < 6 && j < 3) {
      return true;
    }

    return false;
  }

  private bool LeftSquare(int i, int j) {
    if (i > 2 && i < 6 && j > 5) {
      return true;
    }

    return false;
  }

  private bool RightSquare(int i, int j) {
    if (j > 2 && j < 6 && i < 3) {
      return true;
    }

    return false;
  }

  private bool BottomSquare(int i, int j) {
    if (j > 2 && j < 6 && i > 5) {
      return true;
    }

    return false;
  }
}