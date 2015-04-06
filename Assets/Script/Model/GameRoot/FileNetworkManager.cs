using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace MiniWeChat
{
    public class FileNetworkManager : Singleton<FileNetworkManager>
    {

        private Dictionary<string, WWW> _uploadFileDict;
        private Dictionary<string, WWW> _downloadFileDict;

        public override void Init()
        {
            base.Init();

            _uploadFileDict = new Dictionary<string, WWW>();
            _downloadFileDict = new Dictionary<string, WWW>();
        }

        public override void Release()
        {
            base.Release();
        }


        #region SendFile

        public void UploadFile(string urlPath, string msgID, Image image)
        {
            byte[] bytes = ((Texture2D)image.mainTexture).EncodeToPNG();
            WWWForm form = new WWWForm();
            form.AddBinaryData("fileUpload", bytes, "fileUpload" , "image/png");
            WWW uploadReq = new WWW(urlPath, form);

            if (!_uploadFileDict.ContainsKey(msgID))
            {
                _uploadFileDict.Add(msgID, uploadReq);
            }

            Debug.Log(uploadReq.text);

            StartCoroutine(DoUploadFile(msgID, uploadReq));
        }

        public IEnumerator DoUploadFile(string msgID, WWW uploadReq)
        {
            yield return uploadReq;

            _uploadFileDict.Remove(msgID);
            if (uploadReq.error != null)
            {
                Debug.Log("Error uploading file : " + msgID);
            }
        }

        #endregion
    }
}

