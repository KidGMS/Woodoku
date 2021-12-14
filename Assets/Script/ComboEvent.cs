using DG.Tweening;
using Script;
using UnityEngine;
using UnityEngine.UI;
public class ComboEvent : MonoBehaviour {
  [SerializeField]
  private Transform _comboTransform;
  [SerializeField]
  private Text _comboText;
  [SerializeField]
  private Transform _toMove;
  
  public RectTransform _comboPosition;
  public Vector3 test;
  void Start() {
    MyEvents.Combo += Combo;
    _comboPosition = GetComponent<RectTransform>();
  }

  private void OnDestroy() {
    MyEvents.Combo -= Combo;
  }

  private void Combo(string obj) {
    _comboTransform.gameObject.SetActive(true);
    _comboText.text = obj;
    _comboTransform.DOScale(1, 2);
    _comboTransform.DOPunchScale(Vector3.one, 1, 1).OnComplete(() => {
      _comboTransform.DOMove(_toMove.position, 2f, true);
      _comboTransform.DOScale(0, 2).OnComplete(() => {
        _comboPosition.anchoredPosition = new Vector2(0,30f);
        _comboPosition.localScale = Vector3.zero;
        _comboTransform.gameObject.SetActive(false); 
      });
    });
  }
}