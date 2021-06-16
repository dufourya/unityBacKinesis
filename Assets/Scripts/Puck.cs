using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Puck : MonoBehaviour
{
    Rigidbody2D _rbPuck;
    SpriteRenderer _srPuck;
    ParticleSystem _particles;
    float _puckHue;
    Color _puckColor;
    float _thrust = 1000.0f;
    float _drag = 10.0f;
    float _torque = 100000.0f;
    float _hueRate = 0.00005f;
    void Awake()
    {
        _rbPuck = GetComponent<Rigidbody2D>();
        _srPuck = GetComponent<SpriteRenderer>();
        _particles = GetComponent<ParticleSystem>();
        _puckHue = Random.value;
        UpdateColors();
        _rbPuck.drag = _drag;
        _rbPuck.angularDrag = _drag;
        _hueRate += Random.value * _hueRate;
    }
    void Start()
    {
        _rbPuck.AddTorque(Random.Range(-_torque, _torque), ForceMode2D.Impulse);
        _rbPuck.AddForce(Rotate(Vector2.up, _rbPuck.rotation) * _thrust);
    }
    void FixedUpdate()
    {
        _rbPuck.AddTorque(Random.Range(-_torque, _torque));
        _rbPuck.AddForce(Rotate(Vector2.up, _rbPuck.rotation) * _thrust);
        _rbPuck.AddForce(-_rbPuck.position.normalized * Mathf.Pow(_rbPuck.position.magnitude/5,2));
        _puckHue = (_puckHue + _hueRate) % 1;
        UpdateColors();
    }
    Vector2 Rotate(Vector2 direction, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        return new Vector2(
            direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
            direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
        ).normalized;
    }
    void UpdateColors()
    {
        _puckColor = Color.HSVToRGB(_puckHue,0.5f,0.3f);
        _srPuck.color = _puckColor;
        ParticleSystem.MainModule psMain = _particles.main;
        Color particleColor= Color.HSVToRGB(_puckHue,1.0f,1.0f);
        particleColor.a = 0.005f;
        psMain.startColor = particleColor;
    }
    void OnMouseDrag(){
        _puckHue = (_puckHue + 0.005f) % 1;
        UpdateColors();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _rbPuck.MovePosition(mousePosition);
    }
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    void OnMouseEnter() {
        Debug.Log(cursorTexture);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    void OnMouseExit() {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
