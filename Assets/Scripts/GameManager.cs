using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] bottles;
    public  Color[] colorsToSet;

    public GameState state;

    private BottleController firstBottle = null;
    private BottleController secondBottle = null;

    private int currLevelID;
    private int fullBottleCondition;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currLevelID = 1;
        fullBottleCondition = 1;
        HandleGenerateStage();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N)) 
        {
            GameManager.Instance.UpdateGameState(GameState.NextStage);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<BottleController>() != null)
                {
                    if (firstBottle == null)
                    {
                        firstBottle = hit.collider.GetComponent<BottleController>();
                    }
                    else
                    {
                        if (firstBottle == hit.collider.GetComponent<BottleController>())
                        {
                            firstBottle = null;
                        }

                        else
                        {
                            secondBottle = hit.collider.GetComponent<BottleController>();
                            firstBottle.bottleControllerRef = secondBottle;

                            firstBottle.UpdateTopColorValues();
                            secondBottle.UpdateTopColorValues();

                            if (secondBottle.FillBottleCheck(firstBottle.topColor) == true)
                            {
                                firstBottle.TransferColors();
                                firstBottle = null;
                                secondBottle = null;
                            }
                            else
                            {
                                firstBottle = null;
                                secondBottle = null;
                            }
                        }
                    }
                }
            }
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.GenerateStage:
                HandleGenerateStage();
                break;
            case GameState.NextStage:
                HandleNextStage();
                break;
        }
    }

    public bool CheckWinCondition()
    {
        int fullStackCount = 0;
        for (int i = 0; i < bottles.Length; i++)
        {
            if (bottles[i].GetComponent<BottleController>().fullColorStack)
            {
                fullStackCount++;
            }
        }

        if (fullStackCount == fullBottleCondition)
        {
            return true;
        }
        return false;
    }
    
    public IEnumerator MoveToNextStage()
    {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.UpdateGameState(GameManager.GameState.NextStage);
        
    }

    public void HandleNextStage()
    {
        for (int i = 0; i < bottles.Length; ++i)
        {
            if (bottles[i].activeSelf)
            {
                bottles[i].GetComponent<BottleController>().colorsInBottle = 0;
                bottles[i].GetComponent<BottleController>().colorsToTransfer = 0;
                bottles[i].GetComponent<BottleController>().topColorLayers = 0;
                bottles[i].GetComponent<BottleController>().fullColorStack = false;
                bottles[i].GetComponent<BottleController>().topColor = Color.black;
                bottles[i].GetComponent<Collider2D>().enabled = true;
                bottles[i].SetActive(false);
            }      
        }
        currLevelID += 1;
        Debug.Log(currLevelID);
        GameManager.Instance.UpdateGameState(GameState.GenerateStage);
    }    

    private void HandleGenerateStage()
    {
        switch (currLevelID)
        {
            case 1:
                fullBottleCondition = 1;


                bottles[0].GetComponent<BottleController>().colorsInBottle = 2;
                bottles[0].GetComponent<BottleController>().bottleColors[0] = colorsToSet[0];
                bottles[0].GetComponent<BottleController>().bottleColors[1] = colorsToSet[0];
                bottles[0].GetComponent<BottleController>().UpdateBottleState();

                bottles[0].SetActive(true);    
                    
                bottles[1].GetComponent<BottleController>().colorsInBottle = 2;
                bottles[1].GetComponent<BottleController>().bottleColors[0] = colorsToSet[0];
                bottles[1].GetComponent<BottleController>().bottleColors[1] = colorsToSet[0];
                bottles[1].GetComponent<BottleController>().UpdateBottleState();


                bottles[1].SetActive(true);
                break;

            case 2:
                fullBottleCondition = 2;


                bottles[0].GetComponent<BottleController>().colorsInBottle = 3;
                bottles[0].GetComponent<BottleController>().bottleColors[0] = colorsToSet[1];
                bottles[0].GetComponent<BottleController>().bottleColors[1] = colorsToSet[1];
                bottles[0].GetComponent<BottleController>().bottleColors[2] = colorsToSet[0];
                bottles[0].GetComponent<BottleController>().UpdateBottleState();

                bottles[0].SetActive(true);


                bottles[1].GetComponent<BottleController>().colorsInBottle = 3;
                bottles[1].GetComponent<BottleController>().bottleColors[0] = colorsToSet[0];
                bottles[1].GetComponent<BottleController>().bottleColors[1] = colorsToSet[0];
                bottles[1].GetComponent<BottleController>().bottleColors[2] = colorsToSet[0];
                bottles[1].GetComponent<BottleController>().UpdateBottleState();

                bottles[1].SetActive(true);


                bottles[2].GetComponent<BottleController>().colorsInBottle = 2;
                bottles[2].GetComponent<BottleController>().bottleColors[0] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().bottleColors[1] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().UpdateBottleState();

                bottles[2].SetActive(true);
                break;

            case 3:
                fullBottleCondition = 3;


                bottles[0].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[0].GetComponent<BottleController>().bottleColors[0] = colorsToSet[2];
                bottles[0].GetComponent<BottleController>().bottleColors[1] = colorsToSet[1];
                bottles[0].GetComponent<BottleController>().bottleColors[2] = colorsToSet[1];
                bottles[0].GetComponent<BottleController>().bottleColors[3] = colorsToSet[0];
                bottles[0].GetComponent<BottleController>().UpdateBottleState();

                bottles[0].SetActive(true);


                bottles[1].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[1].GetComponent<BottleController>().bottleColors[0] = colorsToSet[2];
                bottles[1].GetComponent<BottleController>().bottleColors[1] = colorsToSet[1];
                bottles[1].GetComponent<BottleController>().bottleColors[2] = colorsToSet[2];
                bottles[1].GetComponent<BottleController>().bottleColors[3] = colorsToSet[2];
                bottles[1].GetComponent<BottleController>().UpdateBottleState();

                bottles[1].SetActive(true);

                bottles[2].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[2].GetComponent<BottleController>().bottleColors[0] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().bottleColors[1] = colorsToSet[0];
                bottles[2].GetComponent<BottleController>().bottleColors[2] = colorsToSet[0];
                bottles[2].GetComponent<BottleController>().bottleColors[3] = colorsToSet[0];
                bottles[2].GetComponent<BottleController>().UpdateBottleState();

                bottles[2].SetActive(true);

                bottles[3].GetComponent<BottleController>().UpdateBottleState();
                bottles[3].SetActive(true);

                bottles[4].GetComponent<BottleController>().UpdateBottleState();
                bottles[4].SetActive(true);

                break;

            case 4:
                fullBottleCondition = 3;
                bottles[0].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[0].GetComponent<BottleController>().bottleColors[0] = colorsToSet[3];
                bottles[0].GetComponent<BottleController>().bottleColors[1] = colorsToSet[3];
                bottles[0].GetComponent<BottleController>().bottleColors[2] = colorsToSet[2];
                bottles[0].GetComponent<BottleController>().bottleColors[3] = colorsToSet[1];
                bottles[0].GetComponent<BottleController>().UpdateBottleState();

                bottles[0].SetActive(true);

                bottles[1].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[1].GetComponent<BottleController>().bottleColors[0] = colorsToSet[2];
                bottles[1].GetComponent<BottleController>().bottleColors[1] = colorsToSet[3];
                bottles[1].GetComponent<BottleController>().bottleColors[2] = colorsToSet[2];
                bottles[1].GetComponent<BottleController>().bottleColors[3] = colorsToSet[3];
                bottles[1].GetComponent<BottleController>().UpdateBottleState();

                bottles[1].SetActive(true);

                bottles[2].GetComponent<BottleController>().colorsInBottle = 4;
                bottles[2].GetComponent<BottleController>().bottleColors[0] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().bottleColors[1] = colorsToSet[2];
                bottles[2].GetComponent<BottleController>().bottleColors[2] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().bottleColors[3] = colorsToSet[1];
                bottles[2].GetComponent<BottleController>().UpdateBottleState();

                bottles[2].SetActive(true);

                bottles[3].GetComponent<BottleController>().UpdateBottleState();
                bottles[3].SetActive(true);

                bottles[4].GetComponent<BottleController>().UpdateBottleState();
                bottles[4].SetActive(true);
                break;
        }
    }

    public enum GameState
    {
        GenerateStage,
        NextStage,
    }
}
