using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.EventSystems;

public class SelectionChangeEvent : UnityEvent<Transform> { }

public class SelectionTool : MonoBehaviour
{
    public float holdMinDistance = 2f;
    public float raycastMaxDistance = 500f;
    public Texture borderTexture;

    public List<Transform> selected;
    public UnityEvent selectionChangeEvent = new UnityEvent();
    public SelectionChangeEvent itemSelectedEvent = new SelectionChangeEvent();
    public SelectionChangeEvent itemUnselectedEvent = new SelectionChangeEvent();
    public UnityEvent selectionListClearEvent = new UnityEvent();
    
    
    private bool buttonHeld = false;

    private byte maxSelectedItems = 25;

    public Camera _camera;
    public Transform cameraTransform;
    private GameMain gameMain;

    private Ray holdstartray;
    private Ray holdendray;

    private Vector3 mousestartpos;
    private Vector3 holdstartpos;
    void Start()
    {
        gameMain = GameMain.instance;
        if(_camera == null) _camera = gameMain.localCamera;
        if(cameraTransform == null) cameraTransform = _camera.transform;
        
        selected = new List<Transform>(maxSelectedItems);
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            buttonHeld = false;
            mousestartpos = Input.mousePosition;
            onHoldStart();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!buttonHeld)
                onClick();
            else
            {
                buttonHeld = false;
                onHoldEnd();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!buttonHeld && manhattanDistance(mousestartpos, Input.mousePosition) > holdMinDistance)
                buttonHeld = true;
            if(buttonHeld)
                onHold();
        }
    }

    float manhattanDistance(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.z - b.z);
    }

    private Func<Transform, bool> chooseAction()
    {
        Func<Transform, bool> action;
        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool ctrl = Input.GetKey(KeyCode.LeftControl);
        if (!shift && !ctrl)
            clearSelectedList();
        if (ctrl) action = removeItem;
        else action = addSelectedItem;
        return action;
    }

    private void onClick()
    {

        RaycastHit[] hits;
        if ((hits = Physics.RaycastAll(_camera.ScreenPointToRay(Input.mousePosition), raycastMaxDistance)) != null)
        {
            Func<Transform, bool> action = chooseAction();
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Selectable"))
                {
                    action(hit.transform);
                    break;
                }
            }
        }
        holdstartray = _camera.ScreenPointToRay(Input.mousePosition);
    }

    private void onHoldStart()
    {
        holdstartray = _camera.ScreenPointToRay(Input.mousePosition);
        holdstartpos = cameraTransform.position;
    }

    private void onHold()
    {
    }

    private void onHoldEnd()
    {
        Vector3 startpos = Vector3.zero, endpos = Vector3.zero;
        holdendray = _camera.ScreenPointToRay(Input.mousePosition);
         
        RaycastHit hit, hit2;
        if (Physics.Raycast(holdstartray, out hit, raycastMaxDistance)) startpos = hit.point;
        else return;

        if (Physics.Raycast(holdendray, out hit2, raycastMaxDistance)) endpos = hit2.point;
        else return;

        Func<Transform, bool> action = chooseAction();
        findInSelectionRect(startpos, endpos).ForEach(x => action(x));
    }
    public static void DrawRect(Vector3 min, Vector3 max, Color color, float duration)
    {
        UnityEngine.Debug.DrawLine(new Vector3(min.x, 100, min.z), new Vector3(min.x, 100, max.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(min.x, 100, max.z), new Vector3(max.x, 100, max.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(max.x, 100, max.z), new Vector3(max.x, 100, min.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(min.x, 100, min.z), new Vector3(max.x, 100, min.z), color, duration);

        UnityEngine.Debug.DrawLine(new Vector3(min.x, 100, min.z), new Vector3(min.x, 0, min.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(min.x, 100, max.z), new Vector3(min.x, 0, max.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(max.x, 100, min.z), new Vector3(max.x, 0, min.z), color, duration);
        UnityEngine.Debug.DrawLine(new Vector3(max.x, 100, max.z), new Vector3(max.x, 0, max.z), color, duration);
    }

    void OnGUI()
    {
        if (buttonHeld)
        {
            Vector3 diff = cameraTransform.position - holdstartpos;
            Rect rect = GetScreenRect(mousestartpos-new Vector3(diff.x, diff.z), Input.mousePosition);
            DrawScreenRectBorder(rect, 2, Color.red);
        }
    }

    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawBorderRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawBorderRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawBorderRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawBorderRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    void DrawBorderRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, borderTexture);
        GUI.color = Color.white;
    }

    Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public List<Transform> findInSelectionRect(Vector3 a, Vector3 b)
    {
        Bounds bounds = new Bounds();
        bounds.center = a;
        bounds.Encapsulate(b);

        List<Transform> transforms = new List<Transform>();
        gameMain.GetEntitiesInBounds(bounds).ForEach(entity => transforms.Add(entity.GetComponent<Transform>()));

        return transforms;

    }

    public bool addSelectedItem(Transform transform)
    {
        if(selected.Count < maxSelectedItems && !selected.Contains(transform))
        {
            selected.Add(transform);
            transform.GetComponent<Outline>().enabled = true;
            selectionChangeEvent?.Invoke();
            itemSelectedEvent?.Invoke(transform);
            return true;
        }

        Debug.Log("Max selection or selected");
        return false;
            
    }

    public void clearSelectedList()
    {
        foreach (Transform transform in selected)
            transform.GetComponent<Outline>().enabled = false;
        selected.Clear();
        selectionChangeEvent?.Invoke();
        selectionListClearEvent?.Invoke();
    }

    public bool removeItem(Transform transform)
    {
        bool removed = selected.Remove(transform);
        if (removed)
        {
            transform.GetComponent<Outline>().enabled = false;
            selectionChangeEvent?.Invoke();
            itemUnselectedEvent?.Invoke(transform);
        }
        return removed;
    }

    public int selectedCount()
    {
        return selected.Count;
    }
}
