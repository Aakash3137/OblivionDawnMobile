using System;
using UnityEngine;

public class UnitButton : MonoBehaviour
{
   public BattleUnitEnum BattleUnitEnum;
   internal string buttonname;

   private void Start()
   {
      buttonname = BattleUnitEnum.ToString();
   }
   
}
