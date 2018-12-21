using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ErasureManager : MonoBehaviour //Class to manage task with Erasure Type
{
    //References from tne Inspector
    public Material maskMaterial;
    public GameObject spatulaPrefab;
    public GameObject huskPrefab;
    public Transform erasureCanvas;
    public Vector3 penOffest = Vector3.up;

    private float penSpeed; //Erasing speed
    private Texture2D penTexture, maskTexture, eraser;
    private Image taskImage;
    private BoxCollider2D colliderBox; //Images bounds
    private Collider2D colliderMatch;
    private Color[] pencil; //Array of pen pixels
    private Vector3 currentPosition, tempPoint, moveToPoint;
    private Vector2 origin;
    private int pixelPerWorldUnitX, pixelPerWorldUnitY;
    private int imageWidth, imageHeight;
    private int counter;
    private string path;
    private Color[] tempArea; //Array of target area pixels
    private GameObject spatula;
    private static Queue<GameObject> huskPool; //Pooling system
    private int fullPixelCounter;
    private float timer;

    public void OnStart(LevelInfo data) //Initializing method called from the UIMnager at the level start
    {
        Initialize(data);
        InitializePenSize();
        CreateAlphaMask(data.maskPath);
    }

    private void Initialize(LevelInfo data) //Erasure type level initializer
    {
        penSpeed = DataManager.Instance.penSpeed;
        penTexture = DataManager.Instance.pen;
        erasureCanvas.gameObject.SetActive(true);
        path = Path.Combine(Application.persistentDataPath, data.directoryName + "mask.png"); //Path for saving alphamask
        LevelManager.OnDataSave += SavePNG; //Subscribe for datasaving event
        spatula = Instantiate(spatulaPrefab, gameObject.transform.parent); //Instantiate a spatula prefab
        huskPool = new Queue<GameObject>(); //Object pooling system
        taskImage = GetComponent<Image>(); //Reference to the main image of the current task
        taskImage.material = maskMaterial;
        imageWidth = taskImage.mainTexture.width;
        imageHeight = taskImage.mainTexture.height;
        colliderBox = GetComponent<BoxCollider2D>();
        origin = colliderBox.transform.TransformPoint(new Vector3(-colliderBox.size.x / 2, -colliderBox.size.y / 2)); //(0,0) coordinates in the world space of the images pixel grid
        pixelPerWorldUnitX = Mathf.RoundToInt(imageWidth / Camera.main.orthographicSize); //One world unit in images pixels
        pixelPerWorldUnitY = Mathf.RoundToInt(imageHeight / Camera.main.orthographicSize);

        if (LettersBoard.ratio2) //Is 18:9 ratio?
        {
            pixelPerWorldUnitX += 30; //Cause main canvas scaled, world units are needed to be encreased
            pixelPerWorldUnitY += 30;
        }
    }

    private void SavePNG(LevelInfo data) //Save current alphamask to the file
    {
        Texture2D tex = (Texture2D)taskImage.material.GetTexture("_Alpha");
        byte[] rawdata = tex.EncodeToPNG();
        if (data.maskPath != null && data.maskPath != "")
        {
            File.WriteAllBytes(data.maskPath, rawdata);
        }
    }

    private void CreateAlphaMask(string path) //Create new alphamask or load it from the file
    {

        maskTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.ARGB32, false);

        if (path == null || path == "")
        {
            var pixels = maskTexture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }
            maskTexture.SetPixels(pixels);
        }
        else
        {
            maskTexture.LoadImage(File.ReadAllBytes(path));
        }

        maskTexture.Apply();
        taskImage.material.SetTexture("_Alpha", maskTexture);
    }



    private void InitializePenSize() //Pen texture rescaling depending on the images resolution
    {
        pencil = penTexture.GetPixels();
        eraser = new Texture2D(penTexture.width, penTexture.height, TextureFormat.ARGB32, false);
        eraser.SetPixels(pencil);
        TextureScaler.Bilinear(eraser, (int)(eraser.width / (960f / imageWidth)), (int)(eraser.height / (960f / imageHeight)));
        pencil = null;
        pencil = eraser.GetPixels();
    }

    void Update()
    {

        if (Input.GetMouseButton(0) && !LevelManager.isPaused)
        {
            currentPosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveToPoint = currentPosition + penOffest;
            colliderMatch = Physics2D.OverlapPoint(moveToPoint);

            if (colliderBox == colliderMatch) //If the mouse or touch point hits our image collider
            {
                TryToDraw();
            }
            else
            {
                tempPoint = Vector3.zero;
            }
        }

        if (Input.GetMouseButtonUp(0)) //Touch stopped
        {
            spatula.SetActive(false);
            tempPoint = Vector3.zero;
        }

        if (spatula.activeSelf)
        {
            spatula.transform.position = currentPosition; //Make the spatula to follow cursor or touches
        }
    }

    private void TryToDraw()
    {
        if (tempPoint == Vector3.zero)
        {
            tempPoint = moveToPoint;
        }
        if (tempPoint != moveToPoint) //If there is new point to move
        {
            if (StaticBehaviors.EnoughCoinsForHint(5, false))
            {
                DrawOnMask();

                if (!spatula.activeSelf)
                {
                    spatula.SetActive(true);
                }
            }
        }
    }

    private void DrawOnMask()
    {
        Vector3 newPoint = new Vector3((Mathf.Lerp(tempPoint.x, moveToPoint.x, penSpeed)),
            (Mathf.Lerp(tempPoint.y, moveToPoint.y, penSpeed)), moveToPoint.z); //Smooth out new point to prevent big erasure jumps

        tempPoint = newPoint;
        timer -= Time.deltaTime;

        int targetPixelX = Mathf.RoundToInt((tempPoint.x + Mathf.Abs(origin.x)) * (pixelPerWorldUnitX)); //Calculate target pixel
        int targetPixelY = Mathf.RoundToInt((tempPoint.y + Mathf.Abs(origin.y)) * (pixelPerWorldUnitY));


        //Hold pixel target in bounds

        if (targetPixelX + eraser.width > imageWidth)
        {
            targetPixelX -= (targetPixelX + eraser.width) - imageWidth;
        }
        if (targetPixelY + eraser.height > imageHeight)
        {
            targetPixelY -= (targetPixelY + eraser.height) - imageHeight;
        }

        DrawFrame(targetPixelX, targetPixelY);

        if (counter % 10 == 0)
        {
            GetFromPool().transform.position = tempPoint; //Create husks
        }

    }

    private void DrawFrame(int x, int y)
    {
        tempArea = maskTexture.GetPixels(x, y, eraser.width, eraser.height); //Area on the mask to be painted out

        fullPixelCounter = 0;

        //Compare area pixels with pen pixels
        for (int i = 0; i < tempArea.Length; i++)
        {
            if (tempArea[i] != Color.white)
            {
                tempArea[i] = tempArea[i].a < pencil[i].a ? pencil[i] : tempArea[i];
            }
            else
            {
                fullPixelCounter++; //Check if all pixels painted out
            }
        }

        if (fullPixelCounter < tempArea.Length) //Draw new pixels on the area
        {

            maskTexture.SetPixels(x, y, eraser.width, eraser.height, tempArea);
            maskTexture.Apply();
            taskImage.material.SetTexture("_Alpha", maskTexture);
            counter++; //Drawn frames
            if (counter == 30)
            {
                counter = 0;
                UIManager.HintEvent(5, path);
                tempArea = null;
            }
        }
    }

    private GameObject GetFromPool() //Take a husk from the pool or instantiate new one if needed
    {
        if (huskPool.Count == 0 || huskPool == null)
        {
            GameObject newGo = Instantiate(huskPrefab, gameObject.transform);
            huskPool.Enqueue(newGo);
        }
        GameObject temp = huskPool.Dequeue();
        temp.SetActive(true);
        return temp;
    }

    public static void ReturnToPool(GameObject husk)
    {
        husk.SetActive(false);
        huskPool.Enqueue(husk);
    }

    public void OnDestroy()
    {
        LevelManager.OnDataSave -= SavePNG;
    }
}
