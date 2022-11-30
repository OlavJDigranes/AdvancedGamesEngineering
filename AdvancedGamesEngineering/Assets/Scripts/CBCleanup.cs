using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBCleanup : MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
   {
      Destroy(gameObject);
   }
}
