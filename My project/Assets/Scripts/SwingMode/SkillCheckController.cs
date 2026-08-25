using UnityEngine;
using UnityEngine.Events;

public class SkillCheckController : MonoBehaviour
{
    [Header("References UI")]
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private RectTransform cursor;

    [Header("Parametres de la zone verte")]
    [SerializeField] private float greenZoneMinWidth = 50f;
    [SerializeField] private float greenZoneMaxWidth = 200f;

    [Header("Evenements")]
    public UnityEvent onSuccess;
    public UnityEvent onFail;

    private float _containerHeight;
    private float _direction = 1f;
    private float _cursorY;
    private float _currentSpeed;
    private bool _isRunning;

    // -------------------------------------------------------------------------
    // API publique — appelée par ForcePhase
    // -------------------------------------------------------------------------

    public void StartSkillCheck(float speed)
    {
        _containerHeight = container.rect.height;
        PlaceGreenZoneRandomly();
        _cursorY = 0f;
        _direction = 1f;
        _currentSpeed = speed;
        _isRunning = true;
        gameObject.SetActive(true);
    }

    public void Confirm()
    {
        if (!_isRunning) return;

        _isRunning = false;
        gameObject.SetActive(false);

        float zoneMin = greenZone.anchoredPosition.y;
        float zoneMax = zoneMin + greenZone.sizeDelta.y;
        bool success = _cursorY >= zoneMin && _cursorY <= zoneMax;

        if (success) onSuccess?.Invoke();
        else onFail?.Invoke();
    }

    public void Cancel()
    {
        _isRunning = false;
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------
    // Logique interne
    // -------------------------------------------------------------------------

    private void Update()
    {
        if (!_isRunning) return;

        _cursorY += _direction * _currentSpeed * Time.deltaTime;

        if (_cursorY >= _containerHeight) { _cursorY = _containerHeight; _direction = -1f; }
        else if (_cursorY <= 0f) { _cursorY = 0f; _direction = 1f; }

        cursor.anchoredPosition = new Vector2(cursor.anchoredPosition.x, _cursorY);
    }

    private void PlaceGreenZoneRandomly()
    {
        float zoneWidth = Random.Range(greenZoneMinWidth, greenZoneMaxWidth);
        float posX = Random.Range(0f, _containerHeight - zoneWidth);

        greenZone.sizeDelta = new Vector2(greenZone.sizeDelta.x, zoneWidth);
        greenZone.anchoredPosition = new Vector2(greenZone.anchoredPosition.x, posX);
    }
}