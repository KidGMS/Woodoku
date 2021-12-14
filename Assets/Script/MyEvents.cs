using System;

namespace Script {
  public static class MyEvents {
    public static Action<bool> SpawnShapes;
    public static Action<ShapeControl> RemoveShapes;
    public static Action<int, int> ValidationField;
    public static Action SoundSetSlot;
    public static Action<string> Combo;
    public static Action SoundGameOver;

    public static Action SoundError;
    public static Action SoundClear;

    public static IsGameOver isGameOver;
    public static GameOver gameOver;
    public static GetPoint getPoint;
    public static ClearPoint clearPoint;
    public static ClearShape clearShape;
    public static ClearField clearField;
    public static CheckSpawnShapes checkSpawnShapes;
    public static CheckShapes checkShapes;

    public delegate bool CheckSpawnShapes();

    public delegate void CheckShapes();

    public delegate void ClearField();

    public delegate void ClearShape();

    public delegate void ClearPoint();

    public delegate void GameOver();

    public delegate int GetPoint();

    public delegate void IsGameOver();
  }
}