using System;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
   public BattleUnitEnum BattleUnitEnum;
   internal string buttonname;

   private void Start()
   {
      //assigning enum to button name
      buttonname = BattleUnitEnum.ToString();
      
   }
   
}
