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
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Looper;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;

public class MyForegroundService extends Service {

    public static final String SERVICE_STARTED = "SERVICE_STARTED";
    public static final String SERVICE_START_FAILED = "SERVICE_START_FAILED";

    private SignalingServer signalingServer;

    private ConditionReceiver conditionReceiver;

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
                    signalingServer = new SignalingServer();
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
            signalingServer = new SignalingServer();
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

}
