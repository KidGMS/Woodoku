using Script;
using UnityEngine;

public class Sound : MonoBehaviour {
  [SerializeField]
  private AudioSource _soundSetSlot;

  [SerializeField]
  private AudioSource _soundGameOver;

  [SerializeField]
  private AudioSource _soundError;

  [SerializeField]
  private AudioSource _clearField;

  private void Start() {
    MyEvents.SoundSetSlot += SoundSetSlot;
    MyEvents.SoundGameOver += SoundGameOver;
    MyEvents.SoundError += SoundError;
    MyEvents.SoundClear += SoundClear;
  }

  private void OnDestroy() {
    MyEvents.SoundGameOver -= SoundGameOver;
    MyEvents.SoundSetSlot -= SoundSetSlot;
    MyEvents.SoundError -= SoundError;
    MyEvents.SoundClear -= SoundClear;
  }

  private void SoundClear() {
    _clearField.Play();
  }

  private void SoundError() {
    _soundError.Play();
  }

  private void SoundGameOver() {
    _soundGameOver.Play();
  }

  private void SoundSetSlot() {
    _soundSetSlot.Play();
  }
}