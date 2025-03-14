using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    Button button;
    Image image;

    public GameObject muteIcon;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        button.onClick.AddListener(Mute);
    }

    void Mute()
    {
        AudioManager.instance.Mute();

        muteIcon.SetActive(GameManager.instance.isMute);
    }
}
