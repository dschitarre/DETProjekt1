using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerGui : MonoBehaviour {
    GameObject player;
    GameObject infektionsrate, lebenAnzahl, coronaSpritzenLeft, koSpritzenLeft, raketenLeft, ladeanzeige;
    void Start() {
        player = GameObject.Find("player");
        infektionsrate = GameObject.Find("Canvas/infektionsrate");
        lebenAnzahl = GameObject.Find("Canvas/lebenAnzahl");
        coronaSpritzenLeft = GameObject.Find("Canvas/coronaSpritzenLeft");
        koSpritzenLeft = GameObject.Find("Canvas/koSpritzenLeft");
        raketenLeft = GameObject.Find("Canvas/raketenLeft");
    }

    void Update() {
        infektionsrate.GetComponent<TextMeshProUGUI>().SetText( (100 * Game.Instance.anzahlInfizierte / Game.Instance.anzahlNormalos).ToString() + "%");
        lebenAnzahl.GetComponent<TextMeshProUGUI>().SetText(player.GetComponent<PlayerMovement>().leben.ToString());
        coronaSpritzenLeft.GetComponent<TextMeshProUGUI>().SetText(player.GetComponent<Shooting>().impfDosen.ToString());
        koSpritzenLeft.GetComponent<TextMeshProUGUI>().SetText(player.GetComponent<Shooting>().koBullets.ToString());
        raketenLeft.GetComponent<TextMeshProUGUI>().SetText(player.GetComponent<Shooting>().raketen.ToString());
    }
}