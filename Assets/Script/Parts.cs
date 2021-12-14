using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Parts : MonoBehaviour, IPointerEnterHandler {
  private Image _partsImage;
  private bool _partsActive;
  private Transform _parent;

  private void Start() {
    _parent = transform.parent;
    _partsImage = transform.GetComponent<Image>();
    if (_parent != null) {
      SetActiveShape(_parent.GetComponent<ShapeControl>().IsActiveShape());
    }
  }

  public void OnPointerEnter(PointerEventData eventData) {
    if (_partsActive && ShapeIsParent()) {
      SelectPartsPosition();
    }
  }

  public void SelectPartsPosition() {
    var lengthMapX = _parent.GetComponent<Shape>().GetMapShape().GetLength(0);
    var lengthMapY = _parent.GetComponent<Shape>().GetMapShape().GetLength(1);
    for (var i = 0; i < lengthMapX; i++) {
      for (var j = 0; j < lengthMapY; j++) {
        if (PartsCoordinateIsMatch(i, j)) {
          SetCoordinatePartsSelection(i, j);
        }
      }
    }
  }

  public void SetActiveShape(bool active) {
    if (active) {
      ActiveShape();
    } else {
      InActiveShape();
    }
  }

  private void SetCoordinatePartsSelection(int x, int y) {
    _parent.GetComponent<ShapeControl>().PartsCoordinateSelectionX = x;
    _parent.GetComponent<ShapeControl>().PartsCoordinateSelectionY = y;
  }

  private bool ShapeIsParent() {
    if (transform.parent.GetComponent<Shape>() == null) {
      return false;
    }

    return true;
  }

  private void ActiveShape() {
    _partsActive = true;
    transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
  }

  private void InActiveShape() {
    _partsActive = false;
    transform.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
  }

  private bool PartsCoordinateIsMatch(int i, int j) {
    if (transform.parent.GetComponent<Shape>().GetMapShape()[i, j] == transform) {
      return true;
    }

    return false;
  }
}