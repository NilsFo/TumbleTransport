using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAttachment : MonoBehaviour
{

    public AttachmentType myType;
    
    public enum AttachmentType: UInt16
    {
        None,
        Eyelet,
        TruckBed,
        Cargo
    }
    
}
