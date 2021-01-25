package fr.pchab.androidrtc;

import android.graphics.Bitmap;
import android.graphics.Color;
import android.util.Log;

import androidx.annotation.Nullable;

import com.google.zxing.BarcodeFormat;
import com.google.zxing.WriterException;
import com.google.zxing.common.BitMatrix;
import com.google.zxing.qrcode.QRCodeWriter;

import java.net.Inet4Address;
import java.net.InterfaceAddress;
import java.net.NetworkInterface;
import java.util.Enumeration;

public final class Utils {

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
}
