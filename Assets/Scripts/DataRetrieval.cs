using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEditor.EditorTools;
using UnityEngine;

public class DataRetrieval : MonoBehaviour
{

    [Tooltip("The default speed at which this object moves")]
    [SerializeField] float moveSpeed = 1f;
    [Tooltip("The default speed at which this object rotates")]
    [SerializeField] float rotateSpeed = 1f;

    //----------------------------------------------------------------------------------------
    // Corutines - All courtines will be listed here for reference

    private Coroutine moveToPosition;
    private Coroutine rotateCoroutine;

    //----------------------------------------------------------------------------------------

    /**
        This method is called from the WebListener and recieves any packets
        obtained from the WebListener
    */
    public void RecievePacket(Dictionary<string, object> packet)
    {
        InterpretPacket(packet);
    }

    private void InterpretPacket(Dictionary<string, object> packet)
    {
        foreach (KeyValuePair<string, object> pair in packet)
        {
            /**
                This foreach loop will iterate through the packet and perform the desired
                instruction given within the packet by calling various methods of known keys
            */
            switch (pair.Key)
            {
                case "position":
                    Position(pair.Value);
                    break;
                case "rotate":
                    Rotate(pair.Value);
                    break;
                case "stop":
                    StopMoving();
                    StopRotating();
                    break;
                case "color":
                    ColorChange(pair.Value);
                    break;
                default:
                    Debug.Log("Error: No such command exists -> " + pair.Key);
                    break;
            }
        }
    }

    /**
        Changes the objects current position
        Format: "X,Y,Z" or "X,Y,Z,S" where is S is speed.
        If S is not present then the object default speed will be used.
    */
    private void Position(object value)
    {
        if (CheckIfNull(value)) { return; } // Checks to make sure value isn't null
        string[] coords = value.ToString().Split(",");
        if (!CheckValueSize(new int[] { 3, 4 }, coords.Length)) { return; } // Checks to make sure there are 3 or 4 arguments
        try
        {
            Vector3 newPos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
            StopMoving(); // Stops the object from moving if it is
            if (coords.Length == 4)
            {
                moveToPosition = StartCoroutine(MoveToPosition(newPos, float.Parse(coords[3])));
            }
            else
            {
                moveToPosition = StartCoroutine(MoveToPosition(newPos, moveSpeed));
            }
        }
        catch (ArgumentException e)
        {
            Debug.Log("Invalid Coordinates Given. Coordinates must of of type float\n" + e.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /**
        Moves an object to its new position
    */
    IEnumerator MoveToPosition(Vector3 newPosition, float speed)
    {
        Debug.Log("Moving: " + newPosition.ToString());
        while (gameObject.transform.position != newPosition)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, newPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    /**
        Stops the object in its current position
    */
    private void StopMoving()
    {
        if (moveToPosition != null) { StopCoroutine(moveToPosition); }
    }

    /**
        Rotates an object to its new angle
        Format X,Y,Z or X,Y,Z,S where S is speed
    */
    private void Rotate(object value)
    {
        if(CheckIfNull(value)) { return; }
        string[] coords = value.ToString().Split(",");
        if (!CheckValueSize(new int[] { 3, 4 }, coords.Length)) { return; } // Checks to make sure there are 3 or 4 arguments
        try
        {
            Vector3 newPos = new Vector3(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2]));
            StopRotating(); // Stops the object from rotating if it currently is
            if (coords.Length == 4)
            {
                rotateCoroutine = StartCoroutine(RotateToPosition(newPos, float.Parse(coords[3])));
            }
            else
            {
                rotateCoroutine = StartCoroutine(RotateToPosition(newPos, rotateSpeed));
            }
        }
        catch (ArgumentException e)
        {
            Debug.Log("Invalid Coordinates Given. Coordinates must of of type float\n" + e.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    IEnumerator RotateToPosition(Vector3 newPosition, float speed)
    {
        Debug.Log("Rotating: " + newPosition.ToString());
        Quaternion rotateTo = new Quaternion();
        rotateTo.eulerAngles = newPosition;
        while(transform.rotation.eulerAngles != rotateTo.eulerAngles)
        {
            Debug.Log("Rotating: " + transform.rotation.ToString());
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, rotateTo, speed * Time.deltaTime);
            yield return null;
        }
    }

    /**
        Stops the objects rotation in its new position
    */
    private void StopRotating()
    {
        if (rotateCoroutine != null) { StopCoroutine(rotateCoroutine); }
    }


    /**
        Changes the current color of the object
        Format "r,g,b"
    */
    private void ColorChange(object color)
    {
        if (CheckIfNull(color)) { return; }
        string[] rgbCode = color.ToString().Split(",");
        if (!CheckValueSize(3, rgbCode.Length)) { return; }
        Renderer renderer = gameObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.Log("Color Change Failed: Object doesn't contain a renderer");
            return;
        }
        try
        {
            Color newColor = new Color(float.Parse(rgbCode[0]) / 255, float.Parse(rgbCode[1]) / 255, float.Parse(rgbCode[2]) / 255);
            Debug.Log("Changing Color: " + color.ToString());
            renderer.material.color = newColor;
        }
        catch (ArgumentException e)
        {
            Debug.Log("Invalid RGB Given. Coordinates must of of type int\n" + e.ToString());
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /**
        Checks to see a value is null. If 
    */
    private bool CheckIfNull(object value)
    {
        if (value == null)
        {
            Debug.Log("Error: Given value is null");
            return true;
        }
        return false;
    }

    /**
        Checks to see if the value is a correct aniticipated size.
        There can be multiple sizes it could be
    */
    private bool CheckValueSize(int[] sizes, int actualSize)
    {
        foreach (int size in sizes)
        {
            if (size == actualSize) { return true; }
        }
        Debug.Log("Invalid command given. Expected " + sizes.ToString() + " arguments. Recieved: " + actualSize);
        return false;
    }

    /**
        Checks to see if the value is a correct aniticipated size.
    */
    private bool CheckValueSize(int size, int actualSize)
    {
        if (size == actualSize)
        {
            return true;
        }
        Debug.Log("Invalid command given. Expected " + size + " arguments. Recieved: " + actualSize);
        return false;
    }

}
