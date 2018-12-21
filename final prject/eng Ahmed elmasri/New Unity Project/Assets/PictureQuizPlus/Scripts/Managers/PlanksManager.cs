using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanksManager : MonoBehaviour //Class to manage task with Planks Type
{
    //References from the Inspector
    public GameObject plankPrefab;
    public GameObject aimPrefab;
    public GameObject puffPrefab;
    public Button aimButton;
    public Sprite buttonSprite;

    private int gridSize;
    private bool isMovingAimType;
    private Image taskImage;
    private List<GameObject> planks = new List<GameObject>();
    private List<int> planksOppened = new List<int>();
    private Vector2 leftBounds;
    private Vector2 rightBounds;
    private GameObject aimGo;
    private bool isAiming;


    public void OnStart(LevelInfo data) //Initializing method called from the UIMnager at the level start
    {
        isMovingAimType = DataManager.Instance.isMovingAimType;
        gridSize = DataManager.Instance.gridSize;

        //Initialize data
        taskImage = GetComponent<Image>();
        BoxCollider2D boundsBox = GetComponent<BoxCollider2D>();
        float sideX = boundsBox.size.x; //Image size in a local space
        float sideY = boundsBox.size.y;
        leftBounds = boundsBox.transform.TransformPoint(new Vector3(-sideX / 2, -sideY / 2));//Left and bottom sides of the image square
        rightBounds = boundsBox.transform.TransformPoint(new Vector3(sideX / 2, sideY / 2)) - new Vector3(0.20f, 0.20f, 0);//Right and upper sides of the image square

        if (LettersBoard.ratio2)// Is 18:9(IphoneX) ratio?
        {
            leftBounds += new Vector2(0.20f, 0.20f); //Cause main canvas resolution scaled, bounds sides are needed to be decreased
            rightBounds -= new Vector2(0.20f, 0.20f);
        }
        //Configure particles
        var main = puffPrefab.GetComponent<ParticleSystem>().main;
        main.startColor = UIManager.levelColor;

        //Set up planks grid
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(sideX / gridSize, sideY / gridSize);

        for (int i = 0; i < Mathf.Pow(gridSize, 2); i++)
        {
            int x = i;

            //Instantiate, configure and add to collection each plank
            GameObject tempplank = Instantiate(plankPrefab, taskImage.transform);
            tempplank.AddComponent<BoxCollider2D>().size = grid.cellSize + new Vector2(5f, 5f);
            planks.Add(tempplank);
            Button tempBut = planks[x].GetComponent<Button>();
            tempBut.onClick.AddListener(() => HitPlank(x));

            if (isMovingAimType)
            {
                if (DataManager.Instance.RemoveClearButtons)
                {
                    tempplank.GetComponent<Image>().raycastTarget = false;
                }
                tempBut.interactable = false;
            }
        }

        if (data.openedPlanks != null) //Set up planks that are already opened earlier
        {
            foreach (var item in data.openedPlanks)
            {
                planks[item].GetComponent<Image>().enabled = false;
            }
            planksOppened.AddRange(data.openedPlanks);
        }

        if (isMovingAimType) //Configure hint button
        {
            if (DataManager.Instance.RemoveClearButtons) //Force task image to be a hint button
            {
                taskImage.raycastTarget = true;
                Button newBut = taskImage.gameObject.AddComponent<Button>();
                newBut.transition = Selectable.Transition.None;
                aimButton = newBut;
            }
            else
            {
                aimButton.image.sprite = buttonSprite;
                aimButton.GetComponent<RectTransform>().sizeDelta += new Vector2(50, 50);
            }

            aimButton.onClick.RemoveAllListeners();
            aimButton.onClick.AddListener(HandleAimButton);
        }
    }

    private void HitPlank(int number) //When the plank is clicked or shooted
    {
        if (StaticBehaviors.EnoughCoinsForHint(4))
        {
            GameObject currentPlank = planks[number];
            if (currentPlank.GetComponent<Image>().enabled)
            {
                currentPlank.GetComponent<Image>().enabled = false;
                planksOppened.Add(number);
                Destroy(Instantiate(puffPrefab, currentPlank.transform.position, Quaternion.identity, transform), 2); //Particles
                MusicManager.instance.PlaySound("shot");
            }
            else
            {
                MusicManager.instance.PlaySound("miss");
            }
            UIManager.HintEvent(4, planksOppened.ToArray());
        }
    }

    private void Update()
    {
        if (isAiming && Input.GetKeyDown("escape"))
        {
            StopAiming();
        }
    }

    private void HandleAimButton() //Listener for hint button
    {
        if (!isAiming)
        {
            StartAiming();
        }
        else
        {
            Shoot();
        }
    }

    private void StartAiming()
    {
        MusicManager.instance.PlaySound("startAim");
        isAiming = true;
        Vector3 randomStartPoint = new Vector3(Random.Range(leftBounds.x, rightBounds.x), leftBounds.y, 0);
        aimGo = Instantiate(aimPrefab, gameObject.transform.parent);
        aimGo.transform.position = randomStartPoint;
        aimGo.GetComponent<AimHandler>().OnStart(leftBounds, rightBounds);
        if (!DataManager.Instance.RemoveClearButtons)
        {
            aimButton.image.color = Color.red;
            aimButton.GetComponent<Animator>().SetBool("isLooping", true);
        }
    }

    private void Shoot()
    {
        isAiming = false;
        Collider2D colliderOnShot = Physics2D.OverlapPoint(aimGo.transform.position, 1);
        colliderOnShot.gameObject.GetComponent<Button>().onClick.Invoke();
        Destroy(aimGo);
        if (!DataManager.Instance.RemoveClearButtons)
        {
            aimButton.GetComponent<Animator>().SetBool("isLooping", false);
            aimButton.image.color = Color.white;
        }
    }

    private void StopAiming()
    {
        isAiming = false;
        Destroy(aimGo);
        if (!DataManager.Instance.RemoveClearButtons)
        {
            aimButton.GetComponent<Animator>().SetBool("isLooping", false);
            aimButton.image.color = Color.white;
        }
    }
}
