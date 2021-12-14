using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeControl : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler {
  private bool _shapeActive;
  private RectTransform _rectTransform;
  private CanvasGroup _canvasGroup;
  private Canvas _canvas;
  private Transform _parentShape;
  private List<Parts> _parts;
  private Shape _shape;
  private void Start() {
    _shape = GetComponent<Shape>();
    _rectTransform = GetComponent<RectTransform>();
    _canvasGroup = GetComponent<CanvasGroup>();
    _canvas = FindObjectOfType<Canvas>();
    _shapeActive = true;
    _parentShape = transform.parent;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    if (_shapeActive) {
      _canvasGroup.blocksRaycasts = false;
    }
  }

  public void OnDrag(PointerEventData eventData) {
    if (_shapeActive) {
      eventData.pointerDrag.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 1f);
      _rectTransform.SetParent(_canvas.transform);
      _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
  }

  public void OnEndDrag(PointerEventData eventData) {
    if (_shapeActive) {
      _rectTransform.SetParent(_parentShape);
      eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
      eventData.pointerDrag.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1);
      _canvasGroup.blocksRaycasts = true;
    }
  }

  public void ShapeActive(bool active) {
    _parts = transform.GetComponent<Shape>().GetParts();
    foreach (var obj in _parts) {
      obj.GetComponent<Parts>().SetActiveShape(active);
    }

    _shapeActive = active;
  }
  public int GetPartsCount() {
    _parts = transform.GetComponent<Shape>().GetParts();
    return _parts.Count;
  }
  public void OnPointerClick(PointerEventData pointerEventData)
  {
    RotateShape();
  }
  public bool IsActiveShape() {
    return _shapeActive;
  }

  private void RotateShape() {
    _rectTransform.Rotate(Vector3.forward,-90f);
    _shape.RecreateMap();
  }
  
  public int PartsCoordinateSelectionX { get; set; }

  public int PartsCoordinateSelectionY { get; set; }
}