using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour //Component class for a win popup
{
    //UI elements references
    public Image image;
    public Button continueButton;
    public Text answer;
    public Text cost;
    public GameObject firework;
    public GameObject firework2;
    public Transform mainTransform;

    Vector3[] spawnPoints; //Points in the world space for fireworks to spawn

    void Start()
    {
        Initialize();
        StartCoroutine(SpawnFireWorks()); //Spawn fireworks
    }

    private void Initialize()
    {
        spawnPoints = new Vector3[]
        {
            new Vector3(0.03f, -2.669f, 0),
            new Vector3(2f, -2.569f, 0),
            new Vector3(-2f, -2.569f, 0)
        };

        //Set up the victory event data
        image.sprite = GameObject.FindGameObjectWithTag("winImage").GetComponent<Image>().sprite;
        GetComponent<Image>().color = UIManager.levelColor;
        answer.text = LevelManager.rightAnswer.ToUpper();
        cost.text = "+" + LevelManager.completeLvlCoins.ToString();
        continueButton.onClick.AddListener(() => StaticBehaviors.OnLevelComplete(LevelManager.GetCurrentState()));
        MusicManager.instance.PlaySound("finish");
    }

    IEnumerator SpawnFireWorks()
    {
        while (LevelManager.isPaused)
        {
            yield return new WaitForSeconds(1f); //Delay
            Fire(1);
            yield return new WaitForSeconds(0.3f);
            Fire(2);
            yield return new WaitForSeconds(0.3f);
            Fire(3);
        }
    }

    public void Fire(int count) //Logic for fireworks spawning with random colors
    {
        //Create a gameobject from the prefab in the needed point
        GameObject temp = count == 1 ?
        Instantiate(firework, spawnPoints[count - 1], Quaternion.identity, mainTransform) :
        Instantiate(firework2, spawnPoints[count - 1], Quaternion.identity, mainTransform); 

        Color newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        ParticleSystem[] child = temp.GetComponentsInChildren<ParticleSystem>(); 

        //Paint over all particles with new random color
        for (int i = 0; i < child.Length; i++)
        {
            var module = child[i].main;
            module.startColor = newColor;
        }

        MusicManager.instance.PlaySound("firework");
        Destroy(temp, 2f); //Dstroy the prefab after 2 seconds
    }




}
