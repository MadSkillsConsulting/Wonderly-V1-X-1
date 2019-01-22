/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//  Module: LWALocalMediaObjects.cs
//  Written By: Mad Skills Consulting LLC
//  Date: 2018-12-18 - Dev Michael - First checkin
//  Date: 2019-01-21 - Dev Michael - Added Class Properties
//  Date: 2019-01-22 - Maia Monet - Added Co-Developer
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Sample;
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

public class LWALocalMediaObject : MonoBehaviour
{
    private static int maxSize = 512;       // Maximum image pixels on iOS
    private static string GetImageTitle = "Select an image";
    private static string GetImageMime = "image/*";
    private static string GetVideoTitle = "Select a video";
    private static string GetVideoMime = "video/*";
    private static FilesManager fm;

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /// Properties
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //  TargetObjectBaseName - Basename of file, has a settable default value
    //
    //  Usage:
    //
    //      basename = LWALocalMediaObject.TargetObjectBaseName;
    //
    //      LWALocalMediaObject.TargetObjectBaseName = "new-base-name";
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    private string targetObjectBaseNameDefault = "target-object";

    private string targetObjectBaseName = targetObjectBaseNameDefault;

    public string TargetObjectBaseName
    {
        get
        {
            return targetObjectBaseName;
        }
        set
        {
            targetObjectBaseName = value;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    private string targetObjectPath = null; // Full path to target object

    public string TargetObjectPath
    {
        get
        {
            return targetObjectPath;
        }
        set
        {
            targetObjectPath = value;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    private string targetObjectExtension = null; // Filename extension of target object

    public string TargetObjectExtension
    {
        get
        {
            return targetObjectExtension;
        }
        set
        {
            targetObjectExtension = value;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    // TakePicture()
    /////////////////////////////////////////////////////////////////////////////
    public void TakePicture()
    {
        if (IsCameraReady())
        {
            NativeCamera.Permission permission = NativeCamera.TakePicture((ImagePath) =>
            {
                MoveToTargetObjectPath(ImagePath);
            }, maxSize);

            Debug.Log("Permission result: (" + permission + ")");
        }
        else
        {
            Debug.Log("Camera not ready");
        }
    }
    /////////////////////////////////////////////////////////////////////////////
    // RecordVideo()
    /////////////////////////////////////////////////////////////////////////////
    public void RecordVideo()
    {
        if (IsCameraReady())
        {
            NativeCamera.Permission permission = NativeCamera.RecordVideo((VideoPath) =>
            {
                MoveToTargetObjectPath(VideoPath);
            });
            Debug.Log("Permission result: (" + permission + ")");
        }
        else
        {
            Debug.Log("Camera not ready");
        }
    }
    /////////////////////////////////////////////////////////////////////////////
    // GetImageFromGallery()
    /////////////////////////////////////////////////////////////////////////////
    public void GetImageFromGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((ImagePath) =>
        {
            MoveToTargetObjectPath(ImagePath);
        }, GetImageTitle, GetImageMime, maxSize);
        Debug.Log("Permission result: (" + permission + ")");
        Debug.Log("GetImageTitle: (" + GetImageTitle + ")");
        Debug.Log("GetImageMime: (" + GetImageMime + ")");
        Debug.Log("maxSize: (" + maxSize + ")");
    }
    /////////////////////////////////////////////////////////////////////////////
    // GetVideoFromGallery()
    /////////////////////////////////////////////////////////////////////////////
    public void GetVideoFromGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((VideoPath) =>
        {
            MoveToTargetObjectPath(VideoPath);
        }, GetVideoTitle, GetVideoMime);
        Debug.Log("Permission result: (" + permission + ")");
        Debug.Log("GetVideoTitle: (" + GetVideoTitle + ")");
        Debug.Log("GetVideoMime: (" + GetVideoMime + ")");
    }
    /////////////////////////////////////////////////////////////////////////////
    // Utility methods
    /////////////////////////////////////////////////////////////////////////////
    private static bool IsCameraReady()
    {
        if (NativeCamera.DeviceHasCamera()) // Does it even have a camera?
        {
            Debug.Log("NativeCamera: Continue (camera was found)");
        }
        else
        {
            Debug.Log("NativeCamera: Aborted (no camera found)");
            return false;
        }
        if (NativeCamera.IsCameraBusy())  // Is it busy doing something else?
        {
            Debug.Log("NativeCamera: Aborted (the camera is busy)");
            return false;
        }
        else
        {
            Debug.Log("NativeCamera: Continue (the camera is not busy)");
        }
        // No issues, we should be able to use the camera
        return true;
    }
    /////////////////////////////////////////////////////////////////////////////
    private static void MoveToTargetObjectPath(string sourceObjectPath)
    // Once we have an object, move it to a known location
    {
        if (sourceObjectPath == null)
        {
            Debug.Log("sourceObjectPath is null");
        }
        else
        {
            Debug.Log("sourceObjectPath: (" + sourceObjectPath + ")");
            string mediaExtension = System.IO.Path.GetExtension(sourceObjectPath);
            string targetObjectPath = fm.MarksDirectory
                                    + targetObjectBaseName
                                    + mediaExtension;
            Debug.Log("targetObjectPath: (" + targetObjectPath + ")");
            FileUtil.ReplaceFile(sourceObjectPath, targetObjectPath);
        }
    }
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
}
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//  End: LWALocalMediaObjects.cs
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////