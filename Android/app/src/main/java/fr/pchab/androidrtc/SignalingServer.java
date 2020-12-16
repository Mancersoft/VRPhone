package fr.pchab.androidrtc;

import android.util.Log;

import com.corundumstudio.socketio.AckMode;
import com.corundumstudio.socketio.AckRequest;
import com.corundumstudio.socketio.AuthorizationListener;
import com.corundumstudio.socketio.Configuration;
import com.corundumstudio.socketio.HandshakeData;
import com.corundumstudio.socketio.SocketIOClient;
import com.corundumstudio.socketio.SocketIOServer;
import com.corundumstudio.socketio.Transport;
import com.corundumstudio.socketio.listener.ConnectListener;
import com.corundumstudio.socketio.listener.DataListener;
import com.corundumstudio.socketio.listener.DisconnectListener;
import com.corundumstudio.socketio.listener.ExceptionListener;
import com.corundumstudio.socketio.listener.PingListener;

import java.util.List;
import java.util.UUID;

import io.netty.channel.ChannelHandlerContext;

public class SignalingServer {

    public static final String TAG = "SIGNALING_SERVER";

    private final SocketIOServer server;

    private static class ConnectData {
        private String id;

        public ConnectData() {
        }

        public ConnectData(String id) {
            this.id = id;
        }

        public String getId() {
            return id;
        }
        public void setId(String id) {
            this.id = id;
        }
    }

    private static class ReadyToStreamData {
        private String name;
        public String getName() {
            return name;
        }
        public void setName(String name) {
            this.name = name;
        }
    }

    private static class MessageInData {
        private String to;
        private String type;
        private Object payload;

        public String getTo() {
            return to;
        }
        public void setTo(String to) {
            this.to = to;
        }
        public String getType() {
            return type;
        }
        public void setType(String type) {
            this.type = type;
        }
        public Object getPayload() {
            return payload;
        }
        public void setPayload(Object payload) {
            this.payload = payload;
        }
    }

    private static class MessageOutData {
        private String from;
        private String type;
        private Object payload;

        public MessageOutData() {
        }
        public MessageOutData(String from, String type, Object payload) {
            this.from = from;
            this.type = type;
            this.payload = payload;
        }
        public String getFrom() {
            return from;
        }
        public void setFrom(String from) {
            this.from = from;
        }
        public String getType() {
            return type;
        }
        public void setType(String type) {
            this.type = type;
        }
        public Object getPayload() {
            return payload;
        }
        public void setPayload(Object payload) {
            this.payload = payload;
        }
    }

    public SignalingServer() {
        Configuration config = new Configuration();
        config.setHostname("0.0.0.0");
        config.setPort(9092);
        config.setTransports(Transport.WEBSOCKET);
        config.setAckMode(AckMode.AUTO);
        config.setAllowCustomRequests(true);
        config.setAuthorizationListener(new AuthorizationListener() {
            @Override
            public boolean isAuthorized(HandshakeData data) {
                return data.getSingleUrlParam("transport").toLowerCase().equals("websocket");
            }
        });
        config.setExceptionListener(new ExceptionListener() {
            @Override
            public void onEventException(Exception e, List<Object> args, SocketIOClient client) {
                Log.e(TAG, "onEvent exception", e);
            }

            @Override
            public void onDisconnectException(Exception e, SocketIOClient client) {
                Log.e(TAG, "onDisconnectException exception", e);
            }

            @Override
            public void onConnectException(Exception e, SocketIOClient client) {
                Log.e(TAG, "onConnectException exception", e);
            }

            @Override
            public void onPingException(Exception e, SocketIOClient client) {
                Log.e(TAG, "onPingException exception", e);
            }

            @Override
            public boolean exceptionCaught(ChannelHandlerContext ctx, Throwable e) {
                Log.e(TAG, "exceptionCaught", e);
                return true;
            }
        });
        server = new SocketIOServer(config);
        server.addConnectListener(new ConnectListener() {
            @Override
            public void onConnect(SocketIOClient client) {
                String id = client.getSessionId().toString();
                Log.d(TAG, "client " + id + " joined");
                client.sendEvent("id", new ConnectData(id));
            }
        });
        server.addEventListener("message", MessageInData.class, new DataListener<MessageInData>() {
            @Override
            public void onData(SocketIOClient client, MessageInData data, AckRequest ackSender) {
                Log.d(TAG, "on message");

                SocketIOClient otherClient = server.getClient(UUID.fromString(data.to));
                if (otherClient == null) {
                    Log.d(TAG, "no such client found");
                    return;
                }

                otherClient.sendEvent("message",
                        new MessageOutData(client.getSessionId().toString(), data.type, data.payload));
            }
        });
        server.addEventListener("readyToStream", ReadyToStreamData.class, new DataListener<ReadyToStreamData>() {
            @Override
            public void onData(SocketIOClient client, ReadyToStreamData data, AckRequest ackSender) {
                Log.d(TAG, "on readyToStream " + data.getName());
                ConnectData clientIdObj = new ConnectData(client.getSessionId().toString());
                for (SocketIOClient other: server.getAllClients()) {
                    if (!other.getSessionId().equals(client.getSessionId())) {
                        other.sendEvent("remoteId", clientIdObj);
                        ConnectData otherIdObj = new ConnectData(other.getSessionId().toString());
                        client.sendEvent("remoteId", otherIdObj);
                    }
                }
            }
        });
        server.addDisconnectListener(new DisconnectListener() {
            @Override
            public void onDisconnect(SocketIOClient client) {
                Log.d(TAG, "client " + client.getSessionId() + " left");
            }
        });
    }

    public void start() {
        server.start();
    }

    public void stop() {
        server.stop();
    }
}
