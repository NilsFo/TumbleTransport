using UnityEngine;

namespace UI.Display
{
  public class ClockDisplay : MonoBehaviour {
    private GameState gameState;
    public float minRotation;
    public float maxRotation;
    public float offsetRotation;
    public float maxWorkTime = 12f;

    public GameObject longHand;
    public GameObject shortHand;

    private float _maxTime = Mathf.PI * 2;
    private float _timer = 0;

    void Start() {
      gameState = FindObjectOfType<GameState>();
    }

    void FixedUpdate()
    {
      
      _timer += Time.deltaTime;
      if (_timer > _maxTime)
      {
        _timer = 0;
      }

      {
        var diffRotation = (maxRotation - minRotation);
        
        var newRotationShort = minRotation +
          (diffRotation * (gameState.workTimeLeft / maxWorkTime));
        var newRotationLong = minRotation +
          (diffRotation * (gameState.workTimeLeft - (int)gameState.workTimeLeft));
        longHand.transform.localRotation = Quaternion.Euler(0, 0, (newRotationLong - newRotationLong % 6) - offsetRotation);
        shortHand.transform.localRotation = Quaternion.Euler(0, 0, (newRotationShort - newRotationShort % 6) - offsetRotation);
      }
    }
  }
}