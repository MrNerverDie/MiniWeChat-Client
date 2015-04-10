using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace MiniWeChat
{
    public class FileTransferParam
    {
        public string msgID;
        public WWW www;
    }

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


        #region UploadFile
        public void UploadFile(string urlPath, string msgID, Sprite image)
        {
            byte[] bytes = ((Texture2D)image.texture).EncodeToPNG();
            UploadFile(urlPath, msgID, bytes, "image/png");
        }

        public void UploadFile(string urlPath, string msgID, Image image)
        {
            byte[] bytes = ((Texture2D)image.mainTexture).EncodeToPNG();
            UploadFile(urlPath, msgID, bytes, "image/png");
        }

        private void UploadFile(string urlPath, string msgID, byte[] fileData, string mineType)
        {
            if (_uploadFileDict.ContainsKey(msgID))
            {
                throw new System.Exception(" uploading ");
            }

            WWWForm form = new WWWForm();
            form.AddBinaryData("fileUpload", fileData, "fileUpload", mineType);
            WWW uploadReq = new WWW(urlPath, form);
            _uploadFileDict.Add(msgID, uploadReq);

            Debug.Log(System.Convert.ToBase64String(form.data));

            StartCoroutine(DoUploadFile(msgID, uploadReq));
        }

        private IEnumerator DoUploadFile(string msgID, WWW uploadReq)
        {
            yield return uploadReq;

            _uploadFileDict.Remove(msgID);
            if (uploadReq.error != null)
            {
                Debug.Log("Error uploading file : " + msgID);
            }
            else
            {
                MessageDispatcher.GetInstance().DispatchMessage((uint)EModelMessage.UPLOAD_FINISH,
                    new FileTransferParam
                    {
                        msgID = msgID,
                        www = uploadReq,
                    });
            }
        }

        #endregion

        #region DownloadFile


        private void DownloadFile(string urlPath, string msgID, byte[] fileData, string mineType)
        {
            if (_downloadFileDict.ContainsKey(msgID))
            {
                throw new System.Exception(" downloading ");
            }

            WWW downloadReq = new WWW(urlPath);
            if (!_uploadFileDict.ContainsKey(msgID))
            {
                _uploadFileDict.Add(msgID, downloadReq);
            }
            StartCoroutine(DoDownloadFile(msgID, downloadReq));
        }

        private IEnumerator DoDownloadFile(string msgID, WWW downloadReq)
        {
            yield return downloadReq;

            _uploadFileDict.Remove(msgID);
            if (downloadReq.error != null)
            {
                Debug.Log("Error downloading file : " + msgID);
            }
            else
            {
                MessageDispatcher.GetInstance().DispatchMessage((uint)EModelMessage.DOWNLOAD_FINISH,
                    new FileTransferParam 
                    {
                        msgID = msgID,
                        www = downloadReq,
                    });
            }
        }

        #endregion

        #region QueryFile

        public float GetTransferProgress(string msgID)
        {
            if (_uploadFileDict.ContainsKey(msgID))
            {
                return _uploadFileDict[msgID].uploadProgress;
            }
            else if (_downloadFileDict.ContainsKey(msgID))
            {
                return _downloadFileDict[msgID].progress;
            }else
            {
                throw new System.Exception("No Such File : " + msgID);
            }
        }

        #endregion
    }
}

