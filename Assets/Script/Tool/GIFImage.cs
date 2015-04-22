using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MiniWeChat
{
    [RequireComponent(typeof(RawImage))]
    public class GIFImage : MonoBehaviour
    {
        public string loadingGifPath;

        private int interval = 2;
        private static Dictionary<string, List<Texture2D>> _gifFrameDict = new Dictionary<string, List<Texture2D>>();
        private static HashSet<string> _finishGIFs = new HashSet<string>();
        private RawImage _showingImage;

        void Start()
        {
            StartCoroutine(LoadGifFrames());
            _showingImage = GetComponent<RawImage>();
        }

        public void OnDestroy()
        {
            if (!_finishGIFs.Contains(loadingGifPath))
            {
                _gifFrameDict.Remove(loadingGifPath);
            }
        }

        private IEnumerator LoadGifFrames()
        {
            if (_gifFrameDict.ContainsKey(loadingGifPath))
            {
                yield break;
            }

            List<Texture2D> gifFrames = new List<Texture2D>();
            _gifFrameDict[loadingGifPath] = gifFrames;

            //_showingImage = GetComponent<UnityEngine.UI.Image>();

            System.Drawing.Image gifImage = null;

#if UNITY_ANDROID

            WWW www = new WWW(Application.streamingAssetsPath + "/Image/" + loadingGifPath);

            yield return www;

            using(MemoryStream ms = new MemoryStream(www.bytes))
            {
                gifImage = System.Drawing.Image.FromStream(ms);
            }

#elif UNITY_EDITOR
            gifImage = System.Drawing.Image.FromFile(Application.streamingAssetsPath + "/Image/" + loadingGifPath);
#endif

            var dimension = new FrameDimension(gifImage.FrameDimensionsList[0]);
            int frameCount = gifImage.GetFrameCount(dimension);
            for (int i = 0; i < frameCount; i++)
            {
                gifImage.SelectActiveFrame(dimension, i);
                var frameTexture = new Texture2D(gifImage.Width, gifImage.Height);

                var frame = new Bitmap(gifImage.Width, gifImage.Height);
                System.Drawing.Graphics.FromImage(frame).DrawImage(gifImage, Point.Empty);
                ImageConverter converter = new ImageConverter();
                frameTexture.LoadImage((byte[])converter.ConvertTo(frame, typeof(byte[])));

                //Sprite sprite = Sprite.Create(frameTexture, new Rect(0, 0, frameTexture.width, frameTexture.height), new Vector3(0.5f, 0.5f), 1f);

                gifFrames.Add(frameTexture);
                //yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.06f)) ;
                yield return null;
            }

            _finishGIFs.Add(loadingGifPath);
        }

        void Update()
        {
            if (!_gifFrameDict.ContainsKey(loadingGifPath))
            {
                LoadGifFrames();
            }

            List<Texture2D> gifFrames = _gifFrameDict[loadingGifPath];

            if (Time.frameCount % interval == 0 && gifFrames.Count > 0)
            {
                if (IsFinishedLoading())
                {
                    _showingImage.texture = gifFrames[(int)(Time.frameCount / interval) % gifFrames.Count];
                }
                else
                {
                    _showingImage.texture = gifFrames[0];
                }
            }
        }

        private bool IsFinishedLoading()
        {
            return _finishGIFs.Contains(loadingGifPath);
        }
    }
}

