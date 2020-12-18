using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public delegate void QrCodeScanned(string text);

public class QRCodeManagerScript : MonoBehaviour
{
	public DeviceCameraScript deviceCameraScript;

	public bool isScanning;

	public event QrCodeScanned QrCodeScanned;

	private Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

	void Update()
	{
		if (deviceCameraScript.activeCameraTexture != null && isScanning)
		{
			try
			{
				IBarcodeReader barcodeReader = new BarcodeReader();
				var camTexture = deviceCameraScript.activeCameraTexture;
				var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
				if (result != null && QrCodeScanned != null)
				{
					QrCodeScanned(result.Text);
				}
			}
			catch
			{
			}
		}
	}

	public void StartScan()
	{
		isScanning = true;
	}

	public void StopScan()
	{
		isScanning = false;
	}

	private static Color32[] Encode(string textForEncoding, int width, int height)
	{
		var writer = new BarcodeWriter
		{
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions
			{
				Height = height,
				Width = width,
			},
		};
		return writer.Write(textForEncoding);
	}

	public Sprite GenerateQrCode(string text)
	{
		if (cache.ContainsKey(text))
		{
			return cache[text];
		}

		var encoded = new Texture2D(2048, 2048);
		var color32 = Encode(text, encoded.width, encoded.height);
		encoded.SetPixels32(color32);
		encoded.Apply();
		var sprite = Sprite.Create(encoded, new Rect(0, 0, encoded.width, encoded.height), new Vector2(0.5f, 0.5f), 100);
		cache.Clear();
		cache.Add(text, sprite);
		return sprite;
	}
}
