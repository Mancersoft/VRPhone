
package com.mancersoft.androidrtc;

import android.annotation.TargetApi;
import android.app.Activity;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.media.projection.MediaProjection;
import android.media.projection.MediaProjectionManager;
import android.os.Build;
import android.os.Bundle;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import org.webrtc.ScreenCapturerAndroid;
import org.webrtc.VideoCapturer;

import static android.content.ContentValues.TAG;

public class RtcActivity extends Activity implements WebRtcClient.RtcListener {

    private static final int CAPTURE_PERMISSION_REQUEST_CODE = 1;
    private static Intent mMediaProjectionPermissionResultData;
    private static int mMediaProjectionPermissionResultCode;

    public static final String STREAM_NAME_PREFIX = "android_device_stream";
    private static final String[] MANDATORY_PERMISSIONS = {
            "android.permission.MODIFY_AUDIO_SETTINGS",
            "android.permission.RECORD_AUDIO",
            "android.permission.INTERNET"};

    public static int sDeviceWidth;
    public static int sDeviceHeight;
    public static final int SCREEN_RESOLUTION_SCALE = 2;

    private static final String QR_CODE_KEY = "SCREEN_CAPTURE_IP=";

    public static WebRtcClient mWebRtcClient;

    private TextView textViewIp;
    private ImageView imageViewQrCode;
    private int qrCodeSize;

    private final BroadcastReceiver serviceMessagesReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            switch (intent.getAction()) {
                case MyForegroundService.SERVICE_STARTED:
                    init();
                    break;
                case MyForegroundService.SERVICE_START_FAILED:
                    stopService(new Intent(RtcActivity.this, MyForegroundService.class));
                    Toast.makeText(RtcActivity.this,
                            "Server failed to start! Please restart the app and try again",
                            Toast.LENGTH_SHORT).show();
                    break;
            }
        }
    };

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        //noinspection deprecation
        getWindow().addFlags(
                WindowManager.LayoutParams.FLAG_FULLSCREEN
                        | WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON
                        | WindowManager.LayoutParams.FLAG_DISMISS_KEYGUARD
                        | WindowManager.LayoutParams.FLAG_SHOW_WHEN_LOCKED
                        | WindowManager.LayoutParams.FLAG_TURN_SCREEN_ON);
        setContentView(R.layout.activity_rtc);
        DisplayMetrics metrics = new DisplayMetrics();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            getDisplay().getRealMetrics(metrics);
        }
        sDeviceWidth = metrics.widthPixels;
        sDeviceHeight = metrics.heightPixels;
        qrCodeSize = Math.min(sDeviceWidth, sDeviceHeight);

        Utils.setRealDeviceSize(this);
        Log.d(WebRtcClient.TAG, "width: " + Utils.deviceParamsMessage.width + "; height: " +
                Utils.deviceParamsMessage.height + "; ratio: " + Utils.deviceParamsMessage.ratio);

        textViewIp = findViewById(R.id.textViewIp);
        imageViewQrCode = findViewById(R.id.imageViewQrCode);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            requestPermissions(MANDATORY_PERMISSIONS, 0);
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            startScreenCapture();
        }
        else {
            startService();
        }
    }

    @Override
    protected void onResume() {
        super.onResume();
        IntentFilter filter = new IntentFilter();
        filter.addAction(MyForegroundService.SERVICE_STARTED);
        filter.addAction(MyForegroundService.SERVICE_START_FAILED);
        registerReceiver(serviceMessagesReceiver, filter);
    }

    @Override
    protected void onPause() {
        super.onPause();
        unregisterReceiver(serviceMessagesReceiver);
    }

    @TargetApi(21)
    private void startScreenCapture() {
        MediaProjectionManager mediaProjectionManager =
                (MediaProjectionManager) getApplication().getSystemService(
                        Context.MEDIA_PROJECTION_SERVICE);
        startActivityForResult(
                mediaProjectionManager.createScreenCaptureIntent(), CAPTURE_PERMISSION_REQUEST_CODE);
    }

    @TargetApi(21)
    private VideoCapturer createScreenCapturer() {
        if (mMediaProjectionPermissionResultCode != Activity.RESULT_OK) {
            report("User didn't give permission to capture the screen.");
            return null;
        }

        return new ScreenCapturerAndroid(
                mMediaProjectionPermissionResultData, new MediaProjection.Callback() {
            @Override
            public void onStop() {
                report("User revoked permission to capture the screen.");
            }
        });
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode != CAPTURE_PERMISSION_REQUEST_CODE)
            return;
        mMediaProjectionPermissionResultCode = resultCode;
        mMediaProjectionPermissionResultData = data;
        startService();
        //init();
    }

    private void startService() {
        if (mMediaProjectionPermissionResultCode != Activity.RESULT_OK) {
            return;
        }

        if (Build.VERSION.SDK_INT >= 26) {
            startForegroundService(new Intent(this, MyForegroundService.class));
        } else {
            startService(new Intent(this, MyForegroundService.class));
        }
    }

    private void init() {
        PeerConnectionClient.PeerConnectionParameters peerConnectionParameters =
                new PeerConnectionClient.PeerConnectionParameters(true, false,
                        true, sDeviceWidth / SCREEN_RESOLUTION_SCALE, sDeviceHeight / SCREEN_RESOLUTION_SCALE, 0,
                        0, "VP8",
                        false,
                        true,
                        0,
                        "OPUS", false, false, false, false, false, false, false, false, null);
        mWebRtcClient = new WebRtcClient(getApplicationContext(), this, createScreenCapturer(), peerConnectionParameters);

        String ipAddress = Utils.getDeviceIpAddress();
        textViewIp.setText(ipAddress);
        imageViewQrCode.setImageBitmap(Utils.generateQrCode(QR_CODE_KEY + ipAddress, qrCodeSize));

        mWebRtcClient.connect(getString(R.string.default_ip_port));
    }

    public void report(String info) {
        Log.e(TAG, info);
    }

    @Override
    public void onReady(String callId) {
        mWebRtcClient.start(STREAM_NAME_PREFIX);
    }

    @Override
    public void onStatusChanged(final String newStatus) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(getApplicationContext(), newStatus, Toast.LENGTH_SHORT).show();
            }
        });
    }

    public void onStopServiceClick(View view) {
        stopService(new Intent(this, MyForegroundService.class));
        Toast.makeText(RtcActivity.this,  R.string.service_stopped, Toast.LENGTH_SHORT).show();
    }
}
