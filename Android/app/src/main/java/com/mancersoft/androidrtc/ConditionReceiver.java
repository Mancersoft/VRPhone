package com.mancersoft.androidrtc;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

import org.webrtc.DataChannel;

import java.nio.ByteBuffer;

public class ConditionReceiver extends BroadcastReceiver {

    public static final String ACTION = "com.mancersoft.FITTS_LAW";

    @Override
    public void onReceive(Context context, Intent intent) {
        Log.d(SignalingServer.TAG, "Broadcast received!");
        if (intent.getAction().equals(ACTION)) {
            int condition = intent.getIntExtra(Utils.CONDITION_TYPE, -1);
            Log.d(SignalingServer.TAG, "Experiment condition received: " + condition);
            Utils.sendObjViaDataChannel(new Utils.ConditionMessage(condition));
            Utils.sendObjViaDataChannel(Utils.deviceParamsMessage);
        }
    }
}
