using DG.Tweening;
using Script;
using UnityEngine;
using UnityEngine.UI;

public class CheckField : MonoBehaviour {
  private enum ComboClear {
    Null,
    ComboX2,
    ComboX3,
    ComboX4,
    ComboX5
  }

  [SerializeField]
  private Text _pointText;

  private GameObject[,] _mapField;
  private int _sizeField;
  private ComboClear _combo;
  private int _pointCount;
  private int _pointCountForOne;

  private void Start() {
    _sizeField = GetComponent<CreateField>().GetSizeField();
    _mapField = new GameObject[_sizeField, _sizeField];
    _mapField = CreateField.GetMapField();
    MyEvents.ValidationField += ValidationField;
    MyEvents.clearPoint += ClearPoint;
    MyEvents.getPoint += GETPoint;
    PrintPoint();
  }

  private void OnDestroy() {
    MyEvents.ValidationField -= ValidationField;
    MyEvents.clearPoint -= ClearPoint;
    MyEvents.getPoint -= GETPoint;
  }

  private int GETPoint() {
    return _pointCount;
  }

  private void ValidationField(int x, int y) {
    Clear(x, y);
  }

  private int CheckFilledColumns(int numberColumns) {
    var counter = 0;
    for (var i = 0; i < _sizeField; i++) {
      if (_mapField[i, numberColumns].transform.childCount > 0) {
        counter++;
      }
    }

    return counter;
  }

  private int CheckFilledRows(int numberRows) {
    var counter = 0;
    for (var i = 0; i < _sizeField; i++) {
      if (_mapField[numberRows, i].transform.childCount > 0) {
        counter++;
      }
    }

    return counter;
  }

  private void CheckSquares() {
    var counter = 0;
    var sqrX = 0;
    var sqrY = 0;
    for (var i = 1; i <= _sizeField; i++) {
      if (sqrX == 9) {
        sqrX = 0;
      }

      for (var j = 0; j < _sizeField / 3; j++) {
        for (var k = 0; k < _sizeField / 3; k++) {
          if (CheckChildCount(j + sqrX, k + sqrY) > 0) {
            counter++;
          }
        }
      }

      ClearSlotsSquares(sqrX, sqrY, counter);

      if (i % 3 == 0) {
        sqrY += 3;
      }

      sqrX += 3;

      counter = 0;
    }
  }

  private void ClearSlotsSquares(int xAdd, int yAdd, int countFilledSlot) {
    if (IsFilled(countFilledSlot)) {
      MyEvents.SoundClear?.Invoke();
      for (var j = 0; j < _sizeField / 3; j++) {
        for (var k = 0; k < _sizeField / 3; k++) {
          RefilledSlot(j + xAdd, k + yAdd);
          ClearSlot(j + xAdd, k + yAdd);
        }
      }
    }
  }

  private int CheckChildCount(int x, int y) {
    return _mapField[x, y].transform.childCount;
  }

  private bool IsFilled(int counter) {
    if (counter >= _sizeField) {
      return true;
    }

    return false;
  }

  private void Clear(int x, int y) {
    CheckSquares();
    for (var i = 0; i < _sizeField; i++) {
      for (var j = 0; j < _sizeField; j++) {
        if (IsFilled(CheckFilledColumns(j))) {
          if (_mapField[i, j].transform.childCount > 0) {
            MyEvents.SoundClear?.Invoke();
            RefilledSlot(i, j);
            ClearSlot(i, j);
          }
        }

        if (IsFilled(CheckFilledRows(i))) {
          if (_mapField[i, j].transform.childCount > 0) {
            MyEvents.SoundClear?.Invoke();
            RefilledSlot(i, j);
            ClearSlot(i, j);
          }
        }
      }
    }
    if (_pointCountForOne != 0) {
      PrintPoint();
    }
  }

  private void AddPoint(int count) {
    _pointCountForOne += count;
  }

  private ComboClear CheckCombo(int point) {
    switch (point / _sizeField) {
      case 1:
        return ComboClear.Null;
      case 2:
        return ComboClear.ComboX2;
      case 3:
        return ComboClear.ComboX3;
      case 4:
        return ComboClear.ComboX4;
      case 5:
        return ComboClear.ComboX5;
      default:
        return ComboClear.Null;
    }
  }

  private void PrintPoint() {
    var check = CheckCombo(_pointCountForOne);
    if (check != ComboClear.Null) {
      MyEvents.Combo?.Invoke(check.ToString().ToUpper());
      _pointCountForOne *= (_pointCountForOne / _sizeField);
    }
    _pointCount += _pointCountForOne;
    _pointCountForOne = 0;
    _pointText.text = _pointCount.ToString();
  }

  private void ClearSlot(int x, int y) {
    _mapField[x, y].transform.GetChild(0).transform.DOScale(0f, 0.5f).OnComplete(() => { Destroy(_mapField[x, y].transform.GetChild(0).gameObject); });
    AddPoint(+1);
  }

  private void RefilledSlot(int x, int y) {
    _mapField[x, y].GetComponent<SlotControl>().SetFilledSlot(false);
  }

  private void ClearPoint() {
    _pointText.text = "0";
    _pointCount = 0;
    _pointCountForOne = 0;
  }
}