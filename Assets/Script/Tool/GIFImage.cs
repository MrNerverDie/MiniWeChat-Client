using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MiniWeChat
{
    //[RequireComponent(typeof(UnityEngine.UI.Image))]
    public class GIFImage : MonoBehaviour
    {
        public string loadingGifPath;
        public float speed = 1;

        private UnityEngine.UI.Image _showingImage;
        private static Dictionary<string, List<Sprite>> _gifFrameDict = new Dictionary<string, List<Sprite>>();
        private static HashSet<string> _finishGIFs = new HashSet<string>();

        void Awake()
        {
            StartCoroutine(LoadGifFrames());
        }

        private IEnumerator LoadGifFrames()
        {
            if (_gifFrameDict.ContainsKey(loadingGifPath))
            {
                yield break;
            }

            List<Sprite> gifFrames = new List<Sprite>();
            _gifFrameDict[loadingGifPath] = gifFrames;

            //_showingImage = GetComponent<UnityEngine.UI.Image>();
            var gifImage = System.Drawing.Image.FromFile(Application.dataPath + "/Raw/Image/" + loadingGifPath);
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

                Sprite sprite = Sprite.Create(frameTexture, new Rect(0, 0, frameTexture.width, frameTexture.height), new Vector3(0.5f, 0.5f), 1f);

                gifFrames.Add(sprite);
                //yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.06f)) ;
                yield return null;
            }

            _finishGIFs.Add(loadingGifPath);
        }

        void OnGUI()
        {
            List<Sprite> gifFrames = _gifFrameDict[loadingGifPath];

            if (IsFinishedLoading())
            {
                //GUI.DrawTexture(new Rect(transform.position.x, Screen.height - transform.position.y, gifFrames[0].width, gifFrames[0].height), gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count]);
                //_showingImage.overrideSprite = gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count];
                GetComponent<SpriteRenderer>().sprite = gifFrames[(int)(Time.frameCount * speed) % gifFrames.Count];
            }
            else
            {
                //GUI.DrawTexture(new Rect(transform.position.x, Screen.height - transform.position.y, gifFrames[0].width, gifFrames[0].height), gifFrames[0]);
                //_showingImage.overrideSprite = gifFrames[0];
                GetComponent<SpriteRenderer>().sprite = gifFrames[0];
            }
        }

        private bool IsFinishedLoading()
        {
            return _finishGIFs.Contains(loadingGifPath);
        }
    }
}

