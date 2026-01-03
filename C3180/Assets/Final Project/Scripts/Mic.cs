using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Whisper.Utils;
using Whisper;
using UnityEngine.Video;

namespace MyProject.Audio
{
    public class Mic : MonoBehaviour
    {
        [SerializeField] private WhisperManager whisper;
        [SerializeField] private TMP_Text outputText;
        [SerializeField] private TMP_Text debugText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private MicrophoneRecord microphoneRecord;
        private string result;
        private GameObject target;
        private bool isTranslating;
        private float recordStartTime;
        [SerializeField] private Camera vrCamera;
        [SerializeField] private LayerMask layerMask;
        private bool seeEnemy;
        private Renderer lastEnemyRenderer;
        private bool stopDueToMaxLength;
        [SerializeField] private GameObject snapStart;
        [SerializeField] private GameObject snapEnd;
        [SerializeField] private GameObject fingerStart;
        [SerializeField] private GameObject fingerEnd;
        [SerializeField] private GameObject frostStart;
        [SerializeField] private GameObject frostEnd;
        private bool fingerSpellResult;
        private bool snapSpellResult;
        [SerializeField] private GameObject bullet;
        [SerializeField] private GameObject fingerTip;
        [SerializeField] private ParticleSystem frost;
        private bool frostSpellResult;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private VideoClip clickClip;
        [SerializeField] private VideoClip fingerGunClip;
        [SerializeField] private VideoClip frostClip;

        void Awake()
        {
            microphoneRecord.OnRecordStop += OnRecordStop;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            seeEnemy = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (microphoneRecord.IsRecording)
            {
                float elapsed = Time.time - recordStartTime;
                if (elapsed >= microphoneRecord.maxLengthSec)
                {
                    Debug.Log("Max recording length reached — stopping.");
                    debugText.text = "Recording reached max length! Stop recording manually.";
                    stopDueToMaxLength = true;
                    microphoneRecord.StopRecord();
                }
            }

            Ray ray = new Ray(vrCamera.transform.position, vrCamera.transform.forward);
            RaycastHit hit;

            Debug.DrawRay(vrCamera.transform.position, vrCamera.transform.forward * Mathf.Infinity, Color.blue);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    seeEnemy = true;
                    Debug.DrawRay(vrCamera.transform.position, vrCamera.transform.forward * Mathf.Infinity, Color.red);

                    Renderer rend = hit.collider.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        if (lastEnemyRenderer != null && lastEnemyRenderer != rend)
                        {
                            lastEnemyRenderer.material.color = Color.white;
                        }

                        rend.material.color = Color.red;
                        lastEnemyRenderer = rend;
                    }
                }
                else
                {
                    seeEnemy = false;
                    Debug.DrawRay(vrCamera.transform.position, vrCamera.transform.forward * Mathf.Infinity, Color.blue);

                    if (lastEnemyRenderer != null)
                    {
                        lastEnemyRenderer.material.color = Color.white;
                        lastEnemyRenderer = null;
                    }
                }
            }
            else
            {
                seeEnemy = false;
                Debug.DrawRay(vrCamera.transform.position, vrCamera.transform.forward * Mathf.Infinity, Color.blue);

                if (lastEnemyRenderer != null)
                {
                    lastEnemyRenderer.material.color = Color.white;
                    lastEnemyRenderer = null;
                }
            }
        }

        public void recordStart()
        {
            if (!seeEnemy)
            {
                Debug.Log("cannot see an enamy.");
                debugText.text = "cannot see an enemy.";
                return;
            }
            
            if (isTranslating)
            {
                Debug.Log("Still translating — please wait.");
                debugText.text = "Still translating — please wait.";
                return;
            }

            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                recordStartTime = Time.time;
                debugText.text = "Recording started.";
                Debug.Log("Recording started.");
            }
        }
        
        public void recordStop()
        {
            if (microphoneRecord.IsRecording)
            {
                stopDueToMaxLength = false;
                microphoneRecord.StopRecord();
                debugText.text = "Recording stopped.";
                Debug.Log("Recording stopped.");
            }
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            if (stopDueToMaxLength)
            {
                stopDueToMaxLength = false;
                debugText.text = "max record length reached, recording stopped.";
                Debug.Log("Recording stopped.");
                return;
            }

            isTranslating = true;

            outputText.text = "translating.....";
            float translationStart = Time.realtimeSinceStartup;

            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            float translationTime = Time.realtimeSinceStartup - translationStart;
            timeText.text = $"Translation time: {translationTime:F2} seconds";  
            if (res == null)
            {
                isTranslating = false;
                return;
            }

            var text = res.Result;
            text = text.ToLower();

            outputText.text = text;

            resultCast(text);
            Debug.Log(text);
            isTranslating = false;
        }

        public void resultCast(string s)
        {
            if (s == null)
            {
                return;
            }

            if (s.Contains(result) && snapSpellResult == true)
            {
                target.GetComponent<Spells>().spellComplete();
            }
            else if (s.Contains(result) && fingerSpellResult == true)
            {
                Instantiate(bullet, fingerTip.transform.position, fingerTip.transform.rotation);
            }
            else if(s.Contains(result) && frostSpellResult == true)
            {
                frost.Play();
            }
        }

        public void expectedString(string s)
        {
            result = s;
        }

        public void setGameObject(GameObject g)
        {
            target = g;
        }

        public void setFinger()
        {
            snapStart.SetActive(false);
            snapEnd.SetActive(false);
            fingerStart.SetActive(true);
            fingerEnd.SetActive(true);
            frostStart.SetActive(false);
            frostEnd.SetActive(false);
            fingerSpellResult = true;
            snapSpellResult = false;
            frostSpellResult = false;
            videoPlayer.clip = fingerGunClip;
            videoPlayer.Play();
        }

        public void setSnap()
        {
            snapStart.SetActive(true);
            snapEnd.SetActive(true);
            fingerEnd.SetActive(false);
            fingerStart.SetActive(false);
            frostStart.SetActive(false);
            frostEnd.SetActive(false);
            snapSpellResult = true;
            fingerSpellResult = false;
            frostSpellResult = false;
            videoPlayer.clip = clickClip;
            videoPlayer.Play();
        }

        public void setFrost()
        {
            snapStart.SetActive(false);
            snapEnd.SetActive(false);
            fingerEnd.SetActive(false);
            fingerStart.SetActive(false);
            frostStart.SetActive(true);
            frostEnd.SetActive(true);
            snapSpellResult = false;
            fingerSpellResult = false;
            frostSpellResult = true;
            videoPlayer.clip = frostClip;
            videoPlayer.Play();
        }
    }
}
