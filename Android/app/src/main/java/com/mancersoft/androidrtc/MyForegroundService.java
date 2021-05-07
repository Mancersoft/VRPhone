package com.mancersoft.androidrtc;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.ServiceInfo;
import android.graphics.Color;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;

import org.webrtc.DataChannel;

import java.nio.ByteBuffer;

public class MyForegroundService extends Service implements SensorEventListener {

    public static final String SERVICE_STARTED = "SERVICE_STARTED";
    public static final String SERVICE_START_FAILED = "SERVICE_START_FAILED";

    public static final int MIN_PORT = 9092;
    public static final int MAX_PORT = 9192;

    private SignalingServer signalingServer;

    private SensorManager sensorManager;

    private ConditionReceiver conditionReceiver;

    private long lastTimestamp = 0;

    private int port = MIN_PORT;

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        super.onCreate();

        if (conditionReceiver != null) {
            unregisterReceiver(conditionReceiver);
        }

        conditionReceiver = new ConditionReceiver();
        registerReceiver(conditionReceiver, new IntentFilter(ConditionReceiver.ACTION));

        sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        Sensor sensor = sensorManager.getDefaultSensor(Sensor.TYPE_ROTATION_VECTOR);
        sensorManager.registerListener(this, sensor, SensorManager.SENSOR_DELAY_FASTEST);

        startForeground();
    }

    private final SignalingServer.OnServerStarted serverStartedListener = new SignalingServer.OnServerStarted() {
        @Override
        public void onStarted() {
            sendBroadcast(new Intent(SERVICE_STARTED));
        }
    };

    private final  SignalingServer.OnServerFailedToStart serverFailedListener = new SignalingServer.OnServerFailedToStart() {
        @Override
        public void onFailed() {
            new Handler(Looper.getMainLooper()).postDelayed(new Runnable() {

                @Override
                public void run() {
                    sendBroadcast(new Intent(SERVICE_START_FAILED));

                    port = Utils.managePort(MyForegroundService.this);

                    signalingServer = new SignalingServer(port);
                    signalingServer.setOnServerStarted(serverStartedListener);
                    signalingServer.setOnServerFailedToStart(serverFailedListener);
                    signalingServer.start();
                }

            }, 1000);
        }
    };

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        if (signalingServer == null) {
            port = intent.getIntExtra(Utils.PORT_KEY, MIN_PORT);
            signalingServer = new SignalingServer(port);
            signalingServer.setOnServerStarted(serverStartedListener);
            signalingServer.setOnServerFailedToStart(serverFailedListener);
            signalingServer.start();
        } else {
            if (signalingServer.isStarted()) {
                Log.d(SignalingServer.TAG, "Server has already started");
                serverStartedListener.onStarted();
            } else {
                Log.d(SignalingServer.TAG, "Server failed to start");
                serverFailedListener.onFailed();
            }
        }

        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        if (signalingServer != null) {
            signalingServer.stop();
        }

        if (conditionReceiver != null) {
            unregisterReceiver(conditionReceiver);
        }

        sensorManager.unregisterListener(this);
        super.onDestroy();
    }

    private void startForeground() {
        Notification.Builder notificationBuilder;
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            notificationBuilder =
                    new Notification.Builder(this,
                            createNotificationChannel("screen_capture", "Screen Capture"));
        } else {
            notificationBuilder = new Notification.Builder(this);
        }

        Notification notification = notificationBuilder.setOngoing(true)
                .setSmallIcon(R.mipmap.ic_launcher)
                .setPriority(Notification.PRIORITY_MAX)
                .build();
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            startForeground(101, notification, ServiceInfo.FOREGROUND_SERVICE_TYPE_MEDIA_PROJECTION);
        } else {
            startForeground(101, notification);
        }
    }


    @SuppressWarnings("SameParameterValue")
    @RequiresApi(Build.VERSION_CODES.O)
    private String createNotificationChannel(String channelId, String channelName) {
        NotificationChannel chan = new NotificationChannel(channelId,
                channelName, NotificationManager.IMPORTANCE_NONE);
        chan.setLightColor(Color.BLUE);
        chan.setLockscreenVisibility(Notification.VISIBILITY_PRIVATE);
        NotificationManager service = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        service.createNotificationChannel(chan);
        return channelId;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == Sensor.TYPE_ROTATION_VECTOR
                && RtcActivity.mWebRtcClient != null
                && RtcActivity.mWebRtcClient.peers.size() > 0
                && event.timestamp > lastTimestamp) {
            lastTimestamp = event.timestamp;
            float[] quaternion = Utils.getQuaternion(event.values);
            WebRtcClient.Peer peer = RtcActivity.mWebRtcClient.peers.entrySet().iterator().next().getValue();
            DataChannel dataChannel = peer.udpDataChannel;
            if (dataChannel != null && dataChannel.state() == DataChannel.State.OPEN) {
                ByteBuffer data = ByteBuffer.allocate(8 + 4 * quaternion.length);
                data.putLong(event.timestamp);
                for (float v : quaternion) {
                    data.putFloat(v);
                }
                byte[] dataArray = data.array();
                dataChannel.send(new DataChannel.Buffer(ByteBuffer.wrap(dataArray), true));
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
    }
}
