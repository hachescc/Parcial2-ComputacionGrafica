using UnityEngine;

public class MovimientoJugadores : MonoBehaviour
{
    [Header("Velocidad")]
    public float velocidad = 5f;

    [Header("Personajes")]
    public Rigidbody2D rbHeroe1;
    public Rigidbody2D rbHeroe2;
    public Rigidbody2D rbHeroe3;
    public Rigidbody2D rbHeroe4;

    [Header("Transforms")]
    public Transform tHeroe1;
    public Transform tHeroe2;
    public Transform tHeroe3;
    public Transform tHeroe4;

    private float horizontal;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        VoltearPersonajes();
    }

    void FixedUpdate()
    {
        MoverPersonaje(rbHeroe1);
        MoverPersonaje(rbHeroe2);
        MoverPersonaje(rbHeroe3);
        MoverPersonaje(rbHeroe4);
    }

    void MoverPersonaje(Rigidbody2D rb)
    {
        if (rb == null) return;
        rb.linearVelocity = new Vector2(horizontal * velocidad, rb.linearVelocity.y);
    }

    void VoltearPersonajes()
    {
        if (horizontal > 0)
        {
            VoltearTransform(tHeroe1, false);
            VoltearTransform(tHeroe2, false);
            VoltearTransform(tHeroe3, false);
            VoltearTransform(tHeroe4, false);
        }
        else if (horizontal < 0)
        {
            VoltearTransform(tHeroe1, true);
            VoltearTransform(tHeroe2, true);
            VoltearTransform(tHeroe3, true);
            VoltearTransform(tHeroe4, true);
        }
    }

    void VoltearTransform(Transform t, bool izquierda)
    {
        if (t == null) return;
        Vector3 escala = t.localScale;
        escala.x = izquierda ? -Mathf.Abs(escala.x) : Mathf.Abs(escala.x);
        t.localScale = escala;
    }
}