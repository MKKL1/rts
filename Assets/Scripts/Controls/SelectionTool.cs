using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.EventSystems;
using Assets.Scripts.Controls;

public class SelectionTool : MonoBehaviour
{
    public float raycastMaxDistance = 500f;
    public Texture borderTexture;

    //TODO only entities need list, so it would be better to make two variables for selected items
    // 1 list of selected entities
    // 2 transform variable of selected item that is not entity
    public List<Transform> selected;
    public event Action selectionChangeEvent;
    public event Action<Transform> itemSelectedEvent;
    public event Action<Transform> itemUnselectedEvent;
    public event Action selectionListClearEvent;
    
    private byte maxSelectedItems = 25;

    public Camera _camera;
    public Transform cameraTransform;
    private GameMain gameMain;
    private DrawUIRect drawUIRect;
    private MouseDragDetection mouseDrag;

    private Ray holdstartray;
    private Ray holdendray;
    private Vector3 holdstartpos;


    void Start()
    {
        gameMain = GameMain.instance;
        if (_camera == null) _camera = gameMain.localCamera;
        if(cameraTransform == null) cameraTransform = _camera.transform;

        drawUIRect = new DrawUIRect(borderTexture);
        selected = new List<Transform>(maxSelectedItems);
        mouseDrag = new MouseDragDetection();

        mouseDrag.clickEvent += onClick;
        mouseDrag.holdEndEvent += onHoldEnd;
        mouseDrag.holdStartEvent += onHoldStart;
    }
    
    private Func<Transform, bool> chooseSelectionAction()
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

    private void Update()
    {
        mouseDrag.DragUpdate();
    }

    private void onClick()
    {

        RaycastHit[] hits;
        if ((hits = Physics.RaycastAll(_camera.ScreenPointToRay(Input.mousePosition), raycastMaxDistance)) != null)
        {
            Func<Transform, bool> action = chooseSelectionAction();
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

    private void onHoldEnd()
    {
        Vector3 startpos = Vector3.zero, endpos = Vector3.zero;
        holdendray = _camera.ScreenPointToRay(Input.mousePosition);
         
        RaycastHit hit, hit2;
        if (Physics.Raycast(holdstartray, out hit, raycastMaxDistance)) startpos = hit.point;
        else return;

        if (Physics.Raycast(holdendray, out hit2, raycastMaxDistance)) endpos = hit2.point;
        else return;

        Func<Transform, bool> action = chooseSelectionAction();
        findInSelectionRect(startpos, endpos).ForEach(x => action(x));
    }

    void OnGUI()
    {
        if (mouseDrag.buttonHeld)
        {
            Vector3 diff = cameraTransform.position - holdstartpos; //TODO not really working
            drawUIRect.DrawRect(mouseDrag.mousestartpos - new Vector3(diff.x, diff.z), Input.mousePosition, 2, Color.red);
        }
    }



    public List<Transform> findInSelectionRect(Vector3 a, Vector3 b)
    {
        Bounds bounds = new Bounds();
        bounds.center = a;
        bounds.Encapsulate(b);

        List<Transform> transforms = new List<Transform>();
        //TODO select only entities of owner
        foreach(var entity in gameMain.entityManager.GetEntitiesInBounds(bounds))
        {
            transforms.Add(entity.transform);
        }
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
