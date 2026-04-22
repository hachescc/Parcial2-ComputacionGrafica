using UnityEngine;

public class IntroBoss : MonoBehaviour
{
    [Header("Panel de intro")]
    public GameObject panelIntroBoss;

    void Start()
    {
        if (panelIntroBoss != null)
        {
            panelIntroBoss.SetActive(true);
        }
    }

    public void CerrarIntroIniciarCombate()
    {
        if (panelIntroBoss != null)
        {
            panelIntroBoss.SetActive(false);
        }

        if (IABoss.Instance != null)
        {
            IABoss.Instance.IniciarCombateBoss();
        }
    }
}