using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Capsule : MonoBehaviour
{
    Rigidbody2D _rbCapsule;
    SpriteRenderer _srCapsule;
    TrailRenderer _trailRenderer;
    float _thrust = 100.0f;
    float _drag = 10.0f;
    float _torque = 20.0f;
    bool _isSwimming = true;
    float _capsuleHue;
    List<Rigidbody2D> _rbPucks = new List<Rigidbody2D>();
    List<SpriteRenderer> _srPucks = new List<SpriteRenderer>();
    List<float> _weightHue = new List<float>();
    float _sumDistanceToPucks;    
    void Awake()
    {
        _capsuleHue = Random.value;
        _rbCapsule = GetComponent<Rigidbody2D>();
        _srCapsule = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _srCapsule.color = Color.HSVToRGB(_capsuleHue,0.7f,0.8f);
        _trailRenderer.startColor = Color.HSVToRGB(_capsuleHue,0.7f,0.7f);;
        _trailRenderer.endColor = Color.HSVToRGB(_capsuleHue,0.7f,0.5f);;

        Puck[] puckObjects = GameObject.FindObjectsOfType<Puck>();
        foreach (Puck puck in puckObjects)
        {
            _rbPucks.Add(puck.GetComponent<Rigidbody2D>());
            SpriteRenderer srPuck = puck.GetComponent<SpriteRenderer>();
            _srPucks.Add(srPuck);
            float H,S,V;
            Color.RGBToHSV(srPuck.color, out H, out S, out V);
            float distHue = Mathf.Abs(H-_capsuleHue);
            _weightHue.Add((Mathf.Pow(Mathf.Max(distHue, 1-distHue),2)-0.375f));        
        }
    }
    void Start()
    {
        _rbCapsule.drag = _drag;
        _rbCapsule.angularDrag = _drag;
        _rbCapsule.AddTorque(Random.Range(-_torque, _torque), ForceMode2D.Impulse);
        _rbCapsule.AddForce(Rotate(Vector2.up, _rbCapsule.rotation) * _thrust);
        _sumDistanceToPucks = CalculateSumDistanceToPucks();
    }
    float CalculateSumDistanceToPucks()
    {
        float sumDistances = 0;
        for (int k = 0; k < _rbPucks.Count; k++)
        {            
            float H,S,V;
            Color.RGBToHSV(_srPucks[k].color, out H, out S, out V);
            float distHue = Mathf.Abs(H-_capsuleHue);
            _weightHue[k] = Mathf.Pow(Mathf.Max(distHue, 1-distHue),2)-0.375f;  
            sumDistances += _weightHue[k] * Mathf.Sqrt((_rbCapsule.position - _rbPucks[k].position).magnitude);
        }
        return sumDistances;
    }
    void FixedUpdate()
    {   
        _isSwimming = Random.value > 3*(CalculateSumDistanceToPucks()-_sumDistanceToPucks) + 0.02f;
        if (_isSwimming)
        {
            _rbCapsule.AddTorque(Random.Range(-_torque, _torque));
            _rbCapsule.AddForce(Rotate(Vector2.up, _rbCapsule.rotation) * _thrust);
        } else
        {
            _rbCapsule.AddTorque(Random.Range(-_torque, _torque), ForceMode2D.Impulse);
        }
        _sumDistanceToPucks = CalculateSumDistanceToPucks();
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Wall")
        {
            _rbCapsule.MoveRotation(_rbCapsule.rotation + 180.0f);
            _rbCapsule.AddTorque(Random.Range(-_torque, _torque), ForceMode2D.Impulse);
        } 
    }
    Vector2 Rotate(Vector2 direction, float angle)
    {
        angle = Mathf.Deg2Rad * angle;
        return new Vector2(
            direction.x * Mathf.Cos(angle) - direction.y * Mathf.Sin(angle),
            direction.x * Mathf.Sin(angle) + direction.y * Mathf.Cos(angle)
        ).normalized;
    }
}
