using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    public RepairButtonHandler repairButtonHandler;
    public ObjectType _Object;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        if(_Object == ObjectType.Remove)
        {
            if(repairButtonHandler.IsMain)
            {
                Debug.Log("Main Wall Cannot Be Removed");
                return;
            }
            repairButtonHandler.StatsData.Die();
            Debug.Log("Remove");
            return;    
        }

        if(_Object == ObjectType.Repair)
        {
            Debug.Log("Repair");
            repairButtonHandler.OnClickRepairBtn();
            return;
        }

    }


}
