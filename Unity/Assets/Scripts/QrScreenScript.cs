using SolAR;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QrScreenScript : MonoBehaviour
{
	public QRCodeManagerScript qrCodeManagerScript;

	public TextMeshProUGUI textQrHint;

	public SolARPipeline solarPipelineObject;
	public SocketIOSignaler webRtcSignaler;

	public GameObject findDeviceCanvas;

	private Coroutine wrongCodeCoroutine;

	private const string QR_SCAN_HINT = "Scan QR Code with server address in second smartphone";
	private const string WRONG_QR_HINT = "Wrong QR Code!";

	private const string QR_CODE_KEY = "SCREEN_CAPTURE_ADDRESS=";

	void OnEnable()
	{
		textQrHint.text = QR_SCAN_HINT;
		qrCodeManagerScript.QrCodeScanned += QrCodeManagerScript_QrCodeScanned;
		qrCodeManagerScript.StartScan();
	}

	void OnDisable()
	{
		qrCodeManagerScript.StopScan();
		qrCodeManagerScript.QrCodeScanned -= QrCodeManagerScript_QrCodeScanned;
	}

	private void QrCodeManagerScript_QrCodeScanned(string text)
	{
		try
		{
			if (!text.Contains(QR_CODE_KEY))
			{
				NotifyWrongQrCode();
				return;
			}

			gameObject.SetActive(false);
			string[] ipPort = text.Split('=')[1].Split(':');
			webRtcSignaler.signalingServerIp = ipPort[0];
			webRtcSignaler.signalingServerPort = ipPort[1];
			webRtcSignaler.Connect();
			findDeviceCanvas.SetActive(true);
			solarPipelineObject.isScanning = true;
		}
        catch
        {
        }
	}

	private void NotifyWrongQrCode()
	{
		if (wrongCodeCoroutine != null)
		{
			StopCoroutine(wrongCodeCoroutine);
		}

		wrongCodeCoroutine = StartCoroutine(WrongQrCodeCoroutine());
	}

	private IEnumerator WrongQrCodeCoroutine()
	{
		textQrHint.text = WRONG_QR_HINT;
		yield return new WaitForSecondsRealtime(1);
		textQrHint.text = QR_SCAN_HINT;
	}
}