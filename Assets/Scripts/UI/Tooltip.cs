using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Tooltip : MonoBehaviour
{
    private float textPaddingSize = 8f;

    private Text tooltipText;
    private RectTransform tooltipRectTransform;

    private static Tooltip instance;
    private static Direction _direction = Direction.RIGHT;

    public enum Direction
    {
        RIGHT,
        LEFT
    }

    void Awake()
    {
        instance = this;
        tooltipRectTransform = GetComponent<RectTransform>();
        tooltipText = transform.Find("Text").GetComponent<Text>();

        gameObject.SetActive(false);
    }

    private Vector2 localPoint;
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        
        if(_direction == Direction.LEFT)
            transform.localPosition = new Vector2(localPoint.x + tooltipRectTransform.sizeDelta.x/2f, localPoint.y + tooltipRectTransform.sizeDelta.y/2f);
        else    
            transform.localPosition = new Vector2(localPoint.x - tooltipRectTransform.sizeDelta.x/2f, localPoint.y - tooltipRectTransform.sizeDelta.y/2f);
    }

    public static void AddTooltipEvent(string objectName, string hint, Direction direction = Direction.LEFT)
    {
        Entry exitEvent = new Entry();
        exitEvent.eventID = EventTriggerType.PointerExit;
        exitEvent.callback.AddListener((eventData) => instance.HideTooltip());

        GameObject obj = GameObject.Find(objectName);
        obj.GetComponent<EventTrigger>().triggers.Add(ShowTooltipEvent(hint, direction));
        obj.GetComponent<EventTrigger>().triggers.Add(exitEvent);
    }

    private void ShowTooltip(string tooltipString)
    {
        tooltipText.text = tooltipString; 
        
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        tooltipRectTransform.sizeDelta = backgroundSize;
        backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 2f, tooltipText.preferredHeight + textPaddingSize * 2f);
        tooltipRectTransform.sizeDelta = backgroundSize;

        gameObject.SetActive(true);
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private static Entry ShowTooltipEvent(string hint, Direction direction = Direction.LEFT)
    {
        Entry eventType = new Entry();
        eventType.eventID = EventTriggerType.PointerEnter;
        eventType.callback.AddListener((eventData) => Show(hint, direction));

        return eventType;
    }

    private static void Show(string tooltipString, Direction direction = Direction.RIGHT)
    {
        _direction = direction;
        instance.ShowTooltip(tooltipString);
    }

}
