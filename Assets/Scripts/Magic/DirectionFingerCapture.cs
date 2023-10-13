using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionFingerCapture : MonoBehaviour
{
    public enum directionType { Right, Left, Up, Down, RightDown, LeftDown, RigthUp, LeftUP };
    private bool mouseDown;
    private Vector2 lastMousePosition = Vector3.zero, positionMouse = Vector2.positiveInfinity;
    private List<DirectRecord> directPaths = new List<DirectRecord>();
    private bool record = false;
    private DirectRecord directRecord;
    private directionType nowDirection;
    private float _timer, timercheck = 0.1f;

    private Camera cam;
    private void Update()
    {
        

        positionMouse = Input.mousePosition;
        _timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            cam = Camera.main;
            MouseDown();
        }
        else
        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
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
    private void MouseUp()
    {
        mouseDown = false;
        record = false;
        ResaultMouseDrag();
    }

    private void MouseDown()
    {
        lastMousePosition = positionMouse;
        mouseDown = true;
    }

    private void MouseDrag(Vector2 position)
    {
        if (positionMouse == lastMousePosition) return;

        Vector2 direction = CalculateDirection(position, lastMousePosition);
        RecordDrag(position, lastMousePosition, direction);
    }

    private void ResaultMouseDrag()
    {
        Debug.Log("___________________ResultRecord____________________");
        foreach (var dir in directPaths)
        {
            Debug.Log(dir.directionType + " " + dir.distance);
        }
        Debug.Log("__________________EndResultRecord__________________");
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
        if (!record)
        {
            directRecord = new DirectRecord();
            nowDirection = CheckDirection(direction);
            record = true;
        }
        var heading = position - lastPosition;
        var distance = heading.magnitude;

        Vector3 asd = lastMousePosition;
        asd.z = 1f;
        asd = cam.ScreenToWorldPoint(asd);
        Vector3 qwe = position;
        qwe.z = 1f;
        qwe = cam.ScreenToWorldPoint(qwe);


        Debug.DrawLine(asd, qwe, Color.green);
        //Debug.Log("MouseDrag on coords " + asd + " LastCoord"+ qwe + " direction is " + direction+ "  distance "+ distance);
        Debug.Log("<color=red> {angle = } </color>" + CheckDirection(direction));
        /*if(nowDirection != CheckDirection(direction))
        {
            record = false;
            directRecord.distance = distance;
            directPaths.Add(directRecord);
            lastMousePosition = positionMouse;
        }*/

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
            angle = Mathf.Atan(y0 / x0)* 57.2958f;
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

public struct DirectRecord
{
    public float distance;
    public DirectionFingerCapture.directionType directionType;
}
