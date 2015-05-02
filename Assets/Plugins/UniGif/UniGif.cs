/*
UniGif
Copyright (c) 2015 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static partial class UniGif
{

    private static Dictionary<int, UniGif.GifAnimation> gifDict = new Dictionary<int, UniGif.GifAnimation>();

    /// <summary>
    /// Get GIF texture list (This is a possibility of lock up)
    /// </summary>
    /// <param name="bytes">GIF file byte data</param>
    /// <param name="loopCount">out Animation loop count</param>
    /// <param name="width">out GIF image width (px)</param>
    /// <param name="height">out GIF image height (px)</param>
    /// <param name="filterMode">Textures filter mode</param>
    /// <param name="wrapMode">Textures wrap mode</param>
    /// <param name="debugLog">Debug Log Flag</param>
    /// <returns>GIF texture list</returns>
    public static List<GifTexture> GetTextureList (byte[] bytes, out int loopCount, out int width, out int height,
        FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool debugLog = false)
    {
        loopCount = -1;
        width = 0;
        height = 0;

        // Set GIF data
        var gifData = new GifData ();
        if (SetGifData (bytes, ref gifData, debugLog) == false) {
            Debug.LogError ("GIF file data set error.");
            return null;
        }

        // Decode to textures from GIF data
        var gifTexList = new List<GifTexture> ();
        if (DecodeTexture (gifData, gifTexList, filterMode, wrapMode) == false) {
            Debug.LogError ("GIF texture decode error.");
            return null;
        }

        loopCount = gifData.appEx.loopCount;
        width = gifData.logicalScreenWidth;
        height = gifData.logicalScreenHeight;
        return gifTexList;
    }

    /// <summary>
    /// Get GIF texture list Coroutine (Avoid lock up but more slow)
    /// </summary>
    /// <param name="mb">MonoBehaviour to start the coroutine</param>
    /// <param name="bytes">GIF file byte data</param>
    /// <param name="cb">Callback method(param is GIF texture list, Animation loop count, GIF image width (px), GIF image height (px))</param>
    /// <param name="filterMode">Textures filter mode</param>
    /// <param name="wrapMode">Textures wrap mode</param>
    /// <param name="debugLog">Debug Log Flag</param>
    /// <returns>IEnumerator</returns>
    public static IEnumerator GetTextureListCoroutine (MonoBehaviour mb, byte[] bytes, int instanceId, List<GifTexture> gifTexList,
        Action<List<GifTexture>, int, int, int> cb, FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool debugLog = false)
    {
        int loopCount = -1;
        int width = 0;
        int height = 0;
        if (gifTexList == null)
        {
            gifTexList = new List<GifTexture>();
        }
        else
        {
            gifTexList = gifDict[instanceId].textureList;
        }

        if (!gifDict.ContainsKey(instanceId))
        {
            // Set GIF data
            var gifData = new GifData();
            if (SetGifData(bytes, ref gifData, debugLog) == false)
            {
                Debug.LogError("GIF file data set error.");
                if (cb != null)
                {
                    cb(null, loopCount, width, height);
                }
                yield break;
            }

            loopCount = gifData.appEx.loopCount;
            width = gifData.logicalScreenWidth;
            height = gifData.logicalScreenHeight;

            Debug.Log("loopCount " + loopCount);

            gifDict.Add(instanceId, new GifAnimation
            {
                loopCount = loopCount,
                width = width,
                height = height,
                textureList = gifTexList,
            });

            // avoid lock up
            yield return 0;

            // Decode to textures from GIF data
            yield return mb.StartCoroutine(UniGif.DecodeTextureCoroutine(gifData, gifTexList, filterMode, wrapMode));
        }

        if (cb != null)
        {
            //Debug.Log(gifDict[instanceId].textureList.Count + " " + gifDict[instanceId].loopCount);
            //if (gifDict[instanceId].textureList.Count == gifDict[instanceId].loopCount)
            //{
                loopCount = gifDict[instanceId].loopCount;
                width = gifDict[instanceId].width;
                height = gifDict[instanceId].height;
                gifTexList = gifDict[instanceId].textureList;

                cb(gifTexList, loopCount, width, height);
            //    yield break;
            //}
            //yield return new WaitForSeconds(0.1f);
        }

    }

    /// <summary>
    /// Get GIF texture list Coroutine (Avoid lock up but more slow)
    /// </summary>
    /// <param name="mb">MonoBehaviour to start the coroutine</param>
    /// <param name="bytes">GIF file byte data</param>
    /// <param name="cb">Callback method(param is GIF texture list, Animation loop count, GIF image width (px), GIF image height (px))</param>
    /// <param name="filterMode">Textures filter mode</param>
    /// <param name="wrapMode">Textures wrap mode</param>
    /// <param name="debugLog">Debug Log Flag</param>
    /// <returns>IEnumerator</returns>
    public static IEnumerator GetTextureListCoroutine(MonoBehaviour mb, byte[] bytes, Action<List<GifTexture>, int, int, int> cb,
        FilterMode filterMode = FilterMode.Bilinear, TextureWrapMode wrapMode = TextureWrapMode.Clamp, bool debugLog = false)
    {
        int loopCount = -1;
        int width = 0;
        int height = 0;

        // Set GIF data
        var gifData = new GifData();
        if (SetGifData(bytes, ref gifData, debugLog) == false)
        {
            Debug.LogError("GIF file data set error.");
            if (cb != null)
            {
                cb(null, loopCount, width, height);
            }
            yield break;
        }

        // avoid lock up
        yield return 0;

        // Decode to textures from GIF data
        List<GifTexture> gifTexList = null;
        yield return mb.StartCoroutine(UniGif.DecodeTextureCoroutine(gifData, gifTexList, filterMode, wrapMode));

        if (gifTexList == null)
        {
            Debug.LogError("GIF texture decode error.");
            if (cb != null)
            {
                cb(null, loopCount, width, height);
            }
            yield break;
        }

        loopCount = gifData.appEx.loopCount;
        width = gifData.logicalScreenWidth;
        height = gifData.logicalScreenHeight;

        if (cb != null)
        {
            cb(gifTexList, loopCount, width, height);
        }
    }
}