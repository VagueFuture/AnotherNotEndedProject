using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionFingerCapture : MonoBehaviour
{
    public enum directionType { Right, Left, Up, Down, RightDown, LeftDown, RigthUp, LeftUP, none };
    private bool mouseDown;
    private Vector2 lastMousePosition = Vector3.zero, positionMouse = Vector2.positiveInfinity, avgDirection = Vector2.zero;
    private List<DirectRecord> directPaths = new List<DirectRecord>();
    private DirectRecord directRecord;
    private directionType previousDirection,nowDirection, checkDirection = directionType.none;
    private float _timer, timercheck = 0.05f, distanceDrag = 0;
    private int countDirectionInput = 0, countDirectionChanged = 0;
    [SerializeField] int ofsssetDirectionChange = 2;
    public Action<Vector3> OnMouseDown, OnMouseUp, OnMouseDrag;
    public Action OnDirectionChanged;

    private Camera cam;
    private void Update()
    {
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
            if (_timer >= timercheck)
            {
                _timer = 0;
                MouseDrag(positionMouse);
            }
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
    private void MouseUp(Vector3 position)
    {
        mouseDown = false;
        SaveRecord(nowDirection, distanceDrag);
        ClearAvgDirection();
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

        Vector2 direction = CalculateDirection(position, lastMousePosition);
        RecordDrag(position, lastMousePosition, direction);
        OnMouseDrag?.Invoke(position);
    }

    private void ResaultMouseDrag()
    {
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

    private void RecordDrag(Vector2 position, Vector2 lastPosition, Vector2 direction)
    {

        var heading = position - lastPosition;
        var distance = heading.magnitude;
        distanceDrag += distance;

        direction = AvgDirection(direction);
        directionType dirType = CheckDirection(direction);
        //Debug.Log("AvgDirection = " + direction);
        //Debug.Log("<color=red> {Direction = } </color>" + dirType);/*go to the left {if left svobodno}*/

        if (AvgDirectionChanged(dirType))
            SaveRecord(previousDirection, distanceDrag);
    }

    private bool AvgDirectionChanged(directionType nowDir)
    {
        nowDirection = nowDir;
        if (checkDirection == directionType.none)
            checkDirection = nowDir;

        if (checkDirection != nowDir)
            countDirectionChanged++;
        else
            countDirectionChanged = 0;

        if (countDirectionChanged >= ofsssetDirectionChange)
        {
            previousDirection = checkDirection;
            checkDirection = nowDir;
            OnDirectionChanged?.Invoke();
            return true;
        }

        return false;
    }

    private Vector2 AvgDirection(Vector2 direction)
    {
        countDirectionInput++;
        avgDirection = (avgDirection + direction) * (1f / countDirectionInput);

        return avgDirection;
    }

    private void ClearAvgDirection()
    {
        countDirectionInput = 0;
        avgDirection = Vector2.zero;
        checkDirection = directionType.none;
        distanceDrag = 0;
    }

    private void SaveRecord(directionType directionType, float distance)
    {
        directRecord.directionType = directionType;
        directPaths.Add(directRecord);
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
public struct DirectRecord
{
    private float distance;
    public DirectionFingerCapture.directionType directionType;
}
