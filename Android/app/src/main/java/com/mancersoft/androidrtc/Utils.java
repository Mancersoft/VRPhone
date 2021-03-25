package com.mancersoft.androidrtc;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.Point;
import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Display;
import android.view.WindowManager;

import androidx.annotation.Nullable;

import com.google.gson.Gson;
import com.google.zxing.BarcodeFormat;
import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;

import org.webrtc.DataChannel;

import java.net.Inet4Address;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.Enumeration;

public final class Utils {

    public static void sendObjViaDataChannel(Object obj) {
        String objStr = new Gson().toJson(obj);
        byte[] objBytes = objStr.getBytes(StandardCharsets.UTF_8);
        WebRtcClient.Peer peer = RtcActivity.mWebRtcClient.peers.entrySet().iterator().next().getValue();
        DataChannel dataChannel = peer.tcpDataChannel;
        if (dataChannel != null && dataChannel.state() == DataChannel.State.OPEN) {
            dataChannel.send(new DataChannel.Buffer(ByteBuffer.wrap(objBytes), true));
        }
    }

    @Nullable
    public static String getDeviceIpAddress() {
        InterfaceAddress interfaceAddress = getInterfaceAddress();
        return interfaceAddress != null ? interfaceAddress.getAddress().getHostAddress() : null;
    }

    @Nullable
    public static Bitmap generateQrCode(String content, int size) {
        QRCodeWriter writer = new QRCodeWriter();
        try {
            BitMatrix bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, size, size);
            int width = bitMatrix.getWidth();
            int height = bitMatrix.getHeight();
            Bitmap bmp = Bitmap.createBitmap(width, height, Bitmap.Config.RGB_565);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    bmp.setPixel(x, y, bitMatrix.get(x, y) ? Color.BLACK : Color.WHITE);
                }
            }
            return bmp;
        } catch (WriterException e) {
            Log.e(SignalingServer.TAG, "generateQrCode error", e);
            return null;
        }
    }

    @Nullable
    private static InterfaceAddress getInterfaceAddress() {
        try {
            for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces();
                 en.hasMoreElements(); ) {
                NetworkInterface networkInterface = en.nextElement();
                for (InterfaceAddress addr : networkInterface.getInterfaceAddresses()) {
                    if (addr.getAddress() != null && addr.getAddress() instanceof Inet4Address
                            && !addr.getAddress().isLoopbackAddress()
                            && addr.getBroadcast() != null) {
                        return addr;
                    }
                }
            }
            for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces();
                 en.hasMoreElements(); ) {
                NetworkInterface networkInterface = en.nextElement();
                for (InterfaceAddress addr : networkInterface.getInterfaceAddresses()) {
                    if (addr.getAddress() != null && addr.getAddress() instanceof Inet4Address
                            && !addr.getAddress().isLoopbackAddress()) {
                        return addr;
                    }
                }
            }
            for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces();
                 en.hasMoreElements(); ) {
                NetworkInterface networkInterface = en.nextElement();
                for (InterfaceAddress addr : networkInterface.getInterfaceAddresses()) {
                    if (addr.getAddress() != null && addr.getAddress() instanceof Inet4Address) {
                        return addr;
                    }
                }
            }
        } catch (Exception ex) {
            Log.d("IP Address", "getLocalIpAddress Error: " + ex.getMessage());
        }
        return null;
    }

    public static DeviceParamsMessage deviceParamsMessage;

    public static void setRealDeviceSize(Activity activity) {
        WindowManager windowManager = activity.getWindowManager();
        Display display = windowManager.getDefaultDisplay();
        DisplayMetrics displayMetrics = new DisplayMetrics();
        display.getMetrics(displayMetrics);

        Point size = new Point(displayMetrics.widthPixels, displayMetrics.heightPixels);
        DisplayMetrics dm = new DisplayMetrics();
        activity.getWindowManager().getDefaultDisplay().getMetrics(dm);
        try {
            Point realSize = new Point();
            Display.class.getMethod("getRealSize", Point.class).invoke(display, realSize);
            size = realSize;
        } catch (Exception ignored) {
        }

        deviceParamsMessage = new DeviceParamsMessage();
        deviceParamsMessage.width = size.x / dm.xdpi;
        deviceParamsMessage.height = size.y / dm.ydpi;
        deviceParamsMessage.ratio = size.y / (float) size.x;
    }

    public static float[] getQuaternion(float[] rotationVector) {
        float q0;
        float q1 = rotationVector[0];
        float q2 = rotationVector[1];
        float q3 = rotationVector[2];
        if (rotationVector.length >= 4) {
            q0 = rotationVector[3];
        } else {
            q0 = 1 - q1 * q1 - q2 * q2 - q3 * q3;
            q0 = (q0 > 0) ? (float) Math.sqrt(q0) : 0;
        }

        return new float[] { q0, q1, q2, q3 };
    }

    public static final String CONDITION_TYPE = "CONDITION";
    public static final String DEVICE_PARAMS_TYPE = "DEVICE_PARAMS";

    public static class ConditionMessage {
        public String type;
        public int value;
        public ConditionMessage(int value) {
            type = CONDITION_TYPE;
            this.value = value;
        }
    }

    public static class DeviceParamsMessage {
        public String type;
        public float width;
        public float height;
        public float ratio;
        public DeviceParamsMessage()
        {
        }
        public DeviceParamsMessage(float width, float height, float ratio) {
            type = DEVICE_PARAMS_TYPE;
            this.width = width;
            this.height = height;
            this.ratio = ratio;
        }
    }
}
