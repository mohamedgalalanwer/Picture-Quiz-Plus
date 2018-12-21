using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour //Gallery component class
{
    //Ui elements references
    public Button back, arrowRight, arrowLeft;
    public Image image;
    public Text directory, task, reward, number;

    private int counter = 0;
    private string dirName;
    private Texture2D newTex;
    private Sprite newSprite;
    private TaskExample[] levelsData;

    public void OnStart(string dirName)
    {
        this.dirName = dirName;
        directory.text = DataManager.Instance.GetData(dirName).localizedDir.ToUpper();
        levelsData = DataManager.Instance.GetTasksInfo(dirName);
        arrowRight.onClick.AddListener(OnArrowRight);
        arrowLeft.onClick.AddListener(OnArrowLeft);
        SetValues(counter);
        back.onClick.AddListener(() => Destroy(gameObject));
    }

    private void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Destroy(gameObject);
        }
    }

    private void SetValues(int taskNumber)
    {
        task.text = levelsData[counter].rightAnswer.ToUpper();
        reward.text = levelsData[counter].cost != 0 ?
        "+" + levelsData[counter].cost.ToString().ToUpper() : "+" + DataManager.Instance.defaultWinCoins.ToString();
        number.text = levelsData[counter].number.ToString();
        newTex = Resources.Load(dirName + "\\" + (counter + 1).ToString()) as Texture2D;
        newSprite = Sprite.Create(newTex, new Rect(0, 0, newTex.width, newTex.height), new Vector2(0.5f, 0.5f));
        image.sprite = newSprite;
    }

    private void OnArrowRight()
    {
        if (counter < levelsData.Length - 1)
        {
            SetValues(++counter);
            MusicManager.instance.PlaySound("blup");
        }
    }
    private void OnArrowLeft()
    {
        if (counter > 0)
        {
            SetValues(--counter);
            MusicManager.instance.PlaySound("blup");
        }
    }
}
