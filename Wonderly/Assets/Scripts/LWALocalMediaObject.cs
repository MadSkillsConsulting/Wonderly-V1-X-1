/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//  Start: LWALocalMediaObject.cs
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//  Class: LWALocalMediaObject
/////////////////////////////////////////////////////////////////////////////
//  Written By: Mad Skills Consulting LLC
/////////////////////////////////////////////////////////////////////////////
//  Date: 2018-12-18    - Dev Michael   - First checkin
//  Date: 2019-01-21    - Dev Michael   - Added class properties
//  Date: 2019-01-22    - Maia Monet    - Added Co-Developer
//  Date: 2019-01-22    - Dev Michael   - Added usage comments
//  Date: 2019-01-24    - Dev Michael   - Made internal properties static
//  Date: 2019-01-24    - Dev Michael   - Removed default directory var  
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//
//  Usage
//
//      Primary Functionality Methods:
//
//          LWALocalMediaObject.TakePicture();
//
//          LWALocalMediaObject.RecordVideo();
//
//          LWALocalMediaObject.GetImageFromGallery();
//
//          LWALocalMediaObject.GetVideoFromGallery();
//
//      Proprties:
//
//          Path - Full path to media file after success (READ ONLY)
//              Usage:
//                  mediaPath = LWALocalMediaObject.Path;
//
//          BaseName - Base name of media file, (+extension = filename)
//              Usage:
//                  baseName = LWALocalMediaObject.BaseName;
//                  LWALocalMediaObject.BaseName = "new-base-name";
//
//          Directory - Path to directory, in which to place media files
//              Usage:
//                  directory = LWALocalMediaObject.Directory;
//                  LWALocalMediaObject.Directory = "/new/path/to/directory";
//
//          Extension - Media file name extension, indicates type (READ ONLY)
//              Usage:
//                  extension = LWALocalMediaObject.Extension;
//
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
//  Uses the following classes:
//
//      Unity Plugins:
//          NativeCamera
//          NativeGallery
//
//      Other Classes:
//
//          FilesManager
//          FileUtil
//          Debug
//          System.IO.Path
//
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

public class LWALocalMediaObject : MonoBehaviour
{

    private static FilesManager fm; // directory default
    private const string BaseNameDefault = "target-object";

    private const int MaxSize = 512;       // Maximum image pixels on iOS
    private const string GetImageTitle = "Select an image";
    private const string GetImageMime = "image/*";
    private const string GetVideoTitle = "Select a video";
    private const string GetVideoMime = "video/*";

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /// Properties
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //  BaseName - Basename of file, has a settable default value
    //
    //  Usage:
    //
    //      basename = LWALocalMediaObject.BaseName;
    //
    //      LWALocalMediaObject.BaseName = "new-base-name";
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    private static string baseName = BaseNameDefault;

    public static string BaseName
    {
        get
        {
            return baseName;
        }
        set
        {
            baseName = value;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //  Directory - Path to directory destination for media files
    //
    //  Usage:
    //
    //      directoryPath = LWALocalMediaObject.Directory;
    //
    //      LWALocalMediaObject.Directory = "/new/path/to/directory";
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    private static string directory = null;

    public static string Directory
    {
        get
        {
            if (null == directory)
            {
                directory = fm.MarksDirectory;
            }
            return directory;
        }
        set
        {
            directory = value;
        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //  Path - Full path to media file after success (READ ONLY)
    //
    //  Usage:
    //
    //      mediaPath = LWALocalMediaObject.Path;
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    private static string path = null; // Full path to target object

    public static string Path
    {
        get
        {
            return path;
        }
//  read only property, no setter
//        set
//        {
//            path = value;
//        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //  Extension - Media file name extension, indicates type (READ ONLY)
    //
    //  Usage:
    //
    //      extension = LWALocalMediaObject.Extension;
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    private static string extension = null; // Filename extension of target object

    public static string Extension
    {
        get
        {
            return extension;
        }
//  read only property, no setter
//        set
//        {
//            extension = value;
//        }
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    //
    //  Functional Methods
    //
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////
    // TakePicture()
    /////////////////////////////////////////////////////////////////////////////
    public static void TakePicture()
    {
        if (IsCameraReady())
        {
            NativeCamera.Permission permission = NativeCamera.TakePicture((imagePath) =>
            {
                MoveToTargetObjectPath(imagePath);
            }, MaxSize);

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
    public static void RecordVideo()
    {
        if (IsCameraReady())
        {
            NativeCamera.Permission permission = NativeCamera.RecordVideo((videoPath) =>
            {
                MoveToTargetObjectPath(videoPath);
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
    public static void GetImageFromGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((imagePath) =>
        {
            MoveToTargetObjectPath(imagePath);
        }, GetImageTitle, GetImageMime, MaxSize);
        Debug.Log("Permission result: (" + permission + ")");
        Debug.Log("GetImageTitle: (" + GetImageTitle + ")");
        Debug.Log("GetImageMime: (" + GetImageMime + ")");
        Debug.Log("maxSize: (" + MaxSize + ")");
    }

    /////////////////////////////////////////////////////////////////////////////
    // GetVideoFromGallery()
    /////////////////////////////////////////////////////////////////////////////
    public static void GetVideoFromGallery()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((videoPath) =>
        {
            MoveToTargetObjectPath(videoPath);
        }, GetVideoTitle, GetVideoMime);
        Debug.Log("Permission result: (" + permission + ")");
        Debug.Log("GetVideoTitle: (" + GetVideoTitle + ")");
        Debug.Log("GetVideoMime: (" + GetVideoMime + ")");
    }

    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
    // Utility methods
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////

    /////////////////////////////////////////////////////////////////////////////
    //  Verify that the camera exists and is ready to use:
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
    // Once we have an object, move it to a known location
    private static void MoveToTargetObjectPath(string sourceObjectPath)
    {
        if (sourceObjectPath == null)
        {
            Debug.Log("sourceObjectPath is null");
        }
        else
        {
            Debug.Log("sourceObjectPath: (" + sourceObjectPath + ")");
            extension = System.IO.Path.GetExtension(sourceObjectPath);
            Debug.Log("targetObjectExtension: (" + Extension + ")");
            path = Directory + BaseName + Extension;
            Debug.Log("targetObjectPath: (" + Path + ")");
            FileUtil.ReplaceFile(sourceObjectPath, Path);
        }
    }
    /////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////
}

/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////
//  End: LWALocalMediaObjects.cs
/////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////