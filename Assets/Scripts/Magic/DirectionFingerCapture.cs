using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DirectionFingerCapture : MonoBehaviour
{
    public Slider sliderOfssset, minLineCount, checkTime, junkPercentSlider;
    public Text text1, text2, text3,text4;

    public enum directionType { Right, Left, Up, Down, RightDown, LeftDown, RigthUp, LeftUP, none };
    private bool mouseDown;
    private Vector2 lastMousePosition = Vector3.zero, positionMouse = Vector2.positiveInfinity, avgDirection = Vector2.zero;
    private List<DirectRecord> directPaths = new List<DirectRecord>();
    private directionType previousDirection = directionType.none, nowDirection, checkDirection = directionType.none;
    private float _timer, timercheck = 0.001f, distanceDrag = 0, distanceLine = 0;
    private int countDirectionInput = 0, countDirectionChanged = 0, junkPercent = 5;
    [SerializeField] int ofsssetDirectionChange = 2;
    public Action<Vector3> OnMouseDown, OnMouseUp, OnMouseDrag;
    public Action OnDirectionChanged;

    private Camera cam;

    private void Start()
    {
        sliderOfssset.onValueChanged.AddListener((value) => { float res = Mathf.Lerp(1f, 10f, value); ofsssetDirectionChange = (int)res; });
        minLineCount.onValueChanged.AddListener((value) => { float res = Mathf.Lerp(1f, 20f, value); minLineCountDirection = (int)res; });
        checkTime.onValueChanged.AddListener((value) => { float res = Mathf.Lerp(0.001f, 0.01f, value); timercheck = res; });
        junkPercentSlider.onValueChanged.AddListener((value) => { float res = Mathf.Lerp(1f, 10f, value); junkPercent = (int)res; });
    }

    private void Update()
    {
        text1.text = ofsssetDirectionChange + "";
        text2.text = minLineCountDirection + "";
        text3.text = timercheck + "";
        text4.text = junkPercent + "";

        lastMousePosition = positionMouse;
        positionMouse = Input.mousePosition;
        _timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            cam = Camera.main;
            MouseDown(positionMouse);
        }
        else
        if (Input.GetMouseButtonUp(0))
        {
            MouseUp(positionMouse);
        }
        else
        if (mouseDown)
        {
            MouseDrag(positionMouse);
        }
        //DrowDebugCircle();
    }

    private void DebugLineTosee()
    {
        Vector3 asd = lastMousePosition;
        asd.z = 1f;
        asd = cam.ScreenToWorldPoint(asd);
        Vector3 qwe = positionMouse;
        qwe.z = 1f;
        qwe = cam.ScreenToWorldPoint(qwe);

        Debug.DrawLine(asd, qwe, Color.green);
        //Debug.Log("MouseDrag on coords " + asd + " LastCoord" + qwe);
    }
    private void DrowDebugCircle()
    {
        for (float x = -1; x <= 1; x += .01f)
        {
            for (float y = -1; y <= 1; y += .01f)
            {
                Vector2 newVector = new Vector2(x, y);

                Color color = Color.blue;
                switch (CheckDirection(newVector))
                {
                    case directionType.Right:
                        color = Color.red;
                        break;
                    case directionType.Left:
                        color = Color.yellow;
                        break;
                    case directionType.Up:
                        color = Color.green;
                        break;
                    case directionType.Down:
                        color = Color.white;
                        break;
                    case directionType.RightDown:
                        color = Color.magenta;
                        break;
                    case directionType.LeftDown:
                        color = Color.grey;
                        break;
                    case directionType.RigthUp:
                        color = Color.cyan;
                        break;
                    case directionType.LeftUP:
                        break;
                }
                Debug.DrawLine(Vector2.zero, newVector, color);
            }
        }
    }
    private void DebugShowResultDrag(List<DirectRecord> directPaths)
    {
        Debug.Log("_________________________Result___________________________");
        foreach (var d in directPaths)
            Debug.Log(d.directionType);
        Debug.Log("__________________________end__________________________");
    }
    private void MouseUp(Vector3 position)
    {
        mouseDown = false;
        ClearAvgDirection();
        ClearMinLineAvgDirection();
        ResaultMouseDrag();
        OnMouseUp?.Invoke(position);
    }

    private void MouseDown(Vector3 position)
    {
        mouseDown = true;
        OnMouseDown?.Invoke(position);
    }

    private void MouseDrag(Vector2 position)
    {
        if (positionMouse == lastMousePosition) return;

        if (_timer >= timercheck)
        {
            _timer = 0;
            Vector2 direction = CalculateDirection(position, lastMousePosition);
            RecordDrag(direction);
        }
        OnMouseDrag?.Invoke(position);
    }

    private void ResaultMouseDrag()
    {
        //DebugShowResultDrag(directPaths);
        directPaths = ClearDragRecordFromJunkDirection();
        //DebugShowResultDrag(directPaths);
        distanceDrag = 0;
        distanceLine = 0;
        checkDirection = directionType.none;
        GameManager.Inst.OnDragRuneEnd?.Invoke(directPaths);
        directPaths.Clear();
    }

    private Vector2 CalculateDirection(Vector2 pos, Vector2 nextPos)
    {
        var heading = nextPos - pos;
        float distance = heading.magnitude;
        Vector2 result = heading / distance;
        return result;
    }

    //Debug.Log("AvgDirection = " + direction);
    //Debug.Log("<color=red> {Direction = } </color>" + dirType);/*go to the left {if left svobodno}*/

    private List<DirectRecord> ClearDragRecordFromJunkDirection()
    {
        if (directPaths.Count == 1)
            return directPaths;

        List<DirectRecord> resDirectPaths = new List<DirectRecord>();

        foreach (var dir in directPaths)
        {
            
            if (dir.distance/distanceDrag * 100f < junkPercent)
            {
                Debug.Log(distanceDrag + "  from " + dir.distance + " = procent " + dir.distance / distanceDrag);
                Debug.Log("<color=green> {DropJunkDirection = } </color>" + dir.directionType);
                continue;
            }
            resDirectPaths.Add(dir);
        }
        return resDirectPaths;
    }

    private void RecordDrag(Vector2 direction)
    {
        var heading = positionMouse - lastMousePosition;
        var distance = heading.magnitude;
        distanceDrag += distance;
        distanceLine += distance;

        Vector2 minLDirection = MinLineAvg(direction);
        minLineDirection = CheckDirection(minLDirection);

        Vector2 allLineAvgDirection = AvgDirection(direction);
        nowDirection = CheckDirection(allLineAvgDirection);

        if (MinLineDirectionChanged(minLineDirection))
        {
            //Debug.Log("<color=red> {minLineDirection = } </color>" + previousDirection);/*go to the left {if left svobodno}*/
            SaveRecord(previousDirection, distanceDrag);
        }
    }

    private bool MinLineDirectionChanged(directionType nowDir)
    {

        if (checkDirection != nowDir)
            countDirectionChanged++;
        else
            countDirectionChanged = 0;

        if (countDirectionChanged >= ofsssetDirectionChange)
        {
            checkDirection = nowDir;

            previousDirection = nowDirection;
            OnDirectionChanged?.Invoke();

            ClearAvgDirection();
            ClearMinLineAvgDirection();
            countDirectionChanged = 0;

            return true;
        }

        return false;
    }

    [SerializeField] int minLineCountDirection;
    directionType minLineDirection = directionType.none;
    Queue<Vector2> minLineDirections = new Queue<Vector2>();
    private Vector3 MinLineAvg(Vector2 direction)
    {
        while (minLineDirections.Count >= minLineCountDirection)
        {
            minLineDirections.Dequeue();
        }

        minLineDirections.Enqueue(direction);
        Vector2 sumOfVectores = Vector2.zero;

        foreach (var vector in minLineDirections)
            sumOfVectores += vector;


        return sumOfVectores * (1f / minLineDirections.Count);
    }

    public void ClearMinLineAvgDirection()
    {
        while (minLineDirections.Count > 0)
        {
            minLineDirections.Dequeue();
        }
    }


    private Vector2 AvgDirection(Vector2 direction)
    {
        countDirectionInput++;
        avgDirection += direction;

        return avgDirection * (1f / countDirectionInput);
    }

    private void ClearAvgDirection()
    {
        countDirectionInput = 0;
        avgDirection = Vector2.zero;
    }

    private void SaveRecord(directionType directionType, float distance)
    {
        if (directionType == directionType.none) return;
        if (directPaths.Count > 0)
        {
            if (directPaths[directPaths.Count - 1].directionType == directionType) return;
            
            directPaths[directPaths.Count - 1].distance = distanceLine;
        }
        DirectRecord directRecord = new DirectRecord();
        directRecord.directionType = directionType;
        directRecord.distance = distanceDrag;
        directPaths.Add(directRecord);
        distanceLine = 0;
        lastMousePosition = positionMouse;
    }

    private directionType CheckDirection(Vector2 dir)
    {

        float angle = CalculateAngleFromDirection(dir);
        //Debug.Log(angle);

        if (angle >= 22.5f && angle < 67.5f)
            return directionType.LeftDown;
        if (angle >= 67.5f && angle < 112.5f)
            return directionType.Down;
        if (angle >= 112.5f && angle < 157.5f)
            return directionType.RightDown;
        if (angle >= 157.5f && angle < 202.5f)
            return directionType.Right;
        if (angle >= 202.5f && angle < 247.5f)
            return directionType.RigthUp;
        if (angle > 247.5f && angle < 292.5f)
            return directionType.Up;
        if (angle >= 292.5f && angle < 337.5f)
            return directionType.LeftUP;

        return directionType.Left;
    }

    private float CalculateAngleFromDirection(Vector2 dir)
    {
        float angle = 0;
        float x0 = (float)dir.x;
        float y0 = (float)dir.y;
        if (x0 > 0 && y0 >= 0)//arctg(y0 / x0), если x0 > 0, y0 >= 0,
            angle = Mathf.Atan(y0 / x0) * 57.2958f;
        if (x0 < 0)//arctg(y0 / x0) + 180 градусов, если x0< 0, y0 - любое,.
            angle = Mathf.Atan(y0 / x0) * 57.2958f + 180f;
        if (x0 > 0 && dir.y < 0)//arctg(y0 / x0) + 360 градусов, если x0 > 0, y0 < 0
            angle = Mathf.Atan(y0 / x0) * 57.2958f + 360f;
        if (x0 == 0 && y0 > 0)//90 градусов, если x0 = 0.y > 0
            angle = 90;
        if (x0 == 0 && y0 < 0) //270 градусов, если x0 = 0, y < 0
            angle = 270;
        return angle;
    }
}

[Serializable]
public class DirectRecord
{
    public float distance;
    public DirectionFingerCapture.directionType directionType;
}
