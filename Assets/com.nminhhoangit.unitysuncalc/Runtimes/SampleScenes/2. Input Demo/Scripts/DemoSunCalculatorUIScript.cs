using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace nminhhoangit.SunCalculator
{
    public class DemoSunCalculatorUIScript : MonoBehaviour
    {
        // General UIs
        public Button BtnNow;
        public Image ImgNowIcon;
        public Button BtnForward;
        public Image ImgForwardIcon;
        public Button BtnBackward;
        public Image ImgBackwardIcon;
        public Button BtnSetting;
        public Text TxtDateTime;

        // Popup Input Data UIs
        public GameObject GoPopupInputData;
        public InputField IpfLatitude;
        public InputField IpfLongitude;
        public InputField IpfDate;
        public InputField IpfTime;
        public Text TxtError;
        public Button BtnBack;
        public Button BtnUpdate;

        // Params
        public Action<float, float, DateTime> OnInputDatasChangedEvent;

        public void InitView()
        {
            // Add listeners
            BtnForward?.onClick.AddListener(ForwardOnClick);
            BtnBackward?.onClick.AddListener(BackwardOnClick);
            BtnNow?.onClick.AddListener(NowOnClick);
            BtnSetting?.onClick.AddListener(SettingOnClick);
            //
            BtnBack?.onClick.AddListener(BackOnClick);
            BtnUpdate?.onClick.AddListener(UpdateOnClick);
            IpfLatitude?.onEndEdit.AddListener(content => SetBtnUpdateInteract());
            IpfLongitude?.onEndEdit.AddListener(content => SetBtnUpdateInteract());
            IpfDate?.onEndEdit.AddListener(OnDateEndEditHandler);
            IpfTime?.onEndEdit.AddListener(OnTimeEndEditHandler);
        }

        private void OnDateEndEditHandler(string content)
        {
            if (DateTime.TryParse(content, out DateTime date))
            {
                IpfDate.SetTextWithoutNotify(date.ToString("yyyy-MM-dd"));
            }

            SetBtnUpdateInteract();
        }

        private void OnTimeEndEditHandler(string content)
        {
            if (DateTime.TryParse(content, out DateTime date))
            {
                IpfTime.SetTextWithoutNotify(date.ToString("hh:mm:ss"));
            }
            
            SetBtnUpdateInteract();
        }

        private void NowOnClick()
        {
            if (DemoSunCalculatorScript.Api == null)
                return;

            if (DemoSunCalculatorScript.Api.IsUpdateRealtime)
            {
                // Turn off if already run
                DemoSunCalculatorScript.Api.IsUpdateRealtime = false;
            }
            else
            {
                // Active auto backward update
                DemoSunCalculatorScript.Api.IsUpdateRealtime = true;
                //
                DemoSunCalculatorScript.Api.IsDecreaseTime = false;
                DemoSunCalculatorScript.Api.IsIncreaseTime = false;

                ResetColorActionIcons();
            }

            // Update icon status
            if(ImgNowIcon) ImgNowIcon.color = DemoSunCalculatorScript.Api.IsUpdateRealtime ? Color.green : Color.white;
        }

        private void BackwardOnClick()
        {
            if (DemoSunCalculatorScript.Api == null)
                return;

            if (DemoSunCalculatorScript.Api.IsDecreaseTime)
            {
                // Turn off if already run
                DemoSunCalculatorScript.Api.IsDecreaseTime = false;
            }
            else
            {
                // Active auto backward update
                DemoSunCalculatorScript.Api.IsDecreaseTime = true;
                //
                DemoSunCalculatorScript.Api.IsUpdateRealtime = false;
                DemoSunCalculatorScript.Api.IsIncreaseTime = false;

                ResetColorActionIcons();
            }

            // Update icon status
            if (ImgBackwardIcon) ImgBackwardIcon.color = DemoSunCalculatorScript.Api.IsDecreaseTime ? Color.green : Color.white;
        }

        private void ForwardOnClick()
        {
            if (DemoSunCalculatorScript.Api == null)
                return;

            if (DemoSunCalculatorScript.Api.IsIncreaseTime)
            {
                // Turn off if already run
                DemoSunCalculatorScript.Api.IsIncreaseTime = false;
            }
            else
            {
                // Active auto forward update
                DemoSunCalculatorScript.Api.IsIncreaseTime = true;
                //
                DemoSunCalculatorScript.Api.IsUpdateRealtime = false;
                DemoSunCalculatorScript.Api.IsDecreaseTime = false;

                ResetColorActionIcons();
            }

            // Update icon status
            if (ImgForwardIcon) ImgForwardIcon.color = DemoSunCalculatorScript.Api.IsIncreaseTime ? Color.green : Color.white;
        }

        private void SettingOnClick()
        {
            DemoSunCalculatorScript.Api?.GetInputDatas((latitude, longtitude, dateTime) => {
                ShowPopup(latitude, longtitude, dateTime);
            });
        }

        private void BackOnClick()
        {
            ClosePopup();
        }

        private void ResetColorActionIcons()
        {
            if (ImgNowIcon) ImgNowIcon.color = Color.white;
            if (ImgBackwardIcon) ImgBackwardIcon.color = Color.white;
            if (ImgForwardIcon) ImgForwardIcon.color = Color.white;
        }

        private void SetBtnUpdateInteract()
        {
            BtnUpdate.interactable = IsAllInputDatasValid();
        }

        private bool IsAllInputDatasValid()
        {
            if (!float.TryParse(IpfLatitude.text, out float latitude))
            {
                SetTextInputDataError("Invalid latitude!");
                return false;
            }

            if (!float.TryParse(IpfLongitude.text, out float longtitude))
            {
                SetTextInputDataError("Invalid longtitude!");
                return false;
            }

            if (!DateTime.TryParse(IpfDate.text, out DateTime dateInput))
            {
                SetTextInputDataError("Invalid date! Sample correct format: yyyy-MM-dd");
                return false;
            }

            if (!DateTime.TryParse(IpfTime.text, out DateTime timeInput))
            {
                SetTextInputDataError("Invalid time! Sample correct format: hh:mm:ss");
                return false;
            }

            SetTextInputDataError(null);
            return true;
        }

        private void SetTextInputDataError(string content)
        {
            if(TxtError)
            {
                TxtError.gameObject.SetActive(!string.IsNullOrEmpty(content));
                TxtError.text = content;
            }
        }

        private void UpdateOnClick()
        {
            try
            {
                float latitude = 0f;
                float longtitude = 0f;
                DateTime datetimeInput = DateTime.Now;

                if (!float.TryParse(IpfLatitude.text, out latitude))
                {
                    IsAllInputDatasValid();
                    return;
                }

                if (!float.TryParse(IpfLongitude.text, out longtitude))
                {
                    IsAllInputDatasValid();
                    return;
                }

                if (DateTime.TryParse(IpfDate.text, out DateTime dateInput))
                {
                    if (DateTime.TryParse(IpfTime.text, out DateTime timeInput))
                    {
                        datetimeInput = dateInput.Date.Add(timeInput.TimeOfDay);
                    }
                    else
                    {
                        IsAllInputDatasValid();
                        return;
                    }
                }
                else
                {
                    IsAllInputDatasValid();
                    return;
                }

                // All valid and callback data
                ClosePopup(() =>
                {
                    Debug.Log($"UpdateOnClick success: {latitude}, {longtitude}, {datetimeInput}");
                    OnInputDatasChangedEvent?.Invoke(latitude, longtitude, datetimeInput);
                });
            }
            catch (Exception ex)
            {
                Debug.Log($"UpdateOnClick failed: {ex.ToString()}");
            }
        }

        private void ShowPopup(float latitude, float longtitude, DateTime curDateTime)
        {
            // Update views
            BtnSetting.gameObject.SetActive(false);
            GoPopupInputData.SetActive(true);
            //
            IpfLatitude.text = latitude.ToString();
            IpfLongitude.text = longtitude.ToString();
            IpfDate.text = curDateTime.ToString("yyyy-MM-dd");
            IpfTime.text = curDateTime.ToString("hh:mm:ss");
            
            // Valiataion preset data input
            IsAllInputDatasValid();

            // Active popup
            GoPopupInputData.SetActive(true);
        }

        private void ClosePopup(Action callback = null)
        {
            // Hide popup
            BtnSetting.gameObject.SetActive(true);
            GoPopupInputData.SetActive(false);
            //
            callback?.Invoke();
        }

        /// <summary>
        /// Set text sun calculator on UI
        /// </summary>
        public void SetTextSunCalcUI(float latitude, float longtitude, DateTime calculatorDateTime)
        {
            TxtDateTime.text = $"<size=24>lat: {latitude.ToString("F4")}, long: {longtitude.ToString("F4")}</size>\n<size=64>{calculatorDateTime.ToString("dd MMM yyyy")}</size>\n{calculatorDateTime.ToString("hh:mm:ss \"GMT\"zzz")}";
        }
    }
}