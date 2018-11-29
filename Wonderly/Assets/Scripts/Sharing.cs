using System.Collections;

using System.Collections.Generic;

using UnityEngine;



public class Sharing : MonoBehaviour
{
    // Use this for initialization

    public void SharingCode()

    {

        new NativeShare().SetText("Congratulations! (Full Name) has just sent you the code for (###), an experience within the Wonderly application."+ "\n" +"If you do not have the Wonderly application, download it here (###)").Share();

    }

}