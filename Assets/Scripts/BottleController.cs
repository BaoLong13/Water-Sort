using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    private const int _MAXCOLOR = 4;

    public Color[] bottleColors;
    public SpriteRenderer bottleMaskSR;
    public AudioSource pourSound;
    public AudioSource fullSound;

    public float timeToRotate = 1f;

    public AnimationCurve scaleAndRotationMultiplierCurve;
    public AnimationCurve fillAmountCurve;
    public AnimationCurve rotationMultiplierCurve;

    [Range(0, 4)]
  
    public float[] fillAmounts;
    public float[] rotationValues;

    private int rotationIndex = 0;

    public int colorsToTransfer = 0;
    public int colorsInBottle = 0;
    public Color topColor;
    public int topColorLayers = 0;
 
    public BottleController bottleControllerRef;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;

    private Transform chosenRotation;
    private float directionMultiplier = 1.0f;
    public bool fullColorStack = false;

    private Vector3 originalPosition;
    private Vector3 startPosition;
    private Vector3 endPosition;

    public LineRenderer lineRenderer;

    void Start()
    {
        UpdateBottleState();
        originalPosition = transform.position;

    }

    public void UpdateBottleState()
    {
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[colorsInBottle]);
        UpdateColorOnShader();
        UpdateTopColorValues();
    }

    public void TransferColors()
    {
        ChooseRotationDirection();
        colorsToTransfer = Mathf.Min(topColorLayers, _MAXCOLOR - bottleControllerRef.colorsInBottle);

        for (int i = 0; i < colorsToTransfer; ++i)
        {
            int index = Mathf.Abs(bottleControllerRef.colorsInBottle + i);
            bottleControllerRef.bottleColors[index] = topColor;
        }

        bottleControllerRef.UpdateColorOnShader();

        CalculateRotationIndex(_MAXCOLOR - bottleControllerRef.colorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        bottleMaskSR.sortingOrder += 2;

        StartCoroutine(MoveBottle());
    }

    public void UpdateColorOnShader()
    {
        bottleMaskSR.material.SetColor("_C1", bottleColors[1]);
        bottleMaskSR.material.SetColor("_C2", bottleColors[0]);
        bottleMaskSR.material.SetColor("_C3", bottleColors[2]);
        bottleMaskSR.material.SetColor("_C4", bottleColors[3]);
    }

    IEnumerator MoveBottle() 
    {
        startPosition = transform.position;
        if (chosenRotation == leftRotationPoint)
        {
            endPosition = bottleControllerRef.rightRotationPoint.position;
        }
        else
        {
            endPosition = bottleControllerRef.leftRotationPoint.position;
        }

        float t = 0;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;

        StartCoroutine(RotateBottle());
    }

    IEnumerator ReturnBottle()
    {
        startPosition = transform.position;
        endPosition = originalPosition;

        float t = 0;
        while (t < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime * 2;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPosition;

        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        bottleMaskSR.sortingOrder -= 2;

        if (GameManager.Instance.CheckWinCondition())
        {
            StartCoroutine(GameManager.Instance.MoveToNextStage());
        }
    }

    IEnumerator RotateBottle()
    {
        float time = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = 0;

        while(time < timeToRotate)
        {
            lerpValue = time / timeToRotate;
            angleValue = Mathf.Lerp(0f, directionMultiplier * rotationValues[rotationIndex], lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotation.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_ScaleRotation", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmounts[colorsInBottle] > fillAmountCurve.Evaluate(angleValue) + 0.05f)
            {
                if (lineRenderer.enabled == false)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;

                    lineRenderer.SetPosition(0, chosenRotation.position);
                    lineRenderer.SetPosition(1,chosenRotation.position - Vector3.up * 4.5f);

                    lineRenderer.enabled = true;
                }

                bottleMaskSR.material.SetFloat("_FillAmount", fillAmountCurve.Evaluate(angleValue));
                bottleControllerRef.FillUp(fillAmountCurve.Evaluate(lastAngleValue) - fillAmountCurve.Evaluate(angleValue));

            }
            time += Time.deltaTime*rotationMultiplierCurve.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }
        pourSound.Play();
        angleValue = directionMultiplier * rotationValues[rotationIndex];
        //transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleRotation", scaleAndRotationMultiplierCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmountCurve.Evaluate(angleValue));

        colorsInBottle -= colorsToTransfer;
        bottleControllerRef.colorsInBottle += colorsToTransfer;

        bottleControllerRef.UpdateTopColorValues();
        if (bottleControllerRef.topColorLayers == 4)
        {
            fullSound.PlayDelayed(1f);
            bottleControllerRef.GetComponent<Collider2D>().enabled = false;
            bottleControllerRef.fullColorStack = true;
        }

        this.bottleControllerRef = null;

        lineRenderer.enabled = false;

        StartCoroutine(RotateBottleBack());
    }
    IEnumerator RotateBottleBack()
    {
        float time = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = directionMultiplier * rotationValues[rotationIndex];

        while (time < timeToRotate)
        {
            lerpValue = time / timeToRotate;
            angleValue = Mathf.Lerp( directionMultiplier * rotationValues[rotationIndex], 0f, lerpValue);

           // transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotation.position, Vector3.forward, lastAngleValue - angleValue);
            bottleMaskSR.material.SetFloat("_ScaleRotation", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

            lastAngleValue = angleValue;

            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        UpdateTopColorValues();
        angleValue = 0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_ScaleRotation", scaleAndRotationMultiplierCurve.Evaluate(angleValue));

        StartCoroutine(ReturnBottle());
    }

    public void UpdateTopColorValues()
    {
        if (colorsInBottle != 0)
        {
            topColorLayers = 1;
            topColor = bottleColors[colorsInBottle - 1];

            switch (colorsInBottle)
            {
                case 4:
                    if (bottleColors[3].Equals(bottleColors[2]))
                    {
                        topColorLayers = 2;
                        
                        if (bottleColors[2].Equals(bottleColors[1]))
                        {
                            topColorLayers = 3;

                            if (bottleColors[1].Equals(bottleColors[0]))
                            {
                                topColorLayers = 4;
                            }
                        }
                    }
                    break;

                case 3:
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        topColorLayers = 2;
                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            topColorLayers = 3;
                        }
                    }
                    break;
                case 2:
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        topColorLayers = 2;
                    }
                    break;
                default: 
                    break;
            }

            rotationIndex = 3 - (colorsInBottle - topColorLayers);
        }
        else
        {
            topColorLayers = 0;
        }
    }

    
    public bool FillBottleCheck(Color colorToCheck)
    {
        if (colorsInBottle == 0)
        {
            return true;
        }
        else
        {
            if (colorsInBottle == 4)
            {
                return false;
            }
            else
            {
                if(topColor.Equals(colorToCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private void CalculateRotationIndex(int emptySpacesInSecondBottle)
    {
        rotationIndex = 3 - (colorsInBottle - Mathf.Min(emptySpacesInSecondBottle, topColorLayers));
    }

    private void FillUp(float amountToFill)
    {
        bottleMaskSR.material.SetFloat("_FillAmount", bottleMaskSR.material.GetFloat("_FillAmount") + amountToFill);
    }

    private void ChooseRotationDirection()
    {
        if (transform.position.x > bottleControllerRef.transform.position.x)
        {
            chosenRotation = leftRotationPoint;
            directionMultiplier = -1f;
        }
        else
        {
            chosenRotation = rightRotationPoint;
            directionMultiplier = 1f;
        }
    }
    
}
