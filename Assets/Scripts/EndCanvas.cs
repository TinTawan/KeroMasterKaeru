using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fliesNum, canvasText;
    [SerializeField] private Slider fliesSlider;

    [SerializeField] private GameObject FogBlock;

    bool playSound;
    bool oneTimePlay = true;
    

    void Update()
    {
        UpdateDisplay();
        if(playSound)
        {
            AudioManager am = FindObjectOfType<AudioManager>();
            am.StartAudio(AudioManager.Sound.flyRequirement, transform.position, .7f);
            playSound = false;
        }
    }

    void UpdateDisplay()
    {
        fliesNum.text = Manager.flies.ToString();
        fliesSlider.value = Manager.flies;

        //when player gets 10 flies, remove block and text on world view canvas
        if (Manager.flies >= 10)
        {
            FogBlock.SetActive(false);

            fliesNum.enabled = false;
            canvasText.enabled = false;
        }
        if(!Manager.tutorialComplete && Manager.flies == 10 && oneTimePlay)
        {
            playSound = true;
            oneTimePlay = false;
        }
    }
}
