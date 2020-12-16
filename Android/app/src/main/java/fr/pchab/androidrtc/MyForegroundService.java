package fr.pchab.androidrtc;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.IBinder;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.annotation.RequiresApi;

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

public class MyForegroundService extends Service {

    private SignalingServer signalingServer;
    private final ExecutorService executor = Executors.newSingleThreadExecutor();
    private Future<?> serviceThread = null;

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        if (serviceThread == null || serviceThread.isDone()) {
            serviceThread = executor.submit(new Runnable() {
                @Override
                public void run() {
                    try {
                        if (signalingServer == null) {
                            signalingServer = new SignalingServer();
                        }

                        signalingServer.start();
                    } catch (Exception e) {
                        Log.e(SignalingServer.TAG, "error", e);
                    }
                }
            });
        }

        startForeground();
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        if (signalingServer != null) {
            signalingServer.stop();
        }
        if (serviceThread != null) {
            serviceThread.cancel(true);
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
        startForeground(101, notification);
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
