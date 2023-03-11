using Fusion;
using FusionExamples.Tanknarok;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftCoolDown : MonoBehaviour
{
    [SerializeField]
    public GameObject _player;
    // Start is called before the first frame update
    [SerializeField]
    public static float cooldown = 0;
    void Start()
    {
        
    }

    public void SetCoolDown(float _cooldown)
    {
        cooldown = _cooldown;
        GetComponent<Image>().fillAmount = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Image>().fillAmount = cooldown;
    }
}
