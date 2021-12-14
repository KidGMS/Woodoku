using System;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UINav : MonoBehaviour {
  [SerializeField]
  private Transform _record;

  [SerializeField]
  private Text _recordText;

  [SerializeField]
  private List<GameObject> _game;

  [SerializeField]
  private GameObject _setting;

  [SerializeField]
  private GameObject _main;

  [SerializeField]
  private GameObject _shadow;

  [SerializeField]
  private GameObject _gameOverPanel;

  [SerializeField]
  private Text _pointText;

  [SerializeField]
  private AudioSource _soundTap;

  [SerializeField]
  private AudioMixerGroup _myMixer;

  [SerializeField]
  private Image _onSound;

  [SerializeField]
  private Image _offSound;

  [SerializeField]
  private Image _onMusic;

  [SerializeField]
  private Image _offMusic;

  [SerializeField]
  private GameObject _loader;

  [SerializeField]
  private List<Transform> _shapeLoader;

  [SerializeField]
  private Transform _logo;

  [SerializeField]
  private float _animTimeForSeconds;

  private bool _gameIsActive;
  private bool _isGameOver;
  private float _valueSound;
  private float _valueMusic;

  private void Start() {
    StartCoroutine(PreLoad(false));
    MyEvents.gameOver += GameOver;
  }

  private void SwichUIOnLoad(bool play) {
    if (play) {
      RescaleLoader();
      ToGameFromMainMenu();
    } else {
      _main.SetActive(true);
      RescaleLoader();
    }
  }

  private IEnumerator PreLoad(bool play) {
    _loader.SetActive(true);
    _main.SetActive(false);
    for (int i = 0; i < _shapeLoader.Count; i++) {
      _shapeLoader[i].DOScale(0, _animTimeForSeconds);
      yield return new WaitForSeconds(_animTimeForSeconds);
    }

    _logo.DOScale(0, _animTimeForSeconds);
    yield return new WaitForSeconds(_animTimeForSeconds);
    SwichUIOnLoad(play);
  }

  private void RescaleLoader() {
    for (int i = 0; i < _shapeLoader.Count; i++) {
      _shapeLoader[i].localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    _logo.localScale = Vector3.one;
    _loader.SetActive(false);
  }

  private void OnDestroy() {
    MyEvents.gameOver -= GameOver;
  }

  public void ButtonOpenSetting() {
    _soundTap.Play();
    if (_gameIsActive) {
      _shadow.SetActive(true);
      _setting.SetActive(true);
    } else {
      _shadow.SetActive(true);
      _main.SetActive(false);
      _setting.SetActive(true);
    }
  }

  public void ButtonExitSetting() {
    _soundTap.Play();
    if (_gameIsActive) {
      _shadow.SetActive(false);
      _setting.SetActive(false);
    } else {
      _shadow.SetActive(false);
      _main.SetActive(true);
      _setting.SetActive(false);
    }
  }

  public void ButtonExitGame() {
    _soundTap.Play();
    ToMainMenuFromGame();
    _main.SetActive(true);
  }

  public void ButtonPlay() {
    _soundTap.Play();
    StartCoroutine(PreLoad(true));
  }

  public void ButtonShadow() {
    _soundTap.Play();
    if (_isGameOver) {
      ButtonRestartGame();
    } else {
      ButtonExitSetting();
    }
  }

  public void ButtonSound() {
    _soundTap.Play();
    SetActiveMusic("SoundVolume", _onSound, _offSound);
  }

  public void ButtonMusic() {
    _soundTap.Play();
    SetActiveMusic("MusicVolume", _onMusic, _offMusic);
  }

  public void ButtonRestartGame() {
    _soundTap.Play();
    MyEvents.clearShape?.Invoke();
    MyEvents.clearField?.Invoke();
    MyEvents.clearPoint?.Invoke();
    _isGameOver = false;
    _shadow.SetActive(false);
    _gameOverPanel.SetActive(false);
    _record.gameObject.SetActive(false);
  }

  public void ButtonExitApp() {
    Application.Quit();
  }

  private void SetActiveMusic(string nameMixer, Image on, Image off) {
    float value;
    var result = _myMixer.audioMixer.GetFloat(nameMixer, out value);
    Debug.Log(value);
    if (result) {
      if (value == 0) {
        on.enabled = false;
        off.enabled = true;
        _myMixer.audioMixer.SetFloat(nameMixer, -80);
      } else {
        on.enabled = true;
        off.enabled = false;
        _myMixer.audioMixer.SetFloat(nameMixer, 0);
      }
    }
  }

  private void ToMainMenuFromGame() {
    SetActiveGame(false);
  }

  private void ToGameFromMainMenu() {
    SetActiveGame(true);
    MyEvents.clearShape?.Invoke();
    MyEvents.clearField?.Invoke();
    MyEvents.clearPoint?.Invoke();
  }

  private void SetActiveGame(bool isActive) {
    foreach (var VARIABLE in _game) {
      VARIABLE.SetActive(isActive);
    }

    _gameIsActive = isActive;
  }


  private void GameOver() {
    _shadow.SetActive(true);
    var point = MyEvents.getPoint.Invoke();
    SetRecord(point);
    PlayerPrefs.Save();
    _recordText.text = PlayerPrefs.GetInt("Point").ToString();
    _pointText.text = point.ToString();
    _gameOverPanel.SetActive(true);
    _isGameOver = true;
  }

  private void SetRecord(int point) {
    if (PlayerPrefs.GetInt("Point") == 0) {
      PlayerPrefs.SetInt("Point", point);
      PlayerPrefs.Save();
    } else {
      if (PlayerPrefs.GetInt("Point") < point) {
        PlayerPrefs.SetInt("Point", point);
        _record.gameObject.SetActive(true);
        _record.DOPunchScale(Vector3.one, 1, 1);
        PlayerPrefs.Save();
      }
    }
  }
  
}